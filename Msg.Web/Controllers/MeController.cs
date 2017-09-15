using System;
using System.Linq;
using System.Web.Mvc;
using Glimpse.Core.Extensions;
using Microsoft.Practices.Unity;
using Msg.Bll;
using Msg.Bll.Helpers;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Logging;
using Msg.Utils;
using Msg.Web.App_Start;

namespace Msg.Web.Controllers
{
    [AuthFilters]
    public class MeController : Controller
    {
        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();
        private readonly CartHelper _cartHelper = UnityConfig.GetConfiguredContainer().Resolve<CartHelper>();
        private readonly ProductsHelper _productsHelper = UnityConfig.GetConfiguredContainer().Resolve<ProductsHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();
        private readonly SuppliersHelper _suppliersHelper = UnityConfig.GetConfiguredContainer().Resolve<SuppliersHelper>();
        private readonly ExchangesHelper _exchangesHelper = UnityConfig.GetConfiguredContainer().Resolve<ExchangesHelper>();
        private readonly CommentsHelper _commentsHelper = UnityConfig.GetConfiguredContainer().Resolve<CommentsHelper>();
        private readonly NotifiesHelper _notifiesHelper = UnityConfig.GetConfiguredContainer().Resolve<NotifiesHelper>();
        private readonly CouponsHelper _couponsHelper = UnityConfig.GetConfiguredContainer().Resolve<CouponsHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();
        //
        // GET: /Me/

        #region 首页

