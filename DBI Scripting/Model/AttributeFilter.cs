using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Model
{
    class AttributeFilter
    {
        public string ProjectId { get; set; }
        public string AttribFilterId { get; set; }
        public string QId { get; set; }
        public string InheritedQId { get; set; }
        public string FilterType { get; set; }
        public string ExceptionalValue { get; set; }
        public string LabelTakenFrom { get; set; }
    }
}
