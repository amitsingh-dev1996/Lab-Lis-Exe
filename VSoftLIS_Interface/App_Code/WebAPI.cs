using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VSoftLIS_Interface.Common;
using VSoftLIS_Interface.BLL;
using System.Net;
using System.Security.Policy;

namespace VSoftLIS_Interface
{
    public class WebAPI
    {
        public static bool IsVSoftApiCommunication
        {
            get { return !Program.AnalyzerConfiguration.ConnectionSettings.IsCharbiConnected; }
        }
        public static string VSoftLisApiBaseUrl { get { return CommonSettings.VSoftLisApiBaseUrl; } }
        //public static string VSoftApiBaseUrl { get { return CommonSettings.VSoftApiBaseUrl; } }
        public static string VSoftApiBaseUrl_STAGING { get { return "http://VSoftstng.southindia.cloudapp.azure.com/api"; } }
        public static bool IsVSoftConnectionSucceeded { get; set; }

        public Analyzer GetAnalyzerDetailsById(int analyzerId)
        {

            Analyzer analyzer = new Analyzer();
            analyzer = PostApi(VSoftLisApiBaseUrl, "api/Fetchinstrumentinfo/" + analyzerId, new object(), analyzer, isGetRequest: true, TimeoutMilliSeconds: 5 * 1000) as Analyzer;

            return analyzer;

        }

        public static worklist GetWorklist(sampleList slBarcodeRequest, int TimeoutMilliseconds = 0)
        {
            worklist wList = new worklist();
            //return PostApi(VSoftLisApiBaseUrl, "Interface/worklist", slBarcodeRequest, wList, TimeoutMilliSeconds: TimeoutMilliseconds) as worklist;
            return PostApi(VSoftLisApiBaseUrl, "api/FetchSampleList", slBarcodeRequest, wList, TimeoutMilliSeconds: TimeoutMilliseconds) as worklist;
        }

        public static ApiResponse TagWorkList(TagWOTest tagWOTest)
        {
            ApiResponse apiResponse = new ApiResponse();
            // return PostApi(VSoftLisApiBaseUrl, "Interface/TagWorkList", tagWOTest, apiResponse) as ApiResponse;
            return null;
        }

        public static worklist GetBinInfo(int analyzerId, string barcode, string sortingRule)
        {
            worklist wList = new worklist();
            BinInfo binInfo = new BinInfo();
            //binInfo = PostApi(VSoftLisApiBaseUrl, "LIS/BinByBarcode/" + barcode + "/" + analyzerId + "/" + sortingRule, new object(), binInfo, isGetRequest: true) as BinInfo;

            //wList.barcodeList = new List<BarcodeList> { new BarcodeList() { Barcode = barcode, BinInfo = binInfo } };
            //return wList;
            return null;
        }

        //public static ApiResponse UpdateResult(TestResult testResults/*, bool IsReattempt = false*/, int TimeoutSeconds = 60)
        //{
        //    ApiResponse resultResponse = new ApiResponse();
        //    return PostApi(VSoftLisApiBaseUrl, "PostInstrumentresult", testResults, resultResponse, TimeoutMilliSeconds: TimeoutSeconds * 1000) as ApiResponse;
        //}

        public static UpdateResultOutput UpdateResult(InstrumentResult testResults/*, bool IsReattempt = false*/, int TimeoutSeconds = 60)
        {
            UpdateResultOutput resultResponse = new UpdateResultOutput();
            return PostApi(VSoftLisApiBaseUrl, "api/PostInstrumentresult", testResults, resultResponse, TimeoutMilliSeconds: TimeoutSeconds * 1000) as UpdateResultOutput;
        }

