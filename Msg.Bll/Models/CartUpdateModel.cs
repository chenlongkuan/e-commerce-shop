using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msg.Bll.Models
{
   public class CartUpdateModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        /// <value>
        /// The goods identifier.
        /// </value>
       public int GoodsId { get; set; }

       /// <summary>
       /// 购物车数量
       /// </summary>
       /// <value>
       /// The quantity.
       /// </value>
       public int Quantity { get; set; }

       /// <summary>
       /// 剩余库存
       /// </summary>
       /// <value>
       /// The left quantity.
       /// </value>
       public int LeftQuantity { get; set; }
    }
}
