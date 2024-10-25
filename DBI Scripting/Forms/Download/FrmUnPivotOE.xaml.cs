using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmUnPivotOE.xaml
    /// </summary>
    public partial class FrmUnPivotOE : Window
    {
        private String myPath;
        private String sSelectedSheet;
        private int myCounter;

        private List<String> lstOfOESheetName;
        private TextWriter txtWriter;

        private Dictionary<string, List<MyOEVerbatim>> dicSheetNameVsOEVerbatims;

        public FrmUnPivotOE()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowseExcelData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lstOfOESheetName = new List<String>();

                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel Data (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtExcelDataPath.Text = openFileDialog1.FileName;
                    myPath = txtExcelDataPath.Text.Substring(0, txtExcelDataPath.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtExcelDataPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadWorksheet()
        {
            try
            {
                if (File.Exists(txtExcelDataPath.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelDataPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked == true)
            {
                foreach (var item in chkListBoxWorksheet.Items)
                {
                    chkListBoxWorksheet.SelectedItems.Add(item);
                }
            }
            else
            {
                foreach (var item in chkListBoxWorksheet.Items)
                {
                    chkListBoxWorksheet.SelectedItems.Remove(item);
                }
            }
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            myCounter = 0;
            lstOfOESheetName.Clear();
            foreach (var item in chkListBoxWorksheet.Items)
            {
                for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
                {
                    if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
                    {
                        lstOfOESheetName.Add(item.ToString());
                        myCounter++;
                    }
                }

            }
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtExcelDataPath.Text != "")
            {
                if (File.Exists(txtExcelDataPath.Text))
                {
                    if (txtOESyntaxName.Text != "")
                    {
                        if (lstOfOESheetName.Count > 0)
                        {
                            if (File.Exists(txtExcelDataPath.Text) == true)
                            {
                                dicSheetNameVsOEVerbatims = new Dictionary<string, List<MyOEVerbatim>>();


                                Excel.Application xlApp = new Excel.Application();
                                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelDataPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                                {
                                    string sheetName = myWorksheet.Name;
                                    List<MyOEVerbatim> listOfOEVerbatims = new List<MyOEVerbatim>();
                                    
                                    if (lstOfOESheetName.Contains(sheetName))
                                    {

                                        Excel.Range range;
                                        //Read the excel file
                                        range = myWorksheet.UsedRange;
                                        for (int c = 2; c <= range.Columns.Count; c++)
                                        {
                                            for (int r = 1; r <= range.Rows.Count ; r++)
                                            {
                                                if(myWorksheet.Cells[r, c].Value2 != null)
                                                {
                                                    string respondentId = myWorksheet.Cells[r, 1].Value.ToString().Trim();
                                                    string oeText = myWorksheet.Cells[r, c].Value.ToString().Trim();
                                                    listOfOEVerbatims.Add(new MyOEVerbatim(respondentId, oeText));
                                                }
                                            }
                                        }

                                        dicSheetNameVsOEVerbatims.Add(sheetName, listOfOEVerbatims);
                                    }

                                    
                                }



                                releaseObject(xlWorkBook);
                                releaseObject(xlApp);
                                //xlApp.Quit();

                                //MessageBox.Show("");
                            }

                            this.exportToExcel();
                        }

                    }
                    else
                        MessageBox.Show("Have to write OE syntax file name");
                }
                else
                    MessageBox.Show("Invalid File Location");
            }
            else
                MessageBox.Show("Have to select OE Excel");
        }


        private void exportToExcel()
        {
            object misValue = System.Reflection.Missing.Value;

            Excel.Application xlApp2 = new Excel.Application();
            Excel.Workbook xlWorkBook2 = xlApp2.Workbooks.Add(misValue);
            Excel.Worksheet xlWorkSheet2 = new Excel.Worksheet();


            Excel.Sheets worksheets = xlWorkBook2.Worksheets;

            //xlWorkBook2 = xlApp.Workbooks.Add(misValue);

            int i = 1;
            foreach (KeyValuePair<string, List<MyOEVerbatim>> pair in dicSheetNameVsOEVerbatims)
            {
                var xlNewSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
                xlNewSheet.Name = pair.Key;

                //xlNewSheet.Cells[1, 1] = "'" + pair.Key;

                //xlNewSheet.Cells[3, 1] = "'Respondent Id";
                //xlNewSheet.Cells[3, 2] = "'Attribute Value";
                //xlNewSheet.Cells[3, 3] = "'Rsponse";
                //xlNewSheet.Cells[3, 4] = "'Code";

                for (int j = 1; j <= pair.Value.Count; j++)
                {
                    xlNewSheet.Cells[j, 1] = "'" + pair.Value[j - 1].respondentId;
                    xlNewSheet.Cells[j, 2] = "'" + pair.Value[j - 1].oeText;
                    //xlNewSheet.Cells[j, 3] = "'" + pair.Value[j - 1].Response;
                }
                xlNewSheet.Columns.AutoFit();
                i = i + 1;

                //xlNewSheet.get_Range("B:B").EntireColumn.Hidden = true;
            }

            if (dicSheetNameVsOEVerbatims.Count > 0)
                xlWorkBook2.Worksheets["Sheet1"].Delete();

            //xlApp.Visible = true;

            //Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[getSheetIndex(sheetName, xlWorkBook2.Sheets)];
            Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[1];

            oSheet.Sort.Apply();

            if (dicSheetNameVsOEVerbatims.Count > 0)
            {
                string savefileName = txtExcelDataPath.Text.Substring(txtExcelDataPath.Text.LastIndexOf('\\') + 1);
                savefileName = savefileName.Split('.')[0];
                //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook2.SaveAs(myPath + "\\" + savefileName + "_OE.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
            }

            xlWorkBook2.Close(true, misValue, misValue);
            xlApp2.Quit();


            releaseObject(xlWorkSheet2);
            releaseObject(xlWorkBook2);
            releaseObject(xlApp2);

        }

    }

    class MyOEVerbatim
    {
        public string respondentId;
        public string oeText;

        public MyOEVerbatim(string _respondentId, string _oeText)
        {
            respondentId = _respondentId;
            oeText = _oeText;
        }
    }

}


