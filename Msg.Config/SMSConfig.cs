using System;

namespace Msg.Config
{
    /// <summary>

    /// 短信配置实体
    /// </summary>
    [Serializable]
    public class SMSConfigInfo : IConfigInfo
    {

        public string Account { get; set; }

        public string pwd { get; set; }

        public string MobileRegex { get; set; }

    }

    /// <summary>
    /// 短信配置
    /// </summary>
    public sealed class SMSConfigManager : ConfigManagerBase
    {
        /// <summary>
        /// 返回邮件服务配置实体
        /// </summary>
        /// <param name="configFilePath">配置文件物理路径</param>
        /// <returns></returns>
        public static SMSConfigInfo GetConfig(string configFilePath = @"C:\Config\SMS\SMS.Config")
        {
            return (SMSConfigInfo)ConfigManagerBase.LoadConfig(configFilePath, typeof(SMSConfigInfo));
        }
    }
}
