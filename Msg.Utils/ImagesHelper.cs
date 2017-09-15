namespace Msg.Utils
{
   public static class ImagesHelper
    {

        /// <summary>
        /// 获取图片
        /// url="/upload/....";
        /// width=宽度
        /// height=高度
        /// isbase=是否原图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="quality">质量 默认80%</param>
        /// <returns></returns>
        public static string GetImgWidthHeight(string url, int width, int height, bool IsBase, int quality = 80)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            if (!url.Contains("http://file.meisugou.com"))
            {
                return url;
            }
            url = url.ToLower().Replace("http://file.meisugou.com", "");
            string strParams = string.Format("url={0}&width={1}&height={2}&isbase={3}&quality={4}", url, width, height, IsBase, quality);
            strParams = Utils.UrlTokenEncode(strParams);
            string strUrl = string.Format("http://file.meisugou.com/Photo/img_height_width?strParams={0}", strParams);
            return strUrl;
        }

       /// <summary>
       /// 获取图片 不裁剪
       /// url="/upload/....";
       /// width=宽度
       /// height=高度
       /// isbase=是否原图
       /// </summary>
       /// <param name="url"></param>
       /// <param name="width"></param>
       /// <param name="height"></param>
       /// <param name="isBase"></param>
       /// <param name="quality">质量 默认80%</param>
       /// <returns></returns>
       public static string GetImgWidthHeightNoCut(string url, int width, int height, bool isBase,int quality=80)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "";
            }
            if (!url.Contains("http://file.meisugou.com"))
            {
                return url;
            }
            url = url.ToLower().Replace("http://file.meisugou.com", "");
            string strParams = string.Format("url={0}&width={1}&height={2}&isbase={3}&quality={4}", url, width, height, isBase,quality);
            strParams = Utils.UrlTokenEncode(strParams);
            string strUrl = string.Format("http://file.meisugou.com/Photo/img_height_widthNoCut?strParams={0}", strParams);
            return strUrl;
        }


       /// <summary>
       /// 按最大宽度获取图片 不裁剪
       /// url="/upload/....";
       /// width=宽度
       /// height=高度
       /// isbase=是否原图
       /// </summary>
       /// <param name="url"></param>
       /// <param name="width"></param>
       /// <param name="height"></param>
       /// <param name="quality">质量 默认80%</param>
       /// <returns></returns>
       public static string GetImgByMaxWidthNoCut(string url, int width, int height, bool IsBase, int quality = 80)
       {
           if (string.IsNullOrEmpty(url))
           {
               return "";
           }
           if (!url.ToLower().Contains("http://file.meisugou.com"))
           {
               return url;
           }
           url = url.ToLower().Replace("http://file.meisugou.com", "");
           string strParams = string.Format("url={0}&width={1}&height={2}&isbase={3}&quality={4}", url, width, height, IsBase, quality);
           strParams = Utils.UrlTokenEncode(strParams);
           string strUrl = string.Format("http://file.meisugou.com/Photo/img_max_width_NoCut?strParams={0}", strParams);
           return strUrl;
       }
    }
}
