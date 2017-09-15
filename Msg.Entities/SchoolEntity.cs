using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     学校表
    /// </summary>
    [Serializable]
    [Table("SchoolEntity")]
    public class SchoolEntity : BaseEntity<int>
    {
        public SchoolEntity()
        {
            IsHot = false;
            IsUsable = true;
            OrderNum = 0;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     学校名称
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; }


        /// <summary>
        ///     学校名拼音首字母
        /// </summary>
        [Required]
        [StringLength(1)]
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

        #region 关系

        /// <summary>
        ///     所属区域
        /// </summary>
        public virtual RegionEntity Region { get; set; }

        #endregion
    }
}