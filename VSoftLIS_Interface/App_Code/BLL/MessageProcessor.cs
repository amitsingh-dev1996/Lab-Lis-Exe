using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using VSoftLIS_Interface.Common;

namespace VSoftLIS_Interface.BLL
{
    public class MessageProcessor
    {
        ErrorLog errorLog = new ErrorLog();

        public DataTable dtResults = null;
        public InstrumentResult testResults = null;
        List<string> queryBarcodes = new List<string>();
        List<string> queryBarcodesForResults = new List<string>();
        static string WOBarcode_Single = "";
        List<RecordInfo> responseRecords = new List<RecordInfo>();
        Dictionary<string, string> CopyValues = new Dictionary<string, string>();
        public string SortingRule = "";
        MessageConfiguration msgConfig = null;
        public int AnalyzerID { get; private set; }

        public MemoryCache chache { get; set; }

        //variables to hold reference to current RecordInfo type
        string strHeader = string.Empty;
        string strQuery = string.Empty;
        string strQrdData = string.Empty;
        RecordInfo riQuery_Incoming = null;
        RecordInfo riQuery = null;
        RecordInfo riHeader = null;
        RecordInfo riPatient = null;
        RecordInfo riOrder = null;
        RecordInfo riResult = null;

        private static int lastDummySrNo = 0;

        static MessageProcessor()
        {
            //https://stackoverflow.com/questions/15804059/exception-while-running-system-threading-tasks-task
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                foreach (var ex in args.Exception.InnerExceptions.GroupBy(r => new string(r.Message.Take(20).ToArray())))
                {
                    UiMediator.LogAndShowError(Program.AnalyzerId, ex.First(), "TaskScheduler Error");
                }
                args.SetObserved();
            };

        }

        public MessageProcessor(int analyzerID)
        {
            AnalyzerID = analyzerID;
            msgConfig = new MessageConfiguration(analyzerID);
        }

        //public List<string> ProcessCompleteRecord(List<string> receivedData)
        //{
        //    string WOBarcode_Single;
        //    return ProcessCompleteRecord(receivedData, out WOBarcode_Single);
        //}

