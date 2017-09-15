using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     仓库产品表
    /// </summary>
    [Serializable]
    [Table("ProductsEntity")]
    public class ProductsEntity : BaseEntity<int>
    {
        public ProductsEntity()
        {
            IsUseable = true;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     产品名称
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        ///     入库价格
        /// </summary>
        /// <value>
        ///     The price.
        /// </value>
        public decimal Price { get; set; }

        /// <summary>
        ///     入库数量
        /// </summary>
        /// <value>
        ///     The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        ///     规格
        /// </summary>
        /// <value>
        ///     The spec.
        /// </value>
        [StringLength(2000)]
        public string Spec { get; set; }


        #region 票务相关

        /// <summary>
        /// 使用日期
        /// </summary>
        /// <value>
        /// The using date.
        /// </value>
        public DateTime? UsingDate { get; set; }

        /// <summary>
        ///场馆
        /// </summary>
        /// <value>
        /// The venue.
        /// </value>
        [StringLength(50)]
        public string Venue { get; set; }


        #endregion

        #region 旅游相关

        /// <summary>
        /// 目的地
        /// </summary>
        /// <value>
        /// The destination.
        /// </value>
        [StringLength(20)]
        public string Destination { get; set; }

        #endregion

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
        [Required]
        [StringLength(4000)]
        public string Logo { get; set; }

        /// <summary>
        /// 主要Logo 不映射到数据
        /// </summary>
        /// <value>
        /// The main logo.
        /// </value>
        [NotMapped]
        public string MainLogo
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|')[0];
                }
                else
                {
                    return Logo;
                }
            }
        }

        /// <summary>
        /// Gets the second logo.
        /// </summary>
        /// <value>
        /// The second logo.
        /// </value>
        [NotMapped]
        public string SecondLogo
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|')[1];
                }
                else
                {
                    return Logo;
                }
            }
        }

        /// <summary>
        /// Logo集合 不映射到数据
        /// </summary>
        /// <value>
        /// The logo list.
        /// </value>
        [NotMapped]
        public List<string> LogoList
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|').ToList();
                }
                else
                {
                    return new List<string>() { Logo };
                }
            }
        }


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
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }


        #region 关系

        /// <summary>
        /// Gets or sets the apply entity.
        /// </summary>
        /// <value>
        /// The apply entity.
        /// </value>
        public virtual ProductsApplyEntity ApplyEntity { get; set; }



        /// <summary>
        ///     所属类目
        /// </summary>
        /// <value>
        ///     The category.
        /// </value>
        public virtual CategoriesEntity Category { get; set; }

        /// <summary>
        ///     所属品牌
        /// </summary>
        /// <value>
        ///     The brand.
        /// </value>
        public virtual BrandsEntity Brand { get; set; }


        /// <summary>
        ///     对应已上架的产品集合
        /// </summary>
        /// <value>
        ///     The goods.
        /// </value>
        public virtual ICollection<GoodsEntity> Goods { get; set; }


        /// <summary>
        /// 对应库存的供应商（创业者）
        /// </summary>
        /// <value>
        /// The suppliers.
        /// </value>
        public virtual SuppliersEntity Suppliers { get; set; }



        #endregion
    }
}