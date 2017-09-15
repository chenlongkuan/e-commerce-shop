using System;
using System.Configuration;
using Msg.Config;
using Msg.Utils.Cryptography;
using WCF.Lib.File.Entity;

namespace Msg.FileUpload
{
    #region 附件
    /// <summary>
    /// 附件
    /// </summary>
    public class Attachments
    {
        /// <summary>
        /// 文件扩展名
        /// jepg/jpg,png,gif,rar,zip
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// 文件夹
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public int Root { get; set; }
    }
    #endregion

    /// <summary>
    /// 上传文件（AppSettings需设置Key）
    /// </summary>
    public class UploadHelper
    {
        private static readonly string key = ConfigurationManager.AppSettings["Key"].ToString();

        #region 头像

        public static string SetAvatar(int userId, byte[] jpgContent)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            var avatar = gc.SetAvatar(userId, 0, jpgContent);
           
            return avatar;
        }

        #endregion


        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="att">附件</param>
        /// <param name="result">图片地址</param>
        /// <returns>是否成功</returns>
        public static bool UploadFile(Attachments att, ref string result)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;

            string cert = att.Extension + att.FileData.Length + att.Category + key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            AttachmentsEntity attachments = new AttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = att.Category;
            attachments.FileData = att.FileData;
            attachments.Extension = att.Extension;

            Message msg = gc.UploadFile(attachments);
            switch (msg.Error)
            {
                case 0: result = msg.FileName; break;
                case 1: result = "文件格式错误!"; break;
                case 2: result = "文件尺寸超过允许范围!"; break;
                case 3: result = "非法请求"; break;
                case 4: result = "系统错误"; break;
            }

            if (msg.Error == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 上传相册图片
        /// <summary>
        /// 上传相册图片
        /// </summary>
        /// <param name="uid">用户Id</param>
        /// <param name="albumid">相册Id</param>
        /// <param name="data">图片数据</param>
        /// <param name="filename">文件名</param>
        /// <param name="result">返回值</param>
        /// <returns>是否成功</returns>
        public static bool UpLoadPhotos(int uid, int albumid, byte[] data, string filename, out string result)
        {

            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;


            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();
            Message msg = gc.SavePhoto(uid, albumid, data, filename);
            proxy.Dispose();
            result = "";
            switch (msg.Error)
            {
                case 0: result = msg.FileName; break;
                case 1: result = "文件格式错误!"; break;
                case 2: result = "文件尺寸超过允许范围!"; break;
                case 3: result = "非法请求"; break;
                case 4: result = "系统错误"; break;
            }

            if (msg.Error == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 上传图片并生成缩略图
        /// <summary>
        /// 上传图片并生成缩略图
        /// </summary>
        /// <param name="att"></param>
        /// <param name="maxHeight"></param>
        /// <param name="maxWidth"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool UploadMakeThumbnailImage(Attachments att, int maxHeight, int maxWidth, ref string result)
        {
            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;

            string cert = att.Extension + att.FileData.Length + att.Category + key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            AttachmentsEntity attachments = new AttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = att.Category;
            attachments.FileData = att.FileData;
            attachments.Extension = att.Extension;

            Message msg = gc.UploadMakeThumbnailImage(attachments, maxHeight, maxWidth);

            proxy.Dispose();

            switch (msg.Error)
            {
                case 0: result = msg.FileName; break;
                case 1: result = "文件格式错误!"; break;
                case 2: result = "文件尺寸超过允许范围!"; break;
                case 3: result = "非法请求"; break;
                case 4: result = "系统错误"; break;
            }

            if (msg.Error == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 根据最大宽度生产缩略图，按比例缩放

        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放(加了水印)
        /// </summary>
        /// <param name="att">附件</param> 
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="logo">水印图片 如1.jpg</param>
        public static bool MakeSmailImageByMaxWidthLogo(Attachments att, int maxWidth, string logo, ref string result)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;

            string cert = att.Extension + att.FileData.Length + att.Category + key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            AttachmentsEntity attachments = new AttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = att.Category;
            attachments.FileData = att.FileData;
            attachments.Extension = att.Extension;
            Message msg = gc.MakeSmailImageByMaxWidthLogo(attachments, maxWidth, logo);

            proxy.Dispose();

            switch (msg.Error)
            {
                case 0: result = msg.FileName; break;
                case 1: result = "文件格式错误!"; break;
                case 2: result = "文件尺寸超过允许范围!"; break;
                case 3: result = "非法请求"; break;
                case 4: result = "系统错误"; break;
            }

            if (msg.Error == 0)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="att">附件</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool MakeSmailImageByMaxWidth(Attachments att, int maxWidth, ref string result)
        {

            string remoteAddress = WcfAddressConfigManager.GetConfig().FileAddress;

            string cert = att.Extension + att.FileData.Length + att.Category + key;
            string salt = Crypto.MD5(cert);

            var binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;

            var proxy = new UploadProxy(binding, remoteAddress);

            var gc = proxy.CreateChannel();

            AttachmentsEntity attachments = new AttachmentsEntity();
            attachments.Salt = salt;
            attachments.Category = att.Category;
            attachments.FileData = att.FileData;
            attachments.Extension = att.Extension;
            Message msg = gc.MakeSmailImageByMaxWidth(attachments, maxWidth);

            proxy.Dispose();

            switch (msg.Error)
            {
                case 0: result = msg.FileName; break;
                case 1: result = "文件格式错误!"; break;
                case 2: result = "文件尺寸超过允许范围!"; break;
                case 3: result = "非法请求"; break;
                case 4: result = "系统错误"; break;
            }

            if (msg.Error == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

    }
}
