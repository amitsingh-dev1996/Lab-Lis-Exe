using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Security.Cryptography;

/// <summary>
/// Summary description for Function
/// </summary>
public class Functions
{
    static string key = "456as4d6a73a2fghHJS4865a87932d(d4586qzxxiwopdGKQPGT712lsa4d4sadas8";
    public Functions()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static string GenerateScript(SqlParameter[] param)
    {
        StringBuilder b = new StringBuilder();
        int i;

        //#region forloop
        for (i = 0; i < param.Length; i++)
        {
            #region try
            try
            {
                if (param[i] != null)
                {
                    string paramname = (param[i].ParameterName).ToString();
                    string paramvalue = (param[i].Value).ToString();

                    if (param[i].SqlDbType.ToString().ToLower().Contains("varchar")) //For varchar as parameter datatype
                    {
                        if (!(param[i].ParameterName.ToLower().Contains("@CustomErrorMessage"))) //@custome error message as output variable as varchar
                        {
                            paramvalue = "'" + paramvalue + "'";
                            b.AppendLine(" " + paramname + "=" + paramvalue + ",");
                        }
                    }

                    else if (param[i].SqlDbType.ToString().ToLower().Contains("xml")) //For xml as parameter datatype
                    {
                        paramvalue = "'" + paramvalue + "'";
                        b.AppendLine(" " + paramname + "=" + paramvalue + ",");
                    }

                    else if (param[i].SqlDbType.ToString().ToLower().Contains("int")) //For int as parameter datatype
                    {
                        if (!(param[i].ParameterName.ToLower().Contains("@outputidentity")) && !(param[i].ParameterName.ToLower().Contains("@issuccess")))  //@issuccess & @outputidentity error message as output variable
                        {
                            paramvalue = "'" + paramvalue + "'";
                            b.AppendLine(" " + paramname + "=" + paramvalue + ",");
                        }
                    }
                    else if (param[i].SqlDbType.ToString().ToLower().Contains("datetime")) //For xml as parameter datatype
                    {
                        paramvalue = "'" + paramvalue + "'";
                        b.AppendLine(" " + paramname + "=" + paramvalue + ",");
                    }
                    else
                    {
                        paramvalue = "'" + paramvalue + "'";
                        b.AppendLine(" " + paramname + "=" + paramvalue + ",");
                    }

                }
            }
            #endregion
            #region catch
            catch
            {
                return string.Empty;
            }
            #endregion
        }
        //#endregion
        return b.ToString();

    }

