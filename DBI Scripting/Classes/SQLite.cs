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

        private HashSet<string> listOfMSQuestion = new HashSet<string>();
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
                            List<String> listOfOE = new List<String>();

                            foreach (DataRow dr2 in Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                                if (dr2["TakeOpenended"].ToString() != "")
                                    listOfOE.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_OE");
                            }

                            if (listOfOE.Count > 0)
                            {
                                for (int x = 0; x < listOfOE.Count; x++)
                                    columnName.Add(listOfOE[x]);
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
                            if (dr2["LinkId1"].ToString() == "1" || dr2["LinkId1"].ToString() == "3" || dr2["LinkId1"].ToString() == "4" || dr2["LinkId1"].ToString() == "14" || dr2["LinkId1"].ToString() == "15" || dr2["LinkId1"].ToString() == "22" || dr2["LinkId1"].ToString() == "24")
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

                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());
                        
                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            if (dr2["TakeOpenended"].ToString() != "")
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_OE");
                        }

                    }

                }

            }
            columnName.Add("Intv_Type");
            columnName.Add("FICode");
            columnName.Add("AccompaniedBy");
            columnName.Add("BackCheckedBy");
            columnName.Add("Status");
            columnName.Add("field_ex2");
            columnName.Add("intv_info9");
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

            return lookupRows("T_OptAttribute", ref optAttributeCache, ProjectId, QId);
        }

        private DataTable getGridAttributeNumber(string ProjectId, string QId, string AttributeId)
        {
            if (AttributeId != "")
                QId = AttributeId;

            return lookupRows("T_GridInfo", ref gridInfoCache, ProjectId, QId);
        }

        // T_OptAttribute / T_GridInfo have no indexes, so one query per question meant a full
        // table scan per question. Each table is now read once and grouped by (ProjectId, QId),
        // keeping the table's row order — the same rows, in the same order, as the old queries.
        private Dictionary<string, DataTable> optAttributeCache, gridInfoCache;

        private DataTable lookupRows(string table, ref Dictionary<string, DataTable> cache, string projectId, string qId)
        {
            if (cache == null)
            {
                var all = new DataTable();
                using (var dadpt = new SQLiteDataAdapter("SELECT * FROM " + table, Qconnection))
                    dadpt.Fill(all);

                cache = new Dictionary<string, DataTable>();
                cache[""] = all.Clone();   // shared empty result
                DataColumn cProject = all.Columns["ProjectId"], cQId = all.Columns["QId"];
                foreach (DataRow r in all.Rows)
                {
                    string key = Convert.ToString(r[cProject]) + "\u0001" + Convert.ToString(r[cQId]);
                    DataTable group;
                    if (!cache.TryGetValue(key, out group))
                        cache.Add(key, group = all.Clone());
                    group.ImportRow(r);
                }
            }

            DataTable rows;
            return cache.TryGetValue(projectId.Trim() + "\u0001" + qId, out rows) ? rows : cache[""];
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
                            List<String> listOfOE = new List<String>();

                            foreach (DataRow dr2 in Attribute_Table.Rows)
                            {
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString());
                                if (dr2["TakeOpenended"].ToString() != "")
                                    listOfOE.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_OE");
                            }

                            if (listOfOE.Count > 0)
                            {
                                for (int x = 0; x < listOfOE.Count; x++)
                                    columnName.Add(listOfOE[x]);
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
                            if (dr2["LinkId1"].ToString() == "1" || dr2["LinkId1"].ToString() == "3" || dr2["LinkId1"].ToString() == "4" || dr2["LinkId1"].ToString() == "14" || dr2["LinkId1"].ToString() == "15" || dr2["LinkId1"].ToString() == "22" || dr2["LinkId1"].ToString() == "24")
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
                        DataTable Attribute_Table = getAttributeNumber(dr["ProjectId"].ToString(), dr["QId"].ToString(), dr["AttributeId"].ToString());

                        foreach (DataRow dr2 in Attribute_Table.Rows)
                        {
                            if (dr2["TakeOpenended"].ToString() != "")
                                columnName.Add(dr["QId"].ToString() + "_" + dr2["AttributeOrder"].ToString() + "_OE");
                        }
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
            columnName.Add("intv_info9");
            columnName.Add("TabId");

            return columnName;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return null;
            //}
        }

        /// <summary>Message of the last failure inside getTableDataReport (which returns null on failure).</summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Builds one output row per interview (respondents INNER JOIN answers, ordered by
        /// interview id, q_order, resp_order), plus open-ended answers in the *_OE columns.
        /// Answers and open-ended rows are grouped by interview once up front, so the cost is
        /// linear in the data size instead of (interviews x open-ended rows).
        /// Returns null on failure; see LastError.
        /// </summary>
        public List<List<string>> getTableDataReport(List<string> columnName, DataTable dtTInterviewInfo, DataTable dtTRespAnswer, DataTable dtTRespOpenended, ProgressBar myProgressBar)
        {
            LastError = null;
            try
            {
                return buildReportRows(columnName, dtTInterviewInfo, dtTRespAnswer, dtTRespOpenended);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return null;
            }
            finally
            {
                Qconnection.Close();
            }
        }

        private struct ReportAnswer
        {
            public long QOrder, ROrder;
            public int Seq;
            public string QId, Response;
        }

        private List<List<string>> buildReportRows(List<string> columnName, DataTable dtTInterviewInfo, DataTable dtTRespAnswer, DataTable dtTRespOpenended)
        {
            var listOfColumnData = new List<List<string>>();
            if (dtTInterviewInfo == null || dtTRespAnswer == null ||
                dtTInterviewInfo.Rows.Count == 0 || dtTRespAnswer.Rows.Count == 0)
                return listOfColumnData;

            // ---- interviews, ordered by numeric id (first copy wins if the server sent duplicates)
            DataColumnCollection ic = dtTInterviewInfo.Columns;
            DataColumn cId = ic["id"];
            var interviews = new SortedDictionary<long, DataRow>();
            foreach (DataRow r in dtTInterviewInfo.Rows)
            {
                long id = toLong(r, cId);
                if (!interviews.ContainsKey(id)) interviews.Add(id, r);
            }

            // ---- answers grouped by interview id
            DataColumnCollection ac = dtTRespAnswer.Columns;
            DataColumn aIntv = ac["interview_info_id"], aQId = ac["q_id"], aResp = ac["response"],
                       aQOrder = ac["q_order"], aROrder = ac["resp_order"];
            var answersByInterview = new Dictionary<long, List<ReportAnswer>>();
            int seq = 0;
            foreach (DataRow r in dtTRespAnswer.Rows)
            {
                long intvId = toLong(r, aIntv);
                if (!interviews.ContainsKey(intvId)) continue;   // inner join
                List<ReportAnswer> list;
                if (!answersByInterview.TryGetValue(intvId, out list))
                    answersByInterview.Add(intvId, list = new List<ReportAnswer>());
                list.Add(new ReportAnswer
                {
                    QOrder = toLong(r, aQOrder),
                    ROrder = toLong(r, aROrder),
                    Seq = seq++,
                    QId = str(r, aQId),
                    Response = str(r, aResp)
                });
            }

            // ---- open-ended grouped by interview id: "QId_AttributeValue_OE" -> response
            var oeByInterview = new Dictionary<string, Dictionary<string, string>>();
            if (dtTRespOpenended != null && dtTRespOpenended.Rows.Count > 0)
            {
                DataColumnCollection oc = dtTRespOpenended.Columns;
                DataColumn oIntv = oc["interview_info_id"], oQId = oc["q_id"], oAttr = oc["attribute_value"], oResp = oc["response"];
                foreach (DataRow r in dtTRespOpenended.Rows)
                {
                    if (oIntv == null || r.IsNull(oIntv)) continue;
                    string intvKey = Convert.ToString(r[oIntv]);
                    Dictionary<string, string> dic;
                    if (!oeByInterview.TryGetValue(intvKey, out dic))
                        oeByInterview.Add(intvKey, dic = new Dictionary<string, string>());

                    string key = str(r, oQId) + "_" + str(r, oAttr) + "_OE";
                    string prior;
                    // A duplicate is actually an error in the data; keep both texts as before.
                    dic[key] = dic.TryGetValue(key, out prior) ? prior + str(r, oResp) : str(r, oResp);
                }
            }
            var noOpenended = new Dictionary<string, string>();

            // ---- interview-level fields: output name -> source column
            var metaFields = new[]
            {
                new KeyValuePair<string, string>("RespondentId", "respondent_id"),
                new KeyValuePair<string, string>("name_resp", "name_resp"),
                new KeyValuePair<string, string>("mobile_resp", "mobile_resp"),
                new KeyValuePair<string, string>("Latitude", "latitude"),
                new KeyValuePair<string, string>("Longitude", "longitude"),
                new KeyValuePair<string, string>("SurveyDateTime", "survey_start_at"),
                new KeyValuePair<string, string>("SurveyEndTime", "survey_end_at"),
                new KeyValuePair<string, string>("LengthOfIntv", "length_of_intv"),
                new KeyValuePair<string, string>("FICode", "fi_code"),
                new KeyValuePair<string, string>("FSCode", "fs_code"),
                new KeyValuePair<string, string>("AccompaniedBy", "accompanied_by"),
                new KeyValuePair<string, string>("BackCheckedBy", "back_checked_by"),
                new KeyValuePair<string, string>("ScriptVersion", "script_version"),
                new KeyValuePair<string, string>("SyncDateTime", "created_at"),
                new KeyValuePair<string, string>("Intv_Type", "intv_type"),
                new KeyValuePair<string, string>("Status", "status"),
                new KeyValuePair<string, string>("field_ex2", "field_ex2"),
                new KeyValuePair<string, string>("intv_info9", "intv_info9"),
                new KeyValuePair<string, string>("TabId", "tab_id")
            };
            DataColumn[] metaColumns = metaFields.Select(m => ic[m.Value]).ToArray();

            var dicFieldNameResponse = new Dictionary<string, string>();
            foreach (KeyValuePair<long, DataRow> intv in interviews)
            {
                List<ReportAnswer> answers;
                if (!answersByInterview.TryGetValue(intv.Key, out answers)) continue;   // inner join

                answers.Sort((x, y) =>
                {
                    int c = x.QOrder.CompareTo(y.QOrder);
                    if (c == 0) c = x.ROrder.CompareTo(y.ROrder);
                    return c != 0 ? c : x.Seq.CompareTo(y.Seq);   // stable, like the old LINQ orderby
                });

                dicFieldNameResponse.Clear();
                string autoId = intv.Key.ToString();
                dicFieldNameResponse["Id"] = autoId;
                for (int m = 0; m < metaFields.Length; m++)
                    dicFieldNameResponse[metaFields[m].Key] = str(intv.Value, metaColumns[m]);

                bool firstAnswer = true;
                foreach (ReportAnswer a in answers)
                {
                    if (firstAnswer)
                    {
                        // The old code always stored the first answer under its plain QId as well.
                        if (!dicFieldNameResponse.ContainsKey(a.QId))
                            dicFieldNameResponse.Add(a.QId, a.Response);
                        firstAnswer = false;
                    }

                    if (!listOfMSQuestion.Contains(a.QId))
                    {
                        string prior;
                        if (!dicFieldNameResponse.TryGetValue(a.QId, out prior))
                            dicFieldNameResponse.Add(a.QId, a.Response);
                        else if (prior != a.Response)   // redundant data (that is an error) - keep both, as before
                            dicFieldNameResponse[a.QId] = prior + a.Response;
                    }
                    else
                    {
                        string key = a.QId + "_" + a.ROrder;
                        if (!dicFieldNameResponse.ContainsKey(key))
                            dicFieldNameResponse.Add(key, a.Response);
                    }
                }

                Dictionary<string, string> oe;
                if (!oeByInterview.TryGetValue(autoId, out oe)) oe = noOpenended;

                var columnData = new List<string>(columnName.Count);
                foreach (string col in columnName)
                {
                    string v;
                    if (dicFieldNameResponse.TryGetValue(col, out v) || oe.TryGetValue(col, out v))
                        columnData.Add(v);
                    else
                        columnData.Add("");
                }
                listOfColumnData.Add(columnData);
            }

            return listOfColumnData;
        }

        private static string str(DataRow r, DataColumn c)
        {
            return c == null || r.IsNull(c) ? "" : Convert.ToString(r[c]);
        }

        private static long toLong(DataRow r, DataColumn c)
        {
            long v;
            return long.TryParse(str(r, c).Trim(), out v) ? v : 0;
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
