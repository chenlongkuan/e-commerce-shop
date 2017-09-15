using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using WCF.WEB.FileService.Models;

namespace WCF.WEB.FileService.Controllers
{
    public class PhotoController : Controller
    {
        private static string HTTP_URL = ConfigurationManager.AppSettings["http_url"];
        /// <summary>
        /// 原图  
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// 
        /// </returns>
        //[OutputCache(Duration = 1440, Location = System.Web.UI.OutputCacheLocation.Client, VaryByParam = "url")]
        public ActionResult Http_img_url(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new EmptyResult();

            try
            {
                url = UrlEncodeHelper.UrlTokenDecode(url);
                NameValueCollection qParams = HttpUtility.ParseQueryString(url);
                string strUrl = "ucenter\\" + qParams["uid"] + "\\album\\" + qParams["albumid"] + "\\" + qParams["url"];

                int width, height;
                if (!int.TryParse(qParams["width"], out width))
                {
                    width = 0;
                }

                if (!int.TryParse(qParams["width"], out height))
                {
                    height = 0;
                }

                strUrl = WCF.Lib.File.Upload.SavePath + strUrl;
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

        //url="\\ucenter\\uid\\album\\albumid\\22000.jpg&width=500&height=100&"
        public ActionResult img_height_width(string strParams)
        {
            if (string.IsNullOrEmpty(strParams))
                return new EmptyResult();

            try
            {
                strParams = UrlEncodeHelper.UrlTokenDecode(strParams);
                NameValueCollection qParams = HttpUtility.ParseQueryString(strParams);
                string strUrl = qParams["url"];
                bool isbase = Convert.ToBoolean(qParams["isbase"]);
                int width, height, quality;
                if (!int.TryParse(qParams["width"], out width))
                {
                    width = 0;
                }

                if (!int.TryParse(qParams["height"], out height))
                {
                    height = 0;
                }
                if (!int.TryParse(qParams["quality"], out quality))
                {
                    quality = 80;
                }
                strUrl = WCF.Lib.File.Upload.SavePath + strUrl;
                if (!System.IO.File.Exists(strUrl))
                {
                    strUrl = Server.MapPath("/images/no_photo_s.jpg");
                }

                if ((width > 0 || height > 0) && !isbase)
                {
                    string ext = System.IO.Path.GetExtension(strUrl);
                    string name = System.IO.Path.GetFileNameWithoutExtension(strUrl);

                    var newdir = strUrl.Replace(name + ext, string.Format("{0}×{1}\\", width, height));
                    WCF.Lib.File.Helper.Utils.CreateDir(newdir);

                    if (ext.ToUpper() == ".GIF")
                    {
                        ext = ".jpg";
                    }

                    var newurl = newdir + name + ext;

                    if (!System.IO.File.Exists(newurl))
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(strUrl);
                        if (originalImage.Width < width && originalImage.Height < 100)
                        {
                            strUrl = Server.MapPath("/images/no_photo_s.jpg");
                            ext = System.IO.Path.GetExtension(strUrl);
                            name = System.IO.Path.GetFileNameWithoutExtension(strUrl);
                            newdir = strUrl.Replace(name + ext, string.Format("{0}×{1}\\", width, height));
                            WCF.Lib.File.Helper.Utils.CreateDir(newdir);

                            if (ext.ToUpper() == ".GIF")
                            {
                                ext = ".jpg";
                            }
                            newurl = newdir + name + ext;
                        }
                        else
                        {
                            WCF.Lib.File.Helper.Thumbnail.CutForCustom(originalImage, newurl, width, height, quality);
                            //WCF.Lib.File.Helper.NewThumbnail.MakeSmailImageByMaxHeightAndMaxWidth(originalImage, newurl, width, height);
                        }
                        return base.File(newurl, "image/Jpeg");

                    }
                    else
                    {
                        return base.File(newurl, "image/Jpeg");
                    }
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


        public ActionResult img_height_widthNoCut(string strParams)
        {
            if (string.IsNullOrEmpty(strParams))
                return new EmptyResult();

            try
            {
                strParams = UrlEncodeHelper.UrlTokenDecode(strParams);
                NameValueCollection qParams = HttpUtility.ParseQueryString(strParams);
                string strUrl = qParams["url"];
                bool isbase = Convert.ToBoolean(qParams["isbase"]);
                int width, height,quality;
                if (!int.TryParse(qParams["width"], out width))
                {
                    width = 0;
                }

                if (!int.TryParse(qParams["height"], out height))
                {
                    height = 0;
                }
                if (!int.TryParse(qParams["quality"], out quality))
                {
                    quality = 80;
                }
                strUrl = WCF.Lib.File.Upload.SavePath + strUrl;
                if (!System.IO.File.Exists(strUrl))
                {
                    strUrl = Server.MapPath("/images/no_photo_s.jpg");
                }

                if ((width > 0 || height > 0) && !isbase)
                {
                    string ext = System.IO.Path.GetExtension(strUrl);
                    string name = System.IO.Path.GetFileNameWithoutExtension(strUrl);

                    var newdir = strUrl.Replace(name + ext, string.Format("{0}×{1}\\", width, height));
                    WCF.Lib.File.Helper.Utils.CreateDir(newdir);

                    if (ext.ToUpper() == ".GIF")
                    {
                        ext = ".jpg";
                    }

                    var newurl = newdir + name + ext;

                    if (!System.IO.File.Exists(newurl))
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(strUrl);

                        WCF.Lib.File.Helper.NewThumbnail.MakeSmailImageByMaxHeightAndMaxWidth(originalImage, newurl, width, height, quality);

                        return base.File(newurl, "image/Jpeg");

                        //Response.ClearContent();
                        //string ext = System.IO.Path.GetExtension(strUrl);



                        //string mime = "image/";
                        //if (ext == "jpg")
                        //{
                        //    mime += "Jpeg";
                        //}
                        //else
                        //{
                        //    mime += ext;
                        //}

                        ////return base.File(buff, mime);
                    }
                    else
                    {
                        return base.File(newurl, "image/Jpeg");
                    }
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


        public ActionResult img_max_width_NoCut(string strParams)
        {
            if (string.IsNullOrEmpty(strParams))
                return new EmptyResult();

            try
            {
                strParams = UrlEncodeHelper.UrlTokenDecode(strParams);
                NameValueCollection qParams = HttpUtility.ParseQueryString(strParams);
                string strUrl = qParams["url"];
                bool isbase = Convert.ToBoolean(qParams["isbase"]);
                int width, height, quality;
                if (!int.TryParse(qParams["width"], out width))
                {
                    width = 0;
                }

                if (!int.TryParse(qParams["height"], out height))
                {
                    height = 0;
                }
                 if (!int.TryParse(qParams["quality"], out quality))
                {
                    quality = 80;
                }
                strUrl = WCF.Lib.File.Upload.SavePath + strUrl;
                if (!System.IO.File.Exists(strUrl))
                {
                    strUrl = Server.MapPath("/images/no_photo_s.jpg");
                }

                if ((width > 0 || height > 0) && !isbase)
                {
                    string ext = System.IO.Path.GetExtension(strUrl);
                    string name = System.IO.Path.GetFileNameWithoutExtension(strUrl);

                    var newdir = strUrl.Replace(name + ext, string.Format("{0}×{1}\\", width, height));
                    WCF.Lib.File.Helper.Utils.CreateDir(newdir);

                    if (ext.ToUpper() == ".GIF")
                    {
                        ext = ".jpg";
                    }

                    var newurl = newdir + name + ext;

                    if (!System.IO.File.Exists(newurl))
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(strUrl);

                        WCF.Lib.File.Helper.NewThumbnail.MakeSmailImageByMaxWidth(originalImage, newurl, width, quality);

                        return base.File(newurl, "image/Jpeg");


                    }
                    else
                    {
                        return base.File(newurl, "image/Jpeg");
                    }
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
