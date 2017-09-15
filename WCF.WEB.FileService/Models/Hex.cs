using System;
using System.Text;
using System.Web;

namespace WCF.WEB.FileService.Models
{
    public class UrlEncodeHelper
    {
        /// <summary>
        /// 返回处理后的十六进制字符串
        /// </summary>
        /// <param name="mStr"></param>
        /// <returns></returns> 
        public static string StrToHex(string mStr)
        {
            string val = BitConverter.ToString(ASCIIEncoding.Default.GetBytes(mStr)).Replace("-", "");
            return val.ToLower();
        }

        /// <summary>
        /// 返回十六进制代表的字符串
        /// </summary>
        /// <param name="mHex"></param>
        /// <returns></returns>
        public static string HexToStr(string mHex)
        {
            mHex = mHex.Replace(" ", "");
            if (mHex.Length <= 0) return "";
            byte[] vBytes = new byte[mHex.Length / 2];
            for (int i = 0; i < mHex.Length; i += 2)
                if (!byte.TryParse(mHex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out vBytes[i / 2]))
                    vBytes[i / 2] = 0;
            return ASCIIEncoding.Default.GetString(vBytes);
        }

        /// <summary>
        /// url64进制编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlTokenEncode(string url)
        {
            byte[] bt = System.Text.Encoding.Default.GetBytes(url);
            url = HttpServerUtility.UrlTokenEncode(bt);
            return url;
        }

        /// <summary>
        /// url64进制解码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlTokenDecode(string url)
        {
            byte[] bt = HttpServerUtility.UrlTokenDecode(url);
            url = System.Text.Encoding.Default.GetString(bt);
            return url;
        }
    }
}