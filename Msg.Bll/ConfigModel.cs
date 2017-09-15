using Msg.Utils;

namespace Msg.Bll
{
    public static class ConfigModel
    {



        /// <summary>
        /// 默认包邮订单金额
        /// </summary>
        /// <value>
        /// The howmuch to free express cost.
        /// </value>
        public static int HowmuchToFreeExpressCost
        {
            get { return ConfigHelper.GetConfigInt("HowmuchToFreeExpressCost"); }
        }

        /// <summary>
        /// 默认邮费
        /// </summary>
        /// <value>
        /// The default express cost.
        /// </value>
        public static int DefaultExpressCost
        {
            get { return ConfigHelper.GetConfigInt("defaultExpressCost"); }
        }
    }
}
