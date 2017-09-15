using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Msg.Tools
{
    /// <summary>
    ///     可持久到数据库的领域模型的基类。
    /// </summary>
    [Serializable]
    public abstract class BaseEntity<TKey> 
    {
        #region 构造函数



        #endregion

        #region 属性

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }



        #endregion

    }



}