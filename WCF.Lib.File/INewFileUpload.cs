using System.ServiceModel;
using WCF.Lib.File.Entity;

namespace WCF.Lib.File
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“INewFileUpload”。
    [ServiceContract]
    public interface INewFileUpload
    {
        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="attachmen"></param>
        /// <returns></returns>
        [OperationContract]
        NewMessage SaveSmailImage(NewAttachmentsEntity attachmen);

        [OperationContract]
        Entity.NewMessage SaveSmailImageByQuality(Entity.NewAttachmentsEntity attachmen, int quality);

        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="Height">高度</param>
        /// <param name="Width">宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSmailImageByHeightAndWidth(NewAttachmentsEntity attachmen, int Height, int Width);


        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，宽度小于缩放宽度的不空白填充
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="Height">高度</param>
        /// <param name="Width">宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSmailImageNoFillWidthByHeightAndWidth(NewAttachmentsEntity attachmen, int Height, int Width);

        /// <summary>
        /// 根据指定最大宽高，生成与原图同比例的缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth(NewAttachmentsEntity attachmen, int maxHeight, int maxWidth);


        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充   酷乐团用  后期添加水印
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(NewAttachmentsEntity attachmen, int maxHeight, int maxWidth);

        /// <summary>
        /// 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSmailImageByMaxWidth(NewAttachmentsEntity attachmen, int maxWidth);

        /// <summary>
        /// 上传图片并生产缩略图，按指定宽高剪裁
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="width">高度</param>
        /// <param name="height">宽度</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadAndMakeSquareImage(NewAttachmentsEntity attachmen, int width, int height);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadFile(NewAttachmentsEntity attachmen);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="saveSource">是否保存原图</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadFileByOriginal(NewAttachmentsEntity attachmen, bool saveSource);

        /// <summary>
        /// 上传文件 特惠淘
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <returns></returns>
        [OperationContract]
        NewMessage UploadFile_Tuan(NewAttachmentsEntity attachmen);
        /// <summary>
        /// 保存学校Logo
        /// </summary>
        /// <param name="schoolId">学校Id</param>
        /// <param name="buff"></param>
        /// <returns></returns>
        [OperationContract]
        bool SaveSchoolLogo(int schoolId, byte[] buff);

        [OperationContract]
        int GetMaxImgWidthSrcAndMaxHeight(RecordImgMsg record);



    }
}
