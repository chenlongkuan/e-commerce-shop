using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    /// 商品上下架操作日志
    /// </summary>
    [Serializable]
    [Table("GoodsOpreationLogsEntity")]
    public class GoodsOpreationLogsEntity : BaseEntity<int>
    {

        public GoodsOpreationLogsEntity()
        {
            OperateTime = DateTime.Now;
        }

        /// <summary>
        ///商品ID
        /// </summary>
        /// <value>
        /// The goods identifier.
        /// </value>
        public int GoodsId { get; set; }

        /// <summary>
        /// 操作内容
        /// </summary>
        /// <value>
        /// The content of the operation.
        /// </value>
        [Required]
        [StringLength(100)]
        public string OperationContent { get; set; }

        /// <summary>
        ///执行操作用户Id
        /// </summary>
        /// <value>
        /// The operator user identifier.
        /// </value>
        public int OperatorUserId { get; set; }

        /// <summary>
        /// Gets or sets the operate time.
        /// </summary>
        /// <value>
        /// The operate time.
        /// </value>
        public DateTime OperateTime { get; set; }


    }


    public class OperationDesc
    {
        public const string Up = "上架";

        public const string Down = "下架";
    }
}
