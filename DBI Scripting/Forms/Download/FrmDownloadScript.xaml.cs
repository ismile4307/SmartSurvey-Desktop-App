using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
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

namespace DBI_Scripting.Forms.Download
{
    /// <summary>
    /// Interaction logic for FrmDownloadScript.xaml
    /// </summary>
    public partial class FrmDownloadScript : Window
    {
        Dictionary<string, string> dicProjectNameVsDatabaseName;
        private string myPath;

        public FrmDownloadScript()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void chkBaseDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (chkBaseDirectory.IsChecked == true)
                //txtSaveLocation.Text = System.AppDomain.CurrentDomain.BaseDirectory.Substring(0,System.AppDomain.CurrentDomain.BaseDirectory.Length-1);
                txtSaveLocation.Text=@"C:\Temp\";
            else
                txtSaveLocation.Text = "";
        }

        private void frmDownloadScript_Loaded(object sender, RoutedEventArgs e)
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
                openFileDialog1.Filter = "All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    string tempPath = openFileDialog1.FileName;
                    myPath = tempPath.Substring(0, tempPath.LastIndexOf('\\'));
                    txtSaveLocation.Text = myPath;

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    chkBaseDirectory.IsChecked = false;
                }
                else
                    txtSaveLocation.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (txtSaveLocation.Text != "")
            {
                if (Directory.Exists(txtSaveLocation.Text))
                {
                    if (comProjectName.Text != "")
                    {
                        try
                        {
                            //lblDownloadStatus.Content = "Downloaded : Script Database";
                            //DoEvents();

                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                            using (WebClient client = new WebClient())
                            {
                                //string source = Properties.Settings.Default.ServerAddress + "/" + dicProjectNameVsDatabaseName[comProjectName.Text];
                                string source = StaticClass.SERVER_URL + "/scripts/" + dicProjectNameVsDatabaseName[comProjectName.Text];
                                //string temp2 = txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp";
                                string destination = txtSaveLocation.Text + "\\" + dicProjectNameVsDatabaseName[comProjectName.Text];
                                //if (!File.Exists(destination))
                                    //client.DownloadFile(serverPath + "/audio/" + lstRespondentId[i]["RespondentId"] + ".3gp", txtSaveLocation.Text + "\\" + lstRespondentId[i]["Region"] + "_" + lstRespondentId[i]["RespondentId"] + ".3gp");
                                    client.DownloadFile(source, destination);
                            }
                            MessageBox.Show("Script downloaded successfully");
                        }
                        catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/}
                    }
                    else
                        MessageBox.Show("Need to select a project");
                }
                else
                    MessageBox.Show("Invalid directory selected");
            }
            else
                MessageBox.Show("Save locatoin must be selected");
        }
    }
}
