using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    public partial class FrmCTableSigTest : Window
    {
        // ── Section 1 state
        private string myPathS1 = "";
        private string sSelectedSheetS1 = "";

        // ── Section 2 state
        private string myPathS2 = "";
        private string sSelectedSheetS2 = "";
        private List<List<int>> _colGroups = new List<List<int>>();  // column-number groups (above ####)
        private List<string> lstOfSigGroup = new List<string>();      // letter groups (below ####, reference only)
        private List<string> lstOfConfidence = new List<string>();

        // ── Letter sets (restart from A / P for each banner group)
        private static readonly string[] Set1 = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O" };
        private static readonly string[] Set2 = { "P","Q","R","S","T","U","V","W","X","Y","Z" };

        // ── Group def generation tracking (populated during WriteLetterRow)
        private List<BannerGroup> _capturedGroups = null;
        private int _maxSet1Letters = 0;
        private int _maxSet2Letters = 0;

        // ── Confidence → z-score
        private static readonly Dictionary<string, double> ConfidenceThresholds = new Dictionary<string, double>
        {
            { "80% Level of Confidence", 1.282 },
            { "85% Level of Confidence", 1.440 },
            { "90% Level of Confidence", 1.645 },
            { "95% Level of Confidence", 1.960 },
            { "98% Level of Confidence", 2.326 },
            { "99% Level of Confidence", 2.576 }
        };

        public FrmCTableSigTest()
        {
            InitializeComponent();
        }

        private void frmCTableSigTest_Loaded(object sender, RoutedEventArgs e)
        {
            chkListBoxConfidence.Items.Clear();
            foreach (string level in ConfidenceThresholds.Keys)
                chkListBoxConfidence.Items.Add(level);
        }

        // ══════════════════════════════════════════════
        // SECTION 1 — Prepare Table for Sig Test
        // ══════════════════════════════════════════════

        private void btnBrowseExcelS1_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*" };
            if (!string.IsNullOrEmpty(myPathS1)) dlg.InitialDirectory = myPathS1;
            if (dlg.ShowDialog() == true)
            {
                txtExcelPathS1.Text = dlg.FileName;
                myPathS1 = Path.GetDirectoryName(dlg.FileName);
                LoadWorksheetS1();
            }
        }

        private void LoadWorksheetS1()
        {
            if (!File.Exists(txtExcelPathS1.Text)) return;
            Excel.Application xlApp = null;
            Excel.Workbook xlWb = null;
            try
            {
                xlApp = new Excel.Application { Visible = false };
                xlWb = xlApp.Workbooks.Open(txtExcelPathS1.Text, 0, true, 5, "", "", true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                chkListBoxWorksheetS1.Items.Clear();
                sSelectedSheetS1 = "";
                for (int i = 1; i <= xlWb.Worksheets.Count; i++)
                    chkListBoxWorksheetS1.Items.Add(((Excel.Worksheet)xlWb.Worksheets[i]).Name);
            }
            catch (Exception ex) { MessageBox.Show("Error loading worksheets: " + ex.Message); }
            finally
            {
                if (xlWb != null) { xlWb.Close(false); ReleaseObject(xlWb); }
                if (xlApp != null) { xlApp.Quit(); ReleaseObject(xlApp); }
            }
        }

        private void chkListBoxWorksheetS1_ItemSelectionChanged(object sender,
            Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheetS1.SelectedItems.Count > 1)
                chkListBoxWorksheetS1.SelectedItems.Remove(chkListBoxWorksheetS1.SelectedItems[0].ToString());
            sSelectedSheetS1 = chkListBoxWorksheetS1.SelectedItems.Count > 0
                ? chkListBoxWorksheetS1.SelectedItems[0].ToString() : "";
        }

        private void btnRunS1_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtExcelPathS1.Text))
            { MessageBox.Show("Please select a Table Excel file."); return; }
            if (string.IsNullOrEmpty(sSelectedSheetS1))
            { MessageBox.Show("Please select a worksheet."); return; }

            btnRunS1.IsEnabled = false;
            Excel.Application xlApp = null;
            Excel.Workbook xlWb = null;
            Excel.Worksheet ws = null;
            try
            {
                xlApp = new Excel.Application { Visible = false, DisplayAlerts = false };
                xlWb = xlApp.Workbooks.Open(txtExcelPathS1.Text, 0, false, 5, "", "", false,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);
                ws = (Excel.Worksheet)xlWb.Worksheets[sSelectedSheetS1];

                SetStatusS1("Clearing dummy rows...");
                ClearDummyRowsOnSheet(ws);

                SetStatusS1("Preparing table...");
                var overflowMessages = new List<string>();
                PrepareTableWithLetters(ws, overflowMessages);

                SetStatusS1("Saving...");
                xlWb.Save();

                progressBarS1.Value = progressBarS1.Maximum;
                SetStatusS1("Prepare complete. Generating group definition file...");

                if (overflowMessages.Count > 0)
                    MessageBox.Show(
                        "Letter overflow — some groups exceeded their set size:\n\n" +
                        string.Join("\n", overflowMessages) +
                        "\n\nColumns beyond the set limit were left without a letter.",
                        "Overflow Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Generate group definition file
                bool groupDefSaved = false;
                if (_capturedGroups != null && _capturedGroups.Count > 0)
                {
                    string defContent = GenerateGroupDefContent(_capturedGroups, _maxSet1Letters, _maxSet2Letters);
                    string defaultName = Path.GetFileNameWithoutExtension(txtExcelPathS1.Text) + "_GroupDef.txt";
                    var saveDlg = new SaveFileDialog
                    {
                        Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*",
                        FileName = defaultName,
                        InitialDirectory = myPathS1,
                        Title = "Save Group Definition File"
                    };
                    if (saveDlg.ShowDialog() == true)
                    {
                        File.WriteAllText(saveDlg.FileName, defContent);
                        txtGroupDefPath.Text = saveDlg.FileName;
                        PrepareGroupDefView();
                        SetStatusS1("Group definition file saved.");
                        groupDefSaved = true;
                    }
                    else
                    {
                        SetStatusS1("Group definition file save canceled.");
                    }
                }
                else
                {
                    SetStatusS1("No letter rows inserted — group definition file not generated.");
                }

                if (groupDefSaved)
                {
                    MessageBox.Show(
                        "Prepare Table complete.\n\n" +
                        "• Letter row inserted just above each Total row.\n" +
                        "• S.TEST rows inserted between data rows.\n" +
                        "• Group Definition file generated and loaded into Section 2.\n\n" +
                        "Select your worksheet in Section 2 and run the sig test.",
                        "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (_capturedGroups != null && _capturedGroups.Count > 0)
                {
                    MessageBox.Show(
                        "Prepare Table complete, but the Group Definition file was NOT saved " +
                        "(the Save dialog was canceled).\n\n" +
                        "Run Section 1 again to be prompted to save it, or browse to an existing " +
                        "Group Definition file in Section 2.",
                        "Complete — Group Def Not Saved", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "Prepare Table complete, but NO Group Definition file was generated.\n\n" +
                        "This happens when every \"Total\" row in this worksheet already has a letter row " +
                        "above it (e.g. from an earlier Prepare run), so no new letter row was inserted. " +
                        "If you need a fresh Group Definition file, remove the existing letter rows first, " +
                        "or browse to a Group Definition file you already saved.",
                        "Complete — No Group Def Generated", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                SetStatusS1("Error.");
            }
            finally
            {
                if (ws != null) ReleaseObject(ws);
                if (xlWb != null) { try { xlWb.Close(false); } catch { } ReleaseObject(xlWb); }
                if (xlApp != null) { try { xlApp.Quit(); } catch { } ReleaseObject(xlApp); }
                btnRunS1.IsEnabled = true;
            }
        }

        // ──────────────────────────────────────────────
        // PrepareTable core logic
        // Single pass: insert letter row just above each Total row,
        // then insert S.TEST rows between consecutive data rows.
        // ──────────────────────────────────────────────

        private struct BannerGroup
        {
            public string Name;
            public int StartCol;
            public int EndCol;
            public BannerGroup(string name, int startCol, int endCol)
            { Name = name; StartCol = startCol; EndCol = endCol; }
        }

        // Three-pass attribute detection:
        //   Pass 1 — read banner variables from the Base row (merged cells give variable spans).
        //   Pass 2 — for each variable, inspect the sub-attribute row (baseRow+1):
        //     · If all cells inside the variable range have span=1  → no nesting,
        //       the whole variable is ONE group (e.g. Product C3-C9 = 1 group).
        //     · If any cell has span>1 (merged) → split into sub-attribute groups
        //       (e.g. Centre → Dhaka 4-col merge + Rajshahi 4-col merge = 2 groups).
        //   Pass 3 — for each sub-attribute group found in Pass 2, inspect the row below it
        //     (baseRow+2) the same way: if it also has merged cells, split that sub-attribute
        //     into leaf groups instead of keeping it whole (e.g. Centre → Dhaka → Panel 1-3,
        //     each a 2-col leaf group, rather than treating all of Dhaka as one group).
        private List<BannerGroup> GetAttributeGroups(Excel.Worksheet ws, int baseRow)
        {
            var result = new List<BannerGroup>();
            int attrRow = baseRow + 1;
            int leafRow = baseRow + 2;
            int totalCols = ws.UsedRange.Columns.Count;

            int c = 2;
            while (c <= totalCols)
            {
                // ── Read one variable from the Base row ──
                Excel.Range baseCell = (Excel.Range)ws.Cells[baseRow, c];
                int varStart = baseCell.MergeArea.Column;
                int varEnd   = varStart + baseCell.MergeArea.Columns.Count - 1;
                object varVal = ws.Cells[baseRow, varStart].Value2;
                string varName = varVal != null ? varVal.ToString().Trim() : "";

                // Skip truly empty single cells (gaps between variables)
                if (varVal == null && varEnd == varStart) { c++; continue; }

                // ATotal column (C2): keep in list so WriteLetterRow can skip it
                if (varEnd <= 2)
                {
                    result.Add(new BannerGroup(varName, varStart, varEnd));
                    c = varEnd + 1;
                    continue;
                }

                // ── Check sub-attribute row for merged cells inside this variable ──
                bool hasSubGroups = RangeHasMerge(ws, attrRow, Math.Max(varStart, 3), varEnd);

                if (!hasSubGroups)
                {
                    // No sub-nesting → entire variable is one letter group
                    result.Add(new BannerGroup(varName, varStart, varEnd));
                }
                else
                {
                    // Each merged cell in the sub-attribute row = one attribute group
                    int sc = Math.Max(varStart, 3);
                    while (sc <= varEnd)
                    {
                        Excel.Range attrCell = (Excel.Range)ws.Cells[attrRow, sc];
                        int attrStart = attrCell.MergeArea.Column;
                        int attrEnd   = attrStart + attrCell.MergeArea.Columns.Count - 1;
                        object attrVal = ws.Cells[attrRow, attrStart].Value2;
                        string attrName = attrVal != null ? attrVal.ToString().Trim() : "";

                        // ── Check one level deeper for further nesting inside this sub-attribute ──
                        if (RangeHasMerge(ws, leafRow, attrStart, attrEnd))
                        {
                            int lc = attrStart;
                            while (lc <= attrEnd)
                            {
                                Excel.Range leafCell = (Excel.Range)ws.Cells[leafRow, lc];
                                int leafStart = leafCell.MergeArea.Column;
                                int leafEnd   = leafStart + leafCell.MergeArea.Columns.Count - 1;
                                object leafVal = ws.Cells[leafRow, leafStart].Value2;
                                string leafName = leafVal != null ? leafVal.ToString().Trim() : "";
                                result.Add(new BannerGroup(leafName, leafStart, leafEnd));
                                lc = leafEnd + 1;
                            }
                        }
                        else
                        {
                            result.Add(new BannerGroup(attrName, attrStart, attrEnd));
                        }

                        sc = attrEnd + 1;
                    }
                }

                c = varEnd + 1;
            }

            return result;
        }

        // True if any cell in [fromCol, toCol] on the given row is part of a multi-column merge.
        private bool RangeHasMerge(Excel.Worksheet ws, int row, int fromCol, int toCol)
        {
            for (int c = fromCol; c <= toCol; c++)
            {
                Excel.Range cell = (Excel.Range)ws.Cells[row, c];
                if (cell.MergeArea.Columns.Count > 1) return true;
            }
            return false;
        }

        private bool HasLetterRowAbove(Excel.Worksheet ws, int totalRow)
        {
            int above = totalRow - 1;
            if (above < 1) return false;
            object c1 = ws.Cells[above, 1].Value2;
            if (c1 != null && c1.ToString().Trim() != "") return false; // C1 must be blank
            object c3 = ws.Cells[above, 3].Value2;
            if (c3 == null) return false;
            string s = c3.ToString().Trim();
            return s.Length == 1 && s[0] >= 'A' && s[0] <= 'Z';
        }

        private void PrepareTableWithLetters(Excel.Worksheet ws, List<string> overflowMessages)
        {
            _capturedGroups = null;
            _maxSet1Letters = 0;
            _maxSet2Letters = 0;

            bool lastWasData = false;
            int baseRow = -1; // row index of the most recent "Base : All Respondents" row
            int j = 1;

            while (j <= ws.UsedRange.Rows.Count)
            {
                progressBarS1.Maximum = ws.UsedRange.Rows.Count;
                progressBarS1.Value = j;
                if (j % 30 == 0) SetStatusS1("Scanning row " + j + "...");
                PumpDispatcher();

                object raw = ws.Cells[j, 1].Value2;
                string col1 = raw != null ? raw.ToString().Trim() : "";

                // ── Table title row
                if (col1.StartsWith("Table "))
                {
                    lastWasData = false;
                    baseRow = -1;
                    j++; continue;
                }

                // ── Banner group row — just record its position; attribute groups are read
                //    from the row directly below (baseRow + 1) which carries sub-attribute
                //    labels with merge spans that define each attribute group.
                if (col1 == "Base : All Respondents")
                {
                    baseRow = j;
                    lastWasData = false;
                    j++; continue;
                }

                // ── Total / Base count row
                if (col1 == "Total")
                {
                    if (baseRow > 0 && !HasLetterRowAbove(ws, j))
                    {
                        // Read attribute groups from the row immediately after "Base" row.
                        // For Section 1 tables (1 sub-header row): baseRow+1 = sub-column row.
                        // For Section 2 tables (2 sub-header rows): baseRow+1 = attribute row
                        //   (merged cells for multi-column attributes like Dhaka, Rajshahi).
                        var attrGroups = GetAttributeGroups(ws, baseRow);
                        ws.Rows[j].Insert(1);
                        WriteLetterRow(ws, j, attrGroups, overflowMessages);
                        j++; // advance past the new letter row to the original Total row
                    }
                    baseRow = -1;
                    lastWasData = false;
                    j++; continue;
                }

                // ── Home row — end of table
                if (col1.ToUpper() == "HOME")
                {
                    if (lastWasData)
                    {
                        ws.Rows[j].Insert(1);
                        ws.Cells[j, 1].Value2 = "S.TEST";
                        j++;
                    }
                    lastWasData = false;
                    j++; continue;
                }

                // ── Already a S.TEST / SIG.TEST row (re-run guard)
                if (col1 == "S.TEST" || col1 == "SIG. TEST")
                {
                    lastWasData = false;
                    j++; continue;
                }

                // ── S.D. / S.E. / DUMMY ROW — derived metric rows; never insert S.TEST before them
                if (col1 == "S.D." || col1 == "S. D." ||
                    col1 == "S.E." || col1 == "S. E." ||
                    col1 == "DUMMY ROW")
                {
                    lastWasData = false;
                    j++; continue;
                }

                // ── MEAN row — insert S.TEST before it if a data row preceded it,
                //              then insert SIG. TEST immediately after it
                if (col1 == "MEAN")
                {
                    if (lastWasData)
                    {
                        ws.Rows[j].Insert(1);
                        ws.Cells[j, 1].Value2 = "S.TEST";
                        j++; // skip past inserted S.TEST; j now points at MEAN again
                    }
                    object nextRaw = ws.Cells[j + 1, 1].Value2;
                    string nextCol1 = nextRaw != null ? nextRaw.ToString().Trim() : "";
                    if (nextCol1 != "SIG. TEST")
                    {
                        ws.Rows[j + 1].Insert(1);
                        ws.Cells[j + 1, 1].Value2 = "SIG. TEST";
                        j++; // skip past the inserted SIG. TEST row
                    }
                    lastWasData = false;
                    j++; continue;
                }

                // ── Empty row (sub-column headers or blank separator)
                // If the previous row was a data row, use this blank row as the S.TEST row
                // rather than inserting a new one (handles blank-before-Home pattern).
                if (col1 == "")
                {
                    if (lastWasData)
                        ws.Cells[j, 1].Value2 = "S.TEST";
                    lastWasData = false;
                    j++; continue;
                }

                // ── Data row
                if (lastWasData)
                {
                    ws.Rows[j].Insert(1);
                    ws.Cells[j, 1].Value2 = "S.TEST";
                    j++; // skip past the inserted S.TEST to the original data row
                }
                lastWasData = true;
                j++;
            }

            SetStatusS1("Scan complete.");
        }

        private void WriteLetterRow(Excel.Worksheet ws, int letterRow,
            List<BannerGroup> groups, List<string> overflowMessages)
        {
            // C1 = blank, C2 = blank (ATotal / Total column), letters from C3 onwards.
            // Groups alternate: odd index → Set 1 (A-O), even index → Set 2 (P-Z).
            // ATotal group (endCol <= 2) is skipped and does not count toward the index.

            int setIdx = 0; // 0 = Set 1, 1 = Set 2, alternates per non-ATotal group

            foreach (BannerGroup grp in groups)
            {
                if (grp.EndCol <= 2) continue; // skip ATotal (C2)

                string[] currentSet = (setIdx % 2 == 0) ? Set1 : Set2;
                int assignFrom = Math.Max(grp.StartCol, 3);
                int colCount = grp.EndCol - assignFrom + 1;
                int lettersUsed = Math.Min(colCount, currentSet.Length);
                int letterIdx = 0;

                for (int c = assignFrom; c <= grp.EndCol; c++)
                {
                    if (letterIdx < currentSet.Length)
                    {
                        ws.Cells[letterRow, c].Value2 = currentSet[letterIdx];
                        letterIdx++;
                    }
                }

                // Track maximum letters used per set for group def file generation
                if (setIdx % 2 == 0)
                    _maxSet1Letters = Math.Max(_maxSet1Letters, lettersUsed);
                else
                    _maxSet2Letters = Math.Max(_maxSet2Letters, lettersUsed);

                if (colCount > currentSet.Length)
                    overflowMessages.Add(
                        $"  • '{grp.Name}' (Set {(setIdx % 2 == 0 ? "1  A–O" : "2  P–Z")}): " +
                        $"{colCount} columns but only {currentSet.Length} letters available " +
                        $"(×{colCount - currentSet.Length} columns without a letter).");

                setIdx++;
            }

            // Keep the groups from the table with the most attribute groups (most columns)
            int nonATotal = groups.Count(g => g.EndCol > 2);
            int capturedNonATotal = _capturedGroups != null ? _capturedGroups.Count(g => g.EndCol > 2) : -1;
            if (nonATotal > capturedNonATotal)
                _capturedGroups = new List<BannerGroup>(groups);
        }

        private string GenerateGroupDefContent(List<BannerGroup> groups, int maxSet1, int maxSet2)
        {
            var sb = new StringBuilder();

            // Column-number section: one line per attribute group
            foreach (BannerGroup grp in groups)
            {
                if (grp.EndCol <= 2) continue;
                int from = Math.Max(grp.StartCol, 3);
                sb.AppendLine(string.Join(",", Enumerable.Range(from, grp.EndCol - from + 1)));
            }

            sb.AppendLine("############################");

            // Letter section: max Set1 word then max Set2 word
            if (maxSet1 > 0)
                sb.AppendLine(string.Concat(Set1.Take(maxSet1)));
            if (maxSet2 > 0)
                sb.AppendLine(string.Concat(Set2.Take(maxSet2)));

            return sb.ToString();
        }

        private void btnClearDummyRows_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtExcelPathS1.Text))
            { MessageBox.Show("Please select a Table Excel file."); return; }
            if (string.IsNullOrEmpty(sSelectedSheetS1))
            { MessageBox.Show("Please select a worksheet."); return; }

            btnClearDummyRows.IsEnabled = false;
            Excel.Application xlApp = null;
            Excel.Workbook xlWb = null;
            Excel.Worksheet ws = null;
            try
            {
                xlApp = new Excel.Application { Visible = false, DisplayAlerts = false };
                xlWb = xlApp.Workbooks.Open(txtExcelPathS1.Text, 0, false, 5, "", "", false,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);
                ws = (Excel.Worksheet)xlWb.Worksheets[sSelectedSheetS1];

                SetStatusS1("Clearing DUMMY ROW data...");
                int count = ClearDummyRowsOnSheet(ws);

                SetStatusS1("Saving...");
                xlWb.Save();

                progressBarS1.Value = progressBarS1.Maximum;
                SetStatusS1("Clear complete.");
                MessageBox.Show(
                    $"DUMMY ROW data cleared.\n{count} row(s) processed — labels kept, data erased.",
                    "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                SetStatusS1("Error.");
            }
            finally
            {
                if (ws != null) ReleaseObject(ws);
                if (xlWb != null) { try { xlWb.Close(false); } catch { } ReleaseObject(xlWb); }
                if (xlApp != null) { try { xlApp.Quit(); } catch { } ReleaseObject(xlApp); }
                btnClearDummyRows.IsEnabled = true;
            }
        }

        // Clears all data cells (C2 onwards) in every "DUMMY ROW" row.
        // The row itself and its C1 label are preserved.
        private int ClearDummyRowsOnSheet(Excel.Worksheet ws)
        {
            int totalRows = ws.UsedRange.Rows.Count;
            int totalCols = ws.UsedRange.Columns.Count;
            int count = 0;

            progressBarS1.Minimum = 0;
            progressBarS1.Maximum = totalRows;
            progressBarS1.Value = 0;

            for (int j = 1; j <= totalRows; j++)
            {
                progressBarS1.Value = j;
                if (j % 50 == 0) { SetStatusS1("Scanning row " + j + "..."); PumpDispatcher(); }

                object raw = ws.Cells[j, 1].Value2;
                if (raw == null || raw.ToString().Trim() != "DUMMY ROW") continue;

                // Clear the entire row including C1
                for (int c = 1; c <= totalCols; c++)
                    ws.Cells[j, c].Value2 = null;

                count++;
            }
            return count;
        }

        private void btnKillProcessS1_Click(object sender, RoutedEventArgs e)
        {
            foreach (Process p in Process.GetProcessesByName("EXCEL")) p.Kill();
            MessageBox.Show("All Excel processes have been killed.");
        }

        private void btnCloseS1_Click(object sender, RoutedEventArgs e) => Close();

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SigTest_Help.html");
            if (File.Exists(helpPath))
                Process.Start(helpPath);
            else
                MessageBox.Show("Help file not found:\n" + helpPath, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ══════════════════════════════════════════════
        // SECTION 2 — Set Sig Test Value
        // ══════════════════════════════════════════════

        private void btnBrowseExcelS2_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*" };
            if (!string.IsNullOrEmpty(myPathS2)) dlg.InitialDirectory = myPathS2;
            if (dlg.ShowDialog() == true)
            {
                txtExcelPathS2.Text = dlg.FileName;
                myPathS2 = Path.GetDirectoryName(dlg.FileName);
                LoadWorksheetS2();
            }
        }

        private void LoadWorksheetS2()
        {
            if (!File.Exists(txtExcelPathS2.Text)) return;
            Excel.Application xlApp = null;
            Excel.Workbook xlWb = null;
            try
            {
                xlApp = new Excel.Application { Visible = false };
                xlWb = xlApp.Workbooks.Open(txtExcelPathS2.Text, 0, true, 5, "", "", true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                chkListBoxWorksheetS2.Items.Clear();
                sSelectedSheetS2 = "";
                for (int i = 1; i <= xlWb.Worksheets.Count; i++)
                    chkListBoxWorksheetS2.Items.Add(((Excel.Worksheet)xlWb.Worksheets[i]).Name);
            }
            catch (Exception ex) { MessageBox.Show("Error loading worksheets: " + ex.Message); }
            finally
            {
                if (xlWb != null) { xlWb.Close(false); ReleaseObject(xlWb); }
                if (xlApp != null) { xlApp.Quit(); ReleaseObject(xlApp); }
            }
        }

        private void chkListBoxWorksheetS2_ItemSelectionChanged(object sender,
            Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheetS2.SelectedItems.Count > 1)
                chkListBoxWorksheetS2.SelectedItems.Remove(chkListBoxWorksheetS2.SelectedItems[0].ToString());
            sSelectedSheetS2 = chkListBoxWorksheetS2.SelectedItems.Count > 0
                ? chkListBoxWorksheetS2.SelectedItems[0].ToString() : "";
        }

        private void btnBrowseGroupDef_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*" };
            if (!string.IsNullOrEmpty(myPathS2)) dlg.InitialDirectory = myPathS2;
            if (dlg.ShowDialog() == true)
            {
                txtGroupDefPath.Text = dlg.FileName;
                PrepareGroupDefView();
            }
        }

        private void PrepareGroupDefView()
        {
            if (!File.Exists(txtGroupDefPath.Text)) return;
            txtViewGroup.Text = File.ReadAllText(txtGroupDefPath.Text);
        }

        private void chkListBoxConfidence_ItemSelectionChanged(object sender,
            Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            lstOfConfidence.Clear();
            foreach (var item in chkListBoxConfidence.SelectedItems)
                lstOfConfidence.Add(item.ToString());
        }

        // ──────────────────────────────────────────────
        // Group definition file parser
        // Each line = a set of letters that should be compared pairwise (e.g. "AB", "PQR")
        // ──────────────────────────────────────────────

        private void ParseGroupDefFile()
        {
            _colGroups.Clear();
            lstOfSigGroup.Clear();
            if (!File.Exists(txtGroupDefPath.Text)) return;

            bool pastSeparator = false;
            foreach (string line in File.ReadAllLines(txtGroupDefPath.Text))
            {
                string trimmed = line.Trim();
                if (trimmed == "") continue;
                if (trimmed.StartsWith("####")) { pastSeparator = true; continue; }

                if (!pastSeparator)
                {
                    // Column-number group: "3,4,5,6,7,8,9"
                    var cols = new List<int>();
                    foreach (string part in trimmed.Split(','))
                        if (int.TryParse(part.Trim(), out int col)) cols.Add(col);
                    if (cols.Count > 0) _colGroups.Add(cols);
                }
                else
                {
                    lstOfSigGroup.Add(trimmed.ToUpper()); // letter groups kept for reference
                }
            }
        }

        // Returns true when colA and colB appear in the same column-number group.
        // This replaces letter-based ShouldSigTest and avoids cross-group comparisons
        // caused by duplicate letters (e.g. both Product-C3 and Panel2-C12 have letter A).
        private bool ShouldCompare(int colA, int colB)
        {
            foreach (var group in _colGroups)
                if (group.Contains(colA) && group.Contains(colB))
                    return true;
            return false;
        }

        // Proportion z-test for percentage data rows (Yes/No, TOP 2 BOX, etc.)
        private double ZScore(int n1, int n2, double p1, double p2)
        {
            if (n1 == 0 || n2 == 0) return 0;
            double r1 = p1 / 100.0, r2 = p2 / 100.0;
            double denom = Math.Sqrt((r1 * (1 - r1)) / n1 + (r2 * (1 - r2)) / n2);
            return denom == 0 ? 0 : Math.Abs((r1 - r2) / denom);
        }

        // Mean z-test for MEAN rows using Standard Error values
        // z = |mean1 - mean2| / sqrt(SE1² + SE2²)
        private double ZScoreMean(double mean1, double mean2, double se1, double se2)
        {
            double denom = Math.Sqrt(se1 * se1 + se2 * se2);
            return denom == 0 ? 0 : Math.Abs((mean1 - mean2) / denom);
        }

        // Converts a z critical value to the equivalent t critical value for the given
        // degrees of freedom using the Cornish-Fisher series expansion.
        // Accurate to < 0.01 for df >= 10; converges to z as df → ∞.
        private double GetTCritical(double z, int df)
        {
            if (df <= 0 || df >= 1000) return z;
            double n = df;
            double z2 = z * z;
            double t = z
                + (z2 * z + z) / (4.0 * n)
                + (5.0 * z2 * z2 * z + 16.0 * z2 * z + 3.0 * z) / (96.0 * n * n)
                + (3.0 * z2 * z2 * z2 * z + 19.0 * z2 * z2 * z + 17.0 * z2 * z - 15.0 * z) / (384.0 * n * n * n);
            return t;
        }

        // Returns the effective threshold to compare against the computed z/t score.
        // Z-Test: returns the fixed z critical value.
        // T-Table: adjusts for df = n1 + n2 - 2 using Cornish-Fisher expansion.
        private double GetEffectiveThreshold(double zCritical, int n1, int n2)
        {
            if (rdoTTest.IsChecked == true)
                return GetTCritical(zCritical, n1 + n2 - 2);
            return zCritical;
        }

        // ──────────────────────────────────────────────
        // Read letter map from the row immediately above the Total row.
        // That row has blank C1/C2 and single uppercase letters in C3+.
        // ──────────────────────────────────────────────

        private Dictionary<int, string> ReadLetterMap(Excel.Worksheet ws, int totalRow)
        {
            var map = new Dictionary<int, string>();
            int letterRow = totalRow - 1;
            if (letterRow < 1) return map;

            int totalCols = ws.UsedRange.Columns.Count;
            for (int c = 3; c <= totalCols; c++)
            {
                object v = ws.Cells[letterRow, c].Value2;
                if (v == null) continue;
                string s = v.ToString().Trim();
                if (s.Length == 1 && s[0] >= 'A' && s[0] <= 'Z')
                    map[c] = s;
            }
            return map;
        }

        private void btnRunSigTest_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtExcelPathS2.Text))
            { MessageBox.Show("Please select a Table Excel file."); return; }
            if (string.IsNullOrEmpty(sSelectedSheetS2))
            { MessageBox.Show("Please select a worksheet."); return; }
            if (!File.Exists(txtGroupDefPath.Text))
            { MessageBox.Show("Please select a Group Definition file.\n\nRun Section 1 first to prepare the table."); return; }
            if (lstOfConfidence.Count == 0)
            { MessageBox.Show("Please select at least one Level of Confidence."); return; }

            btnRunSigTest.IsEnabled = false;
            Excel.Application xlApp = null;
            Excel.Workbook xlWb = null;
            Excel.Worksheet ws = null;
            try
            {
                ParseGroupDefFile();
                if (_colGroups.Count == 0)
                { MessageBox.Show("No column groups found in the Group Definition file.\nLines above #### should list column numbers, e.g. 3,4,5,6,7"); return; }

                xlApp = new Excel.Application { Visible = false, DisplayAlerts = false };
                xlWb = xlApp.Workbooks.Open(txtExcelPathS2.Text, 0, false, 5, "", "", false,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);
                ws = (Excel.Worksheet)xlWb.Worksheets[sSelectedSheetS2];

                var sigWarnings = new List<string>();
                SetStatusS2("Running significance test...");
                RunSigTestOnSheet(ws, sigWarnings);

                SetStatusS2("Saving...");
                xlWb.Save();

                progressBarS2.Value = progressBarS2.Maximum;
                SetStatusS2("Significance test complete.");

                if (sigWarnings.Count > 0)
                    MessageBox.Show(
                        "Significance test completed, but some MEAN rows were skipped:\n\n" +
                        string.Join("\n", sigWarnings) +
                        "\n\nEach MEAN row must be followed by S. D. then S. E. for its sig test to run. " +
                        "Check those rows in the worksheet.",
                        "Mean Sig Test Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                MessageBox.Show("Significance test completed successfully.",
                    "Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                SetStatusS2("Error.");
            }
            finally
            {
                if (ws != null) ReleaseObject(ws);
                if (xlWb != null) { try { xlWb.Close(false); } catch { } ReleaseObject(xlWb); }
                if (xlApp != null) { try { xlApp.Quit(); } catch { } ReleaseObject(xlApp); }
                btnRunSigTest.IsEnabled = true;
            }
        }

        private void RunSigTestOnSheet(Excel.Worksheet ws, List<string> warnings)
        {
            // Build threshold list (sorted descending — index 0 = upper/strict, index 1 = lower)
            var thresholds = new List<double>();
            foreach (string l in lstOfConfidence)
                if (ConfidenceThresholds.ContainsKey(l)) thresholds.Add(ConfidenceThresholds[l]);
            thresholds.Sort((a, b) => b.CompareTo(a));
            double upperZ = thresholds.Count > 0 ? thresholds[0] : 1.960;
            double lowerZ = thresholds.Count > 1 ? thresholds[1] : -1;

            int totalRows = ws.UsedRange.Rows.Count;
            progressBarS2.Minimum = 0;
            progressBarS2.Maximum = totalRows;
            progressBarS2.Value = 0;

            Dictionary<int, string> letterMap = null;
            var nValues = new Dictionary<int, int>();

            for (int j = 1; j <= totalRows; j++)
            {
                progressBarS2.Value = j;
                if (j % 30 == 0) SetStatusS2("Testing row " + j + " of " + totalRows + "...");
                PumpDispatcher();

                object raw = ws.Cells[j, 1].Value2;
                string col1 = raw != null ? raw.ToString().Trim() : "";

                // ── Total row: refresh letter map and base N values
                if (col1 == "Total")
                {
                    letterMap = ReadLetterMap(ws, j);
                    nValues.Clear();
                    foreach (int c in letterMap.Keys)
                    {
                        object nv = ws.Cells[j, c].Value2;
                        if (nv is double) nValues[c] = (int)(double)nv;
                    }
                    continue;
                }

                // ── Process both S.TEST (after data rows) and SIG. TEST (after MEAN row)
                bool isSigRow  = col1 == "S.TEST";
                bool isMeanSig = col1 == "SIG. TEST";
                if ((!isSigRow && !isMeanSig) || letterMap == null || letterMap.Count == 0) continue;

                // The data / MEAN row is always directly above the sig test row
                int dataRow = j - 1;
                if (dataRow < 1) continue;

                // Read MEAN (or %) values from the row above
                var pVals = new Dictionary<int, double>();
                foreach (int c in letterMap.Keys)
                {
                    object pv = ws.Cells[dataRow, c].Value2;
                    if (pv is double) pVals[c] = (double)pv;
                }

                // For SIG. TEST rows: read S.E. values from j+2
                // Structure is always: SIG. TEST → S. D. → S. E.
                var seVals = new Dictionary<int, double>();
                if (isMeanSig)
                {
                    int seRow = j + 2;
                    object seLabel = ws.Cells[seRow, 1].Value2;
                    string seLabelStr = seLabel != null ? seLabel.ToString().Trim() : "";
                    if (seLabelStr == "S. E." || seLabelStr == "S.E.")
                    {
                        foreach (int c in letterMap.Keys)
                        {
                            object sev = ws.Cells[seRow, c].Value2;
                            if (sev is double) seVals[c] = (double)sev;
                        }
                    }
                    else
                    {
                        // S.D./S.E. not where expected — skip this row rather than silently
                        // falling back to a proportion z-test on raw mean values (which would
                        // produce meaningless/misleading sig letters).
                        warnings.Add($"  • Row {j}: expected 'S. E.' two rows below SIG. TEST but found " +
                            $"'{(seLabelStr == "" ? "(blank)" : seLabelStr)}' — mean sig test skipped for this row.");
                        continue;
                    }
                }

                // Build SigItem list
                var items = new List<SigItem>();
                foreach (int c in letterMap.Keys)
                {
                    double p = pVals.ContainsKey(c) ? pVals[c] : 0;
                    int n = nValues.ContainsKey(c) ? nValues[c] : 0;
                    items.Add(new SigItem(c, letterMap[c], p, n));
                }

                // Sort ascending by value so higher-index items have higher values
                items.Sort((a, b) => a.Proportion.CompareTo(b.Proportion));

                var sigOut = new Dictionary<int, string>();
                foreach (SigItem it in items) sigOut[it.ColNum] = "";

                // Pairwise comparison — fst has higher value, scnd has lower value.
                // Only compare pairs that belong to the same column-number group.
                for (int fst = items.Count - 1; fst > 0; fst--)
                {
                    for (int scnd = fst - 1; scnd >= 0; scnd--)
                    {
                        if (!ShouldCompare(items[fst].ColNum, items[scnd].ColNum)) continue;
                        if (items[fst].N == 0 || items[scnd].N == 0) continue;

                        double z;
                        if (isMeanSig && seVals.Count > 0)
                        {
                            // Mean z-test: z = |mean1 - mean2| / sqrt(SE1² + SE2²)
                            double se1 = seVals.ContainsKey(items[fst].ColNum) ? seVals[items[fst].ColNum] : 0;
                            double se2 = seVals.ContainsKey(items[scnd].ColNum) ? seVals[items[scnd].ColNum] : 0;
                            z = ZScoreMean(items[fst].Proportion, items[scnd].Proportion, se1, se2);
                        }
                        else
                        {
                            // Proportion z-test for percentage rows
                            z = ZScore(items[fst].N, items[scnd].N,
                                items[fst].Proportion, items[scnd].Proportion);
                        }

                        double effUpper = GetEffectiveThreshold(upperZ, items[fst].N, items[scnd].N);
                        double effLower = lowerZ > 0 ? GetEffectiveThreshold(lowerZ, items[fst].N, items[scnd].N) : -1;

                        if (z >= effUpper)
                            sigOut[items[fst].ColNum] += items[scnd].Letter.ToUpper();
                        else if (effLower > 0 && z >= effLower)
                            sigOut[items[fst].ColNum] += items[scnd].Letter.ToLower();
                    }
                }

                foreach (SigItem it in items)
                    if (sigOut[it.ColNum] != "")
                        ws.Cells[j, it.ColNum].Value2 = sigOut[it.ColNum];

                // Remove the S.TEST label from C1 — the row stays with its letters
                if (isSigRow)
                    ws.Cells[j, 1].Value2 = null;
            }

            SetStatusS2("Sig test scan complete.");
        }

        private class SigItem
        {
            public int ColNum;
            public string Letter;
            public double Proportion;
            public int N;
            public SigItem(int col, string letter, double proportion, int n)
            { ColNum = col; Letter = letter; Proportion = proportion; N = n; }
        }

        private void btnCloseS2_Click(object sender, RoutedEventArgs e) => Close();

        // ══════════════════════════════════════════════
        // Shared helpers
        // ══════════════════════════════════════════════

        private void SetStatusS1(string text) { lblStatusS1.Text = text; PumpDispatcher(); }
        private void SetStatusS2(string text) { lblStatusS2.Text = text; PumpDispatcher(); }

        private void PumpDispatcher()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
        }

        private void ReleaseObject(object obj)
        {
            try { System.Runtime.InteropServices.Marshal.ReleaseComObject(obj); }
            catch { }
            finally { GC.Collect(); }
        }
    }
}
