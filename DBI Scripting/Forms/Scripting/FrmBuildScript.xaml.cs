using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for FrmBuildScript.xaml
    /// </summary>
    public partial class FrmBuildScript : Window
    {
        #region Define global variables
        private String serverAddress = "https://capiapi.surveyhivebd.xyz";
        private String myPath;

        ProjectInfoScript projectInfoScript;
        Dictionary<String, List<AttributeMain>> dicListNameVsList;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeList;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan1;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan2;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan3;

        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan4;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan5;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan6;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan7;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan8;
        Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan9;

        List<Question> listOfQuestion;
        List<Question> listOfQuestionLan1;
        List<Question> listOfQuestionLan2;
        List<Question> listOfQuestionLan3;

        List<Question> listOfQuestionLan4;
        List<Question> listOfQuestionLan5;
        List<Question> listOfQuestionLan6;
        List<Question> listOfQuestionLan7;
        List<Question> listOfQuestionLan8;
        List<Question> listOfQuestionLan9;

        List<AttributeFilter> listOfAttributeFilter;
        List<AutoResponse> listOfAutoResponse;
        List<LogicalSyntax> listOfLogicalSyntax;
        Question currentQuestion;
        Question currentQuestionLan1;
        Question currentQuestionLan2;
        Question currentQuestionLan3;

        Question currentQuestionLan4;
        Question currentQuestionLan5;
        Question currentQuestionLan6;
        Question currentQuestionLan7;
        Question currentQuestionLan8;
        Question currentQuestionLan9;


        Dictionary<String, List<GridInfo>> dicGridListNameVsList;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan1;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan2;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan3;

        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan4;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan5;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan6;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan7;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan8;
        Dictionary<String, List<GridInfo>> dicGridListNameVsListLan9;
        List<String> listOfLanguage;

        List<String> listOfKeyWords;
        List<string> listOfQuestionIdForReject;

        Boolean preparedScript = false;
        String scriptFilePath = "";

        String silentRecording = "";

        CheckLogicalExp checkLogicalExp;


        private bool hasEnd = false;
        private bool hasTerminate = false;
        #endregion

        public FrmBuildScript()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            ClearBuildOutput();
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Script File (*.q)|*.q|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtScriptPath.Text = openFileDialog1.FileName;
                    myPath = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtScriptPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            ClearBuildOutput();
            SetUIState(running: true);
            await Task.Yield(); // let UI refresh before synchronous build starts
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
            #region Define local variables
            checkLogicalExp = new CheckLogicalExp();


            projectInfoScript = new ProjectInfoScript();
            dicListNameVsList = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeList = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan1 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan2 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan3 = new Dictionary<string, List<AttributeMain>>();

            dicQidVsAttributeListLan4 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan5 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan6 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan7 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan8 = new Dictionary<string, List<AttributeMain>>();
            dicQidVsAttributeListLan9 = new Dictionary<string, List<AttributeMain>>();

            listOfQuestion = new List<Question>();
            listOfQuestionLan1 = new List<Question>();
            listOfQuestionLan2 = new List<Question>();
            listOfQuestionLan3 = new List<Question>();

            listOfQuestionLan4 = new List<Question>();
            listOfQuestionLan5 = new List<Question>();
            listOfQuestionLan6 = new List<Question>();
            listOfQuestionLan7 = new List<Question>();
            listOfQuestionLan8 = new List<Question>();
            listOfQuestionLan9 = new List<Question>();

            listOfAttributeFilter = new List<AttributeFilter>();
            listOfAutoResponse = new List<AutoResponse>();
            listOfLogicalSyntax = new List<LogicalSyntax>();
            dicGridListNameVsList = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan1 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan2 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan3 = new Dictionary<string, List<GridInfo>>();

            dicGridListNameVsListLan4 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan5 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan6 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan7 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan8 = new Dictionary<string, List<GridInfo>>();
            dicGridListNameVsListLan9 = new Dictionary<string, List<GridInfo>>();

            listOfLanguage = new List<String>();

            List<string> listOfQuestionIdForDupliCheck = new List<string>();
            List<string> listOfGridListForDupliCheck = new List<string>();



            List<string> listOfQuestionIdForDupliCheckLan2 = new List<string>();
            List<string> listOfGridListForDupliCheckLan2 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan3 = new List<string>();
            List<string> listOfGridListForDupliCheckLan3 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan4 = new List<string>();
            List<string> listOfGridListForDupliCheckLan4 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan5 = new List<string>();
            List<string> listOfGridListForDupliCheckLan5 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan6 = new List<string>();
            List<string> listOfGridListForDupliCheckLan6 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan7 = new List<string>();
            List<string> listOfGridListForDupliCheckLan7 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan8 = new List<string>();
            List<string> listOfGridListForDupliCheckLan8 = new List<string>();

            List<string> listOfQuestionIdForDupliCheckLan9 = new List<string>();
            List<string> listOfGridListForDupliCheckLan9 = new List<string>();


            listOfQuestionIdForReject = new List<string>();
            listOfQuestionIdForReject.Add("UNION");
            listOfQuestionIdForReject.Add("ABS");
            listOfQuestionIdForReject.Add("JOIN");
            listOfQuestionIdForReject.Add("SELECT");
            listOfQuestionIdForReject.Add("INTO");
            listOfQuestionIdForReject.Add("WHERE");
            listOfQuestionIdForReject.Add("IF");
            listOfQuestionIdForReject.Add("EXISTS");
            listOfQuestionIdForReject.Add("ORDER");
            listOfQuestionIdForReject.Add("BY");
            listOfQuestionIdForReject.Add("UPDATE");
            listOfQuestionIdForReject.Add("DELETE");
            listOfQuestionIdForReject.Add("MAX");
            listOfQuestionIdForReject.Add("MIN");

            bool hasDKCS = false;
            bool hasFIFS = false;
            bool hasSingleDropdown = false;
            preparedScript = false;

            // take all syntax in a list due to have facility to back tracking
            List<String> lines = new List<string>();
            List<String> linesLanguage1 = new List<string>();
            List<String> linesLanguage2 = new List<string>();
            List<String> linesLanguage3 = new List<string>();


            List<String> linesLanguage4 = new List<string>();
            List<String> linesLanguage5 = new List<string>();
            List<String> linesLanguage6 = new List<string>();
            List<String> linesLanguage7 = new List<string>();
            List<String> linesLanguage8 = new List<string>();
            List<String> linesLanguage9 = new List<string>();
            #endregion

            #region Reading the script line by line
            TextReader txtReader = new StreamReader(txtScriptPath.Text);
            String strline = txtReader.ReadLine();
            Dictionary<int, int> dicLine = new Dictionary<int, int>();

            TextWriter txtWriter = new StreamWriter(myPath + "\\BuildResult.txt");


            int a = 0;
            int b = 0;
            while (strline != null)
            {
                if (strline.ToUpper().Contains("@LANGUAGE"))
                {
                    string[] langArray = strline.Split('"');
                    if (langArray.Length == 3)
                        listOfLanguage.Add(langArray[1]);
                    else
                        MessageBox.Show("Invalid @LANGUAGE Syntax");

                    break;
                }
                else
                {
                    b++;
                    if (strline.Trim() != "" && strline.Substring(0, 1) != "#" && strline.Substring(0, 1) != "$")
                    {
                        a++;
                        lines.Add(Regex.Replace(strline.Trim(), @"\s+", " "));
                        dicLine.Add(a, b);
                    }
                }
                strline = txtReader.ReadLine();

            }

            // Languages 1-9 — read each section using unified helper
            ReadLanguageSection(txtReader, linesLanguage1, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage2, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage3, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage4, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage5, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage6, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage7, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage8, ref a, ref b, dicLine);
            ReadLanguageSection(txtReader, linesLanguage9, ref a, ref b, dicLine);


            txtReader.Close();
            #endregion

            #region Prepare English Question

            int j = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                strline = lines[i];

                #region Script Info

                if (strline.ToUpper().Contains("PROJECT NAME") && strline.Contains(":"))
                {
                    projectInfoScript.ProjectName = strline.Split(':')[1].Trim();
                    goto next;
                }
                else if (strline.ToUpper().Contains("PROJECT CODE") && strline.Contains(":"))
                {
                    projectInfoScript.ProjectCode = strline.Split(':')[1].Trim();
                    goto next;
                }
                else if (strline.ToUpper().Contains("SCRIPT VERSION") && strline.Contains(":"))
                {
                    projectInfoScript.ScriptVersion = strline.Split(':')[1].Trim();
                    goto next;
                }
                else if (strline.ToUpper().Contains("SCRIPT NAME") && strline.Contains(":"))
                {
                    projectInfoScript.DatabaseName = strline.Split(':')[1].Trim();
                    goto next;
                }
                else if (strline.ToUpper().Contains("SCRIPTED BY") && strline.Contains(":"))
                {
                    projectInfoScript.ScriptedBy = strline.Split(':')[1].Trim();
                    goto next;
                }

                if (projectInfoScript.ProjectName == null)
                    txtWriter.WriteLine("Line : 3 Project Name Missing");
                if (projectInfoScript.ProjectCode == null)
                    txtWriter.WriteLine("Line : 4 Project Code Missing");
                if (projectInfoScript.ScriptVersion == null)
                    txtWriter.WriteLine("Line : 5 Script Version Missing");
                if (projectInfoScript.DatabaseName == null)
                    txtWriter.WriteLine("Line : 6 Script Name Missing");
                if (projectInfoScript.ScriptedBy == null)
                    txtWriter.WriteLine("Line : 8 Scripted by name Missing");

                #endregion

                if (strline.Substring(0, 1) == "*")
                {
                    #region Prepare LIST
                    if (strline.Split(' ')[0].ToUpper() == "*LIST")
                    {
                        //PreparList function is called
                        i = this.prepareList(lines, i, txtWriter, dicLine);
                        strline = lines[i];

                    }
                    #endregion

                    #region Prepare GRIDLIST
                    if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                    {
                        i = this.prepareGridList(lines, i, listOfGridListForDupliCheck, txtWriter, dicLine);
                        strline = lines[i];
                    }
                    #endregion

                    #region Prepare IF
                    //if (strline.Trim().Split(' ')[0].ToUpper() == "*IF" && !strline.ToUpper().Contains("REGULAREXPOF"))
                    if (strline.Trim().Split(' ')[0].ToUpper() == "*IF")
                    {
                        List<AutoResponse> listOfAutoResponseTemp = new List<AutoResponse>();
                        List<LogicalSyntax> listOfLogicalSyntaxTemp = new List<LogicalSyntax>();

                        i = this.prepareIf(lines, i, listOfQuestionIdForDupliCheck, listOfAutoResponseTemp, listOfLogicalSyntaxTemp, txtWriter, dicLine);
                        strline = lines[i];

                        for (int x = 0; x < listOfAutoResponseTemp.Count; x++)
                        {
                            listOfAutoResponse.Add(listOfAutoResponseTemp[x]);
                        }

                        for (int x = 0; x < listOfLogicalSyntaxTemp.Count; x++)
                        {
                            listOfLogicalSyntax.Add(listOfLogicalSyntaxTemp[x]);
                        }


                    }
                    #endregion

                    #region Prepare INCLUDE && EXCLUDE
                    if (strline.Trim().Split(' ')[0].Trim().ToUpper() == "*INCLUDE" || strline.Trim().Split(' ')[0].Trim().ToUpper() == "*EXCLUDE")
                    {
                        List<AutoResponse> listOfAutoResponseTemp = new List<AutoResponse>();

                        i = this.prepareIncludeExclude(lines, i, listOfQuestionIdForDupliCheck, listOfAutoResponseTemp, txtWriter, dicLine);
                        strline = lines[i];

                        for (int x = 0; x < listOfAutoResponseTemp.Count; x++)
                        {
                            listOfAutoResponse.Add(listOfAutoResponseTemp[x]);
                        }
                    }
                    #endregion

                    #region RECORDING
                    if (strline.Trim().Split(' ')[0].ToUpper() == "*STARTREC")
                    {
                        string[] xyz = strline.Split('"');
                        if (xyz.Length != 3)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
                        }
                        else
                        {
                            silentRecording = xyz[1];
                        }
                    }
                    if (strline.Trim().Split(' ')[0].ToUpper() == "*ENDREC")
                    {
                        if (strline.Trim().Length != 7)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
                        }
                        else
                        {
                            silentRecording = "";
                        }
                    }
                    #endregion

                    #region Prepare REPEAT BLOCK
                    if (strline.Trim().Split(' ')[0].ToUpper() == "*REPEAT")
                    {
                        int bStart = strline.IndexOf('[');
                        int bEnd   = strline.IndexOf(']');
                        if (bStart < 0 || bEnd <= bStart)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " *REPEAT syntax invalid — missing [source]");
                            i++;
                            while (i < lines.Count && lines[i].Trim().Split(' ')[0].ToUpper() != "*ENDREPEAT") i++;
                            goto next;
                        }
                        string repeatSource = strline.Substring(bStart + 1, bEnd - bStart - 1).Trim();

                        // collect buffer until *ENDREPEAT
                        List<string> repeatBuffer = new List<string>();
                        bool foundEndRepeat = false;
                        i++;
                        while (i < lines.Count)
                        {
                            if (lines[i].Trim().Split(' ')[0].ToUpper() == "*ENDREPEAT")
                            { foundEndRepeat = true; break; }
                            repeatBuffer.Add(lines[i]);
                            i++;
                        }
                        if (!foundEndRepeat)
                        {
                            txtWriter.WriteLine("*REPEAT block not closed with *ENDREPEAT");
                            goto next;
                        }

                        // build iteration list and expand
                        List<string> iterationList = BuildRepeatIterationList(repeatSource, txtWriter, dicLine[i + 1]);
                        if (iterationList != null && iterationList.Count > 0)
                            ExpandRepeatBlockEnglish(repeatBuffer, iterationList,
                                listOfQuestionIdForDupliCheck, listOfGridListForDupliCheck, txtWriter);

                        strline = lines[i]; // i → *ENDREPEAT line
                    }
                    #endregion

                    #region Prepare QUESTION
                    if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                    {
                        //Pronab added for repeat
                        List<AttributeMain> listOfAttributeTemp = new List<AttributeMain>();
                        bool hasRepeat = false;
                        string[] word = strline.Split('*');
                        for (int n = 1; n < word.Length; n++)
                        {
                            string myText = "*" + word[n];

                            if (myText.ToUpper().Trim().Contains("*REPEAT"))
                            {
                                string[] xyz = word[n].Trim().Split(' ');
                                if (xyz.Length == 2)
                                {
                                    string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                                    {
                                        listOfAttributeTemp = dicQidVsAttributeList[abc[1].Trim()];
                                    }
                                }
                                hasRepeat = true;
                            }
                            else
                            {
                                hasRepeat = false;
                            }
                        }

                        if (hasRepeat)
                        {
                            int iStart = i;
                            for (int k = 0; k < listOfAttributeTemp.Count; k++)
                            {
                                if (listOfAttributeTemp[k].AttributeEnglish.Contains("None"))
                                    break;

                                AttributeMain attributeMainR = new AttributeMain();
                                attributeMainR.AttributeEnglish = listOfAttributeTemp[k].AttributeEnglish;
                                attributeMainR.AttributeValue = listOfAttributeTemp[k].AttributeValue;

                                currentQuestion = new Question();

                                List<LogicalSyntax> listOfLogicalSyntaxTemp = new List<LogicalSyntax>();
                                List<Question> listOfQuestionTemp = new List<Question>();
                                Question currentQuestionTemp = new Question();

                                Dictionary<String, List<AttributeMain>> dicQidVsAttributeListTemp = new Dictionary<String, List<AttributeMain>>();
                                List<AttributeFilter> listOfAttributeFilterTemp = new List<AttributeFilter>();

                                i = iStart;

                                i = this.prepareQuestion(lines, i, listOfQuestionIdForDupliCheck, listOfGridListForDupliCheck, listOfLogicalSyntaxTemp, listOfQuestionTemp, currentQuestionTemp, dicQidVsAttributeListTemp, listOfAttributeFilterTemp, txtWriter, dicLine, attributeMainR);
                                strline = lines[i];

                                for (int x = 0; x < listOfLogicalSyntaxTemp.Count; x++)
                                {
                                    listOfLogicalSyntax.Add(listOfLogicalSyntaxTemp[x]);
                                }

                                for (int x = 0; x < listOfQuestionTemp.Count; x++)
                                {
                                    listOfQuestion.Add(listOfQuestionTemp[x]);
                                }
                                currentQuestion = listOfQuestionTemp[0];
                                //currentQuestion = currentQuestionTemp;

                                foreach (KeyValuePair<String, List<AttributeMain>> pair in dicQidVsAttributeListTemp)
                                {
                                    dicQidVsAttributeList.Add(pair.Key, pair.Value);
                                }


                                for (int x = 0; x < listOfAttributeFilterTemp.Count; x++)
                                {
                                    listOfAttributeFilter.Add(listOfAttributeFilterTemp[x]);
                                }
                            }
                        } //Pronab end End Repeat
                        else
                        {
                            currentQuestion = new Question();

                            List<LogicalSyntax> listOfLogicalSyntaxTemp = new List<LogicalSyntax>();
                            List<Question> listOfQuestionTemp = new List<Question>();
                            Question currentQuestionTemp = new Question();

                            Dictionary<String, List<AttributeMain>> dicQidVsAttributeListTemp = new Dictionary<String, List<AttributeMain>>();
                            List<AttributeFilter> listOfAttributeFilterTemp = new List<AttributeFilter>();

                            i = this.prepareQuestion(lines, i, listOfQuestionIdForDupliCheck, listOfGridListForDupliCheck, listOfLogicalSyntaxTemp, listOfQuestionTemp, currentQuestionTemp, dicQidVsAttributeListTemp, listOfAttributeFilterTemp, txtWriter, dicLine);
                            strline = lines[i];

                            for (int x = 0; x < listOfLogicalSyntaxTemp.Count; x++)
                            {
                                listOfLogicalSyntax.Add(listOfLogicalSyntaxTemp[x]);
                            }

                            for (int x = 0; x < listOfQuestionTemp.Count; x++)
                            {
                                listOfQuestion.Add(listOfQuestionTemp[x]);
                            }
                            currentQuestion = listOfQuestionTemp[0];
                            //currentQuestion = currentQuestionTemp;

                            foreach (KeyValuePair<String, List<AttributeMain>> pair in dicQidVsAttributeListTemp)
                            {
                                dicQidVsAttributeList.Add(pair.Key, pair.Value);
                            }


                            for (int x = 0; x < listOfAttributeFilterTemp.Count; x++)
                            {
                                listOfAttributeFilter.Add(listOfAttributeFilterTemp[x]);
                            }
                        }
                    }
                    #endregion

                }
            next:
                j++;
            }

            // ****************** Check mandatory qid ***********************************
            if (!listOfQuestionIdForDupliCheck.Contains("RespName"))
                txtWriter.WriteLine("RespName question is missing..");
            if (!listOfQuestionIdForDupliCheck.Contains("RespMobile"))
                txtWriter.WriteLine("RespMobile question is missing..");
            if (!listOfQuestionIdForDupliCheck.Contains("Centre"))
                txtWriter.WriteLine("Centre question is missing..");
            if (!listOfQuestionIdForDupliCheck.Contains("FIFSInfo"))
                txtWriter.WriteLine("FIFSInfo question is missing..");

            #endregion

            #region Prepare Language1 Question

            if (linesLanguage1.Count > 0)
            {
                int ln1 = lines.Count + 1;
                j = 0;
                for (int i = 0; i < linesLanguage1.Count; i++)
                {
                    strline = linesLanguage1[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage1, i, txtWriter, dicLine, ln1, 1);
                            strline = linesLanguage1[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage1, i, txtWriter, dicLine, ln1, 1);
                            strline = linesLanguage1[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            i = this.prepareQuestionForLanguage(linesLanguage1, i, txtWriter, dicLine, ln1, 1);
                        }
                        #endregion

                        #region Prepare REPEAT BLOCK
                        if (strline.Trim().Split(' ')[0].ToUpper() == "*REPEAT")
                        {
                            int bStart = strline.IndexOf('[');
                            int bEnd   = strline.IndexOf(']');
                            if (bStart >= 0 && bEnd > bStart)
                            {
                                string repeatSource = strline.Substring(bStart + 1, bEnd - bStart - 1).Trim();
                                List<string> repeatBuffer = new List<string>();
                                bool foundEnd = false;
                                i++;
                                while (i < linesLanguage1.Count)
                                {
                                    if (linesLanguage1[i].Trim().Split(' ')[0].ToUpper() == "*ENDREPEAT")
                                    { foundEnd = true; break; }
                                    repeatBuffer.Add(linesLanguage1[i]);
                                    i++;
                                }
                                if (!foundEnd)
                                    txtWriter.WriteLine("*REPEAT block in Language 1 not closed with *ENDREPEAT");
                                else
                                {
                                    List<string> iterList = BuildRepeatIterationList(repeatSource, txtWriter, ln1);
                                    if (iterList != null && iterList.Count > 0)
                                        ExpandRepeatBlockLanguage(repeatBuffer, iterList, 1, txtWriter);
                                }
                            }
                            else
                                txtWriter.WriteLine("*REPEAT syntax invalid in Language 1 — missing [source]");
                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language2 Question

            if (linesLanguage2.Count > 0)
            {
                int ln2 = lines.Count + linesLanguage1.Count + 2;

                j = 0;
                for (int i = 0; i < linesLanguage2.Count; i++)
                {
                    strline = linesLanguage2[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage2, i, txtWriter, dicLine, ln2, 2);
                            strline = linesLanguage2[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage2, i, txtWriter, dicLine, ln2, 2);
                            strline = linesLanguage2[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            i = this.prepareQuestionForLanguage(linesLanguage2, i, txtWriter, dicLine, ln2, 2);
                        }
                        #endregion

                        #region Prepare REPEAT BLOCK
                        if (strline.Trim().Split(' ')[0].ToUpper() == "*REPEAT")
                        {
                            int bStart = strline.IndexOf('[');
                            int bEnd   = strline.IndexOf(']');
                            if (bStart >= 0 && bEnd > bStart)
                            {
                                string repeatSource = strline.Substring(bStart + 1, bEnd - bStart - 1).Trim();
                                List<string> repeatBuffer = new List<string>();
                                bool foundEnd = false;
                                i++;
                                while (i < linesLanguage2.Count)
                                {
                                    if (linesLanguage2[i].Trim().Split(' ')[0].ToUpper() == "*ENDREPEAT")
                                    { foundEnd = true; break; }
                                    repeatBuffer.Add(linesLanguage2[i]);
                                    i++;
                                }
                                if (!foundEnd)
                                    txtWriter.WriteLine("*REPEAT block in Language 2 not closed with *ENDREPEAT");
                                else
                                {
                                    List<string> iterList = BuildRepeatIterationList(repeatSource, txtWriter, ln2);
                                    if (iterList != null && iterList.Count > 0)
                                        ExpandRepeatBlockLanguage(repeatBuffer, iterList, 2, txtWriter);
                                }
                            }
                            else
                                txtWriter.WriteLine("*REPEAT syntax invalid in Language 2 — missing [source]");
                        }
                        #endregion

                    }
                    //next:
                    j++;
                }


            }
            #endregion

            #region Prepare Language3 Question

            if (linesLanguage3.Count > 0)
            {
                int ln3 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + 3;
                j = 0;
                for (int i = 0; i < linesLanguage3.Count; i++)
                {
                    strline = linesLanguage3[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage3, i, txtWriter, dicLine, ln3, 3);
                            strline = linesLanguage3[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage3, i, txtWriter, dicLine, ln3, 3);
                            strline = linesLanguage3[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan3 = new Question();
                            Question myQuestionLan3 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan3 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan3.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan3.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan3.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan3.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan3.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan3.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan3.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan3.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage3[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage3[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan3.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan3.Add(myQuestionLan3);
                            currentQuestionLan3 = myQuestionLan3;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan3 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan3 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan3 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan3.Add(attributeMain1);
                                listOfattributeMainLan3.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan3.Add(attributeMainFIName);
                                listOfattributeMainLan3.Add(attributeMainFICode);
                                listOfattributeMainLan3.Add(attributeMainFSName);
                                listOfattributeMainLan3.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan3.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan3.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan3[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan3.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan3 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan3.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan3.Add(attributeMainLan3);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan3.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan3.Add(listOfattributeMainLan3[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan3.Add(listOfattributeMainLan3[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage3[++i];
                                strline = linesLanguage3[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan3.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan3.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan3[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan3.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan3.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan3.Add(listOfattributeMainLan3[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan3.Add(listOfattributeMainLan3[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage3[++i];
                                //strline = linesLanguage3[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan3 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan3.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan3.AttributeValue = myWord[0].Trim();
                                                attributeMainLan3.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan3.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan3.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan3.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan3 != "")
                                            //    {

                                            //        attributeMainLan3.LinkId1 = "1";
                                            //        attributeMainLan3.LinkId2 = currentGridListNameLan3;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan3.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan3 != "")
                                            //    {

                                            //        attributeMainLan3.LinkId1 = "2";
                                            //        attributeMainLan3.LinkId2 = currentGridListNameLan3;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan3.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan3.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan3 != "")
                                            //    {
                                            //        attributeMainLan3.LinkId1 = "1";
                                            //        attributeMainLan3.LinkId2 = currentGridListNameLan3;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan3.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan3 != "")
                                            //    {
                                            //        attributeMainLan3.LinkId1 = "2";
                                            //        attributeMainLan3.LinkId2 = currentGridListNameLan3;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan3.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan3.Add(attributeMainLan3);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage3[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan3.QId != null)
                                dicQidVsAttributeListLan3.Add(myQuestionLan3.QId, listOfattributeMainLan3);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln3 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language4 Question

            if (linesLanguage4.Count > 0)
            {
                int ln4 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + 4;

                j = 0;
                for (int i = 0; i < linesLanguage4.Count; i++)
                {
                    strline = linesLanguage4[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage4, i, txtWriter, dicLine, ln4, 4);
                            strline = linesLanguage4[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage4, i, txtWriter, dicLine, ln4, 4);
                            strline = linesLanguage4[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan4 = new Question();
                            Question myQuestionLan4 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan4 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan4.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan4.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan4.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan4.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan4.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan4.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan4.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan4.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage4[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage4[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan4.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan4.Add(myQuestionLan4);
                            currentQuestionLan4 = myQuestionLan4;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan4 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan4 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan4 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan4.Add(attributeMain1);
                                listOfattributeMainLan4.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan4.Add(attributeMainFIName);
                                listOfattributeMainLan4.Add(attributeMainFICode);
                                listOfattributeMainLan4.Add(attributeMainFSName);
                                listOfattributeMainLan4.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan4.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan4.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan4[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan4.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan4 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan4.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan4.Add(attributeMainLan4);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan4.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan4.Add(listOfattributeMainLan4[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan4.Add(listOfattributeMainLan4[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage4[++i];
                                strline = linesLanguage4[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan4.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan4.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan4[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan4.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan4.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan4.Add(listOfattributeMainLan4[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan4.Add(listOfattributeMainLan4[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage4[++i];
                                //strline = linesLanguage4[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan4 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan4.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan4.AttributeValue = myWord[0].Trim();
                                                attributeMainLan4.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan4.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan4.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan4.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan4 != "")
                                            //    {

                                            //        attributeMainLan4.LinkId1 = "1";
                                            //        attributeMainLan4.LinkId2 = currentGridListNameLan4;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan4.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan4 != "")
                                            //    {

                                            //        attributeMainLan4.LinkId1 = "2";
                                            //        attributeMainLan4.LinkId2 = currentGridListNameLan4;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan4.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan4.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan4 != "")
                                            //    {
                                            //        attributeMainLan4.LinkId1 = "1";
                                            //        attributeMainLan4.LinkId2 = currentGridListNameLan4;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan4.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan4 != "")
                                            //    {
                                            //        attributeMainLan4.LinkId1 = "2";
                                            //        attributeMainLan4.LinkId2 = currentGridListNameLan4;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan4.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan4.Add(attributeMainLan4);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage4[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan4.QId != null)
                                dicQidVsAttributeListLan4.Add(myQuestionLan4.QId, listOfattributeMainLan4);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln4 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language5 Question

            if (linesLanguage5.Count > 0)
            {
                int ln5 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + linesLanguage4.Count + 5;
                j = 0;
                for (int i = 0; i < linesLanguage5.Count; i++)
                {
                    strline = linesLanguage5[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage5, i, txtWriter, dicLine, ln5, 5);
                            strline = linesLanguage5[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage5, i, txtWriter, dicLine, ln5, 5);
                            strline = linesLanguage5[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan5 = new Question();
                            Question myQuestionLan5 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan5 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan5.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan5.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan5.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan5.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan5.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan5.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan5.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan5.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage5[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage5[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan5.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan5.Add(myQuestionLan5);
                            currentQuestionLan5 = myQuestionLan5;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan5 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan5 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan5 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan5.Add(attributeMain1);
                                listOfattributeMainLan5.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan5.Add(attributeMainFIName);
                                listOfattributeMainLan5.Add(attributeMainFICode);
                                listOfattributeMainLan5.Add(attributeMainFSName);
                                listOfattributeMainLan5.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan5.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan5.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan5[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan5.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan5 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan5.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan5.Add(attributeMainLan5);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan5.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan5.Add(listOfattributeMainLan5[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan5.Add(listOfattributeMainLan5[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage5[++i];
                                strline = linesLanguage5[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan5.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan5.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan5[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan5.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan5.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan5.Add(listOfattributeMainLan5[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan5.Add(listOfattributeMainLan5[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage5[++i];
                                //strline = linesLanguage5[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan5 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan5.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan5.AttributeValue = myWord[0].Trim();
                                                attributeMainLan5.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan5.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan5.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan5.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan5 != "")
                                            //    {

                                            //        attributeMainLan5.LinkId1 = "1";
                                            //        attributeMainLan5.LinkId2 = currentGridListNameLan5;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan5.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan5 != "")
                                            //    {

                                            //        attributeMainLan5.LinkId1 = "2";
                                            //        attributeMainLan5.LinkId2 = currentGridListNameLan5;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan5.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan5.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan5 != "")
                                            //    {
                                            //        attributeMainLan5.LinkId1 = "1";
                                            //        attributeMainLan5.LinkId2 = currentGridListNameLan5;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan5.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan5 != "")
                                            //    {
                                            //        attributeMainLan5.LinkId1 = "2";
                                            //        attributeMainLan5.LinkId2 = currentGridListNameLan5;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan5.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan5.Add(attributeMainLan5);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage5[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan5.QId != null)
                                dicQidVsAttributeListLan5.Add(myQuestionLan5.QId, listOfattributeMainLan5);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln5 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language6 Question

            if (linesLanguage6.Count > 0)
            {
                int ln6 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + linesLanguage4.Count + linesLanguage5.Count + 6;
                j = 0;
                for (int i = 0; i < linesLanguage6.Count; i++)
                {
                    strline = linesLanguage6[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage6, i, txtWriter, dicLine, ln6, 6);
                            strline = linesLanguage6[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage6, i, txtWriter, dicLine, ln6, 6);
                            strline = linesLanguage6[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan6 = new Question();
                            Question myQuestionLan6 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan6 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan6.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan6.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan6.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan6.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan6.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan6.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan6.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan6.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage6[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage6[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan6.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan6.Add(myQuestionLan6);
                            currentQuestionLan6 = myQuestionLan6;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan6 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan6 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan6 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan6.Add(attributeMain1);
                                listOfattributeMainLan6.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan6.Add(attributeMainFIName);
                                listOfattributeMainLan6.Add(attributeMainFICode);
                                listOfattributeMainLan6.Add(attributeMainFSName);
                                listOfattributeMainLan6.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan6.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan6.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan6[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan6.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan6 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan6.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan6.Add(attributeMainLan6);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan6.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan6.Add(listOfattributeMainLan6[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan6.Add(listOfattributeMainLan6[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage6[++i];
                                strline = linesLanguage6[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan6.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan6.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan6[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan6.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan6.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan6.Add(listOfattributeMainLan6[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan6.Add(listOfattributeMainLan6[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage6[++i];
                                //strline = linesLanguage6[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan6 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan6.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan6.AttributeValue = myWord[0].Trim();
                                                attributeMainLan6.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan6.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan6.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan6.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan6 != "")
                                            //    {

                                            //        attributeMainLan6.LinkId1 = "1";
                                            //        attributeMainLan6.LinkId2 = currentGridListNameLan6;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan6.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan6 != "")
                                            //    {

                                            //        attributeMainLan6.LinkId1 = "2";
                                            //        attributeMainLan6.LinkId2 = currentGridListNameLan6;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan6.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan6.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan6 != "")
                                            //    {
                                            //        attributeMainLan6.LinkId1 = "1";
                                            //        attributeMainLan6.LinkId2 = currentGridListNameLan6;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan6.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan6 != "")
                                            //    {
                                            //        attributeMainLan6.LinkId1 = "2";
                                            //        attributeMainLan6.LinkId2 = currentGridListNameLan6;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan6.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan6.Add(attributeMainLan6);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage6[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan6.QId != null)
                                dicQidVsAttributeListLan6.Add(myQuestionLan6.QId, listOfattributeMainLan6);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln6 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language7 Question

            if (linesLanguage7.Count > 0)
            {
                int ln7 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + linesLanguage4.Count + linesLanguage5.Count + linesLanguage6.Count + 7;
                j = 0;
                for (int i = 0; i < linesLanguage7.Count; i++)
                {
                    strline = linesLanguage7[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage7, i, txtWriter, dicLine, ln7, 7);
                            strline = linesLanguage7[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage7, i, txtWriter, dicLine, ln7, 7);
                            strline = linesLanguage7[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan7 = new Question();
                            Question myQuestionLan7 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan7 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan7.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan7.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan7.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan7.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan7.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan7.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan7.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan7.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage7[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage7[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan7.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan7.Add(myQuestionLan7);
                            currentQuestionLan7 = myQuestionLan7;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan7 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan7 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan7 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan7.Add(attributeMain1);
                                listOfattributeMainLan7.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan7.Add(attributeMainFIName);
                                listOfattributeMainLan7.Add(attributeMainFICode);
                                listOfattributeMainLan7.Add(attributeMainFSName);
                                listOfattributeMainLan7.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan7.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan7.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan7[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan7.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan7 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan7.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan7.Add(attributeMainLan7);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan7.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan7.Add(listOfattributeMainLan7[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan7.Add(listOfattributeMainLan7[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage7[++i];
                                strline = linesLanguage7[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan7.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan7.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan7[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan7.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan7.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan7.Add(listOfattributeMainLan7[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan7.Add(listOfattributeMainLan7[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage7[++i];
                                //strline = linesLanguage7[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan7 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan7.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan7.AttributeValue = myWord[0].Trim();
                                                attributeMainLan7.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan7.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan7.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan7.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan7 != "")
                                            //    {

                                            //        attributeMainLan7.LinkId1 = "1";
                                            //        attributeMainLan7.LinkId2 = currentGridListNameLan7;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan7.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan7 != "")
                                            //    {

                                            //        attributeMainLan7.LinkId1 = "2";
                                            //        attributeMainLan7.LinkId2 = currentGridListNameLan7;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan7.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan7.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan7 != "")
                                            //    {
                                            //        attributeMainLan7.LinkId1 = "1";
                                            //        attributeMainLan7.LinkId2 = currentGridListNameLan7;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan7.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan7 != "")
                                            //    {
                                            //        attributeMainLan7.LinkId1 = "2";
                                            //        attributeMainLan7.LinkId2 = currentGridListNameLan7;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan7.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan7.Add(attributeMainLan7);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage7[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan7.QId != null)
                                dicQidVsAttributeListLan7.Add(myQuestionLan7.QId, listOfattributeMainLan7);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln7 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language8 Question

            if (linesLanguage8.Count > 0)
            {
                int ln8 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + linesLanguage4.Count + linesLanguage5.Count + linesLanguage6.Count + linesLanguage7.Count + 8;
                j = 0;
                for (int i = 0; i < linesLanguage8.Count; i++)
                {
                    strline = linesLanguage8[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage8, i, txtWriter, dicLine, ln8, 8);
                            strline = linesLanguage8[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage8, i, txtWriter, dicLine, ln8, 8);
                            strline = linesLanguage8[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan8 = new Question();
                            Question myQuestionLan8 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan8 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan8.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan8.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan8.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan8.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan8.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan8.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan8.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan8.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage8[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage8[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan8.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan8.Add(myQuestionLan8);
                            currentQuestionLan8 = myQuestionLan8;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan8 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan8 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan8 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan8.Add(attributeMain1);
                                listOfattributeMainLan8.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan8.Add(attributeMainFIName);
                                listOfattributeMainLan8.Add(attributeMainFICode);
                                listOfattributeMainLan8.Add(attributeMainFSName);
                                listOfattributeMainLan8.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan8.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan8.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan8[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan8.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan8 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan8.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan8.Add(attributeMainLan8);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan8.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan8.Add(listOfattributeMainLan8[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan8.Add(listOfattributeMainLan8[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage8[++i];
                                strline = linesLanguage8[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan8.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan8.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan8[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan8.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan8.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan8.Add(listOfattributeMainLan8[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan8.Add(listOfattributeMainLan8[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage8[++i];
                                //strline = linesLanguage8[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan8 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan8.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan8.AttributeValue = myWord[0].Trim();
                                                attributeMainLan8.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan8.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan8.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan8.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan8 != "")
                                            //    {

                                            //        attributeMainLan8.LinkId1 = "1";
                                            //        attributeMainLan8.LinkId2 = currentGridListNameLan8;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan8.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan8 != "")
                                            //    {

                                            //        attributeMainLan8.LinkId1 = "2";
                                            //        attributeMainLan8.LinkId2 = currentGridListNameLan8;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan8.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan8.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan8 != "")
                                            //    {
                                            //        attributeMainLan8.LinkId1 = "1";
                                            //        attributeMainLan8.LinkId2 = currentGridListNameLan8;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan8.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan8 != "")
                                            //    {
                                            //        attributeMainLan8.LinkId1 = "2";
                                            //        attributeMainLan8.LinkId2 = currentGridListNameLan8;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan8.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan8.Add(attributeMainLan8);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage8[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan8.QId != null)
                                dicQidVsAttributeListLan8.Add(myQuestionLan8.QId, listOfattributeMainLan8);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln8 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region Prepare Language9 Question

            if (linesLanguage9.Count > 0)
            {
                int ln9 = lines.Count + linesLanguage1.Count + linesLanguage2.Count + linesLanguage3.Count + linesLanguage4.Count + linesLanguage5.Count + linesLanguage6.Count + linesLanguage7.Count + linesLanguage8.Count + 9;

                j = 0;
                for (int i = 0; i < linesLanguage9.Count; i++)
                {
                    strline = linesLanguage9[i];

                    if (strline.Substring(0, 1) == "*")
                    {
                        #region Prepare LIST
                        if (strline.Split(' ')[0].ToUpper() == "*LIST")
                        {
                            i = this.prepareListForLanguage(linesLanguage9, i, txtWriter, dicLine, ln9, 9);
                            strline = linesLanguage9[i];
                        }
                        #endregion

                        #region Prepare GRIDLIST
                        if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                        {
                            i = this.prepareGridListForLanguage(linesLanguage9, i, txtWriter, dicLine, ln9, 9);
                            strline = linesLanguage9[i];
                        }
                        #endregion

                        #region Prepare QUESTION
                        if (strline.Split(' ')[0].ToUpper() == "*QUESTION")
                        {
                            AttributeMain attributeMain1 = new AttributeMain();
                            AttributeMain attributeMain2 = new AttributeMain();

                            AttributeMain attributeMainFIName = new AttributeMain();
                            AttributeMain attributeMainFICode = new AttributeMain();
                            AttributeMain attributeMainFSName = new AttributeMain();
                            AttributeMain attributeMainFSCode = new AttributeMain();

                            hasDKCS = false;

                            currentQuestionLan9 = new Question();
                            Question myQuestionLan9 = new Question();
                            AttributeFilter myAttributeFilter = new AttributeFilter();
                            string[] word = strline.Split('*');
                            int QTypeCounter = 0;
                            List<string> listOfQuestionProperties = new List<string>();
                            String currentGridListNameLan9 = "";



                            #region Question Properties
                            for (int n = 1; n < word.Length; n++)
                            {
                                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                                string myText = "*" + word[n];

                                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                                {
                                    //QID
                                    string[] xyz = word[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                                        {
                                            if (!listOfQuestionIdForDupliCheckLan9.Contains(xyz[1].Trim()))
                                            {
                                                myQuestionLan9.QId = xyz[1].Trim();
                                                listOfQuestionIdForDupliCheckLan9.Add(xyz[1].Trim());

                                                //if (myQuestion.QId == "SQ21")
                                                //    MessageBox.Show("");
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                                }
                                //Question Type
                                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RANK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DATE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*TIME"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*FORM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*INFO"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                                { hasEnd = true; }
                                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                                { hasTerminate = true; }
                                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                                {
                                    myQuestionLan9.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                                    attributeMainFIName = new AttributeMain();
                                    attributeMainFIName.QId = myQuestionLan9.QId;
                                    attributeMainFIName.AttributeEnglish = "FI Name";
                                    attributeMainFIName.AttributeValue = "1";
                                    attributeMainFIName.AttributeOrder = "1";
                                    attributeMainFIName.LinkId1 = "3";
                                    attributeMainFIName.ForceAndMsgOpt = "11";

                                    attributeMainFICode = new AttributeMain();
                                    attributeMainFICode.QId = myQuestionLan9.QId;
                                    attributeMainFICode.AttributeEnglish = "FI Code";
                                    attributeMainFICode.AttributeValue = "2";
                                    attributeMainFICode.AttributeOrder = "2";
                                    attributeMainFICode.LinkId1 = "3";
                                    attributeMainFICode.ForceAndMsgOpt = "11";

                                    attributeMainFSName = new AttributeMain();
                                    attributeMainFSName.QId = myQuestionLan9.QId;
                                    attributeMainFSName.AttributeEnglish = "FS Name";
                                    //attributeMainFSName.AttributeEnglish = "FI Mobile Number";
                                    attributeMainFSName.AttributeValue = "3";
                                    attributeMainFSName.AttributeOrder = "3";
                                    attributeMainFSName.LinkId1 = "3";
                                    attributeMainFSName.ForceAndMsgOpt = "11";

                                    attributeMainFSCode = new AttributeMain();
                                    attributeMainFSCode.QId = myQuestionLan9.QId;
                                    attributeMainFSCode.AttributeEnglish = "FS Code";
                                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                                    attributeMainFSCode.AttributeValue = "4";
                                    attributeMainFSCode.AttributeOrder = "4";
                                    attributeMainFSCode.LinkId1 = "3";
                                    attributeMainFSCode.ForceAndMsgOpt = "11";

                                    hasFIFS = true;
                                }


                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                //else if (word[n].ToUpper().Trim().Contains(""))
                                //    myQuestion.QType = "7";
                                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*ROT"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }

                                else if (myText.ToUpper().Trim().Contains("*MIN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* Attribute Filter ***********************************************
                                else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                                {
                                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                                    if (xyz.Length == 5)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                        if (xyz[1].Trim() != "")
                                        {
                                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                            {
                                                hasDKCS = true;

                                                attributeMain1 = new AttributeMain();
                                                attributeMain1.QId = currentQuestion.QId;
                                                attributeMain1.AttributeEnglish = "";
                                                attributeMain1.AttributeValue = "1";
                                                attributeMain1.AttributeOrder = "1";

                                                attributeMain2 = new AttributeMain();
                                                attributeMain2.QId = currentQuestion.QId;
                                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                                attributeMain2.AttributeValue = xyz[3].Trim();
                                                attributeMain2.AttributeOrder = "2";
                                                attributeMain2.IsExclusive = "1";



                                                //Add the attribute list 
                                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Attribute Label missing " + xyz[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Syntax for *DKCS is invalid ");
                                }
                                else if (myText.ToUpper().Trim().Contains("IF"))
                                { txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                                //************************************* End of Attribute Filter ****************************************

                            }
                            #endregion

                            string questionText = "";
                            strline = linesLanguage9[++i];
                            bool getquestionText = false;
                            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                            {
                                questionText = questionText + strline + "<br>";
                                strline = linesLanguage9[++i];
                                getquestionText = true;
                            }

                            if (questionText == "")
                                txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invalid Question Text : should not exist");
                            else
                                myQuestionLan9.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                            //add question object to list
                            listOfQuestionLan9.Add(myQuestionLan9);
                            currentQuestionLan9 = myQuestionLan9;

                            //this portion is for question attribute

                            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            List<AttributeMain> listOfattributeMainLan9 = new List<AttributeMain>();
                            int index = 1;

                            List<String> listOfAttributeValueForDupliCheckLan9 = new List<string>();
                            List<String> listOfAttributeLabelForDupliCheckLan9 = new List<string>();

                            if (hasDKCS == true)
                            {
                                listOfattributeMainLan9.Add(attributeMain1);
                                listOfattributeMainLan9.Add(attributeMain2);
                                hasDKCS = false;
                            }

                            if (hasFIFS == true)
                            {
                                listOfattributeMainLan9.Add(attributeMainFIName);
                                listOfattributeMainLan9.Add(attributeMainFICode);
                                listOfattributeMainLan9.Add(attributeMainFSName);
                                listOfattributeMainLan9.Add(attributeMainFSCode);

                                hasFIFS = false;
                            }

                            #region USELIST
                            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                            {
                                //Pronab made changes in this block
                                string[] word1 = strline.Split(' ');
                                if (word1.Length == 2)
                                {
                                    if (word1[1].Split('"').Length == 3)
                                    {
                                        if (dicQidVsAttributeListLan9.ContainsKey(word1[1].Split('"')[1].Trim()))
                                        {
                                            if (dicQidVsAttributeListLan9.ContainsKey(word1[1].Split('"')[1].Trim()))
                                            {
                                                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan9[word1[1].Split('"')[1].Trim()];

                                                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                                {
                                                    if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                                    {
                                                        listOfattributeMainLan9.Add(listOfAttributeTemp[x]);
                                                        index++;
                                                    }
                                                    else
                                                    {
                                                        //If attribute have properties
                                                        #region Attribute Properties
                                                        string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                        AttributeMain attributeMainLan9 = new AttributeMain();
                                                        //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                                        //{

                                                        attributeMainLan9.AttributeEnglish = myKey[0].Trim();
                                                        //}


                                                        for (int n = 1; n < myKey.Length; n++)
                                                        {
                                                            if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                                txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            else
                                                            {
                                                                if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                                else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                            }
                                                        }
                                                        //Add the attribute in 
                                                        listOfattributeMainLan9.Add(attributeMainLan9);
                                                        #endregion
                                                    }

                                                }

                                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                                //index = listOfAttributeMain.Count + 1;
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                            for (int x = 0; x < listOfattributeMainLan9.Count; x++)
                                            {
                                                listOfAttributeValueForDupliCheckLan9.Add(listOfattributeMainLan9[x].AttributeValue);
                                                listOfAttributeLabelForDupliCheckLan9.Add(listOfattributeMainLan9[x].AttributeEnglish);
                                            }

                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                strline = linesLanguage9[++i];
                                strline = linesLanguage9[++i];
                                //Pronab end

                                //string[] word1 = strline.Split(' ');
                                //if (word1.Length == 2)
                                //{
                                //    if (word1[1].Split('"').Length == 3)
                                //    {
                                //        if (dicQidVsAttributeListLan9.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //        {
                                //            if (dicQidVsAttributeListLan9.ContainsKey(word1[1].Split('"')[1].Trim()))
                                //            {
                                //                List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLan9[word1[1].Split('"')[1].Trim()];

                                //                for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                //                {
                                //                    listOfattributeMainLan9.Add(listOfAttributeTemp[x]);
                                //                    index++;
                                //                }

                                //                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //                //index = listOfAttributeMain.Count + 1;
                                //            }
                                //            else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                //            for (int x = 0; x < listOfattributeMainLan9.Count; x++)
                                //            {
                                //                listOfAttributeValueForDupliCheckLan9.Add(listOfattributeMainLan9[x].AttributeValue);
                                //                listOfAttributeLabelForDupliCheckLan9.Add(listOfattributeMainLan9[x].AttributeEnglish);
                                //            }

                                //        }
                                //        else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                                //    }
                                //    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                                //}
                                //else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                                //strline = linesLanguage9[++i];
                                //strline = linesLanguage9[++i];
                            }
                            #endregion

                            #region Attribute with :
                            if (isAttribute(strline))
                            {
                                while (!strline.Trim().Substring(0, 1).Contains("*"))
                                {

                                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                    if (strline.Contains(":"))
                                    {
                                        AttributeMain attributeMainLan9 = new AttributeMain();
                                        String[] myWord = strline.Split(':');

                                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                                        {
                                            if (!listOfAttributeValueForDupliCheckLan9.Contains(myWord[0].Trim()))
                                            {
                                                attributeMainLan9.AttributeValue = myWord[0].Trim();
                                                attributeMainLan9.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                                index++;

                                                //Add value in list
                                                listOfAttributeValueForDupliCheckLan9.Add(myWord[0].Trim());

                                            }//else {Error Message}

                                        }//else {Error Message}
                                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                                        if (!mylabel.Contains("*"))
                                        {
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                            //{

                                            attributeMainLan9.AttributeEnglish = mylabel.Trim();

                                            //if (myQuestionLan9.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan9 != "")
                                            //    {

                                            //        attributeMainLan9.LinkId1 = "1";
                                            //        attributeMainLan9.LinkId2 = currentGridListNameLan9;

                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan9.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan9 != "")
                                            //    {

                                            //        attributeMainLan9.LinkId1 = "2";
                                            //        attributeMainLan9.LinkId2 = currentGridListNameLan9;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Grid list unavailable");
                                            //}
                                            //Add value in list
                                            listOfAttributeLabelForDupliCheckLan9.Add(mylabel.Trim().ToUpper());
                                            //}//else {Error Message}
                                        }
                                        else
                                        {
                                            // *********** If grid attribute has property ********************
                                            //if (myQuestionLan9.QType == "7")
                                            //{
                                            //    if (currentGridListNameLan9 != "")
                                            //    {
                                            //        attributeMainLan9.LinkId1 = "1";
                                            //        attributeMainLan9.LinkId2 = currentGridListNameLan9;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Grid list unavailable");
                                            //}
                                            //else if (myQuestionLan9.QType == "8")
                                            //{
                                            //    if (currentGridListNameLan9 != "")
                                            //    {
                                            //        attributeMainLan9.LinkId1 = "2";
                                            //        attributeMainLan9.LinkId2 = currentGridListNameLan9;
                                            //    }
                                            //    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Grid list unavailable");
                                            //}
                                            //*****************************************************


                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = mylabel.Split('*');

                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLan9.AttributeEnglish = myKey[0].Trim();
                                            //}


                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (myKey[n].ToUpper().Trim().Contains("MIN") || myKey[n].ToUpper().Trim().Contains("MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist");
                                                else
                                                {
                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invlaid Syntax, " + myKey[n].Trim() + " Should not exist");
                                                }
                                            }
                                            #endregion

                                        }

                                        //Add the attribute in 
                                        listOfattributeMainLan9.Add(attributeMainLan9);

                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Invalid syntax, Attribute code missing");

                                    if (i < lines.Count - 1)
                                    {
                                        strline = linesLanguage9[++i];
                                    }
                                }

                                if (i < lines.Count - 1)
                                    i--;
                            }
                            else
                            {
                                if (i < lines.Count - 1)
                                    i--;
                            }

                            #endregion

                            if (myQuestionLan9.QId != null)
                                dicQidVsAttributeListLan9.Add(myQuestionLan9.QId, listOfattributeMainLan9);
                            else
                                txtWriter.WriteLine("Line : " + dicLine[i + ln9 + 1] + " Question Id missing");

                            if (myAttributeFilter.QId != null)
                                listOfAttributeFilter.Add(myAttributeFilter);

                        }
                        #endregion

                    }
                    //next:
                    j++;
                }
            }
            #endregion

            #region end question and error checks
            //********************************************************************************
            List<string> listOfErrmsg = checkScript();
            if (listOfErrmsg.Count > 0)
            {
                for (int x = 0; x < listOfErrmsg.Count; x++)
                    txtWriter.WriteLine(listOfErrmsg[x]);
            }
            //********************************************************************************

            if (hasEnd == false)
                txtWriter.WriteLine("End Question not exist");
            if (hasTerminate == false)
                txtWriter.WriteLine("Terminate Question not exist");


            //********************************************************************************
            #endregion

            #region checking languages
            if (linesLanguage1.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan1, dicQidVsAttributeListLan1, dicGridListNameVsListLan1, 1);
            if (linesLanguage2.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan2, dicQidVsAttributeListLan2, dicGridListNameVsListLan2, 2);
            if (linesLanguage3.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan3, dicQidVsAttributeListLan3, dicGridListNameVsListLan3, 3);
            if (linesLanguage4.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan4, dicQidVsAttributeListLan4, dicGridListNameVsListLan4, 4);
            if (linesLanguage5.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan5, dicQidVsAttributeListLan5, dicGridListNameVsListLan5, 5);
            if (linesLanguage6.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan6, dicQidVsAttributeListLan6, dicGridListNameVsListLan6, 6);
            if (linesLanguage7.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan7, dicQidVsAttributeListLan7, dicGridListNameVsListLan7, 7);
            if (linesLanguage8.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan8, dicQidVsAttributeListLan8, dicGridListNameVsListLan8, 8);
            if (linesLanguage9.Count > 0)
                this.checkEnglishBengaliScript(txtWriter, listOfQuestionLan9, dicQidVsAttributeListLan9, dicGridListNameVsListLan9, 9);


            //********************************************************************************

            txtWriter.Close();

            if (File.ReadAllLines(myPath + "\\BuildResult.txt").Length == 0)
            {
                TextWriter txtWrite2 = new StreamWriter(myPath + "\\BuildResult.txt");
                txtWrite2.WriteLine("Build successful...");

                txtWrite2.Close();

                this.prepareScriptDB();

                if (linesLanguage1.Count > 0)
                {
                    this.updateBengaliTranslation();
                }
                if (linesLanguage2.Count > 0)
                {
                    this.update3rdTranslation();
                }
                if (linesLanguage3.Count > 0)
                {
                    this.update4thTranslation();
                }
                if (linesLanguage4.Count > 0)
                {
                    this.update5thTranslation();
                }
                if (linesLanguage5.Count > 0)
                {
                    this.update6thTranslation();
                }
                if (linesLanguage6.Count > 0)
                {
                    this.update7thTranslation();
                }
                if (linesLanguage7.Count > 0)
                {
                    this.update8thTranslation();
                }
                if (linesLanguage8.Count > 0)
                {
                    this.update9thTranslation();
                }
                if (linesLanguage9.Count > 0)
                {
                    this.update10thTranslation();
                }
                preparedScript = true;

            }


            GC.Collect();
            GC.WaitForPendingFinalizers();

            DisplayBuildResult(myPath + "\\BuildResult.txt");
            SetUIState(running: false);
            #endregion
            } // end try
            catch (Exception ex)
            {
                AppendResult("Unexpected build error: " + ex.Message, true);
                SetUIState(running: false);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        // Helper: reads one language section from the script file into targetLines
        private void ReadLanguageSection(TextReader txtReader, List<String> targetLines, ref int a, ref int b, Dictionary<int, int> dicLine)
        {
            String strline = txtReader.ReadLine();
            while (strline != null)
            {
                if (strline.ToUpper().Contains("@LANGUAGE"))
                {
                    string[] langArray = strline.Split('"');
                    if (langArray.Length == 3)
                        listOfLanguage.Add(langArray[1]);
                    else
                        MessageBox.Show("Invalid @LANGUAGE Syntax");
                    break;
                }
                else
                {
                    b++;
                    if (strline.Trim() != "" && strline.Substring(0, 1) != "#" && strline.Substring(0, 1) != "$")
                    {
                        a++;
                        targetLines.Add(Regex.Replace(strline.Trim(), @"\s+", " "));
                        dicLine.Add(a, b);
                    }
                }
                strline = txtReader.ReadLine();
            }
        }

        private int prepareList(List<String> lines, int i, TextWriter txtWriter, Dictionary<int, int> dicLine)
        {

            ///////////////////////////////////////////////////////////
            //     *LIST "YesNo"
            //     1:Yes
            //     2:No
            //
            //     *LIST "NameAge"
            //     1:Name *ALPHA *MANDATORY
            //     2:Aag *NUMBER *MIN 15 *MAX 65
            ///////////////////////////////////////////////////////////

            int index = 1;
            String strline = lines[i];
            List<String> listOfAttributeValueForDupliCheck = new List<string>();
            List<String> listOfAttributeLabelForDupliCheck = new List<string>();

            string listName = strline.Split(' ')[1].Split('"')[1];
            List<AttributeMain> listOfAttributeMainTemp = new List<AttributeMain>();

            strline = lines[++i];
            if (strline.Contains(":"))
            {
                while (!strline.Trim().Substring(0, 1).Contains("*"))
                {
                    if (strline.Contains(":") && strline.Split(':').Length == 2)
                    {
                        AttributeMain attributeMain = new AttributeMain();
                        String[] word = strline.Split(':');

                        if (Regex.Match(word[0].Trim(), @"^\d+$").Success)
                        {
                            if (!listOfAttributeValueForDupliCheck.Contains(word[0].Trim()))
                            {
                                attributeMain.AttributeValue = word[0].Trim();
                                attributeMain.AttributeOrder = word[0].Trim(); //index.ToString();
                                index++;

                                //Add value in list
                                listOfAttributeValueForDupliCheck.Add(word[0].Trim());

                                //if (attributeMain.AttributeValue == "123")
                                //    MessageBox.Show("");

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute value " + word[0].Trim() + " is duplicate");

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute value " + word[0].Trim() + " is Non numeric");



                        //If Attribute has any properties like 1:Name *ALPHA *MANDATORY
                        if (!word[1].Contains("*"))
                        {
                            if (!listOfAttributeLabelForDupliCheck.Contains(word[1].Trim().ToUpper()))
                            {
                                attributeMain.AttributeEnglish = word[1].Trim().Replace("'", "''");
                                //Add value in list
                                listOfAttributeLabelForDupliCheck.Add(word[1].Trim().ToUpper());
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute label " + word[0].Trim() + " is duplicate");
                        }
                        else //start Pronab added
                        {
                            if (!listOfAttributeLabelForDupliCheck.Contains(word[1].Split('*')[0].Trim().ToUpper()))
                            {
                                attributeMain.AttributeEnglish = word[1].Trim();
                                //Add value in list
                                listOfAttributeLabelForDupliCheck.Add(word[1].Split('*')[0].Trim().ToUpper());
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute label " + word[0].Trim() + " is duplicate");


                            string[] myKey = word[1].Split('*');
                            if (myKey.Length > 1)
                            {
                                //attributeMain.AttributeEnglish = myKey[0].Trim();

                                for (int x = 1; x < myKey.Length; x++)
                                {
                                    if (("*" + myKey[1]).ToUpper().Trim().Contains("*OPEN"))
                                    {
                                        attributeMain.TakeOpenended = "1";
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*ALPHA"))
                                    {
                                        attributeMain.LinkId1 = "3";
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*NUMBER"))
                                    {
                                        attributeMain.LinkId1 = "4";
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*NMUL"))
                                    {
                                        attributeMain.IsExclusive = "1";
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*MANDATORY"))
                                    {
                                        attributeMain.ForceAndMsgOpt = "11";
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*PICT"))
                                    {
                                        string[] xy = myKey[1].Trim().Split(' ');
                                        if (xy.Length == 2)
                                        {
                                            //listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                            string[] xyz = xy[1].Split('"');
                                            if (xyz.Length != 3)
                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                            else
                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*VIDEO"))
                                    {
                                        string[] xy = myKey[1].Trim().Split(' ');
                                        if (xy.Length == 2)
                                        {
                                            //listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                            string[] xyz = xy[1].Split('"');
                                            if (xyz.Length != 3)
                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                            else
                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");
                                    }
                                    else if (("*" + myKey[1]).ToUpper().Trim().Contains("*MIN"))
                                    {
                                        string[] xyz = myKey[1].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MinValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                    }
                                    else if (("*" + myKey[2]).ToUpper().Trim().Contains("*MAX"))
                                    {
                                        string[] xyz = myKey[2].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MaxValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                    }
                                    else if (("*" + myKey[2]).ToUpper().Trim().Contains("*MANDATORY"))
                                    {
                                        attributeMain.ForceAndMsgOpt = "11";
                                    }
                                }


                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute label " + word[0].Trim() + " is incorrect");








                        } //end Pronab added


                        listOfAttributeMainTemp.Add(attributeMain);

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Not proper attribute syntax : " + strline);

                    strline = lines[++i];
                }

                if ((strline.Split(' ')[0].ToUpper() == "*GRIDLIST" || strline.Split(' ')[0].ToUpper() == "*LIST") && i < lines.Count - 1)
                    i--;
            }
            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Not proper attribute syntax : " + strline);

            if (!dicListNameVsList.ContainsKey(listName))
                dicListNameVsList.Add(listName, listOfAttributeMainTemp);
            else
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate list name : " + listName);

            return i;
        }

        private int prepareListForLanguage(List<String> linesLanguage, int i, TextWriter txtWriter, Dictionary<int, int> dicLine, int ln1, int languageNo)
        {

            ///////////////////////////////////////////////////////////
            //     *LIST "YesNo"
            //     1:Yes
            //     2:No
            //
            //     *LIST "NameAge"
            //     1:Name
            //     2:Aag
            ///////////////////////////////////////////////////////////

            String strline = linesLanguage[i];
            List<String> listOfAttributeValueForDupliCheckLan1 = new List<string>();
            List<String> listOfAttributeLabelForDupliCheckLan1 = new List<string>();

            string listName = strline.Split(' ')[1].Split('"')[1];
            List<AttributeMain> listOfAttributeMainLanX = new List<AttributeMain>();
            strline = linesLanguage[++i];

            if (isAttribute(strline))
            {
                while (!strline.Trim().Substring(0, 1).Contains("*"))
                {
                    if (strline.Contains(":") && strline.Split(':').Length == 2)
                    {

                        AttributeMain attributeMainLanX = new AttributeMain();
                        String[] word = strline.Split(':');

                        if (Regex.Match(word[0].Trim(), @"^\d+$").Success)
                        {
                            if (!listOfAttributeValueForDupliCheckLan1.Contains(word[0].Trim()))
                            {
                                attributeMainLanX.AttributeValue = word[0].Trim();
                                attributeMainLanX.AttributeOrder = word[0].Trim(); //index.ToString();
                                //Add value in list
                                listOfAttributeValueForDupliCheckLan1.Add(word[0].Trim());

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute value " + word[0].Trim() + " is duplicate");

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute value " + word[0].Trim() + " is Non numeric");

                        if (!word[1].Contains("*"))
                        {
                            if (!listOfAttributeLabelForDupliCheckLan1.Contains(word[1].Trim().ToUpper()))
                            {
                                attributeMainLanX.AttributeEnglish = word[1].Trim().Replace("'", "''");
                                //Add value in list
                                listOfAttributeLabelForDupliCheckLan1.Add(word[1].Trim().ToUpper());
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute label " + word[0].Trim() + " is duplicate");
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute label " + word[0].Trim() + " is incorrect, Attribute parameter should not be exist");

                        listOfAttributeMainLanX.Add(attributeMainLanX);

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Not proper attribute syntax : " + strline);

                    strline = linesLanguage[++i];
                }

                if ((strline.Split(' ')[0].ToUpper() == "*GRIDLIST" || strline.Split(' ')[0].ToUpper() == "*LIST") && i < linesLanguage.Count - 1)
                    i--;
            }
            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Not proper attribute syntax : " + strline);

            if (languageNo == 1) dicQidVsAttributeListLan1.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 2) dicQidVsAttributeListLan2.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 3) dicQidVsAttributeListLan3.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 4) dicQidVsAttributeListLan4.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 5) dicQidVsAttributeListLan5.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 6) dicQidVsAttributeListLan6.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 7) dicQidVsAttributeListLan7.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 8) dicQidVsAttributeListLan8.Add(listName, listOfAttributeMainLanX);
            else if (languageNo == 9) dicQidVsAttributeListLan9.Add(listName, listOfAttributeMainLanX);

            return i;
        }

        private int prepareGridList(List<String> lines, int i, List<string> listOfGridListForDupliCheck, TextWriter txtWriter, Dictionary<int, int> dicLine)
        {
            try
            {
                ///////////////////////////////////////////////////////////
                //     *GRIDLIST "YesNo"
                //     1:Yes
                //     2:No
                ///////////////////////////////////////////////////////////

                int index = 1;
                String strline = lines[i];
                List<String> listOfAttributeValueForDupliCheck = new List<string>();
                List<String> listOfAttributeLabelForDupliCheck = new List<string>();

                string gridListName = strline.Split(' ')[1].Split('"')[1];
                List<GridInfo> listOfGridInfo = new List<GridInfo>();
                strline = lines[++i];
                if (strline.Contains(":"))
                {
                    while (!strline.Trim().Substring(0, 1).Contains("*"))
                    {
                        if (strline.Contains(":") && strline.Split(':').Length == 2)
                        {
                            GridInfo gridInfo = new GridInfo();
                            String[] word = strline.Split(':');

                            if (Regex.Match(word[0].Trim(), @"^\d+$").Success)
                            {
                                if (!listOfAttributeValueForDupliCheck.Contains(word[0].Trim()))
                                {
                                    gridInfo.AttributeValue = word[0].Trim();
                                    gridInfo.AttributeOrder = word[0].Trim(); //index.ToString();
                                    index++;

                                    //Add value in list
                                    listOfAttributeValueForDupliCheck.Add(word[0].Trim());

                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute value " + word[0].Trim() + " is duplicate");

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute value " + word[0].Trim() + " is Non numeric");

                            if (!word[1].Contains("*"))
                            {
                                if (!listOfAttributeLabelForDupliCheck.Contains(word[1].Trim().ToUpper()))
                                {
                                    gridInfo.AttributeEnglish = word[1].Trim();
                                    //Add value in list
                                    listOfAttributeLabelForDupliCheck.Add(word[1].Trim().ToUpper());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute label " + word[0].Trim() + " is duplicate");
                            }
                            else if (word[1].Contains("*"))
                            {
                                string[] myKey = word[1].Split('*');
                                if (myKey.Length == 2)
                                {
                                    gridInfo.AttributeEnglish = myKey[0].Trim();

                                    if (myKey[1].ToUpper().Trim().Contains("OPEN"))
                                        gridInfo.TakeOpenended = "1";
                                    else if (myKey[1].ToUpper().Trim().Contains("NMUL"))
                                        gridInfo.IsExclusive = "1";
                                    else if (myKey[1].ToUpper().Trim().Contains("MANDATORY"))
                                        gridInfo.ForceAndMsgOpt = "11";
                                    else if (myKey[1].ToUpper().Trim().Contains("PICT"))
                                    {
                                        string[] xy = myKey[1].Trim().Split(' ');
                                        if (xy.Length == 2)
                                        {
                                            //listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                            string[] xyz = xy[1].Split('"');
                                            if (xyz.Length != 3)
                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                            else
                                                gridInfo.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                    }
                                    else if (myKey[1].ToUpper().Trim().Contains("VIDEO"))
                                    {
                                        string[] xy = myKey[1].Trim().Split(' ');
                                        if (xy.Length == 2)
                                        {
                                            //listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                            string[] xyz = xy[1].Split('"');
                                            if (xyz.Length != 3)
                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                            else
                                                gridInfo.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");
                                    }
                                }
                                else if (myKey.Length > 2)
                                {
                                    gridInfo.AttributeEnglish = myKey[0].Trim();

                                    for (int x = 1; x < myKey.Length; x++)
                                    {
                                        if (("*" + myKey[1]).ToUpper().Trim().Contains("*MIN"))
                                        {
                                            string[] xyz = myKey[1].Split(' ');
                                            if (xyz.Length >= 2)
                                            {
                                                if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                    gridInfo.MinValue = xyz[1].Trim();
                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                        }
                                        else if (("*" + myKey[2]).ToUpper().Trim().Contains("*MAX"))
                                        {
                                            string[] xyz = myKey[2].Split(' ');
                                            if (xyz.Length >= 2)
                                            {
                                                if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                    gridInfo.MaxValue = xyz[1].Trim();
                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                        }
                                        else if (("*" + myKey[2]).ToUpper().Trim().Contains("*MANDATORY"))
                                        {
                                            gridInfo.ForceAndMsgOpt = "11";
                                        }
                                    }


                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute label " + word[0].Trim() + " is incorrect");
                            }


                            listOfGridInfo.Add(gridInfo);

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Not proper attribute syntax : " + strline);

                        strline = lines[++i];
                    }
                    if ((strline.Split(' ')[0].ToUpper() == "*GRIDLIST" || strline.Split(' ')[0].ToUpper() == "*LIST") && i < lines.Count - 1)
                        i--;
                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Not proper attribute syntax : " + strline);

                dicGridListNameVsList.Add(gridListName, listOfGridInfo);

                listOfGridListForDupliCheck.Add(gridListName);

                return i;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Line : " + dicLine[i + 1] + ": Syntax Error in for Grid/Same Grid name exist");
                return i;
            }
        }

        private int prepareGridListForLanguage(List<String> linesLanguage, int i, TextWriter txtWriter, Dictionary<int, int> dicLine, int ln1, int languageNo)
        {
            try
            {
                ///////////////////////////////////////////////////////////
                //     *GRIDLIST "YesNo"
                //     1:Yes
                //     2:No
                ///////////////////////////////////////////////////////////

                //int index = 1;
                String strline = linesLanguage[i];
                List<string> listOfGridListForDupliCheckLan = new List<string>();
                List<String> listOfAttributeValueForDupliCheckLan1 = new List<string>();
                List<String> listOfAttributeLabelForDupliCheckLan1 = new List<string>();

                string gridListName = strline.Split(' ')[1].Split('"')[1];
                List<GridInfo> listOfGridInfoLanX = new List<GridInfo>();
                strline = linesLanguage[++i];
                if (isAttribute(strline))
                {
                    while (!strline.Trim().Substring(0, 1).Contains("*"))
                    {
                        //if (strline.Contains(":") && strline.Split(':').Length == 2)
                        if (strline.Contains(":"))
                        {
                            GridInfo gridInfoLanX = new GridInfo();
                            String[] word = strline.Split(':');

                            if (Regex.Match(word[0].Trim(), @"^\d+$").Success)
                            {
                                if (!listOfAttributeValueForDupliCheckLan1.Contains(word[0].Trim()))
                                {
                                    gridInfoLanX.AttributeValue = word[0].Trim();
                                    gridInfoLanX.AttributeOrder = word[0].Trim(); //index.ToString();
                                    //index++;

                                    //Add value in list
                                    listOfAttributeValueForDupliCheckLan1.Add(word[0].Trim());

                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute value " + word[0].Trim() + " is duplicate");

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute value " + word[0].Trim() + " is Non numeric");

                            if (!word[1].Contains("*"))
                            {
                                if (!listOfAttributeLabelForDupliCheckLan1.Contains(word[1].Trim().ToUpper()))
                                {
                                    gridInfoLanX.AttributeEnglish = word[1].Trim();
                                    //Add value in list
                                    listOfAttributeLabelForDupliCheckLan1.Add(word[1].Trim().ToUpper());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute label " + word[1].Trim() + " is duplicate");
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute label " + word[1].Trim() + " is incorrect, Attribute parameter should not be exist");

                            listOfGridInfoLanX.Add(gridInfoLanX);

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Not proper attribute syntax : " + strline);

                        strline = linesLanguage[++i];
                    }
                    //if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST" && i < linesLanguage.Count - 1)
                    if ((strline.Split(' ')[0].ToUpper() == "*GRIDLIST" || strline.Split(' ')[0].ToUpper() == "*LIST") && i < linesLanguage.Count - 1)
                        //if (i < linesLanguage.Count - 1)
                        i--;
                }
                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Not proper attribute syntax : " + strline);

                listOfGridListForDupliCheckLan.Add(gridListName);

                if (languageNo == 1) dicGridListNameVsListLan1.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 2) dicGridListNameVsListLan2.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 3) dicGridListNameVsListLan3.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 4) dicGridListNameVsListLan4.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 5) dicGridListNameVsListLan5.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 6) dicGridListNameVsListLan6.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 7) dicGridListNameVsListLan7.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 8) dicGridListNameVsListLan8.Add(gridListName, listOfGridInfoLanX);
                else if (languageNo == 9) dicGridListNameVsListLan9.Add(gridListName, listOfGridInfoLanX);


                return i;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Line : " + dicLine[i + 1] + ": Syntax Error in for Grid/Same Grid name exist");
                return i;
            }
        }

        private int prepareIf(List<String> lines, int i, List<string> listOfQuestionIdForDupliCheck, List<AutoResponse> listOfAutoResponseTemp, List<LogicalSyntax> listOfLogicalSyntaxTemp, TextWriter txtWriter, Dictionary<int, int> dicLine)
        {
            String strline = lines[i];
            if (strline.Trim().Split(' ')[0].ToUpper() == "*IF" && !strline.ToUpper().Contains("REGULAREXPOF"))
            {

                LogicalSyntax myLogicalSyntax;// = new LogicalSyntax();

                string[] pqr = strline.Trim().Split('*');
                if (pqr.Length == 3)
                {

                    string[] mno = pqr[1].Split(' ');
                    //string ifCondition = pqr[1].Split(new Char[] { '[', ']' })[1];

                    string ifCondition = pqr[1].Substring(pqr[1].IndexOf('[') + 1);//pqr[1].Split(new Char[] { '[', ']' })[1];
                    ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']'));

                    // Check logical Expression
                    if (!checkLogicalExp.checkIfCondition(ifCondition, listOfQuestionIdForDupliCheck))
                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax/Incorrect Qid " + ifCondition);


                    //*IF [Q5=1] *GOTO Q2
                    if (pqr[2].Trim().Contains("GOTO"))
                    {
                        myLogicalSyntax = new LogicalSyntax();
                        string[] xyz = pqr[2].Split(' ');
                        if (xyz.Length != 2)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        }
                        else
                        {
                            //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                            //{
                            if (Regex.Match(xyz[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                            {
                                myLogicalSyntax.ThenValue = xyz[1].Trim();
                                myLogicalSyntax.QId = currentQuestion.QId;
                                myLogicalSyntax.LogicTypeId = "3";
                                myLogicalSyntax.IfCondition = ifCondition;

                                //Add in list
                                listOfLogicalSyntaxTemp.Add(myLogicalSyntax);
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                            //}
                            //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        }
                    }
                    //*IF [Q5=1] *MSG "Mobile Number should be correct"
                    else if (pqr[2].Trim().Contains("MSG"))
                    {
                        myLogicalSyntax = new LogicalSyntax();
                        string[] xyz = pqr[2].Split('"');
                        if (xyz.Length != 3)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        }
                        else
                        {
                            //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                            //{
                            //if (Regex.Match(xyz[1].Trim(), "\"[^\"]*\"").Success)
                            //{
                            myLogicalSyntax.ThenValue = xyz[1];
                            myLogicalSyntax.QId = currentQuestion.QId;
                            myLogicalSyntax.LogicTypeId = "2";
                            myLogicalSyntax.IfCondition = ifCondition;

                            //Add in list
                            listOfLogicalSyntaxTemp.Add(myLogicalSyntax);
                            //}
                            //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                            //}
                            //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        }
                    }
                    //*IF [Q5=1] *INCLUDE Q3Dummy [01;03;04;05]
                    else if (pqr[2].Trim().Contains("INCLUDE") || pqr[2].Trim().Contains("EXCLUDE"))
                    {
                        AutoResponse myAutoResponse = new AutoResponse();

                        myAutoResponse.IfCondition = ifCondition;

                        String IncludeExclude = "";
                        if (pqr[2].Trim().Split(' ')[0].Trim().ToUpper() == "INCLUDE")
                            IncludeExclude = "Include";
                        else
                            IncludeExclude = "Exclude";


                        string[] abc = pqr[2].Trim().Split(' ');
                        if (abc.Length != 3 && abc.Length != 5)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
                        }
                        else if (abc.Length == 3)
                        {
                            if (!abc[2].Trim().Contains("["))
                            {
                                //*INCLUDE Q3Dummy Q1
                                if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                                {
                                    if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                                        myAutoResponse.QId = abc[1].Trim();
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                                myAutoResponse.LogicId = "1";

                                if (Regex.Match(abc[2].Trim(), "^[a-zA-Z0-9.]+$").Success)
                                {
                                    if (listOfQuestionIdForDupliCheck.Contains(abc[2].Trim().Split('.')[0]))
                                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim() + ", Must be started with Alpha");
                            }
                            else
                            {
                                // get the QID
                                //*INCLUDE Q3Dummy [01;03;04;05]
                                if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                                {
                                    if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                                        myAutoResponse.QId = abc[1].Trim();
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                                myAutoResponse.LogicId = "1";

                                // get the condition
                                //*INCLUDE Q3Dummy [01;03;04;05]
                                if (Regex.Match(abc[2].Trim(), @"^\[\d+(;\d+)*\]$").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                                }

                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                                }
                                //*INCLUDE Q3Dummy ASCRANKOf[Q2]
                                //*INCLUDE Q3Dummy DSCRANKOf[Q2]
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"ASCRANKOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success ||
                                         Regex.Match(abc[2].Trim().ToUpper(), @"DSCRANKOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                                }
                                //else if (abc[2].Trim() == "DSCRANKOf[Q1AllSalse]")
                                //{
                                //    myAutoResponse.ThenValue = IncludeExclude + "["+abc[2].Trim()+"]";
                                //}
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"SPLITOF\[[a-zA-Z0-9]+[,:]+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"RANDOMVALUEOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (abc[2].Trim().ToUpper().Contains("RANBETWEENOF"))
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINDEXOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUBSTROF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)+(,\d+)+(,\d+)\]").Success ||
                                         Regex.Match(abc[2].Trim().ToUpper(), @"SUBSTROF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"TYPEOF\[INTERVIEW]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"LANGUAGEOF\[INTERVIEW]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"NUMBEROF\[INTERVIEW]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"NUMBEROF\[INTERVIEW,[a-zA-Z]+[a-zA-Z0-9]]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"^NUMBEROF\[INTERVIEW,[A-Z][A-Z0-9]*,[A-Z][A-Z0-9]*\]$").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"DATEOF\[TODAY]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"DAYOF\[TODAY]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"TIMEOF\[NOW]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"TIMEDIFFOF\[[a-zA-Z]+[a-zA-Z0-9]+(,)+[a-zA-Z]+[a-zA-Z0-9]\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[([-]\d+)+(,\d+)\]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[(\d+)+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"TOTALOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUMOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                     Regex.Match(abc[2].Trim().ToUpper(), @"SUMOF\[(([a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,))+)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUBTRACTOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                         Regex.Match(abc[2].Trim().ToUpper(), @"SUBTRACTOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"MULTIPLYOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                         Regex.Match(abc[2].Trim().ToUpper(), @"MULTIPLYOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"DIVIDEOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                         Regex.Match(abc[2].Trim().ToUpper(), @"DIVIDEOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"PANELINFOOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"POSTCODEVALUEOF\[[a-zA-Z]+[a-zA-Z.0-9]+(,\d+)\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEFROM\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEBTNOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                     Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEBTNOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else if (Regex.Match(abc[2].Trim().ToUpper(), @"IDOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                                     Regex.Match(abc[2].Trim().ToUpper(), @"IDOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[2].Trim() + ", Should be [Number to Number]");

                            }
                        }
                        else if (abc.Length == 5)
                        {
                            // get the QID
                            //*INCLUDE Q3Dummy [01 to 05]
                            if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                            {
                                if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                                    myAutoResponse.QId = abc[1].Trim();
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                            }

                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                            myAutoResponse.LogicId = "1";

                            if (abc[2].Trim().Contains("["))
                            {   //*INCLUDE Q3Dummy [01 to 05]
                                string myExp = abc[2] + " " + abc[3] + " " + abc[4];
                                if (Regex.Match(myExp.Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                                {
                                    myAutoResponse.ThenValue = IncludeExclude + myExp.Trim();
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + myExp + ", Should be [Number to Number]");

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline + ", Should be [Number to Number]");

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[1].Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");


                        listOfAutoResponseTemp.Add(myAutoResponse);

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[1].Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");

                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid IF Statement " + strline + ", Format is *IF [Condition] SuccessExpression");


            }
            else if (strline.Trim().Split(' ')[0].ToUpper() == "*IF" && strline.ToUpper().Contains("REGULAREXPOF"))
            {
                //*IF [RegexOf[Q17Mojo.1]!=RegularExpOf[^[1-9]\d*$]] *MSG "Invalid buying price, Must be decimal Number"

                LogicalSyntax myLogicalSyntax;// = new LogicalSyntax();

                string[] pqrTemp = strline.Trim().Split('*');
                string[] pqr = new string[3];
                if (pqrTemp.Length == 3)
                {
                    pqr = pqrTemp;
                }
                else if (pqrTemp.Length == 4)
                {
                    pqr[0] = pqrTemp[0];
                    pqr[1] = pqrTemp[1] + "*" + pqrTemp[2];
                    pqr[2] = pqrTemp[3];

                }
                if (pqr.Length == 3)
                {
                    string[] mno = pqr[1].Split(' ');
                    //string ifCondition = pqr[1].Split(new Char[] { '[', ']' })[1];

                    string ifCondition = pqr[1].Substring(pqr[1].IndexOf('[') + 1);//pqr[1].Split(new Char[] { '[', ']' })[1];
                    ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']'));

                    // Check logical Expression
                    if (!checkLogicalExp.checkIfCondition(ifCondition, listOfQuestionIdForDupliCheck))
                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + ifCondition);


                    if (pqr[2].Trim().Contains("MSG"))
                    {
                        myLogicalSyntax = new LogicalSyntax();
                        string[] xyz = pqr[2].Split('"');
                        if (xyz.Length != 3)
                        {
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        }
                        else
                        {
                            //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                            //{
                            //if (Regex.Match(xyz[1].Trim(), "\"[^\"]*\"").Success)
                            //{
                            myLogicalSyntax.ThenValue = xyz[1];
                            myLogicalSyntax.QId = currentQuestion.QId;
                            myLogicalSyntax.LogicTypeId = "2";
                            myLogicalSyntax.IfCondition = ifCondition;

                            //Add in list
                            listOfLogicalSyntaxTemp.Add(myLogicalSyntax);
                            //}
                            //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                            //}
                            //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        }
                    }
                }
            }
            return i;
        }

        private int prepareIncludeExclude(List<String> lines, int i, List<string> listOfQuestionIdForDupliCheck, List<AutoResponse> listOfAutoResponseTemp, TextWriter txtWriter, Dictionary<int, int> dicLine)
        {
            String strline = lines[i];
            AutoResponse myAutoResponse = new AutoResponse();

            String IncludeExclude = "";
            if (strline.Trim().Split(' ')[0].Trim().ToUpper() == "*INCLUDE")
                IncludeExclude = "Include";
            else
                IncludeExclude = "Exclude";


            string[] abc = strline.Trim().Split(' ');
            if (abc.Length != 3 && abc.Length != 5)
            {
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
            }
            else if (abc.Length == 3)
            {
                if (!abc[2].Trim().Contains("["))
                {
                    //*INCLUDE Q3Dummy Q1
                    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                    {
                        if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            myAutoResponse.QId = abc[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                    myAutoResponse.LogicId = "1";

                    if (Regex.Match(abc[2].Trim(), "^[a-zA-Z0-9.]+$").Success)
                    {
                        if (listOfQuestionIdForDupliCheck.Contains(abc[2].Trim().Split('.')[0]))
                            myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim() + ", Must be started with Alpha");
                }
                else
                {
                    // get the QID
                    //*INCLUDE Q3Dummy [01;03;04;05]
                    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                    {
                        if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            myAutoResponse.QId = abc[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                    myAutoResponse.LogicId = "1";


                    // get the condition
                    //*INCLUDE Q3Dummy [01;03;04;05]
                    if (Regex.Match(abc[2].Trim(), @"^\[\d+(;\d+)*\]$").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                    }
                    //*INCLUDE Q3Dummy [1 TO 5]
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                    }
                    //*INCLUDE Q3Dummy ASCRANKOf[Q2]
                    //*INCLUDE Q3Dummy DSCRANKOf[Q2]
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"ASCRANKOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success ||
                             Regex.Match(abc[2].Trim().ToUpper(), @"DSCRANKOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    //else if (abc[2].Trim() == "DSCRANKOf[Q1AllSalse]")
                    //{
                    //    myAutoResponse.ThenValue = IncludeExclude + "["+abc[2].Trim()+"]";
                    //}
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"SPLITOF\[[a-zA-Z0-9]+[,:]+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"RANDOMVALUEOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (abc[2].Trim().ToUpper().Contains("RANBETWEENOF"))
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINDEXOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUBSTROF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)+(,\d+)+(,\d+)\]").Success ||
                             Regex.Match(abc[2].Trim().ToUpper(), @"SUBSTROF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"TYPEOF\[INTERVIEW]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"LANGUAGEOF\[INTERVIEW]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"NUMBEROF\[INTERVIEW]").Success || 
                        Regex.Match(abc[2].Trim().ToUpper(), @"NUMBEROF\[INTERVIEW,[a-zA-Z]+[a-zA-Z0-9]]").Success ||
                        Regex.Match(abc[2].Trim().ToUpper(), @"^NUMBEROF\[INTERVIEW,[A-Z][A-Z0-9]*,[A-Z][A-Z0-9]*\]$").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"DATEOF\[TODAY]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"DAYOF\[TODAY]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"TIMEOF\[NOW]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"TIMEDIFFOF\[[a-zA-Z]+[a-zA-Z0-9]+(,)+[a-zA-Z]+[a-zA-Z0-9]\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[([-]\d+)+(,\d+)\]").Success ||
                                        Regex.Match(abc[2].Trim().ToUpper(), @"VALUEINPOSITIONOF\[(\d+)+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"TOTALOF\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUMOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                         Regex.Match(abc[2].Trim().ToUpper(), @"SUMOF\[(([a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,))+)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"SUBTRACTOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                             Regex.Match(abc[2].Trim().ToUpper(), @"SUBTRACTOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"MULTIPLYOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                             Regex.Match(abc[2].Trim().ToUpper(), @"MULTIPLYOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"DIVIDEOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                             Regex.Match(abc[2].Trim().ToUpper(), @"DIVIDEOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"PANELINFOOF\[[a-zA-Z]+[a-zA-Z0-9]+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"POSTCODEVALUEOF\[[a-zA-Z]+[a-zA-Z.0-9]+(,\d+)\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEFROM\[[a-zA-Z]+[a-zA-Z0-9]+\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEBTNOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,\d+)\]").Success ||
                         Regex.Match(abc[2].Trim().ToUpper(), @"DISTANCEBTNOF\[[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?(,)+[a-zA-Z]+[a-zA-Z0-9]+(.\d+)?\]").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[2].Trim() + ", Should be [Number to Number]");

                }
            }
            else if (abc.Length == 5)
            {
                //*INCLUDE Q3Dummy Q1
                if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                {
                    if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        myAutoResponse.QId = abc[1].Trim();
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                myAutoResponse.LogicId = "1";

                if (abc[2].Trim().Contains("["))
                {   //*INCLUDE Q3Dummy [01 to 05]
                    string myExp = abc[2] + " " + abc[3] + " " + abc[4];
                    if (Regex.Match(myExp.Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                    {
                        myAutoResponse.ThenValue = IncludeExclude + myExp.Trim();
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + myExp + ", Should be [Number to Number]");

                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline + ", Should be [Number to Number]");

            }
            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline.Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");

            listOfAutoResponseTemp.Add(myAutoResponse);

            return i;
        }

        private int prepareQuestion(List<String> lines, int i, List<string> listOfQuestionIdForDupliCheck, List<string> listOfGridListForDupliCheck, List<LogicalSyntax> listOfLogicalSyntaxTemp, List<Question> listOfQuestionTemp, Question currentQuestionTemp, Dictionary<String, List<AttributeMain>> dicQidVsAttributeListTemp, List<AttributeFilter> listOfAttributeFilterTemp, TextWriter txtWriter, Dictionary<int, int> dicLine)
        {
            AttributeMain attributeMain1 = new AttributeMain();
            AttributeMain attributeMain2 = new AttributeMain();

            AttributeMain attributeMainFIName = new AttributeMain();
            AttributeMain attributeMainFICode = new AttributeMain();
            AttributeMain attributeMainFSName = new AttributeMain();
            AttributeMain attributeMainFSCode = new AttributeMain();
            AttributeMain attributeSingleDropDown = new AttributeMain();

            String strline = lines[i];

            bool hasDKCS = false;
            bool hasFIFS = false;
            bool hasSingleDropdown = false;

            Question myQuestion = new Question();
            AttributeFilter myAttributeFilter = new AttributeFilter();
            string[] word = strline.Split('*');
            int QTypeCounter = 0;
            List<string> listOfQuestionProperties = new List<string>();
            String currentGridListName = "";
            String GridFilterQId = "";
            String GridFilterType = "";
            int qTypeForGridQid = 0;
            String qIdForGridQid = "";

            #region Question Properties
            for (int n = 1; n < word.Length; n++)
            {
                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                    txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                string myText = "*" + word[n];

                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                {
                    qTypeForGridQid = 0;
                    //QID
                    string[] xyz = myText.Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                        {
                            if (!listOfQuestionIdForDupliCheck.Contains(xyz[1].Trim()))
                            {
                                if (!listOfQuestionIdForReject.Contains(xyz[1].Trim().ToUpper()))
                                {
                                    qIdForGridQid = xyz[1].Trim();
                                    myQuestion.QId = xyz[1].Trim();
                                    listOfQuestionIdForDupliCheck.Add(xyz[1].Trim());

                                    //if (myQuestion.QId == "SQ21")
                                    //    MessageBox.Show("");
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + xyz[1].Trim() + " should not be used as QId");
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                }
                //Question Type
                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                { myQuestion.QType = "1"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                { myQuestion.QType = "2"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                { myQuestion.QType = "3"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMBER") && !myText.ToUpper().Trim().Contains("*NUMBEROFRESPONSE"))
                { myQuestion.QType = "4"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*RANK"))
                { myQuestion.QType = "5"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*IMAGE") && !myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                {
                    myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    //string[] xy = word[n].Trim().Split(' ');
                    //if (xy.Length == 2)
                    //{
                    //    string[] xyz = xy[1].Split('"');
                    //    if (xyz.Length != 3)
                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                    //    else
                    //        myQuestion.FilePath = xyz[1];

                    //}
                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *IMAGE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                {
                    myQuestion.QType = "7"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SCALE7"))
                {
                    myQuestion.QType = "32"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SCALE10"))
                {
                    myQuestion.QType = "61"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                {
                    myQuestion.QType = "8"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    qTypeForGridQid = 8;
                }
                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                { myQuestion.QType = "9"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                { myQuestion.QType = "10"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                { myQuestion.QType = "12"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                { myQuestion.QType = "13"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DATE"))
                { myQuestion.QType = "14"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*TIME"))
                { myQuestion.QType = "15"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                { myQuestion.QType = "16"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                { myQuestion.QType = "17"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("*AUTOCOMPLETELIST") && !myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                {
                    myQuestion.QType = "22"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = true;
                }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETELIST"))
                {
                    myQuestion.QType = "22"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = false;
                }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                { myQuestion.QType = "23"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                {
                    myQuestion.QType = "24"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = true;
                }
                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                { myQuestion.QType = "24"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DRAGDROP"))
                { myQuestion.QType = "26"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GRIDNUM"))
                {
                    myQuestion.QType = "27"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    qTypeForGridQid = 8;
                }
                else if (myText.ToUpper().Trim().Contains("*MAXDIFF"))
                { myQuestion.QType = "40"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GPS"))
                { myQuestion.QType = "41"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*FORM") && !myText.ToUpper().Trim().Contains("*SHOWASFORM"))
                {
                    myQuestion.QType = "48";
                    QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*INFO"))
                { myQuestion.QType = "49"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                { myQuestion.QType = "50"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); hasEnd = true; }
                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                { myQuestion.QType = "51"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); hasTerminate = true; }
                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                {
                    myQuestion.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    attributeMainFIName = new AttributeMain();
                    attributeMainFIName.QId = myQuestion.QId;
                    attributeMainFIName.AttributeEnglish = "FI Name";
                    attributeMainFIName.AttributeValue = "1";
                    attributeMainFIName.AttributeOrder = "1";
                    attributeMainFIName.LinkId1 = "3";
                    attributeMainFIName.ForceAndMsgOpt = "11";

                    attributeMainFICode = new AttributeMain();
                    attributeMainFICode.QId = myQuestion.QId;
                    attributeMainFICode.AttributeEnglish = "FI Code";
                    attributeMainFICode.AttributeValue = "2";
                    attributeMainFICode.AttributeOrder = "2";
                    attributeMainFICode.LinkId1 = "3";
                    attributeMainFICode.ForceAndMsgOpt = "11";

                    attributeMainFSName = new AttributeMain();
                    attributeMainFSName.QId = myQuestion.QId;
                    attributeMainFSName.AttributeEnglish = "FS Name";
                    //attributeMainFSName.AttributeEnglish = "FI Mobile No";
                    attributeMainFSName.AttributeValue = "3";
                    attributeMainFSName.AttributeOrder = "3";
                    attributeMainFSName.LinkId1 = "3";
                    attributeMainFSName.ForceAndMsgOpt = "11";

                    attributeMainFSCode = new AttributeMain();
                    attributeMainFSCode.QId = myQuestion.QId;
                    attributeMainFSCode.AttributeEnglish = "FS Code";
                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                    attributeMainFSCode.AttributeValue = "4";
                    attributeMainFSCode.AttributeOrder = "4";
                    attributeMainFSCode.LinkId1 = "3";
                    attributeMainFSCode.ForceAndMsgOpt = "11";

                    hasFIFS = true;
                }


                //else if (word[n].ToUpper().Trim().Contains(""))
                //    myQuestion.QType = "7";
                //else if (word[n].ToUpper().Trim().Contains(""))
                //    myQuestion.QType = "7";
                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                { myQuestion.HasRandomAttrib = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ROT") && !myText.ToUpper().Trim().Contains("*QROT") && !myText.ToUpper().Trim().Contains("*GROUPROT") && !myText.ToUpper().Trim().Contains("*OTPGROUPROT") && !myText.ToUpper().Trim().Contains("*OTPROTGROUP") && !myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                { myQuestion.HasRandomAttrib = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Group Rot Not Option
                else if (myText.ToUpper().Trim().Contains("*OTPGROUPROT"))
                { myQuestion.HasRandomAttrib = "10"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Optioin Rot Not Group
                else if (myText.ToUpper().Trim().Contains("*OTPROTGROUP"))
                { myQuestion.HasRandomAttrib = "01"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Group Rot Option Rot
                else if (myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                { myQuestion.HasRandomAttrib = "11"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*FROT"))
                { myQuestion.HasRandomAttrib = "5"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }

                else if (myText.ToUpper().Trim().Contains("*QROT"))
                { myQuestion.HasRandomQntr = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GROUPROT") && !myText.ToUpper().Trim().Contains("*OTPGROUPROT") && !myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.HasMessageLogic = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *GROUPROT is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*FONTSIZE"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.WrittenOEInPaper = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *FONTSIZE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*MIN"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                            myQuestion.NoOfResponseMin = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                            myQuestion.NoOfResponseMax = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MAX is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.NumberOfColumn = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *COLUMN is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*IMGADJBY"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.HasMediaPath = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *IMGADJ is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*JUMPFOR"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.ResumeQntrJump = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *JUMPFOR is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.ResumeQntrJump = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *BLOCK is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*HORIZONTAL"))
                { myQuestion.NumberOfColumn = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*TAKEONLYONE"))
                { myQuestion.NumberOfColumn = "4"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                { myQuestion.HasAutoResponse = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                { myQuestion.HasAutoResponse = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                        {
                            myQuestion.ShowInReport = xyz[1].Trim();
                            //listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *DELAY is invalid "); 
                    
                    
                }
                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                { myQuestion.DisplayBackButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                { myQuestion.DisplayNextButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GROT"))
                { myQuestion.ForceToTakeOE = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GRANDOM"))
                { myQuestion.ForceToTakeOE = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS1"))
                { myQuestion.DisplayJumpButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS2"))
                { myQuestion.DisplayJumpButton = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS3"))
                { myQuestion.DisplayJumpButton = "3"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS4"))
                { myQuestion.DisplayJumpButton = "4"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*SHOWASFORM"))
                {
                    myQuestion.NumberOfColumn = "3"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*DIRIMAGE"))
                {
                    myQuestion.WrittenOEInPaper = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SHOWASNUMTEXT"))
                {
                    myQuestion.WrittenOEInPaper = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*INRLD"))
                {
                    myQuestion.ForceToTakeOE = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*PICT"))
                {
                    //myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    string[] xy = word[n].Trim().Split(' ');
                    if (xy.Length == 2)
                    {
                        string[] xyz = xy[1].Split('"');
                        if (xyz.Length != 3)
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                        else
                            myQuestion.FilePath = xyz[1];

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*VIDEO"))
                {
                    //myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    string[] xy = word[n].Trim().Split(' ');
                    if (xy.Length == 2)
                    {
                        string[] xyz = xy[1].Split('"');
                        if (xyz.Length != 3)
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                        else
                            myQuestion.FilePath = xyz[1];

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*QLABEL"))
                {
                    //myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    string[] xy = word[n].Trim().Split('"');
                    if (xy.Length == 3)
                    {
                        string[] xyz = xy[1].Split('"');
                        if (xyz.Length != 1)
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax, Double Quatetion should not be here " + xyz[1]);
                        else
                            myQuestion.Comments = xyz[0];

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *QLABEL is invalid, Double Quatetion should not be here ");

                }

                //************************************* Attribute Filter ***********************************************
                else if (myText.ToUpper().Trim().Contains("*INCLUDE") && !myText.ToUpper().Trim().Contains("*INCLUDEGRIDLIST") && !myText.ToUpper().Trim().Contains("*INCLUDEBYORDER"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                myAttributeFilter.ProjectId = projectInfoScript.ProjectCode;
                                myAttributeFilter.QId = myQuestion.QId;
                                myAttributeFilter.InheritedQId = abc[1].Trim();
                                myAttributeFilter.FilterType = "1";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*INCLUDEBYORDER"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                myAttributeFilter.ProjectId = projectInfoScript.ProjectCode;
                                myAttributeFilter.QId = myQuestion.QId;
                                myAttributeFilter.InheritedQId = abc[1].Trim();
                                myAttributeFilter.FilterType = "5";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                myAttributeFilter.ProjectId = projectInfoScript.ProjectCode;
                                myAttributeFilter.QId = myQuestion.QId;
                                myAttributeFilter.InheritedQId = abc[1].Trim();
                                myAttributeFilter.FilterType = "2";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });

                        currentGridListName = abc[1].Trim();

                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfGridListForDupliCheck.Contains(abc[1].Trim()))
                            {
                                if (hasSingleDropdown == true)
                                {
                                    attributeSingleDropDown = new AttributeMain();
                                    attributeSingleDropDown.QId = myQuestion.QId;
                                    attributeSingleDropDown.AttributeEnglish = "";
                                    attributeSingleDropDown.AttributeValue = "1";
                                    attributeSingleDropDown.AttributeOrder = "1";
                                    attributeSingleDropDown.LinkId1 = "1";
                                    attributeSingleDropDown.LinkId2 = currentGridListName;
                                    attributeSingleDropDown.ForceAndMsgOpt = "11";
                                }
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid GridListName : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " GridList Name must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *USEGRIDLIST is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*INCLUDEGRIDLIST"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                GridFilterQId = abc[1].Trim();
                                GridFilterType = "1";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                {
                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                    if (xyz.Length == 5)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                        if (xyz[1].Trim() != "")
                        {
                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                            {
                                hasDKCS = true;

                                attributeMain1 = new AttributeMain();
                                //attributeMain1.QId = currentQuestion.QId;
                                attributeMain1.QId = myQuestion.QId;
                                attributeMain1.AttributeEnglish = "";
                                attributeMain1.AttributeValue = "1";
                                attributeMain1.AttributeOrder = "1";
                                attributeMain1.MinValue = "5";

                                attributeMain2 = new AttributeMain();
                                //attributeMain2.QId = currentQuestion.QId;
                                attributeMain2.QId = myQuestion.QId;
                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                attributeMain2.AttributeValue = xyz[3].Trim();
                                attributeMain2.AttributeOrder = "2";
                                attributeMain2.IsExclusive = "1";



                                //Add the attribute list 
                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute code must be Number " + xyz[3].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute Label missing " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *DKCS is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("IF"))
                {
                    LogicalSyntax myLogicalSyntax = new LogicalSyntax();

                    //string ifCondition = word[n].Split(new Char[] { '[', ']' })[1];

                    string ifCondition = word[n].Substring(word[n].IndexOf('[') + 1);
                    ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']'));

                    // Check logical Expression
                    if (!checkLogicalExp.checkIfCondition(ifCondition, listOfQuestionIdForDupliCheck))
                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + ifCondition);

                    myLogicalSyntax.ThenValue = myQuestion.QId;
                    myLogicalSyntax.QId = myQuestion.QId;
                    myLogicalSyntax.LogicTypeId = "4";
                    myLogicalSyntax.IfCondition = ifCondition;

                    //Add in list
                    listOfLogicalSyntaxTemp.Add(myLogicalSyntax);
                }
                //************************************* End of Attribute Filter ****************************************

            }
            #endregion

            //Check Question properties is duplicate or not

            var query = listOfQuestionProperties.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();

            if (query.Count > 0)
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate Token ");

            if (myQuestion.QType == null)
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Question Type must be exist ");

            string questionText = "";
            strline = lines[++i];
            bool getquestionText = false;
            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
            {
                questionText = questionText + strline + "<br>";
                strline = lines[++i];
                getquestionText = true;
            }

            if (questionText == "")
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid Question Text : should not exist");
            myQuestion.QuestionEnglish = questionText;
            myQuestion.SilentRecording = silentRecording;

            //add question object to list
            listOfQuestionTemp.Add(myQuestion);
            currentQuestionTemp = myQuestion;


            //****************************************************this portion is for question attribute ********************************

            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
            {
                if (i < lines.Count - 1)
                    i--;
            }

            List<AttributeMain> listOfAttributeMain = new List<AttributeMain>();
            int index = 1;

            List<String> listOfAttributeValueForDupliCheck = new List<string>();
            List<String> listOfAttributeLabelForDupliCheck = new List<string>();

            if (hasDKCS == true)
            {
                listOfAttributeMain.Add(attributeMain1);
                listOfAttributeMain.Add(attributeMain2);
                hasDKCS = false;
            }

            if (hasFIFS == true)
            {
                listOfAttributeMain.Add(attributeMainFIName);
                listOfAttributeMain.Add(attributeMainFICode);
                listOfAttributeMain.Add(attributeMainFSName);
                listOfAttributeMain.Add(attributeMainFSCode);

                hasFIFS = false;
            }

            if (hasSingleDropdown == true)
            {
                listOfAttributeMain.Add(attributeSingleDropDown);
                hasSingleDropdown = false;
            }

            #region USEGRIDLIST
            //This is for single Dropdown

            #endregion

            #region USELIST



            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
            {
                string[] word1 = strline.Split(' ');
                if (word1.Length == 2)
                {
                    if (word1[1].Split('"').Length == 3)
                    {
                        if (dicListNameVsList.ContainsKey(word1[1].Split('"')[1].Trim()))
                        {
                            if (dicListNameVsList.ContainsKey(word1[1].Split('"')[1].Trim()))
                            {
                                List<AttributeMain> listOfAttributeTemp = new List<AttributeMain>(dicListNameVsList[word1[1].Split('"')[1].Trim()]);

                                if (myQuestion.QType == "7" || myQuestion.QType == "26" || myQuestion.QType == "40" || myQuestion.QType == "61")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {


                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;


                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "1";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;

                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");

                                }
                                else if (myQuestion.QType == "8")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "2";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");


                                }
                                else if (myQuestion.QType == "22")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "22";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                                }
                                else if (myQuestion.QType == "27")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "27";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                                }
                                else
                                {
                                    //Start pronab added
                                    for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                    {
                                        if (listOfAttributeTemp[x].AttributeEnglish != null)
                                        {
                                            if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                            {
                                                listOfAttributeMain.Add(listOfAttributeTemp[x]);
                                                index++;
                                            }
                                            else
                                            {
                                                AttributeMain attributeMain = new AttributeMain();
                                                //*********** If grid attribute has property ********************

                                                #region Attribute Properties
                                                string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                                attributeMain.AttributeEnglish = myKey[0].Trim();
                                                attributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                                attributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;

                                                for (int n = 1; n < myKey.Length; n++)
                                                {
                                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN") || ("*" + myKey[n]).ToUpper().Trim().Contains("*MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                    {
                                                        if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                                        if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN"))
                                                        {
                                                            string[] xyz = myKey[n].Split(' ');
                                                            if (xyz.Length >= 2)
                                                            {
                                                                if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                                    attributeMain.MinValue = xyz[1].Trim();
                                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                                            }
                                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                                        }
                                                        else if (("*" + myKey[n]).ToUpper().Trim().Contains("*MAX"))
                                                        {
                                                            string[] xyz = myKey[n].Split(' ');
                                                            if (xyz.Length >= 2)
                                                            {
                                                                if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                                    attributeMain.MaxValue = xyz[1].Trim();
                                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                                            }
                                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                                        }
                                                    }
                                                    else if (myKey[n].ToUpper().Trim().Contains("PICT"))
                                                    {
                                                        string[] xy = myKey[n].Trim().Split(' ');
                                                        if (xy.Length == 2)
                                                        {
                                                            listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                                            string[] xyz = xy[1].Split('"');
                                                            if (xyz.Length != 3)
                                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                                            else
                                                            {
                                                                if (myQuestion.QType == "17")
                                                                    attributeMain.Comments = xyz[1].Trim();
                                                                else
                                                                    attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                                            }
                                                        }
                                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");

                                                    }
                                                    else if (myKey[n].ToUpper().Trim().Contains("VIDEO"))
                                                    {
                                                        string[] xy = myKey[n].Trim().Split(' ');
                                                        if (xy.Length == 2)
                                                        {
                                                            listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                                            string[] xyz = xy[1].Split('"');
                                                            if (xyz.Length != 3)
                                                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                                            else
                                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                                        }
                                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");

                                                    }
                                                    else
                                                    {
                                                        if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim()))
                                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                                        if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                        {
                                                            attributeMain.TakeOpenended = "1";
                                                            if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " OPEN Should not be exist for Form attribute");
                                                        }
                                                        else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                        {
                                                            attributeMain.IsExclusive = "1";
                                                            if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NMUL Should not be exist for Form attribute");

                                                        }
                                                        else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                            attributeMain.ForceAndMsgOpt = "11";
                                                        else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                        {
                                                            if (attributeMain.IsExclusive == "" || attributeMain.IsExclusive == null)
                                                                attributeMain.IsExclusive = "2";

                                                            if (myAttributeFilter.ExceptionalValue == null)
                                                                myAttributeFilter.ExceptionalValue = "";

                                                            myAttributeFilter.ExceptionalValue = myAttributeFilter.ExceptionalValue + attributeMain.AttributeValue + ",";

                                                            if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NOCON Should not be exist for Form attribute");
                                                        }
                                                        else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                            attributeMain.LinkId1 = "1";
                                                        else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                            attributeMain.LinkId1 = "2";
                                                        else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                            attributeMain.LinkId1 = "3";
                                                        else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                            attributeMain.LinkId1 = "4";
                                                        else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                            attributeMain.LinkId1 = "14";
                                                        else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                            attributeMain.LinkId1 = "15";
                                                        else if (myKey[n].ToUpper().Trim().Contains("AUTOCOMPLETE"))
                                                            attributeMain.LinkId1 = "22";
                                                        else if (myKey[n].ToUpper().Trim().Contains("DROPDOWN"))
                                                            attributeMain.LinkId1 = "24";

                                                    }
                                                }
                                                #endregion

                                                listOfAttributeMain.Add(attributeMain);


                                                index++;

                                            }
                                        }
                                        //End pronab added
                                    }
                                }


                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //index = listOfAttributeMain.Count + 1;
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                            for (int x = 0; x < listOfAttributeMain.Count; x++)
                            {
                                listOfAttributeValueForDupliCheck.Add(listOfAttributeMain[x].AttributeValue);
                                listOfAttributeLabelForDupliCheck.Add(listOfAttributeMain[x].AttributeEnglish);
                            }

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                strline = lines[++i];
                strline = lines[++i];

            }
            #endregion

            #region Attribute with :
            if (isAttribute(strline))
            {
                while (!strline.Trim().Substring(0, 1).Contains("*"))
                {

                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                    if (strline.Contains(":"))
                    {
                        AttributeMain attributeMain = new AttributeMain();
                        String[] myWord = strline.Split(':');

                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                        {
                            if (!listOfAttributeValueForDupliCheck.Contains(myWord[0].Trim()))
                            {
                                attributeMain.AttributeValue = myWord[0].Trim();
                                attributeMain.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                index++;

                                //Add value in list
                                listOfAttributeValueForDupliCheck.Add(myWord[0].Trim());
                                //ismile
                                //Add qid with value label for grid attribute condition
                                if (qTypeForGridQid == 8)
                                    listOfQuestionIdForDupliCheck.Add(qIdForGridQid + "_" + myWord[0].Trim());


                            }//else {Error Message}

                        }//else {Error Message}
                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);

                        if (myQuestion.QType == "48")
                        {
                            if (mylabel.Contains("*SR") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*MR") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*DROPDOWN") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*AUTOCOMPLETE") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            //if (mylabel.Contains("*NUMBER") & !mylabel.Contains("*MIN")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " MIN must be exist for Form attribute");
                            //if (mylabel.Contains("*NUMBER") & !mylabel.Contains("*MAX")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " MAX must be exist for Form attribute");
                        }


                        if (!mylabel.Contains("*"))
                        {
                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                            //{

                            if (myQuestion.QType == "48")
                            {
                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Form attribute must have a type");
                            }


                            attributeMain.AttributeEnglish = mylabel.Trim();

                            if (myQuestion.QType == "7" || myQuestion.QType == "22" || myQuestion.QType == "24" || myQuestion.QType == "26" || myQuestion.QType == "40" || myQuestion.QType == "61")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "1";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;

                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + "List unavailable");
                            }
                            else if (myQuestion.QType == "8")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "2";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                            }
                            else if (myQuestion.QType == "27")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "27";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                            }
                            //else if (myQuestion.QType == "22")
                            //{
                            //    if (currentGridListName != "")
                            //    {

                            //        attributeMain.LinkId1 = "22";
                            //        attributeMain.LinkId2 = currentGridListName;

                            //        attributeMain.FilterQid = GridFilterQId;
                            //        attributeMain.FilterType = GridFilterType;
                            //    }
                            //    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            //}

                            //Add value in list
                            listOfAttributeLabelForDupliCheck.Add(mylabel.Trim().ToUpper());
                            //}//else {Error Message}
                        }
                        else
                        {
                            // *********** If grid attribute has property ********************
                            if (myQuestion.QType == "7" || myQuestion.QType == "22" || myQuestion.QType == "24" || myQuestion.QType == "26")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "1";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                            }
                            else if (myQuestion.QType == "8")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "2";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " list unavailable");
                            }
                            else if (myQuestion.QType == "27")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "27";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " list unavailable");
                            }
                            else if (myQuestion.QType == "22")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "22";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " list unavailable");
                            }

                            //*****************************************************


                            //If attribute have properties
                            #region Attribute Properties
                            string[] myKey = mylabel.Split('*');

                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                            //{

                            attributeMain.AttributeEnglish = myKey[0].Trim();
                            //}


                            for (int n = 1; n < myKey.Length; n++)
                            {
                                if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN") || ("*" + myKey[n]).ToUpper().Trim().Contains("*MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MinValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                    }
                                    else if (("*" + myKey[n]).ToUpper().Trim().Contains("*MAX"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MaxValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                    {
                                        string[] xyz = myKey[n].Trim().Split(' ');
                                        if (xyz.Length == 2)
                                        {
                                            listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                            string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                            if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                                            {
                                                if (listOfGridListForDupliCheck.Contains(abc[1].Trim()))
                                                {
                                                    currentGridListName = abc[1].Trim();
                                                    attributeMain.LinkId2 = currentGridListName;
                                                }
                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid GridListName : " + abc[1].Trim());
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " GridList Name must be followed by Alpha Charecter " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *USEGRIDLIST is invalid ");
                                    }
                                }
                                else if (("*" + myKey[n]).ToUpper().Trim().Contains("*LAT") || ("*" + myKey[n]).ToUpper().Trim().Contains("*LON") || myKey[n].ToUpper().Trim().Contains("COMPVAL"))
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*LAT"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MinValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                    }
                                    else if (("*" + myKey[n]).ToUpper().Trim().Contains("*LON"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MaxValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("COMPVAL"))
                                    {
                                        string[] xyz = myKey[n].Trim().Split(' ');
                                        if (xyz.Length == 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                            {
                                                attributeMain.ExcepValue = xyz[1].Trim();
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *COMPVAL is invalid ");
                                    }
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("INCLUDE"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9.]+$").Success)
                                        {
                                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Split('.')[0].Trim()))
                                            {
                                                if (qTypeForGridQid == 8)
                                                    attributeMain.FilterQid = abc[1].Trim() + "_" + attributeMain.AttributeValue;    //filter QID
                                                else
                                                    attributeMain.FilterQid = abc[1].Trim();

                                                attributeMain.FilterType = "1";
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *INCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("EXCLUDE"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9.]+$").Success)
                                        {
                                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Split('.')[0].Trim()))
                                            {
                                                if (qTypeForGridQid == 8)
                                                    attributeMain.FilterQid = abc[1].Trim() + "_" + attributeMain.AttributeValue;    //filter QID
                                                else
                                                    attributeMain.FilterQid = abc[1].Trim();

                                                attributeMain.FilterType = "2";
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("EXCEPT"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[0-9]+$").Success)
                                        {
                                            attributeMain.ExcepValue = abc[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *INCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("PICT"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                        {
                                            if (myQuestion.QType == "17")
                                                attributeMain.Comments = xyz[1].Trim();
                                            else
                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("VIDEO"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                            attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("GROUPNAME"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                            attributeMain.GroupName = xyz[1].Trim();
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("GROUPHEAD"))
                                {
                                    attributeMain.Comments = "GroupHead";

                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                    //    else
                                    //        attributeMain.GroupName = xyz[1].Trim();
                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim()))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                    {
                                        attributeMain.TakeOpenended = "1";
                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " OPEN Should not be exist for Form attribute");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                    {
                                        attributeMain.IsExclusive = "1";
                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NMUL Should not be exist for Form attribute");

                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                        attributeMain.ForceAndMsgOpt = "11";
                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                    {
                                        if (attributeMain.IsExclusive == "" || attributeMain.IsExclusive == null)
                                            attributeMain.IsExclusive = "2";

                                        if (myAttributeFilter.ExceptionalValue == null)
                                            myAttributeFilter.ExceptionalValue = "";

                                        myAttributeFilter.ExceptionalValue = myAttributeFilter.ExceptionalValue + attributeMain.AttributeValue + ",";

                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NOCON Should not be exist for Form attribute");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                        attributeMain.LinkId1 = "1";
                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                        attributeMain.LinkId1 = "2";
                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                        attributeMain.LinkId1 = "3";
                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                        attributeMain.LinkId1 = "4";
                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                        attributeMain.LinkId1 = "14";
                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                        attributeMain.LinkId1 = "15";
                                    else if (myKey[n].ToUpper().Trim().Contains("AUTOCOMPLETE"))
                                        attributeMain.LinkId1 = "22";
                                    else if (myKey[n].ToUpper().Trim().Contains("DROPDOWN"))
                                        attributeMain.LinkId1 = "24";


                                }
                            }
                            #endregion

                        }

                        //Add the attribute in 
                        listOfAttributeMain.Add(attributeMain);

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid syntax x");

                    if (i < lines.Count - 1)
                    {
                        strline = lines[++i];
                    }
                }

                if (i < lines.Count - 1)
                    i--;
            }
            else
            {
                if (i < lines.Count - 1)
                    i--;
            }

            #endregion

            if (myQuestion.QId != null)
                dicQidVsAttributeListTemp.Add(myQuestion.QId, listOfAttributeMain);
            else
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Question Id missing");

            if (myAttributeFilter.QId != null)
                listOfAttributeFilterTemp.Add(myAttributeFilter);


            return i;

        }


        private int prepareQuestionForLanguage(List<String> linesLanguageX, int i, TextWriter txtWriter, Dictionary<int, int> dicLine, int ln1, int languageNo)
        {
            //Pronab added for repeat
            List<AttributeMain> listOfAttributeTempLanX = new List<AttributeMain>();

            List<string> listOfQuestionIdForDupliCheckLanX = new List<string>();
            List<string> listOfGridListForDupliCheckLanX = new List<string>();


            bool hasRepeat = false;

            String strline = linesLanguageX[i];

            string[] word = strline.Split('*');

            //Pronab
            for (int n = 1; n < word.Length; n++)
            {
                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                    txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                string myText = "*" + word[n];


                if (myText.ToUpper().Trim().Contains("*REPEAT"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            listOfAttributeTempLanX = dicQidVsAttributeListLan1[abc[1].Trim()];
                        }
                    }
                    hasRepeat = true;
                }
                else
                {
                    hasRepeat = false;
                }

            }


            if (hasRepeat)
            {
                //int iStart = i;
                //for (int k = 0; k < listOfAttributeTempLan1.Count; k++)
                //{
                //    if (listOfAttributeTempLan1[k].AttributeValue.Contains("99")) //Pronab Need to work on this for local language
                //        break;


                //    AttributeMain attributeMainR = new AttributeMain();
                //    attributeMainR.AttributeEnglish = listOfAttributeTempLan1[k].AttributeEnglish;
                //    attributeMainR.AttributeValue = listOfAttributeTempLan1[k].AttributeValue;

                //    currentQuestion = new Question();

                //    List<LogicalSyntax> listOfLogicalSyntaxTemp = new List<LogicalSyntax>();
                //    List<Question> listOfQuestionTemp = new List<Question>();
                //    Question currentQuestionTemp = new Question();

                //    Dictionary<String, List<AttributeMain>> dicQidVsAttributeListTempLan1 = new Dictionary<String, List<AttributeMain>>();
                //    List<AttributeFilter> listOfAttributeFilterTempLan1 = new List<AttributeFilter>();

                //    i = iStart;

                //    i = this.prepareQuestionLan1(linesLanguage1, i, listOfQuestionIdForDupliCheckLan1, listOfGridListForDupliCheckLan1, listOfLogicalSyntaxTemp, listOfQuestionTemp, currentQuestionTemp, dicQidVsAttributeListTempLan1, listOfAttributeFilterTempLan1, txtWriter, dicLine, attributeMainR, ln1);
                //    strline = linesLanguage1[i];

                //    for (int x = 0; x < listOfLogicalSyntaxTemp.Count; x++)
                //    {
                //        listOfLogicalSyntax.Add(listOfLogicalSyntaxTemp[x]);
                //    }

                //    for (int x = 0; x < listOfQuestionTemp.Count; x++)
                //    {
                //        listOfQuestionLan1.Add(listOfQuestionTemp[x]);
                //    }
                //    currentQuestion = listOfQuestionTemp[0];
                //    //currentQuestion = currentQuestionTemp;

                //    foreach (KeyValuePair<String, List<AttributeMain>> pair in dicQidVsAttributeListTempLan1)
                //    {
                //        dicQidVsAttributeListLan1.Add(pair.Key, pair.Value);
                //    }


                //    for (int x = 0; x < listOfAttributeFilterTempLan1.Count; x++)
                //    {
                //        listOfAttributeFilter.Add(listOfAttributeFilterTempLan1[x]);
                //    }

                //}

            }//Pronab
            else
            {
                AttributeMain attributeMain1 = new AttributeMain();
                AttributeMain attributeMain2 = new AttributeMain();

                AttributeMain attributeMainFIName = new AttributeMain();
                AttributeMain attributeMainFICode = new AttributeMain();
                AttributeMain attributeMainFSName = new AttributeMain();
                AttributeMain attributeMainFSCode = new AttributeMain();

                Boolean hasDKCS = false;

                Question currentQuestionLanX = new Question();
                Question myQuestionLanX = new Question();
                AttributeFilter myAttributeFilterX = new AttributeFilter();

                int QTypeCounter = 0;
                List<string> listOfQuestionPropertiesX = new List<string>();
                String currentGridListNameLanX = "";


                #region Question Properties
                for (int n = 1; n < word.Length; n++)
                {
                    if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                        txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                    string myText = "*" + word[n];


                    if (myText.ToUpper().Trim().Contains("*QUESTION"))
                    {

                        //QID
                        string[] xyz = word[n].Trim().Split(' ');
                        if (xyz.Length == 2)
                        {
                            if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                            {
                                if (!listOfQuestionIdForDupliCheckLanX.Contains(xyz[1].Trim()))
                                {
                                    myQuestionLanX.QId = xyz[1].Trim();
                                    listOfQuestionIdForDupliCheckLanX.Add(xyz[1].Trim());

                                    //if (myQuestion.QId == "SQ21")
                                    //    MessageBox.Show("");
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + xyz[1].Trim() + ", QId must be unique");
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid QId " + xyz[1].Trim() + ", Must be started with Alpha");
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                    }
                    //Question Type
                    else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*NUMBER"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*RANK"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*IMAGE"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("NUMLISTTOTAL"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DATE"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*TIME"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("AUTOCOMPLETEANS"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("DROPDOWNLIST"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*FORM"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*INFO"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                    { hasEnd = true; }
                    else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                    { hasTerminate = true; }
                    else if (myText.ToUpper().Trim().Contains("*FIFS"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, *FIFS Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*ROT"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*MIN"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DELAY"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    //************************************* Attribute Filter ***********************************************
                    else if (myText.ToUpper().Trim().Contains("*INCLUDE"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    else if (myText.ToUpper().Trim().Contains("*DKCS"))
                    {
                        string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                        if (xyz.Length == 5)
                        {
                            listOfQuestionPropertiesX.Add(xyz[0].ToUpper().Trim());

                            //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                            if (xyz[1].Trim() != "")
                            {
                                if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                                {
                                    hasDKCS = true;

                                    attributeMain1 = new AttributeMain();
                                    attributeMain1.QId = currentQuestion.QId;
                                    attributeMain1.AttributeEnglish = "";
                                    attributeMain1.AttributeValue = "1";
                                    attributeMain1.AttributeOrder = "1";

                                    attributeMain2 = new AttributeMain();
                                    attributeMain2.QId = currentQuestion.QId;
                                    attributeMain2.AttributeEnglish = xyz[1].Trim();
                                    attributeMain2.AttributeValue = xyz[3].Trim();
                                    attributeMain2.AttributeOrder = "2";
                                    attributeMain2.IsExclusive = "1";



                                    //Add the attribute list 
                                    //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute code must be Number " + xyz[3].Trim());
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Attribute Label missing " + xyz[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Syntax for *DKCS is invalid ");
                    }
                    else if (myText.ToUpper().Trim().Contains("IF"))
                    { txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, " + word[n].Trim() + " Should not exist"); }
                    //************************************* End of Attribute Filter ****************************************

                }
                #endregion

                string questionText = "";
                strline = linesLanguageX[++i];
                bool getquestionText = false;
                while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                {
                    questionText = questionText + strline + "<br>";
                    strline = linesLanguageX[++i];
                    getquestionText = true;
                }

                if (questionText == "")
                    txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invalid Question Text : should not exist");
                else
                    myQuestionLanX.QuestionEnglish = questionText.Substring(0, questionText.Length - 4);

                //add question object to list
                if (languageNo == 1) listOfQuestionLan1.Add(myQuestionLanX);
                else if (languageNo == 2) listOfQuestionLan2.Add(myQuestionLanX);
                else if (languageNo == 3) listOfQuestionLan3.Add(myQuestionLanX);
                else if (languageNo == 4) listOfQuestionLan4.Add(myQuestionLanX);
                else if (languageNo == 5) listOfQuestionLan5.Add(myQuestionLanX);
                else if (languageNo == 6) listOfQuestionLan6.Add(myQuestionLanX);
                else if (languageNo == 7) listOfQuestionLan7.Add(myQuestionLanX);
                else if (languageNo == 8) listOfQuestionLan8.Add(myQuestionLanX);
                else if (languageNo == 9) listOfQuestionLan9.Add(myQuestionLanX);

                currentQuestionLanX = myQuestionLanX;

                //this portion is for question attribute

                if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                {
                    if (i < linesLanguageX.Count - 1)
                        i--;
                }

                List<AttributeMain> listOfAttributeMainLanX = new List<AttributeMain>();
                int index = 1;

                List<String> listOfAttributeValueForDupliCheckLanX = new List<string>();
                List<String> listOfAttributeLabelForDupliCheckLanX = new List<string>();

                if (hasDKCS == true)
                {
                    listOfAttributeMainLanX.Add(attributeMain1);
                    listOfAttributeMainLanX.Add(attributeMain2);
                    hasDKCS = false;
                }

                #region USELIST

                Dictionary<string, List<AttributeMain>> dicQidVsAttributeListLanX = new Dictionary<string, List<AttributeMain>>();
                if (languageNo == 1) dicQidVsAttributeListLanX = dicQidVsAttributeListLan1;
                else if (languageNo == 2) dicQidVsAttributeListLanX = dicQidVsAttributeListLan2;
                else if (languageNo == 3) dicQidVsAttributeListLanX = dicQidVsAttributeListLan3;
                else if (languageNo == 4) dicQidVsAttributeListLanX = dicQidVsAttributeListLan4;
                else if (languageNo == 5) dicQidVsAttributeListLanX = dicQidVsAttributeListLan5;
                else if (languageNo == 6) dicQidVsAttributeListLanX = dicQidVsAttributeListLan6;
                else if (languageNo == 7) dicQidVsAttributeListLanX = dicQidVsAttributeListLan7;
                else if (languageNo == 8) dicQidVsAttributeListLanX = dicQidVsAttributeListLan8;
                else if (languageNo == 9) dicQidVsAttributeListLanX = dicQidVsAttributeListLan9;


                if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                {
                    //Pronab made changes in this block
                    string[] word1 = strline.Split(' ');
                    if (word1.Length == 2)
                    {
                        if (word1[1].Split('"').Length == 3)
                        {
                            if (dicQidVsAttributeListLanX.ContainsKey(word1[1].Split('"')[1].Trim()))
                            {
                                if (dicQidVsAttributeListLanX.ContainsKey(word1[1].Split('"')[1].Trim()))
                                {
                                    List<AttributeMain> listOfAttributeTemp = dicQidVsAttributeListLanX[word1[1].Split('"')[1].Trim()];

                                    for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                    {
                                        if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                        {
                                            listOfAttributeMainLanX.Add(listOfAttributeTemp[x]);
                                            index++;
                                        }
                                        else
                                        {
                                            //If attribute have properties
                                            #region Attribute Properties
                                            string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                            AttributeMain attributeMainLanX = new AttributeMain();
                                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                            //{

                                            attributeMainLanX.AttributeEnglish = myKey[0].Trim();
                                            //}



                                            //Add the attribute in 
                                            listOfAttributeMainLanX.Add(attributeMainLanX);
                                            #endregion
                                        }

                                    }

                                    //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                    //index = listOfAttributeMain.Count + 1;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                                for (int x = 0; x < listOfAttributeMainLanX.Count; x++)
                                {
                                    listOfAttributeValueForDupliCheckLanX.Add(listOfAttributeMainLanX[x].AttributeValue);
                                    listOfAttributeLabelForDupliCheckLanX.Add(listOfAttributeMainLanX[x].AttributeEnglish);
                                }

                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                    strline = linesLanguageX[++i];
                    strline = linesLanguageX[++i];
                    //Pronab end 
                }
                #endregion

                #region Attribute with :
                if (isAttribute(strline))
                {
                    while (!strline.Trim().Substring(0, 1).Contains("*"))
                    {

                        //if (strline.Contains(":") && strline.Split(':').Length == 2)
                        if (strline.Contains(":"))
                        {
                            AttributeMain attributeMainLanX = new AttributeMain();
                            String[] myWord = strline.Split(':');

                            if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                            {
                                if (!listOfAttributeValueForDupliCheckLanX.Contains(myWord[0].Trim()))
                                {
                                    attributeMainLanX.AttributeValue = myWord[0].Trim();
                                    attributeMainLanX.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                    index++;

                                    //Add value in list
                                    listOfAttributeValueForDupliCheckLanX.Add(myWord[0].Trim());

                                }//else {Error Message}

                            }//else {Error Message}
                            string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);
                            if (!mylabel.Contains("*"))
                            {
                                //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                                //{

                                attributeMainLanX.AttributeEnglish = mylabel.Trim();

                                //Add value in list
                                listOfAttributeLabelForDupliCheckLanX.Add(mylabel.Trim().ToUpper());
                                //}//else {Error Message}
                            }
                            else
                            {

                                //If attribute have properties
                                #region Attribute Properties
                                string[] myKey = mylabel.Split('*');

                                //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                                //{

                                attributeMainLanX.AttributeEnglish = myKey[0].Trim();
                                //}

                                for (int n = 0; n < listOfKeyWords.Count; n++)
                                {
                                    if (mylabel.Contains("*" + listOfKeyWords[n])) txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invlaid Syntax, *" + listOfKeyWords[n] + " Should not exist");
                                }

                                #endregion

                            }

                            //Add the attribute in 
                            listOfAttributeMainLanX.Add(attributeMainLanX);

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Invalid syntax, Attribute code missing");

                        if (i < linesLanguageX.Count - 1)
                        {
                            strline = linesLanguageX[++i];
                        }
                    }

                    if (i < linesLanguageX.Count - 1)
                        i--;
                }
                else
                {
                    if (i < linesLanguageX.Count - 1)
                        i--;
                }

                #endregion

                if (myQuestionLanX.QId != null)
                {
                    if (languageNo == 1)
                    {
                        if (!dicQidVsAttributeListLan1.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan1.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 1, QId must be unique");
                    }
                    else if (languageNo == 2)
                    {
                        if (!dicQidVsAttributeListLan2.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan2.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 2, QId must be unique");
                    }
                    else if (languageNo == 3)
                    {
                        if (!dicQidVsAttributeListLan3.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan3.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 3, QId must be unique");
                    }
                    else if (languageNo == 4)
                    {
                        if (!dicQidVsAttributeListLan4.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan4.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 4, QId must be unique");
                    }
                    else if (languageNo == 5)
                    {
                        if (!dicQidVsAttributeListLan5.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan5.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 5, QId must be unique");
                    }
                    else if (languageNo == 6)
                    {
                        if (!dicQidVsAttributeListLan6.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan6.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 6, QId must be unique");
                    }
                    else if (languageNo == 7)
                    {
                        if (!dicQidVsAttributeListLan7.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan7.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 7, QId must be unique");
                    }
                    else if (languageNo == 8)
                    {
                        if (!dicQidVsAttributeListLan8.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan8.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 8, QId must be unique");
                    }
                    else if (languageNo == 9)
                    {
                        if (!dicQidVsAttributeListLan9.ContainsKey(myQuestionLanX.QId))
                            dicQidVsAttributeListLan9.Add(myQuestionLanX.QId, listOfAttributeMainLanX);
                        else
                            txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Duplicate QId " + myQuestionLanX.QId + " in Language 9, QId must be unique");
                    }
                }
                else
                    txtWriter.WriteLine("Line : " + dicLine[i + ln1 + 1] + " Question Id missing");

                //if (myAttributeFilter.QId != null)
                //    listOfAttributeFilter.Add(myAttributeFilter);
            }

            return i;
        }








        //Pronab added this overload method for repeat questions
        private int prepareQuestion(List<String> lines, int i, List<string> listOfQuestionIdForDupliCheck, List<string> listOfGridListForDupliCheck, List<LogicalSyntax> listOfLogicalSyntaxTemp, List<Question> listOfQuestionTemp, Question currentQuestionTemp, Dictionary<String, List<AttributeMain>> dicQidVsAttributeListTemp, List<AttributeFilter> listOfAttributeFilterTemp, TextWriter txtWriter, Dictionary<int, int> dicLine, AttributeMain attributeR)
        {
            AttributeMain attributeMain1 = new AttributeMain();
            AttributeMain attributeMain2 = new AttributeMain();

            AttributeMain attributeMainFIName = new AttributeMain();
            AttributeMain attributeMainFICode = new AttributeMain();
            AttributeMain attributeMainFSName = new AttributeMain();
            AttributeMain attributeMainFSCode = new AttributeMain();
            AttributeMain attributeSingleDropDown = new AttributeMain();

            String strline = lines[i];

            bool hasDKCS = false;
            bool hasFIFS = false;
            bool hasSingleDropdown = false;

            Question myQuestion = new Question();
            AttributeFilter myAttributeFilter = new AttributeFilter();
            string[] word = strline.Split('*');
            int QTypeCounter = 0;
            List<string> listOfQuestionProperties = new List<string>();
            String currentGridListName = "";
            String GridFilterQId = "";
            String GridFilterType = "";
            int qTypeForGridQid = 0;
            String qIdForGridQid = "";

            #region Question Properties
            for (int n = 1; n < word.Length; n++)
            {
                if (!listOfKeyWords.Contains(word[n].ToUpper().Trim().Split(' ')[0].Trim()))
                    txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid Token : " + word[n].ToUpper().Trim().Split(' ')[0].Trim());

                string myText = "*" + word[n];

                if (myText.ToUpper().Trim().Contains("*QUESTION"))
                {
                    qTypeForGridQid = 0;
                    //QID
                    string[] xyz = myText.Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        if (Regex.IsMatch(xyz[1].Trim(), "^[a-zA-Z0-9]+$"))
                        {
                            if (!listOfQuestionIdForDupliCheck.Contains(xyz[1].Trim().Replace("?R", attributeR.AttributeValue)))
                            {
                                qIdForGridQid = xyz[1].Trim().Replace("?R", attributeR.AttributeValue);
                                myQuestion.QId = xyz[1].Trim().Replace("?R", attributeR.AttributeValue);
                                listOfQuestionIdForDupliCheck.Add(xyz[1].Trim().Replace("?R", attributeR.AttributeValue));

                                //if (myQuestion.QId == "SQ21")
                                //    MessageBox.Show("");
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate QId " + xyz[1].Trim().Replace("?R", attributeR.AttributeValue) + ", QId must be unique");
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + xyz[1].Trim().Replace("?R", attributeR.AttributeValue) + ", Must be started with Alpha");
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId syntax" + xyz[0].Trim() + ", Only Qid exist after *QUESTION");

                }
                //Question Type
                else if (myText.ToUpper().Trim().Contains("*SR") && !myText.ToUpper().Trim().Contains("*GRIDSR"))
                { myQuestion.QType = "1"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*MR") && !myText.ToUpper().Trim().Contains("*GRIDMR"))
                { myQuestion.QType = "2"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*OPEN") && !myText.ToUpper().Trim().Contains("*ALPHALIST"))
                { myQuestion.QType = "3"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMBER") && !myText.ToUpper().Trim().Contains("*NUMBEROFRESPONSE"))
                { myQuestion.QType = "4"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*RANK"))
                { myQuestion.QType = "5"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*IMAGE") && !myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                {
                    myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    //string[] xy = word[n].Trim().Split(' ');
                    //if (xy.Length == 2)
                    //{
                    //    string[] xyz = xy[1].Split('"');
                    //    if (xyz.Length != 3)
                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                    //    else
                    //        myQuestion.FilePath = xyz[1];

                    //}
                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *IMAGE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*GRIDSR"))
                {
                    myQuestion.QType = "7"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SCALE7"))
                {
                    myQuestion.QType = "32"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SCALE10"))
                {
                    myQuestion.QType = "61"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*GRIDMR"))
                {
                    myQuestion.QType = "8"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    qTypeForGridQid = 8;
                }
                else if (myText.ToUpper().Trim().Contains("*MEDIA"))
                { myQuestion.QType = "9"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*RECORDING"))
                { myQuestion.QType = "10"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ALPHALIST"))
                { myQuestion.QType = "12"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMLIST") && !myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                { myQuestion.QType = "13"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DATE"))
                { myQuestion.QType = "14"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*TIME"))
                { myQuestion.QType = "15"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*CAPTUREIMAGE"))
                { myQuestion.QType = "16"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NUMLISTTOTAL"))
                { myQuestion.QType = "17"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETE") && !myText.ToUpper().Trim().Contains("*AUTOCOMPLETELIST") && !myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                {
                    myQuestion.QType = "22"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = true;
                }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETELIST"))
                {
                    myQuestion.QType = "22"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = false;
                }
                else if (myText.ToUpper().Trim().Contains("*AUTOCOMPLETEANS"))
                { myQuestion.QType = "23"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DROPDOWN") && !myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                {
                    myQuestion.QType = "24"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    hasSingleDropdown = true;
                }
                else if (myText.ToUpper().Trim().Contains("*DROPDOWNLIST"))
                { myQuestion.QType = "24"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DRAGDROP"))
                { myQuestion.QType = "26"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GRIDNUM"))
                {
                    myQuestion.QType = "27"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                    qTypeForGridQid = 8;
                }
                else if (myText.ToUpper().Trim().Contains("*MAXDIFF"))
                { myQuestion.QType = "40"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GPS"))
                { myQuestion.QType = "41"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*FORM") && !myText.ToUpper().Trim().Contains("*SHOWASFORM"))
                {
                    myQuestion.QType = "48";
                    QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*INFO"))
                { myQuestion.QType = "49"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*END") && !myText.ToUpper().Trim().Contains("*ENDREC"))
                { myQuestion.QType = "50"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); hasEnd = true; }
                else if (myText.ToUpper().Trim().Contains("*TERMINATE"))
                { myQuestion.QType = "51"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); hasTerminate = true; }
                else if (myText.ToUpper().Trim().Contains("*FIFS"))
                {
                    myQuestion.QType = "60"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    attributeMainFIName = new AttributeMain();
                    attributeMainFIName.QId = myQuestion.QId;
                    attributeMainFIName.AttributeEnglish = "FI Name";
                    attributeMainFIName.AttributeValue = "1";
                    attributeMainFIName.AttributeOrder = "1";
                    attributeMainFIName.LinkId1 = "3";
                    attributeMainFIName.ForceAndMsgOpt = "11";

                    attributeMainFICode = new AttributeMain();
                    attributeMainFICode.QId = myQuestion.QId;
                    attributeMainFICode.AttributeEnglish = "FI Code";
                    attributeMainFICode.AttributeValue = "2";
                    attributeMainFICode.AttributeOrder = "2";
                    attributeMainFICode.LinkId1 = "3";
                    attributeMainFICode.ForceAndMsgOpt = "11";

                    attributeMainFSName = new AttributeMain();
                    attributeMainFSName.QId = myQuestion.QId;
                    attributeMainFSName.AttributeEnglish = "FS Name";
                    //attributeMainFSName.AttributeEnglish = "FI Mobile No";
                    attributeMainFSName.AttributeValue = "3";
                    attributeMainFSName.AttributeOrder = "3";
                    attributeMainFSName.LinkId1 = "3";
                    attributeMainFSName.ForceAndMsgOpt = "11";

                    attributeMainFSCode = new AttributeMain();
                    attributeMainFSCode.QId = myQuestion.QId;
                    attributeMainFSCode.AttributeEnglish = "FS Code";
                    //attributeMainFSCode.AttributeEnglish = "FI Designation";
                    attributeMainFSCode.AttributeValue = "4";
                    attributeMainFSCode.AttributeOrder = "4";
                    attributeMainFSCode.LinkId1 = "3";
                    attributeMainFSCode.ForceAndMsgOpt = "11";

                    hasFIFS = true;
                }


                //else if (word[n].ToUpper().Trim().Contains(""))
                //    myQuestion.QType = "7";
                //else if (word[n].ToUpper().Trim().Contains(""))
                //    myQuestion.QType = "7";
                else if (myText.ToUpper().Trim().Contains("*RANDOM"))
                { myQuestion.HasRandomAttrib = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ROT") && !myText.ToUpper().Trim().Contains("*QROT") && !myText.ToUpper().Trim().Contains("*GROUPROT") && !myText.ToUpper().Trim().Contains("*OTPGROUPROT") && !myText.ToUpper().Trim().Contains("*OTPROTGROUP") && !myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                { myQuestion.HasRandomAttrib = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Group Rot Not Option
                else if (myText.ToUpper().Trim().Contains("*OTPGROUPROT"))
                { myQuestion.HasRandomAttrib = "10"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Optioin Rot Not Group
                else if (myText.ToUpper().Trim().Contains("*OTPROTGROUP"))
                { myQuestion.HasRandomAttrib = "01"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                //Group Rot Option Rot
                else if (myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                { myQuestion.HasRandomAttrib = "11"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*FROT"))
                { myQuestion.HasRandomAttrib = "11"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }


                else if (myText.ToUpper().Trim().Contains("*QROT"))
                { myQuestion.HasRandomQntr = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*GROUPROT") && !myText.ToUpper().Trim().Contains("*OTPGROUPROT") && !myText.ToUpper().Trim().Contains("*OTPROTGROUPROT"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.HasMessageLogic = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *GROUPROT is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*FONTSIZE"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.WrittenOEInPaper = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *FONTSIZE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*MIN"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                            myQuestion.NoOfResponseMin = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*MAX") && !myText.ToUpper().Trim().Contains("*MAXDIFF"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                            myQuestion.NoOfResponseMax = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MAX is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*COLUMN"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.NumberOfColumn = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *COLUMN is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*IMGADJBY"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.HasMediaPath = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *IMGADJ is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*IMGSIZE"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.HasMediaPath = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *IMGADJ is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*JUMPFOR"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.ResumeQntrJump = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *JUMPFOR is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*BLOCK"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                            myQuestion.ResumeQntrJump = xyz[1].Trim();
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *BLOCK is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*HORIZONTAL"))
                { myQuestion.NumberOfColumn = "2"; }
                else if (myText.ToUpper().Trim().Contains("*DUMMY1"))
                { myQuestion.HasAutoResponse = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DUMMY2"))
                { myQuestion.HasAutoResponse = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*DELAY"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success)
                        {
                            myQuestion.ShowInReport = xyz[1].Trim();
                            //listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *DELAY is invalid "); 
                    
                }
                else if (myText.ToUpper().Trim().Contains("*NOBACKBTN"))
                { myQuestion.DisplayBackButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*NONEXTBTN"))
                { myQuestion.DisplayNextButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*EXTCAMERA"))
                { myQuestion.ForceToTakeOE = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS1"))
                { myQuestion.DisplayJumpButton = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS2"))
                { myQuestion.DisplayJumpButton = "2"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS3"))
                { myQuestion.DisplayJumpButton = "3"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*ADDRESS4"))
                { myQuestion.DisplayJumpButton = "4"; listOfQuestionProperties.Add(word[n].ToUpper().Trim()); }
                else if (myText.ToUpper().Trim().Contains("*SHOWASFORM"))
                {
                    myQuestion.NumberOfColumn = "3"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*DIRIMAGE"))
                {
                    myQuestion.WrittenOEInPaper = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*SHOWASNUMTEXT"))
                {
                    myQuestion.WrittenOEInPaper = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*INRLD"))
                {
                    myQuestion.ForceToTakeOE = "1"; listOfQuestionProperties.Add(word[n].ToUpper().Trim());
                }
                else if (myText.ToUpper().Trim().Contains("*PICT"))
                {
                    //myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    string[] xy = word[n].Trim().Split(' ');
                    if (xy.Length == 2)
                    {
                        string[] xyz = xy[1].Split('"');
                        if (xyz.Length != 3)
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                        else
                            myQuestion.FilePath = xyz[1];

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*VIDEO"))
                {
                    //myQuestion.QType = "6"; QTypeCounter++; listOfQuestionProperties.Add(word[n].ToUpper().Trim());

                    string[] xy = word[n].Trim().Split(' ');
                    if (xy.Length == 2)
                    {
                        string[] xyz = xy[1].Split('"');
                        if (xyz.Length != 3)
                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                        else
                            myQuestion.FilePath = xyz[1];

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");

                }

                //************************************* Attribute Filter ***********************************************
                else if (myText.ToUpper().Trim().Contains("*INCLUDE") && !myText.ToUpper().Trim().Contains("*INCLUDEGRIDLIST"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                myAttributeFilter.ProjectId = projectInfoScript.ProjectCode;
                                myAttributeFilter.QId = myQuestion.QId;
                                myAttributeFilter.InheritedQId = abc[1].Trim();
                                myAttributeFilter.FilterType = "1";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*EXCLUDE"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                myAttributeFilter.ProjectId = projectInfoScript.ProjectCode;
                                myAttributeFilter.QId = myQuestion.QId;
                                myAttributeFilter.InheritedQId = abc[1].Trim();
                                myAttributeFilter.FilterType = "2";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*USEGRIDLIST"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfGridListForDupliCheck.Contains(abc[1].Trim()))
                            {
                                currentGridListName = abc[1].Trim();

                                if (hasSingleDropdown == true)
                                {
                                    attributeSingleDropDown = new AttributeMain();
                                    attributeSingleDropDown.QId = myQuestion.QId;
                                    attributeSingleDropDown.AttributeEnglish = "";
                                    attributeSingleDropDown.AttributeValue = "1";
                                    attributeSingleDropDown.AttributeOrder = "1";
                                    attributeSingleDropDown.LinkId1 = "1";
                                    attributeSingleDropDown.LinkId2 = currentGridListName;
                                    attributeSingleDropDown.ForceAndMsgOpt = "11";
                                }
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid GridListName : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " GridList Name must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *USEGRIDLIST is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("*INCLUDEGRIDLIST"))
                {
                    string[] xyz = word[n].Trim().Split(' ');
                    if (xyz.Length == 2)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                        {
                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                            {
                                GridFilterQId = abc[1].Trim();
                                GridFilterType = "1";
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");

                }
                else if (myText.ToUpper().Trim().Contains("*DKCS"))
                {
                    string[] xyz = word[n].Trim().Split(new Char[] { '\"' });
                    if (xyz.Length == 5)
                    {
                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                        //string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                        if (xyz[1].Trim() != "")
                        {
                            if (Regex.Match(xyz[3].Trim(), @"^\d+$").Success)
                            {
                                hasDKCS = true;

                                attributeMain1 = new AttributeMain();
                                //attributeMain1.QId = currentQuestion.QId;
                                attributeMain1.QId = myQuestion.QId;
                                attributeMain1.AttributeEnglish = "";
                                attributeMain1.AttributeValue = "1";
                                attributeMain1.AttributeOrder = "1";
                                attributeMain1.MinValue = "5";

                                attributeMain2 = new AttributeMain();
                                //attributeMain2.QId = currentQuestion.QId;
                                attributeMain2.QId = myQuestion.QId;
                                attributeMain2.AttributeEnglish = xyz[1].Trim();
                                attributeMain2.AttributeValue = xyz[3].Trim();
                                attributeMain2.AttributeOrder = "2";
                                attributeMain2.IsExclusive = "1";



                                //Add the attribute list 
                                //dicQidVsAttributeList.Add(myQuestion.QId, listOfAttributeMain1);
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute code must be Number " + xyz[3].Trim());
                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Attribute Label missing " + xyz[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *DKCS is invalid ");
                }
                else if (myText.ToUpper().Trim().Contains("IF"))
                {
                    LogicalSyntax myLogicalSyntax = new LogicalSyntax();

                    //string ifCondition = word[n].Split(new Char[] { '[', ']' })[1];

                    string ifCondition = word[n].Substring(word[n].IndexOf('[') + 1);
                    ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']')).Replace("?R", attributeR.AttributeValue);

                    // Check logical Expression
                    if (!checkLogicalExp.checkIfCondition(ifCondition, listOfQuestionIdForDupliCheck))
                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + ifCondition);

                    myLogicalSyntax.ThenValue = myQuestion.QId;
                    myLogicalSyntax.QId = myQuestion.QId;
                    myLogicalSyntax.LogicTypeId = "4";
                    myLogicalSyntax.IfCondition = ifCondition;

                    //Add in list
                    listOfLogicalSyntaxTemp.Add(myLogicalSyntax);
                }
                //************************************* End of Attribute Filter ****************************************

            }
            #endregion

            //Check Question properties is duplicate or not

            var query = listOfQuestionProperties.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();

            if (query.Count > 0)
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate Token ");

            if (myQuestion.QType == null)
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Question Type must be exist ");

            string questionText = "";
            strline = lines[++i];
            bool getquestionText = false;
            while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
            {
                questionText = questionText + strline + "<br>";
                strline = lines[++i];
                getquestionText = true;
            }

            if (questionText == "")
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid Question Text : should not exist");
            myQuestion.QuestionEnglish = questionText.Replace("?R", attributeR.AttributeValue).Replace("xyz", attributeR.AttributeEnglish);
            myQuestion.SilentRecording = silentRecording;

            //add question object to list
            listOfQuestionTemp.Add(myQuestion);
            currentQuestionTemp = myQuestion;



            //****************************************************this portion is for question attribute ********************************

            if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
            {
                if (i < lines.Count - 1)
                    i--;
            }

            List<AttributeMain> listOfAttributeMain = new List<AttributeMain>();
            int index = 1;

            List<String> listOfAttributeValueForDupliCheck = new List<string>();
            List<String> listOfAttributeLabelForDupliCheck = new List<string>();

            if (hasDKCS == true)
            {
                listOfAttributeMain.Add(attributeMain1);
                listOfAttributeMain.Add(attributeMain2);
                hasDKCS = false;
            }

            if (hasFIFS == true)
            {
                listOfAttributeMain.Add(attributeMainFIName);
                listOfAttributeMain.Add(attributeMainFICode);
                listOfAttributeMain.Add(attributeMainFSName);
                listOfAttributeMain.Add(attributeMainFSCode);

                hasFIFS = false;
            }

            if (hasSingleDropdown == true)
            {
                listOfAttributeMain.Add(attributeSingleDropDown);
                hasSingleDropdown = false;
            }

            #region USEGRIDLIST
            //This is for single Dropdown

            #endregion


            #region USELIST



            if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
            {
                string[] word1 = strline.Split(' ');
                if (word1.Length == 2)
                {
                    if (word1[1].Split('"').Length == 3)
                    {
                        if (dicListNameVsList.ContainsKey(word1[1].Split('"')[1].Trim()))
                        {
                            if (dicListNameVsList.ContainsKey(word1[1].Split('"')[1].Trim()))
                            {
                                List<AttributeMain> listOfAttributeTemp = new List<AttributeMain>(dicListNameVsList[word1[1].Split('"')[1].Trim()]);

                                if (myQuestion.QType == "7" || myQuestion.QType == "26" || myQuestion.QType == "40" || myQuestion.QType == "61")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {


                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;


                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "1";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;

                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");

                                }
                                else if (myQuestion.QType == "8")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "2";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");


                                }
                                else if (myQuestion.QType == "22")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "22";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                                }
                                else if (myQuestion.QType == "27")
                                {
                                    if (currentGridListName != "")
                                    {
                                        for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                        {
                                            AttributeMain myAttributeMain = new AttributeMain();
                                            //myAttributeMain = listOfAttributeTemp[x];
                                            myAttributeMain.ProjectId = listOfAttributeTemp[x].ProjectId;
                                            myAttributeMain.QId = listOfAttributeTemp[x].QId;
                                            myAttributeMain.AttributeEnglish = listOfAttributeTemp[x].AttributeEnglish;
                                            myAttributeMain.AttributeBengali = listOfAttributeTemp[x].AttributeBengali;
                                            myAttributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            myAttributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;
                                            myAttributeMain.TakeOpenended = listOfAttributeTemp[x].TakeOpenended;
                                            myAttributeMain.IsExclusive = listOfAttributeTemp[x].IsExclusive;
                                            myAttributeMain.LinkId1 = listOfAttributeTemp[x].LinkId1;
                                            myAttributeMain.LinkId2 = listOfAttributeTemp[x].LinkId2;
                                            myAttributeMain.MinValue = listOfAttributeTemp[x].MinValue;
                                            myAttributeMain.MaxValue = listOfAttributeTemp[x].MaxValue;
                                            myAttributeMain.ForceAndMsgOpt = listOfAttributeTemp[x].ForceAndMsgOpt;
                                            myAttributeMain.GroupName = listOfAttributeTemp[x].GroupName;
                                            myAttributeMain.FilterQid = listOfAttributeTemp[x].FilterQid;
                                            myAttributeMain.FilterType = listOfAttributeTemp[x].FilterType;
                                            myAttributeMain.ExcepValue = listOfAttributeTemp[x].ExcepValue;
                                            myAttributeMain.Comments = listOfAttributeTemp[x].Comments;
                                            myAttributeMain.AttributeLang3 = listOfAttributeTemp[x].AttributeLang3;
                                            myAttributeMain.AttributeLang4 = listOfAttributeTemp[x].AttributeLang4;
                                            myAttributeMain.AttributeLang5 = listOfAttributeTemp[x].AttributeLang5;
                                            myAttributeMain.AttributeLang6 = listOfAttributeTemp[x].AttributeLang6;
                                            myAttributeMain.AttributeLang7 = listOfAttributeTemp[x].AttributeLang7;
                                            myAttributeMain.AttributeLang8 = listOfAttributeTemp[x].AttributeLang8;
                                            myAttributeMain.AttributeLang9 = listOfAttributeTemp[x].AttributeLang9;
                                            myAttributeMain.AttributeLang10 = listOfAttributeTemp[x].AttributeLang10;

                                            listOfAttributeMain.Add(myAttributeMain);
                                            listOfAttributeMain[x].LinkId1 = "27";
                                            listOfAttributeMain[x].LinkId2 = currentGridListName;

                                            listOfAttributeMain[x].FilterQid = GridFilterQId;
                                            listOfAttributeMain[x].FilterType = GridFilterType;

                                            index++;
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " List unavailable");
                                }
                                else
                                {
                                    //Start pronab added
                                    for (int x = 0; x < listOfAttributeTemp.Count; x++)
                                    {
                                        if (!listOfAttributeTemp[x].AttributeEnglish.Contains("*"))
                                        {
                                            listOfAttributeMain.Add(listOfAttributeTemp[x]);
                                            index++;
                                        }
                                        else
                                        {
                                            AttributeMain attributeMain = new AttributeMain();
                                            //*********** If grid attribute has property ********************

                                            #region Attribute Properties
                                            string[] myKey = listOfAttributeTemp[x].AttributeEnglish.Split('*');

                                            attributeMain.AttributeEnglish = myKey[0].Trim();
                                            attributeMain.AttributeValue = listOfAttributeTemp[x].AttributeValue;
                                            attributeMain.AttributeOrder = listOfAttributeTemp[x].AttributeOrder;

                                            for (int n = 1; n < myKey.Length; n++)
                                            {
                                                if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN") || ("*" + myKey[n]).ToUpper().Trim().Contains("*MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                                {
                                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN"))
                                                    {
                                                        string[] xyz = myKey[n].Split(' ');
                                                        if (xyz.Length >= 2)
                                                        {
                                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                                attributeMain.MinValue = xyz[1].Trim();
                                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                                        }
                                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                                    }
                                                    else if (("*" + myKey[n]).ToUpper().Trim().Contains("*MAX"))
                                                    {
                                                        string[] xyz = myKey[n].Split(' ');
                                                        if (xyz.Length >= 2)
                                                        {
                                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                                attributeMain.MaxValue = xyz[1].Trim();
                                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                                        }
                                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                                    }
                                                }
                                                else if (myKey[n].ToUpper().Trim().Contains("PICT"))
                                                {
                                                    string[] xy = myKey[n].Trim().Split(' ');
                                                    if (xy.Length == 2)
                                                    {
                                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                                        string[] xyz = xy[1].Split('"');
                                                        if (xyz.Length != 3)
                                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                                        else
                                                        {
                                                            if (myQuestion.QType == "17")
                                                                attributeMain.Comments = xyz[1].Trim();
                                                            else
                                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                                        }
                                                    }
                                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");

                                                }
                                                else if (myKey[n].ToUpper().Trim().Contains("VIDEO"))
                                                {
                                                    string[] xy = myKey[n].Trim().Split(' ');
                                                    if (xy.Length == 2)
                                                    {
                                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                                        string[] xyz = xy[1].Split('"');
                                                        if (xyz.Length != 3)
                                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                                        else
                                                            attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                                    }
                                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");

                                                }
                                                else
                                                {
                                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim()))
                                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                                    {
                                                        attributeMain.TakeOpenended = "1";
                                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " OPEN Should not be exist for Form attribute");
                                                    }
                                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                                    {
                                                        attributeMain.IsExclusive = "1";
                                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NMUL Should not be exist for Form attribute");

                                                    }
                                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                                        attributeMain.ForceAndMsgOpt = "11";
                                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                                    {
                                                        if (attributeMain.IsExclusive == "" || attributeMain.IsExclusive == null)
                                                            attributeMain.IsExclusive = "2";

                                                        if (myAttributeFilter.ExceptionalValue == null)
                                                            myAttributeFilter.ExceptionalValue = "";

                                                        myAttributeFilter.ExceptionalValue = myAttributeFilter.ExceptionalValue + attributeMain.AttributeValue + ",";

                                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NOCON Should not be exist for Form attribute");
                                                    }
                                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                                        attributeMain.LinkId1 = "1";
                                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                                        attributeMain.LinkId1 = "2";
                                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                                        attributeMain.LinkId1 = "3";
                                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                                        attributeMain.LinkId1 = "4";
                                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                                        attributeMain.LinkId1 = "14";
                                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                                        attributeMain.LinkId1 = "15";
                                                    else if (myKey[n].ToUpper().Trim().Contains("AUTOCOMPLETE"))
                                                        attributeMain.LinkId1 = "22";
                                                    else if (myKey[n].ToUpper().Trim().Contains("DROPDOWN"))
                                                        attributeMain.LinkId1 = "24";

                                                }
                                            }
                                            #endregion

                                            listOfAttributeMain.Add(attributeMain);


                                            index++;

                                        }
                                        //End pronab added
                                    }
                                }


                                //listOfAttributeMain = dicQidVsAttributeList[word1[1].Split('"')[1].Trim()];
                                //index = listOfAttributeMain.Count + 1;
                            }
                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Duplicate Attribute list name/Qid" + word1[1].Split('"')[1].Trim());

                            for (int x = 0; x < listOfAttributeMain.Count; x++)
                            {
                                listOfAttributeValueForDupliCheck.Add(listOfAttributeMain[x].AttributeValue);
                                listOfAttributeLabelForDupliCheck.Add(listOfAttributeMain[x].AttributeEnglish);
                            }

                        }
                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid use list name" + word1[1].Split('"')[1].Trim());
                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());
                }
                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for use list is wrong" + word1[1].Split('"')[1].Trim());

                strline = lines[++i];
                strline = lines[++i];

            }
            #endregion

            #region Attribute with :
            if (isAttribute(strline))
            {
                while (!strline.Trim().Substring(0, 1).Contains("*"))
                {

                    //if (strline.Contains(":") && strline.Split(':').Length == 2)
                    if (strline.Contains(":"))
                    {
                        AttributeMain attributeMain = new AttributeMain();
                        String[] myWord = strline.Split(':');

                        if (Regex.Match(myWord[0].Trim(), @"^\d+$").Success)
                        {
                            if (!listOfAttributeValueForDupliCheck.Contains(myWord[0].Trim()))
                            {
                                attributeMain.AttributeValue = myWord[0].Trim();
                                attributeMain.AttributeOrder = myWord[0].Trim(); //index.ToString();
                                index++;

                                //Add value in list
                                listOfAttributeValueForDupliCheck.Add(myWord[0].Trim());
                                //ismile
                                //Add qid with value label for grid attribute condition
                                if (qTypeForGridQid == 8)
                                    listOfQuestionIdForDupliCheck.Add(qIdForGridQid + "_" + myWord[0].Trim());


                            }//else {Error Message}

                        }//else {Error Message}
                        string mylabel = strline.Substring(strline.IndexOf(":", 0) + 1);

                        if (myQuestion.QType == "48")
                        {
                            if (mylabel.Contains("*SR") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*MR") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*DROPDOWN") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            if (mylabel.Contains("*AUTOCOMPLETE") & !mylabel.Contains("*USEGRIDLIST")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " USEGRIDLIST must be exist for Form attribute");
                            //if (mylabel.Contains("*NUMBER") & !mylabel.Contains("*MIN")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " MIN must be exist for Form attribute");
                            //if (mylabel.Contains("*NUMBER") & !mylabel.Contains("*MAX")) txtWriter.WriteLine("Line : " + dicLine[i + 1] + " MAX must be exist for Form attribute");
                        }


                        if (!mylabel.Contains("*"))
                        {
                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[1].Trim().ToUpper()))
                            //{

                            if (myQuestion.QType == "48")
                            {
                                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Form attribute must have a type");
                            }


                            attributeMain.AttributeEnglish = mylabel.Trim();

                            if (myQuestion.QType == "7" || myQuestion.QType == "22" || myQuestion.QType == "24" || myQuestion.QType == "26" || myQuestion.QType == "40" || myQuestion.QType == "61")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "1";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;

                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            else if (myQuestion.QType == "8")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "2";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            else if (myQuestion.QType == "27")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "27";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            //else if (myQuestion.QType == "22")
                            //{
                            //    if (currentGridListName != "")
                            //    {

                            //        attributeMain.LinkId1 = "22";
                            //        attributeMain.LinkId2 = currentGridListName;

                            //        attributeMain.FilterQid = GridFilterQId;
                            //        attributeMain.FilterType = GridFilterType;
                            //    }
                            //    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            //}

                            //Add value in list
                            listOfAttributeLabelForDupliCheck.Add(mylabel.Trim().ToUpper());
                            //}//else {Error Message}
                        }
                        else
                        {
                            // *********** If grid attribute has property ********************
                            if (myQuestion.QType == "7" || myQuestion.QType == "22" || myQuestion.QType == "24" || myQuestion.QType == "26")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "1";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            else if (myQuestion.QType == "8")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "2";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            else if (myQuestion.QType == "27")
                            {
                                if (currentGridListName != "")
                                {
                                    attributeMain.LinkId1 = "27";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }
                            else if (myQuestion.QType == "22")
                            {
                                if (currentGridListName != "")
                                {

                                    attributeMain.LinkId1 = "22";
                                    attributeMain.LinkId2 = currentGridListName;

                                    attributeMain.FilterQid = GridFilterQId;
                                    attributeMain.FilterType = GridFilterType;
                                }
                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Grid list unavailable");
                            }

                            //*****************************************************


                            //If attribute have properties
                            #region Attribute Properties
                            string[] myKey = mylabel.Split('*');

                            //if (!listOfAttributeLabelForDupliCheck.Contains(myWord[0].Trim().ToUpper()))
                            //{

                            attributeMain.AttributeEnglish = myKey[0].Trim();
                            //}


                            for (int n = 1; n < myKey.Length; n++)
                            {
                                if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN") || ("*" + myKey[n]).ToUpper().Trim().Contains("*MAX") || myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*MIN"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MinValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                    }
                                    else if (("*" + myKey[n]).ToUpper().Trim().Contains("*MAX"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MaxValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("USEGRIDLIST"))
                                    {
                                        string[] xyz = myKey[n].Trim().Split(' ');
                                        if (xyz.Length == 2)
                                        {
                                            listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                            string[] abc = xyz[1].Trim().Split(new Char[] { '\"' });
                                            if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9]+$").Success)
                                            {
                                                if (listOfGridListForDupliCheck.Contains(abc[1].Trim()))
                                                {
                                                    currentGridListName = abc[1].Trim();
                                                    attributeMain.LinkId2 = currentGridListName;
                                                }
                                                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid GridListName : " + abc[1].Trim());
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " GridList Name must be followed by Alpha Charecter " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *USEGRIDLIST is invalid ");
                                    }
                                }
                                else if (("*" + myKey[n]).ToUpper().Trim().Contains("*LAT") || ("*" + myKey[n]).ToUpper().Trim().Contains("*LON") || myKey[n].ToUpper().Trim().Contains("COMPVAL"))
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim().Split(' ')[0]))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (("*" + myKey[n]).ToUpper().Trim().Contains("*LAT"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MinValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *MIN is invalid ");
                                    }
                                    else if (("*" + myKey[n]).ToUpper().Trim().Contains("*LON"))
                                    {
                                        string[] xyz = myKey[n].Split(' ');
                                        if (xyz.Length >= 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                                attributeMain.MaxValue = xyz[1].Trim();
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *Max is invalid ");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("COMPVAL"))
                                    {
                                        string[] xyz = myKey[n].Trim().Split(' ');
                                        if (xyz.Length == 2)
                                        {
                                            if (Regex.Match(xyz[1].Trim(), @"^\d+$").Success || Regex.Match(xyz[1].Trim(), @"^\d.+$").Success)
                                            {
                                                attributeMain.ExcepValue = xyz[1].Trim();
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Must be non negetive integer " + xyz[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *COMPVAL is invalid ");
                                    }
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("INCLUDE"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9.]+$").Success)
                                        {
                                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Split('.')[0].Trim()))
                                            {
                                                if (qTypeForGridQid == 8)
                                                    attributeMain.FilterQid = abc[1].Trim() + "_" + attributeMain.AttributeValue;    //filter QID
                                                else
                                                    attributeMain.FilterQid = abc[1].Trim();

                                                attributeMain.FilterType = "1";
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *INCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("EXCLUDE"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z0-9.]+$").Success)
                                        {
                                            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Split('.')[0].Trim()))
                                            {
                                                if (qTypeForGridQid == 8)
                                                    attributeMain.FilterQid = abc[1].Trim() + "_" + attributeMain.AttributeValue;    //filter QID
                                                else
                                                    attributeMain.FilterQid = abc[1].Trim();

                                                attributeMain.FilterType = "2";
                                            }
                                            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid QId : " + abc[1].Trim());
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *EXCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("EXCEPT"))
                                {
                                    string[] xyz = myKey[n].Trim().Split(' ');
                                    if (xyz.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xyz[0].ToUpper().Trim());

                                        string[] abc = xyz[1].Trim().Split(new Char[] { '[', ']' });
                                        if (Regex.Match(abc[1].Trim(), "^[0-9]+$").Success)
                                        {
                                            attributeMain.ExcepValue = abc[1].Trim();
                                        }
                                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " QId must be followed by Alpha Charecter " + abc[1].Trim());
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *INCLUDE is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("PICT"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                        {
                                            if (myQuestion.QType == "17")
                                                attributeMain.Comments = xyz[1].Trim();
                                            else
                                                attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                        }
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("VIDEO"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                            attributeMain.ForceAndMsgOpt = xyz[1].Trim();
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("GROUPNAME"))
                                {
                                    string[] xy = myKey[n].Trim().Split(' ');
                                    if (xy.Length == 2)
                                    {
                                        listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                        string[] xyz = xy[1].Split('"');
                                        if (xyz.Length != 3)
                                            txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                        else
                                            attributeMain.GroupName = xyz[1].Trim();
                                    }
                                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else if (myKey[n].ToUpper().Trim().Contains("GROUPHEAD"))
                                {
                                    attributeMain.Comments = "GroupHead";

                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    listOfQuestionProperties.Add(xy[0].ToUpper().Trim());

                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xy[2]);
                                    //    else
                                    //        attributeMain.GroupName = xyz[1].Trim();
                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *VIDEO is invalid ");


                                    //string[] xy = myKey[n].Trim().Split(' ');
                                    //if (xy.Length == 2)
                                    //{
                                    //    string[] xyz = xy[1].Split('"');
                                    //    if (xyz.Length != 3)
                                    //        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + xyz[2]);
                                    //    else
                                    //        myQuestion.FilePath = xyz[1];

                                    //}
                                    //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Syntax for *PICT is invalid ");
                                }
                                else
                                {
                                    if (!listOfKeyWords.Contains(myKey[n].ToUpper().Trim()))
                                        txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid token " + myKey[n].ToUpper().Trim());

                                    if (myKey[n].ToUpper().Trim().Contains("OPEN"))
                                    {
                                        attributeMain.TakeOpenended = "1";
                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " OPEN Should not be exist for Form attribute");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("NMUL"))
                                    {
                                        attributeMain.IsExclusive = "1";
                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NMUL Should not be exist for Form attribute");

                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("MANDATORY"))
                                        attributeMain.ForceAndMsgOpt = "11";
                                    else if (myKey[n].ToUpper().Trim().Contains("NOCON"))
                                    {
                                        if (attributeMain.IsExclusive == "" || attributeMain.IsExclusive == null)
                                            attributeMain.IsExclusive = "2";

                                        if (myAttributeFilter.ExceptionalValue == null)
                                            myAttributeFilter.ExceptionalValue = "";

                                        myAttributeFilter.ExceptionalValue = myAttributeFilter.ExceptionalValue + attributeMain.AttributeValue + ",";

                                        if (myQuestion.QType == "48") txtWriter.WriteLine("Line : " + dicLine[i + 1] + " NOCON Should not be exist for Form attribute");
                                    }
                                    else if (myKey[n].ToUpper().Trim().Contains("SR"))
                                        attributeMain.LinkId1 = "1";
                                    else if (myKey[n].ToUpper().Trim().Contains("MR"))
                                        attributeMain.LinkId1 = "2";
                                    else if (myKey[n].ToUpper().Trim().Contains("ALPHA"))
                                        attributeMain.LinkId1 = "3";
                                    else if (myKey[n].ToUpper().Trim().Contains("NUMBER"))
                                        attributeMain.LinkId1 = "4";
                                    else if (myKey[n].ToUpper().Trim().Contains("DATE"))
                                        attributeMain.LinkId1 = "14";
                                    else if (myKey[n].ToUpper().Trim().Contains("TIME"))
                                        attributeMain.LinkId1 = "15";
                                    else if (myKey[n].ToUpper().Trim().Contains("AUTOCOMPLETE"))
                                        attributeMain.LinkId1 = "22";
                                    else if (myKey[n].ToUpper().Trim().Contains("DROPDOWN"))
                                        attributeMain.LinkId1 = "24";


                                }
                            }
                            #endregion

                        }

                        //Add the attribute in 
                        listOfAttributeMain.Add(attributeMain);

                    }
                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invalid syntax y");

                    if (i < lines.Count - 1)
                    {
                        strline = lines[++i];
                    }
                }

                if (i < lines.Count - 1)
                    i--;
            }
            else
            {
                if (i < lines.Count - 1)
                    i--;
            }

            #endregion

            if (myQuestion.QId != null)
                dicQidVsAttributeListTemp.Add(myQuestion.QId, listOfAttributeMain);
            else
                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Question Id missing");

            if (myAttributeFilter.QId != null)
                listOfAttributeFilterTemp.Add(myAttributeFilter);


            return i;

        }

        private bool isAttribute(string strline)
        {
            if (strline != "")
            {
                string[] word = strline.Split(':');
                if (Regex.Match(word[0].Trim(), @"^\d+$").Success)
                    return true;
                else
                    return false;
            }

            return false;
        }

        private void prepareScriptDB()
        {
            try
            {
                if (File.Exists(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
                    File.Delete(myPath + "\\" + projectInfoScript.DatabaseName + ".db");


                //String sTemp = "C:\\Temp\\ShellDB";
                //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + "ShellDB\\SYSHELDB.db";
                //string databasePath = System.AppDomain.CurrentDomain.BaseDirectory + "ShellDB\\" + comShellDBType.Text;
                string databasePath = "C:\\Temp\\ShellDB\\" + comShellDBType.Text;
                File.Copy(databasePath, myPath + "\\" + projectInfoScript.DatabaseName + ".db");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region insert Project Info
                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                SQLiteTransaction transaction = connectionDB.sqlite_conn.BeginTransaction();

                SQLiteCommand sqlite_cmd0;
                sqlite_cmd0 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd0.CommandText = "INSERT INTO T_ProjectInfo ("
                                            + " ProjectId,"
                                            + " ProjectName,"
                                            + " JobNo,"
                                            + " Version,"
                                            + " Status,"
                                            + " WebServerAddress) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + "'" + replaceNull(projectInfoScript.ProjectName) + "',"
                                            + "'',"
                                            + "'" + replaceNull(projectInfoScript.ScriptVersion) + "',"
                                            + "'2',"
                                            + "'" + serverAddress + "');";


                sqlite_cmd0.ExecuteNonQuery();
                sqlite_cmd0.Dispose();

                #endregion

                #region insert Question DB

                SQLiteCommand sqlite_cmd1;
                sqlite_cmd1 = connectionDB.sqlite_conn.CreateCommand();

                for (int i = 0; i < listOfQuestion.Count; i++)
                {
                    Question myQuestion = listOfQuestion[i];
                    if (i == 0) myQuestion.DisplayBackButton = "1";

                    String qText = replaceNull(myQuestion.QuestionEnglish);
                    sqlite_cmd1.CommandText = "INSERT INTO T_Question ("
                                            + " ProjectId,"
                                            + " QId,"
                                            + " QuestionEnglish,"
                                            + " QuestionBengali,"
                                            + " AttributeId,"
                                            + " Comments,"
                                            + " QType,"
                                            + " NoOfResponseMin,"
                                            + " NoOfResponseMax,"
                                            + " HasAutoResponse,"
                                            + " HasRandomAttrib,"
                                            + " NumberOfColumn,"
                                            + " ShowInReport,"
                                            + " HasRandomQntr,"
                                            + " HasMessageLogic,"
                                            + " WrittenOEInPaper,"
                                            + " ForceToTakeOE,"
                                            + " HasMediaPath,"
                                            + " DisplayBackButton,"
                                            + " DisplayNextButton,"
                                            + " DisplayJumpButton,"
                                            + " ResumeQntrJump,"
                                            + " SilentRecording,"
                                            + " FilePath,"
                                            + " OrderTag,"
                                            + " OrderTag1,"
                                            + " OrderTag2,"
                                            + " OrderTag3,"
                                            + " OrderTag4,"
                                            + " OrderTag5,"
                                            + " QuestionLang3,"
                                            + " QuestionLang4,"
                                            + " QuestionLang5,"
                                            + " QuestionLang6,"
                                            + " QuestionLang7,"
                                            + " QuestionLang8,"
                                            + " QuestionLang9,"
                                            + " QuestionLang10) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + "'" + replaceNull(myQuestion.QId) + "',"
                                            + "'" + replaceNull(myQuestion.QuestionEnglish).Substring(0, qText.Length - 4) + "',"
                                            + "'" + replaceNull(myQuestion.QuestionBengali) + "',"
                                            + "'" + replaceNull(myQuestion.AttributeId) + "',"
                                            + "'" + (myQuestion.Comments == null ? replaceNull(myQuestion.QuestionEnglish).Substring(0, qText.Length - 4) : replaceNull(myQuestion.Comments)) + "',"
                                            + "'" + replaceNull(myQuestion.QType) + "',"
                                            + "'" + replaceNull(myQuestion.NoOfResponseMin) + "',"
                                            + "'" + replaceNull(myQuestion.NoOfResponseMax) + "',"
                                            + "'" + replaceNull(myQuestion.HasAutoResponse) + "',"
                                            + "'" + replaceNull(myQuestion.HasRandomAttrib) + "',"
                                            + "'" + replaceNull(myQuestion.NumberOfColumn) + "',"
                                            + "'" + replaceNull(myQuestion.ShowInReport) + "',"
                                            + "'" + replaceNull(myQuestion.HasRandomQntr) + "',"
                                            + "'" + replaceNull(myQuestion.HasMessageLogic) + "',"
                                            + "'" + replaceNull(myQuestion.WrittenOEInPaper) + "',"
                                            + "'" + replaceNull(myQuestion.ForceToTakeOE) + "',"
                                            + "'" + replaceNull(myQuestion.HasMediaPath) + "',"
                                            + "'" + replaceNull(myQuestion.DisplayBackButton) + "',"
                                            + "'" + replaceNull(myQuestion.DisplayNextButton) + "',"
                                            + "'" + replaceNull(myQuestion.DisplayJumpButton) + "',"
                                            + "'" + replaceNull(myQuestion.ResumeQntrJump) + "',"
                                            + "'" + replaceNull(myQuestion.SilentRecording) + "',"
                                            + "'" + replaceNull(myQuestion.FilePath) + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'" + (i + 1).ToString() + "',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'');";


                    //VALUES();

                    sqlite_cmd1.ExecuteNonQuery();
                }

                sqlite_cmd1.Dispose();

                #endregion

                #region insert Attribute DB

                //*********************************************************************************************

                SQLiteCommand sqlite_cmd2;
                sqlite_cmd2 = connectionDB.sqlite_conn.CreateCommand();

                try
                {
                    foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeList)
                    {
                        List<AttributeMain> listOfAttribute = pair.Value;

                        for (int i = 0; i < listOfAttribute.Count; i++)
                        {
                            AttributeMain myAttribute = listOfAttribute[i];

                            sqlite_cmd2.CommandText = "INSERT INTO T_OptAttribute ("
                                                    + " ProjectId,"
                                                    + " QId,"
                                                    + " AttributeEnglish,"
                                                    + " AttributeBengali,"
                                                    + " AttributeValue,"
                                                    + " AttributeOrder,"
                                                    + " TakeOpenended,"
                                                    + " IsExclusive,"
                                                    + " LinkId1,"
                                                    + " LinkId2,"
                                                    + " MinValue,"
                                                    + " MaxValue,"
                                                    + " ForceAndMsgOpt,"
                                                    + " GroupName,"
                                                    + " FilterQid,"
                                                    + " FilterType,"
                                                    + " ExcepValue,"
                                                    + " Comments,"
                                                    + " AttributeLang3,"
                                                    + " AttributeLang4,"
                                                    + " AttributeLang5,"
                                                    + " AttributeLang6,"
                                                    + " AttributeLang7,"
                                                    + " AttributeLang8,"
                                                    + " AttributeLang9,"
                                                    + " AttributeLang10) VALUES("
                                                    + projectInfoScript.ProjectCode + ","
                                                    + "'" + pair.Key + "',"
                                                    + "'" + replaceNull(myAttribute.AttributeEnglish) + "',"
                                                    + "'" + replaceNull(myAttribute.AttributeBengali) + "',"
                                                    + "'" + replaceNull(myAttribute.AttributeValue) + "',"
                                                    + "" + replaceNull(myAttribute.AttributeOrder) + ","
                                                    + "'" + replaceNull(myAttribute.TakeOpenended) + "',"
                                                    + "'" + replaceNull(myAttribute.IsExclusive) + "',"
                                                    + "'" + replaceNull(myAttribute.LinkId1) + "',"
                                                    + "'" + replaceNull(myAttribute.LinkId2) + "',"
                                                    + "'" + replaceNull(myAttribute.MinValue) + "',"
                                                    + "'" + replaceNull(myAttribute.MaxValue) + "',"
                                                    + "'" + replaceNull(myAttribute.ForceAndMsgOpt) + "',"
                                                    + "'" + replaceNull(myAttribute.GroupName) + "',"
                                                    + "'" + replaceNull(myAttribute.FilterQid) + "',"
                                                    + "'" + replaceNull(myAttribute.FilterType) + "',"
                                                    + "'" + replaceNull(myAttribute.ExcepValue) + "',"
                                                    + "'" + replaceNull(myAttribute.Comments) + "',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'');";


                            //VALUES();

                            sqlite_cmd2.ExecuteNonQuery();
                        }

                        sqlite_cmd2.CommandText = "INSERT INTO T_OptAttribute ("
                                                    + " ProjectId,"
                                                    + " QId,"
                                                    + " AttributeEnglish,"
                                                    + " AttributeBengali,"
                                                    + " AttributeValue,"
                                                    + " AttributeOrder,"
                                                    + " TakeOpenended,"
                                                    + " IsExclusive,"
                                                    + " LinkId1,"
                                                    + " LinkId2,"
                                                    + " MinValue,"
                                                    + " MaxValue,"
                                                    + " ForceAndMsgOpt,"
                                                    + " GroupName,"
                                                    + " FilterQid,"
                                                    + " FilterType,"
                                                    + " ExcepValue,"
                                                    + " Comments,"
                                                    + " AttributeLang3,"
                                                    + " AttributeLang4,"
                                                    + " AttributeLang5,"
                                                    + " AttributeLang6,"
                                                    + " AttributeLang7,"
                                                    + " AttributeLang8,"
                                                    + " AttributeLang9,"
                                                    + " AttributeLang10) VALUES("
                                                    + projectInfoScript.ProjectCode + ","
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "0,"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'',"
                                                    + "'');";


                        //VALUES();

                        sqlite_cmd2.ExecuteNonQuery();
                        sqlite_cmd2.ExecuteNonQuery();
                        sqlite_cmd2.ExecuteNonQuery();
                        sqlite_cmd2.ExecuteNonQuery();
                        sqlite_cmd2.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\nT_OptAttribute\n" + sqlite_cmd2.CommandText);
                }


                sqlite_cmd2.Dispose();

                #endregion

                #region insert GridInfo DB

                //*********************************************************************************************

                SQLiteCommand sqlite_cmd3;
                sqlite_cmd3 = connectionDB.sqlite_conn.CreateCommand();

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
                {
                    List<GridInfo> listOfGridInfo = pair.Value;

                    for (int i = 0; i < listOfGridInfo.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfo[i];

                        sqlite_cmd3.CommandText = "INSERT INTO T_GridInfo ("
                                                + " ProjectId,"
                                                + " QId,"
                                                + " AttributeEnglish,"
                                                + " AttributeBengali,"
                                                + " AttributeValue,"
                                                + " AttributeOrder,"
                                                + " TakeOpenended,"
                                                + " IsExclusive,"
                                                + " MinValue,"
                                                + " MaxValue,"
                                                + " ForceAndMsgOpt,"
                                                + " Comments,"
                                                + " AttributeLang3,"
                                                + " AttributeLang4,"
                                                + " AttributeLang5,"
                                                + " AttributeLang6,"
                                                + " AttributeLang7,"
                                                + " AttributeLang8,"
                                                + " AttributeLang9,"
                                                + " AttributeLang10) VALUES("
                                                + projectInfoScript.ProjectCode + ","
                                                + "'" + pair.Key + "',"
                                                + "'" + replaceNull(myGridInfo.AttributeEnglish) + "',"
                                                + "'" + replaceNull(myGridInfo.AttributeBengali) + "',"
                                                + "'" + replaceNull(myGridInfo.AttributeValue) + "',"
                                                + "" + replaceNull(myGridInfo.AttributeOrder) + ","
                                                + "'" + replaceNull(myGridInfo.TakeOpenended) + "',"
                                                + "'" + replaceNull(myGridInfo.IsExclusive) + "',"
                                                + "'" + replaceNull(myGridInfo.MinValue) + "',"
                                                + "'" + replaceNull(myGridInfo.MaxValue) + "',"
                                                + "'" + replaceNull(myGridInfo.ForceAndMsgOpt) + "',"
                                                + "'" + replaceNull(myGridInfo.Comments) + "',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'');";


                        //VALUES();

                        sqlite_cmd3.ExecuteNonQuery();
                    }

                    sqlite_cmd3.CommandText = "INSERT INTO T_GridInfo ("
                                                + " ProjectId,"
                                                + " QId,"
                                                + " AttributeEnglish,"
                                                + " AttributeBengali,"
                                                + " AttributeValue,"
                                                + " AttributeOrder,"
                                                + " TakeOpenended,"
                                                + " IsExclusive,"
                                                + " MinValue,"
                                                + " MaxValue,"
                                                + " ForceAndMsgOpt,"
                                                + " Comments,"
                                                + " AttributeLang3,"
                                                + " AttributeLang4,"
                                                + " AttributeLang5,"
                                                + " AttributeLang6,"
                                                + " AttributeLang7,"
                                                + " AttributeLang8,"
                                                + " AttributeLang9,"
                                                + " AttributeLang10) VALUES("
                                                + projectInfoScript.ProjectCode + ","
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "0,"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'',"
                                                + "'');";


                    //VALUES();

                    sqlite_cmd3.ExecuteNonQuery();

                }


                sqlite_cmd3.Dispose();

                #endregion

                #region insert LogicTable DB

                //*********************************************************************************************

                SQLiteCommand sqlite_cmd4;
                sqlite_cmd4 = connectionDB.sqlite_conn.CreateCommand();

                //foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
                //{
                //    List<GridInfo> listOfGridInfo = pair.Value;

                int j = 1;
                for (int i = 0; i < listOfLogicalSyntax.Count; i++)
                {
                    LogicalSyntax myLogicalSyntax = listOfLogicalSyntax[i];

                    sqlite_cmd4.CommandText = "INSERT INTO T_LogicTable ("
                                            + " ProjectId,"
                                            + " LogicId,"
                                            + " QId,"
                                            + " LogicTypeId,"
                                            + " IfCondition,"
                                            + " [Then],"
                                            + " [Else]) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + "" + j.ToString() + ","
                                            + "'" + replaceNull(myLogicalSyntax.QId) + "',"
                                            + "'" + replaceNull(myLogicalSyntax.LogicTypeId) + "',"
                                            + "'" + replaceNull(myLogicalSyntax.IfCondition) + "',"
                                            + "'" + replaceNull(myLogicalSyntax.ThenValue) + "',"
                                            + "'" + replaceNull(myLogicalSyntax.ElseValue) + "');";


                    //VALUES();

                    sqlite_cmd4.ExecuteNonQuery();

                    j++;

                    sqlite_cmd4.CommandText = "INSERT INTO T_LogicTable ("
                                            + " ProjectId,"
                                            + " LogicId,"
                                            + " QId,"
                                            + " LogicTypeId,"
                                            + " IfCondition,"
                                            + " [Then],"
                                            + " [Else]) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + j.ToString() + ","
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'');";


                    //VALUES();

                    sqlite_cmd4.ExecuteNonQuery();

                    j++;
                }



                //}

                sqlite_cmd4.Dispose();


                #endregion

                #region insert LogicTableAuto DB

                //*********************************************************************************************

                SQLiteCommand sqlite_cmd5;
                sqlite_cmd5 = connectionDB.sqlite_conn.CreateCommand();

                //foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
                //{
                //    List<GridInfo> listOfGridInfo = pair.Value;
                j = 1;
                for (int i = 0; i < listOfAutoResponse.Count; i++)
                {
                    AutoResponse myAutoResponse = listOfAutoResponse[i];

                    sqlite_cmd5.CommandText = "INSERT INTO T_LogicAuto ("
                                            + " ProjectId,"
                                            + " LogicId,"
                                            + " QId,"
                                            + " LogicTypeId,"
                                            + " IfCondition,"
                                            + " [Then],"
                                            + " [Else]) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + "" + j.ToString() + ","
                                            + "'" + replaceNull(myAutoResponse.QId) + "',"
                                            + "'" + replaceNull(myAutoResponse.LogicTypeId) + "',"
                                            + "'" + replaceNull(myAutoResponse.IfCondition) + "',"
                                            + "'" + replaceNull(myAutoResponse.ThenValue) + "',"
                                            + "'" + replaceNull(myAutoResponse.ElseValue) + "');";


                    //VALUES();

                    sqlite_cmd5.ExecuteNonQuery();

                    j++;

                    sqlite_cmd5.CommandText = "INSERT INTO T_LogicAuto ("
                                            + " ProjectId,"
                                            + " LogicId,"
                                            + " QId,"
                                            + " LogicTypeId,"
                                            + " IfCondition,"
                                            + " [Then],"
                                            + " [Else]) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + j.ToString() + ","
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'');";

                    j++;

                    //VALUES();

                    sqlite_cmd5.ExecuteNonQuery();
                }



                //}

                sqlite_cmd5.Dispose();


                #endregion

                #region insert AttributeFilter DB

                //*********************************************************************************************

                SQLiteCommand sqlite_cmd6;
                sqlite_cmd6 = connectionDB.sqlite_conn.CreateCommand();

                //foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
                //{
                //    List<GridInfo> listOfGridInfo = pair.Value;
                j = 1;
                for (int i = 0; i < listOfAttributeFilter.Count; i++)
                {
                    AttributeFilter myAttributeFilter = listOfAttributeFilter[i];

                    sqlite_cmd6.CommandText = "INSERT INTO T_OptAttrbFilter ("
                                            + " ProjectId,"
                                            + " AttribFilterId,"
                                            + " QId,"
                                            + " InheritedQId,"
                                            + " FilterType,"
                                            + " ExceptionalValue,"
                                            + " LabelTakenFrom) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + "" + j.ToString() + ","
                                            + "'" + replaceNull(myAttributeFilter.QId) + "',"
                                            + "'" + replaceNull(myAttributeFilter.InheritedQId) + "',"
                                            + "'" + replaceNull(myAttributeFilter.FilterType) + "',"
                                            + "'" + replaceNull(myAttributeFilter.ExceptionalValue) + "',"
                                            + "'" + replaceNull(myAttributeFilter.LabelTakenFrom) + "');";


                    //VALUES();

                    sqlite_cmd6.ExecuteNonQuery();

                    j++;

                    sqlite_cmd6.CommandText = "INSERT INTO T_OptAttrbFilter ("
                                            + " ProjectId,"
                                            + " AttribFilterId,"
                                            + " QId,"
                                            + " InheritedQId,"
                                            + " FilterType,"
                                            + " ExceptionalValue,"
                                            + " LabelTakenFrom) VALUES("
                                            + projectInfoScript.ProjectCode + ","
                                            + j.ToString() + ","
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'',"
                                            + "'');";

                    j++;

                    //VALUES();

                    sqlite_cmd6.ExecuteNonQuery();
                }



                //}


                sqlite_cmd6.Dispose();

                #endregion

                #region update language status
                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", Status='1' WHERE DisplayOrder=1";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();


                SQLiteCommand sqlite_cmd71;
                sqlite_cmd71 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd71.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", Status='2' WHERE DisplayOrder>1";

                sqlite_cmd71.ExecuteNonQuery();

                sqlite_cmd71.Dispose();
                #endregion

                //MessageBox.Show("");
                transaction.Commit();
                connectionDB.sqlite_conn.Close();
                connectionDB.sqlite_conn.Dispose();
                connectionDB = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void updateBengaliTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan1.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionBengali='" + listOfQuestionLan1[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan1[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();


                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan1)
                {
                    List<AttributeMain> listOfAttributeLan1 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan1.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan1[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan1)
                {
                    List<GridInfo> listOfGridInfoLan1 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan1.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan1[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion

                #region insert Attribute DB

                ////*********************************************************************************************

                //if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                //    connectionDB.sqlite_conn.Open();

                //SQLiteCommand sqlite_cmd2;
                //sqlite_cmd2 = connectionDB.sqlite_conn.CreateCommand();

                //foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeList)
                //{
                //    List<AttributeMain> listOfAttribute = pair.Value;

                //    for (int i = 0; i < listOfAttribute.Count; i++)
                //    {
                //        AttributeMain myAttribute = listOfAttribute[i];

                //        sqlite_cmd2.CommandText = "INSERT INTO T_OptAttribute ("
                //                                + " ProjectId,"
                //                                + " QId,"
                //                                + " AttributeEnglish,"
                //                                + " AttributeBengali,"
                //                                + " AttributeValue,"
                //                                + " AttributeOrder,"
                //                                + " TakeOpenended,"
                //                                + " IsExclusive,"
                //                                + " LinkId1,"
                //                                + " LinkId2,"
                //                                + " MinValue,"
                //                                + " MaxValue,"
                //                                + " ForceAndMsgOpt,"
                //                                + " GroupName,"
                //                                + " FilterQid,"
                //                                + " FilterType,"
                //                                + " ExcepValue,"
                //                                + " Comments,"
                //                                + " AttributeLang3,"
                //                                + " AttributeLang4,"
                //                                + " AttributeLang5,"
                //                                + " AttributeLang6,"
                //                                + " AttributeLang7,"
                //                                + " AttributeLang8,"
                //                                + " AttributeLang9,"
                //                                + " AttributeLang10) VALUES("
                //                                + projectInfoScript.ProjectCode + ","
                //                                + "'" + pair.Key + "',"
                //                                + "'" + replaceNull(myAttribute.AttributeEnglish) + "',"
                //                                + "'" + replaceNull(myAttribute.AttributeBengali) + "',"
                //                                + "'" + replaceNull(myAttribute.AttributeValue) + "',"
                //                                + "" + replaceNull(myAttribute.AttributeOrder) + ","
                //                                + "'" + replaceNull(myAttribute.TakeOpenended) + "',"
                //                                + "'" + replaceNull(myAttribute.IsExclusive) + "',"
                //                                + "'" + replaceNull(myAttribute.LinkId1) + "',"
                //                                + "'" + replaceNull(myAttribute.LinkId2) + "',"
                //                                + "'" + replaceNull(myAttribute.MinValue) + "',"
                //                                + "'" + replaceNull(myAttribute.MaxValue) + "',"
                //                                + "'" + replaceNull(myAttribute.ForceAndMsgOpt) + "',"
                //                                + "'" + replaceNull(myAttribute.GroupName) + "',"
                //                                + "'" + replaceNull(myAttribute.FilterQid) + "',"
                //                                + "'" + replaceNull(myAttribute.FilterType) + "',"
                //                                + "'" + replaceNull(myAttribute.ExcepValue) + "',"
                //                                + "'" + replaceNull(myAttribute.Comments) + "',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'');";


                //        //VALUES();

                //        sqlite_cmd2.ExecuteNonQuery();
                //    }

                //    sqlite_cmd2.CommandText = "INSERT INTO T_OptAttribute ("
                //                                + " ProjectId,"
                //                                + " QId,"
                //                                + " AttributeEnglish,"
                //                                + " AttributeBengali,"
                //                                + " AttributeValue,"
                //                                + " AttributeOrder,"
                //                                + " TakeOpenended,"
                //                                + " IsExclusive,"
                //                                + " LinkId1,"
                //                                + " LinkId2,"
                //                                + " MinValue,"
                //                                + " MaxValue,"
                //                                + " ForceAndMsgOpt,"
                //                                + " GroupName,"
                //                                + " FilterQid,"
                //                                + " FilterType,"
                //                                + " ExcepValue,"
                //                                + " Comments,"
                //                                + " AttributeLang3,"
                //                                + " AttributeLang4,"
                //                                + " AttributeLang5,"
                //                                + " AttributeLang6,"
                //                                + " AttributeLang7,"
                //                                + " AttributeLang8,"
                //                                + " AttributeLang9,"
                //                                + " AttributeLang10) VALUES("
                //                                + projectInfoScript.ProjectCode + ","
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "0,"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'');";


                //    //VALUES();

                //    sqlite_cmd2.ExecuteNonQuery();
                //    sqlite_cmd2.ExecuteNonQuery();
                //    sqlite_cmd2.ExecuteNonQuery();
                //    sqlite_cmd2.ExecuteNonQuery();
                //    sqlite_cmd2.ExecuteNonQuery();

                //}


                //sqlite_cmd2.Dispose();

                //if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                //    connectionDB.sqlite_conn.Close();

                #endregion

                #region insert GridInfo DB

                //*********************************************************************************************

                //if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                //    connectionDB.sqlite_conn.Open();

                //SQLiteCommand sqlite_cmd3;
                //sqlite_cmd3 = connectionDB.sqlite_conn.CreateCommand();

                //foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
                //{
                //    List<GridInfo> listOfGridInfo = pair.Value;

                //    for (int i = 0; i < listOfGridInfo.Count; i++)
                //    {
                //        GridInfo myGridInfo = listOfGridInfo[i];

                //        sqlite_cmd3.CommandText = "INSERT INTO T_GridInfo ("
                //                                + " ProjectId,"
                //                                + " QId,"
                //                                + " AttributeEnglish,"
                //                                + " AttributeBengali,"
                //                                + " AttributeValue,"
                //                                + " AttributeOrder,"
                //                                + " TakeOpenended,"
                //                                + " IsExclusive,"
                //                                + " MinValue,"
                //                                + " MaxValue,"
                //                                + " ForceAndMsgOpt,"
                //                                + " Comments,"
                //                                + " AttributeLang3,"
                //                                + " AttributeLang4,"
                //                                + " AttributeLang5,"
                //                                + " AttributeLang6,"
                //                                + " AttributeLang7,"
                //                                + " AttributeLang8,"
                //                                + " AttributeLang9,"
                //                                + " AttributeLang10) VALUES("
                //                                + projectInfoScript.ProjectCode + ","
                //                                + "'" + pair.Key + "',"
                //                                + "'" + replaceNull(myGridInfo.AttributeEnglish) + "',"
                //                                + "'" + replaceNull(myGridInfo.AttributeBengali) + "',"
                //                                + "'" + replaceNull(myGridInfo.AttributeValue) + "',"
                //                                + "" + replaceNull(myGridInfo.AttributeOrder) + ","
                //                                + "'" + replaceNull(myGridInfo.TakeOpenended) + "',"
                //                                + "'" + replaceNull(myGridInfo.IsExclusive) + "',"
                //                                + "'" + replaceNull(myGridInfo.MinValue) + "',"
                //                                + "'" + replaceNull(myGridInfo.MaxValue) + "',"
                //                                + "'" + replaceNull(myGridInfo.ForceAndMsgOpt) + "',"
                //                                + "'" + replaceNull(myGridInfo.Comments) + "',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'');";


                //        //VALUES();

                //        sqlite_cmd3.ExecuteNonQuery();
                //    }

                //    sqlite_cmd3.CommandText = "INSERT INTO T_GridInfo ("
                //                                + " ProjectId,"
                //                                + " QId,"
                //                                + " AttributeEnglish,"
                //                                + " AttributeBengali,"
                //                                + " AttributeValue,"
                //                                + " AttributeOrder,"
                //                                + " TakeOpenended,"
                //                                + " IsExclusive,"
                //                                + " MinValue,"
                //                                + " MaxValue,"
                //                                + " ForceAndMsgOpt,"
                //                                + " Comments,"
                //                                + " AttributeLang3,"
                //                                + " AttributeLang4,"
                //                                + " AttributeLang5,"
                //                                + " AttributeLang6,"
                //                                + " AttributeLang7,"
                //                                + " AttributeLang8,"
                //                                + " AttributeLang9,"
                //                                + " AttributeLang10) VALUES("
                //                                + projectInfoScript.ProjectCode + ","
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "0,"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'',"
                //                                + "'');";


                //    //VALUES();

                //    sqlite_cmd3.ExecuteNonQuery();

                //}


                //sqlite_cmd3.Dispose();

                //if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                //    connectionDB.sqlite_conn.Close();

                #endregion

                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[0] + "', Status='1' WHERE DisplayOrder=2";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionBengali FROM T_Question WHERE QuestionEnglish<>'' AND QuestionBengali='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeBengali, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeBengali='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeBengali, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeBengali='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionBengali='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeBengali='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeBengali='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }



        }

        private void update3rdTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan2.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang3='" + listOfQuestionLan2[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan2[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan2)
                {
                    List<AttributeMain> listOfAttributeLan2 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan2.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan2[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang3='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan2)
                {
                    List<GridInfo> listOfGridInfoLan2 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan2.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan2[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang3='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[1] + "', Status='1' WHERE DisplayOrder=3";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang3 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang3='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang3, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang3='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang3, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang3='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang3='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang3='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang3='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update4thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan3.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang4='" + listOfQuestionLan3[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan3[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan3)
                {
                    List<AttributeMain> listOfAttributeLan3 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan3.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan3[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang4='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan3)
                {
                    List<GridInfo> listOfGridInfoLan3 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan3.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan3[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang4='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[2] + "', Status='1' WHERE DisplayOrder=4";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang4 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang4='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang4, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang4='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang4, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang4='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang4='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang4='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang4='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update5thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan4.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang5='" + listOfQuestionLan4[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan4[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan4)
                {
                    List<AttributeMain> listOfAttributeLan4 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan4.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan4[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang5='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan4)
                {
                    List<GridInfo> listOfGridInfoLan4 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan4.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan4[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang5='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[3] + "', Status='1' WHERE DisplayOrder=5";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang5 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang5='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang5, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang5='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang5, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang5='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang5='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang5='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang5='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update6thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan5.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang6='" + listOfQuestionLan5[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan5[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan5)
                {
                    List<AttributeMain> listOfAttributeLan5 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan5.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan5[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang6='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan5)
                {
                    List<GridInfo> listOfGridInfoLan5 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan5.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan5[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang6='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[4] + "', Status='1' WHERE DisplayOrder=6";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang6 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang6='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang6, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang6='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang6, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang6='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang6='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang6='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang6='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update7thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan6.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang7='" + listOfQuestionLan6[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan6[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan6)
                {
                    List<AttributeMain> listOfAttributeLan6 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan6.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan6[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang7='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan6)
                {
                    List<GridInfo> listOfGridInfoLan6 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan6.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan6[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang7='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[5] + "', Status='1' WHERE DisplayOrder=7";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang7 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang7='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang7, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang7='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang7, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang7='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang7='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang7='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang7='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update8thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan7.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang8='" + listOfQuestionLan7[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan7[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan7)
                {
                    List<AttributeMain> listOfAttributeLan7 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan7.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan7[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang8='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan7)
                {
                    List<GridInfo> listOfGridInfoLan7 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan7.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan7[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang8='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[6] + "', Status='1' WHERE DisplayOrder=8";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang8 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang8='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang8, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang8='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang8, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang8='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang8='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang8='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang8='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update9thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan8.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang9='" + listOfQuestionLan8[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan8[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan8)
                {
                    List<AttributeMain> listOfAttributeLan8 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan8.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan8[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang9='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan8)
                {
                    List<GridInfo> listOfGridInfoLan8 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan8.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan8[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang9='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[7] + "', Status='1' WHERE DisplayOrder=9";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang9 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang9='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang9, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang9='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang9, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang9='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang9='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang9='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang9='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void update10thTranslation()
        {
            ConnectionDB connectionDB = new ConnectionDB();

            if (connectionDB.connect(myPath + "\\" + projectInfoScript.DatabaseName + ".db"))
            {

                #region update Question DB

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand command1 = new SQLiteCommand(connectionDB.sqlite_conn);
                for (int x = 0; x < listOfQuestionLan9.Count; x++)
                {
                    command1.CommandText = ("UPDATE T_Question SET QuestionLang10='" + listOfQuestionLan9[x].QuestionEnglish.Replace("'", "''") + "' WHERE QId='" + listOfQuestionLan9[x].QId + "'");
                    command1.ExecuteNonQuery();
                }

                command1.Dispose();

                #endregion

                #region update Attribute DB

                SQLiteCommand command2 = new SQLiteCommand(connectionDB.sqlite_conn);
                foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeListLan9)
                {
                    List<AttributeMain> listOfAttributeLan9 = pair.Value;

                    for (int i = 0; i < listOfAttributeLan9.Count; i++)
                    {
                        AttributeMain myAttribute = listOfAttributeLan9[i];


                        command2.CommandText = ("UPDATE T_OptAttribute SET AttributeLang10='" + myAttribute.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myAttribute.AttributeValue + "'");
                        command2.ExecuteNonQuery();
                    }
                }

                command2.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                #region update GridInfo DB

                SQLiteCommand command3 = new SQLiteCommand(connectionDB.sqlite_conn);

                foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsListLan9)
                {
                    List<GridInfo> listOfGridInfoLan9 = pair.Value;

                    for (int i = 0; i < listOfGridInfoLan9.Count; i++)
                    {
                        GridInfo myGridInfo = listOfGridInfoLan9[i];


                        command3.CommandText = ("UPDATE T_GridInfo SET AttributeLang10='" + myGridInfo.AttributeEnglish.Replace("'", "''") + "' WHERE QId='" + pair.Key + "' AND AttributeValue='" + myGridInfo.AttributeValue + "'");
                        command3.ExecuteNonQuery();
                    }
                }

                command3.Dispose();

                //for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                //{
                //    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                //    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                //    command.ExecuteNonQuery();
                //}

                #endregion

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();


                #region update language status

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();

                SQLiteCommand sqlite_cmd7;
                sqlite_cmd7 = connectionDB.sqlite_conn.CreateCommand();

                sqlite_cmd7.CommandText = "UPDATE T_LanguageMaster SET ProjectId=" + projectInfoScript.ProjectCode + ", LanguageName='" + listOfLanguage[8] + "', Status='1' WHERE DisplayOrder=10";

                sqlite_cmd7.ExecuteNonQuery();

                sqlite_cmd7.Dispose();

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                #endregion


                #region check missing language

                Dictionary<string, string> dicQidvsEnglishLabelQuestion = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelAttribute = new Dictionary<string, string>();
                Dictionary<string, string> dicQidvsEnglishLabelGridAttribute = new Dictionary<string, string>();

                if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                    connectionDB.sqlite_conn.Open();
                // Question Table
                SQLiteDataAdapter da = new SQLiteDataAdapter("SELECT QId, QuestionEnglish, QuestionLang10 FROM T_Question WHERE QuestionEnglish<>'' AND QuestionLang10='' ", connectionDB.sqlite_conn);

                //Using Data Table
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelQuestion.Add(row["QId"].ToString(), row["QuestionEnglish"].ToString());
                }

                //Attribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang10, AttributeOrder FROM T_OptAttribute WHERE AttributeEnglish<>'' AND AttributeLang10='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                //GridAttribute Table
                da = new SQLiteDataAdapter("SELECT QId, AttributeEnglish, AttributeLang10, AttributeOrder FROM T_GridInfo WHERE AttributeEnglish<>'' AND AttributeLang10='' ", connectionDB.sqlite_conn);

                //Using Data Table
                dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    dicQidvsEnglishLabelGridAttribute.Add(row["QId"].ToString() + "*" + row["AttributeOrder"].ToString(), row["AttributeEnglish"].ToString());
                }

                if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                    connectionDB.sqlite_conn.Close();

                //******************************************************************************************************************

                if (dicQidvsEnglishLabelQuestion.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd8 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelQuestion)
                    {
                        sqlite_cmd8.CommandText = "UPDATE T_Question SET QuestionLang10='" + pair.Value + "' WHERE QId='" + pair.Key + "'";
                        sqlite_cmd8.ExecuteNonQuery();
                    }

                    sqlite_cmd8.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }
                //Attribute Table
                if (dicQidvsEnglishLabelAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd9 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelAttribute)
                    {
                        sqlite_cmd9.CommandText = "UPDATE T_OptAttribute SET AttributeLang10='" + pair.Value.Replace("'", "''") + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd9.ExecuteNonQuery();
                    }

                    sqlite_cmd9.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }

                //GridAttribute Table
                if (dicQidvsEnglishLabelGridAttribute.Count > 0)
                {
                    if (connectionDB.sqlite_conn.State == ConnectionState.Closed)
                        connectionDB.sqlite_conn.Open();

                    SQLiteCommand sqlite_cmd10 = new SQLiteCommand(connectionDB.sqlite_conn);
                    foreach (KeyValuePair<string, string> pair in dicQidvsEnglishLabelGridAttribute)
                    {
                        sqlite_cmd10.CommandText = "UPDATE T_GridInfo SET AttributeLang10='" + pair.Value + "' WHERE QId='" + pair.Key.Split('*')[0] + "' AND AttributeOrder=" + pair.Key.Split('*')[1];
                        sqlite_cmd10.ExecuteNonQuery();
                    }

                    sqlite_cmd10.Dispose();

                    if (connectionDB.sqlite_conn.State == ConnectionState.Open)
                        connectionDB.sqlite_conn.Close();

                }


                #endregion

                //MessageBox.Show("");
                connectionDB = null;

                scriptFilePath = myPath + "\\" + projectInfoScript.DatabaseName + ".db";
            }

        }

        private void checkEnglishBengaliScript(TextWriter txtWriter, List<Question> listOfQuestionLan, Dictionary<String, List<AttributeMain>> dicQidVsAttributeListLan, Dictionary<String, List<GridInfo>> dicGridListNameVsListLan, int LanguageNo)
        {
            for (int i = 0; i < listOfQuestion.Count; i++)
            {
                string QidEnglish = listOfQuestion[i].QId;
                string QidLan1 = "";
                if (listOfQuestion[i].HasAutoResponse != "1" && listOfQuestion[i].HasAutoResponse != "2")
                {
                    bool hasQid = false;

                    for (int j = 0; j < listOfQuestionLan.Count; j++)
                    {
                        QidLan1 = listOfQuestionLan[j].QId;

                        if (QidEnglish == QidLan1)
                        {
                            hasQid = true;
                            break;
                        }
                    }

                    if (hasQid == false)
                    {
                        //txtWriter.WriteLine(QidEnglish + " " + QidLan1 + " not exit in Language " + LanguageNo.ToString());
                        if (QidEnglish != null)
                            txtWriter.WriteLine(QidEnglish + " not exit in Language " + LanguageNo.ToString());
                    }
                }

            }

            foreach (KeyValuePair<string, List<AttributeMain>> pair in dicQidVsAttributeList)
            {
                List<AttributeMain> listOfAttribute = pair.Value;

                if (dicQidVsAttributeListLan.ContainsKey(pair.Key))
                {
                    List<AttributeMain> listOfAttributeLan1 = dicQidVsAttributeListLan[pair.Key];

                    if (listOfAttribute.Count != listOfAttributeLan1.Count)
                        txtWriter.WriteLine(pair.Key + " Number of attributes are not same " + LanguageNo.ToString());

                    for (int i = 0; i < listOfAttribute.Count; i++)
                    {
                        string attributeValue = listOfAttribute[i].AttributeValue;
                        bool hasQid = false;
                        for (int j = 0; j < listOfAttributeLan1.Count; j++)
                        {
                            string attributeValueLan1 = listOfAttributeLan1[j].AttributeValue;

                            if (attributeValue == attributeValueLan1)
                            {
                                hasQid = true;
                                break;
                            }
                        }

                        if (hasQid == false)
                        {
                            txtWriter.WriteLine(pair.Key + " : Attribute not matched in Language  " + LanguageNo.ToString());
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, List<GridInfo>> pair in dicGridListNameVsList)
            {
                List<GridInfo> listOfGridAttribute = pair.Value;

                if (dicGridListNameVsListLan.ContainsKey(pair.Key))
                {
                    List<GridInfo> listOfGridAttributeLan = dicGridListNameVsListLan[pair.Key];

                    if (listOfGridAttribute.Count != listOfGridAttributeLan.Count)
                        txtWriter.WriteLine(pair.Key + " Number of attributes are not same " + LanguageNo.ToString());

                    for (int i = 0; i < listOfGridAttribute.Count; i++)
                    {
                        string attributeValue = listOfGridAttribute[i].AttributeValue;
                        bool hasQid = false;
                        for (int j = 0; j < listOfGridAttributeLan.Count; j++)
                        {
                            string attributeValueLan = listOfGridAttributeLan[j].AttributeValue;
                            if (attributeValue == attributeValueLan)
                            {
                                hasQid = true;
                                break;
                            }
                        }

                        if (hasQid == false)
                        {
                            txtWriter.WriteLine(pair.Key + " : Attribute not matched in Language  " + LanguageNo.ToString());
                        }
                    }
                }
            }
        }

        private string[] splitedBy(string myString, string splitedby)
        {
            string[] splitArray = new string[] { };
            string[] splitValue = splitedby.Split(',');
            List<string> list = splitValue.ToList();

            int index = 0;
            string temp = "";
            for (int i = 0; i < myString.Length; i++)
            {
                if (!list.Contains(myString.Substring(i, 1)))
                    temp = temp + myString.Substring(i, 1);
                else
                {
                    splitArray[index] = temp.Trim();
                }
            }

            return splitArray;
        }

        private string replaceNull(object myObj)
        {
            if (myObj == null)
                return "";
            else
                return myObj.ToString().Replace("'", "''");//.Replace("\"", "''");
        }

        private void frmBuildScript_Loaded(object sender, RoutedEventArgs e)
        {
            listOfKeyWords = new List<string>();

            listOfKeyWords.Add("QUESTION");
            listOfKeyWords.Add("SR");
            listOfKeyWords.Add("MR");
            listOfKeyWords.Add("ALPHA");
            listOfKeyWords.Add("NUMBER");
            listOfKeyWords.Add("RANK");
            listOfKeyWords.Add("IMAGE");
            listOfKeyWords.Add("GRIDSR");
            listOfKeyWords.Add("GRIDMR");
            listOfKeyWords.Add("GRIDNUM");
            listOfKeyWords.Add("MEDIA");
            listOfKeyWords.Add("ALPHALIST");
            listOfKeyWords.Add("NUMLIST");
            listOfKeyWords.Add("DATE");
            listOfKeyWords.Add("TIME");
            listOfKeyWords.Add("CAPTUREIMAGE");
            listOfKeyWords.Add("NUMLISTTOTAL");
            listOfKeyWords.Add("AUTOCOMPLETE");
            listOfKeyWords.Add("AUTOCOMPLETELIST");
            listOfKeyWords.Add("AUTOCOMPLETEANS");
            listOfKeyWords.Add("DROPDOWN");
            listOfKeyWords.Add("DROPDOWNLIST");
            listOfKeyWords.Add("DRAGDROP");
            listOfKeyWords.Add("FORM");
            listOfKeyWords.Add("INFO");
            listOfKeyWords.Add("MAXDIFF");
            listOfKeyWords.Add("GPS");
            listOfKeyWords.Add("RECORDING");
            listOfKeyWords.Add("NOBACKBTN");
            listOfKeyWords.Add("NONEXTBTN");
            listOfKeyWords.Add("INCLUDEGRIDLIST");
            listOfKeyWords.Add("SHOWASFORM");
            listOfKeyWords.Add("DIRIMAGE");
            listOfKeyWords.Add("SHOWASNUMTEXT");
            //listOfKeyWords.Add("KEEPDOWNLOAD");
            listOfKeyWords.Add("INRLD");
            listOfKeyWords.Add("QLABEL");
            listOfKeyWords.Add("IDOF");


            listOfKeyWords.Add("IF");
            listOfKeyWords.Add("RANDOM");
            listOfKeyWords.Add("ROT");
            listOfKeyWords.Add("QROT");
            listOfKeyWords.Add("FROT");
            listOfKeyWords.Add("GROUPROT");
            listOfKeyWords.Add("BLOCK");
            listOfKeyWords.Add("GRANDOM");
            listOfKeyWords.Add("GROT");

            listOfKeyWords.Add("OTPGROUPROT");
            listOfKeyWords.Add("OTPROTGROUP");
            listOfKeyWords.Add("OTPROTGROUPROT");


            listOfKeyWords.Add("MIN");
            listOfKeyWords.Add("MAX");
            listOfKeyWords.Add("MANDATORY");
            listOfKeyWords.Add("COLUMN");
            listOfKeyWords.Add("INCLUDE");
            listOfKeyWords.Add("INCLUDEBYORDER");
            listOfKeyWords.Add("EXCLUDE");
            listOfKeyWords.Add("FILTER");
            listOfKeyWords.Add("GOTO");
            listOfKeyWords.Add("DELAY");
            listOfKeyWords.Add("EXCEPT");
            listOfKeyWords.Add("FONTSIZE");
            listOfKeyWords.Add("IMGADJBY");
            listOfKeyWords.Add("LAT");
            listOfKeyWords.Add("LON");
            listOfKeyWords.Add("COMPVAL");


            listOfKeyWords.Add("LIST");
            listOfKeyWords.Add("USELIST");
            listOfKeyWords.Add("GRIDLIST");
            listOfKeyWords.Add("USEGRIDLIST");
            listOfKeyWords.Add("OPEN");
            listOfKeyWords.Add("NMUL");
            listOfKeyWords.Add("NOCON");
            listOfKeyWords.Add("DKCS");
            listOfKeyWords.Add("FIFS");
            listOfKeyWords.Add("END");
            listOfKeyWords.Add("TERMINATE");
            listOfKeyWords.Add("HORIZONTAL");
            listOfKeyWords.Add("JUMPFOR");
            listOfKeyWords.Add("TAKEONLYONE");



            listOfKeyWords.Add("DUMMY1");
            listOfKeyWords.Add("DUMMY2");

            listOfKeyWords.Add("STARTREC");
            listOfKeyWords.Add("ENDREC");
            listOfKeyWords.Add("EXTCAMERA");
            listOfKeyWords.Add("SCALE7");
            listOfKeyWords.Add("SCALE10");

            listOfKeyWords.Add("ADDRESS1");
            listOfKeyWords.Add("ADDRESS2");
            listOfKeyWords.Add("ADDRESS3");
            listOfKeyWords.Add("ADDRESS4");
            listOfKeyWords.Add("PICT");
            listOfKeyWords.Add("VIDEO");

            listOfKeyWords.Add("REPEAT");
            listOfKeyWords.Add("ENDREPEAT");


            this.getShellDB();

        }

        private void getShellDB()
        {

            if (!Directory.Exists("C:\\Temp\\ShellDB"))
                Directory.CreateDirectory("C:\\Temp\\ShellDB");

            //String sTemp = System.AppDomain.CurrentDomain.BaseDirectory + "\\ShellDB";
            String sTemp = "C:\\Temp\\ShellDB";
            string[] fileArray = Directory.GetFiles(sTemp, "*.db");

            comShellDBType.Items.Clear();

            for (int i = 0; i < fileArray.Length; i++)
            {
                comShellDBType.Items.Add(fileArray[i].Substring(fileArray[i].LastIndexOf('\\') + 1));
            }

            if (comShellDBType.Items.Contains("SYSHELDB.db"))
                comShellDBType.Text = "SYSHELDB.db";
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text == "")
            {
                MessageBox.Show("Script must be selected first.");
                return;
            }

            if (!File.Exists(txtScriptPath.Text))
            {
                MessageBox.Show("Selected script file is not valid.");
                return;
            }

            string dbFileName     = projectInfoScript.DatabaseName + ".db";
            string dbFilePath     = myPath + "\\" + dbFileName;
            string sSelectedQFile = System.IO.Path.GetFileName(txtScriptPath.Text);

            if (!File.Exists(dbFilePath))
            {
                MessageBox.Show("Build output (.db) not found. Please execute the build first.");
                return;
            }

            ClearBuildOutput();
            SetUIState(true);
            txtStatus.Text = "Uploading, please wait...";

            try
            {
                // Copy .db to temp folder
                if (!Directory.Exists(myPath + "\\Temp"))
                    Directory.CreateDirectory(myPath + "\\temp");
                if (!File.Exists(myPath + "\\temp\\" + dbFileName))
                    File.Copy(dbFilePath, myPath + "\\temp\\" + dbFileName);
                else
                {
                    File.Delete(myPath + "\\temp\\" + dbFileName);
                    File.Copy(dbFilePath, myPath + "\\temp\\" + dbFileName);
                }

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                // Step 1 - Upload .db file
                txtStatus.Text = "Uploading .db file...";
                AppendResult("Uploading database file (" + dbFileName + ")...", false);

                string uploadMessage;
                using (WebClient client = new WebClient())
                {
                    client.Credentials = CredentialCache.DefaultCredentials;
                    byte[] responseArray = await client.UploadFileTaskAsync(
                        StaticClass.SERVER_URL + "/deskapi/uploaddbfile.php", "POST",
                        myPath + "\\temp\\" + dbFileName);
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
                        txtScriptPath.Text);
                    uploadMessage = client2.Encoding.GetString(responseArray2);
                }

                if (uploadMessage != "Script uploaded successfully..")
                {
                    AppendResult("Script upload failed: " + uploadMessage, true);
                    txtStatus.Text = "Upload failed.";
                    return;
                }
                AppendResult("Script file uploaded successfully.", false);

                // Step 3 - Update script version
                txtStatus.Text = "Updating script version...";
                AppendResult("Updating script version on server...", false);

                MyWebRequest myRequest = new MyWebRequest(
                    StaticClass.SERVER_URL + "/deskapi/updatescriptversion.php",
                    "POST",
                    "projectId=" + projectInfoScript.ProjectCode +
                    "&scriptVersion=" + projectInfoScript.ScriptVersion +
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

        private List<string> checkScript()
        {
            List<string> listOfErrorMsg = new List<string>();
            for (int i = 0; i < listOfQuestion.Count; i++)
            {
                string qid = listOfQuestion[i].QId;
                string qtype = listOfQuestion[i].QType;
                if (qid == null)
                    listOfErrorMsg.Add("QId Missing");
                else
                {
                    if (qtype == "48")
                    {
                        List<AttributeMain> listOfAttribute = dicQidVsAttributeList[qid];
                        if (listOfAttribute.Count == 0)
                        {
                            listOfErrorMsg.Add(qid + " - Form Question has no attribute");
                        }
                        //else
                        //{
                        //    for (int x = 0; x < listOfAttribute.Count; x++)
                        //    {
                        //        if (listOfAttribute[x].LinkId1 == "" || listOfAttribute[x].LinkId1 == null)
                        //            listOfErrorMsg.Add(qid + " - Invalid attribute properties, Form Question must have qtype");
                        //        else if ((listOfAttribute[x].LinkId1 == "1" || listOfAttribute[x].LinkId1 == "2") && (listOfAttribute[x].LinkId2 == "" || listOfAttribute[x].LinkId2 == null))
                        //            listOfErrorMsg.Add(qid + " - Invalid attribute properties, Form Question must have qtype");

                        //        //if (listOfAttribute[x].LinkId2 == "" || listOfAttribute[x].LinkId2 == null)
                        //        //    listOfErrorMsg.Add( qid + " - Invalid attribute properties, Must have qtype");
                        //    }
                        //}
                    }
                    if (qtype == "60")
                    {
                        List<AttributeMain> listOfAttribute = dicQidVsAttributeList[qid];
                        if (listOfAttribute.Count == 0)
                        {
                            listOfErrorMsg.Add(qid + " - FIFSInfo Question has no attribute");
                        }
                        else
                        {
                            for (int x = 0; x < listOfAttribute.Count; x++)
                            {
                                if (listOfAttribute[x].LinkId1 == "" || listOfAttribute[x].LinkId1 == null)
                                    listOfErrorMsg.Add(qid + " - Invalid attribute properties, Form Question must have qtype");
                                else if ((listOfAttribute[x].LinkId1 == "1" || listOfAttribute[x].LinkId1 == "2") && (listOfAttribute[x].LinkId2 == "" || listOfAttribute[x].LinkId2 == null))
                                    listOfErrorMsg.Add(qid + " - Invalid attribute properties, Form Question must have qtype");

                                //if (listOfAttribute[x].LinkId2 == "" || listOfAttribute[x].LinkId2 == null)
                                //    listOfErrorMsg.Add( qid + " - Invalid attribute properties, Must have qtype");
                            }
                        }
                    }
                    else if (qtype == "1" || (qtype == "2"))
                    {
                        List<AttributeMain> listOfAttribute = dicQidVsAttributeList[qid];
                        if (listOfAttribute.Count == 0)
                        {
                            listOfErrorMsg.Add(qid + " - Attribute missing");
                        }
                        else
                        {
                            for (int x = 0; x < listOfAttribute.Count; x++)
                            {
                                if (listOfAttribute[x].LinkId1 != "" && listOfAttribute[x].LinkId1 != null)
                                    listOfErrorMsg.Add(qid + " - Attribute properties should not exist");
                                else if (listOfAttribute[x].LinkId2 != "" && listOfAttribute[x].LinkId2 != null)
                                    listOfErrorMsg.Add(qid + " - Attribute properties should not exist");

                                //if (listOfAttribute[x].LinkId2 == "" || listOfAttribute[x].LinkId2 == null)
                                //    listOfErrorMsg.Add( qid + " - Invalid attribute properties, Must have qtype");
                            }
                        }
                    }
                }

            }

            return listOfErrorMsg;
        }

        private string checkIfCondition(String expression)
        {
            string myExpression = expression.ToUpper();
            if (myExpression.Contains("MOBILENUMBER"))
            {
                char ch = '[';
                int freq = Regex.Matches(myExpression, ch.ToString()).Count;
                if (freq != 1) return "Incorrect expression [ must be exist at least one";
                ch = ']';
                freq = Regex.Matches(myExpression, ch.ToString()).Count;
                if (freq != 1) return "Incorrect expression ] must be exist at least one";
            }

            return "true";
        }

        // ── *REPEAT helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Builds the iteration value list for a *REPEAT block.
        /// source is either "1 TO 10" (numeric range) or a *QUESTION QId.
        /// Returns null and writes an error if the source is invalid.
        /// </summary>
        private List<string> BuildRepeatIterationList(string source, TextWriter txtWriter, int lineNo)
        {
            List<string> result = new List<string>();

            // numeric range: "X TO Y"
            Match rangeMatch = Regex.Match(source.Trim(), @"^(\d+)\s+TO\s+(\d+)$", RegexOptions.IgnoreCase);
            if (rangeMatch.Success)
            {
                int start = int.Parse(rangeMatch.Groups[1].Value);
                int end   = int.Parse(rangeMatch.Groups[2].Value);
                if (start >= end)
                {
                    txtWriter.WriteLine("Line : " + lineNo + " *REPEAT range invalid: start must be less than end [" + source + "]");
                    return null;
                }
                for (int v = start; v <= end; v++)
                    result.Add(v.ToString());
                return result;
            }

            // *QUESTION QId source
            string qid = source.Trim();
            if (!dicQidVsAttributeList.ContainsKey(qid))
            {
                txtWriter.WriteLine("Line : " + lineNo + " *REPEAT source QId '" + qid + "' not found");
                return null;
            }
            foreach (AttributeMain attr in dicQidVsAttributeList[qid])
            {
                if (attr.AttributeEnglish != null && attr.AttributeEnglish.Contains("None"))
                    break;
                if (!string.IsNullOrEmpty(attr.AttributeValue))
                    result.Add(attr.AttributeValue);
            }
            if (result.Count == 0)
            {
                txtWriter.WriteLine("Line : " + lineNo + " *REPEAT source QId '" + qid + "' has no attributes");
                return null;
            }
            return result;
        }

        /// <summary>
        /// Expands a *REPEAT buffer for the English (main) script section.
        /// Pass 1: pre-registers all QIds that will be generated so cross-references
        ///         inside the block (e.g. *IF [Brand?R=1]) validate correctly.
        /// Pass 2: substitutes ?R with each iteration value and feeds lines through
        ///         the existing parsers unchanged.
        /// </summary>
        private void ExpandRepeatBlockEnglish(
            List<string> repeatBuffer,
            List<string> iterationList,
            List<string> listOfQuestionIdForDupliCheck,
            List<string> listOfGridListForDupliCheck,
            TextWriter txtWriter)
        {
            // Pass 1 — pre-register all generated QIds
            foreach (string iterVal in iterationList)
            {
                foreach (string bufLine in repeatBuffer)
                {
                    if (bufLine.Trim().Split(' ')[0].ToUpper() != "*QUESTION") continue;
                    string expandedLine = bufLine.Replace("?R", iterVal);
                    string[] parts = expandedLine.Trim().Split(' ');
                    if (parts.Length >= 2)
                    {
                        string genQId = parts[1].Trim();
                        if (Regex.IsMatch(genQId, "^[a-zA-Z0-9]+$") &&
                            !listOfQuestionIdForDupliCheck.Contains(genQId))
                            listOfQuestionIdForDupliCheck.Add(genQId);
                    }
                }
            }

            // Pass 2 — expand and parse each iteration
            foreach (string iterVal in iterationList)
            {
                List<string> expandedLines = new List<string>();
                foreach (string bl in repeatBuffer)
                    expandedLines.Add(bl.Replace("?R", iterVal));
                expandedLines.Add("*"); // sentinel — terminates prepareQuestion's lookahead

                // build a local dicLine for this buffer (buffer-relative line numbers)
                Dictionary<int, int> dicLineLocal = new Dictionary<int, int>();
                for (int x = 0; x < expandedLines.Count + 5; x++)
                    dicLineLocal[x + 1] = x + 1;

                for (int bi = 0; bi < expandedLines.Count; bi++)
                {
                    string bLine = expandedLines[bi];
                    if (string.IsNullOrWhiteSpace(bLine) || bLine[0] != '*') continue;

                    if (bLine.Split(' ')[0].ToUpper() == "*LIST")
                    {
                        bi = prepareList(expandedLines, bi, txtWriter, dicLineLocal);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                    }
                    if (bi < expandedLines.Count && bLine.Split(' ')[0].ToUpper() == "*GRIDLIST")
                    {
                        bi = prepareGridList(expandedLines, bi, listOfGridListForDupliCheck, txtWriter, dicLineLocal);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                    }
                    if (bi < expandedLines.Count && bLine.Trim().Split(' ')[0].ToUpper() == "*IF")
                    {
                        List<AutoResponse>   arTemp = new List<AutoResponse>();
                        List<LogicalSyntax>  lsTemp = new List<LogicalSyntax>();
                        bi = prepareIf(expandedLines, bi, listOfQuestionIdForDupliCheck, arTemp, lsTemp, txtWriter, dicLineLocal);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                        foreach (AutoResponse  ar in arTemp) listOfAutoResponse.Add(ar);
                        foreach (LogicalSyntax ls in lsTemp) listOfLogicalSyntax.Add(ls);
                    }
                    if (bi < expandedLines.Count &&
                        (bLine.Trim().Split(' ')[0].Trim().ToUpper() == "*INCLUDE" ||
                         bLine.Trim().Split(' ')[0].Trim().ToUpper() == "*EXCLUDE"))
                    {
                        List<AutoResponse> arTemp = new List<AutoResponse>();
                        bi = prepareIncludeExclude(expandedLines, bi, listOfQuestionIdForDupliCheck, arTemp, txtWriter, dicLineLocal);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                        foreach (AutoResponse ar in arTemp) listOfAutoResponse.Add(ar);
                    }
                    if (bi < expandedLines.Count && bLine.Trim().Split(' ')[0].ToUpper() == "*STARTREC")
                    {
                        string[] xyz = bLine.Split('"');
                        if (xyz.Length == 3) silentRecording = xyz[1];
                        else txtWriter.WriteLine("*REPEAT block: Invalid *STARTREC syntax");
                    }
                    if (bi < expandedLines.Count && bLine.Trim().Split(' ')[0].ToUpper() == "*ENDREC")
                    {
                        silentRecording = "";
                    }
                    if (bi < expandedLines.Count && bLine.Split(' ')[0].ToUpper() == "*QUESTION")
                    {
                        currentQuestion = new Question();
                        List<LogicalSyntax>  lsTemp     = new List<LogicalSyntax>();
                        List<Question>       qTemp      = new List<Question>();
                        Question             cqTemp     = new Question();
                        Dictionary<string, List<AttributeMain>> attrTemp   = new Dictionary<string, List<AttributeMain>>();
                        List<AttributeFilter>                   filterTemp = new List<AttributeFilter>();

                        bi = this.prepareQuestion(expandedLines, bi,
                            listOfQuestionIdForDupliCheck, listOfGridListForDupliCheck,
                            lsTemp, qTemp, cqTemp, attrTemp, filterTemp, txtWriter, dicLineLocal);

                        foreach (LogicalSyntax ls in lsTemp) listOfLogicalSyntax.Add(ls);
                        foreach (Question      q  in qTemp)  listOfQuestion.Add(q);
                        if (qTemp.Count > 0) currentQuestion = qTemp[0];
                        foreach (KeyValuePair<string, List<AttributeMain>> pair in attrTemp)
                            dicQidVsAttributeList[pair.Key] = pair.Value;
                        foreach (AttributeFilter f in filterTemp) listOfAttributeFilter.Add(f);
                    }
                }
            }
        }

        /// <summary>
        /// Expands a *REPEAT buffer for a language section (Lan1, Lan2).
        /// Substitutes ?R with each iteration value and calls the language-specific parsers.
        /// </summary>
        private void ExpandRepeatBlockLanguage(
            List<string> repeatBuffer,
            List<string> iterationList,
            int languageNo,
            TextWriter txtWriter)
        {
            foreach (string iterVal in iterationList)
            {
                List<string> expandedLines = new List<string>();
                foreach (string bl in repeatBuffer)
                    expandedLines.Add(bl.Replace("?R", iterVal));
                expandedLines.Add("*"); // sentinel — terminates prepareQuestionForLanguage's lookahead

                Dictionary<int, int> dicLineLocal = new Dictionary<int, int>();
                for (int x = 0; x < expandedLines.Count + 5; x++)
                    dicLineLocal[x + 1] = x + 1;

                for (int bi = 0; bi < expandedLines.Count; bi++)
                {
                    string bLine = expandedLines[bi];
                    if (string.IsNullOrWhiteSpace(bLine) || bLine[0] != '*') continue;

                    if (bLine.Split(' ')[0].ToUpper() == "*LIST")
                    {
                        bi = prepareListForLanguage(expandedLines, bi, txtWriter, dicLineLocal, 0, languageNo);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                    }
                    if (bi < expandedLines.Count && bLine.Split(' ')[0].ToUpper() == "*GRIDLIST")
                    {
                        bi = prepareGridListForLanguage(expandedLines, bi, txtWriter, dicLineLocal, 0, languageNo);
                        if (bi < expandedLines.Count) bLine = expandedLines[bi];
                    }
                    if (bi < expandedLines.Count && bLine.Split(' ')[0].ToUpper() == "*QUESTION")
                    {
                        bi = prepareQuestionForLanguage(expandedLines, bi, txtWriter, dicLineLocal, 0, languageNo);
                    }
                }
            }
        }

        // ── UI helpers ────────────────────────────────────────────────────────────

        private void ClearBuildOutput()
        {
            txtBuildResult.Document.Blocks.Clear();
        }

        private void AppendResult(string text, bool isError)
        {
            var para = new Paragraph(new Run(text))
            {
                Foreground = isError ? Brushes.Red : Brushes.DarkGreen,
                Margin = new Thickness(0)
            };
            txtBuildResult.Document.Blocks.Add(para);
            txtBuildResult.ScrollToEnd();
        }

        private void DisplayBuildResult(string resultFilePath)
        {
            ClearBuildOutput();
            if (!File.Exists(resultFilePath)) return;

            string[] resultLines = File.ReadAllLines(resultFilePath);
            bool hasErrors = resultLines.Any(l => !l.Contains("Build successful"));

            foreach (string line in resultLines)
            {
                bool lineIsError = line.Trim().Length > 0 && !line.Contains("Build successful");
                AppendResult(line, lineIsError);
            }

            txtStatus.Text = hasErrors ? "Build completed with errors." : "Build successful.";
            btnUpload.IsEnabled = preparedScript;
        }

        private void SetUIState(bool running)
        {
            btnExecute.IsEnabled = !running;
            btnBrowse.IsEnabled  = !running;
            btnUpload.IsEnabled  = !running && preparedScript;
            progressBar.Visibility = running ? Visibility.Visible : Visibility.Collapsed;
            txtStatus.Text = running ? "Building, please wait…" : txtStatus.Text;
        }

        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
        }
    }

    class TranslatedQtext
    {
        public string Qid { get; set; }
        public string QText { get; set; }
    }

    class TranslatedAttribtext
    {
        public string Qid { get; set; }
        public string AttribText { get; set; }
        public string AttribValue { get; set; }
    }
}
