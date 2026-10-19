using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBI_Scripting.Classes
{
    static class StaticClass
    {
        public static String SERVER_URL = Properties.Settings.Default.ServerAddress;//"https://smartsurveybd.com";
        public static String QDBPath = "";
        public static String ADBPath = "";
        public static String USER_ID = "";
        public static String USER_TYPE_ID = "";
        public static String ORG_ID = "";
        public static String DESK_API_TOKEN = "";
        public static String USER_EMAIL = "";
        public static String USER_PASSCODE = "";

        private static readonly object _deskTokenLock = new object();

        /// <summary>
        /// Makes sure DESK_API_TOKEN holds a Sanctum token for the /api/desk/* endpoints.
        /// Logs in with the saved credentials when there is no token yet, or when the current
        /// token is staleToken (the one a request just got 401 with, e.g. because the same
        /// account logged in on another PC and revoked it). Several requests failing together
        /// pass the same staleToken, so only the first logs in again and the rest reuse its token.
        /// Returns null on success, otherwise the reason the token could not be acquired.
        /// Blocking call; safe from any thread.
        /// </summary>
        public static string EnsureDeskToken(string staleToken = null)
        {
            lock (_deskTokenLock)
            {
                if (!string.IsNullOrEmpty(DESK_API_TOKEN) && DESK_API_TOKEN != staleToken)
                    return null;
                if (string.IsNullOrEmpty(USER_EMAIL))
                    return "not logged in";

                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                    string body = "email="     + Uri.EscapeDataString(USER_EMAIL)
                                + "&password=" + Uri.EscapeDataString(USER_PASSCODE);

                    string json;
                    using (var wc = new WebClient())
                    {
                        wc.Encoding = Encoding.UTF8;
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        wc.Headers[HttpRequestHeader.Accept] = "application/json";
                        try
                        {
                            json = wc.UploadString(SERVER_URL + "/api/desk/login", "POST", body);
                        }
                        catch (WebException wex) when (wex.Response != null)
                        {
                            // 401 carries {"success":false,"message":...}
                            using (var reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.UTF8))
                                json = reader.ReadToEnd();
                        }
                    }

                    var obj = Newtonsoft.Json.Linq.JObject.Parse(json);
                    if (obj.Value<bool>("success"))
                    {
                        // Sanctum tokens look like "{row id}|{secret}". The server's token table has no
                        // auto-increment (every row id is 0), so a lookup by id finds the wrong row and
                        // answers 401. Sending only the secret makes Sanctum look the token up by its hash.
                        string token = obj.Value<string>("token") ?? "";
                        int bar = token.IndexOf('|');
                        DESK_API_TOKEN = bar >= 0 ? token.Substring(bar + 1) : token;
                        return DESK_API_TOKEN == "" ? "server returned an empty token" : null;
                    }
                    DESK_API_TOKEN = "";
                    return obj.Value<string>("message") ?? "login rejected";
                }
                catch (Exception ex)
                {
                    DESK_API_TOKEN = "";
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// "api_token=..." form field carrying DESK_API_TOKEN. Sent with every /api/desk/* request
        /// next to the Authorization header, because some Apache setups strip that header;
        /// the server's DeskTokenFromBody middleware falls back to this field.
        /// </summary>
        public static string DeskTokenField()
        {
            return "api_token=" + Uri.EscapeDataString(DESK_API_TOKEN ?? "");
        }

        public static bool success_check_user()
        {
            string userId = "";
            string passcode = "";

            string sTemp;

            //sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
            sTemp = "C:\\Temp";
            TextReader txtReader = new StreamReader(sTemp + "\\index.ini");

            string strline = txtReader.ReadLine();
            int i = 1;
            while (strline != null)
            {
                if (i == 1)
                    userId = strline;
                if (i == 2)
                    passcode = strline;

                i++;

                strline = txtReader.ReadLine();
            }
            txtReader.Close();

            if (userId == "" && passcode == "")
                return false;
            else
            {
                USER_EMAIL   = userId;
                USER_PASSCODE = passcode;
                DESK_API_TOKEN = "";   // new credentials: the next desk API call logs in again
                return user_check_status(userId, passcode);
            }

        }

        public static bool user_check_status(string userId, string passcode)
        {
            try
            {
                WebClient c = new WebClient();

                MyWebRequest myRequest;
                //if (chkDeletedRec.Checked == false)
                //myRequest1 = new MyWebRequest(Properties.Settings.Default.ServerAddress + "/rejectinterview.php", "POST", "RespondentId=" + listOfRespondentId[i] + "&ProjectId=" + dicProjectNameVsCode[comProjectName.Text]);
                myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/checkcredential.php", "POST", "UserId=" + userId + "&Passcode=" + passcode);
                //else
                //myRequest1 = new MyWebRequest("http://capiapi.chronometerhub.com/download_data/respondentdel.php", "POST", "startDate=" + startDate + "&endDate=" + endDate + "&dateType=" + dicDateConsiderVsCode[comDateConsider.Text] + "&projectCode=" + dicProjectNameVsCode[comProject.Text]);

                //Console.WriteLine(data);
                //JObject o = JObject.Parse(data);
                ////string data = myRequest1.GetResponse().ToString();
                ////JObject o = JObject.Parse(data);
                ////String id = o.GetValue("message").ToString();
                //MessageBox.Show(o.GetValue("message").ToString());


                USER_ID = "";

                string data = myRequest.GetResponse().ToString();

                DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                if (dt1_temp != null)
                {
                    for (int i = 0; i < dt1_temp.Rows.Count; i++)
                    {
                        USER_ID = dt1_temp.Rows[i]["id"].ToString();
                        USER_TYPE_ID = dt1_temp.Rows[i]["user_type_id"].ToString();
                        ORG_ID = dt1_temp.Rows[i]["organization_id"].ToString();
                    }
                }

                if (USER_ID == "")
                    return false;
                else
                    return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

        }

        public static void releaseObject(object obj)
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
    }
}
