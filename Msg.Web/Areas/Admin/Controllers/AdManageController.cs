using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using Glimpse.Core.Tab.Assist;
using Msg.Web.App_Start;
using Msg.Web.Areas.Admin.Models;

namespace Msg.Web.Areas.Admin.Controllers
{
    public class AdManageController : Controller
    {
        //
        // GET: /Admin/AdManage/


        // 广告首页
        // GET: /admin/admanage/
        [AdminAuthFilter]
        public ActionResult AdManage()
        {
            string path = Server.MapPath("~/AdScript"); //"F:/Vsworkspace/Projects/Meisugou/Msg.Web/AdScript";
            var filesNameArr = Directory.GetFiles(path, "*.xml");
            var pathArr = new string[filesNameArr.Count<string>()];
            for (int i = 0; i < pathArr.Length; i++)
            {
                // var stringArr = filesNameArr[i].Split('\\');
                pathArr[i] = filesNameArr[i]; //stringArr[stringArr.Length - 1];
            }
            //var pathArr = new string[] { "~/AdScript/index_top.xml", "~/AdScript/index_slide.xml", "~/AdScript/index_buttom.xml" };

            var slidsList = new List<Slide>();

            for (int i = 0; i < pathArr.Length; i++)
            {
                ReadXmlbyFilePath(pathArr[i], ref slidsList);
            }

            // slidsList.OrderBy<Slide,>();
            //var slides = Models.Slide.GetRoot().SubSlides;
            //var idObj = slides.OrderBy(o => o.Id).LastOrDefault();
            //var id = idObj != null ? idObj.Id : 0;

            slidsList = slidsList.OrderBy(s => s.FileName).ThenBy(s => s.OrderNo).ToList();
            return View(slidsList);
        }



        /// <summary>
        /// 从XML文件中读取广告内容
        /// </summary>
        /// <param name="path">xml文件路径物理路径</param>
        /// <param name="slidsList">返回的广告ModelList</param>
        private void ReadXmlbyFilePath(string path, ref List<Slide> slidsList)
        {
            //物理路径
            var serverpath = path;
            //相对路径
            //var serverpath = Server.MapPath(path);
            try
            {
                var xml = new XmlDocument();
                xml.Load(serverpath);
                var root = xml.SelectSingleNode("Root");
                // var AdItems = xml.Value;
                if (root == null) throw new Exception("数据文件错误");
                root = root.SelectSingleNode("AdItems");
                if (root == null) throw new Exception("数据文件错误");



                var sublides = root.ChildNodes;
                for (int i = 0; i < sublides.Count; i++)
                {
                    var sModel = new Slide();
                    sModel.AdSiteName = "";
                    var xmlAttributeCollection = sublides[i].Attributes;
                    if (xmlAttributeCollection != null)
                    {
                        //sModel.Id = int.Parse(xmlAttributeCollection["Id"].Value);

                        sModel.Image = xmlAttributeCollection["Image"].Value;    //图片路径
                        sModel.Title = xmlAttributeCollection["Title"].Value;   //标题
                        sModel.OrderNo = Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value); //排序号
                        sModel.Url = xmlAttributeCollection["Url"].Value;  //链接地址
                        sModel.AdSiteName = xmlAttributeCollection["AdSiteName"].Value;  //广告位置
                        sModel.Remark = xmlAttributeCollection["Remark"].Value;  //备注
                        sModel.FileName = serverpath.Split('\\')[serverpath.Split('\\').Length - 1].Split('.')[0];


                    }
                    slidsList.Add(sModel);
                }
            }
            catch
            {
            }
        }



