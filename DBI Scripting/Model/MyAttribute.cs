using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class MyAttribute
    {
        public String attributeLabel;
        public String attributeValue;
        public int attributeOrder;
        public String takenOpenended;
        public String isExclusive;
        public String minValue;
        public String maxValue;
        public String linkId1;
        public String linkId2;
        public String forceAndMsgOpt;
        public List<MyGridAttribute> gridAttributes;


        public MyAttribute(String _attributeLabel, String _attributeValue, int _attributeOrder,
                         String _takenOpenended, String _isExclusive, String _linkId1, String _linkId2,
                         String _minValue, String _maxValue, String _forceAndMsgOpt, List<MyGridAttribute> _gridAttributes)
        {
            attributeLabel=_attributeLabel;
            attributeValue=_attributeValue ;
            attributeOrder=_attributeOrder ;
            takenOpenended=_takenOpenended;
            isExclusive=_isExclusive;
            linkId1=_linkId1;
            linkId2=_linkId2;
            minValue=_minValue;
            maxValue=_maxValue;
            forceAndMsgOpt=_forceAndMsgOpt;
            gridAttributes=_gridAttributes;
        }
    }
}
