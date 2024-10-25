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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmSyncData.xaml
    /// </summary>
    public partial class FrmSyncData : Window
    {

        private String myPath, fileName, fileDirectory;

        private Dictionary<string, string> dicProjectNameVsCode;
        private Dictionary<string, string> dicDataTypeVsCode;

        public FrmSyncData()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtServerAddress.Text = StaticClass.SERVER_URL + "/deskapi/";
            dicDataTypeVsCode = new Dictionary<string, string>();
            this.populateDic();
            dtpDateFrom.Text = DateTime.Now.ToShortDateString().ToString();
            this.getProjectsFromServer();
        }

        private void populateDic()
        {
            comDataType.Items.Clear();
            comDataType.Items.Add("All Data");
            comDataType.Items.Add("Not Sync Data");

            comDataType.Text = "Not Sync Data";

            dicDataTypeVsCode.Clear();
            dicDataTypeVsCode.Add("All Data", "2");
            dicDataTypeVsCode.Add("Not Sync Data", "0");
        }

        private async void getProjectsFromServer()
        {
            await DoWorkAsync();
            try
            {
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

        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                //do some work HERE
                Thread.Sleep(1000);
            });
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

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            if (comProjectName.Text != "")
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
                        txtDataBasePath.Text = openFileDialog1.FileName;
                        myPath = txtDataBasePath.Text.Substring(0, txtDataBasePath.Text.LastIndexOf('\\'));
                        fileName = txtDataBasePath.Text.Substring(txtDataBasePath.Text.LastIndexOf('\\') + 1);
                        fileDirectory = txtDataBasePath.Text.Substring(0, txtDataBasePath.Text.LastIndexOf('\\'));

                        Properties.Settings.Default.StartupPath = myPath;
                        Properties.Settings.Default.Save();

                        this.loadGrid();

                    }
                    else
                        txtDataBasePath.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Project name should be selected");
            }
        }

        private void loadGrid()
        {
            dataGridView.ItemsSource = null;
            if (txtDataBasePath.Text != "")
            {
                if (File.Exists(txtDataBasePath.Text))
                {
                    ConnectionDB connDB = new ConnectionDB();
                    if (connDB.connect(txtDataBasePath.Text) == true)
                    {
                        if (connDB.sqlite_conn.State == ConnectionState.Closed)
                            connDB.sqlite_conn.Open();

                        SQLiteDataAdapter dadpt = null;

                        if (comDataType.Text == "Not Sync Data")
                        {
                            dadpt = new SQLiteDataAdapter("SELECT ProjectId,RespondentId,Latitude,Longitude,SurveyDateTime,SurveyEndTime,LengthOfIntv,Intv_Type,FICode,FSCode,Status,SyncStatus,ScriptVersion,FIName,FSName,CentreCode,NameResp,MobileResp,AddressResp "
                                                                        + "FROM T_InterviewInfo WHERE ProjectId=" + dicProjectNameVsCode[comProjectName.Text] + " AND Intv_Type='1' AND Status='1' AND SyncStatus='0'", connDB.sqlite_conn);
                        }
                        else
                        {
                            dadpt = new SQLiteDataAdapter("SELECT ProjectId,RespondentId,Latitude,Longitude,SurveyDateTime,SurveyEndTime,LengthOfIntv,Intv_Type,FICode,FSCode,Status,SyncStatus,ScriptVersion,FIName,FSName,CentreCode,NameResp,MobileResp,AddressResp "
                                                                        + "FROM T_InterviewInfo WHERE ProjectId=" + dicProjectNameVsCode[comProjectName.Text] + " AND Intv_Type='1' AND Status='1' ", connDB.sqlite_conn);
                        }

                        try
                        {
                            DataSet ds = new DataSet();
                            //dadpt.Fill(ds, "Table1");

                            DataTable dt = new DataTable();
                            dadpt.Fill(dt);

                            dataGridView.ItemsSource = dt.DefaultView;

                            txtNoOfRecord.Content = "No of Rec : " + dt.Rows.Count;

                            //if (ds.Tables["Table1"].Rows.Count > 0)
                            //{
                            //    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                            //    {
                            //        txtScriptVersion.Text = dr["Version"].ToString();
                            //        priorScriptVersion = dr["Version"].ToString();
                            //        txtProjectName.Text = dr["ProjectName"].ToString();
                            //        projectId = dr["ProjectId"].ToString();
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("No Data Found");
                        }

                        if (connDB.sqlite_conn.State == ConnectionState.Open)
                            connDB.sqlite_conn.Close();

                        connDB.sqlite_conn.Dispose();
                        connDB = null;

                    }
                }
                else
                    MessageBox.Show("Invalid script file location");
            }
            else
                MessageBox.Show("Script location should not be blank");
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // execute some code
            MessageBox.Show("Ismile");
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int index = dataGridView.SelectedIndex;

            //var cellInfo = dataGridView.SelectedCells[0];
            //MessageBox.Show(cellInfo.ToString());

            if (dataGridView.SelectedItem != null)
            {
                object item = dataGridView.SelectedItem;
                string ID = (dataGridView.SelectedCells[1].Column.GetCellContent(item) as TextBlock).Text;
                //MessageBox.Show(ID);
                txtRespondentId.Text = ID;
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            if (txtRespondentId.Text != "")
            {
                //try
                //{
                    if (txtRespondentId.Text != "")
                    {

                        //SQLite sql = new SQLite(txtAnswerDBPath.Text);
                        //sql.connect();
                        string temp = "";
                        //for (int x = 0; x < listOfUnSyncRespId.Count; x++)
                        //{
                        //SQLModule.sendDataToSQLServer(txt_MobileNo.Text, SQLconDB, AccessConDB);     //
                        //List<String> lstMySyncData = getWebFormatData(dicProjectNameVsCode[comProjectSyncData.Text], txtSelectedID.Text, txtAnswerDBPath.Text);
                        List<String> lstMySyncData = getWebFormatData(dicProjectNameVsCode[comProjectName.Text], txtRespondentId.Text, txtDataBasePath.Text);
                        //create the constructor with post type and few data
                        for (int i = 0; i < lstMySyncData.Count; i++)
                        {
                            MyWebRequest myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/savedesk.php", "POST", lstMySyncData[i].ToString()); //"a=Nasim&b=Rajahshi&c=01911018447&d=1");

                            temp = myRequest.GetResponse().ToString();
                            if (temp == "New record created successfully")
                            {
                                UpdateSyncStatusToComplete(dicProjectNameVsCode[comProjectName.Text], txtRespondentId.Text, txtDataBasePath.Text);
                                this.loadGrid();
                                //this.loadGrid(txtAnswerDBPath.Text);
                                //MessageBox.Show("One record has been uploaded successfully...");


                                //lblMessageSyncData.Content = txtRespondentId.Text + " uploaded sucessfully";


                                //Application.DoEvents();

                                MessageBox.Show(temp);
                            }
                            else
                            {
                                MessageBox.Show(temp);
                            }
                        }
                        //}






                        //if (temp == "New record created successfully")
                        //{
                        //    this.loadGrid(txtAnswerDBPath.Text);
                        //    MessageBox.Show("All record has been uploaded successfully...");
                        //}












                        //MessageBox.Show(myRequest.GetResponse());
                        //MessageBox.Show("Data has been sync with server successfully\nRespondent Id : " + txt_RespondentId.Text);


                        txtRespondentId.Text = "";
                    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
            }
            else
            {
                MessageBox.Show("Respondent Id should be selected");
            }
        }

        public List<String> getWebFormatData(string projectId, string RespondentId, string dbPath)
        {
            try
            {
                SQLite sql = new SQLite(dbPath);
                sql.connect();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_InterviewInfo WHERE ProjectId=" + projectId + " AND RespondentId=" + RespondentId, sql.Qconnection);
                DataSet ds = new DataSet();

                dadpt.Fill(ds, "Table1");

                List<String> lstColumnName = new List<String>();
                List<String> lstMySyncData = new List<String>();

                for (int i = 0; i < ds.Tables["Table1"].Columns.Count; i++)
                {
                    lstColumnName.Add(ds.Tables["Table1"].Columns[i].ColumnName);
                }

                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    string myData = "";

                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        for (int i = 0; i < lstColumnName.Count; i++)
                        {
                            //if (i != lstColumnName.Count - 1)
                            myData = myData + lstColumnName[i] + "=" + dr[i].ToString() + "&";
                            //else
                            //myData = myData + lstColumnName[i] + "=" + dr[i].ToString();

                        }
                    }


                    SQLiteDataAdapter dadpt2 = new SQLiteDataAdapter("SELECT * FROM T_RespAnswer WHERE ProjectId=" + projectId + " AND RespondentId=" + RespondentId + " ORDER BY qOrderTag", sql.Qconnection);
                    DataSet ds2 = new DataSet();

                    dadpt2.Fill(ds2, "Table1");

                    if (ds2.Tables["Table1"].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds2.Tables["Table1"].Rows)
                        {
                            if (dr[2].ToString() == "C4f")
                            {
                                string Response = dr[3].ToString();
                                string ResponseDateTime = dr[4].ToString();
                            }
                            //if (dr[2].ToString() == "FS" && dr[3].ToString().Replace("'", "''") == "9114")
                            //    MessageBox.Show("");

                            myData = myData + "QId[]=" + dr[2].ToString() + "&";
                            myData = myData + "Response[]=" + dr[3].ToString().Replace("'", "''") + "&";
                            myData = myData + "ResponseDateTime[]=" + dr[4].ToString() + "&";
                            myData = myData + "qElapsedTime[]=" + dr[5].ToString() + "&";
                            myData = myData + "qOrderTag[]=" + dr[6].ToString() + "&";
                            myData = myData + "rOrderTag[]=" + dr[7].ToString() + "&";
                        }
                    }
                    else
                    {
                        myData = myData + "QId[]=&";
                        myData = myData + "Response[]=&";
                        myData = myData + "ResponseDateTime[]=&";
                        myData = myData + "qElapsedTime[]=&";
                        myData = myData + "qOrderTag[]=&";
                        myData = myData + "rOrderTag[]=&";
                    }




                    SQLiteDataAdapter dadpt3 = new SQLiteDataAdapter("SELECT * FROM T_RespOpenended WHERE ProjectId=" + projectId + " AND RespondentId=" + RespondentId, sql.Qconnection);
                    DataSet ds3 = new DataSet();

                    dadpt3.Fill(ds3, "Table1");

                    if (ds3.Tables["Table1"].Rows.Count > 0)
                    {
                        int NoOfRow = ds3.Tables["Table1"].Rows.Count;
                        int i = 1;
                        foreach (DataRow dr in ds3.Tables["Table1"].Rows)
                        {
                            myData = myData + "QIdOE[]=" + dr[2].ToString() + "&";
                            myData = myData + "AttributeValue[]=" + dr[3].ToString() + "&";
                            //myData = myData + "OpenendedResp[]=" + dr[4].ToString() + "&";

                            //if (i != NoOfRow)
                            myData = myData + "OpenendedResp[]=" + dr[4].ToString() + "&";
                            myData = myData + "OEResponseType[]=" + dr[5].ToString() + "&";

                            //else
                            //myData = myData + "OpenendedResp[]=" + dr[4].ToString() + "";           //End of the string
                        }
                    }
                    else
                    {
                        myData = myData + "QIdOE[]=&";
                        myData = myData + "AttributeValue[]=&";
                        myData = myData + "OpenendedResp[]=&";           //End of the string
                        myData = myData + "OEResponseType[]=&";           //End of the string
                    }

                    myData = myData + "SyncDateTime=" + DateTime.Now.ToString();
                    lstMySyncData.Add(myData);
                }
                return lstMySyncData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }


        }

        public void UpdateSyncStatusToComplete(string ProjectId, string RespondentId, string dbPath)
        {
            try
            {

                SQLite sql = new SQLite(dbPath);
                sql.connect();
                //Here 3 means Terminate
                SQLiteCommand sqlCmd = new SQLiteCommand("UPDATE T_InterviewInfo SET SyncStatus = '1' WHERE ProjectId = " + ProjectId + " AND RespondentId = " + RespondentId + "", sql.Qconnection);

                sqlCmd.ExecuteNonQuery();
                //conDB.connForAns.Close();
            }
            catch (Exception ex)
            {
                //if (conDB.connForAns.State == ConnectionState.Open)
                //    conDB.connForAns.Close();

                MessageBox.Show(ex.Message);
            }
        }

        private void comProjectName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comProjectName_DropDownClosed(object sender, EventArgs e)
        {
            //MessageBox.Show(comProjectName.Text);
            if (txtDataBasePath.Text != "")
            {
                this.loadGrid();
            }
        }

        private void comDataType_DropDownClosed(object sender, EventArgs e)
        {
            if (txtDataBasePath.Text != "" && comProjectName.Text != "")
            {
                this.loadGrid();
            }
        }


    }
}
