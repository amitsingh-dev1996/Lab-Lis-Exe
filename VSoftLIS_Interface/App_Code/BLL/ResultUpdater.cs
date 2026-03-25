using VSoftLIS_Interface.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlServerCe;
using VSoftLIS_Interface.DLL;
using System.IO;
using System.Data;
using VSoftLIS_Interface.Common;

namespace VSoftLIS_Interface
{
    public class ResultUpdater
    {
        private static LocalSqlCE objSqlCE = null;
        //private static string FailedResultFolderPath
        //{
        //    get
        //    {
        //        return Path.Combine(MainForm.ApplicationDataFolder, "FailedResults");
        //    }
        //}

        private static List<int> AnalyzerIDsIncludingModules = null;

        public ResultUpdater()
        {

        }

        static ResultUpdater()
        {
            objSqlCE = new LocalSqlCE(CommonSettings.LocalDbFileFullPathSqlce);
            Configure();
            AnalyzerIDsIncludingModules = new List<int> { VSoftLISMAIN.analyzer.instrumentid };
            if (VSoftLISMAIN.analyzer.ModuleAnalyzers?.Count() > 0)
            {
                AnalyzerIDsIncludingModules = AnalyzerIDsIncludingModules.Union(VSoftLISMAIN.analyzer.ModuleAnalyzers.Select(r => r.AnalyzerId)).ToList();
            }
        }

        private static void Configure()
        {
            ClearSucceededRecords();
        }

        public static void AddResult(InstrumentResult testResults)
        {
            using (SqlCeTransaction tran = LocalSqlCE.SharedConnection.BeginTransaction())
            {
                try
                {
                    SqlCeCommand cmd = new SqlCeCommand("", tran.Connection as SqlCeConnection, tran);
                    cmd.CommandText = "insert into tbl_FailedResults(Barcode, ResultId, TestCodeId, TestCode, TestValue, Unit, Prefix, ResultAbnormalFlag, CommentText, ResultedTime, AnalyzerId, UpdatedTime, IsResultSent, ResultSentTime, DescriptiveId) " +
                                "values(@Barcode, @ResultId, @TestCodeId, @TestCode, @TestValue, @Unit, @Prefix, @ResultAbnormalFlag, @CommentText, @ResultedTime, @AnalyzerId, @UpdatedTime, @IsResultSent, @ResultSentTime, @DescriptiveId)";
                    foreach (var barcodeDetail in testResults.results)
                    {
                        foreach (var testDetail in barcodeDetail.investigationresults)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Barcode", barcodeDetail.barcode);
                            cmd.Parameters.AddWithValue("@ResultId", DBNull.Value);
                            cmd.Parameters.AddWithValue("@TestCodeId", testDetail.testid);
                            cmd.Parameters.AddWithValue("@TestCode", testDetail.testcode);
                            cmd.Parameters.AddWithValue("@TestValue", testDetail.result);
                            cmd.Parameters.AddWithValue("@Unit", testDetail.resultunit ?? "");
                            cmd.Parameters.AddWithValue("@Prefix", testDetail.symbol ?? "");
                            cmd.Parameters.AddWithValue("@ResultAbnormalFlag", testDetail.instrumentresultnote ?? "");
                            cmd.Parameters.AddWithValue("@CommentText", testDetail.InstrumentRemarks ?? "");
                            cmd.Parameters.AddWithValue("@ResultedTime", testDetail.instrumenttime);
                            cmd.Parameters.AddWithValue("@AnalyzerId", testDetail.instrumentid);
                            cmd.Parameters.AddWithValue("@UpdatedTime", DataType.DateTime).Value = DateTime.Now;
                            cmd.Parameters.AddWithValue("@IsResultSent", 0);
                            cmd.Parameters.AddWithValue("@ResultSentTime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DescriptiveId", testDetail.TextResultId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    UiMediator.LogAndShowError(Program.AnalyzerId, ex, "Error saving failed results to local.");
                }
            }
        }

        public static void ReattemptResults()
        {
            try
            {
                Dictionary<int, TestResult> testResults = new Dictionary<int, TestResult>();

                int maxAttemptCount = 60; //running every 2 minutes, attempt for 120 minutes
                DataTable dt = LocalSqlCE.ExecuteDataSet("select * from tbl_FailedResults where IsResultSent=0 and AttemptCount<" + maxAttemptCount + " and AnalyzerId in (" + String.Join(", ", AnalyzerIDsIncludingModules) + ") order by UpdatedTime").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        string barcode = (string)dr["Barcode"];
                        string testCode = (string)dr["TestCode"];
                        try
                        {
                            MessageProcessor messageProcessor = new MessageProcessor(Program.AnalyzerId);
                            InstrumentResult testResult = messageProcessor.AddResultRecord(barcode, testCode, decimal.Parse(dr["TestValue"].ToString()), (string)dr["Unit"], (string)dr["Prefix"], (DateTime?)dr["ResultedTime"], descriptiveId: (int)dr["DescriptiveId"], AnalyzerIDForResult: (int)dr["AnalyzerId"], IsReattempt: true);
                            WebAPI.UpdateResult(testResult);
                            LocalSqlCE.ExecuteNonQuery(LocalSqlCE.PrepareCommand("update tbl_FailedResults set IsResultSent=@IsResultSent, ResultSentTime=@ResultSentTime where Id=@Id"
                                , new Dictionary<string, object>() { { "Id", (int)dr["Id"] }, { "IsResultSent", true }, { "ResultSentTime", DateTime.Now } }));
                        }
                        catch (Exception ex1)
                        {
                            UiMediator.LogAndShowError(Program.AnalyzerId, ex1, "Error re-attempting result upload (Barcode: " + barcode + ", Test: " + testCode + ")");
                            LocalSqlCE.ExecuteNonQuery(LocalSqlCE.PrepareCommand("update tbl_FailedResults set AttemptCount=(AttemptCount+1), LastAttemptedTime=@LastAttemptedTime where Id=@Id"
                            , new Dictionary<string, object>() { { "Id", (int)dr["Id"] }, { "LastAttemptedTime", DateTime.Now } }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UiMediator.LogAndShowError(Program.AnalyzerId, ex, "Error re-ttempting results upload");
            }
        }

        public static void ClearSucceededRecords()
        {
            LocalSqlCE.ExecuteNonQuery("delete from tbl_FailedResults where IsResultSent=1 and UpdatedTime<getdate()-3");
        }
    }
}
