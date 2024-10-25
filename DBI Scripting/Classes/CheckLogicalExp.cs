using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class CheckLogicalExp
    {
        private String _postfixExpression = "";

        List<string> listOfFunctionName;
        List<string> listOfQuestionIdForDupliCheck;

        public CheckLogicalExp()
        {
            makeFuncitonNameList();
            listOfQuestionIdForDupliCheck = new List<string>();
        }

        public bool checkIfCondition(string expression, List<string> _listOfQuestionIdForDupliCheck)
        {
            _postfixExpression = "";
            listOfQuestionIdForDupliCheck = _listOfQuestionIdForDupliCheck;
            listOfQuestionIdForDupliCheck.Add("Interview");

            if (expression.ToUpper().Contains("MOBILENUMBER"))
                expression = expression + "=1";

            if (!expression.ToUpper().Contains("REGULAREXPOF"))
                if (!Regex.IsMatch(expression.Trim(), @"^[A-Za-z0-9=.,;<>!_/\&\|\[\]\(\)\{\}\-\s]+$"))
                    return false;

            if (expression.Contains("(") && !expression.Contains(")")) return false;
            if (!expression.Contains("(") && expression.Contains(")")) return false;

            if (expression.Contains("(") && expression.Contains(")") && Regex.Matches(expression, @"\(").Count != Regex.Matches(expression, @"\)").Count) return false;



            expression = expression.Replace(" ", "").Trim();
            String seperators = "\\&|\\|";
            String[] operands = expression.Split(seperators.ToCharArray());
            if (operands.Length == 1)
            {
                //return ExecuteConditionalElement(expression);
                //MessageBox.Show(expression);

                bool temp1 = checkAndOrExpression(expression);
                bool temp2 = checkSingleExpression(expression);

                return temp1 & temp2;

                //MessageBox.Show("");
            }
            expression = "(" + expression + ")";
            char[] expInCharArray = expression.ToCharArray();
            Stack<Char> myStack = new Stack<Char>();

            try
            {
                int i;
                for (i = 0; i < expInCharArray.Length; i++)
                {
                    if (expInCharArray[i] == '(')
                        myStack.Push('(');
                    else if (expInCharArray[i] == '|' || expInCharArray[i] == '&')
                    {
                        //_postfixExpression += ",";
                        _postfixExpression += "#";
                        if (myStack.Count > 1 && myStack.Peek() != '(')
                        {
                            //_postfixExpression += myStack.Pop() + ",";
                            _postfixExpression += myStack.Pop() + "#";
                        }
                        myStack.Push(expInCharArray[i]);
                    }
                    else if (expInCharArray[i] == ')')
                    {
                        while (myStack.Peek() != '(')
                        {
                            //_postfixExpression += "," + myStack.Pop();// + ",";
                            _postfixExpression += "#" + myStack.Pop();// + ",";
                        }
                        myStack.Pop();
                    }
                    else
                        _postfixExpression += expInCharArray[i];
                    // _postfixExpression = _postfixExpression.trim();

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            //return executePostfixExpression(_postfixExpression);


            //MessageBox.Show(_postfixExpression);

            bool tempx = checkAndOrExpression(_postfixExpression);
            bool tempy = checkExpression(_postfixExpression);

            return tempx & tempy;
        }
        private bool checkAndOrExpression(String expression)
        {
            Stack<Boolean> myStack = new Stack<Boolean>();
            try
            {
                int i;

                //String seperator = ",";
                String seperator = "#";
                String[] elements = expression.Split(seperator.ToCharArray());
                for (i = 0; i < elements.Length; i++)
                {

                    if (elements[i] == "|")
                        myStack.Push(myStack.Pop() | myStack.Pop());
                    else if (elements[i] == "&")
                        myStack.Push(myStack.Pop() & myStack.Pop());
                    else
                        myStack.Push(true);
                    //myStack.Push(ExecuteConditionalElement(elements[i]));

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            //return true;
        }

        private bool checkSingleExpression(String expression)
        {
            if (expression.Contains("=") && Regex.Matches(expression, "=").Count != 1) return false;
            if (expression.Contains(">") && Regex.Matches(expression, ">").Count != 1) return false;
            if (expression.Contains("<") && Regex.Matches(expression, "<").Count != 1) return false;

            String seperators = "\\=|\\!|\\>|\\<";
            String leftPart = expression.Split(seperators.ToCharArray())[0];
            String rightPart = getRightPart(expression);

            //Check regular expression syntax
            if (leftPart.ToUpper().Contains("REGEXOF") && !rightPart.ToUpper().Contains("REGULAREXPOF")) return false;
            if (!leftPart.ToUpper().Contains("REGEXOF") && rightPart.ToUpper().Contains("REGULAREXPOF")) return false;

            if (leftPart.ToUpper().Contains("REGULAREXPOF")) return false;
            if (rightPart.ToUpper().Contains("REGEXOF")) return false;
            //*******************************


            if (leftPart.Contains("[") || leftPart.Contains("]"))
            {
                // Check [ and ] exist
                if (Regex.Matches(leftPart, @"\[").Count != 1) return false;
                if (Regex.Matches(leftPart, @"\]").Count != 1) return false;
                int x;

                if (leftPart.Contains("SubStrOf"))
                    x = 1;
                // Check ValueOf]Q2[ issue
                if (leftPart.IndexOf('[') > leftPart.IndexOf(']')) return false;

                string[] word = leftPart.Trim().Split(new Char[] { '[', ']' });

                if (word.Length == 3 && word[2] != "") return false;
                if (word.Length != 3) return false;

                if (!listOfFunctionName.Contains(word[0].ToUpper())) return false;

                if (word[0] == "DateDiffOf")
                {
                    string[] logicPart = word[1].Split(',');
                    for (int n = 0; n < logicPart.Length; n++)
                    {
                        if (logicPart[n].Contains("."))
                        {
                            if (!listOfQuestionIdForDupliCheck.Contains(logicPart[n].Split('.')[0].Trim())) return false;
                        }
                        else if (logicPart[n].Contains(","))
                        {
                            if (!listOfQuestionIdForDupliCheck.Contains(logicPart[n].Split(',')[0].Trim())) return false;
                        }
                        else
                        {
                            if (!listOfQuestionIdForDupliCheck.Contains(logicPart[n].Trim())) return false;
                        }
                    }
                }
                else
                {
                    if (word[1].Contains("."))
                    {
                        if (!listOfQuestionIdForDupliCheck.Contains(word[1].Split('.')[0].Trim())) return false;
                    }
                    else if (word[1].Contains(","))
                    {
                        if (!listOfQuestionIdForDupliCheck.Contains(word[1].Split(',')[0].Trim())) return false;
                    }
                    else
                    {
                        if (!listOfQuestionIdForDupliCheck.Contains(word[1].Trim())) return false;
                    }
                }

            }
            else
            {
                if (!Regex.Match(leftPart.Trim(), "^[a-zA-Z0-9_.]+$").Success)
                    return false;
                if (leftPart.Contains("."))
                {
                    if (!listOfQuestionIdForDupliCheck.Contains(leftPart.Trim().Split('.')[0]))
                        return false;
                }
                else
                {
                    if (!listOfQuestionIdForDupliCheck.Contains(leftPart.Trim()))
                        return false;
                }
            }

            // Check Right Part
            if (rightPart.Contains("[") || rightPart.Contains("]"))
            {
                // Check [ and ] exist
                if (!rightPart.Contains("RegularExpOf"))
                {
                    if (Regex.Matches(rightPart, @"\[").Count != 1) return false;
                    if (Regex.Matches(rightPart, @"\]").Count != 1) return false;

                    // Check ValueOf]Q2[ issue
                    if (rightPart.IndexOf('[') > rightPart.IndexOf(']')) return false;

                    string[] word = rightPart.Trim().Split(new Char[] { '[', ']' });

                    if (word.Length == 3 && word[2] != "") return false;
                    if (word.Length != 3) return false;

                    if (!listOfFunctionName.Contains(word[0].ToUpper())) return false;
                }
            }
            else
            {
                if (!Regex.Match(rightPart.Trim(), "^[a-zA-Z0-9/]+$").Success && !Regex.Match(rightPart.Trim(), @"^\d+$").Success && !Regex.Match(rightPart.Trim(), @"^\d.+$").Success && !Regex.Match(rightPart.Trim(), @"^\-\d+$").Success)
                    return false;


                //if (!Regex.Match(rightPart.Trim(), @"^\d+$").Success)
                //    return false;
            }

            return true;
        }

        private bool checkExpression(String expression)
        {
            try
            {
                Stack<Boolean> myStack = new Stack<Boolean>();
                myStack.Push(true);

                int i;

                //String seperator = ",";
                String seperator = "#";
                String[] elements = expression.Split(seperator.ToCharArray());
                for (i = 0; i < elements.Length; i++)
                {

                    if (elements[i] != "|" && elements[i] != "&")
                        myStack.Push(myStack.Pop() & checkSingleExpression(elements[i]));

                }
                return myStack.Pop();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string getRightPart(String expression)
        {
            expression = expression.Trim();
            String rightPart = "";
            if (expression.Contains("<="))
                rightPart = expression.Substring(expression.IndexOf('=') + 1);
            else if (expression.Contains(">="))
                rightPart = expression.Substring(expression.IndexOf('=') + 1);
            else if (expression.Contains("!="))
                rightPart = expression.Substring(expression.IndexOf('=') + 1);
            else if (expression.Contains("<"))
                rightPart = expression.Substring(expression.IndexOf('<') + 1);
            else if (expression.Contains(">"))
                rightPart = expression.Substring(expression.IndexOf('>') + 1);
            else if (expression.Contains("="))
                rightPart = expression.Substring(expression.IndexOf('=') + 1);

            return rightPart;
        }


        private void makeFuncitonNameList()
        {
            listOfFunctionName = new List<string>();

            listOfFunctionName.Add("VALUEOF");
            listOfFunctionName.Add("SUMOF");
            listOfFunctionName.Add("TOTALOF");
            listOfFunctionName.Add("MODOF");
            listOfFunctionName.Add("MOBILENUMBER");
            listOfFunctionName.Add("NUMBEROFRESPONSE");
            listOfFunctionName.Add("SUBSTRUCTOF");
            listOfFunctionName.Add("MULTIPLYOF");
            listOfFunctionName.Add("DEVIDEOF");
            listOfFunctionName.Add("REGEXOF");
            listOfFunctionName.Add("REGULAREXPOF");
            listOfFunctionName.Add("SUBSTROF");
            listOfFunctionName.Add("LENGTHOF");
            listOfFunctionName.Add("DATEVALUEOF");
            listOfFunctionName.Add("DATEDIFFOF");
            listOfFunctionName.Add("TIMEDIFFOF");
            listOfFunctionName.Add("TYPEOF");
            listOfFunctionName.Add("DISTANCEFROM");
        }

    }
}
