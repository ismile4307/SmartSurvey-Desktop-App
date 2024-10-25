using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls.Ribbon;
using DBI_Scripting.Forms;
using DBI_Scripting.Forms.Download;
using System.IO;
using DBI_Scripting.Forms.Scripting;
using DBI_Scripting.Forms.Analytics;
using DBI_Scripting.Forms.Admin;
using System.Net;
using DBI_Scripting.Classes;
using Newtonsoft.Json.Linq;
using System.Data;
using Newtonsoft.Json;
using DBI_Scripting.Forms.WebPortal;

namespace DBI_Scripting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {

        private string id;
        public MainWindow()
        {
            InitializeComponent();

        }


        private void Scripting_Click(object sender, RoutedEventArgs e)
        {
            FrmScripting childWindow = new FrmScripting();
            childWindow.ShowDialog();
            //childWindow.ShowInTaskbar = false;
            //childWindow.Owner = this;
            //childWindow.Show();
            //childWindow.Owner = null;
        }

        private void BtnDownloadData_Click(object sender, RoutedEventArgs e)
        {
            FrmDownloadData frmDownloadData = new FrmDownloadData();
            frmDownloadData.ShowDialog();
        }

        private void btnSettingsServer_Click(object sender, RoutedEventArgs e)
        {
            FrmServerAddress frmServerAddress = new FrmServerAddress();
            frmServerAddress.ShowDialog();
        }

        private void btnAddProject_Click(object sender, RoutedEventArgs e)
        {
            FrmAddProject frmAddProject = new FrmAddProject();
            frmAddProject.ShowDialog();
        }

        private void btnDownloadMedia_Click(object sender, RoutedEventArgs e)
        {
            FrmDownloadMedia frmDownloadMedia = new FrmDownloadMedia();
            frmDownloadMedia.ShowDialog();
        }

