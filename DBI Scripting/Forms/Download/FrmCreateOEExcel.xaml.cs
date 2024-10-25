using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
    /// Interaction logic for FrmCreateOEExcel.xaml
    /// </summary>
    public partial class FrmCreateOEExcel : Window
    {
        Dictionary<string, string> dicProjectNameVsDatabaseName;
        private string myPath;
        List<string> lstWorkSheetName;
        private Dictionary<String, List<OEVerbatim>> dicQidVsOEVerbatim;
        private List<String> lstQIdforOE;

        String baseDirectory = @"C:\Temp\";



        private String sSelectedSheet;
        private int myCounter;

        private List<String> lstOfOESheetName;
        private TextWriter txtWriter;


        public FrmCreateOEExcel()
        {
            InitializeComponent();
        }

        private async void getProjectsFromServer()
        {
            try
            {
                await DoWorkAsync();

                dicProjectNameVsDatabaseName = new Dictionary<string, string>();

                DownloadClass myDownloadClass = new DownloadClass();

                List<ProjectInfo> listOfProjectInfo = new List<ProjectInfo>();

                listOfProjectInfo = myDownloadClass.getProjectInfoFromServer();

                comProjectName.Items.Clear();
                for (int i = 0; i < listOfProjectInfo.Count; i++)
                {
                    string projectName = listOfProjectInfo[i].ProjectName;
                    comProjectName.Items.Add(projectName);

                    dicProjectNameVsDatabaseName.Add(projectName, listOfProjectInfo[i].DatabaseName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                //do some work HERE
                Thread.Sleep(1000);
            });
        }

        private void frmCreateOEExcel_Loaded(object sender, RoutedEventArgs e)
        {
            this.getProjectsFromServer();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtExcelDataPath.Text = openFileDialog1.FileName;
                    myPath = txtExcelDataPath.Text.Substring(0, txtExcelDataPath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    //this.getScriptVersion();

                }
                else
                    txtExcelDataPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            List<string> listOEQuestoin = new List<string>();
            listOEQuestoin = this.getOEQuestionList();
            lstWorkSheetName = new List<string>();
            dicQidVsOEVerbatim = new Dictionary<string, List<OEVerbatim>>();
            lstQIdforOE = new List<string>();

            if (txtExcelDataPath.Text != "")
            {
                if (File.Exists(txtExcelDataPath.Text) == true)
                {
                    List<String> lstTextFile = new List<string>();


                    lstWorkSheetName.Clear();
                    lstWorkSheetName.Add("Openeneded");

                    //if (lstWorkSheetName.Count > 0)
                    //{
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelDataPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    lstTextFile.Clear();            //Clear the txt file path list
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        if (lstWorkSheetName.Contains(xlWorkBook.Worksheets[i].Name.ToString()))
                        {
                            string sheetName = xlWorkBook.Worksheets[i].Name.ToString();
                            if (File.Exists(baseDirectory + sheetName + ".txt"))
                                File.Delete(baseDirectory + sheetName + ".txt");

                            Excel.Worksheet worksheet = (Excel.Worksheet)xlApp.Worksheets[sheetName];

                            worksheet.Select(true);

                            xlWorkBook.SaveAs(baseDirectory + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                            lstTextFile.Add(baseDirectory + sheetName + ".txt");
                        }
                    }
                    xlWorkBook.Close(true);
                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);

                    dicQidVsOEVerbatim.Clear();
                    //********************************************************
                    //this.createList();
                    //********************************************************
                    for (int i = 0; i < lstTextFile.Count; i++)
                    {
                        //Dim file_Path As String = GetFoxproDBDir(txt_FoxDB_Location.Text) + "\FoxproDB\" + Trim(ews.Name.ToString()) + ".DBF"
                        string s_temp, strline;
                        TextReader txtReader = new StreamReader(lstTextFile[i]);
                        //int lenReader = File.ReadAllLines(lstTextFile[i]).Length;
                        //s_temp = lstTextFile[i].ToString();
                        //s_temp = s_temp.Substring(s_temp.LastIndexOf('\\'));
                        //lblWorkOn.Text = s_temp.Substring(1, s_temp.LastIndexOf('.') - 1);
                        //lblTotalRecord.Text = lenReader.ToString();
                        //lblComplete.Text = (i).ToString() + "/" + lstTextFile.Count.ToString();
                        //Application.DoEvents();

                        strline = txtReader.ReadLine();     //Read the Headding
                        string[] heading = strline.Split('\t');

                        //string file_Path = txt_FoxDB_Location.Text;
                        //string DatabaseName = file_Path.Substring(file_Path.LastIndexOf('\\') + 1);
                        Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();


                        //bool startToTakeBrandCode = false;
                        strline = txtReader.ReadLine();     //Read the 2nd Line
                        while (strline != null)
                        {
                            //progressBar1.Value = p;
                            string[] word = strline.Split('\t');

                            this.populateOEDictionary(word);

                            strline = txtReader.ReadLine();
                            //p = p + 1;


                        }

                        //}
                        txtReader.Close();
                    }


                    this.PopulateQIdForOE();

                    //****************************************

                    if (chkOnlyTakeOE.IsChecked == false)
                    {
                        lstWorkSheetName.Clear();
                        lstWorkSheetName.Add("Data");


                        xlApp = new Excel.Application();
                        xlWorkBook = xlApp.Workbooks.Open(txtExcelDataPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        lstTextFile.Clear();            //Clear the txt file path list
                        for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                        {
                            if (lstWorkSheetName.Contains(xlWorkBook.Worksheets[i].Name.ToString()))
                            {
                                string sheetName = xlWorkBook.Worksheets[i].Name.ToString();
                                if (File.Exists(baseDirectory + sheetName + ".txt"))
                                    File.Delete(baseDirectory + sheetName + ".txt");

                                Excel.Worksheet worksheet = (Excel.Worksheet)xlApp.Worksheets[xlWorkBook.Worksheets[i].Name.ToString()];

                                worksheet.Select(true);

                                xlWorkBook.SaveAs(baseDirectory + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                                lstTextFile.Add(baseDirectory + sheetName + ".txt");
                            }
                        }
                        xlWorkBook.Close(true);
                        releaseObject(xlWorkBook);
                        releaseObject(xlApp);
                        //****************************************


                        TextReader txtReader2 = new StreamReader(lstTextFile[0]);
                        //int lenReader = File.ReadAllLines(lstTextFile[i]).Length;
                        //s_temp = lstTextFile[i].ToString();
                        //s_temp = s_temp.Substring(s_temp.LastIndexOf('\\'));
                        //lblWorkOn.Text = s_temp.Substring(1, s_temp.LastIndexOf('.') - 1);
                        //lblTotalRecord.Text = lenReader.ToString();
                        //lblComplete.Text = (i).ToString() + "/" + lstTextFile.Count.ToString();
                        //Application.DoEvents();

                        string strline2 = txtReader2.ReadLine();     //Read the Headding
                        string[] heading2 = strline2.Split('\t');

                        //string file_Path = txt_FoxDB_Location.Text;
                        //string DatabaseName = file_Path.Substring(file_Path.LastIndexOf('\\') + 1);
                        Dictionary<string, string> dicFieldValue2 = new Dictionary<string, string>();


                        //bool startToTakeBrandCode = false;
                        strline2 = txtReader2.ReadLine();     //Read the 2nd Line
                        while (strline2 != null)
                        {
                            //progressBar1.Value = p;
                            string[] word = strline2.Split('\t');

                            dicFieldValue2.Clear();
                            for (int j = 0; j < heading2.Length; j++)
                            {
                                if (!dicFieldValue2.ContainsKey(heading2[j]))
                                    dicFieldValue2.Add(heading2[j], word[j]);
                            }

                            this.populateOEDictionaryFromData(dicFieldValue2);

                            strline2 = txtReader2.ReadLine();
                            //p = p + 1;


                        }

                        //}
                        txtReader2.Close();
                    }

                    //****************************************

                    this.exportToExcel();

                    //************************************** CreateDirectory *********************************************************

                    //***********************************************************************************************
                    if (dicQidVsOEVerbatim.Count > 0)
                        MessageBox.Show("Write Complete");
                    else
                        MessageBox.Show("No OE verbatims found to create OE sheet");

                }
                else
                    MessageBox.Show("Select Excel Sheet");

            }
        }

        private void populateOEDictionaryFromData(Dictionary<string, string> dicfieldValue)
        {

            for (int i = 0; i < lstQIdforOE.Count; i++)
            {
                if (dicfieldValue.ContainsKey(lstQIdforOE[i]))
                {
                    if (dicfieldValue[lstQIdforOE[i]] != "")
                    {
                        if (dicQidVsOEVerbatim.ContainsKey(lstQIdforOE[i]))
                        {
                            List<OEVerbatim> listOfOEVerbatim = dicQidVsOEVerbatim[lstQIdforOE[i]];
                            OEVerbatim oeVerbatimObj = new OEVerbatim(dicfieldValue["RespondentId"], "", dicfieldValue[lstQIdforOE[i]]);
                            listOfOEVerbatim.Add(oeVerbatimObj);
                            dicQidVsOEVerbatim.Remove(lstQIdforOE[i]);
                            dicQidVsOEVerbatim.Add(lstQIdforOE[i], listOfOEVerbatim);

                        }
                        else
                        {
                            List<OEVerbatim> listOfOEVerbatim = new List<OEVerbatim>();
                            OEVerbatim oeVerbatimObj = new OEVerbatim(dicfieldValue["RespondentId"], "", dicfieldValue[lstQIdforOE[i]]);
                            listOfOEVerbatim.Add(oeVerbatimObj);
                            dicQidVsOEVerbatim.Add(lstQIdforOE[i], listOfOEVerbatim);
                        }
                    }
                }
            }

        }
        private void populateOEDictionary(string[] word)
        {

            if (dicQidVsOEVerbatim.ContainsKey(word[1] + "_" + word[2]))
            {
                List<OEVerbatim> listOfOEVerbatim = dicQidVsOEVerbatim[word[1] + "_" + word[2]];
                OEVerbatim oeVerbatimObj = new OEVerbatim(word[0], word[2], word[3]);
                listOfOEVerbatim.Add(oeVerbatimObj);
                dicQidVsOEVerbatim.Remove(word[1] + "_" + word[2]);
                dicQidVsOEVerbatim.Add(word[1] + "_" + word[2], listOfOEVerbatim);

            }
            else
            {
                List<OEVerbatim> listOfOEVerbatim = new List<OEVerbatim>();
                OEVerbatim oeVerbatimObj = new OEVerbatim(word[0], word[2], word[3]);
                listOfOEVerbatim.Add(oeVerbatimObj);
                dicQidVsOEVerbatim.Add(word[1] + "_" + word[2], listOfOEVerbatim);
            }
        }
        private void PopulateQIdForOE()
        {

            //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory  + dicProjectNameVsDatabaseName[comProjectName.Text];

            string databasePath = baseDirectory + dicProjectNameVsDatabaseName[comProjectName.Text];

            if (File.Exists(databasePath) == false)
                return;

            SQLite sql = new SQLite(databasePath);
            sql.connect();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId FROM T_Question WHERE QType='3' AND DisplayJumpButton!='1' AND DisplayJumpButton!='2' AND DisplayJumpButton!='3' Order by T_Question.OrderTag", sql.Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            lstQIdforOE.Clear();
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    lstQIdforOE.Add(dr[0].ToString());
                }
            }



            SQLiteDataAdapter dadpt2 = new SQLiteDataAdapter("SELECT T_OptAttribute.QId, T_OptAttribute.AttributeOrder FROM T_OptAttribute INNER JOIN T_Question ON T_OptAttribute.QId=T_Question.QId WHERE (T_Question.QType='12' OR T_Question.QType='18' OR T_Question.QType='48') AND T_OptAttribute.IsExclusive='' Order by T_Question.OrderTag", sql.Qconnection);
            DataSet ds2 = new DataSet();
            dadpt2.Fill(ds2, "Table2");

            if (ds2.Tables["Table2"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds2.Tables["Table2"].Rows)
                {
                    lstQIdforOE.Add(dr[0].ToString() + "_" + dr[1].ToString());
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

            int i = 1;
            foreach (KeyValuePair<string, List<OEVerbatim>> pair in dicQidVsOEVerbatim.OrderByDescending(x=>x.Key))
            {
                var xlNewSheet = (Excel.Worksheet)worksheets.Add(worksheets[1], Type.Missing, Type.Missing, Type.Missing);
                xlNewSheet.Name = pair.Key;

                xlNewSheet.Cells[1, 1] = "'" + pair.Key;

                xlNewSheet.Cells[3, 1] = "'Respondent Id";
                xlNewSheet.Cells[3, 2] = "'Attribute Value";
                xlNewSheet.Cells[3, 3] = "'Rsponse";
                xlNewSheet.Cells[3, 4] = "'Code";

                for (int j = 1; j <= pair.Value.Count; j++)
                {
                    xlNewSheet.Cells[j + 3, 1] = "'" + pair.Value[j - 1].RespondentId;
                    xlNewSheet.Cells[j + 3, 2] = "'" + pair.Value[j - 1].AttributeValue;
                    xlNewSheet.Cells[j + 3, 3] = "'" + pair.Value[j - 1].Response;
                }
                xlNewSheet.Columns.AutoFit();
                i = i + 1;

                xlNewSheet.get_Range("B:B").EntireColumn.Hidden = true;
            }

            if (dicQidVsOEVerbatim.Count > 0)
                xlWorkBook2.Worksheets["Sheet1"].Delete();

            //xlApp.Visible = true;

            //Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[getSheetIndex(sheetName, xlWorkBook2.Sheets)];
            Excel.Worksheet oSheet = (Excel.Worksheet)xlWorkBook2.Sheets[1];

            oSheet.Sort.Apply();

            if (dicQidVsOEVerbatim.Count > 0)
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

        // Method that returns the index of the specified worksheet name
        private int getSheetIndex(string sheetName, Excel.Sheets shs)
        {
            int i = 0;
            foreach (Excel.Worksheet sh in shs)
            {
                if (sheetName == sh.Name)
                {
                    return i + 1;
                }
                i += 1;
            }
            return 0;
        }

        private List<string> getOEQuestionList()
        {
            String databasePath = baseDirectory + dicProjectNameVsDatabaseName[comProjectName.Text];
            if (File.Exists(databasePath) == false)
                return null;

            SQLite sql = new SQLite(databasePath);
            sql.connect();

            List<string> listOEQuestoin = new List<string>();

            listOEQuestoin.Clear();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.QId FROM T_Question WHERE T_Question.QType = '3' AND DisplayJumpButton!='1' AND DisplayJumpButton!='2' AND DisplayJumpButton!='3' Order by T_Question.OrderTag", sql.Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    listOEQuestoin.Add(dr["Qid"].ToString());
                }
            }

            return listOEQuestoin;
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
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
                    txtDataFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtDataFileLocation.Text.Substring(0, txtDataFileLocation.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    chkSelectAll.IsChecked = false;
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    class OEVerbatim
    {
        public string RespondentId;
        public string AttributeValue;
        public string Response;

        public OEVerbatim(string _RespondentId, string _AttributeValue, string _Response)
        {
            RespondentId = _RespondentId;
            AttributeValue = _AttributeValue;
            Response = _Response;
        }
    }
}
