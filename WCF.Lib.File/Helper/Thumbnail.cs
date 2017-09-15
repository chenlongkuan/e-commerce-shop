using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace WCF.Lib.File.Helper
{
    /// </summary>    
    /// 修改记录1: SaveImage 方法
    /// 修改日期: 2012-3-22
    /// 版 本 号: 
    /// 修 改 人:陈龙宽
    /// 修改内容:图片保存质量由100修改为80
    /// </summary>        

    /// <summary>
    /// Thumbnail 的摘要说明。
    /// </summary>
    public class Thumbnail
    {
        #region Helper
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        public static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)80));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
        }

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;

            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        private static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        #endregion
        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="fileName">原图的文件路径</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newWidth, int newHeight)
        {
            Image image = Image.FromFile(fileName);

            //int i = 0;
            int width = image.Width;
            int height = image.Height;
            //if (width > height)
            //{
            //    i = height;
            //}
            //else
            //{
            //    i = width;
            //}
            Bitmap b = new Bitmap(newWidth, newHeight);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                {
                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);
                }

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(fileName).ToString().ToLower()));
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }
        /// <summary>
        /// 缩率图片按指定大小剪裁图片，不填充
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="newFileName"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        public static void MakeSquareImage(Image image, string newFileName, int newWidth, int newHeight)
        {
            int i = 0;
            int width = image.Width;
            int height = image.Height;
            if (width > height)
            {
                i = height;
            }
            else
            {
                i = width;
            }
            Bitmap b = new Bitmap(newWidth, newHeight);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                {
                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);
                }

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }


        /// <summary>
        /// 指定长宽裁剪
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="initImage">原图对象</param>
        /// <param name="fileSaveUrl">保存路径</param>
        /// <param name="maxWidth">最大宽(单位:px)</param>
        /// <param name="maxHeight">最大高(单位:px)</param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void CutForCustom(Image initImage, string fileSaveUrl, int maxWidth, int maxHeight, int quality)
        {

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= maxWidth && initImage.Height <= maxHeight)
            {
                initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //模版的宽高比例
                double templateRate = (double)maxWidth / maxHeight;
                //原图片的宽高比例
                double initRate = (double)initImage.Width / initImage.Height;

                //原图与模版比例相等，直接缩放
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.Transparent);
                    templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                    templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                //原图与模版比例不等，裁剪后缩放
                else
                {
                    //裁剪对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //定位
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                    //宽为标准进行裁剪
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化
                        pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)Math.Floor(initImage.Width / templateRate));
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        //裁剪源定位
                        fromR.X = 0;
                        fromR.Y = (int)Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                        fromR.Width = initImage.Width;
                        fromR.Height = (int)Math.Floor(initImage.Width / templateRate);

                        //裁剪目标定位
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = (int)Math.Floor(initImage.Width / templateRate);
                    }
                    //高为标准进行裁剪
                    else
                    {
                        pickedImage = new System.Drawing.Bitmap((int)Math.Floor(initImage.Height * templateRate), initImage.Height);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        fromR.X = (int)Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (int)Math.Floor(initImage.Height * templateRate);
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)Math.Floor(initImage.Height * templateRate);
                        toR.Height = initImage.Height;
                    }

                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //裁剪
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.Transparent);
                    templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                    //关键质量控制
                    //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
                    ImageCodecInfo[] icis = ImageCodecInfo.GetImageEncoders();
                    ImageCodecInfo ici = GetCodecInfo("image/jpeg");
                  
                    EncoderParameters ep = new EncoderParameters(1);
                    ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

                    //保存缩略图
                    templateImage.Save(fileSaveUrl, ici, ep);

                    //释放资源
                    templateG.Dispose();
                    templateImage.Dispose();

                    pickedG.Dispose();
                    pickedImage.Dispose();
                }
            }

            //释放资源
            initImage.Dispose();
        }


        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">原图路径</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(byte[] buff, string newFileName, int maxWidth, int maxHeight)
        {
            using (MemoryStream memory = new MemoryStream(buff))
            {
                Image original = Image.FromStream(memory);
                Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
                Image displayImage = new Bitmap(original, _newSize);
                try
                {
                    displayImage.Save(newFileName, GetFormat(newFileName));
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    original.Dispose();
                    displayImage.Dispose();
                }
            }
        }

        public static void SavePhoto(byte[] buff, string newFileName, int maxWidth, int maxHeight)
        {
            using (MemoryStream memory = new MemoryStream(buff))
            {
                Image original = Image.FromStream(memory);
                try
                {
                    SaveImage(original, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    original.Dispose();
                }
            }
        }

        public static void MakeThumbnailImage(string filename, string newFileName, int maxWidth, int maxHeight)
        {
            using (Image original = Image.FromFile(filename))
            {
                Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
                Image displayImage = new Bitmap(original, _newSize);
                try
                {
                    displayImage.Save(newFileName, GetFormat(newFileName));
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    original.Dispose();
                    displayImage.Dispose();
                }
            }
        }

    }
}
