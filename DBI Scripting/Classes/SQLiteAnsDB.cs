using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DBI_Scripting.Classes
{
    class SQLiteAnsDB
    {
        private string QdbConnString;
        private string AdbConnString;

        public SQLiteConnection Qconnection;

        private List<string> listOfMSQuestion = new List<string>();
        private List<String> listOfResponseTypeQId = new List<String>();
        public SQLiteAnsDB(string Qdb)
        {
            this.QdbConnString = @"Data Source=" + Qdb + "; Version=3;";
            this.Qconnection = new SQLiteConnection(this.QdbConnString);

        }

        public void connect()
        {
            Qconnection.Open();
        }

        private List<string> getTableColumn()
        {
            try
            {
                List<string> columnName = new List<string>();

                columnName.Add("RespondentId");
                //columnName.Add("MobileNo");
                //columnName.Add("Centre");
                columnName.Add("Latitude");
                columnName.Add("Longitude");
                columnName.Add("SurveyDateTime");

                this.populateListForResponseType();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.ProjectId, T_Question.QId, T_Question.QDesc3, T_Question.QType FROM T_Question INNER JOIN T_QType ON T_Question.QType = T_QType.ForQuesLink WHERE T_QType.ShowInReport='1' AND T_Question.LanguageId='2' Order by T_Question.OrderTag", Qconnection);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        if (listOfResponseTypeQId.Contains(dr["QType"].ToString()))
                        {
                            int Attribute_Count = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["QDesc3"].ToString());

                            listOfMSQuestion.Add(dr["QId"].ToString());

                            for (int i = 1; i <= Attribute_Count; i++)
                            {
                                columnName.Add(dr["QId"].ToString() + "__" + i.ToString());
                            }
                        }
                        else
                        {
                            columnName.Add(dr["QId"].ToString());
                        }

                    }

                }
                columnName.Add("Intv_Type");
                columnName.Add("FICode");
                columnName.Add("AccompaniedBy");
                columnName.Add("BackCheckedBy");
                columnName.Add("Status");
                columnName.Add("TabId");

                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private int getAttributeNumber(string ProjectId, string QId, string QDesc3)
        {
            if (QDesc3 != "")
                QId = QDesc3;

            SQLiteDataAdapter dadpt1 = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute where ProjectId=" + ProjectId + " AND QId ='" + QId + "' AND LanguageId='2'", Qconnection);
            DataSet ds = new DataSet();
            dadpt1.Fill(ds, "Table1");
            return ds.Tables["Table1"].Rows.Count;
        }

        public List<string> getTableColumnReport()
        {
            try
            {
                List<string> columnName = new List<string>();

                columnName.Add("RespondentId");
                //columnName.Add("MobileNo");
                //columnName.Add("RespondentName");
                columnName.Add("Latitude");
                columnName.Add("Longitude");
                columnName.Add("SurveyDateTime");
                columnName.Add("SurveyEndTime");
                columnName.Add("LengthOfIntv");

                this.populateListForResponseType();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.ProjectId, T_Question.QId, T_Question.QDesc3, T_Question.QType FROM T_Question INNER JOIN T_QType ON T_Question.QType = T_QType.ForQuesLink WHERE T_QType.ShowInReport='1' AND T_Question.LanguageId='2' Order by T_Question.OrderTag", Qconnection);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        if (listOfResponseTypeQId.Contains(dr["QType"].ToString()))
                        {
                            int Attribute_Count = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["QDesc3"].ToString());

                            listOfMSQuestion.Add(dr["QId"].ToString());

                            for (int i = 1; i <= Attribute_Count; i++)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + i.ToString());
                            }
                        }
                        else
                        {
                            columnName.Add(dr["QId"].ToString());
                        }

                    }

                }
                columnName.Add("Intv_Type");
                columnName.Add("FICode");
                columnName.Add("FSCode");
                columnName.Add("AccompaniedBy");
                columnName.Add("BackCheckedBy");
                columnName.Add("ScriptVersion");
                columnName.Add("SyncDateTime");
                columnName.Add("Status");
                columnName.Add("TabId");

                return columnName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public List<List<string>> getTableDataReport(List<string> columnName, DataTable dtTInterviewInfo, DataTable dtTRespAnswer, DataTable dtTRespOpenended, ProgressBar myProgressBar)
        {
            //try
            //{
            List<string> columnData = new List<string>();
            List<List<string>> listOfColumnData = new List<List<string>>();
            SQLiteDataAdapter dadpt;

            Dictionary<string, string> dicFieldNameResponse = new Dictionary<string, string>();

            Dictionary<string, string> dicFieldNameOpenResponse = new Dictionary<string, string>();

            string priorRespId = "0";
            string priorAutoId = "0";

            DataTable TInterviewInfo = dtTInterviewInfo.Clone();
            TInterviewInfo.Columns["RespondentId"].DataType = typeof(Int64);

            DataTable TRespAnswer = dtTRespAnswer.Clone();
            TRespAnswer.Columns["RespondentId"].DataType = typeof(Int64);
            TRespAnswer.Columns["rOrderTag"].DataType = typeof(Int64);
            TRespAnswer.Columns["qOrderTag"].DataType = typeof(Int64);

            foreach (DataRow row in dtTInterviewInfo.Rows)
            {
                TInterviewInfo.ImportRow(row);
            }
            foreach (DataRow row in dtTRespAnswer.Rows)
            {
                TRespAnswer.ImportRow(row);
            }

            var result = from T_InterviewInfo in TInterviewInfo.AsEnumerable()
                         join T_RespAnswer in TRespAnswer.AsEnumerable() on T_InterviewInfo.Field<Int64>("RespondentId") equals T_RespAnswer.Field<Int64>("RespondentId")
                         orderby (Int64)T_InterviewInfo["RespondentId"], (Int64)T_RespAnswer["qOrderTag"], (Int64)T_RespAnswer["rOrderTag"]
                         select new
                         {
                             AutoId = (Int64)T_InterviewInfo["RespondentId"],
                             RespondentId = (Int64)T_InterviewInfo["RespondentId"],
                             Latitude = (string)T_InterviewInfo["Latitude"],
                             Longitude = (string)T_InterviewInfo["Longitude"],
                             SurveyDateTime = T_InterviewInfo["SurveyDateTime"].ToString(),
                             SurveyEndTime = T_InterviewInfo["SurveyEndTime"].ToString(),
                             LengthOfIntv = (string)T_InterviewInfo["LengthOfIntv"],
                             intv_type = (string)T_InterviewInfo["Intv_Type"],
                             FICode = (string)T_InterviewInfo["FICode"],
                             FSCode = (string)T_InterviewInfo["FSCode"],
                             AccompaniedBy = (string)T_InterviewInfo["AccompaniedBy"],
                             BackCheckedBy = (string)T_InterviewInfo["BackCheckedBy"],
                             ScriptVersion = (string)T_InterviewInfo["ScriptVersion"],
                             SyncDataTime = T_InterviewInfo["SurveyDateTime"].ToString(),
                             status = (string)T_InterviewInfo["Status"],
                             TabId = T_InterviewInfo["TabId"],
                             QId = (string)T_RespAnswer["QId"],
                             Response = (string)T_RespAnswer["Response"],
                             qElapsedTime = T_RespAnswer["qElapsedTime"],
                             rOrderTag = ((Int64)T_RespAnswer["rOrderTag"]).ToString()
                         };


            //var result = from T_InterviewInfo in TInterviewInfo.AsEnumerable()
            //             join T_RespAnswer in TRespAnswer.AsEnumerable() on T_InterviewInfo.Field<Int64>("RespondentId") equals T_RespAnswer.Field<Int64>("RespondentId")
            //             orderby (Int64)T_InterviewInfo["RespondentId"], (Int64)T_RespAnswer["qOrderTag"], (Int64)T_RespAnswer["rOrderTag"]
            //             select new
            //             {
            //                 AutoId = (Int64)T_InterviewInfo["RespondentId"],
            //                 RespondentId = (string)T_InterviewInfo["RespondentId"],
            //                 Latitude = (string)T_InterviewInfo["Latitude"],
            //                 Longitude = (string)T_InterviewInfo["Longitude"],
            //                 SurveyDateTime = (string)T_InterviewInfo["SurveyDateTime"],
            //                 SurveyEndTime = (string)T_InterviewInfo["SurveyEndTime"],
            //                 LengthOfIntv = (string)T_InterviewInfo["LengthOfIntv"],
            //                 intv_type = (string)T_InterviewInfo["Intv_Type"],
            //                 FICode = (string)T_InterviewInfo["FICode"],
            //                 FSCode = (string)T_InterviewInfo["FSCode"],
            //                 AccompaniedBy = (string)T_InterviewInfo["AccompaniedBy"],
            //                 BackCheckedBy = (string)T_InterviewInfo["BackCheckedBy"],
            //                 ScriptVersion = (string)T_InterviewInfo["ScriptVersion"],
            //                 SyncDataTime = (string)T_InterviewInfo["SurveyDateTime"],
            //                 status = (string)T_InterviewInfo["Status"],
            //                 TabId = (string)T_InterviewInfo["TabId"],
            //                 QId = (string)T_RespAnswer["QId"],
            //                 Response = (string)T_RespAnswer["Response"],
            //                 qElapsedTime = (string)T_RespAnswer["qElapsedTime"],
            //                 rOrderTag = ((Int64)T_RespAnswer["rOrderTag"]).ToString()
            //             };
            //DataSet ds = new DataSet();
            //dadpt.Fill(ds, "Table1");

            //if (result.Count<.Rows.Count > 0)
            //{
            long noOfRow = result.Count();

            myProgressBar.Minimum = 0;
            myProgressBar.Maximum = TInterviewInfo.Rows.Count * TRespAnswer.Rows.Count;
            int p = 0;

            foreach (var dr in result)
            {
                p++;
                myProgressBar.Value = p;
                //if (dr["RespondentId"].ToString()=="5")
                //{
                //    MessageBox.Show("");
                //}
                //if (priorRespId != dr.RespondentId.ToString())
                if (priorAutoId != dr.AutoId.ToString())
                {
                    //if (priorRespId != "0")
                    if (priorAutoId != "0")
                    {
                        //Update the response with openended value

                        //This is only for taking openended data 

                        ////////////////////Dictionary<String, String> dicOpenendedQIDvsResponse = getOpenendedForReport(priorAutoId, dtTRespOpenended);

                        ////////////////////if (dicOpenendedQIDvsResponse.Count > 0)
                        ////////////////////{
                        ////////////////////    foreach (KeyValuePair<string, string> pair in dicOpenendedQIDvsResponse)
                        ////////////////////    {
                        ////////////////////        if (dicFieldNameResponse.ContainsKey(pair.Key))
                        ////////////////////        {
                        ////////////////////            string s_temp = dicFieldNameResponse[pair.Key] + pair.Value;
                        ////////////////////            dicFieldNameResponse.Remove(pair.Key);
                        ////////////////////            dicFieldNameResponse.Add(pair.Key, s_temp);
                        ////////////////////        }
                        ////////////////////    }
                        ////////////////////}

                        //**********************************************



                        for (int i = 0; i < columnName.Count; i++)
                        {
                            if (dicFieldNameResponse.ContainsKey(columnName[i]))
                                columnData.Add(dicFieldNameResponse[columnName[i]]);
                            else
                                columnData.Add("");
                        }

                        listOfColumnData.Add(columnData);
                    }
                    columnData = new List<string>();

                    dicFieldNameResponse.Clear();


                    //priorRespId = dr.RespondentId.ToString();
                    priorAutoId = dr.AutoId.ToString();

                    dicFieldNameResponse.Add("RespondentId", dr.RespondentId.ToString());
                    //dicFieldNameResponse.Add("MobileNo", dr.MobileNo.ToString());
                    //dicFieldNameResponse.Add("RespondentName", dr.RespondentName.ToString());
                    //dicFieldNameResponse.Add("Centre", dr.Centre.ToString());
                    dicFieldNameResponse.Add("Latitude", dr.Latitude.ToString());
                    dicFieldNameResponse.Add("Longitude", dr.Longitude.ToString());
                    dicFieldNameResponse.Add("SurveyDateTime", dr.SurveyDateTime.ToString());
                    dicFieldNameResponse.Add("SurveyEndTime", dr.SurveyEndTime.ToString());
                    dicFieldNameResponse.Add("LengthOfIntv", dr.LengthOfIntv.ToString());
                    dicFieldNameResponse.Add("FICode", dr.FICode.ToString());
                    dicFieldNameResponse.Add("FSCode", dr.FSCode.ToString());
                    dicFieldNameResponse.Add("AccompaniedBy", dr.AccompaniedBy.ToString());
                    dicFieldNameResponse.Add("BackCheckedBy", dr.BackCheckedBy.ToString());
                    dicFieldNameResponse.Add("ScriptVersion", dr.ScriptVersion.ToString());
                    dicFieldNameResponse.Add("SyncDateTime", dr.SyncDataTime.ToString());

                    dicFieldNameResponse.Add("Intv_Type", dr.intv_type.ToString());
                    dicFieldNameResponse.Add("Status", dr.status.ToString());
                    dicFieldNameResponse.Add("TabId", dr.TabId.ToString());

                    dicFieldNameResponse.Add(dr.QId.ToString(), dr.Response.ToString());
                }
                else
                {
                    if (!listOfMSQuestion.Contains(dr.QId.ToString()))
                    {
                        if (dicFieldNameResponse.ContainsKey(dr.QId.ToString()) == false)
                            dicFieldNameResponse.Add(dr.QId.ToString(), dr.Response.ToString());
                        else
                        {
                            ////if (dr.QId.ToString() == "Q3a")
                            ////{
                            ////    string s_temp = dicFieldNameResponse[dr.QId.ToString()] + " ; " + dr.Response.ToString();
                            ////    dicFieldNameResponse.Remove(dr.QId.ToString());
                            ////    dicFieldNameResponse.Add(dr.QId.ToString(), s_temp);
                            ////}
                            ////else
                            ////{
                            //string s_temp = dicFieldNameResponse[dr.QId.ToString()] + dr.Response.ToString();
                            //dicFieldNameResponse.Remove(dr.QId.ToString());
                            //dicFieldNameResponse.Add(dr.QId.ToString(), s_temp);
                            ////}


                            //Take only single one response
                            dicFieldNameResponse.Remove(dr.QId.ToString());
                            dicFieldNameResponse.Add(dr.QId.ToString(), dr.Response.ToString());
                        }

                    }
                    else
                    {
                        //T_RespAnswer.rOrderTag
                        if (dicFieldNameResponse.ContainsKey(dr.QId.ToString() + "_" + dr.rOrderTag.ToString()) == false)
                            dicFieldNameResponse.Add(dr.QId.ToString() + "_" + dr.rOrderTag.ToString(), dr.Response.ToString());
                    }
                }
            }

            //This is only for taking openended data 

            ////////////////////////Dictionary<String, String> dicOpenendedQIDvsResponse2 = getOpenendedForReport(priorAutoId, dtTRespOpenended);

            ////////////////////////if (dicOpenendedQIDvsResponse2.Count > 0)
            ////////////////////////{
            ////////////////////////    foreach (KeyValuePair<string, string> pair in dicOpenendedQIDvsResponse2)
            ////////////////////////    {
            ////////////////////////        if (dicFieldNameResponse.ContainsKey(pair.Key))
            ////////////////////////        {
            ////////////////////////            string s_temp = dicFieldNameResponse[pair.Key] + pair.Value;
            ////////////////////////            dicFieldNameResponse.Remove(pair.Key);
            ////////////////////////            dicFieldNameResponse.Add(pair.Key, s_temp);
            ////////////////////////        }
            ////////////////////////    }
            ////////////////////////}

            //This is for the last respondent;
            for (int i = 0; i < columnName.Count; i++)
            {
                if (dicFieldNameResponse.ContainsKey(columnName[i]))
                    columnData.Add(dicFieldNameResponse[columnName[i]]);
                else
                    columnData.Add("");
            }

            listOfColumnData.Add(columnData);
            //}
            return listOfColumnData;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return null;
            //}
        }

        private Dictionary<String, String> getOpenendedForReport(string AutoId, DataTable dtTRespOpenended)
        {
            Dictionary<String, String> dicOpenendedQIDvsResponse = new Dictionary<String, String>();

            //SQLiteDataAdapter dadpt = new SQLiteDataAdapter(@"SELECT * FROM T_RespOpenended WHERE  RespondentId=" + RespondentId + " AND OEResponseType='2';", Aconnection);

            var result1 = from T_RespOpenended in dtTRespOpenended.AsEnumerable()
                          where T_RespOpenended.Field<string>("RespondentId") == AutoId && T_RespOpenended.Field<string>("OEResponseType") == "2"
                          select new
                          {
                              RespondentId = (string)T_RespOpenended["RespondentId"],
                              QId = (string)T_RespOpenended["QId"],
                              AttributeValue = (string)T_RespOpenended["AttributeValue"],
                              OpenendedResp = (string)T_RespOpenended["OpenendedResp"],
                              OEResponseType = (string)T_RespOpenended["OEResponseType"]
                          };


            foreach (var dr in result1)
            {
                if (!dicOpenendedQIDvsResponse.ContainsKey(dr.QId.ToString()))
                {
                    dicOpenendedQIDvsResponse.Add(dr.QId.ToString(), dr.OpenendedResp.ToString());
                }
                else
                {
                    string s_temp = dicOpenendedQIDvsResponse[dr.QId.ToString()] + dr.OpenendedResp.ToString();
                    dicOpenendedQIDvsResponse.Remove(dr.QId.ToString());
                    dicOpenendedQIDvsResponse.Add(dr.QId.ToString(), s_temp);
                }
            }

            return dicOpenendedQIDvsResponse;
        }

        //public SQLiteDataReader getDataTableOpenended()
        //{
        //    //DataTable dt = new DataTable();
        //    SQLiteCommand cmd1 = new SQLiteCommand("SELECT * FROM T_RespOpenended", Aconnection);
        //    SQLiteDataReader drd = cmd1.ExecuteReader();

        //    // dt.Load(drd);
        //    return drd;
        //}

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

        private void populateListForResponseType()
        {
            listOfResponseTypeQId.Clear();
            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_QType", Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    if (dr["ResponseType"].ToString() == "2")
                    {
                        listOfResponseTypeQId.Add(dr["ID"].ToString());
                    }
                }
            }
        }
    }
}