        private void frmMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("C:\\Temp"))
                Directory.CreateDirectory("C:\\Temp");

            //MessageBox.Show("Ismile");
            string sTemp;

            sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            if (!File.Exists(sTemp + "\\index.ini"))
            {
                TextWriter txtWriter = new StreamWriter(sTemp + "\\index.ini");
                txtWriter.WriteLine("ismile.hossain@dbibd.com");
                txtWriter.Close();
            }

            StaticClass.SERVER_URL = Properties.Settings.Default.ServerAddress;
            this.Activated += AfterLoading;
            lblServerName.Content = "Server Name : " + StaticClass.SERVER_URL;
        }

        private void btnRejectInterview_Click(object sender, RoutedEventArgs e)
        {
            FrmRejectInterview frmRejectInterview = new FrmRejectInterview();
            frmRejectInterview.ShowDialog();
        }

        private void btnRLD_Click(object sender, RoutedEventArgs e)
        {
            FrmRLDPreparation frmRLDPreparation = new FrmRLDPreparation();
            frmRLDPreparation.ShowDialog();
        }

        private void btnGetQuestionnaire_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBuildScript_Click(object sender, RoutedEventArgs e)
        {
            FrmBuildScript frmBuildScript = new FrmBuildScript();
            frmBuildScript.ShowDialog();
        }

        private void btnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            FrmCreateLoopSyntax frmCreateLoopSyntax = new FrmCreateLoopSyntax();
            frmCreateLoopSyntax.ShowDialog();
            //MessageBox.Show("Please create a prject from web portal");
        }

        private void btnUploadScript_Click(object sender, RoutedEventArgs e)
        {
            FrmUploadScript frmUploadScript = new FrmUploadScript();
            frmUploadScript.ShowDialog();
        }

        private void btnAnalyticsStructure_Click(object sender, RoutedEventArgs e)
        {
            FrmAnalysisStructure frmAnalysisStructure = new FrmAnalysisStructure();
            frmAnalysisStructure.ShowDialog();
        }

        private void btnCreateSPSSSyntax_Click(object sender, RoutedEventArgs e)
        {
            FrmBuildSPSSScript frmBuildSPSSScript = new FrmBuildSPSSScript();
            frmBuildSPSSScript.ShowDialog();
        }

        private void btnCreateOEExcel_Click(object sender, RoutedEventArgs e)
        {
            FrmCreateOEExcel frmCreateOEExcel = new FrmCreateOEExcel();
            frmCreateOEExcel.ShowDialog();
        }

        private void btnDownloadScript_Click(object sender, RoutedEventArgs e)
        {
            FrmDownloadScript frmDownlodScript = new FrmDownloadScript();
            frmDownlodScript.ShowDialog();
        }

        private void btnDummyData_Click(object sender, RoutedEventArgs e)
        {
            FrmDummyData frmDummyData = new FrmDummyData();
            frmDummyData.ShowDialog();
        }

        private void btnUpdateRLD_Click(object sender, RoutedEventArgs e)
        {
            //FrmUpdateRLD frmUpdateRLD = new FrmUpdateRLD();
            //frmUpdateRLD.ShowDialog();

            FrmUpdateLOI frmUpdateLOI = new FrmUpdateLOI();
            frmUpdateLOI.ShowDialog();
        }

        private void btnSPSSLabelToCode_Click(object sender, RoutedEventArgs e)
        {
            FrmLabelToCode frmLabelToCode = new FrmLabelToCode();
            frmLabelToCode.ShowDialog();
        }

        private void btnTranspose_Click(object sender, RoutedEventArgs e)
        {
            FrmTransposeSyntax frmTransposeSyntax = new FrmTransposeSyntax();
            frmTransposeSyntax.ShowDialog();
        }

        private void btnOESyntaxSPSS_Click(object sender, RoutedEventArgs e)
        {
            FrmOESyntaxSPSS frmOESyntaxSPSS = new FrmOESyntaxSPSS();
            frmOESyntaxSPSS.ShowDialog();
        }

        private void btnPlaceholder2_Click(object sender, RoutedEventArgs e)
        {
            FrmTransPlaceholder2 frmTransPlaceholder2 = new FrmTransPlaceholder2();
            frmTransPlaceholder2.ShowDialog();
        }

        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            ChkPostFixNote chkPostFixNotation = new ChkPostFixNote();
            chkPostFixNotation.ShowDialog();
        }

        private void btnCETableLink_Click(object sender, RoutedEventArgs e)
        {
            FrmTableLink2 myFrmTableLink2 = new FrmTableLink2();
            myFrmTableLink2.ShowDialog();
        }

        private void btnTableSyntax_Click(object sender, RoutedEventArgs e)
        {
            FrmAnalysisTable myFrmAnalysisTable = new FrmAnalysisTable();
            myFrmAnalysisTable.ShowDialog();
        }

        private void btnCumulativeSyntax_Click(object sender, RoutedEventArgs e)
        {
            FrmCumulativeSyntax myFrmCumulativeSyntax = new FrmCumulativeSyntax();
            myFrmCumulativeSyntax.ShowDialog();
        }

        private void btnUploadMedia_Click(object sender, RoutedEventArgs e)
        {
            FrmUploadMedia myFrmUploadMedia = new FrmUploadMedia();
            myFrmUploadMedia.ShowDialog();
        }

        private void AfterLoading(object sender, EventArgs e)
        {
            this.Activated -= AfterLoading;
            //Write your code here.

            if (StaticClass.success_check_user() == false)
            {
                FrmLogin myFrmLogin = new FrmLogin();
                myFrmLogin.ShowDialog();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            FrmLogin myFrmLogin = new FrmLogin();
            myFrmLogin.ShowDialog();
        }

        private void btnOEBill_Click(object sender, RoutedEventArgs e)
        {
            FrmOEBill myFrmOEBill = new FrmOEBill();
            myFrmOEBill.ShowDialog();
        }

        //private void btnRemoveMedia_Click(object sender, RoutedEventArgs e)
        //{
        //    FrmRemoveMedia myFrmRemoveMedia = new FrmRemoveMedia();
        //    myFrmRemoveMedia.ShowDialog();
        //}

        private void btnCreateSyntax_Click(object sender, RoutedEventArgs e)
        {
            FrmCreateOESyntax myFrmCreateOESyntax = new FrmCreateOESyntax();
            myFrmCreateOESyntax.ShowDialog();
        }

        private void btnUpdateSyntaxPrep_Click(object sender, RoutedEventArgs e)
        {
            FrmUpdateSyntaxPrep frmUpdateSyntaxPrep = new FrmUpdateSyntaxPrep();
            frmUpdateSyntaxPrep.ShowDialog();
        }

        private void btnSigTest_Click(object sender, RoutedEventArgs e)
        {
            FrmSigTest frmSigTest = new FrmSigTest();
            frmSigTest.ShowDialog();
        }

        private void btnCTableLink_Click(object sender, RoutedEventArgs e)
        {
            FrmCTableLink frmCTableLink = new FrmCTableLink();
            frmCTableLink.ShowDialog();
        }

        private void btnSRTable_Click(object sender, RoutedEventArgs e)
        {
            FrmSRSyntaxPrep frmSRSyntaxPrep = new FrmSRSyntaxPrep();
            frmSRSyntaxPrep.ShowDialog();
        }

        private void btnDbStructure_Click(object sender, RoutedEventArgs e)
        {
            FrmDBStructure frmDBStructure = new FrmDBStructure();
            frmDBStructure.ShowDialog();
        }

        private void btnQntrTable_Click(object sender, RoutedEventArgs e)
        {
            FrmQuestionTable frmQuestionTable = new FrmQuestionTable();
            frmQuestionTable.ShowDialog();
        }

        private void btnAttribute_Click(object sender, RoutedEventArgs e)
        {
            FrmAttributes frmAttributes = new FrmAttributes();
            frmAttributes.ShowDialog();
        }

        private void btnMRTable_Click(object sender, RoutedEventArgs e)
        {
            FrmMRSyntaxPrep frmMRSyntaxPrep = new FrmMRSyntaxPrep();
            frmMRSyntaxPrep.ShowDialog();
        }

        private void BtnDownloadTime_Click(object sender, RoutedEventArgs e)
        {
            FrmDownloadTime frmDownloadTime = new FrmDownloadTime();
            frmDownloadTime.ShowDialog();
        }

        private void btnPrepareAnsDBFromServer_Click(object sender, RoutedEventArgs e)
        {
            FrmPrepareAnsDB frmPrepareAnsDB = new FrmPrepareAnsDB();
            frmPrepareAnsDB.ShowDialog();
        }

        private void btnUnPivotOE_Click(object sender, RoutedEventArgs e)
        {
            FrmUnPivotOE frmUnPivotOE = new FrmUnPivotOE();
            frmUnPivotOE.ShowDialog();
        }

        private void btnOEUnPivot_Click(object sender, RoutedEventArgs e)
        {
            FrmUnPivotOESyntax frmUnPivotOESyntax = new FrmUnPivotOESyntax();
            frmUnPivotOESyntax.ShowDialog();
        }

        private void btnPrepareLoopSyntax_Click(object sender, RoutedEventArgs e)
        {
            FrmCreateLoopSyntax frmCreateLoopSyntax = new FrmCreateLoopSyntax();
            frmCreateLoopSyntax.ShowDialog();
            //MessageBox.Show("Please create a prject from web portal");
        }

        private void btnAddPanelData_Click(object sender, RoutedEventArgs e)
        {
            FrmAddPanelData frmAddPanelData = new FrmAddPanelData();
            frmAddPanelData.ShowDialog();
        }

        private void btnSyncData_Click(object sender, RoutedEventArgs e)
        {
            FrmSyncData frmSyncData = new FrmSyncData();
            frmSyncData.ShowDialog();
        }




    }
}
