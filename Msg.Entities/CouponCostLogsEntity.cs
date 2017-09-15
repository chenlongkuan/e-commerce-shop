using System;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     优惠券消费记录表
    /// </summary>
    [Serializable]
    [Table("CouponCostLogsEntity")]
    public class CouponCostLogsEntity : BaseEntity<int>
    {
        /// <summary>
        ///     消费时间
        /// </summary>
        /// <value>
        ///     The cost time.
        /// </value>
        public DateTime CostTime { get; set; }

        /// <summary>
        /// Gets or sets the cost number.
        /// </summary>
        /// <value>
        /// The cost number.
        /// </value>
        public int CostNum { get; set; }

        #region 关系

        /// <summary>
        ///     对应的发送记录
        /// </summary>
        /// <value>
        ///     The send log.
        /// </value>
        public virtual CouponSendLogsEntity SendLog { get; set; }

        /// <summary>
        ///     对应所属人
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }


        /// <summary>
        ///     对应使用的订单
        /// </summary>
        /// <value>
        ///     The order.
        /// </value>
        public virtual OrdersEntity Order { get; set; }

        #endregion
    }
}