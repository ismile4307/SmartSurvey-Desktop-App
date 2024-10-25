using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBI_Scripting.Classes
{
    class Club
    {
        public string groupName;
        public float mean;
        public float sdValue;
        public int n;
        public int xlRow;
        public int xlColnm;
        public string sigValue = "";

        public Club(string grName, string mn, string sd, string n1, int r, int c)
        {
            groupName = grName;
            mean = float.Parse(mn);
            sdValue = float.Parse(sd);
            n = Convert.ToInt32(float.Parse(n1));
            xlColnm = c;
            xlRow = r;
        }
    }
}
