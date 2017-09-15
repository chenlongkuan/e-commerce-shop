
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Com.Alipay;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Extensions;
using Msg.Tools.Logging;
using Msg.Web.App_Start;

namespace Msg.Web.Controllers
{
    public class AlipayController : Controller
    {

        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();
        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();

        /// <summary>
        /// 服务器异步通知页面
        /// </summary>
        /// <returns></returns>
        public ActionResult notify_url()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                var aliNotify = new Notify();
                var verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    try
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //请在这里加上商户的业务逻辑程序代码
                        //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                        //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表
                        string trade_no = Request.Form["trade_no"];         //支付宝交易号
                        string order_no = Request.Form["out_trade_no"];     //获取订单号
                        string total_fee = Request.Form["total_fee"];       //获取总金额
                        string subject = Request.Form["subject"];           //商品名称、订单名称
                        string body = Request.Form["body"];                 //商品描述、订单备注、描述
                        string buyer_email = Request.Form["buyer_email"];   //买家支付宝账号
                        string trade_status = Request.Form["trade_status"]; //交易状态
                        string refund_status = Request.Form["refund_status"];//退款状态

                        var order = _ordersHelper.GetOrdersEntityByOrderNo(order_no);
                        if (order != null)
                        {
                            if (!string.IsNullOrEmpty(trade_status))
                            {
                                #region 交易处理

                                if (trade_status == "TRADE_SUCCESS")
                                {
                                    #region 处理订单状态  已付款 -> 正在出库

                                    //该判断示买家已在支付宝交易管理中产生了交易记录且付款成功，但卖家没有发货

                                    //判断该笔订单是否在商户网站中已经做过处理
                                    //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                                    //如果有做过处理，不执行商户的业务程序

                                    //修改订单状态
                                    var result = _ordersHelper.ModifyOrderStatus(order.Id.ToString(),
                                        OrderStatusEnum.Outputing,
                                        order.User.Id);

                                    if (result.ResultType.Equals(OperationResultType.Success))
                                    {
                                        //修改商品售卖数量
                                        _goodsHelper.ModifyGoodsSoldCount(order.Items);


                                        Response.Write("success");  //请不要修改或删除
                                    }

                                    #endregion


                                }

                                //添加交易日志
                                AddTradeLog(order.User.Id, order.User.NickName, total_fee, order.Id.ToString(),
                                    trade_no, buyer_email, trade_status);

                                #endregion

                                if (!string.IsNullOrEmpty(refund_status))
                                {
                                    #region 退款处理  暂不支持

                                    //switch (refund_status)
                                    //{
                                    //    case "WAIT_SELLER_AGREE"://等待卖家同意退款
                                    //        #region 处理订单状态  申请退款

                                    //        #endregion
                                    //        break;
                                    //    case "WAIT_BUYER_RETURN_GOODS"://等待卖家退货
                                    //        #region WAIT_BUYER_RETURN_GOODS

                                    //        #endregion
                                    //        break;
                                    //    case "WAIT_SELLER_CONFIRM_GOODS"://买家已退货，等待卖家收到退货
                                    //        #region WAIT_SELLER_CONFIRM_GOODS



                                    //        #endregion
                                    //        break;
                                    //    case "REFUND_SUCCESS"://卖家同意退款，退款成功，交易结束
                                    //        #region 处理订单状态  退款成功

                                    //        #endregion
                                    //        break;
                                    //    case "SELLER_REFUSE_BUYER"://卖家拒绝退款
                                    //        #region 处理订单状态   驳回退款申请


                                    //        #endregion
                                    //        break;
                                    //}
                                    //Response.Write("success");  //请不要修改或删除

                                    #endregion
                                }
                            }
                        }