        /// <summary>
        /// 修改广告图片或者内容(非幻灯片)
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public JsonResult Msg_ModifyImage(Slide model)
        {
            var result = Json(new {isError = false}, JsonRequestBehavior.AllowGet);
            try
            {
                var serverpath = Server.MapPath("~/AdScript/" + model.FileName + ".xml");
                var xml = new XmlDocument();
                xml.Load(serverpath);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");
                //root = root.SelectSingleNode("AdItems");
                //if (root == null) throw new Exception("数据文件错误");


                root.RemoveAll();
                var element = xml.CreateElement("AdItems");
                var elementModel = xml.CreateElement("Item");
                elementModel.SetAttribute("Title", model.Title);
                //if (!string.IsNullOrEmpty(model.Url) && model.Url.Length > 7 && model.Url.Substring(0,6) == "http://")
                //{
                    elementModel.SetAttribute("Url", model.Url);
                //}
                //else
                //{
                //    result = Json(new { isError = true, msg = "请按照  http://  格式输入连接地址" }, JsonRequestBehavior.AllowGet);
                //    return null;
                //}
                
                elementModel.SetAttribute("Image", model.Image);
                elementModel.SetAttribute("Remark", model.Remark);
                elementModel.SetAttribute("OrderNo", model.OrderNo.ToString());
                elementModel.SetAttribute("AdSiteName", model.AdSiteName);
                elementModel.SetAttribute("FileName", model.FileName);
                element.AppendChild(elementModel);
                root.AppendChild(element);
                xml.AppendChild(root);
                xml.Save(serverpath);

                //缺少删除文件



                //var xmlColletion = root.ChildNodes;
                //for (var i = 0;i<xmlColletion.Count;i++)
                //{
                //    var xmlAttributeCollection = xmlColletion[i].Attributes;
                //    if (xmlAttributeCollection != null)
                //    {
                //        xmlAttributeCollection["Title"].InnerXml = model.Title;
                //    }
                //}


                //<Item Id="1" Title="首页底部通栏广告" OrderNo="0"   Url="http://www.9yjob.com" Image="~/Content/images/logo.png" Remark="首页顶部"   AdSiteName="首页顶部" />

                return result;
            }
            catch (Exception ex)
            {
                return   Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }




        /// <summary>
        /// 修改幻灯片内容
        /// </summary>
        /// <param name="model"> model</param>
        /// <returns></returns>
        public JsonResult Msg_ModifySlide(Slide model, int oldOrderNo)
        {
            try
            {
                var serverpath = Server.MapPath("~/AdScript/" + model.FileName + ".xml");
                var xml = new XmlDocument();
                xml.Load(serverpath);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");
                root = root.SelectSingleNode("AdItems");
                if (root == null) throw new Exception("数据文件错误");



                var sublides = root.ChildNodes;
                if (model.OrderNo == oldOrderNo)
                {
                    for (var i = 0; i < sublides.Count; i++)
                    {
                        var xmlElementAttrbuteCollections = sublides[i].Attributes;
                        //var xmlAttrbuteCollections = 
                        if (xmlElementAttrbuteCollections != null && xmlElementAttrbuteCollections["OrderNo"].Value == oldOrderNo.ToString())
                        {
                            xmlElementAttrbuteCollections["OrderNo"].InnerXml = model.OrderNo.ToString();
                            //var element = sublides[i];
                            if (xmlElementAttrbuteCollections["OrderNo"].Value != null) xmlElementAttrbuteCollections["OrderNo"].InnerXml = model.OrderNo.ToString();
                            if (xmlElementAttrbuteCollections["Title"].Value != null) xmlElementAttrbuteCollections["Title"].InnerXml = model.Title;

                            if (xmlElementAttrbuteCollections["Url"].Value != null)  xmlElementAttrbuteCollections["Url"].InnerXml = model.Url;
                            //{
                            //    if (string.IsNullOrEmpty(model.Url)&&model.Url.Length > 7 && model.Url.Substring(0, 6) == "http://")
                            //    {

                                  
                                //}
                                //else
                                //{
                                //    return Json(new { isError = true, msg = "请按照  http://  格式输入连接地址" },
                                //        JsonRequestBehavior.AllowGet);
                                //}
                            //}
                            if (xmlElementAttrbuteCollections["Image"].Value != null) xmlElementAttrbuteCollections["Image"].InnerXml = model.Image;
                            if (xmlElementAttrbuteCollections["Remark"].Value != null) xmlElementAttrbuteCollections["Remark"].InnerXml = model.Remark;
                            if (xmlElementAttrbuteCollections["AdSiteName"].Value != null) xmlElementAttrbuteCollections["AdSiteName"].InnerXml = model.AdSiteName;
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < sublides.Count; i++)
                    {
                        var xmlElement = sublides[i].Attributes;
                        if (xmlElement != null)
                        {
                            if (xmlElement["OrderNo"].Value != null && xmlElement["OrderNo"].Value == oldOrderNo.ToString())
                            {
                                xmlElement["OrderNo"].InnerXml = model.OrderNo.ToString();
                                //var element = sublides[i];
                                if (xmlElement["OrderNo"] != null) xmlElement["OrderNo"].InnerXml = "temp";
                                if (xmlElement["Title"] != null) xmlElement["Title"].InnerXml = model.Title;
                                //if (xmlElement["Url"].Value != null)
                                //{
                                //    if (string.IsNullOrEmpty(model.Url)&&model.Url.Length > 7 && model.Url.Substring(0, 6) == "http://")
                                //    {

                                        xmlElement["Url"].InnerXml = model.Url;
                                    //}
                                    //else
                                    //{
                                    //    return Json(new { isError = true, msg = "请按照  http://  格式输入连接地址" },
                                    //        JsonRequestBehavior.AllowGet);
                                    //}
                                //}
                                if (xmlElement["Image"] != null) xmlElement["Image"].InnerXml = model.Image;
                                if (xmlElement["Remark"] != null) xmlElement["Remark"].InnerXml = model.Remark;
                                if (xmlElement["AdSiteName"] != null) xmlElement["AdSiteName"].InnerXml = model.AdSiteName;
                            }
                        }
                        //{
                        //    element["Title"].InnerXml = model.Title;
                        //    element["Url"].InnerXml = model.Url;
                        //    element["Image"].InnerXml = model.Image;
                        //    element["Remark"].InnerXml = model.Remark;
                        //    element["AdSiteName"].InnerXml = model.AdSiteName;
                        //}
                    }

                    for (var i = 0; i < sublides.Count; i++)
                    {
                        var xmlAttributeCollection = sublides[i].Attributes;
                        if (xmlAttributeCollection != null)
                        {
                            //大改小  两个数字之间 全部加1
                            if (xmlAttributeCollection["OrderNo"].Value == "temp")
                            {
                                continue;
                            }

                            if (oldOrderNo > model.OrderNo &&
                                Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) <= oldOrderNo &&
                                Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) >= model.OrderNo)
                            {
                                xmlAttributeCollection["OrderNo"].InnerXml =
                                    (Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) + 1).ToString();
                            }



                                //小改大   两个数字之间全部减1
                            else if (oldOrderNo < model.OrderNo &&
                                     Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) >= oldOrderNo &&
                                     Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) <= model.OrderNo)
                            {
                                xmlAttributeCollection["OrderNo"].InnerXml =
                                    (Convert.ToInt32(xmlAttributeCollection["OrderNo"].Value) - 1).ToString();
                            }

                        }

                    }
                    for (var i = 0; i < sublides.Count; i++)
                    {
                        var xmlElement = sublides[i].Attributes;
                        if (xmlElement != null)
                        {
                            if (xmlElement["OrderNo"].Value != null && xmlElement["OrderNo"].Value == "temp")
                            {
                                xmlElement["OrderNo"].InnerXml = model.OrderNo.ToString();
                            }
                        }
                    }

                }
                xml.Save(serverpath);
                //<Item Id="1" Title="首页底部通栏广告" OrderNo="0"   Url="http://www.9yjob.com" Image="~/Content/images/logo.png" Remark="首页顶部"   AdSiteName="首页顶部" />
                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 新增一张幻灯片
        /// </summary>
        /// <param name="model">model</param>
        /// <returns></returns>
        public JsonResult Msg_AddSlide(Slide model)
        {
            try
            {

                var serverpath = Server.MapPath("~/AdScript/" + model.FileName + ".xml");
                var xml = new XmlDocument();
                xml.Load(serverpath);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");
                root = root.SelectSingleNode("AdItems");
                if (root == null) throw new Exception("数据文件错误");

                var sublies = root.ChildNodes;
                int temp = 0;
                for (var i = 0; i < sublies.Count; i++)
                {
                    var xmlAttrbuteCollections = sublies[i].Attributes;
                    if (xmlAttrbuteCollections != null)
                    {
                        temp = Convert.ToInt32(xmlAttrbuteCollections["OrderNo"].Value) > temp
                            ? Convert.ToInt32(xmlAttrbuteCollections["OrderNo"].Value)
                            : temp;
                    }
                    else
                    {
                        throw new Exception("数据文件错误");
                    }

                }
                var newele = xml.CreateElement("Item");
                newele.SetAttribute("Title", model.Title);
                newele.SetAttribute("Url", model.Url);
                newele.SetAttribute("Image", model.Image);
                newele.SetAttribute("Remark", model.Remark);
                newele.SetAttribute("OrderNo", (temp + 1).ToString());
                newele.SetAttribute("AdSiteName", model.AdSiteName);
                newele.SetAttribute("FileName", model.FileName);
                root.AppendChild(newele);
                xml.Save(serverpath);


                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// 删除一张幻灯片
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult Msg_DelSlide(Slide model)
        {
            try
            {

                var serverpath = Server.MapPath("~/AdScript/" + model.FileName + ".xml");
                var xml = new XmlDocument();
                xml.Load(serverpath);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");
                root = root.SelectSingleNode("AdItems");
                if (root == null) throw new Exception("数据文件错误");



                var slides = root.ChildNodes;
                if (slides.Count == 1)
                {
                    return Json(new { isError = true , msg = "Last" }, JsonRequestBehavior.AllowGet);
                }
                for (var i = 0; i < slides.Count; i++)
                {
                    var xmlElement = slides[i].Attributes;
                    if (xmlElement["OrderNo"].Value != null && xmlElement["OrderNo"].Value == model.OrderNo.ToString())
                    {
                        root.RemoveChild(slides[i]);
                    }
                }


                xml.Save(serverpath);
                //缺少删除文件
                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }






        #region 原先的代码
        
       


        // 首页幻灯片广告管理
        // GET: /admin/admanage/slide
        [AdminAuthFilter]
        public ActionResult Slide()
        {
            var slides = Models.Slide.GetRoot().SubSlides.OrderBy(o => o.OrderNo).ToList();
            return View(slides);
        }

        // 获取单个幻灯片信息
        // GET: /admin/admanage/GetSlide
        public JsonResult GetSlide(int id)
        {
            var slide = Models.Slide.GetRoot().SubSlides.SingleOrDefault(o => o.Id == id);
            return slide == null ? Json(new { isError = true }, JsonRequestBehavior.AllowGet) : Json(new { slide }, JsonRequestBehavior.AllowGet);
        }

        // 添加幻灯片
        // GET: /admin/admanage/AddSlide
        public JsonResult AddSlide(Models.Slide model)
        {
            var path = Server.MapPath("~/AdScript/index_slide.xml");
            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                var slides = Models.Slide.GetRoot().SubSlides;
                var idObj = slides.OrderBy(o => o.Id).LastOrDefault();
                var id = idObj != null ? idObj.Id : 0;
                slides.Add(new Models.Slide
                {
                    Id = id + 1,
                    Title = model.Title,
                    Url = model.Url,
                    Image = model.Image,
                    Remark = model.Remark,
                    OrderNo = model.OrderNo
                });

                root.FirstChild.RemoveAll();

                foreach (var item in slides.OrderBy(o => o.OrderNo))
                {
                    var node = xml.CreateElement("Slide");
                    node.SetAttribute("Id", item.Id.ToString());
                    node.SetAttribute("OrderNo", item.OrderNo.ToString());
                    node.SetAttribute("Title", item.Title);
                    node.SetAttribute("Url", item.Url);
                    node.SetAttribute("Image", item.Image);
                    node.SetAttribute("Remark", item.Remark);
                    root.FirstChild.AppendChild(node);
                }

                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 修改幻灯片
        // GET: /admin/admanage/ModifySlide
        public JsonResult ModifySlide(Models.Slide model)
        {
            var path = Server.MapPath("~/AdScript/index_slide.xml");
            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                foreach (var item in root.FirstChild.ChildNodes.Cast<XmlElement>().Where(item => item.GetAttribute("Id") == model.Id.ToString()))
                {
                    item.SetAttribute("OrderNo", model.OrderNo.ToString());
                    item.SetAttribute("Title", model.Title);
                    item.SetAttribute("Url", model.Url);
                    item.SetAttribute("Image", model.Image);
                    item.SetAttribute("Remark", model.Remark);
                }

                var list = root.FirstChild.ChildNodes.Cast<XmlElement>().ToList();
                root.FirstChild.RemoveAll();

                foreach (var element in list.OrderBy(o => o.GetAttribute("OrderNo")))
                {
                    root.FirstChild.AppendChild(element);
                }

                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        // 删除幻灯片
        // GET: /admin/admanage/DeleteSlide
        public JsonResult DeleteSlide(int id)
        {
            var path = Server.MapPath("~/AdScript/index_slide.xml");
            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                foreach (var item in root.FirstChild.ChildNodes.Cast<XmlElement>().Where(item => item.GetAttribute("Id") == id.ToString()))
                {
                    root.FirstChild.RemoveChild(item);
                }

                xml.Save(path);
                /////////缺少删除文件

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // 首页右侧广告管理
        // GET: /admin/admanage/IndexRight
        [AdminAuthFilter]
        public ActionResult IndexRight()
        {
            var path = Server.MapPath("~/AdScript/index_right.xml");

            var xml = new XmlDocument();
            xml.Load(path);
            var root = xml.SelectSingleNode("Root");
            if (root == null) throw new Exception("数据文件错误");

            return View(root.ChildNodes.Cast<XmlElement>().First());
        }

        // 编辑右侧广告
        // GET: /admin/admanage/EditRight
        public JsonResult EditRight(string title, string url, string image)
        {
            var path = Server.MapPath("~/AdScript/index_right.xml");

            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                root.RemoveAll();

                var element = xml.CreateElement("Item");
                element.SetAttribute("Title", title);
                element.SetAttribute("Url", url);
                element.SetAttribute("Image", image);

                root.AppendChild(element);
                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // 单图广告管理
        // GET: /admin/admanage/IndexButtom
        [AdminAuthFilter]
        public ActionResult IndexButtom()
        {
            var path = Server.MapPath("~/AdScript/index_buttom.xml");

            var xml = new XmlDocument();
            xml.Load(path);
            var root = xml.SelectSingleNode("Root");
            if (root == null) throw new Exception("数据文件错误");

            return View(root.ChildNodes.Cast<XmlElement>().First());
        }

        // 编辑右侧广告
        // GET: /admin/admanage/EditButtom
        public JsonResult EditButtom(string title, string url, string image)
        {
            var path = Server.MapPath("~/AdScript/index_buttom.xml");

            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                root.RemoveAll();

                var element = xml.CreateElement("Item");
                element.SetAttribute("Title", title);
                element.SetAttribute("Url", url);
                element.SetAttribute("Image", image);

                root.AppendChild(element);
                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // 找职位右侧广告列表管理
        // GET: /admin/admanage/JobsRight
        [AdminAuthFilter]
        public ActionResult JobsRight()
        {
            var path = Server.MapPath("~/AdScript/jobs_right.xml");

            var xml = new XmlDocument();
            xml.Load(path);
            var root = xml.SelectSingleNode("Root");
            if (root == null) throw new Exception("数据文件错误");

            var list = root.ChildNodes.Cast<XmlElement>().ToList();

            return View(list);
        }

        // 添加找位置右侧广告
        // GET: /admin/admanage/AddJobsRight
        public JsonResult AddJobsRight(string orderNo, string title, string url, string image)
        {
            var path = Server.MapPath("~/AdScript/jobs_right.xml");

            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                if (root.ChildNodes.Count >= 10) throw new Exception("找职位右侧广告数量不能大于10个");

                var list = root.ChildNodes.Cast<XmlElement>().ToList();
                var item = xml.CreateElement("Item");
                var idObj = list.OrderBy(o => int.Parse(o.GetAttribute("Id"))).LastOrDefault();
                var id = idObj != null ? int.Parse(idObj.GetAttribute("Id")) : 0;
                item.SetAttribute("Id", (id + 1).ToString());
                item.SetAttribute("Title", title);
                item.SetAttribute("Url", url);
                item.SetAttribute("OrderNo", orderNo);
                item.SetAttribute("Image", image);
                list.Add(item);

                root.RemoveAll();

                foreach (var element in list.OrderBy(o => o.GetAttribute("OrderNo")))
                {
                    root.AppendChild(element);
                }

                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 编辑找位置右侧广告
        // GET: /admin/admanage/ModifyJobsRight
        public JsonResult ModifyJobsRight(string id, string orderNo, string title, string url, string image)
        {
            var path = Server.MapPath("~/AdScript/jobs_right.xml");

            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                foreach (var item in root.ChildNodes.Cast<XmlElement>().Where(item => item.GetAttribute("Id") == id))
                {
                    item.SetAttribute("Title", title);
                    item.SetAttribute("Url", url);
                    item.SetAttribute("OrderNo", orderNo);
                    item.SetAttribute("Image", image);
                }

                var list = root.ChildNodes.Cast<XmlElement>().ToList();
                root.RemoveAll();

                foreach (var element in list.OrderBy(o => o.GetAttribute("OrderNo")))
                {
                    root.AppendChild(element);
                }

                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 删除找位置右侧广告
        // GET: /admin/admanage/DeleteJobsRight
        public JsonResult DeleteJobsRight(string id)
        {
            var path = Server.MapPath("~/AdScript/jobs_right.xml");

            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                var root = xml.SelectSingleNode("Root");
                if (root == null) throw new Exception("数据文件错误");

                foreach (var item in root.ChildNodes.Cast<XmlElement>().Where(item => item.GetAttribute("Id") == id))
                {
                    root.RemoveChild(item);
                }

                xml.Save(path);

                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        // 上传文件管理首页
        // GET: /Admin/AdManage/FileManage
        [AdminAuthFilter]
        public ActionResult FileManage(string path = "upfile\\")
        {
            if (path == "" || path.IndexOf(".") != -1) path = "upfile\\";
            Session["WenjianPath"] = path;
            return View();
        }

        // 创建目录
        // GET: /admin/admanage/CreateDic
        public ActionResult CreateDic(string dicName)
        {
            try
            {
                if (string.IsNullOrEmpty(dicName)) throw new Exception("请填写新建文件夹名称！");
                var strUpFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + Session["WenjianPath"] + dicName;
                Directory.CreateDirectory(strUpFile);
                return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 删除目录
        // GET: /admin/admanage/DeleteDic
        public ActionResult DeleteDic(string dicName)
        {
            try
            {
                if (string.IsNullOrEmpty(dicName)) throw new Exception("不能删除文件夹根目录");

                if (UserAuth.IsAuthenticated && UserAuth.Role.RoleName == "admin")
                {
                    var strUpFile = AppDomain.CurrentDomain.BaseDirectory + dicName;
                    Directory.Delete(strUpFile, true);
                    return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { isError = true, msg = "无权操作，您登录可能已经失效" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 上传文件
        // GET: /admin/admanage/upfile
        [AdminAuthFilter]
        public ActionResult UpFile()
        {
            return View();
        }

        // 保存上传的文件
        // GET: /admin/admanage/SaveFileUpload
        public ActionResult SaveFileUpload(string currentPath)
        {
            try
            {
                //if (UserSessionManager.IsLogin() && UserSessionManager.User.IsManage)
                //{
                var file = Request.Files[0];
                if (file != null)
                {
                    var fileType = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + fileType;
                    var filepath = "http://www.9yjob.com/" + currentPath.Replace(@"\", @"/") + fileName;
                    if (file.ContentLength > 0)
                    {
                        var strUpFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + currentPath + fileName;
                        file.SaveAs(strUpFile);
                    }
                    return Json(new { isError = false, filepath }, JsonRequestBehavior.AllowGet);
                }
                //}

                return Json(new { isError = true, msg = "上传图失败" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // 删除文件
        // GET: /admin/admanage/DeleteFile
        public ActionResult DeleteFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) throw new Exception("删除失败，文件名不能为空");

                if (UserAuth.IsAuthenticated && UserAuth.Role.RoleName == "admin")
                {
                    var strUpFile = AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName;
                    System.IO.File.Delete(strUpFile);
                    return Json(new { isError = false }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { isError = true, msg = "无权操作，您登录可能已经失效" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isError = true, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }






        #endregion





    }
}
