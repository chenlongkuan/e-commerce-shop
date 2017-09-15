using Msg.Tools;

namespace Msg.Entities
{
    public class OrderItemsEntity : BaseEntity<int>
    {

        /// <summary>
        ///商品数量
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        /// <value>
        /// The single price.
        /// </value>
        public decimal SinglePrice { get; set; }

        #region 关系

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public virtual OrdersEntity Order { get; set; }


        /// <summary>
        ///     对应商品
        /// </summary>
        /// <value>
        ///     The goods.
        /// </value>
        public virtual GoodsEntity Goods { get; set; }

        #endregion
    }
}
