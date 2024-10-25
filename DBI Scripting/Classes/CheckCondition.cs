using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class CheckCondition
    {
        private String sProjectId;
        private String sRespondentId;
        private String QuestionId;
        private String _postfixExpression = "";
        private ConnectionDB connAnsDB;
        private ConnectionDB connQntrDB;

        private DBHelper myDBHelper;

        public CheckCondition(ConnectionDB _connAnsDB, ConnectionDB _connQntrDB)
        {
            connAnsDB = _connAnsDB;
            connQntrDB = _connQntrDB;
            myDBHelper = new DBHelper();
        }

        public bool convetToPostFixNotationAndExecute(String aProjectId, String aRespondentId, String aQid, String expression)
        {
            sProjectId = aProjectId;
            sRespondentId = aRespondentId;
            QuestionId = aQid;

            expression = expression.Replace(" ", "").Trim();
            String seperators = "\\&|\\|";
            String[] operands = expression.Split(seperators.ToCharArray());
            if (operands.Length == 1)
                return ExecuteConditionalElement(expression);
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
                MessageBox.Show("wrong expression", QuestionId
                        + " : Check the condition for bracet \"" + expression
                        + "\"");
            }

            return executePostfixExpression(_postfixExpression);
        }

        private bool executePostfixExpression(String expression)
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
                        myStack.Push(ExecuteConditionalElement(elements[i]));

                }
                return myStack.Pop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("wrong operator", QuestionId
                        + " : Check the condition for operator");
            }

            return false;
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
                FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, rightPart, connAnsDB,connQntrDB);        //This constructure didn't get any agument
                compareValue = obj.getComparedValue();

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
                FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, leftPart, compareValue, myOperator.Trim(), connAnsDB, connQntrDB);
                bResult = obj.executeConditon();
            }
            else
            {
                String query = "SELECT Response FROM T_RespAnswer WHERE QId='" + leftPart
                        + "' AND ProjectId=" + sProjectId + " AND RespondentID="
                        + sRespondentId + "  " + " AND Response" + myOperator + "'" + compareValue + "';";
                bResult = executeMyQuery(query);
            }

            if (element.Contains("!="))
                if (bResult == true)
                    bResult = false;
                else
                    bResult = true;

            return bResult;
        }

        private bool executeMyQuery(String query)
        {
            try
            {
                // MyDbAdapter adapter = new MyDbAdapter(mContext, "CAPIADB.db");
                //Log.e("Query", query);
                DataTable dt= myDBHelper.getAnsTableData(query, connAnsDB);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //e.getMessage();
                return false;
            }
        }
    }
}
