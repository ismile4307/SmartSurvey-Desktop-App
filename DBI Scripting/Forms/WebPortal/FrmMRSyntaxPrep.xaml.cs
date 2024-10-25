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
    /// Interaction logic for FrmMRSyntaxPrep.xaml
    /// </summary>
    public partial class FrmMRSyntaxPrep : Window
    {
        private string myPath;
        private int Previous_Position, counter;
        private bool first_Time;
        private TextWriter txtWriter;
        private Record myRecord;
        private int card_No = 0;
        int p;

        private string sSelectedSheet;
        private string sSelectedQid;

        //private List<TableStructure> listOfTableStructure;
        Dictionary<String, String> dicSelectedVarvsVarArray;

        Dictionary<String, Variable> dicNameVsVariable;
        Dictionary<Int32, Variable> dicNumberVsVariable;
        Dictionary<String, Int32> dicNameVsFieldNumber;

        List<String> lstOfQid;
        public FrmMRSyntaxPrep()
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

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();

            this.loadMultipleResponseQlist();
        }


        private void loadMultipleResponseQlist()
        {
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtAnalysisExcelPath.Text))
            {

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        dicSelectedVarvsVarArray = new Dictionary<String, String>();
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        string mrVarList = "";

                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            if (myWorksheet.Cells[i, 4].Value2 != null)
                            {
                                string varName = myWorksheet.Cells[i, 1].Value.ToString();
                                string vartype = myWorksheet.Cells[i, 4].Value.ToString();

                                if (vartype == "6")
                                {
                                    if (myWorksheet.Cells[i, 7].Value2 != null)
                                    {
                                        string brkpoint = myWorksheet.Cells[i, 7].Value.ToString();
                                        string mrVarName = myWorksheet.Cells[i, 6].Value.ToString();
                                        if (brkpoint.ToUpper() == "XXX")
                                        {
                                            mrVarList = mrVarList + varName + ",";
                                            dicSelectedVarvsVarArray.Add(mrVarName, mrVarList.Substring(0, mrVarList.Length - 1));
                                            mrVarList = "";
                                        }
                                    }
                                    else
                                    {
                                        mrVarList = mrVarList + varName + ",";
                                    }
                                }
                            }


                        }
                        chkListBoxMRVarList.Items.Clear();
                        foreach (KeyValuePair<string, string> pair in dicSelectedVarvsVarArray)
                        {
                            chkListBoxMRVarList.Items.Add(pair.Key);
                        }
                    }
                }

                xlWorkBook.Close();
                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (txtSPSSPath.Text != "")
            {
                if (checkData())
                {
                    if (File.Exists(txtSPSSPath.Text) == true)
                    {
                        for (int i = 0; i < lstOfQid.Count; i++)
                        {
                            string destDir = txtSPSSPath.Text.Substring(0, txtSPSSPath.Text.LastIndexOf('\\')) + "\\MRResponse";
                            if (!Directory.Exists(destDir))
                                Directory.CreateDirectory(destDir);

                            string file_Path = destDir + "\\" + lstOfQid[i] + ".sql";

                            this.createDatFile(file_Path);

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

                                String qid = lstOfQid[i];
                                String project_code = txtProjectCode.Text;

                                string[] fieldName = dicSelectedVarvsVarArray[lstOfQid[i]].Split(',');
                                //*********************************************************************************
                                WritelnData(1, "DROP TABLE IF EXISTS `" + qid + "_" + project_code + "`;");
                                WritelnData(1, "");
                                WritelnData(1, "");
                                WritelnData(1, "CREATE TABLE `" + qid + "_" + project_code + "` (");
                                WritelnData(1, "  `id` bigint(20) NOT NULL,");
                                WritelnData(1, "  `RespondentId` bigint(20) NOT NULL,");
                                WritelnData(1, "  `qid` varchar(50) COLLATE utf8_unicode_ci NOT NULL,");
                                WritelnData(1, "  `response` varchar(10) COLLATE utf8_unicode_ci NOT NULL,");
                                WritelnData(1, "  `attribute_order` int(11) NOT NULL");
                                WritelnData(1, ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;");
                                WritelnData(1, "");
                                WritelnData(1, "ALTER TABLE `" + qid + "_" + project_code + "`");
                                WritelnData(1, "  ADD PRIMARY KEY (`id`);");
                                WritelnData(1, "");
                                WritelnData(1, "ALTER TABLE `" + qid + "_" + project_code + "`");
                                WritelnData(1, "  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;");
                                WritelnData(1, "COMMIT;");
                                WritelnData(1, "");
                                WritelnData(1, "INSERT INTO `" + qid + "_" + project_code + "` (`RespondentId`, `qid`, `response`, `attribute_order`) VALUES");

                                //*********************************************************************************

                                string s_temp;


                                string s_data;
                                int myCode;
                                int colPos;


                                //*********************************************************************************
                                p = 1;
                                List<String> listOfSyntax = new List<string>();

                                progressBar1.Minimum = 1;
                                progressBar1.Maximum = RecNumber;
                                // Iterate through all data rows in the file
                                foreach (var record in spssDataset.Records)
                                {
                                    myRecord = record;

                                    progressBar1.Value = p;
                                    card_No = 0;
                                    //************************** Write your code from Here *************************

                                    string resp_id = field_Value("RespondentId");
                                    //************************** Card No 1 *************************        
                                    //1	Slno	1	4	4
                                    //2	CardNo	8	9	2

                                    for (int x = 0; x < fieldName.Length; x++)
                                    {
                                        string myValue = field_Value(fieldName[x]);
                                        if (myValue != "")
                                            listOfSyntax.Add("(" + resp_id + ",'" + qid + "','" + myValue + "'," + myValue + "),");

                                    }









                                    //******************************************************************************

                                    p = p + 1;
                                }

                                if (listOfSyntax.Count > 0)
                                {
                                    for (int j = 0; j < listOfSyntax.Count - 1; j++)
                                    {
                                        WritelnData(1, listOfSyntax[j]);
                                    }
                                    string newStr = listOfSyntax[listOfSyntax.Count - 1];
                                    newStr = newStr.Substring(0, newStr.Length - 1) + ";";
                                    WritelnData(1, newStr);
                                }
                                else
                                    MessageBox.Show("No syntax found");

                                txtWriter.Close();
                            }

                            //String last = File.ReadLines(file_Path).Last();
                            //MessageBox.Show(last);
                        }

                        MessageBox.Show("Write Complete\nTotal No Of Record: " + (p - 1).ToString());
                    }
                }
            }
        }

        private bool checkData()
        {
            if (txtProjectCode.Text == "")
            {
                MessageBox.Show("Project Code should not be blank");
                return false;
            }
            if (lstOfQid.Count == 0)
            {
                MessageBox.Show("Question Id should be selected");
                return false;
            }
            return true;
        }
        private void createDatFile(string FilePath)
        {
            txtWriter = new StreamWriter(FilePath);
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

        private void chkListBoxMRVarList_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            //if (chkListBoxMRVarList.SelectedItems.Count > 1)
            //{
            //    string selecteditem = chkListBoxMRVarList.SelectedItems[0].ToString();
            //    //string item = e.Item as string;
            //    chkListBoxMRVarList.SelectedItems.Remove(selecteditem);
            //}

            //sSelectedQid = chkListBoxMRVarList.SelectedItems[0].ToString();



            lstOfQid.Clear();
            foreach (var item in chkListBoxMRVarList.Items)
            {
                for (int i = 0; i < chkListBoxMRVarList.SelectedItems.Count; i++)
                {
                    if (chkListBoxMRVarList.SelectedItems[i].ToString() == item.ToString())
                    {
                        lstOfQid.Add(item.ToString());

                    }
                }

            }

        }

        #region Data Write module
        //-----------My function Writeln data for Write New Line--------------------
        private void WriteData(int start_Position, string my_Value)
        {
            try
            {
                int padding_Value;
                string write_Value;

                padding_Value = start_Position - Previous_Position;

                if (Previous_Position > start_Position)
                {
                    MessageBox.Show("Error,  Card No. " + card_No.ToString() + " Start Position " + start_Position + " is invalid, Rec No : " + p.ToString());
                    return;
                }

                this.Previous_Position = start_Position + my_Value.Length;

                write_Value = my_Value.PadLeft(padding_Value + my_Value.Length, ' ');

                txtWriter.Write(write_Value);
                counter = counter + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        //-----------My function Writeln data for Write New Line--------------------
        private void WritelnData(int start_Position, string my_Value)
        {
            try
            {
                int padding_Value;
                string writeln_Value;
                card_No = card_No + 1;

                padding_Value = 0 + start_Position;

                this.Previous_Position = start_Position + my_Value.Length;

                writeln_Value = my_Value.PadLeft(padding_Value + my_Value.Length - 1, ' ');
                if (first_Time == false)
                    txtWriter.WriteLine();

                txtWriter.Write(writeln_Value);
                //wd.txt_writer.WriteLine(writeln_Value);
                counter = counter + 1;
                first_Time = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void frmAnalysisTable_Loaded(object sender, RoutedEventArgs e)
        {
            lstOfQid = new List<string>();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked == true)
            {
                foreach (var item in chkListBoxMRVarList.Items)
                {
                    chkListBoxMRVarList.SelectedItems.Add(item);
                }
            }
            else
            {
                foreach (var item in chkListBoxMRVarList.Items)
                {
                    chkListBoxMRVarList.SelectedItems.Remove(item);
                }
            }
        }
    }


}
