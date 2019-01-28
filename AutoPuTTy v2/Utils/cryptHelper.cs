using AutoPuTTY.Properties;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace AutoPuTTY.Utils
{
    class CryptHelper
    {
        #region Text Encrypt

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

        #endregion

        #region VNC Crypt

        public static string EncryptPassword(string passwd)
        {
            // Use standard VNC Server Key
            byte[] rawKey = new byte[8];
            rawKey[0] = 23;
            rawKey[1] = 82;
            rawKey[2] = 107;
            rawKey[3] = 6;
            rawKey[4] = 35;
            rawKey[5] = 78;
            rawKey[6] = 88;
            rawKey[7] = 7;
            // revert it
            rawKey = FixDESBug(rawKey);
            byte[] Passwd_Bytes = new byte[8];
            if (passwd.Length >= 8)
            {
                Encoding.ASCII.GetBytes(passwd, 0, 8, Passwd_Bytes, 0);
            }
            else
            {
                Encoding.ASCII.GetBytes(passwd, 0, passwd.Length, Passwd_Bytes, 0);
            }

            // VNC uses DES, not 3DES as written in some documentation
            DES des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.ECB;

            ICryptoTransform enc = des.CreateEncryptor(rawKey, null);

            byte[] passwd_enc = new byte[8];
            enc.TransformBlock(Passwd_Bytes, 0, Passwd_Bytes.Length, passwd_enc, 0);
            string ret = "";

            for (int i = 0; i < 8; i++)
            {
                ret += passwd_enc[i].ToString("x2");
            }
            return ret;
        }

        /// <summary>VNC DES authentication has a bug, such that keys are reversed.  This code 
        /// was written by Dominic Ullmann (dominic_ullmann@swissonline.ch) and is 
        /// is being used under the GPL.</summary>
        /// <param name="desKey">The key to be altered.</param>
        /// <returns>Returns the fixed key as an array of bytes.</returns>
        public static byte[] FixDESBug(byte[] desKey)
        {
            byte[] newkey = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                // revert desKey[i]:
                newkey[i] = (byte)(
                    ((desKey[i] & 0x01) << 7) |
                    ((desKey[i] & 0x02) << 5) |
                    ((desKey[i] & 0x04) << 3) |
                    ((desKey[i] & 0x08) << 1) |
                    ((desKey[i] & 0x10) >> 1) |
                    ((desKey[i] & 0x20) >> 3) |
                    ((desKey[i] & 0x40) >> 5) |
                    ((desKey[i] & 0x80) >> 7)
                );
            }
            //for (int i = 0; i < newkey.Length; i++)
            //{
            //    Console.WriteLine(newkey[i]);
            //}
            return newkey;
        }

        #endregion

        #region DPAPI Crypt

        private const int CRYPTPROTECT_UI_FORBIDDEN = 0x1;
        private static readonly IntPtr NullPtr = ((IntPtr)((int)(0)));

        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CryptProtectData(
        ref DATA_BLOB pPlainText,
        [MarshalAs(UnmanagedType.LPWStr)]string szDescription,
        IntPtr pEntroy,
        IntPtr pReserved,
        IntPtr pPrompt,
        int dwFlags,
        ref DATA_BLOB pCipherText);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }

        private static void InitBLOB(byte[] data, ref DATA_BLOB blob)
        {
            blob.pbData = Marshal.AllocHGlobal(data.Length);
            if (blob.pbData == IntPtr.Zero) throw new Exception("Unable to allocate buffer for BLOB data.");

            blob.cbData = data.Length;
            Marshal.Copy(data, 0, blob.pbData, data.Length);
        }

        public static string encryptpw(string pw)
        {
            byte[] pwba = Encoding.Unicode.GetBytes(pw);
            DATA_BLOB dataIn = new DATA_BLOB();
            DATA_BLOB dataOut = new DATA_BLOB();
            StringBuilder epwsb = new StringBuilder();
            try
            {
                try
                {
                    InitBLOB(pwba, ref dataIn);
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot initialize dataIn BLOB.", ex);
                }

                bool success = CryptProtectData(
                ref dataIn,
                "psw",
                NullPtr,
                NullPtr,
                NullPtr,
                CRYPTPROTECT_UI_FORBIDDEN,
                ref dataOut);

                if (!success)
                {
                    int errCode = Marshal.GetLastWin32Error();
                    throw new Exception("CryptProtectData failed.", new Win32Exception(errCode));
                }

                byte[] epwba = new byte[dataOut.cbData];
                Marshal.Copy(dataOut.pbData, epwba, 0, dataOut.cbData);
                // Convert hex data to hex characters (suitable for a string)
                for (int i = 0; i < dataOut.cbData; i++) epwsb.Append(Convert.ToString(epwba[i], 16).PadLeft(2, '0').ToUpper());
            }
            catch (Exception ex)
            {
                throw new Exception("unable to encrypt data.", ex);
            }
            finally
            {
                if (dataIn.pbData != IntPtr.Zero) Marshal.FreeHGlobal(dataIn.pbData);

                if (dataOut.pbData != IntPtr.Zero) Marshal.FreeHGlobal(dataOut.pbData);
            }
            return epwsb.ToString();
        }

        #endregion
    }
}
