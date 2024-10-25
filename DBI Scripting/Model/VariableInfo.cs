using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class VariableInfo
    {
        public string variableName;
        public string variableLabel;
        public List<Valueinfo> listOfValueInfo;

        public VariableInfo(string _variableName, string _variableLabel, List<Valueinfo> _listOfValueInfo)
        {
            variableName = _variableName;
            variableLabel = _variableLabel;
            listOfValueInfo = _listOfValueInfo;
        }
    }

    class Valueinfo
    {
        public string value;
        public string label;

        public Valueinfo(string _value,string _label)
        {
            value=_value;
            label=_label;
        }

    }
}
