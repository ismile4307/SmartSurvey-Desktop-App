using DBI_Scripting.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

                MyWebRequest myRequest = new MyWebRequest(StaticClass.SERVER_URL + "/deskapi/getprojects.php", "POST", "UserId="+StaticClass.USER_ID+"&UserTypeId="+StaticClass.USER_TYPE_ID+"&OrganizationId="+StaticClass.ORG_ID);

                string data = myRequest.GetResponse().ToString();

                DataTable dt1_temp = (DataTable)JsonConvert.DeserializeObject(data, (typeof(DataTable)));

                if (dt1_temp != null)
                {
                    for (int i = 0; i < dt1_temp.Rows.Count; i++)
                    {
                        ProjectInfo myProjectInfo = new ProjectInfo(Convert.ToInt32(dt1_temp.Rows[i]["id"]),
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
            }catch(Exception ex)
            {
                //MessageBox.Show("Server connection failed");
                return null;

            }

        }
    }
}
