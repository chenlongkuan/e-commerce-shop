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

namespace Msg.Web.Areas.Mobile.Controllers
{
    public class HomeController : Controller
    {
        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();
        private readonly CartHelper _cartHelper = UnityConfig.GetConfiguredContainer().Resolve<CartHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly CouponsHelper _couponsHelper = UnityConfig.GetConfiguredContainer().Resolve<CouponsHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();
        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();

        //首页 type=milk:小美奶客,snacks:零食，dailyUse:日化，live:生活，market:创业
        public ActionResult Index(string type)
        {
            var index = 1;
            var size = 6;
            var total = 0;
            IQueryable<GoodsEntity> list = null;
            var divStr = "热门购买";
            switch (type)
            {
                case "milk"://奶客
                    var brandId1 = _goodsHelper.GetBrandIdFromBrandDescInCache("yili");
                    var brandId2 = _goodsHelper.GetBrandIdFromBrandDescInCache("mengniu");
                    var brandId3 = _goodsHelper.GetBrandIdFromBrandDescInCache("tianyou");
                    var brandId4 = _goodsHelper.GetBrandIdFromBrandDescInCache("dream");
                    var brandId5 = _goodsHelper.GetBrandIdFromBrandDescInCache("smash");
                    int?[] brandIds = { brandId1, brandId2, brandId3, brandId4, brandId5 };
                    list = _goodsHelper.GetMobileIndex(brandIds, null, null, index, size, out total);
                    divStr = "小美奶客";
                    break;
                case "snacks"://零食
                    int?[] cateIds = { (int)CategoriesEnum.Snacks, (int)CategoriesEnum.Nuts, (int)CategoriesEnum.InstantFood, (int)CategoriesEnum.Drinks, (int)CategoriesEnum.Specialty };
                    list = _goodsHelper.GetMobileIndex(null, cateIds, null, index, size, out total);
                    divStr = "小美零食";
                    break;
                case "dailyUse"://日化
                    int?[] cateIds2 = { (int)CategoriesEnum.DayliUse };
                    list = _goodsHelper.GetMobileIndex(null, cateIds2, null, index, size, out total);
                    divStr = "小美日用";
                    break;
                case "live"://生活
                    int?[] cateIds3 = { (int)CategoriesEnum.Travel, (int)CategoriesEnum.Tickets, (int)CategoriesEnum.StuSupplies };
                    list = _goodsHelper.GetMobileIndex(null, cateIds3, null, index, size, out total);
                    divStr = "小美生活";
                    break;
                case "market"://创业
                    int?[] cateIds4 = { (int)CategoriesEnum.DayliUse, (int)CategoriesEnum.SkinCare, (int)CategoriesEnum.Integration };
                    list = _goodsHelper.GetMobileIndex(null, cateIds4, true, index, size, out total);
                    divStr = "创业集市";
                    break;
                default://首页
                    list = _goodsHelper.GetMobileIndex(null, null, null, index, size, out total);
                    break;
            }
            ViewBag.divStr = divStr;
            ViewBag.type = type;
            return View(list);
        }

        // type=milk:小美奶客,Snacks:零食，dailyUse:日化，live:生活，market:创业
        public PartialViewResult _Index(string type, int page = 1)
        {
            var size = 6;
            var total = 0;
            IQueryable<GoodsEntity> list = null;
            switch (type)
            {
                case "milk"://奶客
                    var brandId1 = _goodsHelper.GetBrandIdFromBrandDescInCache("yili");
                    var brandId2 = _goodsHelper.GetBrandIdFromBrandDescInCache("mengniu");
                    var brandId3 = _goodsHelper.GetBrandIdFromBrandDescInCache("tianyou");
                    var brandId4 = _goodsHelper.GetBrandIdFromBrandDescInCache("dream");
                    var brandId5 = _goodsHelper.GetBrandIdFromBrandDescInCache("smash");
                    int?[] brandIds = { brandId1, brandId2, brandId3, brandId4, brandId5 };
                    list = _goodsHelper.GetMobileIndex(brandIds, null, null, page, size, out total);
                    break;
                case "snacks"://零食
                    int?[] cateIds = { (int)CategoriesEnum.Snacks, (int)CategoriesEnum.Nuts, (int)CategoriesEnum.InstantFood, (int)CategoriesEnum.Drinks, (int)CategoriesEnum.Specialty };
                    list = _goodsHelper.GetMobileIndex(null, cateIds, null, page, size, out total);
                    break;
                case "dailyUse"://日化
                    int?[] cateIds2 = { (int)CategoriesEnum.DayliUse };
                    list = _goodsHelper.GetMobileIndex(null, cateIds2, null, page, size, out total);
                    break;
                case "live"://生活
                    int?[] cateIds3 = { (int)CategoriesEnum.Travel, (int)CategoriesEnum.Tickets, (int)CategoriesEnum.StuSupplies };
                    list = _goodsHelper.GetMobileIndex(null, cateIds3, null, page, size, out total);
                    break;
                case "market"://创业
                    int?[] cateIds4 = { (int)CategoriesEnum.DayliUse, (int)CategoriesEnum.SkinCare, (int)CategoriesEnum.Integration };
                    list = _goodsHelper.GetMobileIndex(null, cateIds4, true, page, size, out total);
                    break;
                default://首页
                    list = _goodsHelper.GetMobileIndex(null, null, null, page, size, out total);
                    break;
            }
            ViewBag.type = type;
            return PartialView(list);
        }

