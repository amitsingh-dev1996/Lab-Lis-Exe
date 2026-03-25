using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class AnalyzerActiveVersions
    {
        public int AnalyzerId { get; set; }
        public List<LISVersion> ActiveVersions { get; set; }

        public LISVersion PopulateLatestVersion()
        {
            return InterfaceHelper.PopulateLatestVersion(ActiveVersions);
        }
    }
}
