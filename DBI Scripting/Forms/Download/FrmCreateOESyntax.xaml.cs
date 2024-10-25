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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmCreateOESyntax.xaml
    /// </summary>
    public partial class FrmCreateOESyntax : Window
    {

        private String myPath;
        private String sSelectedSheet;
        private int myCounter;

        private List<String> lstOfOESheetName;
        private TextWriter txtWriter;

        public FrmCreateOESyntax()
        {
            InitializeComponent();
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
                    txtDataFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtDataFileLocation.Text.Substring(0, txtDataFileLocation.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtDataFileLocation.Text = "";
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
                if (File.Exists(txtDataFileLocation.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtDataFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            if (txtDataFileLocation.Text != "")
            {
                if (File.Exists(txtDataFileLocation.Text))
                {
                    if (txtOESyntaxName.Text != "")
                    {
                        if (lstOfOESheetName.Count > 0)
                        {
                            this.createFileOESyntax(txtDataFileLocation.Text, txtOESyntaxName.Text);

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtDataFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                            int n = 0;
                            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                            {
                                if (lstOfOESheetName.Contains(myWorksheet.Name))
                                {
                                    lblProgress.Content = "Progress : " + (n + 1).ToString() + "/" + myCounter.ToString();
                                    lblStatus.Content = "   Status : " + myWorksheet.Name;
                                    DoEvents();
                                    n++;
                                    Excel.Range range;
                                    //Read the excel file
                                    range = myWorksheet.UsedRange;

                                    int iStartRow = 4;
                                    string s_temp1 = myWorksheet.Name.ToString() + "_OE";

                                    if (chkDontCreateVar.IsChecked == false)
                                        txtWriter.WriteLine("STRING " + s_temp1 + " (A100).");
                                    else
                                        txtWriter.WriteLine("");

                                    for (int i = iStartRow; i <= range.Rows.Count; i++)
                                    {
                                        string temp1 = myWorksheet.Cells[i, 1].Value;
                                        //string temp3 = myWorksheet.Cells[i, 4].Value.ToString();

                                        string temp3 = myWorksheet.Cells[i, 3].Value.ToString();

                                        //if (String.IsNullOrEmpty(myWorksheet.Cells[i, 1].Value) == false && String.IsNullOrEmpty(myWorksheet.Cells[i, 4].Value.ToString()) == false)
                                        if (String.IsNullOrEmpty(myWorksheet.Cells[i, 1].Value) == false && String.IsNullOrEmpty(myWorksheet.Cells[i, 3].Value.ToString()) == false)
                                        {
                                            txtWriter.WriteLine("IF RespondentId = '" + myWorksheet.Cells[i, 1].Value.ToString() + "' " + s_temp1 + "='" + myWorksheet.Cells[i, 3].Value.ToString() + "'.");

                                        }
                                    }
                                }
                            }
                            txtWriter.WriteLine("");
                            txtWriter.WriteLine("EXECUTE.");

                            txtWriter.Close();
                            MessageBox.Show("Write Complete");

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

        private void createFileOESyntax(string filePath, string fileName)
        {
            try
            {
                string createFilePath = filePath.Substring(0, filePath.LastIndexOf('\\'));
                txtWriter = new StreamWriter(createFilePath + "\\" + fileName + ".sps");
                txtWriter.WriteLine("*Excel File Name : " + fileName);
                txtWriter.WriteLine("*Operation Date  : " + DateTime.Now.ToShortDateString());
                txtWriter.WriteLine("*Operation Time  : " + DateTime.Now.ToShortTimeString());
                txtWriter.WriteLine("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }
}
