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
    class SQLite
    {
        private string QdbConnString;
        private string AdbConnString;

        public SQLiteConnection Qconnection;

        private List<string> listOfMSQuestion = new List<string>();
        private List<string> listOfSRQuestion = new List<string>();
        private List<String> listOfResponseTypeQId = new List<String>();
        private List<String> listOfMRGridQId = new List<String>();
        private List<String> listOfFormQId = new List<String>();
        private List<String> listOfResponseTypeQIdMaxDiff = new List<String>();
        public SQLite(string Qdb)
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
            //try
            //{
            List<string> columnName = new List<string>();

            columnName.Add("Id");
            columnName.Add("RespondentId");
            columnName.Add("name_resp");
            columnName.Add("mobile_resp");
            columnName.Add("Latitude");
            columnName.Add("Longitude");
            columnName.Add("SurveyDateTime");

            this.populateListForResponseType();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.ProjectId, T_Question.QId, T_Question.AttributeId, T_Question.QType FROM T_Question INNER JOIN T_QType ON T_Question.QType = T_QType.ForQuesLink WHERE T_QType.ShowInReport='1' Order by T_Question.OrderTag", Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    if (listOfResponseTypeQId.Contains(dr["QType"].ToString()) || dr["QId"].ToString() == "FIFSInfo")
                    {

                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());

                        listOfMSQuestion.Add(dr["QId"].ToString());

                        if (dr["QId"].ToString() == "FIFSInfo")
                        {
                            columnName.Add("FIFSInfo_1");
                            columnName.Add("FIFSInfo_2");
                            columnName.Add("FIFSInfo_3");
                            columnName.Add("FIFSInfo_4");
                        }
                        else if (dr["QType"].ToString() == "41")
                        {
                            columnName.Add(dr["QId"].ToString() + "_1");
                            columnName.Add(dr["QId"].ToString() + "_2");
                        }
                        else
                        {
                            foreach (DataRow dr2 in Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                            }
                        }

                    }
                    else if (listOfMRGridQId.Contains(dr["QType"].ToString()))
                    {
                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());



                        //listOfMSQuestion.Add(dr["QId"].ToString());

                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            DataTable Grid_Attribute_Table = getGridAttributeNumber(dr2["ProjectId"].ToString(), dr2["QId"].ToString(), dr2["LinkId2"].ToString());

                            listOfMSQuestion.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());

                            foreach (DataRow dr3 in Grid_Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_" + dr3["AttributeOrder"].ToString());
                            }
                        }
                    }
                    else if (listOfFormQId.Contains(dr["QType"].ToString()))
                    {
                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());



                        //listOfMSQuestion.Add(dr["QId"].ToString());

                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            if (dr2["LinkId1"].ToString() == "1" || dr2["LinkId1"].ToString() == "3" || dr2["LinkId1"].ToString() == "4" || dr2["LinkId1"].ToString() == "14" || dr2["LinkId1"].ToString() == "15" || dr2["LinkId1"].ToString() == "22")
                            {
                                //DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());

                                listOfMSQuestion.Add(dr["QId"].ToString());

                                //foreach (DataRow dr2 in Attribute_Table.Rows)
                                //{
                                    columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                                //}
                            }
                            else if (dr2["LinkId1"].ToString() == "2")
                            {
                                DataTable Grid_Attribute_Table = getGridAttributeNumber(dr2["ProjectId"].ToString(), dr2["QId"].ToString(), dr2["LinkId2"].ToString());

                                listOfMSQuestion.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());

                                foreach (DataRow dr3 in Grid_Attribute_Table.Rows)
                                {
                                    columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_" + dr3["AttributeOrder"].ToString());
                                }
                            }
                            else if (dr2["LinkId1"].ToString() == "22")
                            {
                                DataTable Grid_Attribute_Table = getGridAttributeNumber(dr2["ProjectId"].ToString(), dr2["QId"].ToString(), dr2["LinkId2"].ToString());

                                listOfMSQuestion.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());

                                foreach (DataRow dr3 in Grid_Attribute_Table.Rows)
                                {
                                    columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                                }
                            }

                        }
                    }
                    else if (listOfResponseTypeQIdMaxDiff.Contains(dr["QType"].ToString()))
                    {
                        listOfMSQuestion.Add(dr["QId"].ToString());

                        for (int x = 1; x <= 2; x++)
                        {
                            columnName.Add(dr["QId"].ToString() + "_" + x.ToString());
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
            columnName.Add("field_ex2");
            columnName.Add("TabId");

            return columnName;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return null;
            //}
        }

        private DataTable getAttributeNumber(string ProjectId, string QId, string AttributeId)
        {
            if (AttributeId != "")
                QId = AttributeId;

            SQLiteDataAdapter dadpt1 = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute where ProjectId=" + ProjectId + " AND QId ='" + QId + "'", Qconnection);
            DataSet ds = new DataSet();
            dadpt1.Fill(ds, "Table1");
            return ds.Tables["Table1"];
        }

        private DataTable getGridAttributeNumber(string ProjectId, string QId, string AttributeId)
        {
            if (AttributeId != "")
                QId = AttributeId;

            SQLiteDataAdapter dadpt1 = new SQLiteDataAdapter("SELECT * FROM T_GridInfo where ProjectId=" + ProjectId + " AND QId ='" + QId + "'", Qconnection);
            DataSet ds = new DataSet();
            dadpt1.Fill(ds, "Table1");
            return ds.Tables["Table1"];
        }

        public List<string> getTableColumnReport()
        {
            //try
            //{
            List<string> columnName = new List<string>();

            columnName.Add("Id");
            columnName.Add("RespondentId");
            columnName.Add("name_resp");
            columnName.Add("mobile_resp");
            columnName.Add("Latitude");
            columnName.Add("Longitude");
            columnName.Add("SurveyDateTime");
            columnName.Add("SurveyEndTime");
            columnName.Add("LengthOfIntv");

            this.populateListForResponseType();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT T_Question.ProjectId, T_Question.QId, T_Question.AttributeId, T_Question.QType FROM T_Question INNER JOIN T_QType ON T_Question.QType = T_QType.ForQuesLink WHERE T_QType.ShowInReport='1' Order by T_Question.OrderTag", Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    if (listOfResponseTypeQId.Contains(dr["QType"].ToString()) || dr["QId"].ToString()=="FIFSInfo")
                    {

                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());

                        listOfMSQuestion.Add(dr["QId"].ToString());

                        if (dr["QId"].ToString() == "FIFSInfo")
                        {
                            columnName.Add("FIFSInfo_1");
                            columnName.Add("FIFSInfo_2");
                            columnName.Add("FIFSInfo_3");
                            columnName.Add("FIFSInfo_4");
                        }
                        else if (dr["QType"].ToString() == "41")
                        {
                            columnName.Add(dr["QId"].ToString() + "_1");
                            columnName.Add(dr["QId"].ToString() + "_2");
                        }
                        else
                        {
                            foreach (DataRow dr2 in Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                            }
                        }
                        
                    }
                    else if (listOfMRGridQId.Contains(dr["QType"].ToString()))
                    {
                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());



                        //listOfMSQuestion.Add(dr["QId"].ToString());

                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            DataTable Grid_Attribute_Table = getGridAttributeNumber(dr2["ProjectId"].ToString(), dr2["QId"].ToString(), dr2["LinkId2"].ToString());

                            listOfMSQuestion.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                            foreach (DataRow dr3 in Grid_Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_" + dr3["AttributeOrder"].ToString());
                            }
                        }
                    }
                    else if (listOfFormQId.Contains(dr["QType"].ToString()))
                    {
                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());



                        //listOfMSQuestion.Add(dr["QId"].ToString());

                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            if (dr2["LinkId1"].ToString() == "1" || dr2["LinkId1"].ToString() == "3" || dr2["LinkId1"].ToString() == "4" || dr2["LinkId1"].ToString() == "14" || dr2["LinkId1"].ToString() == "15" || dr2["LinkId1"].ToString() == "22")
                            {
                                //DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());

                                listOfMSQuestion.Add(dr["QId"].ToString());

                                //foreach (DataRow dr2 in Attribute_Table.Rows)
                                //{
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                                //}
                            }
                            else if (dr2["LinkId1"].ToString() == "2")
                            {
                                DataTable Grid_Attribute_Table = getGridAttributeNumber(dr2["ProjectId"].ToString(), dr2["QId"].ToString(), dr2["LinkId2"].ToString());

                                listOfMSQuestion.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());

                                foreach (DataRow dr3 in Grid_Attribute_Table.Rows)
                                {
                                    columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_" + dr3["AttributeOrder"].ToString());
                                }
                            }

                        }
                    }
                    else if (listOfResponseTypeQIdMaxDiff.Contains(dr["QType"].ToString()))
                    {
                        listOfMSQuestion.Add(dr["QId"].ToString());

                        for (int x = 1; x <= 2; x++)
                        {
                            columnName.Add(dr["QId"].ToString() + "_" + x.ToString());
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
            columnName.Add("field_ex2");
            columnName.Add("TabId");

            return columnName;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return null;
            //}
        }

        public List<List<string>> getTableDataReport(List<string> columnName, DataTable dtTInterviewInfo, DataTable dtTRespAnswer, DataTable dtTRespOpenended, ProgressBar myProgressBar)
        {
            try
            {
                List<string> columnData = new List<string>();
                List<List<string>> listOfColumnData = new List<List<string>>();
                SQLiteDataAdapter dadpt;

                Dictionary<string, string> dicFieldNameResponse = new Dictionary<string, string>();

                Dictionary<string, string> dicFieldNameOpenResponse = new Dictionary<string, string>();

                string priorRespId = "0";
                string priorAutoId = "0";

                //                if (TypeOfReport == "1")
                //                    dadpt = new SQLiteDataAdapter(@"SELECT T_InterviewInfo.RespondentId, T_InterviewInfo.Latitude, T_InterviewInfo.Longitude, T_InterviewInfo.SurveyDateTime, T_InterviewInfo.SurveyEndTime, T_InterviewInfo.LengthOfIntv, T_InterviewInfo.Intv_Type, 
                //                                                  T_InterviewInfo.FICode,T_InterviewInfo.FSCode,T_InterviewInfo.AccompaniedBy, T_InterviewInfo.BackCheckedBy, T_InterviewInfo.Status, T_InterviewInfo.TabId, T_RespAnswer.QId, 
                //                                                  T_RespAnswer.Response, T_RespAnswer.qElapsedTime, T_RespAnswer.rOrderTag FROM T_InterviewInfo 
                //                                                  INNER JOIN T_RespAnswer ON (T_InterviewInfo.ProjectId = T_RespAnswer.ProjectId) AND (T_InterviewInfo.AutoId = T_RespAnswer.IntvInfoId)
                //                                                  WHERE T_InterviewInfo.Intv_Type='1' AND (T_InterviewInfo.Status='1' OR T_InterviewInfo.Status='3') AND T_InterviewInfo.DeletedAt='' ORDER BY T_InterviewInfo.AutoId, T_RespAnswer.qOrderTag, T_RespAnswer.rOrderTag;", Aconnection);
                //                else if (TypeOfReport == "2")
                //                    dadpt = new SQLiteDataAdapter(@"SELECT T_InterviewInfo.RespondentId, T_InterviewInfo.Latitude, T_InterviewInfo.Longitude, T_InterviewInfo.SurveyDateTime, T_InterviewInfo.SurveyEndTime, T_InterviewInfo.LengthOfIntv, T_InterviewInfo.Intv_Type, 
                //                                                  T_InterviewInfo.FICode,T_InterviewInfo.FSCode,T_InterviewInfo.AccompaniedBy, T_InterviewInfo.BackCheckedBy, T_InterviewInfo.Status, T_InterviewInfo.TabId, T_RespAnswer.QId, 
                //                                                  T_RespAnswer.Response, T_RespAnswer.qElapsedTime, T_RespAnswer.rOrderTag FROM T_InterviewInfo 
                //                                                  INNER JOIN T_RespAnswer ON (T_InterviewInfo.ProjectId = T_RespAnswer.ProjectId) AND (T_InterviewInfo.AutoId = T_RespAnswer.IntvInfoId)
                //                                                  WHERE T_InterviewInfo.Intv_Type='1' AND T_InterviewInfo.Status='2' AND T_InterviewInfo.DeletedAt='' ORDER BY T_InterviewInfo.AutoId, T_RespAnswer.qOrderTag, T_RespAnswer.rOrderTag;", Aconnection);
                //                else if (TypeOfReport == "3")
                //                    dadpt = new SQLiteDataAdapter(@"SELECT T_InterviewInfo.RespondentId, T_InterviewInfo.Latitude, T_InterviewInfo.Longitude, T_InterviewInfo.SurveyDateTime, T_InterviewInfo.SurveyEndTime, T_InterviewInfo.LengthOfIntv, T_InterviewInfo.Intv_Type, 
                //                                                  T_InterviewInfo.FICode,T_InterviewInfo.FSCode,T_InterviewInfo.AccompaniedBy, T_InterviewInfo.BackCheckedBy, T_InterviewInfo.Status, T_InterviewInfo.TabId, T_RespAnswer.QId, 
                //                                                  T_RespAnswer.Response, T_RespAnswer.qElapsedTime, T_RespAnswer.rOrderTag FROM T_InterviewInfo 
                //                                                  INNER JOIN T_RespAnswer ON (T_InterviewInfo.ProjectId = T_RespAnswer.ProjectId) AND (T_InterviewInfo.AutoId = T_RespAnswer.IntvInfoId)
                //                                                  WHERE T_InterviewInfo.Intv_Type='1' AND T_InterviewInfo.Status='2' AND T_InterviewInfo.DeletedAt='' ORDER BY T_InterviewInfo.AutoId, T_RespAnswer.qOrderTag, T_RespAnswer.rOrderTag;", Aconnection);
                //                else
                //                    dadpt = new SQLiteDataAdapter(@"SELECT T_InterviewInfo.RespondentId, T_InterviewInfo.Latitude, T_InterviewInfo.Longitude, T_InterviewInfo.SurveyDateTime, T_InterviewInfo.SurveyEndTime, T_InterviewInfo.LengthOfIntv, T_InterviewInfo.Intv_Type, 
                //                                                  T_InterviewInfo.FICode,T_InterviewInfo.FSCode,T_InterviewInfo.AccompaniedBy, T_InterviewInfo.BackCheckedBy, T_InterviewInfo.Status, T_InterviewInfo.TabId, T_RespAnswer.QId, 
                //                                                  T_RespAnswer.Response, T_RespAnswer.qElapsedTime, T_RespAnswer.rOrderTag FROM T_InterviewInfo 
                //                                                  INNER JOIN T_RespAnswer ON (T_InterviewInfo.ProjectId = T_RespAnswer.ProjectId) AND (T_InterviewInfo.AutoId = T_RespAnswer.IntvInfoId)
                //                                                  WHERE T_InterviewInfo.Intv_Type='1' AND (T_InterviewInfo.Status='1' OR T_InterviewInfo.Status='3') AND T_InterviewInfo.DeletedAt='' ORDER BY T_InterviewInfo.AutoId, T_RespAnswer.qOrderTag, T_RespAnswer.rOrderTag;", Aconnection);


                //TInterviewInfo.Columns[0].DataType = typeof(Int64);
                //TRespAnswer.Columns[1].DataType = typeof(Int64);
                //TRespAnswer.Columns[0].DataType = typeof(Int64);
                //TRespAnswer.Columns[0].DataType = typeof(Int64);

                DataTable TInterviewInfo = dtTInterviewInfo.Clone();
                TInterviewInfo.Columns["id"].DataType = typeof(Int64);

                DataTable TRespAnswer = dtTRespAnswer.Clone();
                TRespAnswer.Columns["interview_info_id"].DataType = typeof(Int64);
                TRespAnswer.Columns["resp_order"].DataType = typeof(Int64);
                TRespAnswer.Columns["q_order"].DataType = typeof(Int64);

                foreach (DataRow row in dtTInterviewInfo.Rows)
                {
                    TInterviewInfo.ImportRow(row);
                }
                foreach (DataRow row in dtTRespAnswer.Rows)
                {
                    TRespAnswer.ImportRow(row);
                }

                var result = from T_InterviewInfo in TInterviewInfo.AsEnumerable()
                             join T_RespAnswer in TRespAnswer.AsEnumerable() on T_InterviewInfo.Field<Int64>("id") equals T_RespAnswer.Field<Int64>("interview_info_id")
                             orderby (Int64)T_InterviewInfo["id"], (Int64)T_RespAnswer["q_order"], (Int64)T_RespAnswer["resp_order"]
                             select new
                             {
                                 AutoId = (Int64)T_InterviewInfo["id"],
                                 RespondentId = (string)T_InterviewInfo["respondent_id"],

                                 name_resp = (string)T_InterviewInfo["name_resp"],
                                 mobile_resp = (string)T_InterviewInfo["mobile_resp"],

                                 Latitude = (string)T_InterviewInfo["latitude"],
                                 Longitude = (string)T_InterviewInfo["longitude"],
                                 SurveyDateTime = (string)T_InterviewInfo["survey_start_at"],
                                 SurveyEndTime = (string)T_InterviewInfo["survey_end_at"],
                                 LengthOfIntv = (string)T_InterviewInfo["length_of_intv"],
                                 intv_type = (string)T_InterviewInfo["intv_type"],
                                 FICode = (string)T_InterviewInfo["fi_code"],
                                 FSCode = (string)T_InterviewInfo["fs_code"],
                                 AccompaniedBy = (string)T_InterviewInfo["accompanied_by"],
                                 BackCheckedBy = (string)T_InterviewInfo["back_checked_by"],
                                 ScriptVersion = (string)T_InterviewInfo["script_version"],
                                 SyncDataTime = (string)T_InterviewInfo["created_at"],
                                 status = (string)T_InterviewInfo["status"],
                                 field_ex2 = (string)T_InterviewInfo["field_ex2"],
                                 TabId = (string)T_InterviewInfo["tab_id"],
                                 QId = (string)T_RespAnswer["q_id"],
                                 Response = (string)T_RespAnswer["response"],
                                 qElapsedTime = (string)T_RespAnswer["q_elapsed_time"],
                                 rOrderTag = ((Int64)T_RespAnswer["resp_order"]).ToString()
                             };


                //DataSet ds = new DataSet();
                //dadpt.Fill(ds, "Table1");

                //if (result.Count<.Rows.Count > 0)
                //{


                //myProgressBar.Minimum = 0;
                //myProgressBar.Maximum = TInterviewInfo.Rows.Count * TRespAnswer.Rows.Count;
                int p = 0;
                foreach (var dr in result)
                {
                    p++;
                    //myProgressBar.Value = p;

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

                        dicFieldNameResponse.Add("Id", dr.AutoId.ToString());
                        dicFieldNameResponse.Add("RespondentId", dr.RespondentId.ToString());
                        dicFieldNameResponse.Add("name_resp", dr.name_resp.ToString());
                        dicFieldNameResponse.Add("mobile_resp", dr.mobile_resp.ToString());
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
                        //dicFieldNameResponse.Add(dr.QId.ToString(), dr.qElapsedTime.ToString());


                        /// This is for if first question is a form question
                        if (!listOfMSQuestion.Contains(dr.QId.ToString()))
                        {
                            if (dicFieldNameResponse.ContainsKey(dr.QId.ToString()) == false)
                                dicFieldNameResponse.Add(dr.QId.ToString(), dr.Response.ToString());
                        }
                        else
                        {
                            if (dicFieldNameResponse.ContainsKey(dr.QId.ToString() + "_" + dr.rOrderTag.ToString()) == false)
                                dicFieldNameResponse.Add(dr.QId.ToString() + "_" + dr.rOrderTag.ToString(), dr.Response.ToString());
                        }
                    }
                    else
                    {
                        if (!listOfMSQuestion.Contains(dr.QId.ToString()))
                        {
                            if (dicFieldNameResponse.ContainsKey(dr.QId.ToString()) == false)
                                dicFieldNameResponse.Add(dr.QId.ToString(), dr.Response.ToString());
                            else
                            {
                                //if (dr.QId.ToString() == "Q3a")
                                //{
                                //    string s_temp = dicFieldNameResponse[dr.QId.ToString()] + " ; " + dr.Response.ToString();
                                //    dicFieldNameResponse.Remove(dr.QId.ToString());
                                //    dicFieldNameResponse.Add(dr.QId.ToString(), s_temp);
                                //}
                                //else
                                //{
                                if (dicFieldNameResponse[dr.QId.ToString()] != dr.Response.ToString())  //If redundent data exist (that is error)
                                {
                                    string s_temp = dicFieldNameResponse[dr.QId.ToString()] + dr.Response.ToString();
                                    dicFieldNameResponse.Remove(dr.QId.ToString());
                                    dicFieldNameResponse.Add(dr.QId.ToString(), s_temp);
                                }
                                //}
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
                Qconnection.Close();
                return listOfColumnData;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        private Dictionary<String, String> getOpenendedForReport(string AutoId, DataTable dtTRespOpenended)
        {
            Dictionary<String, String> dicOpenendedQIDvsResponse = new Dictionary<String, String>();

            //SQLiteDataAdapter dadpt = new SQLiteDataAdapter(@"SELECT * FROM T_RespOpenended WHERE  RespondentId=" + RespondentId + " AND OEResponseType='2';", Aconnection);

            var result1 = from T_RespOpenended in dtTRespOpenended.AsEnumerable()
                          where T_RespOpenended.Field<string>("interview_info_id") == AutoId && T_RespOpenended.Field<string>("response_type") == "2"
                          select new
                          {
                              RespondentId = (string)T_RespOpenended["respondent_id"],
                              QId = (string)T_RespOpenended["q_id"],
                              AttributeValue = (string)T_RespOpenended["attribute_value"],
                              OpenendedResp = (string)T_RespOpenended["response"],
                              OEResponseType = (string)T_RespOpenended["response_type"]
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
            listOfSRQuestion.Clear();
            listOfMRGridQId.Clear();
            listOfFormQId.Clear();
            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_QType", Qconnection);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    if (dr["ResponseType"].ToString() == "2")
                    {
                        // 2 means multiple response
                        listOfResponseTypeQId.Add(dr["ID"].ToString());
                    }
                    else if (dr["ResponseType"].ToString() == "3")
                    {
                        // 3 means Multiple response Grid
                        listOfMRGridQId.Add(dr["ID"].ToString());
                    }
                    else if (dr["ResponseType"].ToString() == "1")
                    {
                        // 1 means Single Response
                        listOfSRQuestion.Add(dr["ID"].ToString());
                    }
                    else if (dr["ResponseType"].ToString() == "4")
                    {
                        // 4 means maxdiff
                        listOfResponseTypeQIdMaxDiff.Add(dr["ID"].ToString());
                    }
                    else if (dr["ResponseType"].ToString() == "5")
                    {
                        // 5 means Form Type Response
                        listOfFormQId.Add(dr["ID"].ToString());
                    }

                }
            }
        }
    }
}
