
using System;

namespace Msg.Config
{
    #region Model

    /// <summary>
    /// WCF服务实体
    /// </summary>
    [Serializable]
    public class CacheConfigInfo : IConfigInfo
    {
        /// <summary>
        /// 缓存服务地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        public int HostPort { get; set; }

        /// <summary>
        /// 缓存时间 （分钟）
        /// </summary>
        public int timeout { get; set; }

        /// <summary>
        /// 缓存模式
        /// 1=使用
        /// 0=不使用
        /// </summary>
        public int model { get; set; }

        /// <summary>
        /// 缓存名字
        /// </summary>
        public string CacheName { get; set; }
    }

    #endregion

    #region 获取缓存类配置文件
    
    /// <summary>
    /// 签到规则配置管理
    /// 默认配置文件存放在：D:\config
    /// 默认配置文件名:WcfAddress.config
    /// </summary>
    public sealed class CacheConfigManager : ConfigManagerBase
    {
        /// <summary>
        /// 返回WCF服务实体
        /// </summary>
        /// <param name="configFilePath">配置文件物理路径</param>
        public static CacheConfigInfo GetConfig(string configFilePath = @"c:\config\Cache.config")
        {
            if (!System.IO.File.Exists(configFilePath))
            {
                CacheConfigInfo cacheinfo = new CacheConfigInfo()
                {
                    HostName = "localhost",
                    HostPort = 6379,
                    model = 1,
                    timeout = 60*24, //一天
                    CacheName = "default"
                };
                ConfigManagerBase.SaveConfig(configFilePath, cacheinfo);
                return cacheinfo;
            }

            return (CacheConfigInfo)ConfigManagerBase.LoadConfig(configFilePath, typeof(CacheConfigInfo));
        }
    }

    #endregion
}
