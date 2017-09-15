using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     积分商品表
    /// </summary>
    [Serializable]
    [Table("CreditGoodsEntity")]
    public class CreditGoodsEntity : BaseEntity<int>
    {

        public CreditGoodsEntity()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        ///     所需兑换积分
        /// </summary>
        /// <value>
        ///     The need credits.
        /// </value>
        public int NeedCredits { get; set; }

        /// <summary>
        ///     可兑换次数
        /// </summary>
        /// <value>
        ///     The exchange times.
        /// </value>
        public int ExchangeTimes { get; set; }

        /// <summary>
        ///     Gets or sets the start time.
        /// </summary>
        /// <value>
        ///     The start time.
        /// </value>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the end time.
        /// </summary>
        /// <value>
        ///     The end time.
        /// </value>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     规格
        /// </summary>
        /// <value>
        ///     The spec.
        /// </value>
        [StringLength(200)]
        public string Spec { get; set; }

        /// <summary>
        ///     描述
        /// </summary>
        /// <value>
        ///     The desc.
        /// </value>
        [StringLength(2000)]
        public string Desc { get; set; }

        /// <summary>
        ///     Gets or sets the logo.
        /// </summary>
        /// <value>
        ///     The logo.
        /// </value>
        [StringLength(200)]
        public string Logo { get; set; }

        /// <summary>
        ///     库存数量
        /// </summary>
        /// <value>
        ///     The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// 是否虚拟商品
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }


        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>
        /// The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

        #region 关系

        /// <summary>
        /// 评论
        /// </summary>
        /// <value>
        /// The comments.
        /// </value>
        public virtual ICollection<CommentsEntity> Comments { get; set; }

        /// <summary>
        ///     兑换记录
        /// </summary>
        /// <value>
        ///     The exchange logs.
        /// </value>
        public virtual ICollection<CreditsExchangeLogsEntity> ExchangeLogs { get; set; }

        #endregion
    }
}