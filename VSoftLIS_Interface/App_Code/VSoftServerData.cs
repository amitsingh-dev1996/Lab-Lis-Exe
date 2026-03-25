using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSoft.IT.Helper;

namespace VSoftLIS_Interface
{
    public class VSoftServerData
    {
        static string _connectionstring_Local = Functions.Decrypt(ConfigurationManager.ConnectionStrings["LocalDBConnection"].ToString());


        public DataSet GetResultOfAQuery(string _query)
        {
            SqlConnection webSqlcon = null;
            try
            {
                webSqlcon = new SqlConnection(_connectionstring_Local);
                webSqlcon.Open();
                return SqlHelper.ExecuteDataset(webSqlcon, CommandType.Text, _query);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                webSqlcon.Close();
            }
        }

        public VSoftServerData()
        {

        }

        public DataSet ExecuteSPWithOutParameters(string spName)
        {
            SqlConnection webSqlcon = new SqlConnection(_connectionstring_Local);
            try
            {
                webSqlcon.Open();
                return SqlHelper.ExecuteDataset(webSqlcon, spName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                webSqlcon.Close();
            }
        }

        public DataSet ExecuteSPWithParameters(string spName, SqlParameter[] _params)
        {
            SqlConnection webSqlcon = new SqlConnection(_connectionstring_Local);
            try
            {
                webSqlcon.Open();
                return SqlHelper.ExecuteDataset(webSqlcon, spName, _params);
            }
            catch
            {
                throw;
            }
            finally
            {
                webSqlcon.Close();
            }
        }

        public DataSet ExecuteSPGetDataSet(string spName, SqlParameter[] _params, int TimeoutMilliseconds = 0)
        {
            SqlConnection webSqlcon = new SqlConnection(_connectionstring_Local);
            try
            {
                webSqlcon.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = webSqlcon;
                cmd.CommandTimeout = TimeoutMilliseconds;
                foreach (var param in _params)
                {
                    cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch
            {
                throw;
            }
            finally
            {
                webSqlcon.Close();
            }
        }

        public int ExecuteSPNonQuery(string spName, SqlParameter[] _params, int TimeoutMilliseconds = 0)
        {
            int rowsAffected = 0;
            SqlConnection webSqlcon = new SqlConnection(_connectionstring_Local);
            try
            {
                webSqlcon.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = webSqlcon;
                cmd.CommandTimeout = TimeoutMilliseconds;
                foreach (var param in _params)
                {
                    cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                }
                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                webSqlcon.Close();
            }
            return rowsAffected;
        }
    }
}