        //商品详情  
        public ActionResult Details(int id)
        {
            var goods = _goodsHelper.GetEntity(id);
            return View(goods);
        }

        //购物车
        public ActionResult Cart()
        {
            var data = _cartHelper.GetCartModels();
            return View(data);
        }

        //结算页面,id：选中的优惠卷Id sendTimeType: 0 上午时间，1：下午时间
        [MobileAuthFilters]
        public ActionResult CheckOut(string p, int id = 0, int addressId = 0, int sendTimeType = -1, int sendDate = 0)
        {
            var result = _cartHelper.GetCheckOutGoods(p, UserAuth.UserId);
            var cartData = (List<CartModel>)result.AppendData;
            if (result.ResultType.Equals(OperationResultType.Success) && cartData != null)
            {
                ViewBag.Address = _usersHelper.GetUserDefaultAddressEntities(UserAuth.UserId, addressId);
                ViewBag.Coupons = _couponsHelper.GetMyUnUseCoupons(UserAuth.UserId, cartData.Sum(f => f.SellPrice), 4);
            }
            else
            {
                ViewBag.ErrMsg = result.Message;
            }
            ViewBag.p = p;
            ViewBag.couponId = id;
            ViewBag.addressId = addressId;
            ViewBag.sendDate = sendDate;

            var noonExpireTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
            var nightExpireTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17, 0, 0);
            var sendDateStr = DateTime.Now.AddDays(DateTime.Now >= nightExpireTime ? sendDate + 1 : sendDate).ToString("M月dd日");
            ViewBag.sendDateStr = sendDateStr;
            if (sendTimeType >= 0)
            {
                ViewBag.SendTimeBuckets = sendTimeType == 0 ? SendTimeBucketsEnum.Noon.ToString() : SendTimeBucketsEnum.AfterNoon.ToString();
                ViewBag.SendTimeBucketsStr = sendTimeType == 0 ? SendTimeBucketsEnum.Noon.ToDescription() : SendTimeBucketsEnum.AfterNoon.ToDescription();
            }
            else
            {
                if (!(DateTime.Now < noonExpireTime || DateTime.Now >= nightExpireTime))
                {
                    sendTimeType = 1;
                    ViewBag.SendTimeBuckets = SendTimeBucketsEnum.AfterNoon.ToString();
                    ViewBag.SendTimeBucketsStr = SendTimeBucketsEnum.AfterNoon.ToDescription();
                }
                else
                {
                    sendTimeType = 0;
                    ViewBag.SendTimeBuckets = SendTimeBucketsEnum.Noon.ToString();
                    ViewBag.SendTimeBucketsStr = SendTimeBucketsEnum.Noon.ToDescription();
                }
            }
            ViewBag.sendTimeType = sendTimeType;
            return View(cartData);
        }

        //选择收货地址
        public ActionResult SelectAddresses(string p, int couponId = 0)
        {
            ViewBag.p = p;
            ViewBag.couponId = couponId;
            var data = _usersHelper.GetUserAddressEntities(UserAuth.UserId);

            return View(data);
        }

        //选择配送时间 sendTimeType: 0 上午时间，1：下午时间
        public ActionResult SelectTime(string p, int sendTimeType = 0, int sendDate = 0, int couponId = 0)
        {
            ViewBag.p = p;
            ViewBag.couponId = couponId;
            ViewBag.sendTimeType = sendTimeType;
            ViewBag.sendDate = sendDate;
            return View();
        }

        //选择优惠卷
        public ActionResult SelectCoupon(string p, int id = 0)
        {
            ViewBag.p = p;
            ViewBag.couponId = id;
            var result = _cartHelper.GetCheckOutGoods(p, UserAuth.UserId);
            var cartData = (List<CartModel>)result.AppendData;
            List<CouponSendLogModel> coupons = null;
            if (cartData != null)
            {
                coupons = _couponsHelper.GetMyUnUseCoupons(UserAuth.UserId, cartData.Sum(f => f.SellPrice), 4);
            }
            return View(coupons);
        }

        //搜索
        public ActionResult Search(string key, string order, int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities(key, null, null, order, page, 10, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.Key = key;
            ViewBag.TotalCount = total;
            return View(data);
        }

        public PartialViewResult _Search(string key, string order, int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities(key, null, null, order, page, 10, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            ViewBag.Key = key;
            ViewBag.TotalCount = total;
            return PartialView(data);
        }

        [MobileAuthFilters]
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
            return View();
        }
    }
}
