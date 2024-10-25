using DBI_Scripting.Classes;
using DBI_Scripting.Model;
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
    /// Interaction logic for FrmDownloadMedia.xaml
    /// </summary>
    public partial class FrmDownloadMedia : Window
    {

        private Dictionary<string, string> dicProjectNameVsCode;
        private Dictionary<string, string> dicProjectNameVsDBName;

        private String myPath;

        private string QdbConnString;
        private string mProjectName;
        private String serverPath = "";

        public SQLiteConnection Qconnection;

        private List<String> listOfImages;
        private List<String> listOfRecordings;
        private String dbName;

        public FrmDownloadMedia()
        {
            InitializeComponent();
        }

        public void connect()
        {
            //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
            string databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectName.Text];
            this.QdbConnString = @"Data Source=" + databasePath + "; Version=3;";
            this.Qconnection = new SQLiteConnection(this.QdbConnString);

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void frmDownloadMedia_Loaded(object sender, RoutedEventArgs e)
        {
            this.populateCombo();
            this.getProjectsFromServer();
            serverPath = StaticClass.SERVER_URL;// Properties.Settings.Default.ServerAddress;

        }

        private async void getProjectsFromServer()
        {
            await DoWorkAsync();

            dicProjectNameVsCode = new Dictionary<string, string>();
            dicProjectNameVsDBName = new Dictionary<string, string>();

            DownloadClass myDownloadClass = new DownloadClass();

            List<ProjectInfo> listOfProjectInfo = new List<ProjectInfo>();

            listOfProjectInfo = myDownloadClass.getProjectInfoFromServer();

            comProjectName.Items.Clear();
            for (int i = 0; i < listOfProjectInfo.Count; i++)
            {
                string projectName = listOfProjectInfo[i].ProjectName;
                comProjectName.Items.Add(projectName);

                dicProjectNameVsCode.Add(projectName, listOfProjectInfo[i].ProjectCode);
                dicProjectNameVsDBName.Add(projectName, listOfProjectInfo[i].DatabaseName);
            }

        }

        private void populateCombo()
        {
            comMediaType.Items.Clear();
            comMediaType.Items.Add("Recording");
            comMediaType.Items.Add("Image");

            comMediaType.Text = "Recording";
        }

        private void btnBrowseDataFile_Click(object sender, RoutedEventArgs e)
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
                    txtDataFileLocation.Text = openFileDialog1.FileName;
                    myPath = txtDataFileLocation.Text.Substring(0, txtDataFileLocation.Text.LastIndexOf('\\'));
                    if (comMediaType.Text == "Image")
                        txtSaveLocation.Text = myPath + "\\Images";
                    else
                        txtSaveLocation.Text = myPath + "\\Recordings";

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

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (!Directory.Exists(txtSaveLocation.Text))
                    Directory.CreateDirectory(txtSaveLocation.Text);

                //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                string databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectName.Text];
                if (File.Exists(databasePath) == false)
                {
                    try
                    {
                        lblDownloadStatus.Content = "Downloaded : Script Database";
                        DoEvents();

                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                        using (WebClient client = new WebClient())
                        {
                            string source = StaticClass.SERVER_URL + "/scripts/" + dicProjectNameVsDBName[comProjectName.Text];
                            //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                            //string destination = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                            string destination = @"C:\Temp\" + dicProjectNameVsDBName[comProjectName.Text];

                            if (!File.Exists(destination))
                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                client.DownloadFile(source, destination);
                        }
                    }
                    catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                }


                if (Directory.Exists(txtSaveLocation.Text))
                {

                    if (File.Exists(txtDataFileLocation.Text) == true)
                    {

                        this.prepareImageAndRecordingList();

                        List<String> lstTextFile = new List<string>();

                        List<Dictionary<String, String>> lstRespondentId = new List<Dictionary<String, String>>();


                        //if (lstWorkSheetName.Count > 0)
                        //{
                        Excel.Application xlApp = new Excel.Application();
                        Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtDataFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        lstTextFile.Clear();            //Clear the txt file path list
                        for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                        {
                            if (xlWorkBook.Worksheets[i].Name.ToString() == "Data")
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


                                Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();

                                for (int j = 0; j < heading.Length; j++)
                                {
                                    if (!dicFieldValue.ContainsKey(heading[j]))

                                        dicFieldValue.Add(heading[j], word[j]);
                                }

                                lstRespondentId.Add(dicFieldValue);

                                strline = txtReader.ReadLine();
                                //p = p + 1;


                            }
                            //}
                            txtReader.Close();
                        }

                        string totalCount = lstRespondentId.Count.ToString();

                        if (comMediaType.Text == "Image")
                        {
                            for (int i = 0; i < lstRespondentId.Count; i++)
                            {
                                for (int j = 0; j < listOfImages.Count; j++)
                                {
                                    lblDownloadStatus.Content = "Downloaded : " + (i + 1).ToString() + " / " + totalCount;
                                    DoEvents();
                                    try
                                    {
                                        ServicePointManager.Expect100Continue = true;
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                                        using (WebClient client = new WebClient())
                                        {
                                            //// using System.Net;
                                            //ServicePointManager.Expect100Continue = true;
                                            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                            //// Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                                            string temp1 = serverPath + "/images/img_" + dicProjectNameVsCode[mProjectName] + "/" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg";
                                            //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                                            string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg";
                                            if (!File.Exists(temp2))
                                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                                client.DownloadFile(serverPath + "/images/img_" + dicProjectNameVsCode[mProjectName] + "/" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg", txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfImages[j] + ".jpg");
                                        }
                                    }
                                    catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < lstRespondentId.Count; i++)
                            {
                                for (int j = 0; j < listOfRecordings.Count; j++)
                                {
                                    lblDownloadStatus.Content = "Downloaded : " + (i + 1).ToString() + " / " + totalCount;
                                    DoEvents();
                                    try
                                    {
                                        ServicePointManager.Expect100Continue = true;
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                                        using (WebClient client = new WebClient())
                                        {
                                            //// using System.Net;
                                            //ServicePointManager.Expect100Continue = true;
                                            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                            //// Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                                            string temp1 = serverPath + "/audio/rec_" + dicProjectNameVsCode[mProjectName] + "/" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3";
                                            //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".mp3";
                                            string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3";
                                            if (!File.Exists(temp2))
                                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".mp3", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".mp3");
                                                client.DownloadFile(serverPath + "/audio/rec_" + dicProjectNameVsCode[mProjectName] + "/" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3", txtSaveLocation.Text + "\\" + lstRespondentId[i]["RespondentId"] + "_" + listOfRecordings[j] + ".mp3");
                                        }
                                    }
                                    catch (Exception ex) {/* MessageBox.Show(ex.ToString());*/ }
                                }
                            }
                        }

                        //************************************** CreateDirectory *********************************************************

                        //***********************************************************************************************
                        MessageBox.Show("Download Completed");

                    }
                    else
                        MessageBox.Show("Select Excel Sheet");
                }
                else
                {
                    MessageBox.Show("Directory not exist");
                }
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

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
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

        private void btnKillProcess_Click(object sender, RoutedEventArgs e)
        {
            this.quitProcess();
            MessageBox.Show("Excel process is killed");
        }

        private bool setData()
        {
            if (checkData())
            {
                return true;
            }
            return false;
        }

        private bool checkData()
        {
            if (comProjectName.Text == "")
            {
                MessageBox.Show("Project Name should be selected");
                return false;
            }
            if (txtDataFileLocation.Text == "")
            {
                MessageBox.Show("Data file location should not be blank");
                return false;
            }
            if (!File.Exists(txtDataFileLocation.Text))
            {
                MessageBox.Show("Invalid data file location");
                return false;
            }
            if (txtSaveLocation.Text == "")
            {
                MessageBox.Show("Save location should not be blank");
                return false;
            }
            return true;
        }

        private void comMediaType_DropDownClosed(object sender, EventArgs e)
        {
            if (myPath != "")
            {
                if (comMediaType.Text == "Image")
                    txtSaveLocation.Text = myPath + "\\Images";
                else
                    txtSaveLocation.Text = myPath + "\\Recordings";
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

        private void comProjectName_DropDownClosed(object sender, EventArgs e)
        {
            if (comProjectName.Text != "")
                mProjectName = comProjectName.Text;
        }

    }
}
