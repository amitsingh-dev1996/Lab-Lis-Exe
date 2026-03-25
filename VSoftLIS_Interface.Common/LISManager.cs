using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO.Pipes;
using System.Windows.Forms;

namespace VSoftLIS_Interface.Common
{
    //https://www.c-sharpcorner.com/UploadFile/naresh.avari/develop-and-install-a-windows-service-in-C-Sharp/
    public partial class LISManager
    {
        Dictionary<int, Process> RunningProcesses = new Dictionary<int, Process>();
        List<AnalyzerActiveVersions> analyzerActiveVersions = new List<AnalyzerActiveVersions>();
        string fileNameForProcessIDs = Path.Combine(CommonSettings.ApplicationDataFolder, "Processes.json");
        List<AnalyzerMaster> AnalyzerList = null;
        //AnonymousPipeServerStream pipeServer = null;
        public string versionNumberOfUpdateInProgress = "";

        public LISManager()
        {
            AnalyzerList = InterfaceHelper.GetAnalyzerList(CommonSettings.LocationId);
        }

        public bool OpenLISConfigurationForm()
        {
            try
            {
                TextLogger.WriteLogEntry("ServiceLogs", "Starting OpenLISConfigurationForm()");
                if (analyzerActiveVersions?.Count == 0)
                {
                    TextLogger.WriteLogEntry("ServiceLogs", "Attempting GetLisVersionsForAnalyzers()");
                    analyzerActiveVersions = InterfaceHelper.GetLisVersionsForAnalyzers(new List<int> { 0 });
                }
                LISVersion latestVersion = analyzerActiveVersions.Where(r => r.AnalyzerId == 0).Single().ActiveVersions.Single();
                LoadRunningProcessesAndAnalyzerId();

                int analyzerId_AdminPortal = 0;
                if (!RunningProcesses.ContainsKey(analyzerId_AdminPortal) || RunningProcesses[analyzerId_AdminPortal] == null)
                {
                    string status = OpenLISExe(new AnalyzerConfiguration() { AnalyzerId = analyzerId_AdminPortal, LisVersion = latestVersion });
                    if (status != "SUCCESS")
                    {
                        TextLogger.WriteLogEntry("ServiceLogs", "Failed to launch LIS Admin:" + Environment.NewLine + status);
                        MessageBox.Show("Failed to launch LIS Admin:" + Environment.NewLine + status, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    WriteRunningProcessIDs();
                }
                else
                {
                    TextLogger.WriteLogEntry("ServiceLogs", "Process for AnalyzerId=" + analyzerId_AdminPortal + " is already open.");
                }
                return true;
            }
            catch (Exception ex)
            {
                TextLogger.LogErrorInLocalFile(ex);
                return false;
            }
        }

        public void RunAllConfiguredLIS()
        {
            try
            {
                TextLogger.WriteLogEntry("ServiceLogs", "RunAllConfiguredLIS Started. Current running LIS Processes Count: " + Process.GetProcessesByName("VSoftLIS_Interface").Count());

                LocalSqlCE localSqlCE = new LocalSqlCE(CommonSettings.LocalDbFileFullPathSqlce);

                InterfaceHelper.ScheduleMethodInThread(EnsureAllLISOpen, 1);

            }
            catch (Exception ex)
            {
                TextLogger.LogErrorInLocalFile(ex);
            }
        }

        private void EnsureAllLISOpen()
        {
            //TODO: If not already open, open exe for admin portal

            Dictionary<int, string> statusDetails = new Dictionary<int, string>();
            TextLogger.WriteLogEntry("ServiceLogs", "Fetching configured LIS list");
            DataTable dtActiveLIS = LocalSqlCE.ExecuteDataSet("select * from tbl_LISSetting where IsActive=1").Tables[0];
            DataTable dtInactiveLIS = LocalSqlCE.ExecuteDataSet("select * from tbl_LISSetting where IsActive=0").Tables[0];

            //get version details from server for all configured
            List<int> analyzerIDs = dtActiveLIS.AsEnumerable().Select(r => r.Field<int>("AnalyzerId")).ToList();

            TextLogger.WriteLogEntry("ServiceLogs", "Fetched configured LIS, count: " + dtActiveLIS.Rows.Count);

            LoadRunningProcessesAndAnalyzerId();

            if (analyzerIDs.Any())
            {
                //hit API only if new analyzer id available, or every 5 minutes
                if (analyzerActiveVersions.Count == 0 || analyzerIDs.Any(r => analyzerActiveVersions.Where(r1 => r1.AnalyzerId == r).Count() == 0)
                    || (DateTime.Now.Second == 0 && DateTime.Now.Minute % 5 == 0))
                {
                    TextLogger.WriteLogEntry("ServiceLogs", "Fetching latest versions for configured analyzers");
                    analyzerActiveVersions = InterfaceHelper.GetLisVersionsForAnalyzers(analyzerIDs);
                }

                foreach (DataRow dr in dtActiveLIS.Rows)
                {
                    int analyzerId = (int)dr["AnalyzerId"];
                    statusDetails.Add(analyzerId, "");
                    try
                    {
                        AnalyzerConfiguration analyzer = new AnalyzerConfiguration()
                        {
                            AnalyzerId = analyzerId,
                            ConnectionSettings = AnalyzerConfiguration.PopulateConnectionSettings((int)dr["AnalyzerTypeId"], dr)
                        };

                        //populate most latest version number
                        analyzer.LisVersion = analyzerActiveVersions.Where(r => r.AnalyzerId == analyzerId).Single().PopulateLatestVersion();

                        TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + analyzer.AnalyzerId + ", version: " + analyzer.LisVersion.VersionNumber);
                        string status = OpenLISExe(analyzer/*, versionToOpen*/);
                        statusDetails[analyzerId] = status;
                        //Thread.Sleep(2000);
                    }
                    catch (Exception ex)
                    {
                        TextLogger.LogErrorInLocalFile(ex);
                    }
                }

                var statusDetailsQuery = from sd in statusDetails
                                         join a in AnalyzerList on sd.Key equals a.AnalyzerId
                                         where sd.Value != "SUCCESS"
                                         orderby sd.Value, a.AnalyzerName
                                         select a.AnalyzerName + ": " + sd.Value;
                string statusDetailsPopupMessage = String.Join(Environment.NewLine, statusDetailsQuery);
                if (statusDetailsQuery.Any())
                {
                    MessageBox.Show("Failed to launch LIS:" + Environment.NewLine + statusDetailsPopupMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //close inactivated LIS EXEs
            foreach (DataRow dr in dtInactiveLIS.Rows)
            {
                int analyzerId = (int)dr["AnalyzerId"];
                CloseLISExe(analyzerId);
            }

            WriteRunningProcessIDs();
            //OpenLISConfigurationForm();
            TextLogger.WriteLogEntry("ServiceLogs", "EnsureAllLISOpen end");
        }

        private void LoadRunningProcessesAndAnalyzerId()
        {
            //TODO: detect if LIS exe opened by old instance of manager exe
            int currentProcessID = Process.GetCurrentProcess().Id;
            //var runningLISApplications = Process.GetProcessesByName("LIS_Interface"); //.Where(r => r.Id != currentProcessID);
            //foreach (var p in runningLISApplications)
            //{
            //    TextLogger.WriteLogEntry("ServiceLogs", "Already Running LIS commandargs: " + p.StartInfo.Arguments);
            //    int analyzerId = 0;
            //    RunningProcesses[analyzerId] = p;
            //}
            List<RunningLisIDs> runningIDs = new List<RunningLisIDs>(); // new Dictionary<int, int>();//key:AnalyzerId, value:ProcessId
            if (File.Exists(fileNameForProcessIDs))
            {
                string fileContent = File.ReadAllText(fileNameForProcessIDs);
                if (!String.IsNullOrWhiteSpace(fileContent))
                    runningIDs = InterfaceHelper.DeserializeFromJson(fileContent, runningIDs) as List<RunningLisIDs>;
            }
            foreach (var item in runningIDs)
            {
                RunningProcesses[item.AnalyzerId] = Process.GetProcesses().Where(r => r.Id == item.ProcessId).FirstOrDefault(); //GetProcessById throws error if process is not running

                if (RunningProcesses[item.AnalyzerId] == null)
                    TextLogger.WriteLogEntry("ServiceLogs", $"LIS for AnalyzerId {item.AnalyzerId} is found to be closed. Will be re-opened automatically.");
            }
            TextLogger.WriteLogEntry("ServiceLogs", "LoadRunningProcessesAndAnalyzerId call ended, running processes count: " + runningIDs.Count());
        }

        public string OpenLISExe(AnalyzerConfiguration Analyzer/*, LISVersion lisVersion*/)
        {
            string status = "";
            try
            {
                //Process process = null;
                if (!RunningProcesses.ContainsKey(Analyzer.AnalyzerId))
                    RunningProcesses.Add(Analyzer.AnalyzerId, null);

                //TextLogger.WriteLogEntry("ServiceLogs", "RunningProcesses[Analyzer.AnalyzerId]: " + RunningProcesses[Analyzer.AnalyzerId]);
                //TextLogger.WriteLogEntry("ServiceLogs", "RunningProcesses[Analyzer.AnalyzerId]?.Id: " + (RunningProcesses[Analyzer.AnalyzerId]?.Id));
                ////TextLogger.WriteLogEntry("ServiceLogs", "RunningProcesses[Analyzer.AnalyzerId]?.HasExited ?? true" + (RunningProcesses[Analyzer.AnalyzerId]?.HasExited ?? true));

                if (!IsLISRunningForAnalyzer(Analyzer.AnalyzerId))
                {
                    //process = OpenLISExe(Analyzer, versionToOpen);
                    if (!String.IsNullOrEmpty(versionNumberOfUpdateInProgress) && Analyzer.LisVersion.VersionNumber == versionNumberOfUpdateInProgress)
                    {
                        TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", Version: " + Analyzer.LisVersion.VersionNumber + ", Skipped Opening LIS, as version update is in progress");
                    }

                    TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", Version: " + Analyzer.LisVersion.VersionNumber + ", Opening LIS");

                    string errorMessage = "";
                    bool isDownloadSuccess = EnsureExeDonwloaded(Analyzer.LisVersion, out errorMessage);

                    TextLogger.WriteLogEntry("ServiceLogs", "isDownloadSuccess: " + isDownloadSuccess);
                    if (isDownloadSuccess)
                    {
                        string exePath = Path.Combine(/*Application.StartupPath*/CommonSettings.ExeRootFolder, Analyzer.LisVersion.VersionNumber, /*Application.ProductName*/"LIS_Interface" + ".exe");
                        string json = InterfaceHelper.SerializeToJson(Analyzer);
                        json = "\"" + System.Text.RegularExpressions.Regex.Replace(json, @"\t|\n|\r", "").Replace("\"", "\\\"") + "\"";

                        TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", exePath: " + exePath + ", Opening LIS exe from folder");
                        Process p = Process.Start(exePath, json);

                        //https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-anonymous-pipes-for-local-interprocess-communication
                        //Process p = new Process();
                        //p.StartInfo.FileName = exePath;
                        //p.StartInfo.Arguments = json + " " + pipeServer.GetClientHandleAsString();
                        //p.StartInfo.UseShellExecute = false;
                        //p.Start();
                        //pipeServer.DisposeLocalCopyOfClientHandle();

                        //RunningProcesses[Analyzer.AnalyzerId] = p;

                        //using (StreamWriter sw = new StreamWriter(pipeServer))
                        //{
                        //    sw.AutoFlush = true;
                        //    // Send a 'sync message' and wait for client to receive it.
                        //    sw.WriteLine("SYNC");
                        //    pipeServer.WaitForPipeDrain();

                        //    sw.WriteLine("VERSIONNUMBER::" + );
                        //}

                        TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", Opened LIS, Version: " + Analyzer.LisVersion.VersionNumber + ", ProcessId: " + p.Id);
                        status = "SUCCESS";
                        RunningProcesses[Analyzer.AnalyzerId] = p;
                    }
                    else
                    {
                        status = "Error in downloading LIS exe:" + errorMessage;
                        TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", Version: " + Analyzer.LisVersion.VersionNumber + ", Failed downloading LIS");
                        TextLogger.WriteLogEntry("ServiceLogs", "Exe download failed for version " + Analyzer.LisVersion.VersionNumber);
                    }
                }
                else
                {
                    status = "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + Analyzer.AnalyzerId + ", Version: " + Analyzer.LisVersion.VersionNumber + ", Failed Opening LIS");
                TextLogger.LogErrorInLocalFile(ex);
                status = "Error in downloading LIS exe:" + ex.Message;
            }

            return status;
        }

