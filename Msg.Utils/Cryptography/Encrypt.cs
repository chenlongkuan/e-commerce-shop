using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Msg.Utils.Cryptography
{
    /// <summary>
    /// MD5 SHA256加密
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] buff = Sha256.ComputeHash(SHA256Data);
            string Result = "";
            for (int i = 0; i < buff.Length; i++)
                Result += buff[i].ToString("x").PadLeft(2, '0');
            return Result;
        }
    }

    /// <summary> 
    /// 加密 
    /// </summary> 
    public class AES
    {
        //默认密钥向量
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };
        private static string encryptKey = "13983837198";
        /// <summary>
        /// Encode加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encode(string encryptString)
        {
            string encryptKeys = Utils.GetSubString(encryptKey, 32, "");
            encryptKeys = encryptKey.PadRight(32, ' ');

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKeys.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }
        /// <summary>
        /// Decode解密 
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string Decode(string decryptString)
        {
            try
            {
                string decryptKey = Utils.GetSubString(encryptKey, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                throw;
            }

        }

    }

    /// <summary> 
    /// 加密
    /// </summary> 
    public class DES
    {
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string encryptKey)
        {
            encryptKey = Utils.GetSubString(encryptKey, 8, "");
            encryptKey = encryptKey.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());

        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = Utils.GetSubString(decryptKey, 8, "");
                decryptKey = decryptKey.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();

                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return "";
            }

        }

    }


    /// <summary>
    /// 构造一个对称算法,使用3Des加密
    /// 两种语言之间有两种兼容方式：
    /// 1.C#采用CBC Mode，PKCS7 Padding,Java采用CBC Mode，PKCS5Padding Padding
    /// 2.C#采用ECB Mode，PKCS7 Padding,Java采用ECB Mode，PKCS5Padding Padding
    /// </summary>
    public class CryptoTripleDes
    {


        #region 字节数组型  加解密

        //加密矢量
        private static byte[] IV = { 0xB0, 0xA2, 0xB8, 0xA3, 0xDA, 0xCC, 0xDA, 0xCC };

        /// <summary>
        /// 使用指定的24字节的密钥对字节数组进行3Des加密
        /// </summary>
        /// <param name="keys">密钥，24字节</param>
        /// <param name="values">要加密的数组</param>
        /// <returns>已加密的数组</returns>
        public static byte[] CreateEncryptByte(byte[] keys, byte[] values)
        {
            TripleDESCryptoServiceProvider tdsc = new TripleDESCryptoServiceProvider();
            //指定密匙长度，默认为192位
            tdsc.KeySize = 128;
            //使用指定的key和IV（加密向量）
            tdsc.Key = keys;
            tdsc.IV = IV;
            //加密模式，偏移
            tdsc.Mode = CipherMode.ECB;
            tdsc.Padding = PaddingMode.PKCS7;
            //进行加密转换运算
            ICryptoTransform ct = tdsc.CreateEncryptor();

            byte[] results = ct.TransformFinalBlock(values, 0, values.Length);

            return results;
        }
        /// <summary>
        /// 使用指定的24字节的密钥对字符串（8位）进行3Des加密
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static byte[] CreateEncryptString(string strKey, string strValue)
        {
            TripleDESCryptoServiceProvider tdsc = new TripleDESCryptoServiceProvider();
            byte[] results = new byte[strValue.Length];
            tdsc.KeySize = 128;
            if (!string.IsNullOrEmpty(strKey))
            {
                tdsc.Key = Encoding.UTF8.GetBytes(strKey);
            }
            tdsc.IV = IV;

            tdsc.Padding = PaddingMode.PKCS7;
            using (ICryptoTransform ct = tdsc.CreateDecryptor())
            {
                byte[] byt = Encoding.UTF8.GetBytes(strValue);
                results = ct.TransformFinalBlock(byt, 0, byt.Length);
            }
            return results;
        }
        /// <summary>
        /// 对加密字符串进行解密
        /// </summary>
        /// <param name="keys">密匙</param>
        /// <param name="values">已加密字符串</param>
        /// <returns>解密结果</returns>
        public static byte[] CreateDescryptByte(byte[] keys, byte[] values)
        {
            TripleDESCryptoServiceProvider tdsc = new TripleDESCryptoServiceProvider();

            //指定密匙长度，默认为192位
            tdsc.KeySize = 128;
            //使用指定的key和IV（加密向量）
            tdsc.Key = keys;
            tdsc.IV = IV;
            //加密模式，偏移
            tdsc.Mode = CipherMode.ECB;
            tdsc.Padding = PaddingMode.PKCS7;
            //进行加密转换运算
            ICryptoTransform ct = tdsc.CreateDecryptor();

            byte[] results = ct.TransformFinalBlock(values, 0, values.Length);

            return results;
        }


        /// <summary>
        /// 对加密字符串进行解密
        /// </summary>
        /// <param name="keys">密匙</param>
        /// <param name="values">已加密字符串</param>
        /// <returns>解密结果</returns>
        public static byte[] CreateDescryptString(string keys, string values)
        {
            TripleDESCryptoServiceProvider tdsc = new TripleDESCryptoServiceProvider();
            if (!string.IsNullOrEmpty(keys))
            {
                //使用指定的key和IV（加密向量）
                tdsc.Key = Encoding.UTF8.GetBytes(keys);
            }
            //指定密匙长度，默认为192位
            tdsc.KeySize = 128;

            tdsc.IV = IV;
            //加密模式，偏移
            tdsc.Mode = CipherMode.ECB;
            tdsc.Padding = PaddingMode.PKCS7;
            //进行加密转换运算
            ICryptoTransform ct = tdsc.CreateDecryptor();
            byte[] byt = Encoding.UTF8.GetBytes(values);
            byte[] results = ct.TransformFinalBlock(byt, 0, values.Length);

            return results;
        }

        #endregion
    }


    /// <summary>
    /// 哈希加密类
    /// </summary>
    public class HashMethod
    {
        /// <summary>
        /// 加密方法 SHA1
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public static string Encrypto(string Source)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
    }
}
