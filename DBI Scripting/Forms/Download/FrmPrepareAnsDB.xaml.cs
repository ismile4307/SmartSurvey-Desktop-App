using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmPrepareAnsDB.xaml
    /// </summary>
    public partial class FrmPrepareAnsDB : Window
    {
        Dictionary<string, string> dicProjectNameVsCode;
        Dictionary<string, string> dicProjectNameVsDbName;
        List<string> listOfRespondentId;

        private string myPath;
        private int myCounter;

        private DataTable _dt1, _dt2, _dt3;
        private long _globalId;
        private List<string> _sourceFiles;

        public FrmPrepareAnsDB()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            listOfRespondentId = new List<string>();
            myCounter = 0;
            this.getProjectsFromServer();
        }

        private async void getProjectsFromServer()
        {
            try
            {
                await DoWorkAsync();

                dicProjectNameVsCode    = new Dictionary<string, string>();
                dicProjectNameVsDbName  = new Dictionary<string, string>();

                DownloadClass myDownloadClass = new DownloadClass();
                List<ProjectInfo> listOfProjectInfo = myDownloadClass.getProjectInfoFromServer();

                comProjectName.Items.Clear();
                for (int i = 0; i < listOfProjectInfo.Count; i++)
                {
                    string projectName = listOfProjectInfo[i].ProjectName;
                    comProjectName.Items.Add(projectName);
                    dicProjectNameVsCode.Add(projectName,   listOfProjectInfo[i].ProjectCode);
                    dicProjectNameVsDbName.Add(projectName, listOfProjectInfo[i].DatabaseName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task DoWorkAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
        }

        // ─── Browse button ────────────────────────────────────────────────────

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = Properties.Settings.Default.StartupPath;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Script File (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    myPath = Path.GetDirectoryName(openFileDialog1.FileName);
                    txtAnsDBPath.Text = myPath;
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                    this.loadDBList();
                }
                else
                    txtAnsDBPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadDBList()
        {
            chkListBoxDBName.Items.Clear();
            string[] dbFiles = Directory.GetFiles(myPath, "*.db");
            foreach (string file in dbFiles)
                chkListBoxDBName.Items.Add(Path.GetFileName(file));
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ─── CheckListBox events ──────────────────────────────────────────────

        private void chkListBoxRespondentId_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            myCounter = 0;
            listOfRespondentId.Clear();
            foreach (var item in chkListBoxDBName.SelectedItems)
            {
                listOfRespondentId.Add(item.ToString());
                myCounter++;
            }
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked == true)
            {
                foreach (var item in chkListBoxDBName.Items)
                    chkListBoxDBName.SelectedItems.Add(item);
            }
            else
            {
                foreach (var item in chkListBoxDBName.Items)
                    chkListBoxDBName.SelectedItems.Remove(item);
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        // ─── Export to Excel ──────────────────────────────────────────────────

        private async void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            string excelName = txtOutputExcelName.Text.Trim();
            if (excelName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                excelName = excelName.Substring(0, excelName.Length - 5);

            string savePath = Path.Combine(myPath, excelName + ".xlsx");

            btnExportToExcel.IsEnabled = false;
            try
            {

            string projectCode = dicProjectNameVsCode[comProjectName.Text];
            string dbName      = dicProjectNameVsDbName[comProjectName.Text];
            string scriptPath  = Path.Combine(Path.GetTempPath(), dbName);

            // ── Download script from server ───────────────────────────────────
            try
            {
                lblProgress.Content = "Downloading project script from server...";
                DoEvents();
                await DownloadScriptAsync(dbName, scriptPath);
                lblProgress.Content = "Script downloaded.";
                DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download project script:\n" + ex.Message);
                lblProgress.Content = "Script download failed.";
                return;
            }

            // ── Collect data from selected .db files ──────────────────────────
            var selectedFiles = new List<string>();
            foreach (var item in chkListBoxDBName.SelectedItems)
                selectedFiles.Add(item.ToString());

            InitDataTables();
            var seenIds = new HashSet<string>();
            _globalId = 0;

            for (int i = 0; i < selectedFiles.Count; i++)
            {
                string dbPath = Path.Combine(myPath, selectedFiles[i]);
                lblProgress.Content = "Reading " + (i + 1) + " of " + selectedFiles.Count + ": " + selectedFiles[i];
                DoEvents();
                CollectDataFromDb(dbPath, projectCode, seenIds, selectedFiles[i]);
            }

            if (_dt1.Rows.Count == 0)
            {
                MessageBox.Show("No qualifying interviews found (Intv_Type=1, Status=1) for the selected project.");
                lblProgress.Content = "No data found.";
                return;
            }

            // ── Build column structure from downloaded script ──────────────────
            lblProgress.Content = "Building column structure...";
            DoEvents();

            var sql = new SQLite(scriptPath);
            sql.connect();
            List<string> columns = sql.getTableColumnReport();

            // ── Pivot data ────────────────────────────────────────────────────
            lblProgress.Content = "Pivoting data (" + _dt1.Rows.Count + " interview(s))...";
            DoEvents();

            List<List<string>> data = sql.getTableDataReport(columns, _dt1, _dt2, _dt3, null);

            if (data == null || data.Count == 0)
            {
                MessageBox.Show("No data could be built. Please check the database structure.");
                lblProgress.Content = "Export failed.";
                return;
            }

            // ── Append source file column ─────────────────────────────────────
            columns.Add("Source_DB");
            for (int i = 0; i < data.Count; i++)
                data[i].Add(i < _sourceFiles.Count ? _sourceFiles[i] : "");

            // ── Export ────────────────────────────────────────────────────────
            lblProgress.Content = "Exporting to Excel...";
            DoEvents();

            ExportToExcel(columns, data, savePath);

            lblProgress.Content = "Done — " + _dt1.Rows.Count + " interview(s) exported.";
            MessageBox.Show("Export complete.\n" + _dt1.Rows.Count + " interview(s) exported to:\n" + savePath,
                "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                btnExportToExcel.IsEnabled = true;
            }
        }

        private async Task DownloadScriptAsync(string dbName, string scriptPath)
        {
            if (File.Exists(scriptPath)) File.Delete(scriptPath);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol  = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (var wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(
                    StaticClass.SERVER_URL + "/scripts/" + dbName, scriptPath);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(comProjectName.Text))
            { MessageBox.Show("Please select a project."); return false; }
            if (string.IsNullOrEmpty(txtAnsDBPath.Text))
            { MessageBox.Show("Please browse to the database folder."); return false; }
            if (chkListBoxDBName.SelectedItems.Count == 0)
            { MessageBox.Show("Please select at least one database file."); return false; }
            if (string.IsNullOrWhiteSpace(txtOutputExcelName.Text))
            { MessageBox.Show("Please enter an output Excel file name."); return false; }
            return true;
        }

        // ─── Data collection ─────────────────────────────────────────────────

        private void InitDataTables()
        {
            _sourceFiles = new List<string>();
            _dt1 = new DataTable();
            _dt1.Columns.Add("id",               typeof(string));
            _dt1.Columns.Add("respondent_id",     typeof(string));
            _dt1.Columns.Add("name_resp",         typeof(string));
            _dt1.Columns.Add("mobile_resp",       typeof(string));
            _dt1.Columns.Add("latitude",          typeof(string));
            _dt1.Columns.Add("longitude",         typeof(string));
            _dt1.Columns.Add("survey_start_at",   typeof(string));
            _dt1.Columns.Add("survey_end_at",     typeof(string));
            _dt1.Columns.Add("length_of_intv",    typeof(string));
            _dt1.Columns.Add("intv_type",         typeof(string));
            _dt1.Columns.Add("fi_code",           typeof(string));
            _dt1.Columns.Add("fs_code",           typeof(string));
            _dt1.Columns.Add("accompanied_by",    typeof(string));
            _dt1.Columns.Add("back_checked_by",   typeof(string));
            _dt1.Columns.Add("script_version",    typeof(string));
            _dt1.Columns.Add("created_at",        typeof(string));
            _dt1.Columns.Add("status",            typeof(string));
            _dt1.Columns.Add("field_ex2",         typeof(string));
            _dt1.Columns.Add("intv_info9",        typeof(string));
            _dt1.Columns.Add("tab_id",            typeof(string));

            _dt2 = new DataTable();
            _dt2.Columns.Add("interview_info_id", typeof(string));
            _dt2.Columns.Add("q_id",              typeof(string));
            _dt2.Columns.Add("response",          typeof(string));
            _dt2.Columns.Add("q_elapsed_time",    typeof(string));
            _dt2.Columns.Add("q_order",           typeof(string));
            _dt2.Columns.Add("resp_order",        typeof(string));

            _dt3 = new DataTable();
            _dt3.Columns.Add("interview_info_id", typeof(string));
            _dt3.Columns.Add("respondent_id",     typeof(string));
            _dt3.Columns.Add("q_id",              typeof(string));
            _dt3.Columns.Add("attribute_value",   typeof(string));
            _dt3.Columns.Add("response",          typeof(string));
            _dt3.Columns.Add("response_type",     typeof(string));
        }

        private void CollectDataFromDb(string dbPath, string projectCode, HashSet<string> seenIds, string sourceFileName)
        {
            if (!File.Exists(dbPath)) return;

            try
            {
                using (var conn = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;"))
                {
                    conn.Open();

                    // ── T_InterviewInfo ──────────────────────────────────────
                    var cmd1 = new SQLiteCommand(
                        "SELECT rowid AS local_id, RespondentId, " +
                        "COALESCE(NameResp,'') AS NameResp, COALESCE(MobileResp,'') AS MobileResp, " +
                        "COALESCE(Latitude,'') AS Latitude, COALESCE(Longitude,'') AS Longitude, " +
                        "COALESCE(SurveyDateTime,'') AS SurveyDateTime, COALESCE(SurveyEndTime,'') AS SurveyEndTime, " +
                        "COALESCE(LengthOfIntv,'') AS LengthOfIntv, COALESCE(Intv_Type,'') AS Intv_Type, " +
                        "COALESCE(FICode,'') AS FICode, COALESCE(FSCode,'') AS FSCode, " +
                        "COALESCE(AccompaniedBy,'') AS AccompaniedBy, COALESCE(BackCheckedBy,'') AS BackCheckedBy, " +
                        "COALESCE(ScriptVersion,'') AS ScriptVersion, COALESCE(Status,'') AS Status, " +
                        "COALESCE(FieldExtra2,'') AS FieldExtra2, COALESCE(IntvInfo9,'') AS IntvInfo9, " +
                        "COALESCE(TabId,'') AS TabId " +
                        "FROM T_InterviewInfo " +
                        "WHERE ProjectId=" + projectCode + " AND Intv_Type='1' AND Status='1'", conn);

                    var tempDt1 = new DataTable();
                    new SQLiteDataAdapter(cmd1).Fill(tempDt1);

                    // Map this file's rowid → global id (for answer/OE linking)
                    var localIdToGlobalId = new Dictionary<string, string>();
                    var newRespondentIds  = new List<string>();

                    foreach (DataRow row in tempDt1.Rows)
                    {
                        string respId  = row["RespondentId"].ToString();
                        string localId = row["local_id"].ToString();

                        if (seenIds.Contains(respId)) continue;  // duplicate across files — skip

                        seenIds.Add(respId);
                        _globalId++;
                        string gId = _globalId.ToString();
                        localIdToGlobalId[localId] = gId;
                        newRespondentIds.Add(respId);

                        DataRow r = _dt1.NewRow();
                        r["id"]             = gId;
                        r["respondent_id"]  = respId;
                        r["name_resp"]      = row["NameResp"];
                        r["mobile_resp"]    = row["MobileResp"];
                        r["latitude"]       = row["Latitude"];
                        r["longitude"]      = row["Longitude"];
                        r["survey_start_at"]= row["SurveyDateTime"];
                        r["survey_end_at"]  = row["SurveyEndTime"];
                        r["length_of_intv"] = row["LengthOfIntv"];
                        r["intv_type"]      = row["Intv_Type"];
                        r["fi_code"]        = row["FICode"];
                        r["fs_code"]        = row["FSCode"];
                        r["accompanied_by"] = row["AccompaniedBy"];
                        r["back_checked_by"]= row["BackCheckedBy"];
                        r["script_version"] = row["ScriptVersion"];
                        r["created_at"]     = "";
                        r["status"]         = row["Status"];
                        r["field_ex2"]      = row["FieldExtra2"];
                        r["intv_info9"]     = row["IntvInfo9"];
                        r["tab_id"]         = row["TabId"];
                        _dt1.Rows.Add(r);
                        _sourceFiles.Add(sourceFileName);
                    }

                    if (newRespondentIds.Count == 0) return;

                    string inClause = string.Join(",",
                        newRespondentIds.Select(id => "'" + id.Replace("'", "''") + "'"));

                    // ── T_RespAnswer ─────────────────────────────────────────
                    // JOIN back to T_InterviewInfo to get the rowid (local_id) for linking
                    var cmd2 = new SQLiteCommand(
                        "SELECT TI.rowid AS local_id, TR.QId, " +
                        "COALESCE(TR.Response,'') AS Response, " +
                        "COALESCE(TR.qElapsedTime,'') AS qElapsedTime, " +
                        "COALESCE(CAST(TR.qOrderTag AS TEXT),'0') AS qOrderTag, " +
                        "COALESCE(CAST(TR.rOrderTag AS TEXT),'0') AS rOrderTag " +
                        "FROM T_RespAnswer TR " +
                        "INNER JOIN T_InterviewInfo TI " +
                        "  ON TR.RespondentId = TI.RespondentId AND TR.ProjectId = TI.ProjectId " +
                        "WHERE TR.ProjectId=" + projectCode +
                        "  AND TI.Intv_Type='1' AND TI.Status='1' " +
                        "  AND TR.RespondentId IN (" + inClause + ")", conn);

                    var tempDt2 = new DataTable();
                    new SQLiteDataAdapter(cmd2).Fill(tempDt2);

                    foreach (DataRow row in tempDt2.Rows)
                    {
                        string localId = row["local_id"].ToString();
                        if (!localIdToGlobalId.ContainsKey(localId)) continue;

                        DataRow r = _dt2.NewRow();
                        r["interview_info_id"] = localIdToGlobalId[localId];
                        r["q_id"]              = row["QId"];
                        r["response"]          = row["Response"];
                        r["q_elapsed_time"]    = row["qElapsedTime"];
                        r["q_order"]           = row["qOrderTag"];
                        r["resp_order"]        = row["rOrderTag"];
                        _dt2.Rows.Add(r);
                    }

                    // ── T_RespOpenended ──────────────────────────────────────
                    try
                    {
                        var cmd3 = new SQLiteCommand(
                            "SELECT TI.rowid AS local_id, TE.RespondentId, TE.QId, " +
                            "COALESCE(TE.AttributeValue,'') AS AttributeValue, " +
                            "COALESCE(TE.OpenendedResp,'') AS OpenendedResp, " +
                            "COALESCE(TE.OEResponseType,'') AS OEResponseType " +
                            "FROM T_RespOpenended TE " +
                            "INNER JOIN T_InterviewInfo TI " +
                            "  ON TE.RespondentId = TI.RespondentId AND TE.ProjectId = TI.ProjectId " +
                            "WHERE TE.ProjectId=" + projectCode +
                            "  AND TI.Intv_Type='1' AND TI.Status='1' " +
                            "  AND TE.RespondentId IN (" + inClause + ")", conn);

                        var tempDt3 = new DataTable();
                        new SQLiteDataAdapter(cmd3).Fill(tempDt3);

                        foreach (DataRow row in tempDt3.Rows)
                        {
                            string localId = row["local_id"].ToString();
                            if (!localIdToGlobalId.ContainsKey(localId)) continue;

                            DataRow r = _dt3.NewRow();
                            r["interview_info_id"] = localIdToGlobalId[localId];
                            r["respondent_id"]     = row["RespondentId"];
                            r["q_id"]              = row["QId"];
                            r["attribute_value"]   = row["AttributeValue"];
                            r["response"]          = row["OpenendedResp"];
                            r["response_type"]     = row["OEResponseType"];
                            _dt3.Rows.Add(r);
                        }
                    }
                    catch
                    {
                        // T_RespOpenended may not exist in all db files — skip silently
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading " + Path.GetFileName(dbPath) + ": " + ex.Message);
            }
        }

        // ─── Excel export ─────────────────────────────────────────────────────

        private void ExportToExcel(List<string> columns, List<List<string>> data, string savePath)
        {
            Excel.Application xlApp  = null;
            Excel.Workbook    xlBook = null;
            object miss = System.Reflection.Missing.Value;
            try
            {
                xlApp  = new Excel.Application();
                xlBook = xlApp.Workbooks.Add(miss);

                // Sheet 1 — Open-ended
                var wsOE = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);
                wsOE.Name = "Openended";
                WriteOeSheet(wsOE);

                // Sheet 2 — Main data
                var wsData = (Excel.Worksheet)xlBook.Worksheets.Add(xlBook.Worksheets[1]);
                wsData.Name = "Data";
                WriteDataSheet(wsData, columns, data);

                xlBook.SaveAs(savePath, Excel.XlFileFormat.xlWorkbookDefault);
            }
            finally
            {
                xlBook?.Close(true, miss, miss);
                xlApp?.Quit();
                if (xlBook != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlBook);
                if (xlApp  != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        private void WriteOeSheet(Excel.Worksheet ws)
        {
            ws.Cells[1, 1] = "Respondent Id";
            ws.Cells[1, 2] = "QId";
            ws.Cells[1, 3] = "Attribute Value";
            ws.Cells[1, 4] = "OE Verbatim";

            int row = 2;
            foreach (DataRow dr in _dt3.Rows)
            {
                ws.Cells[row, 1] = "'" + dr["respondent_id"];
                ws.Cells[row, 2] = "'" + dr["q_id"];
                ws.Cells[row, 3] = "'" + dr["attribute_value"];
                ws.Cells[row, 4] = "'" + Clean(dr["response"].ToString());
                row++;
            }
            ws.Columns.AutoFit();
        }

        private void WriteDataSheet(Excel.Worksheet ws, List<string> columns, List<List<string>> data)
        {
            for (int i = 0; i < columns.Count; i++)
                ws.Cells[1, i + 1] = "'" + columns[i];

            int totalRows = data.Count;
            int totalCols = totalRows > 0 ? data[0].Count : columns.Count;
            const int batchSize = 500;

            for (int rowStart = 0; rowStart < totalRows; rowStart += batchSize)
            {
                int batch = Math.Min(batchSize, totalRows - rowStart);
                var arr = new object[batch, totalCols];
                for (int i = 0; i < batch; i++)
                    for (int j = 0; j < totalCols; j++)
                        arr[i, j] = "'" + Clean(data[rowStart + i][j]);

                var startCell = (Excel.Range)ws.Cells[rowStart + 2, 1];
                var range = startCell.get_Resize(batch, totalCols);
                range.Value2 = arr;

                System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(startCell);

                lblProgress.Content = "Writing rows " + (rowStart + batch) + " / " + totalRows + "...";
                DoEvents();
            }
            ws.Columns.AutoFit();
        }

        private static string Clean(string s)
            => s.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
    }
}
