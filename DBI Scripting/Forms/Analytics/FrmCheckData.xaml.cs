using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    public partial class FrmCheckData : Window
    {
        // ── Project lookup ────────────────────────────────────────────────────
        private Dictionary<string, string> _projectCodeMap = new Dictionary<string, string>();
        private Dictionary<string, string> _projectDbMap   = new Dictionary<string, string>();

        // ── Results ───────────────────────────────────────────────────────────
        private ObservableCollection<CheckResultItem> _results = new ObservableCollection<CheckResultItem>();

        // ── Cancel support ────────────────────────────────────────────────────
        private CancellationTokenSource _cts;

        private const int HttpTimeoutMs = 300_000;

        // ══════════════════════════════════════════════════════════════════════
        //  INNER MODEL CLASSES
        // ══════════════════════════════════════════════════════════════════════

        public class CheckResultItem
        {
            public string RespondentId { get; set; }
            public string QId          { get; set; }
            public string CheckType    { get; set; }
            public string Issue        { get; set; }
        }

        private class QuestionInfo
        {
            public string QId          { get; set; }
            public string ResponseType { get; set; }  // 1=Single 2=Multi 3=Grid 4=MaxDiff 5=Form
            public string ControlType  { get; set; }
            public int    OrderTag1    { get; set; }
            public int    MinResp      { get; set; }
            public int    MaxResp      { get; set; }
        }

        private class AttribInfo
        {
            public string AttributeValue { get; set; }
            public int    AttributeOrder { get; set; }
            public bool   IsExclusive    { get; set; }
            public bool   TakeOE         { get; set; }
            public string LinkId2        { get; set; }  // grid column-set id
        }

        private class GridColInfo
        {
            public string AttributeValue { get; set; }
            public int    AttributeOrder { get; set; }
        }

        private class LogicInfo
        {
            public string QId         { get; set; }
            public string LogicTypeId { get; set; }
            public string IfCondition { get; set; }
            public string ThenValue   { get; set; }
            public string ElseValue   { get; set; }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  INIT
        // ══════════════════════════════════════════════════════════════════════

        public FrmCheckData()
        {
            InitializeComponent();
            dgResults.ItemsSource = _results;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol  = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            await LoadProjectsAsync();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  PROJECT LOADING
        // ══════════════════════════════════════════════════════════════════════

        private async Task LoadProjectsAsync()
        {
            Log("Connecting to server...");
            btnProcess.IsEnabled = false;
            try
            {
                _projectCodeMap.Clear();
                _projectDbMap.Clear();

                List<ProjectInfo> projects = await Task.Run(
                    () => new DownloadClass().getProjectInfoFromServer());

                cmbProject.Items.Clear();
                if (projects != null && projects.Count > 0)
                {
                    foreach (var p in projects)
                    {
                        cmbProject.Items.Add(p.ProjectName);
                        _projectCodeMap[p.ProjectName] = p.ProjectCode;
                        _projectDbMap[p.ProjectName]   = p.DatabaseName;
                    }
                    Log($"{projects.Count} project(s) loaded.");
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
                btnProcess.IsEnabled = true;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UI EVENT HANDLERS
        // ══════════════════════════════════════════════════════════════════════

        private void cmbProject_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title  = "Select Downloaded Excel Data File",
                Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls"
            };
            if (dlg.ShowDialog() == true)
                txtExcelPath.Text = dlg.FileName;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
        }

        private async void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProject.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a project.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtExcelPath.Text) || !File.Exists(txtExcelPath.Text))
            {
                MessageBox.Show("Please select a valid Excel data file.", "Validation",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBusy(true);
            _results.Clear();
            lblCount.Text       = "";
            btnExport.IsEnabled = false;

            _cts = new CancellationTokenSource();
            try
            {
                await RunChecksAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                Log("Cancelled.");
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Title    = "Save Check Results",
                Filter   = "Excel files (*.xlsx)|*.xlsx",
                FileName = "CheckData_Results_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx"
            };
            if (dlg.ShowDialog() != true) return;

            btnExport.IsEnabled = false;
            try
            {
                var snapshot = _results.ToList();
                await Task.Run(() => ExportResults(dlg.FileName, snapshot));
                Log("Exported to: " + dlg.FileName);
                MessageBox.Show("Export complete.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log("Export failed: " + ex.Message);
                MessageBox.Show(ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnExport.IsEnabled = _results.Count > 0;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  MAIN PIPELINE
        // ══════════════════════════════════════════════════════════════════════

        private async Task RunChecksAsync(CancellationToken ct)
        {
            string projectName = cmbProject.SelectedItem.ToString();
            string dbName      = _projectDbMap[projectName];
            string scriptPath  = Path.Combine("C:\\Temp", dbName);

            // 1. Download / locate script
            if (chkRedownload.IsChecked == true)
                await DownloadScriptAsync(dbName, scriptPath, ct);
            else if (!File.Exists(scriptPath))
            {
                Log($"Cached script not found at {scriptPath}. Downloading...");
                await DownloadScriptAsync(dbName, scriptPath, ct);
            }
            else
                Log($"Using cached script: {scriptPath}");

            ct.ThrowIfCancellationRequested();

            // 2. Load script metadata
            Log("Loading script data...");
            Dictionary<string, QuestionInfo> questions = null;
            Dictionary<string, List<AttribInfo>>  attrs    = null;
            Dictionary<string, List<GridColInfo>> gridCols = null;
            List<LogicInfo> logics = null;

            await Task.Run(() =>
            {
                var result = LoadScriptData(scriptPath);
                questions = result.Item1;
                attrs     = result.Item2;
                gridCols  = result.Item3;
                logics    = result.Item4;
            }, ct);

            Log($"Loaded {questions.Count} questions, {logics.Count} logic rule(s).");
            ct.ThrowIfCancellationRequested();

            // 3. Read Excel
            Log("Reading Excel data...");
            DataTable dtData = null, dtOE = null;
            await Task.Run(() =>
            {
                var r = ReadExcel(txtExcelPath.Text);
                dtData = r.Item1;
                dtOE   = r.Item2;
            }, ct);

            Log($"Loaded {dtData.Rows.Count} respondent row(s) from Excel.");
            ct.ThrowIfCancellationRequested();

            // 4. Build ordered question list and order-index
            var sortedQs = questions.Values
                .OrderBy(q => q.OrderTag1)
                .ToList();

            var qOrderIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < sortedQs.Count; i++)
                qOrderIndex[sortedQs[i].QId] = i;

            // 5. Validate all respondents
            Log("Running checks...");
            int total     = dtData.Rows.Count;
            int processed = 0;
            var allErrors = new List<CheckResultItem>();

            await Task.Run(() =>
            {
                foreach (DataRow row in dtData.Rows)
                {
                    ct.ThrowIfCancellationRequested();

                    string respondentId = row.Table.Columns.Contains("RespondentId")
                        ? row["RespondentId"]?.ToString() ?? ""
                        : row[0]?.ToString() ?? "";

                    var rowErrors = ValidateRespondent(
                        respondentId, row, sortedQs, qOrderIndex,
                        questions, attrs, gridCols, logics, dtOE);

                    allErrors.AddRange(rowErrors);
                    processed++;

                    double pct = (double)processed / total * 100;
                    Dispatcher.InvokeAsync(() => prgProgress.Value = pct);
                }
            }, ct);

            // 6. Push to UI
            foreach (var err in allErrors)
                _results.Add(err);

            int issueCount = allErrors.Count;
            lblCount.Text       = issueCount == 0
                ? "No issues found."
                : $"{issueCount} issue(s) found across {dtData.Rows.Count} respondent(s).";
            btnExport.IsEnabled = issueCount > 0;
            Log($"Done. {issueCount} issue(s) found in {dtData.Rows.Count} respondent(s).");
        }

        // ══════════════════════════════════════════════════════════════════════
        //  SCRIPT DOWNLOAD
        // ══════════════════════════════════════════════════════════════════════

        private async Task DownloadScriptAsync(string dbName, string localPath, CancellationToken ct)
        {
            Log("Downloading script from server...");
            string url = StaticClass.SERVER_URL + "/scripts/" + dbName;
            if (File.Exists(localPath)) File.Delete(localPath);
            using (var wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ct.Register(() => wc.CancelAsync());
                await wc.DownloadFileTaskAsync(url, localPath);
            }
            Log("Script downloaded successfully.");
        }

        // ══════════════════════════════════════════════════════════════════════
        //  LOAD SCRIPT DATA FROM SQLITE
        // ══════════════════════════════════════════════════════════════════════

        private Tuple<Dictionary<string, QuestionInfo>,
                      Dictionary<string, List<AttribInfo>>,
                      Dictionary<string, List<GridColInfo>>,
                      List<LogicInfo>>
            LoadScriptData(string scriptPath)
        {
            var questions = new Dictionary<string, QuestionInfo>(StringComparer.OrdinalIgnoreCase);
            var attrs     = new Dictionary<string, List<AttribInfo>>(StringComparer.OrdinalIgnoreCase);
            var gridCols  = new Dictionary<string, List<GridColInfo>>(StringComparer.OrdinalIgnoreCase);
            var logics    = new List<LogicInfo>();

            using (var conn = new SQLiteConnection("Data Source=" + scriptPath))
            {
                conn.Open();

                // T_QType: ID → (ResponseType, ControlType)
                var qtypeMap = new Dictionary<string, Tuple<string, string>>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = new SQLiteCommand(
                    "SELECT ID, ControlType, ResponseType FROM T_QType", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        qtypeMap[rdr["ID"].ToString()] = Tuple.Create(
                            rdr["ResponseType"].ToString(),
                            rdr["ControlType"].ToString());
                    }
                }

                // T_Question
                using (var cmd = new SQLiteCommand(
                    "SELECT QId, QType, OrderTag1, NoOfResponseMin, NoOfResponseMax " +
                    "FROM T_Question WHERE TRIM(QId) != '' " +
                    "ORDER BY CAST(OrderTag1 AS INTEGER)", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string qId   = rdr["QId"].ToString().Trim();
                        string qType = rdr["QType"].ToString().Trim();
                        string respType = "1", ctrlType = "";
                        if (qtypeMap.TryGetValue(qType, out var qt))
                        {
                            respType = qt.Item1;
                            ctrlType = qt.Item2;
                        }
                        questions[qId] = new QuestionInfo
                        {
                            QId          = qId,
                            ResponseType = respType,
                            ControlType  = ctrlType,
                            OrderTag1    = ToInt(rdr["OrderTag1"]),
                            MinResp      = ToInt(rdr["NoOfResponseMin"]),
                            MaxResp      = ToInt(rdr["NoOfResponseMax"])
                        };
                    }
                }

                // T_OptAttribute (question rows / multi options / grid row-attributes)
                using (var cmd = new SQLiteCommand(
                    "SELECT QId, AttributeValue, AttributeOrder, IsExclusive, " +
                    "       TakeOpenended, LinkId2 " +
                    "FROM T_OptAttribute WHERE TRIM(QId) != '' " +
                    "ORDER BY QId, CAST(AttributeOrder AS INTEGER)", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string qId = rdr["QId"].ToString().Trim();
                        if (!attrs.ContainsKey(qId)) attrs[qId] = new List<AttribInfo>();
                        attrs[qId].Add(new AttribInfo
                        {
                            AttributeValue = rdr["AttributeValue"].ToString().Trim(),
                            AttributeOrder = ToInt(rdr["AttributeOrder"]),
                            IsExclusive    = ToBool(rdr["IsExclusive"]),
                            TakeOE         = ToBool(rdr["TakeOpenended"]),
                            LinkId2        = rdr["LinkId2"].ToString().Trim()
                        });
                    }
                }

                // T_GridInfo (grid column definitions — keyed by grid-set id / LinkId2)
                using (var cmd = new SQLiteCommand(
                    "SELECT QId, AttributeValue, AttributeOrder " +
                    "FROM T_GridInfo WHERE TRIM(QId) != '' " +
                    "ORDER BY QId, CAST(AttributeOrder AS INTEGER)", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        string qId = rdr["QId"].ToString().Trim();
                        if (!gridCols.ContainsKey(qId)) gridCols[qId] = new List<GridColInfo>();
                        gridCols[qId].Add(new GridColInfo
                        {
                            AttributeValue = rdr["AttributeValue"].ToString().Trim(),
                            AttributeOrder = ToInt(rdr["AttributeOrder"])
                        });
                    }
                }

                // T_LogicTable — only jump (3) and appearance (4) logic
                using (var cmd = new SQLiteCommand(
                    "SELECT QId, LogicTypeId, IfCondition, [Then], [Else] " +
                    "FROM T_LogicTable " +
                    "WHERE TRIM(QId) != '' AND (LogicTypeId = '3' OR LogicTypeId = '4') " +
                    "AND TRIM(IfCondition) != ''", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        logics.Add(new LogicInfo
                        {
                            QId         = rdr["QId"].ToString().Trim(),
                            LogicTypeId = rdr["LogicTypeId"].ToString().Trim(),
                            IfCondition = rdr["IfCondition"].ToString().Trim(),
                            ThenValue   = rdr["Then"].ToString().Trim(),
                            ElseValue   = rdr["Else"].ToString().Trim()
                        });
                    }
                }

                conn.Close();
            }

            return Tuple.Create(questions, attrs, gridCols, logics);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  READ EXCEL (both sheets)
        // ══════════════════════════════════════════════════════════════════════

        private Tuple<DataTable, DataTable> ReadExcel(string filePath)
        {
            Excel.Application xlApp = null;
            Excel.Workbook    xlWb  = null;
            try
            {
                xlApp = new Excel.Application { Visible = false, DisplayAlerts = false };
                xlWb  = xlApp.Workbooks.Open(
                    filePath, ReadOnly: true,
                    IgnoreReadOnlyRecommended: true);

                DataTable dtData = ReadSheet(xlWb, "Data");
                DataTable dtOE   = ReadSheet(xlWb, "Openended");
                return Tuple.Create(dtData, dtOE);
            }
            finally
            {
                if (xlWb  != null) { xlWb.Close(false); StaticClass.releaseObject(xlWb); }
                if (xlApp != null) { xlApp.Quit();       StaticClass.releaseObject(xlApp); }
            }
        }

        private DataTable ReadSheet(Excel.Workbook wb, string sheetName)
        {
            var dt = new DataTable(sheetName);
            Excel.Worksheet ws = null;

            foreach (Excel.Worksheet s in wb.Sheets)
            {
                if (string.Equals(s.Name, sheetName, StringComparison.OrdinalIgnoreCase))
                { ws = s; break; }
            }
            if (ws == null) return dt;

            try
            {
                Excel.Range used = ws.UsedRange;
                object[,] vals   = (object[,])used.Value2;
                if (vals == null) return dt;

                int rows = vals.GetLength(0);
                int cols = vals.GetLength(1);

                // Row 1 = headers
                for (int c = 1; c <= cols; c++)
                {
                    string hdr = vals[1, c]?.ToString().Trim() ?? $"Col{c}";
                    while (dt.Columns.Contains(hdr)) hdr += "_" + c;
                    dt.Columns.Add(hdr);
                }

                // Rows 2+
                for (int r = 2; r <= rows; r++)
                {
                    var row = dt.NewRow();
                    for (int c = 1; c <= cols; c++)
                        row[c - 1] = vals[r, c]?.ToString() ?? "";
                    dt.Rows.Add(row);
                }
            }
            finally
            {
                StaticClass.releaseObject(ws);
            }

            return dt;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  VALIDATE ONE RESPONDENT
        // ══════════════════════════════════════════════════════════════════════

        private List<CheckResultItem> ValidateRespondent(
            string respondentId,
            DataRow row,
            List<QuestionInfo> sortedQs,
            Dictionary<string, int> qOrderIndex,
            Dictionary<string, QuestionInfo> questions,
            Dictionary<string, List<AttribInfo>>  attrs,
            Dictionary<string, List<GridColInfo>> gridCols,
            List<LogicInfo> logics,
            DataTable dtOE)
        {
            var errors  = new List<CheckResultItem>();

            // ── Build skip set ──────────────────────────────────────────────

            var skipSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Appearance logic (type 4): skip question when condition is FALSE
            foreach (var lg in logics.Where(l => l.LogicTypeId == "4"))
            {
                bool condMet = EvaluateCondition(lg.IfCondition, row, questions, attrs);
                if (!condMet)
                    skipSet.Add(lg.QId);
            }

            // Jump logic (type 3): mark in-between questions as skipped
            foreach (var lg in logics.Where(l => l.LogicTypeId == "3"))
            {
                if (!qOrderIndex.ContainsKey(lg.QId)) continue;

                bool   condMet  = EvaluateCondition(lg.IfCondition, row, questions, attrs);
                string targetQId = condMet ? lg.ThenValue : lg.ElseValue;
                if (string.IsNullOrEmpty(targetQId)) continue;
                if (!qOrderIndex.ContainsKey(targetQId)) continue;

                int fromPos = qOrderIndex[lg.QId];
                int toPos   = qOrderIndex[targetQId];

                // Forward jump: mark skipped range
                if (toPos > fromPos + 1)
                {
                    for (int p = fromPos + 1; p < toPos; p++)
                        skipSet.Add(sortedQs[p].QId);
                }
            }

            // ── Validate each question ──────────────────────────────────────

            foreach (var q in sortedQs)
            {
                attrs.TryGetValue(q.QId, out var attribList);
                attribList = attribList ?? new List<AttribInfo>();

                // Grid column set: taken from the first attribute's LinkId2
                string gridSetId = attribList
                    .FirstOrDefault(a => !string.IsNullOrEmpty(a.LinkId2))?.LinkId2;
                gridCols.TryGetValue(gridSetId ?? "", out var colList);
                colList = colList ?? new List<GridColInfo>();

                bool shouldSkip = skipSet.Contains(q.QId);
                bool hasData    = HasAnyData(row, q, attribList, colList);

                if (shouldSkip)
                {
                    if (hasData)
                        errors.Add(new CheckResultItem
                        {
                            RespondentId = respondentId,
                            QId          = q.QId,
                            CheckType    = "Should Be Skipped",
                            Issue        = "Question was routed out by logic but contains data"
                        });
                    continue;
                }

                // Question should be visible — validate it
                ValidateQuestion(respondentId, row, q, attribList, colList, dtOE, errors);
            }

            return errors;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  PER-QUESTION VALIDATION
        // ══════════════════════════════════════════════════════════════════════

        private void ValidateQuestion(
            string respondentId,
            DataRow row,
            QuestionInfo q,
            List<AttribInfo> attribList,
            List<GridColInfo> colList,
            DataTable dtOE,
            List<CheckResultItem> errors)
        {
            bool isNPS = q.ControlType.IndexOf("NPS", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isNPS)
            {
                ValidateNPS(respondentId, row, q, errors);
                return;
            }

            switch (q.ResponseType)
            {
                case "2": ValidateMulti(respondentId, row, q, attribList, dtOE, errors);          break;
                case "3": ValidateGrid(respondentId, row, q, attribList, colList, errors);        break;
                default:  ValidateSingle(respondentId, row, q, attribList, dtOE, errors);         break;
            }
        }

        // ─── Single choice ────────────────────────────────────────────────────

        private void ValidateSingle(string respondentId, DataRow row, QuestionInfo q,
            List<AttribInfo> attribList, DataTable dtOE, List<CheckResultItem> errors)
        {
            if (!row.Table.Columns.Contains(q.QId))
            {
                if (q.MinResp > 0)
                    errors.Add(Make(respondentId, q.QId, "Column Missing",
                        $"Expected column '{q.QId}' not in Excel"));
                return;
            }

            string val = row[q.QId]?.ToString().Trim() ?? "";

            if (string.IsNullOrEmpty(val))
            {
                if (q.MinResp > 0)
                    errors.Add(Make(respondentId, q.QId, "Missing Answer",
                        "Mandatory single-choice question has no answer"));
                return;
            }

            // Validate answer code
            if (attribList.Count > 0)
            {
                var validCodes = new HashSet<string>(
                    attribList.Select(a => a.AttributeValue),
                    StringComparer.OrdinalIgnoreCase);
                if (!validCodes.Contains(val))
                    errors.Add(Make(respondentId, q.QId, "Invalid Code",
                        $"Value '{val}' is not in the valid answer list"));
            }

            // OE check for selected attribute
            var selAttr = attribList.FirstOrDefault(a =>
                string.Equals(a.AttributeValue, val, StringComparison.OrdinalIgnoreCase));
            if (selAttr != null && selAttr.TakeOE)
                CheckOE(respondentId, q.QId, dtOE, errors);
        }

        // ─── Multi choice ─────────────────────────────────────────────────────

        private void ValidateMulti(string respondentId, DataRow row, QuestionInfo q,
            List<AttribInfo> attribList, DataTable dtOE, List<CheckResultItem> errors)
        {
            if (attribList.Count == 0) return;

            int selectedCount  = 0;
            bool hasExclusive  = false;
            bool exclusiveOnly = false;
            var selectedAttrs  = new List<AttribInfo>();

            foreach (var attr in attribList)
            {
                string colName = q.QId + "_" + attr.AttributeOrder;
                if (!row.Table.Columns.Contains(colName)) continue;

                bool isSelected = row[colName]?.ToString().Trim() == "1";
                if (!isSelected) continue;

                selectedCount++;
                selectedAttrs.Add(attr);

                if (attr.IsExclusive)
                {
                    hasExclusive  = true;
                    exclusiveOnly = selectedCount == 1;
                }
            }

            if (selectedCount == 0)
            {
                if (q.MinResp > 0)
                    errors.Add(Make(respondentId, q.QId, "Missing Answer",
                        "Mandatory multi-choice question has no answer"));
                return;
            }

            if (q.MinResp > 0 && selectedCount < q.MinResp)
                errors.Add(Make(respondentId, q.QId, "Min Responses",
                    $"Selected {selectedCount} option(s); minimum required is {q.MinResp}"));

            if (q.MaxResp > 0 && selectedCount > q.MaxResp)
                errors.Add(Make(respondentId, q.QId, "Max Responses",
                    $"Selected {selectedCount} option(s); maximum allowed is {q.MaxResp}"));

            if (hasExclusive && selectedCount > 1)
                errors.Add(Make(respondentId, q.QId, "Exclusive Violation",
                    "An exclusive option is selected together with other options"));

            // OE check for each selected attribute that requires OE
            foreach (var attr in selectedAttrs.Where(a => a.TakeOE))
                CheckOE(respondentId, q.QId, dtOE, errors);
        }

        // ─── Grid ─────────────────────────────────────────────────────────────

        private void ValidateGrid(string respondentId, DataRow row, QuestionInfo q,
            List<AttribInfo> attribList, List<GridColInfo> colList, List<CheckResultItem> errors)
        {
            // attribList = grid rows (from T_OptAttribute)
            // colList    = grid columns (from T_GridInfo via LinkId2)

            if (attribList.Count == 0) return;

            bool anyAnswered = false;

            foreach (var gridRow in attribList)
            {
                int colCount = 0;

                foreach (var gridCol in colList)
                {
                    string cellCol = $"{q.QId}_R{gridRow.AttributeOrder}_C{gridCol.AttributeOrder}";
                    if (!row.Table.Columns.Contains(cellCol)) continue;

                    string val = row[cellCol]?.ToString().Trim() ?? "";
                    if (!string.IsNullOrEmpty(val) && val != "0")
                        colCount++;
                }

                if (colCount > 0) anyAnswered = true;

                if (colCount > 1)
                    errors.Add(Make(respondentId, q.QId, "Grid Multi-Select",
                        $"Row {gridRow.AttributeOrder} has {colCount} answers selected (expected 1)"));
            }

            if (q.MinResp > 0 && !anyAnswered)
                errors.Add(Make(respondentId, q.QId, "Missing Answer",
                    "Mandatory grid question has no answers"));
        }

        // ─── NPS ──────────────────────────────────────────────────────────────

        private void ValidateNPS(string respondentId, DataRow row, QuestionInfo q,
            List<CheckResultItem> errors)
        {
            if (!row.Table.Columns.Contains(q.QId)) return;

            string val = row[q.QId]?.ToString().Trim() ?? "";

            if (string.IsNullOrEmpty(val))
            {
                if (q.MinResp > 0)
                    errors.Add(Make(respondentId, q.QId, "Missing Answer",
                        "Mandatory NPS question has no answer"));
                return;
            }

            if (!int.TryParse(val, out int npsVal) || npsVal < 0 || npsVal > 10)
                errors.Add(Make(respondentId, q.QId, "Invalid NPS Value",
                    $"Value '{val}' is not in the valid NPS range 0–10"));
        }

        // ─── OE verbatim check ────────────────────────────────────────────────

        private void CheckOE(string respondentId, string qId, DataTable dtOE,
            List<CheckResultItem> errors)
        {
            if (dtOE == null || dtOE.Rows.Count == 0) return;

            // Determine column names (handles both naming conventions)
            string respCol = FindColumn(dtOE, "Respondent Id", "RespondentId", "respondent_id");
            string qCol    = FindColumn(dtOE, "QId", "q_id");
            string oeCol   = FindColumn(dtOE, "OE Verbatim", "Attribute Value", "response");

            if (respCol == null || qCol == null || oeCol == null) return;

            bool found = dtOE.AsEnumerable().Any(r =>
                string.Equals(r[respCol]?.ToString(), respondentId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(r[qCol]?.ToString(), qId, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(r[oeCol]?.ToString()));

            if (!found)
                errors.Add(Make(respondentId, qId, "Missing OE Verbatim",
                    "Attribute requires open-ended text but no verbatim found in Openended sheet"));
        }

        // ══════════════════════════════════════════════════════════════════════
        //  HAS ANY DATA
        // ══════════════════════════════════════════════════════════════════════

        private bool HasAnyData(DataRow row, QuestionInfo q,
            List<AttribInfo> attribList, List<GridColInfo> colList)
        {
            switch (q.ResponseType)
            {
                case "2":
                    return attribList.Any(a =>
                    {
                        string col = q.QId + "_" + a.AttributeOrder;
                        return row.Table.Columns.Contains(col) && row[col]?.ToString() == "1";
                    });

                case "3":
                    foreach (var gr in attribList)
                        foreach (var gc in colList)
                        {
                            string col = $"{q.QId}_R{gr.AttributeOrder}_C{gc.AttributeOrder}";
                            if (!row.Table.Columns.Contains(col)) continue;
                            string v = row[col]?.ToString().Trim() ?? "";
                            if (!string.IsNullOrEmpty(v) && v != "0") return true;
                        }
                    return false;

                default:
                    return row.Table.Columns.Contains(q.QId) &&
                           !string.IsNullOrEmpty(row[q.QId]?.ToString().Trim());
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CONDITION EVALUATOR (Excel-adapted, mirrors CheckCondition logic)
        // ══════════════════════════════════════════════════════════════════════

        private bool EvaluateCondition(string expression, DataRow row,
            Dictionary<string, QuestionInfo> questions,
            Dictionary<string, List<AttribInfo>> attrs)
        {
            try
            {
                expression = expression.Replace(" ", "").Trim();
                if (string.IsNullOrEmpty(expression)) return true;

                // Single element — evaluate directly
                if (!expression.Contains('&') && !expression.Contains('|'))
                    return EvaluateSimple(expression, row, questions, attrs);

                // Multi-element: convert to postfix then evaluate (same algorithm as CheckCondition)
                expression = "(" + expression + ")";
                var opStack = new Stack<char>();
                var postfix = new StringBuilder();

                foreach (char c in expression)
                {
                    if      (c == '(') opStack.Push('(');
                    else if (c == '&' || c == '|')
                    {
                        postfix.Append(',');
                        if (opStack.Count > 1 && opStack.Peek() != '(')
                            postfix.Append(opStack.Pop()).Append(',');
                        opStack.Push(c);
                    }
                    else if (c == ')')
                    {
                        while (opStack.Peek() != '(')
                            postfix.Append(',').Append(opStack.Pop());
                        opStack.Pop();
                    }
                    else postfix.Append(c);
                }

                var boolStack = new Stack<bool>();
                foreach (string token in postfix.ToString().Split(','))
                {
                    if      (token == "&") boolStack.Push(boolStack.Pop() & boolStack.Pop());
                    else if (token == "|") boolStack.Push(boolStack.Pop() | boolStack.Pop());
                    else if (!string.IsNullOrEmpty(token))
                        boolStack.Push(EvaluateSimple(token, row, questions, attrs));
                }

                return boolStack.Count > 0 && boolStack.Pop();
            }
            catch { return false; }
        }

        private bool EvaluateSimple(string element, DataRow row,
            Dictionary<string, QuestionInfo> questions,
            Dictionary<string, List<AttribInfo>> attrs)
        {
            element = element.Trim();

            // Parse: left OPERATOR right
            string left, right, op;
            if      (element.Contains("<=")) { op = "<="; SplitAt(element, "<=", out left, out right); }
            else if (element.Contains(">=")) { op = ">="; SplitAt(element, ">=", out left, out right); }
            else if (element.Contains("!=")) { op = "!="; SplitAt(element, "!=", out left, out right); }
            else if (element.Contains("==")) { op = "=";  SplitAt(element, "==", out left, out right); }
            else if (element.Contains("<"))  { op = "<";  SplitAt(element, "<",  out left, out right); }
            else if (element.Contains(">"))  { op = ">";  SplitAt(element, ">",  out left, out right); }
            else if (element.Contains("="))  { op = "=";  SplitAt(element, "=",  out left, out right); }
            else return false;

            left  = left.Trim();
            right = right.Trim();
            if (string.IsNullOrEmpty(left)) return false;

            if (!questions.TryGetValue(left, out var q)) return false;

            // Multi-choice: check if specific attribute value is selected
            if (q.ResponseType == "2")
            {
                if (!attrs.TryGetValue(left, out var attrList)) return false;
                var matchAttr = attrList.FirstOrDefault(a =>
                    string.Equals(a.AttributeValue, right, StringComparison.OrdinalIgnoreCase));
                if (matchAttr == null) return false;

                string colName = left + "_" + matchAttr.AttributeOrder;
                bool isSelected = row.Table.Columns.Contains(colName)
                    && row[colName]?.ToString() == "1";

                if (op == "=" || op == "==") return isSelected;
                if (op == "!="             ) return !isSelected;
                return false;
            }

            // Single-choice / NPS / other: compare column value directly
            if (!row.Table.Columns.Contains(left)) return false;
            string actual = row[left]?.ToString().Trim() ?? "";
            return CompareValues(actual, right, op);
        }

        private static void SplitAt(string s, string sep, out string left, out string right)
        {
            int idx = s.IndexOf(sep, StringComparison.Ordinal);
            left  = s.Substring(0, idx);
            right = s.Substring(idx + sep.Length);
        }

        private static bool CompareValues(string actual, string expected, string op)
        {
            int a = 0, b = 0;
            bool numeric = int.TryParse(actual, out a) && int.TryParse(expected, out b);
            switch (op)
            {
                case "=":  case "==": return numeric ? a == b : actual == expected;
                case "!=":            return numeric ? a != b : actual != expected;
                case "<":             return numeric && a < b;
                case ">":             return numeric && a > b;
                case "<=":            return numeric && a <= b;
                case ">=":            return numeric && a >= b;
                default:              return false;
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  EXPORT TO EXCEL
        // ══════════════════════════════════════════════════════════════════════

        private void ExportResults(string filePath, List<CheckResultItem> results)
        {
            Excel.Application xlApp = null;
            Excel.Workbook    xlWb  = null;
            Excel.Worksheet   xlWs  = null;
            try
            {
                xlApp = new Excel.Application { Visible = false, DisplayAlerts = false };
                xlWb  = xlApp.Workbooks.Add();
                xlWs  = (Excel.Worksheet)xlWb.Sheets[1];
                xlWs.Name = "Check Results";

                // Headers
                string[] headers = { "Respondent ID", "Q ID", "Check Type", "Issue" };
                for (int c = 0; c < headers.Length; c++)
                    xlWs.Cells[1, c + 1] = headers[c];

                // Style header row
                var hdrRange = (Excel.Range)xlWs.Range["A1:D1"];
                hdrRange.Font.Bold        = true;
                hdrRange.Font.Color       = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                hdrRange.Interior.Color   = System.Drawing.ColorTranslator.ToOle(
                    System.Drawing.Color.FromArgb(37, 99, 174));

                // Data rows — write in bulk via array for speed
                if (results.Count > 0)
                {
                    object[,] data = new object[results.Count, 4];
                    for (int i = 0; i < results.Count; i++)
                    {
                        data[i, 0] = results[i].RespondentId;
                        data[i, 1] = results[i].QId;
                        data[i, 2] = results[i].CheckType;
                        data[i, 3] = results[i].Issue;
                    }
                    var dataRange = (Excel.Range)xlWs.Range[
                        xlWs.Cells[2, 1],
                        xlWs.Cells[results.Count + 1, 4]];
                    dataRange.Value2 = data;
                }

                // Auto-fit columns
                ((Excel.Range)xlWs.Columns["A:D"]).AutoFit();

                // Freeze top row
                xlApp.ActiveWindow.SplitRow = 1;
                xlApp.ActiveWindow.FreezePanes = true;

                xlWb.SaveAs(filePath,
                    Excel.XlFileFormat.xlOpenXMLWorkbook,
                    Type.Missing, Type.Missing, false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            finally
            {
                if (xlWs  != null) StaticClass.releaseObject(xlWs);
                if (xlWb  != null) { xlWb.Close(false); StaticClass.releaseObject(xlWb); }
                if (xlApp != null) { xlApp.Quit();       StaticClass.releaseObject(xlApp); }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UI HELPERS
        // ══════════════════════════════════════════════════════════════════════

        private void Log(string msg)
        {
            Dispatcher.InvokeAsync(() =>
            {
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}]  {msg}\n");
                txtLog.ScrollToEnd();
            });
        }

        private void SetBusy(bool busy)
        {
            btnProcess.IsEnabled = !busy;
            btnCancel.IsEnabled  =  busy;
            cmbProject.IsEnabled = !busy;
            btnBrowse.IsEnabled  = !busy;
            if (!busy) prgProgress.Value = 0;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  STATIC HELPERS
        // ══════════════════════════════════════════════════════════════════════

        private static CheckResultItem Make(string rid, string qid, string type, string issue) =>
            new CheckResultItem { RespondentId = rid, QId = qid, CheckType = type, Issue = issue };

        private static string FindColumn(DataTable dt, params string[] candidates)
        {
            foreach (string c in candidates)
                if (dt.Columns.Contains(c)) return c;
            return null;
        }

        private static int ToInt(object val)
        {
            if (val == null) return 0;
            return int.TryParse(val.ToString(), out int r) ? r : 0;
        }

        private static bool ToBool(object val)
        {
            if (val == null) return false;
            string s = val.ToString().Trim();
            return s == "1"
                || s.Equals("true", StringComparison.OrdinalIgnoreCase)
                || s.Equals("yes",  StringComparison.OrdinalIgnoreCase);
        }
    }
}