        public static List<TestListItem> GetTestList(int Id)
        {
            try
            {
                List<TestListItem> testList = new List<TestListItem>();
                return PostApi(VSoftLisApiBaseUrl, "api/Fetchinstrumenttests/" + Id, new object(), testList, isGetRequest: true) as List<TestListItem>;
                //return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static QualitativeProductList GetDescriptiveResultMaster(int AnalyzerId)
        {
            //if (IsVsoftApiCommunication)
            //{
            QualitativeProductList DescriptiveResultMaster = new QualitativeProductList();
            return PostApi(VSoftLisApiBaseUrl, "api/FetchQualitativeResults/" + AnalyzerId, new object(), DescriptiveResultMaster, isGetRequest: true) as QualitativeProductList;
            //return null;
            //}
            //else
              //  return null;
        }


        public static QualitativeProductList SetDescriptiveResultMaster(int AnalyzerId)
        {
            //if (IsVsoftApiCommunication)
            //{
            QualitativeProductList DescriptiveResultMaster = new QualitativeProductList();
            return PostApi(VSoftLisApiBaseUrl, "api/FetchQualitativeResults/" + AnalyzerId, new object(), DescriptiveResultMaster, isGetRequest: true) as QualitativeProductList;
            //}
            //else
            //    return null;
        }

        public static SampleArchivalResponse UpdateArchiveSample(SampleArchivalInput Input)
        {
            SampleArchivalResponse resultResponse = new SampleArchivalResponse();
            //return PostApi(VSoftLisApiBaseUrl, "UpdateDetails/ArchiveSample", Input, resultResponse) as SampleArchivalResponse;
            return null;
        }
        public static void InsertCommunicationLog(int analyzerId, List<tbl_Communication_Log> communicationLogs)
        {
            string strResponse = "";
            //var objInput = new { Analyzer_Id = analyzerId, communicationLogs };
            communicationLogs.ForEach(r => r.Analyzer_Id = analyzerId);
            // PostApi(VSoftLisApiBaseUrl, "CommunicationLog", communicationLogs, strResponse);
           
        }
        public static void InsertErrorLog(tbl_Error_Log errorLog)
        {
            string strResponse = "";
            //var objInput = new { Analyzer_Id = analyzerId, communicationLogs };
            //PostApi(VSoftLisApiBaseUrl, "ErrorLog", errorLog, strResponse);
        }

        public static object PostApi(string baseUrl, string urlControllerActionName, object input, object outputRef, bool isGetRequest = false, int TimeoutMilliSeconds = 0)
        {
            //used NewtonSoft here to serialize anonymous objects, as it is not possible to do with .net classes
            string strRequest = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            string url = baseUrl + (baseUrl.EndsWith("/") ? "" : "/") + urlControllerActionName;

            DateTime dtmStart;
            string outputJson = "";
            using (WebClientWithTimeout client = new WebClientWithTimeout())
            {
                client.TimeoutMilliSeconds = (TimeoutMilliSeconds == 0 ? (10 * 60 * 1000) : TimeoutMilliSeconds); //set by default 10 minutes timeout
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("LISVersion", Program.CurrentLisVersionNumber);

                dtmStart = DateTime.Now;
                try
                {
                    IsVSoftConnectionSucceeded = true;
                    outputJson = isGetRequest ? client.DownloadString(url) : client.UploadString(url, strRequest);
                }
                catch (Exception ex)
                {
                    string message = "";
                    string hostName = new Uri(baseUrl).Host;
                    if (!hostName.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string serverName = hostName.Substring(0, hostName.IndexOf("."));
                        if (ex.Message.StartsWith("The remote name could not be resolved"))
                        {
                            message = "Kindly contact IT-Hardware Team or check internet connetion. Unable to connect " + serverName.ToUpper() + " server.";
                            IsVSoftConnectionSucceeded = false;
                        }
                        else if (ex.Message.Contains("Could not establish trust relationship for the SSL/TLS secure channel"))
                        {
                            message = "Kindly intimate IT-Hardware Team to check SSL certificate issue on LIS PC. (Issue observed while connecting " + serverName + " server)";
                            IsVSoftConnectionSucceeded = false;
                        }
                    }

                    if (message == "")
                    {
                        throw;
                    }
                    else
                    {
                        throw new Exception(message, ex);
                    }
                }
            }
            MessageLogger.WriteTimeDiffLog("PostApi", dtmStart, url + " - " + new string(strRequest.Take(1000).ToArray()));

            return InterfaceHelper.DeserializeFromJson(outputJson, outputRef);
        }
    }

    public class WebClientWithTimeout : System.Net.WebClient
    {
        public int TimeoutMilliSeconds { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            int timeout = TimeoutMilliSeconds == 0 ? (5 * 60 * 1000) : TimeoutMilliSeconds;
            lWebRequest.Timeout = timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = timeout;
            return lWebRequest;
        }
    }
}
