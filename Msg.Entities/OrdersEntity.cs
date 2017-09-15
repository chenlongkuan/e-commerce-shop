using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;
using Msg.Tools.Extensions;

namespace Msg.Entities
{
    /// <summary>
    ///     订单表
    /// </summary>
    [Serializable]
    [Table("OrdersEntity")]
    public class OrdersEntity : BaseEntity<Guid>
    {
        public OrdersEntity()
        {
            OrderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;

        }



        /// <summary>
        ///     订单编号 （格式：年月日时分秒毫秒)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单原价金额 （不包含邮费、优惠券抵扣等）
        /// </summary>
        /// <value>
        /// The order price.
        /// </value>
        public decimal OrderPrice { get; set; }

        /// <summary>
        /// 订单优惠金额 （仅包含优惠券抵扣）
        /// </summary>
        /// <value>
        /// The sale price.
        /// </value>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 显示应付金额
        /// </summary>
        /// <value>
        /// The dispaly price.
        /// </value>
        [NotMapped]
        public decimal DisplayPrice {
            get { return OrderPrice - SalePrice + ExpressCost; }
        }

        /// <summary>
        ///     邮费
        /// </summary>
        public decimal ExpressCost { get; set; }

        /// <summary>
        ///     订单状态
        /// </summary>
        /// <value>
        ///     1:未支付，2：已支付，3：正在出库，4：已发货，5：已收货,6：完成关闭，7：自助取消，8：过期关闭
        /// </value>
        public int Status { get; set; }

        /// <summary>
        ///订单状态 描述
        /// </summary>
        /// <value>
        /// The status desc.
        /// </value>
        [NotMapped]
        public string StatusDesc
        {
            get
            {
                switch (Status)
                {
                    case (int)OrderStatusEnum.UnPay:
                        return OrderStatusEnum.UnPay.ToDescription();
                    case (int)OrderStatusEnum.Payed:
                        return OrderStatusEnum.Payed.ToDescription();
                    case (int)OrderStatusEnum.Outputing:
                        return OrderStatusEnum.Outputing.ToDescription();
                    case (int)OrderStatusEnum.Sended:
                        return OrderStatusEnum.Sended.ToDescription();
                    case (int)OrderStatusEnum.Received:
                        return OrderStatusEnum.Received.ToDescription();
                    case (int)OrderStatusEnum.Done:
                        return OrderStatusEnum.Done.ToDescription();
                    case (int)OrderStatusEnum.Cancel:
                        return OrderStatusEnum.Cancel.ToDescription();
                    case (int)OrderStatusEnum.Expired:
                        return OrderStatusEnum.Expired.ToDescription();
                    default:
                        return "";
                }
            }
        }


        /// <summary>
        ///     买家留言
        /// </summary>
        [StringLength(100)]
        public string BuyerLeaveMsg { get; set; }


        /// <summary>
        ///     下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     订单确认时间（送货时间）
        /// </summary>
        public DateTime? ConfirmTime { get; set; }

        /// <summary>
        ///     支付时间（配货时间）
        /// </summary>
        public DateTime? PayTime { get; set; }

        /// <summary>
        ///     支付方式
        /// </summary>
        [Required]
        [StringLength(10)]
        public string PayWay { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        /// <value>
        /// The express way.
        /// </value>
        [Required]
        [StringLength(10)]
        public string ExpressWay { get; set; }

        /// <summary>
        ///     最后修改者
        /// </summary>
        public int LastModifyUser { get; set; }

        /// <summary>
        ///     最后修改时间（完成时间）
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        /// <summary>
        /// 用户选择的送货时间段 日期
        /// </summary>
        /// <value>
        /// The send time date.
        /// </value>
        public DateTime SendTimeDate { get; set; }

        /// <summary>
        ///用户选择的送货时间段
        /// </summary>
        /// <value>
        /// The send time.
        /// </value>
        [Required]
        [StringLength(30)]
        public string SendTimeBuckets { get; set; }

        #region 关系

        /// <summary>
        ///     对应用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        /// <summary>
        /// 对应收货地址
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public virtual UserAddressEntity Address { get; set; }

        /// <summary>
        /// 订单对应商品
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public virtual ICollection<OrderItemsEntity> Items { get; set; }

        /// <summary>
        /// 优惠券使用记录
        /// </summary>
        /// <value>
        /// The coupon cost logs.
        /// </value>
        public virtual CouponCostLogsEntity CouponCostLogs { get; set; }

        #endregion
    }

    /// <summary>
    /// 订单状态枚举
    /// </summary>
    public enum OrderStatusEnum
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [Description("等待付款")]
        UnPay = 1,
        /// <summary>
        /// 已支付
        /// </summary>
        [Description("已支付")]
        Payed = 2,
        /// <summary>
        /// 正在出库
        /// </summary>
        [Description("正在出库")]
        Outputing = 3,
        /// <summary>
        /// 已发货
        /// </summary>
        [Description("已发货")]
        Sended = 4,
        /// <summary>
        /// 已收货
        /// </summary>
        [Description("已收货")]
        Received = 5,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Done = 6,
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 7,
        /// <summary>
        /// 已过期
        /// </summary>
        [Description("已过期")]
        Expired = 8
    }

    /// <summary>
    /// 支付方式枚举
    /// </summary>
    public enum PayWayEnum
    {
        [Description("在线支付")]
        CashOnline,
        [Description("货到付款")]
        CashDelivery
    }

    /// <summary>
    /// 配送方式枚举
    /// </summary>
    public enum ExpressWayEnum
    {
        [Description("美速购快递")]
        MsgExpress,
        [Description("自提")]
        SelfGet
    }

    /// <summary>
    /// 送货时间段
    /// </summary>
    public enum SendTimeBucketsEnum
    {
        [Description("12:30—14:00")]
        Noon,
        [Description("17:30—19:00")]
        AfterNoon
    }

    /// <summary>
    /// 送货时间段对应的结束小时
    /// </summary>
    public enum SendTimeBucketsEndTimeEnum : int
    {
      
        Noon=14,
     
        AfterNoon=19
    }
}