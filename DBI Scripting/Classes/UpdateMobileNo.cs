using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class UpdateMobileNo
    {
        public String WrongMobileNo;
        public String CorrectedMobileNO;

        public UpdateMobileNo(String _WrongMobileNo, String _CorrectedMobileNO)
        {
            WrongMobileNo = _WrongMobileNo;
            CorrectedMobileNO = _CorrectedMobileNO;
        }
    }
}
