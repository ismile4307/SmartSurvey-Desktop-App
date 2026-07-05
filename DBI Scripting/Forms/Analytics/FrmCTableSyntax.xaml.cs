using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SpssLib.DataReader;
using SpssLib.SpssDataset;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmCTableSyntax.xaml
    /// </summary>
    public partial class FrmCTableSyntax : Window
    {
        private string myPath;
        private string bannerText1 = "";
        private string bannerText2 = "";
        private string outputExcelFileName = "";
        private string txtPath = "";

        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;

        private TextWriter txt_writer;

        private List<string> lstWorkSheetName = new List<string>();

        private List<string> lstTableType = new List<string>();
        private List<string> lstVariableName = new List<string>();
        private List<string> lstVariableLabel = new List<string>();

        private List<string> lstMRUniqueVariableName = new List<string>();
        private List<string> lstMRBreakPoint = new List<string>();
        private List<string> lstMRVariableLabel = new List<string>();

        private List<string> lstFilterCondition = new List<string>();
        private List<string> lstFilterLabel = new List<string>();

        private string mrVarList = "";
        private string type8VarList = "";
        private string analysisType = "";
        private string _currentSyntaxFilePath = "";

        public FrmCTableSyntax()
        {
            InitializeComponent();
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
                    txtSpssDataFile.Text = openFileDialog1.FileName;
                    myPath = txtSpssDataFile.Text.Substring(0, txtSpssDataFile.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtSpssDataFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtStructureExcelPath.Text = openFileDialog1.FileName;
                    this.loadCategoryList();
                    myPath = txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtStructureExcelPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowseBanner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Text Files(*.*txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtBannerFilePath.Text = openFileDialog1.FileName;
                    myPath = txtBannerFilePath.Text.Substring(0, txtBannerFilePath.Text.LastIndexOf('\\'));
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    this.getBannerText();

                }
                else
                    txtBannerFilePath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSpssDataFile.Text) || !File.Exists(txtSpssDataFile.Text))
            {
                MessageBox.Show("Please select a valid SPSS Data File (.sav) before executing.");
                return;
            }

            this.saveExcellSheetAsText();
            this.loadAllList();


            if (radioBtnRecodeSyntax.IsChecked == true)
            {
                if (createFile("Recode"))
                {
                    if (this.recodeScript())
                    {
                        txt_writer.Close();
                        MessageBox.Show("Write Completed");
                    }
                }
            }
            else if (radioBtnPctTableSyntax.IsChecked == true)
            {
                if (createFile("Table_Cpt"))
                {

                    analysisType = "COLPCT.COUNT F40." + txtDecimalPlace.Text;
                    this.prepareScript();
                    txt_writer.Close();
                    MessageBox.Show("Write Completed");
                }
            }
            else if (radioBtnPct2TableSyntax.IsChecked == true)
            {
                if (createFile("Table_Cpt_Pct"))
                {
                    analysisType = "COLPCT.COUNT PCT8.0";
                    this.prepareScript();
                    txt_writer.Close();
                    MessageBox.Show("Write Completed");
                }
            }
            else
            {
                if (createFile("Table_Count"))
                {
                    analysisType = "COUNT F40.0";
                    this.prepareScript();
                    txt_writer.Close();
                    MessageBox.Show("Write Completed");
                }
            }



        }

        //***************************** Recode Script ***************************************
        private bool recodeScript()
        {
            // ── Read SPSS variable metadata (for value labels) ──────────────
            var dicNameVsVariable = new Dictionary<string, Variable>();
            using (var fs = new FileStream(txtSpssDataFile.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
            {
                SpssReader spssReader = new SpssReader(fs);
                foreach (Variable variable in spssReader.Variables)
                    dicNameVsVariable[variable.Name] = variable;
            }

            // ── Read Banner Variables sheet from structure Excel (6 columns) ──
            var bannerGroupOrder = new List<string>();
            var dicBannerSpssVars = new Dictionary<string, string>();
            var dicBannerLabel = new Dictionary<string, string>();
            var dicBannerCodes = new Dictionary<string, List<string>>();
            var dicBannerNewLabels = new Dictionary<string, List<string>>();
            var dicBannerConditions = new Dictionary<string, List<string>>();

            Excel.Application bannerXlApp = new Excel.Application();
            Excel.Workbook bannerXlWorkBook = bannerXlApp.Workbooks.Open(txtStructureExcelPath.Text, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            try
            {
                foreach (Excel.Worksheet ws in bannerXlWorkBook.Worksheets)
                {
                    if (ws.Name != "Banner Variables") continue;
                    Excel.Range range = ws.UsedRange;
                    for (int bi = 2; bi <= range.Rows.Count; bi++)
                    {
                        object c1 = ws.Cells[bi, 1].Value2;
                        object c2 = ws.Cells[bi, 2].Value2;
                        object c3 = ws.Cells[bi, 3].Value2;
                        object c4 = ws.Cells[bi, 4].Value2;
                        object c5 = ws.Cells[bi, 5].Value2;
                        object c6 = ws.Cells[bi, 6].Value2;

                        string spssV = c1 != null ? c1.ToString().Trim() : "";
                        string bannerV = c2 != null ? c2.ToString().Trim() : "";
                        string labelV = c3 != null ? c3.ToString().Trim() : "";
                        string newCode = c4 != null ? c4.ToString().Trim() : "";
                        string newLabel = c5 != null ? c5.ToString().Trim() : "";
                        string condition = c6 != null ? c6.ToString().Trim() : "";

                        if (string.IsNullOrWhiteSpace(spssV) || string.IsNullOrWhiteSpace(bannerV))
                            continue;

                        if (!bannerGroupOrder.Contains(bannerV))
                        {
                            bannerGroupOrder.Add(bannerV);
                            dicBannerSpssVars[bannerV] = spssV;
                            dicBannerLabel[bannerV] = labelV;
                            dicBannerCodes[bannerV] = new List<string>();
                            dicBannerNewLabels[bannerV] = new List<string>();
                            dicBannerConditions[bannerV] = new List<string>();
                        }
                        else if (string.IsNullOrWhiteSpace(dicBannerLabel[bannerV]) && !string.IsNullOrWhiteSpace(labelV))
                        {
                            dicBannerLabel[bannerV] = labelV;
                        }

                        dicBannerCodes[bannerV].Add(newCode);
                        dicBannerNewLabels[bannerV].Add(newLabel);
                        dicBannerConditions[bannerV].Add(condition);
                    }
                    break;
                }
            }
            finally
            {
                bannerXlWorkBook.Close(false);
                bannerXlApp.Quit();
                releaseObject(bannerXlWorkBook);
                releaseObject(bannerXlApp);
            }

            // ── Validate Banner Variables ─────────────────────────────────────
            var validationErrors = new List<string>();

            foreach (string bannerV in bannerGroupOrder)
            {
                var codes = dicBannerCodes[bannerV];
                var newLabels = dicBannerNewLabels[bannerV];
                var conditions = dicBannerConditions[bannerV];
                string spssVars = dicBannerSpssVars[bannerV];
                bool isMultiVar = spssVars.Trim().Contains(" ");

                // Row-level: New Code / New Label / Condition must all be filled or all empty
                for (int vi = 0; vi < codes.Count; vi++)
                {
                    bool hasCode = !string.IsNullOrWhiteSpace(codes[vi]);
                    bool hasLabel = !string.IsNullOrWhiteSpace(newLabels[vi]);
                    bool hasCond = !string.IsNullOrWhiteSpace(conditions[vi]);
                    if ((hasCode || hasLabel || hasCond) && !(hasCode && hasLabel && hasCond))
                    {
                        validationErrors.Add("'" + bannerV + "' row " + (vi + 1) + ": New Code, New Label and Condition must all be filled or all empty.");
                        break;
                    }
                }

                // Group-level: cannot mix filled and empty rows
                bool anyFilled = codes.Any(c => !string.IsNullOrWhiteSpace(c));
                bool anyEmpty = codes.Any(c => string.IsNullOrWhiteSpace(c));
                if (anyFilled && anyEmpty)
                    validationErrors.Add("'" + bannerV + "': cannot mix rows with and without custom grouping (New Code).");

                if (anyFilled)
                {
                    // Duplicate New Code check
                    var filledCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    if (filledCodes.Count != filledCodes.Distinct().Count())
                        validationErrors.Add("'" + bannerV + "': duplicate New Code values found.");

                    // Mode 2 (single var RECODE): New Code must be numeric
                    if (!isMultiVar)
                    {
                        foreach (string code in filledCodes)
                        {
                            if (!int.TryParse(code, out _))
                                validationErrors.Add("'" + bannerV + "': New Code '" + code + "' must be a whole number for RECODE.");
                        }
                    }
                }
                else
                {
                    // Mode 1: SPSS Variables must be a single variable and exist in the dataset
                    if (isMultiVar)
                        validationErrors.Add("'" + bannerV + "': multiple SPSS variables listed but no custom grouping (New Code) provided.");
                    else if (!dicNameVsVariable.ContainsKey(spssVars.Trim()))
                        validationErrors.Add("'" + bannerV + "': SPSS variable '" + spssVars.Trim() + "' was not found in the dataset.");
                }
            }

            if (validationErrors.Count > 0)
            {
                txt_writer.Close();
                try { File.Delete(_currentSyntaxFilePath); } catch { }
                MessageBox.Show("Banner Variables validation failed:\n\n" + string.Join("\n\n", validationErrors));
                return false;
            }

            txt_writer.WriteLine("");
            //txt_writer.WriteLine("*****************************Allah is Almighty*******************************");

            txt_writer.WriteLine("*Please specify the file get path.");
            txt_writer.WriteLine("GET ");
            txt_writer.WriteLine(@"FILE='" + myPath + "\\Data Preparation\\" + outputExcelFileName + "_Final.sav'.");
            txt_writer.WriteLine("DATASET NAME DataSet1 WINDOW=FRONT.");

            txt_writer.WriteLine("");

            txt_writer.WriteLine("*************************************Banner************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("COMPUTE ATotal=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS ATotal ""Total"".");
            txt_writer.WriteLine(@"VALUE LABELS ATotal 1 ""Total"".");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("NUMERIC nBlank (f8.0).");
            txt_writer.WriteLine("COMPUTE nBlank=99.");
            txt_writer.WriteLine(@"VALUE LABELS nBlank 99 ""DUMMY ROW"".");
            txt_writer.WriteLine(@"VARIABLE LEVEL nBlank(NOMINAL).");
            txt_writer.WriteLine("");




            // ── Generate banner syntax (mode-based) ──────────────────────────
            foreach (string bannerV in bannerGroupOrder)
            {
                string spssVars = dicBannerSpssVars[bannerV];
                string bannerLbl = dicBannerLabel[bannerV];
                var codes = dicBannerCodes[bannerV];
                var newLabels = dicBannerNewLabels[bannerV];
                var conds = dicBannerConditions[bannerV];
                bool isCustom = codes.Any(c => !string.IsNullOrWhiteSpace(c));
                bool isMultiVar = spssVars.Trim().Contains(" ");

                txt_writer.WriteLine("NUMERIC " + bannerV + " (f8.0).");

                if (!isCustom)
                {
                    // Mode 1: direct copy
                    txt_writer.WriteLine("COMPUTE " + bannerV + " = " + spssVars.Trim() + ".");
                }
                else if (!isMultiVar)
                {
                    // Mode 2: RECODE (single source variable, grouped values)
                    var sb = new System.Text.StringBuilder("RECODE " + spssVars.Trim() + " ");
                    for (int ri = 0; ri < codes.Count; ri++)
                    {
                        string cond = conds[ri].Trim();
                        if (cond.ToUpper().Contains("THRU"))
                            sb.Append("(" + cond + "=" + codes[ri] + ")");
                        else
                        {
                            string joined = string.Join(",", cond.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                            sb.Append("(" + joined + "=" + codes[ri] + ")");
                        }
                    }
                    sb.Append("(ELSE=SYSMIS) INTO " + bannerV + ".");
                    txt_writer.WriteLine(sb.ToString());
                }
                else
                {
                    // Mode 3: IF statements (multiple source variables)
                    for (int ri = 0; ri < codes.Count; ri++)
                        txt_writer.WriteLine("IF (" + conds[ri] + ") " + bannerV + "=" + codes[ri] + ".");
                }

                if (!string.IsNullOrWhiteSpace(bannerLbl))
                    txt_writer.WriteLine("VARIABLE LABELS " + bannerV + " \"" + bannerLbl + "\".");

                if (!isCustom)
                {
                    // Value labels from SPSS dataset
                    if (dicNameVsVariable.TryGetValue(spssVars.Trim(), out Variable spssVar)
                        && spssVar.ValueLabels != null && spssVar.ValueLabels.Count > 0)
                    {
                        txt_writer.WriteLine("VALUE LABELS " + bannerV);
                        foreach (var kv in spssVar.ValueLabels.OrderBy(k => k.Key))
                            txt_writer.WriteLine("  " + (int)kv.Key + " \"" + kv.Value + "\"");
                        txt_writer.WriteLine(".");
                    }
                }
                else
                {
                    // Value labels from New Label column
                    txt_writer.WriteLine("VALUE LABELS " + bannerV);
                    for (int ri = 0; ri < codes.Count; ri++)
                        txt_writer.WriteLine("  " + codes[ri] + " \"" + newLabels[ri] + "\"");
                    txt_writer.WriteLine(".");
                }

                txt_writer.WriteLine("");
            }

            //txt_writer.WriteLine("COMPUTE nBlank=1.");
            //txt_writer.WriteLine("RECODE nBlank (1=SYSMIS).");
            //txt_writer.WriteLine("");

            //txt_writer.WriteLine("COMPUTE SigTest=1.");
            //txt_writer.WriteLine("RECODE SigTest (1=SYSMIS).");
            //txt_writer.WriteLine(@"VARIABLE LABELS SigTest ""SIG. TEST"".");
            //txt_writer.WriteLine("");

            //txt_writer.WriteLine("COMPUTE Dummy=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            //txt_writer.WriteLine("");

            //txt_writer.WriteLine("COMPUTE aBase=1.");
            ////txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            //txt_writer.WriteLine(@"VALUE LABELS aBase 1 ""Base"".");
            //txt_writer.WriteLine("");



            //txt_writer.WriteLine("COMPUTE NPS=1.");
            //txt_writer.WriteLine("RECODE NPS (1=SYSMIS).");
            //txt_writer.WriteLine(@"VALUE LABELS NPS 1 ""NPS Score"".");
            //txt_writer.WriteLine("");

            //if (comAnalysisType.Text == "Weighted Analysis")
            //{
            //    txt_writer.WriteLine("COMPUTE uBase=1.");
            //    txt_writer.WriteLine(@"VALUE LABELS uBase 1 ""Base: Unweighted"".");
            //    txt_writer.WriteLine("");
            //}

            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("EXECUTE.");

            txt_writer.WriteLine("");


            //txt_writer.WriteLine("***************************************************************************");
            //txt_writer.WriteLine("");


            //txt_writer.WriteLine("* ADD FILES /FILE=*");
            //txt_writer.WriteLine(@" /FILE='D:\DBI Projects\2023 Projects\Data_Dummy.sav'.");
            //txt_writer.WriteLine("* EXECUTE.");

            //txt_writer.WriteLine("COMPUTE DummyATotal=1.");
            //txt_writer.WriteLine(@"VALUE LABELS DummyATotal 1 ""DummyTotal"".");

            //txt_writer.WriteLine("COMPUTE Dummy=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");

            //txt_writer.WriteLine("***************************************************************************");
            //txt_writer.WriteLine("");

            for (int i = 0; i < lstTableType.Count; i++)
            {
                if (lstTableType[i] == "2")
                {

                }
                else if (lstTableType[i] == "3")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "5")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "6")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN (Rev)\".");
                    txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (1=5)(2=4)(3=3)(4=2)(5=1).");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "7")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "8")
                {


                }
                else if (lstTableType[i] == "9")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "10")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "11")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S. D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S. E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "12")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"MEAN\".");
                    txt_writer.WriteLine("");

                    txt_writer.WriteLine("COMPUTE X_" + lstVariableName[i] + " = (" + lstVariableName[i] + " >= 9).");
                    txt_writer.WriteLine("COMPUTE Y_" + lstVariableName[i] + " = (" + lstVariableName[i] + " <= 6).");
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + " = (X_" + lstVariableName[i] + " - Y_" + lstVariableName[i] + ")*100.");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"NPS Score\".");

                }

                //txt_writer.Close();
            }
            txt_writer.WriteLine(" ");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("*Please specify the file save path.");
            txt_writer.WriteLine("SAVE OUTFILE='" + myPath + "\\" + outputExcelFileName + "_Final.sav'");
            txt_writer.WriteLine("/COMPRESSED.");

            return true;
        }

        //***************************** Column Percentage Script ***************************************
        private void prepareScript()
        {
            mrVarList = "";
            type8VarList = "";

            txt_writer.WriteLine("");

            txt_writer.WriteLine("*Please specify the file get path.");
            txt_writer.WriteLine("GET ");
            txt_writer.WriteLine(@"FILE='" + myPath + "\\" + outputExcelFileName + "_Final.sav'.");
            txt_writer.WriteLine("DATASET NAME DataSet1 WINDOW=FRONT.");

            txt_writer.WriteLine("");

            txt_writer.WriteLine("*************************************OMS Syntax************************************");


            //txt_writer.WriteLine("Define @ColVar() ATotal*Product+(Center+UserType)*Product  !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal Center UserType   Product  !Enddefine.");

            //txt_writer.WriteLine("Define @ColVar() ATotal+S1Banner+S2Banner+DhakaBanner+DhakaUrbanRuralBanner+CTGBanner+CTGUrbanRuralBanner+SECBanner+PBQ1Banner !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal S1Banner S2Banner DhakaBanner DhakaUrbanRuralBanner CTGBanner CTGUrbanRuralBanner SECBanner PBQ1Banner !Enddefine.");

            //txt_writer.WriteLine("Define @ColVar() " + bannerText1 + " !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() " + bannerText2 + " !Enddefine.");


            txt_writer.WriteLine("");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMS");
            txt_writer.WriteLine("/SELECT TABLES");
            txt_writer.WriteLine("/IF COMMANDS=['CTables'] SUBTYPES=['Custom Table']");
            txt_writer.WriteLine("/DESTINATION FORMAT=XLSX");
            txt_writer.WriteLine("Viewer = No");
            txt_writer.WriteLine(@"OUTFILE='" + myPath + "\\01." + outputExcelFileName + " -Cpt.xlsx'.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("*************************************Table Syntax************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("");

            if (comAnalysisType.Text == "Weighted Analysis")
            {
                txt_writer.WriteLine("WEIGHT BY Weight.");
            }
            txt_writer.WriteLine("");

            int i_TableNo = 1;
            string emptyBannerValue = chkExcludeEmptyBanner.IsChecked == true ? "EXCLUDE" : "INCLUDE";

            String nestedVairables = GetBannerNestedVariable(bannerText1);
            String bannerVariablesForLabel = GetBannerVariablesForLabel(bannerText1);

            for (int i = 0; i < lstTableType.Count; i++)
            {

                //************************************ For Table ************************************************
                if (lstTableType[i] == "1")
                {
                    //1	Single Response	CpcT

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }
                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");

                    if (chkIncludeBlankRow.IsChecked == true)
                    {
                        txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " nBlank DISPLAY=NONE");
                        txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + nBlank[C] [COUNT F40.0] BY " + bannerText1 + "");
                    }
                    else
                    {
                        txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");
                        txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] BY " + bannerText1 + "");
                    }
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " ORDER=A KEY=VALUE EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    if (chkIncludeBlankRow.IsChecked == true)
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=nBlank ORDER=A KEY=VALUE EMPTY=INCLUDE");
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "2")
                {
                    //6	Multiple Response	CpcT


                    mrVarList = mrVarList + lstVariableName[i] + " ";

                    if (lstMRBreakPoint[i] == "XXX")
                    {
                        bool addBlankRow = chkIncludeBlankRow.IsChecked == true
                            && !mrVarList.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Contains("nBlank");

                        txt_writer.WriteLine("* Define Multiple Response Sets.");
                        txt_writer.WriteLine("MRSETS");
                        txt_writer.WriteLine("  /MCGROUP NAME=$" + lstMRUniqueVariableName[i] + " LABEL='Spontaneous Awareness' VARIABLES=" + mrVarList + " ");
                        txt_writer.WriteLine("  /DISPLAY NAME=[$" + lstMRUniqueVariableName[i] + "].");
                        txt_writer.WriteLine("");

                        if (lstFilterCondition[i] != "")
                        {
                            txt_writer.WriteLine("TEMPORARY.");
                            txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("* Custom Tables.");
                        txt_writer.WriteLine("CTABLES");
                        txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");

                        if (addBlankRow)
                        {
                            txt_writer.WriteLine("  /VLABELS VARIABLES=$" + lstMRUniqueVariableName[i] + " " + nestedVairables + " nBlank DISPLAY=NONE");
                            txt_writer.WriteLine("  /TABLE $" + lstMRUniqueVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + nBlank[C] [COUNT F40.0] BY " + bannerText1 + "");
                        }
                        else
                        {
                            txt_writer.WriteLine("  /VLABELS VARIABLES=$" + lstMRUniqueVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");
                            txt_writer.WriteLine("  /TABLE $" + lstMRUniqueVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] BY " + bannerText1 + "");
                        }
                        txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=$" + lstMRUniqueVariableName[i] + " ORDER=A KEY=VALUE EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                        if (addBlankRow)
                            txt_writer.WriteLine("  /CATEGORIES VARIABLES=nBlank ORDER=A KEY=VALUE EMPTY=INCLUDE");
                        txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                        txt_writer.WriteLine("  /TITLES");
                        txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                        txt_writer.WriteLine("    CAPTION='Home'.");
                        txt_writer.WriteLine("");

                        mrVarList = "";
                        i_TableNo = i_TableNo + 1;
                    }
                }
                else if (lstTableType[i] == "3")
                {
                    // For Single Response With Mean


                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }
                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=m_" + lstVariableName[i] + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " ORDER=A KEY=VALUE EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;

                }
                else if (lstTableType[i] == "5")
                {
                    //1	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.D. S.E. 


                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([4]+[5])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [5/4]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"BOTTOM 2 BOX [1/2]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, &cat1, &cat2, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");


                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "6")
                {
                    //2	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.D. S.E. 
                    //2 For Reverse 
                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean (Rev)\".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (1=5)(2=4)(3=3)(4=2)(5=1).");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([4]+[5])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [1/2]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"BOTTOM 2 BOX [4/5]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [COLPCT.COUNT F40." + txtDecimalPlace.Text + ", TOTAL[COUNT F40.0]] +  m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, &cat1, &cat2, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");


                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "7")
                {
                    //3	Scaled Question (7)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    //10	Scaled Question (10)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([6]+[7])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [6/7]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([5]+[6]+[7])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"TOP 3 BOX [5/6/7]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat3 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat3 LABEL = \"BOTTOM 2 BOX [1/2]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat4 = EXPR([1]+[2]+[3])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat4 LABEL = \"BOTTOM 3 BOX [1/2/3]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, 6, 7, &cat1, &cat2, &cat3, &cat4, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "8")
                {
                    type8VarList = type8VarList + lstVariableName[i] + " ";

                    if (lstMRBreakPoint[i] == "XXX")
                    {
                        if (lstFilterCondition[i] != "")
                        {
                            txt_writer.WriteLine("TEMPORARY.");
                            txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("* Define Total Variable.");
                        txt_writer.WriteLine("COMPUTE Total=1.");
                        txt_writer.WriteLine("EXECUTE.");
                        txt_writer.WriteLine("");
                        txt_writer.WriteLine("VARIABLE LEVEL Total (SCALE).");
                        txt_writer.WriteLine("");

                        txt_writer.WriteLine("* Custom Tables.");
                        txt_writer.WriteLine("CTABLES");
                        txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                        txt_writer.WriteLine("  /VLABELS VARIABLES=" + nestedVairables + " DISPLAY=NONE");

                        var sb8 = new System.Text.StringBuilder("  /TABLE Total[S] [SUM F40.0]");
                        string[] vars8 = type8VarList.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string v in vars8)
                            sb8.Append("\n  + " + v + " [S][MEAN]");
                        sb8.Append("\n  BY " + bannerText1);
                        txt_writer.WriteLine(sb8.ToString());

                        txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                        txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                        txt_writer.WriteLine("  /TITLES");
                        txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                        txt_writer.WriteLine("    CAPTION='Home'.");
                        txt_writer.WriteLine("");

                        type8VarList = "";
                        i_TableNo = i_TableNo + 1;
                    }
                }
                else if (lstTableType[i] == "9")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    //10	Scaled Question (10)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([8]+[9])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [8/9]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([7]+[8]+[9])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"TOP 3 BOX [7/8/9]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat3 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat3 LABEL = \"BOTTOM 2 BOX [1/2]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat4 = EXPR([1]+[2]+[3])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat4 LABEL = \"BOTTOM 3 BOX [1/2/3]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, 6, 7, 8, 9, &cat1, &cat2, &cat3, &cat4, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "10")
                {
                    //10	Scaled Question (10)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([9]+[10])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [09/10]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([8]+[9]+[10])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"TOP 3 BOX [08/09/10]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat3 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat3 LABEL = \"BOTTOM 2 BOX [01/02]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat4 = EXPR([1]+[2]+[3])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat4 LABEL = \"BOTTOM 3 BOX [01/02/03]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, &cat1, &cat2, &cat3, &cat4, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "11")
                {
                    //11	Scaled Question (11)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    //txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    //txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    //txt_writer.WriteLine("");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([10]+[11])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"TOP 2 BOX [10/11]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([9]+[10]+[11])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"TOP 3 BOX [09/10/11]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat3 = EXPR([1]+[2])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat3 LABEL = \"BOTTOM 2 BOX [01/02]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat4 = EXPR([1]+[2]+[3])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat4 LABEL = \"BOTTOM 3 BOX [01/02/03]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] + sd_" + lstVariableName[i] + " [S][STDDEV F40.2] + se_" + lstVariableName[i] + " [S][SEMEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, &cat1, &cat2, &cat3, &cat4, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "12")
                {
                    //11	Scaled Question (NPS)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");
                    //txt_writer.WriteLine("");

                    //txt_writer.WriteLine("COMPUTE X_" + lstVariableName[i] + " = (" + lstVariableName[i] + " >= 9).");
                    //txt_writer.WriteLine("COMPUTE Y_" + lstVariableName[i] + " = (" + lstVariableName[i] + " <= 6).");
                    //txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + " = (X_" + lstVariableName[i] + " - Y_" + lstVariableName[i] + ")*100.");
                    //txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"NPS Score\".");

                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("TEMPORARY.");
                        txt_writer.WriteLine("SELECT IF " + lstFilterCondition[i] + ".");
                        txt_writer.WriteLine("");
                    }

                    txt_writer.WriteLine("* Custom Tables.");
                    txt_writer.WriteLine("CTABLES");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + bannerVariablesForLabel + " DISPLAY=LABEL");
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");

                    txt_writer.WriteLine("  /PCOMPUTE &cat1 = EXPR([0]+[1]+[2]+[3]+[4]+[5]+[6])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat1 LABEL = \"Detractors [0-6]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat2 = EXPR([7]+[8])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat2 LABEL = \"Passives [7-8]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");
                    txt_writer.WriteLine("  /PCOMPUTE &cat3 = EXPR([9]+[10])");
                    txt_writer.WriteLine("  /PPROPERTIES &cat3 LABEL = \"Promoters [9-10]\" FORMAT=COLPCT.VALIDN F40.1 HIDESOURCECATS=NO");

                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] + m_" + lstVariableName[i] + " [S][MEAN F40.2] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, &cat1, &cat2, &cat3, OTHERNM] EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=" + emptyBannerValue);
                    txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                    txt_writer.WriteLine("  /TITLES");
                    txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                    txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                    txt_writer.WriteLine("    CAPTION='Home'.");
                    txt_writer.WriteLine("");

                    i_TableNo = i_TableNo + 1;
                }

            }

            if (comAnalysisType.Text == "Weighted Analysis")
            {
                txt_writer.WriteLine("WEIGHT OFF.");
            }
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMSEND.");

            //txt_writer.Close();
        }

        //***************************** Table Count Script ***************************************

        private void saveExcellSheetAsText()
        {
            try
            {
                if (txtStructureExcelPath.Text != "")
                {
                    if (File.Exists(txtStructureExcelPath.Text) == true)
                    {
                        if (txtOutputFileName.Text != "")
                        {
                            List<String> lstTextFile = new List<string>();

                            if (lstWorkSheetName.Count > 0)
                            {


                                xlApp = new Excel.Application();
                                xlWorkBook = xlApp.Workbooks.Open(txtStructureExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                                lstTextFile.Clear();            //Clear the txt file path list
                                for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                                {
                                    if (lstWorkSheetName.Contains(xlWorkBook.Worksheets[i].Name.ToString()))
                                    {
                                        string sheetName = xlWorkBook.Worksheets[i].Name.ToString();
                                        if (File.Exists("C:\\Temp" + "\\" + sheetName + ".ism"))
                                            File.Delete("C:\\Temp" + "\\" + sheetName + ".ism");

                                        Excel.Worksheet worksheet = (Excel.Worksheet)xlApp.Worksheets[sheetName];

                                        worksheet.Select(true);

                                        xlWorkBook.SaveAs("C:\\Temp" + "\\" + sheetName + ".ism", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                                        lstTextFile.Add("C:\\Temp" + "\\" + sheetName + ".ism");
                                        txtPath = "C:\\Temp" + "\\" + sheetName + ".ism";
                                    }
                                }
                                xlWorkBook.Close(false);
                                releaseObject(xlWorkBook);
                                releaseObject(xlApp);
                                //this.quitProcess();

                                //Delete all txt fiel
                                //for (int ix = 0; ix < lstTextFile.Count; ix++)
                                //{
                                //    this.DeleteFilesFromFolders(lstTextFile[ix]);
                                //}


                            }
                        }
                        else
                            MessageBox.Show("Project Name should not be blank");
                    }
                    else
                        MessageBox.Show("File does not exist in the selected location");
                }
                else
                    MessageBox.Show("Excel File location should not be blank");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadAllList()
        {
            if (txtPath != "")
            {
                lstTableType.Clear();
                lstVariableName.Clear();
                lstVariableLabel.Clear();
                lstMRUniqueVariableName.Clear();
                lstMRBreakPoint.Clear();
                lstMRVariableLabel.Clear();
                lstFilterCondition.Clear();
                lstFilterLabel.Clear();

                using (TextReader txtReader = new StreamReader(txtPath))
                {
                    txtReader.ReadLine();   // skip first header row
                    string strline = txtReader.ReadLine();  // skip second header row, read first data row

                    while (strline != null)
                    {
                        string[] word = strline.Split('\t');

                        if (word.Length >= 9)
                        {
                            lstTableType.Add(word[0]);
                            lstVariableName.Add(word[1]);
                            lstVariableLabel.Add(removeDoubleQuote(word[3]));
                            lstMRUniqueVariableName.Add(word[4]);
                            lstMRBreakPoint.Add(word[5]);
                            lstMRVariableLabel.Add(removeDoubleQuote(word[6]));
                            lstFilterCondition.Add(word[7]);
                            lstFilterLabel.Add(word[8] == "" ? "All Respondents" : word[8]);
                        }

                        strline = txtReader.ReadLine();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a sheet");
            }
        }

        private bool createFile(string syntaxType)
        {
            string basePath = txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\'));
            string prefix = syntaxType == "Table_Cpt" ? "01" : syntaxType == "Table_Count" ? "02" : "00";
            string sheetSuffix = lstWorkSheetName.Count > 0 ? "_" + lstWorkSheetName[0] : "";
            string fileName = prefix + "." + txtOutputFileName.Text + "_" + syntaxType + sheetSuffix + ".SPS";
            string filePath = basePath + "\\" + fileName;

            if (File.Exists(filePath))
            {
                MessageBox.Show("SPSS file with same name [" + fileName + "] exist\nPlease check it first and rename the file name..");
                return false;
            }

            _currentSyntaxFilePath = filePath;
            txt_writer = new StreamWriter(filePath);
            return true;
        }

        private string removeDoubleQuote(string myString)
        {
            string returnString = "";
            for (int i = 0; i < myString.Length; i++)
            {
                if (myString.Substring(i, 1) == "\"")
                {

                }
                else
                {
                    returnString = returnString + myString.Substring(i, 1);
                }
            }
            if (returnString.Length > 150)
                returnString = returnString.Substring(0, 150);

            return returnString;
        }

        private void loadCategoryList()
        {
            try
            {
                if (File.Exists(txtStructureExcelPath.Text) == true)
                {
                    Excel.Application localXlApp = new Excel.Application();
                    Excel.Workbook localXlWorkBook = localXlApp.Workbooks.Open(txtStructureExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                    chkListBoxWorksheet.Items.Clear();
                    for (int i = 1; i <= localXlWorkBook.Worksheets.Count; i++)
                    {
                        chkListBoxWorksheet.Items.Add(localXlWorkBook.Worksheets[i].Name.ToString());
                    }

                    releaseObject(localXlWorkBook);
                    releaseObject(localXlApp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getBannerText()
        {
            bannerText1 = "";
            bannerText2 = "";
            outputExcelFileName = "";

            using (TextReader txtReader = new StreamReader(txtBannerFilePath.Text))
            {
                string strline = txtReader.ReadLine();

                while (strline != null)
                {
                    if (strline.Trim() != "")
                    {
                        if (strline.Substring(0, 1) != "*")
                        {
                            if (bannerText1 == "")
                                bannerText1 = strline;
                            else if (bannerText2 == "")
                                bannerText2 = strline;
                            else if (outputExcelFileName == "")
                                outputExcelFileName = strline;
                        }
                    }
                    strline = txtReader.ReadLine();
                }
            }

            txtOutputFileName.Text = outputExcelFileName;
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.AppDomain.CurrentDomain.BaseDirectory + "CTableSyntax_Help.html";
            if (File.Exists(helpPath))
                Process.Start(helpPath);
            else
                MessageBox.Show("Help file not found:\n" + helpPath);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void populateCombo()
        {
            comAnalysisType.Items.Clear();
            comAnalysisType.Items.Add("UnWeighted Analysis");
            comAnalysisType.Items.Add("Weighted Analysis");
            comAnalysisType.Text = "UnWeighted Analysis";
        }

        private void frmCTableSyntax_Loaded(object sender, RoutedEventArgs e)
        {
            this.populateCombo();
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count == 0) return;

            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            lstWorkSheetName.Clear();
            lstWorkSheetName.Add(chkListBoxWorksheet.SelectedItems[0].ToString());
        }

        private void btnBrowseBannerFile_Click(object sender, RoutedEventArgs e)
        {
            String sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            TextWriter txtWriter = new StreamWriter(sTemp + "\\banner_help.txt");
            txtWriter.WriteLine("********************************************************************************************************************");
            txtWriter.WriteLine("*  ********* ColVar ***********************                                                                        *");
            txtWriter.WriteLine("*  ATotal+Product+(Center+UserType)>Product                                                                        *");
            txtWriter.WriteLine("*  ATotal+S1Banner+S2Banner+DhakaBanner+DhakaUrbanRuralBanner+CTGBanner+CTGUrbanRuralBanner+SECBanner+PBQ1Banner   *");
            txtWriter.WriteLine("*                                                                                                                  *");
            txtWriter.WriteLine("*  ********* ColVarPct ********************                                                                        *");
            txtWriter.WriteLine("*  ATotal Product Center UserType Product                                                                          *");
            txtWriter.WriteLine("*  ATotal S1Banner S2Banner DhakaBanner DhakaUrbanRuralBanner CTGBanner CTGUrbanRuralBanner SECBanner PBQ1Banner   *");
            txtWriter.WriteLine("*                                                                                                                  *");
            txtWriter.WriteLine("********************************************************************************************************************");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("*Banner (ColVar)");
            txtWriter.WriteLine("ATotal+Product+(Center+UserType)>Product");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("*Banner (ColVarPct)");
            txtWriter.WriteLine("ATotal Product Center UserType Product");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("*Output File Name");
            txtWriter.WriteLine("ProjectName");

            txtWriter.Close();


            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\banner_help.txt");
        }

        private async void btnBrowseGetStrucutureFile_Click(object sender, RoutedEventArgs e)
        {
            if (txtSpssDataFile.Text == "" || !File.Exists(txtSpssDataFile.Text))
            {
                MessageBox.Show("Please select a valid SPSS Data File (.sav) first.");
                return;
            }

            // Capture before going async — myPath must not be read from background thread
            string spssPath = txtSpssDataFile.Text;
            string outputFolder = myPath;

            btnBrowseGetStrucutureFile.IsEnabled = false;
            btnBrowseGetStrucutureFile.Content = "Creating...";
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                await RunOnStaThread(() =>
                {
                    var dicNameVsVariable = new Dictionary<string, Variable>();
                    using (var fs = new FileStream(spssPath, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                    {
                        SpssReader spssDataset = new SpssReader(fs);
                        foreach (Variable variable in spssDataset.Variables)
                            dicNameVsVariable[variable.Name] = variable;
                    }

                    createStructureExportToExcel(dicNameVsVariable, outputFolder);
                    createStructurePrepareAnalysisCode(outputFolder);
                });

                MessageBox.Show("Structure file created successfully.\n\n" + outputFolder + "\\SPSS Analysis strcture.xlsx");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
                btnBrowseGetStrucutureFile.IsEnabled = true;
                btnBrowseGetStrucutureFile.Content = "Create Structure File";
            }
        }

        private static Task RunOnStaThread(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
            {
                try { action(); tcs.SetResult(true); }
                catch (Exception ex) { tcs.SetException(ex); }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }

        private void createStructureExportToExcel(Dictionary<string, Variable> dicNameVsVariable, string outputFolder)
        {
            object misValue = System.Reflection.Missing.Value;

            Excel.Application xlApp2 = new Excel.Application();
            Excel.Workbook xlWorkBook2 = xlApp2.Workbooks.Add(misValue);
            Excel.Worksheet xlWorkSheet2 = new Excel.Worksheet();
            Excel.Sheets worksheets = xlWorkBook2.Worksheets;

            // ── Code Mapping sheet ──────────────────────────────────────
            var xlCodeSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlCodeSheet.Name = "Code Mapping";

            xlCodeSheet.Cells[1, 1] = "0"; xlCodeSheet.Cells[1, 2] = "Don't use";
            xlCodeSheet.Cells[2, 1] = "1"; xlCodeSheet.Cells[2, 2] = "Single Response"; xlCodeSheet.Cells[2, 3] = "Column Pct";
            xlCodeSheet.Cells[3, 1] = "2"; xlCodeSheet.Cells[3, 2] = "Multiple Response"; xlCodeSheet.Cells[3, 3] = "Column Pct";
            xlCodeSheet.Cells[4, 1] = "3"; xlCodeSheet.Cells[4, 2] = "Single Response With Mean"; xlCodeSheet.Cells[4, 3] = "Column Pct with Mean";
            xlCodeSheet.Cells[5, 1] = "4"; xlCodeSheet.Cells[5, 2] = "Rank Response"; xlCodeSheet.Cells[5, 3] = "";
            xlCodeSheet.Cells[6, 1] = "5"; xlCodeSheet.Cells[6, 2] = "Scaled Question (5)"; xlCodeSheet.Cells[6, 3] = "T2B Cpct B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[7, 1] = "6"; xlCodeSheet.Cells[7, 2] = "Scaled Question - Reverse (5)"; xlCodeSheet.Cells[7, 3] = "T2B Cpct B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[8, 1] = "7"; xlCodeSheet.Cells[8, 2] = "Scaled Question (7)"; xlCodeSheet.Cells[8, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[9, 1] = "8"; xlCodeSheet.Cells[9, 2] = "Mean Summary"; xlCodeSheet.Cells[9, 3] = "Summary Of Mean";
            xlCodeSheet.Cells[10, 1] = "9"; xlCodeSheet.Cells[10, 2] = "Scaled Question (9)"; xlCodeSheet.Cells[10, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[11, 1] = "10"; xlCodeSheet.Cells[11, 2] = "Scaled Question (10)"; xlCodeSheet.Cells[11, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[12, 1] = "11"; xlCodeSheet.Cells[12, 2] = "Scaled Question (11)"; xlCodeSheet.Cells[12, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[13, 1] = "12"; xlCodeSheet.Cells[13, 2] = "NPS Question (11)"; xlCodeSheet.Cells[13, 3] = "CPT Promoter [9-10] Passive [7-8] Detractor [0-6]";

            xlCodeSheet.Columns.AutoFit();
            xlCodeSheet.Columns[1].ColumnWidth = 10;

            // ── Banner Variables sheet ──────────────────────────────────
            var xlBannerSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlBannerSheet.Name = "Banner Variables";

            xlBannerSheet.Cells[1, 1] = "SPSS Variables";
            xlBannerSheet.Cells[1, 2] = "Banner Variables";
            xlBannerSheet.Cells[1, 3] = "Banner Labels";
            xlBannerSheet.Cells[1, 4] = "New Code";
            xlBannerSheet.Cells[1, 5] = "New Label";
            xlBannerSheet.Cells[1, 6] = "Condition";

            xlBannerSheet.Columns[1].ColumnWidth = 22;
            xlBannerSheet.Columns[2].ColumnWidth = 20;
            xlBannerSheet.Columns[3].ColumnWidth = 25;
            xlBannerSheet.Columns[4].ColumnWidth = 12;
            xlBannerSheet.Columns[5].ColumnWidth = 25;
            xlBannerSheet.Columns[6].ColumnWidth = 50;
            xlBannerSheet.Rows[1].Font.Bold = true;

            // ── Table Structure sheet ───────────────────────────────────
            var xlStructSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlStructSheet.Name = "Table Structure";

            xlStructSheet.Cells[1, 1] = "Analysis Type";
            xlStructSheet.Cells[1, 2] = "Variable Name";
            xlStructSheet.Cells[1, 3] = "Variable Type";
            xlStructSheet.Cells[1, 4] = "Variable Label";
            xlStructSheet.Cells[1, 5] = "MR Variable Name";
            xlStructSheet.Cells[1, 6] = "Break Point";
            xlStructSheet.Cells[1, 7] = "MR Label";
            xlStructSheet.Cells[1, 8] = "Filter Condition";
            xlStructSheet.Cells[1, 9] = "Filter Label";

            int i = 1;
            foreach (KeyValuePair<string, Variable> pair in dicNameVsVariable)
            {
                if (!pair.Key.Contains("_OE"))
                {
                    xlStructSheet.Cells[i + 1, 1] = "";
                    xlStructSheet.Cells[i + 1, 2] = pair.Key;
                    xlStructSheet.Cells[i + 1, 3] = pair.Value.Type.ToString();
                    xlStructSheet.Cells[i + 1, 4] = pair.Value.Label == null ? "" : pair.Value.Label.ToString();
                    xlStructSheet.Cells[i + 1, 5] = "";
                    xlStructSheet.Cells[i + 1, 6] = "";
                    xlStructSheet.Cells[i + 1, 7] = "";
                    xlStructSheet.Cells[i + 1, 8] = "";
                    xlStructSheet.Cells[i + 1, 9] = "";
                    i++;
                }
            }

            xlStructSheet.Columns[1].ColumnWidth = 11;
            xlStructSheet.Columns[2].ColumnWidth = 20;
            xlStructSheet.Columns[3].ColumnWidth = 12;
            xlStructSheet.Columns[4].ColumnWidth = 70;
            xlStructSheet.Columns[5].ColumnWidth = 15;
            xlStructSheet.Columns[6].ColumnWidth = 15;
            xlStructSheet.Columns[7].ColumnWidth = 15;
            xlStructSheet.Columns[8].ColumnWidth = 15;
            xlStructSheet.Columns[9].ColumnWidth = 15;
            xlStructSheet.Rows[1].Font.Bold = true;

            // Remove the default blank sheet Excel adds (sheet 4 at this point)
            ((Excel.Worksheet)xlWorkBook2.Sheets[4]).Delete();

            xlWorkBook2.SaveAs(outputFolder + "\\SPSS Analysis strcture.xlsx", Excel.XlFileFormat.xlWorkbookDefault);
            xlWorkBook2.Close(true, misValue, misValue);
            xlApp2.Quit();

            releaseObject(xlWorkSheet2);
            releaseObject(xlWorkBook2);
            releaseObject(xlApp2);
        }

        private void createStructurePrepareAnalysisCode(string outputFolder)
        {
            string structurePath = outputFolder + "\\SPSS Analysis strcture.xlsx";
            if (!File.Exists(structurePath)) return;

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(structurePath, 0, false, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
            {
                if (myWorksheet.Name != "Table Structure") continue;

                Excel.Range range = myWorksheet.UsedRange;
                bool firstTime = true;
                string priorQid = "";
                string currentQid = "";

                for (int i = 2; i <= range.Rows.Count; i++)
                {
                    string temp1 = myWorksheet.Cells[i, 2].Value.ToString();
                    string varType = myWorksheet.Cells[i, 3].Value.ToString();

                    if (!varType.ToUpper().Contains("TEXT"))
                    {
                        if (!temp1.Contains("_"))
                        {
                            myWorksheet.Cells[i, 1] = 1;
                            if (!firstTime)
                            {
                                myWorksheet.Cells[i - 1, 1] = 2;
                                myWorksheet.Cells[i - 1, 5] = priorQid;
                                myWorksheet.Cells[i - 1, 6] = "XXX";
                                myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i - 1, 4];
                            }
                            firstTime = true;
                        }
                        else
                        {
                            string[] qId = temp1.Split('_');
                            currentQid = qId.Length >= 3 ? qId[0] + "_" + qId[1] : qId[0];

                            if (priorQid != currentQid && !firstTime)
                            {
                                myWorksheet.Cells[i, 1] = 2;
                                myWorksheet.Cells[i - 1, 5] = priorQid;
                                myWorksheet.Cells[i - 1, 6] = "XXX";
                                myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i - 1, 4];
                            }
                            else
                            {
                                myWorksheet.Cells[i, 1] = 2;
                            }

                            firstTime = false;
                            priorQid = currentQid;
                        }
                    }
                    else
                    {
                        myWorksheet.Cells[i, 1] = 0;
                        if (!firstTime)
                        {
                            myWorksheet.Cells[i - 1, 1] = 2;
                            myWorksheet.Cells[i - 1, 5] = priorQid;
                            myWorksheet.Cells[i - 1, 6] = "XXX";
                            myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i - 1, 4];
                        }
                        firstTime = true;
                    }
                }
            }

            xlWorkBook.Save();
            xlWorkBook.Close();
            xlApp.Quit();

            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        string GetBannerNestedVariable(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || !input.Contains(">"))
                return string.Empty;

            var result = new List<string>();

            foreach (string level in input.Split('>').Skip(1))
            {
                string trimmed = level.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                // Levels starting with '(' are parenthesised column-header groups —
                // they belong to GetBannerVariablesForLabel (DISPLAY=LABEL), not here.
                if (trimmed.StartsWith("("))
                    continue;

                // Standalone nesting key: take the name up to the first +, (, or space.
                int cut = trimmed.IndexOfAny(new[] { '+', '(', ' ' });
                string varName = cut > 0 ? trimmed.Substring(0, cut).Trim() : trimmed;

                if (!string.IsNullOrWhiteSpace(varName) && !result.Contains(varName))
                    result.Add(varName);
            }

            return string.Join(" ", result);
        }

        static string GetBannerVariablesForLabel(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var variables = new List<string>();
            var levels = input.Split('>');

            // Root level: non-paren vars first (preserves banner order), then each () group.
            string root = levels[0];
            string rootWithoutParen = Regex.Replace(root, @"\(.*?\)", "").Trim();
            variables.AddRange(rootWithoutParen.Split('+').Select(x => x.Trim()).Where(x => x != ""));
            foreach (Match m in Regex.Matches(root, @"\((.*?)\)"))
                variables.AddRange(m.Groups[1].Value.Split('+').Select(x => x.Trim()).Where(x => x != ""));

            // Sub-levels: only vars inside () are column-header groups (DISPLAY=LABEL).
            // Standalone vars after > are nesting keys handled by GetBannerNestedVariable (DISPLAY=NONE).
            for (int i = 1; i < levels.Length; i++)
            {
                foreach (Match m in Regex.Matches(levels[i], @"\((.*?)\)"))
                    variables.AddRange(m.Groups[1].Value.Split('+').Select(x => x.Trim()).Where(x => x != ""));
            }

            return string.Join(" ", variables.Distinct());
        }

    }
}
