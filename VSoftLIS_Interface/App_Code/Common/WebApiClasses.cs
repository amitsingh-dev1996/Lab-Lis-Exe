using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class Analyzer
    {
        public int instrumentid { get; set; }
        public string instrumentname { get; set; }
        public string instrumentLocation { get; set; }
        public int instrumentgroupid { get; set; }
      
        public List<ModuleAnalyzer> ModuleAnalyzers { get; set; }
    }

    public class ModuleAnalyzer
    {
        public int AnalyzerId { get; set; }
        public string AnalyzerName { get; set; }
        public string ModularIdentificationCode { get; set; }

    }
}