                        //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteException("支付宝异步支付回调异常 ---> ", ex);
                    }

                }
                else//验证失败
                {
                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("无通知参数");
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 页面跳转同步通知页面
        /// </summary>
        /// <returns></returns>
        public ActionResult return_url()
        {
            var sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                var aliNotify = new Notify();
                var verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    try
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //请在这里加上商户的业务逻辑程序代码

                        //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                        //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表
                        var trade_no = Request.QueryString["trade_no"]; //支付宝交易号
                        var order_no = Request.QueryString["out_trade_no"]; //获取订单号
                        var type = Request.QueryString["type"]; //获取类别，0：web支付，1：app支付
                        var total_fee = Request.QueryString["total_fee"]; //获取总金额
                        var subject = Request.QueryString["subject"]; //商品名称、订单名称
                        var body = Request.QueryString["body"]; //商品描述、订单备注、描述
                        var buyer_email = Request.QueryString["buyer_email"]; //买家支付宝账号
                        var trade_status = Request.QueryString["trade_status"]; //交易状态

                        var order = _ordersHelper.GetOrdersEntityByOrderNo(order_no);
                        if (order != null)
                        {

                            if (trade_status == "TRADE_SUCCESS")
                            {
                                #region 处理订单状态  已付款 -> 正在出库
                                //该判断示买家已在支付宝交易管理中产生了交易记录且付款成功，但卖家没有发货

                                //判断该笔订单是否在商户网站中已经做过处理
                                //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                                //如果有做过处理，不执行商户的业务程序

                                //修改订单状态
                                var result = _ordersHelper.ModifyOrderStatus(order.Id.ToString(), OrderStatusEnum.Outputing,
                                 order.User.Id);

                                if (result.ResultType.Equals(OperationResultType.Success))
                                {
                                    //修改商品售卖数量
                                    _goodsHelper.ModifyGoodsSoldCount(order.Items);

                                    //添加交易日志
                                    AddTradeLog(order.User.Id, order.User.NickName, total_fee, order.Id.ToString(), trade_no, buyer_email, trade_status);

                                    return RedirectToAction("PayOk", new { orderNo = order.OrderNo });

                                }

                                #endregion


                            }
                            else
                            {
                                //添加交易日志
                                AddTradeLog(order.User.Id, order.User.NickName, total_fee, order.Id.ToString(), trade_no, buyer_email, trade_status);
                                Response.Write("trade_status=" + Request.QueryString["trade_status"]);
                            }




                        }

                        //打印页面
                        Response.Write("验证成功<br />");

                        //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteException("支付宝同步支付回调异常 ---> ", ex);
                    }
                }
                else//验证失败
                {
                    Response.Write("验证失败");
                }
            }
            else
            {
                Response.Write("无返回参数");
            }
            return new EmptyResult();
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            var sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 处理结算
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        [AuthFilters]
        public ActionResult Payment(string orderNo)
        {
            string msg = "error";

            if (string.IsNullOrEmpty(orderNo))
            {
                ViewBag.ErrMsg = "订单号为空";
                ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
                ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
                ViewBag.NoNeedTopNav = true;
                return View();
            }

            //当前订单详情
            var result = _ordersHelper.GetMyOrderEntity(orderNo, UserAuth.UserId);
            if (!result.ResultType.Equals(OperationResultType.Success))
            {
                ViewBag.ErrMsg = result.ResultType.ToDescription() + result.Message;
                ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
                ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
                ViewBag.NoNeedTopNav = true;
                return View();
            }
            var order = (OrdersEntity)result.AppendData;

            var Buyable = true;
            var UnBuyableDesc = "";

            //验证商品可购买性
            foreach (var goods in order.Items.Select(item => item.Goods))
            {
                if (!goods.IsOnSelling)
                {
                    Buyable = false;
                    UnBuyableDesc = "<a href='" + Url.Action("Details", "Home", new { id = goods.Id }) + "'>" + goods.ShortTitle + "</a>  【已下架】";
                    break;
                }
                if (!(goods.IsUseable || goods.Product.IsUseable))
                {
                    Buyable = false;
                    UnBuyableDesc = "<a href='" + Url.Action("Details", "Home", new { id = goods.Id }) + "'>" + goods.ShortTitle + "</a>  【商品已删除】";
                    break;
                }
                if (goods.SoldCount >= goods.Product.Quantity)
                {
                    Buyable = false;
                    UnBuyableDesc = "<a href='" + Url.Action("Details", "Home", new { id = goods.Id }) + "'>" + goods.ShortTitle + "</a>  【库存不足】";
                    break;
                }
            }
            if (!Buyable)
            {
                ViewBag.ErrMsg = UnBuyableDesc;
                ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
                ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
                ViewBag.NoNeedTopNav = true;
                return View();
            }



            //订单总金额，显示在支付宝收银台里的“应付总额”里
            //string total_fee = TxtTotal_fee.Text.Trim();
            string total_fee = order.DisplayPrice.ToString("F");


            ////////////////////////////////////////////请求参数////////////////////////////////////////////


            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = "http://" + Request.Url.Host.ToString() + Url.Action("notify_url");
            //需http://格式的完整路径，不能加?id=123这类自定义参数

            //页面跳转同步通知页面路径
            string return_url = "http://" + Request.Url.Host.ToString() + Url.Action("return_url");
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/

            //卖家支付宝帐户
            string seller_email = Com.Alipay.Config.Seller_email;
            //必填

            //商户订单号
            string out_trade_no = orderNo;
            //商户网站订单系统中唯一订单号，必填

            //订单名称
            string subject = order.Items.FirstOrDefault().Goods.ShortTitle;
            //必填

            //付款金额
            string price = total_fee;
            //必填

            //商品数量
            string quantity = "1";
            //必填，建议默认为1，不改变值，把一次交易看成是一次下订单而非购买一件商品
            //物流费用
            string logistics_fee = order.ExpressCost.ToString();
            //必填，即运费
            //物流类型
            string logistics_type = Com.Alipay.Config.Logistics_type;
            //必填，三个值可选：EXPRESS（快递）、POST（平邮）、EMS（EMS）
            //物流支付方式
            string logistics_payment = order.ExpressCost > 0 ? "BUYER_PAY" : "SELLER_PAY";
            //必填，两个值可选：SELLER_PAY（卖家承担运费）、BUYER_PAY（买家承担运费）
            //订单描述

            string body = order.Items.FirstOrDefault().Goods.ShortTitle + "";
            //商品展示地址
            string show_url = "http://" + Request.Url.Host.ToString() + Url.Action("Details", "Home", new { id = order.Items.FirstOrDefault().Goods.Id });
            //需以http://开头的完整路径，如：http://www.xxx.com/myorder.html

            ////收货人姓名
            //string receive_name = order.Address.ReciverName;
            ////如：张三

            ////收货人地址
            //string receive_address = order.Address.CityName + order.Address.RegionName + order.Address.SchoolName + order.Address.DetailAddress;
            ////如：XX省XXX市XXX区XXX路XXX小区XXX栋XXX单元XXX号

            ////收货人邮编
            //string receive_zip = order.Address.PostCode;
            ////如：123456

            ////收货人电话号码
            //string receive_phone = "";
            ////如：0571-88158090

            ////收货人手机号码
            //string receive_mobile = order.Address.ReciverTel;
            ////如：13312341234



            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            var sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Com.Alipay.Config.Partner);
            sParaTemp.Add("_input_charset", Com.Alipay.Config.Input_charset.ToLower());
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("seller_email", seller_email);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("anti_phishing_key", Submit.Query_timestamp());
            sParaTemp.Add("exter_invoke_ip", Request.UserHostAddress);
            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            Response.Write(sHtmlText);
            ViewBag.ErrMsg = sHtmlText;
            return new EmptyResult();
        }

        [AuthFilters]
        public ActionResult PayOk(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
            {
                ViewBag.ErrMsg = "参数错误";
            }
            else
            {
                ViewBag.OrderNo = orderNo;
            }
            if (Utils.Utils.IsMobileDevice())
            {
                return Redirect("/mobile/home/PayOk?orderNo=" + orderNo);
            }
            return View();
        }

        /// <summary>
        /// 新增交易日志
        /// </summary>
        /// <param name="total_fee">总金额</param>
        /// <param name="orderId">订单ID</param>
        private void AddTradeLog(int uid, string username, string total_fee, string orderId, string trade_no, string buyerEmail, string trade_status)
        {
            var tradeLog = new UserTradeLogsEntity();
            tradeLog.UserId = uid;
            tradeLog.UserNickName = username;
            tradeLog.Sum = decimal.Parse(total_fee);
            tradeLog.OrderId = orderId;
            tradeLog.TradeNo = trade_no;
            tradeLog.TradeAccount = buyerEmail;
            tradeLog.TradeStatus = trade_status;
            tradeLog.OperateTime = DateTime.Now;
            tradeLog.OperateType = 1;
            tradeLog.Desc = "支付消费";
            _ordersHelper.AddTradeLog(tradeLog);
        }

    }
}
