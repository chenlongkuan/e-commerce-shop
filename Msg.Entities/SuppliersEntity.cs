using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     创业集市供应商表
    /// </summary>
    [Serializable]
    [Table("SuppliersEntity")]
    public class SuppliersEntity : BaseEntity<int>
    {

        public SuppliersEntity()
        {
            IsVerified = false;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     申请原因
        /// </summary>
        /// <value>
        ///     The reason.
        /// </value>
        [Required]
        [StringLength(500)]
        public string Reason { get; set; }


        /// <summary>
        ///     Gets or sets the link man.
        /// </summary>
        /// <value>
        ///     The link man.
        /// </value>
        [Required]
        [StringLength(10)]
        public string LinkMan { get; set; }


        /// <summary>
        ///     Gets or sets the link tel.
        /// </summary>
        /// <value>
        ///     The link tel.
        /// </value>
        [Required]
        [StringLength(11)]
        public string LinkTel { get; set; }


        /// <summary>
        ///    是否通过审核
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is verified; otherwise, <c>false</c>.
        /// </value>
        public bool IsVerified { get; set; }


        /// <summary>
        ///     拒绝原因
        /// </summary>
        /// <value>
        ///     The refused reason.
        /// </value>
        [StringLength(200)]
        public string RefusedReason { get; set; }


        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }



        #region 关系

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }




        /// <summary>
        /// 对应的库存申请记录
        /// </summary>
        /// <value>
        /// The products apply.
        /// </value>
        public virtual ICollection<ProductsApplyEntity> ProductsApply { get; set; }

        #endregion
    }
}