using System;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户积分兑换记录表
    /// </summary>
    [Serializable]
    [Table("UserCreditsExchangeLogsEntity")]
    public class CreditsExchangeLogsEntity : BaseEntity<int>
    {
        /// <summary>
        /// 兑换数量
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// 消耗积分数量
        /// </summary>
        /// <value>
        /// The credits cost.
        /// </value>
        public int CreditsCost { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

        ///// <summary>
        /////     是否同意兑换奖品
        ///// </summary>
        ///// <value>
        /////     <c>true</c> if this instance is confirm exchange; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsConfirmExchange { get; set; }

        /// <summary>
        ///是否已发货
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is sended; otherwise, <c>false</c>.
        /// </value>
        public bool IsSended { get; set; }

        #region 关系

        /// <summary>
        ///     对应兑换的用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        /// <summary>
        ///     对应的兑换的积分商品
        /// </summary>
        /// <value>
        ///     The credit goods.
        /// </value>
        public virtual CreditGoodsEntity CreditGoods { get; set; }

        /// <summary>
        /// 实物兑换礼品的收货地址
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public virtual UserAddressEntity Address { get; set; }

        #endregion
    }
}