using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class ProjectInfo
    {
        public int Id;
        public String ProjectName;
        public String ProjectCode;
        public String DatabaseName;
        public String StartDate;
        public String Status;

        public ProjectInfo(int _id,string _sProjectName,string _sProjectCode,string _sDatabaseName,string _sStartDate,string _sStatus)
        {
            Id = _id;
            ProjectName = _sProjectName;
            ProjectCode = _sProjectCode;
            DatabaseName = _sDatabaseName;
            StartDate = _sStartDate;
            Status = _sStatus;

        }

    }
}
