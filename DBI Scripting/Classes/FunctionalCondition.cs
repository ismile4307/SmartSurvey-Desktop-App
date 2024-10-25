using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class FunctionalCondition
    {
        String sElement, sProjectId, sRespondentId, sComparedValue, sOperator;

        private ConnectionDB connAnsDB;
        private ConnectionDB connQntrDB;

        private DBHelper myDBHelper;

        int iStartPosition, iLength;
        String Param1, Param2;

        public FunctionalCondition(String _projectId, String _respondentId, String _conditionPart, ConnectionDB _connAnsDB, ConnectionDB _connQntrDB)
        {
            sProjectId = _projectId;
            sRespondentId = _respondentId;
            sElement = _conditionPart;
            connAnsDB = _connAnsDB;
            connQntrDB = _connQntrDB;
            myDBHelper = new DBHelper();
        }

        public FunctionalCondition(String _projectId, String _respondentId, String _conditionPart, String _comparedValue, String _operator, ConnectionDB _connAnsDB, ConnectionDB _connQntrDB)
        {
            sProjectId = _projectId;
            sRespondentId = _respondentId;
            sElement = _conditionPart;
            sComparedValue = _comparedValue;
            sOperator = _operator;
            connAnsDB = _connAnsDB;
            connQntrDB = _connQntrDB;
            myDBHelper = new DBHelper();
        }

        public bool executeConditon()
        {
            String myFunction = sElement.Substring(0, sElement.IndexOf('[')).ToLower();
            String[] qIds = this.getQid();
            String query = this.getQueryString(qIds);

            if (myFunction.Contains("numberofresponse"))
            {
                String resultedValue = "" + executeNumberOfResponse(query);
                bool res = compare(resultedValue);
                return res;
            }
            else if (myFunction.Contains("totalof"))
            {
                String resultedValue = "" + executeSumOfTotalResponse(query);
                bool res = compare(resultedValue);
                return res;
            }
            else if (myFunction.Contains("valueof"))
            {
                String resultedValue = "" + executeValueOfResponse(query);
                bool res = compare2(resultedValue);
                return res;
            }
            else if (myFunction.Contains("modof"))
            {
                qIds = this.getQids();
                query = this.getQueryString(qIds[0]);
                int divisor = Convert.ToInt32(qIds[1]);
                String resultedValue = "" + executeModOfResponse(query, divisor);
                bool res = compare(resultedValue);
                return res;
            }
            else if (myFunction.Contains("sumof"))
            {
                qIds = this.getQids();
                long sum = 0;
                for (int i = 0; i < qIds.Length; i++)
                {
                    query = this.getQueryString(qIds[i]);
                    sum = sum + Convert.ToInt32(executeSumOfResponse(query));
                }
                String resultedValue = "" + sum;
                bool res = compare(resultedValue);
                return res;
            }
            else if (myFunction.Contains("substr"))
            {
                iStartPosition = 0;
                iLength = 0;

                qIds = this.getQidForSubStr();
                query = this.getQueryString(qIds);
                String resultedValue = "" + executeSubStringResponse(query);

            }
            else if (myFunction.Contains("lengthof"))
            {
                query = this.getQueryString(qIds);
                String resultedValue = "" + executeLengthOfResponse(query);

                bool res = compare(resultedValue);
                return res;
            }
            return false;
        }

        //This is for current question Operation
        public bool executeConditonforCurrentQuestion(List<Response> lstMyData)
        {
            Param1 = ""; Param2 = "";
            String myFunction = sElement.Substring(0, sElement.IndexOf('[')).ToLower();
            String[] qIds = this.getQid();
            String query = this.getQueryString(qIds);

            if (myFunction.Contains("numberofresponse"))
            {
                String computedValue = "" + executeNumberOfResponseFromCurrentResponse(lstMyData);
                bool res = compare(computedValue);
                return res;
            }
            else if (myFunction.Contains("totalof"))
            {
                String computedValue = "" + executeSumOfTotalResponseFromCurrentResponse(lstMyData);
                bool res = compare(computedValue);
                return res;
            }
            else if (myFunction.Contains("valueof"))
            {
                String computedValue = "" + executeValueOfResponseFromCurrentResponse(lstMyData, qIds);
                bool res = compare(computedValue);
                return res;
            }
            else if (myFunction.Contains("mobilenumber"))
            {
                bool res = executeMobileNumberFromCurrentResponse(lstMyData, qIds);
                return !res;
            }
            else if (myFunction.Contains("decimalnumber"))
            {
                bool res = executeDecimalNumberFromCurrentResponse(lstMyData);
                return !res;
            }
            else if (myFunction.Contains("rocketnumber"))
            {
                bool res = executeMobileNumberFromCurrentResponse2(lstMyData);
                return !res;
            }
            else if (myFunction.Contains("substr"))
            {
                iStartPosition = 0;
                iLength = 0;

                qIds = this.getQidForSubStr();
                String computedValue = executeSubStringFromCurrentResponse(lstMyData, qIds);
                bool res = compare(computedValue);
                return res;
            }
            else if (myFunction.Contains("lengthof"))
            {
                String computedValue = "" + executeLengthOfResponseFromCurrentResponse(lstMyData, qIds);
                bool res = compare(computedValue);
                return res;
            }
            else if (myFunction.Contains("regexof"))
            {
                String computedValue = "" + executeValueOfResponseFromCurrentResponse(lstMyData, qIds);

                String regularExpression = sComparedValue.Substring(sComparedValue.IndexOf('[') + 1).Trim();
                regularExpression = regularExpression.Substring(0, regularExpression.LastIndexOf(']'));
                bool res = compareRegex(computedValue, regularExpression);
                return res;
            }
            return false;
        }

        public bool compare(String resultedValue)
        {
            try
            {
                if (resultedValue == "")
                    return false;

                int i_value = Convert.ToInt32(sComparedValue.Trim());
                int i_resultedValue = Convert.ToInt32(resultedValue.Trim());
                sOperator = sOperator.Trim();
                if (sOperator == ">=")
                    return i_resultedValue >= i_value;
                if (sOperator == "<=")
                    return i_resultedValue <= i_value;
                if (sOperator == "!=")
                    return i_resultedValue != i_value;
                if (sOperator == "=")
                    return i_resultedValue == i_value;
                if (sOperator == ">")
                    return i_resultedValue > i_value;
                if (sOperator == "<")
                    return i_resultedValue < i_value;
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool compare2(String resultedValue)
        {
            try
            {
                if (resultedValue == "")
                    return false;
                String[] resultedValues = resultedValue.Split(',');

                int i_value = Convert.ToInt32(sComparedValue.Trim());
                bool myResult = false;
                for (int i = 0; i < resultedValues.Length; i++)
                {
                    int i_resultedValue = Convert.ToInt32(resultedValues[i].Trim());
                    sOperator = sOperator.Trim();
                    if (sOperator == ">=")
                        myResult = i_resultedValue >= i_value;
                    if (sOperator == "<=")
                        myResult = i_resultedValue <= i_value;
                    if (sOperator == "!=")
                        myResult = i_resultedValue != i_value;
                    if (sOperator == "=")
                        myResult = i_resultedValue == i_value;
                    if (sOperator == ">")
                        myResult = i_resultedValue > i_value;
                    if (sOperator == "<")
                        myResult = i_resultedValue < i_value;

                    if (myResult == true)
                        return myResult;
                }
                return myResult;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool compareRegex(String computedValue, String regularExpression)
        {
            Regex re = new Regex(regularExpression);
            return re.IsMatch(computedValue);
        }

        public String getComparedValue()
        {

            String myFunction = sElement.Substring(0, sElement.IndexOf('[')).ToLower();
            String[] qIds = this.getQid();
            String query = this.getQueryString(qIds);

            if (myFunction == "numberofresponse")
                return executeNumberOfResponse(query);
            else if (myFunction == "totalof")
                return executeSumOfTotalResponse(query);
            else if (myFunction == "valueof")
                return executeValueOfResponse(query);
            else if (myFunction == "modof")
            {
                qIds = this.getQids();
                query = this.getQueryString(qIds[0]);
                int divisor = Convert.ToInt32(qIds[1]);
                return executeModOfResponse(query, divisor);
            }
            else if (myFunction.Contains("sumof"))
            {
                qIds = this.getQids();
                long sum = 0;
                for (int i = 0; i < qIds.Length; i++)
                {
                    query = this.getQueryString(qIds[i]);
                    sum = sum + Convert.ToInt32(executeSumOfResponse(query));
                }
                return "" + sum;
            }
            else
                return "";

        }

        //Get the Question Id and Attribute Order
        public String[] getQid()
        {
            String allQId = sElement.Substring(sElement.IndexOf('[') + 1).Trim();
            allQId = allQId.Substring(0, allQId.IndexOf(']'));
            //Here 0 Indexed value is qid and 1 indexed value is attribute Order
            String[] QIds;

            if (allQId.Contains(";"))
            {
                String[] QId = allQId.Split(';');

                if (QId.Length == 2)
                    Param1 = QId[1];
                if (QId.Length == 3)
                    Param2 = QId[2];

                if (QId[0].Contains("."))
                {
                    QIds = QId[0].Split('.');
                }
                else
                {
                    QIds = new String[1];
                    QIds[0] = QId[0];
                }
            }
            else if (allQId.Contains("."))
                QIds = allQId.Split('.');
            else
            {
                QIds = new String[1];
                QIds[0] = allQId;
            }

            //        if (allQId.Contains("."))
            //            QIds = allQId.split("\\.");
            //        else {
            //            QIds = new String[1];
            //            QIds[0] = allQId;
            //        }
            return QIds;
        }

        public String[] getQids()
        {
            String allQId = sElement.Substring(sElement.IndexOf('[') + 1).Trim();
            allQId = allQId.Substring(0, allQId.IndexOf(']'));
            //Here 0 Indexed value is qid and 1 indexed value is attribute Order
            String[] QIds;
            if (allQId.Contains(","))
                QIds = allQId.Split(',');
            else
            {
                QIds = new String[1];
                QIds[0] = allQId;
            }
            return QIds;
        }

        //Get the Question Id for Substring Function
        public String[] getQidForSubStr()
        {
            String allQId = sElement.Substring(sElement.IndexOf('[') + 1).Trim();
            allQId = allQId.Substring(0, allQId.IndexOf(']'));
            //Here 0 Indexed value is qid and 1 indexed value is attribute Order
            String[] QIds;

            if (allQId.Contains(";"))
            {
                String[] QId = allQId.Split(';');

                iStartPosition = Convert.ToInt32(QId[1]);
                iLength = Convert.ToInt32(QId[2]);
                if (QId[0].Contains("."))
                {
                    QIds = QId[0].Split('.');
                }
                else
                {
                    QIds = new String[1];
                    QIds[0] = QId[0];
                }
            }
            else
            {
                QIds = new String[1];
                QIds[0] = allQId;
            }
            return QIds;
        }

        private String getQueryString(String[] qIds)
        {
            if (qIds.Length > 1)
                return "SELECT Response FROM T_RespAnswer WHERE QId='" + qIds[0] + "' AND rOrderTag=" + qIds[1] + " AND ProjectId=" + sProjectId + " AND RespondentID=" + sRespondentId + "  ";
            else
                return "SELECT Response FROM T_RespAnswer WHERE QId='" + qIds[0] + "' AND ProjectId=" + sProjectId + " AND RespondentID=" + sRespondentId + "  ";

        }

        private String getQueryString(String qId)
        {
            return "SELECT Response FROM T_RespAnswer WHERE QId='" + qId + "' AND ProjectId=" + sProjectId + " AND RespondentID=" + sRespondentId + "  ";
        }

        private String executeNumberOfResponse(String query)
        {
            try
            {
                //MyDbAdapter adapter = new MyDbAdapter(mContext, "CAPIADB.db");
                int totalRaw = 0;
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                if(dt.Rows.Count>0)
                    totalRaw = dt.Rows.Count;

                return "" + totalRaw;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private String executeSumOfTotalResponse(String query)
        {
            try
            {
                //MyDbAdapter adapter = new MyDbAdapter(mContext, "CAPIADB.db");
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                int iTotal = 0;
                //int totalRaw = crs == null ? 0 : crs.getCount();

                foreach (DataRow dr in dt.Rows)
                {
                    iTotal = iTotal + Convert.ToInt32(dr["Response"].ToString());
                }

                return "" + iTotal;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Function for get Value Response
        public String executeValueOfResponse(String query)
        {
            try
            {
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                String sValue = "";

                foreach (DataRow dr in dt.Rows)
                {
                    sValue = sValue + dr["Response"].ToString() + ",";
                }
                if (sValue != "")
                    return sValue.Substring(0, sValue.Length - 1);
                else
                    return sValue;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Function for get Sum Value
        public String executeSumOfResponse(String query)
        {
            try
            {
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                int sValue = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    sValue = sValue + Convert.ToInt32(dr["Response"].ToString());
                }
                return "" + sValue;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Function for get Modulus value
        public String executeModOfResponse(String query, int divisor)
        {
            try
            {
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                String sValue = "";
                int temp = dt.Rows.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Response"].ToString()!="")
                    {
                        sValue = "" + (Convert.ToInt32(dr["Response"].ToString()) % divisor);
                    }

                }
                return sValue;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Function for get Substring String Response
        public String executeSubStringResponse(String query)
        {
            try
            {
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                String sValue = "";
                int temp = dt.Rows.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    sValue = sValue + dr["Response"].ToString();
                }
                if (sValue.Length < (iStartPosition + iLength - 1))
                    return "" + sValue;
                else
                    return sValue.Substring(iStartPosition - 1, (iStartPosition + iLength - 1));
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public String executeLengthOfResponse(String query)
        {
            try
            {
                DataTable dt = myDBHelper.getAnsTableData(query, connAnsDB);
                String sValue = "";
                int temp = dt.Rows.Count;
                foreach (DataRow dr in dt.Rows)
                {
                    sValue = sValue + dr["Response"].ToString() + ",";
                }
                if (sValue != "")
                    return "" + sValue.Substring(0, sValue.Length - 1).Length;
                else
                    return "" + sValue.Length;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        //******************************************************************************************************************************


        public String getComparedValueFromCurrentResponse(List<Response> lstMyData)
        {

            String myFunction = sElement.Substring(0, sElement.IndexOf('[')).ToLower();
            String[] qIds = this.getQid();
            String query = this.getQueryString(qIds);

            if (myFunction == "numberofresponse")
                return "" + executeNumberOfResponseFromCurrentResponse(lstMyData);
            else if (myFunction == "totalof")
                return "" + executeSumOfTotalResponseFromCurrentResponse(lstMyData);
            else if (myFunction == "valueof")
                return executeValueOfResponseFromCurrentResponse(lstMyData, qIds);
            else if (myFunction.Contains("substr"))
            {
                iStartPosition = 0;
                iLength = 0;

                qIds = this.getQidForSubStr();
                return executeSubStringFromCurrentResponse(lstMyData, qIds);

            }
            else if (myFunction == "lengthof")
                return executeLengthOfResponseFromCurrentResponse(lstMyData, qIds);
            else
                return "";

        }

        //Function for get Total number of Response
        private int executeNumberOfResponseFromCurrentResponse(List<Response> lstMyData)
        {
            try
            {
                int iTotal = 0;
                for (int i = 0; i < lstMyData.Count; i++)
                {
                    if (lstMyData[i].responseValue.Trim() != "")
                    {
                        iTotal = iTotal + 1;
                    }
                }
                return iTotal;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //Function for get Sum of Total Response
        private int executeSumOfTotalResponseFromCurrentResponse(List<Response> lstMyData)
        {
            try
            {
                int iTotal = 0;
                for (int i = 0; i < lstMyData.Count; i++)
                {
                    iTotal = iTotal + Convert.ToInt32(lstMyData[i].responseValue.Trim() == "" ? "0" : lstMyData[i].responseValue);
                }
                return iTotal;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        //Get Value of Response
        public String executeValueOfResponseFromCurrentResponse(List<Response> lstMyData, String[] qIds)
        {
            try
            {
                String sValue = "";
                if (qIds.Length > 1)
                {
                    sValue = lstMyData[Convert.ToInt32(qIds[1]) - 1].responseValue;
                }
                else
                {
                    sValue = lstMyData[0].responseValue;
                }

                if (sValue == "")
                    sValue = "0";

                //			for (int i=0;i<lstMyData.size();i++){
                //				sValue=sValue+lstMyData.get(i);
                //			}
                return sValue;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Check Mobile Number
        private bool executeMobileNumberFromCurrentResponse(List<Response> listOfResponse, String[] qIds)
        {
            try
            {
                bool result = true;
                //            for (int i = 0; i < listOfResponse.size(); i++) {

                String mobileNumber = "";
                if (qIds.Length > 1)
                {
                    mobileNumber = listOfResponse[Convert.ToInt32(qIds[1]) - 1].responseValue;
                }
                else
                {
                    mobileNumber = listOfResponse[0].responseValue;
                }

                if (mobileNumber == "")
                    result = result & true;
                else
                {
                    if (mobileNumber.Length != 11)
                    {
                        result = result & false;
                    }
                    if (mobileNumber.Substring(0, 3) != "017" &&
                            mobileNumber.Substring(0, 3) != "019" &&
                            mobileNumber.Substring(0, 3) != "016" &&
                            mobileNumber.Substring(0, 3) != "018" &&
                            mobileNumber.Substring(0, 3) != "015" &&
                            mobileNumber.Substring(0, 3) != "013" &&
                            mobileNumber.Substring(0, 3) != "014" &&
                            mobileNumber.Substring(0, 3) != "011")
                    {
                        result = result & false;
                    }
                }
                //            }
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Check Decimal Number
        private bool executeDecimalNumberFromCurrentResponse(List<Response> listOfResponse)
        {
            try
            {
                bool result = true;
                int myCounter = 0;
                for (int i = 0; i < listOfResponse.Count; i++)
                {
                    String myNumber = listOfResponse[i].responseValue;

                    for (int n = 0; n < myNumber.Length; n++)
                    {
                        String temp = myNumber.Substring(n, n + 1);
                        if (temp != ".")
                        {
                            bool flag = Char.IsDigit(myNumber, n);
                            if (!flag)
                                return false;
                        }
                        else
                        {
                            myCounter++;
                        }
                    }
                }

                if (myCounter > 1)
                    return false;

                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Check Mobile Number2
        private bool executeMobileNumberFromCurrentResponse2(List<Response> listOfResponse)
        {
            try
            {
                bool result = true;
                for (int i = 0; i < listOfResponse.Count; i++)
                {
                    String mobileNumber = listOfResponse[i].responseValue;

                    if (mobileNumber == "")
                        result = result & true;
                    else
                    {
                        if (mobileNumber.Length != 12)
                        {
                            result = result & false;
                        }
                        if (mobileNumber.Substring(0, 3) != "017" &&
                                mobileNumber.Substring(0, 3) != "019" &&
                                mobileNumber.Substring(0, 3) != "016" &&
                                mobileNumber.Substring(0, 3) != "018" &&
                                mobileNumber.Substring(0, 3) != "015" &&
                                mobileNumber.Substring(0, 3) != "013" &&
                                mobileNumber.Substring(0, 3) != "014" &&
                                mobileNumber.Substring(0, 3) != "011")
                        {
                            result = result & false;
                        }
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //Function for get Substring String Response
        public String executeSubStringFromCurrentResponse(List<Response> listOfResponse, String[] qIds)
        {
            try
            {
                String sValue = "";
                if (qIds.Length > 1)
                {
                    sValue = listOfResponse[Convert.ToInt32(qIds[1]) - 1].responseValue;
                }
                else
                {
                    sValue = listOfResponse[0].responseValue;
                }
                if (sValue.Length < (iStartPosition + iLength - 1))
                    return "" + sValue;
                else
                    return sValue.Substring(iStartPosition - 1, (iStartPosition + iLength - 1));
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Get Value of Response
        public String executeLengthOfResponseFromCurrentResponse(List<Response> lstMyData, String[] qIds)
        {
            try
            {
                String sValue = "";
                if (qIds.Length > 1)
                {
                    sValue = lstMyData[Convert.ToInt32(qIds[1]) - 1].responseValue;
                }
                else
                {
                    sValue = lstMyData[0].responseValue;
                }

                //			for (int i=0;i<lstMyData.size();i++){
                //				sValue=sValue+lstMyData.get(i);
                //			}
                return "" + sValue.Length;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        //Check Regex Expression
        public bool executeRegexExpressionFromCurrentResponse(List<Response> listOfResponse, String[] qIds)
        {
            try
            {
                bool result = true;
                //            for (int i = 0; i < listOfResponse.size(); i++) {

                String myValue = "";
                if (qIds.Length > 1)
                {
                    myValue = listOfResponse[Convert.ToInt32(qIds[1]) - 1].responseValue;
                }
                else
                {
                    myValue = listOfResponse[0].responseValue;
                }
                String regexExp = Param1;

                Regex re = new Regex(regexExp);
                return re.IsMatch(myValue);

                //            }
                return result;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
