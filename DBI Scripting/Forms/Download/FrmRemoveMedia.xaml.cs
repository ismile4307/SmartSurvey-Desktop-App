using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
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
    /// Interaction logic for FrmRemoveMedia.xaml
    /// </summary>
    public partial class FrmRemoveMedia : Window
    {
        private String serverAddress;
        private String databasePath;
        private String myPath;

        private String sSelectedSheet;


        DataTable dt1;



        private Dictionary<string, string> dicProjectNameVsCode;
        private Dictionary<string, string> dicProjectNameVsDBName;

        private string QdbConnString;
        private String serverPath = "";

        public SQLiteConnection Qconnection;

        private List<String> listOfImages;
        private List<String> listOfRecordings;

        public FrmRemoveMedia()
        {
            InitializeComponent();
        }

        private void FrmRemoveMedia1_Loaded(object sender, RoutedEventArgs e)
        {
            //txtServerAddress.Text = Properties.Settings.Default.ServerAddress;
            serverAddress = StaticClass.SERVER_URL + "/deskapi/";


            serverPath = StaticClass.SERVER_URL;// Properties.Settings.Default.ServerAddress;

            this.getProjectsFromServer();
        }

        private async void getProjectsFromServer()
        {
            await DoWorkAsync();
            try
            {
                dicProjectNameVsCode = new Dictionary<string, string>();
                dicProjectNameVsDBName = new Dictionary<string, string>();

                DownloadClass myDownloadClass = new DownloadClass();

                List<ProjectInfo> listOfProjectInfo = new List<ProjectInfo>();

                listOfProjectInfo = myDownloadClass.getProjectInfoFromServer();

                comProjectNameDownload.Items.Clear();
                comProjectNameRemove.Items.Clear();
                for (int i = 0; i < listOfProjectInfo.Count; i++)
                {
                    string projectName = listOfProjectInfo[i].ProjectName;
                    comProjectNameDownload.Items.Add(projectName);
                    comProjectNameRemove.Items.Add(projectName);

                    dicProjectNameVsCode.Add(projectName, listOfProjectInfo[i].ProjectCode);
                    dicProjectNameVsDBName.Add(projectName, listOfProjectInfo[i].DatabaseName);
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Server connection failed");
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowseDownload_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel 2007|*.xlsx|All Files|*.*";
            saveFileDialog1.Title = "Save Data File";
            //saveFileDialog1.ShowDialog();

            if (saveFileDialog1.ShowDialog() == true)
            {
                string s_temp = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf('.'));
                string fileFormat = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf('.'));
                txtSaveLocation.Text = s_temp + fileFormat;
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (setData())
                {
                    String baseDirectory = @"C:\Temp\";

                    if (!Directory.Exists(@"C:\Temp"))
                    {
                        MessageBox.Show("Temp Derecory not exist in C drive. Pleaes create it first...");
                        return;
                    }
                    //databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                    databasePath = baseDirectory + dicProjectNameVsDBName[comProjectNameDownload.Text];

                    if (File.Exists(databasePath) == true)
                        File.Delete(databasePath);

                    if (File.Exists(databasePath) == false)
                    {
                        try
                        {

                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                            using (WebClient client = new WebClient())
                            {
                                //string source = Properties.Settings.Default.ServerAddress + "/" + dicProjectNameVsDBName[comProjectName.Text];
                                string source = StaticClass.SERVER_URL + "/scripts/" + dicProjectNameVsDBName[comProjectNameDownload.Text];
                                //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                                //string destination = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                                string destination = baseDirectory + dicProjectNameVsDBName[comProjectNameDownload.Text];
                                //if (!File.Exists(destination))
                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                client.DownloadFile(source, destination);
                            }
                        }
                        catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                    }



                    dt1 = new DataTable();

                    this.populateTInterviewInfo();

                    this.exportToExcel();


                    MessageBox.Show("Data download completed");


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void populateTInterviewInfo()
        {
            try
            {
                //lblExecute.Content = "Execute Now : " + "Download Data";
                //DoEvents();

                WebClient c = new WebClient();
                MyWebRequest myRequest1;
                //if (chkDeletedRec.Checked == false)
                //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/respondent.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
                myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/respondentremove.php", "POST", "projectCode=" + dicProjectNameVsCode[comProjectNameDownload.Text]);
                //else
                //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);

                //Console.WriteLine(data);
                //JObject o = JObject.Parse(data);
                string data = myRequest1.GetResponse().ToString();

                DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                if (dt1_temp.Rows.Count > 0)
                    dt1.Merge(dt1_temp);

                //if (!dicDateVsTInterviewInfo.ContainsKey(startDate))
                //    dicDateVsTInterviewInfo.Add(startDate, dt1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportToExcel()
        {
            //string TypeOfReport;
            try
            {

                databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectNameDownload.Text];
                //databasePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + dicProjectNameVsDBName[comProjectName.Text];

                if (File.Exists(databasePath) == false)
                {
                    MessageBox.Show("Script file not found..");
                    return;
                }


                SQLite sql = new SQLite(databasePath);
                sql.connect();



                //lblOperationNo.Content = "Operation No : 3/3";
                //lblExecute.Content = "Execute Now : Populate Excel";
                ////Application.DoEvents();




                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Name = "RespInfo";


                xlWorkSheet.Cells[1, 1] = "Id";
                xlWorkSheet.Cells[1, 2] = "Respondent Id";
                xlWorkSheet.Cells[1, 3] = "Project_id";
                xlWorkSheet.Cells[1, 4] = "Survey_start_at";
                xlWorkSheet.Cells[1, 5] = "Survey_end_at";

                int row = 2;
                DataTableReader drd = dt1.CreateDataReader();// sql.getDataTableOpenended();

                while (drd.Read())
                {
                    xlWorkSheet.Cells[row, 1] = "'" + drd["id"].ToString();
                    xlWorkSheet.Cells[row, 2] = "'" + drd["respondent_id"].ToString();
                    xlWorkSheet.Cells[row, 3] = "'" + drd["project_id"].ToString();
                    xlWorkSheet.Cells[row, 4] = "'" + drd["Survey_start_at"].ToString();
                    xlWorkSheet.Cells[row, 5] = "'" + drd["Survey_end_at"].ToString();
                    row = row + 1;
                }

                xlWorkSheet.Columns.AutoFit();










                ////******************* Get the Openended *****************************************************

                //Microsoft.Office.Interop.Excel.Sheets worksheets = xlWorkBook.Worksheets;
                //var xlNewSheet = (Microsoft.Office.Interop.Excel.Worksheet)worksheets.Add(worksheets[1]);
                //xlNewSheet.Name = "Data";


                //List<string> columnName = new List<string>();
                //List<List<string>> tableData = new List<List<string>>();

                //columnName = sql.getTableColumnReport();
                //tableData = sql.getTableDataReport(columnName, dt1, dt2, dt3, progressBar1);

                //for (int i = 1; i <= columnName.Count; i++)
                //{
                //    xlNewSheet.Cells[1, i] = "'" + columnName[i - 1];
                //}



                //int p = 1;
                //progressBar1.Minimum = 0;
                //progressBar1.Maximum = tableData.Count * tableData[0].Count;

                //// Get dimensions of the 2-d array
                //int rowCount = tableData.Count;// arrays.GetLength(0);
                //int columnCount = tableData[0].Count;// arrays.GetLength(0);

                //string[,] arrays = new string[rowCount, columnCount];//tableData.Select(a => a.ToArray()).ToArray();


                //for (int i = 1; i <= tableData.Count; i++)
                //{
                //    for (int j = 1; j <= tableData[i - 1].Count; j++)
                //    {
                //        progressBar1.Value = p;
                //        p++;

                //        arrays[i - 1, j - 1] = "'" + ReplaceNewlines(tableData[i - 1].ToList()[j - 1], " ");

                //        //xlNewSheet.Cells[i + 1, j] = "'" + ReplaceNewlines(tableData[i - 1].ToList()[j - 1], " ");
                //    }
                //}



                //// Get an Excel Range of the same dimensions
                //Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)xlNewSheet.Cells[2, 1];
                //range = range.get_Resize(rowCount, columnCount);
                //// Assign the 2-d array to the Excel Range
                //range.set_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault, arrays);


                //xlNewSheet.Columns.AutoFit();

                ////xlApp.Visible = true;




                //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                //xlWorkBook.SaveAs("D:\\Ismile Personal\\New folder (2)\\Analysis\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.SaveAs(txtSaveLocation.Text, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();


                sql.releaseObject(xlWorkSheet);
                sql.releaseObject(xlWorkBook);
                sql.releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Data found for given date range");
            }
        }

        private bool setData()
        {
            if (checkData())
            {
                return true;
            }
            return false;
        }

        private bool setDataRemove()
        {
            if (checkDataRemove())
            {
                return true;
            }
            return false;
        }


        private bool checkData()
        {
            if (comProjectNameDownload.Text == "")
            {
                MessageBox.Show("Project Name should be slected");
                return false;
            }
            if (txtSaveLocation.Text == "")
            {
                MessageBox.Show("Please select the save location to save the data");
                return false;
            }

            return true;
        }

        private bool checkDataRemove()
        {
            if (comProjectNameRemove.Text == "")
            {
                MessageBox.Show("Project Name should be slected");
                return false;
            }
            if (txtExcelFilePath.Text == "")
            {
                MessageBox.Show("Please select the save location to save the data");
                return false;
            }

            return true;
        }


        private void btnBrowseRemove_Click(object sender, RoutedEventArgs e)
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
                    txtExcelFilePath.Text = openFileDialog1.FileName;
                    myPath = txtExcelFilePath.Text.Substring(0, txtExcelFilePath.Text.LastIndexOf('\\'));
                    //if (comMediaType.Text == "Image")
                    //    txtSaveLocation.Text = myPath + "\\Images";
                    //else
                    //    txtSaveLocation.Text = myPath + "\\Recordings";

                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtExcelFilePath.Text = "";
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
                if (File.Exists(txtExcelFilePath.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelFilePath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


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

        private void prepareImageAndRecordingList()
        {
            listOfImages = new List<string>();
            listOfRecordings = new List<string>();

            this.connect();

            if (Qconnection.State == ConnectionState.Closed)
                Qconnection.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.QId, T_Question.QType, T_Question.SilentRecording FROM T_Question Order by T_Question.OrderTag", Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    string Qid = dr["QId"].ToString();
                    string recordingName = dr["SilentRecording"].ToString();

                    if (dr["QType"].ToString() == "16")
                        if (!listOfImages.Contains(Qid))
                            listOfImages.Add(Qid);

                    if (recordingName != "")
                        if (!listOfRecordings.Contains(recordingName))
                            listOfRecordings.Add(recordingName);

                    if (dr["QType"].ToString() == "10")
                        if (!listOfRecordings.Contains(Qid))
                            listOfRecordings.Add(Qid);

                }
            }

            if (Qconnection.State == ConnectionState.Open)
                Qconnection.Close();
        }

        public void connect()
        {
            //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
            string databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectNameRemove.Text];
            this.QdbConnString = @"Data Source=" + databasePath + "; Version=3;";
            this.Qconnection = new SQLiteConnection(this.QdbConnString);

        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (setDataRemove())
            {
                //if (!Directory.Exists(txtExcelFilePath.Text))
                //    Directory.CreateDirectory(txtExcelFilePath.Text);

                //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                string databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectNameRemove.Text];
                if (File.Exists(databasePath) == false)
                {
                    try
                    {
                        lblDownloadStatus.Content = "Now Remove : ";
                        DoEvents();

                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                        using (WebClient client = new WebClient())
                        {
                            string source = StaticClass.SERVER_URL + "/scripts/" + dicProjectNameVsDBName[comProjectNameRemove.Text];
                            //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                            //string destination = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                            string destination = @"C:\Temp\" + dicProjectNameVsDBName[comProjectNameRemove.Text];

                            if (!File.Exists(destination))
                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                client.DownloadFile(source, destination);
                        }
                    }
                    catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                }


                //if (Directory.Exists(txtExcelFilePath.Text))
                //{

                if (File.Exists(txtExcelFilePath.Text) == true)
                {
                    if (sSelectedSheet != "")
                    {
                        this.prepareImageAndRecordingList();

                        List<String> lstTextFile = new List<string>();

                        List<String> lstRespondentId = new List<String>();


                        //if (lstWorkSheetName.Count > 0)
                        //{
                        Excel.Application xlApp = new Excel.Application();
                        Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtExcelFilePath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        lstTextFile.Clear();            //Clear the txt file path list
                        for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                        {
                            if (xlWorkBook.Worksheets[i].Name.ToString() == sSelectedSheet)
                            {
                                string sheetName = xlWorkBook.Worksheets[i].Name.ToString();
                                if (File.Exists("C:\\Temp\\" + sheetName + ".txt"))
                                    File.Delete("C:\\Temp\\" + sheetName + ".txt");

                                Excel.Worksheet worksheet = (Excel.Worksheet)xlApp.Worksheets[sheetName];

                                worksheet.Select(true);

                                xlWorkBook.SaveAs("C:\\Temp\\" + sheetName + ".txt", Excel.XlFileFormat.xlTextWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                                lstTextFile.Add("C:\\Temp\\" + sheetName + ".txt");
                            }
                        }
                        xlWorkBook.Close(true);
                        releaseObject(xlWorkBook);
                        releaseObject(xlApp);

                        lstRespondentId.Clear();
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
                            //Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();


                            //bool startToTakeBrandCode = false;
                            strline = txtReader.ReadLine();     //Read the 2nd Line
                            while (strline != null)
                            {
                                //progressBar1.Value = p;

                                //this.populateOEDictionary(word);

                                //while (strline.Split('\t').Length < 380)
                                //{
                                //    strline = strline + txtReader.ReadLine();
                                //}

                                string[] word = strline.Split('\t');


                                //Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();

                                //for (int j = 0; j < heading.Length; j++)
                                //{
                                //    if (!dicFieldValue.ContainsKey(heading[j]))

                                //        dicFieldValue.Add(heading[j], word[j]);
                                //}

                                lstRespondentId.Add(word[1]);

                                strline = txtReader.ReadLine();
                                //p = p + 1;


                            }
                            //}
                            txtReader.Close();
                        }

                        string totalCount = lstRespondentId.Count.ToString();



                        //if (listOfImages.Count>0)
                        //{
                        //    for (int i = 0; i < lstRespondentId.Count; i++)
                        //    {
                        //        for (int j = 0; j < listOfImages.Count; j++)
                        //        {
                        //            lblDownloadStatus.Content = "Now Remove : " + (i + 1).ToString() + " / " + totalCount;
                        //            DoEvents();
                        //            try
                        //            {
                        //                ServicePointManager.Expect100Continue = true;
                        //                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        //                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                        //                using (WebClient client = new WebClient())
                        //                {
                        //                    //// using System.Net;
                        //                    //ServicePointManager.Expect100Continue = true;
                        //                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        //                    //// Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                        //                    string temp1 = serverPath + "/images/" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg";
                        //                    //string temp2 = txtExcelFilePath.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                        //                    string temp2 = txtExcelFilePath.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg";
                        //                    if (!File.Exists(temp2))
                        //                        //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                        //                        client.DownloadFile(serverPath + "/images/" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg", txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg");
                        //                }
                        //            }
                        //            catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                        //        }
                        //    }
                        //}



                        if (listOfRecordings.Count > 0)
                        {
                            for (int i = 0; i < lstRespondentId.Count; i++)
                            {
                                for (int j = 0; j < listOfRecordings.Count; j++)
                                {
                                    lblDownloadStatus.Content = "Now Remove : " + (i + 1).ToString() + " / " + totalCount;


                                    DoEvents();
                                    try
                                    {
                                        ServicePointManager.Expect100Continue = true;
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                                        MyWebRequest myRequest1;
                                        string myPath = "file_name=" + lstRespondentId[i] + "_" + listOfRecordings[j] + ".mp3";
                                        myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/deletemedia.php", "POST", "file_name=" + lstRespondentId[i] + "_" + listOfRecordings[j] + ".mp3");

                                        //Console.WriteLine(data);
                                        //JObject o = JObject.Parse(data);
                                        string data = myRequest1.GetResponse().ToString();

                                        lblDownloadStatusMessage.Content = "Server Message : " + lstRespondentId[i] + "__" + listOfRecordings[j] + ".mp3" + data;
                                        DoEvents();
                                        //MessageBox.Show(data);
                                        //using (WebClient client = new WebClient())
                                        //{
                                        //    ////// using System.Net;
                                        //    ////ServicePointManager.Expect100Continue = true;
                                        //    ////ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        //    ////// Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                                        //    //string temp1 = serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3";
                                        //    ////string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".mp3";
                                        //    //string temp2 = txtExcelFilePath.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3";
                                        //    //if (!File.Exists(temp2))
                                        //    //    //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".mp3", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".mp3");
                                        //    //    client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3", txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3");
                                        //}


                                    }
                                    catch (Exception ex) {/* MessageBox.Show(ex.ToString());*/ }
                                }
                            }
                        }

                        //************************************** CreateDirectory *********************************************************

                        //***********************************************************************************************
                        MessageBox.Show("Media Remove Completed");
                    }
                    else
                        MessageBox.Show("Select Excel Sheet");
                }
                else
                    MessageBox.Show("File not exist");
                //}
                //else
                //{
                //    MessageBox.Show("Directory not exist");
                //}
            }
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
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

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();
        }
    }
}
