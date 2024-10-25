using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class UpdateFIFSInfo
    {
        public String FIName;
        public String FICode;
        public String FSName;
        public String FSCode;

        public UpdateFIFSInfo(String _FIName, String _FICode, String _FSName, String _FSCode)
        {
            FIName = _FIName;
            FICode = _FICode;
            FSName = _FSName;
            FSCode = _FSCode;
        }
    }
}
