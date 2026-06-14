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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        // ─── Downloaded tables ───────────────────────────────────────────────
        private DataTable _dt1, _dt2, _dt3;

        // ─── Cancel support ──────────────────────────────────────────────────
        private CancellationTokenSource _cts;

        public FrmDownloadData()
        {
            InitializeComponent();
        }

        // ─── Initialisation ──────────────────────────────────────────────────

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol  = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            txtServerAddress.Text = StaticClass.SERVER_URL + "/deskapi/";

            PopulateCombos();

            dtpDateFrom.SelectedDate = DateTime.Today;
            dtpDateTo.SelectedDate   = DateTime.Today;
            comInterviewType.Text    = "Final Interviews";
            comConsiderDate.Text     = "Sync Date";
            comFileType.Text         = "Excel";

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
                Log("Project load failed: " + ex.Message);
            }
            finally
            {
                btnExecute.IsEnabled = true;
            }
        }

        // ─── HTTP helper (async POST via WebClient) ──────────────────────────

        private static async Task<string> PostAsync(string url, string body, CancellationToken ct)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                ct.Register(() => wc.CancelAsync());
                return await wc.UploadStringTaskAsync(url, "POST", body);
            }
        }

        private static DataTable ParseJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new DataTable();
            return JsonConvert.DeserializeObject(json, typeof(DataTable)) as DataTable
                   ?? new DataTable();
        }

        // ─── Download phases ─────────────────────────────────────────────────

        private async Task DownloadScriptAsync(string dbName, string databasePath,
            CancellationToken ct)
        {
            Log("Downloading project script from server...");
            string source = StaticClass.SERVER_URL + "/scripts/" + dbName;
            if (File.Exists(databasePath)) File.Delete(databasePath);
            using (var wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ct.Register(() => wc.CancelAsync());
                await wc.DownloadFileTaskAsync(source, databasePath);
            }
            Log("Script downloaded.");
        }

        private async Task DownloadRespondentsAsync(string startDate, string endDate,
            string dateType, string projectCode, string interviewType, CancellationToken ct)
        {
            Log("Downloading respondents...");
            SetStatus("Phase 1/3 — Respondents");
            string body = "startDate=" + startDate + "&endDate=" + endDate
                        + "&dateType=" + dateType + "&projectCode=" + projectCode
                        + "&interviewType=" + interviewType;
            string json = await PostAsync(
                StaticClass.SERVER_URL + "/deskapi/respondentbyproject.php", body, ct);
            DataTable dt = ParseJson(json);
            if (dt.Rows.Count > 0) _dt1.Merge(dt);
            Log("Respondents: " + _dt1.Rows.Count + " record(s).");
        }

        private async Task DownloadAnswersAsync(string startDate, string endDate,
            string dateType, string projectCode, string interviewType, CancellationToken ct)
        {
            Log("Downloading answers...");
            SetStatus("Phase 2/3 — Answers");
            long offset = 0;
            long batchCount;
            int page = 1;
            do
            {
                ct.ThrowIfCancellationRequested();
                string body = "startDate=" + startDate + "&endDate=" + endDate
                            + "&dateType=" + dateType + "&projectCode=" + projectCode
                            + "&myOffset=" + offset + "&interviewType=" + interviewType;
                string json = await PostAsync(
                    StaticClass.SERVER_URL + "/deskapi/answerbyproject.php", body, ct);
                DataTable dt = ParseJson(json);
                batchCount = dt.Rows.Count;
                if (batchCount > 0) _dt2.Merge(dt);
                offset += batchCount;
                Log("Answers page " + page + ": " + _dt2.Rows.Count + " row(s) so far...");
                page++;
            }
            while (batchCount == 10000);
            Log("Answers complete: " + _dt2.Rows.Count + " total row(s).");
        }

        private async Task DownloadOpenEndedAsync(string startDate, string endDate,
            string dateType, string projectCode, string interviewType, CancellationToken ct)
        {
            Log("Downloading open-ended responses...");
            SetStatus("Phase 3/3 — Open-Ended");
            string body = "startDate=" + startDate + "&endDate=" + endDate
                        + "&dateType=" + dateType + "&projectCode=" + projectCode
                        + "&interviewType=" + interviewType;
            string json = await PostAsync(
                StaticClass.SERVER_URL + "/deskapi/openendedbyproject.php", body, ct);
            DataTable dt = ParseJson(json);
            if (dt.Rows.Count > 0) _dt3.Merge(dt);
            Log("Open-ended: " + _dt3.Rows.Count + " record(s).");
        }

        // ─── Execute handler ─────────────────────────────────────────────────

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            btnExecute.IsEnabled = false;
            btnCancel.IsEnabled  = true;
            progressBar1.Value   = 0;
            txtLog.Clear();

            string projectName    = comProjectName.Text;
            string dbName         = _projectDbMap[projectName];
            string tempPath       = Path.GetTempPath();
            string databasePath   = Path.Combine(tempPath, dbName);
            string startDate      = dtpDateFrom.SelectedDate.Value.ToString("yyyy-MM-dd");
            string endDate        = dtpDateTo.SelectedDate.Value.ToString("yyyy-MM-dd");
            string dateType       = _dateTypeMap[comConsiderDate.Text];
            string projectCode    = _projectCodeMap[projectName];
            string interviewType  = _interviewTypeMap[comInterviewType.Text];
            string format         = comFileType.Text;

            try
            {
                // Step 1 — Download script
                if (!File.Exists(databasePath) || chkDownloadScript.IsChecked == true)
                {
                    await DownloadScriptAsync(dbName, databasePath, ct);
                }
                else
                {
                    Log("Using cached project script.");
                }
                progressBar1.Value = 10;

                // Step 2 — Download data
                _dt1 = new DataTable();
                _dt2 = new DataTable();
                _dt3 = new DataTable();

                await DownloadRespondentsAsync(startDate, endDate, dateType, projectCode, interviewType, ct);
                progressBar1.Value = 30;

                await DownloadAnswersAsync(startDate, endDate, dateType, projectCode, interviewType, ct);
                progressBar1.Value = 60;

                await DownloadOpenEndedAsync(startDate, endDate, dateType, projectCode, interviewType, ct);
                progressBar1.Value = 75;

                // Step 3 — Export (runs on UI thread; Excel COM must be UI-thread-affine)
                Log("Building report...");
                SetStatus("Exporting...");
                ExportData(format, databasePath, projectName);
                progressBar1.Value = 100;

                Log("Complete.");
                MessageBox.Show("Data download complete.", "Done",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (OperationCanceledException)
            {
                Log("Cancelled by user.");
                progressBar1.Value = 0;
                SetStatus("Cancelled.");
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnExecute.IsEnabled = true;
                btnCancel.IsEnabled  = false;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            btnCancel.IsEnabled = false;
            Log("Cancelling...");
        }

        // ─── Export (Phase 2 cleanup) ─────────────────────────────────────────

        private void ExportData(string format, string databasePath, string projectName)
        {
            if (!File.Exists(databasePath))
            {
                MessageBox.Show("Script file not found:\n" + databasePath);
                return;
            }

            var sql = new SQLite(databasePath);
            sql.connect();

            try
            {
                SetStatus("Building columns...");
                List<string> columns      = sql.getTableColumnReport();
                List<List<string>> data   = sql.getTableDataReport(columns, _dt1, _dt2, _dt3, progressBar1);

                if (format == "Excel")
                    ExportToExcel(columns, data);
                else
                    ExportToCsv(columns, data);
            }
            finally
            {
                sql.Qconnection?.Close();
            }
        }

        private void ExportToExcel(List<string> columns, List<List<string>> data)
        {
            SetStatus("Writing Excel...");
            Microsoft.Office.Interop.Excel.Application xlApp  = null;
            Microsoft.Office.Interop.Excel.Workbook    xlBook = null;
            object miss = System.Reflection.Missing.Value;
            try
            {
                xlApp  = new Microsoft.Office.Interop.Excel.Application();
                xlBook = xlApp.Workbooks.Add(miss);

                // Sheet 1: Open-ended
                var wsOE = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item(1);
                wsOE.Name = "Openended";
                WriteOeSheet(wsOE);

                // Sheet 2: Main data
                var wsData = (Microsoft.Office.Interop.Excel.Worksheet)
                    xlBook.Worksheets.Add(xlBook.Worksheets[1]);
                wsData.Name = "Data";
                WriteDataSheet(wsData, columns, data);

                xlBook.SaveAs(txtSaveLocation.Text,
                    Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                Log("Saved: " + txtSaveLocation.Text);
            }
            finally
            {
                xlBook?.Close(true, miss, miss);
                xlApp?.Quit();
                if (xlBook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                if (xlApp  != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        private void ExportToCsv(List<string> columns, List<List<string>> data)
        {
            // OE sheet still goes to a companion Excel file
            SetStatus("Writing OE Excel...");
            Microsoft.Office.Interop.Excel.Application xlApp  = null;
            Microsoft.Office.Interop.Excel.Workbook    xlBook = null;
            object miss = System.Reflection.Missing.Value;
            try
            {
                xlApp  = new Microsoft.Office.Interop.Excel.Application();
                xlBook = xlApp.Workbooks.Add(miss);
                var wsOE = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets.get_Item(1);
                wsOE.Name = "Openended";
                WriteOeSheet(wsOE);
                string xlsxPath = Path.ChangeExtension(txtSaveLocation.Text, ".xlsx");
                xlBook.SaveAs(xlsxPath,
                    Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                Log("OE saved: " + xlsxPath);
            }
            finally
            {
                xlBook?.Close(true, miss, miss);
                xlApp?.Quit();
                if (xlBook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                if (xlApp  != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }

            // Main data → CSV
            SetStatus("Writing CSV...");
            string csvPath = Path.ChangeExtension(txtSaveLocation.Text, ".csv");
            SaveToCsvStream(columns, data, csvPath);
            Log("CSV saved: " + csvPath);
        }

        // ─── Sheet writers ───────────────────────────────────────────────────

        private void WriteOeSheet(Microsoft.Office.Interop.Excel.Worksheet ws)
        {
            ws.Cells[1, 1] = "Respondent Id";
            ws.Cells[1, 2] = "QId";
            ws.Cells[1, 3] = "Attribute Value";
            ws.Cells[1, 4] = "OE Verbatim";

            int row = 2;
            using (DataTableReader r = _dt3.CreateDataReader())
            {
                while (r.Read())
                {
                    ws.Cells[row, 1] = "'" + r["respondent_id"];
                    ws.Cells[row, 2] = "'" + r["q_id"];
                    ws.Cells[row, 3] = "'" + r["attribute_value"];
                    ws.Cells[row, 4] = "'" + Clean(r["response"].ToString());
                    row++;
                }
            }
            ws.Columns.AutoFit();
        }

        private void WriteDataSheet(Microsoft.Office.Interop.Excel.Worksheet ws,
            List<string> columns, List<List<string>> data)
        {
            for (int i = 0; i < columns.Count; i++)
                ws.Cells[1, i + 1] = "'" + columns[i];

            int totalRows = data.Count;
            int totalCols = totalRows > 0 ? data[0].Count : columns.Count;
            const int batchSize = 500;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = totalRows;
            progressBar1.Value   = 0;

            for (int rowStart = 0; rowStart < totalRows; rowStart += batchSize)
            {
                int batch = Math.Min(batchSize, totalRows - rowStart);
                var arr = new object[batch, totalCols];

                for (int i = 0; i < batch; i++)
                    for (int j = 0; j < totalCols; j++)
                        arr[i, j] = "'" + Clean(data[rowStart + i][j]);

                var startCell = (Microsoft.Office.Interop.Excel.Range)ws.Cells[rowStart + 2, 1];
                var range     = startCell.get_Resize(batch, totalCols);
                range.Value2  = arr;

                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(startCell);

                int done = rowStart + batch;
                progressBar1.Value = done;
                SetStatus("Writing rows " + done + " / " + totalRows + "...");
                DoEvents();
            }

            ws.Columns.AutoFit();
        }

        // ─── CSV helpers ─────────────────────────────────────────────────────

        public static void SaveToCsvStream(
            List<string> columnName, List<List<string>> tableData, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
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
                if (f.Contains(",") || f.Contains("\"") || f.Contains("\n"))
                    return "\"" + f.Replace("\"", "\"\"") + "\"";
                return f;
            }));
        }

        // ─── Validation ──────────────────────────────────────────────────────

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(comProjectName.Text))
            { MessageBox.Show("Please select a project."); return false; }
            if (string.IsNullOrEmpty(comConsiderDate.Text))
            { MessageBox.Show("Please select a date type."); return false; }
            if (string.IsNullOrEmpty(comInterviewType.Text))
            { MessageBox.Show("Please select an interview type."); return false; }
            if (string.IsNullOrEmpty(txtSaveLocation.Text))
            { MessageBox.Show("Please select a save location."); return false; }
            if (dtpDateFrom.SelectedDate == null || dtpDateTo.SelectedDate == null)
            { MessageBox.Show("Please select valid dates."); return false; }
            if (dtpDateFrom.SelectedDate.Value > dtpDateTo.SelectedDate.Value)
            { MessageBox.Show("Start date must not be after end date."); return false; }
            return true;
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
                string dir     = Path.GetDirectoryName(dlg.FileName);
                string nameOnly = Path.GetFileNameWithoutExtension(dlg.FileName);
                string ext     = Path.GetExtension(dlg.FileName);
                string suffix  = dtpDateFrom.SelectedDate?.ToString("yyyyMMdd")
                               + "_" + dtpDateTo.SelectedDate?.ToString("yyyyMMdd");
                txtSaveLocation.Text = Path.Combine(dir, nameOnly + "_" + suffix + ext);
                Properties.Settings.Default.StartupPath = dir;
                Properties.Settings.Default.Save();
            }
        }

        // ─── Project combo handlers ──────────────────────────────────────────

        private void comProjectName_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

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

        private static string ConvertDateFormat(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            string[] p = raw.Split('-');
            return p.Length == 3 ? p[1] + "-" + p[0] + "-" + p[2] : raw;
        }

        private static string Clean(string s)
            => s.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

        private void Log(string msg)
        {
            string line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + "\n";
            if (txtLog.Dispatcher.CheckAccess())
            {
                txtLog.AppendText(line);
                txtLog.ScrollToEnd();
            }
            else
            {
                txtLog.Dispatcher.Invoke(() => { txtLog.AppendText(line); txtLog.ScrollToEnd(); });
            }
        }

        private void SetStatus(string msg)
        {
            if (lblCurrentOperation.Dispatcher.CheckAccess())
                lblCurrentOperation.Text = msg;
            else
                lblCurrentOperation.Dispatcher.Invoke(() => lblCurrentOperation.Text = msg);
        }

        private static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Background,
                new System.Threading.ThreadStart(delegate { }));
        }
    }
}
