using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WCF.WEB.FileService.Models
{
    /// <summary>
    /// Thumbnail 的摘要说明。
    /// </summary>
    public class Thumbnail
    {
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)100));
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

        public static byte[] MakeThumbnailImage(string filename, int maxWidth, int maxHeight)
        {
            using (Image original = Image.FromFile(filename))
            {
                Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
                Image displayImage = new Bitmap(original, _newSize);
                try
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    displayImage.Save(ms, GetFormat(filename));
                    byte[] buff = ms.ToArray();
                    ms.Dispose();
                    return buff;
                }
                catch (Exception exp)
                {
                    throw (exp);
                }
                finally
                {
                    displayImage.Dispose();
                    original.Dispose();
                }
            }
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径） </param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static byte[] MakeSquareImage(string filename, int newWidth, int newHeight)
        {
            using (Image image = Image.FromFile(filename))
            {
                int width = image.Width;
                int height = image.Height;

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

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    b.Save(ms, GetFormat(filename));
                    byte[] buff = ms.ToArray();
                    return buff;
                }
                finally
                {
                    image.Dispose();
                    b.Dispose();
                }
            }
        }
    }
}
