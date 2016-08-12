using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DBUtility
{
   public class BaseClass
    {
        /// <summary>
        /// 获取数据库类型Get DbType etc:oracle or mssql
        /// </summary>
        /// <returns>mssql 或 oracle</returns>
        public static string GetDbType(string dbtype = null)
        {
            dbtype = "mssql";
            return dbtype;
        }

       /// <summary>
       /// 根据业务类型和数据库配置来获取数据库连接字符串
       /// </summary>
       /// <param name="businesstype"></param>
       /// <returns></returns>
       public static string GetConnectionStr(string businesstype = null)
       {

           var constr = ConfigurationManager.AppSettings["ConStr"].ToString();
           return constr;
       }

       public static string Decrypt(string text, string key, string vector)
       {
           if (string.IsNullOrEmpty(text)) return "";

           Byte[] bKey = new Byte[32];
           Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
           Byte[] bVector = new Byte[16];
           Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);

           return DecryptStringFromBytes(Convert.FromBase64String(text), bKey, bVector);
       }
       static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
       {
           string plaintext = string.Empty;
           try
           {
               // Check arguments. 
               if (cipherText == null || cipherText.Length <= 0)
                   throw new ArgumentNullException("cipherText");
               if (key == null || key.Length <= 0)
                   throw new ArgumentNullException("Key");
               if (iv == null || iv.Length <= 0)
                   throw new ArgumentNullException("Key");

               // Declare the string used to hold 
               // the decrypted text. 


               // Create an RijndaelManaged object 
               // with the specified key and IV. 
               using (RijndaelManaged rijAlg = new RijndaelManaged())
               {
                   rijAlg.Key = key;
                   rijAlg.IV = iv;

                   // Create a decrytor to perform the stream transform.
                   ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                   // Create the streams used for decryption. 
                   using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                   {
                       using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                       {
                           using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                           {

                               // Read the decrypted bytes from the decrypting stream 
                               // and place them in a string.
                               plaintext = srDecrypt.ReadToEnd();
                           }
                       }
                   }

               }
           }
           catch (Exception ex)
           {
               //TODO Write Log
           }


           return plaintext;

       }
    }
}
