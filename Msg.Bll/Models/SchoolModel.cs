using System;

namespace Msg.Bll.Models
{
    public class SchoolModel
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        /// <value>
        /// The region identifier.
        /// </value>
        public int RegionId { get; set; }

        /// <summary>
        ///     学校名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///     学校名拼音首字母
        /// </summary>
        public string SchoolFirstCode { get; set; }

        /// <summary>
        ///     是否热门学校，true：是，false：否
        /// </summary>
        public bool IsHot { get; set; }

        /// <summary>
        ///     排序编号 编号越小越排在前面
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        ///     是否可用
        /// </summary>
        public bool IsUsable { get; set; }

        /// <summary>
        ///     添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
