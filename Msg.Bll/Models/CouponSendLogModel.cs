using Msg.Entities;

namespace Msg.Bll.Models
{
    public class CouponSendLogModel
    {
        public int Id { get; set; }

        public string CouponName { get; set; }

        public string Logo { get; set; }

        /// <summary>
        ///     优惠券类型
        /// </summary>
        /// <value>
        ///     1：现金抵扣券，2：满省抵扣券
        /// </value>
        public CouponTypeEnum Type { get; set; }

        /// <summary>
        ///     优惠券价值
        /// </summary>
        /// <value>
        ///     The coupon value.
        /// </value>
        public decimal CouponValue { get; set; }


        /// <summary>
        ///     发送数量
        /// </summary>
        /// <value>
        ///     The number.
        /// </value>
        public int Num { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        /// <value>
        /// The left number.
        /// </value>
        public int LeftNum { get; set; }
    }
}
