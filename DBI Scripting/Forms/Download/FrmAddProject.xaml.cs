using DBI_Scripting.Classes;
using Newtonsoft.Json;
using System;
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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmAddProject.xaml
    /// </summary>
    public partial class FrmAddProject : Window
    {
        private String sProjectName;
        private String sProjectCode;
        private String sDatabaseName;
        private String sStartDate;
        private String sProjectStatus;


        private bool bLoadGrid;
        public FrmAddProject()
        {
            InitializeComponent();
        }

        private void FrmAddProject1_Loaded(object sender, RoutedEventArgs e)
        {
            this.refresh();
            this.loadGrid();
        }

        private async void loadGrid()
        {
            //ProgressBar PB = new ProgressBar();
            await DoWorkAsync();

            //PB.IsIndeterminate = true;
            bLoadGrid = true;
            DataTable dt = getDataFromServer();
            if (dt != null)
            {
                dataGridProject.ItemsSource = dt.DefaultView;

                dataGridProject.Columns[0].Width = 50;
                dataGridProject.Columns[1].Width = 200;
                dataGridProject.Columns[2].Width = 100;
                dataGridProject.Columns[3].Width = 150;

                dataGridProject.Columns[4].Width = 120;
                dataGridProject.Columns[5].Width = 100;

                //PB.IsIndeterminate = false;
                //this.dataGridProject.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            }
        }



        //void dataGrid_AutoGeneratingColumn(object sender,DataGridAutoGeneratingColumnEventArgs e)
        //{
        //    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        //}

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
            try
            {
                //lblExecute.Content = "Execute Now : " + "Download Data";
                //DoEvents();

                WebClient c = new WebClient();
                MyWebRequest myRequest1;
                //if (chkDeletedRec.Checked == false)
                myRequest1 = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/getprojectdownload.php", "POST", "");

                //Console.WriteLine(data);
                //JObject o = JObject.Parse(data);
                string data = myRequest1.GetResponse().ToString();

                DataTable dt1_temp;

                dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                return dt1_temp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Connection Error");
                return null;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void refresh()
        {
            comProjectStatus.Items.Clear();
            comProjectStatus.Items.Add("Active");
            comProjectStatus.Items.Add("Close");

            txtProejctCode.Text = "";
            txtProjectName.Text = "";
            txtScriptFileName.Text = "";
            dtpStartDate.Text = DateTime.Now.ToShortDateString().ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (setData())
            {
                MyWebRequest myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/addproject.php", "POST", "projectName=" + sProjectName + "&projectCode=" + sProjectCode + "&databaseName=" + sDatabaseName + "&startDate=" + sStartDate + "&projectStatus=" + sProjectStatus); //"a=Nasim&b=Rajahshi&c=01911018447&d=1");

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
            }
        }

        private bool setData()
        {
            if (checkData())
            {
                sProjectName = txtProjectName.Text.Replace("'", "''");
                sProjectCode = txtProejctCode.Text.Replace("'", "''");
                sDatabaseName = txtScriptFileName.Text.Replace("'", "''");
                sStartDate = dtpStartDate.Text;
                sProjectStatus = comProjectStatus.Text;

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
            if (comProjectStatus.Text == "")
            {
                MessageBox.Show("Project status should be selected");
                return false;
            }
            return true;
        }
    }
}
