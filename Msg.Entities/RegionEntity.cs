using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Msg.Tools;

namespace Msg.Entities
{
    /// <summary>
    ///     区域表
    /// </summary>
    [Serializable]
    [Table("RegionEntity")]
    public class RegionEntity : BaseEntity<int>
    {
        public RegionEntity()
        {
            IsUsable = true;
            CreateTime = DateTime.Now;
        }


        /// <summary>
        ///     区域名称
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        /// <summary>
        ///     区域名称拼音简码
        /// </summary>
        [Required]
        [StringLength(10)]
        public string PinyinCode { get; set; }

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
        ///     下属学校
        /// </summary>
        public virtual ICollection<SchoolEntity> Schools { get; set; }


        #endregion
    }
}