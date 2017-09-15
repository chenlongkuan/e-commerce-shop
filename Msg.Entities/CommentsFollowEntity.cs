using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    /// 评论的回复表
    /// </summary>
    [Serializable]
    [Table("CommentsFollowEntity")]
    public class CommentsFollowEntity : BaseEntity<int>
    {
        /// <summary>
        ///  评论内容
        /// </summary>
        [Required]
        [StringLength(300)]
        public string Content { get; set; }


        /// <summary>
        ///     评论时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        #region 关系

        /// <summary>
        ///     评论人
        /// </summary>
        public virtual UsersEntity User { get; set; }

        /// <summary>
        /// 被评论人
        /// </summary>
        /// <value>
        /// The be replyed user.
        /// </value>
        public virtual UsersEntity BeReplyedUser { get; set; }

        /// <summary>
        /// 父级评论
        /// </summary>
        /// <value>
        /// The parent comment.
        /// </value>
        public virtual CommentsEntity ParentComment { get; set; }

        #endregion
    }
}
