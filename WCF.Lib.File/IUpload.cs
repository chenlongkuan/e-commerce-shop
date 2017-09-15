using System.ServiceModel;
using WCF.Lib.File.Entity;

namespace WCF.Lib.File
{
    // 注意: 如果更改此处的接口名称 "IUpload"，也必须更新 App.config 中对 "IUpload" 的引用。
    [ServiceContract]
    public interface IUpload
    {
        [OperationContract]
        Message UploadFile(AttachmentsEntity attachmen);

        [OperationContract]
        int Deletefile(string filename, string salt);

        [OperationContract]
        bool MovePhoto(int uid, int albumid, string filename, int newAlbumid);

        [OperationContract]
        string SetAvatar(int uid, int avatartype, byte[] data);

        [OperationContract]
        bool WeiboAvatar(int uid, string Avatarurl,out string msg);

        [OperationContract]
        Message SavePhoto(int uid, int albumid, byte[] data, string filename);

        [OperationContract]
        bool deletePhoto(int uid, int albumid, string filename);

        [OperationContract]
        bool deleteAlbum(int uid, int albumid);

        /// <summary>
        /// 上传图片并生成缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        [OperationContract]
        Message UploadMakeThumbnailImage(AttachmentsEntity attachmen, int maxHeight, int maxWidth);

        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放(加了水印)
        /// </summary>
        /// <param name="attachmen">附件</param> 
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="logo">水印图片 如1.jpg</param>
        [OperationContract]
        Message MakeSmailImageByMaxWidthLogo(AttachmentsEntity attachmen, int maxWidth, string logo);

        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param> 
        /// <param name="maxWidth">最大宽度</param>
        [OperationContract]
        Message MakeSmailImageByMaxWidth(AttachmentsEntity attachmen, int maxWidth);

    }
}
