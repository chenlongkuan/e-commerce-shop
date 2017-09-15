using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     优惠券发送记录
    /// </summary>
    [Serializable]
    [Table("CouponSendLogsEntity")]
    public class CouponSendLogsEntity : BaseEntity<int>
    {
        public CouponSendLogsEntity()
        {
            Num = 1;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     发送数量
        /// </summary>
        /// <value>
        ///     The number.
        /// </value>
        public int Num { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

  

        #region 关系

        /// <summary>
        ///     被发送的用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        /// <summary>
        /// 优惠券使用记录
        /// </summary>
        /// <value>
        /// The cost logs.
        /// </value>
        public virtual ICollection<CouponCostLogsEntity> CostLogs { get; set; }


        /// <summary>
        ///     对应优惠券
        /// </summary>
        /// <value>
        ///     The coupon.
        /// </value>
        public virtual CouponsEntity Coupon { get; set; }

        #endregion
    }
}