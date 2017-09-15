using System;

namespace Msg.Bll.Models
{
    public class BrandsModel
    {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        ///     品牌名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Logo Url
        /// </summary>
        public string Logo { get; set; }


        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
