using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     优惠券表
    /// </summary>
    [Serializable]
    [Table("CouponsEntity")]
    public class CouponsEntity : BaseEntity<Guid>
    {
        public CouponsEntity()
        {
            //Id = Guid.NewGuid();
            CreateTime = DateTime.Now;
        }


        ///// <summary>
        /////     Gets or sets the identifier.
        ///// </summary>
        ///// <value>
        /////     The identifier.
        ///// </value>
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[StringLength(50)]
        //public new string Id { get; set; }

        /// <summary>
        ///     优惠券名称
        /// </summary>
        /// <value>
        ///     The name of the coupon.
        /// </value>
        [Required]
        [StringLength(50)]
        public string CouponName { get; set; }

        /// <summary>
        /// Gets or sets the logo.
        /// </summary>
        /// <value>
        /// The logo.
        /// </value>
        [Required]
        [StringLength(200)]
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
        ///     满多少金额
        /// </summary>
        /// <value>
        ///     The full price.
        /// </value>
        public decimal? FullPrice { get; set; }

        /// <summary>
        ///     省多少金额
        /// </summary>
        /// <value>
        ///     The reduce price.
        /// </value>
        public decimal? ReducePrice { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }


        /// <summary>
        ///     生效开始时间
        /// </summary>
        /// <value>
        ///     The start time.
        /// </value>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     生效结束时间
        /// </summary>
        /// <value>
        ///     The end time.
        /// </value>
        public DateTime EndTime { get; set; }
    }

    public enum CouponTypeEnum : byte
    {
        [Description("现金抵扣券")]
        CashDeduction = 1,
        [Description("满省抵扣券")]
        FullReduceDeduction = 2
    }

}