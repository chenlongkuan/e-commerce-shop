using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Msg.Utils
{
    /// <summary>
    /// html过滤类
    /// </summary>
    public class UtilsHtmlFilter
    {
        /// <summary>
        /// 提取图片
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<string> GetTagsInHtml(string source)
        {
            Regex reg = new Regex(@"<img(.[^>]*)>", RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(source);
            List<string> list = new List<string>();
            if (m.Count > 0)
            {
                foreach (Match s in m)
                {
                    if (!s.Value.Contains("/plugins/emoticons/"))
                    {
                        list.Add(s.Value);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 提取图片
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetImgTagsInHtml(string source)
        {
            Regex reg = new Regex(@"<img(.[^>]*)>", RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(source);
            string imgs = string.Empty;
            if (m.Count > 0)
            {
                foreach (Match s in m)
                {
                    if (!s.Value.Contains("/plugins/emoticons/"))
                    {
                        imgs += "," + s.Value;
                    }
                }

                if (!string.IsNullOrEmpty(imgs))
                {
                    imgs = imgs.Substring(1);
                }
            }
            return imgs;
        }

        #region 截取字符长度和提取图片

        /// <summary>
        /// 截取字符长度和提取图片
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="arrayImage">图片列表</param>
        /// <param name="shortContent">内容</param>
        public static void IntercepStringAndExtractionImage(string content, out List<string> arrayImage, out string shortContent)
        {
            arrayImage = null;
            shortContent = "";

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            //按img分割的数组
            var list_content = new List<string>();
            //图片集合
            var list_img = new List<string>();

            Regex reg = new Regex(@"<img(.[^>]*)>", RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(content);
            if (m.Count > 0)
            {
                foreach (Match s in m)
                {
                    list_img.Add(s.Value);
                    int imglength = s.Value.Length;
                    int count = content.IndexOf(s.Value);
                    list_content.Add(Utils.RemoveHtml(content.Substring(0, count)));
                    content = content.Substring(count + imglength, content.Length - count - imglength);
                }
            }
            list_content.Add(Utils.RemoveHtml(content));
            int maxlen_content = 0;
            for (int i = 0; i < list_content.Count; i++)
            {
                if (maxlen_content < 100)
                {
                    if ((maxlen_content + list_content[i].Length) > 100)
                    {
                        shortContent += list_content[i].Substring(0, 100 - maxlen_content);
                    }
                    else
                    {
                        shortContent += list_content[i];
                    }
                    if (i < list_content.Count - 1)
                    {
                        if (list_img[i].Contains("/images/icon/") || list_img[i].Contains("/editor/xheditor_emot/"))
                        {
                            shortContent += list_img[i];
                        }
                        maxlen_content += list_content[i].Length;
                    }
                }
            }
            int maxlen_img = 0;
            var temp = new List<string>();
            foreach (var item in list_img)
            {
                if (!item.Contains("/images/icon/") && !item.Contains("/editor/xheditor_emot/"))
                {
                    if (maxlen_img < 4)
                    {
                        Regex reg1 = new Regex("src=\"([^\"]+?)\"", RegexOptions.IgnoreCase);
                        MatchCollection m1 = reg1.Matches(item);
                        if (m1.Count > 0)
                        {
                            foreach (Match s in m1)
                            {
                                temp.Add(s.Groups[1].Value);
                            }
                        }

                        maxlen_img += 1;
                    }
                }
            }
            arrayImage = temp;
        }
        #endregion

    }
}
