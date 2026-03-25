using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class LISVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revised { get; set; }
        public string VersionNumber { get { return $"{Major}.{Minor}.{Build}.{Revised}"; } }
        public bool IsPriority { get; set; }
    }
}