        public ActionResult Index()
        {
            var totalUnPay = 0;
            var totalUnReceived = 0;
            ViewBag.UnPayOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.UnPay, 1, 10,
                out totalUnPay);
            ViewBag.UnReceivedOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.Sended, 1,
                10, out totalUnReceived);
            ViewBag.UnPayOrdersPager = PagerHelper.CreatePagerByAjax(1, 10, totalUnPay, "index.loadUnPayOrders");
            ViewBag.UnReceivedOrdersPager = PagerHelper.CreatePagerByAjax(1, 10, totalUnReceived,
                "index.loadUnReceivedOrders");
            ViewBag.Carts = _cartHelper.GetCartModels();
            ViewBag.NoNeedTopAdvert = true;
            ViewBag.MenuIndexClassOver = "over";
            return View();
        }

        public PartialViewResult _UnPayOrders(int page = 1)
        {
            var totalUnPay = 0;
            var unPayOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.UnPay, 1, 10,
                out totalUnPay);
            ViewBag.UnPayOrdersPager = PagerHelper.CreatePagerByAjax(1, 10, totalUnPay, "index.loadUnPayOrders");
            return PartialView(unPayOrders);
        }


        public PartialViewResult _UnReceivedOrders(int page = 1)
        {
            var totalUnReceived = 0;
            var unReceivedOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.Sended, 1,
                10, out totalUnReceived);
            ViewBag.UnReceivedOrdersPager = PagerHelper.CreatePagerByAjax(1, 10, totalUnReceived,
                "index.loadUnReceivedOrders");
            return PartialView(unReceivedOrders);
        }


        public ActionResult ModifyAvatar()
        {
            ViewBag.NoNeedTopAdvert = true;
            return View();
        }


        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AvatarUpload()
        {
            byte[] jpgContent = Request.BinaryRead(Request.ContentLength);
            bool s = true;
            try
            {
                if (jpgContent.Length > 0)
                {
                    var avatar = FileUpload.UploadHelper.SetAvatar(UserAuth.UserId, jpgContent);
                    s = _usersHelper.ModifyAvatar(UserAuth.UserId, avatar);
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteException("上传头像失败，出现异常", ex);
                s = false;
            }
            return Json(new { s = s, msg = s ? "上传成功" : "上传失败" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 我的订单

        //我的订单
        public ActionResult MyOrders()
        {
            var total = 0;
            ViewBag.MenuMyOrdersClassOver = "over";
            //默认未支付的订单
            var unReceivedOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.UnPay, 1,
                10, out total);
            ViewBag.NoNeedTopAdvert = true;
            return View(unReceivedOrders);
        }


        public PartialViewResult _MyOrders(int page, int? orderStatus)
        {
            var total = 0;

            IQueryable<OrdersEntity> orders;

            switch (orderStatus)
            {
                case (int)OrderStatusEnum.UnPay:
                    orders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.UnPay, page, 10,
                        out total);
                    break;
                case (int)OrderStatusEnum.Sended:
                    orders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.Sended, page, 10,
                        out total);
                    break;
                case (int)OrderStatusEnum.Done:
                    orders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, OrderStatusEnum.Done, page, 10,
                        out total);
                    break;
                default:
                    orders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, null, page, 10, out total);
                    break;
            }
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "myOrders.loadOrders");
            return PartialView(orders);
        }

        //我的订单详情
        public ActionResult MyOrder(string no)
        {
            ViewBag.MenuMyOrdersClassOver = "over";
            var result = _ordersHelper.GetMyOrderEntity(no, UserAuth.UserId);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                ViewBag.NoNeedTopAdvert = true;
                return View((OrdersEntity)result.AppendData);
            }
            ViewBag.Errmsg = result.ResultType.ToDescription() + result.Message;
            ViewBag.NoNeedTopAdvert = true;
            return View((OrdersEntity)result.AppendData);
        }

        //取消订单
        public JsonResult CancelOrder(string id)
        {
            var result = _ordersHelper.ModifyOrderStatus(id, OrderStatusEnum.Cancel, UserAuth.UserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //确认收货
        public JsonResult ConfirmReceipt(string id)
        {
            var result = _ordersHelper.ModifyOrderStatus(id, OrderStatusEnum.Received, UserAuth.UserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }


        #endregion

        #region 我的创业集市

        public ActionResult MyMarket(int page=1)
        {
            var total = 0;
            var data = _productsHelper.GetApplyEntities(UserAuth.UserId, page, 10, out total);
            ViewBag.IsAuthorizedSupplier = _suppliersHelper.IsSupplierAuthorized(UserAuth.UserId);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 10, total, "");
            ViewBag.MenuMyMarketClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }


        //申请成为创业者 视图
        public ActionResult ToBeSupplier()
        {
            ViewBag.NoNeedTopAdvert = true;
            var result = _suppliersHelper.AuthorizeSupplier(UserAuth.UserId);

            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return View();
            }
            if (result.ResultType.Equals(OperationResultType.NoChanged))
            {
                return RedirectToAction("ApplyProduct");
            }
            if (result.ResultType.Equals(OperationResultType.Error) ||
                result.ResultType.Equals(OperationResultType.IllegalOperation))
            {
                ViewBag.ErrMsg = result.ResultType.ToDescription() + result.Message;
                return View();
            }
            ViewBag.MenuMyMarketClassOver = "over";

            return View();
        }

        //申请成为创业者 处理
        [HttpPost]
        public JsonResult ToBeSupplier(string linkMan, string linkTel, string reason)
        {
            var result = _suppliersHelper.AddToBeSupplier(UserAuth.UserId, linkMan, linkTel, reason);
            return ReturnHelper.Instance.GetJsonResult(result);

        }

        //产品申请、编辑  视图
        [HttpGet]
        public ActionResult ApplyProduct(int id = 0)
        {
            ViewBag.NoNeedTopAdvert = true;
            ViewBag.MenuMyMarketClassOver = "over";
            ViewBag.Categories = CategoryHelper.Instance.GetUseableCategoriesInCache().Where(f => f.IsForMarket).ToList();
            var product = _productsHelper.GetApplyEntity(id);
            return View(product);
        }

        //产品申请、编辑  处理
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ApplyProduct(ProductsApplyEntity apply, int categoryId)
        {
            ViewBag.NoNeedTopAdvert = true;
            var result = apply.Id > 0 ? _productsHelper.ModifyProductApply(apply, UserAuth.UserId, categoryId) :
            _productsHelper.AddProductApply(apply, UserAuth.UserId, categoryId);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("MyMarket");
            }
            ViewBag.ErrMsg = result.ResultType.ToDescription() + result.Message;
            return View((ProductsApplyEntity)result.AppendData);
        }


        //商品上架视图
        [HttpGet]
        public ActionResult GoodsUp(int productId, int? goodsId = null)
        {
            ViewBag.NoNeedTopAdvert = true;
            var product = _productsHelper.GetEntity(productId);
            var goods = _goodsHelper.GetEntity(goodsId);
            ViewBag.Product = product;
            return View(goods);

        }

        //商品上架处理
        [HttpPost]
        public ActionResult GoodsUp(int productId, GoodsEntity goods, string remark)
        {
            ViewBag.NoNeedTopAdvert = true;
            OperationResult result = _goodsHelper.UpGoods(goods, productId, UserAuth.UserId, remark);

            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("MyMarket");
            }
            ViewBag.ErrMsg = result.Message + result.ResultType.ToDescription();
            return View();
        }

        //商品下架
        public JsonResult GoodsDown(int goodsId, string remark)
        {
            var result = _goodsHelper.DownGoods(goodsId, UserAuth.UserId, remark);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //删除未通过审核的集市产品
        public JsonResult DeleteMarketProduct(int id)
        {
            var result = _productsHelper.DeleteProductApply(id, UserAuth.UserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 我的积分兑换

        public ActionResult MyExchanges(int page = 1)
        {
            var total = 0;
            var data = _exchangesHelper.GetExchangeLogsEntities(UserAuth.UserId, page, 20, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 20, total, "");
            ViewBag.MenuMyExchangesClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        #endregion

        #region 我的评论

        public ActionResult MyComments(int page = 1)
        {
            var total = 0;
            var data = _commentsHelper.GetUserCommentsEntities(UserAuth.UserId, page, 20, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 20, total, "");
            ViewBag.MyCommentsMenuClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteComment(int id)
        {
            var result = _commentsHelper.DeleteComment(id, UserAuth.UserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }
        #endregion

        #region 我的站内信

        public ActionResult MyNotifies(int page = 1)
        {
            var total = 0;
            var data = _notifiesHelper.GetNotifiesEntities(UserAuth.UserId, page, 20, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 20, total, "");
            ViewBag.MyNotifiesMenuClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        public JsonResult ToBeRead(int id)
        {
            var result = _notifiesHelper.ToBeRead(UserAuth.UserId, id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //删除站内信
        public JsonResult DeleteMyNotify(int id)
        {
            var result = _notifiesHelper.DeleteNotify(id, UserAuth.UserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 我的优惠券

        public ActionResult MyCoupons(int page = 1)
        {
            var total = 0;
            var data = _couponsHelper.GetMyCouponsEntities(UserAuth.UserId, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.MyCouponsMenuClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        #endregion

        #region 账户管理

        public ActionResult MyAddresses()
        {
            var data = _usersHelper.GetUserAddressEntities(UserAuth.UserId);
            ViewBag.Schools = _schoolHelper.GetUseableSchoolsInCache(UserAuth.UserRegionId);
            ViewBag.Regions = _schoolHelper.GetUseableRegionInCache();
            ViewBag.MyAddressMenuClassOver = "over";
            ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        //编辑、新增地址
        [HttpPost]
        public JsonResult SaveAddress(UserAddressEntity address)
        {
            var result = _usersHelper.SaveAddress(UserAuth.UserId, address);
            var status = result.ResultType.Equals(OperationResultType.Success);
            if (status)
            {
                Session.Remove("regionId");
                Session.Remove("schoolId");
            }
            return Json(new
            {
                status = status,
                msg = result.ResultType.ToDescription() + result.Message,
                obj = result.ResultType.Equals(OperationResultType.Success) ? (UserAddressEntity)result.AppendData : null
            }, JsonRequestBehavior.AllowGet);
        }

        //删除地址
        public JsonResult DeleteAddress(int id)
        {
            var result = _usersHelper.DeleteAddress(UserAuth.UserId, id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //设置默认地址
        public JsonResult SetDefaultAddress(int id)
        {
            var result = _usersHelper.SetToDefaultAddress(UserAuth.UserId, id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //找回密码
        public ActionResult ModifyPassword()
        {
            ViewBag.NoNeedTopAdvert = true;
            ViewBag.ModifyPwdMenuClassOver = "over";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ModifyPassword(string oldPwd, string newPwd)
        {
            var result = _usersHelper.ModifyPwd(UserAuth.UserId, oldPwd, newPwd);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

    }
}
