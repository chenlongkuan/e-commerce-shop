using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     产品类目表
    /// </summary>
    [Serializable]
    [Table("CategoriesEntity")]
    public class CategoriesEntity : BaseEntity<int>
    {
        public CategoriesEntity()
        {
            CreateTime = DateTime.Now;
            IsUseable = true;
            IsForMarket = false;
        }


        /// <summary>
        ///     类目名称
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        /// <summary>
        /// 是否创业集市使用
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is for market; otherwise, <c>false</c>.
        /// </value>
        public bool IsForMarket { get; set; }

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

        #region 关系

        /// <summary>
        ///     对应类目下属的产品
        /// </summary>
        /// <value>
        ///     The products.
        /// </value>
        public virtual ICollection<ProductsEntity> Products { get; set; }

        #endregion
    }

    /// <summary>
    /// 基础类目枚举
    /// </summary>
    public enum CategoriesEnum
    {
        /// <summary>
        /// 牛奶
        /// </summary>
        Milk = 1,
        /// <summary>
        /// 坚果
        /// </summary>
        Nuts = 2,
        /// <summary>
        /// 方便食品
        /// </summary>
        InstantFood = 3,
        /// <summary>
        ///冲泡饮料
        /// </summary>
        Drinks = 4,
        /// <summary>
        /// 休闲食品
        /// </summary>
        Snacks = 5,
        /// <summary>
        /// 地域特产
        /// </summary>
        Specialty = 6,
        /// <summary>
        /// 小美旅游
        /// </summary>
        Travel = 7,
        /// <summary>
        /// 小美票务
        /// </summary>
        Tickets = 8,
        /// <summary>
        /// 学子用品
        /// </summary>
        StuSupplies = 9,
        /// <summary>
        /// 小美教育
        /// </summary>
        Education=10,
        /// <summary>
        /// 日用品
        /// </summary>
        DayliUse=11,
        /// <summary>
        /// 护肤品
        /// </summary>
        SkinCare=12,
        /// <summary>
        /// 综合用品
        /// </summary>
        Integration=13
    }
}