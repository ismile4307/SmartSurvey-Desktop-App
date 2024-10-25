using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class Response
    {
        public String responseValue;
        public int responseOrder;

        public Response(String _responseValue, int _responseOrder)
        {
            responseValue = _responseValue;
            responseOrder = _responseOrder;
        }
    }
}
