using System.Runtime.Serialization;

namespace WCF.Lib.File.Entity
{
    #region 上传附件

    /// <summary>
    /// 附件
    /// </summary>
    [DataContract]
    public class NewAttachmentsEntity
    {
        /// <summary>
        /// 文件扩展名
        /// jepg/jpg,png,gif,rar,zip
        /// </summary>
        [DataMember]
        public string Extension { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public byte[] FileData { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        public string Salt { get; set; }

        /// <summary>
        /// 目录
        /// </summary>
        [DataMember]
        public string Category { get; set; }

        /// <summary>
        /// 主目录
        /// 0:upload
        /// 1:bbs
        /// 2:school
        /// </summary>
        [DataMember]
        public int Root { get; set; }
    }
    #endregion

    #region 返回消息
    /// <summary>
    /// 返回消息
    /// </summary>
    [DataContract]
    public class NewMessage
    {
        /// <summary>
        /// 状态
        /// true 上传成功
        /// false 上传失败
        /// </summary>
        [DataMember]
        public bool Status { get; set; }
        /// <summary>
        /// 返回新文件名
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
        /// <summary>
        /// 返回消息
        /// 0 成功
        /// 1 文件类型错误
        /// 2 文件大小错误
        /// 3 文件字节为空
        /// 4 非法数据
        /// 5 系统错误
        /// </summary>
        [DataMember]
        public string Error { get; set; }

        /// <summary>
        /// 图片高度（仅返回原图高度，其他返回值为Null）
        /// </summary>
        [DataMember]
        public int? Height { get; set; }
    }

   

    #endregion

    [DataContract]
    public class RecordImgMsg
    {
        [DataMember]
        public string fileSrc { get; set; }

        [DataMember]
        public int maxW { get; set; }

        [DataMember]
        public int maxH { get; set; }
    }
}
