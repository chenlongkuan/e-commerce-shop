using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户权限配置表
    /// </summary>
    [Serializable]
    [Table("UserRoleMapEntity")]
    public class UserRoleMapEntity : BaseEntity<int>
    {
        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

        #region 关系

        /// <summary>
        ///     权限对应管辖区域
        /// </summary>
        /// <value>
        ///     The region.
        /// </value>
        public virtual ICollection<UserRoleMapRegionEntity> RegionMaping { get; set; }

        /// <summary>
        ///     对应的角色
        /// </summary>
        /// <value>
        ///     The role.
        /// </value>
        public virtual UserRoleEntity Role { get; set; }

        /// <summary>
        ///     权限对应用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        #endregion
    }
}