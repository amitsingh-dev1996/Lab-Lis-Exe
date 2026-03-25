using VSoftLIS_Interface.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.BLL
{
    public class MessageLogger
    {
        //static DataTable dtCommunicationLog = null;
        static List<tbl_Communication_Log> tbl_Communication_Log = null;
        internal static int logFrequency_Milliseconds = 5000;
        private int Analyzer_ID = 0;
        private static object fileLock_CommLog = new object();
        private static object fileLock_TimeDiff = new object();

        public MessageLogger(int analyzer_ID)
        {
            Analyzer_ID = analyzer_ID;
         
            tbl_Communication_Log = new List<tbl_Communication_Log>();
        }
        internal static void LogCommunication(string message1, string senderName1)
        {
            new Task(() =>
            {
                //if (tbl_Communication_Log.Count >= 2048 || tbl_Communication_Log.Any(r => r.TimeOfCommunication < DateTime.Now.AddMinutes(-2)))
                //    LogCommunicationInServer();
                string message = message1, senderName = senderName1;

                if (Program.IsDebugMode)
                {
                    senderName += "_D";
                }

                string writableMessage = InterfaceHelper.ConvertToWritableMessage(message);
                if (!senderName.StartsWith("DETAIL"))
                {
                    lock (fileLock_CommLog)
                    {
                        tbl_Communication_Log.Add(new tbl_Communication_Log { TimeOfCommunication = DateTime.Now, Sender = senderName, Message = writableMessage });
                    }
                    //}
                }
                string folderPath = Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "Logs");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filenamePrefix = "Communication" /*+ senderName*/ + "_" + (senderName.StartsWith("DETAIL") ? "DETAIL_INPUT_" : "");
                string fileFullPath = Path.Combine(folderPath, filenamePrefix + DateTime.Now.Date.ToString("yyyy_MM_dd") + ".txt");

                StringBuilder detailMessage = new StringBuilder();
                detailMessage.Append(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffffff"));
                detailMessage.Append(String.Format("\t{0,-10}", "Thread" + System.Threading.Thread.CurrentThread.ManagedThreadId));
                detailMessage.Append(String.Format("\t{0,-15}", senderName));
                detailMessage.Append("\t" + writableMessage);
                detailMessage.Append(Environment.NewLine);

                TextLogger.AttemptAppendAllText(fileFullPath, detailMessage.ToString());

            }).Start();
        }

        internal static void LogCommunicationInServer()
        {
            if (/*dtCommunicationLog.Rows.Count == 0 ||*/ tbl_Communication_Log.Count == 0)
                return;

            ////copy log to insert in separate datatable, as rows being added from different thread
            int initialCount = tbl_Communication_Log.Count;

            //insert log until successful
            bool succeeded = false;
            int retryAttempt = 0;
            while (!succeeded && retryAttempt < 10)
            {

                SqlParameter[] sqlparams = null;
                try
                {
                    List<tbl_Communication_Log> tempCommLog = null;
                    lock (fileLock_CommLog)
                    {
                        tempCommLog = tbl_Communication_Log.Take(initialCount).ToList();
                        tbl_Communication_Log.RemoveRange(0, initialCount);
                    }
                   // WebAPI.InsertCommunicationLog(Program.AnalyzerId, tempCommLog);

                    //}
                    succeeded = true;
                }
                catch (Exception ex)
                {
                    retryAttempt++;
                    new ErrorLog().err_insert(ex, sqlparams);
                    System.Threading.Thread.Sleep(logFrequency_Milliseconds / 2);
                }
            }
        }

        public static void WriteTimeDiffLog(string fileKey, DateTime? dtmStart = null, string description = "")
        {
            try
            {
                if (dtmStart == null)
                    dtmStart = DateTime.Now;

                string folderPath = Path.Combine(VSoftLISMAIN.ApplicationDataFolder, "TimeDiff");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                string fileFullPath = Path.Combine(folderPath, "TimeDiff_" + fileKey + "_" + DateTime.Now.Date.ToString("yyyyMMdd") + ".txt");
                double millisecondsElapsed = DateTime.Now.Subtract(dtmStart.Value).TotalMilliseconds;
                string fileContent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " :: " + millisecondsElapsed.ToString("000000") + " :: " + description + " :: " + Environment.NewLine;

                lock (fileLock_TimeDiff)
                {
                    File.AppendAllText(fileFullPath, fileContent);
                }
            }
            catch (Exception ex)
            {
                new ErrorLog().err_insert(ex);
            }
        }
    }
}
