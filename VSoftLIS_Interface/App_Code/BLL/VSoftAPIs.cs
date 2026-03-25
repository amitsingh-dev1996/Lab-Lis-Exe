using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.BLL
{
    public class sampleList
    {
        public int instrumentid { get; set; }
        public List<string> barcode { get; set; }
        public int mode { get; set; }

    }
    public class TestCode
    {
        public int testId { get; set; }
        public string testCode { get; set; }
        public string instrumentCode { get; set; }
        public int resultId { get; set; }
        public bool IsRepeat { get; set; }
    }

    public class BarcodeList
    {
        public string barcode { get; set; }
        public DateTime ProcessDate { get; set; } // { return (BVT.Value.Hour < 5 ? BVT.Value.AddDays(-1) : BVT.Value).Date; } }
        public string customername { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string AgeType { get; set; }
        public string investigationid { get; set; }
        public DateTime? collectiontime { get; set; }
        public DateTime? registertime { get; set; }
        public string refDr { get; set; }
        public string SCP { get; set; }
        public int Labcode { get; set; }
        public List<TestCode> testlist { get; set; }
        public BinInfo BinInfo { get; set; }
        public string ClientCode { get; set; }
        public string SampleType { get; set; }
        public bool IsBarcodeWoSentEarlier { get; set; }
        public string LabcodeWithDate { get { return ProcessDate.ToString("ddMMyyyy") + Labcode.ToString("000000"); } }
        public string LabcodeWithDateAndSourceCode { get { return LabcodeWithDate + "/" + ClientCode; } }
        //public DateTime DateOfBirth { get { return new DateTime(ProcessDate.Year - age, 1, 1); } }
        public DateTime? DateOfBirth { get; set; }
    }
    public class worklist
    {
        public List<BarcodeList> samples { get; set; }
    }
    public class TestDetail
    {
        public string instrumentCode { get; set; }
        public string testCode { get; set; }
        public int testId { get; set; }
        public int resultId { get; set; }
        public double result { get; set; }
        public string resultunit { get; set; }
        public string symbol { get; set; }
        public string instrumentresultnote { get; set; }
        public string InstrumentRemarks { get; set; }
        public DateTime instrumenttime { get; set; }
        public int TextResultId { get; set; }
        public int instrument_id { get; set; }
    }

    public class BarcodeDetail
    {
        public string barcode { get; set; }
        public List<TestDetail> testDetails { get; set; }
    }

    public class TagWOTest
    {
        public int mode { get; set; }
        public int AnalyzerID { get; set; }
        public List<int> ResultIDs { get; set; }
    }

    public class TestResult
    {
        public int analyzerId { get; set; }
        public List<BarcodeDetail> BarcodeDetails { get; set; }
    }

    public class ApiResponse
    {
        public int ResponseId { get; set; }
        public string Response { get; set; }
    }

    public class TestListItem
    {
        public int testid { get; set; }
        public string testcode { get; set; }
        public string instrumentcode { get; set; }
    }



    public class DescriptiveResultItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class DescriptiveResultProducts
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public List<DescriptiveResultItem> ResultList { get; set; }
    }

    public class DescriptiveResultList
    {
        public List<DescriptiveResultProducts> Products { get; set; }
    }

    public class BinInfo
    {
        public string Bin { get; set; }
        public string LabCode { get; set; }
        public string Remarks { get; set; }
    }

    public class ValueLimits
    {
        public int TestcodeId { get; set; }
        public double LowerLimit { get; set; }
        public double HigherLimit { get; set; }
    }
    public class SampleArchivalResponse
    {
        public SampleArchivalInput inputPara { get; set; }
        public int status { get; set; }
        public string Message { get; set; }
    }
    public class SampleArchivalInput
    {
        public string barcode { get; set; }
        public string ArchivalPosition { get; set; }
        public string ArchivalRack { get; set; }
    }

}
