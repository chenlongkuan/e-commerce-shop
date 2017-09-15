using System;
using System.Collections.Generic;
using System.Linq;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Redis;
using Msg.Tools;
using Msg.Tools.Logging;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 购物车逻辑帮助类
    /// </summary>
    public class CartHelper
    {
        public const string CartCookieKey = "MyCart";


        #region 单例

        private static CartHelper _instance;

        public static CartHelper Instance
        {
            get { return _instance ?? (_instance = new CartHelper()); }
        }

        #endregion


        private List<CartModel> GetCartData()
        {
            var cookies = Utils.Utils.GetCookie(CartCookieKey);
            var cartData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartModel>>(cookies);
            return cartData ?? new List<CartModel>();
        }

        private void WirteDataToCookie(List<CartModel> cartdata)
        {
            var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(cartdata);
            Utils.Utils.WriteCookie(CartCookieKey, cartJson, 43200);
        }

        /// <summary>
        /// Gets the cart count.
        /// </summary>
        /// <returns></returns>
        public int GetCartCount()
        {
            var cookies = Utils.Utils.GetCookie(CartCookieKey);
            var cartData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartModel>>(cookies);
            return cartData == null ? 0 : cartData.Sum(f => f.Quantity);
        }

        private OperationResult UpdateCartData(int goodsId, int quantity, string type, ref List<CartModel> cartdata)
        {
            try
            {

                var goods = GoodsHelper.Instance.GetEntity(goodsId);
                var cartUpdate = new CartUpdateModel()
                {
                    GoodsId = goodsId,
                    LeftQuantity = goods.Product.Quantity - goods.SoldCount
                };

                if (cartdata.Count == 0 && cartUpdate.LeftQuantity > 1)
                {
                    if (quantity > 0)
                    {
                        cartdata = new List<CartModel> { new CartModel() { GoodsId = goodsId, Quantity = quantity } };

                    }
                    cartUpdate.Quantity = quantity > 0 ? quantity : 0;
                }
                else if (cartUpdate.LeftQuantity > 2)
                {
                    if (cartdata.Any(f => f.GoodsId == goodsId) && quantity > 0)
                    {
                        var existsGoods = cartdata.SingleOrDefault(f => f.GoodsId == goodsId);

                        if (existsGoods != null)
                        {
                            cartdata.Remove(existsGoods);
                            switch (type)
                            {
                                case "add":
                                    existsGoods.Quantity += quantity;
                                    break;
                                case "update":
                                    existsGoods.Quantity = quantity;
                                    break;
                                default:
                                    existsGoods.Quantity += 1;
                                    break;
                            }

                            cartdata.Add(existsGoods);
                        }

                    }
                    else if (quantity > 0)
                    {
                        cartdata.Add(new CartModel() { GoodsId = goodsId, Quantity = quantity });

                    }
                    cartUpdate.Quantity = cartdata.Sum(f => f.Quantity);

                }

                return new OperationResult(OperationResultType.Success, "", cartUpdate);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("修改购物车数量异常", ex);
                return new OperationResult(OperationResultType.Error, "修改购物车数量异常", 0);
            }
        }

        /// <summary>
        /// Updates the quantity.
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="type">add/update</param>
        /// <returns>购物车商品数量</returns>
        public OperationResult UpdateQuantity(int goodsId, int quantity, string type)
        {
            var cartData = GetCartData();
            var result = UpdateCartData(goodsId, quantity, type, ref cartData);
            WirteDataToCookie(cartData);
            return result;
        }

        /// <summary>
        /// Removes from cart.
        /// </summary>
        /// <param name="goodsIds">The goods ids.</param>
        /// <returns></returns>
        public OperationResult RemoveFromCart(string goodsIds)
        {
            var cartData = GetCartData();
            var result = 0;
            if (goodsIds.Contains(","))
            {
                try
                {
                    var idList = goodsIds.Split(',').Select(int.Parse).ToList();
                    result += idList.Sum(i => RemoveFromCart(i, ref cartData));
                    WirteDataToCookie(cartData);
                }
                catch
                {
                    return new OperationResult(OperationResultType.Error, "参数错误");
                }
            }
            else
            {
                var goodsId = 0;
                if (int.TryParse(goodsIds, out goodsId))
                {
                    result += RemoveFromCart(goodsId, ref cartData);
                    WirteDataToCookie(cartData);
                }
            }
            return new OperationResult(OperationResultType.Success, "", cartData.Sum(f => f.Quantity));
        }


        /// <summary>
        ///从购物车删除商品
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <returns>购物车商品数量</returns>
        private int RemoveFromCart(int goodsId, ref List<CartModel> cartData)
        {

            if (cartData.Count == 0)
            {
                return 0;
            }
            if (cartData.Any(f => f.GoodsId == goodsId))
            {
                var item = cartData.SingleOrDefault(f => f.GoodsId == goodsId);
                cartData.Remove(item);

            }
            return cartData.Count;
        }


        /// <summary>
        /// 获取某用户的购物车
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public List<CartModel> GetCartModels()
        {
            var cookies = Utils.Utils.GetCookie(CartCookieKey);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartModel>>(cookies);

            var goodsRep = EfRepository<GoodsEntity, int>.Instance;
            if (obj != null)
            {
                for (var index = 0; index < obj.Count; index++)
                {
                    var item = obj[index];
                    obj[index] = MappingCartGoodsFromDb(goodsRep, item);
                }
            }

            return obj;
        }

        private CartModel MappingCartGoodsFromDb(EfRepository<GoodsEntity, int> goodsRep, CartModel item)
        {
            var goods = goodsRep.FindById(item.GoodsId);
            if (goods == null) return null;
            item.GoodsLogo = goods.Product.MainLogo;
            item.GoodsShortTitle = goods.ShortTitle;

            item.LimitBuyCount = goods.LimitBuyCount == 0 ? 99 : goods.LimitBuyCount;
            item.IsCashDelivery = goods.IsCashDelivery;
            item.IsCashOnline = goods.IsCashOnline;
            //是否超过限购数量
            item.Quantity = item.Quantity > item.LimitBuyCount ? item.LimitBuyCount : item.Quantity;


            //if (item.LimitBuyCount < 99)
            //{
            //    //已购买数量
            //    var hasBuyCount = EfRepository<OrderItemsEntity, int>.Instance.LoadEntities(f => f.Goods.Id == goods.Id
            //         && f.Order.Status != (int)OrderStatusEnum.UnPay
            //         && f.Order.Status != (int)OrderStatusEnum.Expired
            //         && f.Order.Status != (int)OrderStatusEnum.Cancel).Sum(s => (int?)s.Quantity) ?? 0;
            //    //可购买数量
            //    item.CouldBuyCount = (item.LimitBuyCount - hasBuyCount) < 0 ? 0 : (item.LimitBuyCount - hasBuyCount);
            //}
            //else
            //{
            item.CouldBuyCount = item.LimitBuyCount;
            //}

            if (item.Quantity > item.CouldBuyCount)
            {
                item.Quantity = item.CouldBuyCount;

            }

            item.Buyable = true;
            if (!goods.IsOnSelling)
            {
                item.Buyable = false;
                item.UnBuyableDesc = "已下架";
            }
            if (!(goods.IsUseable && goods.Product.IsUseable))
            {
                item.Buyable = false;
                item.UnBuyableDesc = "商品已删除";
            }
            if (goods.SoldCount >= goods.Product.Quantity || (goods.Product.Quantity - goods.SoldCount) <= item.Quantity)
            {
                item.Buyable = false;
                item.UnBuyableDesc = "库存不足";
            }
            if (item.CouldBuyCount == 0)
            {
                item.Buyable = false;
                item.UnBuyableDesc = string.Format("限购{0}件，您已购买{0}件", goods.LimitBuyCount);
            }

            if (!item.Buyable)
            {
                //单价
                item.SinglePrice = 0;

            }
            else
            {
                item.SinglePrice = goods.IsSale && goods.SaleStartTime >= DateTime.Now &&
                                   goods.SaleEndTime <= DateTime.Now
                    ? goods.SalePrice.Value
                    : goods.SellPrice;
            }
            //计算价格
            item.SellPrice = item.SinglePrice * item.Quantity;

            return item;
        }

        ///  <summary>
        /// 获取需要结账的商品
        ///  </summary>
        ///  <param name="paramters">The paramters.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public OperationResult GetCheckOutGoods(string paramters, int userId)
        {
            try
            {

                paramters = Utils.Utils.UrlDecode(paramters);
                if (string.IsNullOrEmpty(paramters))
                {
                    return new OperationResult(OperationResultType.ParamError, "参数不足，没有要结算的商品");
                }
                if (!paramters.Contains(",") && !paramters.Contains("$"))
                {
                    return new OperationResult(OperationResultType.ParamError);
                }
                var paraList = paramters.Split(',').ToList();

                var goodsList = new List<CartModel>();
                var cartData = GetCartData();
                foreach (var para in paraList)
                {
                    if (para.Contains("$") && para.Split('$').Length == 2)
                    {
                        var goodsId = 0;
                        var quantity = 0;
                        var model = new CartModel();
                        var idParseStatus = int.TryParse(para.Split('$')[0], out goodsId);
                        var quantityParseStatus = int.TryParse(para.Split('$')[1], out quantity);
                        if (idParseStatus && quantityParseStatus)
                        {
                            model.GoodsId = goodsId;
                            model.Quantity = quantity;
                            var goodsRep = EfRepository<GoodsEntity, int>.Instance;
                            model = MappingCartGoodsFromDb(goodsRep, model);
                            //更新购物车
                            UpdateCartData(model.GoodsId, model.Quantity, "update", ref cartData);

                            goodsList.Add(model);

                        }

                    }
                    else
                    {
                        return new OperationResult(OperationResultType.ParamError, "未找到购物车商品信息");
                    }
                }
                WirteDataToCookie(cartData);

                //存入系统缓存，以免Redis宕掉，造成无法结算
                Utils.CacheHelper.RemoveAllCache(CacheKeys.USERCART + userId.ToString());
                Utils.CacheHelper.SetCache(CacheKeys.USERCART + userId.ToString(), goodsList);

                return new OperationResult(OperationResultType.Success, "", goodsList);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("GetCheckOutGoods => ", ex);
                return new OperationResult(OperationResultType.ParamError, "未找到购物车商品信息");
            }
        }

    }
}
