using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class MyGridAttribute
    {
        public String attributeLabel;
        public String attributeValue;
        public int attributeOrder;
        public String takenOpenended;
        public String isExclusive;
        public String minValue;
        public String maxValue;
        public String forceAndMsgOpt;



        public MyGridAttribute(String _attributeLabel, String _attributeValue, int _attributeOrder,
                         String _takenOpenended, String _isExclusive, String _minValue, String _maxValue, String _forceAndMsgOpt)
        {
            attributeLabel=_attributeLabel;
            attributeValue=_attributeValue ;
            attributeOrder=_attributeOrder ;
            takenOpenended=_takenOpenended;
            isExclusive=_isExclusive;
            minValue=_minValue;
            maxValue=_maxValue;
            forceAndMsgOpt=_forceAndMsgOpt;
        }
    }
}
