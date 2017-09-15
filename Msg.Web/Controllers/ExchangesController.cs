using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Msg.Bll.Helpers;
using Msg.Bll.Models;
using Msg.Utils;
using Msg.Web.App_Start;

namespace Msg.Web.Controllers
{
    //积分兑换
    public class ExchangesController : Controller
    {
        private readonly GoodsHelper _goodsHelper = UnityConfig.GetConfiguredContainer().Resolve<GoodsHelper>();
        private readonly CommentsHelper _commentsHelper = UnityConfig.GetConfiguredContainer().Resolve<CommentsHelper>();
        private readonly UsersHelper _usersHelper = UnityConfig.GetConfiguredContainer().Resolve<UsersHelper>();
        private readonly ExchangesHelper _exchangesHelper = UnityConfig.GetConfiguredContainer().Resolve<ExchangesHelper>();

        //
        // GET: /Exchanges/

        public ActionResult Index()
        {
            var total = 0;
            var data = _goodsHelper.GetCreditGoods(0, 1000, 1, 30, out total);
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(1, 30, total, "loadCreditGoods");
            ViewBag.NoNeedBanner = true; ViewBag.NoNeedTopAdvert = true;
            return View(data);
        }

        public PartialViewResult _Index(int tabId, int page = 1)
        {
            var total = 0;
            List<CreditGoodsLiteModel> data;
            switch (tabId)
            {
                case 1:
                    data = _goodsHelper.GetCreditGoods(0, 1000, page, 30, out total);
                    break;
                case 2:
                    data = _goodsHelper.GetCreditGoods(1001, 3000, page, 30, out total);
                    break;
                case 3:
                    data = _goodsHelper.GetCreditGoods(3001, 5000, page, 30, out total);
                    break;
                case 4:
                    data = _goodsHelper.GetCreditGoods(5001, 8000, page, 30, out total);
                    break;
                default:
                    data = _goodsHelper.GetCreditGoods(8000, 9999999, page, 30, out total);
                    break;
            }
            ViewBag.Pager = PagerHelper.CreatePagerByAjax(1, 30, total, "loadCreditGoods");
            return PartialView(data);
        }

        //积分兑换商品详情
        public ActionResult Item(int id, int page = 1)
        {
            var total = 0;
            var model = _goodsHelper.GetCreditGoods(id);
            ViewBag.Comments = _commentsHelper.GetCreditGoodsCommentsEntities(id, page, 10, out total);
            ViewBag.HotSelling = _goodsHelper.GetHotSellingGoodsEntities(5, 1);
            ViewBag.ViewHistory = _goodsHelper.GetGoodsViewHistory(5);
            ViewBag.Pager = PagerHelper.CreatePagerByUrl(page, 10, total, "");
            ViewBag.PageNum = page;
            ViewBag.NoNeedTopAdvert = true;
            ViewBag.NoNeedBanner = true;
            return View(model);
        }

        //我的地址
        [AuthFilters]
        public PartialViewResult _MyAddress()
        {
            var data = _usersHelper.GetUserAddressEntities(UserAuth.UserId);
            return PartialView(data);
        }

        //处理兑换
        [AuthFilters]
        public JsonResult ToExchange(int goodsId, int quantiy, int addressId)
        {
            var result = _exchangesHelper.ExchangeGoods(UserAuth.UserId, goodsId, quantiy, addressId);
            return ReturnHelper.Instance.GetJsonResult(result);
        }



    }
}
