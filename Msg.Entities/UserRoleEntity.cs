using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户角色表
    /// </summary>
    [Serializable]
    [Table("UserRoleEntity")]
    public class UserRoleEntity : BaseEntity<int>
    {
        /// <summary>
        ///     Gets or sets the name of the role.
        /// </summary>
        /// <value>
        ///     The name of the role.
        /// </value>
        public string RoleName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }

        #region 关系

        /// <summary>
        ///     角色对应的权限配置集合
        /// </summary>
        /// <value>
        ///     The user role maps.
        /// </value>
        public virtual ICollection<UserRoleMapEntity> UserRoleMaps { get; set; }

        #endregion
    }
}