using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using SpssLib.DataReader;
using SpssLib.SpssDataset;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmGetStructureFile.xaml
    /// </summary>
    public partial class FrmGetStructureFile : Window
    {
        String myPath = "";
        private SpssReader spssDataset;
        Dictionary<String, Variable> dicNameVsVariable = new Dictionary<String, Variable>();
        Dictionary<Int32, Variable> dicNumberVsVariable = new Dictionary<Int32, Variable>();
        Dictionary<String, Int32> dicNameVsFieldNumber = new Dictionary<String, Int32>();
        Dictionary<String, Dictionary<String, String>> dicNameVsDicValueVsLabel = new Dictionary<String, Dictionary<String, String>>();
        Dictionary<String, Dictionary<String, String>> dicNameVsDicOldVarVsNewVar = new Dictionary<String, Dictionary<String, String>>();
        List<String> lstOfNoRecodeVar = new List<string>();

        private Record myRecord;
        private bool populateData = false;

        public FrmGetStructureFile()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtSavFilePath.Text != "")
            {
                if (File.Exists(txtSavFilePath.Text) == true)
                {
                    // Open file, can be read only and sequetial (for performance), or anything else
                    using (FileStream fileStream = new FileStream(txtSavFilePath.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                    {
                        //this.createDatFileForError(txt_SAV_Location.Text.Substring(0, txt01SAVLocation.Text.LastIndexOf('\\')) + "\\MissingVarName.TXT");

                        //this.saveSAVLocation(txt_SAV_Location.Text, txt_File_Name.Text, txtWeekNo.Text);

                        dicNameVsVariable = new Dictionary<String, Variable>();
                        dicNumberVsVariable = new Dictionary<Int32, Variable>();
                        dicNameVsFieldNumber = new Dictionary<String, Int32>();

                        // Create the reader, this will read the file header
                        spssDataset = new SpssReader(fileStream);

                        int n = 1;
                        foreach (var variable in spssDataset.Variables)
                        {

                            dicNameVsVariable.Add(variable.Name, variable);
                            dicNumberVsVariable.Add(n, variable);
                            dicNameVsFieldNumber.Add(variable.Name, n);
                            n++;
                        }


                        this.exportToExcel();
                        this.prepareAnalysisCode();

                        MessageBox.Show("Structure file has been created successfully");

                    }
                }
            }
        }

        private void exportToExcel()
        {
            object misValue = System.Reflection.Missing.Value;

            Excel.Application xlApp2 = new Excel.Application();
            Excel.Workbook xlWorkBook2 = xlApp2.Workbooks.Add(misValue);
            Excel.Worksheet xlWorkSheet2 = new Excel.Worksheet();


            Excel.Sheets worksheets = xlWorkBook2.Worksheets;

            //xlWorkBook2 = xlApp.Workbooks.Add(misValue);


            var xlNewSheet2 = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlNewSheet2.Name = "Code Mapping";

            xlNewSheet2.Cells[1, 1] = "0";
            xlNewSheet2.Cells[1, 2] = "Don’t use";

            xlNewSheet2.Cells[2, 1] = "1";
            xlNewSheet2.Cells[2, 2] = "Single Response";
            xlNewSheet2.Cells[2, 3] = "Column Pct";

            xlNewSheet2.Cells[3, 1] = "2";
            xlNewSheet2.Cells[3, 2] = "Multiple Response";
            xlNewSheet2.Cells[3, 3] = "Column Pct";

            xlNewSheet2.Cells[4, 1] = "3";
            xlNewSheet2.Cells[4, 2] = "Single Response With Mean";
            xlNewSheet2.Cells[4, 3] = "Column Pct with Mean";

            xlNewSheet2.Cells[5, 1] = "4";
            xlNewSheet2.Cells[5, 2] = "Rank Response";
            xlNewSheet2.Cells[5, 3] = "";

            xlNewSheet2.Cells[6, 1] = "5";
            xlNewSheet2.Cells[6, 2] = "Scaled Question (5)";
            xlNewSheet2.Cells[6, 3] = "T2B Cpct B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[7, 1] = "6";
            xlNewSheet2.Cells[7, 2] = "Scaled Question - Reverse (5)";
            xlNewSheet2.Cells[7, 3] = "T2B Cpct B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[8, 1] = "7";
            xlNewSheet2.Cells[8, 2] = "Scaled Question (7)";
            xlNewSheet2.Cells[8, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[9, 1] = "8";
            xlNewSheet2.Cells[9, 2] = "";
            xlNewSheet2.Cells[9, 3] = "";

            xlNewSheet2.Cells[10, 1] = "9";
            xlNewSheet2.Cells[10, 2] = "Scaled Question (9)";
            xlNewSheet2.Cells[10, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[11, 1] = "10";
            xlNewSheet2.Cells[11, 2] = "Scaled Question (10)";
            xlNewSheet2.Cells[11, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[12, 1] = "11";
            xlNewSheet2.Cells[12, 2] = "Scaled Question (11)";
            xlNewSheet2.Cells[12, 3] = "T2B T3B Cpct B3B B2B Mean S.D. S.E. ";

            xlNewSheet2.Cells[13, 1] = "12";
            xlNewSheet2.Cells[13, 2] = "NPS Question (11)";
            xlNewSheet2.Cells[13, 3] = "CPT Promoter [9-10] Passive [7-8] Detractor [0-6]";

            xlNewSheet2.Columns.AutoFit();
            xlNewSheet2.Columns[1].ColumnWidth = 10;


            int i = 1;

            var xlNewSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlNewSheet.Name = "Table Structure";

            xlNewSheet.Cells[1, 1] = "Analysis Type";
            xlNewSheet.Cells[1, 2] = "Variable Name";
            xlNewSheet.Cells[1, 3] = "Variable Type";
            xlNewSheet.Cells[1, 4] = "Variable Label";
            xlNewSheet.Cells[1, 5] = "MR Variable Name";
            xlNewSheet.Cells[1, 6] = "Break Point";
            xlNewSheet.Cells[1, 7] = "MR Label";
            xlNewSheet.Cells[1, 8] = "Filter Conditioin";
            xlNewSheet.Cells[1, 9] = "Filter Label";

            foreach (KeyValuePair<String, Variable> pair in dicNameVsVariable)
            {
                if (!pair.Key.Contains("_OE"))
                {
                    xlNewSheet.Cells[i + 1, 1] = "";
                    xlNewSheet.Cells[i + 1, 2] = "" + pair.Key;
                    xlNewSheet.Cells[i + 1, 3] = "" + pair.Value.Type.ToString();
                    xlNewSheet.Cells[i + 1, 4] = pair.Value.Label == null ? "" : pair.Value.Label.ToString();
                    xlNewSheet.Cells[i + 1, 5] = "";
                    xlNewSheet.Cells[i + 1, 6] = "";
                    xlNewSheet.Cells[i + 1, 7] = "";
                    xlNewSheet.Cells[i + 1, 8] = "";
                    xlNewSheet.Cells[i + 1, 9] = "";
                    i = i + 1;
                }
            }
            //xlNewSheet.Columns.AutoFit();

            xlNewSheet.Columns[1].ColumnWidth = 11;
            xlNewSheet.Columns[2].ColumnWidth = 20;
            xlNewSheet.Columns[3].ColumnWidth = 12;
            xlNewSheet.Columns[4].ColumnWidth = 70;
            xlNewSheet.Columns[5].ColumnWidth = 15;
            xlNewSheet.Columns[6].ColumnWidth = 15;
            xlNewSheet.Columns[7].ColumnWidth = 15;
            xlNewSheet.Columns[8].ColumnWidth = 15;
            xlNewSheet.Columns[9].ColumnWidth = 15;

            xlNewSheet.Rows[1].Font.Bold = true;


            //xlNewSheet.get_Range("B:B").EntireColumn.Hidden = true;

            ((Excel.Worksheet)xlWorkBook2.Sheets[3]).Delete();


            //xlApp.Visible = true;

            //Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[getSheetIndex(sheetName, xlWorkBook2.Sheets)];
            Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[1];

            //oSheet.Sort.Apply();


            //string savefileName = txtSavFilePath.Text.Substring(txtSavFilePath.Text.LastIndexOf('\\') + 1);
            string savefileName = "SPSS Analysis strcture";
            savefileName = savefileName.Split('.')[0];
            //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
            xlWorkBook2.SaveAs(myPath + "\\" + savefileName + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);


            xlWorkBook2.Close(true, misValue, misValue);
            xlApp2.Quit();


            releaseObject(xlWorkSheet2);
            releaseObject(xlWorkBook2);
            releaseObject(xlApp2);

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

        private void btnBrowseSavFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "SPSS Database(*.sav)|*.sav*|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtSavFilePath.Text = openFileDialog1.FileName;
                    myPath = txtSavFilePath.Text.Substring(0, txtSavFilePath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtSavFilePath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }






        private void prepareAnalysisCode()
        {
            string sSelectedSheet = "Table Structure";
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtSavFilePath.Text))
            {
                //txtAnalysisExcelPath.Items.Clear();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(myPath + "\\SPSS Analysis strcture.xlsx", 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        //int ColNo = getOECodeColumnNumber(myWorksheet);
                        bool firstTime = true;
                        string priorQid = "";
                        string currentQid = "";
                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            //if (i == 24) 
                            //    MessageBox.Show("");
                            string temp1 = myWorksheet.Cells[i, 2].Value.ToString();
                            string varType = myWorksheet.Cells[i, 3].Value.ToString();

                            if (!varType.ToUpper().Contains("TEXT"))
                            {
                                if (!temp1.Contains("_"))
                                {
                                    myWorksheet.Cells[i, 1] = 1;


                                    if (firstTime == false)
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

                                    if (qId.Length == 2)
                                        currentQid = qId[0];
                                    else if (qId.Length == 3)
                                        currentQid = qId[0] + "_" + qId[1];

                                    if (priorQid != currentQid && firstTime == false)
                                    {
                                        myWorksheet.Cells[i, 1] = 2;

                                        myWorksheet.Cells[i - 1, 5] = priorQid;
                                        myWorksheet.Cells[i - 1, 6] = "XXX";
                                        myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i - 1, 4];

                                    }
                                    else if (priorQid != currentQid && firstTime == true)
                                    {
                                        myWorksheet.Cells[i, 1] = 2;
                                    }
                                    else if (priorQid == currentQid)
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
                                if (firstTime == false)
                                {
                                    myWorksheet.Cells[i - 1, 1] = 2;

                                    myWorksheet.Cells[i - 1, 5] = priorQid;
                                    myWorksheet.Cells[i - 1, 6] = "XXX";
                                    myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i - 1, 4];
                                }
                                firstTime = true;
                            }


                            //if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            //{
                            //    chkListBoxRespondentId.Items.Add(temp1);

                            //}
                        }

                    }
                }

                xlWorkBook.Save();

                xlWorkBook.Close();
                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                //MessageBox.Show("Write Complete");
            }
        }

    }
}
