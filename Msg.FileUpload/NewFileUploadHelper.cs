using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Msg.Config;
using Msg.Utils.Cryptography;
using WCF.Lib.File.Entity;

namespace Msg.FileUpload
{
    #region 返回消息
    /// <summary>
    /// 返回消息
    /// </summary>
    public class ReturnMessage
    {
        /// <summary>
        /// 状态
        /// true 上传成功
        /// false 上传失败
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileSrc { get; set; }
        /// <summary>
        /// 原始文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// 图片高度（仅返回原图高度，其他返回值为Null）
        /// </summary>
        public int? Height { get; set; }
    }
    #endregion

    /// <summary>
    /// 上传文件操作类 2011-10-13
    /// </summary>
    public class NewFileUploadHelper
    {
        private static readonly string Key = ConfigurationManager.AppSettings["Key"].ToString();

        #region 保存学校Logo
        /// <summary>
        /// 保存学校Logo
        /// </summary>
        /// <returns></returns>
        public static bool SaveSchoolLogo(int schoolId, byte[] buff)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;


            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();


            var result = gc.SaveSchoolLogo(schoolId, buff);
            proxy.Dispose();

            return result;
        }
        #endregion

        #region 保存缩略图
        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="attachmen"></param>
        /// <returns></returns>
        public static ReturnMessage SaveSmailImage(byte[] buff, int quality = 80)
        {
            var msg = new ReturnMessage();
            try
            {
                string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

                msg.Extension = ".jpg";

                var FileData = buff;

                msg.FileSize = FileData.Length;

                string cert = msg.Extension + msg.FileSize + "" + Key;
                string salt = Crypto.MD5(cert);

                var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

                var proxy = new NewFileUploadProxy(binding, remoteAddress);

                var gc = proxy.CreateChannel();

                var attachments = new NewAttachmentsEntity();
                attachments.Salt = salt;
                attachments.Category = "";
                attachments.FileData = FileData;
                attachments.Extension = msg.Extension;

                var result = new NewMessage();
                if (quality != 80)
                {
                    result = gc.SaveSmailImageByQuality(attachments, quality);
                }
                else
                {
                    result = gc.SaveSmailImage(attachments);
                }

                msg.Status = result.Status;
                msg.Error = result.Error;
                msg.FileSrc = result.FileName;
                proxy.Dispose();
            }
            catch (Exception)
            {

            }


            return msg;
        }
        #endregion

