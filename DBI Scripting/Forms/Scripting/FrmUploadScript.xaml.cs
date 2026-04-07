using DBI_Scripting.Classes;
using Microsoft.Win32;
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
using System.Web;
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
    /// Interaction logic for FrmUploadScript.xaml
    /// </summary>
    public partial class FrmUploadScript : Window
    {
        private string myPath;
        private string priorScriptVersion;
        private string fileName;
        private string projectId;
        private string fileDirectory;
        private string sSelectedQFile;

        public FrmUploadScript()
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
                openFileDialog1.Filter = "Script File (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtScriptPath.Text = openFileDialog1.FileName;
                    myPath = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));
                    fileName = txtScriptPath.Text.Substring(txtScriptPath.Text.LastIndexOf('\\') + 1);
                    fileDirectory = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                    sSelectedQFile = "";
                    this.getScriptVersion();
                    this.getQfiles();

                }
                else
                    txtScriptPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getScriptVersion()
        {
            if (txtScriptPath.Text != "")
            {
                if (File.Exists(txtScriptPath.Text))
                {
                    ConnectionDB connDB = new ConnectionDB();
                    if (connDB.connect(txtScriptPath.Text) == true)
                    {
                        if (connDB.sqlite_conn.State == ConnectionState.Closed)
                            connDB.sqlite_conn.Open();

                        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_ProjectInfo", connDB.sqlite_conn);
                        DataSet ds = new DataSet();
                        dadpt.Fill(ds, "Table1");
                        if (ds.Tables["Table1"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables["Table1"].Rows)
                            {
                                txtScriptVersion.Text = dr["Version"].ToString();
                                priorScriptVersion = dr["Version"].ToString();
                                txtProjectName.Text = dr["ProjectName"].ToString();
                                projectId = dr["ProjectId"].ToString();
                            }
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

        private void getQfiles()
        {
            string[] fileArray = Directory.GetFiles(fileDirectory, "*.q");

            chkListBoxQFiles.Items.Clear();

            for (int i = 0; i < fileArray.Length; i++)
            {
                chkListBoxQFiles.Items.Add(fileArray[i].Substring(fileArray[i].LastIndexOf('\\') + 1));
            }
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text == "")
            {
                MessageBox.Show("Script must be selected first.");
                return;
            }
            if (sSelectedQFile == "")
            {
                MessageBox.Show("Q file must be selected.");
                return;
            }
            if (!File.Exists(txtScriptPath.Text))
            {
                MessageBox.Show("Selected file is not valid.");
                return;
            }

            ClearOutput();
            SetUIState(true);
            txtStatus.Text = "Uploading, please wait...";

            try
            {
                // Update script version in the local DB first
                this.updateScriptVersion();

                // Copy .db to temp folder
                if (!Directory.Exists(myPath + "\\Temp"))
                    Directory.CreateDirectory(myPath + "\\temp");
                if (!File.Exists(myPath + "\\temp\\" + fileName))
                    File.Copy(txtScriptPath.Text, myPath + "\\temp\\" + fileName);
                else
                {
                    File.Delete(myPath + "\\temp\\" + fileName);
                    File.Copy(txtScriptPath.Text, myPath + "\\temp\\" + fileName);
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Step 1 - Upload .db file
                txtStatus.Text = "Uploading .db file...";
                AppendResult("Uploading database file (" + fileName + ")...", false);

                string uploadMessage;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    byte[] responseArray = await client.UploadFileTaskAsync(
                        StaticClass.SERVER_URL + "/deskapi/uploaddbfile.php", "POST",
                        myPath + "\\temp\\" + fileName);
                    uploadMessage = client.Encoding.GetString(responseArray);
                }

                if (uploadMessage != "Script uploaded successfully..")
                {
                    AppendResult("DB upload failed: " + uploadMessage, true);
                    txtStatus.Text = "Upload failed.";
                    return;
                }
                AppendResult("Database file uploaded successfully.", false);

                // Step 2 - Upload .q file
                txtStatus.Text = "Uploading script file...";
                AppendResult("Uploading script file (" + sSelectedQFile + ")...", false);

                using (WebClient client2 = new WebClient())
                {
                    client2.Credentials = CredentialCache.DefaultCredentials;
                    byte[] responseArray2 = await client2.UploadFileTaskAsync(
                        StaticClass.SERVER_URL + "/deskapi/uploadqfile.php", "POST",
                        myPath + "\\" + sSelectedQFile);
                    uploadMessage = client2.Encoding.GetString(responseArray2);
                }

                if (uploadMessage != "Script uploaded successfully..")
                {
                    AppendResult("Script upload failed: " + uploadMessage, true);
                    txtStatus.Text = "Upload failed.";
                    return;
                }
                AppendResult("Script file uploaded successfully.", false);

                // Step 3 - Update script version on server
                txtStatus.Text = "Updating script version...";
                AppendResult("Updating script version on server...", false);

                MyWebRequest myRequest = new MyWebRequest(
                    StaticClass.SERVER_URL + "/deskapi/updatescriptversion.php",
                    "POST",
                    "projectId=" + projectId +
                    "&scriptVersion=" + txtScriptVersion.Text +
                    "&qFileName=" + sSelectedQFile);

                string versionResponse = myRequest.GetResponse().ToString();

                if (versionResponse == "Record updated successfully\r\n")
                {
                    AppendResult("Script version updated successfully.", false);
                    AppendResult("-------------------------------------------", false);
                    AppendResult("Upload complete.", false);
                    txtStatus.Text = "Upload complete.";
                }
                else
                {
                    AppendResult("Version update failed: " + versionResponse, true);
                    txtStatus.Text = "Upload failed.";
                }
            }
            catch (Exception err)
            {
                AppendResult("Error: " + err.Message, true);
                txtStatus.Text = "Upload failed.";
            }
            finally
            {
                SetUIState(false);
            }
        }

        private void updateScriptVersion()
        {
            try
            {
                ConnectionDB connDB = new ConnectionDB();
                if (connDB.connect(txtScriptPath.Text) == true)
                {
                    if (connDB.sqlite_conn.State == ConnectionState.Closed)
                        connDB.sqlite_conn.Open();



                    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                    command.CommandText = ("UPDATE T_ProjectInfo SET Version='" + txtScriptVersion.Text + "' WHERE ProjectId=" + projectId);
                    command.ExecuteNonQuery();


                    //SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_ProjectInfo", connDB.sqlite_conn);

                    //DataSet ds = new DataSet();
                    //dadpt.Fill(ds, "Table1");
                    //if (ds.Tables["Table1"].Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    //    {
                    //        txtScriptVersion.Text = dr["Version"].ToString();
                    //        priorScriptVersion = dr["Version"].ToString();
                    //        txtProjectName.Text = dr["ProjectName"].ToString();
                    //    }
                    //}

                    if (connDB.sqlite_conn.State == ConnectionState.Open)
                        connDB.sqlite_conn.Close();

                    connDB.sqlite_conn.Dispose();
                    connDB = null;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnUpload_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text == "")
            {
                MessageBox.Show("Script must be selected first.");
                return;
            }

            ClearOutput();
            SetUIState(true);
            txtStatus.Text = "Reading data...";

            // Read all DB data on the UI thread (methods access txtScriptPath.Text)
            List<string> questionData        = getQuestionDBData();
            List<string> attributeData       = getAttributeDBData();
            List<string> attributeFilterData = getAttributeFilterDBData();
            List<string> gridInfoData        = getGridInfoDBData();
            List<string> logicTableData      = getLogicTableDBData();
            List<string> logicAutoData       = getLogicAutoDBData();
            List<string> languageData        = getLanguageDBData();

            bool anyFailure = false;

            try
            {
                await Task.Run(() =>
                {
                    const int MaxRetries  = 3;
                    const int RetryDelay  = 2000; // ms

                    Action<string, bool> append = (text, isError) =>
                        Dispatcher.Invoke(() => AppendResult(text, isError));
                    Action<string> status = text =>
                        Dispatcher.Invoke(() => { txtStatus.Text = text; });

                    // Returns true on success; prints retry messages and returns false after MaxRetries failures
                    Func<string, string, string, int, bool> uploadWithRetry = (url, data, section, batchIdx) =>
                    {
                        for (int attempt = 1; attempt <= MaxRetries; attempt++)
                        {
                            string resp = new MyWebRequest(url, "POST", data).GetResponse().ToString();
                            if (resp == "New record created successfully")
                            {
                                if (attempt > 1)
                                    append("  " + section + " [batch " + batchIdx + "]: OK on retry " + attempt + ".", false);
                                return true;
                            }
                            if (attempt < MaxRetries)
                            {
                                append("  " + section + " [batch " + batchIdx + "]: " + resp + " — retrying (" + attempt + "/" + MaxRetries + ")...", true);
                                Thread.Sleep(RetryDelay);
                            }
                            else
                            {
                                append("  " + section + " [batch " + batchIdx + "]: Failed after " + MaxRetries + " attempts — " + resp, true);
                            }
                        }
                        return false;
                    };

                    int sent, failed;

                    // -- Questions ----------------------------------------------------------
                    status("Uploading Questions...");
                    append("Uploading Questions...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < questionData.Count; x++)
                    {
                        if (questionData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/savequestion.php", questionData[x], "Questions", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Questions: " + sent + " records sent.", false);
                    else             append("  Questions: " + sent + " sent, " + failed + " failed.", true);

                    // -- Attributes ---------------------------------------------------------
                    status("Uploading Attributes...");
                    append("Uploading Attributes...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < attributeData.Count; x++)
                    {
                        if (attributeData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/saveattribute.php", attributeData[x], "Attributes", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Attributes: " + sent + " records sent.", false);
                    else             append("  Attributes: " + sent + " sent, " + failed + " failed.", true);

                    // -- Attribute Filters --------------------------------------------------
                    status("Uploading Attribute Filters...");
                    append("Uploading Attribute Filters...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < attributeFilterData.Count; x++)
                    {
                        if (attributeFilterData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/saveattributefilter.php", attributeFilterData[x], "Attribute Filters", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Attribute Filters: " + sent + " records sent.", false);
                    else             append("  Attribute Filters: " + sent + " sent, " + failed + " failed.", true);

                    // -- Grid Info ----------------------------------------------------------
                    status("Uploading Grid Info...");
                    append("Uploading Grid Info...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < gridInfoData.Count; x++)
                    {
                        if (gridInfoData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/savegridinfo.php", gridInfoData[x], "Grid Info", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Grid Info: " + sent + " records sent.", false);
                    else             append("  Grid Info: " + sent + " sent, " + failed + " failed.", true);

                    // -- Logic Table --------------------------------------------------------
                    status("Uploading Logic Table...");
                    append("Uploading Logic Table...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < logicTableData.Count; x++)
                    {
                        if (logicTableData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/savelogictable.php", logicTableData[x], "Logic Table", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Logic Table: " + sent + " records sent.", false);
                    else             append("  Logic Table: " + sent + " sent, " + failed + " failed.", true);

                    // -- Logic Auto ---------------------------------------------------------
                    status("Uploading Logic Auto...");
                    append("Uploading Logic Auto...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < logicAutoData.Count; x++)
                    {
                        if (logicAutoData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/savelogicauto.php", logicAutoData[x], "Logic Auto", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Logic Auto: " + sent + " records sent.", false);
                    else             append("  Logic Auto: " + sent + " sent, " + failed + " failed.", true);

                    // -- Language -----------------------------------------------------------
                    status("Uploading Language...");
                    append("Uploading Language...", false);
                    sent = 0; failed = 0;
                    for (int x = 0; x < languageData.Count; x++)
                    {
                        if (languageData[x].Length > 15)
                        {
                            if (uploadWithRetry(StaticClass.SERVER_URL + "/deskapi/savelanguage.php", languageData[x], "Language", x + 1))
                                sent++;
                            else { failed++; anyFailure = true; }
                        }
                    }
                    if (failed == 0) append("  Language: " + sent + " records sent.", false);
                    else             append("  Language: " + sent + " sent, " + failed + " failed.", true);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                });

                AppendResult("-------------------------------------------", false);
                if (!anyFailure)
                {
                    AppendResult("Upload complete.", false);
                    txtStatus.Text = "Upload complete.";
                }
                else
                {
                    AppendResult("Upload completed with errors.", true);
                    txtStatus.Text = "Upload completed with errors.";
                }
            }
            catch (Exception err)
            {
                AppendResult("Error: " + err.Message, true);
                txtStatus.Text = "Upload failed.";
            }
            finally
            {
                SetUIState(false);
            }
        }

        private List<string> getQuestionDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_Question WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "qid[]=" + dr[1].ToString().Replace("'", "''") + "&";
                        myData = myData + "question_english[]=" + HttpUtility.UrlEncode(dr[2].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_bengali[]=" + HttpUtility.UrlEncode(dr[3].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_id[]=" + dr[4].ToString() + "&";
                        myData = myData + "comments[]=" + dr[5].ToString().Replace("'", "''") + "&";
                        myData = myData + "qtype[]=" + dr[6].ToString() + "&";
                        myData = myData + "no_of_response_min[]=" + dr[7].ToString() + "&";
                        myData = myData + "no_of_response_max[]=" + dr[8].ToString() + "&";
                        myData = myData + "has_auto_response[]=" + dr[9].ToString() + "&";
                        myData = myData + "has_random_attrib[]=" + dr[10].ToString() + "&";
                        myData = myData + "number_of_column[]=" + dr[11].ToString() + "&";
                        myData = myData + "show_in_report[]=" + dr[12].ToString() + "&";
                        myData = myData + "has_random_qntr[]=" + dr[13].ToString() + "&";
                        myData = myData + "has_message_logic[]=" + dr[14].ToString() + "&";
                        myData = myData + "written_oe_in_paper[]=" + dr[15].ToString() + "&";
                        myData = myData + "force_to_take_oe[]=" + dr[16].ToString() + "&";
                        myData = myData + "has_media_path[]=" + dr[17].ToString() + "&";
                        myData = myData + "display_back_button[]=" + dr[18].ToString() + "&";
                        myData = myData + "display_next_button[]=" + dr[19].ToString() + "&";
                        myData = myData + "display_jump_button[]=" + dr[20].ToString() + "&";
                        myData = myData + "resume_qntr_jump[]=" + dr[21].ToString() + "&";
                        myData = myData + "silent_recording[]=" + dr[22].ToString() + "&";
                        myData = myData + "file_path[]=" + dr[23].ToString() + "&";
                        myData = myData + "order_tag[]=" + dr[24].ToString() + "&";
                        myData = myData + "order_tag1[]=" + dr[25].ToString() + "&";
                        myData = myData + "order_tag2[]=" + dr[26].ToString() + "&";
                        myData = myData + "order_tag3[]=" + dr[27].ToString() + "&";
                        myData = myData + "order_tag4[]=" + dr[28].ToString() + "&";
                        myData = myData + "order_tag5[]=" + dr[29].ToString() + "&";
                        myData = myData + "question_lang3[]=" + HttpUtility.UrlEncode(dr[30].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang4[]=" + HttpUtility.UrlEncode(dr[31].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang5[]=" + HttpUtility.UrlEncode(dr[32].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang6[]=" + HttpUtility.UrlEncode(dr[33].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang7[]=" + HttpUtility.UrlEncode(dr[34].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang8[]=" + HttpUtility.UrlEncode(dr[35].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang9[]=" + HttpUtility.UrlEncode(dr[36].ToString().Replace("'", "''")) + "&";
                        myData = myData + "question_lang10[]=" + HttpUtility.UrlEncode(dr[37].ToString().Replace("'", "''")) + "&";

                        myCounter++;

                        if (myCounter == 20)
                        {
                            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                            myCounter = 0;
                            myData = "";
                            listCounter++;
                        }
                    }
                    if (myData != "")
                        listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;

            }

            return listOfmyData;
        }

        private List<string> getAttributeDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "qid[]=" + dr[1].ToString().Replace("'", "''") + "&";
                        myData = myData + "attribute_english[]=" + HttpUtility.UrlEncode(dr[2].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_bengali[]=" + HttpUtility.UrlEncode(dr[3].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_value[]=" + dr[4].ToString() + "&";
                        myData = myData + "attribute_order[]=" + dr[5].ToString().Replace("'", "''") + "&";
                        myData = myData + "take_openended[]=" + dr[6].ToString() + "&";
                        myData = myData + "is_exclusive[]=" + dr[7].ToString() + "&";
                        myData = myData + "link_id1[]=" + dr[8].ToString() + "&";
                        myData = myData + "link_id2[]=" + dr[9].ToString() + "&";
                        myData = myData + "min_value[]=" + dr[10].ToString() + "&";
                        myData = myData + "max_value[]=" + dr[11].ToString() + "&";
                        myData = myData + "force_and_msg_opt[]=" + dr[12].ToString() + "&";
                        myData = myData + "group_name[]=" + dr[13].ToString() + "&";
                        myData = myData + "filter_qid[]=" + dr[14].ToString() + "&";
                        myData = myData + "filter_type[]=" + dr[15].ToString() + "&";
                        myData = myData + "excep_value[]=" + dr[16].ToString() + "&";
                        myData = myData + "comments[]=" + dr[17].ToString() + "&";
                        myData = myData + "attribute_lang3[]=" + HttpUtility.UrlEncode(dr[18].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang4[]=" + HttpUtility.UrlEncode(dr[19].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang5[]=" + HttpUtility.UrlEncode(dr[20].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang6[]=" + HttpUtility.UrlEncode(dr[21].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang7[]=" + HttpUtility.UrlEncode(dr[22].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang8[]=" + HttpUtility.UrlEncode(dr[23].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang9[]=" + HttpUtility.UrlEncode(dr[24].ToString().Replace("'", "''")) + "&";
                        myData = myData + "attribute_lang10[]=" + HttpUtility.UrlEncode(dr[25].ToString().Replace("'", "''")) + "&";

                        myCounter++;

                        if (myCounter == 20)
                        {
                            if (myData != "")
                            {
                                listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                                myCounter = 0;
                                myData = "";
                                listCounter++;
                            }
                        }
                    }
                    if (myData != "")
                        listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;


            }

            return listOfmyData;
        }

        private List<string> getGridInfoDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_GridInfo WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        if (dr[1].ToString() != "")
                        {
                            myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                            myData = myData + "qid[]=" + dr[1].ToString().Replace("'", "''") + "&";
                            myData = myData + "attribute_english[]=" + HttpUtility.UrlEncode(dr[2].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_bengali[]=" + HttpUtility.UrlEncode(dr[3].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_value[]=" + dr[4].ToString() + "&";
                            myData = myData + "attribute_order[]=" + dr[5].ToString().Replace("'", "''") + "&";
                            myData = myData + "take_openended[]=" + dr[6].ToString() + "&";
                            myData = myData + "is_exclusive[]=" + dr[7].ToString() + "&";
                            myData = myData + "min_value[]=" + dr[8].ToString() + "&";
                            myData = myData + "max_value[]=" + dr[9].ToString() + "&";
                            myData = myData + "force_and_msg_opt[]=" + dr[10].ToString() + "&";
                            myData = myData + "comments[]=" + dr[11].ToString() + "&";
                            myData = myData + "attribute_lang3[]=" + HttpUtility.UrlEncode(dr[12].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang4[]=" + HttpUtility.UrlEncode(dr[13].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang5[]=" + HttpUtility.UrlEncode(dr[14].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang6[]=" + HttpUtility.UrlEncode(dr[15].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang7[]=" + HttpUtility.UrlEncode(dr[16].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang8[]=" + HttpUtility.UrlEncode(dr[17].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang9[]=" + HttpUtility.UrlEncode(dr[18].ToString().Replace("'", "''")) + "&";
                            myData = myData + "attribute_lang10[]=" + HttpUtility.UrlEncode(dr[19].ToString().Replace("'", "''")) + "&";

                            myCounter++;

                            if (myCounter == 20)
                            {
                                listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                                myCounter = 0;
                                myData = "";
                                listCounter++;
                            }
                        }
                    }

                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;


            }

            return listOfmyData;
        }

        private List<string> getLogicTableDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_LogicTable WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    int counter = 1;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "logic_id[]=" + counter.ToString() + "&";
                        myData = myData + "qid[]=" + dr[2].ToString() + "&";
                        myData = myData + "logic_type_id[]=" + dr[3].ToString() + "&";
                        myData = myData + "if_condition[]=" + HttpUtility.UrlEncode(dr[4].ToString()) + "&";
                        myData = myData + "then_value[]=" + dr[5].ToString().Replace("'", "''") + "&";
                        myData = myData + "else_value[]=" + dr[6].ToString().Replace("'", "''") + "&";

                        counter++;
                        myCounter++;

                        if (myCounter == 50)
                        {
                            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                            myCounter = 0;
                            myData = "";
                            listCounter++;
                        }
                    }

                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();


            }

            return listOfmyData;
        }

        private List<string> getLogicAutoDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_LogicAuto WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    int counter = 1;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "logic_id[]=" + counter.ToString() + "&";
                        myData = myData + "qid[]=" + dr[2].ToString() + "&";
                        myData = myData + "logic_type_id[]=" + dr[3].ToString() + "&";
                        myData = myData + "if_condition[]=" + HttpUtility.UrlEncode(dr[4].ToString()) + "&";
                        myData = myData + "then_value[]=" + dr[5].ToString() + "&";
                        myData = myData + "else_value[]=" + dr[6].ToString() + "&";
                        counter++;
                        myCounter++;

                        if (myCounter == 50)
                        {
                            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                            myCounter = 0;
                            myData = "";
                            listCounter++;
                        }
                    }

                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;


            }

            return listOfmyData;
        }

        private List<string> getLanguageDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_LanguageMaster WHERE status='1'", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "language_id[]=" + dr[1].ToString() + "&";
                        myData = myData + "language_name[]=" + dr[2].ToString() + "&";
                        myData = myData + "status[]=" + dr[5].ToString() + "&";
                        myData = myData + "display_order[]=" + dr[6].ToString() + "&";

                        myCounter++;

                        if (myCounter == 50)
                        {
                            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                            myCounter = 0;
                            myData = "";
                            listCounter++;
                        }
                    }

                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;


            }

            return listOfmyData;
        }

        private List<string> getAttributeFilterDBData()
        {
            List<string> listOfmyData = new List<string>();
            string myData = "";

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_OptAttrbFilter WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    int listCounter = 1;
                    int myCounter = 0;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        myData = myData + "project_id[]=" + dr[0].ToString() + "&";
                        myData = myData + "attrib_filter_id[]=" + dr[1].ToString() + "&";
                        myData = myData + "qid[]=" + dr[2].ToString().Replace("'", "''") + "&";
                        myData = myData + "inherited_qid[]=" + dr[3].ToString().Replace("'", "''") + "&";
                        myData = myData + "filter_type[]=" + dr[4].ToString() + "&";
                        myData = myData + "exceptional_value[]=" + dr[5].ToString().Replace("'", "''") + "&";
                        myData = myData + "label_taken_from[]=" + dr[6].ToString().Replace("'", "''") + "&";

                        myCounter++;

                        if (myCounter == 50)
                        {
                            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
                            myCounter = 0;
                            myData = "";
                            listCounter++;
                        }
                    }

                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                connDB.sqlite_conn.Dispose();
                connDB = null;


            }

            return listOfmyData;
        }

        private void chkListBoxQFiles_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {

            if (chkListBoxQFiles.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxQFiles.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxQFiles.SelectedItems.Remove(selecteditem);
            }

            if (chkListBoxQFiles.SelectedItems.Count == 1)
            {
                sSelectedQFile = chkListBoxQFiles.SelectedItems[0].ToString();
            }

        }

        //private List<string> getQuestionDBData()
        //{
        //    List<string> listOfmyData = new List<string>();
        //    string myData = "";

        //    ConnectionDB connDB = new ConnectionDB();
        //    if (connDB.connect(txtScriptPath.Text) == true)
        //    {
        //        if (connDB.sqlite_conn.State == ConnectionState.Closed)
        //            connDB.sqlite_conn.Open();

        //        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_Question WHERE QId!=''", connDB.sqlite_conn);
        //        DataSet ds = new DataSet();
        //        dadpt.Fill(ds, "Table1");
        //        if (ds.Tables["Table1"].Rows.Count > 0)
        //        {
        //            int listCounter = 1;
        //            int myCounter = 0;
        //            foreach (DataRow dr in ds.Tables["Table1"].Rows)
        //            {
        //                myData = myData + "project_id[]=" + dr[0].ToString() + "&";
        //                myData = myData + "qid[]=" + dr[1].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_english[]=" + dr[2].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_bengali[]=" + dr[3].ToString().Replace("'", "''") + "&";
        //                myData = myData + "attribute_id[]=" + dr[4].ToString() + "&";
        //                myData = myData + "comments[]=" + dr[5].ToString().Replace("'", "''") + "&";
        //                myData = myData + "qtype[]=" + dr[6].ToString() + "&";
        //                myData = myData + "no_of_response_min[]=" + dr[7].ToString() + "&";
        //                myData = myData + "no_of_response_max[]=" + dr[8].ToString() + "&";
        //                myData = myData + "has_auto_response[]=" + dr[9].ToString() + "&";
        //                myData = myData + "has_random_attrib[]=" + dr[10].ToString() + "&";
        //                myData = myData + "number_of_column[]=" + dr[11].ToString() + "&";
        //                myData = myData + "show_in_report[]=" + dr[12].ToString() + "&";
        //                myData = myData + "has_random_qntr[]=" + dr[13].ToString() + "&";
        //                myData = myData + "has_message_logic[]=" + dr[14].ToString() + "&";
        //                myData = myData + "written_oe_in_paper[]=" + dr[15].ToString() + "&";
        //                myData = myData + "force_to_take_oe[]=" + dr[16].ToString() + "&";
        //                myData = myData + "has_media_path[]=" + dr[17].ToString() + "&";
        //                myData = myData + "display_back_button[]=" + dr[18].ToString() + "&";
        //                myData = myData + "display_next_button[]=" + dr[19].ToString() + "&";
        //                myData = myData + "display_jump_button[]=" + dr[20].ToString() + "&";
        //                myData = myData + "resume_qntr_jump[]=" + dr[21].ToString() + "&";
        //                myData = myData + "silent_recording[]=" + dr[22].ToString() + "&";
        //                myData = myData + "file_path[]=" + dr[23].ToString() + "&";
        //                myData = myData + "order_tag[]=" + dr[24].ToString() + "&";
        //                myData = myData + "order_tag1[]=" + dr[25].ToString() + "&";
        //                myData = myData + "order_tag2[]=" + dr[26].ToString() + "&";
        //                myData = myData + "order_tag3[]=" + dr[27].ToString() + "&";
        //                myData = myData + "order_tag4[]=" + dr[28].ToString() + "&";
        //                myData = myData + "order_tag5[]=" + dr[29].ToString() + "&";
        //                myData = myData + "question_lang3[]=" + dr[30].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang4[]=" + dr[31].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang5[]=" + dr[32].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang6[]=" + dr[33].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang7[]=" + dr[34].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang8[]=" + dr[35].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang9[]=" + dr[36].ToString().Replace("'", "''") + "&";
        //                myData = myData + "question_lang10[]=" + dr[37].ToString().Replace("'", "''") + "&";

        //                myCounter++;

        //                if (myCounter == 50)
        //                {
        //                    listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());
        //                    myCounter = 0;
        //                    myData = "";
        //                    listCounter++;
        //                }
        //            }

        //            listOfmyData.Add(myData + "listCounter=" + listCounter.ToString());

        //        }

        //        if (connDB.sqlite_conn.State == ConnectionState.Open)
        //            connDB.sqlite_conn.Close();


        //    }

        //    return listOfmyData;
        //}

        // ── UI helpers ────────────────────────────────────────────────────────────────────────

        private void ClearOutput()
        {
            txtUploadResult.Document.Blocks.Clear();
        }

        private void AppendResult(string text, bool isError)
        {
            var para = new Paragraph(new Run(text))
            {
                Foreground = isError ? Brushes.Red : Brushes.DarkGreen,
                Margin = new Thickness(0)
            };
            txtUploadResult.Document.Blocks.Add(para);
            txtUploadResult.ScrollToEnd();
        }

        private void SetUIState(bool running)
        {
            btnUpload.IsEnabled      = !running;
            btnUpload_Copy.IsEnabled = !running;
            btnBrowse.IsEnabled      = !running;
            btnExit.IsEnabled        = !running;
            progressBar.Visibility   = running ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
