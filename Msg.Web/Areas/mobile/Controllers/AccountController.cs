using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Extensions;
using Msg.Web.App_Start;

namespace Msg.Web.Areas.Mobile.Controllers
{
    public partial class AccountController : Controller
    {
        //
        // GET: /Account/
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();

        public ActionResult Regist()
        {
            var regionId = 1;
            var schoolId = 0;
            if (Session["regionId"] != null)
            {
                try
                {
                    regionId = int.Parse(Session["regionId"].ToString());
                }
                catch (System.Exception)
                {
                }
            }
            if (Session["schoolId"] != null)
            {
                try
                {
                    schoolId = int.Parse(Session["schoolId"].ToString());
                }
                catch (System.Exception)
                {
                }
            }
            var list = _schoolHelper.GetUseableRegionInCache();
            var defaultRegion = regionId > 1 ? list.FirstOrDefault(r => r.Id == regionId) : list.FirstOrDefault();
            var regionName = "";
            if (defaultRegion == null)
            {
                regionId = 1;
                regionName = "重庆市";
            }
            else
            {
                regionId = defaultRegion.Id;
                regionName = defaultRegion.Name;
            }
            ViewBag.regionId = regionId;
            ViewBag.regionName = regionName;
            if (regionId > 0 && schoolId > 0)
            {
                ViewBag.schoolId = schoolId;
                var school = _schoolHelper.GetUseableSchoolsInCache(regionId).FirstOrDefault(r => r.Id == schoolId);
                if (school != null)
                {
                    ViewBag.schoolName = school.Name;
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Regist(int schoolId, string email, string mobile, string pwd, string nickName)
        {
            var user = new UsersEntity()
            {
                Email = email,
                Password = pwd,
                NickName = nickName,
                Mobile = mobile
            };
            var result = _usersHelper.Regist(user, schoolId);

            // to do
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("RegistOk", "Account");
            }
            var schools = _schoolHelper.GetUseableSchoolsInCache();
            ViewBag.ErrMsg = result.ResultType.ToDescription() + result.Message;
            return View(schools);
        }

        //注册成功
        public ActionResult RegistOk()
        {
            return View();
        }

        [NoAuthFilters]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //选择区域
        public ActionResult SelRegion(string from, int id)
        {
            var regions = _schoolHelper.GetUseableRegionInCache();
            ViewBag.regionId = id;
            Session["regionFromUrl"] = from;
            return View(regions);
        }

        //选择区域后跳板页 id:选中的区域id
        public ActionResult JumpRegion(int id)
        {
            if (Session["regionFromUrl"] != null)
            {
                string refferUrl = Session["regionFromUrl"].ToString();
                Session["regionId"] = id;
                Session.Remove("regionFromUrl");
                return Redirect(refferUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //选择学校
        public ActionResult SelSchool(string from, int regionId, int id = 0)
        {
            var school = _schoolHelper.GetUseableSchoolsInCache(regionId);
            ViewBag.regionId = regionId;
            ViewBag.schoolId = id;
            Session["schoolFromUrl"] = from;
            return View(school);
        }

        //选择学校后跳板页 id:选中的学校id
        public ActionResult JumpSchool(int id)
        {
            if (Session["schoolFromUrl"] != null)
            {
                string refferUrl = Session["schoolFromUrl"].ToString();
                Session["schoolId"] = id;
                Session.Remove("schoolFromUrl");
                return Redirect(refferUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [NoAuthFilters]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel login)
        {
            var result = _usersHelper.Login(login);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                if (!string.IsNullOrEmpty(login.ReturnUrl) && Utils.Utils.IsURL(login.ReturnUrl))
                {
                    return Redirect(login.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = login.ReturnUrl;
            ViewBag.ErrMsg = result.Message;
            return View("Login");
        }

        /// <summary>
        /// Logins the out.
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOut(string returnUrl)
        {
            UserAuth.CloseTicket();
            if (Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
