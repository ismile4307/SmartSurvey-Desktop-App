using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmCTableSyntax.xaml
    /// </summary>
    public partial class FrmCTableSyntax : Window
    {
        private String myPath;
        private String bannerText1 = "";
        private String bannerText2 = "";
        private String outputExcelFileName = "";
        private string txtPath = "";

        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;

        private TextWriter txt_writer;

        private List<string> lstWorkSheetName = new List<string>();

        private List<String> lstCategory = new List<string>();

        private string myTitle = "";

        private List<String> lstTableType = new List<String>();
        private List<String> lstVariableName = new List<String>();
        private List<String> lstVariableLabel = new List<String>();


        private List<String> lstMRUniqueVariableName = new List<String>();
        private List<String> lstMRBreakPoint = new List<String>();
        private List<String> lstMRVariableLabel = new List<String>();

        private List<String> lstFilterCondition = new List<String>();
        private List<String> lstFilterLabel = new List<String>();

        private string mrVarList;
        private string decimalNumber = "0";
        private string analysisType = "";

        private Dictionary<String, int> dicFilterTypeVsCode;

        private Dictionary<String, int> dicAnalysisTypeVsCode;

        public FrmCTableSyntax()
        {
            InitializeComponent();
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
                    //my_Path = txt_Excel_Location.Text.Substring(0, txt_Banner.Text.LastIndexOf('\\'));

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
            decimalNumber = txtDecimalPlace.Text;

            this.saveExcellSheetAsText();
            this.loadAllList();


            if (radioBtnRecodeSyntax.IsChecked == true)
            {
                if (createFile("Recode"))
                {
                    this.recodeScript();
                    txt_writer.Close();
                    MessageBox.Show("Write Completed");
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
        private void recodeScript()
        {
            txt_writer.WriteLine("");
            //txt_writer.WriteLine("*****************************Allah is Almighty*******************************");

            txt_writer.WriteLine("*Please specify the file get path.");
            txt_writer.WriteLine("GET ");
            txt_writer.WriteLine(@"FILE='" + myPath.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\Data Preparation\\" + outputExcelFileName + "_Final.sav'.");
            txt_writer.WriteLine("DATASET NAME DataSet1 WINDOW=FRONT.");

            txt_writer.WriteLine("");

            txt_writer.WriteLine("*************************************Banner************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("COMPUTE ATotal=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS ATotal ""Total"".");
            txt_writer.WriteLine(@"VALUE LABELS ATotal 1 ""Total"".");
            txt_writer.WriteLine("");



            String[] word = bannerText2.Split(' ');

            for (int x = 1; x < word.Length; x++)
            {
                txt_writer.WriteLine("NUMERIC " + word[x] + " (f8.0).");
                //txt_writer.WriteLine("NUMERIC " + word[x] + " (f8.0).");
                txt_writer.WriteLine("");
            }
            txt_writer.WriteLine("");

            for (int x = 1; x < word.Length; x++)
            {
                txt_writer.WriteLine("COMPUTE " + word[x] + "=.");
                //txt_writer.WriteLine("COMPUTE " + word[x] + "=.");
                txt_writer.WriteLine("");
            }

            txt_writer.WriteLine("VARIABLE LABELS");
            for (int x = 1; x < word.Length; x++)
            {
                txt_writer.WriteLine(word[x] + "  \"\"");
                //txt_writer.WriteLine("COMPUTE " + word[x] + "=.");
            }
            txt_writer.WriteLine(".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("VALUE LABELS");
            for (int x = 1; x < word.Length; x++)
            {
                txt_writer.WriteLine(word[x]);
                //txt_writer.WriteLine("COMPUTE " + word[x] + "=.");
            }
            txt_writer.WriteLine(".");
            txt_writer.WriteLine("");

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
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "5")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "6")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean (Rev)\".");
                    txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (1=5)(2=4)(3=3)(4=2)(5=1).");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "7")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "8")
                {


                }
                else if (lstTableType[i] == "9")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "10")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "11")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");

                    txt_writer.WriteLine("COMPUTE sd_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS sd_" + lstVariableName[i] + " \"S.D.\".");

                    txt_writer.WriteLine("COMPUTE se_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS se_" + lstVariableName[i] + " \"S.E.\".");
                    txt_writer.WriteLine("");

                }
                else if (lstTableType[i] == "12")
                {
                    txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("VARIABLE LABELS m_" + lstVariableName[i] + " \"Mean\".");
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
            txt_writer.WriteLine("SAVE OUTFILE='" + myPath + "\\" + outputExcelFileName + "_Final.sav'.");
            txt_writer.WriteLine("/COMPRESSED.");
        }

        //***************************** Column Percentage Script ***************************************
        private void prepareScript()
        {
            txt_writer.WriteLine("");
            //txt_writer.WriteLine("*****************************Allah is Almighty*******************************");

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
            int i_varCount = 0;

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
                    txt_writer.WriteLine("  /VLABELS VARIABLES=" + lstVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");
                    txt_writer.WriteLine("  /TABLE " + lstVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] BY " + bannerText1 + "");
                    txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + lstVariableName[i] + " ORDER=A KEY=VALUE EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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
                    i_varCount++;

                    if (lstMRBreakPoint[i] == "XXX")
                    {


                        txt_writer.WriteLine("* Define Multiple Response Sets.");
                        txt_writer.WriteLine("MRSETS");
                        txt_writer.WriteLine("  /MCGROUP NAME=$" + lstMRUniqueVariableName[i] + " LABEL='Spontanious Awareness' VARIABLES=" + mrVarList + " ");
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
                        txt_writer.WriteLine("  /VLABELS VARIABLES=$" + lstMRUniqueVariableName[i] + " " + nestedVairables + " DISPLAY=NONE");
                        txt_writer.WriteLine("  /TABLE $" + lstMRUniqueVariableName[i] + "[C] [" + analysisType + ", TOTAL[COUNT F40.0]] BY " + bannerText1 + "");
                        txt_writer.WriteLine("  /SLABELS POSITION=ROW VISIBLE=NO");
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=$" + lstMRUniqueVariableName[i] + " ORDER=A KEY=VALUE EMPTY=INCLUDE TOTAL=YES POSITION=BEFORE");
                        txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
                        txt_writer.WriteLine("  /CRITERIA CILEVEL=95");
                        txt_writer.WriteLine("  /TITLES");
                        txt_writer.WriteLine("    TITLE='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("    CORNER='Base : " + lstFilterLabel[i] + "'");
                        txt_writer.WriteLine("    CAPTION='Home'.");
                        txt_writer.WriteLine("");

                        mrVarList = "";

                        i_TableNo = i_TableNo + 1;
                        i_varCount = 0;
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
                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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
                    //4	Scaled Question (9)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {

                    }
                    else if (lstFilterCondition[i] != "")
                    {

                    }
                    i_TableNo = i_TableNo + 1;
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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


                    txt_writer.WriteLine("  /CATEGORIES VARIABLES=" + bannerText2 + " ORDER=A KEY=VALUE EMPTY=INCLUDE");
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
            //try
            //{
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

            //this.quitProcess();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void loadAllList()
        {
            if (txtPath != "")
            {
                TextReader txtReader = new StreamReader(txtPath);
                String strline;
                strline = txtReader.ReadLine();         //Leave the first line of the text file
                strline = txtReader.ReadLine();

                lstTableType.Clear();
                lstVariableName.Clear();
                lstVariableLabel.Clear();
                lstMRUniqueVariableName.Clear();
                lstMRBreakPoint.Clear();
                lstMRVariableLabel.Clear();
                lstFilterCondition.Clear();
                lstFilterLabel.Clear();

                while (strline != null)
                {
                    string[] word = strline.Split('\t');

                    lstTableType.Add(word[0]);
                    lstVariableName.Add(word[1]);
                    lstVariableLabel.Add(removeDoubleCot(word[3]));
                    lstMRUniqueVariableName.Add(word[4]);
                    lstMRBreakPoint.Add(word[5]);
                    lstMRVariableLabel.Add(removeDoubleCot(word[6]));
                    lstFilterCondition.Add(word[7]);
                    if (word[8].ToString() == "")
                        lstFilterLabel.Add("All Respondents");
                    else
                        lstFilterLabel.Add(word[8]);

                    strline = txtReader.ReadLine();
                }

                txtReader.Close();
            }
            else
            {
                MessageBox.Show("Please select a sheet");
            }
        }

        private bool createFile(string syntaxType)
        {
            if (syntaxType == "Table_Cpt")
            {
                if (File.Exists(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\01." + txtOutputFileName.Text + "_" + syntaxType + ".SPS"))
                {
                    MessageBox.Show("SPSS file with same name [01." + txtOutputFileName.Text + "_" + syntaxType + ".SPS] exist\nPlease check it first and rename the file name..");
                    return false;
                }
                txt_writer = new StreamWriter(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\01." + txtOutputFileName.Text + "_" + syntaxType + ".SPS");
            }
            else if (syntaxType == "Table_Count")
            {
                if (File.Exists(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\02." + txtOutputFileName.Text + "_" + syntaxType + ".SPS"))
                {
                    MessageBox.Show("SPSS file with same name [02." + txtOutputFileName.Text + "_" + syntaxType + ".SPS] exist\nPlease check it first and rename the file name..");
                    return false;
                }
                txt_writer = new StreamWriter(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\02." + txtOutputFileName.Text + "_" + syntaxType + ".SPS");
            }
            else
            {
                if (File.Exists(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\00." + txtOutputFileName.Text + "_" + syntaxType + ".SPS"))
                {
                    MessageBox.Show("SPSS file with same name [00." + txtOutputFileName.Text + "_" + syntaxType + ".SPS] exist\nPlease check it first and rename the file name..");
                    return false;
                }
                txt_writer = new StreamWriter(txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\')) + "\\00." + txtOutputFileName.Text + "_" + syntaxType + ".SPS");
            }
            return true;
        }

        private string removeDoubleCot(string myString)
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
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtStructureExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                    chkListBoxWorksheet.Items.Clear();
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        chkListBoxWorksheet.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
                    }

                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    //xlApp.Quit();
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
            TextReader txtReader = new StreamReader(txtBannerFilePath.Text);
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
            txtOutputFileName.Text = outputExcelFileName;

            //MessageBox.Show("");
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void populateCombo()
        {
            comBaseType.Items.Clear();
            comBaseType.Items.Add("Use Filter All Base");
            comBaseType.Items.Add("No Filter Answer Base");

            dicFilterTypeVsCode = new Dictionary<string, int>();

            dicFilterTypeVsCode.Add("Use Filter All Base", 1);
            dicFilterTypeVsCode.Add("No Filter Answer Base", 2);

            comBaseType.Text = "No Filter Answer Base";



            comAnalysisType.Items.Clear();
            comAnalysisType.Items.Add("UnWeighted Analysis");
            comAnalysisType.Items.Add("Weighted Analysis");

            dicAnalysisTypeVsCode = new Dictionary<string, int>();

            dicAnalysisTypeVsCode.Add("UnWeighted Analysis", 1);
            dicAnalysisTypeVsCode.Add("Weighted Analysis", 2);

            comAnalysisType.Text = "UnWeighted Analysis";

        }

        private void frmCTableSyntax_Loaded(object sender, RoutedEventArgs e)
        {
            this.populateCombo();
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            //foreach (var item in chkListBoxWorksheet.Items)
            //{
            //    for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
            //    {
            //        if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
            //        {
            //            lstWorkSheetName.Add(item.ToString());

            //        }
            //    }
            //}

            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

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

        private void btnBrowseGetStrucutureFile_Click(object sender, RoutedEventArgs e)
        {

            FrmGetStructureFile frmGetStructureFile = new FrmGetStructureFile();
            frmGetStructureFile.ShowDialog();

            //string sTemp;

            //sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            //string[] arrayPath = sTemp.Split('\\');

            //FileInfo fi = new FileInfo(sTemp + "\\SPSS Analysis strcture.xlsx");
            //if (fi.Exists)
            //{
            //    System.Diagnostics.Process.Start(sTemp + "\\SPSS Analysis strcture.xlsx");
            //}
            //else
            //{
            //    //file doesn't exist
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {



            string finalResult = GetBannerNestedVariable("ATotal+Circle+Region21>QB1+(QB2+QB4)>QB3+QB2>QB17>QB12>QB8");

            MessageBox.Show(finalResult);
        }

        string GetBannerNestedVariable(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || !input.Contains(">"))
                return string.Empty;

            // Split by '>' and skip root
            var levels = input.Split('>')
                              .Skip(1)
                              .Select(x =>
                              {
                                  int cut = x.IndexOfAny(new[] { '+', '(' });
                                  return cut > 0 ? x.Substring(0, cut) : x;
                              })
                              .Where(x => !string.IsNullOrWhiteSpace(x))
                              .ToList();

            if (!levels.Any())
                return string.Empty;

            List<string> result = new List<string>();

            // Always take first
            result.Add(levels.First());

            // Take middle if more than 2 levels
            if (levels.Count > 2)
            {
                int midIndex = levels.Count / 2;
                result.Add(levels[midIndex]);
            }

            // Always take last
            if (levels.Count > 1)
                result.Add(levels.Last());

            // Remove duplicates but preserve order
            return string.Join(" ", result.Distinct());
        }

        static string GetBannerVariablesForLabel(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Split by '>'
            var levels = input.Split('>');

            List<string> variables = new List<string>();

            // 1️⃣ Process root (first level)
            string root = levels[0];
            // Extract variables in parentheses
            var rootParenMatches = Regex.Matches(root, @"\((.*?)\)");
            foreach (Match m in rootParenMatches)
            {
                variables.AddRange(m.Groups[1].Value.Split('+').Select(x => x.Trim()).Where(x => x != ""));
            }
            // Also include any variable outside parentheses
            string rootWithoutParen = Regex.Replace(root, @"\((.*?)\)", "").Trim();
            variables.AddRange(rootWithoutParen.Split('+').Select(x => x.Trim()).Where(x => x != ""));

            // 2️⃣ Process remaining levels, only variables inside parentheses
            for (int i = 1; i < levels.Length; i++)
            {
                string level = levels[i].Trim();
                var parenMatches = Regex.Matches(level, @"\((.*?)\)");
                foreach (Match m in parenMatches)
                {
                    variables.AddRange(m.Groups[1].Value.Split('+').Select(x => x.Trim()).Where(x => x != ""));
                }
            }

            return string.Join(" ", variables);
        }

    }
}
