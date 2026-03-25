using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.BLL
{
    public class InstrumentResult
    {
        public int instrumentid { get; set; }
        public List<ResultList> results { get; set; }
    }

    public class ResultList
    {
        public string barcode { get; set; }
        public List<Result> investigationresults { get; set; }
    }

    public class Result
    {
        public string instrumentCode { get; set; }
        public int testid { get; set; }
        public int instrumentid { get; set; }
        public string testcode { get; set; }
        public int resultid { get; set; }
        public double result { get; set; }
        public string resultunit { get; set; }
        public string symbol { get; set; }
        public string instrumentresultnote { get; set; }
        public string InstrumentRemarks { get; set; }
        public DateTime instrumenttime { get; set; }
        public int TextResultId { get; set; }
    }

    public class UpdateResultOutput
    {
        public int outputflag { get; set; }
        public string outputdetails { get; set; }
    }

    public class QualitativeResult
    {
        public int resultid { get; set; }
        public string result { get; set; }
    }
    public class TestsQualitativeResult
    {
        public int testid { get; set; }
        public string testcode { get; set; }
        public List<QualitativeResult> textresults { get; set; }
    }
    public class QualitativeProductList
    {
        public List<TestsQualitativeResult> testlist { get; set; }
    }

}
