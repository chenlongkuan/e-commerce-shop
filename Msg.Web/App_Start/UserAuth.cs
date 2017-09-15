using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;

namespace Msg.Web.App_Start
{

    public static class UserAuth
    {

        private static UsersHelper userHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private static CartHelper _cartHelper = UnityConfig.GetConfiguredContainer().Resolve<CartHelper>();
        #region 票证相关


        /// <summary>
        /// 是否登录，票证
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {

                    if (Ticket[3] == "2.0") return true;
                    CloseTicket();
                    HttpContext.Current.Session.RemoveAll();
                    return false;
                }
                return false;
            }
        }


        /// <summary>
        /// 票证解密数组 邮箱:0  用户id:1 用户名:2 票证版本:3
        /// </summary>
        public static string[] Ticket
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    string str = HttpUtility.UrlDecode(HttpContext.Current.User.Identity.Name);
                    if (str.IndexOf("##") < 0)
                    {
                        //旧票证 作废
                        CloseTicket();
                        HttpContext.Current.Response.Redirect("/login?returnUrl=" + HttpContext.Current.Request.Url);
                        return null;
                    }
                    string[] ticketArr = Regex.Split(str, "##");
                    return ticketArr;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 作废票证
        /// </summary>
        public static void CloseTicket()
        {
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public static int UserId
        {
            get { return Ticket != null ? Convert.ToInt32(Ticket[1]) : -1; }
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public static string UserName
        {
            get
            {
                return Ticket != null ? Ticket[2] : "";
            }
        }

        /// <summary>
        /// 用户头像
        /// </summary>
        public static string Avatar
        {
            get { return string.Format("http://file.meisugou.com/ucenter/{0}/avatar/h_{0}.jpg", UserId); }
        }

        /// <summary>
        /// Gets the other users avatar.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetUsersAvatar(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = "ucenter/-1/avatar/h_-1.jpg";
            }
            url = url.Replace("http://file.meisugou.com", "");
            byte[] bt = System.Text.Encoding.Default.GetBytes(url);
            url = HttpServerUtility.UrlTokenEncode(bt);
            return string.Format("http://file.meisugou.com/ucenter/Avatar?path={0}&gender=0", url);
        }



        #endregion

        /// <summary>
        /// Gets the user cache.
        /// </summary>
        /// <value>
        /// The user cache.
        /// </value>
        public static UserModel UserCache
        {
            get
            {
                var user = userHelper.GetUserCacheModel(UserId);
                if (user != null && user.LoginTime.Date != DateTime.Now.Date)
                {
                    //更新登陆时间
                    userHelper.LoginAction(UserId);
                }
                return user;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connact user.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connact user; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConnactUser
        {
            get
            {
                return IsAuthenticated
                       && !string.IsNullOrEmpty(UserCache.ConnactUserFrom)
                       && !string.IsNullOrEmpty(UserCache.ConnactUserIdentity);
            }
        }

        /// <summary>
        /// Gets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public static UserRoleModel Role
        {
            get
            {
                return userHelper.GetUserRoleByCache(UserId);
            }
        }

        /// <summary>
        /// Gets the school identifier.
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public static int SchoolId
        {
            get { return UserCache == null ? 0 : UserCache.SchoolId; }
        }

        /// <summary>
        /// 用户所在区域Id
        /// </summary>
        /// <value>
        /// The user region identifier.
        /// </value>
        public static int UserRegionId
        {
            get { return userHelper.GetUserSchoolRegion(UserId); }
        }

        //购物车数量
        public static int CartCount
        {
            get { return _cartHelper.GetCartCount(); }
        }
    }



}