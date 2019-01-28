using AutoPuTTY.Properties;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AutoPuTTY.Utils
{
    class cryptHelper
    {
        /// <summary>
        /// AES encrypt text
        /// </summary>
        /// <param name="toEncrypt">text</param>
        /// <returns>encrypted text</returns>
        public static string Encrypt(string toEncrypt)
        {
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(Settings.Default.cryptkey));

            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = keyArray;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = cryptoServiceProvider.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            md5CryptoServiceProvider.Clear();
            cryptoServiceProvider.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES encrypt text with custom key
        /// </summary>
        /// <param name="toEncrypt">text</param>
        /// <param name="key">custom key</param>
        /// <returns>encrypted text</returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            if (toEncrypt == "") return "";

            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));

            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = keyArray;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = cryptoServiceProvider.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            md5CryptoServiceProvider.Clear();
            cryptoServiceProvider.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Decode AESed text
        /// </summary>
        /// <param name="toDecrypt">text for decrypt</param>
        /// <returns>decrypted text</returns>
        public static string Decrypt(string toDecrypt)
        {
            if (toDecrypt == "") return "";

            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(Settings.Default.cryptkey));

            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = keyArray;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = cryptoServiceProvider.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            md5CryptoServiceProvider.Clear();
            cryptoServiceProvider.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Decode AESed text with custom key
        /// </summary>
        /// <param name="toDecrypt">text for decrypt</param>
        /// <param name="key">key for decrypt</param>
        /// <returns>decrypted text</returns>
        public static string Decrypt(string toDecrypt, string key)
        {
            if (toDecrypt == "") return "";

            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));

            TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider();
            cryptoServiceProvider.Key = keyArray;
            cryptoServiceProvider.Mode = CipherMode.ECB;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = cryptoServiceProvider.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            md5CryptoServiceProvider.Clear();
            cryptoServiceProvider.Clear();

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}
