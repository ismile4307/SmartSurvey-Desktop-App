using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace DBI_Scripting.Forms
{
    /// <summary>
    /// Interaction logic for FrmDownloadData.xaml
    /// </summary>
    public partial class FrmDownloadData : Window
    {
        private Dictionary<string, string> dicDateConsiderVsCode;
        private Dictionary<string, string> dicInterviewTypeVsCode;
        private Dictionary<string, string> dicProjectNameVsCode;

        private Dictionary<string, string> dicProjectNameVsStartDate;

        private Dictionary<string, string> dicProjectNameVsDBName;


        private Dictionary<String, DataTable> dicDateVsTInterviewInfo = new Dictionary<String, DataTable>();
        private Dictionary<String, DataTable> dicDateVsTRespAnswer = new Dictionary<String, DataTable>();
        private Dictionary<String, DataTable> dicDateVsTOpenended = new Dictionary<String, DataTable>();

        DataTable dt1, dt2, dt3;

        private String startDate, endDate, interviewType;
        private String databasePath;

        public FrmDownloadData()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //txtServerAddress.Text = Properties.Settings.Default.ServerAddress;
            txtServerAddress.Text = StaticClass.SERVER_URL + "/deskapi/";

            dicDateConsiderVsCode = new Dictionary<string, string>();
            dicInterviewTypeVsCode = new Dictionary<string, string>();

            this.populateDic();
            dtpDateFrom.Text = DateTime.Now.ToShortDateString().ToString();
            dtpDateTo.Text = DateTime.Now.ToShortDateString().ToString();

            comInterviewType.Text = "Final Interviews";
            comConsiderDate.Text = "Sync Date";

            this.getProjectsFromServer();

        }

        private void populateDic()
        {
            comConsiderDate.Items.Clear();
            comConsiderDate.Items.Add("Sync Date");
            comConsiderDate.Items.Add("Interview Date");

            dicDateConsiderVsCode.Clear();
            dicDateConsiderVsCode.Add("Sync Date", "2");
            dicDateConsiderVsCode.Add("Interview Date", "1");

            comInterviewType.Items.Clear();
            comInterviewType.Items.Add("Final Interviews");
            comInterviewType.Items.Add("Test Interviews");
            comInterviewType.Items.Add("Reject Interviews");
            comInterviewType.Items.Add("Terminate Interviews");
            comInterviewType.Items.Add("Incomplete Interviews");
            comInterviewType.Items.Add("Final & Terminate Interviews");
            comInterviewType.Items.Add("Deleted Interviews");

            dicInterviewTypeVsCode.Clear();
            dicInterviewTypeVsCode.Add("Final Interviews", "1");
            dicInterviewTypeVsCode.Add("Test Interviews", "2");
            dicInterviewTypeVsCode.Add("Reject Interviews", "3");
            dicInterviewTypeVsCode.Add("Terminate Interviews", "4");
            dicInterviewTypeVsCode.Add("Incomplete Interviews", "5");
            dicInterviewTypeVsCode.Add("Final & Terminate Interviews", "6");
            dicInterviewTypeVsCode.Add("Deleted Interviews", "7");
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel 2007|*.xlsx|All Files|*.*";
            saveFileDialog1.Title = "Save Data File";
            //saveFileDialog1.ShowDialog();

            if (saveFileDialog1.ShowDialog() == true)
            {
                string s_temp = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf('.'));
                string fileFormat = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf('.'));
                txtSaveLocation.Text = s_temp + "_" + dtpDateFrom.Text.Replace('/', '_') + "_" + dtpDateTo.Text.Replace('/', '_') + fileFormat;

                Properties.Settings.Default.StartupPath = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf('\\')); ;
                Properties.Settings.Default.Save();
            }
        }

        private async void getProjectsFromServer()
        {
            await DoWorkAsync();
            try
            {
                dicProjectNameVsCode = new Dictionary<string, string>();
                dicProjectNameVsDBName = new Dictionary<string, string>();
                dicProjectNameVsStartDate = new Dictionary<string, string>();

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
                    dicProjectNameVsStartDate.Add(projectName, convertData(listOfProjectInfo[i].StartDate));
                }


                //string temp = myRequest.GetResponse().ToString();
                //if (temp == "New record created successfully")
                //{
                //    UpdateSyncStatusToComplete(dicProjectNameVsCode[comProjectSyncData.Text], listOfUnSyncRespId[x], txtAnswerDBPath.Text);
                //    //this.loadGrid(txtAnswerDBPath.Text);
                //    //MessageBox.Show("One record has been uploaded successfully...");
                //    lblMessageSyncData.Text = listOfUnSyncRespId[x] + " uploaded sucessfully";
                //    Application.DoEvents();
                //}
                //else
                //{
                //MessageBox.Show("");
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server connection failed");
            }

        }

        private string convertData(string myDate)
        {
            string convertedDate = "";
            if (myDate != "")
            {
                string[] word = myDate.Split('-');
                convertedDate = word[1] + "-" + word[0] + "-" + word[2];
                return convertedDate;
            }

            return convertedDate;
        }

        private void populateTInterviewInfo()
        {
            //try
            //{
                lblExecute.Content = "Execute Now : " + "Download Data";
                DoEvents();

                WebClient c = new WebClient();
                MyWebRequest myRequest1;
                //if (chkDeletedRec.Checked == false)
                //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/respondent.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
                myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/respondentbyproject.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
                //else
                //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);
            string temp=StaticClass.SERVER_URL + "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text];
                //Console.WriteLine(data);
                //JObject o = JObject.Parse(data);
                string data = myRequest1.GetResponse().ToString();

                DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                if (dt1_temp.Rows.Count > 0)
                    dt1.Merge(dt1_temp);

                //if (!dicDateVsTInterviewInfo.ContainsKey(startDate))
                //    dicDateVsTInterviewInfo.Add(startDate, dt1);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void populateTResponseInfo()
        {
            lblExecute.Content = "Execute Now : " + "Download Data";
            DoEvents();


            long myOffset = 0;
            long noOfRow = 10000;

            while (noOfRow == 10000)
            {
                WebClient c = new WebClient();
                MyWebRequest myRequest2;
                //if (chkDeletedRec.Checked == false)
                //myRequest2 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/answer.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&myOffset=" + myOffset.ToString() + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
                myRequest2 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/answerbyproject.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&myOffset=" + myOffset.ToString() + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
                //else
                //myRequest2 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/answerdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);
                //Console.WriteLine(data);
                //JObject o = JObject.Parse(data);
                string data = myRequest2.GetResponse().ToString();

                DataTable dt2_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                //if (dt2_temp != null)
                //{
                if (dt2_temp.Rows.Count > 0)
                    dt2.Merge(dt2_temp);


                myOffset = myOffset + dt2_temp.Rows.Count;
                noOfRow = dt2_temp.Rows.Count;
                //}

                c.Dispose();

                //MessageBox.Show("");
            }




            //if (!dicDateVsTRespAnswer.ContainsKey(startDate))
            //    dicDateVsTRespAnswer.Add(startDate, dt2);

        }

        private void populateTOpenendedInfo()
        {
            lblExecute.Content = "Execute Now : " + "Download Data";
            DoEvents();

            WebClient c = new WebClient();
            MyWebRequest myRequest3;
            //if (chkDeletedRec.Checked == false)
            //myRequest3 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/openended.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
            myRequest3 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/openendedbyproject.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comConsiderDate.Text] + "&projectCode=" + dicProjectNameVsCode[comProjectName.Text] + "&interviewType=" + dicInterviewTypeVsCode[comInterviewType.Text]);
            //else
            //myRequest3 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/openendeddel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);
            //Console.WriteLine(data);
            //JObject o = JObject.Parse(data);
            string data = myRequest3.GetResponse().ToString();

            DataTable dt3_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

            if (dt3_temp.Rows.Count > 0)
                dt3.Merge(dt3_temp);

            //if (!dicDateVsTOpenended.ContainsKey(startDate))
            //    dicDateVsTOpenended.Add(startDate, dt3);

        }

        private void exportToExcel()
        {
            //string TypeOfReport;
            //try
            //{

                databasePath = @"C:\Temp\" + dicProjectNameVsDBName[comProjectName.Text];
                //databasePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + dicProjectNameVsDBName[comProjectName.Text];

                if (File.Exists(databasePath) == false)
                {
                    MessageBox.Show("Script file not found..");
                    return;
                }


                SQLite sql = new SQLite(databasePath);
                sql.connect();



                lblOperationNo.Content = "Operation No : 3/3";
                lblExecute.Content = "Execute Now : Populate Excel";
                //Application.DoEvents();




                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Name = "Openeneded";


                xlWorkSheet.Cells[1, 1] = "Respondent Id";
                xlWorkSheet.Cells[1, 2] = "QId";
                xlWorkSheet.Cells[1, 3] = "Attribute Value";
                xlWorkSheet.Cells[1, 4] = "OE Verbatim";

                int row = 2;
                DataTableReader drd = dt3.CreateDataReader();// sql.getDataTableOpenended();

                while (drd.Read())
                {
                    xlWorkSheet.Cells[row, 1] = "'" + drd["respondent_id"].ToString();
                    xlWorkSheet.Cells[row, 2] = "'" + drd["q_id"].ToString();
                    xlWorkSheet.Cells[row, 3] = "'" + drd["attribute_value"].ToString();
                    xlWorkSheet.Cells[row, 4] = "'" + ReplaceNewlines(drd["response"].ToString(), " ");
                    row = row + 1;
                }

                xlWorkSheet.Columns.AutoFit();










                //******************* Get the Openended *****************************************************

                Microsoft.Office.Interop.Excel.Sheets worksheets = xlWorkBook.Worksheets;
                var xlNewSheet = (Microsoft.Office.Interop.Excel.Worksheet)worksheets.Add(worksheets[1]);
                xlNewSheet.Name = "Data";


                List<string> columnName = new List<string>();
                List<List<string>> tableData = new List<List<string>>();

                columnName = sql.getTableColumnReport();
                tableData = sql.getTableDataReport(columnName, dt1, dt2, dt3, progressBar1);

                for (int i = 1; i <= columnName.Count; i++)
                {
                    xlNewSheet.Cells[1, i] = "'" + columnName[i - 1];
                }



                int p = 1;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = tableData.Count * tableData[0].Count;

                // Get dimensions of the 2-d array
                int rowCount = tableData.Count;// arrays.GetLength(0);
                int columnCount = tableData[0].Count;// arrays.GetLength(0);

                string[,] arrays = new string[rowCount, columnCount];//tableData.Select(a => a.ToArray()).ToArray();


                for (int i = 1; i <= tableData.Count; i++)
                {
                    for (int j = 1; j <= tableData[i - 1].Count; j++)
                    {
                        progressBar1.Value = p;
                        p++;

                        arrays[i - 1, j - 1] = "'" + ReplaceNewlines(tableData[i - 1].ToList()[j - 1], " ");

                        //xlNewSheet.Cells[i + 1, j] = "'" + ReplaceNewlines(tableData[i - 1].ToList()[j - 1], " ");
                    }
                }



                // Get an Excel Range of the same dimensions
                Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)xlNewSheet.Cells[2, 1];
                range = range.get_Resize(rowCount, columnCount);
                // Assign the 2-d array to the Excel Range
                range.set_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault, arrays);


                xlNewSheet.Columns.AutoFit();

                //xlApp.Visible = true;




                //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                //xlWorkBook.SaveAs("D:\\Ismile Personal\\New folder (2)\\Analysis\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.SaveAs(txtSaveLocation.Text, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                sql.releaseObject(xlWorkSheet);
                sql.releaseObject(xlWorkBook);
                sql.releaseObject(xlApp);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("No Data found for given date range");
            //}
        }

        private string ReplaceNewlines(string blockOfText, string replaceWith)
        {
            return blockOfText.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            if (setData())
            {
                String baseDirectory = @"C:\Temp\";

                if (!Directory.Exists(@"C:\Temp"))
                {
                    MessageBox.Show("Temp Derecory not exist in C drive. Pleaes create it first...");
                    return;
                }
                //databasePath = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                databasePath = baseDirectory + dicProjectNameVsDBName[comProjectName.Text];

                if (File.Exists(databasePath) == true)
                    File.Delete(databasePath);

                if (File.Exists(databasePath) == false || chkDownloadScript.IsChecked == true)
                {
                    try
                    {
                        progressBar1.Value = 0;
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                        using (WebClient client = new WebClient())
                        {
                            //string source = Properties.Settings.Default.ServerAddress + "/" + dicProjectNameVsDBName[comProjectName.Text];
                            string source = StaticClass.SERVER_URL + "/scripts/" + dicProjectNameVsDBName[comProjectName.Text];
                            //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                            //string destination = System.AppDomain.CurrentDomain.BaseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                            string destination = baseDirectory + dicProjectNameVsDBName[comProjectName.Text];
                            if (!File.Exists(destination))
                                //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                client.DownloadFile(source, destination);
                        }
                    }
                    catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                }



                dt1 = new DataTable();
                dt2 = new DataTable();
                dt3 = new DataTable();



                endDate = "";
                //this.cleanDB();
                double totalDay = (Convert.ToDateTime(dtpDateTo.Text) - Convert.ToDateTime(dtpDateFrom.Text)).TotalDays;

                dicDateVsTInterviewInfo.Clear();
                dicDateVsTRespAnswer.Clear();
                dicDateVsTOpenended.Clear();
                string stDate = dtpDateFrom.Text;
                int iDay = 1;
                for (int i = 0; i <= Convert.ToInt32(totalDay); i = i + iDay)
                {
                    if (endDate.ToString() != "")
                        stDate = Convert.ToDateTime(endDate).AddDays(iDay).ToShortDateString();

                    //string tmp = Convert.ToDateTime(stDate).AddDays(iDay).ToShortDateString();

                    startDate = stDate.Split('/')[2] + "-" + stDate.Split('/')[0] + "-" + stDate.Split('/')[1];
                    endDate = stDate.Split('/')[2] + "-" + stDate.Split('/')[0] + "-" + stDate.Split('/')[1];
                    //endDate = tmp.Split('/')[2] + "-" + tmp.Split('/')[0] + "-" + tmp.Split('/')[1];
                    //string tmp = dateTimePickerFrom.Value.ToShortDateString();
                    //startDate = tmp.Split('/')[2] + "-" + tmp.Split('/')[0] + "-" + tmp.Split('/')[1];
                    //tmp = dateTimePickerTo.Value.ToShortDateString();
                    //endDate = tmp.Split('/')[2] + "-" + tmp.Split('/')[0] + "-" + tmp.Split('/')[1];


                    lblOperationNo.Content = "Operation No : 1/2";
                    lblExecute.Content = "Execute Now : Download Data";
                    lblCurrentDate.Content = startDate;
                    //Application.DoEvents();

                    this.populateTInterviewInfo();
                    this.populateTResponseInfo();
                    this.populateTOpenendedInfo();

                }

                lblOperationNo.Content = "Operation No : 2/2";
                lblExecute.Content = "Execute Now : Populate Table";
                DoEvents();

                this.exportToExcel();

                //if (chkDataBackup.Checked == true)
                //{
                //    this.BackupDataInSQLiteDB();
                //}

                MessageBox.Show("Data populate complete");


            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private bool setData()
        {
            if (checkData())
            {
                startDate = dtpDateFrom.Text;
                endDate = dtpDateTo.Text;

                return true;
            }
            return false;
        }

        private bool checkData()
        {
            if (comProjectName.Text == "")
            {
                MessageBox.Show("Project Name should be slected");
                return false;
            }
            if (comConsiderDate.Text == "")
            {
                MessageBox.Show("Consider Date should be selected");
                return false;
            }
            if (comInterviewType.Text == "")
            {
                MessageBox.Show("Interview Type should be selected");
                return false;
            }
            if (txtSaveLocation.Text == "")
            {
                MessageBox.Show("Please select the save location to save the data");
                return false;
            }
            if (dtpDateFrom.SelectedDate.Value > dtpDateTo.SelectedDate.Value)
            {
                MessageBox.Show("Start date should not be greated than end data");
                return false;
            }

            return true;
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void comProjectName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if(comProjectName.Text!="")
            //dtpDateFrom.Text = dicProjectNameVsStartDate[comProjectName.Text];
        }

        private void comProjectName_DropDownClosed(object sender, EventArgs e)
        {
            if (comProjectName.Text != "")
                dtpDateFrom.Text = dicProjectNameVsStartDate[comProjectName.Text];
        }

        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                //do some work HERE
                Thread.Sleep(1000);
            });
        }
    }
}


//try
//{
//    WebClient client = new WebClient();
//    string myFile = @"D:\test_file.txt";
//    client.Credentials = CredentialCache.DefaultCredentials;
//    client.UploadFile(@"http://localhost/uploads/upload.php", "POST", myFile);
//    client.Dispose();
//}
//catch (Exception err)
//{
//    MessageBox.Show(err.Message);
//}

//<?php
//    $filepath = $_FILES["file"]["tmp_name"];
//    move_uploaded_file($filepath,"test_file.txt");
//?>