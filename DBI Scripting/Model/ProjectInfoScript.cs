using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class ProjectInfoScript
    {
        private String projectName;
        private String projectCode;
        private String scriptVersion;
        private String databaseName;
        private String scriptedBy;


        //public ProjectInfoScript(string _projectName,string _projectCode,string _scriptVersion,string _databaseName, string _scriptedBy)
        //{
        //    projectName = _projectName;
        //    projectCode = _projectCode;
        //    scriptVersion = _scriptVersion;
        //    databaseName = _databaseName;
        //    scriptedBy = _scriptedBy;
        //}

        public string ProjectName   // property
        {
            get { return projectName; }
            set { projectName = value; }
        }
        public string ProjectCode   // property
        {
            get { return projectCode; }
            set { projectCode = value; }
        }
        public string ScriptVersion   // property
        {
            get { return scriptVersion; }
            set { scriptVersion = value; }
        }
        public string DatabaseName   // property
        {
            get { return databaseName; }
            set { databaseName = value; }
        }
        public string ScriptedBy   // property
        {
            get { return scriptedBy; }
            set { scriptedBy = value; }
        }
        
    }
}
