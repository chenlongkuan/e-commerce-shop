using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     品牌管理表
    /// </summary>
    [Serializable]
    [Table("BrandsEntity")]
    public class BrandsEntity : BaseEntity<int>
    {
        public BrandsEntity()
        {
            IsUseable = true;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     品牌名称
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        /// <summary>
        ///     Logo Url
        /// </summary>
        //[Required]
        [StringLength(200)]
        public string Logo { get; set; }


        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        #region 关系

        /// <summary>
        ///     下属产品
        /// </summary>
        /// <value>
        ///     The products.
        /// </value>
        public virtual ICollection<ProductsEntity> Products { get; set; }

        #endregion
    }
}