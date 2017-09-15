using System;

namespace Msg.Config
{
    #region Model

    /// <summary>
    /// email实体
    /// </summary>
    [Serializable]
    public class EmailConfigInfo : IConfigInfo
    {
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string Smtp { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 系统邮箱地址
        /// </summary>
        public string Sysemail { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }

    #endregion

    #region 邮件服务配置实体

    /// <summary>
    /// 邮件服务配置管理
    /// 默认配置文件存放在：D:\config
    /// 默认配置文件名:email.config
    /// </summary>
    public sealed class EmailConfigManager : ConfigManagerBase
    {
        /// <summary>
        /// 返回邮件服务配置实体
        /// </summary>
        /// <param name="configFilePath">配置文件物理路径</param>
        /// <returns></returns>
        public static EmailConfigInfo GetConfig(string configFilePath = @"c:\config\email\MDeamon.config")
        {
         
            return (EmailConfigInfo)ConfigManagerBase.LoadConfig(configFilePath, typeof(EmailConfigInfo));
        }
    }

    #endregion
}
