using System;

namespace Msg.Bll.Models
{
    public class CategoryModel
    {

        public int Id { get; set; }

        /// <summary>
        ///     类目名称
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// 是否创业集市使用
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is for market; otherwise, <c>false</c>.
        /// </value>
        public bool IsForMarket { get; set; }


        /// <summary>
        ///     Gets or sets a value indicating whether this instance is useable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is useable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseable { get; set; }

        /// <summary>
        ///     Gets or sets the create time.
        /// </summary>
        /// <value>
        ///     The create time.
        /// </value>
        public DateTime CreateTime { get; set; }

    }
}
