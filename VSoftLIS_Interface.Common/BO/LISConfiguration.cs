using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class LISConfiguration
    {
        public AnalyzerMaster AnalyzerMaster { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }
        public bool IsActive { get; set; }
    }
}
