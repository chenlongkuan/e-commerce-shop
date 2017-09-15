using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Msg.Web.Areas.Admin.Models
{

    [XmlRoot("Root")]
    public class Slide : ICloneable
    {
        /// <summary>
        /// Id
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [XmlAttribute("OrderNo")]
        public int OrderNo { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlAttribute("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [XmlAttribute("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [XmlAttribute("Image")]
        public string Image { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlAttribute("Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 子项
        /// </summary>
        public List<Slide> SubSlides { get; set; }

        /// <summary>
        /// 广告位置名称
        /// </summary>
        public string AdSiteName { get; set; }

        /// <summary>
        /// 对应文件名
        /// </summary>
        public string FileName { get; set; }


        /// <summary>
        /// 获取根
        /// </summary>
        /// <returns></returns>
        public static Slide GetRoot()
        {
            Slide slide = null;
            var path = string.Format("{0}{1}", HttpRuntime.AppDomainAppPath, "AdScript\\index_slide.xml");
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var xs = new XmlSerializer(typeof(Slide));
                    slide = (Slide)xs.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {

            }
            return slide;
        }



        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var slide = new Slide
                {
                    Id = Id,
                    OrderNo = OrderNo,
                    Title = Title,
                    Url = Url,
                    Image = Image,
                    Remark = Remark
                };

            if (SubSlides != null && SubSlides.Any())
            {
                slide.SubSlides = new List<Slide>();
                foreach (var m in SubSlides)
                {
                    slide.SubSlides.Add((Slide)m.Clone());
                }
            }
            return slide;
        }
    }

}