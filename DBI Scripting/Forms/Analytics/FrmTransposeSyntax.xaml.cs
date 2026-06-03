using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SpssLib.DataReader;
using SpssLib.SpssDataset;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmTransposeSyntax.xaml
    /// </summary>
    public partial class FrmTransposeSyntax : Window
    {
        public FrmTransposeSyntax()
        {
            InitializeComponent();
        }

        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtImageVarFile.Text = openFileDialog1.FileName;
                    Properties.Settings.Default.StartupPath = txtImageVarFile.Text.Substring(0, txtImageVarFile.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.Save();
                }
                else
                    txtImageVarFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowseImageSpss_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "SPSS Dataset (*.sav)|*.sav|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtImageSpssFile.Text = openFileDialog1.FileName;
                    Properties.Settings.Default.StartupPath = txtImageSpssFile.Text.Substring(0, txtImageSpssFile.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.Save();
                }
                else
                    txtImageSpssFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRunTranspose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtImageVarFile.Text == "" || !File.Exists(txtImageVarFile.Text))
                {
                    MessageBox.Show("Please select a valid image variable text file.");
                    return;
                }

                string filePath = txtImageVarFile.Text;
                string outputPath = filePath.Substring(0, filePath.LastIndexOf('\\')) + "\\ImageryTransposeSyntax.sps";

                // Parse variables — group by prefix before last '_'
                List<string> groupOrder = new List<string>();
                Dictionary<string, List<string>> groupSuffixes = new Dictionary<string, List<string>>();

                foreach (string rawLine in File.ReadAllLines(filePath))
                {
                    string line = rawLine.Trim();
                    if (line == "") continue;

                    int lastUnderscore = line.LastIndexOf('_');
                    if (lastUnderscore < 0) continue;

                    string prefix = line.Substring(0, lastUnderscore);
                    string suffix = line.Substring(lastUnderscore + 1);

                    if (!groupSuffixes.ContainsKey(prefix))
                    {
                        groupSuffixes[prefix] = new List<string>();
                        groupOrder.Add(prefix);
                    }
                    groupSuffixes[prefix].Add(suffix);
                }

                if (groupOrder.Count == 0)
                {
                    MessageBox.Show("No variables found in the file.");
                    return;
                }

                // Find common stem = longest common prefix of all group prefixes
                string stem = groupOrder[0];
                for (int i = 1; i < groupOrder.Count; i++)
                    stem = GetCommonPrefix(stem, groupOrder[i]);

                // Brand numbers = group prefix minus stem (e.g. Q191 - Q19 = 1)
                List<string> brandNumbers = new List<string>();
                foreach (string grp in groupOrder)
                    brandNumbers.Add(grp.Substring(stem.Length));

                // Value codes = suffixes of the first group (in file order)
                List<string> valueCodes = groupSuffixes[groupOrder[0]];

                // Validate SPSS file
                if (txtImageSpssFile.Text == "" || !File.Exists(txtImageSpssFile.Text))
                {
                    MessageBox.Show("Please select a valid SPSS dataset.");
                    return;
                }

                // Read variable labels from SPSS dataset
                Dictionary<string, string> dicVarLabel = new Dictionary<string, string>();
                using (FileStream fileStream = new FileStream(txtImageSpssFile.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                {
                    SpssReader spssDataset = new SpssReader(fileStream);
                    foreach (Variable variable in spssDataset.Variables)
                    {
                        if (variable.Label != null && variable.Label.ToString().Trim() != "")
                            dicVarLabel[variable.Name] = variable.Label.ToString().Trim();
                    }
                }

                // Generate SPSS syntax
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (string vc in valueCodes)
                    {
                        // NUMERIC declaration for this value code
                        string numericVars = "";
                        foreach (string brand in brandNumbers)
                            numericVars += stem + "B" + vc + "_" + brand + " ";
                        writer.WriteLine("NUMERIC " + numericVars.Trim() + " (F8.0).");
                        writer.WriteLine();

                        // IF statements for this value code
                        foreach (string brand in brandNumbers)
                        {
                            string srcVar = stem + brand + "_" + vc;
                            string newVar = stem + "B" + vc + "_" + brand;
                            writer.WriteLine("IF " + srcVar + "=" + vc + " " + newVar + "=" + brand + ".");
                        }
                        writer.WriteLine();

                        // VARIABLE LABELS — brand name from last part after ':' in first group's variable
                        string brandName = "";
                        string firstGroupVar = stem + brandNumbers[0] + "_" + vc;
                        if (dicVarLabel.ContainsKey(firstGroupVar))
                        {
                            string fullLabel = dicVarLabel[firstGroupVar];
                            int lastColon = fullLabel.LastIndexOf(':');
                            brandName = lastColon >= 0
                                ? fullLabel.Substring(lastColon + 1).Trim()
                                : fullLabel.Trim();
                        }

                        writer.WriteLine("VARIABLE LABELS");
                        foreach (string brand in brandNumbers)
                            writer.WriteLine("  " + stem + "B" + vc + "_" + brand + " \"Brand Imagery - " + brandName + "\"");
                        writer.WriteLine("  .");
                        writer.WriteLine();

                        // VALUE LABELS — brand numbers labelled with each group's statement (first part before ':')
                        string rankVarsImg = "";
                        foreach (string brand in brandNumbers)
                            rankVarsImg += stem + "B" + vc + "_" + brand + " ";
                        writer.WriteLine("VALUE LABELS " + rankVarsImg.Trim());

                        foreach (string brand in brandNumbers)
                        {
                            string srcVar = stem + brand + "_" + vc;
                            string valLabelText = "";
                            if (dicVarLabel.ContainsKey(srcVar))
                            {
                                string fullLabel = dicVarLabel[srcVar];
                                int firstColon = fullLabel.IndexOf(':');
                                valLabelText = firstColon >= 0
                                    ? fullLabel.Substring(0, firstColon).Trim()
                                    : fullLabel.Trim();
                            }
                            writer.WriteLine("  " + brand + " \"" + valLabelText + "\"");
                        }
                        writer.WriteLine("  .");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show("Imagery transpose syntax generated successfully.\n\n" + outputPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImageHelp_Click(object sender, RoutedEventArgs e)
        {
            string description =
                "List all imagery variables, one per line.\n\n" +
                "Each variable must follow this pattern:\n" +
                "  {Stem}{BrandNumber}_{ValueCode}\n\n" +
                "Rules:\n" +
                "  \u2022 Variables are grouped by everything before the last underscore (the brand prefix).\n" +
                "  \u2022 The longest common part of all brand prefixes becomes the stem.\n" +
                "  \u2022 The remaining part of each prefix becomes the brand number.\n" +
                "  \u2022 All brands must share the same set of value codes.";

            string example =
                "Example  (stem = Q19,  brands = 1 2 3,  value codes = 1 2 3)\n" +
                "\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\n" +
                "Q191_1\n" +
                "Q191_2\n" +
                "Q191_3\n" +
                "Q192_1\n" +
                "Q192_2\n" +
                "Q192_3\n" +
                "Q193_1\n" +
                "Q193_2\n" +
                "Q193_3";

            ShowHelpDialog("Image Var Path \u2014 File Format Help", description, example);
        }

        private void btnRankHelp_Click(object sender, RoutedEventArgs e)
        {
            string description =
                "Define one or more rank question blocks, each separated by a blank line.\n\n" +
                "Each block must start with a header line:\n" +
                "  {VarStem}_RANK={N}      (N = number of rank positions)\n\n" +
                "Then list all item variables for that question, one per line.\n\n" +
                "Rules:\n" +
                "  \u2022 One blank line must separate each block.\n" +
                "  \u2022 The header line is case-insensitive (_rank= also works).\n" +
                "  \u2022 Item variable names must begin with VarStem followed by an underscore.";

            string example =
                "Example  (Q10 with 3 rank positions,  Q20 with 2 rank positions)\n" +
                "\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\u2500\n" +
                "Q10_RANK=3\n" +
                "Q10_1\n" +
                "Q10_2\n" +
                "Q10_3\n" +
                "Q10_4\n" +
                "Q10_5\n" +
                "Q10_6\n" +
                "Q10_7\n" +
                "Q10_8\n" +
                "Q10_9\n" +
                "Q10_10\n" +
                "\n" +
                "Q20_RANK=2\n" +
                "Q20_1\n" +
                "Q20_2\n" +
                "Q20_3\n" +
                "Q20_4\n" +
                "Q20_5";

            ShowHelpDialog("Rank Var Path \u2014 File Format Help", description, example);
        }

        private void ShowHelpDialog(string title, string description, string example)
        {
            Window w = new Window
            {
                Title = title,
                Width = 520,
                Height = 420,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            Grid grid = new Grid { Margin = new Thickness(16) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            TextBlock desc = new TextBlock
            {
                Text = description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(desc, 0);
            grid.Children.Add(desc);

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 12)
            };
            ScrollViewer scroll = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            TextBox tb = new TextBox
            {
                Text = example,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12,
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.NoWrap
            };
            scroll.Content = tb;
            border.Child = scroll;
            Grid.SetRow(border, 1);
            grid.Children.Add(border);

            Button ok = new Button { Content = "OK", Width = 80, HorizontalAlignment = HorizontalAlignment.Right };
            ok.Click += (s, e2) => w.Close();
            Grid.SetRow(ok, 2);
            grid.Children.Add(ok);

            w.Content = grid;
            w.ShowDialog();
        }

        private string GetCommonPrefix(string a, string b)
        {
            int len = Math.Min(a.Length, b.Length);
            int i = 0;
            while (i < len && a[i] == b[i]) i++;
            return a.Substring(0, i);
        }

        private void btnBrowseRank_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtRankFile.Text = openFileDialog1.FileName;
                    Properties.Settings.Default.StartupPath = txtRankFile.Text.Substring(0, txtRankFile.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.Save();
                }
                else
                    txtRankFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRunRank_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtRankFile.Text == "" || !File.Exists(txtRankFile.Text))
                {
                    MessageBox.Show("Please select a valid rank text file.");
                    return;
                }

                if (txtSpssFile.Text == "" || !File.Exists(txtSpssFile.Text))
                {
                    MessageBox.Show("Please select a valid SPSS dataset.");
                    return;
                }

                string filePath = txtRankFile.Text;
                string outputPath = filePath.Substring(0, filePath.LastIndexOf('\\')) + "\\RankTransposeSyntax.sps";

                // Parse blocks from the text file
                List<RankBlock> blocks = new List<RankBlock>();
                string currentStem = "";
                int currentRank = 0;
                List<string> currentItems = new List<string>();

                foreach (string rawLine in File.ReadAllLines(filePath))
                {
                    string line = rawLine.Trim();

                    if (line == "")
                    {
                        if (currentStem != "" && currentItems.Count > 0)
                        {
                            blocks.Add(new RankBlock(currentStem, currentRank, new List<string>(currentItems)));
                            currentStem = "";
                            currentRank = 0;
                            currentItems.Clear();
                        }
                    }
                    else if (line.ToUpper().Contains("_RANK="))
                    {
                        int sepIdx = line.ToUpper().IndexOf("_RANK=");
                        currentStem = line.Substring(0, sepIdx);
                        int.TryParse(line.Substring(sepIdx + 6), out currentRank);
                        currentItems.Clear();
                    }
                    else
                    {
                        currentItems.Add(line);
                    }
                }

                if (currentStem != "" && currentItems.Count > 0)
                    blocks.Add(new RankBlock(currentStem, currentRank, new List<string>(currentItems)));

                // Read variable labels from SPSS dataset
                Dictionary<string, string> dicVarLabel = new Dictionary<string, string>();
                using (FileStream fileStream = new FileStream(txtSpssFile.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                {
                    SpssReader spssDataset = new SpssReader(fileStream);
                    foreach (Variable variable in spssDataset.Variables)
                    {
                        if (variable.Label != null && variable.Label.ToString().Trim() != "")
                            dicVarLabel[variable.Name] = variable.Label.ToString().Trim();
                    }
                }

                // Generate SPSS syntax
                using (StreamWriter writer = new StreamWriter(outputPath))
                {
                    foreach (RankBlock block in blocks)
                    {
                        // NUMERIC declaration
                        string numericVars = "";
                        for (int r = 1; r <= block.RankCount; r++)
                            numericVars += block.VarStem + "_R" + r + " ";
                        writer.WriteLine("NUMERIC " + numericVars.Trim() + " (F8.0).");
                        writer.WriteLine();

                        // IF statements per rank position
                        for (int r = 1; r <= block.RankCount; r++)
                        {
                            string newVar = block.VarStem + "_R" + r;
                            foreach (string itemVar in block.Items)
                            {
                                string suffix = itemVar.Substring(block.VarStem.Length + 1);
                                writer.WriteLine("IF (" + itemVar + "=" + r + ") " + newVar + "=" + suffix + ".");
                            }
                            writer.WriteLine();
                        }

                        // VARIABLE LABELS for this group's rank variables
                        string varLabelText = "";
                        if (block.Items.Count > 0 && dicVarLabel.ContainsKey(block.Items[0]))
                        {
                            string fullLabel = dicVarLabel[block.Items[0]];
                            int firstColon = fullLabel.IndexOf(':');
                            varLabelText = firstColon >= 0
                                ? fullLabel.Substring(0, firstColon).Trim()
                                : fullLabel.Trim();
                        }

                        writer.WriteLine("VARIABLE LABELS");
                        for (int r = 1; r <= block.RankCount; r++)
                            writer.WriteLine("  " + block.VarStem + "_R" + r + " \"" + varLabelText + ": Rank " + r + "\"");
                        writer.WriteLine("  .");
                        writer.WriteLine();

                        // VALUE LABELS immediately after this group's IF block
                        string rankVars = "";
                        for (int r = 1; r <= block.RankCount; r++)
                            rankVars += block.VarStem + "_R" + r + " ";

                        writer.WriteLine("VALUE LABELS " + rankVars.Trim());

                        foreach (string itemVar in block.Items)
                        {
                            string suffix = itemVar.Substring(block.VarStem.Length + 1);
                            string label = "";

                            if (dicVarLabel.ContainsKey(itemVar))
                            {
                                string fullLabel = dicVarLabel[itemVar];
                                int lastColon = fullLabel.LastIndexOf(':');
                                label = lastColon >= 0
                                    ? fullLabel.Substring(lastColon + 1).Trim()
                                    : fullLabel.Trim();
                            }

                            writer.WriteLine("  " + suffix + " \"" + label + "\"");
                        }

                        writer.WriteLine("  .");
                        writer.WriteLine();

                        // FREQUENCIES after VALUE LABELS
                        writer.WriteLine("FREQUENCIES " + rankVars.Trim() + ".");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show("Rank transpose syntax generated successfully.\n\n" + outputPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowseSpss_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "SPSS Dataset (*.sav)|*.sav|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtSpssFile.Text = openFileDialog1.FileName;
                    Properties.Settings.Default.StartupPath = txtSpssFile.Text.Substring(0, txtSpssFile.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.Save();
                }
                else
                    txtSpssFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

    internal class RankBlock
    {
        public string VarStem { get; private set; }
        public int RankCount { get; private set; }
        public List<string> Items { get; private set; }

        public RankBlock(string varStem, int rankCount, List<string> items)
        {
            VarStem = varStem;
            RankCount = rankCount;
            Items = items;
        }
    }
}
