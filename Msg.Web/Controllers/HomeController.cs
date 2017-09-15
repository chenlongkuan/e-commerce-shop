using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Extensions;
using Msg.Utils;
using Msg.Web.App_Start;

namespace Msg.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();
        private readonly CommentsHelper _commentsHelper = UnityConfig.GetConfiguredContainer().Resolve<CommentsHelper>();
        private readonly CartHelper _cartHelper = UnityConfig.GetConfiguredContainer().Resolve<CartHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly CouponsHelper _couponsHelper = UnityConfig.GetConfiguredContainer().Resolve<CouponsHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();
        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();

        private const int IndexMilkTakeSize = 6;
        private const int IndexSnacksTakeSize = 6;

        //首页
        public ActionResult Index()
        {


            //小美奶客
            ViewBag.YiliMilks = _goodsHelper.GetMilksOfIndex("yili", IndexMilkTakeSize);//伊利
            ViewBag.MengniuMilks = _goodsHelper.GetMilksOfIndex("mengniu", IndexMilkTakeSize);//蒙牛
            ViewBag.TianyouMilks = _goodsHelper.GetMilksOfIndex("tianyou", IndexMilkTakeSize);//天友
            ViewBag.DreamMilks = _goodsHelper.GetMilksOfIndex("dream", IndexMilkTakeSize);//奶牛梦工厂
            //ViewBag.SmashMilks = _goodsHelper.GetMilksOfIndex(5, IndexMilkTakeSize);//smash cake
            //小美零食
            ViewBag.NutsFood = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Nuts, IndexSnacksTakeSize);
            ViewBag.InstantFood = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.InstantFood, IndexSnacksTakeSize);
            ViewBag.Drinks = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Drinks, IndexSnacksTakeSize);
            ViewBag.Snacks = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Snacks, IndexSnacksTakeSize);

            ViewBag.DailyUse = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.DayliUse, 5);

            //小美生活
            ViewBag.Travels = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Travel, 4);
            ViewBag.Tickets = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Tickets, 4);
            ViewBag.StuSupplies = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.StuSupplies, 4);

            ViewBag.NoNeedBottomAdvert = true;
            return View();
        }

        //首页创业格子、积分兑换数据
        public JsonResult IndexAjaxData(string further, int? cbegin = 0, int? cend = 100)
        {
            switch (further)
            {
                case "market":
                    var esdata = _goodsHelper.GetEsGridGoodsEntities(10);
                    return Json(new { esdata }, JsonRequestBehavior.AllowGet);
                    break;
                case "credits":
                    var credits = _goodsHelper.GetUseAbleCreditGoodsEntities(cbegin.Value, cend.Value, 5);
                    return Json(new { credits }, JsonRequestBehavior.AllowGet);
                    break;
            }
            return Json(new { }, JsonRequestBehavior.DenyGet);
        }

        //小美零食
        public ActionResult Snacks(string cate, int page = 1)
        {
            var cateId = (int)CategoriesEnum.Snacks;
            switch (cate)
            {
                case "leisure":
                    ViewBag.LeiSureOver = "over";
                    cateId = (int)CategoriesEnum.Snacks;
                    break;
                case "nuts":
                    ViewBag.NutsOver = "over";
                    cateId = (int)CategoriesEnum.Nuts;
                    break;
                case "instant":
                    ViewBag.InstantOver = "over";
                    cateId = (int)CategoriesEnum.InstantFood;
                    break;
                case "drinks":
                    ViewBag.DrinksOver = "over";
                    cateId = (int)CategoriesEnum.Drinks;
                    break;
                case "specialty":
                    ViewBag.Specialty = "over";
                    cateId = (int)CategoriesEnum.Specialty;
                    break;
                default:
                    ViewBag.LeiSureOver = "over";
                    cateId = (int)CategoriesEnum.Snacks;
                    break;
            }

            var total = 0;
            var snacks = _goodsHelper.GetSellingGoodsEntities("", null, cateId, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(snacks);
        }

        //小美奶客
        public ActionResult Milk(string brand = "mengniu", int page = 1)
        {
            var brandId = 1;
            switch (brand)
            {
                case "yili":
                    ViewBag.YiliOver = "over";
                    break;
                case "mengniu":
                    ViewBag.MnOver = "over";
                    break;
                case "tianyou":
                    ViewBag.TyOver = "over";
                    break;
                case "dream":
                    ViewBag.DreamOver = "over";
                    break;
                case "smash":
                    ViewBag.SmashOver = "over";
                    break;
                default:
                    ViewBag.MnOver = "over";
                    break;
            }
            brandId = _goodsHelper.GetBrandIdFromBrandDescInCache(brand);

            var total = 0;
            var milks = _goodsHelper.GetSellingGoodsEntities("", brandId, (int)CategoriesEnum.Milk, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(milks);
        }

        //小美生活
        public ActionResult Live()
        {
            ViewBag.Travels = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Travel, 6);
            ViewBag.Tickets = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.Tickets, 6);
            ViewBag.StuSupplies = _goodsHelper.GetIndexDataByCateId((int)CategoriesEnum.StuSupplies, 6);
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View();
        }

        // 小美旅游
        public ActionResult Travels(int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities("", null, (int)CategoriesEnum.Travel, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        //小美票务
        public ActionResult Tickets(int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities("", null, (int)CategoriesEnum.Tickets, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        //学子用品
        public ActionResult StuSupplies(int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities("", null, (int)CategoriesEnum.StuSupplies, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        //小美日用
        public ActionResult DailyUse(int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities("", null, (int)CategoriesEnum.DayliUse, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        //创业集市
        public ActionResult Market()
        {
            var total = 0;
            var data = _goodsHelper.GetEsGridGoodsEntities(CategoriesEnum.DayliUse, 1, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(1, 30, total, "pagingMarket");
            ViewBag.CateId = CategoriesEnum.DayliUse;
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        public PartialViewResult _Market(CategoriesEnum cateId, int page = 1)
        {
            var total = 0;
            switch (cateId)
            {
                case CategoriesEnum.DayliUse:
                    ViewBag.DayliUseTabClassOver = "over";
                    break;
                case CategoriesEnum.SkinCare:
                    ViewBag.SkinCareTabClassOver = "over";
                    break;
                case CategoriesEnum.Integration:
                    ViewBag.IntegrationTabClassOver = "over";
                    break;
                default:
                    ViewBag.DayliUseTabClassOver = "over";
                    break;
            }
            var data = _goodsHelper.GetEsGridGoodsEntities(cateId, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "pagingMarket");
            ViewBag.CateId = cateId;
            return PartialView(data);
        }

        //搜索
        public ActionResult Search(string key, int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities(key, null, null, "soldCount", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.Key = key;
            ViewBag.TotalCount = total;
            ViewBag.NoNeedTopAdvert = true;
            ViewBag.NoNeedBanner = true;
            return View(data);
        }

        //商品详情  
        public ActionResult Details(int id, int page = 1)
        {
            var total = 0;
            var goods = _goodsHelper.GetEntity(id);
            ViewBag.Comments = _commentsHelper.GetGoodsCommentsEntities(id, page, 10, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 10, total, "");
            ViewBag.PageNum = page;

            _goodsHelper.SaveGoodsViewHistory(id);

            ViewBag.HotSelling = _goodsHelper.GetHotSellingGoodsEntities(5, 1);
            ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);

            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(goods);
        }

        //为你推荐/猜你喜欢
        public PartialViewResult _RecommendForyouGoods()
        {
            var data = _goodsHelper.GetRecommendGoodsEntities(6);
            return PartialView(data);
        }

        //热销产品
        public PartialViewResult _HotSellingGoods(int page = 1)
        {
            var data = _goodsHelper.GetHotSellingGoodsEntities(5, page);
            ViewBag.PageNo = page;
            return PartialView(data);
        }

        //评论
        [HttpPost]
        [AuthFilters]
        public PartialViewResult _AddComment(string content, int replyTo, int goodsId, int beReplyedUserId, int page)
        {
            string pageUrl = Url.Action("Details", new { id = goodsId, page = page });
            var result = _commentsHelper.ReplyComment(replyTo, UserAuth.UserId, content, goodsId, beReplyedUserId, pageUrl);
            //return ReturnHelper.Instance.GetJsonResult(result);
            ViewBag.Comments = beReplyedUserId == 0 ? (CommentsEntity)result.AppendData : null;
            ViewBag.FollowComments = beReplyedUserId > 0 ? (CommentsFollowEntity)result.AppendData : null;
            return PartialView();
        }

        //购物车
        public ActionResult Cart()
        {
            var data = _cartHelper.GetCartModels();
            ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
            ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
            ViewBag.NoNeedTopNav = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }



        //从购物删除商品
        public JsonResult RemoveGoodsFromCart(string ids)
        {
            var result = _cartHelper.RemoveFromCart(ids);
            return Json(new
            {
                status = result.ResultType.Equals(OperationResultType.Success),
                msg = result.ResultType.ToDescription() + result.Message,
                quantity = (int)result.AppendData
            }, JsonRequestBehavior.AllowGet);
        }

        //修改购物车
        public JsonResult UpdateCart(int goodsId, int quantity = 1, string type = "add")
        {
            var result = _cartHelper.UpdateQuantity(goodsId, quantity, type);
            return Json(new
            {
                status = result.ResultType.Equals(OperationResultType.Success),
                msg = result.ResultType.ToDescription() + result.Message,
                update = (CartUpdateModel)result.AppendData
            }, JsonRequestBehavior.AllowGet);
        }

        //结算页面
        [AuthFilters]
        public ActionResult CheckOut(string p)
        {
            var result = _cartHelper.GetCheckOutGoods(p, UserAuth.UserId);
            var cartData = (List<CartModel>)result.AppendData;
            if (result.ResultType.Equals(OperationResultType.Success) && cartData != null)
            {
                ViewBag.Address = _usersHelper.GetUserAddressEntities(UserAuth.UserId);
                ViewBag.Coupons = _couponsHelper.GetMyUnUseCoupons(UserAuth.UserId, cartData.Sum(f => f.SellPrice), 4);
                ViewBag.Regions = _schoolHelper.GetUseableRegionInCache();
                ViewBag.School = _schoolHelper.GetUseableSchoolsInCache(UserAuth.UserRegionId);
            }
            else
            {
                ViewBag.ErrMsg = result.Message;
            }
            ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
            ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
            ViewBag.NoNeedTopNav = true; ViewBag.NoNeedTopAdvert = true;
            return View(cartData);
        }

        //创建订单
        [AuthFilters]
        public ActionResult CreateOrder(int addressId, string payway, string expressway, DateTime sendTimeDate, string sendTimeBuckets, int couponId)
        {
            ViewBag.NoNeedTopNav = true;
            ViewBag.NoNeedTopAdvert = true;
            var result = _ordersHelper.CreateOrder(addressId, payway, expressway, sendTimeDate, sendTimeBuckets, couponId,
                UserAuth.UserId);
            if (result.ResultType.Equals(OperationResultType.Success) && payway.Equals(PayWayEnum.CashDelivery.ToString()))
            {
                ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
                ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
                ViewBag.ErrMsg = result.Message;
                return View();
            }
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("Payment", "Alipay", new { orderNo = ((OrdersEntity)result.AppendData).OrderNo });
            }
            ViewBag.GuessYouLike = _goodsHelper.GetRecommendGoodsEntities(6);
            ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
            ViewBag.ErrMsg = result.Message;
            return View();
        }


    }
}