        private bool CloseLISExe(int analyzerId)
        {
            try
            {
                if (IsLISRunningForAnalyzer(analyzerId))
                {
                    try
                    {
                        RunningProcesses[analyzerId].Kill();
                        RunningProcesses.Remove(analyzerId);
                        TextLogger.WriteLogEntry("ServiceLogs", "Closed LIS EXE for AnalyzerId " + analyzerId);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        TextLogger.LogErrorInLocalFile(ex);
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                TextLogger.LogErrorInLocalFile(ex);
            }
            return false;
        }

        private bool EnsureExeDonwloaded(LISVersion lisVersion, out string ErrorMessage)
        {
            try
            {
                string versionFolderPath = Path.Combine(CommonSettings.ExeRootFolder, lisVersion.VersionNumber);

                bool forceUpdate = false;
                if (File.Exists(Path.Combine(CommonSettings.ExeRootFolder, "ForceUpdate.txt")))
                {
                    forceUpdate = true;
                }

                if (!Directory.Exists(versionFolderPath) || !Directory.GetFiles(versionFolderPath).Any()
                    || forceUpdate)
                {
                    //first download to temp folder, then move to working folder - to minimize duration for keep LIS closing
                    string tempVersionFolderPath = Path.Combine(Path.GetTempPath(), lisVersion.VersionNumber);
                    if (Directory.Exists(tempVersionFolderPath))
                        Directory.Delete(tempVersionFolderPath, true);

                    /*string exeUpdateSourcePath = InterfaceHelper.GetExeUpdateSourcePath();*/ //CommonSettings.ExeUpdateSourcePath;
                    string exeUpdateSourcePath = CommonSettings.ExeUpdateSourcePath; //CommonSettings.ExeUpdateSourcePath;

                    //TODO: implement hash to verify files are same as in server, to avoid files manipulation
                    if (String.IsNullOrEmpty(exeUpdateSourcePath) || exeUpdateSourcePath.Equals("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        TextLogger.WriteLogEntry("ServiceLogs", "Downloading LIS application from web server for version " + lisVersion.VersionNumber + " (Path: " + exeUpdateSourcePath + ")");
                        string zipFilename = lisVersion.VersionNumber + ".zip";
                        string localZipFileFullname = Path.GetTempPath() + zipFilename;
                        Common.WebAPICommon.DownloadFile(CommonSettings.VSoftLisApiBaseUrl, "api/LisVersioning/GetExeFilesInZip?lisVersion=" + lisVersion.VersionNumber, localZipFileFullname);
                        InterfaceHelper.ExtractFileToDirectory(localZipFileFullname, tempVersionFolderPath);
                        File.Delete(localZipFileFullname);
                        TextLogger.WriteLogEntry("ServiceLogs", "Completed downloading LIS application from web server for version " + lisVersion.VersionNumber);
                    }
                    else if (exeUpdateSourcePath.StartsWith(@"\\")) //network share path (UNC)
                    {
                        TextLogger.WriteLogEntry("ServiceLogs", "Downloading LIS application from shared folder for version " + lisVersion.VersionNumber + " (Path: " + exeUpdateSourcePath + ")");
                        string sourceFolderPath = Path.Combine(exeUpdateSourcePath, lisVersion.VersionNumber.ToString());

                        InterfaceHelper.CopyDirectoryContents(sourceFolderPath, tempVersionFolderPath);
                        TextLogger.WriteLogEntry("ServiceLogs", "Completed downloading LIS application from shared folder for version " + lisVersion.VersionNumber);
                    }

                    versionNumberOfUpdateInProgress = lisVersion.VersionNumber;
                    //close Open LIS of the version, to avoid file access error
                    bool closeSuccess = true;
                    foreach (var analyzerId in analyzerActiveVersions.Where(r => r.PopulateLatestVersion().VersionNumber == versionNumberOfUpdateInProgress).Select(r => r.AnalyzerId))
                    {
                        if (IsLISRunningForAnalyzer(analyzerId))
                        {
                            TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + analyzerId + ", Closing LIS, as update is inprogress for version " + versionNumberOfUpdateInProgress);
                            closeSuccess = CloseLISExe(analyzerId);

                            if (!closeSuccess)
                            {
                                TextLogger.WriteLogEntry("ServiceLogs", "AnalyzerId: " + analyzerId + ", Failed to close LIS, kindly disable from configuration");
                                break;
                            }
                        }
                    }
                    if (closeSuccess)
                    {
                        if (Directory.Exists(versionFolderPath))
                        {
                            Directory.Delete(versionFolderPath, true);
                        }

                        //Move function not working inbetween two different disc drives
                        //Directory.Move(tempVersionFolderPath, versionFolderPath);
                        InterfaceHelper.CopyDirectoryContents(tempVersionFolderPath, versionFolderPath);
                        Directory.Delete(tempVersionFolderPath, true);
                        versionNumberOfUpdateInProgress = "";

                        TextLogger.WriteLogEntry("ServiceLogs", "Completed downloading LIS application from server for version " + lisVersion.VersionNumber);

                        if (forceUpdate)
                        {
                            //delete file to avoid repeated update
                            File.Delete(Path.Combine(CommonSettings.ExeRootFolder, "ForceUpdate.txt"));
                        }
                    }
                }

                ErrorMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                TextLogger.WriteLogEntry("ServiceLogs", "Failed to update LIS. Error: " + ex.Message);
                TextLogger.LogErrorInLocalFile(ex);
                ErrorMessage = ex.Message;
                return false;
            }
        }

        private void WriteRunningProcessIDs()
        {
            List<RunningLisIDs> runningIDs = RunningProcesses.Select(r => new RunningLisIDs { AnalyzerId = r.Key, ProcessId = r.Value?.Id ?? 0 })
                .Where(r => r.ProcessId > 0).ToList();
            TextLogger.WriteTextFile(fileNameForProcessIDs, InterfaceHelper.SerializeToJson(runningIDs), replaceContent: true);
        }

        private bool IsLISRunningForAnalyzer(int analyzerId)
        {
            if (RunningProcesses.ContainsKey(analyzerId) && RunningProcesses[analyzerId]?.Id > 0 && (RunningProcesses[analyzerId]?.HasExited ?? true) == false)
                return true;
            else
                return false;
        }
    }

    [Serializable]
    public class RunningLisIDs
    {
        public int AnalyzerId { get; set; }
        public int ProcessId { get; set; }
    }
}
