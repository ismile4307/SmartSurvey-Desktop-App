using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
using SpssLib.DataReader;
using SpssLib.SpssDataset;
using System.Text.RegularExpressions;
using System.Threading;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmAnalysisTable.xaml
    /// </summary>
    public partial class FrmAnalysisTable : Window
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

        private Dictionary<String, int> dicFilterTypeVsCode;

        private Dictionary<String, int> dicAnalysisTypeVsCode;

        private string analysisType = "";
        private string _currentSyntaxFilePath = "";


        public FrmAnalysisTable()
        {
            InitializeComponent();
        }

        private void btnBrowseStructureExcel_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
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
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void loadCategoryList()
        {
            //try
            //{
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
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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

        private void btnBrowseBanner_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
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
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
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

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSpssDataFile.Text) || !File.Exists(txtSpssDataFile.Text))
            {
                MessageBox.Show("Please select a valid SPSS Data File (.sav) before executing.");
                return;
            }

            decimalNumber = txtDecimalPlace.Text;

            this.save_ExcellSheetAsText();
            this.load_AllList();

            if (dicFilterTypeVsCode[comBaseType.Text] == 1)
            {
                if (radioBtnRecodeSyntax.IsChecked == true)
                {
                    this.create_File("Recode");
                    this.recodeScript();
                    txt_writer.Close();
                }
                else if (radioBtnPctTableSyntax.IsChecked == true)
                {
                    this.create_File("Table_Cpt");
                    this.columnPctScript();
                    txt_writer.Close();
                }
                else if (radioBtnPct2TableSyntax.IsChecked == true)
                {
                    this.create_File("Table_Cpt_Pct");
                    this.columnPctScript();
                    txt_writer.Close();
                }
                else
                {
                    this.create_File("Table_Count");
                    this.tableCountScript();
                    txt_writer.Close();
                }
            }
            else if (dicFilterTypeVsCode[comBaseType.Text] == 2)
            {
                if (radioBtnRecodeSyntax.IsChecked == true)
                {
                    this.create_File("Recode_Answer_Base");
                    this.recodeScriptAnswerBase();
                    txt_writer.Close();
                }
                else if (radioBtnPctTableSyntax.IsChecked == true)
                {
                    this.create_File("Table_Answer_Base_Cpt");
                    this.columnPctScriptAnswerBase();
                    txt_writer.Close();
                }
                else if (radioBtnPct2TableSyntax.IsChecked == true)
                {
                    this.create_File("Table_Answer_Base_Cpt_Pct");
                    this.columnPctScriptAnswerBase();
                    txt_writer.Close();
                }
                else
                {
                    this.create_File("Table_Count");
                    this.tableCountScript();
                    txt_writer.Close();
                }
            }

            MessageBox.Show("Write Complete");
        }

        //***************************** Recode Script ***************************************
        private void recodeScript()
        {
            if (!writeBannerSyntax()) return;

            txt_writer.WriteLine("*****************************Allah is Almighty*******************************");

            txt_writer.WriteLine("");
            txt_writer.WriteLine("COMPUTE ATotal=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS ATotal ""Total"".");
            txt_writer.WriteLine(@"VALUE LABELS ATotal 1 ""Total"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE nBlank=1.");
            txt_writer.WriteLine("RECODE nBlank (1=SYSMIS).");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE SigTest=1.");
            txt_writer.WriteLine("RECODE SigTest (1=SYSMIS).");
            txt_writer.WriteLine(@"VARIABLE LABELS SigTest ""SIG. TEST"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE Dummy=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE aBase=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            txt_writer.WriteLine(@"VALUE LABELS aBase 1 ""Base"".");
            txt_writer.WriteLine("");



            txt_writer.WriteLine("COMPUTE NPS=1.");
            txt_writer.WriteLine("RECODE NPS (1=SYSMIS).");
            txt_writer.WriteLine(@"VALUE LABELS NPS 1 ""NPS Score"".");
            txt_writer.WriteLine("");

            if (comAnalysisType.Text == "Weighted Analysis")
            {
                txt_writer.WriteLine("COMPUTE uBase=1.");
                txt_writer.WriteLine(@"VALUE LABELS uBase 1 ""Base: Unweighted"".");
                txt_writer.WriteLine("");
            }

            txt_writer.WriteLine("EXECUTE.");

            txt_writer.WriteLine("");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");


            txt_writer.WriteLine("* ADD FILES /FILE=*");
            txt_writer.WriteLine(@" /FILE='D:\DBI Projects\2023 Projects\Data_Dummy.sav'.");
            txt_writer.WriteLine("* EXECUTE.");

            txt_writer.WriteLine("COMPUTE DummyATotal=1.");
            txt_writer.WriteLine(@"VALUE LABELS DummyATotal 1 ""DummyTotal"".");

            txt_writer.WriteLine("COMPUTE Dummy=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");

            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");

            for (int i = 0; i < lstTableType.Count; i++)
            {
                if (lstTableType[i] == "5" || lstTableType[i] == "6")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (88=sysmis) (99=sysmis).")

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) (4=1) into T2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into B2_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [05]"".");
                    txt_writer.WriteLine("VALUE LABELS T2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [05/04]"".");
                    txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS B2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "7")
                {
                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) into T2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) (5=1) into T3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into B2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into B3_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS T2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [07/06]"".");
                    txt_writer.WriteLine("VALUE LABELS T3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [07/06/05]"".");
                    txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS B2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]"".");
                    txt_writer.WriteLine("VALUE LABELS B3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [01/02/03]"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "8")
                {
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into R1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into R2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into R3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "9")
                {
                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) into T2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) (7=1) into T3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into B2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into B3_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [09]"".");
                    txt_writer.WriteLine("VALUE LABELS T2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [09/08]"".");
                    txt_writer.WriteLine("VALUE LABELS T3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [09/08/07]"".");
                    txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS B2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]"".");
                    txt_writer.WriteLine("VALUE LABELS B3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [01/02/03]"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "10")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) into T2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) (8=1) into T3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) (8=1) (7=1) into T4_" + lstVariableName[i] + ".");


                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into B2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into B3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) into B4_" + lstVariableName[i] + ".");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) into B5_" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) (6=1) into B6_" + lstVariableName[i] + ".");

                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS T2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [10/09]"".");
                    txt_writer.WriteLine("VALUE LABELS T3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [10/09/08]"".");
                    txt_writer.WriteLine("VALUE LABELS T4_" + lstVariableName[i] + @" 1 ""TOP 4 BOX [10/09/08/07]"".");

                    //txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS B2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [00/01]"".");
                    txt_writer.WriteLine("VALUE LABELS B3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [00/01/02]"".");
                    txt_writer.WriteLine("VALUE LABELS B4_" + lstVariableName[i] + @" 1 ""BOTTOM 4 BOX [00/01/02/03]"".");

                    //txt_writer.WriteLine("VALUE LABELS B5_" + lstVariableName[i] + @" 1 ""BOTTOM 5 BOX [01/02/03/04/05]"".");
                    //txt_writer.WriteLine("VALUE LABELS B6_" + lstVariableName[i] + @" 1 ""BOTTOM 6 BOX [01/02/03/04/05/06]"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "11")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) into T2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) (9=1) into T3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) (9=1) (8=1) into T4_" + lstVariableName[i] + ".");


                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into B2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into B3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) into B4_" + lstVariableName[i] + ".");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) into B5_" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) (6=1) into B6_" + lstVariableName[i] + ".");

                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS T2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [11/10]"".");
                    txt_writer.WriteLine("VALUE LABELS T3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [11/10/09]"".");
                    txt_writer.WriteLine("VALUE LABELS T4_" + lstVariableName[i] + @" 1 ""TOP 4 BOX [11/11/09/08]"".");

                    //txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS B2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [00/01]"".");
                    txt_writer.WriteLine("VALUE LABELS B3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [00/01/02]"".");
                    txt_writer.WriteLine("VALUE LABELS B4_" + lstVariableName[i] + @" 1 ""BOTTOM 4 BOX [00/01/02/03]"".");

                    //txt_writer.WriteLine("VALUE LABELS B5_" + lstVariableName[i] + @" 1 ""BOTTOM 5 BOX [01/02/03/04/05]"".");
                    //txt_writer.WriteLine("VALUE LABELS B6_" + lstVariableName[i] + @" 1 ""BOTTOM 6 BOX [01/02/03/04/05/06]"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "12")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) into xTPro_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (8=1) (7=1) into xTPas_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (6=1) (5=1) (4=1) (3=1) (2=1) (1=1) (0=1) into xTDic_" + lstVariableName[i] + ".");


                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPro_" + lstVariableName[i] + @" 1 ""Promoter [9-10]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPas_" + lstVariableName[i] + @" 1 ""Passive [7-8]"".");
                    txt_writer.WriteLine("VALUE LABELS xTDic_" + lstVariableName[i] + @" 1 ""Detractor [0-6]"".");

                    txt_writer.WriteLine("");
                }


                //txt_writer.Close();
            }
            txt_writer.WriteLine(" ");
            txt_writer.WriteLine("EXECUTE.");
        }

        //***************************** Column Percentage Script ***************************************
        private void columnPctScript()
        {
            if (!writeBannerSyntax()) return;

            txt_writer.WriteLine("************************** Allah is Almighty********************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("Filter Off.");
            txt_writer.WriteLine("USE ALL.");
            txt_writer.WriteLine("");


            //txt_writer.WriteLine("Define @ColVar() ATotal*Product+(Center+UserType)*Product  !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal Center UserType   Product  !Enddefine.");

            //txt_writer.WriteLine("Define @ColVar() ATotal+S1Banner+S2Banner+DhakaBanner+DhakaUrbanRuralBanner+CTGBanner+CTGUrbanRuralBanner+SECBanner+PBQ1Banner !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal S1Banner S2Banner DhakaBanner DhakaUrbanRuralBanner CTGBanner CTGUrbanRuralBanner SECBanner PBQ1Banner !Enddefine.");

            txt_writer.WriteLine("Define @ColVar() " + bannerText1 + " !Enddefine.");
            txt_writer.WriteLine("Define @ColVarPct() " + bannerText2 + " !Enddefine.");


            txt_writer.WriteLine("");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMS");
            txt_writer.WriteLine("/SELECT TABLES");
            txt_writer.WriteLine("/IF COMMANDS=['Tables'] SUBTYPES=['Table']");
            txt_writer.WriteLine("/DESTINATION FORMAT=XLSX");
            txt_writer.WriteLine("Viewer = No");
            txt_writer.WriteLine(@"OUTFILE='" + myPath + "\\01." + outputExcelFileName + " -Cpt.xlsx'.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("");

            int i_TableNo = 1;
            int i_varCount = 0;

            for (int i = 0; i < lstTableType.Count; i++)
            {

                //************************************ For Table ************************************************

                if (lstTableType[i] == "1")
                {
                    //5	Single Response	CpcT

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "2")
                {
                    //6	Multiple Response	CpcT


                    mrVarList = mrVarList + lstVariableName[i] + " ";
                    i_varCount++;

                    if (lstMRBreakPoint[i] == "XXX")
                    {

                        if (lstFilterCondition[i] == "")
                        {
                            if (lstFilterLabel[i] != "All Respondents")
                            {
                                txt_writer.WriteLine("Compute Filt=1.");
                                txt_writer.WriteLine("COUNT myCount= " + mrVarList + " (Missing).");
                                txt_writer.WriteLine("if myCount=" + i_varCount.ToString() + " Filt=0.");
                                txt_writer.WriteLine("Filter by Filt.");
                            }

                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ALL");
                            txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                            txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                            txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", " ") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            if (lstFilterLabel[i] != "")
                                txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            else
                                txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        else if (lstFilterCondition[i] != "")
                        {

                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");


                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ALL");
                            txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                            txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                            txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        i_TableNo = i_TableNo + 1;
                        i_varCount = 0;
                    }
                }
                else if (lstTableType[i] == "3")
                {
                    // For Single Response With Mean

                    if (lstFilterCondition[i] == "")
                    {
                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;

                    //    txt_writer.WriteLine("Tables Observation nBlank m_S1");
                    //    txt_writer.WriteLine("/Ptotal=t1 'Base'");
                    //    txt_writer.WriteLine("/Ftotal = f1 ""Total""");
                    //    txt_writer.WriteLine("/Format=Zero MISSING ('')");
                    //    txt_writer.WriteLine("/MrGroup=Mr1 '' ExS1");
                    //    txt_writer.WriteLine("/BASE=ALL");
                    //    txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_S1 by @ColVar");
                    //    txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                    //    txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                    //    txt_writer.WriteLine("Mean(m_S1 (F3.2)'MEAN')");
                    //    txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString + ": QS1. Number of Member in Household'");
                    //    txt_writer.WriteLine("/Corner='Base : All Respondent'.");

                    i_TableNo = i_TableNo + 1;

                }
                else if (lstTableType[i] == "4")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "5")
                {
                    //1	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "6")
                {
                    //2	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "7")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "8")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+R1_" + lstVariableName[i] + "+R2_" + lstVariableName[i] + "+R3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'')");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(R1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(R2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(R3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+R1_" + lstVariableName[i] + "+R2_" + lstVariableName[i] + "+R3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'')");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(R1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(R2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(R3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "9")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "10")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "11")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B6_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row2+@Row1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(B5_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(B6_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row2+@Row1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Cpct(B2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(B3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(B4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(B5_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(B6_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(T2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(T3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(T4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "12")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xTPro_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xTPas_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xTDic_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xTPro_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xTPas_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xTDic_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }

            }
            txt_writer.WriteLine("OMSEND.");

            //txt_writer.Close();
        }

        //***************************** Table Count Script ***************************************

        private void tableCountScript()
        {
            if (!writeBannerSyntax()) return;

            txt_writer.WriteLine("************************** Allah is Almighty********************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("Filter Off.");
            txt_writer.WriteLine("USE ALL.");
            txt_writer.WriteLine("");
            //txt_writer.WriteLine("Define @ColVar() ATotal+DUrbRur+DSex+DAge+Region+XtremWeather1+XtremWeather2+XtremWeather3+XtremWeather4+XtremWeather5+D_chief_occup+DEdu+DIncome+DImpact+D_resp_knowl_level+D_resp_spec_resp+D_Risk_Perception+D_self_efficacy");
            //txt_writer.WriteLine("+D_com_efficacy+D_disc_level+D_NGO+D_info_act+ExQMC1a_a+ExQMC1a_b+ExQMC1a_c+ExQMC1a_d+ExQMC1a_e+NonMedia+MCAccessTv+MCAccessRadio+MCAccessInt+MCAccessPress+MCDark+MCAnyFre+MCAnyFre_bd !Enddefine.");

            //txt_writer.WriteLine("Define @ColVarPct() ATotal DUrbRur DSex DAge Region XtremWeather1 XtremWeather2 XtremWeather3 XtremWeather4 XtremWeather5 D_chief_occup DEdu DIncome DImpact D_resp_knowl_level D_resp_spec_resp D_Risk_Perception D_self_efficacy ");
            //txt_writer.WriteLine("D_com_efficacy D_disc_level D_NGO D_info_act ExQMC1a_a ExQMC1a_b ExQMC1a_c ExQMC1a_d ExQMC1a_e NonMedia MCAccessTv MCAccessRadio MCAccessInt MCAccessPress MCDark MCAnyFre MCAnyFre_bd !Enddefine.");


            //txt_writer.WriteLine("Define @ColVar() ATotal*Product+(Center+UserType)*Product  !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal Center UserType   Product  !Enddefine.");

            //txt_writer.WriteLine("Define @ColVar() S2+S3+SEC  !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() S2 S3 SEC  !Enddefine.");

            txt_writer.WriteLine("Define @ColVar() " + bannerText1 + " !Enddefine.");
            txt_writer.WriteLine("Define @ColVarPct() " + bannerText2 + " !Enddefine.");

            txt_writer.WriteLine("");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMS");
            txt_writer.WriteLine("/SELECT TABLES");
            txt_writer.WriteLine("/IF COMMANDS=['Tables'] SUBTYPES=['Table']");
            //txt_writer.WriteLine("/DESTINATION FORMAT=TABTEXT");
            txt_writer.WriteLine("/DESTINATION FORMAT=XLSX");
            txt_writer.WriteLine("Viewer = No");
            txt_writer.WriteLine(@"OUTFILE='" + myPath + "\\02." + outputExcelFileName + " -Count.xlsx'.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("");



            int i_TableNo = 1;
            int i_varCount = 0;

            for (int i = 0; i < lstTableType.Count; i++)
            {

                //************************************ For Table ************************************************

                if (lstTableType[i] == "1")
                {
                    //5	Single Response	Count

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "2")
                {
                    //6	Multiple Response	Count


                    mrVarList = mrVarList + lstVariableName[i] + " ";
                    i_varCount++;

                    if (lstMRBreakPoint[i] == "XXX")
                    {

                        if (lstFilterCondition[i] == "")
                        {
                            if (lstFilterLabel[i] != "All Respondents")
                            {
                                txt_writer.WriteLine("Compute Filt=1.");
                                txt_writer.WriteLine("COUNT myCount= " + mrVarList + " (Missing).");
                                txt_writer.WriteLine("if myCount=" + i_varCount.ToString() + " Filt=0.");
                                txt_writer.WriteLine("Filter by Filt.");
                            }

                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ALL");
                            txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                            txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                            txt_writer.WriteLine("Count(Mr1 (f5)");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", " ") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            if (lstFilterLabel[i] != "")
                                txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            else
                                txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        else if (lstFilterCondition[i] != "")
                        {

                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");


                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ALL");
                            txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                            txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                            txt_writer.WriteLine("Count(Mr1 (f5)");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        i_TableNo = i_TableNo + 1;
                        i_varCount = 0;
                    }
                }
                else if (lstTableType[i] == "3")
                {
                    // For Single Response With Mean

                    if (lstFilterCondition[i] == "")
                    {
                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;

                    //    txt_writer.WriteLine("Tables Observation nBlank m_S1");
                    //    txt_writer.WriteLine("/Ptotal=t1 'Base'");
                    //    txt_writer.WriteLine("/Ftotal = f1 ""Total""");
                    //    txt_writer.WriteLine("/Format=Zero MISSING ('')");
                    //    txt_writer.WriteLine("/MrGroup=Mr1 '' ExS1");
                    //    txt_writer.WriteLine("/BASE=ALL");
                    //    txt_writer.WriteLine("/Table=t1+nBlank+Mr1+nBlank+m_S1 by @ColVar");
                    //    txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                    //    txt_writer.WriteLine("Count(Mr1 (f5)");
                    //    txt_writer.WriteLine("Mean(m_S1 (F3.2)'MEAN')");
                    //    txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString + ": QS1. Number of Member in Household'");
                    //    txt_writer.WriteLine("/Corner='Base : All Respondent'.");

                    i_TableNo = i_TableNo + 1;

                }
                else if (lstTableType[i] == "4")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "5")
                {
                    //1	Scaled Question (5)	T1B T2B Count B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "6")
                {
                    //2	Scaled Question (5)	T1B T2B Count B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "7")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "8")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+R1_" + lstVariableName[i] + "+R2_" + lstVariableName[i] + "+R3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'')");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(R1_" + lstVariableName[i] + " (f5)) Count(R2_" + lstVariableName[i] + " (f5)) Count(R3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+R1_" + lstVariableName[i] + "+R2_" + lstVariableName[i] + "+R3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'')");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(R1_" + lstVariableName[i] + " (f5)) Count(R2_" + lstVariableName[i] + " (f5)) Count(R3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "9")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)) Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)) Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "10")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)) Count(B4_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)) Count(T4_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)) Count(B4_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)) Count(T4_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "11")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B6_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row2+@Row1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)) Count(B4_" + lstVariableName[i] + " (f5)  Count(B5_" + lstVariableName[i] + " (f5)  Count(B6_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)) Count(T4_" + lstVariableName[i] + " (f5)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row2+@Row1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)) Count(B3_" + lstVariableName[i] + " (f5)) Count(B4_" + lstVariableName[i] + " (f5)  Count(B5_" + lstVariableName[i] + " (f5)  Count(B6_" + lstVariableName[i] + " (f5)");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)) Count(T3_" + lstVariableName[i] + " (f5)) Count(T4_" + lstVariableName[i] + " (f5)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "12")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Count B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(xTPro_" + lstVariableName[i] + " (f5)) Count(xTPas_" + lstVariableName[i] + " (f5)) Count(xTDic_" + lstVariableName[i] + " (f5)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");


                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        //txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        //txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Table=t1+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)");
                        txt_writer.WriteLine("Count(xTPro_" + lstVariableName[i] + " (f5)) Count(xTPas_" + lstVariableName[i] + " (f5)) Count(xTDic_" + lstVariableName[i] + " (f5)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }

            }
            txt_writer.WriteLine("OMSEND.");

            //txt_writer.Close();
        }


        //***************************** Recode Answer Base Script ***************************************
        private void recodeScriptAnswerBase()
        {
            if (!writeBannerSyntax()) return;

            txt_writer.WriteLine("*****************************Allah is Almighty*******************************");

            txt_writer.WriteLine("");
            txt_writer.WriteLine("COMPUTE ATotal=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS ATotal ""Total"".");
            txt_writer.WriteLine(@"VALUE LABELS ATotal 1 ""Total"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE nBlank=1.");
            txt_writer.WriteLine("RECODE nBlank (1=SYSMIS).");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE SigTest=1.");
            txt_writer.WriteLine("RECODE SigTest (1=SYSMIS).");
            txt_writer.WriteLine(@"VARIABLE LABELS SigTest ""SIG. TEST"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE Dummy=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE aBase=1.");
            //txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");
            txt_writer.WriteLine(@"VALUE LABELS aBase 1 ""Base"".");
            txt_writer.WriteLine("");

            txt_writer.WriteLine("COMPUTE NPS=1.");
            txt_writer.WriteLine("RECODE NPS (1=SYSMIS).");
            txt_writer.WriteLine(@"VALUE LABELS NPS 1 ""NPS Score"".");
            txt_writer.WriteLine("");

            if (comAnalysisType.Text == "Weighted Analysis")
            {
                txt_writer.WriteLine("COMPUTE uBase=1.");
                txt_writer.WriteLine(@"VALUE LABELS uBase 1 ""Base: Unweighted"".");
                txt_writer.WriteLine("");
            }

            txt_writer.WriteLine("EXECUTE.");

            txt_writer.WriteLine("");
            txt_writer.WriteLine(""); txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");


            txt_writer.WriteLine("* ADD FILES /FILE=*");
            txt_writer.WriteLine(@" /FILE='D:\DBI Projects\2023 Projects\Data_Dummy.sav'.");
            txt_writer.WriteLine("* EXECUTE.");

            txt_writer.WriteLine("COMPUTE DummyATotal=1.");
            txt_writer.WriteLine(@"VALUE LABELS DummyATotal 1 ""DummyTotal"".");

            txt_writer.WriteLine("COMPUTE Dummy=1.");
            txt_writer.WriteLine(@"VARIABLE LABELS Dummy ""DUMMY ROW"".");

            txt_writer.WriteLine("");
            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");

            for (int i = 0; i < lstTableType.Count; i++)
            {
                if (lstTableType[i] == "5")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (88=sysmis) (99=sysmis).")

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) into xT1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) (4=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xB1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xB2_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT1_" + lstVariableName[i] + ") xT1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB1_" + lstVariableName[i] + ") xB1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("VALUE LABELS xT1_" + lstVariableName[i] + @" 1 ""TOP BOX [05]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [05/04]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "6")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (1=5)(2=4)(3=3)(4=2)(5=1).");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName[i] + " (88=sysmis) (99=sysmis).")

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xT1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (2=1) (1=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) into xB1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (5=1) (4=1) into xB2_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT1_" + lstVariableName[i] + ") xT1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB1_" + lstVariableName[i] + ") xB1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("VALUE LABELS xT1_" + lstVariableName[i] + @" 1 ""TOP BOX [01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [05]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [05/04]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "7")
                {
                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) into xT1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) (5=1) into xT3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xB1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xB2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into xB3_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT1_" + lstVariableName[i] + ") xT1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT3_" + lstVariableName[i] + ") xT3_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB1_" + lstVariableName[i] + ") xB1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB3_" + lstVariableName[i] + ") xB3_" + lstVariableName[i] + "=99.");


                    txt_writer.WriteLine("VALUE LABELS xT1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [07/06]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [07/06/05]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [01/02/03]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "8")
                {
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xR1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xR2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into xR3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "9")
                {
                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) into xT1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) (7=1) into xT3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xB1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xB2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into xB3_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT1_" + lstVariableName[i] + ") xT1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT3_" + lstVariableName[i] + ") xT3_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB1_" + lstVariableName[i] + ") xB1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB3_" + lstVariableName[i] + ") xB3_" + lstVariableName[i] + "=99.");


                    txt_writer.WriteLine("VALUE LABELS xT1_" + lstVariableName[i] + @" 1 ""TOP BOX [09]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [09/08]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [09/08/07]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [01/02/03]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "10")
                {
                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into xT1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) (8=1) into xT3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into xB1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xB2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into xB3_" + lstVariableName[i] + ".");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT1_" + lstVariableName[i] + ") xT1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT3_" + lstVariableName[i] + ") xT3_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB1_" + lstVariableName[i] + ") xB1_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB3_" + lstVariableName[i] + ") xB3_" + lstVariableName[i] + "=99.");


                    txt_writer.WriteLine("VALUE LABELS xT1_" + lstVariableName[i] + @" 1 ""TOP BOX [10]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [10/09]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [10/09/08]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [01/02/03]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "11")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) (9=1) into xT3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) (9=1) (8=1) into xT4_" + lstVariableName[i] + ".");


                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) into B1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) into xB2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) into xB3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) into xB4_" + lstVariableName[i] + ".");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) into xB5_" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (1=1) (2=1) (3=1) (4=1) (5=1) (6=1) into xB6_" + lstVariableName[i] + ".");


                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT2_" + lstVariableName[i] + ") xT2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT3_" + lstVariableName[i] + ") xT3_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xT4_" + lstVariableName[i] + ") xT4_" + lstVariableName[i] + "=99.");

                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB2_" + lstVariableName[i] + ") xB2_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB3_" + lstVariableName[i] + ") xB3_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB4_" + lstVariableName[i] + ") xB4_" + lstVariableName[i] + "=99.");

                    //txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB5_" + lstVariableName[i] + ") xB5_" + lstVariableName[i] + "=99.");
                    //txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xB6_" + lstVariableName[i] + ") xB6_" + lstVariableName[i] + "=99.");



                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [11/10]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [11/10/09]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT4_" + lstVariableName[i] + @" 1 ""TOP 4 BOX [11/10/09/08]""  99  ""DUMMY ROW"".");

                    //txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [00/01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [00/01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB4_" + lstVariableName[i] + @" 1 ""BOTTOM 4 BOX [00/01/02/03]""  99  ""DUMMY ROW"".");

                    //txt_writer.WriteLine("VALUE LABELS xB5_" + lstVariableName[i] + @" 1 ""BOTTOM 5 BOX [01/02/03/04/05]""  99  ""DUMMY ROW"".");
                    //txt_writer.WriteLine("VALUE LABELS xB6_" + lstVariableName[i] + @" 1 ""BOTTOM 6 BOX [01/02/03/04/05/06]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "12")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) into xTPro_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (8=1) (7=1) into xTPas_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (6=1) (5=1) (4=1) (3=1) (2=1) (1=1) (0=1) into xTDic_" + lstVariableName[i] + ".");


                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTPro_" + lstVariableName[i] + ") xTPro_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTPas_" + lstVariableName[i] + ") xTPas_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTDic_" + lstVariableName[i] + ") xTDic_" + lstVariableName[i] + "=99.");

                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPro_" + lstVariableName[i] + @" 1 ""Promoter [9-10]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xTPas_" + lstVariableName[i] + @" 1 ""Passive [7-8]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xTDic_" + lstVariableName[i] + @" 1 ""Detractor [0-6]""  99  ""DUMMY ROW"".");

                    txt_writer.WriteLine("");
                }

                //txt_writer.Close();
            }
            txt_writer.WriteLine(" ");
            txt_writer.WriteLine("EXECUTE.");
        }
        //***************************** Column Percentage Answer Base Script ***************************************
        private void columnPctScriptAnswerBase()
        {
            if (!writeBannerSyntax()) return;

            txt_writer.WriteLine("************************** Allah is Almighty********************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("Filter Off.");
            txt_writer.WriteLine("USE ALL.");
            txt_writer.WriteLine("");


            //txt_writer.WriteLine("Define @ColVar() ATotal*Product+(Center+UserType)*Product  !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal Center UserType   Product  !Enddefine.");

            //txt_writer.WriteLine("Define @ColVar() ATotal+S1Banner+S2Banner+DhakaBanner+DhakaUrbanRuralBanner+CTGBanner+CTGUrbanRuralBanner+SECBanner+PBQ1Banner !Enddefine.");
            //txt_writer.WriteLine("Define @ColVarPct() ATotal S1Banner S2Banner DhakaBanner DhakaUrbanRuralBanner CTGBanner CTGUrbanRuralBanner SECBanner PBQ1Banner !Enddefine.");

            txt_writer.WriteLine("Define @ColVar() " + bannerText1 + " !Enddefine.");
            txt_writer.WriteLine("Define @ColVarPct() " + bannerText2 + " !Enddefine.");


            txt_writer.WriteLine("");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMS");
            txt_writer.WriteLine("/SELECT TABLES");
            txt_writer.WriteLine("/IF COMMANDS=['Tables'] SUBTYPES=['Table']");
            txt_writer.WriteLine("/DESTINATION FORMAT=XLSX");
            txt_writer.WriteLine("Viewer = No");
            txt_writer.WriteLine(@"OUTFILE='" + myPath + "\\01." + outputExcelFileName + " -Cpt.xlsx'.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("");

            if (comAnalysisType.Text == "Weighted Analysis")
                txt_writer.WriteLine("WEIGHT by Weight.");
            txt_writer.WriteLine("");

            int i_TableNo = 1;
            int i_varCount = 0;



            for (int i = 0; i < lstTableType.Count; i++)
            {

                //************************************ For Table ************************************************

                if (lstTableType[i] == "1")
                {
                    //5	Single Response	CpcT

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "2")
                {
                    //6	Multiple Response	CpcT


                    mrVarList = mrVarList + lstVariableName[i] + " ";
                    i_varCount++;

                    if (lstMRBreakPoint[i] == "XXX")
                    {

                        if (lstFilterCondition[i] == "")
                        {
                            if (lstFilterLabel[i] != "All Respondents")
                            {
                                txt_writer.WriteLine("Compute aBase=1.");
                                txt_writer.WriteLine("COUNT myCount= " + mrVarList + " (Missing).");
                                txt_writer.WriteLine("if myCount=" + i_varCount.ToString() + " aBase=$Sysmis.");
                                if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                                txt_writer.WriteLine("");
                            }

                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ANSWERING");
                            if (comAnalysisType.Text == "Weighted Analysis")
                                txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank by @ColVar");
                            else
                                txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank by @ColVar");

                            if (comAnalysisType.Text == "Weighted Analysis")
                                txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                            else
                                txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");


                            txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", " ") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            if (lstFilterLabel[i] != "")
                                txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            else
                                txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        else if (lstFilterCondition[i] != "")
                        {

                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");


                            txt_writer.WriteLine("Tables Observation nBlank");
                            txt_writer.WriteLine("/Ptotal=t1 'Base'");
                            txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                            txt_writer.WriteLine("/Format=Zero MISSING ('')");
                            txt_writer.WriteLine("/MrGroup=Mr1 '' " + mrVarList + "");
                            txt_writer.WriteLine("/BASE=ANSWERING");
                            if (comAnalysisType.Text == "Weighted Analysis")
                                txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank by @ColVar");
                            else
                                txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank by @ColVar");

                            if (comAnalysisType.Text == "Weighted Analysis")
                                txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                            else
                                txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");


                            txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        i_TableNo = i_TableNo + 1;
                        i_varCount = 0;
                    }
                }
                else if (lstTableType[i] == "3")
                {
                    // For Single Response With Mean

                    if (lstFilterCondition[i] == "")
                    {
                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");


                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("COMPUTE m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                        txt_writer.WriteLine("");

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Tables Observation nBlank m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");


                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;

                    //    txt_writer.WriteLine("Tables Observation nBlank m_S1");
                    //    txt_writer.WriteLine("/Ptotal=t1 'Base'");
                    //    txt_writer.WriteLine("/Ftotal = f1 ""Total""");
                    //    txt_writer.WriteLine("/Format=Zero MISSING ('')");
                    //    txt_writer.WriteLine("/MrGroup=Mr1 '' ExS1");
                    //    txt_writer.WriteLine("/BASE=ANSWERING");
                    //    txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank+m_S1 by @ColVar");
                    //                            if (comAnalysisType.Text == "Weighted Analysis")
                    //    txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                    //    txt_writer.WriteLine("Mean(m_S1 (F3.2)'MEAN')");
                    //    txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString + ": QS1. Number of Member in Household'");
                    //    txt_writer.WriteLine("/Corner='Base : All Respondent'.");

                    i_TableNo = i_TableNo + 1;

                }
                else if (lstTableType[i] == "4")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "5")
                {
                    //1	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        //txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2 by @ColVar");

                        //txt_writer.WriteLine("/Stat=Count(aBase(F5)'') Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");


                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");


                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {
                        //Prepare variable for answer base
                        //txt_writer.WriteLine("Compute Temp" + lstVariableName[i] + "=" + lstVariableName[i]+".");


                        //********************************
                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");

                        //txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2 by @ColVar");

                        //txt_writer.WriteLine("/Stat=Count(aBase(F5)'') Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");

                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "6")
                {
                    //2	Scaled Question (5)	T1B T2B Cpct B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        //txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2 by @ColVar");

                        //txt_writer.WriteLine("/Stat=Count(aBase(F5)'') Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");


                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");

                        //txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2 by @ColVar");

                        //txt_writer.WriteLine("/Stat=Count(aBase(F5)'') Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        //txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) ");


                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "7")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }

                else if (lstTableType[i] == "8")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xR1_" + lstVariableName[i] + "+xR2_" + lstVariableName[i] + "+xR3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");


                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xR1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xR2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xR3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xR1_" + lstVariableName[i] + "+xR2_" + lstVariableName[i] + "+xR3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xR1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xR2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xR3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "9")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "10")
                {
                    //3	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xB1_" + lstVariableName[i] + "+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT1_" + lstVariableName[i] + "+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+dummy+nBlank+Mr1+@Row2+@Row1 by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xT1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Cpct(xB1_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title=Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "11")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+xB4_" + lstVariableName[i] + "+xB5_" + lstVariableName[i] + "+xB6_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + "+xT4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + "+xT4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(xB5_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(xB6_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) "); txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xB2_" + lstVariableName[i] + "+xB3_" + lstVariableName[i] + "+xB4_" + lstVariableName[i] + "+xB5_" + lstVariableName[i] + "+xB6_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+SigTest+m_" + lstVariableName[i] + " !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + "+xT4_" + lstVariableName[i] + "+nBlank+SigTest !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+xT2_" + lstVariableName[i] + "+xT3_" + lstVariableName[i] + "+xT4_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest m_" + lstVariableName[i]);
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + "+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xB2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xB3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xB4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(xB5_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)   Cpct(xB6_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) "); txt_writer.WriteLine("Cpct(xT2_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xT3_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xT4_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "12")
                {
                    //4	Scaled Question (5+)	T1B T2B T3B Cpct B3B B2B B1B Mean S.T S.D. S.E. 

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute aBase=$Sysmis.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") aBase=1.");
                            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                            txt_writer.WriteLine("");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xTPro_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xTPas_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xTDic_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        if (lstFilterLabel[i] != "")
                            txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        else
                            txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    else if (lstFilterCondition[i] != "")
                    {

                        txt_writer.WriteLine("Compute aBase=$Sysmis.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " aBase=1.");
                        if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("Compute uBase=aBase.");
                        txt_writer.WriteLine("");


                        txt_writer.WriteLine("Define @Row1() nBlank+xTPro_" + lstVariableName[i] + "+xTPas_" + lstVariableName[i] + "+xTDic_" + lstVariableName[i] + " !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("Tables Observation nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ANSWERING");
                        //txt_writer.WriteLine("/Table=aBase+dummy+@Row1+Mr1+@Row2 by @ColVar");
                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row1+nBlank+NPS+nBlank by @ColVar");

                        if (comAnalysisType.Text == "Weighted Analysis")
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') U Count(uBase(F40)'')");
                        else
                            txt_writer.WriteLine("/Stat=Count(aBase(F5)'') ");

                        txt_writer.WriteLine("Cpct(Mr1 (f3." + decimalNumber + ")'' : @ColVarPct) ");
                        txt_writer.WriteLine("Cpct(xTPro_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct) Cpct(xTPas_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)  Cpct(xTDic_" + lstVariableName[i] + " (f3." + decimalNumber + ")'' : @ColVarPct)");
                        //txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
            }

            if (comAnalysisType.Text == "Weighted Analysis") txt_writer.WriteLine("WEIGHT Off.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("OMSEND.");

            //txt_writer.Close();
        }

        

        private void save_ExcellSheetAsText()
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

        private void load_AllList()
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

                    if (word.Length >= 9)
                    {
                        lstTableType.Add(word[0]);
                        lstVariableName.Add(word[1]);
                        lstVariableLabel.Add(removeDoubleCot(word[3]));
                        lstMRUniqueVariableName.Add(word[4]);
                        lstMRBreakPoint.Add(word[5]);
                        lstMRVariableLabel.Add(removeDoubleCot(word[6]));
                        lstFilterCondition.Add(word[7]);
                        lstFilterLabel.Add(word[8] == "" ? "All Respondents" : word[8]);
                    }

                    strline = txtReader.ReadLine();
                }

                txtReader.Close();
            }
            else
            {
                MessageBox.Show("Please select a sheet");
            }
        }

        private void create_File(string syntaxType)
        {
            string basePath = txtStructureExcelPath.Text.Substring(0, txtStructureExcelPath.Text.LastIndexOf('\\'));
            string prefix   = syntaxType.StartsWith("Table_Cpt") ? "01" : syntaxType.StartsWith("Table_Count") ? "02" : "";
            string filePath = prefix == ""
                ? basePath + "\\" + txtOutputFileName.Text + "_" + syntaxType + ".SPS"
                : basePath + "\\" + prefix + "." + txtOutputFileName.Text + "_" + syntaxType + ".SPS";

            _currentSyntaxFilePath = filePath;
            txt_writer = new StreamWriter(filePath);
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

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.AppDomain.CurrentDomain.BaseDirectory + "CTableSyntax_Help.html";
            if (File.Exists(helpPath))
                Process.Start(helpPath);
            else
                MessageBox.Show("Help file not found:\n" + helpPath);
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

        private void frmAnalysisTable_Loaded(object sender, RoutedEventArgs e)
        {
            this.populateCombo();
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

        private async void btnBrowseGetStrucutureFile_Click(object sender, RoutedEventArgs e)
        {
            if (txtSpssDataFile.Text == "" || !File.Exists(txtSpssDataFile.Text))
            {
                MessageBox.Show("Please select a valid SPSS Data File (.sav) first.");
                return;
            }

            string spssPath    = txtSpssDataFile.Text;
            string outputFolder = myPath;

            btnBrowseGetStrucutureFile.IsEnabled = false;
            btnBrowseGetStrucutureFile.Content   = "Creating...";
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

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
                System.Windows.Input.Mouse.OverrideCursor = null;
                btnBrowseGetStrucutureFile.IsEnabled = true;
                btnBrowseGetStrucutureFile.Content   = "Create Structure File";
            }
        }

        private static Task RunOnStaThread(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            var thread = new Thread(() =>
            {
                try   { action(); tcs.SetResult(true); }
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

            xlCodeSheet.Cells[1,  1] = "0";  xlCodeSheet.Cells[1,  2] = "Don't use";
            xlCodeSheet.Cells[2,  1] = "1";  xlCodeSheet.Cells[2,  2] = "Single Response";              xlCodeSheet.Cells[2,  3] = "Column Pct";
            xlCodeSheet.Cells[3,  1] = "2";  xlCodeSheet.Cells[3,  2] = "Multiple Response";            xlCodeSheet.Cells[3,  3] = "Column Pct";
            xlCodeSheet.Cells[4,  1] = "3";  xlCodeSheet.Cells[4,  2] = "Single Response With Mean";    xlCodeSheet.Cells[4,  3] = "Column Pct with Mean";
            xlCodeSheet.Cells[5,  1] = "4";  xlCodeSheet.Cells[5,  2] = "Rank Response";               xlCodeSheet.Cells[5,  3] = "";
            xlCodeSheet.Cells[6,  1] = "5";  xlCodeSheet.Cells[6,  2] = "Scaled Question (5)";         xlCodeSheet.Cells[6,  3] = "T2B Cpct B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[7,  1] = "6";  xlCodeSheet.Cells[7,  2] = "Scaled Question - Reverse (5)"; xlCodeSheet.Cells[7, 3] = "T2B Cpct B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[8,  1] = "7";  xlCodeSheet.Cells[8,  2] = "Scaled Question (7)";         xlCodeSheet.Cells[8,  3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[9,  1] = "8";  xlCodeSheet.Cells[9,  2] = "";                            xlCodeSheet.Cells[9,  3] = "";
            xlCodeSheet.Cells[10, 1] = "9";  xlCodeSheet.Cells[10, 2] = "Scaled Question (9)";         xlCodeSheet.Cells[10, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[11, 1] = "10"; xlCodeSheet.Cells[11, 2] = "Scaled Question (10)";        xlCodeSheet.Cells[11, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[12, 1] = "11"; xlCodeSheet.Cells[12, 2] = "Scaled Question (11)";        xlCodeSheet.Cells[12, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E.";
            xlCodeSheet.Cells[13, 1] = "12"; xlCodeSheet.Cells[13, 2] = "NPS Question (11)";           xlCodeSheet.Cells[13, 3] = "CPT Promoter [9-10] Passive [7-8] Detractor [0-6]";

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
                    string temp1   = myWorksheet.Cells[i, 2].Value.ToString();
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

        private bool writeBannerSyntax()
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
            var bannerGroupOrder    = new List<string>();
            var dicBannerSpssVars   = new Dictionary<string, string>();
            var dicBannerLabel      = new Dictionary<string, string>();
            var dicBannerCodes      = new Dictionary<string, List<string>>();
            var dicBannerNewLabels  = new Dictionary<string, List<string>>();
            var dicBannerConditions = new Dictionary<string, List<string>>();

            Excel.Application bannerXlApp     = new Excel.Application();
            Excel.Workbook    bannerXlWorkBook = bannerXlApp.Workbooks.Open(txtStructureExcelPath.Text, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
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

                        string spssV     = c1 != null ? c1.ToString().Trim() : "";
                        string bannerV   = c2 != null ? c2.ToString().Trim() : "";
                        string labelV    = c3 != null ? c3.ToString().Trim() : "";
                        string newCode   = c4 != null ? c4.ToString().Trim() : "";
                        string newLabel  = c5 != null ? c5.ToString().Trim() : "";
                        string condition = c6 != null ? c6.ToString().Trim() : "";

                        if (string.IsNullOrWhiteSpace(spssV) || string.IsNullOrWhiteSpace(bannerV))
                            continue;

                        if (!bannerGroupOrder.Contains(bannerV))
                        {
                            bannerGroupOrder.Add(bannerV);
                            dicBannerSpssVars[bannerV]   = spssV;
                            dicBannerLabel[bannerV]      = labelV;
                            dicBannerCodes[bannerV]      = new List<string>();
                            dicBannerNewLabels[bannerV]  = new List<string>();
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
                var    codes      = dicBannerCodes[bannerV];
                var    newLabels  = dicBannerNewLabels[bannerV];
                var    conditions = dicBannerConditions[bannerV];
                string spssVars   = dicBannerSpssVars[bannerV];
                bool   isMultiVar = spssVars.Trim().Contains(" ");

                for (int vi = 0; vi < codes.Count; vi++)
                {
                    bool hasCode  = !string.IsNullOrWhiteSpace(codes[vi]);
                    bool hasLabel = !string.IsNullOrWhiteSpace(newLabels[vi]);
                    bool hasCond  = !string.IsNullOrWhiteSpace(conditions[vi]);
                    if ((hasCode || hasLabel || hasCond) && !(hasCode && hasLabel && hasCond))
                    {
                        validationErrors.Add("'" + bannerV + "' row " + (vi + 1) + ": New Code, New Label and Condition must all be filled or all empty.");
                        break;
                    }
                }

                bool anyFilled = codes.Any(c => !string.IsNullOrWhiteSpace(c));
                bool anyEmpty  = codes.Any(c =>  string.IsNullOrWhiteSpace(c));
                if (anyFilled && anyEmpty)
                    validationErrors.Add("'" + bannerV + "': cannot mix rows with and without custom grouping (New Code).");

                if (anyFilled)
                {
                    var filledCodes = codes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    if (filledCodes.Count != filledCodes.Distinct().Count())
                        validationErrors.Add("'" + bannerV + "': duplicate New Code values found.");

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

            // ── Write banner syntax ───────────────────────────────────────────
            txt_writer.WriteLine("");
            txt_writer.WriteLine("*Please specify the file get path.");
            txt_writer.WriteLine("GET ");
            txt_writer.WriteLine(@"FILE='" + myPath + "\\Data Preparation\\" + outputExcelFileName + "_Final.sav'.");
            txt_writer.WriteLine("DATASET NAME DataSet1 WINDOW=FRONT.");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("*************************************Banner************************************");
            txt_writer.WriteLine("");
            //txt_writer.WriteLine("COMPUTE ATotal=1.");
            //txt_writer.WriteLine(@"VALUE LABELS ATotal 1 ""Total"".");
            //txt_writer.WriteLine("");

            foreach (string bannerV in bannerGroupOrder)
            {
                string spssVars  = dicBannerSpssVars[bannerV];
                string bannerLbl = dicBannerLabel[bannerV];
                var    codes     = dicBannerCodes[bannerV];
                var    newLabels = dicBannerNewLabels[bannerV];
                var    conds     = dicBannerConditions[bannerV];
                bool   isCustom  = codes.Any(c => !string.IsNullOrWhiteSpace(c));
                bool   isMultiVar = spssVars.Trim().Contains(" ");

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
                    txt_writer.WriteLine("VALUE LABELS " + bannerV);
                    for (int ri = 0; ri < codes.Count; ri++)
                        txt_writer.WriteLine("  " + codes[ri] + " \"" + newLabels[ri] + "\"");
                    txt_writer.WriteLine(".");
                }

                txt_writer.WriteLine("");
            }

            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");
            txt_writer.WriteLine("EXECUTE.");
            txt_writer.WriteLine("");

            return true;
        }

        string GetBannerNestedVariable(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || !input.Contains(">"))
                return string.Empty;

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

            result.Add(levels.First());

            if (levels.Count > 2)
            {
                int midIndex = levels.Count / 2;
                result.Add(levels[midIndex]);
            }

            if (levels.Count > 1)
                result.Add(levels.Last());

            return string.Join(" ", result.Distinct());
        }

        static string GetBannerVariablesForLabel(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var levels = input.Split('>');

            List<string> variables = new List<string>();

            // Process root (first level)
            string root = levels[0];
            var rootParenMatches = Regex.Matches(root, @"\((.*?)\)");
            foreach (Match m in rootParenMatches)
            {
                variables.AddRange(m.Groups[1].Value.Split('+').Select(x => x.Trim()).Where(x => x != ""));
            }
            string rootWithoutParen = Regex.Replace(root, @"\((.*?)\)", "").Trim();
            variables.AddRange(rootWithoutParen.Split('+').Select(x => x.Trim()).Where(x => x != ""));

            // Process remaining levels, only variables inside parentheses
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
