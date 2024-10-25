using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class LogicalSyntax
    {
        public string ProjectId { get; set; }
        public string LogicId { get; set; }
        public string QId { get; set; }
        public string LogicTypeId { get; set; }
        public string IfCondition { get; set; }
        public string ThenValue { get; set; }
        public string ElseValue { get; set; }
    }
}
