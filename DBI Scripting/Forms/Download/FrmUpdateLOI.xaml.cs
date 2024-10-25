using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for FrmUpdateLOI.xaml
    /// </summary>
    public partial class FrmUpdateLOI : Window
    {
        Dictionary<string, string> dicProjectNameVsCode;
        Dictionary<String, UpdateLOIInfo> dicRespondentIdVsLOIInfo;
        Dictionary<String, UpdateFIFSInfo> dicRespondentIdVsFIFSInfo;
        Dictionary<String, UpdateMobileNo> dicRespondentIdVsMobileNo;
        List<String> listOfRespondentId;

        private String myPath;
        private String sSelectedSheet;

        private int myCounter;

        public FrmUpdateLOI()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();

            this.loadRespondentId();
        }

        private void loadRespondentId()
        {
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtDataFileLocation.Text))
            {
                chkListBoxRespondentId.Items.Clear();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtDataFileLocation.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                if (sSelectedSheet == "FIFS Info")
                {
                    foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                    {
                        if (myWorksheet.Name == sSelectedSheet)
                        {
                            Excel.Range range;
                            //Read the excel file
                            range = myWorksheet.UsedRange;

                            int ColNo = getOECodeColumnNumber(myWorksheet, "RESPONDENTID");
                            int FIName = getOECodeColumnNumber(myWorksheet, "FIFSINFO_1");
                            int FICode = getOECodeColumnNumber(myWorksheet, "FIFSINFO_2");
                            int FSName = getOECodeColumnNumber(myWorksheet, "FIFSINFO_3");
                            int FSCode = getOECodeColumnNumber(myWorksheet, "FIFSINFO_4");

                            dicRespondentIdVsFIFSInfo = new Dictionary<String, UpdateFIFSInfo>();


                            if (ColNo != 0 && FIName != 0 && FICode != 0 && FSName != 0 && FSCode != 0)
                            {
                                for (int i = 2; i <= range.Rows.Count; i++)
                                {
                                    if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                                    {
                                        string respondentId = myWorksheet.Cells[i, ColNo].Value.ToString();
                                        chkListBoxRespondentId.Items.Add(respondentId.Trim());

                                        string fiName = myWorksheet.Cells[i, FIName].Value.ToString();
                                        string fiCode = myWorksheet.Cells[i, FICode].Value.ToString();
                                        string fsName = myWorksheet.Cells[i, FSName].Value.ToString();
                                        string fsCode = myWorksheet.Cells[i, FSCode].Value.ToString();

                                        dicRespondentIdVsFIFSInfo.Add(respondentId.Trim(), new UpdateFIFSInfo(fiName, fiCode, fsName, fsCode));

                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("RespondentId/FIFSInfo_1/FIFSInfo_2/FIFSInfo_3/FIFSInfo_4 Column not found");
                            }
                        }
                    }
                }
                else if (sSelectedSheet == "Updated LOI")
                {
                    foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                    {
                        if (myWorksheet.Name == sSelectedSheet)
                        {
                            Excel.Range range;
                            //Read the excel file
                            range = myWorksheet.UsedRange;

                            int ColNo = getOECodeColumnNumber(myWorksheet, "RESPONDENTID");
                            int ColNoSTTime = getOECodeColumnNumber(myWorksheet, "SURVEYDATETIME");
                            int ColNoEDTime = getOECodeColumnNumber(myWorksheet, "SURVEYENDTIME");
                            int ColNoLOI = getOECodeColumnNumber(myWorksheet, "LENGTHOFINTV");

                            dicRespondentIdVsLOIInfo = new Dictionary<String, UpdateLOIInfo>();


                            if (ColNo != 0 && ColNoSTTime != 0 && ColNoEDTime != 0 && ColNoLOI != 0)
                            {
                                for (int i = 2; i <= range.Rows.Count; i++)
                                {
                                    if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                                    {
                                        string respondentId = myWorksheet.Cells[i, ColNo].Value.ToString();
                                        chkListBoxRespondentId.Items.Add(respondentId.Trim());

                                        string startTime = myWorksheet.Cells[i, ColNoSTTime].Value.ToString();
                                        string endTime = myWorksheet.Cells[i, ColNoEDTime].Value.ToString();
                                        string loi = myWorksheet.Cells[i, ColNoLOI].Value.ToString();

                                        dicRespondentIdVsLOIInfo.Add(respondentId.Trim(), new UpdateLOIInfo(startTime, endTime, loi));

                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("RespondentId/StartTime/EndTime/LOI Column not found");
                            }
                        }
                    }
                }
                else if (sSelectedSheet == "Updated MobileNo")
                {
                    foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                    {
                        if (myWorksheet.Name == sSelectedSheet)
                        {
                            Excel.Range range;
                            //Read the excel file
                            range = myWorksheet.UsedRange;

                            int ColNo = getOECodeColumnNumber(myWorksheet, "RESPONDENTID");
                            int ColNoWrongNo = getOECodeColumnNumber(myWorksheet, "WRONG NUMBER");
                            int ColNoCorrectedNo = getOECodeColumnNumber(myWorksheet, "CORRECTED NUMBER");

                            dicRespondentIdVsMobileNo = new Dictionary<String, UpdateMobileNo>();


                            if (ColNo != 0 && ColNoWrongNo != 0 && ColNoCorrectedNo != 0)
                            {
                                for (int i = 2; i <= range.Rows.Count; i++)
                                {
                                    if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                                    {
                                        string respondentId = myWorksheet.Cells[i, ColNo].Value.ToString();
                                        chkListBoxRespondentId.Items.Add(respondentId.Trim());

                                        string wrongNumber = myWorksheet.Cells[i, ColNoWrongNo].Value.ToString();
                                        string correctedNumber = myWorksheet.Cells[i, ColNoCorrectedNo].Value.ToString();

                                        dicRespondentIdVsMobileNo.Add(respondentId.Trim(), new UpdateMobileNo(wrongNumber, correctedNumber));

                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("RespondentId/Wrong Number/CorrectedNumber Column not found");
                            }
                        }
                    }
                }

            }
        }

        private int getOECodeColumnNumber(Excel.Worksheet ws, string fieldName)
        {

            Excel.Range range;
            //Read the excel file
            range = ws.UsedRange;
            for (int i = 1; i <= 10; i++)
            {
                if (ws.Cells[1, i].Value2 != null)
                {
                    if (ws.Cells[1, i].Value.ToString().ToUpper() == fieldName)
                        return i;
                }
            }

            return 0;
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (chkSelectAll.IsChecked == true)
            {
                foreach (var item in chkListBoxRespondentId.Items)
                {
                    chkListBoxRespondentId.SelectedItems.Add(item);
                }
            }
            else
            {
                foreach (var item in chkListBoxRespondentId.Items)
                {
                    chkListBoxRespondentId.SelectedItems.Remove(item);
                }
            }
        }

        private void chkListBoxRespondentId_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            myCounter = 0;
            listOfRespondentId.Clear();
            foreach (var item in chkListBoxRespondentId.Items)
            {
                for (int i = 0; i < chkListBoxRespondentId.SelectedItems.Count; i++)
                {
                    if (chkListBoxRespondentId.SelectedItems[i].ToString() == item.ToString())
                    {
                        listOfRespondentId.Add(item.ToString());
                        myCounter++;
                    }
                }

            }
            lblNoOfRejectionId.Content = "No of Rejection Id : " + (myCounter).ToString();
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void frmDownloadLOI_Loaded(object sender, RoutedEventArgs e)
        {
            listOfRespondentId = new List<string>();
            this.getProjectsFromServer();
            myCounter = 0;
        }

        private async void getProjectsFromServer()
        {
            try
            {
                await DoWorkAsync();

                dicProjectNameVsCode = new Dictionary<string, string>();

                DownloadClass myDownloadClass = new DownloadClass();

                List<ProjectInfo> listOfProjectInfo = new List<ProjectInfo>();

                listOfProjectInfo = myDownloadClass.getProjectInfoFromServer();

                comProjectName.Items.Clear();
                for (int i = 0; i < listOfProjectInfo.Count; i++)
                {
                    string projectName = listOfProjectInfo[i].ProjectName;
                    comProjectName.Items.Add(projectName);

                    dicProjectNameVsCode.Add(projectName, listOfProjectInfo[i].ProjectCode);
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        UpdateLOIInfo updateLOIInfo = dicRespondentIdVsLOIInfo[listOfRespondentId[i]];
                        string StartTime = updateLOIInfo.StartTime;
                        string EndTime = updateLOIInfo.EndTime;
                        string LengthOfInterview = updateLOIInfo.LOI;

                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/updateloibyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text] + "&StartTime=" + StartTime + "&EndTime=" + EndTime + "&LengthOfInterview=" + LengthOfInterview);
                            //else
                            //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);

                            //Console.WriteLine(data);
                            //JObject o = JObject.Parse(data);
                            string data = myRequest1.GetResponse().ToString();
                            JObject o = JObject.Parse(data);

                            //MessageBox.Show(o.GetValue("message").ToString());

                            lblProgress.Content = "Progress : " + (i + 1).ToString() + "/" + myCounter.ToString();
                            lblStatus.Content = o.GetValue("message").ToString();
                            DoEvents();

                            //DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                            //if (dt1_temp.Rows.Count > 0)
                            //    dt1.Merge(dt1_temp);

                            //if (!dicDateVsTInterviewInfo.ContainsKey(startDate))
                            //    dicDateVsTInterviewInfo.Add(startDate, dt1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    MessageBox.Show("Complete rejection operation");
                }
                else
                    MessageBox.Show("Need to select the respondent Id that you want to reject");
            }
        }

        private bool setData()
        {
            if (checkData())
                return true;
            else
                return false;
        }

        private bool checkData()
        {
            if (comProjectName.Text == "")
            {
                MessageBox.Show("Please select a project");
                return false;
            }
            if (txtDataFileLocation.Text == "")
            {
                MessageBox.Show("Please select the data file (Excel)");
                return false;
            }
            else if (!File.Exists(txtDataFileLocation.Text))
            {
                MessageBox.Show("Wrong file path");
                return false;
            }

            return true;
        }

        private void btnUpdate_FIFS_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    this.loadRespondentId();

                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        UpdateFIFSInfo updateFIFSInfo = dicRespondentIdVsFIFSInfo[listOfRespondentId[i]];
                        string FIName = updateFIFSInfo.FIName;
                        string FICode = updateFIFSInfo.FICode;
                        string FSName = updateFIFSInfo.FSName;
                        string FSCode = updateFIFSInfo.FSCode;

                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/updatefifsbyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text] + "&FIName=" + FIName + "&FICode=" + FICode + "&FSName=" + FSName + "&FSCode=" + FSCode);
                            //else
                            //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);

                            //Console.WriteLine(data);
                            //JObject o = JObject.Parse(data);
                            string data = myRequest1.GetResponse().ToString();
                            JObject o = JObject.Parse(data);

                            //MessageBox.Show(o.GetValue("message").ToString());

                            lblProgress.Content = "Progress : " + (i + 1).ToString() + "/" + myCounter.ToString();
                            lblStatus.Content = o.GetValue("message").ToString();
                            DoEvents();

                            //DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                            //if (dt1_temp.Rows.Count > 0)
                            //    dt1.Merge(dt1_temp);

                            //if (!dicDateVsTInterviewInfo.ContainsKey(startDate))
                            //    dicDateVsTInterviewInfo.Add(startDate, dt1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    MessageBox.Show("Complete rejection operation");
                }
                else
                    MessageBox.Show("Need to select the respondent Id that you want to reject");
            }
        }

        private void btnUpdateTemplete_Click(object sender, RoutedEventArgs e)
        {
            string sTemp;

            sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] arrayPath = sTemp.Split('\\');

            FileInfo fi = new FileInfo(sTemp + "\\LOI Correction Template.xlsx");
            if (fi.Exists)
            {
                System.Diagnostics.Process.Start(sTemp + "\\LOI Correction Template.xlsx");
            }
            else
            {
                //file doesn't exist
            }
        }

        private void btnUpdate_MobileNo_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    this.loadRespondentId();
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        UpdateMobileNo updateMobileNo = dicRespondentIdVsMobileNo[listOfRespondentId[i]];
                        string WrongNumber = updateMobileNo.WrongMobileNo;
                        string CorrectedNumber = updateMobileNo.CorrectedMobileNO;

                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/updatemobilebyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text] + "&WrongNumber=" + WrongNumber + "&CorrectedNumber=" + CorrectedNumber);
                            //else
                            //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);

                            //Console.WriteLine(data);
                            //JObject o = JObject.Parse(data);
                            string data = myRequest1.GetResponse().ToString();
                            JObject o = JObject.Parse(data);

                            //MessageBox.Show(o.GetValue("message").ToString());

                            lblProgress.Content = "Progress : " + (i + 1).ToString() + "/" + myCounter.ToString();
                            lblStatus.Content = o.GetValue("message").ToString();
                            DoEvents();

                            //DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                            //if (dt1_temp.Rows.Count > 0)
                            //    dt1.Merge(dt1_temp);

                            //if (!dicDateVsTInterviewInfo.ContainsKey(startDate))
                            //    dicDateVsTInterviewInfo.Add(startDate, dt1);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    MessageBox.Show("Complete rejection operation");
                }
                else
                    MessageBox.Show("Need to select the respondent Id that you want to reject");
            }
        }


    }
}