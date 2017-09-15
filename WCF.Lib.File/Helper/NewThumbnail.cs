using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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
    /// 缩略图
    /// </summary>
    public class NewThumbnail
    {
        #region 保存图片
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        public static void SaveImage(Image image, string savePath, ImageCodecInfo ici,int quality)
        {
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }
             
            var size = 0;
            //设置 原图片 对象的 EncoderParameters 对象
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)quality));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
            image.Dispose();
        }
        #endregion

        #region 获取图像编码解码器的所有相关信息

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }
        #endregion

        #region 得到图片格式
        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
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

        #region 生成指定大小的缩略图，图片按比例缩放，空白填充

        /// <summary>
        /// 生成指定大小的缩略图，图片按比例缩放，宽度小于缩放宽度的不空白填充
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="newFileName">文件名</param>
        /// <param name="Width">指定宽度</param>
        /// <param name="Height">指定高度</param>
        public static void MakeSmailImageNoFillWidthByHeightAndWidth(Image image, string newFileName, int Width, int Height, int quality)
        {
            int towidth = Width;//新图宽
            int toheight = Height;//新图高

            int ow = image.Width;//原图宽
            int oh = image.Height;//原图高

            if (ow <= Width && oh <= Height)//不需要缩略图
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                float kbl = (float)ow / (float)Width;//按宽度缩放比例
                float gbl = (float)oh / (float)Height;//按高度缩放比例

                if (kbl > gbl)
                {
                    towidth = (int)(ow / kbl);
                    toheight = (int)(oh / kbl);
                }
                else
                {
                    towidth = (int)(ow / gbl);
                    toheight = (int)(oh / gbl);
                }
            }
            Bitmap b = new Bitmap(Width, Height);
            if (towidth < Width)
            {
                b = new Bitmap(towidth, Height);

            }
            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);

                float x = 0;//坐标x
                float y = 0;//坐标y

                if (towidth < toheight)
                {
                    x = (Width - towidth) / (float)(2.0);
                    y = 0;
                }
                else
                {
                    x = 0;
                    y = (Height - toheight) / (float)(2.0);
                }

                g.DrawImage(image, new Rectangle(0, (int)y, towidth, toheight), new Rectangle((int)x, (int)y, ow, oh), GraphicsUnit.Pixel);
                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()),quality);

            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }


        /// <summary>
        /// 生成指定大小的缩略图，图片按比例缩放，空白填充
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="newFileName">文件名</param>
        /// <param name="Width">指定宽度</param>
        /// <param name="Height">指定高度</param>
        public static void MakeSmailImageByHeightAndWidth(Image image, string newFileName, int Width, int Height, int quality)
        {
            int towidth = Width;//新图宽
            int toheight = Height;//新图高

            int ow = image.Width;//原图宽
            int oh = image.Height;//原图高

            if (ow <= Width && oh <= Height)//不需要缩略图
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                float kbl = (float)ow / (float)Width;//按宽度缩放比例
                float gbl = (float)oh / (float)Height;//按高度缩放比例

                if (kbl > gbl)
                {
                    towidth = (int)(ow / kbl);
                    toheight = (int)(oh / kbl);
                }
                else
                {
                    towidth = (int)(ow / gbl);
                    toheight = (int)(oh / gbl);
                }
            }
            Bitmap b = new Bitmap(Width, Height);
            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);

                float x = 0;//坐标x
                float y = 0;//坐标y

                if (towidth < toheight)
                {
                    x = (Width - towidth) / (float)(2.0);
                    y = 0;
                }
                else
                {
                    x = 0;
                    y = (Height - toheight) / (float)(2.0);
                }

                g.DrawImage(image, x, y, towidth, toheight);
                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()),quality);

            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }
        #endregion

        #region 根据指定最大宽高，生成与原图同比例的缩略图

        /// <summary>
        /// 根据指定最大宽高，生成与原图同比例的缩略图
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="newFileName">文件名</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeSmailImageByMaxHeightAndMaxWidth(Image image, string newFileName, int maxWidth, int maxHeight, int quality)
        {
            int towidth = maxWidth;//新图宽
            int toheight = maxHeight;//新图高

            int ow = image.Width;//原图宽
            int oh = image.Height;//原图高

            if (ow <= maxWidth && oh <= maxHeight)//不需要缩略图
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                float kbl = (float)ow / (float)maxWidth;//按宽度缩放比例
                float gbl = (float)oh / (float)maxHeight;//按高度缩放比例

                if (kbl > gbl)
                {
                    towidth = (int)(ow / kbl);
                    toheight = (int)(oh / kbl);
                }
                else
                {
                    towidth = (int)(ow / gbl);
                    toheight = (int)(oh / gbl);
                }
            }

            Bitmap b = new Bitmap(towidth, toheight);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;


                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);

                float x = 0;//坐标x
                float y = 0;//坐标y

                g.DrawImage(image, x, y, towidth, toheight);

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()),quality);
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }
        #endregion

        #region 根据最大宽度生产缩略图，按比例缩放
        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="newFilename">文件保存路径</param>
        /// <param name="maxWidth">最大宽度</param>
        public static void MakeSmailImageByMaxWidth(Image image, string newFilename, int maxWidth,int quality)
        {
            int towidth = maxWidth;//新图宽
            int toheight = 0;//新图高

            int ow = image.Width;//原图宽
            int oh = image.Height;//原图高

            if (ow <= maxWidth)//不需要缩略图
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                float kbl = (float)ow / (float)maxWidth;//按宽度缩放比例

                towidth = (int)(ow / kbl);
                toheight = (int)(oh / kbl);
            }

            Bitmap b = new Bitmap(towidth, toheight);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);

                float x = 0;//坐标x
                float y = 0;//坐标y
                g.DrawImage(image, x, y, towidth, toheight);

                SaveImage(b, newFilename, GetCodecInfo("image/" + GetFormat(newFilename).ToString().ToLower()),quality);
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 根据最大宽度生产缩略图，按比例缩放
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newFilename"></param>
        /// <param name="maxWidth"></param>
        /// <param name="quality"></param>
        /// <param name="height">输出的图片高度</param>
        public static void MakeSmailImageByMaxWidth(Image image, string newFilename, int maxWidth, int quality,out int height)
        {
            int towidth = maxWidth;//新图宽
            int toheight = 0;//新图高

            int ow = image.Width;//原图宽
            int oh = image.Height;//原图高

            if (ow <= maxWidth)//不需要缩略图
            {
                towidth = ow;
                toheight = oh;
            }
            else
            {
                float kbl = (float)ow / (float)maxWidth;//按宽度缩放比例

                towidth = (int)(ow / kbl);
                toheight = (int)(oh / kbl);
            }

            Bitmap b = new Bitmap(towidth, toheight);

            try
            {
                Graphics g = Graphics.FromImage(b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);

                float x = 0;//坐标x
                float y = 0;//坐标y
                g.DrawImage(image, x, y, towidth, toheight);
                height = toheight;
                SaveImage(b, newFilename, GetCodecInfo("image/" + GetFormat(newFilename).ToString().ToLower()), quality);
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }

        }
        #endregion
    }
}
