using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class AnalyzerConfiguration
    {
        public int AnalyzerId { get; set; }
        public LISVersion LisVersion { get; set; }
        public ConnectionSettings ConnectionSettings { get; set; }
        public ConnectionSettings ConnectionSettings_WO { get; set; }

        public static ConnectionSettings GetDefaultConnectionSettings(int AnalyzerTypeId)
        {
            ConnectionSettings connectionSettings = new ConnectionSettings();

            switch (AnalyzerTypeId)
            {
                case AnalyzerTypes.Architect:
                    connectionSettings.ConnectionType = ConnectionType.Serial;
                    break;

                case AnalyzerTypes.Bechman:
                    connectionSettings.ConnectionType = ConnectionType.Serial;
                    break;

                case AnalyzerTypes.CBC560:
                    connectionSettings.ConnectionType = ConnectionType.Serial;
                    break;

                case AnalyzerTypes.Sysmex:
                case AnalyzerTypes.LABUMAT:
                connectionSettings.ConnectionType = ConnectionType.TCP;

                    switch (AnalyzerTypeId)
                    {
                        case AnalyzerTypes.Sysmex:
                            connectionSettings.TCP_IsServerMode = true;
                            break;

                    }
                    break;

                

                case AnalyzerTypes.QuantStudio:
                    connectionSettings.ConnectionType = ConnectionType.FileUpload;
                    break;

                case AnalyzerTypes.Tosoh_G8:
                //case AnalyzerTypes.Tosoh_G11:
                    connectionSettings.ConnectionType = ConnectionType.FileUpload;
                    break;

                default:
                    connectionSettings.ConnectionType = ConnectionType.TCP;
                    break;
            }

            connectionSettings.Serial_BaudRate = 9600;
            connectionSettings.Serial_Parity = 0;
            connectionSettings.Serial_StopBits = 1;
            connectionSettings.Serial_DataBits = 8;
            

            if (AnalyzerTypeId == AnalyzerTypes.Architect)
            {
                connectionSettings.Serial_DataBits = 7;
            }

            //add additional settings supported by some analyzers LIS
            connectionSettings.AdditionalSettings = new List<AdditionalSetting>();
            if (AnalyzerTypeId == AnalyzerTypes.QuantStudio)
            {
                connectionSettings.AdditionalSettings.Add(new AdditionalSetting { Name = "KitCode", Value = "N gene" });
            }
            else if (AnalyzerTypeId == AnalyzerTypes.Tosoh_G8)
            {
                connectionSettings.AdditionalSettings.Add(new AdditionalSetting { Name = "DeviceName", Value = "G8_TCG1" });
            }

            return connectionSettings;
        }

        public static ConnectionSettings PopulateConnectionSettings(int AnalyzerTypeId, DataRow drLISSettings = null)
        {
            ConnectionSettings cs = AnalyzerConfiguration.GetDefaultConnectionSettings(AnalyzerTypeId);

            if (drLISSettings == null)
                return cs;

            cs.ConnectionType = ((ConnectionType)(int)drLISSettings["ConnectionType"]);
            cs.FilePath = (string)drLISSettings["FilePath"];
            cs.TCP_IPAddress = (string)drLISSettings["TCP_IPAddress"];
            if (drLISSettings["TCP_PortNumber"] != DBNull.Value)
            {
                cs.TCP_PortNumber = (int)drLISSettings["TCP_PortNumber"];
            }
            if (drLISSettings["Serial_PortName"] != DBNull.Value)
            {
                cs.Serial_PortName = (string)drLISSettings["Serial_PortName"];
            }
            cs.Serial_BaudRate = (int)drLISSettings["Serial_BaudRate"];
            cs.Serial_Parity = (int)drLISSettings["Serial_Parity"];
            cs.Serial_StopBits = (int)drLISSettings["Serial_StopBits"];
            cs.Serial_DataBits = (int)drLISSettings["Serial_DataBits"];

            var savedAdditionalSettings = InterfaceHelper.DeserializeFromJson<List<AdditionalSetting>>(drLISSettings["AdditionalSettings"].ToString());
            if (savedAdditionalSettings != null)
            {
                foreach (var item in savedAdditionalSettings)
                {
                    //set values only if key exist in default keys hardcoded for analyzer, i.e. if key is supported by analyzer
                    var setting = cs.AdditionalSettings.Where(r => r.Name == item.Name).SingleOrDefault();
                    if (setting != null)
                    {
                        setting.Value = item.Value;
                    }
                }
            }

            return cs;
        }
    }
}
