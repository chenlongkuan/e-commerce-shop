using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     用户交易日志表
    /// </summary>
    [Serializable]
    [Table("UserTradeLogsEntity")]
    public class UserTradeLogsEntity : BaseEntity<int>
    {
        public UserTradeLogsEntity()
        {
            OperateTime = DateTime.Now;
        }


        /// <summary>
        ///     用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        /// <value>
        ///     The name of the user nick.
        /// </value>
        [Required]
        [StringLength(30)]
        public string UserNickName { get; set; }


        /// <summary>
        ///     操作类型（0--用户消费, 1--已付款，2--已提现，3--已结算,4--充值）
        /// </summary>
        [Required]
        public int OperateType { get; set; }

        /// <summary>
        ///     交易号（支付宝）
        /// </summary>
        [StringLength(100)]
        public string TradeNo { get; set; }


        /// <summary>
        ///     买家支付宝交易账号
        /// </summary>
        /// <value>
        ///     The trade account.
        /// </value>
        [StringLength(200)]
        public string TradeAccount { get; set; }

        /// <summary>
        /// Gets or sets the trade status.
        /// </summary>
        /// <value>
        /// The trade status.
        /// </value>
        [StringLength(50)]
        public string TradeStatus { get; set; }

        /// <summary>
        ///     交易金额
        /// </summary>
        public decimal Sum { get; set; }


        /// <summary>
        ///     备注
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Desc { get; set; }


        /// <summary>
        ///     订单Id
        /// </summary>
        /// <value>
        ///     The order identifier.
        /// </value>
        [StringLength(50)]
        public string OrderId { get; set; }


        /// <summary>
        ///     操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }
    }
}