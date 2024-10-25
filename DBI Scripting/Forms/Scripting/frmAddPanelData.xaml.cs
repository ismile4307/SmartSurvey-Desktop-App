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
        List<String> lstRegion;
        List<String> lstArea;
        List<String> lstTerritory;
        List<String> lstOutletType;
        List<String> lstLDRed;
        List<String> lstLDFresh;


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
                            lstTextFile=new List<String>();
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

                            xlApp.Quit();
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
                                    command.CommandText = ("INSERT INTO T_PanelData (my_key,pdata1,pdata2,pdata3,pdata4,pdata5,pdata6,pdata7,pdata8,pdata9,pdata10) VALUES ('" + lstOutletId[x] + "','" + lstOutletName[x] + "','" + lstOutletType[x] + "','" + lstLDRed[x] + "','"+lstLDFresh[x]+"','','','','','','')");
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
            lstRegion = new List<string>();
            lstArea = new List<string>();
            lstTerritory = new List<string>();
            lstOutletType = new List<string>();
            lstLDRed = new List<string>();
            lstLDFresh = new List<string>();

            while (strline != null)
            {
                string[] word = strline.Split('\t');

                lstOutletId.Add(word[0]);
                lstOutletName.Add(word[1]);
                lstRegion.Add(word[2]);
                lstArea.Add(word[3]);
                lstTerritory.Add(word[4]);
                lstOutletType.Add(word[5]);
                lstLDRed.Add(word[6]);
                lstLDFresh.Add(word[7]);

                strline = txtReader.ReadLine();
            }

            txtReader.Close();
        }


    }
}
