using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace VSoftLIS_Interface.Common
{
    public class FileManager
    {

        static FileManager()
        {
            string folderFullPath = GetFullFolderPath(FolderName.WOJson);
            if (!Directory.Exists(folderFullPath))
                Directory.CreateDirectory(folderFullPath);
        }

        public static string ReadAllText(FolderName FolderName, string FileName, string SubFolderName = "")
        {
            if (String.IsNullOrEmpty(Path.GetExtension(FileName)))
            {
                FileName += ".txt";
            }

            return File.ReadAllText(Path.Combine(GetFullFolderPath(FolderName, SubFolderName), FileName));
        }

        public static void WriteAllText(FolderName FolderName, string FileName, string Content, string SubFolderName = "")
        {
            if (String.IsNullOrEmpty(Path.GetExtension(FileName)))
            {
                FileName += ".txt";
            }

            string folderPath = GetFullFolderPath(FolderName, SubFolderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            File.WriteAllText(Path.Combine(folderPath, FileName), Content);
        }

        public static void DeleteFile(FolderName FolderName, string FileName, string SubFolderName = "")
        {
            File.Delete(Path.Combine(GetFullFolderPath(FolderName, SubFolderName), FileName));
        }

        public static string GetFullFolderPath(FolderName FolderName, string SubFolderName = "")
        {
            switch (FolderName)
            {
                case FolderName.WOJson:
                    return Path.Combine(VSoftLISMAIN.ApplicationDataFolder, FolderName.ToString(), SubFolderName);

                default:
                    return VSoftLISMAIN.ApplicationDataFolder;
            }
        }
    }

    public enum FolderName
    {
        WOJson
    }
}
