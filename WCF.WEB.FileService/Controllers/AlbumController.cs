
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using WCF.WEB.FileService.Models;

namespace WCF.WEB.FileService.Controllers
{
    public class AlbumController : Controller
    {
        private static string HTTP_URL = ConfigurationManager.AppSettings["http_url"];
        /// <summary>
        /// 原图
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        ///   
        /// </returns>
        //[OutputCache(Duration = 1440, Location= System.Web.UI.OutputCacheLocation.Client, VaryByParam="url")]
        public ActionResult Http_img_url(string url)
        {
            if(string.IsNullOrEmpty(url))
                return new EmptyResult();

            try
            {
                url = UrlEncodeHelper.UrlTokenDecode(url);
                NameValueCollection qParams = HttpUtility.ParseQueryString(url);
                string strUrl = "/ucenter/" + qParams["uid"] + "/album/" + qParams["albumid"] + "/" + qParams["url"];

                int width,height;
                if(!int.TryParse(qParams["width"],out width))
                {
                    width = 0;
                }

                if (!int.TryParse(qParams["width"], out height))
                {
                    height = 0;
                }

                strUrl = Server.MapPath(strUrl);
                if (!System.IO.File.Exists(strUrl))
                {
                    strUrl = Server.MapPath("/images/no_photo_s.jpg");
                }

                if (width > 0 || height > 0)
                {
                    byte[] buff = Thumbnail.MakeSquareImage(strUrl, width, height);
                    Response.ClearContent();
                    string ext = System.IO.Path.GetExtension(strUrl);

                    string mime = "image/";
                    if (ext == "jpg")
                    {
                        mime += "Jpeg";
                    }
                    else
                    {
                        mime += ext;
                    }

                    return base.File(buff, mime);
                }
                else
                {
                    return base.File(strUrl, "image/Jpeg");
                }
            }
            catch
            {
                return new EmptyResult();
            }
        }
    }
}