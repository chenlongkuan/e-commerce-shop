using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using WCF.Lib.File.Entity;
using WCF.Lib.File.Helper;

namespace WCF.Lib.File
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Upload : IUpload
    {
        //private static readonly string SavePath = ConfigurationManager.AppSettings["Path"].ToString();
        public static readonly string SavePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string Url = ConfigurationManager.AppSettings["Url"].ToString();
        private static readonly string Key = ConfigurationManager.AppSettings["Key"].ToString();
        private static readonly int maxWidth = int.Parse(ConfigurationManager.AppSettings["intWidth"].ToString());

        /// <summary>
        /// 目录
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private string GetCategory(AttachmentsEntity att)
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

            /// <summary>
            /// 验证来源
            /// </summary>
            /// <param name="attachmen"></param>
            /// <returns></returns>
            private bool Validate(AttachmentsEntity attachmen)
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

        #region 接口实现
        /// <summary>
        /// 上载图片(旧接口)
        /// </summary>
        /// <param name="attachmen">数据包</param>
        /// <returns></returns>
        public Message UploadFile(AttachmentsEntity attachmen)
        {
            if (!Validate(attachmen))
            {
                return new Message { Error = 3 };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 4194304)
            {
                return new Message { Error = 2 };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp" || attachmen.Extension == ".zip" || attachmen.Extension == ".rar")
            {
                filename += attachmen.Extension;
            }
            else
            {
                return new Message { Error = 1 };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Message { Error = 4 };
            }

            string newFilename = save_dir + filename;
            try
            {
                ////取得上传图片高宽
                //Helper.Thumbnail.MakeThumbnailImage(attachmen.FileData, newFilename, maxWidth, maxWidth);
                //Message msg = new Message();
                //msg.Error = 0;
                //msg.FileName = filename;
                //return msg;

                using (var inputStream = Helper.Utils.BytesToStream(attachmen.FileData))
                {
                    var qulity = 100;
                    var height = 0;
                    if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                    {
                        qulity = 80;
                    }
                    if (attachmen.Extension != ".gif")
                    {
                        Helper.Utils.CompressAsJPG(inputStream, newFilename, qulity);
                    }
                    else
                    {
                        using (var fs = new FileStream(newFilename, FileMode.Create))
                        {
                            fs.Write(attachmen.FileData, 0, attachmen.FileData.Length);
                            fs.Flush();
                            fs.Close();
                            fs.Dispose();
                        }
                    }

                }


                dir = dir.Replace("\\", "/");
                Message msg = new Message();
                msg.Error = 0;
                msg.FileName = Url + dir + filename;


                return msg;
            }
            catch
            {
                return new Message { Error = 4 };
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="salt"></param>
        public int Deletefile(string filename, string salt)
        {
            if (string.IsNullOrEmpty(filename) || string.IsNullOrEmpty(salt))
            {
                return 404;
            }

            string cert = Helper.Utils.MD5(filename + Key);
            if (salt.Equals(cert))
            {
                if (filename.Contains(Url))
                {
                    string filepath = filename.Replace(Url, "");
                    filepath = filepath.Replace("/", "\\");
                    filepath = SavePath + filepath;
                    try
                    {
                        System.IO.File.Delete(filepath);
                        return 200;
                    }
                    catch
                    {
                        return 500;
                    }
                }
            }
            return 403;
        }


        /*---------------------------------------------------------------------
         * 
         * 新接口
         * 提供头像设置，相片上传
         * 
         * --------------------------------------------------------------------*/

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SetAvatar(int uid, int avatartype, byte[] buff)
        {

            if (uid > 0 && buff.Length > 0)
            {
                string dir = string.Format("{0}ucenter\\{1}\\" + "avatar" + "\\", SavePath, uid);
                string filename = string.Format("{0}h_{1}.jpg", dir, uid);
                var retFileName = string.Format(@"ucenter/{0}/" + "avatar" + "/h_{0}.jpg", uid);
                if (avatartype == 1)
                {
                    dir = SavePath + "ucenter\\realavatar\\";
                    retFileName = string.Format("ucenter/{0}/" + "realavatar" + "/h_{0}.jpg", uid);
                    filename = string.Format("{0}{1}.jpg", dir, uid);
                }

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                else
                {
                    Utils.CleanFiles(dir);
                    System.IO.Directory.CreateDirectory(dir);
                }

                using (var memory = new MemoryStream(buff))
                {
                    var original = Image.FromStream(memory);
                    try
                    {
                        original.Save(filename, ImageFormat.Jpeg);
                        return Url + retFileName;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                    finally
                    {
                        original.Dispose();
                    }
                }
            }
            return "";
        }

        public bool WeiboAvatar(int uid, string Avatarurl, out string msg)
        {
            msg = "";
            if (uid > 0)
            {
                string dir = SavePath + "ucenter\\" + uid + "\\" + "avatar" + "\\";

                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }
                try
                {
                    string filename = dir + "h_" + uid.ToString() + ".jpg";
                    WebClient c = new WebClient();
                    c.DownloadFile(Avatarurl, filename);
                    return true;
                }
                catch (Exception ex)
                {
                    msg = ex.ToString();
                    //throw;
                }

            }
            return false;
        }

        /// <summary>
        /// 上传相册
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="albumid"></param>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Message SavePhoto(int uid, int albumid, byte[] data, string filename)
        {
            if (data.Length == 0 || data.Length > 4194304)
            {
                return new Message { Error = 2 };
            }

            string[] arr = { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
            string ext = System.IO.Path.GetExtension(filename);
            if (!arr.Contains(ext))
            {
                return new Message { Error = 1 };
            }

            //保存目录
            string dir = SavePath + "ucenter\\" + uid + "\\album\\" + albumid + "\\";
            if (!Helper.Utils.CreateDir(dir))
            {
                return new Message { Error = 4 };
            }

            string newFilename = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
            try
            {
                //取得上传图片高宽
                string savePath = dir + "\\" + newFilename;
                int i = 0;
                while (System.IO.File.Exists(savePath))
                {
                    newFilename = DateTime.Now.ToString("yyyyMMddHHmmss") + i + ext;
                    savePath = dir + "\\" + newFilename;
                    i++;
                }

                Helper.Thumbnail.SavePhoto(data, savePath, maxWidth, maxWidth);

                Message msg = new Message();
                msg.Error = 0;
                msg.FileName = newFilename;

                return msg;
            }
            catch (Exception ex)
            {
                return new Message { Error = 4, FileName = ex.Message };
            }
        }

        /// <summary>
        /// 移动相片
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="newfilepath"></param>
        /// <returns></returns>
        public bool MovePhoto(int uid, int albumid, string filename, int newAlbumid)
        {
            string path = SavePath + "ucenter\\" + uid + "\\album\\" + albumid + "\\" + filename;
            string newpath = SavePath + "ucenter\\" + uid + "\\album\\" + newAlbumid + "\\" + filename;

            try
            {
                if (!Helper.Utils.CreateDir(SavePath + "ucenter\\" + uid + "\\album\\" + newAlbumid + "\\"))
                {
                    return false;
                }

                if (System.IO.File.Exists(path))
                {
                    System.IO.Directory.Move(path, newpath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除相片
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="albumid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool deletePhoto(int uid, int albumid, string filename)
        {
            string filePath = SavePath + "ucenter\\" + uid + "\\album\\" + albumid + "\\" + filename;
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 删除相册
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="albumid"></param>
        /// <returns></returns>
        public bool deleteAlbum(int uid, int albumid)
        {
            string dir = SavePath + "ucenter\\" + uid + "\\album\\" + albumid;
            try
            {
                if (System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.Delete(dir, true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 上传图片并生成缩略图
        /// </summary>
        /// <param name="attachmen">附件</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <returns></returns>
        public Message UploadMakeThumbnailImage(AttachmentsEntity attachmen, int maxHeight, int maxWidth)
        {
            if (!Validate(attachmen))
            {
                return new Message { Error = 3 };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 5242880)
            {
                return new Message { Error = 2 };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                filename += attachmen.Extension;
            }
            else
            {
                return new Message { Error = 1 };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Message { Error = 4 };
            }

            string newFilename = save_dir + filename;

            using (MemoryStream memory = new MemoryStream(attachmen.FileData))
            {
                Image original = Image.FromStream(memory);
                Helper.Thumbnail.MakeSquareImage(original, newFilename, maxWidth, maxHeight);
            }

            dir = dir.Replace("\\", "/");

            Message msg = new Message();
            msg.Error = 0;
            msg.FileName = Url + dir + filename;
            return msg;
        }


        #region 根据最大宽度生产缩略图，按比例缩放

        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放(加了水印)
        /// </summary>
        /// <param name="attachmen">附件</param> 
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="logo">水印图片 如1.jpg</param>
        public Message MakeSmailImageByMaxWidthLogo(AttachmentsEntity attachmen, int maxWidth, string logo)
        {
            if (!Validate(attachmen))
            {
                return new Message { Error = 3 };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 6194304)
            {
                return new Message { Error = 2 };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                if (attachmen.Extension == ".png")
                {
                    filename += ".jpg";
                }
                else
                {
                    filename += attachmen.Extension;
                }

            }
            else
            {
                return new Message { Error = 1 };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Message { Error = 4 };
            }

            string newFilename = save_dir + filename;
            if (attachmen.Extension != ".gif")
            {
                using (var memory = new MemoryStream(attachmen.FileData))
                {
                    Image original = Image.FromStream(memory);
                    var quality = 100;
                    if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                    {
                        quality = 80;
                    }
                    Helper.NewThumbnail.MakeSmailImageByMaxWidth(original, newFilename, maxWidth, quality);
                }
            }
            else
            {
                using (var fs = new FileStream(newFilename, FileMode.Create))
                {
                    fs.Write(attachmen.FileData, 0, attachmen.FileData.Length);
                }
            }

            dir = dir.Replace("\\", "/");
            var msg = new Message();
            msg.Error = 0;
            msg.FileName = Url + dir + filename;


            //水印
            MarkWater(Url + dir + filename, logo);

            return msg;
        }


        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="attachmen">附件</param> 
        /// <param name="maxWidth">最大宽度</param>
        public Message MakeSmailImageByMaxWidth(AttachmentsEntity attachmen, int maxWidth)
        {
            if (!Validate(attachmen))
            {
                return new Message { Error = 3 };
            }

            if (attachmen.FileData == null || attachmen.FileData.Length > 6194304)
            {
                return new Message { Error = 2 };
            }

            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + Utils.RanNum();
            if (attachmen.Extension == ".jpg" || attachmen.Extension == ".jpeg" || attachmen.Extension == ".gif" || attachmen.Extension == ".png" || attachmen.Extension == ".bmp")
            {
                if (attachmen.Extension == ".png")
                {
                    filename += ".jpg";
                }
                else
                {
                    filename += attachmen.Extension;
                }

            }
            else
            {
                return new Message { Error = 1 };
            }

            //保存目录
            string dir = GetCategory(attachmen);
            string save_dir = SavePath + dir;
            if (!Helper.Utils.CreateDir(save_dir))
            {
                return new Message { Error = 4 };
            }

            string newFilename = save_dir + filename;
            if (attachmen.Extension != ".gif")
            {
                using (var memory = new MemoryStream(attachmen.FileData))
                {
                    Image original = Image.FromStream(memory);
                    var quality = 100;
                    if (attachmen.FileData.Length > 214688)//文件大于200K则降低质量，减小文件大小
                    {
                        quality = 80;
                    }
                    Helper.NewThumbnail.MakeSmailImageByMaxWidth(original, newFilename, maxWidth, quality);
                }
            }
            else
            {
                using (var fs = new FileStream(newFilename, FileMode.Create))
                {
                    fs.Write(attachmen.FileData, 0, attachmen.FileData.Length);
                }
            }

            dir = dir.Replace("\\", "/");
            var msg = new Message();
            msg.Error = 0;
            msg.FileName = Url + dir + filename;

            return msg;
        }
        #endregion

        /// <summary> 
        /// 给图片上水印 
        /// </summary> 
        /// <param name="filePath">原图片地址</param> 
        /// <param name="waterFile">水印图片地址</param> 
        static public void MarkWater(string filePath, string waterFile)
        {
            waterFile = ConfigurationManager.AppSettings["ImgUrl"] + "/upload/" + waterFile;
            filePath = filePath.Replace("http://file.meisugou.com", ConfigurationManager.AppSettings["ImgUrl"]);
            int i = filePath.LastIndexOf(".");
            string ex = filePath.Substring(i, filePath.Length - i);
            //GIF不水印 
            if (string.Compare(ex, ".gif", true) == 0)
            {
                return;
            }
            string ModifyImagePath = filePath;//修改的图像路径 
            int lucencyPercent = 25;
            Image modifyImage = null;
            Image drawedImage = null;
            Graphics g = null;
            try
            {
                //建立图形对象 
                modifyImage = Image.FromFile(ModifyImagePath, true);
                drawedImage = Image.FromFile(waterFile, true);
                if (modifyImage.Width >= 170 && modifyImage.Height >= 80)
                {
                    g = Graphics.FromImage(modifyImage);
                    //获取要绘制图形坐标 
                    int x = modifyImage.Width - drawedImage.Width;
                    int y = modifyImage.Height - drawedImage.Height;
                    //设置颜色矩阵 
                    float[][] matrixItems ={ 
new float[] {1, 0, 0, 0, 0}, 
new float[] {0, 1, 0, 0, 0}, 
new float[] {0, 0, 1, 0, 0}, 
new float[] {0, 0, 0, (float)lucencyPercent/100f, 0}, 
new float[] {0, 0, 0, 0, 1}};
                    ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                    ImageAttributes imgAttr = new ImageAttributes();
                    imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    //绘制阴影图像 
                    g.DrawImage(drawedImage, new Rectangle(x, y, drawedImage.Width, drawedImage.Height), 0, 0, drawedImage.Width, drawedImage.Height, GraphicsUnit.Pixel);
                    //保存文件 
                    string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" };
                    FileInfo fi = new FileInfo(ModifyImagePath);
                    ImageFormat imageType = ImageFormat.Gif;
                    switch (fi.Extension.ToLower())
                    {
                        case ".jpg":
                            imageType = ImageFormat.Jpeg;
                            break;
                        case ".gif":
                            imageType = ImageFormat.Gif;
                            break;
                        case ".png":
                            imageType = ImageFormat.Png;
                            break;
                        case ".bmp":
                            imageType = ImageFormat.Bmp;
                            break;
                        case ".tif":
                            imageType = ImageFormat.Tiff;
                            break;
                        case ".wmf":
                            imageType = ImageFormat.Wmf;
                            break;
                        case ".ico":
                            imageType = ImageFormat.Icon;
                            break;
                        default:
                            break;
                    }
                    MemoryStream ms = new MemoryStream();
                    modifyImage.Save(ms, imageType);
                    byte[] imgData = ms.ToArray();
                    modifyImage.Dispose();
                    drawedImage.Dispose();
                    g.Dispose();
                    FileStream fs = null;
                    System.IO.File.Delete(ModifyImagePath);
                    fs = new FileStream(ModifyImagePath, FileMode.Create, FileAccess.Write);
                    if (fs != null)
                    {
                        fs.Write(imgData, 0, imgData.Length);
                        fs.Close();
                    }
                }
            }
            finally
            {
                try
                {
                    drawedImage.Dispose();
                    modifyImage.Dispose();
                    g.Dispose();
                }
                catch { ;}
            }
        }
    }
}
