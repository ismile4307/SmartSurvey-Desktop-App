using System;
using System.Collections.Generic;
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
    /// Interaction logic for ChkPostFixNote.xaml
    /// </summary>
    public partial class ChkPostFixNote : Window
    {
        private String _postfixExpression = "";

        List<string> listOfFunctionName;

        public ChkPostFixNote()
        {
            InitializeComponent();
        }

        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            makeFuncitonNameList();
            _postfixExpression = "";

            String expression = txt_Expression.Text;

            if (checkIfCondition(expression))
                MessageBox.Show("True");
            else
                MessageBox.Show("False");
        }

        private bool checkIfCondition(string expression)
        {
            if (!expression.ToUpper().Contains("REGULAREXPOF"))
                if (!Regex.IsMatch(expression.Trim(), @"^[A-Za-z0-9=.,;<>!\&\|\[\]\(\)\s]+$"))
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
                        _postfixExpression += ",";
                        if (myStack.Count > 1 && myStack.Peek() != '(')
                        {
                            _postfixExpression += myStack.Pop() + ",";
                        }
                        myStack.Push(expInCharArray[i]);
                    }
                    else if (expInCharArray[i] == ')')
                    {
                        while (myStack.Peek() != '(')
                        {
                            _postfixExpression += "," + myStack.Pop();// + ",";
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

                String seperator = ",";
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

                // Check ValueOf]Q2[ issue
                if (leftPart.IndexOf('[') > leftPart.IndexOf(']')) return false;
                    
                string[] word = leftPart.Trim().Split(new Char[] { '[', ']' });

                if (word.Length == 3 && word[2] != "") return false;
                if (word.Length != 3) return false;

                if (!listOfFunctionName.Contains(word[0].ToUpper())) return false;
                    
            }
            else
            {
                if (!Regex.Match(leftPart.Trim(), "^[a-zA-Z0-9]+$").Success)
                    return false;
            }

            // Check Right Part
            if (rightPart.Contains("[") || rightPart.Contains("]"))
            {
                // Check [ and ] exist
                if (Regex.Matches(rightPart, @"\[").Count != 1) return false;
                if (Regex.Matches(rightPart, @"\]").Count != 1) return false;

                // Check ValueOf]Q2[ issue
                if (rightPart.IndexOf('[') > leftPart.IndexOf(']')) return false;

                string[] word = rightPart.Trim().Split(new Char[] { '[', ']' });

                if (word.Length == 3 && word[2] != "") return false;
                if (word.Length != 3) return false;

                if (!listOfFunctionName.Contains(word[0].ToUpper())) return false;

            }
            else
            {
                if (!Regex.Match(rightPart.Trim(), @"^\d+$").Success)
                    return false;
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

                String seperator = ",";
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

        private bool ExecuteConditionalElement(String element)
        {
            element = element.Trim();

            String seperators = "\\=|\\!|\\>|\\<";
            String leftPart = element.Split(seperators.ToCharArray())[0];
            String myOperator = "";
            String compareValue = "";
            String rightPart = "";

            String whereCondition = "";
            if (element.Contains("<="))
            {
                myOperator = "<= ";
                rightPart = element.Substring(element.IndexOf('=') + 1);
            }
            else if (element.Contains(">="))
            {
                myOperator = ">= ";
                rightPart = element.Substring(element.IndexOf('=') + 1);
            }
            else if (element.Contains("!="))
            {
                myOperator = "= ";
                rightPart = element.Substring(element.IndexOf('=') + 1);
            }
            else if (element.Contains("<"))
            {
                myOperator = "<  ";
                rightPart = element.Substring(element.IndexOf('<') + 1);
            }
            else if (element.Contains(">"))
            {
                myOperator = "> ";
                rightPart = element.Substring(element.IndexOf('>') + 1);
            }
            else if (element.Contains("="))
            {
                myOperator = "= ";                    //Here use the = Operatore and return the alternate result
                rightPart = element.Substring(element.IndexOf('=') + 1);
            }

            //Calculating the right part value for compare

            if (Char.IsLetter(rightPart, 1) && rightPart.Contains("["))
            {
                //FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, rightPart, ansDbAdapter);        //This constructure didn't get any agument
                //compareValue = obj.getComparedValue();

            }
            else if (Char.IsLetter(rightPart, 1) && !rightPart.Contains("["))
            {
                //Error express
            }
            else
            {
                compareValue = rightPart;
            }

            if (element.Contains("!="))
            {
                myOperator = "=";
            }
            bool bResult = false;
            if (leftPart.Contains("["))
            {
                //FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, leftPart, compareValue, myOperator.trim(), ansDbAdapter);
                //bResult = obj.executeConditon();
            }
            else
            {
                //String query = "SELECT Response FROM T_RespAnswer WHERE QId='" + leftPart
                //        + "' AND ProjectId=" + sProjectId + " AND RespondentID="
                //        + sRespondentId + "  " + " AND Response" + myOperator + "'" + compareValue + "';";
                //bResult = executeMyQuery(query);
            }

            if (element.Contains("!="))
                if (bResult == true)
                    bResult = false;
                else
                    bResult = true;

            return bResult;
        }

        private void makeFuncitonNameList()
        {
            listOfFunctionName = new List<string>();

            listOfFunctionName.Add("VALUEOF");
            listOfFunctionName.Add("SUMOF");
            listOfFunctionName.Add("MODOF");
            listOfFunctionName.Add("MOBILENUMBER");
            listOfFunctionName.Add("NUMBEROFRESPONSE");
            listOfFunctionName.Add("SUBSTRUCTOF");
            listOfFunctionName.Add("MULTIPLYOF");
            listOfFunctionName.Add("DEVIDEOF");
            listOfFunctionName.Add("REGEXOF");
            listOfFunctionName.Add("REGULAREXPOF");

        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
