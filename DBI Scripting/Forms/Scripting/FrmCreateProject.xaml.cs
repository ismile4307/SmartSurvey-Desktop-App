using DBI_Scripting.Classes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for FrmCreateProject.xaml
    /// </summary>
    public partial class FrmCreateProject : Window
    {

        private String sProjectName;
        private String sProjectCode;
        private String sDatabaseName;
        private String sStartDate;
        private String sProjectStatus;
        private String sScriptVersion;
        private String sMediaVersion;
        private String sTypeOfOperation;
        //private String sProjectStatus;

        private bool bLoadGrid;

        public FrmCreateProject()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool setData()
        {
            if (checkData())
            {
                sProjectName = txtProjectName.Text.Replace("'", "''");
                sProjectCode = txtProejctCode.Text.Replace("'", "''");
                sDatabaseName = txtScriptFileName.Text.Replace("'", "''");
                sScriptVersion = txtScriptVersion.Text.Replace("'", "''");
                sMediaVersion = txtMediaVersion.Text.Replace("'", "''");
                sStartDate = dtpStartDate.Text;
                sProjectStatus = (comProjectStatus.Text == "Active") ? "1" : "0";

                return true;

            }
            return false;
        }

        private bool checkData()
        {
            if (txtProjectName.Text == "")
            {
                MessageBox.Show("Prject Name should not be blank");
                return false;
            }
            if (txtProejctCode.Text == "")
            {
                MessageBox.Show("Project Code should not be blank");
                return false;
            }
            if (txtScriptFileName.Text == "")
            {
                MessageBox.Show("Database name should not be blank");
                return false;
            }
            if (txtScriptVersion.Text == "")
            {
                MessageBox.Show("Script version should not be blank");
                return false;
            }
            if (txtMediaVersion.Text == "")
            {
                MessageBox.Show("Script version should not be blank");
                return false;
            }
            if (comProjectStatus.Text == "")
            {
                MessageBox.Show("Project status should be selected");
                return false;
            }
            return true;
        }

        private void refresh()
        {
            comProjectStatus.Items.Clear();
            comProjectStatus.Items.Add("Active");
            comProjectStatus.Items.Add("Close");

            txtProejctCode.Text = "";
            txtProjectName.Text = "";
            txtScriptFileName.Text = "";
            txtScriptVersion.Text = "";
            txtMediaVersion.Text = "";
            dtpStartDate.Text = DateTime.Now.ToShortDateString().ToString();
            sTypeOfOperation = "1";
        }

        private void frmCreateProject_Loaded(object sender, RoutedEventArgs e)
        {
            sTypeOfOperation = "1";
            this.refresh();
            this.loadGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (setData())
                {
                    MyWebRequest myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/createproject.php", "POST", "projectName=" + sProjectName + "&projectId=" + sProjectCode + "&databaseName=" + sDatabaseName + "&scriptVersion=" + sScriptVersion + "&mediaVersion=" + sMediaVersion + "&startDate=" + sStartDate + "&projectStatus=" + sProjectStatus + "&typeOfOperation=" + sTypeOfOperation); //"a=Nasim&b=Rajahshi&c=01911018447&d=1");

                    string temp = myRequest.GetResponse().ToString();
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
                    MessageBox.Show(temp);
                    //}

                    this.refresh();
                    this.loadGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void loadGrid()
        {
            //ProgressBar PB = new ProgressBar();
           await DoWorkAsync();

            //PB.IsIndeterminate = true;
            bLoadGrid = true;
            DataTable dt = getDataFromServer();
            dataGridProject.ItemsSource = dt.DefaultView;
            //PB.IsIndeterminate = false;
        }

        private async Task DoWorkAsync()
        {
            await Task.Run(() =>
            {
                //do some work HERE
                Thread.Sleep(1000);
            });
        }

        private DataTable getDataFromServer()
        {
            //lblExecute.Content = "Execute Now : " + "Download Data";
            //DoEvents();

            WebClient c = new WebClient();
            MyWebRequest myRequest1;
            //if (chkDeletedRec.Checked == false)
            myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/getprojectinfo.php", "POST", "");
            
            //Console.WriteLine(data);
            //JObject o = JObject.Parse(data);
            string data = myRequest1.GetResponse().ToString();

            DataTable dt1_temp;
            
            dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

            return dt1_temp;
        }

        private void dataGridProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bLoadGrid == false)
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView row_selected = (DataRowView)dg.SelectedItem;
                if (row_selected != null)
                {
                    txtProjectName.Text = row_selected[2].ToString();
                    txtProejctCode.Text = row_selected[1].ToString();
                    txtScriptVersion.Text = row_selected[3].ToString();
                    txtMediaVersion.Text = row_selected[4].ToString();
                    txtScriptFileName.Text = row_selected[7].ToString();
                    string status = row_selected[8].ToString();
                    if (status == "1") comProjectStatus.Text = "Active"; else comProjectStatus.Text = "Close";
                    dtpStartDate.Text = row_selected[6].ToString();
                    //txtProjectName.Text = row_selected[2].ToString();
                    //txtProjectName.Text = row_selected[2].ToString();

                    sTypeOfOperation = "2";
                }
            }

            bLoadGrid = false;
        }
    }
}
