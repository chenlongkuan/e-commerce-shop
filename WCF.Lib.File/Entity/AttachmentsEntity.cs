using System.Runtime.Serialization;

namespace WCF.Lib.File.Entity
{
    /// <summary>
    /// 附件
    /// </summary>
    [DataContract]
    public class AttachmentsEntity
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

    [DataContract]
    public class Message
    {
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
        public int Error { get; set; }
    }
}
