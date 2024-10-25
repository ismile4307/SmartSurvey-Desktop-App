using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for FrmTableLink.xaml
    /// </summary>
    public partial class FrmTableLink : Window
    {
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkBook;
        private Excel.Worksheet xlWorkSheet;


        private String myPath, projectName;
        public FrmTableLink()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtTableSheetName.Text = "";
                txtLinkSheetName.Text = "";

                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtAnalysisExcelPath.Text = openFileDialog1.FileName;
                    myPath = txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\'));
                    projectName = txtAnalysisExcelPath.Text.Substring(txtAnalysisExcelPath.Text.LastIndexOf('\\') + 1);

                    projectName = projectName.Substring(0, projectName.LastIndexOf("."));//.Split('_')[0];

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();


                    renameAndAddSheet(txtAnalysisExcelPath.Text);

                    loadWorkSheet();



                }
                else
                    txtAnalysisExcelPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void renameAndAddSheet(string excelPath)
        {
            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(excelPath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Sheets worksheets = xlWorkBook.Worksheets;

            //foreach (Excel.Sheets sheet in xlWorkBook.Sheets)
            //{
            //    if (sheet.Name.equals("sheetName"))
            //    {
            //        //do something
            //    }
            //}
            bool hasTableSheet = false;
            bool hasIndexSheet = false;
            bool hasSheet1Sheet = false;

            for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
            {
                if (xlWorkBook.Worksheets[i].Name.ToString() == "Table")
                    hasTableSheet = true;
                if (xlWorkBook.Worksheets[i].Name.ToString() == "Index")
                    hasIndexSheet = true;
                if (xlWorkBook.Worksheets[i].Name.ToString() == "Sheet1")
                    hasSheet1Sheet = true;


            }

            if (hasTableSheet == false && hasSheet1Sheet == true)
            {
                var mySheet = (Excel.Worksheet)xlWorkBook.Worksheets["Sheet1"];
                mySheet.Name = "Table";
            }

            if (hasIndexSheet == false)
            {
                var xlNewSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
                xlNewSheet.Name = "Index";
            }

            xlWorkBook.Save();
            xlApp.Quit();

            releaseObject(worksheets);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void loadWorkSheet()
        {
            if (File.Exists(txtAnalysisExcelPath.Text) == true)
            {
                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);

                txtTableSheetName.Text = "";
                txtLinkSheetName.Text = "";
                for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                {
                    if (xlWorkBook.Worksheets[i].Name.ToString() == "Table")
                        txtTableSheetName.Text = "Table";
                    if (xlWorkBook.Worksheets[i].Name.ToString() == "Index")
                        txtLinkSheetName.Text = "Index";
                }


                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRowNo.Text = "2";
            txtColumnNo.Text = "2";

            radioTableIndex.IsChecked = true;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //if (outputSheetlList.Items.Count < 1 || inputSheetList.Items.Count < 1)
            //{
            //    MessageBox.Show("Please select input and output sheet");
            //    return;
            //}
            //*********************************************
            this.DeleteDummyColumn();
            //*********************************************

            //*********************************************
            this.DeleteDummyRow();
            //*********************************************


            int startRow = Int32.Parse(txtRowNo.Text.Trim().Length > 0 ? txtRowNo.Text : "2");
            int startColunm = Int32.Parse(txtColumnNo.Text.Trim().Length > 0 ? txtColumnNo.Text : "2");

            startRow = startRow + 1;

            if (startColunm > 25)
            {
                MessageBox.Show("Start Colunm must be less than 26");
                return;
            }
            string[] my_header = new string[50];

            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet inputSheet = null;
            Excel.Worksheet outputSheet = null;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);
            //xlWorkBook = xlApp.Workbooks.Open(txtRLDExcel.Text,        0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);

            string inputSheetName = txtTableSheetName.Text;
            string outputSheetName = txtLinkSheetName.Text;
            inputSheet = xlWorkBook.Worksheets.get_Item(inputSheetName);
            outputSheet = xlWorkBook.Worksheets.get_Item(outputSheetName);
            int i = 0;
            int j = 0;

            Excel.Range myRange;
            string[] tableTitle = new string[1000];
            string[] tableLink = new string[1000];
            string[] tableFilter = new string[1000];
            string[] tableBase = new string[1000];

            myRange = inputSheet.UsedRange;
            int r = myRange.Columns.Count;
            int index = 0;
            int filterIndex = 0;
            int baseIndex = 0;
            j = 1;
            for (i = 1; i < myRange.Rows.Count; i++)
            //  for(j=1;j<myRange.Columns.Count;j++)
            {
                string tmp = removeDoubleCot(String.Empty + inputSheet.Cells[i, j].value2);
                if (tmp.StartsWith("Table "))
                {
                    inputSheet.get_Range("A" + i.ToString(), "A" + i.ToString()).Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                    //Add the Table 1 : in table title
                    tableTitle[index] = tmp.Substring(tmp.IndexOf(':', 0) + 2); //tableTitle[index];


                    inputSheet.Cells[i, 1] = "Table " + (index + 1) + ": " + tmp.Substring(tmp.IndexOf(':', 0) + 2); 
                    tableLink[index++] = "=HYPERLINK(\"#'" + inputSheetName + "'!" + getCellName(i, j) + "\",\"";


                    if (index > 1)
                        inputSheet.Cells[i - 2, j] = "=HYPERLINK(\"#'" + outputSheetName + "'!" + getCellName(startRow + index - 1, startColunm + j) + "\",\"Home\")";
                }
                //else if (inputSheet.Cells[i, j].Value2 != null && inputSheet.Cells[i, j+1].Value2 == null && inputSheet.Cells[i, j+2].Value2 == null)
                //{
                //    string temp= inputSheet.Cells[i, j].Value.ToString();
                //    if (!temp.Contains("Home"))
                //    {
                //        inputSheet.get_Range("A" + i.ToString(), "A" + i.ToString()).Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                //        tableTitle[index] = tmp.Substring(tmp.IndexOf(':', 0) + 2);
                //        inputSheet.Cells[i, 1] = tableTitle[index];
                //        tableLink[index++] = "=HYPERLINK(\"#'" + inputSheetName + "'!" + getCellName(i, j) + "\",\"";


                //        if (index > 1)
                //            inputSheet.Cells[i - 2, j] = "=HYPERLINK(\"#'" + outputSheetName + "'!" + getCellName(startRow + index - 1, startColunm + j) + "\",\"Home\")";
                //    }
                //}
                else if (tmp.StartsWith("Base :"))
                {
                    tableFilter[filterIndex] = tmp;
                    filterIndex++;

                }
                //else if (tmp.StartsWith("Base"))
                else if (tmp == "Base")
                {
                    string myBase = removeDoubleCot(String.Empty + inputSheet.Cells[i, 2].value2);
                    tableBase[baseIndex] = myBase;
                    baseIndex++;
                }


            }


            outputSheet.Cells[startRow - 1, startColunm] = "Project : " + projectName;
            outputSheet.Cells[startRow - 1, startColunm + 1] = "";
            outputSheet.Cells[startRow - 1, startColunm + 2] = "";
            outputSheet.Cells[startRow - 1, startColunm + 3] = "";

            outputSheet.Cells[startRow, startColunm] = "Table No.";
            outputSheet.Cells[startRow, startColunm + 1] = "Table Title";
            outputSheet.Cells[startRow, startColunm + 2] = "Filter";
            outputSheet.Cells[startRow, startColunm + 3] = "Base";

            myRange.Font.Bold = false;
            for (i = 0; i < index; i++)
            {
                if (radioTableTitle.IsChecked == true)
                {
                    outputSheet.Cells[i + startRow + 1, startColunm] = "Table " + (i + 1).ToString().PadLeft(2, '0');
                    outputSheet.Cells[i + startRow + 1, startColunm + 1] = tableLink[i] + tableTitle[i] + "\")";
                    outputSheet.Cells[i + startRow + 1, startColunm + 2] = tableFilter[i];
                    outputSheet.Cells[i + startRow + 1, startColunm + 3] = tableBase[i];
                }
                else
                {
                    outputSheet.Cells[i + startRow + 1, startColunm] = tableLink[i] + "Table " + (i + 1).ToString().PadLeft(2, '0') + "\")";
                    outputSheet.Cells[i + startRow + 1, startColunm + 1] = tableTitle[i];
                    outputSheet.Cells[i + startRow + 1, startColunm + 2] = tableFilter[i];
                    outputSheet.Cells[i + startRow + 1, startColunm + 3] = tableBase[i];
                }

            }

            //outputSheet.Columns.AutoFit();
            outputSheet.Columns[startColunm].ColumnWidth = 10;
            outputSheet.Columns[startColunm + 1].ColumnWidth = 80;
            outputSheet.Columns[startColunm + 2].ColumnWidth = 22;
            outputSheet.Columns[startColunm + 3].ColumnWidth = 10;

            //**********************************************************************
            //Design Table head
            // myRange.Borders[XlBordersIndex.xlEdgeBottom];
            string startCell = getCellName(startRow - 1, startColunm);
            string endCell = getCellName(startRow - 1, startColunm + 3);
            outputSheet.Range[startCell + ":" + endCell].Borders.LineStyle = 1;
            myRange = outputSheet.get_Range(startCell, endCell);
            myRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);


            outputSheet.Range[getCellName(startRow - 1, startColunm), getCellName(startRow - 1, startColunm + 3)].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);
            outputSheet.Range[getCellName(startRow - 1, startColunm), getCellName(startRow - 1, startColunm + 3)].Font.Bold = true;

            outputSheet.Range[getCellName(startRow - 1, startColunm), getCellName(startRow - 1, startColunm + 3)].Merge();
            myRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            //**********************************************************************

            // Desing full table
            // myRange.Borders[XlBordersIndex.xlEdgeBottom];
            startCell = getCellName(startRow, startColunm);
            endCell = getCellName(startRow + index, startColunm + 3);

            outputSheet.Range[startCell + ":" + endCell].Borders.LineStyle = 1;
            myRange = outputSheet.get_Range(startCell, endCell);
            myRange.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThick);


            outputSheet.Range[getCellName(startRow, startColunm), getCellName(startRow, startColunm + 3)].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);
            outputSheet.Range[getCellName(startRow, startColunm), getCellName(startRow, startColunm + 3)].Font.Bold = true;


            //Centre alignment the Base column
            startCell = getCellName(startRow, startColunm + 3);
            endCell = getCellName(startRow + index, startColunm + 3);
            outputSheet.get_Range(startCell, endCell).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            if (index != 0)
            {
                //outputSheet.Cells[index + startRow + 1, startColunm] = "Prepared By : DBI Research Private Ltd.";
                //outputSheet.Cells[index + startRow + 2, startColunm] = "Date : " + DateTime.Now.ToShortDateString().ToString();
                outputSheet.Cells[index + startRow + 1, startColunm] = "Date : " + DateTime.Now.ToShortDateString().ToString();
            }

            Excel.Worksheet worksheet1 = (Excel.Worksheet)xlApp.Worksheets["Index"];

            worksheet1.Select(true);
            //don't display gridline
            xlApp.ActiveWindow.DisplayGridlines = false;

            //outputSheet.Activate();            

            //xlApp.ActiveWindow.DisplayGridlines = false;
            xlWorkBook.Save();
            //xlApp.Visible = true;
            xlApp.Quit();

            releaseObject(inputSheet);
            releaseObject(outputSheet);
            releaseObject(xlWorkBook);
            releaseObject(xlApp);

            MessageBox.Show("Index for table has been created successfully");
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

            return returnString;
        }

        private string getCellName(int row, int colunm)
        {
            char a = (char)(64 + colunm);
            return a.ToString() + row.ToString();
        }

        private void DeleteDummyColumn()
        {
            Excel.Application xlApp1 = new Excel.Application();
            Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            //Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open("",,


            //Create and add the text file from excel into list
            for (int i = 1; i <= xlWorkBook1.Worksheets.Count; i++)
            {
                if (xlWorkBook1.Worksheets[i].Name.ToString() == "Table")
                {

                    Excel.Worksheet worksheet1 = (Excel.Worksheet)xlApp1.Worksheets["Table"];

                    worksheet1.Select(true);

                    Excel.Range range;
                    //Read the excel file
                    range = worksheet1.UsedRange;

                    //int ColNo = getOECodeColumnNumber(myWorksheet);
                    bool firstTime = true;
                    string priorQid = "";
                    string currentQid = "";
                    progressBar1.Minimum = 1;

                    bool flag = false;

                    for (int j = 2; j <= range.Rows.Count - 2; j++)
                    //for (int j = 5; j <= 100 - 1; j++)
                    {
                        progressBar1.Maximum = range.Rows.Count;
                        progressBar1.Value = j;
                        if (worksheet1.Cells[j, 2].Value2 != null)
                        {
                            string temp1 = worksheet1.Cells[j, 2].Value.ToString();


                            if (temp1 == "DummyTotal")
                            {
                                Excel.Range objRange = (Excel.Range)worksheet1.get_Range("B1", Missing.Value);
                                objRange.EntireColumn.Delete(Missing.Value);
                                goto myJump;
                            }
                            //MessageBox.Show(temp1 + "   " + j.ToString());
                        }
                    }

                    // Inserting 10 rows into the worksheet starting from 3rd row
                    //worksheet1.Rows.Insert(2,10);
                    //xlWorkBook1.SaveAs(Application.StartupPath + "\\" + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //lstTextFile.Add(Application.StartupPath + "\\" + sheetName + ".txt");
                }
            }

        myJump:
            //xlWorkBook1.Save();
            xlWorkBook1.Close(true);
            releaseObject(xlWorkBook1);
            releaseObject(xlApp1);
        }

        private void btnDeleteDummyRow_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp1 = new Excel.Application();
            Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            //Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open("",,


            //Create and add the text file from excel into list
            for (int i = 1; i <= xlWorkBook1.Worksheets.Count; i++)
            {
                if (xlWorkBook1.Worksheets[i].Name.ToString() == "Table")
                {

                    Excel.Worksheet worksheet1 = (Excel.Worksheet)xlApp1.Worksheets["Table"];

                    worksheet1.Select(true);

                    Excel.Range range;
                    //Read the excel file
                    range = worksheet1.UsedRange;

                    //int ColNo = getOECodeColumnNumber(myWorksheet);
                    bool firstTime = true;
                    string priorQid = "";
                    string currentQid = "";
                    progressBar1.Minimum = 1;

                    bool flag = false;

                    for (int j = 2; j <= range.Rows.Count - 2; j++)
                    //for (int j = 5; j <= 100 - 1; j++)
                    {
                        progressBar1.Maximum = range.Rows.Count;
                        progressBar1.Value = j;
                        if (worksheet1.Cells[j, 1].Value2 != null)
                        {
                            string temp1 = worksheet1.Cells[j, 1].Value.ToString();


                            if (temp1 == "DUMMY ROW")
                            {
                                worksheet1.Rows[j].Delete(1);
                                j--;
                            }
                            //MessageBox.Show(temp1 + "   " + j.ToString());
                        }
                    }

                    // Inserting 10 rows into the worksheet starting from 3rd row
                    //worksheet1.Rows.Insert(2,10);




                    //xlWorkBook1.SaveAs(Application.StartupPath + "\\" + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //lstTextFile.Add(Application.StartupPath + "\\" + sheetName + ".txt");
                }
            }

            //xlWorkBook1.Save();
            xlWorkBook1.Close(true);
            releaseObject(xlWorkBook1);
            releaseObject(xlApp1);
            //this.quitProcess();

            MessageBox.Show("Dummy Row deleted completed");
        }

        private void DeleteDummyRow()
        {
            Excel.Application xlApp1 = new Excel.Application();
            Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            //Excel.Workbook xlWorkBook1 = xlApp1.Workbooks.Open("",,


            //Create and add the text file from excel into list
            for (int i = 1; i <= xlWorkBook1.Worksheets.Count; i++)
            {
                if (xlWorkBook1.Worksheets[i].Name.ToString() == "Table")
                {

                    Excel.Worksheet worksheet1 = (Excel.Worksheet)xlApp1.Worksheets["Table"];

                    worksheet1.Select(true);

                    Excel.Range range;
                    //Read the excel file
                    range = worksheet1.UsedRange;

                    //int ColNo = getOECodeColumnNumber(myWorksheet);
                    bool firstTime = true;
                    string priorQid = "";
                    string currentQid = "";
                    progressBar1.Minimum = 1;

                    bool flag = false;

                    for (int j = 2; j <= range.Rows.Count - 2; j++)
                    //for (int j = 5; j <= 100 - 1; j++)
                    {
                        progressBar1.Maximum = range.Rows.Count;
                        progressBar1.Value = j;
                        if (worksheet1.Cells[j, 1].Value2 != null)
                        {
                            string temp1 = worksheet1.Cells[j, 1].Value.ToString();


                            if (temp1 == "DUMMY ROW")
                            {
                                worksheet1.Rows[j].Delete(1);
                                j--;
                            }
                            //MessageBox.Show(temp1 + "   " + j.ToString());
                        }
                    }

                    // Inserting 10 rows into the worksheet starting from 3rd row
                    //worksheet1.Rows.Insert(2,10);




                    //xlWorkBook1.SaveAs(Application.StartupPath + "\\" + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //lstTextFile.Add(Application.StartupPath + "\\" + sheetName + ".txt");
                }
            }

            //xlWorkBook1.Save();
            xlWorkBook1.Close(true);
            releaseObject(xlWorkBook1);
            releaseObject(xlApp1);
        }
    }
}
