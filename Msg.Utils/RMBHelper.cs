

namespace Msg.Utils
{
    /// <summary> 
    /// Rmb 的摘要说明。 
    /// </summary> 
    public class Rmb
    {
        /// <summary>
        /// 折扣
        /// </summary>
        /// <param name="marketprice">市场价</param>
        /// <param name="mallprice">商城价</param>
        /// <returns></returns>
        public static string GetDiscount(decimal marketprice, decimal mallprice)
        {
            if ((marketprice <= 0M) || (mallprice <= 0M))
            {
                return "0折";
            }
            decimal num = (mallprice / marketprice) * 10M;
            return (num.ToString("F1") + "折");
        }
        /// <summary>
        /// 折扣
        /// </summary>
        /// <param name="marketprice">市场价</param>
        /// <param name="mallprice">商城价</param>
        /// <returns></returns>
        public static float GetDiscountReturnFloat(decimal marketprice, decimal mallprice)
        {
            if ((marketprice <= 0M) || (mallprice <= 0M))
            {
                return 0;
            }
            decimal num = (mallprice / marketprice) * 10M;
            return float.Parse(num.ToString("F1"));
        }

     
    }

}
