/******************************************************************************
/// <summary>
/// 版权所有: 版权所有(C) 2012，讯酷科技
/// 内容摘要: 
/// 完成日期: 2012/5/8 10:45:30
/// 版    本: 1.0
/// 作    者: 陈龙宽
///
/// 修改记录1
/// 修改日期:
/// 版 本 号:
/// 修 改 人:
/// 修改内容:
/// </summary> 

 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace UEditorWeb.NEditor.server.upload.net
{
    public partial class up : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<String> arr = new List<string>{ "gif", "jpeg", "png", "jpg" };
            String retmsg = "{{'url':'UpLoad/{0}','title':'{1}','state':'{2}'}}";
            //保存文件路径
            String filePath = Server.MapPath("~") + "UpLoad";
            //判断路径是否存在，不存在则创建
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            //文件名
            string fileName = string.Empty;
            //绝对路径文件名
            string fullFileName = string.Empty;
            //文件后缀名
            string extension = string.Empty;
            //文件描述
            string fileDescription = string.Empty;
            fileDescription = Request["pictitle"];

            //开始保存文件到服务器
            HttpPostedFile file = Request.Files["picdata"] as HttpPostedFile;
            int index = file.FileName.LastIndexOf('.');

            extension = file.FileName.Substring(index + 1);
            if (!arr.Contains(extension))
            {
                retmsg = string.Format(retmsg, string.Empty, string.Empty, string.Empty);
            }
            else
            {
                fileName = string.Format("{0}.{1}", Guid.NewGuid(), extension);
                fullFileName = string.Format("{0}\\{1}", filePath, fileName);
                file.SaveAs(fullFileName);
                retmsg = string.Format(retmsg, fileName, fileDescription, "SUCCESS");
            }

            //上传完毕
            Response.Clear();
            Response.Write(retmsg);
            Response.End();
        }
    }
}