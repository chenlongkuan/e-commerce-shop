using System;

namespace Msg.Bll.Models
{
    public class GoodsModel 
    {
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the short title.
        /// </summary>
        /// <value>
        ///     The short title.
        /// </value>
        public string ShortTitle { get; set; }

        /// <summary>
        ///     Gets or sets the long title.
        /// </summary>
        /// <value>
        ///     The long title.
        /// </value>
        public string LongTitle { get; set; }


        /// <summary>
        ///     售卖价格
        /// </summary>
        /// <value>
        ///     The sell price.
        /// </value>
        public decimal SellPrice { get; set; }



        /// <summary>
        ///     商品描述
        /// </summary>
        /// <value>
        ///     The desc.
        /// </value>
        public string Desc { get; set; }

        /// <summary>
        ///     是否特卖
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is sale; otherwise, <c>false</c>.
        /// </value>
        public bool IsSale { get; set; }

        /// <summary>
        /// 是否虚拟物品
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual { get; set; }


        /// <summary>
        ///     邮费
        /// </summary>
        public decimal ExpressCost { get; set; }

        /// <summary>
        ///     特卖开始时间
        /// </summary>
        /// <value>
        ///     The sale start time.
        /// </value>
        public DateTime? SaleStartTime { get; set; }

        /// <summary>
        ///     特卖结束时间
        /// </summary>
        /// <value>
        ///     The sale end time.
        /// </value>
        public DateTime? SaleEndTime { get; set; }

        /// <summary>
        ///     是否线上支付
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is cash online; otherwise, <c>false</c>.
        /// </value>
        public bool IsCashOnline { get; set; }

        /// <summary>
        ///     是否货到付款
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is cash delivery; otherwise, <c>false</c>.
        /// </value>
        public bool IsCashDelivery { get; set; }

        /// <summary>
        ///     是否正在售卖
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is on selling; otherwise, <c>false</c>.
        /// </value>
        public bool IsOnSelling { get; set; }


        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }
    }
}
