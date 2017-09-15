using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.FileUpload;
using Msg.Tools.Logging;
using Msg.Web.App_Start;

namespace Msg.Web.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        /// <summary>
        /// 接收图片
        /// </summary>
        /// <returns></returns>
        public JsonResult RecivePhoto()
        {
            string msg = "";
            //原图地址
            string filesSrc = "";

            var status = false;
            try
            {
                byte[] jpgContent = Request.BinaryRead(Request.ContentLength);
                if (jpgContent != null)
                {
                    HttpPostedFileBase hpFile = HttpContext.Request.Files[0];
                    if (hpFile != null && hpFile.ContentLength > 0)
                    {
                        //返回消息
                        var originalMsg = NewFileUploadHelper.UploadFile(hpFile);

                        if (originalMsg.Status)
                        {
                            filesSrc = originalMsg.FileSrc;
                            status = true; msg = "上传成功";
                        }
                    }
                }
                else
                {
                    msg = "无上传内容";
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("上传图片异常", ex);

                msg = "可能由于网络原因，照片上传失败，请重试！";
                return Json(new { status = false, msg = msg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status, msg, filesSrc }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the school by region.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        /// <returns></returns>
        public JsonResult GetSchoolByRegion(int regionId)
        {
            var schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();
            var school = schoolHelper.GetUseableSchoolsInCache(regionId).Select(f => new { f.Id, f.RegionId, f.Name });

            return Json(school, JsonRequestBehavior.AllowGet);
        }
    }
}
