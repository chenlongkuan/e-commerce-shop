﻿<%@ WebHandler Language="C#" Class="getRemoteImage" %>
/**
 * Created by visual studio 2010
 * User: xuheng
 * Date: 12-3-8
 * Time: 下午13:33
 * To get the Remote image.
 */

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

public class getRemoteImage : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string savePath = context.Server.MapPath("/UpLoad/webSearch/");       //保存文件地址
        string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };             //文件允许格式
        int fileSize = 3000;                                                        //文件大小限制，单位kb

        string uri = context.Server.HtmlEncode(context.Request["content"]);
        uri = uri.Replace("&amp;", "&");
        string[] imgUrls = Regex.Split(uri, "ue_separate_ue", RegexOptions.IgnoreCase);
        string fileImageUrl = null;

        ArrayList tmpNames = new ArrayList();
        WebClient wc = new WebClient();
        HttpWebResponse res;
        String tmpName = String.Empty;
        String imgUrl = String.Empty;
        String currentType = String.Empty;

        try
        {
            for (int i = 0, len = imgUrls.Length; i < len; i++)
            {
                imgUrl = imgUrls[i];

                if (imgUrl.Substring(0, 7) != "http://")
                {
                    tmpNames.Add("error!");
                    continue;
                }

                //格式验证
                int temp = imgUrl.LastIndexOf('.');
                currentType = imgUrl.Substring(temp);
                if (Array.IndexOf(filetype, currentType) == -1)
                {
                    tmpNames.Add("error!");
                    continue;
                }

                res = (HttpWebResponse)WebRequest.Create(imgUrl).GetResponse();
                //http检测
                if (res.ResponseUri.Scheme.ToLower().Trim() != "http")
                {
                    tmpNames.Add("error!");
                    continue;
                }
                //大小验证
                if (res.ContentLength > fileSize * 1024)
                {
                    tmpNames.Add("error!");
                    continue;
                }
                //死链验证
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    tmpNames.Add("error!");
                    continue;
                }
                //检查mime类型
                if (res.ContentType.IndexOf("image") == -1)
                {
                    tmpNames.Add("error!");
                    continue;
                }
                res.Close();

                //创建保存位置
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                //写入文件
                tmpName = DateTime.Now.ToString("yyyyMMddhhmmssfff") + currentType;
                wc.DownloadFile(imgUrl, savePath + tmpName);
                //tmpNames.Add("../../upload/webSearch/" + tmpName);

                byte[] buff = File.ReadAllBytes(savePath + tmpName);

                xk.FileUpload.Attachments att = new xk.FileUpload.Attachments();
                att.Root = 0;
                att.FileData = buff;
                att.Extension = currentType;

                string result = string.Empty;

                if (xk.FileUpload.UploadHelper.MakeSmailImageByMaxWidth(att, 760, ref result))
                {
                    fileImageUrl = result;
                }

                //删除本地临时图片
                File.Delete(savePath + tmpName);
            }
        }
        catch (Exception)
        {
            tmpNames.Add("error!");
        }
        finally
        {
            wc.Dispose();
        }
        context.Response.Write("{url:'" + fileImageUrl + "',tip:'远程图片抓取成功！',srcUrl:'" + uri + "'}");
    }

    //集合转换字符串
    private string converToString(ArrayList tmpNames)
    {
        String str = String.Empty;
        for (int i = 0, len = tmpNames.Count; i < len; i++)
        {
            str += tmpNames[i] + "ue_separate_ue";
            if (i == tmpNames.Count - 1)
                str += tmpNames[i];
        }
        return str;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}