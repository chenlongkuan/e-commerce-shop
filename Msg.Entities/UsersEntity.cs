using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户表
    /// </summary>
    [Serializable]
    [Table("UsersEntity")]
    public class UsersEntity : BaseEntity<int>
    {
        public UsersEntity()
        {
            IsUseable = true;
            Credits = 0;
            LastLoginTime = DateTime.Now;
            LoginTime = DateTime.Now;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     注册邮箱
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        //[Required]
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        ///     加密的密码
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        [Required]
        [StringLength(32)]
        public string Password { get; set; }

        /// <summary>
        ///     密码盐
        /// </summary>
        /// <value>
        ///     The salt.
        /// </value>
        [Required]
        [StringLength(8)]
        public string Salt { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        /// <value>
        ///     The name of the nick.
        /// </value>
        [Required]
        [StringLength(20)]
        public string NickName { get; set; }

        /// <summary>
        ///     用户头像
        /// </summary>
        [StringLength(200)]
        public string Avatar { get; set; }

        /// <summary>
        ///     用户电话
        /// </summary>
        /// <value>
        ///     The mobile.
        /// </value>
        [StringLength(11)]
        public string Mobile { get; set; }

        /// <summary>
        ///     用户姓名
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        [StringLength(20)]
        public string UserName { get; set; }

        /// <summary>
        /// 性别 （0：男，1：女）
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public byte? Gender { get; set; }


        /// <summary>
        ///     用户状态 True:可用,False：不可用
        /// </summary>
        public bool IsUseable { get; set; }

        /// <summary>
        ///是否激活
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        ///     用户积分
        /// </summary>
        /// <value>
        ///     The credits.
        /// </value>
        public int Credits { get; set; }

        /// <summary>
        ///     上次登录时间
        /// </summary>
        /// <value>
        ///     The last login time.
        /// </value>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        ///     最新登录时间
        /// </summary>
        /// <value>
        ///     The login time.
        /// </value>
        public DateTime LoginTime { get; set; }

        /// <summary>
        ///  登陆次数
        /// </summary>
        /// <value>
        /// The login times.
        /// </value>
        public int LoginTimes { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }


        /// <summary>
        ///     链接用户标识（如：外部接口返回用户Id等）
        /// </summary>
        [StringLength(200)]
        public string ConnactUserIdentity { get; set; }

        /// <summary>
        ///     链接用户来源名称（如：QQ/Weibo）
        /// </summary>
        [StringLength(50)]
        public string ConnactUserFrom { get; set; }


        /// <summary>
        ///     微博绑定
        /// </summary>
        [StringLength(50)]
        public string AccessToKen { get; set; }

        #region 关系

        /// <summary>
        ///     所属学校
        /// </summary>
        /// <value>
        ///     The school.
        /// </value>
        public virtual SchoolEntity School { get; set; }

        /// <summary>
        ///     用户角色
        /// </summary>
        /// <value>
        ///     The role.
        /// </value>
        public virtual UserRoleMapEntity RoleMap { get; set; }

        /// <summary>
        /// Gets or sets the supplier.
        /// </summary>
        /// <value>
        /// The supplier.
        /// </value>
        public virtual SuppliersEntity Supplier { get; set; }

        /// <summary>
        ///     用户订单集合
        /// </summary>
        /// <value>
        ///     The orders.
        /// </value>
        public virtual ICollection<OrdersEntity> Orders { get; set; }

        /// <summary>
        ///     用户收货地址集合
        /// </summary>
        /// <value>
        ///     The user address.
        /// </value>
        public virtual ICollection<UserAddressEntity> UserAddress { get; set; }



        #endregion
    }
}