using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户地址表
    /// </summary>
    [Serializable]
    [Table("UserAddressEntity")]
    public class UserAddressEntity : BaseEntity<int>
    {
        /// <summary>
        ///     收货人
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ReciverName { get; set; }

        /// <summary>
        ///     收货电话
        /// </summary>
        [Required]
        [StringLength(12)]
        public string ReciverTel { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        /// <value>
        /// The name of the city.
        /// </value>
        [StringLength(20)]
        public string CityName { get; set; }

        /// <summary>
        /// 区域Id
        /// </summary>
        /// <value>
        /// The region identifier.
        /// </value>
        public int RegionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the region.
        /// </summary>
        /// <value>
        /// The name of the region.
        /// </value>
        [StringLength(20)]
        public string RegionName { get; set; }

        /// <summary>
        /// 学校Id
        /// </summary>
        /// <value>
        /// The school identifier.
        /// </value>
        public int SchoolId { get; set; }

        /// <summary>
        /// 学校名称
        /// </summary>
        /// <value>
        /// The name of the school.
        /// </value>
        [StringLength(30)]
        public string SchoolName { get; set; }

        /// <summary>
        ///     详细地址
        /// </summary>
        [Required]
        [StringLength(200)]
        public string DetailAddress { get; set; }

        /// <summary>
        ///     邮政编码
        /// </summary>
        [StringLength(6)]
        public string PostCode { get; set; }

        /// <summary>
        ///     是否为默认地址
        /// </summary>
        public bool IsDefult { get; set; }

        /// <summary>
        /// 是否可用，作用于逻辑删除
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }

        #region 关系

        /// <summary>
        ///     对应用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        #endregion
    }
}