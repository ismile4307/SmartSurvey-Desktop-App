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
    /// Interaction logic for FrmUpdateSyntaxPrep.xaml
    /// </summary>
    public partial class FrmUpdateSyntaxPrep : Window
    {
        private string myPath;
        List<string> listOfSheetName;

        public FrmUpdateSyntaxPrep()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (txtExcelFileLocation.Text != "")
            {
                if (File.Exists(txtExcelFileLocation.Text))
                {
                    Dictionary<String, String> dicFileNameVsFilePath = new Dictionary<String, String>();
                    List<String> lstSheetName = new List<String>();
                    //dicVariableNameVsOEDataInfor = new Dictionary<String, OEDataInfo>();

                    List<String> listOfErrorMessage = new List<String>();

                    if (txtSaveFileName.Text != "")
                    {
                        if (listOfSheetName.Count > 0)
                        {

                            TextWriter txtWriter = new StreamWriter(myPath + "\\05." + txtSaveFileName.Text + ".sps");

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                            Dictionary<String, String> dicIntnrVsOECode;
                            int intValueLength;

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

                                    int iStartRow = 3;
                                    intValueLength = 20;


                                    List<int> listOfColNo = new List<int>();
                                    string columnText = txtColumnText.Text;
                                    String[] word = columnText.Split(',');
                                    for (int x = 0; x < word.Length; x++)
                                    {
                                        listOfColNo.Add(Convert.ToInt32(word[x]));
                                    }
                                    txtWriter.WriteLine("*" + myWorksheet.Name+".");
                                    txtWriter.WriteLine("");

                                    //********************************************txtWriter.WriteLine("STRING " + s_temp1 + " (A24).");

                                    for (int j = 0; j < listOfColNo.Count; j++)
                                    {
                                        int ColNo = listOfColNo[j];
                                        //string s_temp1 = myWorksheet.Name.ToString() + "_" + (j + 1).ToString();

                                        for (int i = iStartRow; i <= range.Rows.Count; i++)
                                        {
                                            if (myWorksheet.Cells[i, 1].Value2 != null && myWorksheet.Cells[i, ColNo].Value2 != null)
                                            {
                                                string s_temp1 = myWorksheet.Cells[i, 1].Value.ToString();
                                                string s_temp2 = myWorksheet.Cells[i, ColNo].Value.ToString();
                                                if (s_temp1 != "")
                                                {
                                                    string OEData = myWorksheet.Cells[i, ColNo].Value.ToString().Trim();


                                                    if (OEData != "")
                                                    {
                                                        if(OEData.Substring(OEData.Length-3,3)!="=0.")
                                                            txtWriter.WriteLine(OEData);
                                                    }
                                                        
                                                }
                                                else
                                                    break;
                                                
                                            }
                                            else if (myWorksheet.Cells[i, 1].Value2 != null && myWorksheet.Cells[i, ColNo].Value2 == null)
                                            {
                                                listOfErrorMessage.Add(("Sheet Name:" + myWorksheet.Name + " ").PadRight(50, ' ') + "Row No: " + i.ToString() + " is blank.");
                                            }

                                            lblProgress.Content = "Sheet Name : " + myWorksheet.Name.ToString();


                                            lblStatus.Content = "Progress : " + i.ToString();
                                            DoEvents();
                                        }

                                        txtWriter.WriteLine("");
                                    }
                                }

                                txtWriter.WriteLine("");

                            }
                            txtWriter.WriteLine("EXECUTE.");
                            txtWriter.Close();




                            xlApp.Quit();
                            releaseObject(xlWorkBook);
                            releaseObject(xlApp);


                            MessageBox.Show("Write Complete");

                            //if (listOfErrorMessage.Count > 0)
                            //{
                            //    this.Height = 575;
                            //    txtErrorMessage.Clear();
                            //    for (int i = 0; i < listOfErrorMessage.Count; i++)
                            //    {
                            //        txtErrorMessage.AppendText(listOfErrorMessage[i] + "\n");
                            //    }
                            //}


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
            //MessageBox.Show("");
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void frmUpdateSyntaxPrep_Loaded(object sender, RoutedEventArgs e)
        {
            listOfSheetName = new List<string>();
        }
    }
}
