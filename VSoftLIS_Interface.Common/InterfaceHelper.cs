using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace VSoftLIS_Interface.Common
{
    public class InterfaceHelper
    {
        private const char HexSeparator = '-';
        private const string FilenameForLock = "VSoftLISSingleInstance.lock";

        public static string ConvertToWritableMessage(string message)
        {
            string convertedString = message;
            Enum.GetNames(typeof(enmCharacters)).ToList().ForEach(r => convertedString = convertedString.Replace(Convert.ToChar(Enum.Parse(typeof(enmCharacters), r)).ToString(), "[" + r + "]"));
            return convertedString;
        }

        public static string SerializeToJson(object obj)
        {
            string strJson = "";
            strJson = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return strJson;
        }

        public static object DeserializeFromJson(string strJson, Type type)
        {
            object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(strJson, type);
            return obj;
        }
        public static object DeserializeFromJson(string strJson, object obj)
        {
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(strJson, obj.GetType());
            return obj;
        }
        public static T DeserializeFromJson<T>(string strJson)
        {
            //handle null value for List object
            if (strJson == "{}")
                return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strJson);
        }

        public static string CombineMultiplePaths(string path1, params string[] paths)
        {
            string path = path1;
            foreach (string p in paths)
                path = Path.Combine(path, p);
            return path;
        }

        public static decimal? ExtractNumericValue(string strInput, out string remainingValuePart)
        {
            decimal? value = null;
            remainingValuePart = strInput;
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(strInput, @"\d+(\.\d+)?");
            if (match.Success)
            {
                string strValue = match.Value;
                value = Convert.ToDecimal(strValue);
                remainingValuePart = strInput.Replace(strValue, "");
            }
            return value;
        }

        public static bool IsNumeric(string value)
        {
            decimal test;
            return decimal.TryParse(value, out test);
        }

        public static int? ConvertToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public static string[] SplitWithoutReplace(string strInput, string strSplitString)
        {
            string[] arr = System.Text.RegularExpressions.Regex.Split(strInput, @"(?<=[" + strSplitString + "])");
            return arr;
        }

        public static string RemoveNonAsciiCharacters(string strInput)
        {
            return Regex.Replace((strInput), @"[^\u0000-\u007F]+", "?");
        }
        public static string RemoveNonPrintableCharacters(string strInput)
        {
            //https://stackoverflow.com/questions/40564692/c-sharp-regex-to-remove-non-printable-characters-and-control-characters-in-a
            return Regex.Replace((strInput), @"\p{C}+", "?");
        }

        public static string GetSubstringOrEmpty(string inputString, int startIndex, int Length)
        {
            if (startIndex < 0 || startIndex >= inputString.Length)
                return ""; // throw new Exception("StartIndex is less than zero or excceeds string length");
            else if ((startIndex + Length) > inputString.Length)
                return ""; // throw new Exception("Substring length excceeds input string length");

            return inputString.Substring(startIndex, Length);
        }

        public static string GetUserFriendlyErrorMessage(Exception ex)
        {
            string message = "";
            if (ex is WebException)
            {
                WebException wex = ex as WebException;
                if (wex.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    message = "Kindly check internet connection";
                }
            }
            else if (ex is UnauthorizedAccessException)
            {
                if (ex.Message.StartsWith("Access to the port"))
                    message = "Port already in use, kindly ensure it is not being used by other instance of LIS";
            }
            else
            {
                if (ex.Message.Contains("An existing connection was forcibly closed by the remote host"))
                {
                    message = "Connection closed by instrument";
                }
                else if (ex.Message == "A blocking operation was interrupted by a call to WSACancelBlockingCall")
                {
                    message = "Connection closed by LIS";
                }
                else if (ex.Message.StartsWith("No connection could be made because the target machine actively refused it"))
                {
                    message = "Analyzer connection is not open, kindly check analyzer or network connection";
                }
                else if (ex is SocketException)
                {
                    if (ex.Message.StartsWith("An attempt was made to access a socket in a way forbidden by its access permissions")
                        || ex.Message.StartsWith("Only one usage of each socket address"))
                    {
                        message = "Connection is already open in other application, kindly close it and then retry";
                    }
                }
                else
                {

                }
            }

            if (ex.InnerException == null)
                return message;
            else
            {
                return JoinNonEmptyStrings(" :: ", message, GetUserFriendlyErrorMessage(ex.InnerException));
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                //System.Windows.Forms.MessageBox.Show(ip.ToString());
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static IPAddress ParseValidIpAddress(string IpAddress)
        {
            System.Net.IPAddress ipAddress;
            if (System.Net.IPAddress.TryParse(IpAddress.Replace(" ", ""), out ipAddress))
            {
                return ipAddress;
            }
            return null;
        }

        public static string GetMaskRepresentationIpAddress(string IpAddress)
        {
            return String.Join(".", IpAddress.Split('.').Select(r => r.PadLeft(3, ' ')));
        }

        //https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net
        public static bool CheckForInternetConnection()
        {
            try
            {
                //using (var client = new WebClient())
                //using (client.OpenRead("http://google.com/generate_204"))
                //    return true;

                //https://stackoverflow.com/questions/13457407/why-is-getisnetworkavailable-always-returning-true
                System.Net.Dns.GetHostEntry("google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckForVSoftConnection()
        {
            try
            {
                //https://stackoverflow.com/questions/13457407/why-is-getisnetworkavailable-always-returning-true
                //System.Net.Dns.GetHostEntry("VSoftstng.southindia.cloudapp.azure.com/test/Test.html");
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region ByteToHex

        public static string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, HexSeparator));
            //return the converted value
            return builder.ToString().ToUpper();
        }
        #endregion
        #region HexToByte

        public static byte[] HexToByte(string msg)
        {
            //remove any spaces/dash from the string
            msg = msg.Replace(HexSeparator.ToString(), "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }
        #endregion

        public static string ByteToAsciiString(byte[] comByte)
        {
            //StringBuilder builder = new StringBuilder(comByte.Length);
            ////loop through each byte in the array
            //foreach (byte data in comByte)
            //    //convert the byte to a string and add to the stringbuilder
            //    //builder.Append(Convert.ToInt32(Convert.ToString(data, 16)));
            //    builder.Append(data.ToString("X2"));
            ////return the converted value
            //return HexStringToAsciiString(builder.ToString());

            return Encoding.Default.GetString(comByte);
        }

        public static byte[] AsciiStringToByte(string msg)
        {
            //byte[] comBuffer = new byte[msg.Length];
            ////loop through the length of the provided string
            //for (int i = 0; i < msg.Length; i++)
            //    comBuffer[i] = (byte)Convert.ToByte(msg.Substring(i, 1), 10);

            //return the array
            //return comBuffer;

            //https://stackoverflow.com/questions/15920741/convert-from-string-ascii-to-string-hex
            StringBuilder sb = new StringBuilder();
            foreach (char c in msg)
                sb.AppendFormat("{0:X2}" + HexSeparator, (int)c);
            return HexToByte(sb.ToString().Trim());
        }

        public static string HexStringToAsciiString(string hexString)
        {
            //byte[] comBuffer = HexToByte(hexString);
            //return ByteToAsciiString(comBuffer);

            hexString = hexString.Replace(HexSeparator.ToString(), "");
            StringBuilder builder = new StringBuilder(hexString.Length / 2);
            for (int i = 0; i < hexString.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                builder.Append(Convert.ToChar(Convert.ToUInt32(hexString.Substring(i, 2), 16)));

            return builder.ToString();
        }

        public static string AsciiStringToHexString(string msg)
        {
            //byte[] comBuffer = AsciiStringToByte(msg);
            //return BitConverter.ToString(comBuffer);
            byte[] comBuffer = Encoding.Default.GetBytes(msg);
            var hexString = BitConverter.ToString(comBuffer);
            return hexString.Replace(HexSeparator.ToString(), "");
        }


        public static object CopyObject(object source)
        {
            //Deep Copy to copy all objects, properties
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, source);
                ms.Position = 0;

                return formatter.Deserialize(ms);
            }
        }

        public static void ExtractFileToDirectory(string zipFileName, string outputDirectory)
        {
            using (ZipFile zip = ZipFile.Read(zipFileName))
            {
                //Directory.CreateDirectory(outputDirectory);
                foreach (ZipEntry e in zip)
                {
                    e.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        public static void CopyDirectoryContents(string SourcePath, string DestinationPath)
        {
            if (!Directory.Exists(DestinationPath))
                Directory.CreateDirectory(DestinationPath);

            //https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public static List<AnalyzerActiveVersions> GetLisVersionsForAnalyzers(List<int> analyzerIDs)
        {
            List<AnalyzerActiveVersions> analyzerActiveVersions = new List<AnalyzerActiveVersions>();
            analyzerActiveVersions = WebAPICommon.PostApi(CommonSettings.VSoftLisApiBaseUrl, "LisVersioning/GetVersionsForAnalyzers", analyzerIDs, analyzerActiveVersions) as List<AnalyzerActiveVersions>;
            return analyzerActiveVersions;
        }

        public static string GetExeUpdateSourcePath()
        {
            string ExeUpdateSourcePath = "";
            ExeUpdateSourcePath = WebAPICommon.PostApi(CommonSettings.VSoftLisApiBaseUrl, "LisVersioning/GetExeUpdateSourcePath", null, ExeUpdateSourcePath, isGetRequest: true) as string;
            return ExeUpdateSourcePath;
        }

        public static List<AnalyzerMaster> GetAnalyzerList(int LocationId)
        {
            List<AnalyzerMaster> analyzerList = new List<AnalyzerMaster>();
            return WebAPICommon.PostApi(CommonSettings.VSoftLisApiBaseUrl, "Interface/GetAnalyzerList/" + LocationId, new object(), analyzerList, true) as List<AnalyzerMaster>;
        }

        public static LISVersion PopulateLatestVersion(IList<LISVersion> lisVersions)
        {
            return lisVersions
                        .OrderByDescending(r => r.Major)
                        .ThenByDescending(r => r.Minor)
                        .ThenByDescending(r => r.Build)
                        .ThenByDescending(r => r.Revised)
                        .First();
        }

        //public static bool IsAnotherInstanceRunning(string ApplicationIdentifierName, out System.Threading.Mutex mutex)//added out Mutext parameter to make Mutex scope to caller
        //{
        //    //global Mutex not accessible if running in non-admin login, so implemented same functionality by using file lock

        //    //https://stackoverflow.com/questions/12898616/one-instance-application-over-multiple-windows-user-accounts
        //    string mutexName = String.Format("Global\\{{{0}}}", ApplicationIdentifierName);
        //    //https://stackoverflow.com/questions/6486195/ensuring-only-one-application-instance
        //    bool result;
        //    mutex = new System.Threading.Mutex(true, mutexName, out result);

        //    return !(result);
        //}

        public static bool IsAnotherInstanceRunning()
        {
            //global Mutex not accessible if running in non-admin login, so implemented same functionality by using file lock
            bool isLockAcquired = TryAcquireLockForSingleInstance();
            if (isLockAcquired == true)
                return false;
            else
                return true;
        }

        public static bool AcquireLockForSingleInstance(bool AttemptTillAcquire = false)
        {
            //acquire a lock to common file, so that opening another instance can be avoided by checking if lock is already acquired by earlier instance (in IsAnotherInstanceRunning())
            //this function is also to be called from LIS_Interface (Program class)

            bool isLockAcquired = TryAcquireLockForSingleInstance();

            if (!isLockAcquired && AttemptTillAcquire)
            {
                TextLogger.WriteLogEntry("ServiceLogs", "entering loop to acquire file lock");
                Thread trdAcquireLock = new Thread(() =>
                {
                    while (TryAcquireLockForSingleInstance() == false)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                });
                trdAcquireLock.Start();
            }

            return isLockAcquired;
        }

        private static bool TryAcquireLockForSingleInstance()
        {
            try
            {
                string filePathForLock = Path.Combine(CommonSettings.ApplicationDataFolder_Common, FilenameForLock); //extension can be put anything
                //https://stackoverflow.com/questions/5522232/how-to-lock-a-file-with-c
                FileStream fs = new FileStream(filePathForLock, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                TextLogger.WriteLogEntry("ServiceLogs", "acquired file lock");
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }


        public static void ScheduleMethodInThread(Action MethodReference, int FrequencyInSeconds)
        {
            new System.Threading.Thread(() =>
            {
                while (true)
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    try
                    {
                        MethodReference();
                    }
                    catch (Exception ex)
                    {
                        TextLogger.LogErrorInLocalFile(ex);
                    }

                    //sleep to run after a fixed interval, similar to a job frequency
                    int secondsElapsed = (int)sw.Elapsed.TotalSeconds;
                    System.Threading.Thread.Sleep((secondsElapsed > FrequencyInSeconds ? FrequencyInSeconds : (FrequencyInSeconds - secondsElapsed)) * 1000);
                }
            }).Start();
        }

        public static ConnectionType DetectConnectionType(ConnectionSettings connectionSettings)
        {
            return connectionSettings.TCP_PortNumber.HasValue ? ConnectionType.TCP
                           : (connectionSettings.TCP_PortNumber.HasValue ? ConnectionType.Serial
                           : (!String.IsNullOrEmpty(connectionSettings.FilePath) ? ConnectionType.FilePickup
                           : ConnectionType.FileUpload));
        }

        public static bool IsCurrentUserAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static string JoinNonEmptyStrings(string separator, params string[] inputStrings)
        {
            return string.Join(separator, inputStrings.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}
