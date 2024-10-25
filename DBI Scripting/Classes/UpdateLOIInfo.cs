using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class UpdateLOIInfo
    {
        public String StartTime;
        public String EndTime;
        public String LOI;

        public UpdateLOIInfo(String _StartTime, String _EndTime, String _LOI)
        {
            StartTime = _StartTime;
            EndTime = _EndTime;
            LOI = _LOI;
        }
    }
}
