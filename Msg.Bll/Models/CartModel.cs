namespace Msg.Bll.Models
{
   public class CartModel
    {
       public int UserId { get; set; }

       public int GoodsId { get; set; }

       public int Quantity { get; set; }

       #region 不存储到cookie,仅查看购物车映射数据使用
    
       public string GoodsLogo { get; set; }

       public string GoodsShortTitle { get; set; }


       /// <summary>
       /// 单用户限购数量
       /// </summary>
       /// <value>
       /// The limit buy count.
       /// </value>
       public int LimitBuyCount { get; set; }

       /// <summary>
       /// 可购买数量 减去限购数量得出
       /// </summary>
       /// <value>
       /// The could buy count.
       /// </value>
       public int CouldBuyCount { get; set; }

       /// <summary>
       ///     是否线上支付
       /// </summary>
       /// <value>
       ///     <c>true</c> if this instance is cash online; otherwise, <c>false</c>.
       /// </value>
       public bool IsCashOnline { get; set; }

       /// <summary>
       ///     是否货到付款
       /// </summary>
       /// <value>
       ///     <c>true</c> if this instance is cash delivery; otherwise, <c>false</c>.
       /// </value>
       public bool IsCashDelivery { get; set; }


       /// <summary>
       /// 总价
       /// </summary>
       /// <value>
       /// The sell price.
       /// </value>
       public decimal SellPrice { get; set; }

       /// <summary>
       /// 单价
       /// </summary>
       /// <value>
       /// The single price.
       /// </value>
       public decimal SinglePrice { get; set; }

       /// <summary>
       /// 是否可下单
       /// </summary>
       /// <value>
       ///   <c>true</c> if buyable; otherwise, <c>false</c>.
       /// </value>
       public bool Buyable { get; set; }

       /// <summary>
       /// 不可下单描述
       /// </summary>
       /// <value>
       /// The buyable desc.
       /// </value>
       public string UnBuyableDesc { get; set; }

       #endregion

    }
}
