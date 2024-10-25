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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmUpdateRLD.xaml
    /// </summary>
    public partial class FrmUpdateRLD : Window
    {
        private String myPath;
        private String sSelectedSoruceFieldName, sSelectedTargetFieldName;
        private String sSelectedSoruceSheetName, sSelectedTargetSheetName;
        private Dictionary<string, string> dicIdvsValue;

        public FrmUpdateRLD()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowseSourceFile_Click(object sender, RoutedEventArgs e)
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
                    txtSourceFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtSourceFileLocation.Text.Substring(0, txtSourceFileLocation.Text.LastIndexOf('\\'));
                    this.loadSourceWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtSourceFileLocation.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadSourceWorksheet()
        {
            try
            {
                if (File.Exists(txtSourceFileLocation.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtSourceFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                    chkListBoxSourceWorksheet.Items.Clear();
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        chkListBoxSourceWorksheet.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
                    }
                    xlWorkBook.Close();
                    xlApp.Quit();

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

        private void btnBrowseRLDFile_Click(object sender, RoutedEventArgs e)
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
                    txtRLDFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtRLDFileLocation.Text.Substring(0, txtRLDFileLocation.Text.LastIndexOf('\\'));
                    this.loadRLDWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtRLDFileLocation.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadRLDWorksheet()
        {
            try
            {
                if (File.Exists(txtRLDFileLocation.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtRLDFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                    chkListBoxTargetWorksheet.Items.Clear();
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        chkListBoxTargetWorksheet.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
                    }
                    xlWorkBook.Close();
                    xlApp.Quit();

                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    
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

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (sSelectedSoruceFieldName != "" && sSelectedTargetFieldName != "")
            {
                readSourceData();

                writeToTargetExcel();

                MessageBox.Show("Write Complete");

            }
            else
                MessageBox.Show("Source field or Target field missing");
        }

        private void readSourceData()
        {
            dicIdvsValue = new Dictionary<string, string>();

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtSourceFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
            {
                if (myWorksheet.Name == sSelectedSoruceSheetName)
                {
                    Excel.Range range;
                    //Read the excel file
                    range = myWorksheet.UsedRange;

                    int ColNo = getColumnNumber(myWorksheet, sSelectedSoruceFieldName);
                    if (ColNo != 0)
                    {
                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            //string temp1 = myWorksheet.Cells[i, ColNo].Value.ToString();

                            if (myWorksheet.Cells[i, ColNo].Value2 != null && myWorksheet.Cells[i, 1].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            {
                                string myKey = myWorksheet.Cells[i, 1].Value.ToString();
                                string myValue = myWorksheet.Cells[i, ColNo].Value.ToString();
                                if (myValue.Trim() != "" && myValue != "#NULL!" && myValue!="-2146826288")
                                    dicIdvsValue.Add(myKey, myValue);

                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("RespondentId Column not found");
                    //}
                }
            }
            xlWorkBook.Close();
            xlApp.Quit();

            releaseObject(xlWorkBook);
            releaseObject(xlApp);
        }

        private void writeToTargetExcel()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtRLDFileLocation.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
            {
                if (myWorksheet.Name == sSelectedTargetSheetName)
                {
                    Excel.Range range;
                    //Read the excel file
                    range = myWorksheet.UsedRange;

                    int ColNo = getColumnNumber(myWorksheet, sSelectedTargetFieldName);
                    if (ColNo != 0)
                    {
                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            //string temp1 = myWorksheet.Cells[i, ColNo].Value.ToString();

                            if (myWorksheet.Cells[i, 1].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            {
                                string myKey = myWorksheet.Cells[i, 1].Value.ToString();
                                if (dicIdvsValue.ContainsKey(myKey))
                                {
                                    myWorksheet.Cells[i, ColNo] = dicIdvsValue[myKey].ToString();
                                }
                                else
                                    myWorksheet.Cells[i, ColNo] = "";
                                

                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("RespondentId Column not found");
                    //}
                }
            }
            xlWorkBook.Save();
            xlWorkBook.Close();
            xlApp.Quit();

            releaseObject(xlWorkBook);
            releaseObject(xlApp);
            
        }

        private int getColumnNumber(Excel.Worksheet ws,string fieldName)
        {

            Excel.Range range;
            //Read the excel file
            range = ws.UsedRange;
            for (int i = 1; i <= range.Columns.Count; i++)
            {
                if (ws.Cells[1, i].Value2 != null)
                {
                    if (ws.Cells[1, i].Value.ToString() == fieldName)
                        return i;
                }
            }

            return 0;
        }

        private void chkListBoxSourceWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxSourceWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxSourceWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxSourceWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSoruceSheetName = chkListBoxSourceWorksheet.SelectedItems[0].ToString();

            this.loadSourceFieldName(sSelectedSoruceSheetName);
        }

        private void loadSourceFieldName(string sSelectedSheet)
        {
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtSourceFileLocation.Text))
            {
                chkListBoxSourceFieldName.Items.Clear();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtSourceFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        int rowNo = 1;
                        //int colNo = getOECodeColumnNumber(myWorksheet);
                        //if (ColNo != 0)
                        //{
                        for (int i = 1; i <= range.Columns.Count; i++)
                        {
                            string temp1 = myWorksheet.Cells[rowNo, i].Value.ToString();

                            if (myWorksheet.Cells[rowNo, i].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            {
                                chkListBoxSourceFieldName.Items.Add(temp1);

                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("RespondentId Column not found");
                    //}
                }
            }

        }

        private void chkListBoxTargetWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxTargetWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxTargetWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxTargetWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedTargetSheetName = chkListBoxTargetWorksheet.SelectedItems[0].ToString();

            this.loadTargetFieldName(sSelectedTargetSheetName);
        }

        private void loadTargetFieldName(string sSelectedSheet)
        {
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtRLDFileLocation.Text))
            {
                chkListBoxTargetFieldName.Items.Clear();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtRLDFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        int rowNo = 1;
                        //int colNo = getOECodeColumnNumber(myWorksheet);
                        //if (ColNo != 0)
                        //{
                        for (int i = 1; i <= range.Columns.Count; i++)
                        {
                            string temp1 = myWorksheet.Cells[rowNo, i].Value.ToString();

                            if (myWorksheet.Cells[rowNo, i].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            {
                                chkListBoxTargetFieldName.Items.Add(temp1);

                            }
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("RespondentId Column not found");
                    //}
                }
            }

        }

        private void chkListBoxSourceFieldName_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxSourceFieldName.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxSourceFieldName.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxSourceFieldName.SelectedItems.Remove(selecteditem);
            }

            sSelectedSoruceFieldName = chkListBoxSourceFieldName.SelectedItems[0].ToString();
        }

        private void chkListBoxTargetFieldName_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxTargetFieldName.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxTargetFieldName.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxTargetFieldName.SelectedItems.Remove(selecteditem);
            }

            sSelectedTargetFieldName = chkListBoxTargetFieldName.SelectedItems[0].ToString();
        }

        private void btnKillProcess_Click(object sender, RoutedEventArgs e)
        {
            quitProcess();
            MessageBox.Show("Process killed");
        }

        private void quitProcess()
        {
            //try
            //{
            Process[] proc = Process.GetProcessesByName("EXCEL");
            foreach (Process myProcess in proc)
            {
                myProcess.Kill();
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

    }
}
