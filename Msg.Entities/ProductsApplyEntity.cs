using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    /// 创业集市库存商品申请
    /// </summary>
    [Serializable]
    [Table("ProductsApplyEntity")]
    public class ProductsApplyEntity : BaseEntity<int>
    {

        public ProductsApplyEntity()
        {
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
        /// 品牌名称
        /// </summary>
        /// <value>
        /// The name of the brand.
        /// </value>
        [Required]
        [StringLength(20)]
        public string BrandName { get; set; }

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
        /// Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }


        /// <summary>
        /// Gets or sets the apply status.
        /// </summary>
        /// <value>
        /// 1:审核中，2：申请成功，3：申请失败，4：已上架，5：已下架
        /// </value>
        public byte ApplyStatus { get; set; }

        /// <summary>
        /// 操作备注
        /// </summary>
        /// <value>
        /// The operate remark.
        /// </value>
        [StringLength(400)]
        public string OperateRemark { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }



        #region 关系


        /// <summary>
        /// 对应库存的供应商（创业者）
        /// </summary>
        /// <value>
        /// The suppliers.
        /// </value>
        public virtual SuppliersEntity Supplier { get; set; }

        /// <summary>
        /// 对应已发布的商品
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public virtual ProductsEntity Product { get; set; }

        /// <summary>
        /// 对应产品类目
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public virtual CategoriesEntity Category { get; set; }

        #endregion
    }

    public enum ApplyStatusEnum
    {
        /// <summary>
        /// 1:审核中
        /// </summary>
        Verifying=1,
        /// <summary>
        /// 2：申请成功
        /// </summary>
        ApplySuccess=2,
        /// <summary>
        ///3：申请失败
        /// </summary>
        ApplyFailed=3,
        /// <summary>
        /// 4：已上架
        /// </summary>
        GoodsUp=4,
        /// <summary>
        /// 5：已下架
        /// </summary>
        GoodsDown=5
    }
}
