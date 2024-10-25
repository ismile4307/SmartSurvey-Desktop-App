using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class PClub
    {
        public string groupName;
        public float proportionValue;
        public int n;
        public int xlRow;
        public int xlColnm;
        public string sigValue = "";

        public PClub(string grName, string propValue, string n1, int r, int c)
        {
            if (propValue == "")
                propValue = "0";
            groupName = grName;
            proportionValue = float.Parse(propValue);
            n = Convert.ToInt32(float.Parse(n1));
            xlColnm = c;
            xlRow = r;
        }
    }
}
