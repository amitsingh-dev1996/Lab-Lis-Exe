using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    [Serializable]
    public class ConnectionSettings
    {
        public ConnectionType ConnectionType { get; set; }
        public string FilePath { get; set; }

        //public string ConnectedDbFile { get; set; }
        public bool IsFileBased
        {
            get
            {
                if (TCP_PortNumber.HasValue || !String.IsNullOrEmpty(Serial_PortName))
                    return false;
                //return !String.IsNullOrEmpty(FilePath) || !String.IsNullOrEmpty(ConnectedDbFile);
                return true;
            }
        }
        public bool IsFolderPickup { get { return IsFileBased && !String.IsNullOrEmpty(FilePath); } }
        //public bool IsFileManualUpload { get { { IsFileBased && !IsFolderPickup && String.IsNullOrEmpty(ConnectedDbFile); } } }

        public string TCP_IPAddress { get; set; }
        public int? TCP_PortNumber { get; set; }
        public bool TCP_IsServerMode { get; set; }
        public string Serial_PortName { get; set; }
        public int Serial_BaudRate { get; set; } = 9600;
        public int Serial_Parity { get; set; } = 0; //None = 0, Odd = 1, Even = 2, Mark = 3, Space = 4
        public int Serial_StopBits { get; set; } = 1; //None = 0, One = 1, Two = 2, OnePointFive = 3
        public int Serial_DataBits { get; set; } = 8;
        public bool IsCharbiConnected { get; set; }
        public int ResultCheckInterval { get; set; }

        public List<AdditionalSetting> AdditionalSettings { get; set; }
    }

    [Serializable]
    public class AdditionalSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum ConnectionType
    {
        NotSpecified,
        TCP,
        Serial,
        FilePickup,
        FileUpload,
        FileConnected
    }
}
