using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Extensions;
using Msg.Tools.Logging;
using Msg.Utils;
using Msg.Utils.Cryptography;
using Msg.Web.App_Start;
using Msg.Web.Models;
using NetDimension.Weibo;

namespace Msg.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();

        public ActionResult Regist()
        {
            var defaultRegion=_schoolHelper.GetUseableRegionInCache().FirstOrDefault();
            var schools = _schoolHelper.GetUseableSchoolsInCache(defaultRegion == null ? 1 : defaultRegion.Id);
            ViewBag.Regions = _schoolHelper.GetUseableRegionInCache();
            return View(schools);
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
                return RedirectToAction("Index", "Home");
            }
            var schools = _schoolHelper.GetUseableSchoolsInCache();
            ViewBag.ErrMsg = result.ResultType.ToDescription() + result.Message;
            return View(schools);
        }



        [NoAuthFilters]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
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


        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>      
        public ActionResult ResetPwd(int Gid, string VerifyData)
        {
            if (Gid > 0 && !string.IsNullOrEmpty(VerifyData))
            {
                var u = _usersHelper.GetUserCacheModel(Gid);
                if (u != null)
                {
                    ViewBag.Gid = Gid;
                    ViewBag.VerifyData = VerifyData;
                    Session["ResetPwd"] = Guid.NewGuid();
                    //var reg = new Regex(".");
                    //int replacelLen = u.Email.IndexOf('@') - 2;
                    //u.Email = reg.Replace(u.Email, "*", replacelLen > 0 ? replacelLen : 0, 2);
                    //ViewBag.uMail = u.Email;
                }
                else
                {
                    ViewBag.ErrMsg = "查无此用户";
                }
            }
            else
            {
                ViewBag.ErrMsg = "参数错误";
            }

            return View();
        }

        // 重设密码
        [HttpPost]
        public JsonResult ResetPass(int Gid, string VerifyData, string PassWord)
        {
            var result = _usersHelper.ResetPwd(Gid, VerifyData, PassWord);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //重设成功
        public ActionResult ResetSucess()
        {
            if (Session["ResetPwd"] == null || !Session["ResetPwd"].ToString().IsGuid())
            {
                return Redirect("/");
            }
            Session.Remove("ResetPwd");
            return View();
        }


        //发送重置密码邮件
        public JsonResult SendResetEmail(string email)
        {
            string msg = "1分钟内只能发一次！";
            bool s = false;
            if (Session["sendPassMail"] != null)
            {
                DateTime sendMailTime = (DateTime)Session["sendPassMail"];
                TimeSpan ts = DateTime.Now - sendMailTime;
                if (ts.TotalMinutes < 1)
                {
                    //  return base.Json(new { msg = msg, s = s }, JsonRequestBehavior.AllowGet);
                }
            }
            UsersEntity udal = _usersHelper.GetUserByEmail(email);
            if (udal != null)
            {
                s = UserMail.PassMail(udal);
                if (!s)
                {
                    msg = "系统故障，请联系工作人员！";
                }
                Session["sendPassMail"] = DateTime.Now;
            }
            else
            {
                msg = "帐号不存在！";
            }
            return base.Json(new { msg = msg, status = s }, JsonRequestBehavior.AllowGet);
        }

        #region 账号互通

        /// <summary>
        /// 完善用户信息
        /// </summary>
        /// <param name="lType">链接账号类型</param>
        /// <returns></returns>
        public ActionResult PerfectUserInfo(string lType)
        {
            #region TENCENT

            if (lType == "TENCENT" && Session["QQ_OPENID"] != null && Session["QQ_ACCESS_TOKEN"] != null)
            {
                var token = Session["QQ_ACCESS_TOKEN"].ToString();
                var openId = Session["QQ_OPENID"].ToString();


                var getInfoUrl =
                    string.Format(
                        @"https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}&format=json ",
                        token, ConfigHelper.GetConfigInt("QQ_APP_ID"), openId);
                var infoJson = Utils.Utils.RequestUrlByGet(getInfoUrl);

                if (string.IsNullOrEmpty(infoJson))
                {
                    ViewBag.ErrMsg = "第三方账号服务器未返回任何消息，请重新尝试登陆！";
                    return View("Error");
                }

                var userInfo = Utils.Utils.ParseJson(infoJson);

                if (userInfo["ret"] != "0")
                {
                    #region 记录日志

                    var msg = "第三方账号服务器返回用户信息出现问题，错误信息：" + userInfo["msg"] + "，错误代码：" + userInfo["ret"] + "。";
                    string assembly = "";
                    //取得当前方法命名空间
                    assembly += "命名空间名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace +
                                "\r\n";
                    //取得当前方法类全名
                    assembly += "类名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "\r\n";
                    //取得当前方法名
                    assembly += "方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n";

                    LogHelper.WriteError(msg + "\r OPENID：" + openId + "\r\n" + assembly);

                    #endregion

                    ViewBag.ErrMsg = msg;
                    return View("Error");
                }
                ViewBag.UNICKNAME = userInfo["nickname"];
                ViewBag.USERIDENTITY = openId;
                ViewBag.GENDER = userInfo["gender"] == "男" ? 0 : 1;

            }
            #endregion

            #region SINA

            else if (lType == "SINA" && Session["SINA_OPENID"] != null && Session["SINA_ACCESS_TOKEN"] != null)
            {
                var token = Session["SINA_ACCESS_TOKEN"].ToString();
                var openId = Session["SINA_OPENID"].ToString();


                var getInfoUrl = string.Format(@"https://api.weibo.com/2/users/show.json?uid={0}&access_token={1}",
                                               openId, token);
                var infoJson = Utils.Utils.RequestUrlByGet(getInfoUrl);

                if (string.IsNullOrEmpty(infoJson))
                {
                    ViewBag.ErrMsg = "第三方账号服务器未返回任何消息，请重新尝试登陆！";
                    return View("Error");

                }

                var userInfo = Utils.Utils.ParseJson(infoJson);

                if (!string.IsNullOrEmpty(userInfo["error_code"]))
                {
                    #region 记录日志

                    var msg = "第三方账号服务器返回用户信息出现问题，错误信息：" + userInfo["error"] + "，错误代码：" + userInfo["error_code"] + "。";

                    string assembly = "";
                    //取得当前方法命名空间
                    assembly += "命名空间名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace +
                                "\r\n";
                    //取得当前方法类全名
                    assembly += "类名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "\r\n";
                    //取得当前方法名
                    assembly += "方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n";

                    LogHelper.WriteError(msg + "\r OPENID：" + openId + "\r\n" + assembly);

                    #endregion

                    ViewBag.ErrMsg = msg;
                    return View("Error");
                }

                ViewBag.UNICKNAME = userInfo["screen_name"];
                ViewBag.USERIDENTITY = openId;
                ViewBag.GENDER = userInfo["gender"] == "m" ? 0 : 1;

            }


            #endregion

            #region 处理异常

            else
            {
                ViewBag.ErrMsg = "请求参数有误，请重新链接第三方账号！";
                return View("Error");
            }

            #endregion

            var saveToken = "";
            if (lType == "SINA")
            {
                saveToken = Session["SINA_ACCESS_TOKEN"].ToString();
            }
            else if (lType == "TENCENT")
            {
                saveToken = Session["QQ_ACCESS_TOKEN"].ToString();
            }

            ViewBag.SaveToken = saveToken;
            TempData["SaveTokenSession"] = saveToken;
            TempData["ContactFrom"] = lType;
            return View();
        }

        /// <summary>
        /// 保存第三方账号完善的用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SavePerfectInfo(SaveContactModel saveContact)
        {

            if (!string.IsNullOrEmpty(saveContact.SaveToken) && TempData["SaveTokenSession"] != null &&
                TempData["ContactFrom"] != null)
            {

                if (saveContact.SaveToken.Equals(TempData["SaveTokenSession"]))
                {
                    if ((string)TempData["ContactFrom"] == "TENCENT")
                    {
                        saveContact.SaveToken = "";
                        saveContact.UserIdentity = Session["QQ_OPENID"].ToString();
                    }

                    if (saveContact.SchoolId == 0)
                    {
                        saveContact.SchoolId = 95160;
                        saveContact.SchoolName = "重庆大学";
                    }

                    var u = new UsersEntity
                    {
                        NickName = saveContact.uNickName,
                        Email = saveContact.uMail,
                        Gender = (byte)saveContact.Gender,
                        ConnactUserFrom = TempData["ContactFrom"].ToString(),
                        ConnactUserIdentity = saveContact.UserIdentity,
                        AccessToKen = saveContact.SaveToken,
                        Password = "msg_user"
                    };



                    if (!Utils.Utils.IsSafeSqlString(u.NickName))
                    {
                        ViewBag.ErrMsg = "用户昵称包含违禁字符！";
                        return View("Error");

                    }


                    var result = (UsersEntity)_usersHelper.Regist(u, saveContact.SchoolId).AppendData;
                    if (result.Id > 0)
                    {
                        switch (TempData["ContactFrom"].ToString())
                        {
                            case "SINA":
                                {

                                    var Sina = new Client(new OAuth(ConfigHelper.GetConfigString("SINA_APP_ID"), ConfigHelper.GetConfigString("SINA_APP_KEY"), saveContact.SaveToken, null)); //用cookie里的accesstoken来实例化OAuth，这样OAuth就有操作权限了
                                    var uid = Sina.API.Entity.Account.GetUID();
                                    var avatar = Sina.API.Entity.Users.Show(uid).AvatarLarge;
                                    //上传微博头像
                                    //UploadUserAvatar(result.UId, avatar);


                                    //删除Token
                                    Session.Remove("SINA_ACCESS_TOKEN");
                                    Session.Remove("SINA_OPENID");

                                }
                                break;
                            case "TENCENT":
                                {
                                    #region 获取用户信息
                                    var token = Session["QQ_ACCESS_TOKEN"].ToString();
                                    var openId = Session["QQ_OPENID"].ToString();
                                    var getInfoUrl = string.Format(@"https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}&format=json ",
                                                                   token, ConfigHelper.GetConfigString("QQ_APP_ID"), openId);
                                    var infoJson = Utils.Utils.RequestUrlByGet(getInfoUrl);
                                    var userInfo = Utils.Utils.ParseJson(infoJson);
                                    //上传微博头像
                                    //UploadUserAvatar(result.UId, userInfo["figureurl_2"].Replace(@"\", ""));
                                    #endregion
                                    //删除Token
                                    Session.Remove("QQ_ACCESS_TOKEN");
                                    Session.Remove("QQ_OPENID");
                                }
                                break;

                        }


                        if (HttpContext.Session != null) HttpContext.Session["sendMail"] = u.Email;
                        return Redirect("/Account/Active?uMail=" + u.Email);
                        //return RedirectToAction("Active", "Account", new { uMail = result.uMail });
                    }
                    else
                    {
                        ViewBag.ErrMsg = "绑定账号失败，请重新登录完善用户信息";
                        return View("Error");
                    }

                }
                else
                {
                    ViewBag.ErrMsg = "保存令牌和服务端不匹配，数据可能被非法劫持，请重新登录完善用户信息";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrMsg = "保存令牌失效或者丢失，数据可能被非法劫持，请重新登录完善用户信息";
                return View("Error");
            }

        }


        /// <summary>
        /// QQ登录  ServerSide
        /// </summary>
        /// <param name="code">第三方（腾讯）返回授权码</param>
        /// <returns></returns>
        public ActionResult QqLogin(string code)
        {

            if (string.IsNullOrEmpty(code))
            {
                ViewBag.ErrMsg = "第三方授权码为空值，不可获得授权！";
                return View("Error");

            }
            #region 获取Access_Token

            const string redirectUri = "http://www.meisugou.com/account/qqlogin";
            var getTokenUrl =
                string.Format(
                    "https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&redirect_uri={3}"
                    , ConfigHelper.GetConfigString("QQ_APP_ID"), ConfigHelper.GetConfigString("QQ_APP_KEY"), code, redirectUri);

            var token = Utils.Utils.RequestUrlByGet(getTokenUrl);

            #endregion


            if (!string.IsNullOrEmpty(token))
            {
                #region 验证Token合法性
                NameValueCollection msg;
                if (token.IndexOf("callback", System.StringComparison.Ordinal) != -1)
                {
                    int lpos = token.IndexOf("(", System.StringComparison.Ordinal);
                    int rpos = token.IndexOf(")", System.StringComparison.Ordinal);
                    token = token.Substring(lpos + 1, rpos - lpos - 1);
                    msg = Utils.Utils.ParseJson(token);
                    ViewBag.ErrMsg = "账号链接令牌不合法，错误描述：" + msg["error_description"] + "错误代码：" + msg["error"] + "。";
                    return View("Error");
                }
                #endregion

                #region 获取用户信息
                var ps = Utils.Utils.ParseUrlParameters(token);
                var getOpenIdUrl = string.Format("https://graph.qq.com/oauth2.0/me?access_token={0}", ps["access_token"]);
                var oauthMeJson = Utils.Utils.RequestUrlByGet(getOpenIdUrl);

                if (oauthMeJson.Contains("callback("))
                {
                    oauthMeJson = oauthMeJson.Replace("callback(", "");
                    oauthMeJson = oauthMeJson.Substring(0, oauthMeJson.Length - 1);
                }

                var userOpenModel = Utils.Utils.ParseJson(oauthMeJson);
                #endregion

                //验证用户信息是否获取成功
                if (userOpenModel["error"] == null && string.IsNullOrEmpty(userOpenModel["error_description"]))
                {

                    #region 已绑定美速购账号的直接生产票证登陆

                    var result = _usersHelper.QuickLoginValidataUser(userOpenModel["openid"], "TENCENT");
                    if (result.ResultType.Equals(OperationResultType.IllegalOperation))
                    {
                        ViewBag.ErrMsg = result.ResultType.ToString() + result.Message;
                        return View("Error");
                    }
                    else if (result.ResultType.Equals(OperationResultType.QueryNull))
                    {
                        #region 获取到OPENID 跳转到完善用户信息


                        Session["QQ_OPENID"] = userOpenModel["openid"];
                        Session["QQ_ACCESS_TOKEN"] = ps["access_token"];

                        return RedirectToAction("PerfectUserInfo", "Account", new { lType = "TENCENT" });

                        #endregion
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    #endregion



                }
                else
                {
                    ViewBag.ErrMsg = "获取第三方用户信息失败，账号链接令牌不合法，错误描述：" + userOpenModel["error_description"] + "错误代码：" +
                                     userOpenModel["error"] + "。";
                    return View("Error");
                }
            }
            else
            {
                ViewBag.ErrMsg = "账号链接令牌不合法，链接令牌为空值。";
                return View("Error");
            }
        }




        /// <summary>
        /// 新浪微博登录  ServerSide
        /// </summary>
        /// <param name="code">第三方（新浪）返回授权码</param>
        /// <returns></returns>
        public ActionResult WeiboLogin(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.ErrMsg = "第三方授权码为空值，不可获得授权！";
                return View("Error");

            }

            #region 获取Access_Token

            OAuth oauth = new OAuth(ConfigHelper.GetConfigString("SINA_APP_ID"), ConfigHelper.GetConfigString("SINA_APP_KEY"), ConfigHelper.GetConfigString("SINA_CALL_BACK_URL"));
            var token = oauth.GetAccessTokenByAuthorizationCode(code);
            string accessToken = token.Token;
            string uid = token.UID;
            #endregion


            if (!string.IsNullOrEmpty(accessToken))
            {
                var response = Utils.Utils.ParseJson(accessToken);
                if (!string.IsNullOrEmpty(response["error"]))
                {

                    ViewBag.ErrMsg = "账号链接令牌不合法，错误描述：" + response["error_description"] + "错误代码：" + response["error"] + "。";
                    return View("Error");
                }

                //var getOpenIdUrl = //获取Token的时候已经得到了Uid
                //    string.Format("https://api.weibo.com/2/account/get_uid.json?source={0}&access_token={1}", ConfigHelper.SINA_APP_ID, response["access_token"]);
                //var oauthMeJson = xk.Library.Utils.RequestUrlByGet(getOpenIdUrl);
                //var userOpenModel = xk.Library.Utils.ParseJson(oauthMeJson);

                if (!string.IsNullOrEmpty(uid))//验证ACCESS TOKEN是否合法
                {
                    #region 已绑定美速购账号的直接生产票证登陆

                    var result = _usersHelper.QuickLoginValidataUser(uid, "SINA");
                    if (result.ResultType.Equals(OperationResultType.IllegalOperation))
                    {
                        ViewBag.ErrMsg = result.ResultType.ToString() + result.Message;
                        return View("Error");
                    }
                    else if (result.ResultType.Equals(OperationResultType.QueryNull))
                    {
                        #region 获取到OPENID 跳转到完善用户信息


                        Session["SINA_OPENID"] = uid;
                        Session["SINA_ACCESS_TOKEN"] = accessToken;

                        return RedirectToAction("PerfectUserInfo", "Account", new { lType = "SINA" });

                        #endregion
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    #endregion

                }
                else
                {
                    ViewBag.ErrMsg = "账号链接令牌不合法，错误描述：" + response["error_description"] + "错误代码：" + response["error"] + "。";
                    return View("Error");
                }

            }
            else
            {
                ViewBag.ErrMsg = "账号链接令牌不合法，链接令牌为空值。";
                return View("Error");
            }

        }


        #endregion


        //激活 
        public ActionResult ActiveOk(int Gid, string VerifyData, string return_url)
        {
            var u = _usersHelper.GetUser(Gid);
            if (u.IsActive) { return RedirectToAction("Index", "Home"); }
            ViewBag.return_url = return_url;
            var toVerifyData = Crypto.MD5(string.Format("uMail={0}&Salt={1}", u.Email.ToLower(), u.Salt));
            if (toVerifyData.Equals(VerifyData))
            {
                _usersHelper.ActiveUser(u.Id);

                ViewBag.uNickName = u.NickName;
                ViewBag.uMail = u.Email;

                return View();
            }
            else
            {
                ViewBag.ErrMsg = "激活失败，请检查激活地址！";
                return View("Error");
            }

        }

        //验证码
        public ActionResult ValiCode()
        {
            var yzm = new YZMHelper();

            Session["verifyCode"] = yzm.Text;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Bitmap image = yzm.Image;
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] buffer = ms.ToArray();
                return base.File(buffer, "image/Jpeg");

            }

        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckCaliCode(string code)
        {
            if (!code.ToLower().Equals(Session["verifyCode"].ToString().ToLower()))
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult HttpPostCheckemail(string mail)
        {
            bool success = string.IsNullOrEmpty(mail) && _usersHelper.GetUserByEmail(mail) == null;
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        //检测昵称
        [HttpPost]
        public JsonResult CheckUserName(string username)
        {
            bool status = _usersHelper.IsNickNameExits(username);
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }


        //检测昵称
        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool status = _usersHelper.IsEmailExits(email);
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }

        //重发邮件
        [HttpPost]
        public ActionResult sendMail(string mail)
        {
            string msg = "<span style=\"margin:0px 80px;\">1分钟内只能发一次！</span>";
            if (Session["sendMail"] != null)
            {
                DateTime sendMailTime = (DateTime)Session["sendMail"];
                TimeSpan ts = DateTime.Now - sendMailTime;
                if (ts.TotalMinutes < 1)
                {
                    return base.Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
                }
            }
            UsersEntity udal = _usersHelper.GetUserByEmail(mail);
            bool s = UserMail.RegisterMail(udal, null);
            msg = "<span style=\"margin:0px 70px;\">重发成功，请在邮箱查收！</span>";
            Session["sendMail"] = DateTime.Now;
            return base.Json(new { msg = msg }, JsonRequestBehavior.AllowGet);
        }


    }
}
