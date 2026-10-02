using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmCTableLink2.xaml
    ///
    /// Builds a hyperlinked table of contents ("Index" sheet) for a crosstab workbook.
    /// All Excel work happens in ONE Excel instance on a background STA thread:
    /// columns A:B are read in a single call, the whole layout is planned in memory,
    /// and row inserts / cell writes / formatting are pushed back in batches.
    /// The workbook is only saved when the whole run succeeds.
    /// </summary>
    public partial class FrmCTableLink2 : Window
    {
        private const string DefaultTableSheet = "Table";
        private const string DefaultIndexSheet = "Index";
        private const string StandardFormat = "Standard Format";
        private const string GeneralFormat = "General Format";

        private string projectName;
        private bool isRunning;

        public FrmCTableLink2()
        {
            InitializeComponent();
            Closing += (s, e) =>
            {
                if (isRunning)
                {
                    e.Cancel = true;
                    MessageBox.Show("Please wait until the current run finishes.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRowNo.Text = "2";
            txtColumnNo.Text = "2";

            radioTableIndex.IsChecked = true;

            comLinkFormat.Items.Add(GeneralFormat);
            comLinkFormat.Items.Add(StandardFormat);
            comLinkFormat.Text = StandardFormat;

            comPreparedBy.Items.Add("Arrowhead Research Pvt. Ltd.");
            comPreparedBy.Items.Add("SmartSurveyBD Pvt. Ltd.");
            comPreparedBy.Items.Add("DBI Research Private Ltd.");

            string saved = Properties.Settings.Default.PreparedBy;
            comPreparedBy.Text = string.IsNullOrWhiteSpace(saved) ? "Arrowhead Research Pvt. Ltd." : saved;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = Properties.Settings.Default.StartupPath;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() != true)
                    return;

                string path = openFileDialog1.FileName;
                txtAnalysisExcelPath.Text = path;
                projectName = Path.GetFileNameWithoutExtension(path);

                Properties.Settings.Default.StartupPath = Path.GetDirectoryName(path);
                Properties.Settings.Default.Save();

                // Only READ the sheet names here — the file is not touched until Run.
                List<string> sheets = ReadSheetNames(path);
                txtTableSheetName.Text = GuessTableSheet(sheets);
                txtLinkSheetName.Text = DefaultIndexSheet;

                progressBar1.Value = 0;
                lblStatus.Text = sheets == null
                    ? "File selected. Sheet names will be checked when you press Run."
                    : $"File selected — {sheets.Count} sheet(s): {string.Join(", ", sheets)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>Reads worksheet names straight from the .xlsx package (no Excel needed). Returns null if unreadable.</summary>
        private static List<string> ReadSheetNames(string path)
        {
            try
            {
                using (Package pkg = Package.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    PackagePart part = pkg.GetPart(new Uri("/xl/workbook.xml", UriKind.Relative));
                    using (Stream s = part.GetStream(FileMode.Open, FileAccess.Read))
                    {
                        XDocument doc = XDocument.Load(s);
                        XNamespace ns = doc.Root.Name.Namespace;
                        return doc.Root.Element(ns + "sheets").Elements(ns + "sheet")
                                  .Select(x => (string)x.Attribute("name"))
                                  .ToList();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static string GuessTableSheet(List<string> sheets)
        {
            if (sheets == null || sheets.Contains(DefaultTableSheet) || sheets.Contains("Sheet1"))
                return DefaultTableSheet;   // "Sheet1" is renamed to "Table" on Run
            return sheets.FirstOrDefault(s => s != DefaultIndexSheet) ?? "";
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            CTableLinkOptions options = ReadOptions();
            if (options == null)
                return;

            Properties.Settings.Default.PreparedBy = options.PreparedBy;
            Properties.Settings.Default.Save();

            SetBusy(true);
            try
            {
                var builder = new CTableLinkBuilder(options, ReportProgress);
                int tableCount = await RunOnStaThread(builder.Run);

                progressBar1.Value = progressBar1.Maximum;
                lblStatus.Text = $"Complete — {tableCount} table(s) indexed successfully.";
                MessageBox.Show("Index for table has been created successfully", Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CTableLinkException ex)
            {
                progressBar1.Value = 0;
                lblStatus.Text = ex.Message;
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;
                lblStatus.Text = "Failed: " + ex.Message;
                MessageBox.Show("The operation failed and no changes were saved to the file.\n\n" + ex.Message,
                                Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private CTableLinkOptions ReadOptions()
        {
            string path = txtAnalysisExcelPath.Text.Trim();
            if (path.Length == 0 || !File.Exists(path))
            {
                MessageBox.Show("Please browse and select an Excel file first.");
                return null;
            }

            bool standard = comLinkFormat.Text != GeneralFormat;

            int startRow, startColumn;
            int minRow = standard ? 2 : 1;
            if (!int.TryParse(txtRowNo.Text.Trim(), out startRow) || startRow < minRow || startRow > 1000)
            {
                MessageBox.Show($"Index Start Row must be a whole number between {minRow} and 1000.");
                txtRowNo.Focus();
                return null;
            }
            if (!int.TryParse(txtColumnNo.Text.Trim(), out startColumn) || startColumn < 1 || startColumn > 100)
            {
                MessageBox.Show("Index Start Column must be a whole number between 1 and 100.");
                txtColumnNo.Focus();
                return null;
            }

            string tableSheet = txtTableSheetName.Text.Trim();
            string indexSheet = txtLinkSheetName.Text.Trim();
            if (tableSheet.Length == 0 || indexSheet.Length == 0)
            {
                MessageBox.Show("Please enter both the Table sheet name and the Link sheet name.");
                return null;
            }
            if (string.Equals(tableSheet, indexSheet, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Table sheet and Link sheet must be different sheets.");
                return null;
            }

            if (IsFileLocked(path))
            {
                MessageBox.Show("The file is open in another program (probably Excel).\nPlease close it and press Run again.",
                                Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            return new CTableLinkOptions
            {
                FilePath = path,
                ProjectName = projectName ?? Path.GetFileNameWithoutExtension(path),
                TableSheet = tableSheet,
                IndexSheet = indexSheet,
                PreparedBy = comPreparedBy.Text.Trim(),
                StartRow = startRow,
                StartColumn = startColumn,
                StandardFormat = standard,
                LinkOnTitle = radioTableTitle.IsChecked == true
            };
        }

        private static bool IsFileLocked(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        private void SetBusy(bool busy)
        {
            isRunning = busy;
            btnRun.IsEnabled = !busy;
            btnBrowseExcel.IsEnabled = !busy;
            btnClose.IsEnabled = !busy;
            comPreparedBy.IsEnabled = !busy;
            txtTableSheetName.IsEnabled = !busy;
            txtLinkSheetName.IsEnabled = !busy;
            txtRowNo.IsEnabled = !busy;
            txtColumnNo.IsEnabled = !busy;
            comLinkFormat.IsEnabled = !busy;
            radioTableTitle.IsEnabled = !busy;
            radioTableIndex.IsEnabled = !busy;
            Cursor = busy ? Cursors.Wait : null;
            if (busy)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Value = 0;
            }
        }

        private void ReportProgress(string text, double percent)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblStatus.Text = text;
                progressBar1.Value = Math.Max(0, Math.Min(100, percent));
            }));
        }

        /// <summary>Excel interop is happiest on an STA thread; this keeps the UI responsive while it runs.</summary>
        private static Task<T> RunOnStaThread<T>(Func<T> work)
        {
            var tcs = new TaskCompletionSource<T>();
            var thread = new Thread(() =>
            {
                try { tcs.SetResult(work()); }
                catch (Exception ex) { tcs.SetException(ex); }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }

        // =====================================================================
        //  Worker
        // =====================================================================

        private sealed class CTableLinkOptions
        {
            public string FilePath, ProjectName, TableSheet, IndexSheet, PreparedBy;
            public int StartRow, StartColumn;
            public bool StandardFormat, LinkOnTitle;
        }

        /// <summary>A user-facing problem (bad input, nothing to do). Nothing is saved.</summary>
        private sealed class CTableLinkException : Exception
        {
            public CTableLinkException(string message) : base(message) { }
        }

        private sealed class TableInfo
        {
            public int TitleRow;        // original row of "Table N: ..."
            public string Title;
            public int FilterRow;       // original row of first "Base : ..." (0 = none)
            public string Filter;
            public bool HasBase;
            public object Base;         // column B of the first Base/Total row
            public int HomeRow;         // original row of the last "Home" inside this table (0 = none)
            public int LinkRow;         // final row the Index hyperlink jumps to
        }

        private sealed class CTableLinkBuilder
        {
            private const int TitleMergeColumns = 21;       // A:U, as before
            private const int MaxAddressLength = 250;       // Excel's Range("a,b,c") limit is 255
            private const int MaxFormulaText = 255;         // string-literal limit inside a formula

            private readonly CTableLinkOptions opt;
            private readonly Action<string, double> progress;
            private int lastReportTick;

            private Excel.Application app;
            private Excel.Workbook wb;

            public CTableLinkBuilder(CTableLinkOptions options, Action<string, double> progress)
            {
                opt = options;
                this.progress = progress;
            }

            public int Run()
            {
                try
                {
                    Report("Opening workbook...", 2, true);
                    app = new Excel.Application();
                    app.Visible = false;
                    app.DisplayAlerts = false;
                    app.ScreenUpdating = false;
                    app.EnableEvents = false;

                    wb = app.Workbooks.Open(opt.FilePath, 0, false);

                    Excel.XlCalculation? oldCalc = null;
                    try { oldCalc = app.Calculation; app.Calculation = Excel.XlCalculation.xlCalculationManual; }
                    catch { /* protected / odd workbooks: just leave calculation alone */ }

                    int count = Build();

                    // Calculation mode is stored in the file — restore it before saving.
                    if (oldCalc.HasValue)
                        try { app.Calculation = oldCalc.Value; } catch { }

                    Report("Saving file...", 96, true);
                    wb.Save();
                    return count;
                }
                finally
                {
                    Cleanup();
                }
            }

            private void Cleanup()
            {
                if (wb != null)
                {
                    try { wb.Close(false); } catch { }
                    try { Marshal.ReleaseComObject(wb); } catch { }
                    wb = null;
                }
                if (app != null)
                {
                    try { app.Quit(); } catch { }
                    try { Marshal.ReleaseComObject(app); } catch { }
                    app = null;
                }
                // Release any remaining RCWs so EXCEL.EXE actually exits.
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            private int Build()
            {
                bool standard = opt.StandardFormat;

                // ---------------------------------------------------------- sheets
                Excel.Worksheet tableWs = ResolveTableSheet();
                Excel.Worksheet indexWs = ResolveIndexSheet();

                // ---------------------------------------------------------- read A:B in one call
                Report("Reading table sheet...", 5, true);
                Excel.Range used = tableWs.UsedRange;
                int lastRow = used.Row + used.Rows.Count - 1;
                object[,] ab = (object[,])tableWs.Range["A1", "B" + lastRow].Value2;

                string[] text = new string[lastRow + 2];
                bool[] dummy = new bool[lastRow + 2];
                var sigRows = new List<int>();
                for (int r = 1; r <= lastRow; r++)
                {
                    object v = ab[r, 1];
                    if (v == null) continue;
                    string s = Convert.ToString(v, CultureInfo.CurrentCulture).Replace("\"", "");
                    text[r] = s;
                    if (s.Trim() == "DUMMY ROW") dummy[r] = true;
                    else if (s == "S.TEST" || s == "SIG. TEST") sigRows.Add(r);
                }
                bool hasSigTest = sigRows.Count > 0;

                // Effective content after DUMMY ROWs are cleared.
                Func<int, string> eff = r => (r < 1 || r > lastRow || dummy[r]) ? null : text[r];
                Func<int, bool> hasValue = r => r >= 1 && r <= lastRow && !dummy[r] && ab[r, 1] != null;

                if (standard && text[1] != null && text[1].StartsWith("Project : "))
                    throw new CTableLinkException(
                        $"Sheet '{tableWs.Name}' has already been processed with Standard Format (it starts with \"Project : \").\n" +
                        "Please run it on the original table output file. Nothing was changed.");

                // ---------------------------------------------------------- find tables
                Report("Scanning tables...", 10, true);
                var tables = new List<TableInfo>();
                TableInfo cur = null;
                for (int r = 1; r <= lastRow; r++)
                {
                    string t = eff(r);
                    if (t == null) continue;

                    if (t.StartsWith("Table ") && (!standard || t.Contains(":")))
                    {
                        cur = new TableInfo { TitleRow = r, Title = ParseTitle(t) };
                        tables.Add(cur);
                    }
                    else if (cur == null)
                    {
                        continue;
                    }
                    else if (t.StartsWith("Base :"))
                    {
                        if (cur.FilterRow == 0) { cur.FilterRow = r; cur.Filter = t; }
                    }
                    else if (standard ? (t == "Total" || t == "Base") : IsGeneralBaseRow(t))
                    {
                        if (!cur.HasBase) { cur.HasBase = true; cur.Base = CleanValue(ab[r, 2]); }
                    }
                    else if (t == "Home")
                    {
                        cur.HomeRow = r;
                    }
                }

                if (tables.Count == 0)
                    throw new CTableLinkException(
                        $"No tables were found in sheet '{tableWs.Name}' (column A lines starting with \"Table \"). Nothing was changed.");

                // ---------------------------------------------------------- plan spacing rows (Standard)
                var inserts = new SortedDictionary<int, int>();   // original row -> rows inserted above it
                Action<int, int> addInsert = (row, n) =>
                {
                    int c;
                    inserts.TryGetValue(row, out c);
                    inserts[row] = c + n;
                };

                if (standard)
                {
                    int lastContentRow = lastRow;
                    while (lastContentRow > 0 && !hasValue(lastContentRow)) lastContentRow--;

                    addInsert(1, 5);   // project header block
                    for (int r = 1; r <= lastRow; r++)
                    {
                        string t = eff(r);
                        if (t == null) continue;

                        if (t == "Total")
                        {
                            if ((eff(r + 1) ?? "") != "Home") addInsert(r + 1, 1);
                        }
                        else if (t.StartsWith("Mean") || t.StartsWith("MEAN"))
                        {
                            // only when the row directly above is not already blank
                            if (!inserts.ContainsKey(r) && hasValue(r - 1)) addInsert(r, 1);
                        }
                        else if (t == "Detractors [0-6]") addInsert(r, 1);
                        else if (t == "Promoters [9-10]") addInsert(r + 1, 1);
                        else if (t == "TOP 2 BOX [5/4]" || t == "TOP 2 BOX [1/2]") addInsert(r, 1);
                        else if (t == "TOP 2 BOX [09/10]") addInsert(r, 1);
                        else if (t == "BOTTOM 2 BOX [1/2]" || t == "BOTTOM 2 BOX [4/5]") addInsert(r + 1, 1);
                        else if (t == "BOTTOM 3 BOX [01/02/03]") addInsert(r + 1, 1);
                        else if (t == "Home" && r < lastContentRow) addInsert(r + 1, 4);
                    }
                }

                // original row -> final row, and final row -> original row (0 = inserted blank row)
                int[] newRow = new int[lastRow + 2];
                int shift = 0;
                for (int r = 1; r <= lastRow + 1; r++)
                {
                    int c;
                    if (inserts.TryGetValue(r, out c)) shift += c;
                    newRow[r] = r + shift;
                }
                int finalLastRow = lastRow + shift;
                int[] origAt = new int[finalLastRow + 2];
                for (int r = 1; r <= lastRow; r++) origAt[newRow[r]] = r;

                // ---------------------------------------------------------- plan column-A writes (final rows)
                var writes = new SortedDictionary<int, object>();
                var labelRows = new List<int>();
                var titleRows = new List<int>();
                var filterRows = new List<int>();

                foreach (int r in sigRows) writes[newRow[r]] = "";

                int gridTop = standard ? opt.StartRow + 6 : opt.StartRow + 1;   // Index header row

                if (standard)
                {
                    writes[1] = "Project : " + opt.ProjectName;
                    for (int r = 1; r <= lastRow; r++)
                        if (eff(r) == "Total") writes[newRow[r]] = "Base";

                    for (int k = 1; k <= tables.Count; k++)
                    {
                        TableInfo tb = tables[k - 1];
                        int T = newRow[tb.TitleRow];
                        int F = tb.FilterRow > 0 ? newRow[tb.FilterRow] : T + 1;
                        int L = F - 4, R = F - 3, FR = F - 2;

                        if (L < 2)
                        {
                            // No room above the table for the red header — keep the title in place.
                            writes[T] = AsText("Table " + k + ": " + tb.Title);
                            tb.LinkRow = T;
                            continue;
                        }

                        writes[T] = "";
                        if (tb.FilterRow > 0) writes[F] = "";

                        writes[L] = "Table " + k;
                        writes[R] = AsText(tb.Title);
                        labelRows.Add(L);
                        titleRows.Add(R);
                        if (tb.Filter != null)
                        {
                            writes[FR] = AsText(tb.Filter);
                            filterRows.Add(FR);
                        }
                        tb.LinkRow = R;
                    }
                }
                else
                {
                    for (int k = 1; k <= tables.Count; k++)
                    {
                        TableInfo tb = tables[k - 1];
                        int T = newRow[tb.TitleRow];
                        writes[T] = AsText("Table " + k + ": " + tb.Title);
                        titleRows.Add(T);
                        tb.LinkRow = T;
                    }
                }

                // "Home" links: end of table k -> its row on the Index sheet
                for (int k = 1; k <= tables.Count; k++)
                {
                    TableInfo tb = tables[k - 1];
                    int home = 0;
                    if (tb.HomeRow > 0)
                    {
                        home = newRow[tb.HomeRow];
                    }
                    else if (k < tables.Count)
                    {
                        // Fallback to the old fixed offset, but only onto an empty cell of this table.
                        int c = newRow[tables[k].TitleRow] - (standard ? 6 : 2);
                        int o = c >= 1 && c <= finalLastRow ? origAt[c] : -1;
                        if (c > tb.LinkRow && c > newRow[tb.TitleRow] && o >= 0 && (o == 0 || !hasValue(o)) && !writes.ContainsKey(c))
                            home = c;
                    }
                    if (home > 0)
                        writes[home] = Hyperlink(opt.IndexSheet, CellName(gridTop + k, opt.StartColumn + 1), "Home");
                }

                // ---------------------------------------------------------- apply to table sheet
                if (inserts.Count > 0)
                    InsertRows(tableWs, inserts);

                var dummyAddrs = new List<string>();
                for (int r = 1; r <= lastRow; r++)
                    if (dummy[r]) dummyAddrs.Add(newRow[r] + ":" + newRow[r]);
                if (dummyAddrs.Count > 0)
                {
                    Report($"Clearing {dummyAddrs.Count} DUMMY ROW(s)...", 55, true);
                    ForEachBatch(tableWs, dummyAddrs, rng => rng.ClearContents());
                }

                Report("Writing table titles and links...", 60, true);
                WriteColumnA(tableWs, writes);

                Report("Formatting table sheet...", 70, true);
                if (standard)
                {
                    Excel.Range a1 = tableWs.Range["A1"];
                    a1.Font.Size = 14;
                    a1.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkBlue);
                    a1.Font.Bold = true;
                    // The old code set this through Cell.Style, i.e. on the workbook's Normal style — keep that look.
                    ((Excel.Style)a1.Style).VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    ForEachBatch(tableWs, labelRows.Select(r => "A" + r), rng =>
                    {
                        rng.Font.Size = 12;
                        rng.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
                        rng.Font.Italic = true;
                        rng.Font.Bold = true;
                    });
                    ForEachBatch(tableWs, titleRows.Select(r => "A" + r), rng =>
                    {
                        rng.Font.Size = 10;
                        rng.Font.Italic = true;
                        rng.Font.Bold = true;
                        rng.WrapText = false;
                    });
                    ForEachBatch(tableWs, filterRows.Select(r => "A" + r), rng =>
                    {
                        rng.Font.Size = 10;
                        rng.Font.Italic = true;
                        rng.WrapText = false;
                    });

                    string lastMergeCol = ColumnName(TitleMergeColumns);
                    ForEachBatch(tableWs, titleRows.Select(r => "A" + r + ":" + lastMergeCol + r), rng => rng.Merge());

                    ((Excel.Range)tableWs.Columns["A:A"]).ColumnWidth = 45;
                    tableWs.Range["1:" + finalLastRow].Rows.AutoFit();
                    HideGridlines(tableWs);
                }
                else
                {
                    tableWs.UsedRange.Font.Bold = false;
                    ForEachBatch(tableWs, titleRows.Select(r => "A" + r),
                                 rng => rng.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft);
                }

                // ---------------------------------------------------------- index sheet
                Report("Writing index sheet...", 85, true);
                if (standard)
                    WriteStandardIndex(indexWs, tables, gridTop, hasSigTest, tableWs.Name);
                else
                    WriteGeneralIndex(indexWs, tables, gridTop, tableWs.Name);

                HideGridlines(indexWs);   // also leaves the Index sheet active when the file is opened
                return tables.Count;
            }

            // ------------------------------------------------------------------ sheets

            private Excel.Worksheet FindSheet(string name)
            {
                foreach (Excel.Worksheet ws in wb.Worksheets)
                    if (string.Equals(ws.Name, name, StringComparison.OrdinalIgnoreCase))
                        return ws;
                return null;
            }

            private Excel.Worksheet ResolveTableSheet()
            {
                Excel.Worksheet ws = FindSheet(opt.TableSheet);
                if (ws == null && opt.TableSheet == DefaultTableSheet)
                {
                    ws = FindSheet("Sheet1");
                    if (ws != null) ws.Name = DefaultTableSheet;
                }
                if (ws == null)
                    throw new CTableLinkException($"Sheet '{opt.TableSheet}' was not found in the workbook. Nothing was changed.");
                return ws;
            }

            private Excel.Worksheet ResolveIndexSheet()
            {
                Excel.Worksheet ws = FindSheet(opt.IndexSheet);
                if (ws == null)
                {
                    ws = (Excel.Worksheet)wb.Worksheets.Add(Before: wb.Worksheets[1]);
                    ws.Name = opt.IndexSheet;
                }
                else
                {
                    // Start from a clean sheet so a re-run never leaves stale rows behind.
                    ws.Cells.Clear();
                    ws.Rows.RowHeight = ws.StandardHeight;
                }
                return ws;
            }

            private void HideGridlines(Excel.Worksheet ws)
            {
                try
                {
                    ws.Activate();
                    app.ActiveWindow.DisplayGridlines = false;
                }
                catch { /* cosmetic only */ }
            }

            // ------------------------------------------------------------------ index writers

            private void WriteStandardIndex(Excel.Worksheet ws, List<TableInfo> tables, int gridTop, bool hasSigTest, string tableSheet)
            {
                int sc = opt.StartColumn;

                SetHeaderCell(ws, 1, sc, "Project : " + opt.ProjectName, 14, System.Drawing.Color.DarkBlue, bold: true);
                SetHeaderCell(ws, 2, sc, "Table of Contents", 14, System.Drawing.Color.Black, bold: true);
                SetHeaderCell(ws, 3, sc, AsText("Prepared By : " + opt.PreparedBy), 11, System.Drawing.Color.Black, italic: true);
                SetHeaderCell(ws, 4, sc, "Date : " + Today(), 11, System.Drawing.Color.Black, italic: true);
                Excel.Range hint = SetHeaderCell(ws, 6, sc, "Click on Hyperlink to go to table", 10, System.Drawing.Color.DarkBlue);
                hint.Font.Underline = true;

                if (hasSigTest)
                {
                    SetHeaderCell(ws, 2, sc + 2, "Notes of Sig Test", 11, System.Drawing.Color.DarkBlue, bold: true);
                    SetHeaderCell(ws, 3, sc + 2, "Capital Letter = 95% CL", 11, System.Drawing.Color.DarkRed, italic: true);
                    SetHeaderCell(ws, 4, sc + 2, "Small Letter = 90% CL", 11, System.Drawing.Color.DarkRed, italic: true);
                }

                WriteGrid(ws, tables, gridTop, tableSheet);

                if (sc > 1) ((Excel.Range)ws.Columns[1]).ColumnWidth = 2;
                ((Excel.Range)ws.Columns[sc]).ColumnWidth = 10;
                ((Excel.Range)ws.Columns[sc + 1]).ColumnWidth = 115;
                ((Excel.Range)ws.Columns[sc + 2]).ColumnWidth = 22;
                ((Excel.Range)ws.Columns[sc + 3]).ColumnWidth = 10;
                ((Excel.Range)ws.Rows[5]).RowHeight = 5;
                ((Excel.Range)ws.Rows[7]).RowHeight = 5;

                StyleGrid(ws, gridTop, tables.Count, System.Drawing.Color.SkyBlue);
            }

            private void WriteGeneralIndex(Excel.Worksheet ws, List<TableInfo> tables, int gridTop, string tableSheet)
            {
                int sc = opt.StartColumn;
                int bannerRow = gridTop - 1;

                ws.Range[CellName(bannerRow, sc)].Value2 = AsText("Project : " + opt.ProjectName);
                Excel.Range banner = ws.Range[CellName(bannerRow, sc), CellName(bannerRow, sc + 3)];
                banner.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                banner.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);
                banner.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);
                banner.Font.Bold = true;
                banner.Merge();
                banner.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                WriteGrid(ws, tables, gridTop, tableSheet);

                int n = tables.Count;
                ws.Range[CellName(gridTop + n + 1, sc)].Value2 = AsText("Prepared By : " + opt.PreparedBy);
                ws.Range[CellName(gridTop + n + 2, sc)].Value2 = "Date : " + Today();

                ((Excel.Range)ws.Columns[sc]).ColumnWidth = 10;
                ((Excel.Range)ws.Columns[sc + 1]).ColumnWidth = 80;
                ((Excel.Range)ws.Columns[sc + 2]).ColumnWidth = 22;
                ((Excel.Range)ws.Columns[sc + 3]).ColumnWidth = 10;

                StyleGrid(ws, gridTop, n, System.Drawing.Color.PaleTurquoise);
            }

            /// <summary>Header + one row per table, written as a single array.</summary>
            private void WriteGrid(Excel.Worksheet ws, List<TableInfo> tables, int gridTop, string tableSheet)
            {
                int n = tables.Count;
                var grid = new object[n + 1, 4];
                grid[0, 0] = "Table No.";
                grid[0, 1] = "Table Title";
                grid[0, 2] = "Filter";
                grid[0, 3] = "Base";

                for (int i = 0; i < n; i++)
                {
                    TableInfo tb = tables[i];
                    string no = "Table " + (i + 1).ToString().PadLeft(2, '0');
                    string target = CellName(tb.LinkRow, 1);
                    if (opt.LinkOnTitle)
                    {
                        grid[i + 1, 0] = no;
                        grid[i + 1, 1] = Hyperlink(tableSheet, target, tb.Title);
                    }
                    else
                    {
                        grid[i + 1, 0] = Hyperlink(tableSheet, target, no);
                        grid[i + 1, 1] = AsText(tb.Title);
                    }
                    grid[i + 1, 2] = tb.Filter == null ? null : AsText(tb.Filter);
                    grid[i + 1, 3] = tb.Base;
                }

                int sc = opt.StartColumn;
                ws.Range[CellName(gridTop, sc), CellName(gridTop + n, sc + 3)].Value2 = grid;
            }

            private void StyleGrid(Excel.Worksheet ws, int gridTop, int n, System.Drawing.Color headerColor)
            {
                int sc = opt.StartColumn;
                Excel.Range all = ws.Range[CellName(gridTop, sc), CellName(gridTop + n, sc + 3)];
                all.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                all.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);

                Excel.Range head = ws.Range[CellName(gridTop, sc), CellName(gridTop, sc + 3)];
                head.Interior.Color = System.Drawing.ColorTranslator.ToOle(headerColor);
                head.Font.Bold = true;

                ws.Range[CellName(gridTop, sc + 3), CellName(gridTop + n, sc + 3)].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            }

            private static Excel.Range SetHeaderCell(Excel.Worksheet ws, int row, int col, string value, int size,
                                                     System.Drawing.Color color, bool bold = false, bool italic = false)
            {
                Excel.Range c = ws.Range[CellName(row, col)];
                c.Value2 = value;
                c.Font.Size = size;
                c.Font.Color = System.Drawing.ColorTranslator.ToOle(color);
                c.Font.Bold = bold;
                c.Font.Italic = italic;
                return c;
            }

            // ------------------------------------------------------------------ batched COM helpers

            /// <summary>
            /// Inserts all planned rows with multi-area Range.Insert calls, bottom batch first so the
            /// original row numbers of the rows above stay valid. Areas inside one call must not touch.
            /// </summary>
            private void InsertRows(Excel.Worksheet ws, SortedDictionary<int, int> inserts)
            {
                var batches = new List<string>();
                var sb = new StringBuilder();
                int prevTop = int.MaxValue;
                foreach (KeyValuePair<int, int> kv in inserts.Reverse())
                {
                    int top = kv.Key, bottom = kv.Key + kv.Value - 1;
                    string area = top == bottom ? "A" + top : "A" + top + ":A" + bottom;
                    bool touches = bottom >= prevTop - 1;
                    if (sb.Length > 0 && (touches || sb.Length + 1 + area.Length > MaxAddressLength))
                    {
                        batches.Add(sb.ToString());
                        sb.Clear();
                    }
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append(area);
                    prevTop = top;
                }
                if (sb.Length > 0) batches.Add(sb.ToString());

                for (int i = 0; i < batches.Count; i++)
                {
                    Report($"Inserting spacing rows ({i + 1}/{batches.Count})...", 15 + 38.0 * (i + 1) / batches.Count);
                    try
                    {
                        ws.Range[batches[i]].EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                    }
                    catch (COMException)
                    {
                        // Fallback: one area at a time, bottom-up (areas are already in descending order).
                        foreach (string area in batches[i].Split(','))
                            ws.Range[area].EntireRow.Insert(Excel.XlInsertShiftDirection.xlShiftDown);
                    }
                }
            }

            /// <summary>Writes column-A cells, one array write per run of consecutive rows.</summary>
            private static void WriteColumnA(Excel.Worksheet ws, SortedDictionary<int, object> writes)
            {
                var rows = writes.Keys.ToList();
                int i = 0;
                while (i < rows.Count)
                {
                    int j = i;
                    while (j + 1 < rows.Count && rows[j + 1] == rows[j] + 1) j++;
                    var block = new object[j - i + 1, 1];
                    for (int k = i; k <= j; k++) block[k - i, 0] = writes[rows[k]];
                    ws.Range["A" + rows[i], "A" + rows[j]].Value2 = block;
                    i = j + 1;
                }
            }

            /// <summary>Runs an action on multi-area ranges ("A1,A5,A9"...) instead of cell by cell.</summary>
            private static void ForEachBatch(Excel.Worksheet ws, IEnumerable<string> addresses, Action<Excel.Range> action)
            {
                var sb = new StringBuilder();
                foreach (string a in addresses)
                {
                    if (sb.Length > 0 && sb.Length + 1 + a.Length > MaxAddressLength)
                    {
                        action(ws.Range[sb.ToString()]);
                        sb.Clear();
                    }
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append(a);
                }
                if (sb.Length > 0) action(ws.Range[sb.ToString()]);
            }

            private void Report(string text, double percent, bool force = false)
            {
                int now = Environment.TickCount;
                if (!force && unchecked(now - lastReportTick) < 100) return;
                lastReportTick = now;
                progress(text, percent);
            }

            // ------------------------------------------------------------------ text helpers

            private static string ParseTitle(string line)
            {
                int colon = line.IndexOf(':');
                return (colon >= 0 ? line.Substring(colon + 1) : line.Substring("Table ".Length)).Trim();
            }

            /// <summary>"Base", "Base (weighted)"... but not answer labels such as "Based on my network".</summary>
            private static bool IsGeneralBaseRow(string t)
            {
                return t.StartsWith("Base") && (t.Length == 4 || !char.IsLetter(t[4]));
            }

            private static object CleanValue(object v)
            {
                string s = v as string;
                return s == null ? v : s.Replace("\"", "");
            }

            /// <summary>Stops Excel from treating a label like "- Male" or "=x" as a formula.</summary>
            private static string AsText(string s)
            {
                if (!string.IsNullOrEmpty(s) && "=+-@".IndexOf(s[0]) >= 0) return "'" + s;
                return s;
            }

            private static string Hyperlink(string sheet, string cell, string text)
            {
                string label = (text ?? "").Replace("\"", "");
                if (label.Length > MaxFormulaText) label = label.Substring(0, MaxFormulaText - 3) + "...";
                return "=HYPERLINK(\"#'" + sheet.Replace("'", "''") + "'!" + cell + "\",\"" + label + "\")";
            }

            private static string Today()
            {
                return DateTime.Now.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }

            private static string ColumnName(int columnNumber)
            {
                string name = "";
                while (columnNumber > 0)
                {
                    int modulo = (columnNumber - 1) % 26;
                    name = Convert.ToChar('A' + modulo) + name;
                    columnNumber = (columnNumber - modulo) / 26;
                }
                return name;
            }

            private static string CellName(int row, int column)
            {
                return ColumnName(column) + row;
            }
        }
    }
}
