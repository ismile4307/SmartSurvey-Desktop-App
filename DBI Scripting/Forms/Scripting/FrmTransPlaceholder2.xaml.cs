using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for FrmTransPlaceholder2.xaml
    /// </summary>
    public partial class FrmTransPlaceholder2 : Window
    {
        private string myPath, ProjectName;
        public FrmTransPlaceholder2()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            TextReader txtReader = new StreamReader(txtScriptPath.Text);
            String strline = txtReader.ReadLine();
            Dictionary<int, int> dicLine = new Dictionary<int, int>();


            List<String> lines = new List<string>();
            List<String> linesLanguage1 = new List<string>();

            List<String> linesForExcel = new List<string>();

            bool hasDKCS = false;
            bool hasFIFS = false;

            int a = 0;
            int b = 0;
            while (strline != null)
            {
                if (strline.ToUpper().Contains("@LANGUAGE"))
                    break;
                else
                {
                    b++;
                    if (strline.Trim() != "" && strline.Substring(0, 1) != "#")
                    {
                        a++;
                        lines.Add(Regex.Replace(strline.Trim(), @"\s+", " "));
                        dicLine.Add(a, b);
                    }
                }
                strline = txtReader.ReadLine();

            }

            txtReader.Close();

            int j = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                strline = lines[i];

                #region Script Info

                if (strline.ToUpper().Contains("PROJECT NAME") && strline.Contains(":"))
                {
                    ProjectName = strline.Split(':')[1].Trim();
                    goto next;
                }
                //else if (strline.ToUpper().Contains("PROJECT CODE") && strline.Contains(":"))
                //{
                //    projectInfoScript.ProjectCode = strline.Split(':')[1].Trim();
                //    goto next;
                //}
                //else if (strline.ToUpper().Contains("SCRIPT VERSION") && strline.Contains(":"))
                //{
                //    projectInfoScript.ScriptVersion = strline.Split(':')[1].Trim();
                //    goto next;
                //}
                //else if (strline.ToUpper().Contains("SCRIPT NAME") && strline.Contains(":"))
                //{
                //    projectInfoScript.DatabaseName = strline.Split(':')[1].Trim();
                //    goto next;
                //}
                //else if (strline.ToUpper().Contains("SCRIPTED BY") && strline.Contains(":"))
                //{
                //    projectInfoScript.ScriptedBy = strline.Split(':')[1].Trim();
                //    goto next;
                //}

                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 3 Project Name Missing");
                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 4 Project Code Missing");
                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 5 Script Version Missing");
                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 6 Script Name Missing");
                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 7 Project Name Missing");
                //if (projectInfoScript.ProjectName == null)
                //    txtWriter.WriteLine("Line : 8 Scripted by name Missing");

                #endregion

                if (strline.Substring(0, 1) == "*")
                {
                    #region Prepare LIST
                    if (strline.Split(' ')[0].ToUpper() == "*LIST")
                    {

                        linesForExcel.Add(strline);

                        strline = lines[++i];
                        if (isAttribute(strline))
                        {
                            while (!strline.Trim().Substring(0, 1).Contains("*"))
                            {
                                //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                //{
                                if (isAttribute(strline))
                                    linesForExcel.Add(strline);

                                //}

                                strline = lines[++i];
                            }

                            linesForExcel.Add("");
                            
                            if (strline.Split(' ')[0].ToUpper() == "*LIST" && i < lines.Count - 1)
                            {
                                i--;
                                
                            }
                        }
                    }
                    #endregion


                    #region Prepare GRIDLIST
                    if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST")
                    {
                        linesForExcel.Add(strline);

                        strline = lines[++i];
                        if (isAttribute(strline))
                        {
                            while (!strline.Trim().Substring(0, 1).Contains("*"))
                            {
                                //if (strline.Contains(":") && strline.Split(':').Length == 2)
                                //{
                                if (isAttribute(strline))
                                    linesForExcel.Add(strline);

                                //}
                                strline = lines[++i];
                            }

                            linesForExcel.Add("");
                            
                            if (strline.Split(' ')[0].ToUpper() == "*GRIDLIST" && i < lines.Count - 1)
                            {
                                i--;
                                
                            }
                        }
                    }
                    #endregion


                    #region Prepare IF
                    if (strline.Trim().Split(' ')[0].ToUpper() == "*IF" && !strline.ToUpper().Contains("REGULAREXPOF"))
                    {
                        //    LogicalSyntax myLogicalSyntax;// = new LogicalSyntax();

                        //    string[] pqr = strline.Trim().Split('*');
                        //    if (pqr.Length == 3)
                        //    {

                        //        string[] mno = pqr[1].Split(' ');
                        //        //string ifCondition = pqr[1].Split(new Char[] { '[', ']' })[1];

                        //        string ifCondition = pqr[1].Substring(pqr[1].IndexOf('[') + 1);//pqr[1].Split(new Char[] { '[', ']' })[1];
                        //        ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']'));



                        //        //*IF [Q5=1] *GOTO Q2
                        //        if (pqr[2].Trim().Contains("GOTO"))
                        //        {
                        //            myLogicalSyntax = new LogicalSyntax();
                        //            string[] xyz = pqr[2].Split(' ');
                        //            if (xyz.Length != 2)
                        //            {
                        //                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        //            }
                        //            else
                        //            {
                        //                //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                        //                //{
                        //                if (Regex.Match(xyz[1].Trim(), "^[a-zA-Z]").Success)
                        //                {
                        //                    myLogicalSyntax.ThenValue = xyz[1].Trim();
                        //                    myLogicalSyntax.QId = currentQuestion.QId;
                        //                    myLogicalSyntax.LogicTypeId = "3";
                        //                    myLogicalSyntax.IfCondition = ifCondition;

                        //                    //Add in list
                        //                    listOfLogicalSyntax.Add(myLogicalSyntax);
                        //                }
                        //                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                        //                //}
                        //                //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        //            }
                        //        }
                        //        //*IF [Q5=1] *MSG "Mobile Number should be correct"
                        //        else if (pqr[2].Trim().Contains("MSG"))
                        //        {
                        //            myLogicalSyntax = new LogicalSyntax();
                        //            string[] xyz = pqr[2].Split('"');
                        //            if (xyz.Length != 3)
                        //            {
                        //                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        //            }
                        //            else
                        //            {
                        //                //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                        //                //{
                        //                //if (Regex.Match(xyz[1].Trim(), "\"[^\"]*\"").Success)
                        //                //{
                        //                myLogicalSyntax.ThenValue = xyz[1];
                        //                myLogicalSyntax.QId = currentQuestion.QId;
                        //                myLogicalSyntax.LogicTypeId = "2";
                        //                myLogicalSyntax.IfCondition = ifCondition;

                        //                //Add in list
                        //                listOfLogicalSyntax.Add(myLogicalSyntax);
                        //                //}
                        //                //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                        //                //}
                        //                //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        //            }
                        //        }
                        //        //*IF [Q5=1] *INCLUDE Q3Dummy [01;03;04;05]
                        //        else if (pqr[2].Trim().Contains("INCLUDE") || pqr[2].Trim().Contains("EXCLUDE"))
                        //        {
                        //            AutoResponse myAutoResponse = new AutoResponse();

                        //            myAutoResponse.IfCondition = ifCondition;

                        //            String IncludeExclude = "";
                        //            if (pqr[2].Trim().Split(' ')[0].Trim().ToUpper() == "INCLUDE")
                        //                IncludeExclude = "Include";
                        //            else
                        //                IncludeExclude = "Exclude";


                        //            string[] abc = pqr[2].Trim().Split(' ');
                        //            if (abc.Length != 3 && abc.Length != 5)
                        //            {
                        //                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
                        //            }
                        //            else if (abc.Length == 3)
                        //            {
                        //                if (!abc[2].Trim().Contains("["))
                        //                {
                        //                    //*INCLUDE Q3Dummy Q1
                        //                    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //                    {
                        //                        if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //                            myAutoResponse.QId = abc[1].Trim();
                        //                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //                    }
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                        //                    myAutoResponse.LogicId = "1";

                        //                    if (Regex.Match(abc[2].Trim(), "^[a-zA-Z]").Success)
                        //                    {
                        //                        if (listOfQuestionIdForDupliCheck.Contains(abc[2].Trim()))
                        //                            myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                        //                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim());
                        //                    }
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim() + ", Must be started with Alpha");
                        //                }
                        //                else
                        //                {
                        //                    // get the QID
                        //                    //*INCLUDE Q3Dummy [01;03;04;05]
                        //                    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //                    {
                        //                        if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //                            myAutoResponse.QId = abc[1].Trim();
                        //                        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //                    }
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                        //                    myAutoResponse.LogicId = "1";

                        //                    // get the condition
                        //                    //*INCLUDE Q3Dummy [01;03;04;05]
                        //                    if (Regex.Match(abc[2].Trim(), @"^\[\d+(;\d+)*\]$").Success)
                        //                    {
                        //                        myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                        //                    }

                        //                    else if (Regex.Match(abc[2].Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                        //                    {
                        //                        myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                        //                    }
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[2].Trim() + ", Should be [Number to Number]");

                        //                }
                        //            }
                        //            else if (abc.Length == 5)
                        //            {
                        //                // get the QID
                        //                //*INCLUDE Q3Dummy [01 to 05]
                        //                if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //                {
                        //                    if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //                        myAutoResponse.QId = abc[1].Trim();
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //                }
                        //                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                        //                myAutoResponse.LogicId = "1";

                        //                if (abc[2].Trim().Contains("["))
                        //                {   //*INCLUDE Q3Dummy [01 to 05]
                        //                    string myExp = abc[2] + " " + abc[3] + " " + abc[4];
                        //                    if (Regex.Match(myExp.Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                        //                    {
                        //                        myAutoResponse.ThenValue = IncludeExclude + myExp.Trim();
                        //                    }
                        //                    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + myExp + ", Should be [Number to Number]");

                        //                }
                        //                else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline + ", Should be [Number to Number]");

                        //            }
                        //            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[1].Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");


                        //            listOfAutoResponse.Add(myAutoResponse);

                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[1].Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");

                        //    }
                        //    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid IF Statement " + strline + ", Format is *IF [Condition] SuccessExpression");


                        //}
                        //else if (strline.Trim().Split(' ')[0].ToUpper() == "*IF" && strline.ToUpper().Contains("REGULAREXPOF"))
                        //{
                        //    //*IF [RegexOf[Q17Mojo.1]!=RegularExpOf[^[1-9]\d*$]] *MSG "Invalid buying price, Must be decimal Number"

                        //    LogicalSyntax myLogicalSyntax;// = new LogicalSyntax();

                        //    string[] pqrTemp = strline.Trim().Split('*');
                        //    string[] pqr = new string[3];
                        //    if (pqrTemp.Length == 3)
                        //    {
                        //        pqr = pqrTemp;
                        //    }
                        //    else if (pqrTemp.Length == 4)
                        //    {
                        //        pqr[0] = pqrTemp[0];
                        //        pqr[1] = pqrTemp[1] + "*" + pqrTemp[2];
                        //        pqr[2] = pqrTemp[3];

                        //    }
                        //    if (pqr.Length == 3)
                        //    {
                        //        string[] mno = pqr[1].Split(' ');
                        //        //string ifCondition = pqr[1].Split(new Char[] { '[', ']' })[1];

                        //        string ifCondition = pqr[1].Substring(pqr[1].IndexOf('[') + 1);//pqr[1].Split(new Char[] { '[', ']' })[1];
                        //        ifCondition = ifCondition.Substring(0, ifCondition.LastIndexOf(']'));

                        //        if (pqr[2].Trim().Contains("MSG"))
                        //        {
                        //            myLogicalSyntax = new LogicalSyntax();
                        //            string[] xyz = pqr[2].Split('"');
                        //            if (xyz.Length != 3)
                        //            {
                        //                txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + pqr[2]);
                        //            }
                        //            else
                        //            {
                        //                //if (listOfQuestionIdForDupliCheck.Contains(xyz[1]))
                        //                //{
                        //                //if (Regex.Match(xyz[1].Trim(), "\"[^\"]*\"").Success)
                        //                //{
                        //                myLogicalSyntax.ThenValue = xyz[1];
                        //                myLogicalSyntax.QId = currentQuestion.QId;
                        //                myLogicalSyntax.LogicTypeId = "2";
                        //                myLogicalSyntax.IfCondition = ifCondition;

                        //                //Add in list
                        //                listOfLogicalSyntax.Add(myLogicalSyntax);
                        //                //}
                        //                //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1].Trim() + " Question Id must be followed by a Alpha Char");

                        //                //}
                        //                //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Question Id " + xyz[1]);

                        //            }
                        //        }
                        //    }
                    }
                    #endregion


                    #region Prepare INCLUDE && EXCLUDE
                    if (strline.Trim().Split(' ')[0].Trim().ToUpper() == "*INCLUDE" || strline.Trim().Split(' ')[0].Trim().ToUpper() == "*EXCLUDE")
                    {
                        //AutoResponse myAutoResponse = new AutoResponse();

                        //String IncludeExclude = "";
                        //if (strline.Trim().Split(' ')[0].Trim().ToUpper() == "*INCLUDE")
                        //    IncludeExclude = "Include";
                        //else
                        //    IncludeExclude = "Exclude";


                        //string[] abc = strline.Trim().Split(' ');
                        //if (abc.Length != 3 && abc.Length != 5)
                        //{
                        //    txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline);
                        //}
                        //else if (abc.Length == 3)
                        //{
                        //    if (!abc[2].Trim().Contains("["))
                        //    {
                        //        //*INCLUDE Q3Dummy Q1
                        //        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //        {
                        //            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //                myAutoResponse.QId = abc[1].Trim();
                        //            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                        //        myAutoResponse.LogicId = "1";

                        //        if (Regex.Match(abc[2].Trim(), "^[a-zA-Z]").Success)
                        //        {
                        //            if (listOfQuestionIdForDupliCheck.Contains(abc[2].Trim()))
                        //                myAutoResponse.ThenValue = IncludeExclude + "[" + abc[2].Trim() + "]";
                        //            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim());
                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[2].Trim() + ", Must be started with Alpha");
                        //    }
                        //    else
                        //    {
                        //        // get the QID
                        //        //*INCLUDE Q3Dummy [01;03;04;05]
                        //        if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //        {
                        //            if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //                myAutoResponse.QId = abc[1].Trim();
                        //            else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[1].Trim() + ", Should be [Number;Number;Number]");

                        //        myAutoResponse.LogicId = "1";

                        //        // get the condition
                        //        //*INCLUDE Q3Dummy [01;03;04;05]
                        //        if (Regex.Match(abc[2].Trim(), @"^\[\d+(;\d+)*\]$").Success)
                        //        {
                        //            myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                        //        }

                        //        else if (Regex.Match(abc[2].Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                        //        {
                        //            myAutoResponse.ThenValue = IncludeExclude + abc[2].Trim();
                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + abc[2].Trim() + ", Should be [Number to Number]");

                        //    }
                        //}
                        //else if (abc.Length == 5)
                        //{
                        //    //*INCLUDE Q3Dummy Q1
                        //    if (Regex.Match(abc[1].Trim(), "^[a-zA-Z]").Success)
                        //    {
                        //        if (listOfQuestionIdForDupliCheck.Contains(abc[1].Trim()))
                        //            myAutoResponse.QId = abc[1].Trim();
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim());
                        //    }
                        //    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid QId " + abc[1].Trim() + ", Must be started with Alpha");

                        //    myAutoResponse.LogicId = "1";

                        //    if (abc[2].Trim().Contains("["))
                        //    {   //*INCLUDE Q3Dummy [01 to 05]
                        //        string myExp = abc[2] + " " + abc[3] + " " + abc[4];
                        //        if (Regex.Match(myExp.Trim().ToUpper(), @"^\[\d+(\sTO\s\d+)*\]$").Success)
                        //        {
                        //            myAutoResponse.ThenValue = IncludeExclude + myExp.Trim();
                        //        }
                        //        else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + myExp + ", Should be [Number to Number]");

                        //    }
                        //    else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline + ", Should be [Number to Number]");

                        //}
                        //else txtWriter.WriteLine("Line : " + dicLine[i + 1] + " Invlaid Syntax " + strline.Trim() + ", *GOTO/*INCLUDE/*EXCLUDE");

                        //listOfAutoResponse.Add(myAutoResponse);
                    }
                    #endregion


                    #region Prepare QUESTION
                    if (strline.Split(' ')[0].ToUpper() == "*QUESTION" && !strline.Contains("*DUMMY2") && !strline.Contains("*DUMMY1"))
                    {

                        string[] word = strline.Split('*');
                        int QTypeCounter = 0;
                        List<string> listOfQuestionProperties = new List<string>();
                        String currentQuestion = "";

                        string FIFSAttribute1 = "";
                        string FIFSAttribute2 = "";
                        string FIFSAttribute3 = "";
                        string FIFSAttribute4 = "";

                        #region Question Properties
                        for (int n = 1; n < word.Length; n++)
                        {
                            if (word[n].ToUpper().Trim().Contains("QUESTION"))
                            {
                                //QID
                                linesForExcel.Add("*" + word[n].Trim());

                                currentQuestion = word[n];
                            }
                            else if (word[n].ToUpper().Trim().Contains("FIFS"))
                            {
                                FIFSAttribute1 = "1:FI Name";
                                FIFSAttribute2 = "2:FI Code";
                                FIFSAttribute3 = "3:FS Name";
                                FIFSAttribute4 = "4:FS Code";

                                hasFIFS = true;
                            }
                            else if (word[n].ToUpper().Trim().Contains("DKCS"))
                            {
                                currentQuestion = currentQuestion + "*" + word[n].Trim();
                                hasDKCS = true;
                            }

                            //************************************* End of Attribute Filter ****************************************

                        }

                        if (hasDKCS == true)
                        {
                            linesForExcel.Remove(linesForExcel[linesForExcel.Count - 1]);
                            linesForExcel.Add("*" + currentQuestion);
                            hasDKCS = false;
                        }

                        
                        #endregion


                        strline = lines[++i];
                        bool getquestionText = false;
                        while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                        {
                            linesForExcel.Add(strline);
                            strline = lines[++i];
                            getquestionText = true;
                        }

                        if (hasFIFS == true)
                        {
                            linesForExcel.Add(FIFSAttribute1);
                            linesForExcel.Add(FIFSAttribute2);
                            linesForExcel.Add(FIFSAttribute3);
                            linesForExcel.Add(FIFSAttribute4);


                            hasFIFS = false;
                        }

                        //this portion is for question attribute

                        if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                        {
                            if (i < lines.Count - 1)
                                i--;
                        }



                        #region USELIST
                        if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                        {
                            string[] word1 = strline.Split(' ');
                            if (word1.Length == 2)
                            {
                                if (word1[1].Split('"').Length == 3)
                                {
                                    linesForExcel.Add("*" + strline);
                                }

                            }

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
                                    if (!strline.Contains("*"))
                                    {
                                        linesForExcel.Add(strline);
                                    }
                                    else
                                    {
                                        string[] myKey = strline.Split('*');

                                        linesForExcel.Add(myKey[0].Trim());


                                    }

                                }

                                if (i < lines.Count - 1)
                                {
                                    strline = lines[++i];
                                }
                            }

                            if (i < lines.Count - 1)
                            {
                                i--;
                                linesForExcel.Add("");
                            }
                        }
                        else
                        {
                            linesForExcel.Add("");
                        }
                        #endregion

                    }
                    else if (strline.Split(' ')[0].ToUpper() == "*QUESTION" && strline.Contains("*DUMMY2"))
                    {
                        string[] word = strline.Split('*');
                        int QTypeCounter = 0;
                        List<string> listOfQuestionProperties = new List<string>();
                        String currentQuestion = "";

                        string FIFSAttribute1 = "";
                        string FIFSAttribute2 = "";
                        string FIFSAttribute3 = "";
                        string FIFSAttribute4 = "";

                        #region Question Properties
                        for (int n = 1; n < word.Length; n++)
                        {
                            if (word[n].ToUpper().Trim().Contains("QUESTION"))
                            {
                                //QID
                                //linesForExcel.Add("*" + word[n].Trim());

                                //currentQuestion = word[n];
                            }
                            else if (word[n].ToUpper().Trim().Contains("FIFS"))
                            {
                                FIFSAttribute1 = "1:FI Name";
                                FIFSAttribute2 = "2:FI Code";
                                FIFSAttribute3 = "3:FS Name";
                                FIFSAttribute4 = "4:FS Code";

                                hasFIFS = true;
                            }
                            else if (word[n].ToUpper().Trim().Contains("DKCS"))
                            {
                                currentQuestion = currentQuestion + "*" + word[n].Trim();
                                hasDKCS = true;
                            }

                            //************************************* End of Attribute Filter ****************************************

                        }

                        if (hasDKCS == true)
                        {
                            linesForExcel.Remove(linesForExcel[linesForExcel.Count - 1]);
                            linesForExcel.Add("*" + currentQuestion);
                            hasDKCS = false;
                        }


                        #endregion


                        strline = lines[++i];
                        bool getquestionText = false;
                        while (!isAttribute(strline) && !strline.Substring(0, 1).Contains("*"))
                        {
                            //linesForExcel.Add(strline);
                            strline = lines[++i];
                            getquestionText = true;
                        }

                        if (hasFIFS == true)
                        {
                            linesForExcel.Add(FIFSAttribute1);
                            linesForExcel.Add(FIFSAttribute2);
                            linesForExcel.Add(FIFSAttribute3);
                            linesForExcel.Add(FIFSAttribute4);


                            hasFIFS = false;
                        }

                        //this portion is for question attribute

                        if (getquestionText == true && strline.Substring(0, 1).Contains("*"))
                        {
                            if (i < lines.Count - 1)
                                i--;
                        }



                        #region USELIST
                        if (strline.Split(' ')[0].ToUpper().Contains("*USELIST"))
                        {
                            //string[] word1 = strline.Split(' ');
                            //if (word1.Length == 2)
                            //{
                            //    if (word1[1].Split('"').Length == 3)
                            //    {
                            //        linesForExcel.Add("*" + strline);
                            //    }

                            //}

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
                                    if (!strline.Contains("*"))
                                    {
                                        //linesForExcel.Add(strline);
                                    }
                                    else
                                    {
                                        //string[] myKey = strline.Split('*');

                                        //linesForExcel.Add(myKey[0].Trim());


                                    }

                                }

                                if (i < lines.Count - 1)
                                {
                                    strline = lines[++i];
                                }
                            }

                            if (i < lines.Count - 1)
                            {
                                i--;
                                linesForExcel.Add("");
                            }
                        }
                        else
                        {
                            linesForExcel.Add("");
                        }
                        #endregion

                    }
                    #endregion

                }
            next:
                j++;
            }

            this.writeToExcel(linesForExcel);
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

        private void writeToExcel(List<string> linesForExcel)
        {
            string databasePath = txtScriptPath.Text;
            if (File.Exists(databasePath) == false)
                return;


            if (linesForExcel.Count > 0)
            {
                Microsoft.Office.Interop.Excel.Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                if (ProjectName.Length > 30)
                    ProjectName = ProjectName.Substring(0, 30);
                xlWorkSheet.Name = ProjectName;

                string qid;
                int rowno = 1;
                for (int x = 0; x < linesForExcel.Count; x++)
                {

                    if (linesForExcel[x].Contains("*QUESTION") && !linesForExcel[x].Contains("*DKCS"))
                    {
                        xlWorkSheet.Cells[rowno, 1] = linesForExcel[x];
                    }
                    else if (linesForExcel[x].Contains("*QUESTION") && linesForExcel[x].Contains("*DKCS"))
                    {
                        string[] word = linesForExcel[x].Split('*');
                        xlWorkSheet.Cells[rowno, 1] = "*" + word[1];
                        xlWorkSheet.Cells[rowno, 2] = "*" + word[2];
                    }
                    else if (linesForExcel[x].Contains("*GRIDLIST") || linesForExcel[x].Contains("*LIST"))
                    {
                        xlWorkSheet.Cells[rowno, 1] = linesForExcel[x];
                    }
                    else if (isAttribute(linesForExcel[x]))
                    {
                        string word1 = "'" + linesForExcel[x].Substring(0, linesForExcel[x].IndexOf(':') + 1);
                        string word2 = linesForExcel[x].Substring(linesForExcel[x].IndexOf(':') + 1);

                        xlWorkSheet.Cells[rowno, 1] = word1;
                        xlWorkSheet.Cells[rowno, 2] = word2;
                    }
                    else
                    {
                        xlWorkSheet.Cells[rowno, 2] = linesForExcel[x];
                    }

                    rowno++;
                }

                //xlWorkSheet.Columns.AutoFit();

                xlWorkSheet.Columns[1].ColumnWidth = 10;
                xlWorkSheet.Columns[2].ColumnWidth = 70;
                xlWorkSheet.Columns[3].ColumnWidth = 70;
                xlWorkSheet.Columns[4].ColumnWidth = 70;

                xlWorkSheet.Range["B:B"].Style.WrapText = true;
                xlWorkSheet.Range["C:C"].Style.WrapText = true;

                //xlApp.ActiveWindow.DisplayGridlines = false;

                //xlApp.Visible = true;




                //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                //xlWorkBook.SaveAs("D:\\Ismile Personal\\New folder (2)\\Analysis\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.SaveAs(myPath + "\\" + ProjectName + "_Placeholder.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();


                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

            }


            MessageBox.Show("Write Complete");
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
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
    }


}
