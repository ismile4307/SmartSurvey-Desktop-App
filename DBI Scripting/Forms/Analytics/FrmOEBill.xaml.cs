using Microsoft.Win32;
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

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmOEBill.xaml
    /// </summary>
    public partial class FrmOEBill : Window
    {
        private string myPath;
        List<string> listOfSheetName;
        Dictionary<String, String> dicSheetNameVsCount;

        public FrmOEBill()
        {
            InitializeComponent();
        }

        private void btnBrowseOEExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel Data (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtExcelFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtExcelFileLocation.Text.Substring(0, txtExcelFileLocation.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtExcelFileLocation.Text = "";
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
                if (File.Exists(txtExcelFileLocation.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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
            int myCounter = 0;
            listOfSheetName.Clear();
            foreach (var item in chkListBoxWorksheet.Items)
            {
                for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
                {
                    if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
                    {
                        listOfSheetName.Add(item.ToString());
                        myCounter++;
                    }
                }

            }
            lblNoOfRejectionId.Content = "No of Rejection Id : " + (myCounter).ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtExcelFileLocation.Text != "")
            {
                if (File.Exists(txtExcelFileLocation.Text))
                {
                    dicSheetNameVsCount = new Dictionary<string, string>();
                    Dictionary<String, String> dicFileNameVsFilePath = new Dictionary<String, String>();
                    List<String> lstSheetName = new List<String>();
                    //dicVariableNameVsOEDataInfor = new Dictionary<String, OEDataInfo>();

                    List<String> listOfErrorMessage = new List<String>();

                    if (txtSaveFileName.Text != "")
                    {
                        if (listOfSheetName.Count > 0)
                        {

                            //TextWriter txtWriter = new StreamWriter(myPath + "\\05." + txtSaveFileName.Text + ".sps");

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                            Dictionary<String, String> dicIntnrVsOECode;
                            int intValueLength;
                            int totalcode = 0;
                            int totalcodehalf = 0;

                            //try
                            //{
                            //************************* This is for Single Variable ********************************************
                            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                            {
                                if (listOfSheetName.Contains(myWorksheet.Name))
                                {
                                    Excel.Range range;
                                    //Read the excel file
                                    range = myWorksheet.UsedRange;

                                    dicIntnrVsOECode = new Dictionary<String, String>();

                                    int iStartRow = 4;
                                    intValueLength = 20;
                                    totalcode = 0;
                                    totalcodehalf=0;

                                    string s_temp1 = myWorksheet.Name.ToString();
                                    for (int i = iStartRow; i <= range.Rows.Count; i++)
                                    {
                                        if (myWorksheet.Cells[i, 1].Value2 != null && myWorksheet.Cells[i, 3].Value2 != null && myWorksheet.Cells[i, 4].Value2 != null)
                                        {
                                            string temp = myWorksheet.Cells[i, 3].Value.ToString();
                                            if (temp.Length > 15)
                                                totalcode++;
                                            else
                                                totalcodehalf++;

                                        }

                                        lblProgress.Content = "Sheet Name : " + myWorksheet.Name.ToString();

                                        lblStatus.Content = "Progress : " + i.ToString();
                                        DoEvents();
                                    }
                                    totalcode = totalcode + totalcodehalf / 2;

                                    dicSheetNameVsCount.Add(myWorksheet.Name, totalcode.ToString());
                                }

                                
                            }
                            xlApp.Quit();
                            releaseObject(xlWorkBook);
                            releaseObject(xlApp);

                            //MessageBox.Show(totalcode.ToString());

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

            var xlNewSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
            xlNewSheet.Name = "Post-coding bill";

            xlNewSheet.Cells[1, 1] = "'Sheet Name";
            xlNewSheet.Cells[1, 2] = "'Verbatim Count";

            int Total = 0;
            int i = 1;
            foreach (KeyValuePair<string, string> pair in dicSheetNameVsCount)
            {
                xlNewSheet.Cells[i + 1, 1] = "'" + pair.Key;
                xlNewSheet.Cells[i + 1, 2] = pair.Value;

                Total = Total + Convert.ToInt32(pair.Value);

                i = i + 1;

                //xlNewSheet.get_Range("B:B").EntireColumn.Hidden = true;
            }

            xlNewSheet.Cells[i+1, 1] = "'Total";
            xlNewSheet.Cells[i+1, 2] = Total.ToString();

            xlNewSheet.Columns.AutoFit();
            xlWorkBook2.Worksheets["Sheet1"].Delete();

            //xlApp.Visible = true;


            string savefileName = txtExcelFileLocation.Text.Substring(txtExcelFileLocation.Text.LastIndexOf('\\') + 1);
            savefileName = savefileName.Split('.')[0];
            //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
            xlWorkBook2.SaveAs(myPath + "\\" + savefileName + "_Post_code_bill.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
            xlWorkBook2.Close(true, misValue, misValue);
            xlApp2.Quit();


            releaseObject(xlWorkSheet2);
            releaseObject(xlWorkBook2);
            releaseObject(xlApp2);

            MessageBox.Show("Total Bill : "+Total.ToString());

        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void FrmOEBill1_Loaded(object sender, RoutedEventArgs e)
        {
            listOfSheetName = new List<string>();
        }

    }
}