        public List<string> ProcessCompleteRecord(List<string> receivedData/*, out string WOBarcode_Single*/)
        {

            List<string> records = new List<string>();
            //WOBarcode_Single = "";
            try
            {
                string barcode = String.Empty;
                string testcode = "";
                decimal? testValue = null;
                string testValuesUrinsed2 = "";
                double testValuesUrinsed = 0;
                object objTestValue = "";
                object objTestValue2 = "";
                string unit = string.Empty;
                string resultAbnormalFlag = string.Empty;
                string prefix = string.Empty;
                string reasonForTestcodeRemoval = "";
                DateTime? resultedDateTime = null;
                string strValue = "";
                int DescriptiveId = -1;
                DataRow drResult = null;


                dtResults = GetResultsTableStructure();

                string[] incoming = receivedData.ToArray(); // .Split(Convert.ToChar(Characters.CR));

                string testheader = incoming[0];

                chache = new MemoryCache(testheader);


                //moved variable clearing logic outside of loop, so that they will be cleared only once, to support continuous messages having Header and Last records inbetween
                queryBarcodes.Clear();
                CopyValues.Clear();
                //dtResults = null;
                testResults = null;


                foreach (string s in incoming)
                {

                    string strRecordReceived = s.Trim(Convert.ToChar(Characters.CR)).Trim(Convert.ToChar(Characters.ENQ)).Trim(Convert.ToChar(Characters.STX)); //s.Replace("\\n", string.Empty).Trim();
                    //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Chorus)
                    //{
                    //    if (InterfaceHelper.AsciiStringToHexString(InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 1, 1)) == "D2")//JListCmd
                    //    {
                    //        queryBarcodes.Add(InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 3, 19).Trim('\0'));
                    //        var barcodeList = GetWorklist(barcode)?.barcodeList;
                    //        return PrepareWorklistMessages(queryBarcodes, barcodeList);
                    //    }
                    //    else if (InterfaceHelper.AsciiStringToHexString(InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 1, 1)) == "D7")//ResFrame
                    //    {
                    //        barcode = InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 2, 19).Trim('\0');
                    //        testcode = InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 21, 7).Trim('\0');
                    //        //strValue = InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 28, 1).Trim('\0');
                    //        //DescriptiveId = PopulateDescriptiveIdByMachineCode(testcode, strValue);
                    //        strValue = InterfaceHelper.GetSubstringOrEmpty(strRecordReceived, 29, 12).Trim('\0');
                    //        testValue = InterfaceHelper.ExtractNumericValue(strValue, out prefix);
                    //        AddResultRecord(barcode, testcode, testValue, "", prefix, DateTime.Now, DescriptiveId);

                    //        //records.Add(Characters.EOT);
                    //    }

                    //}

                    #region LOGIC TO UPDATE H9 HPLC VALUES
                    //single line(Raw Data) communication with stx and etx , so devided here to extract the values and update it in Vsoft
                    if (msgConfig.AnalyzerTypeID == 313)
                    {
                        try
                        {
                            string[] PIDString = strRecordReceived.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                            PIDString = strRecordReceived.Split(new string[] { "\u0002" }, StringSplitOptions.RemoveEmptyEntries);
                            var patientIdString = PIDString[0].Substring(7, 2);
                            var IDSplit = PIDString[0].Substring(7, 2);
                            var Patientid = PIDString[0].Substring(9, Convert.ToInt32(patientIdString));
                            var output = Patientid;

                            var ReportParms = new Dictionary<string, string>();
                            strRecordReceived = PIDString[0].Remove(10, Convert.ToInt32(patientIdString) - 1);
                            var machineData = strRecordReceived.Split(new string[] { "0" }, StringSplitOptions.RemoveEmptyEntries);

                            //Part to update the update the values in Vsoft for reporting
                            //Result(%)
                            var PatientId = output;
                            string HbA1a = strRecordReceived.Substring(117, 5);
                            testValue = Convert.ToDecimal(HbA1a);
                            AddResultRecord(PatientId, "HbA1a", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string HbA1b = strRecordReceived.Substring(122, 5);
                            testValue = Convert.ToDecimal(HbA1b);
                            AddResultRecord(PatientId, "HbA1b", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string HbF = strRecordReceived.Substring(126, 5);
                            testValue = Convert.ToDecimal(HbF);
                            AddResultRecord(PatientId, "HbF", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string LA1c = strRecordReceived.Substring(129, 5);
                            testValue = Convert.ToDecimal(LA1c);
                            AddResultRecord(PatientId, "LA1c", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string HbA1c = strRecordReceived.Substring(133, 4);
                            testValue = Convert.ToDecimal(HbA1c);
                            AddResultRecord(PatientId, "HbA1c", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string HbA0 = strRecordReceived.Substring(137, 5);
                            testValue = Convert.ToDecimal(HbA0);
                            AddResultRecord(PatientId, "HbA0", testValue, "", prefix, DateTime.Now, DescriptiveId);

                            //Graph data extraction
                            var outputForHbalc = strRecordReceived.Split('.').First();
                            string Hba1aString = outputForHbalc.Substring((outputForHbalc.Length) - 13);
                            //TIME
                            var Hba1aTime = Hba1aString.Substring(0, 2);
                            testValue = Convert.ToDecimal(Hba1aTime);
                            AddResultRecord(PatientId, "Hba1aTime", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var Hba1bTime = Hba1aString.Substring(2, 2);
                            testValue = Convert.ToDecimal(Hba1bTime);
                            AddResultRecord(PatientId, "Hba1bTime", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbFTime = Hba1aString.Substring(4, 2);
                            testValue = Convert.ToDecimal(HbFTime);
                            AddResultRecord(PatientId, "HbFTime", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var La1cTime = Hba1aString.Substring(6, 2);
                            testValue = Convert.ToDecimal(La1cTime);
                            AddResultRecord(PatientId, "La1cTime", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA1cTime = Hba1aString.Substring(8, 2);
                            testValue = Convert.ToDecimal(HbA1cTime);
                            AddResultRecord(PatientId, "HbA1cTime", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA0Time = Hba1aString.Substring(10, 2);
                            testValue = Convert.ToDecimal(HbA0Time);
                            AddResultRecord(PatientId, "HbA0Time", testValue, "", prefix, DateTime.Now, DescriptiveId);

                            //Absorbance
                            string Hba1aABSString = strRecordReceived.Substring(0, strRecordReceived.Length - 1219);
                            var HbA0Result = Hba1aABSString.Substring(Hba1aABSString.Length - 4, 4);

                            var HbA1aAbs = Hba1aABSString.Substring(45, 6);
                            testValue = Convert.ToDecimal(HbA1aAbs);
                            AddResultRecord(PatientId, "HbA1aAbs", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA1bAbs = Hba1aABSString.Substring(51, 6);
                            testValue = Convert.ToDecimal(HbA1bAbs);
                            AddResultRecord(PatientId, "HbA1bAbs", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbFAbs = Hba1aABSString.Substring(57, 6);
                            testValue = Convert.ToDecimal(HbFAbs);
                            AddResultRecord(PatientId, "HbFAbs", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var LA1cAbs = Hba1aABSString.Substring(63, 6);
                            testValue = Convert.ToDecimal(LA1cAbs);
                            AddResultRecord(PatientId, "LA1cAbs", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA1cAbs = Hba1aABSString.Substring(69, 6);
                            testValue = Convert.ToDecimal(HbA1cAbs);
                            AddResultRecord(PatientId, "HbA1cAbs", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA0Abs = Hba1aABSString.Substring(75, 6);
                            testValue = Convert.ToDecimal(HbA0Abs);
                            AddResultRecord(PatientId, "HbA0Abs", testValue, "", prefix, DateTime.Now, DescriptiveId);


                            //AREA
                            var HbA1aArea = Hba1aABSString.Substring(81, 6);
                            testValue = Convert.ToDecimal(HbA1aArea);
                            AddResultRecord(PatientId, "HbA1aArea", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var Hba1bArea = Hba1aABSString.Substring(87, 6);
                            testValue = Convert.ToDecimal(Hba1bArea);
                            AddResultRecord(PatientId, "Hba1bArea", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbFArea = Hba1aABSString.Substring(93, 6);
                            testValue = Convert.ToDecimal(HbFArea);
                            AddResultRecord(PatientId, "HbFArea", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var La1cArea = Hba1aABSString.Substring(99, 6);
                            testValue = Convert.ToDecimal(La1cArea);
                            AddResultRecord(PatientId, "La1cArea", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var HbA1cArea = Hba1aABSString.Substring(105, 6);
                            testValue = Convert.ToDecimal(HbA1cArea);
                            AddResultRecord(PatientId, "HbA1cArea", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            var Hba10Area = Hba1aABSString.Substring(111, 6);
                            testValue = Convert.ToDecimal(Hba10Area);
                            AddResultRecord(PatientId, "Hba10Area", testValue, "", prefix, DateTime.Now, DescriptiveId);

                            var V_WindowArea = Hba1aABSString.Substring(117, 6);
                            testValue = Convert.ToDecimal(V_WindowArea);
                            AddResultRecord(PatientId, "V_WindowArea", testValue, "", prefix, DateTime.Now, DescriptiveId);

                            var TotalArea = Convert.ToDouble(Hba10Area) + Convert.ToDouble(HbA1cArea) + Convert.ToDouble(La1cArea) + Convert.ToDouble(HbFArea) + Convert.ToDouble(Hba1bArea) + Convert.ToDouble(HbA1aArea);
                            testValue = Convert.ToDecimal(TotalArea);
                            AddResultRecord(PatientId, "TotalArea", testValue, "", prefix, DateTime.Now, DescriptiveId);




                            // Curve Values
                            string HBA1c_IFCC = strRecordReceived.Substring(141, 5);
                            testValue = Convert.ToDecimal(HBA1c_IFCC);
                            AddResultRecord(PatientId, "HBA1c_IFCC", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string eAG_ADA_mmol_L = strRecordReceived.Substring(146, 4);
                            testValue = Convert.ToDecimal(eAG_ADA_mmol_L);
                            AddResultRecord(PatientId, "eAG_ADA_mmol_L", testValue, "", prefix, DateTime.Now, DescriptiveId);
                            string eAG_ADA_mg_dL = strRecordReceived.Substring(150, 5);
                            testValue = Convert.ToDecimal(eAG_ADA_mg_dL);
                            AddResultRecord(PatientId, "eAG_ADA_mg_dL", testValue, "", prefix, DateTime.Now, DescriptiveId);

                            //string Sent_chromatographic_curve_number= strRecordReceived.Substring(150, 5);
                            //string Sent_chromatographic_curve_values = strRecordReceived.Substring(150, 5);

                            //Graph Data : 
                            int CountGraph = 0;
                            string Graph = "";
                            var Position = 158;
                            var Length = 6;

                            for (int i = 0; i <= 200; i++)
                            {

                                Graph = strRecordReceived.Substring(Position, Length);
                                testValue = Convert.ToDecimal(Graph);
                                testcode = "G" + CountGraph;

                                AddResultRecord(PatientId, testcode, testValue, "", prefix, DateTime.Now, DescriptiveId);
                                Position = Position + 6;
                                CountGraph++;
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                        break;
                    }
                    #endregion

                    if (IsRecordTypeMatch(strRecordReceived, msgConfig.HeaderRecordInfo))
                    {
                        strHeader = strRecordReceived;
                        try
                        {
                            CollectCopyValues(strRecordReceived, msgConfig.HeaderRecordInfo);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }

                        if ((string)ExtractRecordValue(strRecordReceived, msgConfig.HeaderRecordInfo, msgConfig.HeaderRecordInfo.CommentOrSpecialInstructions) == "MSA")
                        {
                            return new List<string> { };
                        }


                        //UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Incoming, "Header record Received"/*\n*/);
                    }

                    else if (msgConfig.SupportsOnlyQuery || IsRecordTypeMatch(strRecordReceived, msgConfig.QueryRecordInfo))
                    {
                        strQuery = strRecordReceived;
                        try
                        {
                            int barcodesCount = 1;
                            riQuery_Incoming = msgConfig.QueryRecordInfo.Copy();

                            CollectCopyValues(strRecordReceived, riQuery_Incoming);
                            if (!msgConfig.IsFieldSizeInBytes)
                            {
                                barcodesCount = GetRepeatValuesCount(strRecordReceived, riQuery_Incoming.SampleID);
                            }
                            else
                            {
                                if (msgConfig.SupportsMultipleBarcodes)
                                {
                                    barcodesCount = (int)ExtractRecordValue(strRecordReceived, riQuery_Incoming, riQuery_Incoming.NumberOfInquirySamplesInBlock);
                                }
                            }

                            for (int i = 1; i <= barcodesCount; i++)
                            {

                                barcode = (string)ExtractRecordValue(strRecordReceived, riQuery_Incoming, riQuery_Incoming.SampleID, i);

                                if (barcode == null) //for Centaur XP, sends blank barcode query at last
                                    barcode = "";


                                if (String.IsNullOrEmpty(barcode))
                                {
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Empty barcode received");
                                }
                                else
                                {
                                    queryBarcodes.Add(barcode);
                                    UiMediator.AddUiGridData(AnalyzerID, barcode, "QUERY", string.Empty, null, "", "");
                                }
                            }

                            SortingRule = (string)ExtractRecordValue(strRecordReceived, riQuery_Incoming, riQuery_Incoming.SortingRule);
                        }
                        catch (MyException ex)
                        {
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Barcode could not be read from query message" /*+ "\n"*/);
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.PatientRecordInfo))
                    {
                        drResult = null;
                        riPatient = msgConfig.PatientRecordInfo.Copy();

                        if (riPatient.SampleID != null)
                        {
                            barcode = (string)ExtractRecordValue(strRecordReceived, riPatient, riPatient.SampleID);

                            if (barcode == null)
                                barcode = "";
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.OrderRecordInfo))
                    {
                        drResult = null;
                        riOrder = msgConfig.OrderRecordInfo.Copy();

                        if (riOrder.SampleID != null)
                        {

                            barcode = (string)ExtractRecordValue(strRecordReceived, riOrder, riOrder.SampleID);

                            if (barcode == null)
                                barcode = "";

                            barcode = barcode.Trim(); //for Sysmex XN 1000

                            barcode = barcode.Replace("!", ""); // For CBC Dxh560
                        }
                        //UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Incoming, "Order record Received with barcode " + barcode /*+ "\n"*/);
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.ResultRecordInfo) || IsRecordTypeMatch(strRecordReceived, msgConfig.QcResultRecordInfo))
                    {
                        int resultsCount = 1;
                        bool isQcResult = false;

                        if (IsRecordTypeMatch(strRecordReceived, msgConfig.QcResultRecordInfo))
                        {
                            isQcResult = true;
                            riResult = msgConfig.QcResultRecordInfo.Copy();
                        }
                        else
                        {
                            riResult = msgConfig.ResultRecordInfo.Copy();
                        }

                        if (String.IsNullOrEmpty(barcode) && riResult.SampleID != null)
                        {
                            barcode = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.SampleID);
                        }

                        int blockNumber = 0;
                        int numberOfTestsInBlock = 1;
                        if (riResult.BlockNumber != null)
                        {
                            blockNumber = (int)ExtractRecordValue(strRecordReceived, riResult, riResult.BlockNumber);
                        }

                        if (riResult.NumberOfTestsInBlock != null)
                        {
                            resultsCount = numberOfTestsInBlock = (int)ExtractRecordValue(strRecordReceived, riResult, riResult.NumberOfTestsInBlock);
                        }

                        if (!msgConfig.IsFieldSizeInBytes)
                        {
                            //resultsCount = 1;
                        }
                        else
                        {
                            if (msgConfig.SupportsMultipleTestcodes)
                            {
                                if (numberOfTestsInBlock > 1)
                                {
                                    msgConfig.ResultRecordInfo.RecordFields[riResult.TestValue.FieldNumber].BytesLength = numberOfTestsInBlock * msgConfig.ResultRecordInfo.RequestTest.BytesLength_PerComponentSet;
                                }
                                else
                                {
                                    if (msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                                    {
                                        RecordFieldInfo rfiParent = msgConfig.ResultRecordInfo.RecordFields[riResult.TestValue.FieldNumber];
                                        int bytesLengthBeforeResultField = riResult.RecordFields.Where(r => r.FieldNumber_Incoming < rfiParent.FieldNumber_Incoming).Sum(r => r.BytesLength);
                                        int bytesLengthAfterResultField = riResult.RecordFields.Where(r => r.FieldNumber_Incoming > rfiParent.FieldNumber_Incoming).Sum(r => r.BytesLength);
                                        rfiParent.BytesLength = strRecordReceived.Length - bytesLengthBeforeResultField - bytesLengthAfterResultField;
                                        resultsCount = rfiParent.BytesLength / rfiParent.BytesLength_PerComponentSet;
                                    }
                                }
                            }
                            else
                            {
                                resultsCount = 1;
                            }

                            //if (riResult.SampleID != null)
                            //{
                            //    barcode = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.SampleID);
                            //}
                        }

                        if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Bechman)
                        {
                            int resultValueStartIndex = GetFieldValueStartIndex(riResult, riResult.TestID_ManufacturersTestCode);
                            string strRecordModified = strRecordReceived;
                            while (resultsCount == 0 || strRecordModified.Length > resultValueStartIndex)
                            {
                                if (strRecordModified.Length > (resultValueStartIndex + 11) && strRecordModified.Substring(resultValueStartIndex + 11, 1) == " ")
                                {
                                    strRecordModified = strRecordModified.Insert(resultValueStartIndex + 3, new String(' ', 32));
                                }
                                resultValueStartIndex += riResult.RequestTest.BytesLength_PerComponentSet;
                                resultsCount++;
                            }
                            strRecordReceived = strRecordModified;
                        }

                        int sequenceNumber = -1;

                        if (!msgConfig.IsFieldSizeInBytes && riResult.SequenceNumber != null && riResult.SequenceNumber.FieldNumber > -1)
                        {
                            sequenceNumber = (int)ExtractRecordValue(strRecordReceived, riResult, riResult.SequenceNumber);
                        }

                        List<string> list = new List<string>();
                        for (int i = 1; i <= resultsCount; i++)
                        {
                            TestListItem tli = null;
                            string valueErrorDescription = "";
                            try
                            {
                                testcode = "";
                                testValue = null;
                                objTestValue = "";
                                objTestValue2 = "";
                                testValuesUrinsed2 = "";
                                testValuesUrinsed = 0;
                                unit = string.Empty;
                                resultAbnormalFlag = string.Empty;
                                reasonForTestcodeRemoval = "";
                                resultedDateTime = null;
                                DescriptiveId = -1;
                                strValue = "";

                                //if (msgConfig.AnalyzerTypeID != 313)
                                testcode = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.TestID_ManufacturersTestCode, i);
                                //Logic for oman location Au700 to update the values
                                if (msgConfig.AnalyzerID == 25 && (testcode.Length == 2 || testcode.Length == 1))
                                    testcode = 0 + testcode;

                                tli = GetTestMappingOrEmpty(testcode);

                                // to take units for values updation
                                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT)
                                    unit = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.Units, i);

                                string resultAspects = (string)ExtractRecordValue(strRecordReceived, riResult, msgConfig.ResultRecordInfo.ResultAspects, i);
                                //if (new int[] { 1/*, 50*/ }.Contains(msgConfig.AnalyzerTypeID) && !new string[] { "DOSE", "INDX"/*, "RLU"*/ }.Contains(resultAspects))
                                //    continue;
                                //if (msgConfig.AnalyzerTypeID == 201 && !new string[] { "DOSE" }.Contains(resultAspects))
                                //    continue;
                                //else if (msgConfig.AnalyzerTypeID == 34 && resultAspects != "OD")
                                //    continue;
                                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA && !new string[] { "DOSE", "INDX" }.Contains(resultAspects))
                                    continue;

                                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Architect && resultAspects != "F")
                                    continue;
                                //else if (msgConfig.AnalyzerTypeID == 310 && strRecordReceived.Contains("^MAINFORMAT"))
                                //    continue;
                                else if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT && unit != "p/HPF" && new string[] { "URBC", "UWBC", "UWBCC", "UUA", "UPCAST", "UCAST", "UCaOX", "UHYA", "UNSE", "USQEP", "UYST", "UBACR", "UBACC", "UMUC", "UTRIP", "USPRM" }.Contains(tli.testcode) && tli.testcode != null)
                                    continue;
                                else if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT && new string[] { "mg/dl", "μmol/l", "Leu/ul", "g/l", "mmol/l", "Ery/ul", "umol/l" }.Contains(unit) && new string[] { "UBIL", "UBNGN", "UKET", "ASCORBIC ACID", "UGLU", "UPROT", "UBLD", "UPH", "UNIT", "ULEUC", "UAPPE", "UCOLO", "SPGR", null }.Contains(tli.testcode) /*&& tli._testCode!=null*/)
                                    continue;
                                //To take only Area value instead of Time
                                else if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Biored_Variant_II && resultAspects != "AREA")
                                    continue;

                                if (msgConfig.AnalyzerTypeID != 59)
                                {
                                    valueErrorDescription = " (Barcode: " + barcode + ", Test: " + (tli?.testcode ?? testcode) + ")";

                                    //if (msgConfig.AnalyzerTypeID == 29)
                                    //    testcode += msgConfig.ComponentDelimiter + resultAspects;
                                    if (msgConfig.AnalyzerTypeID == AnalyzerTypes.BD && String.IsNullOrEmpty(testcode))
                                        continue;

                                    if (msgConfig.RemoveUnmappedTestcodes && !CachedData.TestList.Where(r => r.instrumentcode == testcode).Any())
                                    {
                                        reasonForTestcodeRemoval = "Testcode mapping not found";
                                        goto removeTestCode;
                                    }

                                    try
                                    {
                                        if (msgConfig.AnalyzerTypeID != AnalyzerTypes.LABUMAT)
                                            objTestValue = ExtractRecordValue(strRecordReceived, riResult, riResult.TestValue, i);

                                        //To capture value for UCOLO and UAPPE LABUMAT Machine
                                        if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT && (tli.testcode == "UAPPE" || tli.testcode == "UCOLO" || tli.testcode == "SPGR"))
                                            objTestValue = ExtractRecordValue(strRecordReceived, riResult, riResult.TestValue, i);

                                        if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT && objTestValue == null && (tli.testcode == "UAPPE" || tli.testcode == "UCOLO" || tli.testcode == "SPGR"))
                                            continue;

                                        if (riResult.TestValue is NumericFieldInfo && msgConfig.AnalyzerTypeID != AnalyzerTypes.LABUMAT)
                                        {
                                            testValue = (decimal?)objTestValue;
                                        }

                                        else
                                        {
                                            System.IO.File.AppendAllText(System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "descriptive.txt"), testcode + "," + objTestValue + Environment.NewLine);
                                            //Below logic is for Clinitek Status Plus
                                            if (tli.testid != 0)
                                            {
                                                string T_Testcode = tli.testcode;

                                                var product_DescriptiveList = GetDescriptiveMapping(T_Testcode);
                                                if (product_DescriptiveList.Any())
                                                {
                                                    DescriptiveId = PopulateDescriptiveId(T_Testcode, objTestValue.ToString());

                                                    //To take values of sendiment results to verify below descriptive update for urised analyzers
                                                    if ((riResult.TestValue is StringFieldInfo) && msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT)
                                                    {
                                                        try
                                                        {
                                                            if (tli.testid != 84)
                                                            {
                                                                objTestValue2 = ExtractRecordValue(strRecordReceived, riResult, riResult.TestValue, i);
                                                                DescriptiveId = PopulateDescriptiveId(T_Testcode, objTestValue2.ToString());

                                                                testValuesUrinsed2 = objTestValue2.ToString();

                                                                if ((DescriptiveId == 0 || DescriptiveId == -1 || testValuesUrinsed2 != "") && unit == "p/HPF")
                                                                {
                                                                    decimal dcmValue = 0;
                                                                    dcmValue = Convert.ToDecimal(testValuesUrinsed2);

                                                                    testValuesUrinsed = Convert.ToDouble(dcmValue);
                                                                }
                                                            }

                                                            else if (tli.testid == 84)
                                                            {
                                                                objTestValue2 = ExtractRecordValue(strRecordReceived, riResult, riResult.TestValue, i);
                                                                if (new string[] { "(+)", "+", "-", "++", "+++", "++++", "neg", "pos", "norm" }.Contains(objTestValue2))
                                                                {
                                                                    DescriptiveId = PopulateDescriptiveId(T_Testcode, objTestValue2.ToString());
                                                                }
                                                                else
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            throw ex;
                                                        }

                                                    }

                                                    ////Logic is for Sysmex urine analyzer UWAM to take quantitative values 
                                                    //else if ((msgConfig.AnalyzerTypeID == 310 || msgConfig.AnalyzerTypeID == 47) && (DescriptiveId == 0 || DescriptiveId == -1)
                                                    //    && (tli.testcode == "UYST" || tli.testcode == "ULEUC" || tli.testcode == "UBACT" || tli.testcode == "UMAB" || tli.testcode == "UEPIT" || tli.testcode == "URBC" || 
                                                    //    tli.testcode == "UCRYS" || tli.testcode == "UCAST" || tli.testcode == "UMUC" || tli.testcode == "UWBCC" || tli.testcode == "UWBC" || tli.testcode == "USPRM" && tli.testcode == "UNIT"))
                                                    //{
                                                    //    if (tli.testcode != "UVOL" && tli.testcode != "UBPIG" && tli.testcode != "UBSAL" && tli.testcode != "SPGR")
                                                    //    {
                                                    //        testValuesUrinsed2 = objTestValue.ToString();
                                                    //        decimal dcmValue = 0;
                                                    //        dcmValue = Convert.ToDecimal(testValuesUrinsed2);

                                                    //        testValuesUrinsed = Convert.ToDouble(dcmValue);
                                                    //    }
                                                    //}
                                                    //Labumat urised machine logic to uodate the values
                                                    if (msgConfig.AnalyzerTypeID == AnalyzerTypes.LABUMAT)
                                                    {
                                                        // TEST CODE URBC Descritive values range And UWBC
                                                        if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed >= 0 && testValuesUrinsed <= 2)
                                                        {
                                                            DescriptiveId = 541;//0-2
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 2 && testValuesUrinsed <= 4)
                                                        {
                                                            DescriptiveId = 617;//2-4
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 4 && testValuesUrinsed <= 6)
                                                        {
                                                            DescriptiveId = 650;//4-6
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 6 && testValuesUrinsed <= 8)
                                                        {
                                                            DescriptiveId = 682;//6-8
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 8 && testValuesUrinsed <= 10)
                                                        {
                                                            DescriptiveId = 696;//8-10
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 10 && testValuesUrinsed <= 12)
                                                        {
                                                            DescriptiveId = 583;//10-12
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 12 && testValuesUrinsed <= 15)
                                                        {
                                                            DescriptiveId = 593;//12-15
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 15 && testValuesUrinsed <= 20)
                                                        {
                                                            DescriptiveId = 601;//15-20
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 20 && testValuesUrinsed <= 25)
                                                        {
                                                            DescriptiveId = 624;//20-25
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 25 && testValuesUrinsed <= 30)
                                                        {
                                                            DescriptiveId = 631;//25-30
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 30 && testValuesUrinsed <= 40)
                                                        {
                                                            DescriptiveId = 643;//30-40
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 40 && testValuesUrinsed <= 50)
                                                        {
                                                            DescriptiveId = 654;//40-50
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 50 && testValuesUrinsed <= 60)
                                                        {
                                                            DescriptiveId = 672;//50-60
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 60 && testValuesUrinsed <= 70)
                                                        {
                                                            DescriptiveId = 685;//60-70
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 70 && testValuesUrinsed <= 80)
                                                        {
                                                            DescriptiveId = 693;//70-80
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 80 && testValuesUrinsed <= 90)
                                                        {
                                                            DescriptiveId = 702;//80-90
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 || tli.testid == 100 || tli.testid == 101 || tli.testid == 107 && testValuesUrinsed > 90 && testValuesUrinsed <= 100)
                                                        {
                                                            DescriptiveId = 705;//90-100
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 && testValuesUrinsed > 100 && testValuesUrinsed <= 125)
                                                        {
                                                            DescriptiveId = 587;//100-125
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 98 && testValuesUrinsed > 125 && testValuesUrinsed <= 125)
                                                        {
                                                            DescriptiveId = 596;//125-150
                                                            testValue = -1;
                                                        }
                                                        // If values are gereater than 150 for URBC 
                                                        else if (tli.testid == 98 && testValuesUrinsed > 150)
                                                        {
                                                            DescriptiveId = 836;//too numerous to count
                                                            testValue = -1;
                                                        }
                                                        // If values are gereater than 100 for UWBC 
                                                        else if (tli.testid == 100 || tli.testid == 101 && testValuesUrinsed > 100)
                                                        {
                                                            DescriptiveId = 836;//too numerous to count
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For EPITHELIAL CELLS (USQEP	Squamous epithelia)

                                                        else if (tli.testid == 105 && testValuesUrinsed <= 0.50)
                                                        {
                                                            DescriptiveId = 796;//NONE SEEN
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 0.50 && testValuesUrinsed <= 1.00)
                                                        {
                                                            DescriptiveId = 827;//SCANTY
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 1.00 && testValuesUrinsed <= 1.14)
                                                        {
                                                            DescriptiveId = 764;//FEW
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 1.14 && testValuesUrinsed <= 5.68)
                                                        {
                                                            DescriptiveId = 506;//+
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 5.68 && testValuesUrinsed <= 17.05)
                                                        {
                                                            DescriptiveId = 507;//++
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 17.05 && testValuesUrinsed <= 27.27)
                                                        {
                                                            DescriptiveId = 508;//+++
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 105 && testValuesUrinsed > 27.27)
                                                        {
                                                            DescriptiveId = 509;//++++
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For CRYSTALS (CRY) - (UCRYS) And CALCIUM OXALATE (ADD CaOxm and CaOxd VALUES)  -- (UCaOX) And TRIPLE PHOSPHATE (TRI) (UTRIP) And URIC ACID (URI)--()
                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed <= 0.50)
                                                        {
                                                            DescriptiveId = 792;//Nill
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed > 0.50 && testValuesUrinsed <= 1.36)
                                                        {
                                                            DescriptiveId = 764;//FEW
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed > 1.36 && testValuesUrinsed <= 4.09)
                                                        {
                                                            DescriptiveId = 506;//+
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed > 4.09 && testValuesUrinsed <= 13.64)
                                                        {
                                                            DescriptiveId = 507;//++
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed > 13.64 && testValuesUrinsed <= 30)
                                                        {
                                                            DescriptiveId = 508;//+++
                                                            testValue = -1;
                                                        }

                                                        else if (tli.testid == 169 || tli.testid == 109 || tli.testid == 106 || tli.testid == 102 && testValuesUrinsed > 30)
                                                        {
                                                            DescriptiveId = 509;//++++
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For HYALIN CAST -- (UHYA) 
                                                        else if (tli.testid == 108 && testValuesUrinsed <= 0.50)
                                                        {
                                                            DescriptiveId = 792;//Nill
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 108 && testValuesUrinsed > 0.50 && testValuesUrinsed <= 1)
                                                        {
                                                            DescriptiveId = 540;//0-1
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 108 && testValuesUrinsed > 1 && testValuesUrinsed <= 2)
                                                        {
                                                            DescriptiveId = 556;//1-2
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 108 && testValuesUrinsed > 2 && testValuesUrinsed <= 3)
                                                        {
                                                            DescriptiveId = 616;//2-3
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 108 && testValuesUrinsed > 3 && testValuesUrinsed <= 4)
                                                        {
                                                            DescriptiveId = 638;//3-4
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 108 && testValuesUrinsed > 4 && testValuesUrinsed <= 6)
                                                        {
                                                            DescriptiveId = 650;//4-6
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For PATHOLOGICAL CAST
                                                        else if (tli.testid == 103 && testValuesUrinsed <= 0.34)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 103 && testValuesUrinsed > 0.34 && testValuesUrinsed <= 1)
                                                        {
                                                            DescriptiveId = 540;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 103 && testValuesUrinsed > 1 && testValuesUrinsed <= 2)
                                                        {
                                                            DescriptiveId = 556;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 103 && testValuesUrinsed > 2 && testValuesUrinsed <= 3)
                                                        {
                                                            DescriptiveId = 616;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 103 && testValuesUrinsed > 3 && testValuesUrinsed <= 4)
                                                        {
                                                            DescriptiveId = 638;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 103 && testValuesUrinsed > 4 && testValuesUrinsed <= 6)
                                                        {
                                                            DescriptiveId = 650;
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For YEAST (YEA) -- (UYST)
                                                        else if (tli.testid == 99 && testValuesUrinsed <= 0.35)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 99 && testValuesUrinsed > 0.35 && testValuesUrinsed <= 0.68)
                                                        {
                                                            DescriptiveId = 764;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 99 && testValuesUrinsed > 0.68 && testValuesUrinsed <= 2.27)
                                                        {
                                                            DescriptiveId = 506;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 99 && testValuesUrinsed > 2.27 && testValuesUrinsed <= 4.54)
                                                        {
                                                            DescriptiveId = 507;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 99 && testValuesUrinsed > 4.54 && testValuesUrinsed <= 11.36)
                                                        {
                                                            DescriptiveId = 508;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 99 && testValuesUrinsed > 11.36)
                                                        {
                                                            DescriptiveId = 509;
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For BACTERIA (BAC) -- (UBACR & UBACC)
                                                        else if (tli.testid == 110 || tli.testid == 111 && testValuesUrinsed <= 29.55)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 110 || tli.testid == 111 && testValuesUrinsed > 15.00 && testValuesUrinsed <= 29.55)
                                                        {
                                                            DescriptiveId = 764;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 110 || tli.testid == 111 && testValuesUrinsed > 29.55 && testValuesUrinsed <= 75)
                                                        {
                                                            DescriptiveId = 506;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 110 || tli.testid == 111 && testValuesUrinsed > 75 && testValuesUrinsed <= 300)
                                                        {
                                                            DescriptiveId = 507;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 110 || tli.testid == 111 && testValuesUrinsed > 300)
                                                        {
                                                            DescriptiveId = 508;
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For MUCUS (MUC)--(UMUC)
                                                        else if (tli.testid == 97 && testValuesUrinsed <= 60)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 97 && testValuesUrinsed > 30 && testValuesUrinsed <= 60)
                                                        {
                                                            DescriptiveId = 764;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 97 && testValuesUrinsed > 60 && testValuesUrinsed <= 150)
                                                        {
                                                            DescriptiveId = 506;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 97 && testValuesUrinsed > 150 && testValuesUrinsed <= 250)
                                                        {
                                                            DescriptiveId = 507;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 97 && testValuesUrinsed > 250)
                                                        {
                                                            DescriptiveId = 508;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For SPERMATOZOA (SPRM) -- (USPRM)
                                                        else if (tli.testid == 104 && testValuesUrinsed <= 2.73)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 104 && testValuesUrinsed > 1.30 && testValuesUrinsed <= 2.73)
                                                        {
                                                            DescriptiveId = 764;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 104 && testValuesUrinsed > 2.73 && testValuesUrinsed <= 14.09)
                                                        {
                                                            DescriptiveId = 506;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 104 && testValuesUrinsed > 14.09 && testValuesUrinsed <= 43.18)
                                                        {
                                                            DescriptiveId = 507;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 104 && testValuesUrinsed > 43.18)
                                                        {
                                                            DescriptiveId = 508;
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For BILIRUBIN (BIL)  -- (UBIL)
                                                        else if (tli.testid == 79 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For UROBILINOGEN (UBG) -- (UBNGN)
                                                        else if (tli.testid == 81 && DescriptiveId == 798)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For KETONE (KET) -- (UKET)
                                                        else if (tli.testid == 77 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 77 && DescriptiveId == 494)
                                                        {
                                                            DescriptiveId = 837;
                                                            testValue = -1;
                                                        }

                                                        //Descritive values range For GLUCOSE (GLU) -- (UGLU)
                                                        else if (tli.testid == 83 && DescriptiveId == 798)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 83 && DescriptiveId == 494)
                                                        {
                                                            DescriptiveId = 837;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For PROTEIN(PRO) -- (UPROT)
                                                        else if (tli.testid == 82 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For BLOOD (BLD) -- (UBLD)
                                                        else if (tli.testid == 80 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 80 && DescriptiveId == 494)
                                                        {
                                                            DescriptiveId = 837;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For NITRITE(NIT) -- (UNIT)
                                                        else if (tli.testid == 84 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 84 && DescriptiveId == 506)
                                                        {
                                                            DescriptiveId = 3;
                                                            testValue = -1;
                                                        }
                                                        //Descritive values range For PROTEIN (PRO) -- (UPROT)
                                                        else if (tli.testid == 82 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 792;
                                                            testValue = -1;
                                                        }
                                                        else if (tli.testid == 78 && DescriptiveId == 2)
                                                        {
                                                            DescriptiveId = 2;
                                                            testValue = -1;
                                                        }

                                                        //For 0 value data for Labumat Urine Analyzers
                                                        if (msgConfig.AnalyzerTypeID == 5)
                                                        {
                                                            testValue = -1;
                                                        }
                                                    }

                                                    else
                                                    {
                                                        System.IO.File.AppendAllText(System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "descriptive_NOTFOUND.txt"), T_Testcode + "," + objTestValue + Environment.NewLine);
                                                        UiMediator.LogAndShowError(AnalyzerID, new Exception("Not found in Descriptive master: " + T_Testcode + ":" + objTestValue.ToString() + valueErrorDescription));
                                                    }
                                                }

                                                else
                                                {
                                                    testValue = Convert.ToDecimal(objTestValue);
                                                }
                                            }

                                            if (DescriptiveId == 0 && objTestValue != null)
                                            {
                                                string remainingValuePart = "";
                                                testValue = InterfaceHelper.ExtractNumericValue(objTestValue.ToString(), out remainingValuePart);
                                                //if (msgConfig.AnalyzerTypeID == 55)
                                                //{
                                                //    unit = remainingValuePart.Trim();
                                                //}
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Data.Contains("RawValue"))
                                        {
                                            strValue = ex.Data["RawValue"] as string;
                                            if (!String.IsNullOrEmpty(strValue))
                                            {
                                                string remainingValuePart = "";
                                                //if (msgConfig.AnalyzerTypeID == 203 || msgConfig.AnalyzerTypeID == 24)
                                                //{
                                                //    if (strValue.Contains("^") && strValue.Split('^')[1].Trim() != "")
                                                //        testValue = decimal.Parse(strValue.Split('^')[1]);
                                                //}
                                                //else
                                                //{
                                                testValue = InterfaceHelper.ExtractNumericValue(strValue, out remainingValuePart);

                                                //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.AutoBio_A1000)
                                                //    prefix = remainingValuePart;

                                                // }

                                                if (new int[] { 1/*, 2, 66, 50*/ }.Contains(msgConfig.AnalyzerTypeID))
                                                {
                                                    resultAbnormalFlag = remainingValuePart.Trim();
                                                }
                                                if (!testValue.HasValue)
                                                {
                                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Invalid result value received '" + strValue + "'." + valueErrorDescription);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            UiMediator.LogAndShowError(AnalyzerID, ex, "Error in parsing result value." + valueErrorDescription);
                                        }
                                    }

                                    if (String.IsNullOrEmpty(unit))
                                    {
                                        unit = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.Units, i);
                                    }
                                    if (String.IsNullOrEmpty(resultAbnormalFlag))
                                    {
                                        resultAbnormalFlag = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.ResultAbnormalFlags, i);
                                    }

                                    if (msgConfig.AnalyzerTypeID == 5 && !String.IsNullOrEmpty(resultAbnormalFlag))
                                    {
                                        if (testValue.HasValue && Math.Abs(testValue.Value) == 0.01M && resultAbnormalFlag.Trim() == "%")
                                        {
                                            reasonForTestcodeRemoval = "Value 0.01 with prefix %";
                                            goto removeTestCode;
                                        }
                                    }
                                }

                                //if (new int[] { 1, 50 }.Contains(msgConfig.AnalyzerTypeID)) //Centaur XP: take system date, as analyzer date is incorrect
                                //    resultedDateTime = DateTime.Now;
                                //else
                                resultedDateTime = (DateTime?)ExtractRecordValue(strRecordReceived, riResult, riResult.ResultedDateTime, i);

                                //Add time value, when Resulted Date and Time are in separate fields
                                if (riResult.ResultedTime != null && ((DateTime)resultedDateTime).TimeOfDay.TotalMilliseconds == 0)
                                {
                                    object objResultedTime = ExtractRecordValue(strRecordReceived, riResult, riResult.ResultedTime, i);
                                    if (objResultedTime != null)
                                        resultedDateTime = ((DateTime)resultedDateTime).Add(((DateTime)objResultedTime).TimeOfDay);
                                }

                                if (riResult.ResultedDateTime == null || resultedDateTime == null)
                                    resultedDateTime = DateTime.Now;

                                if (Math.Abs(resultedDateTime.Value.Subtract(DateTime.Now).TotalHours) > 6)
                                {
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Too much difference found in System Time and Resulted Time. Kindly check datetime setting of LIS PC or Analyzer, if actual result time is not old.");
                                }

                                string InstrumentIdKey = (string)ExtractRecordValue(strRecordReceived, riResult, riResult.InstrumentIdKey, i);
                                int AnalyzerIdForResult = AnalyzerID;

                                //Added logic to update the values by it's instrument id 
                                //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Turbo_Variant_II)
                                //{
                                //    if (InstrumentIdKey == "1")
                                //    {
                                //        AnalyzerIdForResult = Convert.ToInt32(ConfigurationManager.AppSettings["Machine1"]);
                                //    }
                                //    else if (InstrumentIdKey == "2")
                                //    {
                                //        AnalyzerIdForResult = Convert.ToInt32(ConfigurationManager.AppSettings["Machine2"]);
                                //    }
                                //}
                                //if (msgConfig.AnalyzerTypeID == 153)
                                //{
                                //    AnalyzerIdForResult = AnalyzerID;
                                //}

                                if (msgConfig.AnalyzerTypeID != 153)
                                {
                                    if (!String.IsNullOrEmpty(InstrumentIdKey) && CachedData.Analyzer.ModuleAnalyzers.Any())
                                    {
                                        AnalyzerIdForResult = CachedData.Analyzer.ModuleAnalyzers.Where(r => r.ModularIdentificationCode.Equals(InstrumentIdKey, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault()?.AnalyzerId ?? AnalyzerID;
                                    }
                                }

                                //if (msgConfig.AnalyzerTypeID == 59)
                                //{
                                //    foreach (string str in testcode.Split(';'))
                                //    {
                                //        TextLogger.WriteLogEntry("Debugging", "Message Processor" + str);
                                //        string testcode1 = str.Split(':')[0];
                                //        string testValue1 = str.Split(':')[1];
                                //        DescriptiveId = PopulateDescriptiveIdByMachineCode(testcode1, testValue1);
                                //        if (DescriptiveId != -1)
                                //        {
                                //            AddResultRecord(barcode, testcode1, -1, unit, resultAbnormalFlag, resultedDateTime, descriptiveId: DescriptiveId, additionalInfo: "", AnalyzerIDForResult: AnalyzerIdForResult);
                                //        }
                                //    }
                                //}

                                //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.AutoBio_A1000)
                                //{
                                //    AddResultRecord(barcode, testcode, testValue, unit, prefix, resultedDateTime, descriptiveId: DescriptiveId, additionalInfo: "", AnalyzerIDForResult: AnalyzerIdForResult);
                                //}

                                if (reasonForTestcodeRemoval == "" && msgConfig.AnalyzerTypeID != AnalyzerTypes.Biored_Variant_II)
                                    AddResultRecord(barcode, testcode, testValue, unit, resultAbnormalFlag, resultedDateTime, descriptiveId: DescriptiveId, additionalInfo: "", AnalyzerIDForResult: AnalyzerIdForResult);


                                else
                                {
                                    if (msgConfig.AnalyzerTypeID == 153)
                                        AnalyzerIdForResult = AnalyzerID;

                                    if (reasonForTestcodeRemoval == "")
                                        AddResultRecord(barcode, testcode, testValue, unit, resultAbnormalFlag, resultedDateTime, descriptiveId: DescriptiveId, additionalInfo: "", AnalyzerIDForResult: AnalyzerIdForResult);
                                }
                            //UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Incoming, "Result record Received " + (String.IsNullOrEmpty(testDetail.testCode) ? testDetail.machineCode : testDetail.testCode) + " with value " + testValue /*+ "\n"*/);
                            //string additionalInfo = !String.IsNullOrEmpty(resultAbnormalFlag) ? "\"" + resultAbnormalFlag + "\"" : "";
                            //UiMediator.AddUiGridData(AnalyzerID, barcode, (!testValue.HasValue ? "BLANK " : "") + "RESULT", testcode, testValue, additionalInfo);

                            removeTestCode:
                                if (reasonForTestcodeRemoval != "")
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Testcode removed, " + reasonForTestcodeRemoval + " " + valueErrorDescription + ".");
                            }
                            catch (Exception ex)
                            {
                                UiMediator.LogAndShowError(AnalyzerID, ex, "Error getting test result value." + valueErrorDescription);
                            }
                        }
                        //decimal v = (decimal)ExtractRecordValue(tmp, msgConfig.ResultRecordInfo, msgConfig.ResultRecordInfo.TestValue);
                        //resultedDateTime = (DateTime)ExtractRecordValue(tmp, msgConfig.ResultRecordInfo, msgConfig.ResultRecordInfo.ResultedDateTime);
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.CommentRecordInfo))
                    {
                        RecordInfo riComment = msgConfig.CommentRecordInfo.Copy();
                        string commentCode = (string)ExtractRecordValue(strRecordReceived, riComment, riComment.CommentCode);
                        string commentText = (String.IsNullOrEmpty(commentCode) ? "" : commentCode + ":")
                            + (string)ExtractRecordValue(strRecordReceived, riComment, riComment.CommentText);

                        if (!String.IsNullOrEmpty(commentCode) && commentCode != "0")
                        {
                            //drResult["COMMENT_TEXTS"] = (drResult["COMMENT_TEXTS"] != DBNull.Value && !String.IsNullOrEmpty(drResult["COMMENT_TEXTS"].ToString()) ? drResult["COMMENT_TEXTS"].ToString() + "~" : "") + commentText;
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Comment record Received " + commentText /*+ "\n"*/);
                            //UiMediator.AddUiGridData(AnalyzerID, barcode, "COMMENT", commentText, null, "", "");
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.TerminationRecordInfo))
                    {

                    }
                }


                try
                {
                    if (queryBarcodes.Any())
                    {
                        string barcodesCommaSeparated = String.Join(",", queryBarcodes);
                        TextLogger.WriteLogEntry("Debugging", "Query received for barcode: " + barcodesCommaSeparated);
                        List<BarcodeList> bList = null;
                        int mode = 0;
                        try
                        {
                            bList = GetWorklist(queryBarcodes, mode)?.samples;
                            TextLogger.WriteLogEntry("Debugging", "WO fetched for barcode: " + barcodesCommaSeparated);
                        }
                        catch (Exception ex)
                        {
                            //if (ex is TimeoutException && msgConfig.AnalyzerTypeID == 41)
                            //{
                            //    //set to null, as for AU58000 do not send reply, then analyzer sends query for same barcode and then send from stored WO
                            //    bList = null;
                            //}
                            //else
                            //{
                            bList = new List<BarcodeList>();
                            //}
                            string strBarcodes = String.Join(", ", queryBarcodes);
                            ex.Data.Add("Barcodes", strBarcodes);
                            UiMediator.LogAndShowError(Program.AnalyzerId, ex, "Sample Skipped: " + strBarcodes + " (" + ex.Message + ")");
                        }

                        if (bList != null && !msgConfig.IsHL7)
                        {
                            List<string> worklistMessages = PrepareWorklistMessages(queryBarcodes, bList);
                            records.AddRange(worklistMessages);
                            TextLogger.WriteLogEntry("Debugging", records.Count + " records generated for barcode: " + barcodesCommaSeparated);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UiMediator.LogAndShowError(AnalyzerID, ex, "Error getting order info");
                }

                if (dtResults.Rows.Count > 0 || (testResults != null && testResults.results.Count > 0))
                {
                    new System.Threading.Thread(() =>
                    {
                        try
                        {
                            int testsUpdated = UpdateTestValues();
                            dtResults.Rows.Clear();
                            testResults = null;
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Outgoing, testsUpdated + " Results updated." /*+ "\n"*/);
                        }
                        catch (Exception ex)
                        {
                            if (msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in updating result values");
                        }
                    }).Start();

                }
            }
            catch (Exception ex)
            {
                if (msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                    UiMediator.LogAndShowError(AnalyzerID, ex, "Error processing message");
            }

            PrepareFinalRecords(records);

            return records;
        }

        #region HL7 Code Base

        public List<string> ProcessCompleteRecordHL7(List<string> receivedData/*, out string WOBarcode_Single*/)
        {
            List<string> records = new List<string>();

            //WOBarcode_Single = "";
            try
            {
                string barcode = String.Empty;
                string testcode = "";
                decimal? testValue = null;
                string testValuesUrinsed2 = "";
                double testValuesUrinsed = 0;
                object objTestValue = "";
                object objTestValue2 = "";
                string unit = string.Empty;
                string resultAbnormalFlag = string.Empty;
                string prefix = string.Empty;
                string reasonForTestcodeRemoval = "";
                DateTime? resultedDateTime = null;
                string strValue = "";
                int DescriptiveId = -1;
                string resultflag = "";
                DataRow drResult = null;


                dtResults = GetResultsTableStructure();
                string[] incoming = receivedData.ToArray(); // .Split(Convert.ToChar(Characters.CR));

                string testheader = incoming[0];
                chache = new MemoryCache(testheader);


                //moved variable clearing logic outside of loop, so that they will be cleared only once, to support continuous messages having Header and Last records inbetween
                queryBarcodes.Clear();
                CopyValues.Clear();
                //dtResults = null;
                testResults = null;


                foreach (string s in incoming)
                {
                    string strRecordReceived = s.Trim(Convert.ToChar(Characters.CR)).Trim(Convert.ToChar(Characters.ENQ)).Trim(Convert.ToChar(Characters.STX)); //s.Replace("\\n", string.Empty).Trim();

                    if(msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                    strRecordReceived = strRecordReceived.TrimStart();

                    //Header Preparation and Value Copying which needs to resend in response 
                    #region Header Preparation and Value Copying which needs to resend in response 
                    if (msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6 && msgConfig.AnalyzerTypeID != AnalyzerTypes.MISPA_CX4) {
                        if (strRecordReceived.Contains("MSH"))
                            CollectCopyValues(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACK);
                        if (strRecordReceived.Contains("MSH"))
                            CollectCopyValuesHL7(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKMSATEST);
                        if (strRecordReceived.Contains("MSH"))
                            CollectCopyValuesHL7OKRes(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKResponseOrder);
                        if (strRecordReceived.Contains("MSH"))
                            CollectCopyValuesHL7OKResResults(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKMSAReqToResults);
                    }
                  
                    if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
                    {
                        if (strRecordReceived.Contains("MSH"))
                        {
                            strHeader = strRecordReceived;
                            CollectCopyValues(strRecordReceived, msgConfig.HeaderRecordInfoHL7);
                            CollectCopyValues(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKResponse);
                        }
                        if (strRecordReceived.Contains("QRD"))
                        {
                            strQrdData = strRecordReceived;
                            CollectCopyValuesHL7(strRecordReceived, msgConfig.QueryDefinitionSegmentFromMachine);
                        }

                    }
                    #endregion

                    if (IsRecordTypeMatch(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKResponseOrder))
                    {
                        strHeader = strRecordReceived;
                        try
                        {
                            //CollectCopyValuesHL7OKRes(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACKResponseOrder);
                            //ExtractRecordValueHL7OKRes
                            //CollectCopyValues(strRecordReceived, msgConfig.HeaderRecordInfoHL7ACK);
                            //AddHeaderRecord(strHeader);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }

                        if ((string)ExtractRecordValueHL7(strRecordReceived, msgConfig.HeaderRecordInfoHL7, msgConfig.HeaderRecordInfoHL7.CommentOrSpecialInstructions) == "MSA")
                        {
                            return new List<string> { };
                        }
                    }

                    else if (msgConfig.SupportsOnlyQuery || IsRecordTypeMatch(strRecordReceived, msgConfig.QueryRecordInfoHL7))
                    {
                        strQuery = strRecordReceived;
                        try
                        {
                            int barcodesCount = 1;
                            riQuery_Incoming = msgConfig.QueryRecordInfoHL7.Copy();

                            CollectCopyValues(strRecordReceived, riQuery_Incoming);
                            if (!msgConfig.IsFieldSizeInBytes)
                            {
                                barcodesCount = GetRepeatValuesCount(strRecordReceived, riQuery_Incoming.SampleID);
                            }
                            else
                            {
                                if (msgConfig.SupportsMultipleBarcodes)
                                {
                                    barcodesCount = (int)ExtractRecordValueHL7(strRecordReceived, riQuery_Incoming, riQuery_Incoming.NumberOfInquirySamplesInBlock);
                                }
                            }

                            for (int i = 1; i <= barcodesCount; i++)
                            {

                                barcode = (string)ExtractRecordValueHL7(strRecordReceived, riQuery_Incoming, riQuery_Incoming.SampleID, i);

                                if (barcode == null) //for Centaur XP, sends blank barcode query at last
                                    barcode = "";

                                if (String.IsNullOrEmpty(barcode))
                                {
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Empty barcode received");
                                }
                                else
                                {
                                    queryBarcodes.Add(barcode);
                                    UiMediator.AddUiGridData(AnalyzerID, barcode, "QUERY", string.Empty, null, "", "");
                                }
                            }

                            SortingRule = (string)ExtractRecordValueHL7(strRecordReceived, riQuery_Incoming, riQuery_Incoming.SortingRule);
                        }
                        catch (MyException ex)
                        {
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Barcode could not be read from query message" /*+ "\n"*/);
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.PatientRecordInfoHL7))
                    {
                        drResult = null;
                        riPatient = msgConfig.PatientRecordInfoHL7.Copy();

                        if (riPatient.SampleID != null)
                        {
                            barcode = (string)ExtractRecordValueHL7(strRecordReceived, riPatient, riPatient.SampleID);

                            if (barcode == null)
                                barcode = "";
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.OrderRecordInfoHL7SPM))
                    {
                        drResult = null;
                        riOrder = msgConfig.OrderRecordInfoHL7SPM.Copy();

                        if (riOrder.SampleID != null)
                        {

                            barcode = (string)ExtractRecordValueHL7(strRecordReceived, riOrder, riOrder.SampleID);

                            if (barcode == null)
                                barcode = "";

                            barcode = barcode.Trim(); //for Sysmex XN 1000

                            queryBarcodesForResults.Add(barcode);

                            // Logic to response for results to cobas machine
                            List<string> worklistMessages = PrepareResultResponseMessagesHL7();
                            records.AddRange(worklistMessages);
                            PrepareFinalRecordsHL7Results(records);
                        }
                    }
                    //Zybio EXZ 6000 H6
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.QueryDefinitionSegmentFromMachine))
                    {
                        drResult = null;
                        riOrder = msgConfig.QueryDefinitionSegmentFromMachine.Copy();

                        if (riOrder.SampleID != null)
                        {
                            barcode = (string)ExtractRecordValueHL7(strRecordReceived, riOrder, riOrder.SampleID);
                            if (barcode == null)
                                barcode = "";
                            barcode = barcode.Trim();

                            queryBarcodes.Add(barcode);

                            List<string> worklistMessages = PrepareResultResponseMessagesHL7();
                            records.AddRange(worklistMessages);
                            PrepareFinalRecordsHL7Results(records);
                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.OrderRecordInfoHL7OBR))
                    {
                        drResult = null;
                        riOrder = msgConfig.OrderRecordInfoHL7OBR.Copy();

                        if ((msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6) && riOrder.SampleID != null)
                        {

                            barcode = (string)ExtractRecordValueHL7(strRecordReceived, riOrder, riOrder.SampleID);

                            if (barcode == null)
                                barcode = "";

                            barcode = barcode.Trim(); //for Sysmex XN 1000

                            queryBarcodesForResults.Add(barcode);

                            List<string> worklistMessages = PrepareResultResponseMessagesHL7_ACK();
                            records.AddRange(worklistMessages);
                            PrepareFinalRecordsHL7Results(records);

                        }
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.ResultRecordInfoHL7) || IsRecordTypeMatch(strRecordReceived, msgConfig.QcResultRecordInfo))
                    {
                        int resultsCount = 1;
                        bool isQcResult = false;
                        if (IsRecordTypeMatch(strRecordReceived, msgConfig.QcResultRecordInfo))
                        {
                            isQcResult = true;
                            riResult = msgConfig.QcResultRecordInfo.Copy();
                        }
                        else
                        {
                            riResult = msgConfig.ResultRecordInfoHL7.Copy();
                        }

                        if (String.IsNullOrEmpty(barcode) && riResult.SampleID != null)
                        {
                            barcode = (string)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.SampleID);


                        }

                        int blockNumber = 0;
                        int numberOfTestsInBlock = 1;
                        if (riResult.BlockNumber != null)
                        {
                            blockNumber = (int)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.BlockNumber);
                            //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Advia_1800 && blockNumber > 1)
                            //{
                            //    int indexNumberForInsertingString = GetFieldValueStartIndex(riResult, riResult.SampleID) + riResult.SampleID.BytesLength;
                            //    strRecordReceived = strRecordReceived.Insert(indexNumberForInsertingString, new string(' ', 50));
                            //}
                        }

                        if (riResult.NumberOfTestsInBlock != null)
                        {
                            resultsCount = numberOfTestsInBlock = (int)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.NumberOfTestsInBlock);
                        }

                        if (!msgConfig.IsFieldSizeInBytes)
                        {
                            //resultsCount = 1;
                        }

                        int sequenceNumber = -1;

                        if (!msgConfig.IsFieldSizeInBytes && riResult.SequenceNumber != null && riResult.SequenceNumber.FieldNumber > -1)
                        {
                            sequenceNumber = (int)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.SequenceNumber);
                        }

                        List<string> list = new List<string>();
                        for (int i = 1; i <= resultsCount; i++)
                        {
                            TestListItem tli = null;
                            string valueErrorDescription = "";
                            try
                            {
                                testcode = "";
                                testValue = null;
                                objTestValue = "";
                                objTestValue2 = "";
                                testValuesUrinsed2 = "";
                                testValuesUrinsed = 0;
                                unit = string.Empty;
                                resultflag = "";
                                resultAbnormalFlag = string.Empty;
                                reasonForTestcodeRemoval = "";
                                resultedDateTime = null;
                                DescriptiveId = -1;
                                strValue = "";


                                testcode = (string)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.TestID_ManufacturersTestCode, i);

                                tli = GetTestMappingOrEmpty(testcode);
                                
                                string resulttype = (string)ExtractRecordValueHL7(strRecordReceived, riResult, msgConfig.ResultRecordInfoHL7.Resulttype, i);

                                string resultAspects = (string)ExtractRecordValueHL7(strRecordReceived, riResult, msgConfig.ResultRecordInfoHL7.ResultAspects, i);

                                //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA && resulttype != "NM")
                                //    continue;

                                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA && (!resultAspects.Contains("DOSE") && !resultAspects.Contains("INDX")))
                                    continue;

                                if (msgConfig.AnalyzerTypeID != 59)
                                {
                                    valueErrorDescription = " (Barcode: " + barcode + ", Test: " + (tli?.testcode ?? testcode) + ")";

                                    if (msgConfig.RemoveUnmappedTestcodes && !CachedData.TestList.Where(r => r.instrumentcode == testcode).Any())
                                    {
                                        reasonForTestcodeRemoval = "Testcode mapping not found";
                                        goto removeTestCode;
                                    }

                                    try
                                    {
                                        objTestValue = ExtractRecordValueHL7(strRecordReceived, riResult, riResult.TestValue, i);
                                        if (riResult.TestValue is NumericFieldInfo && (msgConfig.AnalyzerTypeID != 310))
                                        {
                                            testValue = (decimal?)objTestValue;
                                        }

                                        else
                                        {
                                            System.IO.File.AppendAllText(System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "descriptive.txt"), testcode + "," + objTestValue + Environment.NewLine);
                                            //Below logic is for Clinitek Status Plus
                                            if (tli.testid != 0)
                                            {
                                                string T_Testcode = tli.testcode;

                                                var product_DescriptiveList = GetDescriptiveMapping(T_Testcode);
                                                if (product_DescriptiveList.Any())
                                                {
                                                    DescriptiveId = PopulateDescriptiveId(T_Testcode, objTestValue.ToString());

                                                    //Logic is for Sysmex urine analyzer UWAM to take quantitative values 
                                                    if ((msgConfig.AnalyzerTypeID == 310 || msgConfig.AnalyzerTypeID == 47 || msgConfig.AnalyzerTypeID == 314) && (DescriptiveId == 0 || DescriptiveId == -1)
                                                        && (tli.testcode == "UMAB" || tli.testcode == "UWBCC" || tli.testcode == "UWBC" || tli.testcode == "USPRM" && tli.testcode == "UNIT"))
                                                    {
                                                        if (tli.testcode != "UVOL" && tli.testcode != "SPGR")
                                                        {
                                                            testValuesUrinsed2 = objTestValue.ToString();
                                                            decimal dcmValue = 0;
                                                            dcmValue = Convert.ToDecimal(testValuesUrinsed2);

                                                            testValuesUrinsed = Convert.ToDouble(dcmValue);
                                                        }
                                                    }

                                                    else
                                                    {
                                                        System.IO.File.AppendAllText(System.IO.Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "descriptive_NOTFOUND.txt"), T_Testcode + "," + objTestValue + Environment.NewLine);
                                                        UiMediator.LogAndShowError(AnalyzerID, new Exception("Not found in Descriptive master: " + T_Testcode + ":" + objTestValue.ToString() + valueErrorDescription));
                                                    }
                                                }
                                                //To Capture exact numbers from these 3 machine if values are numeric
                                                else if (msgConfig.AnalyzerTypeID == 310 || msgConfig.AnalyzerTypeID == 47 || msgConfig.AnalyzerTypeID == 21 || msgConfig.AnalyzerTypeID == 314)
                                                {
                                                    testValue = Convert.ToDecimal(testValuesUrinsed);
                                                }
                                                else
                                                {
                                                    testValue = Convert.ToDecimal(objTestValue);
                                                }
                                            }

                                            if (DescriptiveId == 0 && objTestValue != null)
                                            {
                                                string remainingValuePart = "";
                                                testValue = InterfaceHelper.ExtractNumericValue(objTestValue.ToString(), out remainingValuePart);
                                                if (msgConfig.AnalyzerTypeID == 55)
                                                {
                                                    unit = remainingValuePart.Trim();
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Data.Contains("RawValue"))
                                        {
                                            strValue = ex.Data["RawValue"] as string;
                                            if (!String.IsNullOrEmpty(strValue))
                                            {
                                                string remainingValuePart = "";
                                                if (msgConfig.AnalyzerTypeID == 203 || msgConfig.AnalyzerTypeID == 24)
                                                {
                                                    if (strValue.Contains("^") && strValue.Split('^')[1].Trim() != "")
                                                        testValue = decimal.Parse(strValue.Split('^')[1]);
                                                }
                                                else
                                                {
                                                    testValue = InterfaceHelper.ExtractNumericValue(strValue, out remainingValuePart);
                                                }

                                                if (!testValue.HasValue)
                                                {
                                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "Invalid result value received '" + strValue + "'." + valueErrorDescription);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            UiMediator.LogAndShowError(AnalyzerID, ex, "Error in parsing result value." + valueErrorDescription);
                                        }
                                    }

                                    if (String.IsNullOrEmpty(unit))
                                    {
                                        unit = (string)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.Units, i);
                                    }
                                    if (String.IsNullOrEmpty(resultAbnormalFlag))
                                    {
                                        if(msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
                                            resultAbnormalFlag = (string)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.ResultAbnormalFlags, i);
                                        else
                                            resultAbnormalFlag = (string)ExtractRecordValueHL7Prefix(strRecordReceived, riResult, riResult.TestValue, i);

                                    }
                                }
                                resultedDateTime = (DateTime?)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.ResultedDateTime, i);

                                //Add time value, when Resulted Date and Time are in separate fields
                                if (riResult.ResultedTime != null && ((DateTime)resultedDateTime).TimeOfDay.TotalMilliseconds == 0)
                                {
                                    object objResultedTime = ExtractRecordValueHL7(strRecordReceived, riResult, riResult.ResultedTime, i);
                                    if (objResultedTime != null)
                                        resultedDateTime = ((DateTime)resultedDateTime).Add(((DateTime)objResultedTime).TimeOfDay);
                                }

                                if (riResult.ResultedDateTime == null || resultedDateTime == null)
                                    resultedDateTime = DateTime.Now;

                                if (Math.Abs(resultedDateTime.Value.Subtract(DateTime.Now).TotalHours) > 6)
                                {
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Too much difference found in System Time and Resulted Time. Kindly check datetime setting of LIS PC or Analyzer, if actual result time is not old.");
                                }

                                string InstrumentIdKey = (string)ExtractRecordValueHL7(strRecordReceived, riResult, riResult.InstrumentIdKey, i);
                                int AnalyzerIdForResult = AnalyzerID;

                                if (!String.IsNullOrEmpty(InstrumentIdKey) && CachedData.Analyzer.ModuleAnalyzers.Any())
                                {
                                    AnalyzerIdForResult = CachedData.Analyzer.ModuleAnalyzers.Where(r => r.ModularIdentificationCode.Equals(InstrumentIdKey, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault()?.AnalyzerId ?? AnalyzerID;
                                }
                                else
                                {
                                    if (reasonForTestcodeRemoval == "")
                                        AddResultRecord(barcode, testcode, testValue, unit, resultAbnormalFlag, resultedDateTime, descriptiveId: DescriptiveId, additionalInfo: "", AnalyzerIDForResult: AnalyzerIdForResult);
                                }
                            removeTestCode:
                                if (reasonForTestcodeRemoval != "")
                                    UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Testcode removed, " + reasonForTestcodeRemoval + " " + valueErrorDescription + ".");
                            }
                            catch (Exception ex)
                            {
                                UiMediator.LogAndShowError(AnalyzerID, ex, "Error getting test result value." + valueErrorDescription);
                            }
                        }
                        //decimal v = (decimal)ExtractRecordValue(tmp, msgConfig.ResultRecordInfo, msgConfig.ResultRecordInfo.TestValue);
                        //resultedDateTime = (DateTime)ExtractRecordValue(tmp, msgConfig.ResultRecordInfo, msgConfig.ResultRecordInfo.ResultedDateTime);
                    }
                    else if (IsRecordTypeMatch(strRecordReceived, msgConfig.CommentRecordInfo))
                    {
                        RecordInfo riComment = msgConfig.CommentRecordInfo.Copy();
                        string commentCode = (string)ExtractRecordValueHL7(strRecordReceived, riComment, riComment.CommentCode);
                        string commentText = (String.IsNullOrEmpty(commentCode) ? "" : commentCode + ":")
                            + (string)ExtractRecordValueHL7(strRecordReceived, riComment, riComment.CommentText);

                        if (!String.IsNullOrEmpty(commentCode) && commentCode != "0")
                        {
                            //drResult["COMMENT_TEXTS"] = (drResult["COMMENT_TEXTS"] != DBNull.Value && !String.IsNullOrEmpty(drResult["COMMENT_TEXTS"].ToString()) ? drResult["COMMENT_TEXTS"].ToString() + "~" : "") + commentText;
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Comment record Received " + commentText /*+ "\n"*/);
                            //UiMediator.AddUiGridData(AnalyzerID, barcode, "COMMENT", commentText, null, "", "");
                        }
                    }
                }


                try
                {

                    //if(queryBarcodesForResults.Any())
                    //{
                    //    List<string> worklistMessages = PrepareResultResponseMessagesHL7();
                    //    records.AddRange(worklistMessages);
                    //    //TextLogger.WriteLogEntry("Debugging", records.Count + " records generated for barcode: " + barcodesCommaSeparated);
                    //}

                    if (queryBarcodes.Any())
                    {
                        string barcodesCommaSeparated = String.Join(",", queryBarcodes);
                        TextLogger.WriteLogEntry("Debugging", "Query received for barcode: " + barcodesCommaSeparated);
                        List<BarcodeList> bList = null;
                        int mode = (msgConfig.AnalyzerTypeID == 30 ? 3 : 0);
                        try
                        {
                            bList = GetWorklist(queryBarcodes, mode)?.samples;
                            TextLogger.WriteLogEntry("Debugging", "WO fetched for barcode: " + barcodesCommaSeparated);
                        }
                        catch (Exception ex)
                        {

                            bList = new List<BarcodeList>();

                            string strBarcodes = String.Join(", ", queryBarcodes);
                            ex.Data.Add("Barcodes", strBarcodes);
                            UiMediator.LogAndShowError(Program.AnalyzerId, ex, "Sample Skipped: " + strBarcodes + " (" + ex.Message + ")");
                        }

                        if (bList != null && (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA || msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4))
                        {
                            List<string> worklistMessages = PrepareWorklistMessagesHL7(queryBarcodes, bList);
                            records.AddRange(worklistMessages);
                            TextLogger.WriteLogEntry("Debugging", records.Count + " records generated for barcode: " + barcodesCommaSeparated);
                        }
                        //Non HL7 machines work list preparating
                        if (bList != null && (msgConfig.AnalyzerTypeID != AnalyzerTypes.ATELLICA || msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6))
                        {
                            List<string> worklistMessages = PrepareWorklistMessages(queryBarcodes, bList);
                            records.AddRange(worklistMessages);
                            TextLogger.WriteLogEntry("Debugging", records.Count + " records generated for barcode: " + barcodesCommaSeparated);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UiMediator.LogAndShowError(AnalyzerID, ex, "Error getting order info");
                }

                if (dtResults.Rows.Count > 0 || (testResults != null && testResults.results.Count > 0))
                {
                    new System.Threading.Thread(() =>
                    {
                        try
                        {
                            int testsUpdated = UpdateTestValues();
                            dtResults.Rows.Clear();
                            testResults = null;
                            UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Outgoing, testsUpdated + " Results updated." /*+ "\n"*/);
                        }
                        catch (Exception ex)
                        {
                            UiMediator.LogAndShowError(AnalyzerID, ex, "Error in updating result values");
                        }
                    }).Start();

                    //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Dimension)
                    //{
                    //    records.Add("M" + Characters.FS + "A"); //Result Acceptance Message
                    //}
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(AnalyzerID, ex, "Error processing message");
            }

            // Records Preparation logics for HL7 and Non HL7 Machines
            if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA || msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
            {
                PrepareFinalRecordsHL7(records);
            }
            else
            {
                PrepareFinalRecords(records);
            }
            return records;
        }

        private void PrepareFinalValueHL7(RecordFieldInfo rfi, int positionNumber = 0)
        {

            List<object> valuesList = null;
            if (rfi.SupportsMultipleValues)
            {
                if (rfi.Value == null)
                    valuesList = new List<object>();
                else
                    valuesList = (List<object>)rfi.Value;

                if (positionNumber > valuesList.Count)
                    valuesList.Add(null);
            }

            object value = rfi.SupportsMultipleValues ? valuesList[positionNumber - 1] : rfi.Value;

            if (value == null || (rfi is StringFieldInfo && rfi.Value.ToString() == ""))
            {
                if (rfi.DefaultValue != null)
                {
                    value = rfi.DefaultValue;
                }


                if (rfi is StringFieldInfo)
                {
                    string copyKey_Outgoing = ((StringFieldInfo)rfi).CopyKey_Outgoing;
                    if (!String.IsNullOrEmpty(copyKey_Outgoing) && CopyValues.ContainsKey(copyKey_Outgoing))
                    {
                        if (copyKey_Outgoing == "SequenceNumber")
                            value = (int.Parse(CopyValues[copyKey_Outgoing]) + 1).ToString();
                        else
                            value = CopyValues[copyKey_Outgoing];
                    }
                }
            }

            string strValue = string.Empty;
            if (rfi is DateFieldInfo)
            {
                DateFieldInfo dfi = rfi as DateFieldInfo;
                if ((value == null || value.ToString() == "") && dfi.IsCurrentDateTime)
                    value = DateTime.Now;

                if (value == null)
                {
                    if (msgConfig.IsFieldSizeInBytes)
                        strValue = new string(dfi.EmptySpaceFillerChar, rfi.BytesLength);

                    else if (msgConfig.AnalyzerTypeID == 310 /*&& value == null*/)
                        strValue = DateTime.Today.AddDays(-1).ToString("yyyyMMddHHmmss"); // case sensitive to sent SCT 1 day before
                    else
                        strValue = "";

                }
                else
                {
                    strValue = ((DateTime)value).ToString(dfi.DateFormat);
                }
            }
            else if (rfi is NumericFieldInfo)
            {
                NumericFieldInfo nfi = rfi as NumericFieldInfo;
                if (value != null)
                {
                    string numberFormat = "0" + (nfi.DecimalPlaces > 0 ? "." + new string('0', nfi.DecimalPlaces) : "");
                    strValue = decimal.Parse(value.ToString()).ToString(numberFormat);

                    //prepend 0
                    if ((msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength) && strValue.Length < nfi.BytesLength)
                    {
                        strValue = FillEmptySpaces(nfi, strValue);
                    }
                }
            }
            else
            {
                //if(msgConfig.AnalyzerTypeID!=AnalyzerTypes.AutoBio_A1000)
                strValue = value == null ? "" : (string)value;
                if ((msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength) && strValue.Length < rfi.BytesLength)
                {
                    strValue = FillEmptySpaces(rfi, strValue);
                }
            }

            if (msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength)
            {
                if (value == null)
                    strValue = new String(rfi.EmptySpaceFillerChar, rfi.BytesLength);

                if (strValue.Length > rfi.BytesLength)
                {
                    strValue = ((string)value).Substring(0, rfi.BytesLength);
                }
            }

            if (rfi.CopyFieldFrom != null)
            {
                strValue = rfi.CopyFieldFrom.Value.ToString();
            }

            if (rfi.SupportsMultipleValues)
            {
                valuesList[positionNumber - 1] = strValue;
                rfi.Value = valuesList;
            }
            else
            {
                rfi.Value = strValue;
            }
        }

        #endregion


        private void PrepareFinalValue(RecordFieldInfo rfi, int positionNumber = 0)
        {
            string result = "";
            string rack_no = "";
            if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Bechman/* && msgConfig.AnalyzerID != 25*/)
            {
                result = chache.Name.ToString();
                rack_no = result.Substring(7, 4);
            }
            List<object> valuesList = null;
            if (rfi.SupportsMultipleValues)
            {
                if (rfi.Value == null)
                    valuesList = new List<object>();
                else
                    valuesList = (List<object>)rfi.Value;

                if (positionNumber > valuesList.Count)
                    valuesList.Add(null);
            }

            object value = rfi.SupportsMultipleValues ? valuesList[positionNumber - 1] : rfi.Value;

            if (value == null || (rfi is StringFieldInfo && rfi.Value.ToString() == ""))
            {
                if (rfi.DefaultValue != null)
                {
                    value = rfi.DefaultValue;

                    //string rackfrompbejcttostring = value.ToString();
                    //to send rack no in communication for bechaman machine at TG kims location machine
                    if (value.ToString() == "123")
                    {
                        value = "00000" + rack_no;
                    }
                }


                if (rfi is StringFieldInfo)
                {
                    string copyKey_Outgoing = ((StringFieldInfo)rfi).CopyKey_Outgoing;
                    if (!String.IsNullOrEmpty(copyKey_Outgoing) && CopyValues.ContainsKey(copyKey_Outgoing))
                    {
                        if (copyKey_Outgoing == "SequenceNumber")
                            value = (int.Parse(CopyValues[copyKey_Outgoing]) + 1).ToString();
                        else
                            value = CopyValues[copyKey_Outgoing];
                    }
                }
            }

            string strValue = string.Empty;
            if (rfi is DateFieldInfo)
            {
                DateFieldInfo dfi = rfi as DateFieldInfo;
                if ((value == null || value.ToString() == "") && dfi.IsCurrentDateTime)
                    value = DateTime.Now;

                if (value == null)
                {
                    if (msgConfig.IsFieldSizeInBytes)
                        strValue = new string(dfi.EmptySpaceFillerChar, rfi.BytesLength);

                }
                else
                {
                    strValue = ((DateTime)value).ToString(dfi.DateFormat);
                }
            }
            else if (rfi is NumericFieldInfo)
            {
                NumericFieldInfo nfi = rfi as NumericFieldInfo;
                if (value != null)
                {
                    string numberFormat = "0" + (nfi.DecimalPlaces > 0 ? "." + new string('0', nfi.DecimalPlaces) : "");
                    strValue = decimal.Parse(value.ToString()).ToString(numberFormat);

                    //prepend 0
                    if ((msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength) && strValue.Length < nfi.BytesLength)
                    {
                        strValue = FillEmptySpaces(nfi, strValue);
                    }
                }
            }
            else
            {
                strValue = value == null ? "" : (string)value;
                if ((msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength) && strValue.Length < rfi.BytesLength)
                {
                    strValue = FillEmptySpaces(rfi, strValue);
                }
            }

            if (msgConfig.IsFieldSizeInBytes || rfi.IsFixedLength)
            {
                if (value == null)
                    strValue = new String(rfi.EmptySpaceFillerChar, rfi.BytesLength);

                if (strValue.Length > rfi.BytesLength && msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                {
                    strValue = ((string)value).Substring(0, rfi.BytesLength);
                }
            }

            if (rfi.CopyFieldFrom != null)
            {
                strValue = rfi.CopyFieldFrom.Value.ToString();
            }

            if (rfi.SupportsMultipleValues)
            {
                valuesList[positionNumber - 1] = strValue;
                rfi.Value = valuesList;
            }
            else
            {
                rfi.Value = strValue;
            }
        }

        private string FillEmptySpaces(RecordFieldInfo rfi, string strValue)
        {
            string stringToFill = new string(rfi.EmptySpaceFillerChar, (rfi.BytesLength - strValue.Length));
            if (rfi.IsRightJustified)
                return stringToFill + strValue;
            else
                return strValue + stringToFill;
        }

        public InstrumentResult AddResultRecord(string barcode, string machineTestcode, decimal? testValue, string unit, string prefix, DateTime? resultedDateTime, int descriptiveId = -1, string additionalInfo = "", bool showInGrid = true, BarcodeList patientInfo = null, int AnalyzerIDForResult = 0, bool IsReattempt = false)
        {
            if (dtResults == null)
                dtResults = GetResultsTableStructure();

            if (AnalyzerIDForResult == 0)
                AnalyzerIDForResult = AnalyzerID;

            DataRow drOrder = dtResults.Rows.Add(barcode);

            if (testResults == null)
            {
                testResults = new InstrumentResult();
            }

            InstrumentResult _testResults = IsReattempt ? new InstrumentResult() : testResults;

            if (_testResults.results == null)
            {
                _testResults.results = new List<ResultList>();
            }

            ResultList barcodeDetail = _testResults.results.Where(r => r.barcode.Equals(barcode, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (barcodeDetail == null)
            {
                barcodeDetail = new ResultList()
                {
                    barcode = barcode,
                    investigationresults = new List<Result>()
                };
                _testResults.results.Add(barcodeDetail);
            }

            Result testDetail = new Result();
            barcodeDetail.investigationresults.Add(testDetail);

            testDetail.instrumentid = AnalyzerIDForResult;

            if (testValue.HasValue)
            {
                drOrder["TESTVALUE"] = testValue.Value;
                testDetail.result = (double)testValue.Value;
            }
            else
            {
                drOrder["TESTVALUE"] = 0; // DBNull.Value;
                testDetail.result = 0;
            }

            if (resultedDateTime.HasValue)
            {
                drOrder["RESULTED_TIME"] = resultedDateTime.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                testDetail.instrumenttime = TimeZoneInfo.ConvertTime(resultedDateTime.Value.ToUniversalTime(), VSoftLISMAIN.TimeZone_India);
            }
            else
            {
                drOrder["RESULTED_TIME"] = DateTime.Now;
                testDetail.instrumenttime = DateTime.Now;
            }

            testDetail.TextResultId = descriptiveId;

            drOrder["TESTCODE"] = machineTestcode;

            if (IsReattempt)
            {
                testDetail.testcode = machineTestcode;

                TestListItem tli = CachedData.TestList.Where(r => r.testcode.Equals(machineTestcode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                testDetail.testcode = tli.testcode ?? "";
                testDetail.testid = tli.testid;
            }
            else

            {
                TestListItem tli = GetTestMappingOrEmpty(machineTestcode);
                testDetail.instrumentCode = machineTestcode;
                testDetail.testcode = tli.testcode ?? "";
                testDetail.testid = tli.testid;
            }

            //drOrder["TESTVALUE"] = testValue;
            drOrder["UNIT"] = unit;
            testDetail.symbol = unit;
            drOrder["PREFIX"] = prefix;
            testDetail.instrumentresultnote = testDetail.symbol = prefix;
            testDetail.InstrumentRemarks = "";
            testDetail.resultunit = unit;

            if (testDetail.instrumentresultnote == null)
                testDetail.instrumentresultnote = "";

            if (showInGrid && !IsReattempt)
            {
                object[] values = null;
                if (msgConfig.UserSelectionToUploadResults)
                    values = new object[] { barcode, testDetail.instrumentCode, testDetail.result, testDetail.resultunit, testDetail.symbol, testDetail.instrumenttime, testDetail.TextResultId };

                additionalInfo = String.IsNullOrEmpty(additionalInfo) ? "" : " :: ";
                additionalInfo += !String.IsNullOrEmpty(prefix) ? "\"" + prefix + "\"" : "";
                additionalInfo += " " + (descriptiveId <= 0 ? "" : GetDescriptiveMapping(testDetail.testcode).Where(r => r.resultid == descriptiveId).FirstOrDefault()?.result);

                if (!new int[] { 9/*, 56*/ }.Contains(msgConfig.AnalyzerTypeID))
                    UiMediator.AddUiGridData(msgConfig.AnalyzerID, barcode, "RESULT", (String.IsNullOrEmpty(testDetail.testcode) ? testDetail.instrumentCode : testDetail.testcode), testValue, testDetail.symbol, additionalInfo, values, patientInfo);
            }
            return _testResults;
        }

        public worklist GetWorklist(string barcode, int mode = 0)
        {
            return GetWorklist(new List<string> { barcode }, mode);
        }

        /// <param name="mode">0: WO for particular barcodes, 1: Bulk WO, 2:Bulk WO Cancellation, 3: Get information wihout tagging</param>
        public worklist GetWorklist(List<string> barcodes, int mode = 0)
        {
            List<string> barcodesUnmodified = barcodes;
            barcodes = barcodes.Select(r => r.Trim()).ToList();
            DateTime dtmStart = DateTime.Now;
            string barcodesCommaSeparated = String.Join(", ", barcodes.ToArray());
            worklist wList = null;
            sampleList slBarcodeRequest = new sampleList() { instrumentid = msgConfig.AnalyzerID, barcode = barcodes, mode = mode };

            try
            {
                if (msgConfig.InstrumentType == InstrumentTypes.Analyzer)
                {
                    bool isSameAsPreviousBarcode = false;
                    //int woTimeout = new int[] { 0 }.Contains(mode) ? msgConfig.WorkOrderTimeoutMilliseconds : (5 * 60 * 1000);
                    int woTimeout = msgConfig.WorkOrderTimeoutMilliseconds;

                    if (barcodes.Count == 1 && !String.IsNullOrEmpty(barcodes[0]) && barcodes[0] != "ALL")
                    {
                        if (WOBarcode_Single == barcodes[0])
                        {
                            isSameAsPreviousBarcode = true;
                        }
                        else
                        {
                            WOBarcode_Single = barcodes[0];
                        }
                    }

                    System.Diagnostics.Stopwatch swWO = new System.Diagnostics.Stopwatch();
                    swWO.Start();
                    System.Threading.Tasks.Task taskWOFetch = System.Threading.Tasks.Task.Factory.StartNew(() =>
                    //System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(() =>
                    {
                        //List<string> responseRecords = null;
                        if (isSameAsPreviousBarcode)
                        {
                            try
                            {
                                //responseRecords = DLL.CommunicationBase.ReadPendingWOFile(WOBarcode_Single);
                                wList = Newtonsoft.Json.JsonConvert.DeserializeObject<worklist>(FileManager.ReadAllText(FolderName.WOJson, WOBarcode_Single));
                                TextLogger.WriteLogEntry("Debugging", "Tests fetched from local file for barcode: " + WOBarcode_Single);
                            }
                            catch (FileNotFoundException) { }
                        }
                        else if (msgConfig.IsBulkWorklistSender)
                        {
                            try
                            {
                                string folderPath = FileManager.GetFullFolderPath(FolderName.WOJson, "Mode_" + mode);
                                if (Directory.Exists(folderPath))
                                {
                                    string[] woFiles = Directory.GetFiles(folderPath);
                                    if (woFiles.Any())
                                    {
                                        wList = new worklist() { samples = new List<BarcodeList>() };
                                        foreach (string strFile in woFiles)
                                        {
                                            string strFilename = Path.GetFileName(strFile);
                                            //responseRecords = DLL.CommunicationBase.ReadPendingWOFile(WOBarcode_Single);
                                            worklist wListTemp = Newtonsoft.Json.JsonConvert.DeserializeObject<worklist>(File.ReadAllText(strFile));
                                            wList.samples.AddRange(wListTemp.samples);
                                            File.Delete(strFile);
                                            TextLogger.WriteLogEntry("Debugging", "Tests fetched from local file: " + strFilename);
                                            UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Normal, "Tests fetched from local file: " + strFilename);
                                        }
                                    }
                                }
                            }
                            catch (FileNotFoundException) { }
                        }

                        if (wList == null)
                        {
                            if (!Program.IsDebugMode)
                            //if (!Program.IsDebugMode)
                            {
                                DateTime dtmStartedTime = DateTime.Now;
                                TextLogger.WriteLogEntry("Debugging", "Started fetching WO from server for barcode: " + barcodesCommaSeparated);
                                wList = WebAPI.GetWorklist(slBarcodeRequest, 0 /*woTimeout*/);
                                TextLogger.WriteLogEntry("Debugging", "Fetched WO from server for " + (wList?.samples?.Count ?? 0) + " barcode: " + barcodesCommaSeparated + " (was started at " + dtmStartedTime.ToString("HH:mm:ss.fff") + ", Duration (Seconds): " + DateTime.Now.Subtract(dtmStart).TotalSeconds + ")");

                            }
                            else
                            {
                                System.Threading.Thread.Sleep(2500);
                                //Dummy worklist for unit testing of WO communication
                                wList = new worklist();
                                wList.samples = new List<BarcodeList>();


                                if (barcodes[0] == "ALL")
                                {
                                    barcodes.Clear();
                                    int totalSamplesToSend = 3;
                                    for (int i = lastDummySrNo + 1; i <= lastDummySrNo + totalSamplesToSend; i++)
                                        barcodes.Add("DUMM" + i.ToString("0000"));

                                    lastDummySrNo += totalSamplesToSend;
                                }

                                foreach (string barcode in barcodes)
                                {
                                    List<TestCode> dummyTestCodes = new List<TestCode>();
                                    dummyTestCodes.AddRange(CachedData.TestList
                                                .OrderBy(r => Guid.NewGuid()).Take(new Random().Next(1, CachedData.TestList.Count()))
                                                .Select(r => new TestCode { testCode = r.testcode, instrumentCode = r.instrumentcode, IsRepeat = true }));

                                    wList.samples.Add(new BarcodeList
                                    {
                                        ProcessDate = DateTime.Today,
                                        barcode = barcode,
                                        age = 11,
                                        registertime = DateTime.Now.AddHours(-1),
                                        gender = "M",
                                        Labcode = barcode.StartsWith("DUMM") ? int.Parse(barcode.Replace("DUMM", "")) : new Random().Next(1, 999999),
                                        investigationid = "PID_" + barcode,
                                        refDr = "RefDr",
                                        customername = "DUMMY PATIENT",
                                        collectiontime = DateTime.Now,
                                        testlist = dummyTestCodes,
                                        ClientCode = "ABC01"
                                    });
                                }
                            }

                            swWO.Stop();

                            TextLogger.WriteLogEntry("Debugging", "swWO.ElapsedMilliseconds: " + swWO.ElapsedMilliseconds + ", woTimeout: " + woTimeout);
                            if (swWO.ElapsedMilliseconds >= (woTimeout * 0.9))
                            {
                                if (wList != null && wList.samples != null && wList.samples.Count > 0)
                                {
                                    string strFilename = "";
                                    if (!String.IsNullOrEmpty(WOBarcode_Single))
                                    {
                                        //responseRecords = PrepareWorklistMessages(barcodes, wList.barcodeList);
                                        //DLL.CommunicationBase.WritePendingWOFile(responseRecords, WOBarcode_Single);
                                        strFilename = WOBarcode_Single;
                                    }
                                    else if (msgConfig.IsBulkWorklistSender)
                                    {
                                        strFilename = Guid.NewGuid().ToString();
                                    }

                                    if (!String.IsNullOrEmpty(strFilename))
                                    {
                                        FileManager.WriteAllText(FolderName.WOJson, strFilename, InterfaceHelper.SerializeToJson(wList), "Mode_" + mode);
                                        UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Warning, "Saved timed out WO to file " + strFilename);
                                    }
                                }
                            }
                        }
                    });

                    if (System.Threading.Tasks.Task.WaitAny(new System.Threading.Tasks.Task[] { taskWOFetch }, woTimeout) == -1)
                    {
                        MessageLogger.WriteTimeDiffLog("Worklist", dtmStart, barcodesCommaSeparated + " (Timeout)");
                        System.Threading.Thread.Sleep(10000);
                        TextLogger.WriteLogEntry("Debugging", "taskWOFetch.Status after 10 seconds wait: " + taskWOFetch.Status);
                        //operation timed out
                        TimeoutException tex = new TimeoutException("WO Timeout");
                        tex.Data.Add("barcode", barcodesCommaSeparated);
                        throw tex;
                    }
                    //else
                    //{
                    //    //WO tagging of fetched tests, if tests are fetched
                    //    if (mode != 3 && (wList?.samples?.Count ?? 0) > 0 && wList.samples.Where(r => r.testlist?.Count > 0).Any())
                    //    {
                    //        new System.Threading.Thread(() =>
                    //        {
                    //            try
                    //            {
                    //                TagWOTest tagWOTest = new TagWOTest() { ResultIDs = new List<int>() };
                    //                tagWOTest.AnalyzerID = slBarcodeRequest.instrumentid;
                    //                tagWOTest.mode = slBarcodeRequest.mode;
                    //                tagWOTest.ResultIDs.AddRange(
                    //                    wList.samples.SelectMany(r => r.testlist).Select(r => r.resultId).Distinct());
                    //                WebAPI.TagWorkList(tagWOTest);
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in WO tagging of barcode " + barcodesCommaSeparated);
                    //            }
                    //        }).Start();
                    //    }
                    //}

                }

                if (wList == null || wList.samples.Count == 0)
                {
                    if (barcodesCommaSeparated == "ALL")
                    {
                        UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Warning, "No barcode found.");
                    }
                }
                else
                {
                    if (barcodesCommaSeparated != "ALL")
                    {
                        //send barcode in same case letters as sent by analyzer, Advia giving NAK response when barcode case is changed
                        //response barcode kept untrimmed, as AU5800 gives sample mismatch error if space character (if available in request) in barcode is trimmed
                        foreach (var bList in wList.samples)
                        {
                            if (!string.IsNullOrEmpty(bList.barcode))
                                bList.barcode = barcodesUnmodified.Where(r => r.Trim().Equals(bList.barcode, StringComparison.InvariantCultureIgnoreCase)).First();
                        }
                    }
                }

                MessageLogger.WriteTimeDiffLog("Worklist", dtmStart, barcodesCommaSeparated);
            }
            catch (SqlException sqlEx)
            {
                sqlEx.Data.Add("barcodes", barcodesCommaSeparated);
                UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Error, "Error getting order info: " + barcodesCommaSeparated + Environment.NewLine + sqlEx.Message);
                errorLog.err_insert(sqlEx);
            }
            //catch (Exception ex)
            //{
            //    ex.Data.Add("barcodes", String.Join(", ", queryBarcodes.ToArray()));
            //    UiMediator.LogAndShowError(msgConfig.AnalyzerID, ex, "Error getting order info: " + barcodesCommaSeparated);
            //}

            return wList ?? new worklist { samples = new List<BarcodeList>() };
        }

        public List<string> PrepareWorklistMessages(List<string> requestedBarcodes, IEnumerable<BarcodeList> barcodeList, bool isCancellation = false)
        {
            List<string> records = new List<string>();

            try
            {
                string terminationCode = "";

                if (requestedBarcodes != null && requestedBarcodes[0] != "ALL")
                {
                    string barcode = requestedBarcodes[0];
                    List<TestCode> testCodes = barcodeList.FirstOrDefault()?.testlist ?? new List<TestCode>();

                }

                if (msgConfig.HeaderAndTerminationRecordRequired)
                {
                    AddHeaderRecord(strHeader);
                }

                if (barcodeList == null)
                    barcodeList = new List<BarcodeList>();

                int sequenceNumber_Patient = 0;

                if (requestedBarcodes != null && requestedBarcodes[0] != "ALL")
                {
                    #region show barcodes that wasn't found and send reply for empty WO as per LIS manual
                    var missingBarcodes = requestedBarcodes.Except(barcodeList.Where(r => r.testlist.Any()).Select(r => r.barcode), StringComparer.InvariantCultureIgnoreCase);
                    if (missingBarcodes.Any())
                    {
                        if (msgConfig.SendPatientOrderRecordWhenNoOrder)
                        {
                            foreach (string barcode in missingBarcodes)
                            {
                                AddPatientRecord(ref sequenceNumber_Patient, new BarcodeList { barcode = barcode, investigationid = barcode, customername = barcode, age = 0 });
                                int sequenceNumber_Order = 0;
                                string emptyTestcode = "";
                                RecordInfo riOrder = AddOrderRecord(ref sequenceNumber_Order, barcode, emptyTestcode);
                                SetRecordValue(riOrder, riOrder.ReportType, msgConfig.ReportTypeFlagForNoWorkOrder); //for Sysmex XN 1000RecordInfo riComment = msgConfig.CommentRecordInfo.Copy();
                            }
                        }
                        else if (msgConfig.SendNegativeQueryRecordWhenNoOrder)
                        {
                            foreach (string barcode in requestedBarcodes)
                            {
                                AddQueryRecord();
                                SetRecordValue(riQuery, riQuery.SampleID, barcode);
                            }
                        }
                        else if (!String.IsNullOrEmpty(msgConfig.TerminationCodeWhenNoOrder))
                        {
                            terminationCode = msgConfig.TerminationCodeWhenNoOrder;
                        }

                        else
                        {
                            UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Error, "Barcode not found, or may be already processed: " + String.Join(", ", missingBarcodes.ToArray()) + ".");
                        }
                    }
                    #endregion
                }

                foreach (BarcodeList bList in barcodeList.Where(r => r.testlist.Any()))
                {
                    //added try catch inside loop, so that eventhough one barcode fails, other barcodes in same query should be processed
                    try
                    {
                        //remove duplicate machine test record, done for Centralink to ensure distinct SORT codes are sent only once for a sample
                        bList.testlist = bList.testlist.GroupBy(r => new { r.instrumentCode }).Select(r => r.First()).ToList();

                        AddPatientRecord(ref sequenceNumber_Patient, bList);

                        if (msgConfig.IsFieldSizeInBytes)
                        {
                            SetRecordValue(riPatient, riPatient.TotalNumberOfBlocks, 1);
                            SetRecordValue(riPatient, riPatient.BlockNumber, 1);
                            SetRecordValue(riPatient, riPatient.NumberOfTestsInBlock, bList.testlist.Count);
                        }

                        int sequenceNumber_Order = 0;
                        int positionNumber = 1;
                        if (!String.IsNullOrEmpty(strQuery))
                        {
                            //if testscodes mentioned in Query, remove other tests from WO message
                            string strTestcodeFromQuery = (string)ExtractRecordValue(strQuery, riQuery_Incoming, riQuery_Incoming.TestID_ManufacturersTestCode);
                            if (!String.IsNullOrEmpty(strTestcodeFromQuery) && strTestcodeFromQuery != "ALL")
                            {
                                bList.testlist.Where(r => !String.Equals(r.instrumentCode, strTestcodeFromQuery, StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(r => bList.testlist.Remove(r));
                            }
                        }

                        if (bList.testlist != null && bList.testlist.Any())
                        {
                            if (msgConfig.ReplacementWOTests != null && msgConfig.ReplacementWOTests.Length > 0)
                            {
                                bool isRepeat = bList.testlist[0].IsRepeat;
                                bList.testlist.RemoveRange(0, bList.testlist.Count);
                                bList.testlist.AddRange(
                                    msgConfig.ReplacementWOTests.Select(r => new TestCode() { instrumentCode = r, testCode = "", IsRepeat = isRepeat })
                                    );
                            }

                            if (msgConfig.ReplacementWOTestsMapping != null && msgConfig.ReplacementWOTestsMapping.Count > 0)
                            {

                                foreach (var test in bList.testlist)
                                {
                                    if (msgConfig.ReplacementWOTestsMapping.ContainsKey(test.instrumentCode))
                                    {
                                        test.instrumentCode = msgConfig.ReplacementWOTestsMapping[test.instrumentCode];
                                    }

                                }
                            }

                            bList.testlist = bList.testlist.Distinct().ToList();

                            foreach (TestCode tCode in bList.testlist.OrderBy(r => r.instrumentCode)) //sorting added for AU
                            {
                                RecordInfo ri = AddOrderRecord(ref sequenceNumber_Order, bList.barcode, tCode.instrumentCode);
                                if (ri.ActionCode != null)
                                {
                                    string ActionCode = "";
                                    if (isCancellation)
                                        ActionCode = msgConfig.ActionCode_Cancellation;
                                    else if (tCode.IsRepeat)
                                        ActionCode = msgConfig.ActionCode_Repeat;
                                    else if (bList.IsBarcodeWoSentEarlier && !String.IsNullOrEmpty(msgConfig.ActionCode_AddTestsInExistingBarcode))
                                        ActionCode = msgConfig.ActionCode_AddTestsInExistingBarcode;
                                    else
                                        ActionCode = ri.ActionCode.DefaultValue.ToString();

                                    SetRecordValue(ri, ri.ActionCode, !String.IsNullOrEmpty(ActionCode) ? ActionCode : (ri.ActionCode?.DefaultValue ?? ""));
                                }
                                if (ri.SampleType != null)
                                {
                                    SetRecordValue(ri, ri.SampleType, bList.SampleType);
                                }

                                positionNumber++;

                                string WOType = "WO";
                                if (isCancellation)
                                    WOType = "Cancel";

                                UiMediator.AddUiGridData(AnalyzerID, bList.barcode, WOType, tCode.testCode, null, "", "");
                            }

                            if (riPatient.TestCount != null)
                            {
                                SetRecordValue(riPatient, riPatient.TestCount, bList.testlist.Count);
                            }
                        }
                        riOrder = null; //set null to start with new record in case of bulk WO

                        //if (msgConfig.InstrumentType == InstrumentTypes.Sorter)
                        //{
                        //    SetRecordValue(riPatient, riPatient.BinNumber, bList.BinInfo.Bin);
                        //}
                    }
                    catch (Exception ex)
                    {
                        UiMediator.LogAndShowError(AnalyzerID, ex, "Error in preparing communication message of barcode " + bList.barcode);
                    }
                }

                if (msgConfig.HeaderAndTerminationRecordRequired)
                {
                    AddTerminationRecord(barcodeList.Count(), terminationCode);
                }
            }
            catch (Exception ex)
            {
                string barcodesCommaSeparated = String.Join(", ", queryBarcodes.ToArray());
                ex.Data.Add("barcodes", barcodesCommaSeparated);
                UiMediator.LogAndShowError(msgConfig.AnalyzerID, ex, "Error preparing communication messages : " + barcodesCommaSeparated);
            }


            try
            {
                if (responseRecords.Any())
                {
                    foreach (RecordInfo ri in responseRecords)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (RecordFieldInfo rfi in ri.RecordFields.OrderBy(r => r.FieldNumber))
                        {
                            if (rfi.SupportsMultipleValues)
                            {
                                if (rfi.ComponentCount > 0 /*&& msgConfig.AnalyzerTypeID != AnalyzerTypes.AutoBio_A1000*/)
                                {
                                    int valueCount = 0;

                                    //List<string> mylist = new List<string>();

                                    var query = rfi.Components.Where(r => r.Value != null);
                                    if (query.Any())
                                    {
                                        valueCount = query.Max(r => ((List<object>)r.Value).Count);
                                    }

                                    //set to 1, to allow calling PrepareFinalValue to set default values
                                    if (valueCount == 0)
                                        valueCount = 1;

                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        rfi.Components.ForEach(r => PrepareFinalValue(r, i + 1));
                                        try
                                        {
                                            sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => ((List<object>)r.Value)[i])));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        if (i < valueCount - 1)
                                            sb.Append(msgConfig.RepeatDelimiter);
                                    }

                                }
                                else
                                {
                                    int valueCount = ((List<object>)rfi.Value).Count;
                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        PrepareFinalValue(rfi, i + 1);
                                    }

                                    sb.Append(String.Join(msgConfig.RepeatDelimiter.ToString(), (List<object>)rfi.Value));
                                }
                            }
                            else
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    rfi.Components.ForEach(r => PrepareFinalValue(r));
                                    sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => r.Value)));
                                }
                                else
                                {
                                    try
                                    {
                                        PrepareFinalValue(rfi);
                                        sb.Append(rfi.Value);

                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            if (rfi.FieldNumber < ri.FieldCount - 1)
                                sb.Append(msgConfig.FieldDelimiter);
                        }

                        string strRecord = sb.ToString();

                        if (!msgConfig.IsFieldSizeInBytes)
                        {

                            if (msgConfig.RemoveTrailingEmptyDelimiters)
                            {
                                strRecord = strRecord.TrimEnd(msgConfig.FieldDelimiter.Value, msgConfig.ComponentDelimiter.Value); //for Sysmex XN 1000

                                //remove multiple component delimiters, having no values inside |^^^^^|
                                strRecord = new System.Text.RegularExpressions.Regex("\\" + msgConfig.FieldDelimiter.Value.ToString() + "\\" + msgConfig.ComponentDelimiter.Value.ToString() + "+" + "\\" + msgConfig.FieldDelimiter.Value.ToString()).Replace(strRecord, msgConfig.FieldDelimiter.Value.ToString() + msgConfig.FieldDelimiter.Value.ToString());
                            }
                        }

                        records.Add(strRecord);
                    }
                    responseRecords.Clear();
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                {
                    records.Clear();

                    Dictionary<int, string> ReplaceCharacterList = new Dictionary<int, string>{
                        { 2, "[STX]" },
                        { 3, "[ETX]" },
                        { 21, "[NAK]" },
                        { 5, "[ENQ]" },
                        { 4, "[EOT]" },
                        { 6, "[ACK]" },
                        { 13, "[CR]" },
                        { 23, "[ETB]" },
                        { 10, "[LF]" },
                        { 1, "[SOH]" }
                        };

                    foreach (string strLine in System.IO.File.ReadLines(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                    {
                        string strLine_Modified = strLine;
                        ReplaceCharacterList.ToList<KeyValuePair<int, string>>().ForEach(r => strLine_Modified = strLine_Modified.Replace(r.Value, Convert.ToChar(r.Key).ToString()));
                        records.Add(strLine_Modified);
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in sending message to Analyzer");
            }

            return records;
        }

        // HL7 Machines Frame builiding
        public List<string> PrepareWorklistMessagesHL7(List<string> requestedBarcodes, IEnumerable<BarcodeList> barcodeList, bool isCancellation = false)
        {
            List<string> records = new List<string>();

            try
            {
                string terminationCode = "";

                if (requestedBarcodes != null && requestedBarcodes[0] != "ALL")
                {
                    string barcode = requestedBarcodes[0];
                    List<TestCode> testCodes = barcodeList.FirstOrDefault()?.testlist ?? new List<TestCode>();
                }

                if (!msgConfig.HeaderAndTerminationRecordRequired && msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                {
                    HeaderRecordInfoHL7ACKResponseOrder(strHeader);
                    AddMSARecordHL7ACK(strHeader);
                    AddErrorRecordHL7ACK(strHeader);
                    AddQueryAckRecordHL7ACK(strHeader);
                    AddQueryDefinitionSegment(strQrdData);
                    AddQueryFilterSegment(strHeader);
                }

                if (!msgConfig.HeaderAndTerminationRecordRequired && msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6 && msgConfig.AnalyzerTypeID != AnalyzerTypes.MISPA_CX4)
                {
                    //ACK to Machine
                    //AddHeaderRecordHL7ACK(strHeader);
                    HeaderRecordInfoHL7ACKResponse(strHeader);
                    //MSA
                    AddMSARecordHL7ACK(strHeader);
                    //QAK
                    AddQAKRecordHL7ACK(strHeader);
                    //QPD
                    QueryRecordInfoHL7Res(strHeader);

                    //Work List to machine
                    AddHeaderRecordHL7(strHeader);
                }

                if(msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
                {
                    HeaderRecordInfoHL7ACKResponseOrder(strHeader);
                    AddMSARecordHL7ACK(strHeader);
                }

                if (barcodeList == null)
                    barcodeList = new List<BarcodeList>();

                //int sequenceNumber_Patient = 0;

                if (requestedBarcodes != null && requestedBarcodes[0] != "ALL")
                {
                    #region show barcodes that wasn't found and send reply for empty WO as per LIS manual
                    var missingBarcodes = requestedBarcodes.Except(barcodeList.Where(r => r.testlist.Any()).Select(r => r.barcode), StringComparer.InvariantCultureIgnoreCase);
                    if (missingBarcodes.Any())
                    {
                        if (msgConfig.SendPatientOrderRecordWhenNoOrder)
                        {
                            foreach (string barcode in missingBarcodes)
                            {
                                AddPatientRecordHL7(new BarcodeList { barcode = barcode, investigationid = barcode, customername = barcode, age = 0 });
                                int sequenceNumber_Order = 0;
                                int sequenceNumber_EXZ600 = 28;
                                string emptyTestcode = (msgConfig.AnalyzerTypeID == 59) ? "0" : "";
                                RecordInfo riOrder = AddOrderRecordHL7OBR(ref sequenceNumber_Order, barcode, emptyTestcode, ref sequenceNumber_EXZ600);
                                SetRecordValueHL7ForBarcodeReadError(riOrder, riOrder.ReportType, msgConfig.ReportTypeFlagForNoWorkOrder); //for Sysmex XN 1000RecordInfo riComment = msgConfig.CommentRecordInfo.Copy();
                            }
                        }
                        else if (msgConfig.SendNegativeQueryRecordWhenNoOrder)
                        {
                            foreach (string barcode in requestedBarcodes)
                            {
                                AddQueryRecordHL7();
                                SetRecordValue(riQuery, riQuery.SampleID, barcode);
                            }
                        }
                        else
                        {
                            UiMediator.AddUiMessage(msgConfig.AnalyzerID, (int)MessageType.Error, "Barcode not found, or may be already processed: " + String.Join(", ", missingBarcodes.ToArray()) + ".");
                        }
                    }
                    #endregion
                }

                foreach (BarcodeList bList in barcodeList.Where(r => r.testlist.Any()))
                {
                    //added try catch inside loop, so that eventhough one barcode fails, other barcodes in same query should be processed
                    try
                    {
                        //remove duplicate machine test record, done for Centralink to ensure distinct SORT codes are sent only once for a sample
                        bList.testlist = bList.testlist.GroupBy(r => new { r.instrumentCode }).Select(r => r.First()).ToList();
                        int sequenceNumber_Order = 0;
                        int sequenceNumber_EXZ600 = 28;

                        if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                        {
                            #region Add DSP
                            AddDisplayDataSegment_1(bList);
                            AddDisplayDataSegment_2(bList);
                            AddDisplayDataSegment_3(bList);
                            AddDisplayDataSegment_4(bList);
                            AddDisplayDataSegment_5(bList);
                            AddDisplayDataSegment_6(bList);
                            AddDisplayDataSegment_7(bList);
                            AddDisplayDataSegment_8(bList);
                            AddDisplayDataSegment_9(bList);
                            AddDisplayDataSegment_10(bList);
                            AddDisplayDataSegment_11(bList);
                            AddDisplayDataSegment_12(bList);
                            AddDisplayDataSegment_13(bList);
                            AddDisplayDataSegment_14(bList);
                            AddDisplayDataSegment_15(bList);
                            AddDisplayDataSegment_16(bList);
                            AddDisplayDataSegment_17(bList);
                            AddDisplayDataSegment_18(bList);
                            AddDisplayDataSegment_19(bList);
                            AddDisplayDataSegment_20(bList);
                            AddDisplayDataSegment_21(bList);
                            AddDisplayDataSegment_22(bList);
                            AddDisplayDataSegment_23(bList);
                            AddDisplayDataSegment_24(bList);
                            AddDisplayDataSegment_25(bList);
                            AddDisplayDataSegment_26(bList);
                            AddDisplayDataSegment_27(bList);
                            AddDisplayDataSegment_28(bList);
                            #endregion
                        }
                        if(msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6)
                        AddPatientRecordHL7(bList);
                        //Patient Visit info
                        AddPatientVisitRecordHl7();
                        AddOrderRecordHL7SPM(bList, sequenceNumber_Order);
                        AddOrderRecordHL7SAC(bList);

                        if (msgConfig.IsFieldSizeInBytes)
                        {
                            SetRecordValue(riPatient, riPatient.TotalNumberOfBlocks, 1);
                            SetRecordValue(riPatient, riPatient.BlockNumber, 1);
                            SetRecordValue(riPatient, riPatient.NumberOfTestsInBlock, bList.testlist.Count);
                        }

                        //int sequenceNumber_Order = 0;
                        int positionNumber = 1;
                        if (!String.IsNullOrEmpty(strQuery))
                        {
                            //if testscodes mentioned in Query, remove other tests from WO message
                            string strTestcodeFromQuery = (string)ExtractRecordValueHL7(strQuery, riQuery_Incoming, riQuery_Incoming.TestID_ManufacturersTestCode);
                            if (!String.IsNullOrEmpty(strTestcodeFromQuery) && strTestcodeFromQuery != "ALL")
                            {
                                bList.testlist.Where(r => !String.Equals(r.instrumentCode, strTestcodeFromQuery, StringComparison.InvariantCultureIgnoreCase)).ToList().ForEach(r => bList.testlist.Remove(r));
                            }
                        }

                        if (bList.testlist != null && bList.testlist.Any())
                        {
                            if (msgConfig.ReplacementWOTests != null && msgConfig.ReplacementWOTests.Length > 0)
                            {
                                bool isRepeat = bList.testlist[0].IsRepeat;
                                bList.testlist.RemoveRange(0, bList.testlist.Count);
                                bList.testlist.AddRange(
                                    msgConfig.ReplacementWOTests.Select(r => new TestCode() { instrumentCode = r, testCode = "", IsRepeat = isRepeat })
                                    );
                            }

                            if (msgConfig.ReplacementWOTestsMapping != null && msgConfig.ReplacementWOTestsMapping.Count > 0)
                            {

                                foreach (var test in bList.testlist)
                                {
                                    if (msgConfig.ReplacementWOTestsMapping.ContainsKey(test.instrumentCode))
                                    {
                                        test.instrumentCode = msgConfig.ReplacementWOTestsMapping[test.instrumentCode];
                                    }

                                }
                            }

                            bList.testlist = bList.testlist.Distinct().ToList();

                            foreach (TestCode tCode in bList.testlist.OrderBy(r => r.instrumentCode)) //sorting added for AU
                            {
                                AddOrderRecordHL7ORC(bList);
                                AddOrderRecordHL7TQ1(bList);

                                RecordInfo ri = AddOrderRecordHL7OBR(ref sequenceNumber_Order, bList.barcode, tCode.instrumentCode, ref sequenceNumber_EXZ600);
                                if (ri.ActionCode != null)
                                {
                                    string ActionCode = "";
                                    if (isCancellation)
                                        ActionCode = msgConfig.ActionCode_Cancellation;
                                    else if (tCode.IsRepeat)
                                        ActionCode = msgConfig.ActionCode_Repeat;
                                    else if (bList.IsBarcodeWoSentEarlier && !String.IsNullOrEmpty(msgConfig.ActionCode_AddTestsInExistingBarcode))
                                        ActionCode = msgConfig.ActionCode_AddTestsInExistingBarcode;
                                    else
                                        ActionCode = ri.ActionCode.DefaultValue.ToString();

                                    SetRecordValue(ri, ri.ActionCode, !String.IsNullOrEmpty(ActionCode) ? ActionCode : (ri.ActionCode?.DefaultValue ?? ""));
                                }
                                if (ri.SampleType != null)
                                {
                                    SetRecordValue(ri, ri.SampleType, bList.SampleType);
                                }

                                positionNumber++;

                                string WOType = "WO";
                                if (isCancellation)
                                    WOType = "Cancel";


                                RecordInfo ri1 = AddOrderRecordHL7TCD(/*ref sequenceNumber_Order,*/ bList.barcode, tCode.instrumentCode);
                                if (ri.ActionCode != null)
                                {
                                    string ActionCode = "";
                                    if (isCancellation)
                                        ActionCode = msgConfig.ActionCode_Cancellation;
                                    else if (tCode.IsRepeat)
                                        ActionCode = msgConfig.ActionCode_Repeat;
                                    else if (bList.IsBarcodeWoSentEarlier && !String.IsNullOrEmpty(msgConfig.ActionCode_AddTestsInExistingBarcode))
                                        ActionCode = msgConfig.ActionCode_AddTestsInExistingBarcode;
                                    else
                                        ActionCode = ri.ActionCode.DefaultValue.ToString();

                                    SetRecordValue(ri, ri.ActionCode, !String.IsNullOrEmpty(ActionCode) ? ActionCode : (ri.ActionCode?.DefaultValue ?? ""));
                                }
                                if (ri.SampleType != null)
                                {
                                    SetRecordValue(ri, ri.SampleType, bList.SampleType);
                                }

                                positionNumber++;

                                WOType = "WO";
                                if (isCancellation)
                                    WOType = "Cancel";
                                UiMediator.AddUiGridData(AnalyzerID, bList.barcode, WOType, tCode.testCode, null, "", "");
                            }
                            if (msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6)
                            {
                                if (riPatient.TestCount != null)
                                {
                                    SetRecordValue(riPatient, riPatient.TestCount, bList.testlist.Count);
                                }
                            }
                        }
                        riOrder = null; //set null to start with new record in case of bulk WO
                    }
                    catch (Exception ex)
                    {
                        UiMediator.LogAndShowError(AnalyzerID, ex, "Error in preparing communication message of barcode " + bList.barcode);
                    }
                }

                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                {
                    AddContinuationPointerSegment(strHeader);
                }

                if (msgConfig.HeaderAndTerminationRecordRequired)
                {
                    AddTerminationRecord(barcodeList.Count(), terminationCode);
                }
            }
            catch (Exception ex)
            {
                string barcodesCommaSeparated = String.Join(", ", queryBarcodes.ToArray());
                ex.Data.Add("barcodes", barcodesCommaSeparated);
                UiMediator.LogAndShowError(msgConfig.AnalyzerID, ex, "Error preparing communication messages : " + barcodesCommaSeparated);
            }


            try
            {
                if (responseRecords.Any())
                {
                    foreach (RecordInfo ri in responseRecords)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (RecordFieldInfo rfi in ri.RecordFields.OrderBy(r => r.FieldNumber))
                        {
                            if (rfi.SupportsMultipleValues)
                            {
                                if (rfi.ComponentCount > 0 /*&& msgConfig.AnalyzerTypeID != AnalyzerTypes.AutoBio_A1000*/)
                                {
                                    int valueCount = 0;

                                    //List<string> mylist = new List<string>();

                                    var query = rfi.Components.Where(r => r.Value != null);
                                    if (query.Any())
                                    {
                                        valueCount = query.Max(r => ((List<object>)r.Value).Count);
                                    }

                                    //set to 1, to allow calling PrepareFinalValue to set default values
                                    if (valueCount == 0)
                                        valueCount = 1;

                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        rfi.Components.ForEach(r => PrepareFinalValueHL7(r, i + 1));
                                        try
                                        {
                                            sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => ((List<object>)r.Value)[i])));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        if (i < valueCount - 1)
                                            sb.Append(msgConfig.RepeatDelimiter);

                                        //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Vitros)
                                        //    sb.Replace(msgConfig.RepeatDelimiter + "^^^1.0+", msgConfig.RepeatDelimiter + "");//Added logic Vitros Analzyer to send work order along with dilution
                                    }
                                }
                                else
                                {
                                    int valueCount = ((List<object>)rfi.Value).Count;
                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        PrepareFinalValueHL7(rfi, i + 1);
                                    }

                                    sb.Append(String.Join(msgConfig.RepeatDelimiter.ToString(), (List<object>)rfi.Value));
                                }
                            }
                            else
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    rfi.Components.ForEach(r => PrepareFinalValueHL7(r));
                                    sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => r.Value)));

                                    #region Logic to replace some characters which machine is not accepting
                                    string test = sb.ToString();
                                    if (test.Contains("SPM"))
                                        sb.Replace("^BARCODE", "&BARCODE");
                                    #endregion
                                }
                                else
                                {
                                    try
                                    {
                                        PrepareFinalValueHL7(rfi);
                                        //Logic to replace ? to component delimeter which is ^
                                        sb.Replace("?", msgConfig.ComponentDelimiter.ToString());
                                        sb.Append(rfi.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            if (rfi.FieldNumber < ri.FieldCount - 1)
                                sb.Append(msgConfig.FieldDelimiter);
                        }

                        string strRecord = sb.ToString();
                        if (!msgConfig.IsFieldSizeInBytes)
                        {
                            if (msgConfig.RemoveTrailingEmptyDelimiters)
                            {
                                //remove multiple component delimiters, having no values inside |^^^^^|
                                strRecord = new System.Text.RegularExpressions.Regex("\\" + msgConfig.FieldDelimiter.Value.ToString() + "\\" + msgConfig.ComponentDelimiter.Value.ToString() + "+" + "\\" + msgConfig.FieldDelimiter.Value.ToString()).Replace(strRecord, msgConfig.FieldDelimiter.Value.ToString() + msgConfig.FieldDelimiter.Value.ToString());
                            }
                        }

                        records.Add(strRecord);
                    }
                    responseRecords.Clear();
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                {
                    records.Clear();

                    Dictionary<int, string> ReplaceCharacterList = new Dictionary<int, string>{
                        { 2, "[STX]" },
                        { 3, "[ETX]" },
                        { 21, "[NAK]" },
                        { 5, "[ENQ]" },
                        { 4, "[EOT]" },
                        { 6, "[ACK]" },
                        { 13, "[CR]" },
                        { 23, "[ETB]" },
                        { 10, "[LF]" },
                        { 1, "[SOH]" }
                        };

                    foreach (string strLine in System.IO.File.ReadLines(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                    {
                        string strLine_Modified = strLine;
                        ReplaceCharacterList.ToList<KeyValuePair<int, string>>().ForEach(r => strLine_Modified = strLine_Modified.Replace(r.Value, Convert.ToChar(r.Key).ToString()));
                        //string strFrame = strLine_Modified.Substring(2, strLine_Modified.Length - 7);
                        //responseFrames.Add(strFrame + FrameHandler.ComputeCheckSum(strFrame) + Characters.CR + Characters.LF);
                        records.Add(strLine_Modified);
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in sending message to Analyzer");
            }

            return records;
        }


        //Logic to prepare response frame which will be contains MSH (Header) and MSA (Acknowledgement)

        public List<string> PrepareResultResponseMessagesHL7_ACK()
        {
            List<string> records = new List<string>();

            try
            {
                if (!msgConfig.HeaderAndTerminationRecordRequired)
                {
                    if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
                    {
                        AddHeaderRecordHL7(strHeader);
                        AddMSARecordHL7ACK(strHeader);
                    }
                }
            }
            catch (Exception ex)
            {
                string barcodesCommaSeparated = String.Join(", ", queryBarcodesForResults.ToArray());
                ex.Data.Add("barcodes", barcodesCommaSeparated);
                UiMediator.LogAndShowError(msgConfig.AnalyzerID, ex, "Error preparing communication messages : " + barcodesCommaSeparated);
            }

            try
            {
                if (responseRecords.Any())
                {
                    foreach (RecordInfo ri in responseRecords)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (RecordFieldInfo rfi in ri.RecordFields.OrderBy(r => r.FieldNumber))
                        {
                            if (rfi.SupportsMultipleValues)
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    int valueCount = 0;

                                    var query = rfi.Components.Where(r => r.Value != null);
                                    if (query.Any())
                                    {
                                        valueCount = query.Max(r => ((List<object>)r.Value).Count);
                                    }

                                    //set to 1, to allow calling PrepareFinalValue to set default values
                                    if (valueCount == 0)
                                        valueCount = 1;

                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        rfi.Components.ForEach(r => PrepareFinalValueHL7(r, i + 1));
                                        try
                                        {
                                            sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => ((List<object>)r.Value)[i])));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        if (i < valueCount - 1)
                                            sb.Append(msgConfig.RepeatDelimiter);
                                    }
                                }
                                else
                                {
                                    int valueCount = ((List<object>)rfi.Value).Count;
                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        PrepareFinalValueHL7(rfi, i + 1);
                                    }

                                    sb.Append(String.Join(msgConfig.RepeatDelimiter.ToString(), (List<object>)rfi.Value));
                                }
                            }
                            else
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    rfi.Components.ForEach(r => PrepareFinalValueHL7(r));
                                    sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => r.Value)));
                                }
                                else
                                {
                                    try
                                    {
                                        PrepareFinalValueHL7(rfi);
                                        sb.Replace("?", msgConfig.ComponentDelimiter.ToString());
                                        sb.Append(rfi.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            if (rfi.FieldNumber < ri.FieldCount - 1)
                                sb.Append(msgConfig.FieldDelimiter);
                        }

                        string strRecord = sb.ToString();
                        if (!msgConfig.IsFieldSizeInBytes)
                        {
                            if (msgConfig.RemoveTrailingEmptyDelimiters)
                            {
                                //remove multiple component delimiters, having no values inside |^^^^^|
                                strRecord = new System.Text.RegularExpressions.Regex("\\" + msgConfig.FieldDelimiter.Value.ToString() + "\\" + msgConfig.ComponentDelimiter.Value.ToString() + "+" + "\\" + msgConfig.FieldDelimiter.Value.ToString()).Replace(strRecord, msgConfig.FieldDelimiter.Value.ToString() + msgConfig.FieldDelimiter.Value.ToString());
                            }
                        }

                        records.Add(strRecord);
                    }
                    responseRecords.Clear();
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                {
                    records.Clear();

                    Dictionary<int, string> ReplaceCharacterList = new Dictionary<int, string>{
                { 2, "[STX]" },
                { 3, "[ETX]" },
                { 21, "[NAK]" },
                { 5, "[ENQ]" },
                { 4, "[EOT]" },
                { 6, "[ACK]" },
                { 13, "[CR]" },
                { 23, "[ETB]" },
                { 10, "[LF]" },
                { 1, "[SOH]" }
                };

                    foreach (string strLine in System.IO.File.ReadLines(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                    {
                        string strLine_Modified = strLine;
                        ReplaceCharacterList.ToList<KeyValuePair<int, string>>().ForEach(r => strLine_Modified = strLine_Modified.Replace(r.Value, Convert.ToChar(r.Key).ToString()));
                        records.Add(strLine_Modified);
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in sending message to Analyzer");
            }

            return records;
        }

        public List<string> PrepareResultResponseMessagesHL7()
        {
            List<string> records = new List<string>();

            try
            {
                if (!msgConfig.HeaderAndTerminationRecordRequired && msgConfig.AnalyzerTypeID != AnalyzerTypes.Zybio_EXZ_6000_H6 && msgConfig.AnalyzerTypeID != AnalyzerTypes.MISPA_CX4)
                {
                    // Header Message
                    ResultRecordInfoHL7ACKResults(strHeader);
                    ////MSA
                    AddMSARecordHL7ACKForResults(strHeader);
                }

                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                {
                    AddHeaderRecordHL7(strHeader);
                    AddMSARecordHL7ACK(strHeader);
                    AddErrorRecordHL7ACK(strHeader);
                    AddQueryAckRecordHL7ACK(strHeader);

                }
            }
            catch (Exception ex)
            {
                string barcodesCommaSeparated = String.Join(", ", queryBarcodesForResults.ToArray());
                ex.Data.Add("barcodes", barcodesCommaSeparated);
                UiMediator.LogAndShowError(msgConfig.AnalyzerID, ex, "Error preparing communication messages : " + barcodesCommaSeparated);
            }

            try
            {
                if (responseRecords.Any())
                {
                    foreach (RecordInfo ri in responseRecords)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (RecordFieldInfo rfi in ri.RecordFields.OrderBy(r => r.FieldNumber))
                        {
                            if (rfi.SupportsMultipleValues)
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    int valueCount = 0;

                                    var query = rfi.Components.Where(r => r.Value != null);
                                    if (query.Any())
                                    {
                                        valueCount = query.Max(r => ((List<object>)r.Value).Count);
                                    }

                                    //set to 1, to allow calling PrepareFinalValue to set default values
                                    if (valueCount == 0)
                                        valueCount = 1;

                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        rfi.Components.ForEach(r => PrepareFinalValueHL7(r, i + 1));
                                        try
                                        {
                                            sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => ((List<object>)r.Value)[i])));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                        if (i < valueCount - 1)
                                            sb.Append(msgConfig.RepeatDelimiter);
                                    }
                                }
                                else
                                {
                                    int valueCount = ((List<object>)rfi.Value).Count;
                                    for (int i = 0; i < valueCount; i++)
                                    {
                                        PrepareFinalValueHL7(rfi, i + 1);
                                    }

                                    sb.Append(String.Join(msgConfig.RepeatDelimiter.ToString(), (List<object>)rfi.Value));
                                }
                            }
                            else
                            {
                                if (rfi.ComponentCount > 0)
                                {
                                    rfi.Components.ForEach(r => PrepareFinalValueHL7(r));
                                    sb.Append(String.Join(msgConfig.ComponentDelimiter.ToString(), rfi.Components.OrderBy(r => r.ComponentNumber).Select(r => r.Value)));
                                }
                                else
                                {
                                    try
                                    {
                                        PrepareFinalValueHL7(rfi);
                                        sb.Replace("?", msgConfig.ComponentDelimiter.ToString());
                                        sb.Append(rfi.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            if (rfi.FieldNumber < ri.FieldCount - 1)
                                sb.Append(msgConfig.FieldDelimiter);
                        }

                        string strRecord = sb.ToString();
                        if (!msgConfig.IsFieldSizeInBytes)
                        {
                            if (msgConfig.RemoveTrailingEmptyDelimiters)
                            {
                                //remove multiple component delimiters, having no values inside |^^^^^|
                                strRecord = new System.Text.RegularExpressions.Regex("\\" + msgConfig.FieldDelimiter.Value.ToString() + "\\" + msgConfig.ComponentDelimiter.Value.ToString() + "+" + "\\" + msgConfig.FieldDelimiter.Value.ToString()).Replace(strRecord, msgConfig.FieldDelimiter.Value.ToString() + msgConfig.FieldDelimiter.Value.ToString());
                            }
                        }

                        records.Add(strRecord);
                    }
                    responseRecords.Clear();
                }

                if (System.IO.File.Exists(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                {
                    records.Clear();

                    Dictionary<int, string> ReplaceCharacterList = new Dictionary<int, string>{
                        { 2, "[STX]" },
                        { 3, "[ETX]" },
                        { 21, "[NAK]" },
                        { 5, "[ENQ]" },
                        { 4, "[EOT]" },
                        { 6, "[ACK]" },
                        { 13, "[CR]" },
                        { 23, "[ETB]" },
                        { 10, "[LF]" },
                        { 1, "[SOH]" }
                        };

                    foreach (string strLine in System.IO.File.ReadLines(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "dummy.txt")))
                    {
                        string strLine_Modified = strLine;
                        ReplaceCharacterList.ToList<KeyValuePair<int, string>>().ForEach(r => strLine_Modified = strLine_Modified.Replace(r.Value, Convert.ToChar(r.Key).ToString()));
                        records.Add(strLine_Modified);
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(AnalyzerID, ex, "Error in sending message to Analyzer");
            }

            return records;
        }

        private void PrepareFinalRecords(List<string> records)
        {
            //common logic goes here
            if (records.Count > 0)
            {
                if (!msgConfig.IsFieldSizeInBytes)
                {
                    string characterToAppend = Characters.CR;

                    for (int i = 0; i < records.Count; i++)
                    {
                        records[i] = records[i] + characterToAppend;
                    }
                }
            }
        }

        // Frame buliding along with VT and FS and CR for Work order
        private void PrepareFinalRecordsHL7(List<string> records)
        {
            //common logic goes here
            if (records.Count > 2)
            {
                if (!msgConfig.IsFieldSizeInBytes)
                {
                    string characterToAppend = Characters.CR;
                    string characterToAppendFS = Characters.FS;
                    string characterToAppendVT = Characters.VT;

                    for (int i = 0; i < records.Count; i++)
                    {
                        if (i == 0)
                            records[i] = characterToAppendVT + records[i] + characterToAppend;
                        else if (i == 3)
                            records[i] = records[i] + characterToAppend + characterToAppendFS + characterToAppend;
                        else if (i == 4)
                            records[i] = characterToAppendVT + records[i] + characterToAppend;
                        else
                            records[i] = records[i] + characterToAppend;
                    }
                }
            }
        }

        // Frame buliding along with VT and FS and CR For Results
        private void PrepareFinalRecordsHL7Results(List<string> records)
        {
            //common logic goes here
            if (records.Count > 0)
            {
                if (!msgConfig.IsFieldSizeInBytes)
                {
                    string characterToAppendCR = Characters.CR;
                    string characterToAppendFS = Characters.FS;
                    string characterToAppendVT = Characters.VT;

                    for (int i = 0; i < records.Count; i++)
                    {
                        if (i == 0)
                            records[i] = characterToAppendVT + records[i] + characterToAppendCR;
                        else
                            records[i] = records[i] + characterToAppendCR;

                        if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 || msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
                        {
                            if (i == 3)
                                records[i] = records[i] + characterToAppendFS + characterToAppendCR;
                        }
                    }
                }
            }
        }



        public int UpdateTestValues()
        {
            DateTime dtmStart = DateTime.Now;

            if (testResults == null || testResults.results == null || testResults.results.Count == 0)
            {
                UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "No result values are available to upload.");
                return 0;
            }

            var testIdsFromMapping = CachedData.TestList.Select(k => k.testid);

            if (new int[] { 9 }.Contains(msgConfig.AnalyzerTypeID))
            {
                foreach (ResultList barcodeDetail in testResults.results)
                {
                    var zeroValueParameters = CachedData.TestList.Where(r => !new string[] { "PCT", "PLCR", "MPV", "PDW", "RDWSD", "RDWSD", "NRBC", "AMON", "AEOS", "ABAS", "IG", "NRBC%", "MONO", "EOS", "BASO", "IG%" }.Contains(r.testcode) && barcodeDetail.investigationresults.Where(r1 => r1.testcode == r.testcode).SingleOrDefault()?.result == 0);
                    if (zeroValueParameters.Any())
                    {
                        UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Error, "All values removed for barcode " + barcodeDetail.barcode + ", as contains zero values for parameters: " + String.Join(", ", zeroValueParameters.Select(r => r.testcode)));
                        //move line to be below message line, otherwise not showing test names after removing
                        barcodeDetail.investigationresults.Clear();
                    }
                }
            }

            //remove testcodes that are not present in mapping
            foreach (ResultList barcodeDetail in testResults.results)
            {
                string testCodesRemoved = "";

                var testcodesToBeRemoved = barcodeDetail.investigationresults
                    .Where(r => !testIdsFromMapping.Contains(r.testid))
                    .ToList();

                if (msgConfig.RemoveUnmappedTestcodes)
                {
                    testCodesRemoved = String.Join(", ", testcodesToBeRemoved.Select(r => r.instrumentCode));
                    testcodesToBeRemoved.ForEach(r => barcodeDetail.investigationresults.Remove(r));
                    if (testcodesToBeRemoved.Count > 0)
                    {
                        UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Testcodes removed for barcode " + barcodeDetail.barcode + " : " + testCodesRemoved);
                    }
                }
                else
                {
                    testcodesToBeRemoved.ForEach(r => r.testcode = r.instrumentCode);
                }
            }

            //testResults.BarcodeDetails.Where(r => r.barcode.Contains("-")).ToList().ForEach(r=>r.barcode = r.barcode.Replace("-", ""));

            var exraLengthResults = testResults.results.Where(r => r.barcode.Length > 25 || r.investigationresults.Any(a => a.InstrumentRemarks?.Length > 250 || a.symbol?.Length > 25 || a.instrumentresultnote?.Length > 25 || a.resultunit?.Length > 10)).ToList();
            if (exraLengthResults.Count > 0)
            {
                exraLengthResults.ForEach(r => testResults.results.Remove(r));
                UiMediator.AddUiMessage(AnalyzerID, (int)MessageType.Warning, "Barcodes removed due to extra length : " + String.Join(", ", exraLengthResults.Select(r => r.barcode).ToArray()));
            }

            testResults.instrumentid = msgConfig.AnalyzerID;
            try
            {
                UpdateResultOutput resultResponse = WebAPI.UpdateResult(testResults, TimeoutSeconds: 15); //set timeout to avoid thread overload in case when connectivity fails and threads are open unnecessarily
                if (!resultResponse.outputdetails.Contains("Success"))
                    throw new Exception(resultResponse.outputdetails);
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(testResults.results[0].investigationresults[0].instrumentid, ex, "Error updating test result value, will be re-attempted in a while.");
                ResultUpdater.AddResult(testResults);
            }

            MessageLogger.WriteTimeDiffLog("UpdateResult", dtmStart, String.Join(", ", testResults.results.Select(r => r.barcode)));
            return testResults.results.Sum(r => r.investigationresults.Count);
        }
        public static DataTable GetResultsTableStructure()
        {
            DataTable dtResults = new DataTable();
            dtResults.Columns.Add("BARCODE");
            dtResults.Columns.Add("TESTCODE");
            dtResults.Columns.Add("TESTVALUE", typeof(float));
            dtResults.Columns.Add("UNIT");
            dtResults.Columns.Add("PREFIX");
            dtResults.Columns.Add("COMMENT_TEXTS");
            dtResults.Columns.Add("RESULTED_TIME", typeof(DateTime));
            dtResults.Columns["TESTVALUE"].DefaultValue = 0;
            return dtResults;
        }

        //private object ExtractRecordValueForAUForBarcode(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        //{
        //    string strValue = "";

        //    strValue = recordReceived.Substring(15, 10);

        //    //rfi.Value = strValue;

        //    return strValue;
        //}


        private object ExtractRecordValueForHL7ACK(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            string strValue = "";

            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;
            //string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];

                if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                {
                    if (strValue == "")
                        return null;

                    string[] arrComponents = strValue.Split('^');

                    strValue = arrComponents[1].ToString();
                }
            }
            // Added logic for Auto_Bio analyzer where need to give specific response in header to process work order or result.
            if (strValue == "QBP^Q11^QBP_Q11")
                strValue = "RSP^K11^RSP_K11";
            //else if (strValue == "REQ5")
            //    strValue = "RSP^K11^RSP_K11";
            //else if (strValue == "REQ7")
            //    strValue = "RSP7";
            //else if (strValue == "REQ9")
            //    strValue = "RSP9";
            //else if (strValue == "REQ15")
            //    strValue = "RSP15";

            return strValue;
        }

        private object ExtractRecordValueHL7Prefix(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;

            string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];
                if (rfi != ri.DelimiterCharacters)
                {
                    if (rfi.SupportsMultipleValues)
                    {
                        if (msgConfig.SupportsMultipleValuesInSingleRecord)
                        {
                            int fieldCountOfMultipleValues = ri.RecordFields.Where(r => r.SupportsMultipleValues).Count();
                            strValue = arrFields[rfi.FieldNumber_Incoming + ((positionNumber - 1) * fieldCountOfMultipleValues)];
                        }
                        else
                        {
                            strValue = strValue.Split(msgConfig.RepeatDelimiter.Value)[positionNumber - 1];
                        }
                    }

                    if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                    {
                        if (strValue == "")
                            return null;

                        string[] arrComponents = strValue.Split('^');

                        if (arrComponents.Length <= rfi.ComponentNumber)
                            //throw new MyException("Input value has lesser components than defined.");
                            return null;

                        strValue = arrComponents[rfi.ComponentNumber];
                    }

                    if (rfi.ForwardSlashSeparatedComponentNumber > 0)
                    {
                        strValue = strValue.Split('/')[rfi.ForwardSlashSeparatedComponentNumber - 1];
                    }

                    if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA)
                    {
                        if (strValue.Contains("<"))
                            strValue = "<";
                        else if (strValue.Contains(">"))
                            strValue = ">";
                        else
                            strValue = "";
                    }
                }
            }

            return strValue;
        }

        // To extract report values
        private object ExtractRecordValueHL7(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;

            string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];

                if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
                {
                    strValue = strValue.Replace(">", "");
                    strValue = strValue.Replace("<", "");
                }

                if (rfi != ri.DelimiterCharacters)
                {
                    if (rfi.SupportsMultipleValues)
                    {
                        if (msgConfig.SupportsMultipleValuesInSingleRecord)
                        {
                            int fieldCountOfMultipleValues = ri.RecordFields.Where(r => r.SupportsMultipleValues).Count();
                            strValue = arrFields[rfi.FieldNumber_Incoming + ((positionNumber - 1) * fieldCountOfMultipleValues)];
                        }
                        else
                        {
                            strValue = strValue.Split(msgConfig.RepeatDelimiter.Value)[positionNumber - 1];
                        }
                    }

                    if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                    {
                        if (strValue == "")
                            return null;

                        string[] arrComponents = strValue.Split('^');

                        if (arrComponents.Length <= rfi.ComponentNumber)
                            //throw new MyException("Input value has lesser components than defined.");
                            return null;

                        strValue = arrComponents[rfi.ComponentNumber];

                        //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA && strValue.Contains("."))
                        //{
                        //    arrComponents = strValue.Split('.');

                        //    strValue = arrComponents[1];
                        //}
                    }

                    if (rfi.ForwardSlashSeparatedComponentNumber > 0)
                    {
                        strValue = strValue.Split('/')[rfi.ForwardSlashSeparatedComponentNumber - 1];
                    }

                    if ((strValue.Contains("<") || strValue.Contains(">")) && (msgConfig.AnalyzerTypeID == AnalyzerTypes.ATELLICA))
                    {
                        strValue = strValue.Replace("<", "").Trim();
                        strValue = strValue.Replace(">", "").Trim();
                    }
                }
            }
            else
            {
                int fieldValueStartIndex = GetFieldValueStartIndex(ri, rfi);

                RecordFieldInfo rfiParent = ri.RecordFields[rfi.FieldNumber_Incoming];
                if (rfiParent.ComponentCount > 0)
                {
                    if (!rfi.SupportsMultipleValues)
                    {
                        positionNumber = 1;
                    }
                    int componentValueStartIndex = rfiParent.Components.Where(r => r.ComponentNumber < rfi.ComponentNumber).Sum(r => r.BytesLength);
                    strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfiParent.BytesLength_PerComponentSet) + componentValueStartIndex, rfi.BytesLength);
                }
                else
                {
                    if (rfi.SupportsMultipleValues)
                        strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfi.BytesLength), rfi.BytesLength);
                    else
                        strValue = recordReceived.Substring(fieldValueStartIndex, rfi.BytesLength);
                }

            }

            //try to convert value, if error, return raw value in error data
            try
            {
                //Advia 1800: ignore overflow testvalue, value equals "////////"
                if (rfi is NumericFieldInfo && strValue.Replace("/", "") == "")
                    return null;
                //Sysmex XN 1000: ---- : Analysis or hardware error, ++++ : Out of range, also ignore image file results
                else if (rfi is NumericFieldInfo && (strValue.Replace("-", "") == "" || strValue.Replace("+", "") == "" || strValue.EndsWith(".PNG")))
                    return null;

                if (rfi.TrimTextWhileExtracting)
                    strValue = strValue.Trim();

                if (strValue == "")
                    return null;

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (sfi.MaxLength > 0 && strValue.Length > sfi.MaxLength)
                    {
                        strValue = strValue.Substring(0, sfi.MaxLength);
                    }

                }
                else if (rfi is DateFieldInfo)
                {
                    DateTime dtmValue;
                    if (!DateTime.TryParseExact(strValue, (rfi as DateFieldInfo).DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtmValue))
                        throw new MyException("Resulted date time (" + strValue + ") is not in correct format.");

                    return dtmValue/*.ToUniversalTime()*/;
                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (nfi.DecimalPlaces > 0)
                    {
                        decimal dcmValue = 0;
                        if (!decimal.TryParse(strValue.Replace(" ", ""), out dcmValue)) //space replaced to get negative symbol which is separated by space with numeric value
                            throw new MyException("Decimal value (" + strValue + ") is not in correct format.");
                        return Math.Round(dcmValue, nfi.DecimalPlaces);
                    }
                    else
                    {
                        int intValue = 0;
                        if (!int.TryParse(strValue, out intValue))
                            throw new MyException("Integer value (" + strValue + ") is not in correct format.");
                        return intValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(ex.Message + " (Value: " + strValue + ")", ex);
                exNew.Data.Add("RawValue", strValue);
                throw exNew;
            }

            return strValue;
        }

        // Added Logic here to add GUID +2 generate unique number
        private object ExtractRecordValueHL7OKRes(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;

            string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];

                if (arrFields[0].Contains("MSH"))
                    strValue = strValue + 2;

                if (rfi != ri.DelimiterCharacters)
                {
                    if (rfi.SupportsMultipleValues)
                    {
                        if (msgConfig.SupportsMultipleValuesInSingleRecord)
                        {
                            int fieldCountOfMultipleValues = ri.RecordFields.Where(r => r.SupportsMultipleValues).Count();
                            strValue = arrFields[rfi.FieldNumber_Incoming + ((positionNumber - 1) * fieldCountOfMultipleValues)];
                        }
                        else
                        {
                            strValue = strValue.Split(msgConfig.RepeatDelimiter.Value)[positionNumber - 1];
                        }
                    }

                    if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                    {
                        if (strValue == "")
                            return null;

                        string[] arrComponents = strValue.Split('^');
                        if (msgConfig.AnalyzerTypeID == 7 && strValue.Contains("&"))
                            arrComponents = strValue.Split('&');
                        if (arrComponents.Length <= rfi.ComponentNumber)
                            //throw new MyException("Input value has lesser components than defined.");
                            return null;

                        strValue = arrComponents[rfi.ComponentNumber];
                    }

                    if (rfi.ForwardSlashSeparatedComponentNumber > 0)
                    {
                        strValue = strValue.Split('/')[rfi.ForwardSlashSeparatedComponentNumber - 1];
                    }
                }
            }
            else
            {
                int fieldValueStartIndex = GetFieldValueStartIndex(ri, rfi);

                RecordFieldInfo rfiParent = ri.RecordFields[rfi.FieldNumber_Incoming];
                if (rfiParent.ComponentCount > 0)
                {
                    if (!rfi.SupportsMultipleValues)
                    {
                        positionNumber = 1;
                    }
                    int componentValueStartIndex = rfiParent.Components.Where(r => r.ComponentNumber < rfi.ComponentNumber).Sum(r => r.BytesLength);
                    strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfiParent.BytesLength_PerComponentSet) + componentValueStartIndex, rfi.BytesLength);
                }
                else
                {
                    if (rfi.SupportsMultipleValues)
                        strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfi.BytesLength), rfi.BytesLength);
                    else
                        strValue = recordReceived.Substring(fieldValueStartIndex, rfi.BytesLength);
                }

            }

            //try to convert value, if error, return raw value in error data
            try
            {
                //Advia 1800: ignore overflow testvalue, value equals "////////"
                if (rfi is NumericFieldInfo && strValue.Replace("/", "") == "")
                    return null;
                //Sysmex XN 1000: ---- : Analysis or hardware error, ++++ : Out of range, also ignore image file results
                else if (rfi is NumericFieldInfo && (strValue.Replace("-", "") == "" || strValue.Replace("+", "") == "" || strValue.EndsWith(".PNG")))
                    return null;

                if (rfi.TrimTextWhileExtracting)
                    strValue = strValue.Trim();

                if (strValue == "")
                    return null;

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (sfi.MaxLength > 0 && strValue.Length > sfi.MaxLength)
                    {
                        strValue = strValue.Substring(0, sfi.MaxLength);
                    }

                }
                else if (rfi is DateFieldInfo)
                {
                    DateTime dtmValue;
                    if (!DateTime.TryParseExact(strValue, (rfi as DateFieldInfo).DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtmValue))
                        throw new MyException("Resulted date time (" + strValue + ") is not in correct format.");

                    return dtmValue/*.ToUniversalTime()*/;
                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (nfi.DecimalPlaces > 0)
                    {
                        decimal dcmValue = 0;
                        if (!decimal.TryParse(strValue.Replace(" ", ""), out dcmValue)) //space replaced to get negative symbol which is separated by space with numeric value
                            throw new MyException("Decimal value (" + strValue + ") is not in correct format.");
                        return Math.Round(dcmValue, nfi.DecimalPlaces);
                    }
                    else
                    {
                        int intValue = 0;
                        if (!int.TryParse(strValue, out intValue))
                            throw new MyException("Integer value (" + strValue + ") is not in correct format.");
                        return intValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(ex.Message + " (Value: " + strValue + ")", ex);
                exNew.Data.Add("RawValue", strValue);
                throw exNew;
            }

            return strValue;
        }
        // Added Logic here to add GUID +3 generate unique number
        private object ExtractRecordValueHL7OKResResult(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;

            string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];

                if (arrFields[0].Contains("MSH"))
                    strValue = strValue + 3;

                if (rfi != ri.DelimiterCharacters)
                {
                    if (rfi.SupportsMultipleValues)
                    {
                        if (msgConfig.SupportsMultipleValuesInSingleRecord)
                        {
                            int fieldCountOfMultipleValues = ri.RecordFields.Where(r => r.SupportsMultipleValues).Count();
                            strValue = arrFields[rfi.FieldNumber_Incoming + ((positionNumber - 1) * fieldCountOfMultipleValues)];
                        }
                        else
                        {
                            strValue = strValue.Split(msgConfig.RepeatDelimiter.Value)[positionNumber - 1];
                        }
                    }

                    if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                    {
                        if (strValue == "")
                            return null;

                        string[] arrComponents = strValue.Split('^');
                        if (msgConfig.AnalyzerTypeID == 7 && strValue.Contains("&"))
                            arrComponents = strValue.Split('&');
                        if (arrComponents.Length <= rfi.ComponentNumber)
                            //throw new MyException("Input value has lesser components than defined.");
                            return null;

                        strValue = arrComponents[rfi.ComponentNumber];
                    }

                    if (rfi.ForwardSlashSeparatedComponentNumber > 0)
                    {
                        strValue = strValue.Split('/')[rfi.ForwardSlashSeparatedComponentNumber - 1];
                    }
                }
            }
            else
            {
                int fieldValueStartIndex = GetFieldValueStartIndex(ri, rfi);

                RecordFieldInfo rfiParent = ri.RecordFields[rfi.FieldNumber_Incoming];
                if (rfiParent.ComponentCount > 0)
                {
                    if (!rfi.SupportsMultipleValues)
                    {
                        positionNumber = 1;
                    }
                    int componentValueStartIndex = rfiParent.Components.Where(r => r.ComponentNumber < rfi.ComponentNumber).Sum(r => r.BytesLength);
                    strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfiParent.BytesLength_PerComponentSet) + componentValueStartIndex, rfi.BytesLength);
                }
                else
                {
                    if (rfi.SupportsMultipleValues)
                        strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfi.BytesLength), rfi.BytesLength);
                    else
                        strValue = recordReceived.Substring(fieldValueStartIndex, rfi.BytesLength);
                }

            }

            //try to convert value, if error, return raw value in error data
            try
            {
                //Advia 1800: ignore overflow testvalue, value equals "////////"
                if (rfi is NumericFieldInfo && strValue.Replace("/", "") == "")
                    return null;
                //Sysmex XN 1000: ---- : Analysis or hardware error, ++++ : Out of range, also ignore image file results
                else if (rfi is NumericFieldInfo && (strValue.Replace("-", "") == "" || strValue.Replace("+", "") == "" || strValue.EndsWith(".PNG")))
                    return null;

                if (rfi.TrimTextWhileExtracting)
                    strValue = strValue.Trim();

                if (strValue == "")
                    return null;

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (sfi.MaxLength > 0 && strValue.Length > sfi.MaxLength)
                    {
                        strValue = strValue.Substring(0, sfi.MaxLength);
                    }

                }
                else if (rfi is DateFieldInfo)
                {
                    DateTime dtmValue;
                    if (!DateTime.TryParseExact(strValue, (rfi as DateFieldInfo).DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtmValue))
                        throw new MyException("Resulted date time (" + strValue + ") is not in correct format.");

                    return dtmValue/*.ToUniversalTime()*/;
                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (nfi.DecimalPlaces > 0)
                    {
                        decimal dcmValue = 0;
                        if (!decimal.TryParse(strValue.Replace(" ", ""), out dcmValue)) //space replaced to get negative symbol which is separated by space with numeric value
                            throw new MyException("Decimal value (" + strValue + ") is not in correct format.");
                        return Math.Round(dcmValue, nfi.DecimalPlaces);
                    }
                    else
                    {
                        int intValue = 0;
                        if (!int.TryParse(strValue, out intValue))
                            throw new MyException("Integer value (" + strValue + ") is not in correct format.");
                        return intValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(ex.Message + " (Value: " + strValue + ")", ex);
                exNew.Data.Add("RawValue", strValue);
                throw exNew;
            }

            return strValue;
        }
        private object ExtractRecordValue(string recordReceived, RecordInfo ri, RecordFieldInfo rfi, int positionNumber = 1)
        {
            if (rfi == null || rfi.FieldNumber_Incoming == -1)
                return null;

            string strValue = "";

            if (!msgConfig.IsFieldSizeInBytes)
            {
                string[] arrFields = recordReceived.Split(msgConfig.FieldDelimiter.Value);
                if (arrFields.Length <= rfi.FieldNumber_Incoming)
                    //throw new MyException("Input value has lesser fields than defined.");
                    return null;

                strValue = arrFields[rfi.FieldNumber_Incoming];

                if (rfi != ri.DelimiterCharacters)
                {
                    if (rfi.SupportsMultipleValues)
                    {
                        if (msgConfig.SupportsMultipleValuesInSingleRecord)
                        {
                            int fieldCountOfMultipleValues = ri.RecordFields.Where(r => r.SupportsMultipleValues).Count();
                            strValue = arrFields[rfi.FieldNumber_Incoming + ((positionNumber - 1) * fieldCountOfMultipleValues)];
                        }
                        else
                        {
                            strValue = strValue.Split(msgConfig.RepeatDelimiter.Value)[positionNumber - 1];
                        }
                    }

                    if (rfi.ComponentNumber > -1 /*|| msgConfig.AnalyzerTypeID == 304*/)
                    {
                        if (strValue == "")
                            return null;

                        string[] arrComponents = strValue.Split('^');
                        if (msgConfig.AnalyzerTypeID == 152 || msgConfig.AnalyzerTypeID == 154)
                            arrComponents = strValue.Split('!');
                        if (arrComponents.Length <= rfi.ComponentNumber)
                            //throw new MyException("Input value has lesser components than defined.");
                            if (msgConfig.AnalyzerTypeID == 47 /*|| msgConfig.AnalyzerTypeID==304*/) //done for Sediment result value, as it is not divided in components
                                rfi.ComponentNumber = 1;
                            else
                                return null;

                        strValue = arrComponents[rfi.ComponentNumber];

                        //if (strValue.Contains("+") && msgConfig.AnalyzerTypeID == AnalyzerTypes.Vitros)
                        //{
                        //    string[] arrComponentsTESt = strValue.Split('+');
                        //    strValue = arrComponentsTESt[1];
                        //}
                    }
                    //To capture barcode for vitros machine
                    //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Vitros && strValue.Contains("^") && rfi.ComponentNumber==-1)
                    //{
                    //    if (strValue == "")
                    //        return null;

                    //    string[] arrComponents = strValue.Split('^');

                    //    strValue = arrComponents[0];
                    //}
                    if (msgConfig.AnalyzerTypeID == 7)
                    {
                        string[] arrComponentss = strValue.Split('^');
                        strValue = arrComponentss[0];
                    }
                    if (rfi.ForwardSlashSeparatedComponentNumber > 0)
                    {
                        strValue = strValue.Split('/')[rfi.ForwardSlashSeparatedComponentNumber - 1];
                    }
                }
            }
            else
            {
                int fieldValueStartIndex = GetFieldValueStartIndex(ri, rfi);


                RecordFieldInfo rfiParent = ri.RecordFields[rfi.FieldNumber_Incoming];
                if (rfiParent.ComponentCount > 0)
                {
                    if (!rfi.SupportsMultipleValues)
                    {
                        positionNumber = 1;
                    }
                    int componentValueStartIndex = rfiParent.Components.Where(r => r.ComponentNumber < rfi.ComponentNumber).Sum(r => r.BytesLength);

                    //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Bechman && recordReceived.Contains("DUBL"))
                    //    fieldValueStartIndex = 35;

                    strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfiParent.BytesLength_PerComponentSet) + componentValueStartIndex, rfi.BytesLength);
                }
                else
                {
                    if (rfi.SupportsMultipleValues)
                        strValue = recordReceived.Substring(fieldValueStartIndex + ((positionNumber - 1) * rfi.BytesLength), rfi.BytesLength);
                    else
                        strValue = recordReceived.Substring(fieldValueStartIndex, rfi.BytesLength);
                }

            }

            //try to convert value, if error, return raw value in error data
            try
            {
                //Advia 1800: ignore overflow testvalue, value equals "////////"
                if (rfi is NumericFieldInfo && strValue.Replace("/", "") == "")
                    return null;
                //Sysmex XN 1000: ---- : Analysis or hardware error, ++++ : Out of range, also ignore image file results
                else if (rfi is NumericFieldInfo && (strValue.Replace("-", "") == "" || strValue.Replace("+", "") == "" || strValue.EndsWith(".PNG")))
                    return null;

                if (rfi.TrimTextWhileExtracting)
                    strValue = strValue.Trim();

                if (strValue == "")
                    return null;

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (sfi.MaxLength > 0 && strValue.Length > sfi.MaxLength && msgConfig.AnalyzerTypeID != AnalyzerTypes.Bechman)
                    {
                        strValue = strValue.Substring(0, sfi.MaxLength);
                    }
                    //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Bechman && strValue.Contains("DUBL"))
                    //{
                    //    strValue = strValue.Substring(0, sfi.BytesLength);
                    //}
                    //else if (msgConfig.AnalyzerTypeID == 302 && sfi.SupportsMultipleValues==true && sfi.)
                    //{
                    //        decimal dcmValue = 0;
                    //        dcmValue = Convert.ToDecimal(strValue);
                    //        return dcmValue;
                    //}
                }
                else if (rfi is DateFieldInfo)
                {
                    DateTime dtmValue;
                    if (!DateTime.TryParseExact(strValue, (rfi as DateFieldInfo).DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtmValue))
                        throw new MyException("Resulted date time (" + strValue + ") is not in correct format.");

                    return dtmValue/*.ToUniversalTime()*/;
                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (nfi.DecimalPlaces > 0)
                    {
                        decimal dcmValue = 0;
                        if (!decimal.TryParse(strValue.Replace(" ", ""), out dcmValue)) //space replaced to get negative symbol which is separated by space with numeric value
                            throw new MyException("Decimal value (" + strValue + ") is not in correct format.");
                        return Math.Round(dcmValue, nfi.DecimalPlaces);
                    }
                    else
                    {
                        int intValue = 0;
                        if (!int.TryParse(strValue, out intValue))
                            throw new MyException("Integer value (" + strValue + ") is not in correct format.");
                        return intValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exNew = new Exception(ex.Message + " (Value: " + strValue + ")", ex);
                exNew.Data.Add("RawValue", strValue);
                throw exNew;
            }

            return strValue;
        }

        private int GetFieldValueStartIndex(RecordInfo ri, RecordFieldInfo rfi)
        {
            return ri.RecordFields.Where(r => r.FieldNumber_Incoming < rfi.FieldNumber_Incoming).Sum(r => r.BytesLength);
        }

        private int GetRepeatValuesCount(string recordReceived, RecordFieldInfo rfi)
        {
            if (!msgConfig.IsFieldSizeInBytes)
            {
                return recordReceived.Split(msgConfig.FieldDelimiter.Value)[rfi.FieldNumber_Incoming].Where(c => c == msgConfig.RepeatDelimiter).Count() + 1;
            }
            else
            {
                throw new Exception("Values count cannot be detected for this type.");
            }
        }

        //private void SetRecordValueBechman(RecordInfo ri, RecordFieldInfo rfi, object value/*, int positionNumber = 0*/)
        //{

        //    rfi.Value = value;
        //}
        private void SetRecordValueHL7ForBarcodeReadError(RecordInfo ri, RecordFieldInfo rfi, object value/*, int positionNumber = 0*/)
        {
            if (rfi == null || rfi.FieldNumber == -1)
                return;

            #region Validate and populate value accordingly
            if (rfi.IsMandatory && value == null)
            {
                throw new MyException("Value is mandatory");
            }
            else
            {
                if (rfi.ValidValues != null && rfi.ValidValues.Count > 0)
                {
                    //for String, set value from set of valid values for purpose of keeping character case of allowed values only
                    if (rfi is StringFieldInfo)
                    {
                        if (value != null && !String.IsNullOrEmpty(value.ToString()))
                            value = rfi.ValidValues.Select(r => r.ToString()).Where(r => String.Equals(r, value.ToString(), StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    }
                    else
                    {
                        if (!rfi.ValidValues.Contains(value))
                            value = null;
                    }
                }

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (value != null)
                    {
                        string strValue = (string)value;
                        //replace non-printable characters - for Architect
                        strValue = InterfaceHelper.RemoveNonPrintableCharacters(strValue);

                        //replace non-ASCII characters, error occurred in XN9000 due to unicode character
                        strValue = InterfaceHelper.RemoveNonAsciiCharacters(strValue);

                        //replace characters having special meaning (WO ONLINE FORMAT error in AU due to & symbol)
                        if (rfi != ri.DelimiterCharacters)
                        {
                            char[] charactersToRemove = new char[] { '|', '\\', '^', '&' };
                            if (charactersToRemove.Any(r => strValue.IndexOf(r) > -1))
                            {
                                foreach (char chr in charactersToRemove)
                                    strValue = strValue.Replace(chr, '?');
                            }
                        }

                        if (sfi.MaxLength > 0 && (strValue).Length > sfi.MaxLength)
                        {
                            strValue = (strValue).Substring(0, sfi.MaxLength);
                        }

                        value = strValue;
                    }
                }
                else if (rfi is DateFieldInfo)
                {
                    if (!(value is DateTime))
                        throw new MyException("Value must be datetime.");
                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (value != null)
                    {
                        decimal dcmValue = decimal.Parse(value.ToString());

                        if (dcmValue < nfi.MinValue)
                            throw new MyException("Value cannot be lesser than MinValue defined.");
                        if (dcmValue > nfi.MaxValue)
                            throw new MyException("Value cannot be greater than MaxValue defined.");

                    }
                }
            }
            #endregion

            //int componentCount = ri.RecordFields[rfi.FieldNumber].ComponentCount;
            if (rfi.Value == null && rfi.SupportsMultipleValues)
            {
                rfi.Value = new List<object>();
            }

            if (rfi.SupportsMultipleValues)
            {
                //((List<string>)rfi.Value)[positionNumber - 1] = strValue;
                ((List<object>)rfi.Value).Add(value);
            }
            else
            {
                rfi.Value = value;
            }
        }

        private void SetRecordValue(RecordInfo ri, RecordFieldInfo rfi, object value/*, int positionNumber = 0*/)
        {

            if (rfi == null || rfi.FieldNumber == -1)
                return;

            #region Validate and populate value accordingly

            if (rfi.IsMandatory && value == null)
            {
                throw new MyException("Value is mandatory");
            }
            else
            {
                if (rfi.ValidValues != null && rfi.ValidValues.Count > 0)
                {
                    //for String, set value from set of valid values for purpose of keeping character case of allowed values only
                    if (rfi is StringFieldInfo)
                    {
                        if (value != null && !String.IsNullOrEmpty(value.ToString()))
                            value = rfi.ValidValues.Select(r => r.ToString()).Where(r => String.Equals(r, value.ToString(), StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                    }
                    else
                    {
                        if (!rfi.ValidValues.Contains(value))
                            value = null;
                    }
                }

                if (rfi is StringFieldInfo)
                {
                    StringFieldInfo sfi = rfi as StringFieldInfo;
                    if (value != null)
                    {
                        string strValue = (string)value;
                        //replace non-printable characters - for Architect
                        strValue = InterfaceHelper.RemoveNonPrintableCharacters(strValue);

                        //replace non-ASCII characters, error occurred in XN9000 due to unicode character
                        strValue = InterfaceHelper.RemoveNonAsciiCharacters(strValue);

                        //replace characters having special meaning (WO ONLINE FORMAT error in AU due to & symbol)
                        if (rfi != ri.DelimiterCharacters)
                        {
                            char[] charactersToRemove = new char[] { '|', '\\', '^', '&' };
                            if (charactersToRemove.Any(r => strValue.IndexOf(r) > -1))
                            {
                                foreach (char chr in charactersToRemove)
                                    strValue = strValue.Replace(chr, '?');
                            }
                        }

                        if (sfi.MaxLength > 0 && (strValue).Length > sfi.MaxLength)
                        {
                            strValue = (strValue).Substring(0, sfi.MaxLength);
                        }

                        value = strValue;
                    }
                }
                else if (rfi is DateFieldInfo)
                {
                    if (!(value is DateTime))
                        throw new MyException("Value must be datetime.");

                }
                else if (rfi is NumericFieldInfo)
                {
                    NumericFieldInfo nfi = rfi as NumericFieldInfo;
                    if (value != null)
                    {
                        decimal dcmValue = decimal.Parse(value.ToString());

                        if (dcmValue < nfi.MinValue)
                            throw new MyException("Value cannot be lesser than MinValue defined.");
                        if (dcmValue > nfi.MaxValue)
                            throw new MyException("Value cannot be greater than MaxValue defined.");
                    }
                }
            }
            #endregion

            //int componentCount = ri.RecordFields[rfi.FieldNumber].ComponentCount;
            if (rfi.Value == null && rfi.SupportsMultipleValues)
            {
                rfi.Value = new List<object>();
            }

            if (rfi.SupportsMultipleValues)
            {
                //((List<string>)rfi.Value)[positionNumber - 1] = strValue;
                ((List<object>)rfi.Value).Add(value);
            }
            else
            {
                rfi.Value = value;
            }

        }

        #region Prepare records For ASTM and HL7 Both
        private RecordInfo AddNewRecord(RecordInfo riConfig, int sequenceNumber = -1)
        {
            RecordInfo riCopy = riConfig.Copy();
            responseRecords.Add(riCopy);
            if (riCopy.SequenceNumber.FieldNumber > -1)
            {
                if (sequenceNumber == -1)
                    throw new MyException("Sequence number must be provided.");

                riCopy.SequenceNumber.Value = sequenceNumber;
            }
            return riCopy;
        }


        private RecordInfo AddNewRecordHL7(RecordInfo riConfig/*, int sequenceNumber = -1*/)
        {
            RecordInfo riCopy = riConfig.Copy();
            responseRecords.Add(riCopy);
            //if (riCopy.SequenceNumber.FieldNumber > -1)
            //{
            //    if (sequenceNumber == -1)
            //        throw new MyException("Sequence number must be provided.");

            //    riCopy.SequenceNumber.Value = sequenceNumber;
            //}
            return riCopy;
        }

        private RecordInfo AddNewRecordHL7WithSeqnnumber(RecordInfo riConfig, int sequenceNumber = -1)
        {
            RecordInfo riCopy = riConfig.Copy();
            responseRecords.Add(riCopy);
            if (riCopy.SequenceNumber.FieldNumber > -1)
            {
                if (sequenceNumber == -1)
                    throw new MyException("Sequence number must be provided.");

                riCopy.SequenceNumber.Value = sequenceNumber;
            }
            return riCopy;
        }

        private void AddQueryRecord()
        {
            riQuery = AddNewRecord(msgConfig.QueryRecordInfo, 1);
        }

        private void AddHeaderRecord(string strHeader)
        {
            riHeader = AddNewRecord(msgConfig.HeaderRecordInfo);

            SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValue(strHeader, riHeader, riHeader.DelimiterCharacters));
            SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValue(strHeader, riHeader, riHeader.ReceiverID));
            SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValue(strHeader, riHeader, riHeader.SenderID));
            //SetRecordValue(riHeader, riHeader.MsgIdentifier, ExtractRecordValueForAUTOBarcode(strHeader, riHeader, riHeader.MsgIdentifier));
            SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValue(strHeader, riHeader, riHeader.VersionNumber));

        }

        private void AddPatientRecord(ref int sequenceNumber, BarcodeList bList)
        {
            riPatient = AddNewRecord(msgConfig.PatientRecordInfo, (++sequenceNumber));

            //if (msgConfig.AnalyzerTypeID == 30)
            //{
            //    bList.customername = bList.customername + "(" + bList.age + bList.AgeType + "/" + bList.gender + ")";
            //    bList.investigationid = bList.LabcodeWithDateAndSourceCode;
            //}
            //else if (msgConfig.AnalyzerTypeID == 50)
            //{
            //    bList.investigationid = bList.LabcodeWithDate;
            //}

            //else if (msgConfig.AnalyzerTypeID == AnalyzerTypes.UN2000 && !String.IsNullOrEmpty(bList.PatientId))
            //{
            //    bList.PatientId = string.Empty;
            //}

            SetRecordValue(riPatient, riPatient.PatientID, bList.investigationid);
            SetRecordValue(riPatient, riPatient.SampleID, bList.barcode); //for Advia 1800, Dynex
            SetRecordValue(riPatient, riPatient.PatientName, bList.customername);
            SetRecordValue(riPatient, riPatient.Gender, bList.gender);
            SetRecordValue(riPatient, riPatient.Age, Convert.ToInt32(bList.age));
            //SetRecordValue(riPatient, riPatient.DateOfBirth, bList.DateOfBirth);
            SetRecordValue(riPatient, riPatient.SCT, bList.collectiontime);
            SetRecordValue(riPatient, riPatient.BVT, bList.registertime);
            SetRecordValue(riPatient, riPatient.RefDr, bList.refDr);
            SetRecordValue(riPatient, riPatient.SCP, bList.SCP);
            SetRecordValue(riPatient, riPatient.ClientCode, bList.ClientCode);
            SetRecordValue(riPatient, riPatient.Labcode, bList.Labcode);
            SetRecordValue(riPatient, riPatient.LabcodeWithDate, bList.LabcodeWithDate);

        }

        private RecordInfo AddOrderRecord(ref int sequenceNumber, string barcode, string testID_ManufacturersTestCode)
        {
            RecordInfo ri = null;
            if (msgConfig.PatientAndOrderRecordsMerged)
                ri = riPatient;
            else
            {
                if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                    riOrder = AddNewRecord(msgConfig.OrderRecordInfo, (++sequenceNumber));

                ri = riOrder;
            }

            SetRecordValue(ri, ri.SampleID, barcode);
            if (!String.IsNullOrEmpty(strQuery))
            {
                SetRecordValue(ri, ri.RackNo, ExtractRecordValue(strQuery, riQuery_Incoming, riQuery_Incoming.RackNo));
                SetRecordValue(ri, ri.RackPosition, ExtractRecordValue(strQuery, riQuery_Incoming, riQuery_Incoming.RackPosition));
                SetRecordValue(ri, ri.SampleNumberAttribute, ExtractRecordValue(strQuery, riQuery_Incoming, riQuery_Incoming.SampleNumberAttribute));
            }
            SetRecordValue(ri, ri.TestID_ManufacturersTestCode, testID_ManufacturersTestCode);

            //if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Automax)
            //{
            //    if (msgConfig.DualMachineCodes.ContainsKey(testID_ManufacturersTestCode))
            //    {
            //        SetRecordValue(ri, ri.DualTestID_ManufacturersTestCode, msgConfig.DualMachineCodes[testID_ManufacturersTestCode]);
            //    }
            //}

            return ri;
        }

        private RecordInfo AddTerminationRecord(int barcodeCount, string terminationCode = null)
        {
            RecordInfo ri = AddNewRecord(msgConfig.TerminationRecordInfo, sequenceNumber: 1);
            SetRecordValue(ri, ri.TerminationCode, terminationCode);
            SetRecordValue(ri, ri.BarcodeCount, barcodeCount);
            SetRecordValue(ri, ri.RecordCount, responseRecords.Count);
            return ri;
        }

        #region HL7WorkList Preparetion

        private void HeaderRecordInfoHL7ACKResponse(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.HeaderRecordInfoHL7ACKResponse);

            SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValueHL7(strHeader, riHeader, riHeader.DelimiterCharacters));
            SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ReceiverID));
            SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.SenderID));
            SetRecordValue(riHeader, riHeader.MsgIdentifier, ExtractRecordValueForHL7ACK(strHeader, riHeader, riHeader.MsgIdentifier));
            SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValueHL7(strHeader, riHeader, riHeader.VersionNumber));

        }

        private void HeaderRecordInfoHL7ACKResponseOrder(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.HeaderRecordInfoHL7ACKResponseOrder);

            SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValueHL7(strHeader, riHeader, riHeader.DelimiterCharacters));
            SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ReceiverID));
            SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.SenderID));
            //SetRecordValue(riHeader, riHeader.MsgIdentifier, ExtractRecordValueForHL7ACK(strHeader, riHeader, riHeader.MsgIdentifier));
            SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValueHL7(strHeader, riHeader, riHeader.VersionNumber));

        }

        private void ResultRecordInfoHL7ACKResults(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.ResultRecordInfoHL7ACKResults);

            SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValueHL7(strHeader, riHeader, riHeader.DelimiterCharacters));
            SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ReceiverID));
            SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.SenderID));
            SetRecordValue(riHeader, riHeader.MsgIdentifier, ExtractRecordValueHL7OKResResult(strHeader, riHeader, riHeader.MsgIdentifier));
            SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValueHL7(strHeader, riHeader, riHeader.VersionNumber));

        }


        private void AddMSARecordHL7ACK(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7MSA);

            //SetRecordValue
        }

        private void AddMSARecordHL7ACKForResults(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7MSAResults);

            //SetRecordValue
        }

        private void AddErrorRecordHL7ACK(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.ErrorSegment);
            SetRecordValue(riHeader, riHeader.ActionCode, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ActionCode));
        }

        private void AddQueryAckRecordHL7ACK(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.QueryAcknowledgmentSegment);
            SetRecordValue(riHeader, riHeader.ActionCode, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ActionCode));
            SetRecordValue(riHeader, riHeader.CommentText, ExtractRecordValueHL7(strHeader, riHeader, riHeader.CommentText));
        }

        private void AddQueryDefinitionSegment(string strQuery)
        {
            riOrder = AddNewRecordHL7(msgConfig.QueryDefinitionSegmentFromLis);
            SetRecordValue(riOrder, riOrder.MsgControlid, ExtractRecordValueHL7(strQuery, riOrder, riOrder.MsgControlid));
        }

        private void AddQueryFilterSegment(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.QueryFilterSegment);
        }

        #region DisplayDataSegment
        private void AddDisplayDataSegment_1(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_1);
            SetRecordValue(riPatient, riPatient.PatientID, bList.investigationid);
        }
        private void AddDisplayDataSegment_2(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_2);
        }
        private void AddDisplayDataSegment_3(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_3);
            SetRecordValue(riPatient, riPatient.PatientName, bList.customername);
        }
        private void AddDisplayDataSegment_4(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_4);
            SetRecordValue(riPatient, riPatient.DateOfBirth, bList.DateOfBirth);
        }
        private void AddDisplayDataSegment_5(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_5);
            SetRecordValue(riPatient, riPatient.Gender, bList.gender);
        }
        private void AddDisplayDataSegment_6(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_6);
        }
        private void AddDisplayDataSegment_7(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_7);
        }
        private void AddDisplayDataSegment_8(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_8);
        }
        private void AddDisplayDataSegment_9(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_9);
        }
        private void AddDisplayDataSegment_10(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_10);
        }
        private void AddDisplayDataSegment_11(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_11);
        }
        private void AddDisplayDataSegment_12(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_12);
        }
        private void AddDisplayDataSegment_13(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_13);
        }
        private void AddDisplayDataSegment_14(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_14);
        }
        private void AddDisplayDataSegment_15(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_15);
        }
        private void AddDisplayDataSegment_16(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_16);
        }
        private void AddDisplayDataSegment_17(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_17);
        }
        private void AddDisplayDataSegment_18(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_18);
        }
        private void AddDisplayDataSegment_19(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_19);
        }
        private void AddDisplayDataSegment_20(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_20);
        }
        private void AddDisplayDataSegment_21(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_21);
            SetRecordValue(riPatient, riPatient.SampleID, bList.barcode);
        }
        private void AddDisplayDataSegment_22(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_22);
            SetRecordValue(riPatient, riPatient.SampleID, bList.barcode);
        }
        private void AddDisplayDataSegment_23(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_23);
            SetRecordValue(riPatient, riPatient.SCT, bList.collectiontime);
        }
        private void AddDisplayDataSegment_24(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_24);
        }
        private void AddDisplayDataSegment_25(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_25);
        }
        private void AddDisplayDataSegment_26(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_26);
            SetRecordValue(riPatient, riPatient.SampleType, bList.SampleType);
        }
        private void AddDisplayDataSegment_27(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_27);
        }
        private void AddDisplayDataSegment_28(BarcodeList bList)
        {
            riPatient = AddNewRecordHL7(msgConfig.DisplayDataSegment_28);
        }
        #endregion

        private void AddContinuationPointerSegment(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.ContinuationPointerSegment);
        }

        private void AddQAKRecordHL7ACK(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7QAK);

        }

        private void AddQPDRecordHL7ACK(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.QueryRecordInfoHL7);

        }

        private void QueryRecordInfoHL7Res(string strHeader)
        {
            riHeader = AddNewRecordHL7(msgConfig.QueryRecordInfoHL7Res);

        }

        private void AddHeaderRecordHL7(string strHeader)
        {
            string[] parts = strHeader.Split('|');
            string messageIdentifier = "";

            if (parts.Length >= 9 && parts[8] == "QRY^Q02")
            {
                parts[8] = "QCK^Q02";
                messageIdentifier = parts[8];
            }

            if (msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6)
            {
                riHeader = AddNewRecordHL7(msgConfig.HeaderRecordInfoHL7ACK);
                SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValueHL7(strHeader, riHeader, riHeader.DelimiterCharacters));
                SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ReceiverID));
                SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.SenderID));
                if (!string.IsNullOrEmpty(messageIdentifier))
                {
                    SetRecordValue(riHeader, riHeader.MsgIdentifier, messageIdentifier);
                }
                SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValueHL7(strHeader, riHeader, riHeader.VersionNumber));
                SetRecordValue(riHeader, riHeader.MsgControlid, ExtractRecordValueHL7(strHeader, riHeader, riHeader.MsgControlid));
            }

            else {
                riHeader = AddNewRecordHL7(msgConfig.HeaderRecordInfoHL7);

                SetRecordValue(riHeader, riHeader.DelimiterCharacters, ExtractRecordValueHL7(strHeader, riHeader, riHeader.DelimiterCharacters));
                SetRecordValue(riHeader, riHeader.SenderID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.ReceiverID));
                SetRecordValue(riHeader, riHeader.ReceiverID, ExtractRecordValueHL7(strHeader, riHeader, riHeader.SenderID));
                //SetRecordValue(riHeader, riHeader.MsgIdentifier, ExtractRecordValueForAUTOBarcode(strHeader, riHeader, riHeader.MsgIdentifier));
                SetRecordValue(riHeader, riHeader.VersionNumber, ExtractRecordValueHL7(strHeader, riHeader, riHeader.VersionNumber));
            }
        }

        private void AddPatientRecordHL7(BarcodeList bList)
        {
            //riPatient = AddNewRecordHL7(msgConfig.PatientRecordInfo, (++sequenceNumber));
            riPatient = AddNewRecordHL7(msgConfig.PatientRecordInfoHL7);

            SetRecordValue(riPatient, riPatient.PatientID, bList.investigationid);
            SetRecordValue(riPatient, riPatient.SampleID, bList.barcode); //for Advia 1800, Dynex
            SetRecordValue(riPatient, riPatient.PatientName, bList.customername);
            SetRecordValue(riPatient, riPatient.Gender, bList.gender);
            SetRecordValue(riPatient, riPatient.Age, Convert.ToInt32(bList.age));
            SetRecordValue(riPatient, riPatient.DateOfBirth, DateTime.Now);
            SetRecordValue(riPatient, riPatient.SCT, bList.collectiontime);
            SetRecordValue(riPatient, riPatient.BVT, bList.registertime);
            SetRecordValue(riPatient, riPatient.RefDr, bList.refDr);
            SetRecordValue(riPatient, riPatient.SCP, bList.SCP);
            SetRecordValue(riPatient, riPatient.ClientCode, bList.ClientCode);
            SetRecordValue(riPatient, riPatient.Labcode, bList.Labcode);
            SetRecordValue(riPatient, riPatient.LabcodeWithDate, bList.LabcodeWithDate);

        }

        private void AddPatientVisitRecordHl7()
        {
            riPatient = AddNewRecordHL7(msgConfig.PatientVisitInfoHL7);
        }

        private void AddQueryRecordHL7()
        {
            riQuery = AddNewRecordHL7(msgConfig.QueryRecordInfo);
        }

        private RecordInfo AddOrderRecordHL7SPM(BarcodeList bList, int sequenceNumber /*string barcode*//*, string testID_ManufacturersTestCode*/)
        {
            RecordInfo ri = null;

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7WithSeqnnumber(msgConfig.OrderRecordInfoHL7SPM, (++sequenceNumber));
            ri = riOrder;

            SetRecordValue(ri, ri.SampleID, bList.barcode);
            return ri;
        }


        private RecordInfo AddOrderRecordHL7SAC(BarcodeList bList/*ref int sequenceNumber,*//* string barcode, string testID_ManufacturersTestCode*/)
        {
            RecordInfo ri = null;

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7SAC);
            ri = riOrder;


            SetRecordValue(ri, ri.SampleID, bList.barcode);

            return ri;
        }

        private RecordInfo AddOrderRecordHL7ORC(BarcodeList bList/*ref int sequenceNumber, string barcode, string testID_ManufacturersTestCode*/)
        {
            RecordInfo ri = null;

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7ORC);
            ri = riOrder;

            SetRecordValue(riOrder, riOrder.SCT, DateTime.Today);

            return ri;
        }

        private RecordInfo AddOrderRecordHL7TQ1(BarcodeList bList/*ref int sequenceNumber, string barcode, string testID_ManufacturersTestCode*/)
        {
            RecordInfo ri = null;

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7TQ1/*, (++sequenceNumber)*/);
            ri = riOrder;


            SetRecordValue(ri, ri.SampleID, bList.barcode);

            return ri;
        }

        private RecordInfo AddOrderRecordHL7OBR(ref int sequenceNumber, string barcode, string testID_ManufacturersTestCode, ref int sequenceNumber_EXZ600)
        {
            RecordInfo ri = null;

            if (msgConfig.AnalyzerTypeID == AnalyzerTypes.MISPA_CX4)
            {
                riOrder = AddOrderRecordHL7DSP3_MISPA_CX4(barcode, testID_ManufacturersTestCode);
            }

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes && msgConfig.AnalyzerTypeID != AnalyzerTypes.MISPA_CX4)
                riOrder = AddNewRecordHL7WithSeqnnumber(msgConfig.OrderRecordInfoHL7OBR, (++sequenceNumber));

            if(msgConfig.AnalyzerTypeID == AnalyzerTypes.Zybio_EXZ_6000_H6 && msgConfig.AnalyzerTypeID != AnalyzerTypes.MISPA_CX4)
                riOrder = AddNewRecordHL7WithSeqnnumber(msgConfig.DisplayDataSegment_TestCode, (++sequenceNumber_EXZ600));

            ri = riOrder;

            if (testID_ManufacturersTestCode == null)
            {
                
            }


            SetRecordValue(ri, ri.SampleID, barcode);
            SetRecordValue(ri, ri.TestID_ManufacturersTestCode, testID_ManufacturersTestCode);

            return ri;
        }

        private RecordInfo AddOrderRecordHL7TCD(string barcode, string testID_ManufacturersTestCode)
        {
            RecordInfo ri = null;

            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7(msgConfig.OrderRecordInfoHL7TCD);
            ri = riOrder;


            SetRecordValue(ri, ri.SampleID, barcode);
            SetRecordValue(ri, ri.TestID_ManufacturersTestCode, testID_ManufacturersTestCode);

            return ri;
        }

        private RecordInfo AddOrderRecordHL7DSP3_MISPA_CX4(string barcode, string testID_ManufacturersTestCode)
        {
            RecordInfo ri = null;
            if (riOrder == null || !msgConfig.SupportsMultipleTestcodes)
                riOrder = AddNewRecordHL7(msgConfig.DisplayDataSegment_TestCode);

            ri = riOrder;

            SetRecordValue(ri, ri.SampleID, barcode);
            SetRecordValue(ri, ri.TestID_ManufacturersTestCode, testID_ManufacturersTestCode);
            return ri;
        }

        #endregion

        private bool IsRecordTypeMatch(string strRecordReceived, RecordInfo ri)
        {
            if (String.IsNullOrEmpty(strRecordReceived) || ri is null)
                return false;

            if (strRecordReceived.StartsWith((string)ri.RecordType.DefaultValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CollectCopyValues(string strRecordReceived, RecordInfo ri)
        {
            var allRecordFields = ri.RecordFields.Union(ri.RecordFields.Where(r => r.ComponentCount > 0).SelectMany(r => r.Components));
            foreach (StringFieldInfo sfi in allRecordFields.Where(r => r is StringFieldInfo && !String.IsNullOrEmpty(((StringFieldInfo)r).CopyKey_Incoming)))
            {
                try
                {
                    //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValue(strRecordReceived, ri, sfi));
                    //if (!CopyValues.ContainsKey(sfi.CopyKey_Incoming))
                    //{
                        //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValue(strRecordReceived, ri, sfi));
                        CopyValues[sfi.CopyKey_Incoming] = (string)ExtractRecordValue(strRecordReceived, ri, sfi);

                    //}
                }
                catch (Exception ex){ 
                    //nothing
                }
            }
        }

        #region Copy values logic from frames which are coming from machine
        private void CollectCopyValuesHL7(string strRecordReceived, RecordInfo ri)
        {
            var allRecordFields = ri.RecordFields.Union(ri.RecordFields.Where(r => r.ComponentCount > 0).SelectMany(r => r.Components));
            foreach (StringFieldInfo sfi in allRecordFields.Where(r => r is StringFieldInfo && !String.IsNullOrEmpty(((StringFieldInfo)r).CopyKey_Incoming)))
            {
                try
                {
                    //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValueHL7(strRecordReceived, ri, sfi));
                    //if (!CopyValues.ContainsKey(sfi.CopyKey_Incoming))
                    //{
                        //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValueHL7(strRecordReceived, ri, sfi));
                    CopyValues[sfi.CopyKey_Incoming] = (string)ExtractRecordValueHL7(strRecordReceived, ri, sfi);

                    //}
                }

                catch (Exception ex)
                {
                    
                }
                
            }
        }

        private void CollectCopyValuesHL7OKRes(string strRecordReceived, RecordInfo ri)
        {
            var allRecordFields = ri.RecordFields.Union(ri.RecordFields.Where(r => r.ComponentCount > 0).SelectMany(r => r.Components));
            foreach (StringFieldInfo sfi in allRecordFields.Where(r => r is StringFieldInfo && !String.IsNullOrEmpty(((StringFieldInfo)r).CopyKey_Incoming)))
            {
                try
                {
                    //if (!CopyValues.ContainsKey(sfi.CopyKey_Incoming))
                    //{
                        //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValueHL7OKRes(strRecordReceived, ri, sfi));
                    CopyValues[sfi.CopyKey_Incoming] = (string)ExtractRecordValueHL7OKRes(strRecordReceived, ri, sfi);


                    //}

                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void CollectCopyValuesHL7OKResResults(string strRecordReceived, RecordInfo ri)
        {
            var allRecordFields = ri.RecordFields.Union(ri.RecordFields.Where(r => r.ComponentCount > 0).SelectMany(r => r.Components));
            foreach (StringFieldInfo sfi in allRecordFields.Where(r => r is StringFieldInfo && !String.IsNullOrEmpty(((StringFieldInfo)r).CopyKey_Incoming)))
            {
                try
                {
                    //if (!CopyValues.ContainsKey(sfi.CopyKey_Incoming))
                    //{
                        //CopyValues.Add(sfi.CopyKey_Incoming, (string)ExtractRecordValueHL7OKResResult(strRecordReceived, ri, sfi));
                    CopyValues[sfi.CopyKey_Incoming] = (string)ExtractRecordValueHL7OKResResult(strRecordReceived, ri, sfi);

                    //}

                }
                catch (Exception ex) { }
            }
        }
        #endregion
        public TestListItem GetTestMappingOrEmpty(string machineCode)
        {
            TestListItem tli = null;
            tli = CachedData.TestList.Where(r => r.instrumentcode.Equals(machineCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            if (tli == null)
            {
                tli = new TestListItem();
            }

            return tli;
        }

        public IEnumerable<QualitativeResult> GetDescriptiveMapping(string TestCode)
        {
            return CachedData.DescriptiveResultMaster.testlist.Where(r => r.testcode.Equals(TestCode, StringComparison.InvariantCultureIgnoreCase))./*Single*/FirstOrDefault()?.textresults ?? new List<QualitativeResult> { };
        }

        public int PopulateDescriptiveId(string TestCode, string TestValue)
        {
            //returns -1 if descriptive mapping does not exist for the test
            int DescriptiveId = -1;
            var product_DescriptiveList = GetDescriptiveMapping(TestCode);
            if (product_DescriptiveList.Any())
            {
                DescriptiveId = 0;
                var query = product_DescriptiveList.Where(r => r.result.Equals(TestValue, StringComparison.InvariantCultureIgnoreCase)
                        || (InterfaceHelper.IsNumeric(TestValue) && InterfaceHelper.IsNumeric(r.result) && decimal.Parse(TestValue) == decimal.Parse(r.result)));
                if (query.Any())
                {
                    DescriptiveId = query.FirstOrDefault().resultid;
                }
            }
            return DescriptiveId;
        }

        public int PopulateDescriptiveIdByMachineCode(string MachineCode, string TestValue)
        {
            TestListItem tli = GetTestMappingOrEmpty(MachineCode);
            return PopulateDescriptiveId(tli.testcode, TestValue);
        }
        #endregion
    }
}
