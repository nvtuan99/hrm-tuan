using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace HRManager.Common
{
    public class Encryptor
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public static string EncryptString(string Message)
        {
            //string s = "Kaka!2345";
            string s = "Kaka!2345";
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider cryptoServiceProvider1 = new MD5CryptoServiceProvider();
            byte[] hash = cryptoServiceProvider1.ComputeHash(utF8Encoding.GetBytes(s));
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Key = hash;
            cryptoServiceProvider2.Mode = CipherMode.ECB;
            cryptoServiceProvider2.Padding = PaddingMode.PKCS7;
            byte[] bytes = utF8Encoding.GetBytes(Message);
            byte[] inArray;
            try
            {
                inArray = cryptoServiceProvider2.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            }
            finally
            {
                cryptoServiceProvider2.Clear();
                cryptoServiceProvider1.Clear();
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DecryptString(string Message)
        {
            string s = "Kaka!2345";
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider cryptoServiceProvider1 = new MD5CryptoServiceProvider();
            byte[] hash = cryptoServiceProvider1.ComputeHash(utF8Encoding.GetBytes(s));
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Key = hash;
            cryptoServiceProvider2.Mode = CipherMode.ECB;
            cryptoServiceProvider2.Padding = PaddingMode.PKCS7;
            byte[] inputBuffer = Convert.FromBase64String(Message);
            byte[] bytes;
            try
            {
                bytes = cryptoServiceProvider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            finally
            {
                cryptoServiceProvider2.Clear();
                cryptoServiceProvider1.Clear();
            }
            return utF8Encoding.GetString(bytes);
        }

        public static string DecryptStringNoPass(string Message)
        {
            string s = "";
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider cryptoServiceProvider1 = new MD5CryptoServiceProvider();
            byte[] hash = cryptoServiceProvider1.ComputeHash(utF8Encoding.GetBytes(s));
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Key = hash;
            cryptoServiceProvider2.Mode = CipherMode.ECB;
            cryptoServiceProvider2.Padding = PaddingMode.PKCS7;
            byte[] inputBuffer = Convert.FromBase64String(Message);
            byte[] bytes;
            try
            {
                bytes = cryptoServiceProvider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            finally
            {
                cryptoServiceProvider2.Clear();
                cryptoServiceProvider1.Clear();
            }
            return utF8Encoding.GetString(bytes);
        }

        public static string EncryptStringActive(string Message)
        {
            string s = "Active!2345";
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider cryptoServiceProvider1 = new MD5CryptoServiceProvider();
            byte[] hash = cryptoServiceProvider1.ComputeHash(utF8Encoding.GetBytes(s));
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Key = hash;
            cryptoServiceProvider2.Mode = CipherMode.ECB;
            cryptoServiceProvider2.Padding = PaddingMode.PKCS7;
            byte[] bytes = utF8Encoding.GetBytes(Message);
            byte[] inArray;
            try
            {
                inArray = cryptoServiceProvider2.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            }
            finally
            {
                cryptoServiceProvider2.Clear();
                cryptoServiceProvider1.Clear();
            }
            return Convert.ToBase64String(inArray);
        }

        public static string DecryptStringActive(string Message)
        {
            string s = "Active!2345";
            UTF8Encoding utF8Encoding = new UTF8Encoding();
            MD5CryptoServiceProvider cryptoServiceProvider1 = new MD5CryptoServiceProvider();
            byte[] hash = cryptoServiceProvider1.ComputeHash(utF8Encoding.GetBytes(s));
            TripleDESCryptoServiceProvider cryptoServiceProvider2 = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider2.Key = hash;
            cryptoServiceProvider2.Mode = CipherMode.ECB;
            cryptoServiceProvider2.Padding = PaddingMode.PKCS7;
            byte[] inputBuffer = Convert.FromBase64String(Message);
            byte[] bytes;
            try
            {
                bytes = cryptoServiceProvider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            }
            finally
            {
                cryptoServiceProvider2.Clear();
                cryptoServiceProvider1.Clear();
            }
            return utF8Encoding.GetString(bytes);
        }

    }
}