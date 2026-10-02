using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms
{
    public partial class FrmDownloadData : Window
    {
        // ─── Lookup dictionaries ─────────────────────────────────────────────
        private Dictionary<string, string> _dateTypeMap;
        private Dictionary<string, string> _interviewTypeMap;
        private Dictionary<string, string> _projectCodeMap;
        private Dictionary<string, string> _projectDbMap;
        private Dictionary<string, string> _projectStartDateMap;

        // ─── Run state ───────────────────────────────────────────────────────
        private CancellationTokenSource _cts;
        private bool _isRunning;

        // ─── HTTP constants ──────────────────────────────────────────────────
        private const int HttpTimeoutMs = 300_000;   // 5 minutes per request
        private const int RetryAttempts = 3;
        private const int RetryDelayMs  = 3_000;     // 3 seconds between retries

        // Page sizes used when the server supports id-based paging (new deskapi).
        private const int RespPageSize = 5_000;
        private const int AnsPageSize  = 20_000;
        private const int OePageSize   = 10_000;

        // The old answerbyproject.php always returns pages of this size.
        private const int LegacyAnsPageSize = 10_000;

        // Response header sent by the new deskapi endpoints.
        private const string PagingHeader = "X-Desk-Paging";

        // Excel limits / write chunking.
        private const int MaxCellText        = 32_766;   // Excel's cell limit is 32,767 characters
        private const int CellsPerWrite      = 200_000;  // cells sent to Excel in one Value2 call
        private const int AutoFitSampleRows  = 2_000;    // column widths are measured on the first rows

        public FrmDownloadData()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                if (!_isRunning) return;
                e.Cancel = true;
                MessageBox.Show("A download is in progress. Press Cancel first, then close the window.",
                    Title, MessageBoxButton.OK, MessageBoxImage.Information);
            };
        }

        // ─── Initialisation ──────────────────────────────────────────────────

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol  = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            // .NET allows only 2 connections per host by default; the three phases download in parallel.
            if (ServicePointManager.DefaultConnectionLimit < 8)
                ServicePointManager.DefaultConnectionLimit = 8;

            PopulateCombos();

            dtpDateFrom.SelectedDate    = DateTime.Today;
            dtpDateTo.SelectedDate      = DateTime.Today;
            comInterviewType.Text       = "Final Interviews";
            comConsiderDate.Text        = "Sync Date";
            comFileType.Text            = "Excel";
            chkDownloadScript.IsChecked = true;

            await LoadProjectsAsync();
        }

        private void PopulateCombos()
        {
            _dateTypeMap = new Dictionary<string, string>
            {
                { "Sync Date",      "2" },
                { "Interview Date", "1" }
            };
            comConsiderDate.ItemsSource = _dateTypeMap.Keys.ToList();

            _interviewTypeMap = new Dictionary<string, string>
            {
                { "Final Interviews",             "1" },
                { "Test Interviews",              "2" },
                { "Reject Interviews",            "3" },
                { "Terminate Interviews",         "4" },
                { "Incomplete Interviews",        "5" },
                { "Final & Terminate Interviews", "6" },
                { "Deleted Interviews",           "7" }
            };
            comInterviewType.ItemsSource = _interviewTypeMap.Keys.ToList();

            comFileType.Items.Clear();
            comFileType.Items.Add("Excel");
            comFileType.Items.Add("CSV");
        }

        private async Task LoadProjectsAsync()
        {
            Log("Connecting to server...");
            btnExecute.IsEnabled = false;
            try
            {
                _projectCodeMap      = new Dictionary<string, string>();
                _projectDbMap        = new Dictionary<string, string>();
                _projectStartDateMap = new Dictionary<string, string>();

                List<ProjectInfo> projects = await Task.Run(
                    () => new DownloadClass().getProjectInfoFromServer());

                comProjectName.Items.Clear();
                if (projects != null && projects.Count > 0)
                {
                    foreach (var p in projects)
                    {
                        comProjectName.Items.Add(p.ProjectName);
                        _projectCodeMap[p.ProjectName]      = p.ProjectCode;
                        _projectDbMap[p.ProjectName]        = p.DatabaseName;
                        _projectStartDateMap[p.ProjectName] = ConvertDateFormat(p.StartDate);
                    }
                    Log(projects.Count + " project(s) loaded.");
                }
                else
                {
                    Log("No projects returned. Check server connection.");
                }
            }
            catch (Exception ex)
            {
                Log("Project load failed: " + GetFullMessage(ex));
            }
            finally
            {
                btnExecute.IsEnabled = true;
            }
        }

        // ─── HTTP helpers ────────────────────────────────────────────────────

        // WebClient with a per-request timeout and gzip/deflate support.
        private sealed class TimeoutWebClient : WebClient
        {
            private readonly int _timeoutMs;
            public TimeoutWebClient(int timeoutMs) { _timeoutMs = timeoutMs; }
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                if (req != null) req.Timeout = _timeoutMs;
                var http = req as HttpWebRequest;
                if (http != null)
                {
                    http.ReadWriteTimeout = _timeoutMs;
                    http.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }
                return req;
            }
        }

        private sealed class PostResult
        {
            public string Body;
            public bool KeysetPaging;   // server understood lastId/pageSize
        }

        private static async Task<PostResult> PostAsync(string url, string body, CancellationToken ct)
        {
            using (var wc = new TimeoutWebClient(HttpTimeoutMs))
            using (ct.Register(() => wc.CancelAsync()))
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                try
                {
                    string text = await wc.UploadStringTaskAsync(url, "POST", body).ConfigureAwait(false);
                    string paging = wc.ResponseHeaders == null ? null : wc.ResponseHeaders[PagingHeader];
                    return new PostResult { Body = text, KeysetPaging = paging == "keyset" };
                }
                catch (WebException) when (ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ct);
                }
                catch (WebException ex) when (ex.Response is HttpWebResponse)
                {
                    // Surface what the server said (e.g. "Error: Query failed: ...") instead of just "(500)".
                    var resp = (HttpWebResponse)ex.Response;
                    string detail = ReadErrorBody(resp);
                    throw new WebException(
                        $"{url.Substring(url.LastIndexOf('/') + 1)} returned HTTP {(int)resp.StatusCode}" +
                        (detail.Length > 0 ? ": " + detail : " (empty response — check the server's PHP error log)"),
                        ex, ex.Status, ex.Response);
                }
            }
        }

        private static string ReadErrorBody(HttpWebResponse resp)
        {
            try
            {
                using (var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                {
                    string text = reader.ReadToEnd().Trim();
                    text = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", " ");   // strip HTML
                    text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
                    return text.Length > 400 ? text.Substring(0, 400) + "..." : text;
                }
            }
            catch
            {
                return "";
            }
        }

        // Retries PostAsync up to RetryAttempts times on transient failure.
        private async Task<PostResult> PostWithRetryAsync(string url, string body, CancellationToken ct)
        {
            Exception lastEx = null;
            for (int attempt = 1; attempt <= RetryAttempts; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    return await PostAsync(url, body, ct).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    throw; // user-initiated cancel — do not retry
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    if (attempt < RetryAttempts)
                    {
                        Log($"Attempt {attempt}/{RetryAttempts} failed: {GetFullMessage(ex)} — retrying in {RetryDelayMs / 1000}s...");
                        await Task.Delay(RetryDelayMs, ct).ConfigureAwait(false);
                    }
                }
            }
            throw new Exception(
                $"Request failed after {RetryAttempts} attempts. Last error: {GetFullMessage(lastEx)}", lastEx);
        }

        // Builds a URL-encoded POST body from the common download parameters.
        private static string BuildRequestBody(string startDate, string endDate,
            string dateType, string projectCode, string interviewType)
        {
            return "startDate="      + Uri.EscapeDataString(startDate)
                 + "&endDate="       + Uri.EscapeDataString(endDate)
                 + "&dateType="      + Uri.EscapeDataString(dateType)
                 + "&projectCode="   + Uri.EscapeDataString(projectCode)
                 + "&interviewType=" + Uri.EscapeDataString(interviewType);
        }

        private static DataTable ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new DataTable();
            string trimmed = json.TrimStart();
            if (!trimmed.StartsWith("["))
                throw new Exception("Unexpected server response: " +
                    (trimmed.Length > 300 ? trimmed.Substring(0, 300) + "..." : trimmed));
            return JsonConvert.DeserializeObject<DataTable>(json) ?? new DataTable();
        }

        // Walks the full InnerException chain and returns a single readable message.
        private static string GetFullMessage(Exception ex)
        {
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append(ex.Message);
                ex = ex.InnerException;
                if (ex != null) sb.Append(" → ");
            }
            return sb.ToString();
        }

        // ─── Download phases ─────────────────────────────────────────────────

        private async Task DownloadScriptAsync(string dbName, string databasePath,
            CancellationToken ct)
        {
            Log("Downloading project script from server...");
            string source = StaticClass.SERVER_URL + "/scripts/" + dbName;
            string tempFile = databasePath + ".download";
            if (File.Exists(tempFile)) File.Delete(tempFile);
            using (var wc = new TimeoutWebClient(HttpTimeoutMs))
            using (ct.Register(() => wc.CancelAsync()))
            {
                try
                {
                    await wc.DownloadFileTaskAsync(source, tempFile);
                }
                catch (WebException) when (ct.IsCancellationRequested)
                {
                    throw new OperationCanceledException(ct);
                }
            }
            // Replace the cached script only once the new one has fully arrived.
            if (File.Exists(databasePath)) File.Delete(databasePath);
            File.Move(tempFile, databasePath);
            Log("Script downloaded.");
        }

        /// <summary>Live row counts of the three parallel phases, shown in the status line.</summary>
        private sealed class DownloadCounters
        {
            public int Respondents, Answers, OpenEnded;
        }

        /// <summary>
        /// Downloads one endpoint page by page into a single DataTable.
        ///  - New server (X-Desk-Paging: keyset): pages by "id &gt; lastId", stops on a short page.
        ///  - Old server: answers page by myOffset (10,000 rows); respondents / open-ended come in one response.
        /// Rows are de-duplicated by id, and a page that brings nothing new ends the loop,
        /// so an old server can never make this loop forever.
        /// </summary>
        private async Task<DataTable> DownloadTableAsync(string endpoint, string label, string baseBody,
            int pageSize, bool legacyPaged, Action<int> onCount, CancellationToken ct)
        {
            string url = StaticClass.SERVER_URL + "/deskapi/" + endpoint;
            DataTable all = null;
            var seenIds = new HashSet<string>();
            long lastId = 0;
            long legacyOffset = 0;
            int page = 1;

            while (true)
            {
                ct.ThrowIfCancellationRequested();
                string body = baseBody
                            + "&lastId="   + lastId
                            + "&pageSize=" + pageSize
                            + "&myOffset=" + legacyOffset;

                PostResult res = await PostWithRetryAsync(url, body, ct).ConfigureAwait(false);

                int received = 0, added = 0;
                long maxId = lastId;
                await Task.Run(() =>
                {
                    DataTable dt = ParseJson(res.Body);
                    received = dt.Rows.Count;
                    DataColumn idCol = dt.Columns["id"];
                    if (all == null) all = dt.Clone();
                    all.BeginLoadData();
                    foreach (DataRow row in dt.Rows)
                    {
                        string id = idCol == null || row.IsNull(idCol) ? null : Convert.ToString(row[idCol]);
                        if (id != null && !seenIds.Add(id)) continue;   // duplicate from the server
                        all.ImportRow(row);
                        added++;
                        long n;
                        if (id != null && long.TryParse(id, out n) && n > maxId) maxId = n;
                    }
                    all.EndLoadData();
                }, ct).ConfigureAwait(false);

                onCount(all.Rows.Count);
                Log($"{label} page {page}: +{added} ({all.Rows.Count} so far)");

                if (received == 0 || added == 0) break;
                if (res.KeysetPaging)
                {
                    if (received < pageSize) break;
                    lastId = maxId;
                }
                else
                {
                    if (!legacyPaged || received < LegacyAnsPageSize) break;
                    legacyOffset += received;
                }
                page++;
            }

            Log($"{label} complete: {all?.Rows.Count ?? 0} row(s).");
            return all ?? new DataTable();
        }

        // ─── Execute handler ─────────────────────────────────────────────────

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            string projectName = comProjectName.Text;

            if (!_projectDbMap.TryGetValue(projectName, out string dbName) ||
                !_projectCodeMap.TryGetValue(projectName, out string projectCode))
            {
                MessageBox.Show("Project data not loaded. Please wait for projects to finish loading.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string format   = comFileType.Text;
            string savePath = NormaliseSavePath(txtSaveLocation.Text.Trim(), format);
            string outputProblem = CheckOutputFiles(savePath, format);
            if (outputProblem != null)
            {
                MessageBox.Show(outputProblem, "Save Location", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            txtSaveLocation.Text = savePath;

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            SetRunning(true);
            progressBar1.Value = 0;
            txtLog.Clear();

            string tempPath      = @"C:\Temp";
            Directory.CreateDirectory(tempPath);
            string databasePath  = Path.Combine(tempPath, dbName);
            string startDate     = dtpDateFrom.SelectedDate.Value.ToString("yyyy-MM-dd");
            string endDate       = dtpDateTo.SelectedDate.Value.ToString("yyyy-MM-dd");
            string dateType      = _dateTypeMap[comConsiderDate.Text];
            string interviewType = _interviewTypeMap[comInterviewType.Text];
            var started          = DateTime.Now;

            try
            {
                // Step 1 — Download script
                if (!File.Exists(databasePath) || chkDownloadScript.IsChecked == true)
                    await DownloadScriptAsync(dbName, databasePath, ct);
                else
                    Log("Using cached project script.");
                progressBar1.Value = 10;

                // Step 2 — Download the three tables in parallel
                string baseBody = BuildRequestBody(startDate, endDate, dateType, projectCode, interviewType);
                var counters = new DownloadCounters();
                Action updateStatus = () => SetStatus(
                    $"Downloading — respondents: {counters.Respondents:N0}, answers: {counters.Answers:N0}, open-ended: {counters.OpenEnded:N0}");
                updateStatus();

                Task<DataTable> tResp = DownloadTableAsync("respondentbyproject.php", "Respondents", baseBody,
                    RespPageSize, false, n => { counters.Respondents = n; updateStatus(); }, ct);
                Task<DataTable> tAns = DownloadTableAsync("answerbyproject.php", "Answers", baseBody,
                    AnsPageSize, true, n => { counters.Answers = n; updateStatus(); }, ct);
                Task<DataTable> tOe = DownloadTableAsync("openendedbyproject.php", "Open-ended", baseBody,
                    OePageSize, false, n => { counters.OpenEnded = n; updateStatus(); }, ct);

                try
                {
                    await Task.WhenAll(tResp, tAns, tOe);
                }
                catch
                {
                    _cts.Cancel();   // one phase failed: stop the others straight away
                    throw;
                }
                progressBar1.Value = 60;

                DataTable respondents = tResp.Result, answers = tAns.Result, openEnded = tOe.Result;
                if (respondents.Rows.Count == 0)
                {
                    Log("No interviews found for the selected filters. Nothing was exported.");
                    SetStatus("No data.");
                    MessageBox.Show("No interviews were found for the selected project, dates and interview type.",
                        "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Step 3 — Build the report and export it (background STA thread: Excel COM is fine there)
                Log("Building report...");
                var export = new ExportJob
                {
                    Format = format, SavePath = savePath, DatabasePath = databasePath,
                    Respondents = respondents, Answers = answers, OpenEnded = openEnded,
                    Progress = ReportProgress, Log = Log, Token = ct
                };
                await RunOnStaThread(export.Run);

                progressBar1.Value = 100;
                SetStatus("Complete.");
                Log($"Complete in {(DateTime.Now - started).TotalSeconds:0.0}s — {export.RowCount:N0} interview(s) exported.");

                string done = format == "Excel"
                    ? "Data download complete.\n\n" + savePath
                    : "Two files were created:\n\n• Data (CSV): " + export.CsvPath + "\n• Open-ended: " + export.OeXlsxPath;
                MessageBox.Show(done, "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                Log("Cancelled by user.");
                progressBar1.Value = 0;
                SetStatus("Cancelled.");
            }
            catch (Exception ex)
            {
                string fullMsg = GetFullMessage(ex);
                Log("Error: " + fullMsg);
                SetStatus("Failed.");
                MessageBox.Show("Error: " + fullMsg, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetRunning(false);
                _cts.Dispose();
                _cts = null;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            btnCancel.IsEnabled = false;
            Log("Cancelling...");
        }

        private void SetRunning(bool running)
        {
            _isRunning = running;
            btnExecute.IsEnabled = !running;
            btnCancel.IsEnabled  = running;
            btnExit.IsEnabled    = !running;
            btnBrowse.IsEnabled  = !running;
            Cursor = running ? Cursors.AppStarting : null;
        }

        private void ReportProgress(string status, double percent)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblCurrentOperation.Text = status;
                progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, percent));
            }));
        }

        private static Task RunOnStaThread(Action work)
        {
            var tcs = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
            {
                try { work(); tcs.SetResult(true); }
                catch (Exception ex) { tcs.SetException(ex); }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }

        // ─── Export (runs on a background STA thread) ────────────────────────

        private sealed class ExportJob
        {
            public string Format, SavePath, DatabasePath;
            public DataTable Respondents, Answers, OpenEnded;
            public Action<string, double> Progress;
            public Action<string> Log;
            public CancellationToken Token;

            public int RowCount;
            public string CsvPath, OeXlsxPath;

            public void Run()
            {
                if (!File.Exists(DatabasePath))
                    throw new Exception("Script file not found: " + DatabasePath);

                Progress("Building columns...", 62);
                List<string> columns;
                List<List<string>> data;
                var sql = new SQLite(DatabasePath);
                sql.connect();
                try
                {
                    columns = sql.getTableColumnReport();
                    Progress("Building report rows...", 65);
                    data = sql.getTableDataReport(columns, Respondents, Answers, OpenEnded, null);
                    if (data == null)
                        throw new Exception("Building the report failed: " + sql.LastError);
                }
                finally
                {
                    sql.Qconnection?.Close();
                }
                RowCount = data.Count;
                Log($"Report built: {data.Count:N0} interview(s) × {columns.Count:N0} column(s).");

                // The downloaded answers are no longer needed; free them before Excel starts.
                Answers = null;
                Respondents = null;

                Token.ThrowIfCancellationRequested();

                if (Format == "Excel")
                {
                    WriteWorkbook(SavePath, columns, data, includeData: true);
                    Log("Saved: " + SavePath);
                }
                else
                {
                    OeXlsxPath = Path.ChangeExtension(SavePath, ".xlsx");
                    CsvPath    = Path.ChangeExtension(SavePath, ".csv");
                    WriteWorkbook(OeXlsxPath, columns, data, includeData: false);
                    Log("Open-ended saved to: " + OeXlsxPath);

                    Progress("Writing CSV...", 95);
                    SaveToCsvStream(columns, data, CsvPath);
                    Log("CSV saved to: " + CsvPath);
                }
            }

            private void WriteWorkbook(string path, List<string> columns, List<List<string>> data, bool includeData)
            {
                Excel.Application app = null;
                Excel.Workbook book = null;
                try
                {
                    Progress("Starting Excel...", 75);
                    app = new Excel.Application();
                    app.Visible        = false;
                    app.DisplayAlerts  = false;   // never block on a hidden "replace file?" prompt
                    app.ScreenUpdating = false;
                    app.EnableEvents   = false;

                    book = app.Workbooks.Add();

                    // Sheet 1: Open-ended
                    var wsOE = (Excel.Worksheet)book.Worksheets[1];
                    wsOE.Name = "Openended";
                    WriteOeSheet(wsOE);

                    // Sheet 2 (placed first): main data
                    if (includeData)
                    {
                        var wsData = (Excel.Worksheet)book.Worksheets.Add(book.Worksheets[1]);
                        wsData.Name = "Data";
                        WriteDataSheet(wsData, columns, data);
                    }

                    Progress("Saving workbook...", 93);
                    book.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook);
                }
                finally
                {
                    if (book != null) { try { book.Close(false); } catch { } Marshal.ReleaseComObject(book); }
                    if (app != null)  { try { app.Quit(); } catch { } Marshal.ReleaseComObject(app); }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            private void WriteOeSheet(Excel.Worksheet ws)
            {
                Progress("Writing open-ended sheet...", 77);
                DataTable oe = OpenEnded ?? new DataTable();
                DataColumn cResp = oe.Columns["respondent_id"], cQ = oe.Columns["q_id"],
                           cAttr = oe.Columns["attribute_value"], cText = oe.Columns["response"];

                int total = oe.Rows.Count;
                var block = new object[total + 1, 4];
                block[0, 0] = "Respondent Id";
                block[0, 1] = "QId";
                block[0, 2] = "Attribute Value";
                block[0, 3] = "OE Verbatim";
                for (int i = 0; i < total; i++)
                {
                    DataRow r = oe.Rows[i];
                    block[i + 1, 0] = AsText(Cell(r, cResp));
                    block[i + 1, 1] = AsText(Cell(r, cQ));
                    block[i + 1, 2] = AsText(Cell(r, cAttr));
                    block[i + 1, 3] = AsText(Clean(Cell(r, cText)));
                }
                WriteBlock(ws, block, 1, 4, "open-ended", 77, 80);
                AutoFit(ws, total + 1, 4);
            }

            private void WriteDataSheet(Excel.Worksheet ws, List<string> columns, List<List<string>> data)
            {
                int totalCols = columns.Count;
                int totalRows = data.Count;

                var header = new object[1, totalCols];
                for (int j = 0; j < totalCols; j++) header[0, j] = AsText(columns[j]);
                ((Excel.Range)ws.Cells[1, 1]).get_Resize(1, totalCols).Value2 = header;

                int chunk = Math.Max(1, CellsPerWrite / Math.Max(1, totalCols));
                for (int start = 0; start < totalRows; start += chunk)
                {
                    Token.ThrowIfCancellationRequested();
                    int n = Math.Min(chunk, totalRows - start);
                    var block = new object[n, totalCols];
                    for (int i = 0; i < n; i++)
                    {
                        List<string> row = data[start + i];
                        for (int j = 0; j < totalCols; j++)
                            block[i, j] = AsText(Clean(j < row.Count ? row[j] : ""));
                    }
                    Excel.Range first = (Excel.Range)ws.Cells[start + 2, 1];
                    first.get_Resize(n, totalCols).Value2 = block;

                    int done = start + n;
                    Progress($"Writing rows {done:N0} / {totalRows:N0}...", 80 + 12.0 * done / totalRows);
                }
                AutoFit(ws, totalRows + 1, totalCols);
            }

            private void WriteBlock(Excel.Worksheet ws, object[,] block, int firstRow, int cols,
                                    string what, double pctFrom, double pctTo)
            {
                int rows = block.GetLength(0);
                int chunk = Math.Max(1, CellsPerWrite / cols);
                for (int start = 0; start < rows; start += chunk)
                {
                    Token.ThrowIfCancellationRequested();
                    int n = Math.Min(chunk, rows - start);
                    var part = new object[n, cols];
                    for (int i = 0; i < n; i++)
                        for (int j = 0; j < cols; j++)
                            part[i, j] = block[start + i, j];
                    Excel.Range first = (Excel.Range)ws.Cells[firstRow + start, 1];
                    first.get_Resize(n, cols).Value2 = part;
                    Progress($"Writing {what} rows {start + n:N0} / {rows:N0}...",
                             pctFrom + (pctTo - pctFrom) * (start + n) / rows);
                }
            }

            // Column widths are measured on the first rows only; on wide data sheets a full
            // AutoFit takes longer than writing the data itself.
            private static void AutoFit(Excel.Worksheet ws, int usedRows, int usedCols)
            {
                int rows = Math.Max(1, Math.Min(usedRows, AutoFitSampleRows));
                Excel.Range first = (Excel.Range)ws.Cells[1, 1];
                first.get_Resize(rows, usedCols).Columns.AutoFit();
            }

            private static string Cell(DataRow r, DataColumn c)
            {
                return c == null || r.IsNull(c) ? "" : Convert.ToString(r[c]);
            }

            // "'" keeps codes such as 01 as text; Excel cells hold at most 32,767 characters.
            private static string AsText(string s)
            {
                s = s ?? "";
                if (s.Length > MaxCellText) s = s.Substring(0, MaxCellText);
                return "'" + s;
            }
        }

        // ─── CSV helpers ─────────────────────────────────────────────────────

        private static void SaveToCsvStream(
            List<string> columnName, List<List<string>> tableData, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8, 1 << 16))
            {
                writer.WriteLine(EscapeCsvLine(columnName));
                foreach (var row in tableData)
                    writer.WriteLine(EscapeCsvLine(row));
            }
        }

        private static string EscapeCsvLine(List<string> fields)
        {
            return string.Join(",", fields.Select(f =>
            {
                if (string.IsNullOrEmpty(f)) return "";
                if (f.IndexOfAny(CsvSpecialChars) >= 0)
                    return "\"" + f.Replace("\"", "\"\"") + "\"";
                return f;
            }));
        }

        private static readonly char[] CsvSpecialChars = { ',', '"', '\n', '\r' };

        // ─── Validation ──────────────────────────────────────────────────────

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(comProjectName.Text))
            { MessageBox.Show("Please select a project."); return false; }
            if (string.IsNullOrEmpty(comConsiderDate.Text) || !_dateTypeMap.ContainsKey(comConsiderDate.Text))
            { MessageBox.Show("Please select a date type."); return false; }
            if (string.IsNullOrEmpty(comInterviewType.Text) || !_interviewTypeMap.ContainsKey(comInterviewType.Text))
            { MessageBox.Show("Please select an interview type."); return false; }
            if (string.IsNullOrWhiteSpace(txtSaveLocation.Text))
            { MessageBox.Show("Please select a save location."); return false; }
            if (dtpDateFrom.SelectedDate == null || dtpDateTo.SelectedDate == null)
            { MessageBox.Show("Please select valid dates."); return false; }
            if (dtpDateFrom.SelectedDate.Value > dtpDateTo.SelectedDate.Value)
            { MessageBox.Show("Start date must not be after end date."); return false; }
            return true;
        }

        // Makes sure the file name carries the extension of the chosen format.
        private static string NormaliseSavePath(string path, string format)
        {
            string ext = format == "Excel" ? ".xlsx" : ".csv";
            return string.Equals(Path.GetExtension(path), ext, StringComparison.OrdinalIgnoreCase)
                ? path
                : Path.ChangeExtension(path, ext);
        }

        /// <summary>Checked before downloading, so a bad path or an open file doesn't waste the download.</summary>
        private static string CheckOutputFiles(string savePath, string format)
        {
            string dir;
            try { dir = Path.GetDirectoryName(Path.GetFullPath(savePath)); }
            catch (Exception ex) { return "The save location is not valid:\n" + ex.Message; }
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return "The save folder does not exist:\n" + dir;

            var files = format == "Excel"
                ? new[] { savePath }
                : new[] { Path.ChangeExtension(savePath, ".csv"), Path.ChangeExtension(savePath, ".xlsx") };
            foreach (string f in files)
            {
                if (!File.Exists(f)) continue;
                try { using (File.Open(f, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { } }
                catch (IOException) { return "This file is open in another program (probably Excel). Please close it first:\n" + f; }
                catch (UnauthorizedAccessException) { return "This file cannot be overwritten:\n" + f; }
            }
            return null;
        }

        // ─── Browse handler ──────────────────────────────────────────────────

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog { Title = "Save Data File" };
            if (comFileType.Text == "Excel")
                dlg.Filter = "Excel 2007|*.xlsx|All Files|*.*";
            else
                dlg.Filter = "CSV|*.csv|All Files|*.*";

            if (dlg.ShowDialog() == true)
            {
                string dir      = Path.GetDirectoryName(dlg.FileName);
                string nameOnly = Path.GetFileNameWithoutExtension(dlg.FileName);
                string ext      = Path.GetExtension(dlg.FileName);
                string suffix   = dtpDateFrom.SelectedDate?.ToString("yyyyMMdd")
                                + "_" + dtpDateTo.SelectedDate?.ToString("yyyyMMdd");
                txtSaveLocation.Text = Path.Combine(dir, nameOnly + "_" + suffix + ext);
                Properties.Settings.Default.StartupPath = dir;
                Properties.Settings.Default.Save();
            }
        }

        // ─── Project combo handler ───────────────────────────────────────────

        private void comProjectName_DropDownClosed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comProjectName.Text)) return;
            if (_projectStartDateMap == null ||
                !_projectStartDateMap.ContainsKey(comProjectName.Text)) return;
            if (DateTime.TryParse(_projectStartDateMap[comProjectName.Text], out DateTime d))
                dtpDateFrom.SelectedDate = d;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) => this.Close();

        // ─── Utility ─────────────────────────────────────────────────────────

        // Converts a server date string (DD-MM-YYYY or other parseable formats) to
        // an ISO yyyy-MM-dd string that DateTime.TryParse() can always handle.
        private static string ConvertDateFormat(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";

            // Primary: server sends DD-MM-YYYY
            if (DateTime.TryParseExact(raw, "dd-MM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime d))
                return d.ToString("yyyy-MM-dd");

            // Fallback: anything else parseable
            if (DateTime.TryParse(raw,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out d))
                return d.ToString("yyyy-MM-dd");

            return "";
        }

        private static string Clean(string s)
            => (s ?? "").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

        // Safe to call from any thread; never blocks the caller.
        private void Log(string msg)
        {
            string line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n";
            Action append = () => { txtLog.AppendText(line); txtLog.ScrollToEnd(); };
            if (Dispatcher.CheckAccess()) append();
            else Dispatcher.BeginInvoke(append);
        }

        private void SetStatus(string msg)
        {
            if (Dispatcher.CheckAccess()) lblCurrentOperation.Text = msg;
            else Dispatcher.BeginInvoke(new Action(() => lblCurrentOperation.Text = msg));
        }
    }
}
