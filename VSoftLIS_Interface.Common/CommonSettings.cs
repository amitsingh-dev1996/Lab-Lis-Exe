using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class CommonSettings
    {
        public static string ProductName { get { return "VSoftLIS"; } }
        public static string VSoftLisApiBaseUrl { get; set; } = "http://VSoft.avmlabs.com/LIS";
        public static string ApplicationDataFolder { set; get; }
        public static string ApplicationDataFolder_Common { set; get; }
        public static string ExeRootFolder { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ProductName); } }
        public static string LocalDbFileFullPathSqlce { get { return Path.Combine(ExeRootFolder, "VSoftLocalStorage.sdf"); } }
        public static string ExeUpdateSourcePath { get { return @"\\192.168.0.17\lab\VSoft LIS Update"; } }
        public static int LocationId { get; set; }

        public static string CommandArgumentForSchedulerCall { get { return "FROMTASKSCHEDULER"; } }
        static CommonSettings()
        {
            ApplicationDataFolder_Common =
                ApplicationDataFolder = Path.Combine(ExeRootFolder, "ApplicationData");
        }

    }
}
