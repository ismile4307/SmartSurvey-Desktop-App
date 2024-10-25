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
                txtAnalysisExcelPath.Text = openFileDialog1.FileName;
                this.loadCategoryList();
                myPath = txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\'));

                Properties.Settings.Default.StartupPath = myPath;
                Properties.Settings.Default.Save();
            }
            else
                txtAnalysisExcelPath.Text = "";
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
            if (File.Exists(txtAnalysisExcelPath.Text) == true)
            {
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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
                txtBannerPath.Text = openFileDialog1.FileName;
                //my_Path = txt_Excel_Location.Text.Substring(0, txt_Banner.Text.LastIndexOf('\\'));

                Properties.Settings.Default.StartupPath = myPath;
                Properties.Settings.Default.Save();

                this.getBannerText();

            }
            else
                txtBannerPath.Text = "";
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
            TextReader txtReader = new StreamReader(txtBannerPath.Text);
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
            txtFileName.Text = outputExcelFileName;

            //MessageBox.Show("");
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            decimalNumber = txtDecimalPlace.Text;

            this.save_ExcellSheetAsText();
            this.load_AllList();

            if (dicFilterTypeVsCode[comBaseType.Text] == 1)
            {
                if (radiobtnRecode.IsChecked == true)
                {
                    this.create_File("Recode");
                    this.recodeScript();
                    txt_writer.Close();
                }
                else if (radiobtnTableCpt.IsChecked == true)
                {
                    this.create_File("Table_Cpt");
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
                if (radiobtnRecode.IsChecked == true)
                {
                    this.create_File("Recode_Answer_Base");
                    this.recodeScriptAnswerBase();
                    txt_writer.Close();
                }
                else if (radiobtnTableCpt.IsChecked == true)
                {
                    this.create_File("Table_Answer_Base_Cpt");
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
                if (lstTableType[i] == "1" || lstTableType[i] == "2")
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
                else if (lstTableType[i] == "3" || lstTableType[i] == "4")
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
                else if (lstTableType[i] == "10")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) into xTPro_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) into xTPas_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) (5=1) (4=1) (3=1) (2=1) (1=1) into xTDic_" + lstVariableName[i] + ".");


                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPro_" + lstVariableName[i] + @" 1 ""Promoter [9-10]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPas_" + lstVariableName[i] + @" 1 ""Passive [7-8]"".");
                    txt_writer.WriteLine("VALUE LABELS xTDic_" + lstVariableName[i] + @" 1 ""Detractor [0-6]"".");

                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "11")
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

                //txt_writer.Close();
            }
            txt_writer.WriteLine(" ");
            txt_writer.WriteLine("EXECUTE.");
        }

        //***************************** Column Percentage Script ***************************************
        private void columnPctScript()
        {
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
                else if (lstTableType[i] == "2")
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
                else if (lstTableType[i] == "3")
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
                else if (lstTableType[i] == "6")
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
                else if (lstTableType[i] == "7")
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
                else if (lstTableType[i] == "10")
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
            }
            txt_writer.WriteLine("OMSEND.");

            //txt_writer.Close();
        }

        //***************************** Table Count Script ***************************************

        private void tableCountScript()
        {
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

                //************************************ For Table ************************************************8
                if (lstTableType[i] == "1")
                {
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
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'') ");
                        txt_writer.WriteLine("Count(Mr1(F5)'')");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'') Count(T2_" + lstVariableName[i] + " (f5)'')");
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
                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'') ");
                        txt_writer.WriteLine("Count(Mr1(F5)'')");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'') Count(T2_" + lstVariableName[i] + " (f5)'')");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
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

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank SigTest");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'') ");
                        txt_writer.WriteLine("Count(Mr1(F5)'')");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'') Count(T2_" + lstVariableName[i] + " (f5)'')");
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
                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'') ");
                        txt_writer.WriteLine("Count(Mr1(F5)'')");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'') Count(T2_" + lstVariableName[i] + " (f5)'')");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }

                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "3")
                {

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
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'' )  Count(B3_" + lstVariableName[i] + " (f5)'' ) ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'' ) Count(T2_" + lstVariableName[i] + " (f5)'' )  Count(T3_" + lstVariableName[i] + " (f5)'' )");
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
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'' )  Count(B3_" + lstVariableName[i] + " (f5)'' ) ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'' ) Count(T2_" + lstVariableName[i] + " (f5)'' )  Count(T3_" + lstVariableName[i] + " (f5)'' )");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN') STDDEV(m_" + lstVariableName[i] + " (F3.2)'S. D.') SEMEAN(m_" + lstVariableName[i] + " (F3.2)'S. E.')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "4")
                {

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
                        txt_writer.WriteLine("/Ptotal=t1 'Base");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'' )  Count(B3_" + lstVariableName[i] + " (f5)'' ) ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'' ) Count(T2_" + lstVariableName[i] + " (f5)'' )  Count(T3_" + lstVariableName[i] + " (f5)'' )");
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
                        txt_writer.WriteLine("Define @Row1() nBlank+B1_" + lstVariableName[i] + "+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T1_" + lstVariableName[i] + "+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");

                        txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B1_" + lstVariableName[i] + " (f5)'') Count(B2_" + lstVariableName[i] + " (f5)'' )  Count(B3_" + lstVariableName[i] + " (f5)'' ) ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T1_" + lstVariableName[i] + " (f5)'' ) Count(T2_" + lstVariableName[i] + " (f5)'' )  Count(T3_" + lstVariableName[i] + " (f5)'' )");
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
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
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
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 '' " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+nBlank+Mr1 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
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
                            txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                            txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstMRVariableLabel[i].Replace("'", "") + "'");
                            txt_writer.WriteLine("/Caption=\"Home\"");
                            if (lstFilterLabel[i] != "")
                                txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                            else
                                txt_writer.WriteLine("/Corner='Base : All Respondents'.");
                            txt_writer.WriteLine("USE ALL.");
                            txt_writer.WriteLine("");

                            mrVarList = "";
                        }
                        if (lstFilterCondition[i] != "")
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
                            txt_writer.WriteLine("Count(Mr1 (f5)'') ");
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
                else if (lstTableType[i] == "7")
                {
                    // Single Response with Mean

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
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
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
                    if (lstFilterCondition[i] != "")
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
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Mean(m_" + lstVariableName[i] + " (F3.2)'MEAN')");
                        txt_writer.WriteLine("/Title='Table " + i_TableNo.ToString() + ": " + lstVariableLabel[i].Replace("'", "") + "'");
                        txt_writer.WriteLine("/Caption=\"Home\"");
                        txt_writer.WriteLine("/Corner='Base : " + lstFilterLabel[i] + "'.");
                        txt_writer.WriteLine("USE ALL.");
                        txt_writer.WriteLine("");
                    }
                    i_TableNo = i_TableNo + 1;
                }
                else if (lstTableType[i] == "11")
                {

                    if (lstFilterCondition[i] == "")
                    {
                        if (lstFilterLabel[i] != "All Respondents")
                        {
                            txt_writer.WriteLine("Compute Filt=0.");
                            txt_writer.WriteLine("if NOT Missing(" + lstVariableName[i] + ") Filt=1.");
                            txt_writer.WriteLine("Filter by Filt.");
                        }

                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B6_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)'') Count(B3_" + lstVariableName[i] + " (f5)'' )  Count(B4_" + lstVariableName[i] + " (f5)'' )   Count(B5_" + lstVariableName[i] + " (f5)'' )   Count(B6_" + lstVariableName[i] + " (f5)'' ) ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)'' ) Count(T3_" + lstVariableName[i] + " (f5)'' )  Count(T4_" + lstVariableName[i] + " (f5)'' )");
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
                    if (lstFilterCondition[i] != "")
                    {
                        txt_writer.WriteLine("Compute Filt=0.");
                        txt_writer.WriteLine(lstFilterCondition[i] + " Filt=1.");
                        txt_writer.WriteLine("Filter by Filt.");
                        txt_writer.WriteLine("Define @Row1() nBlank+B2_" + lstVariableName[i] + "+B3_" + lstVariableName[i] + "+B4_" + lstVariableName[i] + "+B5_" + lstVariableName[i] + "+B6_" + lstVariableName[i] + "+nBlank !Enddefine.");
                        //txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank+m_" + lstVariableName[i] + " !Enddefine.");
                        txt_writer.WriteLine("Define @Row2() nBlank+T2_" + lstVariableName[i] + "+T3_" + lstVariableName[i] + "+T4_" + lstVariableName[i] + "+nBlank !Enddefine.");

                        //txt_writer.WriteLine("Tables Observation m_" + lstVariableName[i] + " nBlank");
                        txt_writer.WriteLine("Tables Observation nBlank");
                        txt_writer.WriteLine("/Ptotal=t1 'Base'");
                        txt_writer.WriteLine(@"/Ftotal = f1 ""Total""");
                        txt_writer.WriteLine("/Format=Zero MISSING ('')");
                        txt_writer.WriteLine("/MrGroup=Mr1 ''  " + lstVariableName[i] + "");
                        txt_writer.WriteLine("/BASE=ALL");
                        txt_writer.WriteLine("/Table=t1+@Row1+Mr1+@Row2 by @ColVar");
                        txt_writer.WriteLine("/Stat=Count(t1(F5)'') Count(B2_" + lstVariableName[i] + " (f5)'') Count(B3_" + lstVariableName[i] + " (f5)'' )  Count(B4_" + lstVariableName[i] + " (f5)'' )  Count(B5_" + lstVariableName[i] + " (f5)'' )   Count(B6_" + lstVariableName[i] + " (f5)'' )  ");
                        txt_writer.WriteLine("Count(Mr1 (f5)'') ");
                        txt_writer.WriteLine("Count(T2_" + lstVariableName[i] + " (f5)'' ) Count(T3_" + lstVariableName[i] + " (f5)'' )  Count(T4_" + lstVariableName[i] + " (f5)'' )");
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
            txt_writer.WriteLine("");

            txt_writer.WriteLine("***************************************************************************");
            txt_writer.WriteLine("");

            for (int i = 0; i < lstTableType.Count; i++)
            {
                if (lstTableType[i] == "1" || lstTableType[i] == "2")
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
                else if (lstTableType[i] == "3" || lstTableType[i] == "4")
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
                else if (lstTableType[i] == "10")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (11=1) (10=1) into xTPro_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (9=1) (8=1) into xTPas_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (7=1) (6=1) (5=1) (4=1) (3=1) (2=1) (1=1) into xTDic_" + lstVariableName[i] + ".");


                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTPro_" + lstVariableName[i] + ") xTPro_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTPas_" + lstVariableName[i] + ") xTPas_" + lstVariableName[i] + "=99.");
                    txt_writer.WriteLine("IF NOT Missing (" + lstVariableName[i] + ") AND Missing(xTDic_" + lstVariableName[i] + ") xTDic_" + lstVariableName[i] + "=99.");

                    //txt_writer.WriteLine("VALUE LABELS T1_" + lstVariableName[i] + @" 1 ""TOP BOX [07]"".");
                    txt_writer.WriteLine("VALUE LABELS xTPro_" + lstVariableName[i] + @" 1 ""Promoter [9-10]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xTPas_" + lstVariableName[i] + @" 1 ""Passive [7-8]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xTDic_" + lstVariableName[i] + @" 1 ""Detractor [0-6]""  99  ""DUMMY ROW"".");

                    txt_writer.WriteLine("");
                }
                else if (lstTableType[i] == "11")
                {

                    txt_writer.WriteLine("Compute m_" + lstVariableName[i] + "=" + lstVariableName[i] + ".");
                    //txt_writer.WriteLine("RECODE m_" + lstVariableName [i] + " (88=sysmis) (99=sysmis).");

                    //txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) into T1_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) into xT2_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) (8=1) into xT3_" + lstVariableName[i] + ".");
                    txt_writer.WriteLine("RECODE " + lstVariableName[i] + " (10=1) (9=1) (8=1) (7=1) into xT4_" + lstVariableName[i] + ".");


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
                    txt_writer.WriteLine("VALUE LABELS xT2_" + lstVariableName[i] + @" 1 ""TOP 2 BOX [10/09]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT3_" + lstVariableName[i] + @" 1 ""TOP 3 BOX [10/09/08]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xT4_" + lstVariableName[i] + @" 1 ""TOP 4 BOX [10/09/08/07]""  99  ""DUMMY ROW"".");

                    //txt_writer.WriteLine("VALUE LABELS B1_" + lstVariableName[i] + @" 1 ""BOTTOM BOX [01]"".");
                    txt_writer.WriteLine("VALUE LABELS xB2_" + lstVariableName[i] + @" 1 ""BOTTOM 2 BOX [00/01]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB3_" + lstVariableName[i] + @" 1 ""BOTTOM 3 BOX [00/01/02]""  99  ""DUMMY ROW"".");
                    txt_writer.WriteLine("VALUE LABELS xB4_" + lstVariableName[i] + @" 1 ""BOTTOM 4 BOX [00/01/02/03]""  99  ""DUMMY ROW"".");

                    //txt_writer.WriteLine("VALUE LABELS xB5_" + lstVariableName[i] + @" 1 ""BOTTOM 5 BOX [01/02/03/04/05]""  99  ""DUMMY ROW"".");
                    //txt_writer.WriteLine("VALUE LABELS xB6_" + lstVariableName[i] + @" 1 ""BOTTOM 6 BOX [01/02/03/04/05/06]""  99  ""DUMMY ROW"".");
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
                else if (lstTableType[i] == "2")
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
                else if (lstTableType[i] == "3")
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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1 by @ColVar");

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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1 by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1 by @ColVar");

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
                else if (lstTableType[i] == "6")
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
                                txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1 by @ColVar");
                            else
                                txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1 by @ColVar");

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
                                txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1 by @ColVar");
                            else
                                txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1 by @ColVar");

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
                else if (lstTableType[i] == "7")
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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");

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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+nBlank+m_" + lstVariableName[i] + " by @ColVar");

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
                else if (lstTableType[i] == "10")
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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + " by @ColVar");

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
                            txt_writer.WriteLine("/Table=aBase+uBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + " by @ColVar");
                        else
                            txt_writer.WriteLine("/Table=aBase+nBlank+dummy+Mr1+@Row2+@Row1+nBlank+m_" + lstVariableName[i] + " by @ColVar");

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
            if (txtAnalysisExcelPath.Text != "")
            {
                if (File.Exists(txtAnalysisExcelPath.Text) == true)
                {
                    if (txtFileName.Text != "")
                    {
                        List<String> lstTextFile = new List<string>();

                        if (lstWorkSheetName.Count > 0)
                        {


                            xlApp = new Excel.Application();
                            xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
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

                    lstTableType.Add(word[0]);
                    lstVariableName.Add(word[1]);
                    lstVariableLabel.Add(removeDoubleCot(word[3]));
                    lstMRUniqueVariableName.Add(word[4]);
                    lstMRBreakPoint.Add(word[5]);
                    lstMRVariableLabel.Add(removeDoubleCot(word[6]));
                    lstFilterCondition.Add(word[7]);
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

        private void create_File(string syntaxType)
        {
            if (syntaxType == "Table_Cpt")
                txt_writer = new StreamWriter(txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\')) + "\\01." + txtFileName.Text + "_" + syntaxType + ".SPS");
            else if (syntaxType == "Table_Count")
                txt_writer = new StreamWriter(txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\')) + "\\02." + txtFileName.Text + "_" + syntaxType + ".SPS");
            else
                txt_writer = new StreamWriter(txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\')) + "\\" + txtFileName.Text + "_" + syntaxType + ".SPS");
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

        private void btnGetStructureHelp_Click(object sender, RoutedEventArgs e)
        {
            string sTemp;

            sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] arrayPath = sTemp.Split('\\');

            FileInfo fi = new FileInfo(sTemp + "\\SPSS Analysis strcture.xlsx");
            if (fi.Exists)
            {
                System.Diagnostics.Process.Start(sTemp + "\\SPSS Analysis strcture.xlsx");
            }
            else
            {
                //file doesn't exist
            }
        }

        private void btnGetBannerHelp_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("notepad++.exe", Application.StartupPath+"\\banner_help.txt");
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\banner_help.txt");
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            //if (chkListBoxWorksheet.SelectedItems.Count > 1)
            //{
            //    string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
            //    //string item = e.Item as string;
            //    chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            //}

            //sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();

            //lstWorkSheetName.Clear();
            //for (int i = 0; i < chkListBoxWorksheet.CheckedItems.Count; i++)
            //{
            //    lstWorkSheetName.Add(chkListBoxWorksheet.CheckedItems[i].ToString());
            //}


            foreach (var item in chkListBoxWorksheet.Items)
            {
                for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
                {
                    if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
                    {
                        lstWorkSheetName.Add(item.ToString());

                    }
                }
            }
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

    }
}
