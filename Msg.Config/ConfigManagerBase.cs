using System;
using Msg.Utils;

namespace Msg.Config
{
    /// <summary>
    /// 配置文件实体接口
    /// </summary>
    public interface IConfigInfo { }

    /// <summary>
    /// 配置文件管理
    /// </summary>
    public abstract class ConfigManagerBase
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object m_lockHelper = new object();

        #region 方法

        /// <summary>
        /// 临时配置对象变量
        /// </summary>
        /// <summary>
        /// 加载(反序列化)指定对象类型的配置对象
        /// </summary>
        /// <returns>返回配置对象</returns>
        public static IConfigInfo LoadConfig(string configfilepath, Type type)
        {
            IConfigInfo m_configinfo = null;//(IConfigInfo)System.Web.HttpContext.Current.Cache[configfilepath];
            lock (m_lockHelper)
            {
                if (m_configinfo == null)
                {
                    m_configinfo = (IConfigInfo)SerializationHelper.Load(type, configfilepath);
                    //System.Web.HttpContext.Current.Cache.Insert(configfilepath, m_configinfo, new System.Web.Caching.CacheDependency(configfilepath));
                }
            }

            return m_configinfo;
        }

        /// <summary>
        /// 保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="configinfo">被保存(序列化)的对象</param>
        /// <returns></returns>
        public static bool SaveConfig(string configfilepath, object obj)
        {
            return SerializationHelper.Save(obj, configfilepath);
        }

        #endregion
    }
}
