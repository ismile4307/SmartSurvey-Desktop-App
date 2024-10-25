using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for FrmBuildSPSSScript.xaml
    /// </summary>
    public partial class FrmBuildSPSSScript : Window
    {
        private string myPath;

        private List<QuestionSPSS> listOfQuestion;
        private Dictionary<string, List<AttributeSPSS>> dicAttributeIdVsAttributeList;
        private Dictionary<string, List<GridSPSS>> dicGridIdVsAttributeList;

        private List<String> listOfAlterTypeSyntax;
        private List<String> listOfVariableLabelSyntax;
        private Dictionary<string, List<String>> dicQIdVsListOfValueLabel;
        private Dictionary<string, List<String>> dicQIdVsListOfQId;

        private List<String> listOfOEVariables;
        public FrmBuildSPSSScript()
        {
            InitializeComponent();
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

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (txtScriptPath.Text != "")
            {
                if (File.Exists(txtScriptPath.Text))
                {
                    listOfAlterTypeSyntax = new List<String>();
                    listOfVariableLabelSyntax = new List<String>();
                    dicQIdVsListOfValueLabel = new Dictionary<string, List<string>>();
                    dicQIdVsListOfQId = new Dictionary<string, List<string>>();
                    listOfOEVariables = new List<String>();

                    this.readQuestionDB();

                    #region SPSS Syntax

                    for (int i = 0; i < listOfQuestion.Count; i++)
                    {
                        string qId = listOfQuestion[i].QId;
                        string qText = listOfQuestion[i].QuestionEnglish;
                        string qType = listOfQuestion[i].QType;

                        string attributeQid;
                        if (listOfQuestion[i].AttributeId.Trim() == "")
                            attributeQid = listOfQuestion[i].QId;
                        else
                            attributeQid = listOfQuestion[i].AttributeId.Trim();


                        if (qType == "1" || qType == "4" || qType == "61")
                        {

                            listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + " (F8.0).");
                            listOfVariableLabelSyntax.Add(qId + @"  """ + qText + "\"");


                            if (qType == "1" || qType == "61")
                            {
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];
                                    List<string> listOfValueLabel = new List<string>();
                                    List<string> listOfQid = new List<string>();
                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        listOfValueLabel.Add(listOfAttribute[j].attributeValue + "  \"" + listOfAttribute[j].attributeEnglish + "\"");
                                    }

                                    dicQIdVsListOfValueLabel.Add(qId, listOfValueLabel);
                                    listOfQid.Add(qId);
                                    dicQIdVsListOfQId.Add(qId, listOfQid);
                                }

                            }
                        }
                        else if (qType == "3")
                        {
                            listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + " (A100).");
                            listOfVariableLabelSyntax.Add(qId + @"  """ + qText + "\"");
                        }
                        else if (qType == "14" || qType == "15")
                        {
                            listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + " (A100).");
                            listOfVariableLabelSyntax.Add(qId + @"  """ + qText + "\"");
                        }
                        else if (qType == "12")
                        {
                            if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            {
                                List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];

                                for (int j = 0; j < listOfAttribute.Count; j++)
                                {
                                    listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (A100).");
                                    listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");
                                }

                            }
                        }
                        else if (qType == "2" || qType == "5" || qType == "7" || qType == "13" || qType == "17" || qType == "19" || qType == "24" || qType == "26" || qType == "32")
                        {
                            if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            {
                                List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];

                                for (int j = 0; j < listOfAttribute.Count; j++)
                                {
                                    listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (F8.0).");
                                    listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");
                                }

                            }
                            if (qType == "2")
                            {
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];
                                    List<string> listOfValueLabel = new List<string>();
                                    List<string> listOfQid = new List<string>();
                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        listOfValueLabel.Add(listOfAttribute[j].attributeValue + "  \"" + listOfAttribute[j].attributeEnglish + "\"");
                                        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                                    }

                                    dicQIdVsListOfValueLabel.Add(qId, listOfValueLabel);

                                    dicQIdVsListOfQId.Add(qId, listOfQid);
                                }
                            }
                            else if (qType == "5")
                            {
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];
                                    List<string> listOfValueLabel = new List<string>();
                                    List<string> listOfQid = new List<string>();
                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        listOfValueLabel.Add((j + 1).ToString() + "  \"Rank-" + (j + 1).ToString() + "\"");
                                        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                                    }

                                    dicQIdVsListOfValueLabel.Add(qId, listOfValueLabel);

                                    dicQIdVsListOfQId.Add(qId, listOfQid);
                                }
                            }
                            else if (qType == "7" || qType == "24" || qType == "26" || qType == "32")
                            {
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];


                                    List<string> listOfValueLabel = new List<string>();
                                    List<string> listOfQid = new List<string>();
                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                                        dicQIdVsListOfQId.Add(qId + "_" + listOfAttribute[j].attributeOrder, listOfQid);

                                    }

                                    List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[0].LinkId2];
                                    for (int k = 0; k < listOfGridAttribute.Count; k++)
                                    {
                                        listOfValueLabel.Add(listOfGridAttribute[k].attributeValue + "  \"" + listOfGridAttribute[k].attributeEnglish + "\"");
                                    }

                                    dicQIdVsListOfValueLabel.Add(qId + "_" + listOfAttribute[0].attributeOrder, listOfValueLabel);

                                }
                            }
                        }
                        else if (qType == "8")
                        {
                            if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            {
                                List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];

                                for (int j = 0; j < listOfAttribute.Count; j++)
                                {
                                    List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[j].LinkId2];
                                    for (int k = 0; k < listOfGridAttribute.Count; k++)
                                    {
                                        listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + "_" + listOfGridAttribute[k].attributeOrder + " (F8.0).");
                                        listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + "_" + listOfGridAttribute[k].attributeOrder + @"  """ + qText + "- " + listOfAttribute[j].attributeEnglish + ": " + listOfGridAttribute[k].attributeEnglish + "\"");

                                    }

                                    //**********************************
                                    if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                    {
                                        //List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];


                                        List<string> listOfValueLabel = new List<string>();
                                        List<string> listOfQid = new List<string>();
                                        for (int k = 0; k < listOfAttribute.Count; k++)
                                        {
                                            for (int l = 0; l < listOfGridAttribute.Count; l++)
                                            {
                                                listOfQid.Add(qId + "_" + listOfAttribute[k].attributeOrder + "_" + listOfGridAttribute[l].attributeOrder);
                                            }

                                            if (!dicQIdVsListOfQId.ContainsKey(qId + "_" + listOfAttribute[k].attributeOrder))
                                                dicQIdVsListOfQId.Add(qId + "_" + listOfAttribute[k].attributeOrder, listOfQid);

                                        }

                                        //List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[0].LinkId2];
                                        for (int k = 0; k < listOfGridAttribute.Count; k++)
                                        {
                                            listOfValueLabel.Add(listOfGridAttribute[k].attributeValue + "  \"" + listOfGridAttribute[k].attributeEnglish + "\"");
                                        }

                                        if (!dicQIdVsListOfValueLabel.ContainsKey(qId + "_" + listOfAttribute[0].attributeOrder))
                                            dicQIdVsListOfValueLabel.Add(qId + "_" + listOfAttribute[0].attributeOrder, listOfValueLabel);
                                    }


                                }

                            }

                            ////**********************************
                            //if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            //{
                            //    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];


                            //    List<string> listOfValueLabel = new List<string>();
                            //    List<string> listOfQid = new List<string>();
                            //    for (int j = 0; j < listOfAttribute.Count; j++)
                            //    {
                            //        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                            //        dicQIdVsListOfQId.Add(qId + "_" + listOfAttribute[j].attributeOrder, listOfQid);
                            //    }

                            //    List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[0].LinkId2];
                            //    for (int k = 0; k < listOfGridAttribute.Count; k++)
                            //    {
                            //        listOfValueLabel.Add(listOfGridAttribute[k].attributeValue + "  \"" + listOfGridAttribute[k].attributeEnglish + "\"");
                            //    }

                            //    dicQIdVsListOfValueLabel.Add(qId + "_" + listOfAttribute[0].attributeOrder, listOfValueLabel);
                            //}

                        }
                        else if (qType == "48")
                        {

                            if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            {
                                List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];

                                for (int j = 0; j < listOfAttribute.Count; j++)
                                {
                                    if (listOfAttribute[j].LinkId1 == "3")
                                    {
                                        listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (A100).");
                                        listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");
                                    }
                                    else if (listOfAttribute[j].LinkId1 == "4")
                                    {
                                        listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (F8.0).");
                                        listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");
                                    }
                                    else if (listOfAttribute[j].LinkId1 == "1")
                                    {
                                        listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (F8.0).");
                                        listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");


                                        //Value labels

                                        List<string> listOfValueLabel = new List<string>();
                                        List<string> listOfQid = new List<string>();
                                        for (int k = 0; k < listOfAttribute.Count; k++)
                                        {
                                            listOfQid.Add(qId + "_" + listOfAttribute[k].attributeOrder);
                                            if (!dicQIdVsListOfQId.ContainsKey(qId + "_" + listOfAttribute[k].attributeOrder))
                                                dicQIdVsListOfQId.Add(qId + "_" + listOfAttribute[k].attributeOrder, listOfQid);

                                        }

                                        List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[j].LinkId2];
                                        for (int k = 0; k < listOfGridAttribute.Count; k++)
                                        {
                                            listOfValueLabel.Add(listOfGridAttribute[k].attributeValue + "  \"" + listOfGridAttribute[k].attributeEnglish + "\"");
                                        }

                                        dicQIdVsListOfValueLabel.Add(qId + "_" + listOfAttribute[j].attributeOrder, listOfValueLabel);


                                    }
                                    else if (listOfAttribute[j].LinkId1 == "2")
                                    {
                                        List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[j].LinkId2];
                                        for (int k = 0; k < listOfGridAttribute.Count; k++)
                                        {
                                            listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + "_" + listOfGridAttribute[k].attributeOrder + " (F8.0).");
                                            listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + "_" + listOfGridAttribute[k].attributeOrder + @"  """ + qText + "- " + listOfAttribute[j].attributeEnglish + ": " + listOfGridAttribute[k].attributeEnglish + "\"");

                                        }


                                        //**********************************
                                        if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                        {
                                            //List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];


                                            List<string> listOfValueLabel = new List<string>();
                                            List<string> listOfQid = new List<string>();
                                            for (int k = 0; k < listOfAttribute.Count; k++)
                                            {
                                                listOfQid.Add(qId + "_" + listOfAttribute[k].attributeOrder);
                                                if (!dicQIdVsListOfQId.ContainsKey(qId + "_" + listOfAttribute[k].attributeOrder))
                                                    dicQIdVsListOfQId.Add(qId + "_" + listOfAttribute[k].attributeOrder, listOfQid);

                                            }

                                            //List<GridSPSS> listOfGridAttribute = dicGridIdVsAttributeList[listOfAttribute[0].LinkId2];
                                            for (int k = 0; k < listOfGridAttribute.Count; k++)
                                            {
                                                listOfValueLabel.Add(listOfGridAttribute[k].attributeValue + "  \"" + listOfGridAttribute[k].attributeEnglish + "\"");
                                            }

                                            if (!dicQIdVsListOfValueLabel.ContainsKey(qId + "_" + listOfAttribute[0].attributeOrder))
                                                dicQIdVsListOfValueLabel.Add(qId + "_" + listOfAttribute[0].attributeOrder, listOfValueLabel);
                                        }
                                    }
                                }

                            }
                        }
                        else if (qType == "60")
                        {

                            if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                            {
                                List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];

                                for (int j = 0; j < listOfAttribute.Count; j++)
                                {
                                    listOfAlterTypeSyntax.Add("ALTER TYPE " + qId + "_" + listOfAttribute[j].attributeOrder + " (A100).");
                                    listOfVariableLabelSyntax.Add(qId + "_" + listOfAttribute[j].attributeOrder + @"  """ + qText + ": " + listOfAttribute[j].attributeEnglish + "\"");
                                }

                            }
                        }
                    }


                    TextWriter txtWriter = new StreamWriter(myPath + "\\00.Syntax_SPSS_DataPrep.sps");

                    txtWriter.WriteLine("GET DATA ");
                    txtWriter.WriteLine("  /TYPE=XLSX ");
                    txtWriter.WriteLine(@"  /FILE='' ");
                    txtWriter.WriteLine("  /SHEET=name 'Data' ");
                    txtWriter.WriteLine("  /CELLRANGE=FULL ");
                    txtWriter.WriteLine("  /READNAMES=ON ");
                    txtWriter.WriteLine("  /DATATYPEMIN PERCENTAGE=95.0 ");
                    txtWriter.WriteLine("  /HIDDEN IGNORE=YES. ");
                    txtWriter.WriteLine("EXECUTE. ");

                    txtWriter.WriteLine("");

                    txtWriter.WriteLine("ALTER TYPE id (F8.0).");
                    for (int j = 0; j < listOfAlterTypeSyntax.Count; j++)
                    {
                        txtWriter.WriteLine(listOfAlterTypeSyntax[j]);
                    }
                    txtWriter.WriteLine("");
                    txtWriter.WriteLine("");
                    txtWriter.WriteLine("VARIABLE LABELS");



                    txtWriter.WriteLine("Id             \"Id\"");
                    txtWriter.WriteLine("RespondentId   \"Respondent Id\"");
                    txtWriter.WriteLine("Latitude       \"Latitude\"");
                    txtWriter.WriteLine("Longitude      \"Longitude\"");
                    txtWriter.WriteLine("SurveyDateTime \"Survey Date Time\"");
                    txtWriter.WriteLine("SurveyEndTime  \"Survey End Time\"");
                    txtWriter.WriteLine("LengthOfIntv   \"Length Of Intv\"");


                    for (int j = 0; j < listOfVariableLabelSyntax.Count; j++)
                    {
                        String myString = listOfVariableLabelSyntax[j].Replace("<b>", " ").Replace("</b>", " ").Replace("<br>", " ").Replace("<big>", " ").Replace("</big>", " ");
                        txtWriter.WriteLine(Regex.Replace(myString, @"\s+", " "));

                    }

                    //txtWriter.WriteLine("Accom_1         \"Accom_1\"");
                    txtWriter.WriteLine("Intv_Type       \"Intv_Type\"");
                    txtWriter.WriteLine("FICode          \"FICode\"");
                    txtWriter.WriteLine("FSCode          \"FSCode\"");
                    txtWriter.WriteLine("AccompaniedBy   \"AccompaniedBy\"");
                    txtWriter.WriteLine("BackCheckedBy   \"BackCheckedBy\"");
                    txtWriter.WriteLine("ScriptVersion   \"ScriptVersion\"");
                    txtWriter.WriteLine("SyncDateTime    \"SyncDateTime\"");
                    txtWriter.WriteLine("Status          \"Status\"");
                    txtWriter.WriteLine("TabId           \"TabId\"");
                    txtWriter.WriteLine(".");
                    txtWriter.WriteLine("");
                    txtWriter.WriteLine("VALUE LABELS");
                    for (int j = 0; j < listOfQuestion.Count; j++)
                    {
                        string qid = listOfQuestion[j].QId;
                        string qType = listOfQuestion[j].QType;

                        if (qType == "7" || qType == "24" || qType == "26" || qType == "32")
                        {
                            List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[qid];
                            //for (int n = 0; n < listOfAttribute.Count; n++)
                            //{
                            string myQid = qid + "_" + listOfAttribute[0].attributeOrder;
                            if (dicQIdVsListOfQId.ContainsKey(myQid))
                            {
                                List<string> listOfQid = dicQIdVsListOfQId[myQid];
                                for (int k = 0; k < listOfQid.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfQid[k]);
                                }
                            }
                            //}
                            //for (int n = 0; n < listOfAttribute.Count; n++)
                            //{
                            myQid = qid + "_" + listOfAttribute[0].attributeOrder;
                            if (dicQIdVsListOfValueLabel.ContainsKey(myQid))
                            {
                                List<string> listOfValueLabel = dicQIdVsListOfValueLabel[myQid];
                                for (int k = 0; k < listOfValueLabel.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfValueLabel[k]);
                                }
                            }

                            //}

                            txtWriter.WriteLine("/");
                        }
                        else if (qType == "8")
                        {
                            List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[qid];
                            //for (int n = 0; n < listOfAttribute.Count; n++)
                            //{
                            string myQid = qid + "_" + listOfAttribute[0].attributeOrder;
                            if (dicQIdVsListOfQId.ContainsKey(myQid))
                            {
                                List<string> listOfQid = dicQIdVsListOfQId[myQid];
                                for (int k = 0; k < listOfQid.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfQid[k]);
                                }
                            }
                            //}
                            //for (int n = 0; n < listOfAttribute.Count; n++)
                            //{
                            myQid = qid + "_" + listOfAttribute[0].attributeOrder;
                            if (dicQIdVsListOfValueLabel.ContainsKey(myQid))
                            {
                                List<string> listOfValueLabel = dicQIdVsListOfValueLabel[myQid];
                                for (int k = 0; k < listOfValueLabel.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfValueLabel[k]);
                                }
                            }

                            //}

                            txtWriter.WriteLine("/");
                        }
                        else
                        {
                            if (dicQIdVsListOfQId.ContainsKey(qid) && dicQIdVsListOfValueLabel.ContainsKey(qid))
                            {
                                List<string> listOfQid = dicQIdVsListOfQId[qid];
                                for (int k = 0; k < listOfQid.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfQid[k]);
                                }

                                List<string> listOfValueLabel = dicQIdVsListOfValueLabel[qid];
                                for (int k = 0; k < listOfValueLabel.Count; k++)
                                {
                                    txtWriter.WriteLine(listOfValueLabel[k]);
                                }

                                txtWriter.WriteLine("/");
                            }
                        }

                    }
                    txtWriter.WriteLine(".");

                    txtWriter.WriteLine("");
                    txtWriter.WriteLine(@"SAVE OUTFILE=''");
                    txtWriter.WriteLine("/COMPRESSED.");


                    if (chkIncludeOE.IsChecked == true)
                    {
                        txtWriter.WriteLine("");
                        txtWriter.WriteLine("");
                        txtWriter.WriteLine("");


                        for (int n = 0; n < listOfOEVariables.Count; n++)
                        {
                            txtWriter.WriteLine("STRING " + listOfOEVariables[n] + " (A100).");
                        }
                        txtWriter.WriteLine("");
                        for (int n = 0; n < listOfOEVariables.Count; n++)
                        {
                            txtWriter.WriteLine("" + listOfOEVariables[n] + "   \"" + listOfOEVariables[n] + "\"");
                        }
                    }


                    txtWriter.Close();

                    #endregion


                    #region SPSS Frequency Syntax

                    List<string> listOfSRQuestionId = new List<string>();
                    TextWriter txtWriter2 = new StreamWriter(myPath + "\\01.Syntax_Freq_SPSS.sps");
                    for (int i = 0; i < listOfQuestion.Count; i++)
                    {
                        string qId = listOfQuestion[i].QId;
                        string qText = listOfQuestion[i].QuestionEnglish;
                        string qType = listOfQuestion[i].QType;

                        string attributeQid;
                        if (listOfQuestion[i].AttributeId.Trim() == "")
                            attributeQid = listOfQuestion[i].QId;
                        else
                            attributeQid = listOfQuestion[i].AttributeId.Trim();



                        if (qType == "1" || qType == "3" || qType == "4" || qType == "14" || qType == "15" || qType == "61")
                        {
                            listOfSRQuestionId.Add(qId);
                        }
                        else if (qType == "2" || qType == "5" || qType == "7" || qType == "12" || qType == "13" || qType == "17" || qType == "18" || qType == "19" || qType == "22" || qType == "23" || qType == "24" || qType == "26" || qType == "32" || qType == "60")
                        {
                            if (listOfSRQuestionId.Count > 0)
                            {
                                string myText = "";
                                for (int x = 0; x < listOfSRQuestionId.Count; x++)
                                {
                                    myText = myText + " " + listOfSRQuestionId[x];
                                }

                                txtWriter2.WriteLine("FREQUENCIES" + myText + ".");
                                txtWriter2.WriteLine("");
                            }

                            listOfSRQuestionId.Clear();

                            if (qType == "2")
                            {
                                List<string> listOfQid = new List<string>();
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];
                                    //List<string> listOfValueLabel = new List<string>();

                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        //listOfValueLabel.Add(listOfAttribute[j].attributeValue + "  \"" + listOfAttribute[j].attributeEnglish + "\"");
                                        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                                    }

                                    //dicQIdVsListOfValueLabel.Add(qId, listOfValueLabel);

                                    //dicQIdVsListOfQId.Add(qId, listOfQid);
                                }
                                int x = listOfQid.Count - 1;
                                txtWriter2.WriteLine("MULT RESPONSE GROUPS=$" + qId + " \"" + qText.Replace("<b>", " ").Replace("</b>", " ").Replace("<br>", " ").Replace("<big>", " ").Replace("</big>", " ") + "\" (" + listOfQid[0] + " to " + listOfQid[x] + " (1,100))");
                                txtWriter2.WriteLine("/FREQUENCIES $" + qId + ".");
                                txtWriter2.WriteLine("");
                                
                            }
                            else
                            {
                                List<string> listOfQid = new List<string>();
                                if (dicAttributeIdVsAttributeList.ContainsKey(attributeQid))
                                {
                                    List<AttributeSPSS> listOfAttribute = dicAttributeIdVsAttributeList[attributeQid];
                                    //List<string> listOfValueLabel = new List<string>();

                                    for (int j = 0; j < listOfAttribute.Count; j++)
                                    {
                                        //listOfValueLabel.Add(listOfAttribute[j].attributeValue + "  \"" + listOfAttribute[j].attributeEnglish + "\"");
                                        listOfQid.Add(qId + "_" + listOfAttribute[j].attributeOrder);
                                    }

                                    //dicQIdVsListOfValueLabel.Add(qId, listOfValueLabel);

                                    //dicQIdVsListOfQId.Add(qId, listOfQid);

                                    string myText = "";
                                    for (int x = 0; x < listOfQid.Count; x++)
                                    {
                                        myText = myText + " " + listOfQid[x];
                                    }

                                    txtWriter2.WriteLine("FREQUENCIES" + myText + ".");
                                }

                            }

                        }
                        else if (qType == "8")
                        {
                            if (listOfSRQuestionId.Count > 0)
                            {

                            }

                            listOfSRQuestionId.Clear();
                        }
                        //else if (qType == "20" ||  qType == "40" || qType == "48")
                        //{
                        //}


                    }

                    // This is the last one for SR Frequency Syntax
                    if (listOfSRQuestionId.Count > 0)
                    {
                        string myText = "";
                        for (int x = 0; x < listOfSRQuestionId.Count; x++)
                        {
                            myText = myText + " " + listOfSRQuestionId[x];
                        }

                        txtWriter2.WriteLine("FREQUENCIES" + myText + ".");
                        txtWriter2.WriteLine("");
                    }

                    listOfSRQuestionId.Clear();


                    txtWriter2.Close();
                    #endregion

                    MessageBox.Show("SPSS syntax prepared successfully");



                }
            }
        }

        private void readQuestionDB()
        {
            listOfQuestion = new List<QuestionSPSS>();
            dicAttributeIdVsAttributeList = new Dictionary<string, List<AttributeSPSS>>();
            dicGridIdVsAttributeList = new Dictionary<string, List<GridSPSS>>();

            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {

                #region ReadQuestion
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt1 = new SQLiteDataAdapter("SELECT * FROM T_Question WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds1 = new DataSet();
                dadpt1.Fill(ds1, "Table1");
                if (ds1.Tables["Table1"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds1.Tables["Table1"].Rows)
                    {
                        QuestionSPSS questionSPSS = new QuestionSPSS();

                        questionSPSS.QId = dr["QId"].ToString();
                        questionSPSS.QuestionEnglish = dr["QuestionEnglish"].ToString();
                        questionSPSS.AttributeId = dr["AttributeId"].ToString();
                        questionSPSS.QType = dr["QType"].ToString();

                        listOfQuestion.Add(questionSPSS);
                    }
                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                #endregion

                #region ReadAttribute

                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt2 = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds2 = new DataSet();
                dadpt2.Fill(ds2, "Table1");
                if (ds2.Tables["Table1"].Rows.Count > 0)
                {
                    List<AttributeSPSS> listOfAttribute = new List<AttributeSPSS>();
                    string priorQid = "";
                    foreach (DataRow dr in ds2.Tables["Table1"].Rows)
                    {
                        AttributeSPSS attributeSPSS = new AttributeSPSS();

                        attributeSPSS.qId = dr["QId"].ToString();
                        attributeSPSS.attributeEnglish = dr["AttributeEnglish"].ToString();
                        attributeSPSS.attributeValue = dr["AttributeValue"].ToString();
                        attributeSPSS.attributeOrder = dr["AttributeOrder"].ToString();
                        attributeSPSS.LinkId1 = dr["LinkId1"].ToString();
                        attributeSPSS.LinkId2 = dr["LinkId2"].ToString();

                        if (attributeSPSS.qId != priorQid && priorQid != "")
                        {
                            dicAttributeIdVsAttributeList.Add(priorQid, listOfAttribute);
                            listOfAttribute = new List<AttributeSPSS>();
                            listOfAttribute.Add(attributeSPSS);
                            priorQid = attributeSPSS.qId;
                        }
                        else
                        {
                            listOfAttribute.Add(attributeSPSS);
                            priorQid = attributeSPSS.qId;
                        }

                        // This is for OE variables only
                        if (dr["TakeOpenended"].ToString() == "1")
                            listOfOEVariables.Add(dr["QId"].ToString() + "_" + dr["AttributeValue"].ToString() + "_OE");


                    }
                    // add the last attribute
                    if (!dicAttributeIdVsAttributeList.ContainsKey(priorQid))
                        dicAttributeIdVsAttributeList.Add(priorQid, listOfAttribute);
                    else
                    {
                        List<AttributeSPSS> listOfAttributeTmp = dicAttributeIdVsAttributeList[priorQid];

                        for (int x = 0; x < listOfAttribute.Count; x++)
                        {
                            listOfAttributeTmp.Add(listOfAttribute[x]);
                        }

                        dicAttributeIdVsAttributeList.Remove(priorQid);

                        dicAttributeIdVsAttributeList.Add(priorQid, listOfAttributeTmp);
                    }

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                #endregion

                #region Read Grid Attribute

                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt3 = new SQLiteDataAdapter("SELECT * FROM T_GridInfo WHERE QId!=''", connDB.sqlite_conn);
                DataSet ds3 = new DataSet();
                dadpt3.Fill(ds3, "Table1");
                if (ds3.Tables["Table1"].Rows.Count > 0)
                {
                    List<GridSPSS> listOfGridAttribute = new List<GridSPSS>();
                    string priorQid = "";
                    foreach (DataRow dr in ds3.Tables["Table1"].Rows)
                    {
                        GridSPSS gridAttributeSPSS = new GridSPSS();

                        gridAttributeSPSS.qId = dr["QId"].ToString();
                        gridAttributeSPSS.attributeEnglish = dr["AttributeEnglish"].ToString();
                        gridAttributeSPSS.attributeValue = dr["AttributeValue"].ToString();
                        gridAttributeSPSS.attributeOrder = dr["AttributeOrder"].ToString();

                        if (gridAttributeSPSS.qId != priorQid && priorQid != "")
                        {
                            dicGridIdVsAttributeList.Add(priorQid, listOfGridAttribute);
                            listOfGridAttribute = new List<GridSPSS>();
                            listOfGridAttribute.Add(gridAttributeSPSS);
                            priorQid = gridAttributeSPSS.qId;
                        }
                        else
                        {
                            listOfGridAttribute.Add(gridAttributeSPSS);
                            priorQid = gridAttributeSPSS.qId;
                        }
                    }
                    // add the last attribute
                    dicGridIdVsAttributeList.Add(priorQid, listOfGridAttribute);

                }

                if (connDB.sqlite_conn.State == ConnectionState.Open)
                    connDB.sqlite_conn.Close();

                #endregion


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
                            }
                        }

                        if (connDB.sqlite_conn.State == ConnectionState.Open)
                            connDB.sqlite_conn.Close();

                    }
                }
                else
                    MessageBox.Show("Invalid script file location");
            }
            else
                MessageBox.Show("Script location should not be blank");
        }
    }

    public class QuestionSPSS
    {
        public string QId { get; set; }
        public string QuestionEnglish { get; set; }
        public string AttributeId { get; set; }
        public string QType { get; set; }
    }
    public class AttributeSPSS
    {
        public string qId { get; set; }
        public string attributeEnglish { get; set; }
        public string attributeValue { get; set; }
        public string attributeOrder { get; set; }
        public string LinkId1 { get; set; }
        public string LinkId2 { get; set; }

    }
    public class GridSPSS
    {
        public string qId { get; set; }
        public string attributeEnglish { get; set; }
        public string attributeValue { get; set; }
        public string attributeOrder { get; set; }
    }
}
