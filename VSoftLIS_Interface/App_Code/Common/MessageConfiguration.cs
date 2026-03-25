using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace VSoftLIS_Interface.Common
{
    internal class MessageConfiguration
    {
        internal InstrumentTypes InstrumentType = InstrumentTypes.Analyzer;
        internal int AnalyzerID = 0;
        internal int AnalyzerTypeID = 0;
        internal RecordInfo HeaderRecordInfo = null;
        internal RecordInfo QueryRecordInfo = null;
        internal RecordInfo PatientRecordInfo = null;
        internal RecordInfo OrderRecordInfo = null;
        // Classes for HL7
        internal RecordInfo HeaderRecordInfoHL7 = null;
        internal RecordInfo QueryRecordInfoHL7 = null;
        internal RecordInfo PatientRecordInfoHL7 = null;
        internal RecordInfo PatientVisitInfoHL7 = null;
        internal RecordInfo OrderRecordInfoHL7SPM = null;
        internal RecordInfo OrderRecordInfoHL7SAC = null;
        internal RecordInfo OrderRecordInfoHL7ORC = null;
        internal RecordInfo OrderRecordInfoHL7TQ1 = null;
        internal RecordInfo OrderRecordInfoHL7OBR = null;
        internal RecordInfo OrderRecordInfoHL7TCD = null;
        internal RecordInfo ResultRecordInfoHL7 = null;
        internal RecordInfo ResultRecordInfoHL7ACK = null;
        internal RecordInfo ResultRecordInfoHL7ACKResults = null;
        internal RecordInfo OrderRecordInfoHL7MSA = null;
        internal RecordInfo OrderRecordInfoHL7MSAResults = null;
        internal RecordInfo OrderRecordInfoHL7QAK = null;
        internal RecordInfo HeaderRecordInfoHL7ACK = null;
        internal RecordInfo HeaderRecordInfoHL7ACKResponse = null;
        internal RecordInfo HeaderRecordInfoHL7ACKResponseOrder = null;
        internal RecordInfo HeaderRecordInfoHL7ACKMSATEST = null;
        internal RecordInfo HeaderRecordInfoHL7ACKMSAReqToResults = null;
        internal RecordInfo QueryRecordInfoHL7Res = null;
        internal RecordInfo ResponseControlInfoHL7 = null;
        internal RecordInfo ResultRecordInfo = null;
        internal RecordInfo QcResultRecordInfo = null;
        internal RecordInfo CommentRecordInfo = null;
        internal RecordInfo TerminationRecordInfo = null;

        internal RecordInfo QueryDefinitionSegmentFromMachine = null;
        internal RecordInfo QueryDefinitionSegmentFromLis = null;
        internal RecordInfo QueryFilterSegment = null;
        internal RecordInfo ErrorSegment = null;
        internal RecordInfo QueryAcknowledgmentSegment = null;

        #region DisplayDataSegment
        internal RecordInfo DisplayDataSegment_1 = null;
        internal RecordInfo DisplayDataSegment_2 = null;
        internal RecordInfo DisplayDataSegment_3 = null;
        internal RecordInfo DisplayDataSegment_4 = null;
        internal RecordInfo DisplayDataSegment_5 = null;
        internal RecordInfo DisplayDataSegment_6 = null;
        internal RecordInfo DisplayDataSegment_7 = null;
        internal RecordInfo DisplayDataSegment_8 = null;
        internal RecordInfo DisplayDataSegment_9 = null;
        internal RecordInfo DisplayDataSegment_10 = null;
        internal RecordInfo DisplayDataSegment_11 = null;
        internal RecordInfo DisplayDataSegment_12 = null;
        internal RecordInfo DisplayDataSegment_13 = null;
        internal RecordInfo DisplayDataSegment_14 = null;
        internal RecordInfo DisplayDataSegment_15 = null;
        internal RecordInfo DisplayDataSegment_16 = null;
        internal RecordInfo DisplayDataSegment_17 = null;
        internal RecordInfo DisplayDataSegment_18 = null;
        internal RecordInfo DisplayDataSegment_19 = null;
        internal RecordInfo DisplayDataSegment_20 = null;
        internal RecordInfo DisplayDataSegment_21 = null;
        internal RecordInfo DisplayDataSegment_22 = null;
        internal RecordInfo DisplayDataSegment_23 = null;
        internal RecordInfo DisplayDataSegment_24 = null;
        internal RecordInfo DisplayDataSegment_25 = null;
        internal RecordInfo DisplayDataSegment_26 = null;
        internal RecordInfo DisplayDataSegment_27 = null;
        internal RecordInfo DisplayDataSegment_28 = null;
        internal RecordInfo DisplayDataSegment_TestCode = null;
        #endregion
        internal RecordInfo ContinuationPointerSegment = null;

        internal bool IsFieldSizeInBytes = false;
        internal char? FieldDelimiter = null;
        internal char? ComponentDelimiter = null;
        internal char? ComponentDelimiterHL7 = null;
        internal char? RepeatDelimiter = null;
        //internal bool EnsureFullRecordInFrame = false;
        internal bool HeaderAndTerminationRecordRequired = false;
        internal bool PatientAndOrderRecordsMerged = false;
        internal bool SendPatientOrderRecordWhenNoOrder = false;
        internal bool SendNegativeQueryRecordWhenNoOrder = false;
        internal string ActionCode_Repeat = "A";
        internal string ActionCode_AddTestsInExistingBarcode = "A";
        internal string ActionCode_Cancellation = "C";
        internal string TerminationCodeWhenNoOrder = "";
        internal bool SupportsMultipleTestcodes = false;
        internal bool SupportsMultipleValuesInSingleRecord = false;
        public bool SupportsOnlyQuery = false;
        public bool SupportsMultipleBarcodes = false;
        public bool RemoveUnmappedTestcodes = true;
        public string[] ReplacementWOTests = null;
        public Dictionary<string, string> ReplacementWOTestsMapping = new Dictionary<string, string>();
        public bool IsBulkWorklistSender = false;
        public string ReportTypeFlagForNoWorkOrder = "";
        public int WorkOrderTimeoutMilliseconds = 4000;
        public bool IsSeparateFrameSessionForEachBarcode = false;
        public bool RemoveTrailingEmptyDelimiters = true;
        public Dictionary<string, string> DualMachineCodes = new Dictionary<string, string>();

        //for file based interface
        public bool ShowPatientDemographics = false;
        public bool UserSelectionToUploadResults = false;
        //public bool IsFileBased = false;
        //public bool IsFolderPickup = true;
        public bool IsFileCommunication = false;
        public bool IsManualTestSelection = false;

        public bool IsHL7 = false;

        // Generate a new GUID
        Guid uniqueGuid = Guid.NewGuid();
        string formattedDateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time")).ToString("yyyyMMddHHmmsszzzz").Replace(":", "");


        public MessageConfiguration(int analyzerID)
        {
            AnalyzerID = analyzerID;
            AnalyzerTypeID = CachedData.Analyzer.instrumentgroupid;

            #region Initialize analyzer configurations
            switch (AnalyzerTypeID)
            {

                case 1: //Architect- 02(T2000)
                    #region Architect
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = true;
                    PatientAndOrderRecordsMerged = false;
                    SupportsMultipleTestcodes = false;
                    SendNegativeQueryRecordWhenNoOrder = true;

                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    //HeaderRecordInfo.SenderID = new StringFieldInfo(5, 10);
                    //HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(10, 10);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(12) { DefaultValue = "P" });
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 7) { DefaultValue = "1" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);

                    QueryRecordInfo = new QueryRecordInfo(13);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SequenceNumber);
                    //QueryRecordInfo.SequenceNumber.FieldNumber = 0;
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    QueryRecordInfo.SampleID = new StringFieldInfo(3, 20) { ComponentNumber = 2 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4 });
                    QueryRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 3) { ComponentNumber = 4, DefaultValue = "ALL" }; //Universal Test ID - for Negative Query Response
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.TestID_ManufacturersTestCode);
                    QueryRecordInfo.AddRecordField(new StringFieldInfo(13, 1) { DefaultValue = "X" }); //Status Code - for Negative Query Response

                    PatientRecordInfo = new PatientRecordInfo(35);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.SampleID = new StringFieldInfo(3, 20);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SampleID);
                    PatientRecordInfo.PatientID = new StringFieldInfo(4, 20);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 3 });
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 20);// { ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);
                    PatientRecordInfo.DateOfBirth = new DateFieldInfo(8, "yyyyMMdd") /*{ DefaultValue = new DateTime(1950, 1, 1) }*/;
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);
                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);
                    //PatientRecordInfo.SCP = new StringFieldInfo(11, 0);
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.SCP);

                    OrderRecordInfo = new OrderRecordInfo(31);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 20);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    //OrderRecordInfo.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 3 });
                    //OrderRecordInfo.PatientID = new StringFieldInfo(4, 20) { ComponentNumber = 1,DefaultValue="DUMMYSPECIMENID" };
                    //OrderRecordInfo.AddRecordField(OrderRecordInfo.PatientID);//Instrument specimenID
                    //OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 4) { ComponentNumber = 2 });//Carrier/Carousel ID
                    //OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 2) { ComponentNumber = 3 });//Carrier/Carousel ID
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 5, SupportsMultipleValues = true });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 8) { ComponentNumber = 4, SupportsMultipleValues = true };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);
                    //OrderRecordInfo.AddRecordField(new RecordFieldInfo(6) { DefaultValue = "R" }); //Priority
                    OrderRecordInfo.SCT = new DateFieldInfo(8, "yyyyMMddHHmmss") /*{ DefaultValue = new DateTime(1970,01,01,00,00) }*/;
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SCT);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(12) { DefaultValue = "N" }); //Action Code
                    OrderRecordInfo.RefDr = new StringFieldInfo(17, 0);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RefDr);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(26) { DefaultValue = "Q" }); //Report Type

                    ResultRecordInfo = new ResultRecordInfo(14);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 11 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 8) { ComponentNumber = 4 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.ResultAspects = new StringFieldInfo(3, 8) { ComponentNumber = 11 }; //Result type //changed for 8 to to 11 as per actual communication
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAspects);
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 3) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.Units = new StringFieldInfo(5, 6);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(7, 15) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(13) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);

                    CommentRecordInfo = new CommentRecordInfo(5);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.RecordType);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.SequenceNumber);
                    CommentRecordInfo.CommentText = new StringFieldInfo(4, 120);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.CommentText);

                    TerminationRecordInfo = new TerminationRecordInfo(2);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);

                    #endregion
                    break;

                case 2://BD FACSCalibur
                    #region BD FACSCalibur
                    #endregion
                    break;

                case 3://Bechman AU700 And AU480
                    #region Bechman AU700 And AU480
                    IsFieldSizeInBytes = true;
                    FieldDelimiter = null;
                    ComponentDelimiter = null;
                    RepeatDelimiter = null;
                    //FieldDelimiter = '|';
                    //ComponentDelimiter = '^';
                    //RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = false;
                    PatientAndOrderRecordsMerged = true;
                    SendPatientOrderRecordWhenNoOrder = true;
                    SupportsMultipleTestcodes = true;
                    SupportsMultipleBarcodes = false;
                    //ReplacementWOTests = new string[] { "002"};

                    QueryRecordInfo = new QueryRecordInfo(6);
                    QueryRecordInfo.RecordType.DefaultValue = "R ";
                    QueryRecordInfo.RecordType.BytesLength = 2;
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.AddRecordField(new StringFieldInfo(2, 4) { BytesLength = 6, CopyKey_Incoming = "Rack No" }); //Machine ID (2) + Rack no.(4)
                    //QueryRecordInfo.AddRecordField(new StringFieldInfo(3, 2) { BytesLength = 2, CopyKey_Incoming = "Cup Position" }); //Cup Position
                    QueryRecordInfo.AddRecordField(new StringFieldInfo(4, 1) { BytesLength = 1, CopyKey_Incoming = "Sample Type" }); //Sample Type
                    QueryRecordInfo.AddRecordField(new StringFieldInfo(5, 4) { BytesLength = 4, CopyKey_Incoming = "Sample no" }); //Sample no.
                    QueryRecordInfo.SampleID = new StringFieldInfo(6, 9) { BytesLength = 10 }; //Sample ID
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);

                    PatientRecordInfo = new PatientRecordInfo(16);
                    PatientRecordInfo.RecordType.DefaultValue = "S ";
                    PatientRecordInfo.RecordType.BytesLength = 2;
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(2, 5) { BytesLength = 5, CopyKey_Outgoing = "Rack No"/*, DefaultValue = "123"*/ }); //Machine ID (2) + Rack no.(4)
                    //PatientRecordInfo.AddRecordField(new StringFieldInfo(3, 2) { BytesLength = 2, CopyKey_Outgoing = "Cup Position" }); //Cup Position
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(4, 1) { BytesLength = 1, CopyKey_Outgoing = "Sample Type" }); //Sample Type
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(5, 4) { BytesLength = 4, CopyKey_Outgoing = "Sample no" }); //Sample no.
                    PatientRecordInfo.SampleID = new StringFieldInfo(6, 10) { BytesLength = 10, IsRightJustified = true }; //Sample ID
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SampleID);
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(7) { BytesLength = 4 }); //Dummy
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(8) { BytesLength = 1, DefaultValue = "E" }); //Block Identification No.
                    //PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { BytesLength = 1, DefaultValue = "0", ValidValues = new List<object> { "M", "F" } }; //Gender
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);
                    //PatientRecordInfo.Age = new NumericFieldInfo(10, 0) { BytesLength = 3 }; //Age
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.Age);
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(11) { BytesLength = 2 }); //Month
                    //PatientRecordInfo.PatientName = new StringFieldInfo(12, 15) { BytesLength = 0 }; //Patient Information 1
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);
                    //PatientRecordInfo.PatientID = new StringFieldInfo(13, 15) { BytesLength = 0 }; //Patient Information 2
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(14) { BytesLength = 0 }); //Patient Information 3
                    //PatientRecordInfo.RefDr = new StringFieldInfo(15, 15) { BytesLength = 0 }; //Patient Information 4
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.RefDr);
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentCount = 2, BytesLength = 0, BytesLength_PerComponentSet = 4, SupportsMultipleValues = true });//Request test
                    PatientRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(16, 0) { ComponentNumber = 1, BytesLength = 3, SupportsMultipleValues = true, IsRightJustified = true, EmptySpaceFillerChar = '0' }; //Test code
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.TestID_ManufacturersTestCode);
                    //PatientRecordInfo.AddRecordField(new StringFieldInfo(16, 1) { ComponentNumber = 2, BytesLength = 1, DefaultValue = "0", SupportsMultipleValues = true }); //Dilution Info

                    ResultRecordInfo = new ResultRecordInfo(20);
                    ResultRecordInfo.RecordType.DefaultValue = "D ";
                    ResultRecordInfo.RecordType.BytesLength = 2;
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(2) { BytesLength = 6 }); //Machine ID (2) + Rack no.(4)
                    //ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { BytesLength = 2 }); //Cup Position
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(4) { BytesLength = 1 }); //Sample Type
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(5) { BytesLength = 4 }); //Sample no.
                    ResultRecordInfo.SampleID = new StringFieldInfo(6, 9) { BytesLength = 10 }; //Sample ID
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SampleID);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(7) { BytesLength = 4 }); //Dummy
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(8) { BytesLength = 1 }); //Block Identification No.
                    //ResultRecordInfo.Gender = new StringFieldInfo(9, 1) { BytesLength = 1 }; //Gender
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.Gender);
                    //ResultRecordInfo.Age = new NumericFieldInfo(10, 0) { BytesLength = 3 }; //Age
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.Age);
                    //ResultRecordInfo.AddRecordField(new RecordFieldInfo(11) { BytesLength = 2 }); //Month
                    //ResultRecordInfo.PatientName = new StringFieldInfo(12, 20) { BytesLength = 0 }; //Patient Information 1
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.PatientName);
                    //ResultRecordInfo.PatientID = new StringFieldInfo(13, 20) { BytesLength = 0 }; //Patient Information 2
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.PatientID);
                    //ResultRecordInfo.AddRecordField(new RecordFieldInfo(14) { BytesLength = 0 }); //Patient Information 3
                    //ResultRecordInfo.RefDr = new StringFieldInfo(15, 20) { BytesLength = 0 }; //Patient Information 4
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.RefDr);
                    ResultRecordInfo.RequestTest = new RecordFieldInfo(16) { ComponentCount = 12, BytesLength = 0, BytesLength_PerComponentSet = 11 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RequestTest);
                    //ANL unit No. and Cuvette side are not being received in current machine result communication
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(16, 3) { ComponentNumber = 1, BytesLength = 3, SupportsMultipleValues = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 2, BytesLength = 0, SupportsMultipleValues = true }); //R1 Lot No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 3, BytesLength = 0, SupportsMultipleValues = true }); //R1 Bottle No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 4, BytesLength = 0, SupportsMultipleValues = true }); //R2 Lot No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 5, BytesLength = 0, SupportsMultipleValues = true }); //R2 Bottle No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 6, BytesLength = 0, SupportsMultipleValues = true }); //R1-2 Lot No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 7, BytesLength = 0, SupportsMultipleValues = true }); //R1-2 Bottle No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 8, BytesLength = 0, SupportsMultipleValues = true }); //R2-2 Lot No.
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 9, BytesLength = 0, SupportsMultipleValues = true }); //R2-2 Bottle No.
                    ResultRecordInfo.TestValue = new NumericFieldInfo(16, 3) { ComponentNumber = 10, BytesLength = 6, SupportsMultipleValues = true }; //Analysis Result
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    //ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(16, 2) { ComponentNumber = 11, BytesLength = 2, SupportsMultipleValues = true, TrimTextWhileExtracting = false };
                    //ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    //Not used ResultAbnormalFlags, to skip prefix values as all are unwanted
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 11, BytesLength = 2, SupportsMultipleValues = true }); //
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(16) { ComponentNumber = 12, BytesLength = 0, SupportsMultipleValues = true }); //ISE Electrode Lot No.

                    QcResultRecordInfo = ResultRecordInfo.Copy();
                    QcResultRecordInfo.RecordType.DefaultValue = "DQ";
                    QcResultRecordInfo.RecordFields[8].BytesLength = 0;
                    QcResultRecordInfo.RecordFields[9].BytesLength = 0;
                    QcResultRecordInfo.RecordFields[10].BytesLength = 0;
                    #endregion
                    break;

                case 4: // CFX
                    #region CFX
                    #endregion
                    break;

                case 5://LABUMAT & URISED
                    #region LABUMAT & URISED
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = true;
                    SupportsMultipleTestcodes = true;

                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"\^&" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(3, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(4, 1));
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 5 });
                    HeaderRecordInfo.SenderID = (new StringFieldInfo(5, 128) { DefaultValue = "UriSed 3 PRO", ComponentNumber = 1 });
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.SenderID = (new StringFieldInfo(5, 128) { DefaultValue = "UriSed 3 PRO", ComponentNumber = 2 });
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(5, 20) { DefaultValue = "4.1.65.7325", ComponentNumber = 3 }); ;
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) {/* DefaultValue = "4.1.65.7325",*/ ComponentNumber = 4 }); ;
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(5, 10) { /*DefaultValue = "0" */ ComponentNumber = 5 });
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(6, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(7, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(8, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(9, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(10, 1));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(11, 1));
                    HeaderRecordInfo.ProcessingID = (new StringFieldInfo(12, 1) { DefaultValue = "P" });
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ProcessingID); // QA（sample query response）
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 7) { DefaultValue = "LIS2-A2" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);
                    HeaderRecordInfo.AddRecordField(new DateFieldInfo(14) { IsCurrentDateTime = true });

                    PatientRecordInfo = new PatientRecordInfo(6);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(3, 1));
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(4, 1));
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(5, 1));
                    PatientRecordInfo.SampleID = new StringFieldInfo(6, 8);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SampleID);

                    OrderRecordInfo = new OrderRecordInfo(26);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 32) { ComponentNumber = 1 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { DefaultValue = "G", ComponentNumber = 2 });
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 4 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 3) { ComponentNumber = 1 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 3) { ComponentNumber = 2 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 15) { ComponentNumber = 3 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(4, 6) { DefaultValue = "SAMPLE", ComponentNumber = 4 });
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 6 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { DefaultValue = "S", ComponentNumber = 1 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { ComponentNumber = 2 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { ComponentNumber = 3 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 7) { DefaultValue = "Cuvette", ComponentNumber = 4 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { ComponentNumber = 5 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { ComponentNumber = 6 });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(6, 1) { DefaultValue = "R" });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(7, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(8, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(9, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(10, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(11, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(12, 1) { DefaultValue = "N" });
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(13, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(14, 1));
                    OrderRecordInfo.SCT = new DateFieldInfo(15);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SCT);
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(16, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(17, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(18, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(19, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(20, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(21, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(22, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(23, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(24, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(25, 1));
                    OrderRecordInfo.AddRecordField(new StringFieldInfo(26, 1) { DefaultValue = "F" });


                    ResultRecordInfo = new ResultRecordInfo(14);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4 });
                    ResultRecordInfo.PatientID = new StringFieldInfo(3, 20) { ComponentNumber = 1 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.PatientID);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { ComponentNumber = 2 });
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { ComponentNumber = 3 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 6) { ComponentNumber = 4, SupportsMultipleValues = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.TestValue = new StringFieldInfo(4, 9) { SupportsMultipleValues = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.Units = new StringFieldInfo(5, 7) { SupportsMultipleValues = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(6, 1));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(7, 1) { DefaultValue = "N" });
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(8, 1));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(9, 1) { DefaultValue = "F" });
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(10, 1));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(11, 20));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(12, 1));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(13, 12));
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(14, 12) { DefaultValue = "UriSed 3 PRO" });

                    TerminationRecordInfo = new TerminationRecordInfo(3);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "N" };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);

                    #endregion
                    break;

                case 6: //QuantStudio
                    ShowPatientDemographics = true;
                    UserSelectionToUploadResults = true;
                    IsManualTestSelection = true;
                    RemoveUnmappedTestcodes = false;
                    break;


                case 8: //TOSOH G8
                    #region TOSOH
                    //IsFileBased = String.IsNullOrEmpty(ConfigurationManager.ConnectionStrings["OledbConnection_MDB"]?.ToString().Trim()) ? false : true;
                    //IsFolderPickup = false;
                    ShowPatientDemographics = false;
                    UserSelectionToUploadResults = true;

                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = true;
                    PatientAndOrderRecordsMerged = false;
                    SupportsMultipleTestcodes = false;

                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.SenderID = new StringFieldInfo(5, 10);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(10, 10);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(12) { DefaultValue = "P" });
                    HeaderRecordInfo.AddRecordField(new DateFieldInfo(14) { IsCurrentDateTime = true });

                    PatientRecordInfo = new PatientRecordInfo(34);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.PatientID = new StringFieldInfo(3, 13);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 40);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);
                    PatientRecordInfo.DateOfBirth = new DateFieldInfo(8, "yyyyMMdd") { DefaultValue = new DateTime(1900, 1, 1) };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);
                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);

                    OrderRecordInfo = new OrderRecordInfo(30);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 16);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4, SupportsMultipleValues = true });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 8) { ComponentNumber = 4, SupportsMultipleValues = true };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);
                    //OrderRecordInfo.ActionCode = new StringFieldInfo(12, 1) { ValidValues = new List<object> { "A", "C" } };
                    //OrderRecordInfo.AddRecordField(OrderRecordInfo.ActionCode);
                    OrderRecordInfo.ActionCode = new StringFieldInfo(16, 7) { DefaultValue = "Sp.1" };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.ActionCode);
                    //OrderRecordInfo.ReportType = new StringFieldInfo(26, 1) { DefaultValue = "O" };
                    //OrderRecordInfo.AddRecordField(OrderRecordInfo.ReportType);

                    ResultRecordInfo = new ResultRecordInfo(13);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4, SupportsMultipleValues = true });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 8) { ComponentNumber = 4, SupportsMultipleValues = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 2) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.Units = new StringFieldInfo(5, 6);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(7, 2) { };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(13) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);

                    TerminationRecordInfo = new TerminationRecordInfo(3);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "N" };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);

                    #endregion
                    break;

                case 9: //Sysmex XN 1000
                    #region Sysmex XN 1000
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    //EnsureFullRecordInFrame = true;
                    //MaxCharacterDataCount = 240;
                    HeaderAndTerminationRecordRequired = true;
                    PatientAndOrderRecordsMerged = false;
                    SendPatientOrderRecordWhenNoOrder = true;
                    SupportsMultipleTestcodes = true;
                    ReportTypeFlagForNoWorkOrder = "X";
                    ActionCode_Repeat = "A";
                    RemoveUnmappedTestcodes = true;
                    WorkOrderTimeoutMilliseconds = 10000;

                    HeaderRecordInfo = new HeaderRecordInfo(13);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0; //remove default added field

                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);

                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 8); //ASTM Version Number
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);

                    QueryRecordInfo = new QueryRecordInfo(13);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SequenceNumber);
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4 });
                    QueryRecordInfo.RackNo = new NumericFieldInfo(3, 0) { ComponentNumber = 1 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RackNo);
                    QueryRecordInfo.RackPosition = new NumericFieldInfo(3, 0) { ComponentNumber = 2 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RackPosition);
                    QueryRecordInfo.SampleID = new StringFieldInfo(3, 22) { ComponentNumber = 3 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);
                    QueryRecordInfo.SampleNumberAttribute = new StringFieldInfo(3, 1) { ComponentNumber = 4 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleNumberAttribute);


                    PatientRecordInfo = new PatientRecordInfo(26);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);

                    PatientRecordInfo.Labcode = new NumericFieldInfo(5, 0); //new StringFieldInfo(5, 16);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Labcode);

                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 3 });
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 20) { ComponentNumber = 2 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);

                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { DefaultValue = "U", ValidValues = new List<object> { "M", "F", "U", "" } };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);

                    //PatientRecordInfo.DateOfBirth = new DateFieldInfo(8, "yyyyMMdd") { DefaultValue = new DateTime(1990, 1, 1) };
                    //PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(14) { DefaultValue = "Dr.2" });
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(26) { ComponentCount = 3 });
                    //PatientRecordInfo.AddRecordField(new RecordFieldInfo(26) { ComponentNumber = 3, DefaultValue = "EAST" });


                    OrderRecordInfo = new OrderRecordInfo(26);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);

                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(3) { FieldNumber_Incoming = 4, ComponentCount = 4 });
                    OrderRecordInfo.RackNo = new NumericFieldInfo(3, 0) { FieldNumber_Incoming = 4, ComponentNumber = 1 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RackNo);
                    OrderRecordInfo.RackPosition = new NumericFieldInfo(3, 0) { FieldNumber_Incoming = 4, ComponentNumber = 2 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RackPosition);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 22) { FieldNumber_Incoming = 4, ComponentNumber = 3, IsFixedLength = true, BytesLength = 22, EmptySpaceFillerChar = ' ', IsRightJustified = true };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.SampleNumberAttribute = new StringFieldInfo(3, 1) { FieldNumber_Incoming = 4, ComponentNumber = 4 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleNumberAttribute);

                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 5, SupportsMultipleValues = true });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 6) { ComponentNumber = 5, SupportsMultipleValues = true };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);

                    OrderRecordInfo.ActionCode = new StringFieldInfo(12, 1) { DefaultValue = "N", ValidValues = new List<object> { "N", "A", "Q" } };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.ActionCode);
                    OrderRecordInfo.ReportType = new StringFieldInfo(26, 1);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.ReportType);


                    ResultRecordInfo = new ResultRecordInfo(13);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);

                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 9 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 27) { ComponentNumber = 5 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);

                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 2);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);

                    ResultRecordInfo.Units = new StringFieldInfo(5, 7);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);

                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(7, 2);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);

                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(13);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);


                    CommentRecordInfo = new CommentRecordInfo(4);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.RecordType);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.SequenceNumber);

                    CommentRecordInfo.CommentText = new StringFieldInfo(4, 100);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.CommentText);


                    TerminationRecordInfo = new TerminationRecordInfo(3);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "N" };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);

                    #endregion
                    break;

                case 7:
                    #region Atelica HL7 Code Base
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    //ComponentDelimiterHL7 = '&';
                    //RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = false;
                    PatientAndOrderRecordsMerged = false;
                    SendPatientOrderRecordWhenNoOrder = true;
                    IsBulkWorklistSender = false;
                    WorkOrderTimeoutMilliseconds = 30000;


                    //Message Header Segment (MSH)
                    HeaderRecordInfoHL7ACK = new HeaderRecordInfoHL7ACK(21);
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.RecordType);
                    HeaderRecordInfoHL7ACK.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.DelimiterCharacters);
                    HeaderRecordInfoHL7ACK.ReceiverID = new StringFieldInfo(3, 20) { DefaultValue = "Siemens Analyzer" };//
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.ReceiverID);
                    HeaderRecordInfoHL7ACK.AddRecordField(new StringFieldInfo(4, 20) { DefaultValue = "" });
                    HeaderRecordInfoHL7ACK.SenderID = new StringFieldInfo(5, 20) { DefaultValue = "host" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.SenderID);
                    HeaderRecordInfoHL7ACK.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACK.MsgIdentifier = new StringFieldInfo(9, 15) /*{ DefaultValue = "RSP^K11^RSP_K11" }*/;
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.MsgIdentifier);
                    HeaderRecordInfoHL7ACK.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Incoming = "GUID" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACK.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.VersionNumber);
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7ACK.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    HeaderRecordInfoHL7ACK.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });

                    HeaderRecordInfoHL7ACKResponseOrder = new HeaderRecordInfoHL7ACKResponseOrder(21);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.RecordType);
                    HeaderRecordInfoHL7ACKResponseOrder.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKResponseOrder.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "UIW_LIS" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.ReceiverID);
                    HeaderRecordInfoHL7ACKResponseOrder.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "LIS_ID" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.SenderID);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACKResponseOrder.MsgIdentifier = new StringFieldInfo(9, 40) /*{ DefaultValue = "RSP^K11^RSP_K11" }*/;
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.MsgIdentifier);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Incoming = "GUID-1" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKResponseOrder.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.VersionNumber);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });

                    //Message Header Segment (MSH)
                    HeaderRecordInfoHL7ACKMSATEST = new HeaderRecordInfoHL7ACKMSATEST(21);
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.RecordType);
                    HeaderRecordInfoHL7ACKMSATEST.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKMSATEST.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "Siemens Analyzer" };
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.ReceiverID);
                    HeaderRecordInfoHL7ACKMSATEST.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "host" };
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.SenderID);
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACKMSATEST.MsgIdentifier = new StringFieldInfo(9, 40) /*{ DefaultValue = "RSP^K11^RSP_K11" }*/;
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.MsgIdentifier);
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Incoming = "GUID-2" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKMSATEST.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(HeaderRecordInfoHL7ACKMSATEST.VersionNumber);
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    HeaderRecordInfoHL7ACKMSATEST.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });


                    HeaderRecordInfoHL7ACKMSAReqToResults = new HeaderRecordInfoHL7ACKMSAReqToResults(21);
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.RecordType);
                    HeaderRecordInfoHL7ACKMSAReqToResults.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKMSAReqToResults.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "Siemens Analyzer" };
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.ReceiverID);
                    HeaderRecordInfoHL7ACKMSAReqToResults.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "host" };
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.SenderID);
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACKMSAReqToResults.MsgIdentifier = new StringFieldInfo(9, 40) /*{ DefaultValue = "RSP^K11^RSP_K11" }*/;
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.MsgIdentifier);
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Incoming = "GUID-3" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKMSAReqToResults.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(HeaderRecordInfoHL7ACKMSAReqToResults.VersionNumber);
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    HeaderRecordInfoHL7ACKMSAReqToResults.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });


                    HeaderRecordInfoHL7ACKResponse = new HeaderRecordInfoHL7ACKResponse(21);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.RecordType);
                    HeaderRecordInfoHL7ACKResponse.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKResponse.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "LIS_ID" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.ReceiverID);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "" });
                    HeaderRecordInfoHL7ACKResponse.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "UIW_LIS" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.SenderID);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(6) { DefaultValue = "" });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new StringFieldInfo(7,26) { DefaultValue = formattedDateTime });
                    HeaderRecordInfoHL7ACKResponse.MsgIdentifier = new StringFieldInfo(9, 40) { DefaultValue = "RSP^K11^RSP_K11" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.MsgIdentifier);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new StringFieldInfo(10, 50) { /*CopyKey_Outgoing = "GUID"*/DefaultValue = uniqueGuid.ToString() }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKResponse.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.VersionNumber);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });


                    //Message Header Segment (MSH)
                    HeaderRecordInfoHL7 = new HeaderRecordInfoHL7(21);
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.RecordType);
                    HeaderRecordInfoHL7.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.DelimiterCharacters);
                    HeaderRecordInfoHL7.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "LIS_ID" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.ReceiverID);
                    HeaderRecordInfoHL7.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "UIW_LIS" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.SenderID);
                    //HeaderRecordInfoHL7.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7.AddRecordField(new StringFieldInfo(7, 26) { DefaultValue = formattedDateTime });
                    HeaderRecordInfoHL7.MsgIdentifier = new StringFieldInfo(9, 40) { DefaultValue = "OML^O33^OML_O33" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.MsgIdentifier);
                    HeaderRecordInfoHL7.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Outgoing = "GUID-1" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.VersionNumber);
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    HeaderRecordInfoHL7.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-28", ComponentNumber = 1 });
                    HeaderRecordInfoHL7.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });


                    //Query Parameter Definition Segment (QPD) for a result and order query
                    QueryRecordInfoHL7 = new QueryRecordInfoHL7(13);
                    QueryRecordInfoHL7.AddRecordField(QueryRecordInfoHL7.RecordType);
                    QueryRecordInfoHL7.AddRecordField(new RecordFieldInfo(2) { ComponentCount = 3 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(2, 20) { DefaultValue = "WOS", ComponentNumber = 1 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(2, 20) { DefaultValue = "Work Order Step", ComponentNumber = 2 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(2, 20) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    //QueryRecordInfoHL7.AddRecordField(new RecordFieldInfo(3)); // Query tag to identify the query. A GUID should be used
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(3, 32) { CopyKey_Incoming = "QPD-2" }); //Copy of QPD-2 from request message.
                    QueryRecordInfoHL7.SampleID = new StringFieldInfo(4, 256) { CopyKey_Incoming = "QPD-3" };
                    QueryRecordInfoHL7.AddRecordField(QueryRecordInfoHL7.SampleID); //Contains the Sample ID
                    QueryRecordInfoHL7.RackNoHL7 = new StringFieldInfo(5, 80) { CopyKey_Incoming = "QPD-4" };
                    QueryRecordInfoHL7.AddRecordField(QueryRecordInfoHL7.RackNoHL7); //Contains the Rack Number
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(6, 80) { CopyKey_Incoming = "QPD-5" });// Position number
                    QueryRecordInfoHL7.AddRecordField(new RecordFieldInfo(11) { ComponentCount = 3 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(11, 20) { DefaultValue = "SERPLAS", ComponentNumber = 1 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(11, 12) { DefaultValue = "99ROC", ComponentNumber = 3 });
                    QueryRecordInfoHL7.AddRecordField(new RecordFieldInfo(12) { ComponentCount = 3 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(12, 20) { DefaultValue = "SC", ComponentNumber = 1 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(12, 12) { DefaultValue = "99ROC", ComponentNumber = 3 });
                    QueryRecordInfoHL7.AddRecordField(new StringFieldInfo(13, 1) { DefaultValue = "R" });//Code indicating the original priority of the rack.  R= Routine

                    //Query Parameter Definition Segment (QPD) for a result and order query
                    QueryRecordInfoHL7Res = new QueryRecordInfoHL7(8);
                    QueryRecordInfoHL7Res.AddRecordField(QueryRecordInfoHL7Res.RecordType);
                    QueryRecordInfoHL7Res.AddRecordField(new RecordFieldInfo(2) { ComponentCount = 3 });
                    QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(2, 20) { DefaultValue = "WOS", ComponentNumber = 1 });
                    QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(2, 20) { DefaultValue = "Work Order Step", ComponentNumber = 2 });
                    QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(2, 12) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    //QueryRecordInfoHL7.AddRecordField(new RecordFieldInfo(3)); // Query tag to identify the query. A GUID should be used
                    QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(3, 32) { CopyKey_Outgoing = "QPD-2" }); //Copy of QPD-2 from request message.
                    QueryRecordInfoHL7Res.SampleID = new StringFieldInfo(4, 256) { CopyKey_Outgoing = "QPD-3" };
                    QueryRecordInfoHL7Res.AddRecordField(QueryRecordInfoHL7Res.SampleID); //Contains the Sample ID
                    //QueryRecordInfoHL7Res.RackNoHL7 = new StringFieldInfo(5, 80) { CopyKey_Outgoing = "QPD-4" };
                    //QueryRecordInfoHL7Res.AddRecordField(QueryRecordInfoHL7Res.RackNoHL7); //Contains the Rack Number
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(6, 80) { CopyKey_Outgoing = "QPD-5" });// Position number
                    //QueryRecordInfoHL7Res.AddRecordField(new RecordFieldInfo(11) { ComponentCount = 3 });
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(11, 20) { DefaultValue = "SERPLAS", ComponentNumber = 1 });
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(11, 12) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    //QueryRecordInfoHL7Res.AddRecordField(new RecordFieldInfo(12) { ComponentCount = 3 });
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(12, 20) { DefaultValue = "SC", ComponentNumber = 1 });
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(12, 12) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    //QueryRecordInfoHL7Res.AddRecordField(new StringFieldInfo(13, 1) { DefaultValue = "R" });

                    //(RCP)- Response Control Parameter Segment
                    ResponseControlInfoHL7 = new ResponseControlInfoHL7(4);
                    ResponseControlInfoHL7.AddRecordField(ResponseControlInfoHL7.RecordType);
                    ResponseControlInfoHL7.AddRecordField(new StringFieldInfo(2, 1) { DefaultValue = "I"});
                    ResponseControlInfoHL7.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 3 });
                    ResponseControlInfoHL7.AddRecordField(new StringFieldInfo(4, 30) { DefaultValue = "R", ComponentNumber = 1 });
                    ResponseControlInfoHL7.AddRecordField(new StringFieldInfo(4, 30) { DefaultValue = "Real Time", ComponentNumber = 2 });
                    ResponseControlInfoHL7.AddRecordField(new StringFieldInfo(4, 30) { DefaultValue = "HL70394", ComponentNumber = 3 });


                    //Patient Identification Segment(PID)
                    PatientRecordInfoHL7 = new PatientRecordInfoHL7(9);
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.RecordType);
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.SequenceNumber);
                    PatientRecordInfoHL7.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 5 });
                    PatientRecordInfoHL7.PatientID = new StringFieldInfo(4, 50) { ComponentNumber = 1 };//Patient ID
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.PatientID);
                    PatientRecordInfoHL7.AddRecordField(new StringFieldInfo(4, 30) { DefaultValue = "PT", ComponentNumber = 5 });
                    PatientRecordInfoHL7.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 7 });
                    PatientRecordInfoHL7.PatientName = new StringFieldInfo(6, 50) { ComponentNumber = 1 };//Patient Name
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.PatientName);
                    PatientRecordInfoHL7.PatientName = new StringFieldInfo(6, 50) { ComponentNumber = 2 };//Patient Name
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.PatientName);
                    PatientRecordInfoHL7.AddRecordField(new StringFieldInfo(6, 30) { DefaultValue = "L", ComponentNumber = 7 });
                    PatientRecordInfoHL7.DateOfBirth = new DateFieldInfo(8, "yyyyMMddhhmmss") { DefaultValue = new DateTime(1950, 1, 1) };
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.DateOfBirth);
                    PatientRecordInfoHL7.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.Gender);

                    //Patient Visit Information(PV1)
                    PatientVisitInfoHL7 = new PatientVisitInfoHL7(8);
                    PatientVisitInfoHL7.AddRecordField(PatientVisitInfoHL7.RecordType);
                    PatientVisitInfoHL7.AddRecordField(new StringFieldInfo(3, 1) { DefaultValue = "P" });
                    PatientVisitInfoHL7.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 2 });
                    PatientVisitInfoHL7.AddRecordField(new StringFieldInfo(4, 80) { DefaultValue = "103", ComponentNumber = 2 });
                    PatientVisitInfoHL7.AddRecordField(new RecordFieldInfo(8) { ComponentCount = 4 });
                    PatientVisitInfoHL7.PatientID = new StringFieldInfo(8, 12) { ComponentNumber = 1 };
                    PatientVisitInfoHL7.AddRecordField(PatientVisitInfoHL7.PatientID);
                    PatientVisitInfoHL7.AddRecordField(new StringFieldInfo(8, 50) { DefaultValue = "Test", ComponentNumber = 2 });
                    PatientVisitInfoHL7.AddRecordField(new StringFieldInfo(8, 30) { DefaultValue = "Test1", ComponentNumber = 3 });
                    PatientVisitInfoHL7.AddRecordField(new StringFieldInfo(8, 30) { DefaultValue = "Test3", ComponentNumber = 4 });

                    //Message Acknowledgment Segment(MSA)
                    OrderRecordInfoHL7MSA = new OrderRecordInfoHL7MSA(3);
                    OrderRecordInfoHL7MSA.AddRecordField(OrderRecordInfoHL7MSA.RecordType);
                    OrderRecordInfoHL7MSA.AddRecordField(new StringFieldInfo(2, 2) { DefaultValue = "AA" });//AcknowledgmentCode
                    OrderRecordInfoHL7MSA.AddRecordField(new StringFieldInfo(3, 20) { CopyKey_Outgoing = "GUID-2" });//Message Control ID

                    //Query Acknowledge Segment(QAK)
                    OrderRecordInfoHL7QAK = new OrderRecordInfoHL7QAK(4);
                    OrderRecordInfoHL7QAK.AddRecordField(OrderRecordInfoHL7QAK.RecordType);
                    OrderRecordInfoHL7QAK.AddRecordField(new StringFieldInfo(2, 32) { CopyKey_Outgoing = "QPD-2" }); //Copy of QPD-2 from request message
                    //OrderRecordInfoHL7QAK.SampleID = new StringFieldInfo(2, 32) { CopyKey_Outgoing = "QPD-3" };
                    //OrderRecordInfoHL7QAK.AddRecordField(OrderRecordInfoHL7QAK.SampleID); //Contains the Sample ID
                    OrderRecordInfoHL7QAK.AddRecordField(new StringFieldInfo(3, 2) { DefaultValue = "OK" });//Query responsestatus
                    OrderRecordInfoHL7QAK.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 3 });
                    OrderRecordInfoHL7QAK.AddRecordField(new StringFieldInfo(4, 1) { DefaultValue = "WOS", ComponentNumber = 1 });//Query responsestatus
                    OrderRecordInfoHL7QAK.AddRecordField(new StringFieldInfo(4, 1) { DefaultValue = "Work Order Step", ComponentNumber = 2 });//Query responsestatus
                    OrderRecordInfoHL7QAK.AddRecordField(new StringFieldInfo(4, 1) { DefaultValue = "IHELAW", ComponentNumber = 3 });//Query responsestatus

                    //Specimen Segment (SPM)
                    OrderRecordInfoHL7SPM = new OrderRecordInfoHL7SPM(27);
                    OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.RecordType);
                    OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.SequenceNumber);
                    OrderRecordInfoHL7SPM.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    OrderRecordInfoHL7SPM.SampleID = new StringFieldInfo(3, 20) { ComponentNumber = 1, CopyKey_Outgoing = "QPD-3" };
                    OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.SampleID);
                    //OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(3, 22) { DefaultValue = "BARCODE", ComponentNumber = 2 });//Sample ID / Sequence Number
                    OrderRecordInfoHL7SPM.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 3 });
                    OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(5, 20) { DefaultValue = "SER", ComponentNumber = 1 });
                    OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(5, 20) { DefaultValue = "Serum", ComponentNumber = 2 });
                    OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(5, 12) { DefaultValue = "HL70487", ComponentNumber = 3 });
                    OrderRecordInfoHL7SPM.AddRecordField(new RecordFieldInfo(12) { ComponentCount = 3 });
                    OrderRecordInfoHL7SPM.ActionCode = new StringFieldInfo(12, 20) { DefaultValue = "P", ComponentNumber = 1 };
                    OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.ActionCode);
                    OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(12, 20) { DefaultValue = "Patient specimen", ComponentNumber = 2 });
                    OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(12, 12) { DefaultValue = "HL70369", ComponentNumber = 3 });
                    //OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(15, 250) { DefaultValue = "~~~~" });
                    //OrderRecordInfoHL7SPM.SCT = new DateFieldInfo(17, "yyyyMMddhhmmss")/* { DefaultValue = new DateTime(1950, 1, 1) }*/;
                    //OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.SCT);
                    //OrderRecordInfoHL7SPM.SCT = new DateFieldInfo(18, "yyyyMMddhhmmss")/* { DefaultValue = new DateTime(1950, 1, 1) }*/;
                    //OrderRecordInfoHL7SPM.AddRecordField(OrderRecordInfoHL7SPM.SCT);
                    //OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(26, 250) { DefaultValue = "^^" });
                    //OrderRecordInfoHL7SPM.AddRecordField(new StringFieldInfo(27, 20));

                    //Specimen Container Detail Segment (SAC)
                    OrderRecordInfoHL7SAC = new OrderRecordInfoHL7SAC(29);
                    OrderRecordInfoHL7SAC.AddRecordField(OrderRecordInfoHL7SAC.RecordType);
                    OrderRecordInfoHL7SAC.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 2 });
                    OrderRecordInfoHL7SAC.SampleID = new StringFieldInfo(4, 20) { ComponentNumber = 1 };
                    OrderRecordInfoHL7SAC.AddRecordField(OrderRecordInfoHL7SAC.SampleID);
                    //OrderRecordInfoHL7SAC.AddRecordField(new StringFieldInfo(4, 10) { DefaultValue = "BARCODE", ComponentNumber = 2 });//BARCODE / Sequence Number
                    //OrderRecordInfoHL7SAC.AddRecordField(new StringFieldInfo(11, 5) { CopyKey_Outgoing = "QPD-4" });// RACK number
                    //OrderRecordInfoHL7SAC.AddRecordField(new StringFieldInfo(12, 1) { CopyKey_Outgoing = "QPD-5" });// Position number
                    OrderRecordInfoHL7SAC.AddRecordField(new StringFieldInfo(29, 20) { DefaultValue = "^1^:^0.5" });//BARCODE / Sequence Number

                    //Common Order Segment (ORC)
                    OrderRecordInfoHL7ORC = new OrderRecordInfoHL7ORC(10);
                    OrderRecordInfoHL7ORC.AddRecordField(OrderRecordInfoHL7ORC.RecordType);
                    OrderRecordInfoHL7ORC.AddRecordField(new StringFieldInfo(2, 250) { DefaultValue = "NW" });
                    OrderRecordInfoHL7ORC.SCT = new DateFieldInfo(10, "yyyyMMddhhmmss") /*{ DefaultValue = new DateTime(1950, 1, 1) }*/; // String in format YYYYMMDDhhmmss
                    OrderRecordInfoHL7ORC.AddRecordField(OrderRecordInfoHL7ORC.SCT);

                    //Timing/Quantity Segment (TQ1)
                    OrderRecordInfoHL7TQ1 = new OrderRecordInfoHL7TQ1(10);
                    OrderRecordInfoHL7TQ1.AddRecordField(OrderRecordInfoHL7TQ1.RecordType);
                    OrderRecordInfoHL7TQ1.AddRecordField(new RecordFieldInfo(10) { ComponentCount = 3 });
                    OrderRecordInfoHL7TQ1.AddRecordField(new StringFieldInfo(10, 1) { DefaultValue = "R", ComponentNumber = 1 });
                    OrderRecordInfoHL7TQ1.AddRecordField(new StringFieldInfo(10, 12) { DefaultValue = "HL70485", ComponentNumber = 3 });

                    //Observation Request Segment(OBR)
                    OrderRecordInfoHL7OBR = new OrderRecordInfoHL7OBR(46);
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.RecordType);
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.SequenceNumber);
                    OrderRecordInfoHL7OBR.AddRecordField(new StringFieldInfo(3, 50) { DefaultValue = "4711" });

                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 3 });

                    OrderRecordInfoHL7OBR.TestID_ManufacturersTestCode = new StringFieldInfo(5, 20) { ComponentNumber = 1 };
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.TestID_ManufacturersTestCode);

                    OrderRecordInfoHL7OBR.AddRecordField(new StringFieldInfo(5, 20) {  ComponentNumber = 2, CopyFieldFrom = OrderRecordInfoHL7OBR.TestID_ManufacturersTestCode });
                    //OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.TestID_ManufacturersTestCode);

                    OrderRecordInfoHL7OBR.AddRecordField(new StringFieldInfo(5, 12) { DefaultValue = "LN", ComponentNumber = 3 });

                    OrderRecordInfoHL7OBR.ActionCode = new StringFieldInfo(12, 1) { DefaultValue = "G" };
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.ActionCode);

                    //Test Code Detail Segment(TCD)
                    OrderRecordInfoHL7TCD = new OrderRecordInfoHL7TCD(11);
                    OrderRecordInfoHL7TCD.AddRecordField(OrderRecordInfoHL7TCD.RecordType);
                    OrderRecordInfoHL7TCD.AddRecordField(new RecordFieldInfo(2) { ComponentCount = 3 });
                    OrderRecordInfoHL7TCD.TestID_ManufacturersTestCode = new StringFieldInfo(2, 20) { ComponentNumber = 1 };//^Assay Number (string)
                    OrderRecordInfoHL7TCD.AddRecordField(OrderRecordInfoHL7TCD.TestID_ManufacturersTestCode);
                    OrderRecordInfoHL7TCD.TestID_ManufacturersTestCode = new StringFieldInfo(2, 20) { ComponentNumber = 2 };//^Assay Number (string)
                    OrderRecordInfoHL7TCD.AddRecordField(OrderRecordInfoHL7TCD.TestID_ManufacturersTestCode);
                    OrderRecordInfoHL7TCD.AddRecordField(new StringFieldInfo(2, 12) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    OrderRecordInfoHL7TCD.AddRecordField(new StringFieldInfo(11, 250) { DefaultValue = "D1" });

                    //Observation/Result Segment (OBX)
                    ResultRecordInfoHL7 = new ResultRecordInfoHL7(30);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.RecordType);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.SequenceNumber);

                    ResultRecordInfoHL7.Resulttype = new StringFieldInfo(3, 2);//Result Type
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.Resulttype);

                    ResultRecordInfoHL7.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 6 });
                    ResultRecordInfoHL7.ResultAspects = new StringFieldInfo(4, 250) { ComponentNumber = 1 };//Aspect.Name
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.ResultAspects);
                    ResultRecordInfoHL7.TestID_ManufacturersTestCode = new StringFieldInfo(4, 16) { ComponentNumber = 2/*, SupportsMultipleValues = true*/ };//^Assay Name (string)
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.TestID_ManufacturersTestCode);
                    //ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(4, 16) { ComponentNumber = 2 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(4, 12) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(4, 12) { DefaultValue = "IHELAW", ComponentNumber = 6 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(5, 20) { DefaultValue = "1" });
                    ResultRecordInfoHL7.TestValue = new NumericFieldInfo(6, 16);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.TestValue);
                    ResultRecordInfoHL7.AddRecordField(new RecordFieldInfo(7) { ComponentCount = 3 });
                    ResultRecordInfoHL7.Units = new StringFieldInfo(7, 20) { ComponentNumber = 1 };
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.Units);
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(7, 199) { DefaultValue = "IHELAW", ComponentNumber = 3 });
                    ResultRecordInfoHL7.AddRecordField(new RecordFieldInfo(9) { ComponentCount = 3 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(9, 3) { ComponentNumber = 1 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(9, 12) { ComponentNumber = 3 });
                    ResultRecordInfoHL7.Gender = new StringFieldInfo(12, 1) { DefaultValue = new List<object> { "M", "F", "U" } };
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.Gender);
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(17, 10) /*{ DefaultValue = "vjc~BATCH" }*/);
                    ResultRecordInfoHL7.AddRecordField(new RecordFieldInfo(19) { ComponentCount = 4 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(19, 25) { ComponentNumber = 1 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(19, 25) { ComponentNumber = 2 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(19, 25) { ComponentNumber = 3 });
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(19, 25) { ComponentNumber = 4 });
                    ResultRecordInfoHL7.DateOfBirth = new DateFieldInfo(20, "yyyyMMddhhmmss") { DefaultValue = new DateTime(1950, 1, 1) }; // String in format YYYYMMDDhhmmss
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.DateOfBirth);
                    ResultRecordInfoHL7.AddRecordField(new StringFieldInfo(30, 4) { DefaultValue = "RSLT" });

                    //Observation/Result // Header Segment (MSH)ACK Response
                    ResultRecordInfoHL7ACKResults = new ResultRecordInfoHL7ACKResults(21);
                    ResultRecordInfoHL7ACKResults.AddRecordField(ResultRecordInfoHL7ACKResults.RecordType);
                    ResultRecordInfoHL7ACKResults.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    ResultRecordInfoHL7ACKResults.AddRecordField(ResultRecordInfoHL7ACKResults.DelimiterCharacters);
                    ResultRecordInfoHL7ACKResults.ReceiverID = new StringFieldInfo(3, 30) { DefaultValue = "Host" };
                    ResultRecordInfoHL7ACKResults.AddRecordField(HeaderRecordInfoHL7ACKResponse.ReceiverID);
                    ResultRecordInfoHL7ACKResults.SenderID = new StringFieldInfo(5, 30) { DefaultValue = "Siemens Analyzer" };
                    ResultRecordInfoHL7ACKResults.AddRecordField(ResultRecordInfoHL7ACKResults.SenderID);
                    ResultRecordInfoHL7ACKResults.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    ResultRecordInfoHL7ACKResults.MsgIdentifier = new StringFieldInfo(9, 40) { DefaultValue = "ACK^R22^ACK" };
                    ResultRecordInfoHL7ACKResults.AddRecordField(ResultRecordInfoHL7ACKResults.MsgIdentifier);
                    ResultRecordInfoHL7ACKResults.AddRecordField(new StringFieldInfo(10, 50) { CopyKey_Outgoing = "GUID-3" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    ResultRecordInfoHL7ACKResults.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    ResultRecordInfoHL7ACKResults.VersionNumber = new StringFieldInfo(12, 60) { DefaultValue = "2.5.1" };
                    ResultRecordInfoHL7ACKResults.AddRecordField(ResultRecordInfoHL7ACKResults.VersionNumber);
                    ResultRecordInfoHL7ACKResults.AddRecordField(new RecordFieldInfo(15) { DefaultValue = "NE" });
                    ResultRecordInfoHL7ACKResults.AddRecordField(new RecordFieldInfo(16) { DefaultValue = "AL" });
                    ResultRecordInfoHL7ACKResults.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "UNICODE UTF-8" });
                    ResultRecordInfoHL7ACKResults.AddRecordField(new RecordFieldInfo(21) { ComponentCount = 2 });
                    ResultRecordInfoHL7ACKResults.AddRecordField(new StringFieldInfo(21, 50) { DefaultValue = "LAB-27", ComponentNumber = 1 });
                    ResultRecordInfoHL7ACKResults.AddRecordField(new StringFieldInfo(21, 20) { DefaultValue = "IHE", ComponentNumber = 2 });



                    //Message Acknowledgment Segment(MSA) Results
                    OrderRecordInfoHL7MSAResults = new OrderRecordInfoHL7MSAResults(3);
                    OrderRecordInfoHL7MSAResults.AddRecordField(OrderRecordInfoHL7MSAResults.RecordType);
                    OrderRecordInfoHL7MSAResults.AddRecordField(new StringFieldInfo(2, 2) { DefaultValue = "AA" });//AcknowledgmentCode
                    OrderRecordInfoHL7MSAResults.AddRecordField(new StringFieldInfo(3, 20) { CopyKey_Outgoing = "GUID-2" });//Message Control ID

                    #endregion
                    break;


                case 152:
                    #region DxH900
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '!';
                    RepeatDelimiter = '~';
                    HeaderAndTerminationRecordRequired = true;
                    RemoveUnmappedTestcodes = true;
                    ReplacementWOTests = new string[] { "C" };

                    //Header record info
                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SequenceNumber);
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 1) { DefaultValue = @"|\!~" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(3, 50));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(4, 1));
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 2 });
                    HeaderRecordInfo.SenderID = new StringFieldInfo(5, 9) { ComponentNumber = 1, DefaultValue = "DxH 500" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(5, 2) { DefaultValue = "01", ComponentNumber = 2 };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.ProcessingID = new StringFieldInfo(12, 1) { DefaultValue = "P" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ProcessingID);
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 7) { DefaultValue = "LIS2-A2" }; //ASTM Version Number
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);
                    HeaderRecordInfo.AddRecordField(new DateFieldInfo(14, "yyyyMMddHHmmss") { IsCurrentDateTime = true });

                    //Patient Record info
                    PatientRecordInfo = new PatientRecordInfo(34);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.PatientID = new StringFieldInfo(4, 16);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 2 });
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 15) { ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);//Last Name
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 15) { ComponentNumber = 2 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);//First Name
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(8) { ComponentCount = 3 });
                    PatientRecordInfo.DateOfBirth = new DateFieldInfo(8, dateFormat: "yyyyMMdd") { ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);
                    PatientRecordInfo.Age = new NumericFieldInfo(8, 3) { ComponentNumber = 2 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Age);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(8, 1) { ComponentNumber = 3, DefaultValue = "Y" });
                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(26, 16));//Location


                    //Inquiry Records
                    //QueryRecordInfo = new QueryRecordInfo(13);
                    //QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    //QueryRecordInfo.AddRecordField(QueryRecordInfo.SequenceNumber);
                    //QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    //QueryRecordInfo.SampleID = new StringFieldInfo(3, 22) { ComponentNumber = 2 };//Sample ID No
                    //QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);
                    //QueryRecordInfo.QueryStatusCode = new StringFieldInfo(13, 1);
                    //QueryRecordInfo.AddRecordField(QueryRecordInfo.QueryStatusCode);

                    //Order record info
                    OrderRecordInfo = new OrderRecordInfo(31);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 16); //Specimen ID
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4 });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 3) { ComponentNumber = 3 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);
                    OrderRecordInfo.SCT = new DateFieldInfo(7, "YYYYMMDDHHMMSS"); //Sample date
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SCT);
                    OrderRecordInfo.ActionCode = new StringFieldInfo(12, 1) { DefaultValue = "N", ValidValues = new List<object> { "C", "Q", "N" } };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.ActionCode);
                    OrderRecordInfo.SampleType = new StringFieldInfo(16, 2) { DefaultValue = "WB" };// Sample Type
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleType);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(17) { ComponentCount = 4 });
                    OrderRecordInfo.RefDr = new StringFieldInfo(17, 16) { ComponentNumber = 1 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RefDr);// PhysianId

                    //Message Result Records
                    ResultRecordInfo = new ResultRecordInfo(15);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 6) { ComponentNumber = 4 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 2 });
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 6) { ComponentNumber = 1 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(4, 4) { ComponentNumber = 2 });
                    ResultRecordInfo.Units = new StringFieldInfo(5, 15);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(7, 24)); //Reference Ranges
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(8, 1);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(14) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);

                    //Comment Records
                    CommentRecordInfo = new CommentRecordInfo(5);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.RecordType);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.SequenceNumber);
                    CommentRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { DefaultValue = "I" }); //Comment Source
                    CommentRecordInfo.CommentText = new StringFieldInfo(4, 120);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.CommentText);
                    CommentRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { DefaultValue = "I" }); //Type

                    //Message Terminator Record
                    TerminationRecordInfo = new TerminationRecordInfo(4);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "N", ValidValues = new List<object> { "N", "T", "R", "E" } };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);

                    #endregion
                    break;

                case 153:
                    #region Biorad Variant II
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = true;
                    PatientAndOrderRecordsMerged = false;
                    SupportsMultipleTestcodes = true;
                    RemoveUnmappedTestcodes = false;

                    HeaderRecordInfo = new HeaderRecordInfo(13);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.SenderID = new StringFieldInfo(5, 10);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(10, 10);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(12) { DefaultValue = "P" });
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 7) { DefaultValue = "1" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);

                    QueryRecordInfo = new QueryRecordInfo(13);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.SequenceNumber.FieldNumber = 0;
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    QueryRecordInfo.SampleID = new StringFieldInfo(3, 20) { ComponentNumber = 2 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);

                    PatientRecordInfo = new PatientRecordInfo(26);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.PatientID = new StringFieldInfo(3, 13);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 3 });
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 30) { ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);
                    PatientRecordInfo.DateOfBirth = new DateFieldInfo(8);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);
                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);

                    OrderRecordInfo = new OrderRecordInfo(26);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 3 });
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 20) { ComponentNumber = 1 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4 });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 8) { ComponentNumber = 4 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);

                    ResultRecordInfo = new ResultRecordInfo(14);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 5 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 8) { ComponentNumber = 4 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.ResultAspects = new StringFieldInfo(3, 8) { ComponentNumber = 5 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAspects);
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 3) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.Units = new StringFieldInfo(5, 6);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(7, 15) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(13) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);
                    ResultRecordInfo.InstrumentIdKey = new StringFieldInfo(14, 1);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.InstrumentIdKey);

                    CommentRecordInfo = new CommentRecordInfo(5);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.RecordType);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.SequenceNumber);
                    CommentRecordInfo.CommentText = new StringFieldInfo(4, 120);
                    CommentRecordInfo.AddRecordField(CommentRecordInfo.CommentText);

                    TerminationRecordInfo = new TerminationRecordInfo(3);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "F" };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);
                    #endregion
                    break;

                case 154:
                    #region DxH800
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '!';
                    RepeatDelimiter = '~';
                    HeaderAndTerminationRecordRequired = true;
                    RemoveUnmappedTestcodes = true;
                    ReplacementWOTests = new string[] { "CBC" };

                    //Header record info
                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 1) { DefaultValue = @"|\!~" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(3, 50));
                    HeaderRecordInfo.AddRecordField(new StringFieldInfo(4, 1));
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 2 });
                    HeaderRecordInfo.SenderID = new StringFieldInfo(5, 9) { ComponentNumber = 1, DefaultValue = "DxH 800" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(5, 2) { DefaultValue = "01", ComponentNumber = 2 };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.ProcessingID = new StringFieldInfo(12, 1) { DefaultValue = "P" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ProcessingID);
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 7) { DefaultValue = "LIS2-A2" }; //ASTM Version Number
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);
                    HeaderRecordInfo.AddRecordField(new DateFieldInfo(14, "yyyyMMddHHmmss") { IsCurrentDateTime = true });

                    //Patient Record info
                    PatientRecordInfo = new PatientRecordInfo(34);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.PatientID = new StringFieldInfo(4, 16);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientID);
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(6) { ComponentCount = 2 });
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 15) { ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);//Last Name
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 15) { ComponentNumber = 2 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);//First Name
                    PatientRecordInfo.AddRecordField(new RecordFieldInfo(8) { ComponentCount = 3 });

                    PatientRecordInfo.DateOfBirth = new DateFieldInfo(8, "yyyyMMdd") { DefaultValue = new DateTime(1950, 1, 1), ComponentNumber = 1 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.DateOfBirth);

                    PatientRecordInfo.Age = new NumericFieldInfo(8, 3) { ComponentNumber = 2 };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Age);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(8, 1) { ComponentNumber = 3, DefaultValue = "Y" });

                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);
                    PatientRecordInfo.AddRecordField(new StringFieldInfo(26, 16));//Location


                    //Inquiry Records
                    QueryRecordInfo = new QueryRecordInfo(13);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SequenceNumber);
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    QueryRecordInfo.SampleID = new StringFieldInfo(3, 22) { ComponentNumber = 2 };//Sample ID No
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);
                    QueryRecordInfo.QueryStatusCode = new StringFieldInfo(13, 1) { DefaultValue = "O" };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.QueryStatusCode);

                    //Order record info
                    OrderRecordInfo = new OrderRecordInfo(31);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 16); //Specimen ID
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4 });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 3) { ComponentNumber = 3 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);
                    OrderRecordInfo.SCT = new DateFieldInfo(7); //Sample date
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SCT);
                    OrderRecordInfo.ActionCode = new StringFieldInfo(12, 1) { DefaultValue = "N", ValidValues = new List<object> { "C", "Q", "N" } };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.ActionCode);
                    OrderRecordInfo.SampleType = new StringFieldInfo(16, 2) { DefaultValue = "WB" };// Sample Type
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleType);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(17) { ComponentCount = 4 });
                    OrderRecordInfo.RefDr = new StringFieldInfo(17, 16) { ComponentNumber = 1 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RefDr);// PhysianId

                    //Message Result Records
                    ResultRecordInfo = new ResultRecordInfo(15);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 6) { ComponentNumber = 4 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 2 });
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 6) { ComponentNumber = 1 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(4, 4) { ComponentNumber = 2 });
                    ResultRecordInfo.Units = new StringFieldInfo(5, 15);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.AddRecordField(new StringFieldInfo(7, 24)); //Reference Ranges
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(8, 1);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(14) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);

                    //Comment Records
                    //CommentRecordInfo = new CommentRecordInfo(5);
                    //CommentRecordInfo.AddRecordField(CommentRecordInfo.RecordType);
                    //CommentRecordInfo.AddRecordField(CommentRecordInfo.SequenceNumber);
                    //CommentRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { DefaultValue = "I" }); //Comment Source
                    //CommentRecordInfo.CommentText = new StringFieldInfo(4, 120);
                    //CommentRecordInfo.AddRecordField(CommentRecordInfo.CommentText);
                    //CommentRecordInfo.AddRecordField(new StringFieldInfo(5, 1) { DefaultValue = "I" }); //Type

                    //Message Terminator Record
                    TerminationRecordInfo = new TerminationRecordInfo(4);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.TerminationCode = new StringFieldInfo(3, 1) { DefaultValue = "N", ValidValues = new List<object> { "N", "T", "R", "E" } };
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.TerminationCode);

                    #endregion
                    break;

                case 313:
                    #region H9 HPLC
                    #endregion
                    break;

                case 314:
                    #region HL7 Code Base For Nabidh
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    HeaderAndTerminationRecordRequired = false;
                    PatientAndOrderRecordsMerged = false;
                    SendPatientOrderRecordWhenNoOrder = true;
                    IsBulkWorklistSender = true;
                    WorkOrderTimeoutMilliseconds = 30000;
                    IsHL7 = true;

                    //Message Header Segment (MSH)
                    HeaderRecordInfoHL7ACK = new HeaderRecordInfoHL7ACK(21);
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.RecordType);
                    HeaderRecordInfoHL7ACK.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.DelimiterCharacters); 
                    
                    #endregion
                    break;

                case 155:
                    #region Zybio EXZ 6000 H6
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    ComponentDelimiterHL7 = '&';
                    RepeatDelimiter = '\\';
                    WorkOrderTimeoutMilliseconds = 30000;
                    IsHL7 = true;

                    //Message Header Segment (MSH) from Machine with result
                    HeaderRecordInfoHL7 = new HeaderRecordInfoHL7(21);
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.RecordType);
                    HeaderRecordInfoHL7.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.DelimiterCharacters);
                    HeaderRecordInfoHL7.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7.MsgIdentifier = new StringFieldInfo(9, 7);//MSG Type
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.MsgIdentifier);
                    HeaderRecordInfoHL7.MsgControlid = new StringFieldInfo(10, 20) { CopyKey_Incoming = "GUID" };//Message Control ID
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.MsgControlid);
                    //HeaderRecordInfoHL7.AddRecordField(new StringFieldInfo(10, 1) { CopyKey_Incoming = "GUID" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7.VersionNumber = new StringFieldInfo(12, 5) { DefaultValue = "2.3.1" };
                    HeaderRecordInfoHL7.AddRecordField(HeaderRecordInfoHL7.VersionNumber);

                    //Message Header Segment (MSH) ACK to Machine 
                    HeaderRecordInfoHL7ACK = new HeaderRecordInfoHL7ACK(21);
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.RecordType);
                    HeaderRecordInfoHL7ACK.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.DelimiterCharacters);
                    HeaderRecordInfoHL7ACK.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACK.MsgIdentifier = new StringFieldInfo(9, 7) { DefaultValue = "ACK^R01" };//MSG Identifier
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.MsgIdentifier);
                    HeaderRecordInfoHL7ACK.MsgControlid = new StringFieldInfo(10, 20) { CopyKey_Outgoing = "GUID" };//Message Control ID
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.MsgControlid);
                    //HeaderRecordInfoHL7ACK.AddRecordField(new StringFieldInfo(10, 1) { CopyKey_Outgoing = "GUID" }); // Message Control ID that uniquely identifies the message, for example, a sequence number or GUID string
                    HeaderRecordInfoHL7ACK.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACK.VersionNumber = new StringFieldInfo(12, 5) { DefaultValue = "2.3.1" };
                    HeaderRecordInfoHL7ACK.AddRecordField(HeaderRecordInfoHL7ACK.VersionNumber);

                    //Patient Identification Segment(PID)
                    PatientRecordInfoHL7 = new PatientRecordInfoHL7(32);
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.RecordType);
                    PatientRecordInfoHL7.PatientName = new StringFieldInfo(7, 48);//Patient Name
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.PatientName);
                    PatientRecordInfoHL7.DateOfBirth = new DateFieldInfo(8, "yyyyMMddhhmmss") /*{ DefaultValue = new DateTime(1950, 1, 1) }*/;
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.DateOfBirth);
                    PatientRecordInfoHL7.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfoHL7.AddRecordField(PatientRecordInfoHL7.Gender);

                    //Observation Request Segment(OBR)
                    OrderRecordInfoHL7OBR = new OrderRecordInfoHL7OBR(32);
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.RecordType);
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.SequenceNumber);
                    OrderRecordInfoHL7OBR.SampleID = new StringFieldInfo(4, 22);//Sample ID == Barcode
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.SampleID);
                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 3});
                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(5) { ComponentNumber = 1, DefaultValue = "01001" });
                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(5) { ComponentNumber = 2, DefaultValue = "Automated Count" });
                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(5) { ComponentNumber = 3, DefaultValue = "99MRC" });
                    OrderRecordInfoHL7OBR.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    OrderRecordInfoHL7OBR.ResultedDateTime = new DateFieldInfo(8, "yyyyMMddhhmmss") { IsCurrentDateTime = true };
                    OrderRecordInfoHL7OBR.SCT = new DateFieldInfo(15, "yyyyMMddhhmmss") { IsCurrentDateTime = true };
                    OrderRecordInfoHL7OBR.SampleType = new StringFieldInfo(16, 6) { DefaultValue = "OTHER" };
                    OrderRecordInfoHL7OBR.AddRecordField(OrderRecordInfoHL7OBR.SampleType);
                    OrderRecordInfoHL7OBR.AddRecordField(new RecordFieldInfo(25) { DefaultValue = "HM" });

                    //Observation/Result Segment (OBX)
                    ResultRecordInfoHL7 = new ResultRecordInfoHL7(19);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.RecordType);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.SequenceNumber);
                    ResultRecordInfoHL7.ResultAspects = new StringFieldInfo(3, 3);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.ResultAspects);
                    ResultRecordInfoHL7.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 3 });
                    ResultRecordInfoHL7.TestID_ManufacturersTestCode = new StringFieldInfo(4, 200) { ComponentNumber = 2};
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.TestID_ManufacturersTestCode);
                    ResultRecordInfoHL7.TestValue = new NumericFieldInfo(6, 16);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.TestValue);
                    ResultRecordInfoHL7.Units = new StringFieldInfo(7, 6);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.Units);
                    ResultRecordInfoHL7.ResultAbnormalFlags = new StringFieldInfo(9, 5);
                    ResultRecordInfoHL7.AddRecordField(ResultRecordInfoHL7.ResultAbnormalFlags);

                    //Query Response ACK Header 
                    HeaderRecordInfoHL7ACKResponse = new HeaderRecordInfoHL7ACKResponse(21);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.RecordType);
                    HeaderRecordInfoHL7ACKResponse.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACKResponse.MsgIdentifier = new StringFieldInfo(9, 7) { DefaultValue = "QCK^Q02" };//MSG Identifier
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.MsgIdentifier);
                    HeaderRecordInfoHL7ACKResponse.MsgControlid = new StringFieldInfo(10, 20) { CopyKey_Outgoing = "GUID" };//Message Control ID
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.MsgControlid);
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKResponse.VersionNumber = new StringFieldInfo(12, 5) { DefaultValue = "2.3.1" };
                    HeaderRecordInfoHL7ACKResponse.AddRecordField(HeaderRecordInfoHL7ACKResponse.VersionNumber);

                    //Message Header Segment (MSH) ACK for Order which will sent to machine
                    HeaderRecordInfoHL7ACKResponseOrder = new HeaderRecordInfoHL7ACKResponseOrder(21);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.RecordType);
                    HeaderRecordInfoHL7ACKResponseOrder.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"^~\&" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.DelimiterCharacters);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new DateFieldInfo(7) { IsCurrentDateTime = true });
                    HeaderRecordInfoHL7ACKResponseOrder.MsgIdentifier = new StringFieldInfo(9, 7) { DefaultValue = "DSR^Q03" };//MSG Identifier
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.MsgIdentifier);
                    HeaderRecordInfoHL7ACKResponseOrder.MsgControlid = new StringFieldInfo(10, 20) { CopyKey_Outgoing = "GUID" };//Message Control ID
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.MsgControlid);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(11) { DefaultValue = "P" });
                    HeaderRecordInfoHL7ACKResponseOrder.VersionNumber = new StringFieldInfo(12, 5) { DefaultValue = "2.3.1" };
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(HeaderRecordInfoHL7ACKResponseOrder.VersionNumber);
                    HeaderRecordInfoHL7ACKResponseOrder.AddRecordField(new RecordFieldInfo(18) { DefaultValue = "ASCII" });

                    //QRD FROM MACHINE
                    QueryDefinitionSegmentFromMachine = new QueryDefinitionSegmentFromMachine(14);
                    QueryDefinitionSegmentFromMachine.AddRecordField(QueryDefinitionSegmentFromMachine.RecordType);
                    QueryDefinitionSegmentFromMachine.SCT = new DateFieldInfo(2, "yyyyMMddhhmmss") { IsCurrentDateTime = true };
                    QueryDefinitionSegmentFromMachine.AddRecordField(new RecordFieldInfo(3) { DefaultValue = "R" });//Query Format Code
                    QueryDefinitionSegmentFromMachine.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "D" });//Query Priority
                    QueryDefinitionSegmentFromMachine.MsgControlid = new StringFieldInfo(5, 20) { CopyKey_Incoming = "GUID_QRD" };//Message Control ID
                    QueryDefinitionSegmentFromMachine.AddRecordField(QueryDefinitionSegmentFromMachine.MsgControlid);
                    QueryDefinitionSegmentFromMachine.AddRecordField(new RecordFieldInfo(8) { DefaultValue = "RD" });//Query Status
                    QueryDefinitionSegmentFromMachine.SampleID = new StringFieldInfo(9, 10);//SampleId
                    QueryDefinitionSegmentFromMachine.AddRecordField(QueryDefinitionSegmentFromMachine.SampleID);
                    QueryDefinitionSegmentFromMachine.AddRecordField(new RecordFieldInfo(10) { DefaultValue = "OTH" });//Query Status
                    QueryDefinitionSegmentFromMachine.AddRecordField(new RecordFieldInfo(13) { DefaultValue = "T" });//Query Priority

                    //QRD FROM LIS
                    QueryDefinitionSegmentFromLis = new QueryDefinitionSegmentFromLis(14);
                    QueryDefinitionSegmentFromLis.AddRecordField(QueryDefinitionSegmentFromLis.RecordType);
                    QueryDefinitionSegmentFromLis.SCT = new DateFieldInfo(2, "yyyyMMddhhmmss") { IsCurrentDateTime = true };
                    QueryDefinitionSegmentFromLis.AddRecordField(new RecordFieldInfo(3) { DefaultValue = "R" });//Query Format Code
                    QueryDefinitionSegmentFromLis.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "D" });//Query Priority
                    QueryDefinitionSegmentFromLis.MsgControlid = new StringFieldInfo(5, 20) { CopyKey_Outgoing = "GUID_QRD" };//Message Control ID
                    QueryDefinitionSegmentFromLis.AddRecordField(QueryDefinitionSegmentFromLis.MsgControlid);
                    QueryDefinitionSegmentFromLis.AddRecordField(new RecordFieldInfo(8) { DefaultValue = "RD" });//Query Status
                    QueryDefinitionSegmentFromLis.SampleID = new StringFieldInfo(9, 10);//SampleId
                    QueryDefinitionSegmentFromLis.AddRecordField(QueryDefinitionSegmentFromLis.SampleID);
                    QueryDefinitionSegmentFromLis.AddRecordField(new RecordFieldInfo(10) { DefaultValue = "OTH" });//Query Status
                    QueryDefinitionSegmentFromLis.AddRecordField(new RecordFieldInfo(13) { DefaultValue = "T" });//Query Priority

                    //QRF Query Filter Segment
                    QueryFilterSegment = new QueryFilterSegment(11);
                    QueryFilterSegment.AddRecordField(QueryFilterSegment.RecordType);
                    QueryFilterSegment.AddRecordField(new RecordFieldInfo(7) { DefaultValue = "RCT" });
                    QueryFilterSegment.AddRecordField(new RecordFieldInfo(8) { DefaultValue = "COR" });
                    QueryFilterSegment.AddRecordField(new RecordFieldInfo(9) { DefaultValue = "ALL" });

                    //ERR Error Segment
                    ErrorSegment = new ErrorSegment(3);
                    ErrorSegment.AddRecordField(ErrorSegment.RecordType);
                    ErrorSegment.CommentText_CMN = new StringFieldInfo(2, 1) { DefaultValue = "0" };
                    ErrorSegment.AddRecordField(ErrorSegment.CommentText_CMN);

                    //QAK Query Acknowledgment Segment
                    QueryAcknowledgmentSegment = new QueryAcknowledgmentSegment(4);
                    QueryAcknowledgmentSegment.AddRecordField(QueryAcknowledgmentSegment.RecordType);
                    QueryAcknowledgmentSegment.CommentText_CMN = new StringFieldInfo(2, 2) { DefaultValue = "SR" };
                    QueryAcknowledgmentSegment.AddRecordField(QueryAcknowledgmentSegment.CommentText_CMN);
                    QueryAcknowledgmentSegment.CommentText = new StringFieldInfo(3, 2) { DefaultValue = "OK" };
                    QueryAcknowledgmentSegment.AddRecordField(QueryAcknowledgmentSegment.CommentText);

                    //DSR Display Data Segment
                    //DSR-1
                    DisplayDataSegment_1 = new DisplayDataSegment_1(7);
                    DisplayDataSegment_1.AddRecordField(DisplayDataSegment_1.RecordType);
                    DisplayDataSegment_1.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 1 });
                    DisplayDataSegment_1.PatientID = new StringFieldInfo(4, 20);
                    DisplayDataSegment_1.AddRecordField(DisplayDataSegment_1.PatientID);

                    //DSR-2
                    DisplayDataSegment_2 = new DisplayDataSegment_2(7);
                    DisplayDataSegment_2.AddRecordField(DisplayDataSegment_2.RecordType);
                    DisplayDataSegment_2.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 2 });
                    DisplayDataSegment_2.BED = new StringFieldInfo(4, 20) { DefaultValue = "27" };
                    DisplayDataSegment_2.AddRecordField(DisplayDataSegment_2.BED);

                    //DSR-3
                    DisplayDataSegment_3 = new DisplayDataSegment_3(7);
                    DisplayDataSegment_3.AddRecordField(DisplayDataSegment_3.RecordType);
                    DisplayDataSegment_3.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 3 });
                    DisplayDataSegment_3.PatientName = new StringFieldInfo(4, 200);
                    DisplayDataSegment_3.AddRecordField(DisplayDataSegment_3.PatientName);

                    //DSR-4
                    DisplayDataSegment_4 = new DisplayDataSegment_4(7);
                    DisplayDataSegment_4.AddRecordField(DisplayDataSegment_4.RecordType);
                    DisplayDataSegment_4.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 4 });
                    DisplayDataSegment_4.DateOfBirth = new DateFieldInfo(4, dateFormat: "yyyyMMddHHmmss");
                    DisplayDataSegment_4.AddRecordField(DisplayDataSegment_4.DateOfBirth);

                    //DSR-5
                    DisplayDataSegment_5 = new DisplayDataSegment_5(7);
                    DisplayDataSegment_5.AddRecordField(DisplayDataSegment_5.RecordType);
                    DisplayDataSegment_5.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 5 });
                    DisplayDataSegment_5.Gender = new StringFieldInfo(4, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    DisplayDataSegment_5.AddRecordField(DisplayDataSegment_5.Gender);

                    //DSR-6
                    DisplayDataSegment_6 = new DisplayDataSegment_6(7);
                    DisplayDataSegment_6.AddRecordField(DisplayDataSegment_6.RecordType);
                    DisplayDataSegment_6.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 6 });
                    DisplayDataSegment_6.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "O" });

                    //DSR-7
                    DisplayDataSegment_7 = new DisplayDataSegment_7(7);
                    DisplayDataSegment_7.AddRecordField(DisplayDataSegment_7.RecordType);
                    DisplayDataSegment_7.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 7 });

                    //DSR-8
                    DisplayDataSegment_8 = new DisplayDataSegment_8(7);
                    DisplayDataSegment_8.AddRecordField(DisplayDataSegment_8.RecordType);
                    DisplayDataSegment_8.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 8 });

                    //DSR-9
                    DisplayDataSegment_9 = new DisplayDataSegment_9(7);
                    DisplayDataSegment_9.AddRecordField(DisplayDataSegment_9.RecordType);
                    DisplayDataSegment_9.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 9 });

                    //DSR-10
                    DisplayDataSegment_10 = new DisplayDataSegment_10(7);
                    DisplayDataSegment_10.AddRecordField(DisplayDataSegment_10.RecordType);
                    DisplayDataSegment_10.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 10 });

                    //DSR-11
                    DisplayDataSegment_11 = new DisplayDataSegment_11(7);
                    DisplayDataSegment_11.AddRecordField(DisplayDataSegment_11.RecordType);
                    DisplayDataSegment_11.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 11 });

                    //DSR-12
                    DisplayDataSegment_12 = new DisplayDataSegment_12(7);
                    DisplayDataSegment_12.AddRecordField(DisplayDataSegment_12.RecordType);
                    DisplayDataSegment_12.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 12 });

                    //DSR-13
                    DisplayDataSegment_13 = new DisplayDataSegment_13(7);
                    DisplayDataSegment_13.AddRecordField(DisplayDataSegment_13.RecordType);
                    DisplayDataSegment_13.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 13 });

                    //DSR-14
                    DisplayDataSegment_14 = new DisplayDataSegment_14(7);
                    DisplayDataSegment_14.AddRecordField(DisplayDataSegment_14.RecordType);
                    DisplayDataSegment_14.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 14 });

                    //DSR-15
                    DisplayDataSegment_15 = new DisplayDataSegment_15(7);
                    DisplayDataSegment_15.AddRecordField(DisplayDataSegment_15.RecordType);
                    DisplayDataSegment_15.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 15 });
                    DisplayDataSegment_15.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "outpatient" });

                    //DSR-16
                    DisplayDataSegment_16 = new DisplayDataSegment_16(7);
                    DisplayDataSegment_16.AddRecordField(DisplayDataSegment_16.RecordType);
                    DisplayDataSegment_16.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 16 });

                    //DSR-17
                    DisplayDataSegment_17 = new DisplayDataSegment_17(7);
                    DisplayDataSegment_17.AddRecordField(DisplayDataSegment_17.RecordType);
                    DisplayDataSegment_17.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 17 });
                    DisplayDataSegment_17.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "own" });

                    //DSR-18
                    DisplayDataSegment_18 = new DisplayDataSegment_18(7);
                    DisplayDataSegment_18.AddRecordField(DisplayDataSegment_18.RecordType);
                    DisplayDataSegment_18.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 18 });

                    //DSR-19
                    DisplayDataSegment_19 = new DisplayDataSegment_19(7);
                    DisplayDataSegment_19.AddRecordField(DisplayDataSegment_19.RecordType);
                    DisplayDataSegment_19.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 19 });

                    //DSR-20
                    DisplayDataSegment_20 = new DisplayDataSegment_20(7);
                    DisplayDataSegment_20.AddRecordField(DisplayDataSegment_20.RecordType);
                    DisplayDataSegment_20.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 20 });

                    //DSR-21
                    DisplayDataSegment_21 = new DisplayDataSegment_21(7);
                    DisplayDataSegment_21.AddRecordField(DisplayDataSegment_21.RecordType);
                    DisplayDataSegment_21.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 21 });
                    DisplayDataSegment_21.SampleID = new StringFieldInfo(4, 10);
                    DisplayDataSegment_21.AddRecordField(DisplayDataSegment_21.SampleID);

                    //DSR-22
                    DisplayDataSegment_22 = new DisplayDataSegment_22(7);
                    DisplayDataSegment_22.AddRecordField(DisplayDataSegment_22.RecordType);
                    DisplayDataSegment_22.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 22 });
                    DisplayDataSegment_22.SampleID = new StringFieldInfo(4, 10);
                    DisplayDataSegment_22.AddRecordField(DisplayDataSegment_22.SampleID);

                    //DSR-23
                    DisplayDataSegment_23 = new DisplayDataSegment_23(7);
                    DisplayDataSegment_23.AddRecordField(DisplayDataSegment_23.RecordType);
                    DisplayDataSegment_23.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 23 });
                    DisplayDataSegment_23.SCT = new DateFieldInfo(4, "yyyyMMddHHmmss");
                    DisplayDataSegment_23.AddRecordField(DisplayDataSegment_23.SCT);

                    //DSR-24
                    DisplayDataSegment_24 = new DisplayDataSegment_24(7);
                    DisplayDataSegment_24.AddRecordField(DisplayDataSegment_24.RecordType);
                    DisplayDataSegment_24.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 24 });
                    DisplayDataSegment_24.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "N" });

                    //DSR-25
                    DisplayDataSegment_25 = new DisplayDataSegment_25(7);
                    DisplayDataSegment_25.AddRecordField(DisplayDataSegment_25.RecordType);
                    DisplayDataSegment_25.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 25 });
                    DisplayDataSegment_25.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "1" });

                    //DSR-26
                    DisplayDataSegment_26 = new DisplayDataSegment_26(7);
                    DisplayDataSegment_26.AddRecordField(DisplayDataSegment_26.RecordType);
                    DisplayDataSegment_26.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 26 });
                    DisplayDataSegment_26.SampleType = new StringFieldInfo(4, 50) { DefaultValue = "SERUM" };
                    DisplayDataSegment_26.AddRecordField(DisplayDataSegment_26.SampleType);

                    //DSR-27
                    DisplayDataSegment_27 = new DisplayDataSegment_27(7);
                    DisplayDataSegment_27.AddRecordField(DisplayDataSegment_27.RecordType);
                    DisplayDataSegment_27.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 27 });

                    //DSR-28
                    DisplayDataSegment_28 = new DisplayDataSegment_28(7);
                    DisplayDataSegment_28.AddRecordField(DisplayDataSegment_28.RecordType);
                    DisplayDataSegment_28.AddRecordField(new NumericFieldInfo(2, 0) { DefaultValue = 28 });


                    //DSR- TEST LIST from 29
                    DisplayDataSegment_TestCode = new DisplayDataSegment_TestCode(7);
                    DisplayDataSegment_TestCode.AddRecordField(DisplayDataSegment_TestCode.RecordType);
                    DisplayDataSegment_TestCode.AddRecordField(DisplayDataSegment_TestCode.SequenceNumber);
                    DisplayDataSegment_TestCode.AddRecordField(new RecordFieldInfo(4) { ComponentCount = 4 });
                    DisplayDataSegment_TestCode.TestID_ManufacturersTestCode = new StringFieldInfo(4, 200) { ComponentNumber = 1 };
                    DisplayDataSegment_TestCode.AddRecordField(DisplayDataSegment_TestCode.TestID_ManufacturersTestCode);

                    //DSC
                    ContinuationPointerSegment = new ContinuationPointerSegment(3);
                    ContinuationPointerSegment.AddRecordField(ContinuationPointerSegment.RecordType);

                    //Message Acknowledgment Segment(MSA)
                    OrderRecordInfoHL7MSA = new OrderRecordInfoHL7MSA(8);
                    OrderRecordInfoHL7MSA.AddRecordField(OrderRecordInfoHL7MSA.RecordType);
                    OrderRecordInfoHL7MSA.ActionCode = new StringFieldInfo(2, 2) { DefaultValue = "AA" };
                    OrderRecordInfoHL7MSA.AddRecordField(OrderRecordInfoHL7MSA.ActionCode);
                    OrderRecordInfoHL7MSA.MsgControlid = new StringFieldInfo(3, 20) { CopyKey_Outgoing = "GUID" };//Message Control ID
                    OrderRecordInfoHL7MSA.AddRecordField(OrderRecordInfoHL7MSA.MsgControlid);
                    OrderRecordInfoHL7MSA.AddRecordField(new RecordFieldInfo(4) { DefaultValue = "Message Accepted" });

                    //Qc Logic for Zybio EXZ 6000 H6
                    QcResultRecordInfo = new QcResultRecordInfo(49);
                    QcResultRecordInfo.AddRecordField(QcResultRecordInfo.RecordType);
                    QcResultRecordInfo.AddRecordField(QcResultRecordInfo.SequenceNumber);
                    QcResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 200);
                    QcResultRecordInfo.AddRecordField(QcResultRecordInfo.TestID_ManufacturersTestCode);

                    break;
                #endregion

                case 157:
                    #region SNIBE ASTM - Maglumi X8
                    IsFieldSizeInBytes = false;
                    FieldDelimiter = '|';
                    ComponentDelimiter = '^';
                    RepeatDelimiter = '\\';
                    HeaderAndTerminationRecordRequired = true;

                    HeaderRecordInfo = new HeaderRecordInfo(14);
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.RecordType);
                    HeaderRecordInfo.SequenceNumber.FieldNumber = 0;
                    HeaderRecordInfo.DelimiterCharacters = new StringFieldInfo(2, 4) { DefaultValue = @"|\^&" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.DelimiterCharacters);
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(3) { DefaultValue = "PSWD" });
                    HeaderRecordInfo.SenderID = new StringFieldInfo(5, 10) { DefaultValue = "Maglumi 4000 Plus" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.SenderID);
                    HeaderRecordInfo.ReceiverID = new StringFieldInfo(10, 10) { DefaultValue = "LIS" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.ReceiverID);
                    HeaderRecordInfo.AddRecordField(new RecordFieldInfo(12) { DefaultValue = "P" });
                    HeaderRecordInfo.VersionNumber = new StringFieldInfo(13, 10) { DefaultValue = "E1394-97" };
                    HeaderRecordInfo.AddRecordField(HeaderRecordInfo.VersionNumber);
                    HeaderRecordInfo.AddRecordField(new DateFieldInfo(14, "YYYYMMDD") { IsCurrentDateTime = true });

                    PatientRecordInfo = new PatientRecordInfo(35);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.RecordType);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.SequenceNumber);
                    PatientRecordInfo.PatientName = new StringFieldInfo(6, 30);
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.PatientName);
                    PatientRecordInfo.Gender = new StringFieldInfo(9, 1) { ValidValues = new List<object> { "M", "F", "U" } };
                    PatientRecordInfo.AddRecordField(PatientRecordInfo.Gender);

                    OrderRecordInfo = new OrderRecordInfo(31);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.RecordType);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SequenceNumber);
                    OrderRecordInfo.SampleID = new StringFieldInfo(3, 22);
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.SampleID);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(5) { ComponentCount = 4 });
                    OrderRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 30) { ComponentNumber = 4 };
                    OrderRecordInfo.AddRecordField(OrderRecordInfo.TestID_ManufacturersTestCode);
                    OrderRecordInfo.AddRecordField(new RecordFieldInfo(6) { DefaultValue = "R" });

                    ResultRecordInfo = new ResultRecordInfo(14);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.RecordType);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.SequenceNumber);
                    ResultRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 4 });
                    ResultRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(3, 10) { ComponentNumber = 4 };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestID_ManufacturersTestCode);
                    ResultRecordInfo.TestValue = new NumericFieldInfo(4, 12) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.TestValue);
                    ResultRecordInfo.Units = new StringFieldInfo(5, 10);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.Units);
                    ResultRecordInfo.ResultAbnormalFlags = new StringFieldInfo(7, 1);
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultAbnormalFlags);
                    ResultRecordInfo.ResultedDateTime = new DateFieldInfo(13) { IsMandatory = true };
                    ResultRecordInfo.AddRecordField(ResultRecordInfo.ResultedDateTime);

                    QueryRecordInfo = new QueryRecordInfo(13);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.RecordType);
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SequenceNumber);
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(3) { ComponentCount = 2 });
                    QueryRecordInfo.SampleID = new StringFieldInfo(3, 22) { ComponentNumber = 2 };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.SampleID);
                    QueryRecordInfo.AddRecordField(new RecordFieldInfo(5));
                    QueryRecordInfo.TestID_ManufacturersTestCode = new StringFieldInfo(5, 10) { DefaultValue = "ALL" };
                    QueryRecordInfo.AddRecordField(QueryRecordInfo.TestID_ManufacturersTestCode);
                    QueryRecordInfo.AddRecordField(new StringFieldInfo(13, 1) { DefaultValue = "O" });

                    TerminationRecordInfo = new TerminationRecordInfo(3);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.RecordType);
                    TerminationRecordInfo.AddRecordField(TerminationRecordInfo.SequenceNumber);
                    TerminationRecordInfo.AddRecordField(new StringFieldInfo(3, 1) { DefaultValue = "N" });
                    #endregion
                    break;

                default:
                    throw new NotImplementedException("LIS IS NOT AVAILABLE FOR THIS MACHINE.");
            }
            #endregion

        }
    }

    #region RecordInfo
    [Serializable]
    class HeaderRecordInfo : RecordInfo
    {
        public HeaderRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "H";
        }
    }

    [Serializable]
    class HeaderRecordInfoHL7 : RecordInfo
    {
        public HeaderRecordInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }


    [Serializable]
    class HeaderRecordInfoHL7ACK : RecordInfo
    {
        public HeaderRecordInfoHL7ACK(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }

    [Serializable]
    class HeaderRecordInfoHL7ACKResponse : RecordInfo
    {
        public HeaderRecordInfoHL7ACKResponse(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }
    //HeaderRecordInfoHL7ACKResponseOrder
    //ResultRecordInfoHL7ACKResults

    [Serializable]
    class ResultRecordInfoHL7ACKResults : RecordInfo
    {
        public ResultRecordInfoHL7ACKResults(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }


    [Serializable]
    class HeaderRecordInfoHL7ACKResponseOrder : RecordInfo
    {
        public HeaderRecordInfoHL7ACKResponseOrder(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }
    //HeaderRecordInfoHL7ACKMSATEST
    [Serializable]
    class HeaderRecordInfoHL7ACKMSATEST : RecordInfo
    {
        public HeaderRecordInfoHL7ACKMSATEST(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }
    //HeaderRecordInfoHL7ACKMSAReqToResults

    [Serializable]
    class HeaderRecordInfoHL7ACKMSAReqToResults : RecordInfo
    {
        public HeaderRecordInfoHL7ACKMSAReqToResults(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSH";
        }
    }

    [Serializable]
    class QueryRecordInfo : RecordInfo
    {
        public QueryRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "Q";
        }
    }

    [Serializable]
    class QueryRecordInfoHL7 : RecordInfo
    {
        public QueryRecordInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QPD";
        }
    }

    [Serializable]
    class ResponseControlInfoHL7 : RecordInfo
    {
        public ResponseControlInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "RCP";
        }
    }


    [Serializable]
    class QueryRecordInfoHL7Res : RecordInfo
    {
        public QueryRecordInfoHL7Res(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QPD";
        }
    }

    [Serializable]
    class PatientRecordInfo : RecordInfo
    {
        public PatientRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "P";
        }
    }

    [Serializable]
    class PatientRecordInfoHL7 : RecordInfo
    {
        public PatientRecordInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "PID";
        }
    }


    [Serializable]
    class PatientVisitInfoHL7 : RecordInfo
    {
        public PatientVisitInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "PV1";
        }
    }

    [Serializable]
    class OrderRecordInfo : RecordInfo
    {
        public OrderRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "O";
        }
    }

    // HL7 Code Block
    [Serializable]
    class OrderRecordInfoHL7SPM : RecordInfo
    {
        public OrderRecordInfoHL7SPM(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "SPM";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7SAC : RecordInfo
    {
        public OrderRecordInfoHL7SAC(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "SAC";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7ORC : RecordInfo
    {
        public OrderRecordInfoHL7ORC(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "ORC";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7TQ1 : RecordInfo
    {
        public OrderRecordInfoHL7TQ1(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "TQ1";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7OBR : RecordInfo
    {
        public OrderRecordInfoHL7OBR(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "OBR";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7TCD : RecordInfo
    {
        public OrderRecordInfoHL7TCD(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "TCD";
        }
    }


    [Serializable]
    class OrderRecordInfoHL7MSA : RecordInfo
    {
        public OrderRecordInfoHL7MSA(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSA";
        }
    }

    //OrderRecordInfoHL7MSAResults

    [Serializable]
    class OrderRecordInfoHL7MSAResults : RecordInfo
    {
        public OrderRecordInfoHL7MSAResults(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "MSA";
        }
    }

    [Serializable]
    class OrderRecordInfoHL7QAK : RecordInfo
    {
        public OrderRecordInfoHL7QAK(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QAK";
        }
    }

    // HL7 Code block ended

    [Serializable]
    class ResultRecordInfo : RecordInfo
    {
        public ResultRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "R";
        }
    }

    [Serializable]
    class ResultRecordInfoHL7 : RecordInfo
    {
        public ResultRecordInfoHL7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "OBX";
        }
    }

    //ResultRecordInfoHL7ACK
    [Serializable]
    class ResultRecordInfoHL7ACK : RecordInfo
    {
        public ResultRecordInfoHL7ACK(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "OBX";
        }
    }

    //Query Definition Segment
    [Serializable]
    class QueryDefinitionSegmentFromMachine : RecordInfo
    {
        public QueryDefinitionSegmentFromMachine(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QRD";
        }
    }

    [Serializable]
    class QueryDefinitionSegmentFromLis : RecordInfo
    {
        public QueryDefinitionSegmentFromLis(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QRD";
        }
    }

    // Query Filter Segment
    [Serializable]
    class QueryFilterSegment : RecordInfo
    {
        public QueryFilterSegment(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QRF";
        }
    }

    //Error Segment
    [Serializable]
    class ErrorSegment : RecordInfo
    {
        public ErrorSegment(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue= "ERR";
        }
    }

    //Query Acknowledgment Segment
    [Serializable]
    class QueryAcknowledgmentSegment : RecordInfo
    {
        public QueryAcknowledgmentSegment(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "QAK";
        }
    }

    //Display Data Segment
    #region DisplayDataSegment

    [Serializable]
    class DisplayDataSegment_1 : RecordInfo
    {
        public DisplayDataSegment_1(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_2 : RecordInfo
    {
        public DisplayDataSegment_2(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_3 : RecordInfo
    {
        public DisplayDataSegment_3(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_4 : RecordInfo
    {
        public DisplayDataSegment_4(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_5 : RecordInfo
    {
        public DisplayDataSegment_5(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_6 : RecordInfo
    {
        public DisplayDataSegment_6(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_7 : RecordInfo
    {
        public DisplayDataSegment_7(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_8 : RecordInfo
    {
        public DisplayDataSegment_8(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_9 : RecordInfo
    {
        public DisplayDataSegment_9(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_10 : RecordInfo
    {
        public DisplayDataSegment_10(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_11 : RecordInfo
    {
        public DisplayDataSegment_11(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_12 : RecordInfo
    {
        public DisplayDataSegment_12(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_13 : RecordInfo
    {
        public DisplayDataSegment_13(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_14 : RecordInfo
    {
        public DisplayDataSegment_14(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_15 : RecordInfo
    {
        public DisplayDataSegment_15(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_16 : RecordInfo
    {
        public DisplayDataSegment_16(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_17 : RecordInfo
    {
        public DisplayDataSegment_17(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_18 : RecordInfo
    {
        public DisplayDataSegment_18(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_19 : RecordInfo
    {
        public DisplayDataSegment_19(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_20 : RecordInfo
    {
        public DisplayDataSegment_20(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_21 : RecordInfo
    {
        public DisplayDataSegment_21(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_22 : RecordInfo
    {
        public DisplayDataSegment_22(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_23 : RecordInfo
    {
        public DisplayDataSegment_23(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_24 : RecordInfo
    {
        public DisplayDataSegment_24(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_25 : RecordInfo
    {
        public DisplayDataSegment_25(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_26 : RecordInfo
    {
        public DisplayDataSegment_26(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_27 : RecordInfo
    {
        public DisplayDataSegment_27(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_28 : RecordInfo
    {
        public DisplayDataSegment_28(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    [Serializable]
    class DisplayDataSegment_TestCode : RecordInfo
    {
        public DisplayDataSegment_TestCode(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSP";
        }
    }

    #endregion

    //Continuation Pointer Segment
    [Serializable]
    class ContinuationPointerSegment : RecordInfo
    {
        public ContinuationPointerSegment(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "DSC";
        }
    }

    [Serializable]
    class QcResultRecordInfo : ResultRecordInfo
    {
        public QcResultRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "R";
        }
    }

    [Serializable]
    class CommentRecordInfo : RecordInfo
    {
        public CommentRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "C";
        }
    }

    [Serializable]
    class TerminationRecordInfo : RecordInfo
    {
        public TerminationRecordInfo(int fieldCount) : base(fieldCount)
        {
            RecordType.DefaultValue = "L";
        }
    }

    [Serializable]
    class RecordInfo
    {
        public int FieldCount { get; private set; }
        //public string RecordTypeCharacter { get; set; }
        //private int sequenceNumberFieldNumber = -1;
        //public int SequenceNumberFieldNumber
        //{
        //    get { return sequenceNumberFieldNumber; }
        //    //settting minus 1, as the number is set in 1 based, but c# arrays require 0 based indexes    
        //    set { sequenceNumberFieldNumber = value - 1; }
        //}
        //public Dictionary<int, int> ComponentCounts = new Dictionary<int, int>();

        public StringFieldInfo RecordType { get; set; }
        public NumericFieldInfo SequenceNumber { get; set; }
        public NumericFieldInfo BlockNumber { get; set; }
        public StringFieldInfo InstrumentIdKey { get; set; }

        public StringFieldInfo DelimiterCharacters = null;
        public StringFieldInfo SenderID = null;
        public StringFieldInfo ReceiverID = null;
        public StringFieldInfo ProcessingID = null;
        public StringFieldInfo VersionNumber = null;
        public StringFieldInfo MsgIdentifier = null;
        public StringFieldInfo MsgControlid = null;
        public StringFieldInfo ReservedField = null;
        public StringFieldInfo CommentOrSpecialInstructions = null;

        public NumericFieldInfo NumberOfInquirySamplesInBlock = null;
        public StringFieldInfo QueryStatusCode = null;

        public DateFieldInfo SDate = null;
        public StringFieldInfo PatientID = null;
        public StringFieldInfo PatientName = null;
        public StringFieldInfo Department = null;
        public StringFieldInfo Area = null;
        public StringFieldInfo BED = null;
        public DateFieldInfo DateOfBirth = null;
        public StringFieldInfo Gender = null;
        public NumericFieldInfo Age = null;
        public DateFieldInfo SCT = null;
        public DateFieldInfo BVT = null;
        public StringFieldInfo RefDr = null;
        public StringFieldInfo SCP = null;
        public StringFieldInfo ClientCode = null;//SourceCode

        public StringFieldInfo SampleID = null;
        public StringFieldInfo SampleNumberAttribute = null;
        public RecordFieldInfo TestID_ManufacturersTestCode = null;
        public RecordFieldInfo TestID_ManufacturersTestCode2 = null;
        public RecordFieldInfo DualTestID_ManufacturersTestCode = null;
        public NumericFieldInfo RackNo = null;
        public StringFieldInfo RackNoHL7 = null;
        public NumericFieldInfo RackPosition = null;
        public StringFieldInfo ActionCode = null;
        public StringFieldInfo SampleType = null;
        public StringFieldInfo ReportType = null;
        public StringFieldInfo SortingRule = null;
        public StringFieldInfo BinNumber = null;

        public StringFieldInfo Resulttype = null;
        public StringFieldInfo ResultAspects = null;
        public RecordFieldInfo TestValue = null;
        public StringFieldInfo Units = null;
        public StringFieldInfo ResultAbnormalFlags = null;
        public DateFieldInfo ResultedDateTime = null;
        public DateFieldInfo ResultedTime = null;
        public NumericFieldInfo TotalNumberOfBlocks = null;
        public NumericFieldInfo NumberOfTestsInBlock = null;
        public RecordFieldInfo RequestTest = null;

        public StringFieldInfo CommentCode = null;
        public StringFieldInfo CommentText = null;
        public StringFieldInfo CommentText_CMN = null;

        public StringFieldInfo TerminationCode = null;
        public NumericFieldInfo Labcode = null;
        public StringFieldInfo LabcodeWithDate = null;
        public NumericFieldInfo BarcodeCount = null;
        public NumericFieldInfo TestCount = null;
        public NumericFieldInfo RecordCount = null;
        public StringFieldInfo Stat = null;

        public List<RecordFieldInfo> RecordFields
        {
            get;
            set;
        }


        public RecordInfo(int fieldCount)
        {
            FieldCount = fieldCount;
            RecordFields = new List<RecordFieldInfo>(FieldCount);
            for (int i = 0; i < FieldCount; i++)
            {
                RecordFields.Add(new RecordFieldInfo(i + 1));
            }

            RecordType = new StringFieldInfo(1, 1);
            SequenceNumber = new NumericFieldInfo(2, 0);
        }

        public void AddRecordField(RecordFieldInfo rfi/*, bool isFieldSizeInBytes*/)
        {
            if (rfi.FieldNumber <= -1)
            {
                throw new Exception("FieldNumber is not set.");
            }

            //if (isFieldSizeInBytes && rfi.BytesLength <= 0)
            //{
            //    throw new Exception("BytesLength not set");
            //}

            if (rfi.ComponentNumber > -1)
            {
                if (RecordFields[rfi.FieldNumber] == null)
                    RecordFields[rfi.FieldNumber] = new RecordFieldInfo(rfi.FieldNumber);

                if ((rfi.ComponentNumber) > RecordFields[rfi.FieldNumber].ComponentCount - 1)
                    throw new MyException("Component Number cannot be greater than Component Count.");

                //if (RecordFields[rfi.FieldNumber].Components.ElementAtOrDefault(rfi.ComponentNumber) != null)
                //    throw new Exception("RecordField component already added at position " + (rfi.ComponentNumber + 1));


                //if (RecordFields[rfi.FieldNumber].Components[rfi.ComponentNumber] == null)
                RecordFields[rfi.FieldNumber].Components[rfi.ComponentNumber] = rfi;
                //else
                //    throw new Exception("Component definition is already added in Field No " + rfi.FieldNumber + ", Component No " + rfi.ComponentNumber);
            }
            else
            {
                //if (RecordFields.ElementAtOrDefault(rfi.FieldNumber) != null)
                //    throw new Exception("RecordField already added at position " + (rfi.FieldNumber + 1));

                //RecordFields.Insert(rfi.FieldNumber, rfi);

                //if (RecordFields[rfi.FieldNumber] == null)
                RecordFields[rfi.FieldNumber] = rfi;
                //else
                //    throw new Exception("Field definition is already added in Field No " + rfi.FieldNumber);
            }
        }

        public string PrepareString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (RecordFieldInfo rfi in RecordFields.Where(r => r.FieldNumber > -1).OrderBy(r => r.FieldNumber))
            {
                throw new NotImplementedException();
            }
            return sb.ToString();
        }

        public RecordInfo Copy()
        {
            RecordInfo riCopy = (RecordInfo)InterfaceHelper.CopyObject(this);

            ////clean current object for reuse
            //foreach(RecordFieldInfo rfi in ri.RecordFields)
            //{
            //    rfi.Value = null;

            //    foreach(RecordFieldInfo rfiComp in rfi.Components)
            //    {
            //        rfiComp.Value = null;
            //    }
            //}

            return riCopy;
        }
    }
    #endregion

    #region FieldInfo
    [Serializable]
    class StringFieldInfo : RecordFieldInfo
    {
        public int MaxLength { get; set; }
        public string CopyKey_Incoming { get; set; }
        public string CopyKey_Outgoing { get; set; }

        public StringFieldInfo(int fieldNumber, int maxLength) : base(fieldNumber)
        {
            MaxLength = maxLength;
            DefaultValue = "";
            IsRightJustified = false;
            EmptySpaceFillerChar = ' ';
        }
    }
    [Serializable]
    class DateFieldInfo : RecordFieldInfo
    {
        public string DateFormat { get; set; }
        public bool IsCurrentDateTime { get; set; }

        public DateFieldInfo(int fieldNumber, string dateFormat = "yyyyMMddHHmmss") : base(fieldNumber)
        {
            DateFormat = dateFormat;
            EmptySpaceFillerChar = ' ';
            IsCurrentDateTime = false;
        }
    }

    [Serializable]
    class NumericFieldInfo : RecordFieldInfo
    {
        public int DecimalPlaces = 0;
        public int MinValue = int.MinValue;
        public int MaxValue = int.MaxValue;
        public NumericFieldInfo(int fieldNumber, int decimalPlaces) : base(fieldNumber)
        {
            DecimalPlaces = decimalPlaces;
            IsRightJustified = true;
            EmptySpaceFillerChar = '0';
        }
    }

    [Serializable]
    class RecordFieldInfo
    {
        private int fieldNumber = -1;
        //FieldNumber to be set to -1, if it is not present in record
        public int FieldNumber
        {
            get { return fieldNumber; }
            //settting minus 1, as the number is set in 1 based, but c# arrays require 0 based indexes
            set
            {
                fieldNumber = value - 1;
                FieldNumber_Incoming = value;
            }
        }
        private int fieldNumber_Incoming = -1;
        public int FieldNumber_Incoming
        {
            get { return fieldNumber_Incoming; }
            //settting minus 1, as the number is set in 1 based, but c# arrays require 0 based indexes
            set
            {
                fieldNumber_Incoming = value - 1;
            }
        }
        private int componentCount = 0;
        public int ComponentCount
        {
            get
            {
                return componentCount;
            }
            set
            {
                componentCount = value;
                Components = new List<RecordFieldInfo>(componentCount);
                //add empty components
                for (int i = 0; i < componentCount; i++)
                {
                    Components.Add(new RecordFieldInfo(this.FieldNumber + 1) { ComponentNumber = i + 1 });
                }
            }
        }
        //public string[] Components { get; set; }
        //private int componentCount = 0;
        //public int ComponentCount
        //{
        //    get { return componentCount; }
        //    set
        //    {
        //        componentCount = value;
        //        Components = new string[componentCount];
        //    }
        //}

        private int componentNumber = -1;
        public int ComponentNumber
        {
            get { return componentNumber; }
            //settting minus 1, as the number is set in 1 based, but c# arrays require 0 based indexes
            set
            {
                //if ((value - 1) > ComponentCount - 1)
                //    throw new MyException("Component Number cannot be greater than Component Count.");

                componentNumber = value - 1;
            }
        }
        public List<RecordFieldInfo> Components { get; private set; }
        public bool IsMandatory { get; set; }
        public object DefaultValue { get; set; }
        public List<object> ValidValues = new List<object>();
        public object Value { get; set; }
        public RecordFieldInfo CopyFieldFrom { get; set; }
        public string ValidationRegEx { get; set; }
        //child field's SupportsMultipleValues value must be same as its parent
        private bool supportsMultipleValues = false;
        public bool SupportsMultipleValues
        {
            get { return supportsMultipleValues; }
            set
            {
                supportsMultipleValues = value;
                if (ComponentCount > 0)
                {
                    foreach (RecordFieldInfo rfiComp in this.Components)
                    {
                        rfiComp.SupportsMultipleValues = true;
                    }
                }
            }
        }
        public int BytesLength { get; set; }
        public int BytesLength_PerComponentSet { get; set; }
        public bool IsRightJustified { get; set; }
        public char EmptySpaceFillerChar { get; set; }
        public bool IsFixedLength { get; set; }
        public bool TrimTextWhileExtracting { get; set; }
        public int ForwardSlashSeparatedComponentNumber { get; set; }

        public RecordFieldInfo(int fieldNumber)
        {
            FieldNumber = fieldNumber;
            EmptySpaceFillerChar = ' ';
            TrimTextWhileExtracting = true;
        }
    }
    #endregion
}
