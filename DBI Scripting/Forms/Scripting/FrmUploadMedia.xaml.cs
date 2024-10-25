using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
    /// Interaction logic for FrmUploadMedia.xaml
    /// </summary>
    public partial class FrmUploadMedia : Window
    {
        private string myPath;
        private string priorScriptVersion;
        private string fileName;
        private string projectId;

        public FrmUploadMedia()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                openFileDialog1.Filter = "Zip File (*.zip)|*.zip|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtScriptPath.Text = openFileDialog1.FileName;
                    myPath = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));
                    fileName = txtScriptPath.Text.Substring(txtScriptPath.Text.LastIndexOf('\\') + 1);


                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    //this.getScriptVersion();

                }
                else
                    txtScriptPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text == "")
                MessageBox.Show("Script must be selected first.");
            else
            {
                if (!File.Exists(txtScriptPath.Text))
                    MessageBox.Show("Selected file is not valid.");
                else
                {

                    //this.updateScriptVersion();


                    if (!Directory.Exists(myPath + "\\Temp"))
                        Directory.CreateDirectory(myPath + "\\temp");
                    if (!File.Exists(myPath + "\\temp\\" + fileName))
                        File.Copy(txtScriptPath.Text, myPath + "\\temp\\" + fileName);
                    else
                    {
                        File.Delete(myPath + "\\temp\\" + fileName);
                        File.Copy(txtScriptPath.Text, myPath + "\\temp\\" + fileName);
                    }
                    //try
                    //{
                    //if (preparedScript == true)
                    //{
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                    WebClient client = new WebClient();
                    //string myFile = scriptFilePath;
                    string myFile = myPath + "\\temp\\" + fileName;// txtScriptPath.Text;
                    client.Credentials = CredentialCache.DefaultCredentials;
                    //string temp=System.AppDomain.CurrentDomain.BaseDirectory+"//uploadfile.php";
                    //string temp = Properties.Settings.Default.ServerAddress + "//uploadfile.php";
                    //byte[] responseArray = client.UploadFile(Properties.Settings.Default.ServerAddress + "//uploadfile.php", "POST", myFile);
                    byte[] responseArray = client.UploadFile(StaticClass.SERVER_URL + "/deskapi/uploadmedia.php", "POST", myFile);
                    client.Dispose();


                    //MessageBox.Show(client.Encoding.GetString(responseArray));
                    string UploadMessage = client.Encoding.GetString(responseArray).ToString();



                    //Update script version *************************************

                    ////MyWebRequest myRequest = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/updatescriptversion.php", "POST", "projectId=" + projectId + "&scriptVersion=" + txtScriptVersion.Text); //"a=Nasim&b=Rajahshi&c=01911018447&d=1");
                    //MyWebRequest myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/updatescriptversion.php", "POST", "projectId=" + projectId + "&scriptVersion=" + txtScriptVersion.Text); //"a=Nasim&b=Rajahshi&c=01911018447&d=1");

                    //string temp = myRequest.GetResponse().ToString();

                    if (UploadMessage == "Script uploaded successfully..")
                        MessageBox.Show("Script uploaded successfully..");
                    else
                        MessageBox.Show("Opps... Somthing error...");
                    //***********************************************************


                    //}
                    //else
                    //    MessageBox.Show("Need to prepare the script first..");
                    //}
                    //catch (Exception err)
                    //{
                    //    MessageBox.Show(err.Message);
                    //}

                    //<?php
                    //    $filepath = $_FILES["file"]["tmp_name"];
                    //    move_uploaded_file($filepath,"test_file.txt");
                    //?>
                    //MessageBox.Show("Version changed");
                }
            }
        }
    }
}
