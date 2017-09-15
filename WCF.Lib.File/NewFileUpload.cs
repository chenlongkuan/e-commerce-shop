
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using WCF.Lib.File.Entity;
using WCF.Lib.File.Helper;

namespace WCF.Lib.File
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“NewFileUpload”。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NewFileUpload : INewFileUpload
    {
        public static readonly string SavePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string Url = ConfigurationManager.AppSettings["Url"].ToString();
        private static readonly string Key = ConfigurationManager.AppSettings["Key"].ToString();

        #region 验证来源
        /// <summary>
        /// 验证来源
        /// </summary>
        /// <param name="attachmen"></param>
        /// <returns></returns>
        private bool Validate(Entity.NewAttachmentsEntity attachmen)
        {
            string cert = attachmen.Extension + attachmen.FileData.Length.ToString() + attachmen.Category + Key;
            cert = Helper.Utils.MD5(cert);

            if (!string.IsNullOrEmpty(attachmen.Salt))
            {
                if (attachmen.Salt.Equals(cert))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 保存目录
        /// <summary>
        /// 保存目录
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private string GetCategory(Entity.NewAttachmentsEntity att)
        {
            string dir = string.Empty;
            switch (att.Root)
            {
                case 0: dir = "upload" + "\\"; break;
                case 1: dir = "bbs" + "\\"; break;
                case 2: dir = "school" + "\\" + att.Category + "\\"; break;
                case 3: dir = "group" + "\\"; break;
            }

            dir += DateTime.Now.ToString("yyyyMMdd") + "\\";
            return dir;
        }
        #endregion

        #region 保存缩略图
        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="attachmen"></param>
        /// <returns></returns>
        public Entity.NewMessage SaveSmailImage(Entity.NewAttachmentsEntity attachmen)
        {
            return SaveImageByQuality(attachmen, 80);
        }

        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="attachmen"></param>
        /// <returns></returns>
        public Entity.NewMessage SaveSmailImageByQuality(Entity.NewAttachmentsEntity attachmen,int quality)
        {
            return SaveImageByQuality(attachmen, quality);
        }

        private NewMessage SaveImageByQuality(NewAttachmentsEntity attachmen, int quality)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage {Status = false, Error = "来源地址错误"};
            }
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum() + ".jpg";

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage {Status = false, Error = "文件夹创建失败"};
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                try
                {
                    //original.Save(newFilename, ImageFormat.Jpeg);
                    /**
                        *
                        *
                        */
                    Helper.NewThumbnail.SaveImage(original, newFilename,
                                                  Helper.NewThumbnail.GetCodecInfo("image/" +
                                                                                   Helper.NewThumbnail.GetFormat(newFilename)
                                                                                         .ToString()
                                                                                         .ToLower()), quality);
                }
                catch
                {
                    return new Entity.NewMessage {Status = false, Error = "系统错误"};
                }
                finally
                {
                    original.Dispose();
                }
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }

        #endregion

        #region 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充   酷乐团用  后期添加水印
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="Height">高度</param>
        /// <param name="Width">宽度</param>
        /// <returns></returns>
        public Entity.NewMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth_Tuan(Entity.NewAttachmentsEntity attachmen, int Height, int Width)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 4194304)
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                filename += ".jpg";
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen) + string.Format("{0}×{1}\\", Width, Height);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);

                Helper.Thumbnail.CutForCustom(original, newFilename, Width, Height, 100);
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }


        /// <summary>
        /// 上传图片并生成指定大小的缩略图，图片按比例缩放，空白填充
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="Height">高度</param>
        /// <param name="Width">宽度</param>
        /// <returns></returns>
        public Entity.NewMessage UploadAndMakeSmailImageByHeightAndWidth(Entity.NewAttachmentsEntity attachmen, int Height, int Width)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 4194304)
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                filename += ".png";
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen) + string.Format("{0}×{1}\\", Width, Height);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                int quality = 100;
                if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                {
                    quality = 80;
                }
                Helper.NewThumbnail.MakeSmailImageByMaxHeightAndMaxWidth(original, newFilename, Width, Height, quality);
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }
        /// <summary>
        /// 生成指定大小的缩略图，图片按比例缩放，宽度小于缩放宽度的不空白填充
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="Width">指定宽度</param>
        /// <param name="Height">指定高度</param>
        public Entity.NewMessage UploadAndMakeSmailImageNoFillWidthByHeightAndWidth(Entity.NewAttachmentsEntity attachmen, int Height, int Width)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 4194304)
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                filename += ".jpg";
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen) + string.Format("{0}×{1}\\", Width, Height);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);

                Helper.Thumbnail.MakeSquareImage(original, newFilename, Width, Height);
                original.Dispose();
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }

        #endregion

        #region 根据指定最大宽高，生成与原图同比例的缩略图
        /// <summary>
        /// 根据指定最大宽高，生成与原图同比例的缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        public Entity.NewMessage UploadAndMakeSmailImageByMaxHeightAndMaxWidth(Entity.NewAttachmentsEntity attachmen, int maxHeight, int maxWidth)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 5242880))//图片大于5M不予上传
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                if (attachmen.Extension == ".gif")
                {
                    filename += ".png";
                }
                else
                {
                    filename += attachmen.Extension;
                }
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen) + string.Format("{0}×{1}\\", maxWidth, maxHeight);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                int quality = 100;
                if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                {
                    quality = 80;
                }
                Helper.NewThumbnail.MakeSmailImageByMaxHeightAndMaxWidth(original, newFilename, maxWidth, maxHeight, quality);
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }
        #endregion

        #region 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// <summary>
        /// 上传图片并根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        public Entity.NewMessage UploadAndMakeSmailImageByMaxWidth(Entity.NewAttachmentsEntity attachmen, int maxWidth)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 5242880))//图片大于5M不予上传
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                if (attachmen.Extension == ".gif")
                {
                    filename += ".png";
                }
                else
                {
                    filename += attachmen.Extension;
                }
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;
            int height = 0;//图片高度

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                #region 计算新高度
                var ow = original.Width;
                var oh = original.Height;
                if (ow <= maxWidth)//不需要缩略图
                {
                    height = oh;
                }
                else
                {
                    float kbl = (float)ow / (float)maxWidth;//按宽度缩放比例
                    height = (int)(oh / kbl);
                }
                #endregion

                Helper.NewThumbnail.MakeSmailImageByMaxWidth(original, newFilename, maxWidth, 80);
                original.Dispose();
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";

            msg.FileName = Url + dir + filename;
            msg.Height = height;
            return msg;
        }
        #endregion


        #region 上传图片并生产缩略图，根据指定长宽剪裁

        /// <summary>
        /// 上传图片并生产缩略图，根据指定长宽剪裁
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public Entity.NewMessage UploadAndMakeSquareImage(Entity.NewAttachmentsEntity attachmen, int width, int height)
        {
            if (!Validate(attachmen))
            {
                return new Entity.NewMessage { Status = false, Error = "来源地址错误" };
            }

            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 5242880))//图片大于5M不予上传
            {
                return new Entity.NewMessage { Status = false, Error = "文件大小错误" };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Helper.Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                if (attachmen.Extension == ".gif")
                {
                    filename += ".png";
                }
                else
                {
                    filename += attachmen.Extension;
                }
            }
            else
            {
                return new Entity.NewMessage { Status = false, Error = "文件类型错误" };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Entity.NewMessage { Status = false, Error = "文件夹创建失败" };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                Helper.Thumbnail.MakeSquareImage(original, newFilename, width, height);
                original.Dispose();
            }

            dir = dir.Replace("\\", "/");

            Entity.NewMessage msg = new Entity.NewMessage();
            msg.Status = true;
            msg.Error = "";
            msg.FileName = Url + dir + filename;

            return msg;
        }

        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <returns></returns>
        public Entity.NewMessage UploadFile(Entity.NewAttachmentsEntity attachmen)
        {
            if (!this.Validate(attachmen))
            {
                return new NewMessage { Status = false, Error = "来源地址错误" };
            }
            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 5242880))//图片大于5M不予上传
            {
                return new NewMessage { Status = false, Error = "文件大小错误" };
            }
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + (Utils.RanNum().ToString()) + attachmen.Extension;
            string dir = this.GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Utils.CreateDir(save_dir))
            {
                return new NewMessage { Status = false, Error = "文件夹创建失败" };
            }
            string newFilename = save_dir + filename;

            using (Stream inputStream = Utils.BytesToStream(attachmen.FileData))
            {
                int quality = 100;
                if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                {
                    quality = 80;
                }
                Utils.CompressAsJPG(inputStream, newFilename, quality);
            }


            dir = dir.Replace(@"\", "/");
            return new NewMessage { Status = true, Error = "", FileName = Url + dir + filename };

        }

        /// <summary>
        /// 上传文件（因为需要保存PNG图片，所以重载了该方法；增加了savaSource）
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="saveSource">是否保存原图</param>
        /// <returns></returns>
        public Entity.NewMessage UploadFileByOriginal(Entity.NewAttachmentsEntity attachmen, bool saveSource)
        {
            if (!this.Validate(attachmen))
            {
                return new NewMessage { Status = false, Error = "来源地址错误" };
            }
            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 4194304))
            {
                return new NewMessage { Status = false, Error = "文件大小错误" };
            }
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + (Utils.RanNum().ToString()) + attachmen.Extension;
            string dir = this.GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Utils.CreateDir(save_dir))
            {
                return new NewMessage { Status = false, Error = "文件夹创建失败" };
            }
            string newFilename = save_dir + filename;

            using (Stream inputStream = Utils.BytesToStream(attachmen.FileData))
            {
                if (saveSource)
                {
                    Utils.SaveAs(inputStream, newFilename);
                }
                else
                {
                    int qulity = 100;
                    if (attachmen.FileData.Length > 0xd1a8)
                    {
                        qulity = 60;
                    }
                    Utils.CompressAsJPG(inputStream, newFilename, qulity);
                }
            }


            dir = dir.Replace(@"\", "/");
            return new NewMessage { Status = true, Error = "", FileName = Url + dir + filename };
        }



        /// <summary>
        /// 上传文件 特惠淘
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <returns></returns>
        public Entity.NewMessage UploadFile_Tuan(Entity.NewAttachmentsEntity attachmen)
        {
            if (!this.Validate(attachmen))
            {
                return new NewMessage { Status = false, Error = "来源地址错误" };
            }
            if ((attachmen.FileData == null) || (attachmen.FileData.Length > 4194304))
            {
                return new NewMessage { Status = false, Error = "文件大小错误" };
            }
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + (Utils.RanNum().ToString()) + attachmen.Extension;
            string dir = this.GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Utils.CreateDir(save_dir))
            {
                return new NewMessage { Status = false, Error = "文件夹创建失败" };
            }
            string newFilename = save_dir + filename;

            using (Stream inputStream = Utils.BytesToStream(attachmen.FileData))
            {
                var img = Image.FromStream(inputStream);
                Helper.NewThumbnail.MakeSmailImageByHeightAndWidth(img, newFilename, img.Width, img.Height, 100);
            }


            dir = dir.Replace(@"\", "/");
            return new NewMessage { Status = true, Error = "", FileName = Url + dir + filename };

        }

        #endregion


        /// <summary>
        /// 保存学校Logo
        /// </summary>
        /// <param name="schoolId">学校Id</param>
        /// <param name="buff"></param>
        /// <returns></returns>
        public bool SaveSchoolLogo(int schoolId, byte[] buff)
        {
            if (schoolId > 0 && buff.Length > 0)
            {
                string dir = SavePath + "schools\\logo" + "\\";
                string filename = dir + schoolId + ".jpg";

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }

                using (MemoryStream memory = new MemoryStream(buff))
                {
                    Image original = Image.FromStream(memory);
                    try
                    {
                        //original.Save(filename, ImageFormat.Jpeg);
                        Helper.NewThumbnail.SaveImage(original, filename, Helper.NewThumbnail.GetCodecInfo("image/" + Helper.NewThumbnail.GetFormat(filename).ToString().ToLower()), 80);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                    finally
                    {
                        original.Dispose();
                    }
                }
            }
            return false;
        }

        public int GetMaxImgWidthSrcAndMaxHeight(RecordImgMsg record)
        {
            bool isbase = false;
            int outHeight = 0;

            record.fileSrc = WCF.Lib.File.Upload.SavePath + record.fileSrc;
            if (!System.IO.File.Exists(record.fileSrc))
            {
                record.fileSrc = "";
            }

            if ((record.maxW > 0 || record.maxW > 0) && !isbase)
            {
                string ext = System.IO.Path.GetExtension(record.fileSrc);
                string name = System.IO.Path.GetFileNameWithoutExtension(record.fileSrc);
                var newdir = record.fileSrc.Replace(name + ext, string.Format("{0}×{1}\\", record.maxW, record.maxH));
                WCF.Lib.File.Helper.Utils.CreateDir(newdir);

                if (ext.ToUpper() == ".GIF")
                {
                    ext = ".jpg";
                }

                var newurl = newdir + name + ext;

                if (!System.IO.File.Exists(newurl))
                {
                    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(record.fileSrc);

                    WCF.Lib.File.Helper.NewThumbnail.MakeSmailImageByMaxWidth(originalImage, newurl, record.maxW, 80, out outHeight);

                }
            }

            return outHeight;

        }

    }
}
