using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Tools;
using Msg.Tools.Extensions;
using Msg.Utils;
using Msg.Web.App_Start;

namespace Msg.Web.Areas.Admin.Controllers
{
    [AdminAuthFilter]
    public class HomeController : Controller
    {
        #region - Variable -

        private readonly CommentsHelper _commentsHelper = UnityConfig.GetConfiguredContainer().Resolve<CommentsHelper>();
        private readonly OrdersHelper _ordersHelper = UnityConfig.GetConfiguredContainer().Resolve<OrdersHelper>();
        private readonly BrandsHelper _brandsHelper = UnityConfig.GetConfiguredContainer().Resolve<BrandsHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly NotifiesHelper _notifiesHelper = UnityConfig.GetConfiguredContainer().Resolve<NotifiesHelper>();
        private readonly CouponsHelper _couponsHelper = UnityConfig.GetConfiguredContainer().Resolve<CouponsHelper>();
        private readonly SchoolHelper _schoolHelper = UnityConfig.GetConfiguredContainer().Resolve<SchoolHelper>();
        private readonly CategoryHelper _categoryHelper = UnityConfig.GetConfiguredContainer().Resolve<CategoryHelper>();
        private readonly ProductsHelper _productsHelper = UnityConfig.GetConfiguredContainer().Resolve<ProductsHelper>();
        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();
        private readonly ExchangesHelper _exchangesHelper = UnityConfig.GetConfiguredContainer().Resolve<ExchangesHelper>();
        private readonly CreditGoodsHelper _creditGoodsHelper = UnityConfig.GetConfiguredContainer().Resolve<CreditGoodsHelper>();
        private readonly SupliersHelper _supliersHelper = UnityConfig.GetConfiguredContainer().Resolve<SupliersHelper>();
        #endregion


        //
        // GET: /Admin/Home/
        //
        public ActionResult Index()
        {
            ViewBag.NewComments =
                _commentsHelper.GetNewCommentsCountForAdminsSinceLastLoginTime(UserAuth.UserCache.LastLoginTime);
            ViewBag.NewOrders =
                _ordersHelper.GetNewOrderCountForAdminsSinceLastLoginTime(UserAuth.UserCache.LastLoginTime);
            ViewBag.NewProductApplys =
                _productsHelper.GetNewApplyCountForAdminsSinceLastLoginTime(UserAuth.UserCache.LastLoginTime);
            return View();
        }

        #region  学校管理

