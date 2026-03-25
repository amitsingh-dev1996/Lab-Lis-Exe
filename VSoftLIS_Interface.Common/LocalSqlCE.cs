using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Threading.Tasks;

namespace VSoftLIS_Interface.Common
{
    public class LocalSqlCE
    {
        private static string DbFileFullPath { get; set; }
        private static string ConnectionString { get { return "Data Source=" + DbFileFullPath; } }
        private static SqlCeConnection _SharedConnection = null;
        public static SqlCeConnection SharedConnection
        {
            get
            {
                if (_SharedConnection == null)
                    _SharedConnection = new SqlCeConnection(ConnectionString);

                if (_SharedConnection.State == ConnectionState.Closed)
                    _SharedConnection.Open();

                return _SharedConnection;
            }
        }

        public LocalSqlCE(string _DbFileFullPath)
        {
            //SQLCE DLLs to be copied from C:\Program Files (x86)\Microsoft SQL Server Compact Edition\v3.5
            //SQLCE 3.5 setup download link https://www.microsoft.com/en-us/download/confirmation.aspx?id=5783

            //DbFileFullPath = _DbFileFullPath;

            //if (!System.IO.File.Exists(DbFileFullPath))
            //{
            //    SqlCeEngine engine = new SqlCeEngine(ConnectionString);
            //    engine.CreateDatabase();
            //}
        }

        static LocalSqlCE()
        {
            DbFileFullPath = CommonSettings.LocalDbFileFullPathSqlce;

            if (!System.IO.File.Exists(DbFileFullPath))
            {
                SqlCeEngine engine = new SqlCeEngine(ConnectionString);
                engine.CreateDatabase();
            }

            if (!LocalSqlCE.CheckTableExists("tbl_GlobalConfiguration"))
            {
                //varchar is not supported, only nvarchar
                LocalSqlCE.ExecuteNonQuery(
                    "CREATE TABLE tbl_GlobalConfiguration (" +
                    "Id int identity(1,1) primary key," +
                    "LocationId int," +
                    "CreatedBy nvarchar(10)," +
                    "CreatedOn datetime," +
                    "LastUpdatedBy nvarchar(10)," +
                    "LastUpdatedOn datetime" +
                    ")"
                    );

                SqlCeCommand cmd = LocalSqlCE.PrepareCommand("insert into tbl_GlobalConfiguration (LocationId, CreatedBy, CreatedOn) " +
                    "Values (0, '', @CreatedOn)", new Dictionary<string, object> { { "CreatedOn", DateTime.Now } });
                LocalSqlCE.ExecuteNonQuery(cmd);
            }


            if (!LocalSqlCE.CheckTableExists("tbl_LISSetting"))
            {
                //varchar is not supported, only nvarchar
                LocalSqlCE.ExecuteNonQuery(
                    "CREATE TABLE tbl_LISSetting (" +
                    "Id int identity(1,1) primary key," +
                    "AnalyzerId int," +
                    "AnalyzerTypeId int," +
                    "LocationId int," +
                    "ConnectionType int," +
                    "TCP_IPAddress nvarchar(50)," +
                    "TCP_PortNumber int," +
                    "Serial_PortName nvarchar(10)," +
                    "Serial_BaudRate int," +
                    "Serial_Parity int," +
                    "Serial_StopBits int," +
                    "Serial_DataBits int," +
                    "FilePath nvarchar(500)," +
                    "KeyValueyConfiguration nvarchar(1000)," +
                    //"LisPcIpAddress nvarchar(20)," +
                    //"LisPcName nvarchar(20)," +
                    "IsActive bit," +
                    "CreatedBy nvarchar(10)," +
                    "CreatedOn datetime," +
                    "LastUpdatedBy nvarchar(10)," +
                    "LastUpdatedOn datetime" +
                    ")"
                    );
            }
            try
            {
                LocalSqlCE.ExecuteNonQuery("ALTER TABLE tbl_LISSetting add AdditionalSettings nvarchar(4000)");
            }
            catch
            {
                //column already exists, do nothing
            }


            if (!LocalSqlCE.CheckTableExists("tbl_FailedResults"))
            {
                //varchar is not supported, only nvarchar
                LocalSqlCE.ExecuteNonQuery(
                    "create table tbl_FailedResults (" +
                    "Id int identity(1,1) primary key," +
                    "Barcode nvarchar(50)," +
                    "ResultId int," +
                    "TestCodeId int," +
                    "TestCode nvarchar(50)," +
                    "TestValue float," +
                    "Unit nvarchar (50)," +
                    "Prefix nvarchar(50)," +
                    "ResultAbnormalFlag nvarchar(50)," +
                    "CommentText nvarchar(1000)," +
                    "ResultedTime datetime," +
                    "AnalyzerId int," +
                    "UpdatedTime datetime," +
                    "IsResultSent bit," +
                    "ResultSentTime datetime," +
                    "DescriptiveId int," +
                    "AttemptCount int NOT NULL DEFAULT(0)," +
                    "LastAttemptedTime datetime" +
                    ")"
                    );
            }
        }

        public static void ChangeConnection(string _DbFileFullPath)
        {
            DbFileFullPath = _DbFileFullPath;
            _SharedConnection = null;
        }

        public static SqlCeCommand PrepareCommand(string commandText, Dictionary<string, object> parameters = null)
        {
            SqlCeCommand cmd = new SqlCeCommand(commandText, SharedConnection);
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            return cmd;
        }

        public static DataSet ExecuteDataSet(string commandText)
        {
            DataSet ds = new DataSet();
            SqlCeCommand cmd = PrepareCommand(commandText);
            SqlCeDataAdapter da = new SqlCeDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public static int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(PrepareCommand(commandText));
        }

        public static int ExecuteNonQuery(SqlCeCommand cmd)
        {
            int recordCount = 0;

            if (cmd.Connection == null)
                cmd.Connection = _SharedConnection;

            SqlCeDataAdapter da = new SqlCeDataAdapter(cmd);
            recordCount = cmd.ExecuteNonQuery();
            return recordCount;
        }

        public static bool CheckTableExists(string TableName)
        {
            return ((int)ExecuteDataSet("SELECT count(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + TableName + "'").Tables[0].Rows[0][0]) > 0 ? true : false;
        }
    }
}
