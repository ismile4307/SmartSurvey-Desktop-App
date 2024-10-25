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

        public static bool success_check_user()
        {
            string userId = "";
            string passcode = "";

            string sTemp;

            sTemp = System.AppDomain.CurrentDomain.BaseDirectory;

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
