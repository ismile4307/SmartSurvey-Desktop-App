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
    /// Interaction logic for FrmRejectInterview.xaml
    /// </summary>
    public partial class FrmRejectInterview : Window
    {
        Dictionary<string, string> dicProjectNameVsCode;
        List<String> listOfRespondentId;

        private String myPath;
        private String sSelectedSheet;

        private int myCounter;
        public FrmRejectInterview()
        {
            InitializeComponent();
        }

        private void frmRejectInterview_Loaded(object sender, RoutedEventArgs e)
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

        private void loadWorkSheet()
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private int getOECodeColumnNumber(Excel.Worksheet ws)
        {

            Excel.Range range;
            //Read the excel file
            range = ws.UsedRange;
            for (int i = 1; i <= 10; i++)
            {
                if (ws.Cells[1, i].Value2 != null)
                {
                    if (ws.Cells[1, i].Value.ToString().ToUpper() == "RESPONDENTID")
                        return i;
                }
            }

            return 0;
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

        public class CheckListItem
        {
            public string Name { get; set; }
            public bool IsSelected { get; set; }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < chkListBoxWorksheet.Items.Count;i++ )
            //{
            //    MessageBox.Show("Ismile");
            //}

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

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        int ColNo = getOECodeColumnNumber(myWorksheet);
                        if (ColNo != 0)
                        {
                            for (int i = 2; i <= range.Rows.Count; i++)
                            {
                                string temp1 = myWorksheet.Cells[i, ColNo].Value.ToString();

                                if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                                {
                                    chkListBoxRespondentId.Items.Add(temp1);

                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("RespondentId Column not found");
                        }
                    }
                }

            }
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
            myCounter=0;
            listOfRespondentId.Clear();
            foreach(var item in chkListBoxRespondentId.Items)
            {
                for(int i=0;i<chkListBoxRespondentId.SelectedItems.Count;i++)
                {
                    if (chkListBoxRespondentId.SelectedItems[i].ToString() == item.ToString())
                    {
                        listOfRespondentId.Add(item.ToString());
                        myCounter++;
                    }
                }
                
            }
            lblNoOfRejectionId.Content= "No of Rejection Id : "+(myCounter).ToString();
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            
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
            if(comProjectName.Text=="")
            {
                MessageBox.Show("Please select a project");
                return false;
            }
            if(txtDataFileLocation.Text=="")
            {
                MessageBox.Show("Please select the data file (Excel)");
                return false;
            }
            else if(!File.Exists(txtDataFileLocation.Text))
            {
                MessageBox.Show("Wrong file path");
                return false;
            }

            return true;
        }

        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                //do some work HERE
                Thread.Sleep(1000);
            });
        }

        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/rejectinterviewbyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
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

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/deleteinterviewbyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
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

        private void btnRedoReject_Click(object sender, RoutedEventArgs e)
        {

            if (setData())
            {
                if (myCounter > 0)
                {
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/redorejectbyproject.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
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
                    MessageBox.Show("Complete redo rejection operation");
                }
                else
                    MessageBox.Show("Need to select the respondent Id that you want to reject");
            }
        }

        private void btnTestToLive_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                if (myCounter > 0)
                {
                    WebClient c = new WebClient();

                    for (int i = 0; i < listOfRespondentId.Count; i++)
                    {
                        try
                        {
                            MyWebRequest myRequest1;
                            //if (chkDeletedRec.Checked == false)
                            //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/testtofinal.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
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
                    MessageBox.Show("Complete redo rejection operation");
                }
                else
                    MessageBox.Show("Need to select the respondent Id that you want to reject");
            }
        }

    }
}
