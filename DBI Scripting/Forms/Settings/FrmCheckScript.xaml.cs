using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DBI_Scripting.Forms.Settings
{
    public partial class FrmCheckScript : Window
    {
        // ── Table definition helper ──────────────────────────────────────────────
        private class TableDef
        {
            public string Table { get; set; }
            public string[] Keys { get; set; }
            public TableDef(string table, string[] keys) { Table = table; Keys = keys; }
        }

        private static readonly List<TableDef> Tables = new List<TableDef>
        {
            new TableDef("T_ProjectInfo",    new[] { "ProjectId" }),
            new TableDef("T_Question",       new[] { "QId" }),
            new TableDef("T_OptAttribute",   new[] { "QId", "AttributeValue" }),
            new TableDef("T_OptAttrbFilter", new[] { "QId", "AttribFilterId" }),
            new TableDef("T_LogicTable",     new[] { "QId", "LogicTypeId" }),
            new TableDef("T_LogicAuto",      new[] { "QId", "LogicTypeId" }),
            new TableDef("T_LanguageMaster", new[] { "LanguageId" }),
            new TableDef("T_GridInfo",       new[] { "QId", "AttributeOrder" }),
            new TableDef("T_PanelData",      new[] { "" }),  // first column used dynamically
        };

        public FrmCheckScript()
        {
            InitializeComponent();
        }

        // ── Browse buttons ───────────────────────────────────────────────────────

        private void btnBrowse1_Click(object sender, RoutedEventArgs e)
        {
            string path = BrowseForDb();
            if (path != null) txtScript1.Text = path;
        }

        private void btnBrowse2_Click(object sender, RoutedEventArgs e)
        {
            string path = BrowseForDb();
            if (path != null) txtScript2.Text = path;
        }

        private string BrowseForDb()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Script Database (*.db)|*.db|All Files (*.*)|*.*";
            dlg.InitialDirectory = Properties.Settings.Default.StartupPath;
            if (dlg.ShowDialog() == true)
                return dlg.FileName;
            return null;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ── Execute ──────────────────────────────────────────────────────────────

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtScript1.Text == "" || txtScript2.Text == "")
            {
                MessageBox.Show("Both script files must be selected.");
                return;
            }
            if (!File.Exists(txtScript1.Text) || !File.Exists(txtScript2.Text))
            {
                MessageBox.Show("One or both selected files do not exist.");
                return;
            }
            if (txtScript1.Text == txtScript2.Text)
            {
                MessageBox.Show("Please select two different script files.");
                return;
            }

            ClearOutput();
            SetUIState(true);
            txtStatus.Text = "Comparing scripts...";

            string db1 = txtScript1.Text;
            string db2 = txtScript2.Text;

            List<string> tablesWithDiff  = new List<string>();
            List<string> tablesIdentical = new List<string>();

            try
            {
                await Task.Run(() =>
                {
                    Action<string, bool> append = (text, isError) =>
                        Dispatcher.Invoke((Action)(() => AppendResult(text, isError)));
                    Action<string, bool> appendBold = (text, isError) =>
                        Dispatcher.Invoke((Action)(() => AppendResultBold(text, isError)));
                    Action<string> status = text =>
                        Dispatcher.Invoke((Action)(() => { txtStatus.Text = text; }));

                    SQLiteConnection conn1 = new SQLiteConnection("Data Source=" + db1);
                    SQLiteConnection conn2 = new SQLiteConnection("Data Source=" + db2);
                    conn1.Open();
                    conn2.Open();

                    try
                    {
                        foreach (TableDef td in Tables)
                        {
                            string tableName = td.Table;
                            string[] keys    = td.Keys;

                            status("Comparing " + tableName + "...");

                            bool exists1 = TableExists(conn1, tableName);
                            bool exists2 = TableExists(conn2, tableName);

                            appendBold("=== " + tableName + " ===", false);

                            if (!exists1 && !exists2)
                            {
                                append("  Skipped (table not found in either database).", false);
                                append("", false);
                                continue;
                            }
                            if (!exists1)
                            {
                                append("  Missing in Script 1.", true);
                                tablesWithDiff.Add(tableName);
                                append("", false);
                                continue;
                            }
                            if (!exists2)
                            {
                                append("  Missing in Script 2.", true);
                                tablesWithDiff.Add(tableName);
                                append("", false);
                                continue;
                            }

                            DataTable dt1 = LoadTable(conn1, tableName);
                            DataTable dt2 = LoadTable(conn2, tableName);

                            // T_PanelData: use first column as key dynamically
                            string[] resolvedKeys = (keys.Length == 1 && keys[0] == "")
                                ? new[] { dt1.Columns[0].ColumnName }
                                : keys;

                            append("  Script 1: " + dt1.Rows.Count + " rows  |  Script 2: " + dt2.Rows.Count + " rows", false);

                            List<string> diffs = CompareDataTables(dt1, dt2, resolvedKeys);

                            if (diffs.Count == 0)
                            {
                                append("  All rows match.", false);
                                tablesIdentical.Add(tableName);
                            }
                            else
                            {
                                foreach (string diff in diffs)
                                    append("  " + diff, true);
                                tablesWithDiff.Add(tableName);
                            }

                            append("", false);
                        }
                    }
                    finally
                    {
                        conn1.Close();
                        conn2.Close();
                        conn1.Dispose();
                        conn2.Dispose();
                    }

                    // ── Summary ───────────────────────────────────────────────
                    append("", false);
                    appendBold("=== SUMMARY ===", false);

                    if (tablesWithDiff.Count == 0)
                    {
                        append("  All tables are identical.", false);
                    }
                    else
                    {
                        append("  Tables with differences (" + tablesWithDiff.Count + "): " +
                               string.Join(", ", tablesWithDiff), true);
                    }

                    if (tablesIdentical.Count > 0)
                    {
                        append("  Tables identical (" + tablesIdentical.Count + "): " +
                               string.Join(", ", tablesIdentical.ToArray()), false);
                    }
                });

                txtStatus.Text = tablesWithDiff.Count == 0
                    ? "Comparison complete — no differences."
                    : "Comparison complete — differences found.";
            }
            catch (Exception err)
            {
                AppendResult("Error: " + err.Message, true);
                txtStatus.Text = "Comparison failed.";
            }
            finally
            {
                SetUIState(false);
            }
        }

        // ── Comparison helpers ───────────────────────────────────────────────────

        private bool TableExists(SQLiteConnection conn, string tableName)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@name", conn))
            {
                cmd.Parameters.AddWithValue("@name", tableName);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private DataTable LoadTable(SQLiteConnection conn, string tableName)
        {
            DataTable dt = new DataTable();
            using (SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT * FROM " + tableName, conn))
                da.Fill(dt);
            return dt;
        }

        private List<string> CompareDataTables(DataTable dt1, DataTable dt2, string[] keyColumns)
        {
            List<string> diffs = new List<string>();

            // Validate key columns exist
            foreach (string key in keyColumns)
            {
                if (!dt1.Columns.Contains(key))
                {
                    diffs.Add("Key column '" + key + "' not found in Script 1.");
                    return diffs;
                }
                if (!dt2.Columns.Contains(key))
                {
                    diffs.Add("Key column '" + key + "' not found in Script 2.");
                    return diffs;
                }
            }

            Dictionary<string, DataRow> map1 = BuildMap(dt1, keyColumns);
            Dictionary<string, DataRow> map2 = BuildMap(dt2, keyColumns);

            // Rows in DB1 missing from DB2
            foreach (string key in map1.Keys)
            {
                if (!map2.ContainsKey(key))
                    diffs.Add("Missing in Script 2: [" + key + "]");
            }

            // Rows in DB2 not in DB1
            foreach (string key in map2.Keys)
            {
                if (!map1.ContainsKey(key))
                    diffs.Add("Extra in Script 2:   [" + key + "]");
            }

            // Same key — compare column values
            foreach (string key in map1.Keys)
            {
                if (!map2.ContainsKey(key)) continue;

                DataRow r1 = map1[key];
                DataRow r2 = map2[key];

                foreach (DataColumn col in dt1.Columns)
                {
                    if (!dt2.Columns.Contains(col.ColumnName)) continue;

                    string v1 = r1[col.ColumnName] != null ? r1[col.ColumnName].ToString() : "";
                    string v2 = r2[col.ColumnName] != null ? r2[col.ColumnName].ToString() : "";

                    if (v1 != v2)
                        diffs.Add("[" + key + "] " + col.ColumnName + ": \"" + v1 + "\" -> \"" + v2 + "\"");
                }
            }

            // Column count mismatch warning
            if (dt1.Columns.Count != dt2.Columns.Count)
                diffs.Insert(0, "Column count differs: Script 1 has " + dt1.Columns.Count +
                                " columns, Script 2 has " + dt2.Columns.Count + " columns.");

            return diffs;
        }

        private Dictionary<string, DataRow> BuildMap(DataTable dt, string[] keyColumns)
        {
            Dictionary<string, DataRow> map = new Dictionary<string, DataRow>();
            foreach (DataRow row in dt.Rows)
            {
                string key = string.Join("|", keyColumns.Select(k => row[k] != null ? row[k].ToString() : "").ToArray());
                if (map.ContainsKey(key))
                {
                    int i = 2;
                    while (map.ContainsKey(key + "#" + i)) i++;
                    key = key + "#" + i;
                }
                map[key] = row;
            }
            return map;
        }

        // ── UI helpers ───────────────────────────────────────────────────────────

        private void ClearOutput()
        {
            txtResult.Document.Blocks.Clear();
        }

        private void AppendResult(string text, bool isError)
        {
            var para = new Paragraph(new Run(text))
            {
                Foreground = isError ? Brushes.Red : Brushes.DarkGreen,
                Margin = new Thickness(0)
            };
            txtResult.Document.Blocks.Add(para);
            txtResult.ScrollToEnd();
        }

        private void AppendResultBold(string text, bool isError)
        {
            var run = new Run(text) { FontWeight = FontWeights.Bold };
            var para = new Paragraph(run)
            {
                Foreground = isError ? Brushes.Red : Brushes.Black,
                Margin = new Thickness(0, 6, 0, 0)
            };
            txtResult.Document.Blocks.Add(para);
            txtResult.ScrollToEnd();
        }

        private void SetUIState(bool running)
        {
            btnExecute.IsEnabled   = !running;
            btnBrowse1.IsEnabled   = !running;
            btnBrowse2.IsEnabled   = !running;
            btnClose.IsEnabled     = !running;
            progressBar.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
