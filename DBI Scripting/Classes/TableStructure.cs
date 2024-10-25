using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class TableStructure
    {
        public String variableName;
        public String fieldWidth;
        public String qText;
        public String qType;
        public String qOrder;
        public String showInSearch;
        public String showInFreq;
        public String showInCorssTable;
        public String showInFilter;
        public String variableNameDB;

        public TableStructure(String _variableName, String _fieldWidth, String _qText, String _qType, String _qOrder, String _showInSearch, String _showInFreq, String _showInCorssTable, String _showInFilter, String _variableNameDB)
        {
            variableName = _variableName;
            fieldWidth = _fieldWidth;
            qText = _qText;
            qType = _qType;
            qOrder = _qOrder;
            showInSearch = _showInSearch;
            showInFreq = _showInFreq;
            showInCorssTable = _showInCorssTable;
            showInFilter = _showInFilter;
            variableNameDB = _variableNameDB;
        }
    }
}