        #region 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="widht">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByHeightAndWidth(HttpPostedFileBase file, int widht, int height)
        {
            return UploadAndMakeSmailImageByHeightAndWidth(file, widht, height, 0);
        }
        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="widht">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByHeightAndWidth(HttpPostedFileBase file, int width, int height, int root)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadAndMakeSmailImageByHeightAndWidth(attachments, height, width);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }

        /// <summary>
        /// 上传图片并缩略图片再按指定宽高剪裁图片
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="widht">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns></returns>
        public static ReturnMessage UploadAndMakeSquareImage(HttpPostedFileBase file, int width, int height, int root)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadAndMakeSquareImage(attachments, width, height);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }


        /// <summary>
        /// 上传图片并生成4种不同大小的图片（酷活动专用）
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="widht">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns>返回消息</returns>
        public static IList<ReturnMessage> UploadAndMakeMultiImages(byte[] jpgContent, int root)
        {
            var msgOriginal = new ReturnMessage();
            var msgSmall = new ReturnMessage();
            var msgBig = new ReturnMessage();
            var msgLittle = new ReturnMessage();

            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            //msgOriginal.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msgOriginal.Extension = ".jpg";

            //int len = file.ContentLength;
            //byte[] buff = new byte[len];
            //file.InputStream.Read(buff, 0, len);
            var FileData = jpgContent;

            msgOriginal.FileSize = FileData.Length;

            string cert = msgOriginal.Extension + msgOriginal.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msgOriginal.Extension;

            //调动上传和生成方法
            var result = gc.UploadFile(attachments);//原图
            //var result2 = gc.UploadAndMakeSmailImageNoFillWidthByHeightAndWidth(attachments, 120, 160);//160*120
            var reuslt3 = gc.UploadAndMakeSmailImageByMaxWidth(attachments, 210);//不限*210
            //var reuslt4 = gc.UploadAndMakeSquareImage(attachments, 50, 50);

            msgOriginal.Status = result.Status;
            msgOriginal.Error = result.Error;
            msgOriginal.FileSrc = result.FileName;

            //msgSmall.Status = result2.Status;
            //msgSmall.Error = result2.Error;
            //msgSmall.FileSrc = result2.FileName;

            msgBig.Status = reuslt3.Status;
            msgBig.Error = reuslt3.Error;
            msgBig.FileSrc = reuslt3.FileName;
            msgBig.Height = reuslt3.Height;

            //msgLittle.Status = reuslt4.Status;
            //msgLittle.Error = reuslt4.Error;
            //msgLittle.FileSrc = reuslt4.FileName;

            IList<ReturnMessage> listMsg = new List<ReturnMessage>();
            listMsg.Add(msgOriginal);
            //listMsg.Add(msgSmall);
            listMsg.Add(msgBig);
            //listMsg.Add(msgLittle);
            proxy.Dispose();
            return listMsg;
        }

        /// <summary>
        /// 上传图片并生成4种不同大小的图片（酷乐团专用）
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="widht">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns>返回消息</returns>
        public static IList<ReturnMessage> UploadAndMakeMultiImages_Tuan(HttpPostedFileBase file, int root)
        {
            var msgThumb = new ReturnMessage();
            var msgList = new ReturnMessage();
            var msgGuide = new ReturnMessage();
            var msgOrder = new ReturnMessage();

            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msgThumb.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msgThumb.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msgThumb.FileSize = FileData.Length;

            string cert = msgThumb.Extension + msgThumb.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msgThumb.Extension;

            //调动上传和生成方法
            var result = gc.UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(attachments, 335, 435);//详细页
            //var result2 = gc.UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(attachments, 185, 300);//300*185
            //var reuslt3 = gc.UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(attachments, 118, 190);//190*118
            //var reuslt4 = gc.UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(attachments, 76, 100);//76*100

            msgThumb.Status = result.Status;
            msgThumb.Error = result.Error;
            msgThumb.FileSrc = result.FileName;

            //msgList.Status = result2.Status;
            //msgList.Error = result2.Error;
            //msgList.FileSrc = result2.FileName;

            //msgGuide.Status = reuslt3.Status;
            //msgGuide.Error = reuslt3.Error;
            //msgGuide.FileSrc = reuslt3.FileName;

            //msgOrder.Status = reuslt4.Status;
            //msgOrder.Error = reuslt4.Error;
            //msgOrder.FileSrc = reuslt4.FileName;

            IList<ReturnMessage> listMsg = new List<ReturnMessage>();
            listMsg.Add(msgThumb);
            //listMsg.Add(msgList);
            //listMsg.Add(msgGuide);
            //listMsg.Add(msgOrder);
            proxy.Dispose();
            return listMsg;
        }

        #endregion

        #region 根据指定最大宽高，生成与原图同比例的缩略图
        /// <summary>
        /// 根据指定最大宽高，生成与原图同比例的缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth(HttpPostedFileBase file, int maxWidht, int maxHeight)
        {
            return UploadAndMakeSmailImageByMaxHeightAndMaxWidth(file, maxWidht, maxHeight, 0);
        }
        /// <summary>
        /// 根据指定最大宽高，生成与原图同比例的缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth(HttpPostedFileBase file, int maxWidth, int maxHeight, int root)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt =Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadAndMakeSmailImageByMaxHeightAndMaxWidth(attachments, maxHeight, maxWidth);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            msg.Height = maxHeight;
            proxy.Dispose();
            return msg;
        }
        #endregion

        #region 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// <summary>
        /// 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByMaxWidth(HttpPostedFileBase file, int maxWidht)
        {
            return UploadAndMakeSmailImageByMaxWidth(file, maxWidht, 0);
        }
        /// <summary>
        /// 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="root"> 主目录 0:upload 1:bbs 2:school</param>
        /// <returns>返回消息</returns>
        public static ReturnMessage UploadAndMakeSmailImageByMaxWidth(HttpPostedFileBase file, int maxWidht, int root)
        {
            var msg = new ReturnMessage();
            string remoteAddress =WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt =Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;


            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadAndMakeSmailImageByMaxWidth(attachments, maxWidht);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }
        #endregion

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ReturnMessage UploadFile(HttpPostedFileBase file)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;


            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadFile(attachments);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveSource">是否保存原图</param>
        /// <returns></returns>
        public static ReturnMessage UploadFile(HttpPostedFileBase file, bool saveSource)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;


            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;

            var result = gc.UploadFileByOriginal(attachments, saveSource);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }

        /// <summary>
        /// 上传文件(特惠淘)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static ReturnMessage UploadFile_Tuan(HttpPostedFileBase file)
        {
            var msg = new ReturnMessage();
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;

            msg.FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            msg.Extension = System.IO.Path.GetExtension(file.FileName).ToLower();

            int len = file.ContentLength;
            byte[] buff = new byte[len];
            file.InputStream.Read(buff, 0, len);
            var FileData = buff;

            msg.FileSize = FileData.Length;

            string cert = msg.Extension + msg.FileSize + "" + Key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;


            var proxy = new NewFileUploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            var attachments = new NewAttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = "";
            attachments.FileData = FileData;
            attachments.Extension = msg.Extension;
            var result = gc.UploadFile_Tuan(attachments);

            msg.Status = result.Status;
            msg.Error = result.Error;
            msg.FileSrc = result.FileName;
            proxy.Dispose();
            return msg;
        }

        public static int GetMaxWidthFileSrcAndOutHeight(string fileSrc, int maxW, int maxH)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().NewFileAddress;
            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;
            var proxy = new NewFileUploadProxy(binding, remoteAddress);
            var gc = proxy.CreateChannel();
            var record = new RecordImgMsg();
            record.fileSrc = fileSrc;
            record.maxW = maxW;
            record.maxH = maxH;
            int height = gc.GetMaxImgWidthSrcAndMaxHeight(record);
            proxy.Dispose();
            return height;
        }
    }
}
