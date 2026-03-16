using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for frmAddPanelData.xaml
    /// </summary>
    public partial class FrmAddPanelData : Window
    {
        String databasePath;
        String myPath, txtPath;
        List<String> listOfGridQid;
        String selectedSheetName;

        List<String> lstTextFile;

        List<String> lstOutletId;
        List<String> lstOutletName;
        List<String> lstPnlData3;
        List<String> lstPnlData4;
        List<String> lstPnlData5;
        List<String> lstPnlData6;
        List<String> lstPnlData7;
        List<String> lstPnlData8;
        List<String> lstPnlData9;
        List<String> lstPnlData10;
        List<String> lstPnlData11;
        List<String> lstPnlData12;
        List<String> lstPnlData13;
        List<String> lstPnlData14;
        List<String> lstPnlData15;
        List<String> lstPnlData16;
        List<String> lstPnlData17;
        List<String> lstPnlData18;
        List<String> lstPnlData19;
        List<String> lstPnlData20;


        public FrmAddPanelData()
        {
            InitializeComponent();
        }

        private void btnBrowseScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Script File (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtScriptPath.Text = openFileDialog1.FileName;
                    myPath = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));
                    //fileName = txtScriptPath.Text.Substring(txtScriptPath.Text.LastIndexOf('\\') + 1);


                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    this.getScriptVersion();

                }
                else
                    txtScriptPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getScriptVersion()
        {
            if (txtScriptPath.Text != "")
            {
                if (File.Exists(txtScriptPath.Text))
                {
                    ConnectionDB connDB = new ConnectionDB();
                    if (connDB.connect(txtScriptPath.Text) == true)
                    {
                        if (connDB.sqlite_conn.State == ConnectionState.Closed)
                            connDB.sqlite_conn.Open();

                        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_ProjectInfo", connDB.sqlite_conn);
                        DataSet ds = new DataSet();
                        dadpt.Fill(ds, "Table1");
                        if (ds.Tables["Table1"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables["Table1"].Rows)
                            {
                                txtScriptVersion.Text = dr["Version"].ToString();
                                //priorScriptVersion = dr["Version"].ToString();
                                txtProjectName.Text = dr["ProjectName"].ToString();
                                //projectId = dr["ProjectId"].ToString();
                            }
                        }

                        if (connDB.sqlite_conn.State == ConnectionState.Open)
                            connDB.sqlite_conn.Close();


                    }
                }
                else
                    MessageBox.Show("Invalid script file location");
            }
            else
                MessageBox.Show("Script location should not be blank");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnKillProcess_Click(object sender, RoutedEventArgs e)
        {
            this.quitProcess();
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

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
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
                    txtPlaceholderExcel.Text = openFileDialog1.FileName;
                    myPath = txtPlaceholderExcel.Text.Substring(0, txtPlaceholderExcel.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtPlaceholderExcel.Text = "";
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
                if (File.Exists(txtPlaceholderExcel.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtPlaceholderExcel.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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

        public void releaseObject(object obj)
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

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            //int myCounter = 0;
            selectedSheetName = "";
            //foreach (var item in chkListBoxWorksheet.Items)
            //{
            //    for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
            //    {
            //        if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
            //        {
            //            listOfSheetName.Add(item.ToString());
            //            myCounter++;
            //        }
            //    }

            //}

            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            selectedSheetName = chkListBoxWorksheet.SelectedItems[0].ToString();
        }

        private void btnInsertPanelData_Click(object sender, RoutedEventArgs e)
        {
            if (txtPlaceholderExcel.Text != "")
            {
                if (File.Exists(txtPlaceholderExcel.Text))
                {
                    if (txtScriptPath.Text != "")
                    {
                        if (selectedSheetName != "")
                        {
                            lstTextFile = new List<String>();
                            txtPath = "";
                            //TextWriter txtWriter = new StreamWriter(myPath + "\\05." + txtSaveFileName.Text + ".sps");

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtPlaceholderExcel.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                            //try
                            //{
                            //************************* This is for Single Variable ********************************************
                            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                            {
                                if (selectedSheetName == myWorksheet.Name)
                                {
                                    if (!Directory.Exists(myPath + "\\Temp"))
                                        Directory.CreateDirectory(myPath + "\\temp");
                                    else
                                    {
                                        if (File.Exists(myPath + "\\temp\\" + selectedSheetName + ".ism"))
                                            File.Delete(myPath + "\\temp\\" + selectedSheetName + ".ism");
                                    }




                                    Excel.Worksheet worksheet = (Excel.Worksheet)xlApp.Worksheets[selectedSheetName];

                                    worksheet.Select(true);

                                    xlWorkBook.SaveAs(myPath + "\\temp\\" + selectedSheetName + ".ism", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                                    lstTextFile.Add(myPath + "\\temp\\" + selectedSheetName + ".ism");
                                    txtPath = myPath + "\\temp\\" + selectedSheetName + ".ism";


                                    //Excel.Range range;
                                    ////Read the excel file
                                    //range = myWorksheet.UsedRange;

                                    //int iStartRow = 1;

                                    //for (int i = iStartRow; i <= range.Rows.Count; i++)
                                    //{
                                    //    if (myWorksheet.Cells[i, 1].Value2 != null && myWorksheet.Cells[i, 3].Value2 != null && myWorksheet.Cells[i, 4].Value2 != null)
                                    //    {
                                    //        string qid = myWorksheet.Cells[i, 1].Value.ToString();
                                    //        string qText = myWorksheet.Cells[i, 4].Value.ToString();



                                    //        //txtWriter.WriteLine("IF RespondentId ='" + myWorksheet.Cells[i, 1].Value.ToString() + "' " + s_temp1 + "=" + myWorksheet.Cells[i, ColNo].Value.ToString().Trim() + ".");
                                    //    }
                                    //}
                                }
                            }


                            //xlWorkBook.Close();

                            //xlApp.Quit();
                            //releaseObject(xlWorkBook);
                            //releaseObject(xlApp);

                            xlWorkBook.Close(false);
                            releaseObject(xlWorkBook);
                            releaseObject(xlApp);

                            //***************************************************************

                            this.load_AllData();

                            //***************************************************************


                            ConnectionDB connDB = new ConnectionDB();
                            if (connDB.connect(txtScriptPath.Text) == true)
                            {
                                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                                    connDB.sqlite_conn.Open();


                                SQLiteCommand command0 = new SQLiteCommand(connDB.sqlite_conn);
                                command0.CommandText = ("DELETE FROM T_PanelData;");
                                command0.ExecuteNonQuery();


                                progressBar1.Minimum = 1;
                                progressBar1.Maximum = lstOutletId.Count;
                                int p = 1;
                                for (int x = 0; x < lstOutletId.Count; x++)
                                {
                                    progressBar1.Value = p;
                                    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                                    command.CommandText = ("INSERT INTO T_PanelData (my_key,pdata1,pdata2,pdata3,pdata4,pdata5,pdata6,pdata7,pdata8,pdata9,pdata10,pdata11,pdata12,pdata13,pdata14,pdata15,pdata16,pdata17,pdata18,pdata19,pdata20) VALUES ('" + lstOutletId[x] + "','" + lstOutletName[x] + "','" + lstPnlData3[x] + "','" + lstPnlData4[x] + "','" + lstPnlData5[x] + "','" + lstPnlData6[x] + "','" + lstPnlData7[x] + "','" + lstPnlData8[x] + "','" + lstPnlData9[x] + "','" + lstPnlData10[x] + "','" + lstPnlData11[x] + "','" + lstPnlData12[x] + "','" + lstPnlData13[x] + "','" + lstPnlData14[x] + "','" + lstPnlData15[x] + "','" + lstPnlData16[x] + "','" + lstPnlData17[x] + "','" + lstPnlData18[x] + "','" + lstPnlData19[x] + "','" + lstPnlData20[x] + "','')");
                                    command.ExecuteNonQuery();
                                    p++;
                                    DoEvents();
                                }


                                if (connDB.sqlite_conn.State == ConnectionState.Open)
                                    connDB.sqlite_conn.Close();

                            }


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

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void load_AllData()
        {
            TextReader txtReader = new StreamReader(txtPath);
            String strline;
            strline = txtReader.ReadLine();         //Leave the first line of the text file
            strline = txtReader.ReadLine();

            lstOutletId = new List<string>();
            lstOutletName = new List<string>();

            lstPnlData3 = new List<string>();
            lstPnlData4 = new List<string>();
            lstPnlData5 = new List<string>();
            lstPnlData6 = new List<string>();
            lstPnlData7 = new List<string>();
            lstPnlData8 = new List<string>();
            lstPnlData9 = new List<string>();
            lstPnlData10 = new List<string>();
            lstPnlData11 = new List<string>();
            lstPnlData12 = new List<string>();
            lstPnlData13 = new List<string>();
            lstPnlData14 = new List<string>();
            lstPnlData15 = new List<string>();
            lstPnlData16 = new List<string>();
            lstPnlData17 = new List<string>();
            lstPnlData18 = new List<string>();
            lstPnlData19 = new List<string>();
            lstPnlData20 = new List<string>();

            while (strline != null)
            {
                string[] word = strline.Split('\t');

                lstOutletId.Add(word[0]);
                lstOutletName.Add(word[1].Replace("'", "''").Trim());

                if (word.Length > 2)
                    lstPnlData3.Add(word[2].Replace("'", "''").Trim());
                else
                    lstPnlData3.Add("");

                if (word.Length > 3)
                    lstPnlData4.Add(word[3].Replace("'", "''").Trim());
                else
                    lstPnlData4.Add("");
                if (word.Length > 4)
                    lstPnlData5.Add(word[4].Replace("'", "''").Trim());
                else
                    lstPnlData5.Add("");
                if (word.Length > 5)
                    lstPnlData6.Add(word[5].Replace("'", "''").Trim());
                else
                    lstPnlData6.Add("");
                if (word.Length > 6)
                    lstPnlData7.Add(word[6].Replace("'", "''").Trim());
                else
                    lstPnlData7.Add("");
                if (word.Length > 7)
                    lstPnlData8.Add(word[7].Replace("'", "''").Trim());
                else
                    lstPnlData8.Add("");
                if (word.Length > 8)
                    lstPnlData9.Add(word[8].Replace("'", "''").Trim());
                else
                    lstPnlData9.Add("");
                if (word.Length > 9)
                    lstPnlData10.Add(word[9].Replace("'", "''").Trim());
                else
                    lstPnlData10.Add("");
                if (word.Length > 10)
                    lstPnlData11.Add(word[10].Replace("'", "''").Trim());
                else
                    lstPnlData11.Add("");
                if (word.Length > 11)
                    lstPnlData12.Add(word[11].Replace("'", "''").Trim());
                else
                    lstPnlData12.Add("");
                if (word.Length > 12)
                    lstPnlData13.Add(word[12].Replace("'", "''").Trim());
                else
                    lstPnlData13.Add("");
                if (word.Length > 13)
                    lstPnlData14.Add(word[13].Replace("'", "''").Trim());
                else
                    lstPnlData14.Add("");
                if (word.Length > 14)
                    lstPnlData15.Add(word[14].Replace("'", "''").Trim());
                else
                    lstPnlData15.Add("");
                if (word.Length > 15)
                    lstPnlData16.Add(word[15].Replace("'", "''").Trim());
                else
                    lstPnlData16.Add("");
                if (word.Length > 16)
                    lstPnlData17.Add(word[16].Replace("'", "''").Trim());
                else
                    lstPnlData17.Add("");
                if (word.Length > 17)
                    lstPnlData18.Add(word[17].Replace("'", "''").Trim());
                else
                    lstPnlData18.Add("");
                if (word.Length > 18)
                    lstPnlData19.Add(word[18].Replace("'", "''").Trim());
                else
                    lstPnlData19.Add("");
                if (word.Length > 19)
                    lstPnlData20.Add(word[19].Replace("'", "''").Trim());
                else
                    lstPnlData20.Add("");

                strline = txtReader.ReadLine();
            }

            txtReader.Close();
        }


    }
}
