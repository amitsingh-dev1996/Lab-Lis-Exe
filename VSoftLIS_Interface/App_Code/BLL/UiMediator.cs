using System;
using System.Linq;
using System.Data;
using VSoftLIS_Interface.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.BLL
{
    public class UiMediator
    {
        public static VSoftLISMAIN mainForm = null;
        public static DataTable UiMessage = null;
        public static DataTable UiGridData = null;

        static UiMediator()
        {
            UiMessage = new DataTable();
            UiMessage.Columns.Add("Analyzer_ID", typeof(int));
            UiMessage.Columns.Add("MessageType", typeof(int));
            UiMessage.Columns.Add("Message", typeof(string));
            UiMessage.Columns.Add("Is_Displayed", typeof(bool));
            UiMessage.Columns.Add("Timestamp", typeof(DateTime));

            UiGridData = new DataTable();
            UiGridData.Columns.Add("Analyzer_ID", typeof(int));
            UiGridData.Columns.Add("Barcode", typeof(string));
            UiGridData.Columns.Add("Type", typeof(string));
            UiGridData.Columns.Add("Testcode", typeof(string));
            UiGridData.Columns.Add("TestValue", typeof(decimal));
            UiGridData.Columns.Add("ResultAbnormalFlag", typeof(string));
            UiGridData.Columns.Add("AdditionalInfo", typeof(string));
            UiGridData.Columns.Add("Is_Displayed", typeof(bool));
            UiGridData.Columns.Add("Timestamp", typeof(DateTime));
            UiGridData.Columns.Add("ValuesForReference", typeof(object[]));
            UiGridData.Columns.Add("PatientInfo", typeof(BarcodeList));
        }

        public static void AddUiMessage(int analyzerID, int messageType, string message)
        {
            //UiMessage.Rows.Add(analyzerID, messageType, message, false, DateTime.Now);
            //mainForm.UpdateUiMessages(new List<DataRow> { UiMessage.Rows[UiMessage.Rows.Count - 1] });

            //if (messageType == (int)MessageType.Incoming && analyzerID == 134)
            //    return;

            DataRow drNew = UiMessage.NewRow();
            drNew.ItemArray = new object[] { analyzerID, messageType, message, false, DateTime.Now };
            mainForm.UpdateUiMessages(new List<DataRow> { drNew });
        }

        public static DataTable GetUiMessages(int analyzerID)
        {
            var query = UiMessage.AsEnumerable().Where(r => r.Field<int>("Analyzer_ID") == analyzerID /*&& r.Field<bool>("Is_Displayed") == false*/);
            try
            {
                if (query.Any())
                {
                    DataTable dt = query.CopyToDataTable();
                    query.ToList().ForEach(r => r.Delete());
                    UiMessage.AcceptChanges();
                    return dt;
                }
                else
                    return UiMessage.Clone();
            }
            catch (Exception)
            {
                return UiMessage.Clone();
            }
        }

        public static void AddUiGridData(int analyzerID, string barcode, string type, string testcode, decimal? testValue, string resultAbnormalFlag, string additionalInfo, object[] valuesForReference = null, BarcodeList patientInfo = null)
        {
            object objTestValue = testValue.HasValue ? testValue.Value : (object)DBNull.Value;
            resultAbnormalFlag = (String.IsNullOrEmpty(resultAbnormalFlag) || resultAbnormalFlag.Trim() == "") ? "" : resultAbnormalFlag;
            if (resultAbnormalFlag.StartsWith(" ") || resultAbnormalFlag.EndsWith(" "))
                resultAbnormalFlag = "\"" + resultAbnormalFlag + "\"";
            //UiGridData.Rows.Add(analyzerID, barcode, type, testcode, objTestValue, resultAbnormalFlag, additionalInfo, false, DateTime.Now, valuesForReference, patientInfo);

            //mainForm.UpdateUiGridMessages(new List<DataRow> { UiGridData.Rows[UiGridData.Rows.Count - 1] });

            DataRow drNew = UiGridData.NewRow();
            drNew.ItemArray = new object[] { analyzerID, barcode, type, testcode, objTestValue, resultAbnormalFlag, additionalInfo, false, DateTime.Now, valuesForReference, patientInfo };
            mainForm.UpdateUiGridMessages(new List<DataRow> { drNew });
        }

        public static DataTable GetUiGridData(int analyzerID)
        {
            var query = UiGridData.AsEnumerable().Where(r => r.Field<int>("Analyzer_ID") == analyzerID && r.Field<bool>("Is_Displayed") == false);
            if (query.Any())
            {
                int count = query.Count();
                DataTable dt = UiGridData.Clone();
                for (int i = 0; i < count; i++)
                {
                    dt.Rows.Add(query.ElementAt(i).ItemArray);
                }
                query.ToList().ForEach(r => r.Delete());
                UiGridData.AcceptChanges();
                return dt;
            }
            else
                return UiMessage.Clone();
        }

        public static void LogAndShowError(int AnalyzerID, Exception ex, string introMessage = "")
        {
            new Task(() =>
            {
                if (introMessage == "")
                {
                    introMessage = InterfaceHelper.GetUserFriendlyErrorMessage(ex);
                }

                new ErrorLog().err_insert(ex);

                if (AnalyzerID > 0)
                {
                    AddUiMessage(AnalyzerID, (int)MessageType.Error, (String.IsNullOrEmpty(introMessage) ? "" : introMessage + ". ") + "System Error: " + ex.Message + (ex.InnerException == null ? "" : " ---> " + ex.InnerException.Message));
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show((!String.IsNullOrEmpty(introMessage) ? introMessage + Environment.NewLine : "") +
                        ex.Message +
                        (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : ""),
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }).Start();
        }
    }
}
