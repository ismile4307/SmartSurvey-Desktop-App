using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
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
    /// Interaction logic for FrmDummyData.xaml
    /// </summary>
    public partial class FrmDummyData : Window
    {
        private string myPath;
        private string priorScriptVersion;
        private string fileName;

        ConnectionDB connQntrDB;
        ConnectionDB connAnsDB;

        DBHelper myDBHelper;

        String sProjectId = "";
        String sLatitude = "GPS Location Not Found";
        String sLongitude = "GPS Location Not Found";
        String sStartDateTime = "";
        String sTypeOfInterview = "1";
        String sFICode = "";
        String sDataValidatedBy = "";
        String sAccompaniedBy = "";
        String sBackCheckedBy = "";
        String tabId = "";
        String sScriptVersion = "";
        String language = "1";



        String sRespondentId;
        String sSelectedOrderTag = "OrderTag";


        bool bStatusAutoNext;

        //User Define Class
        InterviewInfo interviewInfo = new InterviewInfo();
        GetData getMyData = new GetData();
        CheckData checkData = new CheckData();

        Miscellaneous myMiscellaneousObj = new Miscellaneous();


        //Question variables
        String sQId;
        String sQuestionEnglish;
        String sQuestionBengali;
        String sAttributeId;
        String sComments;
        String sQType;
        String sNoOfResponseMin;
        String sNoOfResponseMax;
        String sHasAutoResponse;
        String sHasRandomAttribute;
        String sNumberOfColumn;
        String sShowInReport;
        String sHasRandomQntr;
        String sHasMessageLogic;
        String sWrittenOEInPaper;
        String sForceToTakeOE;
        String sHasMediaPath;
        String sDisplayNextButton;
        String sDisplayBackButton;
        String sDisplayJumpButton;
        String sResumeQntrJump;
        String sSilentRecording;
        String sFilePath;

        String sInstruction;
        String sQuestionText;

        int iQuestionOrder;
        int iCurrentQIndex;

        int numberOfColumn = 1;

        private bool bHasFilterAttr, bHasLogicalJump, bHasLogicalCome, bHasCheckCondition, bClose, bHasAutoResponse;
        String sQIdForAttribute;

        List<Attribute> listOfAttribute;
        List<String> listOfRankValue;

        Button btnNext, btnBack;
        ProgressBar spinProgressBar;


        private bool bCaptureImage, bGetImage, bCaptureImageCam;


        public FrmDummyData()
        {
            InitializeComponent();
            myDBHelper = new DBHelper();
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

                    StaticClass.QDBPath = txtScriptPath.Text;
                    StaticClass.ADBPath ="C:\\Temp\\SYSACDB.db";

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    this.getScriptVersion();

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
                    connQntrDB = new ConnectionDB();
                    if (connQntrDB.connect(txtScriptPath.Text) == true)
                    {
                        if (connQntrDB.sqlite_conn.State == ConnectionState.Closed)
                            connQntrDB.sqlite_conn.Open();

                        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_ProjectInfo", connQntrDB.sqlite_conn);
                        DataSet ds = new DataSet();
                        dadpt.Fill(ds, "Table1");
                        if (ds.Tables["Table1"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables["Table1"].Rows)
                            {
                                txtScriptVersion.Text = dr["Version"].ToString();
                                priorScriptVersion = dr["Version"].ToString();
                                txtProjectName.Text = dr["ProjectName"].ToString();
                                sProjectId = dr["ProjectId"].ToString();
                                sScriptVersion = dr["Version"].ToString();
                            }
                        }

                        if (connQntrDB.sqlite_conn.State == ConnectionState.Open)
                            connQntrDB.sqlite_conn.Close();
                    }
                    connQntrDB = null;
                }
                else
                    MessageBox.Show("Invalid script file location");
            }
            else
                MessageBox.Show("Script location should not be blank");
        }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text == "")
                MessageBox.Show("Script must be selected first.");
            else
            {
                if (!File.Exists(txtScriptPath.Text))
                    MessageBox.Show("Selected file is not valid.");
                else
                {
                    if (txtNumberOfData.Text == "")
                        MessageBox.Show("Number of dummy data should be given");
                    else
                    {
                        this.getAnswerDB();

                        int noOfRecord = Convert.ToInt32(txtNumberOfData.Text);

                        for (int i = 0; i < noOfRecord; i++)
                        {
                            String respondenId = insertInterviewInfo();










                        }

                    }

                }
            }
            MessageBox.Show("Successfully prepared " + txtNumberOfData.Text + " dummy data...");
        }

        private void getAnswerDB()
        {
            if (!Directory.Exists(myPath + "\\DummyData"))
                Directory.CreateDirectory(myPath + "\\DummyData");

            if (File.Exists(myPath + "\\DummyData\\SYSACDB.db"))
            {
                File.Delete(myPath + "\\DummyData\\SYSACDB.db");
            }

            string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + "ShellDB\\SYSACDB.db";
            File.Copy(databasePath, myPath + "\\DummyData\\SYSACDB.db");



        }

        private String insertInterviewInfo()
        {
            connAnsDB = new ConnectionDB();
            string sRespondentId = "";
            if (connAnsDB.connect(myPath + "\\DummyData\\SYSACDB.db"))
            {
                if (connAnsDB.sqlite_conn.State == ConnectionState.Closed)
                    connAnsDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd0;
                sqlite_cmd0 = connAnsDB.sqlite_conn.CreateCommand();

                sRespondentId = getRespondentId();

                Random rd = new Random();

                int rand_num = rd.Next(20, 50);

                sqlite_cmd0.CommandText = "INSERT INTO T_InterviewInfo(ProjectId, RespondentId, Latitude, Longitude, SurveyDateTime, SurveyEndTime, " +
                        "LengthOfIntv, Intv_Type, FICode, FSCode, AccompaniedBy, BackCheckedBy, Status, TabId, SyncStatus, ScriptVersion, " +
                        "LanguageId, FieldExtra1, FieldExtra2) "
                        + "VALUES("
                        + sProjectId
                        + ","
                        + sRespondentId
                        + ",'"
                        + sLatitude
                        + "','"
                        + sLongitude
                        + "','"
                        + sStartDateTime
                        + "','"
                        + sStartDateTime
                        + "','"
                        + rand_num.ToString()
                        + "','"
                        + sTypeOfInterview
                        + "','"
                        + sFICode
                        + "','"
                        + sDataValidatedBy
                        + "','"
                        + sAccompaniedBy
                        + "','"
                        + sBackCheckedBy
                        + "','"
                        + "2"
                        + "','"
                        + tabId
                        + "','"
                        + "0"
                        + "','"
                        + sScriptVersion
                        + "','"
                        + language
                        + "','"
                        + ""
                        + "','"
                        + ""
                        + "')";


                sqlite_cmd0.ExecuteNonQuery();

                if (connAnsDB.sqlite_conn.State == ConnectionState.Open)
                    connAnsDB.sqlite_conn.Close();
            }

            return sRespondentId;
        }

        private String getRespondentId()
        {
            String sdfDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");//dd/MM/yyyy
            //Date now = new Date();
            String strDate = sdfDate.Replace('-', ':').Replace(' ', ':').Replace(":", "");
            sStartDateTime = sdfDate.Substring(0, 19);
            return strDate;
        }


        private Boolean getQuestion(int qIndex)
        {
            try
            {
                {
                    connQntrDB = new ConnectionDB();

                    DataTable dt1 = myDBHelper.getQntrTableData("SELECT * FROM T_Question WHERE OrderTag1=" + qIndex, connQntrDB);

                    if (dt1.Rows.Count == 1)
                    {
                        foreach (DataRow dr in dt1.Rows)
                        {
                            txtScriptVersion.Text = dr["Version"].ToString();
                            priorScriptVersion = dr["Version"].ToString();
                            txtProjectName.Text = dr["ProjectName"].ToString();
                            sProjectId = dr["ProjectId"].ToString();
                            sScriptVersion = dr["Version"].ToString();

                            sQId = dr["QId"].ToString();
                            sQuestionEnglish = dr["QuestionEnglish"].ToString();
                            sQuestionBengali = dr["QuestionBengali"].ToString();

                            sAttributeId = dr["AttributeId"].ToString();
                            sComments = dr["Comments"].ToString();

                            sQType = dr["QType"].ToString();
                            sNoOfResponseMin = dr["NoOfResponseMin"].ToString();
                            sNoOfResponseMax = dr["NoOfResponseMax"].ToString();

                            if (sQType == "1")
                            {
                                sNoOfResponseMin = "1";
                                sNoOfResponseMax = "1";
                            }

                            sHasAutoResponse = dr["HasAutoResponse"].ToString();
                            sHasRandomAttribute = dr["HasRandomAttrib"].ToString();
                            sNumberOfColumn = dr["NumberOfColumn"].ToString();
                            sShowInReport = dr["ShowInReport"].ToString();

                            sHasRandomQntr = dr["HasRandomQntr"].ToString();

                            //Don't use in current Application
                            sHasMessageLogic = dr["HasMessageLogic"].ToString();
                            sWrittenOEInPaper = dr["WrittenOEInPaper"].ToString();
                            sForceToTakeOE = dr["ForceToTakeOE"].ToString();
                            sHasMediaPath = dr["HasMediaPath"].ToString();
                            //--------

                            sDisplayBackButton = dr["DisplayBackButton"].ToString();
                            sDisplayNextButton = dr["DisplayNextButton"].ToString();
                            sDisplayJumpButton = dr["DisplayJumpButton"].ToString();

                            sResumeQntrJump = dr["ResumeQntrJump"].ToString();
                            sSilentRecording = dr["SilentRecording"].ToString();
                            iQuestionOrder = Convert.ToInt32(dr["OrderTag1"].ToString());
                            sFilePath = dr["FilePath"].ToString();

                        }
                    }




                    //*****************************************************************


                    interviewInfo.sQId = sQId;

                    //Get number of column for display attribute, If blank column number is 1
                    numberOfColumn = 1;

                    String sQuestionTextTemp = sQuestionEnglish;
                    sQuestionText = sQuestionTextTemp;


                    //set Qid for Attrbitue
                    if (sAttributeId == "")
                        sQIdForAttribute = sQId;
                    else
                        sQIdForAttribute = sAttributeId;

                    //Check there is any attribute filter or not
                    bHasFilterAttr = getMyData.hasFilterAttribute(sProjectId, sQId, connQntrDB);


                    bHasLogicalJump = false;
                    bHasLogicalCome = false;
                    bHasCheckCondition = false;

                    //******************************************************************


                    DataTable dt2 = myDBHelper.getQntrTableData("SELECT * FROM T_LogicTable WHERE Qid='" + sQId + "'", connQntrDB);

                    if (dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in dt2.Rows)
                        {
                            String logic = dr1["LogicTypeId"].ToString();
                            if (logic == "3")
                            {
                                bHasLogicalJump = true;
                            }
                            else if (logic == "4")
                            {
                                bHasLogicalCome = true;
                            }
                            else if (logic == "2")
                            {
                                bHasCheckCondition = true;
                            }
                        }
                    }


                    bHasAutoResponse = false;

                    DataTable dt3 = myDBHelper.getQntrTableData("SELECT * FROM T_LogicAuto WHERE Qid='" + sQId + "'", connQntrDB);

                    if (dt3.Rows.Count > 0)
                    {
                        bHasAutoResponse = true;
                    }


                    //*******************************************************

                    if (bHasLogicalCome)
                    {
                        int tmp = getTergatQIndexForJump("4");
                        if (tmp > 0 && tmp != qIndex)
                            getQuestion(tmp);
                        else if (tmp == 0)
                        {
                            // Include auto response if exist
                            if (bHasAutoResponse == true)
                            {
                                //List<Attribute> listOfAttributeTemp = getMyData.getAttribute(sProjectId, sRespondentId, sQIdForAttribute, sHasRandomAttribute, quesDbAdapter, ansDbAdapter);

                                ////Get data from using logic form logic table
                                //GlobalModule.listOfResponse.Clear();

                                //GlobalModule.listOfResponse = getMyData.getAutoFillResponse(interviewInfo, listOfAttributeTemp, ansDbAdapter, quesDbAdapter);
                                ////Save data
                                //if (GlobalModule.listOfResponse != null)
                                //    saveResponse();
                            }
                            //*******************************
                            getQuestion(qIndex + 1);
                        }
                    }
                    iCurrentQIndex = iQuestionOrder;

                }
                connQntrDB = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n Question Id : " + sQId);
            }
            return false;






            //    String query = "SELECT * FROM T_Question WHERE " + sSelectedOrderTag + "=" + qIndex;
            //    Cursor crs = quesDbAdapter.getData(query);

            //    if (crs.getCount() == 1)
            //    {
            //        crs.moveToNext();


            //        crs.close();
            //        quesDbAdapter.close();









            

            //        //This is just for dummy question
            //        //Here 2 means Get the data using logic and save into database and go next
            //        if (sHasAutoResponse.equals("2"))
            //        {
            //            if (bIsBackTracking == false)
            //            {
            //                ArrayList<Attribute> listOfAttributeTemp = getMyData.getAttribute(sProjectId, sRespondentId, sQIdForAttribute, sHasRandomAttribute, quesDbAdapter, ansDbAdapter);

            //                //Get data from using logic form logic table
            //                GlobalModule.listOfResponse.clear();
            //                //if (sHasAutoResponse.equals("1") && sQType.equals("1")) {
            //                if (sHasRandomAttribute.equals("1"))
            //                {
            //                    ArrayList<Response> listOfFieldDataTemp = getMyData.getAutoFillResponse(interviewInfo, listOfAttributeTemp, ansDbAdapter, quesDbAdapter);
            //                    GlobalModule.listOfResponse = getMyData.getRandomSelectedAttribute(sRespondentId, listOfFieldDataTemp, sNoOfResponseMin, sNoOfResponseMax);
            //                }
            //                else
            //                {
            //                    GlobalModule.listOfResponse = getMyData.getAutoFillResponse(interviewInfo, listOfAttributeTemp, ansDbAdapter, quesDbAdapter);
            //                }
            //                //} else if (sHasAutoResponse.equals("1") && sQType.equals("2")) {
            //                //    GlobalModule.listOfResponse = getMyData.getAutoFillDataTypeResponse(sProjectId, sRespondentId, sQId, ansDbAdapter, quesDbAdapter);
            //                //}

            //                //Save data
            //                if (GlobalModule.listOfResponse != null)
            //                    saveResponse();


            //                //************************** If has logical jump then below code will execute. *****************
            //                //Go for next question
            //                iCurrentQIndex++;
            //                if (bHasLogicalJump)
            //                {
            //                    int tmp = getTergatQIndexForJump("3");
            //                    // showAlert(tmp+" ");
            //                    if (tmp > 0 && iCurrentQIndex - 1 != tmp)
            //                        iCurrentQIndex = tmp;
            //                }
            //                //***********************************************************

            //                getQuestion(iCurrentQIndex);
            //            }
            //            else
            //            {
            //                //onBackPressed();
            //                bIsBackTracking = true;
            //                iCurrentQIndex = getMyData.getPreviousQuesOrder(sProjectId, sRespondentId, iCurrentQIndex, ansDbAdapter);
            //                iPreviousQIndex = getMyData.getPreviousQuesOrder(sProjectId, sRespondentId, iCurrentQIndex, ansDbAdapter);

            //                getQuestion(iCurrentQIndex);
            //                loadFillData();
            //            }

            //        }


            //        bClose = false;
            //        if (sQType.equals("50") || sQType.equals("51"))
            //        {
            //            Button btn = (Button)findViewById(R.id.btnNext);
            //            btn.setBackgroundDrawable(getResources().getDrawable(R.drawable.ic_finish1));
            //            bClose = true;
            //        }
            //        else
            //        {
            //            Button btn = (Button)findViewById(R.id.btnNext);
            //            btn.setBackgroundDrawable(getResources().getDrawable(R.drawable.ic_next1));
            //        }
            //        //TextView tv = (TextView) findViewById(R.id.currentQuesTextView2);
            //        //tv.setText(iCurrentQIndex + "/" + TOTAL_QUESTION);
            //        crs.close();
            //        quesDbAdapter.close();
            //        return true;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "\n Question Id : " + sQId);
            //}
            //return false;
        }



        //Get Tergat question index using logic
        private int getTergatQIndexForJump(String logicType)
        {
            // TODO Auto-generated method stub
            try
            {
                connQntrDB = new ConnectionDB();
                connAnsDB = new ConnectionDB();
                DataTable dt1 = myDBHelper.getQntrTableData("SELECT * from T_logicTable where Qid='" + sQId + "' AND LogicTypeId='" + logicType + "'", connQntrDB);

                foreach (DataRow dr in dt1.Rows)
                {
                    String expression = dr["IfCondition"].ToString();
                    String thenValue = dr["Then"].ToString();
                    String elseValue = dr["Else"].ToString();

                    String query = "";
                    CheckCondition conditionParser = new CheckCondition(connAnsDB, connQntrDB);
                    // boolean tt =
                    // conditionParser.convetToPostFixNotationAndExecute(
                    // "" + projecct_id, s_RId, s_QId, expression);
                    if (conditionParser.convetToPostFixNotationAndExecute(sProjectId, sRespondentId, sQId, expression))
                    {
                        query = "SELECT OrderTag1 FROM T_Question where QId='" + thenValue + "'";

                        DataTable dt2 = myDBHelper.getQntrTableData(query, connQntrDB);
                            foreach (DataRow drx in dt2.Rows)
                            {
                                int qOrderTag = Convert.ToInt32( drx["OrderTag1"].ToString());
                                return qOrderTag;
                            }
                       
                    }
                    else if (elseValue != null && elseValue!="")
                    {
                        query = "SELECT OrderTag1 FROM T_Question where QId='" + elseValue + "'";

                        DataTable dt2 = myDBHelper.getQntrTableData(query, connQntrDB);
                        foreach (DataRow drx in dt2.Rows)
                        {
                            int qOrderTag = Convert.ToInt32(drx["OrderTag1"].ToString());
                            return qOrderTag;
                        }
                    }
                }
                return 0;
            }
            catch (Exception e)
            {   
                MessageBox.Show(e.Message + " in Function getTergatQIndexForJump()");
                return 0;
            }
            return 0;
        }
    }
}
