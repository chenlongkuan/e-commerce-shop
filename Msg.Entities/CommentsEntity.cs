using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户评论表
    /// </summary>
    [Serializable]
    [Table("CommentsEntity")]
    public class CommentsEntity : BaseEntity<int>
    {
        public CommentsEntity()
        {

            CreateTime = DateTime.Now;
        }

        /// <summary>
        ///  评论内容
        /// </summary>
        [Required]
        [StringLength(300)]
        public string Content { get; set; }

        /// <summary>
        ///     评论分值
        /// </summary>
        public int Score { get; set; }

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
        ///     所属商品
        /// </summary>
        public virtual GoodsEntity Goods { get; set; }

        /// <summary>
        ///所属积分商品
        /// </summary>
        /// <value>
        /// The credit goods.
        /// </value>
        public virtual CreditGoodsEntity CreditGoods { get; set; }

        /// <summary>
        /// Gets or sets the follow comments.
        /// </summary>
        /// <value>
        /// The follow comments.
        /// </value>
        public virtual ICollection<CommentsFollowEntity> FollowComments { get; set; }

        #endregion
    }
}