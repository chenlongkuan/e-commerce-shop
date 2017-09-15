using System.Web.Mvc;
using WCF.WEB.FileService.Models;

namespace WCF.WEB.FileService.Controllers
{
    public class ucenterController : Controller
    {
        public ActionResult Avatar(string path, byte gender)
        {
            string deUrl = UrlEncodeHelper.UrlTokenDecode(path);
            string strUrl = Server.MapPath(deUrl);
            if (!System.IO.File.Exists(strUrl))
            {
                if ( gender == 0)
                {
                    strUrl = Server.MapPath("/images/txmr.png");
                }
                else
                {
                    strUrl = Server.MapPath("/images/txmr.png");
                }
            }
            return base.File(strUrl, "image/jpeg");
        }

        public ActionResult GetRealAvatar(string path, byte gender)
        {
            string deUrl = UrlEncodeHelper.UrlTokenDecode(path);
            string strUrl = Server.MapPath(deUrl);
            if (!System.IO.File.Exists(strUrl))
            {
                if (gender == 0)
                {
                    strUrl = Server.MapPath("/images/txmr.png");
                }
                else
                {
                    strUrl = Server.MapPath("/images/txmr.png");
                }
            }
            return base.File(strUrl, "image/jpeg");
        }
    }
}
