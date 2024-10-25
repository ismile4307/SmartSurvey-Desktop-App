using DBI_Scripting.Classes;
using Microsoft.Win32;
using SpssLib.DataReader;
using SpssLib.SpssDataset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.WebPortal
{
    /// <summary>
    /// Interaction logic for FrmSRSyntaxPrep.xaml
    /// </summary>
    public partial class FrmSRSyntaxPrep : Window
    {
        private string myPath, sSelectedSheet;
        private List<TableStructure> listOfTableStructure;

        Dictionary<String, Variable> dicNameVsVariable;
        Dictionary<Int32, Variable> dicNumberVsVariable;
        Dictionary<String, Int32> dicNameVsFieldNumber;

        private Record myRecord;

        public FrmSRSyntaxPrep()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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


        private void btnBrowseSPSSData_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            string sTemp;

            sTemp = Properties.Settings.Default.StartupPath;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = sTemp;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "SPSS Data File (*.sav)|*.sav|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                txtSPSSPath.Text = openFileDialog1.FileName;
                //myPath = txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\'));
                Properties.Settings.Default.StartupPath = myPath;
                Properties.Settings.Default.Save();
            }
            else
                txtSPSSPath.Text = "";
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void loadWorksheet()
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (checkData())
            {
                prepareTableStructure();

                TextWriter txtWriter = new StreamWriter(myPath + "//Data_SR_" + txtProjectCode.Text + ".sql");

                txtWriter.WriteLine("DROP TABLE IF EXISTS `data_sr_" + txtProjectCode.Text + "`;");
                txtWriter.WriteLine("");
                txtWriter.WriteLine("");
                txtWriter.WriteLine("CREATE TABLE `data_sr_" + txtProjectCode.Text + "` (");
                txtWriter.WriteLine("`id` bigint(20) UNSIGNED NOT NULL,");
                //txtWriter.WriteLine("`project_code` int(11) NOT NULL,");
                txtWriter.WriteLine("`RespondentId` bigint(20) NOT NULL,");

                int noOfVar = listOfTableStructure.Count;
                for (int i = 0; i < noOfVar; i++)
                {
                    if (listOfTableStructure[i].variableName.ToUpper() != "RESPONDENTID")
                    {
                        if (i < noOfVar - 1)
                            txtWriter.WriteLine("`" + listOfTableStructure[i].variableName + "` varchar(" + listOfTableStructure[i].fieldWidth + ") COLLATE utf8_unicode_ci DEFAULT NULL,");
                        else if (i == noOfVar - 1)
                            txtWriter.WriteLine("`" + listOfTableStructure[i].variableName + "` varchar(" + listOfTableStructure[i].fieldWidth + ") COLLATE utf8_unicode_ci DEFAULT NULL");
                    }
                }

                txtWriter.WriteLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;");

                txtWriter.WriteLine("");
                txtWriter.WriteLine("");
                txtWriter.WriteLine("ALTER TABLE `data_sr_" + txtProjectCode.Text + "`");
                txtWriter.WriteLine("ADD PRIMARY KEY (`id`);");

                txtWriter.WriteLine("");
                txtWriter.WriteLine("");
                txtWriter.WriteLine("ALTER TABLE `data_sr_" + txtProjectCode.Text + "`");
                txtWriter.WriteLine("  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;");
                //txtWriter.WriteLine("COMMIT;");

                txtWriter.WriteLine("");
                txtWriter.WriteLine("");

                //*********************************************************************************

                //string file_Path = txtSPSSPath.Text.Substring(0, txtSPSSPath.Text.LastIndexOf('\\')) + "\\" + txt_File_Name.Text + ".sql";
                //this.createDatFile(file_Path);

                // Open file, can be read only and sequetial (for performance), or anything else
                using (FileStream fileStream = new FileStream(txtSPSSPath.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                {
                    // Create the reader, this will read the file header
                    SpssReader spssDataset = new SpssReader(fileStream);

                    dicNameVsVariable = new Dictionary<String, Variable>();
                    dicNumberVsVariable = new Dictionary<Int32, Variable>();
                    dicNameVsFieldNumber = new Dictionary<String, Int32>();

                    // Iterate through all the varaibles
                    int n = 1;
                    foreach (var variable in spssDataset.Variables)
                    {

                        dicNameVsVariable.Add(variable.Name, variable);
                        dicNumberVsVariable.Add(n, variable);
                        dicNameVsFieldNumber.Add(variable.Name, n);
                        n++;
                    }

                    int RecNumber = 0;
                    foreach (var record in spssDataset.Records)
                    {
                        RecNumber++;
                    }

                    //String qid = selectedQid;
                    String project_code = txtProjectCode.Text;

                    //*********************************************************************************


                    //*********************************************************************************

                    string s_temp;


                    string s_data;
                    int myCode;
                    int colPos;


                    //string insert_str = "INSERT INTO `data_sr_" + txtProjectCode.Text + "` (`project_code`, ";
                    string insert_str = "INSERT INTO `data_sr_" + txtProjectCode.Text + "` (";
                    noOfVar = listOfTableStructure.Count;
                    for (int i = 0; i < noOfVar; i++)
                    {
                        string field_name = listOfTableStructure[i].variableName;
                        insert_str = insert_str + "`" + field_name + "`, ";
                    }

                    insert_str = insert_str.Substring(0, insert_str.Length - 2) + " ) VALUES";

                    txtWriter.WriteLine(insert_str);

                    //*********************************************************************************
                    int p = 1;
                    List<String> listOfSyntax = new List<string>();

                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = RecNumber;
                    // Iterate through all data rows in the file
                    foreach (var record in spssDataset.Records)
                    {
                        myRecord = record;

                        progressBar1.Value = p;
                        DoEvents();
                        //************************** Write your code from Here *************************

                        string resp_id = field_Value("RespondentId");
                        //************************** Card No 1 *************************        


                        //string value_syntax = "(" + project_code + ", ";
                        string value_syntax = "(";

                        noOfVar = listOfTableStructure.Count;
                        for (int i = 0; i < noOfVar; i++)
                        {
                            string field_name = listOfTableStructure[i].variableNameDB;
                            if (field_name.ToUpper() == "RESPONDENTID")
                                value_syntax = value_syntax + field_Value(field_name) + ", ";
                            else
                            {
                                if (i < noOfVar - 1)
                                    value_syntax = value_syntax + "'" + field_Value(field_name).Replace("'", "''") + "', ";
                                else
                                    value_syntax = value_syntax + "'" + field_Value(field_name).Replace("'", "''") + "'), ";
                            }
                        }

                        if (p % 1000 == 0)
                        {
                            txtWriter.WriteLine(value_syntax.Substring(0, value_syntax.Length - 2) + ";");
                            txtWriter.WriteLine("");
                            txtWriter.WriteLine(insert_str);
                        }
                        else if (p <RecNumber)
                            txtWriter.WriteLine(value_syntax);
                        else
                        {
                            txtWriter.WriteLine(value_syntax.Substring(0, value_syntax.Length - 2) + ";");
                            //txtWriter.WriteLine("");
                        }
                        //******************************************************************************

                        p = p + 1;
                    }

                    txtWriter.Close();

                }
                //*********************************************************************************

                txtWriter.Close();

                MessageBox.Show("Write Complete");
            }
        }

        private string field_Value(String fieldName)
        {
            if (myRecord.GetValue(dicNameVsVariable[fieldName]) != null)
                if (myRecord.GetValue(dicNameVsVariable[fieldName]).ToString() == "-1")
                    return "";
                else
                    return myRecord.GetValue(dicNameVsVariable[fieldName]).ToString();
            else
                return "";
        }

        private string field_Value(Int32 fieldNumber)
        {
            if (myRecord.GetValue(dicNumberVsVariable[fieldNumber]) != null)
                return myRecord.GetValue(dicNumberVsVariable[fieldNumber]).ToString();
            else
                return "";
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();
        }

        private void prepareTableStructure()
        {

            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtAnalysisExcelPath.Text))
            {
                listOfTableStructure = new List<TableStructure>();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            string row_status = "";
                            if (myWorksheet.Cells[i, 4].Value2 != null)
                                row_status = myWorksheet.Cells[i, 4].Value.ToString();

                            if (row_status != "" && row_status != "0")
                            {
                                string varName = myWorksheet.Cells[i, 1].Value.ToString();
                                string varNameDB = myWorksheet.Cells[i, 1].Value.ToString();
                                if (myWorksheet.Cells[i, 6].Value2 != null)
                                {
                                    string varNameTemp = myWorksheet.Cells[i, 6].Value.ToString();
                                    if (varNameTemp != "")
                                        varName = varNameTemp;
                                }

                                string varWidth = myWorksheet.Cells[i, 3].Value.ToString();
                                string qLabel = "";
                                if (myWorksheet.Cells[i, 8].Value2 != null)
                                    qLabel = myWorksheet.Cells[i, 8].Value.ToString();

                                string qType = "";
                                if (myWorksheet.Cells[i, 9].Value2 != null)
                                    qType = myWorksheet.Cells[i, 9].Value.ToString();

                                string qOrder = "";
                                if (myWorksheet.Cells[i, 10].Value2 != null)
                                    qOrder = myWorksheet.Cells[i, 10].Value.ToString();

                                string showInSearch = "";
                                if (myWorksheet.Cells[i, 11].Value2 != null)
                                    showInSearch = myWorksheet.Cells[i, 11].Value.ToString();

                                string showInFreq = "";
                                if (myWorksheet.Cells[i, 12].Value2 != null)
                                    showInFreq = myWorksheet.Cells[i, 12].Value.ToString();

                                string showInCorss = "";
                                if (myWorksheet.Cells[i, 13].Value2 != null)
                                    showInCorss = myWorksheet.Cells[i, 13].Value.ToString();

                                string showInFilter = "";
                                if (myWorksheet.Cells[i, 14].Value2 != null)
                                    showInFilter = myWorksheet.Cells[i, 14].Value.ToString();

                                if (qType != "")
                                    listOfTableStructure.Add(new TableStructure(varName, varWidth, qLabel, qType, qOrder, showInSearch, showInFreq, showInCorss, showInFilter, varNameDB));
                            }
                        }

                    }
                }

                xlWorkBook.Close();
                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private bool checkData()
        {
            if (txtAnalysisExcelPath.Text == "")
            {
                MessageBox.Show("DB Structure excel should be selected");
                return false;
            }
            else
            {
                if (!File.Exists(txtAnalysisExcelPath.Text))
                {
                    MessageBox.Show("DB Structure excel file path is not correct");
                    return false;
                }
            }

            if (txtSPSSPath.Text == "")
            {
                MessageBox.Show("SPSS Data file should be selected");
                return false;
            }
            else
            {
                if (!File.Exists(txtSPSSPath.Text))
                {
                    MessageBox.Show("SPSS Data file path is not correct");
                    return false;
                }
            }
            if (txtProjectCode.Text == "")
            {
                MessageBox.Show("Project code should not be blank");
                return false;
            }

            return true;
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

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }
}
