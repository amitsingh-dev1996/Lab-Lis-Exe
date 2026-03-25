using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class TextLogger
    {
        public static void AttemptAppendAllText(string FileFullPath, string Content)
        {
            int reAttemptsCount = 0;
            attempt:

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(FileFullPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(FileFullPath));
                if (!File.Exists(FileFullPath))
                {
                    using (FileStream fs = File.Create(FileFullPath))
                    {
                        fs.Close();
                    }
                }

                lock (FileFullPath) //implemented locking because Cobas 8000 giving frequent concurrent file access error in communication log
                {
                    File.AppendAllText(FileFullPath, Content);
                }
            }
            catch (IOException ioe) when (ioe.Message.StartsWith("The process cannot access the file"))
            {
                reAttemptsCount++;
                if (reAttemptsCount <= 5)
                    goto attempt;
                else
                    LogErrorInLocalFile(ioe);
            }
            catch (Exception ex)
            {
                LogErrorInLocalFile(ex);
            }
        }

        public static void WriteTextFile(string filepath, string content, bool replaceContent = false, bool AppendDateInFilename = false, bool AddWithDateTimeStamp = false)
        {
            object objLock = new object();
            try
            {
                string folderPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                if (AppendDateInFilename)
                    filepath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(filepath) + "_" + DateTime.Today.ToString("yyyy_MM_dd") + Path.GetExtension(filepath));

                if (AddWithDateTimeStamp)
                    content = Environment.NewLine + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.fff") + " : " + content;

                lock (objLock)
                {
                    if (!File.Exists(filepath))
                    {
                        using (FileStream fs = File.OpenWrite(filepath))
                        {
                        }
                    }

                    if (replaceContent)
                        File.WriteAllText(filepath, content);
                    else
                        AttemptAppendAllText(filepath, content);
                }
            }
            catch (Exception ex)
            {
                LogErrorInLocalFile(ex);
            }
        }

        public static void LogErrorInLocalFile(Exception ex)
        {
            string errorMessage = ex.ToString();
            //try
            //{
            string folderPath = Path.Combine(CommonSettings.ApplicationDataFolder, "Logs");
            string fileFullPath = Path.Combine(folderPath, "ErrorLog_" + DateTime.Now.Date.ToString("yyyy_MM_dd") + ".txt");
            if (!Directory.Exists(Path.GetDirectoryName(fileFullPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileFullPath));
            AttemptAppendAllText(fileFullPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + errorMessage + Environment.NewLine + Environment.NewLine);
            
        }

        public static void WriteLogEntry(string FolderName, string content)
        {
            string logFileFullname = Path.Combine(CommonSettings.ApplicationDataFolder, "Logs", FolderName, FolderName + ".txt");
            WriteTextFile(logFileFullname, content, AppendDateInFilename: true, AddWithDateTimeStamp: true);
        }
    }
}
