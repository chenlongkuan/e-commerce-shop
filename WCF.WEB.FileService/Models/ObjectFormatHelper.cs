/*--------------------------------------------------------------------------
 *  Author:             Liuyb
 *  Create date:        2010-07-10
 *  描述：用于将临时对象串行化为字符串，数据接收方可将胡限对象转为对像
 * -------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Text;
using System.Web.UI;

namespace WCF.WEB.FileService.Models
{
    public class ObjectFormatHelper
    {
        public static string Serialize( Object classObject)
        {
            if (classObject != null)
            {
                LosFormatter lostformat = new LosFormatter();
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                lostformat.Serialize(writer, classObject);
                return sb.ToString();
            }
            return null;
        }

        public static Object Deserialize(string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                LosFormatter lostformat = new LosFormatter();
                return lostformat.Deserialize(strValue);
            }

            return null;
        }
    }
}