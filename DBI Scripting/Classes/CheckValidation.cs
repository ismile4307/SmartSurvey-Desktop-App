using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class CheckValidation
    {
        //private String sProjectId;
        //private String sRespondentId;
        //private String sQuestionId;
        //private String _postfixExpression = "";
        //private List<Response> lstMyData;
        //ConnectionDB ansDbConnector = new ConnectionDB();
        //ConnectionDB quesDbConnector = new ConnectionDB();

        //public CheckValidation(List<Response> _lstMyData, String _projectId, String _respondent, String _qId, ConnectionDB _ansDbConnector, ConnectionDB _quesDbConnector)
        //{
        //    // TODO Auto-generated constructor stub
        //    sProjectId = _projectId;
        //    sRespondentId = _respondent;
        //    ansDbConnector = _ansDbConnector;
        //    quesDbConnector = _quesDbConnector;
        //    lstMyData = _lstMyData;
        //    sQuestionId = _qId;
        //}

        //public bool convetToPostFixNotationAndExecute(String expression)
        //{
        //    expression = expression.Replace(" ", "").Trim();
        //    String seperators = "\\&|\\|";
        //    String[] operands = expression.Split(seperators.ToCharArray());
        //    if (operands.Length == 1)
        //        return executeCheckCondition(expression);
        //    expression = "(" + expression + ")";
        //    char[] expInCharArray = expression.ToCharArray();
        //    Stack<Char> myStack = new Stack<Char>();

        //    try
        //    {
        //        int i;
        //        for (i = 0; i < expInCharArray.Length; i++)
        //        {
        //            if (expInCharArray[i] == '(')
        //                myStack.Push('(');
        //            else if (expInCharArray[i] == '|' || expInCharArray[i] == '&')
        //            {
        //                _postfixExpression += ",";
        //                if (myStack.Count > 1 && myStack.Peek() != '(')
        //                {
        //                    _postfixExpression += myStack.Pop() + ",";
        //                }
        //                myStack.Push(expInCharArray[i]);
        //            }
        //            else if (expInCharArray[i] == ')')
        //            {
        //                while (myStack.Peek() != '(')
        //                {
        //                    _postfixExpression += "," + myStack.Pop();// + ",";
        //                }
        //                myStack.Pop();
        //            }
        //            else
        //                _postfixExpression += expInCharArray[i];
        //            // _postfixExpression = _postfixExpression.trim();

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("wrong expression", sQuestionId
        //                + " : Check the condition for bracet \"" + expression
        //                + "\"");
        //    }

        //    return executePostfixExpression(_postfixExpression);
        //}

        //private bool executePostfixExpression(String expression)
        //{
        //    Stack<bool> myStack = new Stack<bool>();
        //    try
        //    {
        //        int i;

        //        String seperator = ",";
        //        String[] elements = expression.Split(seperator.ToCharArray());
        //        for (i = 0; i < elements.Length; i++)
        //        {

        //            if (elements[i].Equals("|"))
        //                myStack.Push(myStack.Pop() | myStack.Pop());
        //            else if (elements[i]=="&")
        //                myStack.Push(myStack.Pop() & myStack.Pop());
        //            else
        //                myStack.Push(executeCheckCondition(elements[i]));

        //        }
        //        return myStack.Pop();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("wrong operator", sQuestionId
        //                + " : Check the condition for operator");
        //    }

        //    return false;
        //}

        //public bool executeCheckCondition(String element)
        //{
        //    element = element.Trim();

        //    String seperators = "\\=|\\!|\\>|\\<";
        //    String leftPart = element.Split(seperators.ToCharArray())[0];
        //    String rightPart = "";//checkLogic.split(seperators)[1];
        //    String myOperator = "";
        //    String compareValue = "";

        //    String whereCondition = "";
        //    if (element.Contains("<="))
        //    {
        //        myOperator = "<= ";
        //        rightPart = element.Substring(element.IndexOf('=') + 1);
        //    }
        //    else if (element.Contains(">="))
        //    {
        //        myOperator = ">= ";
        //        rightPart = element.Substring(element.IndexOf('=') + 1);
        //    }
        //    else if (element.Contains("!="))
        //    {
        //        myOperator = "!= ";
        //        rightPart = element.Substring(element.IndexOf('=') + 1);
        //    }
        //    else if (element.Contains("<"))
        //    {
        //        myOperator = "<  ";
        //        rightPart = element.Substring(element.IndexOf('<') + 1);
        //    }
        //    else if (element.Contains(">"))
        //    {
        //        myOperator = "> ";
        //        rightPart = element.Substring(element.IndexOf('>') + 1);
        //    }
        //    else if (element.Contains("="))
        //    {
        //        myOperator = "= ";
        //        rightPart = element.Substring(element.IndexOf('=') + 1);
        //    }
        //    if (!rightPart.Equals(""))
        //    {
        //        if (Char.IsLetter(rightPart,0) && rightPart.Contains("["))
        //        {

        //            FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, rightPart, ansDbConnector);        //This constructure didn't get any agument
        //            if (!rightPart.ToLower().Contains("regularexpof"))
        //            {
        //                String[] arrayQid = obj.getQid();

        //                if (!arrayQid[0].Equals(sQuestionId))
        //                {
        //                    compareValue = obj.getComparedValue();
        //                }
        //                else
        //                {
        //                    compareValue = obj.getComparedValueFromCurrentResponse(lstMyData);
        //                }
        //            }
        //            else
        //            {
        //                compareValue = rightPart.ToLower();
        //            }

        //        }
        //        else if (Char.IsLetter(rightPart,0) && !rightPart.Contains("["))
        //        {
        //            //Error express
        //        }
        //        else
        //        {
        //            compareValue = rightPart;
        //        }
        //    }

        //    if (element.Contains("!="))
        //    {
        //        myOperator = "=";
        //    }

        //    bool bResult = false;
        //    if (leftPart.Contains("["))
        //    {
        //        FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, leftPart, compareValue, myOperator.Trim(), ansDbConnector);
        //        String[] arrayQid = obj.getQid();

        //        if (!arrayQid[0].Equals(sQuestionId))
        //        {
        //            bResult = obj.executeConditon();
        //        }
        //        else
        //        {
        //            bResult = obj.executeConditonforCurrentQuestion(lstMyData);
        //        }

        //    }
        //    else
        //    {
        //        FunctionalCondition obj = new FunctionalCondition(sProjectId, sRespondentId, leftPart, compareValue, myOperator.Trim(), ansDbConnector);

        //        if (!leftPart.Equals(sQuestionId))
        //        {
        //            String query = "SELECT Response FROM T_RespAnswer WHERE QId='" + leftPart
        //                    + "' AND ProjectId=" + sProjectId + " AND RespondentID="
        //                    + sRespondentId + "  " + " AND Response" + myOperator + "'" + compareValue + "';";

        //            bResult = executeMyQuery(query);
        //        }
        //        else
        //        {
        //            //bResult = lstMyData.Contains(compareValue);
        //            for (int n = 0; n < lstMyData.Count; n++)
        //            {
        //                if (!bResult)
        //                    bResult = obj.compare(lstMyData[n].responseValue);
        //            }
        //        }
        //    }

        //    if (element.Contains("!="))
        //        if (bResult == true)
        //            bResult = false;
        //        else
        //            bResult = true;
        //    return bResult;
        //}

        //private bool executeMyQuery(String query)
        //{
        //    try
        //    {
        //        // MyDbAdapter adapter = new MyDbAdapter(mContext, "CAPIADB.db");
        //        //Log.e("Query", query);
        //        if (ansDbConnector.sqlite_conn.State == ConnectionState.Closed)
        //            ansDbConnector.sqlite_conn.Open();

        //        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute WHERE QId!=''", connDB.sqlite_conn);
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
        //                myData = myData + "attribute_english[]=" + dr[2].ToString().Replace("'", "''") + "&";
        //                myData = myData + "attribute_bengali[]=" + dr[3].ToString().Replace("'", "''") + "&";
        //                myData = myData + "attribute_value[]=" + dr[4].ToString() + "&";
        //                myData = myData + "attribute_order[]=" + dr[5].ToString().Replace("'", "''") + "&";
        //                myData = myData + "take_openended[]=" + dr[6].ToString() + "&";
        //                myData = myData + "is_exclusive[]=" + dr[7].ToString() + "&";
        //                myData = myData + "link_id1[]=" + dr[8].ToString() + "&";
        //                myData = myData + "link_id2[]=" + dr[9].ToString() + "&";
        //                myData = myData + "min_value[]=" + dr[10].ToString() + "&";
        //                myData = myData + "max_value[]=" + dr[11].ToString() + "&";
        //                myData = myData + "force_and_msg_opt[]=" + dr[12].ToString() + "&";
        //                myData = myData + "group_name[]=" + dr[13].ToString() + "&";
        //                myData = myData + "filter_qid[]=" + dr[14].ToString() + "&";
        //                myData = myData + "filter_type[]=" + dr[15].ToString() + "&";
        //                myData = myData + "excep_value[]=" + dr[16].ToString() + "&";
        //                myData = myData + "comments[]=" + dr[17].ToString() + "&";
        //                myData = myData + "attribute_lang3[]=" + dr[18].ToString() + "&";
        //                myData = myData + "attribute_lang4[]=" + dr[19].ToString() + "&";
        //                myData = myData + "attribute_lang5[]=" + dr[20].ToString() + "&";
        //                myData = myData + "attribute_lang6[]=" + dr[21].ToString() + "&";
        //                myData = myData + "attribute_lang7[]=" + dr[22].ToString() + "&";
        //                myData = myData + "attribute_lang8[]=" + dr[23].ToString() + "&";
        //                myData = myData + "attribute_lang9[]=" + dr[24].ToString() + "&";
        //                myData = myData + "attribute_lang10[]=" + dr[25].ToString() + "&";

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


        //        Cursor crs = ansDbAdapter.getData(query);
        //        int totalRaw = crs == null ? 0 : crs.getCount();
        //        crs.close();
        //        ansDbAdapter.close();
        //        if (totalRaw > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        //e.getMessage();
        //        return false;
        //    }
        //}
    }
}