    public static string Encode(string ecode)
    {
        try
        {
            int x, y;
            string abto = "", encode = "", ABFrom = "";
            for (x = 0; x <= 25; x++)
            {
                ABFrom = ABFrom + Convert.ToChar(65 + x);
            }
            for (x = 0; x <= 25; x++)
            {
                ABFrom = ABFrom + Convert.ToChar(97 + x);
            }
            for (x = 0; x <= 9; x++)
            {
                ABFrom = ABFrom + Convert.ToString(x);
            }
            int len = ABFrom.Length - 13;
            abto = abto + ABFrom.Substring(13, len);
            abto = abto + ABFrom.Substring(0, 13);

            for (x = 0; x < ecode.Length; x++)
            {
                y = ABFrom.IndexOf(ecode.Substring(x, 1));
                if (y == 0)
                {
                    encode = encode + ecode.Substring(x, 1);
                }
                else
                {
                    encode = encode + abto.Substring(y, 1);
                }
            }
            return encode;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public static string Decode(string qry_str)
    {
        try
        {
            int x, y;
            string abto = "", Decode = "", ABFrom = "";
            for (x = 0; x <= 25; x++)
            {
                ABFrom = ABFrom + Convert.ToChar(65 + x);
            }
            for (x = 0; x <= 25; x++)
            {
                ABFrom = ABFrom + Convert.ToChar(97 + x);
            }
            for (x = 0; x <= 9; x++)
            {
                ABFrom = ABFrom + Convert.ToString(x);
            }
            int len = ABFrom.Length - 13;
            abto = abto + ABFrom.Substring(13, len);
            abto = abto + ABFrom.Substring(0, 13);

            for (x = 0; x < qry_str.Length; x++)
            {
                y = abto.IndexOf(qry_str.Substring(x, 1));
                if (y == 0)
                {
                    Decode = Decode + qry_str.Substring(x, 1);
                }
                else
                {
                    Decode = Decode + ABFrom.Substring(y, 1);
                }
            }
            return Decode;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public static System.Data.SqlTypes.SqlXml CreateXml(DataTable dt)
    {
        RemoveTimezoneForDataSet(dt);
        StringWriter swReq = new StringWriter();
        dt.WriteXml(swReq);
        return ConvertStringToXML(swReq.ToString());
        // dt.WriteXml("items.xml");  // Uncomment this if you want to save this as XML file

    }

    //below function created to convert without using SqlXml, as it gives OutOfMemoryException when converting large datatable
    public static string CreateXml_WithStringOnly(DataTable dt)
    {
        RemoveTimezoneForDataSet(dt);
        StringWriter swReq = new StringWriter();
        dt.WriteXml(swReq);
        return RemoveTroublesomeCharacters(swReq.ToString());
    }

    public static string RemoveTroublesomeCharacters(string inString)
    {
        if (inString == null) return null;

        StringBuilder newString = new StringBuilder();
        char ch;

        for (int i = 0; i < inString.Length; i++)
        {

            ch = inString[i];
            // remove any characters outside the valid UTF-8 range as well as all control characters
            // except tabs and new lines
            //if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
            //if using .NET version prior to 4, use above logic
            if (System.Xml.XmlConvert.IsXmlChar(ch)) //this method is new in .NET 4
            {
                newString.Append(ch);
            }
            else
            { }
        }
        return newString.ToString();

    }

    //implemented below, to remove timezone https://stackoverflow.com/questions/28472824/reading-xml-into-datatable-gives-incorrect-datetime-when-the-time-has-time-zone
    public static void RemoveTimezoneForDataSet(DataTable dt)
    {
        foreach (DataColumn dc in dt.Columns)
        {

            if (dc.DataType == typeof(DateTime))
            {
                dc.DateTimeMode = DataSetDateTime.Unspecified;
            }
        }
    }
    public static System.Data.SqlTypes.SqlXml ConvertStringToXML(string xmlData)
    {
        System.Data.SqlTypes.SqlXml objData;
        try
        {
            objData = new System.Data.SqlTypes.SqlXml(new System.Xml.XmlTextReader(xmlData, System.Xml.XmlNodeType.Document, null));
        }
        catch
        {
            throw;
        }
        return objData;
    }

    public static string Encrypt(string clearText, string portal = "")
    {
        try
        {

            TripleDESCryptoServiceProvider objDESCrypto =
                new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = key;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = ASCIIEncoding.ASCII.GetBytes(clearText);
            string encrypt_txt = Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            if (portal == "")
            {
                encrypt_txt = encrypt_txt.Replace(@"\", "(");
                encrypt_txt = encrypt_txt.Replace(@"/", ")");
                encrypt_txt = encrypt_txt.Replace(" ", "_");
                encrypt_txt = encrypt_txt.Replace("+", "@");
            }

            //encrypt_txt = encrypt_txt.Replace("=", "!");
            return encrypt_txt;
        }
        catch (Exception ex)
        {
            return "Input was not valid. " + ex.Message;
        }
    }

    public static string Decrypt(string cipherText, string portal = "")
    {
        try

        {
            if (portal == "")
            {
                cipherText = cipherText.Replace("(", @"\");
                cipherText = cipherText.Replace(")", @"/");
                cipherText = cipherText.Replace("_", " ");
                cipherText = cipherText.Replace("@", "+");
            }
            else
            {
                cipherText = cipherText.Replace(" ", "+");
            }
            //clearText = clearText.Replace("!", "=");
            // cipherText = "OFAqVodXzSuOZf1hKnCJZ2WBqZt3wS";
            TripleDESCryptoServiceProvider objDESCrypto =
                new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
            byte[] byteHash, byteBuff;
            string strTempKey = key;
            byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
            objHashMD5 = null;
            objDESCrypto.Key = byteHash;
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(cipherText);
            string strDecrypted = ASCIIEncoding.ASCII.GetString
            (objDESCrypto.CreateDecryptor().TransformFinalBlock
            (byteBuff, 0, byteBuff.Length));
            objDESCrypto = null;
            return strDecrypted;
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

}
