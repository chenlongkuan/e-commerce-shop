using System;

namespace Msg.Config
{
    #region Model

    /// <summary>
    /// WCF服务实体
    /// </summary>
    [Serializable]
    public class WcfAddressConfigInfo : IConfigInfo
    {
        /// <summary>
        /// wcf缓存服务地址
        /// </summary>
        public string FileAddress { get; set; }
        /// <summary>
        /// 文件服务器新地址2011-10-13
        /// </summary>
        public string NewFileAddress { get; set; }
    }

    #endregion

    #region 返回WCF服务实体

    /// <summary>
    /// 签到规则配置管理
    /// 默认配置文件存放在：D:\config
    /// 默认配置文件名:WcfAddress.config
    /// </summary>
    public sealed class WcfAddressConfigManager : ConfigManagerBase
    {
        /// <summary>
        /// 返回WCF服务实体
        /// </summary>
        /// <param name="configFilePath">配置文件物理路径</param>
        /// <returns></returns>
        public static WcfAddressConfigInfo GetConfig(string configFilePath = @"C:\config\WcfAddress.config")
        {
            return (WcfAddressConfigInfo)ConfigManagerBase.LoadConfig(configFilePath, typeof(WcfAddressConfigInfo));
        }
    }

    #endregion
}
