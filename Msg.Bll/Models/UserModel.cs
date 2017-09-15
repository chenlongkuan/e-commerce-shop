using System;

namespace Msg.Bll.Models
{
    public class UserModel
    {
        public UserModel()
        {
            IsUseable = true;
            Credits = 0;
            LastLoginTime = DateTime.Now;
            LoginTime = DateTime.Now;
        }


        public int Id { get; set; }


        /// <summary>
        ///     注册邮箱
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        //[Required]
        public string Email { get; set; }

        /// <summary>
        ///     加密的密码
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        ///     密码盐
        /// </summary>
        /// <value>
        ///     The salt.
        /// </value>
        public string Salt { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        /// <value>
        ///     The name of the nick.
        /// </value>
        public string NickName { get; set; }

        /// <summary>
        ///     用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     用户电话
        /// </summary>
        /// <value>
        ///     The mobile.
        /// </value>
        public string Mobile { get; set; }

        /// <summary>
        ///     用户姓名
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        public string UserName { get; set; }


        /// <summary>
        ///     用户状态 True:可用,False：不可用
        /// </summary>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     用户积分
        /// </summary>
        /// <value>
        ///     The credits.
        /// </value>
        public int Credits { get; set; }

        /// <summary>
        ///     上次登录时间
        /// </summary>
        /// <value>
        ///     The last login time.
        /// </value>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        ///     最新登录时间
        /// </summary>
        /// <value>
        ///     The login time.
        /// </value>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登陆次数
        /// </summary>
        /// <value>
        /// The login times.
        /// </value>
        public int LoginTimes { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }


        /// <summary>
        ///     链接用户标识（如：外部接口返回用户Id等）
        /// </summary>
        public string ConnactUserIdentity { get; set; }

        /// <summary>
        ///     链接用户来源名称（如：QQ/Weibo）
        /// </summary>
        public string ConnactUserFrom { get; set; }


        /// <summary>
        ///     微博绑定
        /// </summary>
        public string AccessToKen { get; set; }

        /// <summary>
        /// Gets or sets the shool identifier.
        /// </summary>
        /// <value>
        /// The shool identifier.
        /// </value>
        public int SchoolId { get; set; }

    }
}
