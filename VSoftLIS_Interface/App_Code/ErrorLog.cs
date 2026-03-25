using System.Data;
using System.Data.SqlClient;
using VSoftLIS_Interface;
using System.Data.OleDb;
using System.Xml;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System;
using System.Web;
using System.Net.NetworkInformation;
using System.Management;
using System.Runtime.InteropServices;
using System.Net;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Summary description for Global
/// </summary>
public class ErrorLog
{
    SqlParameter[] sqlparm = new SqlParameter[17];

    public class CustomErrorInfo
    {
        public string EventId { get; set; }
        public string ExceptionTrace { get; set; }
        public string ErrorMessage { get; set; }
        public string ContextInfo { get; set; }

        public override string ToString()
        {
            return (EventId + "\n" + ExceptionTrace + "\n" + ErrorMessage + "\n" + ContextInfo + "\n");
        }
    }

    [DllImport("iphlpapi.dll", ExactSpelling = true)]
    public static extern int SendARP(int destIp, int srcIP, byte[] macAddr, ref uint physicalAddrLen);
    static string GetMacAddress(/*string ip*/)
    {
        
        //https://www.codeproject.com/Questions/371096/get-maq-address-in-message-box-using-csharp
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Only consider Ethernet network interfaces
            if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                nic.OperationalStatus == OperationalStatus.Up)
            {
                return nic.GetPhysicalAddress().ToString();
            }
        }
        return "";
    }

    public string getdomain(string link)
    {
        string domain = "";
        switch (link)
        {
            case "www.charbi.com":
                domain = "CHARBI";

                break;
            case "nueclear.com":
                domain = "NC";
                break;
            case "nueclear.thyrocare.com":
                domain = "NTC";
                break;
            case "crm.thyrocare.com":
                domain = "CTC";
                break;
            case "bookmyscan.com":
                domain = "BMS";
                break;
            case "thyrocare.com":
                domain = "TC";
                break;
            case "tsp.thyrocare.com":
                domain = "TTC";
                break;
            case "wellness.thyrocare.com":
                domain = "WTC";
                break;
            case "m.thyrocare.com":
                domain = "MTHY";
                break;
            case "staff.thyrocare.com":
                domain = "STC";
                break;
            case "www.ashwamedhthequest.com":
                domain = "ATQ";
                break;
            case "mis.thyrocare.com":
                domain = "MTC";
                break;
            default:
                domain = "UNKNOWN";
                break;

        }
        return domain;
    }


    public void err_insert(Exception ex, SqlParameter[] sqlParameters = null, EventArgs e = null/*, int AnalyzerId = 0*/)
    {
        try
        {
            if (sqlParameters != null && sqlParameters.Length > 0)
                ex.Data.Add("SQLParams", String.Join(", ", sqlParameters.Where(r => r != null).Select(r => "@" + r.ParameterName + " = '" + r.Value + "'").ToArray()));

            string err_id = "";
            string Page_link = "", Page1 = "", err_msg = "", Subject = "", msgdata = "";
            string user = "", name = "", domain_name = "", ExceptionType = "", strOS = "", Browser = "", HostName = "", IPAddress = "", MacAddress = "", HostAddress = "";
            string exceptionData = "", machineInfo = "";

            ExceptionType = ex.GetType().ToString();

            string InnerException = string.Empty;

            HostAddress = String.Join(",", Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(r => r.ToString()).ToArray());
            
            IPAddress = Program.LocalIpAddress;

            Browser = "UNKNOWN";
          
            strOS = (String.IsNullOrEmpty(Environment.UserDomainName) ? "" : (Environment.UserDomainName + "\\")) + Environment.MachineName;

            user = "";
            name = "";
            if (name == "")
            {
                name = "UNKNOWN";

            }
            if (user == "")
            {
                user = "UNKNOWN";
            }


            string LoggingType = "ServerSide";
            //Mail

            var customErrorMessage = new CustomErrorInfo();
            customErrorMessage.EventId = Guid.NewGuid().ToString();

            // Exception exception = Server.GetLastError();


            customErrorMessage.ExceptionTrace = ex.ToString();
            //customErrorMessage.ExceptionTrace = ex.InnerException.ToString().Replace("\n", "");

            //customErrorMessage.ExceptionTrace = exception.Message.ToString().Replace("\n", "");
            //customErrorMessage.ExceptionTrace = exception.Data.ToString().Replace("\n", "");
            customErrorMessage.ContextInfo = DateTime.Now.Date.ToString("dd-MM-yyyy");

            err_msg = customErrorMessage.ExceptionTrace.Replace(@"\", @"\\");

            if (LoggingType.Contains("ServerSide"))
            {
                Subject = Page1 + " Exception on " + DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                Subject = Page1 + " JavaScript Exception on " + DateTime.Now.ToString("dd-MM-yyyy");
            }
            try
            {
                MacAddress = GetMacAddress(/*IPAddress*/);
            }
            catch (Exception exp)
            {
                MacAddress = "UNKNOWN";
            }
            //string webname = Page_link.Substring(Page_link.IndexOf("//") + 2, Page_link.IndexOf('/', Page_link.IndexOf("//") + 2) - (Page_link.IndexOf("//") + 2));
            //domain_name = getdomain(webname.ToString());
            domain_name = "INTERFACE";

            machineInfo = "OS Version: " + Environment.OSVersion + ", UserName: " + Environment.UserName + " ";
            if (ex.Data.Count > 0)
            {
                string[] keys = new string[ex.Data.Count];
                ex.Data.Keys.CopyTo(keys, 0);
                exceptionData = String.Join("::", keys.ToList().Select(key => key + " = " + (ex.Data[key] == DBNull.Value ? "NULL" : (ex.Data[key] == "" ? "<BLANK>" : ex.Data[key]))).ToArray());
            }


            sqlparm[0] = new SqlParameter("@type", "page_val");

            sqlparm[5] = new SqlParameter("@page_link ", Page_link);

            tbl_Error_Log errorLog = new tbl_Error_Log();
            errorLog.Error_Datetime = DateTime.Now;
            errorLog.Domain = domain_name;
            errorLog.Page_Name = Page1;
            errorLog.Page_Link = Page_link;
            errorLog.Session_Name = user;
            errorLog.Exception_Type = ExceptionType;
            errorLog.Error_Description = err_msg;
            errorLog.Additional_Info = "Browser: " + Browser + Environment.NewLine + machineInfo;
            errorLog.Exception_Data = exceptionData;
            errorLog.Host = HostAddress;
            errorLog.Mac = MacAddress;
            errorLog.Ip = IPAddress;
            errorLog.Device = strOS;
            errorLog.Usercode = Program.AnalyzerId.ToString();
            WebAPI.InsertErrorLog(errorLog);

        }
        catch (Exception ex1)
        {
            VSoftLIS_Interface.Common.TextLogger.LogErrorInLocalFile(ex1);
        }
        finally
        {
            VSoftLIS_Interface.Common.TextLogger.LogErrorInLocalFile(ex);
        }
    }
}