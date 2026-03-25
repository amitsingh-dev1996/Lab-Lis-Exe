using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSoftLIS_Interface
{
    public partial class tbl_Communication_Log
    {
        //public int RecordId { get; set; }
        public System.DateTime TimeOfCommunication { get; set; }
        public int Analyzer_Id { get; set; }
        public string Sender { get; set; }
        public string Message { get; set; }
        public System.DateTime LoggedTime { get; set; }
    }

    public partial class tbl_Error_Log
    {
        //public int Error_Id { get; set; }
        public DateTime? Error_Datetime { get; set; }
        public string Domain { get; set; }
        public string Page_Name { get; set; }
        public string Page_Link { get; set; }
        public string Session_Name { get; set; }
        public string Exception_Type { get; set; }
        public string Error_Description { get; set; }
        public string Additional_Info { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public string Usercode { get; set; }
        public string Exception_Data { get; set; }
        public string Host { get; set; }
        public string Mac { get; set; }
        public string Ip { get; set; }
        public string Device { get; set; }
    }
}
