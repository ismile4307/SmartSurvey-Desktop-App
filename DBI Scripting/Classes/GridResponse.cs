using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class GridResponse
    {
        public String qId;
        public int attributeOrder;

        public String responseValue;
        public int responseOrder;

        public GridResponse(String _qId, int _attributeOrder, String _responseValue, int _responseOrder)
        {
            qId = _qId;
            attributeOrder = _attributeOrder;
            responseValue = _responseValue;
            responseOrder = _responseOrder;
        }
    }
}
