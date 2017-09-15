using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;
using Msg.Tools.Extensions;

namespace Msg.Entities
{
    /// <summary>
    ///     系统通知表
    /// </summary>
    [Serializable]
    [Table("NotifiesEntity")]
    public class NotifiesEntity : BaseEntity<int>
    {
        public NotifiesEntity()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///     Gets or sets the content.
        /// </summary>
        /// <value>
        ///     The content.
        /// </value>
        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        /// <value>
        ///     The type.（0：管理员通知，1：消费通知，2：发货通知，3：优惠券通知，4：积分通知，5：评论通知）
        /// </value>
        public byte Type { get; set; }

        /// <summary>
        /// 通知类型文字描述
        /// </summary>
        /// <value>
        /// The type desc.
        /// </value>
        [NotMapped]
        public string TypeDesc
        {
            get
            {
                switch (Type)
                {
                    case (byte)NotifyTypeEnum.Admin:
                        return NotifyTypeEnum.Admin.ToDescription();
                    case (byte)NotifyTypeEnum.Payment:
                        return NotifyTypeEnum.Payment.ToDescription();
                    case (byte)NotifyTypeEnum.Deliver:
                        return NotifyTypeEnum.Deliver.ToDescription();
                    case (byte)NotifyTypeEnum.Coupon:
                        return NotifyTypeEnum.Coupon.ToDescription();
                    case (byte)NotifyTypeEnum.Credit:
                        return NotifyTypeEnum.Credit.ToDescription();
                    case (byte)NotifyTypeEnum.Comment:
                        return NotifyTypeEnum.Comment.ToDescription();
                    default:
                        return "";
                }
            }
        }

        /// <summary>
        /// 通知状态
        /// 未读：0
        /// 已读：1
        /// </summary>
        [Column("State", TypeName = "tinyint")]
        public byte State { get; set; }

        /// <summary>
        /// 发送人Id
        /// </summary>
        /// <value>
        /// The sender identifier.
        /// </value>
        public int SenderId { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

        #region 关系

        /// <summary>
        ///     对应接收的用户
        /// </summary>
        /// <value>
        ///     The user.
        /// </value>
        public virtual UsersEntity User { get; set; }

        #endregion
    }

    public enum NotifyTypeEnum
    {
        [Description("系统通知")]
        Admin = 0,
        [Description("支付通知")]
        Payment = 1,
        [Description("物流通知")]
        Deliver = 2,
        [Description("优惠通知")]
        Coupon = 3,
        [Description("积分通知")]
        Credit = 4,
        [Description("评论通知")]
        Comment = 5
    }
}