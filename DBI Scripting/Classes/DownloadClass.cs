using DBI_Scripting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class DownloadClass
    {
        public List<ProjectInfo> getProjectInfoFromServer()
        {
            try
            {
                List<ProjectInfo> listOfProjectInfo = new List<ProjectInfo>();

                string url = StaticClass.SERVER_URL + "/api/desk/projects";

                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method      = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept      = "application/json";
                req.ContentLength = 0;
                if (!string.IsNullOrEmpty(StaticClass.DESK_API_TOKEN))
                    req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + StaticClass.DESK_API_TOKEN);

                string data;
                HttpWebResponse httpResp;
                try { httpResp = (HttpWebResponse)req.GetResponse(); }
                catch (WebException wex) when (wex.Response != null)
                { httpResp = (HttpWebResponse)wex.Response; }

                using (httpResp)
                using (var reader = new StreamReader(httpResp.GetResponseStream(), Encoding.UTF8))
                    data = reader.ReadToEnd();

                if (httpResp.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Server returned {(int)httpResp.StatusCode}: {data}");

                DataTable dt1_temp;
                try
                {
                    dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));
                }
                catch (Exception jsonEx)
                {
                    throw new Exception("Server response (not valid JSON): " + data, jsonEx);
                }

                if (dt1_temp != null)
                {
                    for (int i = 0; i < dt1_temp.Rows.Count; i++)
                    {
                        ProjectInfo myProjectInfo = new ProjectInfo(
                            Convert.ToInt32(dt1_temp.Rows[i]["id"]),
                            dt1_temp.Rows[i]["project_name"].ToString(),
                            dt1_temp.Rows[i]["project_code"].ToString(),
                            dt1_temp.Rows[i]["script_name"].ToString(),
                            dt1_temp.Rows[i]["start_date"].ToString(),
                            dt1_temp.Rows[i]["status"].ToString()
                        );
                        listOfProjectInfo.Add(myProjectInfo);
                    }
                }

                return listOfProjectInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("getProjectInfoFromServer failed: " + ex.Message, ex);
            }
        }
    }
}
