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

namespace Msg.Web.Areas.Mobile.Controllers
{
    public class MeController : Controller
    {
        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly CouponsHelper _couponsHelper = UnityConfig.GetConfiguredContainer().Resolve<CouponsHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();

        #region 首页

        public ActionResult Index()
        {
            if (UserAuth.IsAuthenticated)
            {
                //订单数量
                ViewBag.orderCount = _ordersHelper.GetOrderCountByUid(UserAuth.UserId);
            }
            return View();
        }

        #endregion

        #region 我的订单

        //我的订单
        [MobileAuthFilters]
        public ActionResult MyOrders()
        {
            var total = 0;
            ViewBag.MenuMyOrdersClassOver = "over";
            //默认未支付的订单
            var unReceivedOrders = _ordersHelper.GetUserOrdersByStatus(UserAuth.UserId, null, 1, 10, out total);
            ViewBag.NoNeedTopAdvert = true;
            return View(unReceivedOrders);
        }

        [MobileAuthFilters]
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

        //取消多个订单
        [MobileAuthFilters]
        public JsonResult CancelOrders(string ids)
        {
            OperationResult result = null;
            if (!string.IsNullOrEmpty(ids))
            {
                if (ids.Contains(","))
                {
                    var arrIds = ids.Split(',');
                    foreach (string id in arrIds)
                    {
                        result = _ordersHelper.ModifyOrderStatus(id, OrderStatusEnum.Cancel, UserAuth.UserId);
                    }
                }
            }
            else
            {
                result = _ordersHelper.ModifyOrderStatus(ids, OrderStatusEnum.Cancel, UserAuth.UserId);
            }
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 我的优惠券

        [MobileAuthFilters]
        public ActionResult MyCoupons(int page = 1)
        {
            var total = 0;
            var data = _couponsHelper.GetMyCouponsEntities(UserAuth.UserId, page, 10, out total);
            return View(data);
        }

        [MobileAuthFilters]
        public PartialViewResult _MyCoupons(int page = 1)
        {
            var total = 0;
            var data = _couponsHelper.GetMyCouponsEntities(UserAuth.UserId, page, 10, out total);
            return PartialView(data);
        }

        #endregion

        #region 收货地址管理

        //收货地址管理
        [MobileAuthFilters]
        public ActionResult MyAddresses()
        {
            var data = _usersHelper.GetUserAddressEntities(UserAuth.UserId);
            ViewBag.Schools = _schoolHelper.GetUseableSchoolsInCache(UserAuth.UserRegionId);
            ViewBag.Regions = _schoolHelper.GetUseableRegionInCache();
            return View(data);
        }

        //添加、修改收货地址 id:收货地址id
        [MobileAuthFilters]
        public ActionResult EditAddress(string p, int id = 0, int couponId = 0)
        {
            ViewBag.p = p;
            ViewBag.couponId = couponId;
            ViewBag.id = id;
            UserAddressEntity userAddress = null;
            if (id > 0)
            {
                userAddress = _usersHelper.GetUserAddressEntity(id);
            }
            if (userAddress == null)
            {
                userAddress = new UserAddressEntity { RegionId = UserAuth.UserRegionId, SchoolId = UserAuth.SchoolId };
            }

            var regionId = userAddress.RegionId;
            var schoolId = userAddress.SchoolId;
            if (Session["regionId"] != null)
            {
                try
                {
                    regionId = int.Parse(Session["regionId"].ToString());
                }
                catch (System.Exception)
                {
                }
            }
            if (Session["schoolId"] != null)
            {
                try
                {
                    schoolId = int.Parse(Session["schoolId"].ToString());
                }
                catch (System.Exception)
                {
                }
            }
            var list = _schoolHelper.GetUseableRegionInCache();
            var defaultRegion = regionId > 1 ? list.FirstOrDefault(r => r.Id == regionId) : list.FirstOrDefault();
            var regionName = "";
            if (defaultRegion == null)
            {
                regionId = 1;
                regionName = "重庆市";
            }
            else
            {
                regionId = defaultRegion.Id;
                regionName = defaultRegion.Name;
            }
            ViewBag.regionId = regionId;
            ViewBag.regionName = regionName;
            if (regionId > 0 && schoolId > 0)
            {
                ViewBag.schoolId = schoolId;
                var school = _schoolHelper.GetUseableSchoolsInCache(regionId).FirstOrDefault(r => r.Id == schoolId);
                if (school != null)
                {
                    ViewBag.schoolName = school.Name;
                }
            }

            return View(userAddress);
        }

        #endregion

    }
}
