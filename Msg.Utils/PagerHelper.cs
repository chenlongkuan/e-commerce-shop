using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Msg.Utils
{
    /// <summary>
    /// 分页帮助类
    /// </summary>
    public class PagerHelper
    {
        #region 属性
        /// <summary>
        /// 页码
        /// </summary>
        private int index;
        /// <summary>
        /// 每页显示数量
        /// </summary>
        private int size;
        /// <summary>
        /// 记录总数
        /// </summary>
        private int total;
        /// <summary>
        /// javascript class
        /// </summary>
        private string jsClass;
        /// <summary>
        /// 链接地址
        /// </summary>
        private string url;
        /// <summary>
        /// 数字格式化
        /// </summary>
        private string numLableFormat;
        /// <summary>
        /// 每页显示数字按钮数
        /// </summary>
        private int numButton = 5;
        /// <summary>
        /// 数字是否显示
        /// </summary>
        private bool numLableVisible = false;
        /// <summary>
        /// 数字按钮是否显示
        /// </summary>
        private bool numButtonVisible = true;
        /// <summary>
        /// 首页是否显示
        /// </summary>
        private bool firstPageVisible = true;
        /// <summary>
        /// 上一页是否显示
        /// </summary>
        private bool prevPageVisible = true;
        /// <summary>
        /// 下一页是否显示
        /// </summary>
        private bool nextPageVisible = true;
        /// <summary>
        /// 末页是否显示
        /// </summary>
        private bool lastPageVisible = true;

        /// <summary>
        /// ajax分页
        /// </summary>
        private bool isAjax = true;



        #endregion

        /// <summary>
        /// 获取SQL分页SQL代码
        /// </summary>
        /// <param name="sql">原始查询SQL</param>
        /// <param name="orderColumn">排序字段</param>
        /// <param name="orderType">排序方式（ASC、DESC）</param>
        /// <param name="index">页码</param>
        /// <param name="size">每页数据量</param>
        /// <returns></returns>
        public static string GetPagingSql(string sql, string orderColumn, string orderType, int index, int size)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT  TOP {0} *
                                            FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY bb.{1} {2}) AS RowNumber ,
                                                                *
                                                      FROM      (", size, orderColumn, orderType);
            sbSql.Append(sql);
            sbSql.AppendFormat(@" ) bb
                                ) ss
                        WHERE   ss.RowNumber > {0} * ( {1} - 1 ) ORDER BY RowNumber", size, index);
            return sbSql.ToString();
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="total">总数据量</param>
        /// <param name="size">每页数量</param>
        /// <returns></returns>
        public static int GetTotalPage(int total, int size)
        {
            var pages = 0;
            pages = total / size;
            if (total % size > 0)
            {
                pages += 1;
            }
            return pages;
        }

        /// <summary>
        /// 创建分页代码(ajax)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="jsClass">jsClass</param>
        public static string CreatePagerByAjax(int index, int size, int total, string jsClass)
        {
            var p = new PagerHelper(index, size, total, jsClass, null, null, null, null, null, null, null, null, null, true);
            return p.GetHtml();
        }

        /// <summary>
        /// 创建分页代码(ajax)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="jsClass">jsClass</param>
        /// <param name="numLableFormat">统计信息格式化，如： 共 {0} 条  第 {1} / {2} 页</param>
        /// <param name="numLableVisible">统计是否显示</param>
        public static string CreatePagerByAjax(int index, int size, int total, string jsClass, string numLableFormat, bool numLableVisible)
        {
            var p = new PagerHelper(index, size, total, jsClass, null, numLableFormat, null, numLableVisible, null, null, null, null, null, true);
            return p.GetHtml();
        }

        /// <summary>
        /// 创建分页代码(ajax)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="jsClass">jsClass</param>
        /// <param name="numLableFormat">统计信息格式化，如： 共 {0} 条  第 {1} / {2} 页</param>
        /// <param name="numButton">数字按钮数量</param>
        /// <param name="numLableVisible">统计是否显示</param>
        /// <param name="numButtonVisible">数字按钮是否显示</param>
        /// <param name="firstPageVisible">首页是否显示</param>
        /// <param name="prevPageVisible">上一页是否显示</param>
        /// <param name="nextPageVisible">下一页是否显示</param>
        /// <param name="lastPageVisible">末页是否显示</param>
        public static string CreatePagerByAjax(int index, int size, int total, string jsClass, string numLableFormat, int numButton, bool numLableVisible, bool numButtonVisible, bool firstPageVisible, bool prevPageVisible, bool nextPageVisible, bool lastPageVisible)
        {
            var p = new PagerHelper(index, size, total, jsClass, null, numLableFormat, numButton, numLableVisible, numButtonVisible, firstPageVisible, prevPageVisible, nextPageVisible, lastPageVisible, true);
            return p.GetHtml();
        }

        /// <summary>
        /// 创建分页代码(url)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="url">url</param>
        public static string CreatePagerByUrl(int index, int size, int total, string url)
        {
            var p = new PagerHelper(index, size, total, null, url, null, null, null, null, null, null, null, null, false);
            return p.GetHtml();
        }

        /// <summary>
        /// 创建分页代码(url)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="url">url</param>
        /// <param name="numLableFormat">统计信息格式化，如： 共 {0} 条  第 {1} / {2} 页</param>
        /// <param name="numLableVisible">统计是否显示</param>
        public static string CreatePagerByUrl(int index, int size, int total, string url, string numLableFormat, bool numLableVisible)
        {
            var p = new PagerHelper(index, size, total, null, url, numLableFormat, null, numLableVisible, null, null, null, null, null, false);
            return p.GetHtml();
        }

        /// <summary>
        /// 创建分页代码(url)
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="url">url</param>
        /// <param name="numLableFormat">统计信息格式化，如： 共 {0} 条  第 {1} / {2} 页</param>
        /// <param name="numButton">数字按钮数量</param>
        /// <param name="numLableVisible">统计是否显示</param>
        /// <param name="numButtonVisible">数字按钮是否显示</param>
        /// <param name="firstPageVisible">首页是否显示</param>
        /// <param name="prevPageVisible">上一页是否显示</param>
        /// <param name="nextPageVisible">下一页是否显示</param>
        /// <param name="lastPageVisible">末页是否显示</param>
        public static string CreatePagerByUrl(int index, int size, int total, string url, string numLableFormat, int numButton, bool numLableVisible, bool numButtonVisible, bool firstPageVisible, bool prevPageVisible, bool nextPageVisible, bool lastPageVisible)
        {
            var p = new PagerHelper(index, size, total, null, url, numLableFormat, numButton, numLableVisible, numButtonVisible, firstPageVisible, prevPageVisible, nextPageVisible, lastPageVisible, false);
            return p.GetHtml();
        }

        /// <summary>
        /// 分页类
        /// </summary>
        /// <param name="index">页码</param>
        /// <param name="size">每页显示</param>
        /// <param name="total">总数量</param>
        /// <param name="jsClass">js</param>
        /// <param name="url">url</param>
        /// <param name="numLableFormat">统计信息格式化，如： 共 {0} 条  第 {1} / {2} 页</param>
        /// <param name="numButton">数字按钮数量</param>
        /// <param name="numLableVisible">统计是否显示</param>
        /// <param name="numButtonVisible">数字按钮是否显示</param>
        /// <param name="firstPageVisible">首页是否显示</param>
        /// <param name="prevPageVisible">上一页是否显示</param>
        /// <param name="nextPageVisible">下一页是否显示</param>
        /// <param name="lastPageVisible">末页是否显示</param>
        private PagerHelper(int index, int size, int total, string jsClass, string url, string numLableFormat, int? numButton, bool? numLableVisible, bool? numButtonVisible, bool? firstPageVisible, bool? prevPageVisible, bool? nextPageVisible, bool? lastPageVisible, bool isAjax)
        {
            this.index = index;
            this.size = size;
            this.total = total;
            this.jsClass = jsClass;
            this.url = url;
            this.numLableFormat = numLableFormat;
            if (numButton != null)
            {
                this.numButton = numButton.Value;
            }
            if (numLableVisible != null)
            {
                this.numLableVisible = numLableVisible.Value;
            }
            if (numButtonVisible != null)
            {
                this.numButtonVisible = numButtonVisible.Value;
            }
            if (firstPageVisible != null)
            {
                this.firstPageVisible = firstPageVisible.Value;
            }
            if (prevPageVisible != null)
            {
                this.prevPageVisible = prevPageVisible.Value;
            }
            if (nextPageVisible != null)
            {
                this.nextPageVisible = nextPageVisible.Value;
            }
            if (lastPageVisible != null)
            {
                this.lastPageVisible = lastPageVisible.Value;
            }
            this.isAjax = isAjax;

            if (!isAjax)
            {
                InitUrl();
            }
        }

        /// <summary>
        /// 获取分页html代码
        /// </summary>
        /// <returns></returns>
        private string GetHtml()
        {
            var pages = 1;//总页数
            if (total != 0)
            {
                pages = total / size;
                if (total % size > 0)
                {
                    pages += 1;
                }
            }

            //如果总页数=1，不返回分页代码
            if (pages == 1)
            {
                return null;
            }

            StringBuilder strHtml = new StringBuilder();

            strHtml.Append("<div style='float:right'>");
            strHtml.Append("<input name=\"hid-com-page-pages\" type=\"hidden\" value=\"" + pages + "\" />");
            strHtml.Append("<input name=\"hid-com-page-index\" type=\"hidden\" value=\"" + index + "\" />");

            #region 统计

            if (numLableVisible)
            {
                string strFormat = "共 {0} 条  第 {1} / {2} 页";
                if (!string.IsNullOrWhiteSpace(numLableFormat))
                {
                    strFormat = numLableFormat;
                }
                strHtml.Append("<div class=\"uc_page_count\">");
                strHtml.AppendFormat(strFormat, "<b>" + total + "</b>", "<b>" + index + "</b>", "<b>" + pages + "</b>");
                strHtml.Append("</div>");
            }
            #endregion

            strHtml.Append("<ul class=\"pagination\">");

            #region 首页

            if (firstPageVisible)
            {
                if (index > 1)
                {
                    if (isAjax)
                    {
                        strHtml.AppendFormat("<li><a href=\"javascript:;\" onclick=\"{0}(1); return false;\"><span>首页</span></a></li>", jsClass);
                    }
                    else
                    {
                        strHtml.AppendFormat("<li><a href=\"{0}page=1\"><span>首页</span></a></li>", url);
                    }
                }
                else
                {
                    strHtml.Append("<li class=\"disable\"><span>首页</span></li>");
                }
            }
            #endregion

            #region 上一页
            if (prevPageVisible)
            {
                if (index > 1)
                {
                    if (isAjax)
                    {
                        strHtml.AppendFormat("<li><a href=\"javascript:;\" onclick=\"{0}({1}); return false;\"><span>上一页</span></a></li>", jsClass, index - 1);
                    }
                    else
                    {
                        strHtml.AppendFormat("<li><a href=\"{0}page={1}\"><span>上一页</span></a></li>", url, index - 1);
                    }
                }
                //else
                //{
                //    strHtml.Append("<i><span>上一页</span></i>");
                //}
            }
            #endregion

            #region 数字按钮

            if (numButtonVisible)
            {
                int startPage = 1;//起始页码
                int endPage = 1;//结束页码
                if (pages > numButton)
                {
                    if (index - (numButton / 2) > 0)
                    {
                        if (index + (numButton / 2) < pages)
                        {
                            startPage = index - (numButton / 2);
                            endPage = startPage + numButton - 1;
                        }
                        else
                        {
                            endPage = pages;
                            startPage = endPage - numButton + 1;
                        }
                    }
                    else
                    {
                        endPage = numButton;
                    }
                }
                else
                {
                    startPage = 1;
                    endPage = pages;
                }

                for (int i = startPage; i <= endPage; i++)
                {
                    if (i != index)
                    {
                        if (isAjax)
                        {
                            strHtml.AppendFormat("<li><a href=\"javascript:;\" onclick=\"{0}({1}); return false;\"><span>{1}</span></a></li>", jsClass, i);
                        }
                        else
                        {
                            strHtml.AppendFormat("<li><a href=\"{0}page={1}\"><span>{1}</span></a><li>", url, i);
                        }
                    }
                    else
                    {
                        strHtml.AppendFormat("<li class=\"active\"><a href='javascript:;'>{0}</a></li>", i);
                    }
                }
            }

            #endregion

            #region 下一页
            if (nextPageVisible)
            {
                if (index < pages)
                {
                    if (isAjax)
                    {
                        strHtml.AppendFormat("<li><a href=\"javascript:;\" onclick=\"{0}({1}); return false;\"><span>下一页</span></a></li>", jsClass, index + 1);
                    }
                    else
                    {
                        strHtml.AppendFormat("<li><a href=\"{0}page={1}\"><span>下一页</span></a></li>", url, index + 1);
                    }
                }
                //else
                //{
                //    strHtml.Append("<i><span>下一页</span></i>");
                //}
            }
            #endregion

            #region 末页

            if (lastPageVisible)
            {
                if (index < pages)
                {
                    if (isAjax)
                    {
                        strHtml.AppendFormat("<li><a href=\"javascript:;\" onclick=\"{0}({1}); return false;\"><span>末页</span></a></li>", jsClass, pages);
                    }
                    else
                    {
                        strHtml.AppendFormat("<li><a href=\"{0}page={1}\"><span>末页</span></a></li>", url, pages);
                    }
                }
                else
                {
                    strHtml.Append("<li class=\"disable\"><span>末页</span></li>");
                }
            }
            #endregion

            strHtml.Append("</ul>");
            strHtml.Append("</div>");


            return strHtml.ToString();
        }

        #region 初始化url地址

        /// <summary>
        /// 初始化Url地址
        /// </summary>
        private void InitUrl()
        {
            url = HttpContext.Current.Request.Path;
            NameValueCollection coll = HttpContext.Current.Request.QueryString;
            StringBuilder sb = new StringBuilder();
            if (coll.Count > 0)
            {
                String[] requestarr = coll.AllKeys;
                foreach (string p in requestarr)
                {
                    if (p.EndsWith("page", true, null))
                        continue;
                    if (p.EndsWith("t", true, null))
                        continue;

                    sb.Append(p + "=" + HttpUtility.UrlEncode(HttpContext.Current.Request.QueryString[p]) + "&");
                }

            }
            url = url + "?" + sb.ToString();
        }
        #endregion

    }
}