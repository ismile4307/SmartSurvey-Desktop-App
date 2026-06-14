using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace DBI_Scripting.Forms.Analytics
{
    public partial class FrmSummaryTableSyntax : Window
    {
        private List<MetricSet> _sets = new List<MetricSet>();
        private Dictionary<string, string> _spssLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string _generatedSyntax = "";

        public FrmSummaryTableSyntax()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) { }

        private void btnBrowseVarDef_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                Title = "Select Variable Definition File"
            };
            if (dlg.ShowDialog() == true)
            {
                txtVarDefPath.Text = dlg.FileName;
                TryParseVarDef();
            }
        }

        private void btnBrowseSav_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "SPSS Files (*.sav)|*.sav|All Files (*.*)|*.*",
                Title = "Select SPSS SAV File"
            };
            if (dlg.ShowDialog() == true)
            {
                txtSavPath.Text = dlg.FileName;
                TryParseSav();
            }
        }

        private void TryParseVarDef()
        {
            try
            {
                _sets = ParseVarDefFile(txtVarDefPath.Text);
                lstSets.Items.Clear();
                foreach (var s in _sets)
                {
                    var stem = GetStem(s.Vars[0]);
                    var metrics = new List<string>();
                    if (s.TbValues != null)   metrics.Add("TB=" + string.Join(",", s.TbValues));
                    if (s.T2bValues != null)  metrics.Add("T2B=" + string.Join(",", s.T2bValues));
                    if (s.B2bValues != null)  metrics.Add("B2B=" + string.Join(",", s.B2bValues));
                    if (s.BbValues != null)   metrics.Add("BB=" + string.Join(",", s.BbValues));
                    if (s.MeanValues != null) metrics.Add("Mean");
                    if (s.JrValues != null)   metrics.Add("JR=" + string.Join(",", s.JrValues));
                    lstSets.Items.Add($"{stem} ({s.Vars.Count} vars): {string.Join(", ", metrics)}");
                }
                lblStatus.Text = $"Parsed {_sets.Count} set(s) from variable definition file.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing variable definition file:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TryParseSav()
        {
            try
            {
                _spssLabels = ParseSpssVarLabels(txtSavPath.Text);
                lblStatus.Text = $"Loaded {_spssLabels.Count} variable label(s) from SPSS file.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading SPSS file:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (_sets.Count == 0)
            {
                MessageBox.Show("Please browse and load a variable definition file first.",
                    "Generate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                progressBar1.Value = 20;
                lblStatus.Text = "Generating syntax...";
                _generatedSyntax = GenerateSyntax(_sets, _spssLabels, txtSavPath.Text);
                txtSyntax.Text = _generatedSyntax;
                progressBar1.Value = 100;
                lblStatus.Text = $"Syntax generated — {_sets.Count} set(s), ready to save.";
                btnSaveSPS.IsEnabled = true;
            }
            catch (Exception ex)
            {
                progressBar1.Value = 0;
                MessageBox.Show("Error generating syntax:\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveSPS_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_generatedSyntax)) return;
            var dlg = new SaveFileDialog
            {
                Filter = "SPSS Syntax (*.sps)|*.sps|All Files (*.*)|*.*",
                Title = "Save Syntax File",
                DefaultExt = "sps"
            };
            if (dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, _generatedSyntax, Encoding.GetEncoding(1252));
                lblStatus.Text = "Saved: " + dlg.FileName;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            var tb = new TextBox
            {
                Text = GetFormatHelp(),
                IsReadOnly = true,
                FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                FontSize = 12,
                Margin = new Thickness(12),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.NoWrap,
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.White
            };
            var win = new Window
            {
                Title = "Variable Definition File — Format Reference",
                Width = 500,
                Height = 440,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = System.Windows.Media.Brushes.White,
                Content = tb
            };
            win.ShowDialog();
        }

        private static string GetFormatHelp()
        {
            return
"Variable Definition File Format\r\n" +
"================================\r\n" +
"\r\n" +
"Each block starts with a directive line (*) that defines\r\n" +
"the metrics, followed by variable names one per line.\r\n" +
"Blank lines separate blocks.\r\n" +
"\r\n" +
"Directive syntax:\r\n" +
"  *METRIC=value[,value]; METRIC=value[,value]; ...\r\n" +
"\r\n" +
"Metrics:\r\n" +
"  TB    Top Box       (e.g.  TB=5)\r\n" +
"  T2B   Top 2 Box     (e.g.  T2B=4,5)\r\n" +
"  Mean  Mean range    (e.g.  Mean=1,2,3,4,5)\r\n" +
"  B2B   Bottom 2 Box  (e.g.  B2B=1,2)\r\n" +
"  BB    Bottom Box    (e.g.  BB=1)\r\n" +
"  JR    Just Right    (e.g.  JR=3)\r\n" +
"\r\n" +
"Notes:\r\n" +
"  TB, T2B and Mean carry forward to the next block.\r\n" +
"  B2B, BB and JR apply only to the current block.\r\n" +
"\r\n" +
"Example:\r\n" +
"--------\r\n" +
"*TB=5; T2B=4,5; Mean=1,2,3,4,5; B2B=1,2; BB=1\r\n" +
"S24_1\r\n" +
"S24_2\r\n" +
"S24_3\r\n" +
"...\r\n" +
"S24_34\r\n" +
"\r\n" +
"*JR=3\r\n" +
"S25_1\r\n" +
"S25_2\r\n" +
"...\r\n" +
"S25_7\r\n";
        }

        // ─── Data model ──────────────────────────────────────────────────────

        private class MetricSet
        {
            public List<string> Vars    { get; set; } = new List<string>();
            public List<string> TbValues   { get; set; }
            public List<string> T2bValues  { get; set; }
            public List<string> MeanValues { get; set; }
            public List<string> B2bValues  { get; set; }
            public List<string> BbValues   { get; set; }
            public List<string> JrValues   { get; set; }
        }

        // ─── Parsing ─────────────────────────────────────────────────────────

        private static List<MetricSet> ParseVarDefFile(string path)
        {
            var sets   = new List<MetricSet>();
            var lines  = File.ReadAllLines(path);
            List<string> curTb = null, curT2b = null, curMean = null;
            MetricSet current = null;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (line.StartsWith("*"))
                {
                    if (current != null && current.Vars.Count > 0) sets.Add(current);
                    current = new MetricSet();

                    foreach (var part in line.Substring(1).Split(';'))
                    {
                        var kv = part.Trim().Split('=');
                        if (kv.Length != 2) continue;
                        var key  = kv[0].Trim().ToUpper();
                        var vals = kv[1].Trim().Split(',')
                                        .Select(v => v.Trim())
                                        .Where(v => v.Length > 0)
                                        .ToList();
                        switch (key)
                        {
                            case "TB":   curTb   = vals; break;
                            case "T2B":  curT2b  = vals; break;
                            case "MEAN": curMean = vals; break;
                            case "B2B":  current.B2bValues = vals; break;
                            case "BB":   current.BbValues  = vals; break;
                            case "JR":   current.JrValues  = vals; break;
                        }
                    }
                    current.TbValues   = curTb   != null ? new List<string>(curTb)   : null;
                    current.T2bValues  = curT2b  != null ? new List<string>(curT2b)  : null;
                    current.MeanValues = curMean != null ? new List<string>(curMean) : null;
                }
                else if (!string.IsNullOrWhiteSpace(line) && current != null)
                {
                    current.Vars.Add(line);
                }
            }
            if (current != null && current.Vars.Count > 0) sets.Add(current);
            return sets;
        }

        private static Dictionary<string, string> ParseSpssVarLabels(string savPath)
        {
            var labels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var enc = Encoding.GetEncoding(1252);

            using (var br = new BinaryReader(
                File.Open(savPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), enc))
            {
                br.ReadBytes(176); // skip file header record

                while (br.BaseStream.Position <= br.BaseStream.Length - 4)
                {
                    int recType = br.ReadInt32();

                    if (recType == 2) // variable record
                    {
                        int  type     = br.ReadInt32();
                        int  hasLabel = br.ReadInt32();
                        int  nMiss    = br.ReadInt32();
                        br.ReadInt32(); br.ReadInt32(); // print, write formats
                        string name  = enc.GetString(br.ReadBytes(8)).TrimEnd(' ');
                        string label = "";
                        if (hasLabel != 0)
                        {
                            int len    = br.ReadInt32();
                            int padded = (len + 3) / 4 * 4;
                            var bytes  = br.ReadBytes(padded);
                            label = enc.GetString(bytes, 0, len);
                        }
                        if (nMiss != 0)
                            br.ReadBytes(Math.Abs(nMiss) * 8);
                        if (type != -1 && !string.IsNullOrWhiteSpace(name))
                            labels[name] = label;
                    }
                    else if (recType == 3) // value labels
                    {
                        int n = br.ReadInt32();
                        for (int i = 0; i < n; i++)
                        {
                            br.ReadBytes(8);           // 8-byte value
                            int len = br.ReadByte();   // 1-byte label length
                            int pad = (8 - (9 + len) % 8) % 8;
                            br.ReadBytes(len + pad);
                        }
                    }
                    else if (recType == 4) { int n = br.ReadInt32(); br.ReadBytes(n * 4); }
                    else if (recType == 6) { int n = br.ReadInt32(); br.ReadBytes(n * 80); }
                    else if (recType == 7)
                    {
                        br.ReadInt32(); // subtype
                        int sz = br.ReadInt32(), cnt = br.ReadInt32();
                        br.ReadBytes(sz * cnt);
                    }
                    else if (recType == 999) { br.ReadInt32(); break; }
                    else break; // unknown record — stop
                }
            }
            return labels;
        }

        // ─── Name helpers ────────────────────────────────────────────────────

        private static string GetStem(string v)
        {
            int i = v.LastIndexOf('_');
            return i > 0 ? v.Substring(0, i) : v;
        }

        private static string GetIndex(string v)
        {
            int i = v.LastIndexOf('_');
            return i >= 0 ? v.Substring(i + 1) : "";
        }

        private static string BinVar(string v, string metric)
            => GetStem(v) + metric + "_" + GetIndex(v);

        private static string MVarName(string v)  => "m"  + GetStem(v) + "_" + GetIndex(v);
        private static string SdVarName(string v) => "sd" + GetStem(v) + "_" + GetIndex(v);
        private static string SeVarName(string v) => "se" + GetStem(v) + "_" + GetIndex(v);

        private static string GetSpssLabel(string origVar, Dictionary<string, string> labels)
        {
            if (labels.TryGetValue(origVar, out string lbl)) return lbl ?? "";
            return "";
        }

        // ─── Syntax generation ───────────────────────────────────────────────

        private static string GenerateSyntax(
            List<MetricSet> sets,
            Dictionary<string, string> labels,
            string savPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("* Encoding: windows-1252.");
            sb.AppendLine();

            foreach (var s in sets)
            {
                int srcW  = s.Vars.Max(v => v.Length);
                int idxW  = s.Vars.Count.ToString().Length;

                // Helper: sort metric values descending for natural IF ordering
                List<string> Desc(List<string> vals)
                    => vals?.OrderByDescending(x => { int.TryParse(x, out int n); return n; }).ToList();

                var tbVals  = Desc(s.TbValues);
                var t2bVals = Desc(s.T2bValues);
                var b2bVals = Desc(s.B2bValues);
                var bbVals  = Desc(s.BbValues);
                var jrVals  = Desc(s.JrValues);

                // IF block for a binary metric
                void WriteIfBlock(string metric, List<string> vals)
                {
                    if (vals == null) return;
                    int dstW = s.Vars.Max(v => BinVar(v, metric).Length);
                    for (int i = 0; i < s.Vars.Count; i++)
                    {
                        string src = s.Vars[i].PadRight(srcW);
                        string dst = BinVar(s.Vars[i], metric).PadRight(dstW);
                        string cond = vals.Count == 1
                            ? $"{src}={vals[0]}"
                            : string.Join(" OR ", vals.Select(vv => $"{src}={vv}"));
                        sb.AppendLine($"IF {cond} {dst}={(i+1).ToString().PadLeft(idxW)}.");
                    }
                    sb.AppendLine();
                }

                WriteIfBlock("TB",  tbVals);
                WriteIfBlock("T2B", t2bVals);
                WriteIfBlock("B2B", b2bVals);
                WriteIfBlock("BB",  bbVals);

                // COMPUTE + SYSMIS block (Mean, SD, SE — grouped per variable)
                if (s.MeanValues != null)
                {
                    string anyList = string.Join(",", s.MeanValues);
                    int mW  = s.Vars.Max(v => MVarName(v).Length);
                    int sdW = s.Vars.Max(v => SdVarName(v).Length);
                    int seW = s.Vars.Max(v => SeVarName(v).Length);
                    int cmW = Math.Max(mW, Math.Max(sdW, seW)); // unified column for COMPUTE

                    for (int i = 0; i < s.Vars.Count; i++)
                    {
                        string orig = s.Vars[i];
                        string src  = orig.PadRight(srcW);
                        string mv   = MVarName(orig).PadRight(cmW);
                        string sdv  = SdVarName(orig).PadRight(cmW);
                        string sev  = SeVarName(orig).PadRight(cmW);

                        sb.AppendLine($"COMPUTE {mv}={src}.");
                        sb.AppendLine($"IF NOT(ANY({orig},{anyList})) {mv.TrimEnd()}=$SYSMIS.");
                        sb.AppendLine($"COMPUTE {sdv}={src}.");
                        sb.AppendLine($"IF NOT(ANY({orig},{anyList})) {sdv.TrimEnd()}=$SYSMIS.");
                        sb.AppendLine($"COMPUTE {sev}={src}.");
                        sb.AppendLine($"IF NOT(ANY({orig},{anyList})) {sev.TrimEnd()}=$SYSMIS.");
                    }
                    sb.AppendLine();
                }

                WriteIfBlock("JR", jrVals);

                // VALUE LABELS per block (binary metrics only)
                var activeMetrics = new List<string>();
                if (tbVals  != null) activeMetrics.Add("TB");
                if (t2bVals != null) activeMetrics.Add("T2B");
                if (b2bVals != null) activeMetrics.Add("B2B");
                if (bbVals  != null) activeMetrics.Add("BB");
                if (jrVals  != null) activeMetrics.Add("JR");

                if (activeMetrics.Count > 0)
                {
                    sb.AppendLine("VALUE LABELS");
                    foreach (var m in activeMetrics)
                        foreach (var v in s.Vars)
                            sb.AppendLine(BinVar(v, m));

                    for (int i = 0; i < s.Vars.Count; i++)
                    {
                        string lbl = GetSpssLabel(s.Vars[i], labels);
                        if (string.IsNullOrEmpty(lbl)) lbl = s.Vars[i];
                        sb.AppendLine($"{(i+1).ToString().PadRight(idxW + 1)}\"{lbl}\"");
                    }
                    sb.AppendLine(".");
                    sb.AppendLine();
                }
            }

            // Combined VARIABLE LABELS at end
            var entries = new List<KeyValuePair<string, string>>();
            foreach (var s in sets)
            {
                if (s.TbValues  != null) foreach (var v in s.Vars) entries.Add(new KeyValuePair<string, string>(BinVar(v,"TB"),  BinVar(v,"TB")));
                if (s.T2bValues != null) foreach (var v in s.Vars) entries.Add(new KeyValuePair<string, string>(BinVar(v,"T2B"), BinVar(v,"T2B")));
                if (s.B2bValues != null) foreach (var v in s.Vars) entries.Add(new KeyValuePair<string, string>(BinVar(v,"B2B"), BinVar(v,"B2B")));
                if (s.BbValues  != null) foreach (var v in s.Vars) entries.Add(new KeyValuePair<string, string>(BinVar(v,"BB"),  BinVar(v,"BB")));
                if (s.MeanValues != null)
                {
                    foreach (var v in s.Vars)
                    {
                        string spssLbl = GetSpssLabel(v, labels);
                        string mLabel  = string.IsNullOrEmpty(spssLbl) ? MVarName(v) : spssLbl;
                        entries.Add(new KeyValuePair<string, string>(MVarName(v),  mLabel));
                        entries.Add(new KeyValuePair<string, string>(SdVarName(v), "S.D."));
                        entries.Add(new KeyValuePair<string, string>(SeVarName(v), "S.E."));
                    }
                }
                if (s.JrValues  != null) foreach (var v in s.Vars) entries.Add(new KeyValuePair<string, string>(BinVar(v,"JR"),  BinVar(v,"JR")));
            }

            if (entries.Count > 0)
            {
                int maxW = entries.Max(e => e.Key.Length);
                sb.AppendLine("VARIABLE LABELS");
                foreach (var entry in entries)
                    sb.AppendLine($"{entry.Key.PadRight(maxW + 1)}\"{entry.Value}\"");
                sb.AppendLine(".");
            }

            sb.AppendLine("EXECUTE.");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(savPath))
            {
                sb.AppendLine("*Please specify the file save path.");
                sb.AppendLine($"SAVE OUTFILE='{savPath}'");
                sb.AppendLine("/COMPRESSED.");
            }

            return sb.ToString();
        }
    }
}
