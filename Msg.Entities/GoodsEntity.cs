using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     已上架商品表
    /// </summary>
    [Serializable]
    [Table("GoodsEntity")]
    public class GoodsEntity : BaseEntity<int>
    {
        public GoodsEntity()
        {
            CreateTime = DateTime.Now;
            IsUseable = true;
            IsOnSelling = true;

        }


        /// <summary>
        ///     Gets or sets the short title.
        /// </summary>
        /// <value>
        ///     The short title.
        /// </value>
        [Required]
        [StringLength(200)]
        public string ShortTitle { get; set; }

        /// <summary>
        ///     Gets or sets the long title.
        /// </summary>
        /// <value>
        ///     The long title.
        /// </value>
        [Required]
        [StringLength(500)]
        public string LongTitle { get; set; }

        /// <summary>
        /// 市场价格
        /// </summary>
        /// <value>
        /// The market price.
        /// </value>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 特卖价格
        /// </summary>
        /// <value>
        /// The sale price.
        /// </value>
        public decimal? SalePrice { get; set; }

        [NotMapped]
        private decimal _sellPrice { get; set; }

        /// <summary>
        ///     售卖价格
        /// </summary>
        /// <value>
        ///     The sell price.
        /// </value>
        public decimal SellPrice
        {
            get
            {
                if (IsSale && SalePrice.HasValue)
                {
                    return SalePrice.Value;
                }
                else
                {
                    return _sellPrice;
                }
            }
            set { _sellPrice = value; }
        }


        /// <summary>
        ///     商品描述
        /// </summary>
        /// <value>
        ///     The desc.
        /// </value>
        [Required]
        [StringLength(4000)]
        public string Desc { get; set; }

        /// <summary>
        ///     是否特卖
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is sale; otherwise, <c>false</c>.
        /// </value>
        public bool IsSale { get; set; }

        /// <summary>
        /// 单用户限购数量 0表示不限量
        /// </summary>
        /// <value>
        /// The limit buy count.
        /// </value>
        public int LimitBuyCount { get; set; }

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
        /// 是否推荐到首页
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is for index; otherwise, <c>false</c>.
        /// </value>
        public bool IsForIndex { get; set; }

        /// <summary>
        /// 已售数量
        /// </summary>
        /// <value>
        /// The sold count.
        /// </value>
        public int SoldCount { get; set; }

        /// <summary>
        /// 虚假已售数量
        /// </summary>
        /// <value>
        /// The fake sold count.
        /// </value>
        public int FakeSoldCount { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

        #region 关系

        /// <summary>
        ///     对应库存产品
        /// </summary>
        /// <value>
        ///     The product.
        /// </value>
        public virtual ProductsEntity Product { get; set; }



        /// <summary>
        ///     商品对应评论集合
        /// </summary>
        /// <value>
        ///     The comments.
        /// </value>
        public virtual ICollection<CommentsEntity> Comments { get; set; }

        #endregion
    }
}