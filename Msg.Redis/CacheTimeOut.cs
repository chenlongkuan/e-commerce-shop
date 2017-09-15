namespace Msg.Redis
{
    /// <summary>
    /// 缓存过期时间配置
    /// </summary>
    public class CacheTimeOut
    {
        /// <summary>
        /// 一般过期时间配置
        /// </summary>
        public const int Normal = 1440*1;

        /// <summary>
        /// 用户实体
        /// </summary>
        public const int UserModel = 1440*1;

        /// <summary>
        /// 用户角色
        /// </summary>
        public const int UserRole = 1440*3;

        /// <summary>
        /// The school model
        /// </summary>
        public const int SchoolModel = 1440*3;

        /// <summary>
        /// 购物车过期时间配置
        /// </summary>
        public const int Cart = 1440*15;
    }
}
