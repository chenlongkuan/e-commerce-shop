namespace Msg.Bll.Models
{
    /// <summary>
    /// 用户登录模型
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 获取或设置 登录账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 获取或设置 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置 是否记住登录
        /// </summary>
        public bool IsRememberLogin { get; set; }

        /// <summary>
        /// 获取或设置 登录成功后返回地址
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