        //
        public ActionResult Schools(int page = 1)
        {
            var total = 0;
            var data = _schoolHelper.GetSchoolEntities(page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            return View(data);
        }

        //切换学校可用状态
        public JsonResult ToggleSchoolUseable(int schoolId)
        {
            var result = _schoolHelper.ToggleSchoolUseable(schoolId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //新增、编辑学校
        public JsonResult SaveSchool(SchoolEntity school, int regionId)
        {
            var result = _schoolHelper.SaveSchool(school, regionId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //获取缓存的区域
        public JsonResult GetCacheRegion()
        {
            var data = _schoolHelper.GetUseableRegionInCache();
            return Json(new { regions = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 区域管理

        public ActionResult Regions(int page = 1)
        {
            var total = 0;
            var data = _schoolHelper.GetRegionEntities(page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "");
            return View(data);
        }

        //切换学校可用状态
        public JsonResult ToggleRegionUseable(int id)
        {
            var result = _schoolHelper.ToggleRegionUseable(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //新增、编辑区域
        public JsonResult SaveRegion(RegionEntity region)
        {
            var result = _schoolHelper.SaveRegion(region);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion


        #region 类目管理

        public ActionResult Categories(int page = 1)
        {
            var total = 0;
            var data = _categoryHelper.GetCategoriesEntities(page, 30, out total);
            return View(data);
        }


        public JsonResult SaveCategory(int id, string name)
        {
            OperationResult result;
            result = id > 0 ? _categoryHelper.ModifyCategory(id, name) : _categoryHelper.AddCateGory(name);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        public JsonResult ToggleCateGoryUseable(int id)
        {
            var result = _categoryHelper.ToggleCateGoryUseable(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 评论管理

        /// <summary>
        /// 评论管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Comments(int page = 1)
        {
            var total = 0;
            var data = _commentsHelper.GetCommentsEntities(page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "", "", true);
            return View(data);
        }

        /// <summary>
        /// Deletes the comment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult DeleteComment(int id)
        {
            var result = _commentsHelper.DeleteComment(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        [HttpPost]
        public JsonResult ReplyComment(int id, int beReplyUserId, string content)
        {
            var result = _commentsHelper.ReplyComment(id, UserAuth.UserId, content, null, beReplyUserId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 品牌管理

        /// <summary>
        /// 品牌管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Brands(int page = 1)
        {
            var total = 0;
            var brands = _brandsHelper.GetBrandsModels(page, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 30, total, "", "", true);
            return View(brands);
        }

        //添加品牌
        public JsonResult AddBrand(BrandsModel brand)
        {
            var result = _brandsHelper.AddBrand(brand);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //编辑品牌
        public JsonResult ModifyBrand(BrandsModel brand)
        {
            var result = _brandsHelper.ModifyBrand(brand);
            return ReturnHelper.Instance.GetJsonResult(result);
        }


        //禁用品牌
        public JsonResult DisableBrand(int id)
        {
            var result = _brandsHelper.DisableBrand(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //删除品牌
        public JsonResult DeleteBrand(int id)
        {
            var result = _brandsHelper.DeleteBrand(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 订单管理

        /// <summary>
        /// 订单管理
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders(int page = 1)
        {
            var total = 0;
            var data = _ordersHelper.GetAllOrderListByPage(0, null, null, null, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging", "", true);
            ViewBag.Brands = _brandsHelper.GetUesebaleBrandsModelsInCache();
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache();
            return View(data);
        }

        //列表分布页
        public PartialViewResult _Orders(int? state, DateTime? dateBegin, DateTime? dateEnd, int? brandsId, int page = 1, int pagesize = 10, string orderBy = "", string nickName = "", string goodName = "", string mobile = "")
        {
            int total;
            var listOfOrders = _ordersHelper.GetAllOrderListByPage(state, dateBegin, dateEnd, brandsId, out total, page, pagesize, orderBy, nickName, goodName, mobile);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, pagesize, total, "paging", "", true);
            return PartialView(listOfOrders);
        }

        //订单详情页面
        public ActionResult OrderDetail(string orderNo)
        {
            var model = _ordersHelper.GetOrdersEntityByOrderNo(orderNo);

            ViewBag.InnerTopTitle = "订单编号：" + model.OrderNo;
            return View(model);

        }

        //修改订单状态
        public JsonResult ModifyOrder(string id, OrderStatusEnum orderStatus)
        {
            OperationResult result;
            if (orderStatus.Equals(OrderStatusEnum.Sended))
            {
                result = _ordersHelper.SendedGoods(id, UserAuth.UserId);
            }
            else
            {
                result = _ordersHelper.ModifyOrderStatus(id, orderStatus, UserAuth.UserId, isAdminOperate: true);
            }

            return ReturnHelper.Instance.GetJsonResult(result);
        }




        public ActionResult OutToExcel(int? state, DateTime? dateBegin, DateTime? dateEnd, int? brandsId, string nickName = "", string goodName = "", string mobile = "")
        {
            var result = _ordersHelper.OutToExcel(state, dateBegin, dateEnd, brandsId, nickName, goodName, mobile);

            return new EmptyResult();
        }



        #endregion

        #region 优惠券

        public ActionResult Coupons(string CouponsName, int page = 1)
        {
            var total = 0;
            var list = _couponsHelper.GetCouponsEntities(page, 10, out total, null, true, CouponsName);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 10, total, Request.Url.ToString(), "", true);
            ViewBag.CouponsName = CouponsName;
            return View(list);
        }


        //修改，新增优惠券  视图
        public ActionResult ModifyOrAddCoupons(string ModifyId, string edittype)
        {
            CouponsEntity couponsEntity = new CouponsEntity();
            if (!string.IsNullOrEmpty(ModifyId))
            {
                couponsEntity = _couponsHelper.GetEntity(ModifyId) ?? new CouponsEntity();
            }

            ViewBag.Type = edittype;
            ViewBag.InnerTopTitle = edittype == "Add" ? "新增优惠券" : "编辑优惠券";
            return View(couponsEntity);
        }


        public JsonResult ModifyOrAddCouponsToSolve(CouponsEntity couponsEntity, string edittype)
        {

            OperationResult result = _couponsHelper.AddOrModifyCoupons(couponsEntity, edittype);
            return ReturnHelper.Instance.GetJsonResult(result);

        }



        #endregion

        #region 积分商品

        public ActionResult CreditGoods(int page = 1)
        {
            var total = 0;
            var list = _creditGoodsHelper.GetCreditGoodsEntities(page, null, out total, null);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging", "", true);
            return View(list);
        }

        public PartialViewResult _CreditGoods(DateTime? date, bool? isVirtual, string cgoods = "", int page = 1)
        {
            var total = 0;
            var list = _creditGoodsHelper.GetCreditGoodsEntities(page, date, out total, isVirtual, cgoods);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging", "", true);
            return PartialView(list);
        }


        public ActionResult EditCreditGoods(string edittype)
        {
            int editId = Convert.ToInt32(Request["editId"]);
            ViewBag.edittype = edittype;
            ViewBag.InnerTopTitle = edittype == "Modify" ? "编辑积分商品" : "新增积分商品";
            var model = new CreditGoodsEntity();
            switch (edittype)
            {
                case "Modify":
                    model = _creditGoodsHelper.GetEntity(editId);
                    break;
                case "Add":
                    break;
            }
            return View(model);
        }


        [ValidateInput(false)]
        public JsonResult DealEditCreditGoods(CreditGoodsEntity model, int? editId, string edittype = "Add")
        {
            model.Id = editId.HasValue ? editId.Value : model.Id;
            var result = _creditGoodsHelper.DealCreaditGoodsEdit(model, edittype);
            return ReturnHelper.Instance.GetJsonResult(result);
        }





        #endregion

        #region 积分兑换记录

        public ActionResult CreditsExchangeLogs(int page = 1)
        {
            var total = 0;
            var data = _exchangesHelper.GetAllExchangeLogsEntities(out total, null, null);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging", "", true);
            return View(data);
        }



        //积分兑换分布页
        public PartialViewResult _CreditExhchangeLogs(int? quantity, bool? isSended, string nickName = "", string mobile = "", string goodName = "", int size = 10, int page = 1)
        {
            var total = 0;
            var data = _exchangesHelper.GetAllExchangeLogsEntities(out total, quantity, isSended, nickName, mobile, goodName, page, 10);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging", "", true);
            return PartialView(data);
        }


        //发货积分兑换商品
        public JsonResult SendCreditExchange()
        {
            var id = int.Parse(Request["id"]);
            var result = _exchangesHelper.SendCreditsExchange(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 创业集市

        public ActionResult SuppliersEntity(string linkMan, string userName, string linkTel, string product, int page = 1)
        {
            var total = 0;
            var data = _supliersHelper.GetAllSupliersEntities(linkMan, linkTel, userName, product, out total, page, 10);

            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 10, total, "paging");
            return View(data);
        }

        public JsonResult EditSupplierApply(int id, bool type, string refusdReadon)
        {
            return ReturnHelper.Instance.GetJsonResult(_supliersHelper.DealSupplierApply(id, type, refusdReadon));
        }

        public ActionResult ProductAuthority(string supplierName, int page = 1)
        {
            var total = 0;
            var data = _productsHelper.GetProductsApplyEntities("", supplierName, "", null, null, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "");
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache().Where(f => f.IsForMarket).ToList();
            ViewBag.SupplierName = supplierName;
            return View(data);
        }

        public PartialViewResult _ProductAuthority(string name, string supplierName, string brand, int? cateId, ApplyStatusEnum? statusEnum, int page = 1)
        {
            var total = 0;
            var data = _productsHelper.GetProductsApplyEntities(name,supplierName, brand, cateId, statusEnum, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "");
            return PartialView(data);
        }


        public JsonResult ModifyProductApplyStatus(int id, ApplyStatusEnum statusEnum, string remark)
        {
            var result = _productsHelper.ProductAuthority(id, statusEnum, remark);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 会员管理

        /// <summary>
        /// Userses the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public ActionResult Users(int page = 1)
        {
            var total = 0;
            var data = _usersHelper.GetUsersEntities(null, "", "", page, 30, out total);
            ViewBag.Schools = _schoolHelper.GetUseableSchoolsInCache();
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            return View(data);
        }

        /// <summary>
        /// Userses the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="nickName">Name of the nick.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <returns></returns>
        public PartialViewResult _Users(int? schoolId, string nickName, string sortColumn, int page = 1)
        {
            var total = 0;
            var data = _usersHelper.GetUsersEntities(schoolId, nickName, sortColumn, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            return PartialView(data);
        }

        /// <summary>
        /// Disables the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public JsonResult DisableUser(int id)
        {
            var result = _usersHelper.ToogleUseable(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //发送站内信
        [HttpPost]
        public JsonResult SendNotify(int toUserId, string content)
        {
            var result = _notifiesHelper.SendNotify(toUserId, "", content, NotifyTypeEnum.Admin);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //送优惠券
        public JsonResult SendCoupon(int toUserId, string couponId)
        {
            var result = _couponsHelper.SendCoupon(toUserId, couponId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }
        //批量发送站内信
        public JsonResult SendBatchNotify(string toUserId, string content)
        {
            var result = _notifiesHelper.SendNotifyBatch(toUserId, "", content, NotifyTypeEnum.Admin);
            return ReturnHelper.Instance.GetJsonResult(result);
        }
        //批量发送优惠券
        public JsonResult SendBathcCoupon(string toUserId, string couponId)
        {
            var result = _couponsHelper.SendCouponsBatch(toUserId, couponId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //获取可用优惠券
        public JsonResult GetCoupons(int couponType)
        {
            var data = _couponsHelper.GetUseableCouponsEntitiesByType((CouponTypeEnum)couponType);
            return Json(new { coupons = data }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 库存管理

        //库存列表
        public ActionResult Products(int page = 1, int? supplierId = null)
        {
            var total = 0;
            var data = _productsHelper.GetProductsEntities("", null, null, supplierId, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            ViewBag.Brands = _brandsHelper.GetUesebaleBrandsModelsInCache();
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache();
            ViewBag.SupplierId = supplierId;
            return View(data);
        }

        //列表分布页
        public PartialViewResult _Products(string name, int? brandId, int? cateId, string sortColumn, int? supplierId = null, int page = 1)
        {
            var total = 0;
            var data = _productsHelper.GetProductsEntities(name, brandId, cateId, supplierId, sortColumn, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            return PartialView(data);
        }

        //新增、编辑库存视图
        public ActionResult SaveProduct(int? id)
        {
            var entity = _productsHelper.GetEntity(id);
            ViewBag.Brands = _brandsHelper.GetUesebaleBrandsModelsInCache();
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache();
            return View(entity);
        }

        //新增、编辑处理
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SaveProduct(ProductsEntity products, int brandId, int cateId)
        {
            var result = _productsHelper.SaveProduct(products, cateId, brandId);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("Products");
            }

            ViewBag.ErrMsg = result.Message + result.ResultType.ToDescription();

            ViewBag.Brands = _brandsHelper.GetUesebaleBrandsModelsInCache();
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache();
            var model = (ProductsEntity)result.AppendData;

            return View(model);
        }

        //切换库存产品可用状态
        public JsonResult ToggleProductUseable(int id)
        {
            var result = _productsHelper.ToggleUseable(id);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion

        #region 上下架管理

        public ActionResult Goods(int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities("", null, null, "", page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            ViewBag.Brands = _brandsHelper.GetUesebaleBrandsModelsInCache();
            ViewBag.Categories = _categoryHelper.GetUseableCategoriesInCache();

            return View(data);
        }

        public PartialViewResult _Goods(string name, int? brandId, int? cateId, string sortColumn,
            int page = 1)
        {
            var total = 0;
            var data = _goodsHelper.GetSellingGoodsEntities(name, brandId, cateId, sortColumn, page, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(page, 30, total, "paging", "", true);
            return PartialView(data);
        }

        //修改商品 视图
        public ActionResult ModifyGoods(int id)
        {
            var goods = _goodsHelper.GetEntity(id);
            return View(goods);
        }

        //修改商品 处理
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModifyGoods(GoodsEntity goods)
        {
            var result = _goodsHelper.ModifyGoods(goods);
            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("Goods");
            }
            ViewBag.ErrMsg = result.Message + result.ResultType.ToDescription();
            return View(goods);
        }

        //商品上架视图
        public ActionResult GoodsUp(int productId, int? goodsId = null)
        {
            var product = _productsHelper.GetEntity(productId);
            var goods = _goodsHelper.GetEntity(goodsId);
            if (product != null)
            {
                goods.Product = product;
            }
            ViewBag.Product = product;
            return View(goods);

        }

        //商品上架处理
        [ValidateInput(false)]
        public ActionResult GoodsUpSave(int productId, GoodsEntity goods, string remark)
        {
            OperationResult result = _goodsHelper.UpGoods(goods, productId, UserAuth.UserId, remark);

            if (result.ResultType.Equals(OperationResultType.Success))
            {
                return RedirectToAction("Goods");
            }
            ViewBag.ErrMsg = result.Message + result.ResultType.ToDescription();
            var product = _productsHelper.GetEntity(productId);
            ViewBag.Product = product;
            return View("GoodsUp", goods);
        }


        //商品下架
        public JsonResult GoodsDown(int goodsId, string remark)
        {
            var result = _goodsHelper.DownGoods(goodsId, UserAuth.UserId, remark);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        //切换商品首页特推状态
        public JsonResult GoodsToIndex(int goodsId)
        {
            var result = _goodsHelper.ToggleGoodsToIndex(goodsId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }

        #endregion


    }
}
