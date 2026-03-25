using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSoftLIS_Interface.BLL;

namespace VSoftLIS_Interface.Common
{
    class CachedData
    {
        public static List<TestListItem> TestList = new List<TestListItem>();
        public static Analyzer Analyzer = new Analyzer();
        public static QualitativeProductList DescriptiveResultMaster = null;
        //public static Descriptivetextresults DescriptiveResultMaster = null;
        public static List<ValueLimits> ValueLimits = null;

        public static void UpdateAnalyzerDetails(int analyzerId)
        {
            Analyzer = new WebAPI().GetAnalyzerDetailsById(analyzerId);
        }

        public static void UpdateTestList(int analyzerId)
        {
            //To avoid concurrency issue, first fetch from server, then assign to public variable, as the variable is being accessed constantly in result update
            List<TestListItem> listTemp = WebAPI.GetTestList(analyzerId);
            if (Program.analyzer.instrumentgroupid == Program.analyzerTypeId_LCMS)
            {
                listTemp.Add(new TestListItem { testcode = "DUMMY", instrumentcode = "VDIS" });
            }
            TestList = listTemp;
        }

        public static void UpdateDescriptiveResultMaster(int AnalyzerId, int AnalyzerTypeId)
        {
            DescriptiveResultMaster = WebAPI.GetDescriptiveResultMaster(AnalyzerId);

            //SetDescriptiveResults = WebAPI.SetDescriptiveResultMaster(AnalyzerId);

            foreach (var item in DescriptiveResultMaster.testlist)
            {
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "Neg." });
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "Neg" });
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "neg" });
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "Negative" });
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "negative" });
                item.textresults.Add(new QualitativeResult { resultid = 798, result = "norm" });
                item.textresults.Add(new QualitativeResult { resultid = 7, result = "Pos" });
                item.textresults.Add(new QualitativeResult { resultid = 3, result = "Positive" });
                item.textresults.Add(new QualitativeResult { resultid = 3, result = "positive" });
                item.textresults.Add(new QualitativeResult { resultid = 837, result = "Trac." });
                item.textresults.Add(new QualitativeResult { resultid = 506, result = "+" });
                item.textresults.Add(new QualitativeResult { resultid = 507, result = "++" });
                item.textresults.Add(new QualitativeResult { resultid = 508, result = "+++" });
                item.textresults.Add(new QualitativeResult { resultid = 509, result = "++++" });
                item.textresults.Add(new QualitativeResult { resultid = 494, result = "(+)" });
                item.textresults.Add(new QualitativeResult { resultid = 495, result = "(++)" });
                item.textresults.Add(new QualitativeResult { resultid = 496, result = "(+++)" });
                item.textresults.Add(new QualitativeResult { resultid = 497, result = "(++++)" });
                item.textresults.Add(new QualitativeResult { resultid = 798, result = "normal" });
                item.textresults.Add(new QualitativeResult { resultid = 2, result = "-" });

            }
        }

        public static void UpdateValueLimits()
        {
            //ValueLimits = WebAPI.GetDescriptiveResultMaster();
            ValueLimits = new List<ValueLimits> {
                new ValueLimits{ TestcodeId=87, LowerLimit =50, HigherLimit=50},
                new ValueLimits{ TestcodeId=92, LowerLimit =150, HigherLimit=150},
                new ValueLimits{ TestcodeId=97, LowerLimit =145, HigherLimit=145},
                new ValueLimits{ TestcodeId=88, LowerLimit =50, HigherLimit=100000000},
                new ValueLimits{ TestcodeId=93, LowerLimit =150, HigherLimit=10000000},
                new ValueLimits{ TestcodeId=98, LowerLimit =145, HigherLimit=10000000}
            };
        }
    }
}
