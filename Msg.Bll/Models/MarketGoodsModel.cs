using System.Collections.Generic;
using System.Linq;

namespace Msg.Bll.Models
{
    /// <summary>
    /// 创业集市商品实体
    /// </summary>
    public class MarketGoodsModel
    {
        public int Id { get; set; }

        public string Logo { get; set; }

        /// <summary>
        /// 主要Logo 不映射到数据
        /// </summary>
        /// <value>
        /// The main logo.
        /// </value>
        public string MainLogo
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|')[0];
                }
                else
                {
                    return Logo;
                }
            }
        }

        /// <summary>
        /// Gets the second logo.
        /// </summary>
        /// <value>
        /// The second logo.
        /// </value>
        public string SecondLogo
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|')[1];
                }
                else
                {
                    return Logo;
                }
            }
        }

        /// <summary>
        /// Logo集合 不映射到数据
        /// </summary>
        /// <value>
        /// The logo list.
        /// </value>
        public List<string> LogoList
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo) && Logo.Contains("|"))
                {
                    return Logo.Split('|').ToList();
                }
                else
                {
                    return new List<string>() { Logo };
                }
            }
        }


        public decimal SellPrice { get; set; }

        public int SoldCount { get; set; }

        public string ShortTitle { get; set; }

        public string SchoolName { get; set; }

        public string UserName { get; set; }

        public bool IsOnSelling { get; set; }
    }
}
