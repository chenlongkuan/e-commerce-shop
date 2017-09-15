using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Microsoft.Practices.Unity;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers;
using Msg.Providers.Repository;
using Msg.Providers.UnitOfWork;
using Msg.Redis;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;
using Msg.Tools.Extensions;
using Msg.Tools.Logging;
using Msg.Utils;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 订单帮助类
    /// </summary>
    public class OrdersHelper
    {
        private readonly UnitOfWork _uWork = IoC.Current.Resolve<UnitOfWork>();

        private readonly EfRepository<OrdersEntity, Guid> _orderRepository = EfRepository<OrdersEntity, Guid>.Instance;
        private readonly EfRepository<OrderItemsEntity, int> _itemRepository = EfRepository<OrderItemsEntity, int>.Instance;
        private readonly EfRepository<UserTradeLogsEntity, int> _tradeRepository = EfRepository<UserTradeLogsEntity, int>.Instance;

        /// <summary>
        /// 根据用户编号查询该用户下面的订单个数
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetOrderCountByUid(int userId)
        {
            Expression<Func<OrdersEntity, bool>> filter = f => f.User.Id == userId;

            return _orderRepository.LoadEntities(filter).Count();
        }

        /// <summary>
        ///  获取所有的订单
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <param name="brandsId"></param>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="orderBy"></param>
        /// <param name="nickName"></param>
        /// <param name="goodName"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public IQueryable<OrdersEntity> GetAllOrderListByPage(int? state, DateTime? dateBegin, DateTime? dateEnd, int? brandsId, out int total, int page = 1, int pagesize = 10, string orderBy = "", string nickName = "", string goodName = "", string mobile = "")
        {
            Expression<Func<OrdersEntity, bool>> filter;
            if (state.HasValue && state != 0)
            {
                filter = f => f.Status == state;
            }
            else
            {
                filter = f => f.Status != 0;
            }

            Expression<Func<OrdersEntity, DateTime>> orderby = o => o.CreateTime;

            //筛选条件
            if (brandsId.HasValue)
            {
                //Expression<Func<OrderItemsEntity, bool>> filterbrandsId = s => s.Goods.Product.Brand.Id == brandsId;
                filter =
                    filter.And(f => f.Items.Any(s => s.Goods.Product.Brand.Id == brandsId));
                // filter.And(f => f.Items.)
            }
            if (dateBegin.HasValue)
            {
                filter = filter.And(f => f.ConfirmTime <= dateBegin);
            }
            if (dateEnd.HasValue)
            {
                DateTime dt = Convert.ToDateTime(dateEnd.ToString());
                dateEnd = dt.AddDays(1);
                filter = filter.And(f => f.ConfirmTime <= dateEnd);
            }
            if (!string.IsNullOrEmpty(nickName) && nickName != "")
            {
                filter = filter.And(f => f.User.UserName.Contains(nickName));
            }
            if (!string.IsNullOrEmpty(goodName) && goodName != "")
            {
                filter = filter.And(f => f.Items.Any(s => s.Goods.ShortTitle.Contains(goodName)));
            }
            if (!string.IsNullOrEmpty(mobile) && mobile != "")
            {
                filter = filter.And(f => f.User.Mobile.Contains(mobile));
            }
            if (state != 0)
            {
                filter = filter.And(f => f.Status == state);
            }

            var query =
            _orderRepository.LoadEntitiesByPaging(page, pagesize, filter, orderby, OrderingOrders.DESC, out  total);
            return query;
        }


        //修改订单状态
        public OperationResult SendedGoods(string id, int lastModifyUser)
        {
            var orderEntity = _orderRepository.FindById(Guid.Parse(id));
            if (orderEntity == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "orderEntity");
            }
            orderEntity.Status = (int)OrderStatusEnum.Sended;
            orderEntity.ConfirmTime = DateTime.Now;
            orderEntity.LastModifyUser = lastModifyUser;
            orderEntity.LastModifyTime = DateTime.Now;
            var status = _orderRepository.UpdateEntity(orderEntity);
            if (status && orderEntity.PayWay == PayWayEnum.CashDelivery.ToDescription())//修改库存数量，仅供后台对货到付款商品的已发货操作
            {
                GoodsHelper.Instance.ModifyGoodsSoldCount(orderEntity.Items);
            }
            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }


        /// <summary>
        /// Gets the orders entity by order no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns></returns>
        public OrdersEntity GetOrdersEntityByOrderNo(string orderNo)
        {
            return _orderRepository.LoadEntities(f => f.OrderNo == orderNo, false).SingleOrDefault();
        }

        /// <summary>
        /// Gets the orders entity.
        /// </summary>
        /// <param name="no">The no.</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public OperationResult GetMyOrderEntity(string no, int userId)
        {
            if (string.IsNullOrEmpty(no))
            {
                return new OperationResult(OperationResultType.ParamError, "订单编号");
            }
            var query = _orderRepository.LoadEntities(f => f.OrderNo == no).SingleOrDefault();
            if (query == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "订单不存在");
            }
            if (query.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack, "你无权查看此订单");
            }

            return new OperationResult(OperationResultType.Success, "", query);
        }

        /// <summary>
        /// 获取自上次登录时间的新订单数量
        /// </summary>
        /// <param name="loginTime">The login time.</param>
        /// <returns></returns>
        public int GetNewOrderCountForAdminsSinceLastLoginTime(DateTime loginTime)
        {
            return _orderRepository.LoadEntities(f => f.CreateTime >= loginTime).Count();

        }

        /// <summary>
        /// 获取某用户某种状态的订单
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderStatus">订单状态</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<OrdersEntity> GetUserOrdersByStatus(int userId, OrderStatusEnum? orderStatus, int page, int size, out int total)
        {
            Expression<Func<OrdersEntity, bool>> filter = f => f.User.Id == userId;
            if (orderStatus.HasValue)
            {
                filter = filter.And(f => f.Status == (int)orderStatus);
            }

            return _orderRepository.LoadEntitiesByPaging(page, size, filter, s => s.CreateTime, OrderingOrders.DESC, out total);
        }


        /// <summary>
        ///计算最终金额（检查是否可以包邮）
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        public static decimal CalculateFinalPrice(decimal price)
        {
            var freeExpressCost = ConfigHelper.GetConfigDecimal("HowmuchToFreeExpressCost");
            var defaultExpressCost = ConfigHelper.GetConfigDecimal("DefaultExpressCost");

            var finalPrice = price > freeExpressCost ? price : price + defaultExpressCost;

            return finalPrice;
        }

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="orderStatus">The order status.</param>
        /// <param name="isAdminOperate">是否后台操作</param>
        /// <returns></returns>
        public OperationResult ModifyOrderStatus(string id, OrderStatusEnum orderStatus, int userId, bool isAdminOperate = false)
        {
            if (!id.IsGuid())
            {
                return new OperationResult(OperationResultType.ParamError, "订单编号");
            }
            var order = _orderRepository.FindById(Guid.Parse(id));
            if (order == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "订单不存在");
            }
            if (order.User.Id != userId && !isAdminOperate)
            {
                return new OperationResult(OperationResultType.PurviewLack, "你无权操作此订单");
            }
            if (orderStatus == OrderStatusEnum.Cancel)
            {
                if (order.Status != (int)OrderStatusEnum.UnPay)
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "订单已经进入配货流程，不能取消");
                }
            }
            if (orderStatus == OrderStatusEnum.Outputing || (order.PayWay.Equals(PayWayEnum.CashDelivery.ToDescription()) && orderStatus == OrderStatusEnum.Done))
            {
                order.PayTime = DateTime.Now;
                //更新用户积分
                UsersHelper.Instance.ModifyUserCredit(order.DisplayPrice - order.ExpressCost, order.User.Id);
            }

            order.LastModifyUser = userId;
            order.LastModifyTime = DateTime.Now;
            order.Status = (int)orderStatus;
            var status = _orderRepository.UpdateEntity(order);
            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged, "", order);
        }


        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="addressId">用户选择的收货地址Id.</param>
        /// <param name="payway">支付方式</param>
        /// <param name="expressway">配送方式</param>
        /// <param name="sendDate">送过时间段日期</param>
        /// <param name="sendTimeBuckets">配送时间段</param>
        /// <param name="couponId">优惠券Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public OperationResult CreateOrder(int addressId, string payway, string expressway, DateTime sendDate, string sendTimeBuckets,
            int couponId, int userId)
        {
            #region 参数验证

            if (addressId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "收货地址参数错误");
            }
            if (string.IsNullOrEmpty(payway) || (!payway.Equals(PayWayEnum.CashOnline.ToString()) && !payway.Equals(PayWayEnum.CashDelivery.ToString())))
            {
                return new OperationResult(OperationResultType.ParamError, "支付方式参数错误");
            }
            else
            {
                payway = payway.Equals(PayWayEnum.CashOnline.ToString())
                    ? PayWayEnum.CashOnline.ToDescription()
                    : PayWayEnum.CashDelivery.ToDescription();
            }
            if (string.IsNullOrEmpty(expressway) || (!expressway.Equals(ExpressWayEnum.MsgExpress.ToString()) && !expressway.Equals(ExpressWayEnum.SelfGet.ToString())))
            {
                return new OperationResult(OperationResultType.ParamError, "配送方式参数错误");
            }
            else
            {
                expressway = expressway.Equals(ExpressWayEnum.MsgExpress.ToString())
                    ? ExpressWayEnum.MsgExpress.ToDescription()
                    : ExpressWayEnum.SelfGet.ToDescription();
            }
            if (string.IsNullOrEmpty(sendTimeBuckets) || (!sendTimeBuckets.Equals(SendTimeBucketsEnum.Noon.ToString()) && !sendTimeBuckets.Equals(SendTimeBucketsEnum.AfterNoon.ToString())))
            {
                sendTimeBuckets = expressway.Equals(ExpressWayEnum.SelfGet.ToDescription()) ? "" : SendTimeBucketsEnum.AfterNoon.ToDescription();
            }
            else
            {
                sendTimeBuckets = sendTimeBuckets.Equals(SendTimeBucketsEnum.AfterNoon.ToString())
                    ? SendTimeBucketsEnum.AfterNoon.ToDescription()
                    : SendTimeBucketsEnum.Noon.ToDescription();
            }
            if (couponId < 0)
            {
                couponId = 0;//若优惠券参数错误，默认未使用优惠券
            }

            var address = UsersHelper.Instance.GetUserAddressEntity(addressId);
            if (address == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "查无此收获到地址，请重新选择收货地址再提交订单");
            }
            if (!address.IsUseable || address.User.Id != userId)
            {
                return new OperationResult(OperationResultType.PurviewLack, "收货地址不可用");
            }

            var order = new OrdersEntity();
            CouponSendLogsEntity coupon = null;
            if (couponId > 0)
            {
                coupon = CouponsHelper.Instance.GetSendLogsEntity(couponId);
                if (coupon == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "查无此优惠券，请重新选择优惠券再提交订单");
                }
                if (coupon.User.Id != userId)
                {
                    return new OperationResult(OperationResultType.PurviewLack, "您的账户里没有所选的优惠券");
                }
                if (coupon.CostLogs.Count >= coupon.Num)
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "您选择的优惠券已经使用完了");
                }
                if (coupon.Coupon.StartTime > DateTime.Now)
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "您选择的优惠券还未到生效时间");
                }
                if (coupon.Coupon.EndTime <= DateTime.Now)
                {
                    return new OperationResult(OperationResultType.IllegalOperation, "您选择的优惠券已经过期");
                }
                order.SalePrice = coupon.Coupon.CouponValue;

            }

            #endregion



            //获取用户要购买的商品
            var cartGoods = (List<CartModel>)Utils.CacheHelper.GetCache(Redis.CacheKeys.USERCART + userId);
            if (cartGoods == null || !cartGoods.Any())
            {
                return new OperationResult(OperationResultType.Error, "获取需要结算的商品失败");
            }
            foreach (var item in cartGoods)
            {
                var goods = GoodsHelper.Instance.GetEntity(item.GoodsId);
                if (!goods.IsOnSelling)
                {
                    return new OperationResult(OperationResultType.Error, "商品【" + goods.ShortTitle + "】已下架");

                }
                if (!(goods.IsUseable || goods.Product.IsUseable))
                {
                    return new OperationResult(OperationResultType.Error, "商品【" + goods.ShortTitle + "】已删除");
                }
                if (goods.SoldCount >= goods.Product.Quantity)
                {
                    return new OperationResult(OperationResultType.Error, "商品【" + goods.ShortTitle + "】库存不足");
                }
            }
            var user = UsersHelper.Instance.GetUser(userId);

            order.OrderPrice = cartGoods.Sum(s => s.SellPrice);
            //判断订单金额是否满足包邮条件
            var expressCost = (order.OrderPrice - order.SalePrice) < ConfigModel.HowmuchToFreeExpressCost
                ? ConfigModel.DefaultExpressCost
                : 0;
            //判断配送方式决定邮费
            order.ExpressCost = expressway.Equals(ExpressWayEnum.SelfGet.ToString()) ? 0 : expressCost;
            order.Status = (int)OrderStatusEnum.UnPay;
            order.PayWay = payway;
            order.ExpressWay = expressway;
            order.LastModifyUser = user.Id;
            order.LastModifyTime = DateTime.Now;
            order.SendTimeDate = sendDate;
            order.SendTimeBuckets = sendTimeBuckets;
            order.User = user;
            order.Address = address;

            order = _orderRepository.AddEntity(order);

            if (couponId > 0)//使用优惠券时
            {
                //写入优惠券使用记录
                var costLog = new CouponCostLogsEntity();
                costLog.CostNum = 1;
                costLog.CostTime = DateTime.Now;
                costLog.Order = order;
                costLog.SendLog = coupon;
                costLog.User = user;
                EfRepository<CouponCostLogsEntity, int>.Instance.AddEntity(costLog);
            }

            foreach (var goods in cartGoods)
            {
                //写入OrderItem
                var items = new OrderItemsEntity();
                items.Quantity = goods.Quantity;
                items.SinglePrice = goods.SinglePrice;
                items.Goods = GoodsHelper.Instance.GetEntity(goods.GoodsId);
                items.Order = order;
                _itemRepository.AddEntity(items, immediatelyCommit: false);
            }

            var rowEffect = _uWork.SaveChanges();
            if (rowEffect > 0)
            {
                var goodsIds = cartGoods.Aggregate("", (current, good) => current + (good.GoodsId + ","));
                goodsIds = goodsIds.Substring(0, goodsIds.Length - 1);

                CartHelper.Instance.RemoveFromCart(goodsIds);

                //删除临时缓存
                Utils.CacheHelper.RemoveAllCache(CacheKeys.USERCART + userId.ToString());

                return new OperationResult(OperationResultType.Success, "下单成功", order);
            }
            return new OperationResult(OperationResultType.Error, "生成订单异常");

        }


        /// <summary>
        /// Adds the trade log.
        /// </summary>
        /// <param name="tradeLogs">The trade logs.</param>
        /// <returns></returns>
        public OperationResult AddTradeLog(UserTradeLogsEntity tradeLogs)
        {
            var result = _tradeRepository.AddEntity(tradeLogs);
            return new OperationResult(result.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
        }



        public OperationResult OutToExcel(int? state, DateTime? dateBegin, DateTime? dateEnd, int? brandsId,
            string nickName = "", string goodName = "", string mobile = "")
        {

            Expression<Func<OrdersEntity, bool>> filter;
            if (state.HasValue && state != 0)
            {
                filter = f => f.Status == state;
            }
            else
            {
                filter = f => f.Status != 0;
            }

            Expression<Func<OrdersEntity, DateTime?>> orderby = o => o.ConfirmTime;

            //筛选条件
            if (brandsId.HasValue)
            {
                //Expression<Func<OrderItemsEntity, bool>> filterbrandsId = s => s.Goods.Product.Brand.Id == brandsId;
                filter =
                    filter.And(f => f.Items.Any(s => s.Goods.Product.Brand.Id == brandsId));
                // filter.And(f => f.Items.)
            }
            if (dateBegin.HasValue)
            {
                filter = filter.And(f => f.ConfirmTime <= dateBegin);
            }
            if (dateEnd.HasValue)
            {
                DateTime dt = Convert.ToDateTime(dateEnd.ToString());
                dateEnd = dt.AddDays(1);
                filter = filter.And(f => f.ConfirmTime <= dateEnd);
            }
            if (!string.IsNullOrEmpty(nickName) && nickName != "")
            {
                filter = filter.And(f => f.User.UserName.Contains(nickName));
            }
            if (!string.IsNullOrEmpty(goodName) && goodName != "")
            {
                filter = filter.And(f => f.Items.Any(s => s.Goods.ShortTitle.Contains(goodName)));
            }
            if (!string.IsNullOrEmpty(mobile) && mobile != "")
            {
                filter = filter.And(f => f.User.Mobile.Contains(mobile));
            }
            if (state != 0)
            {
                filter = filter.And(f => f.Status == state);
            }

            var query = _orderRepository.LoadEntities(filter).OrderByDescending(s => s.Id);

            var dynamicQuery = from q in query.ToList()
                               select new
                                   {
                                       订单号 = q.OrderNo
                                       ,
                                       收货人 = q.Address.ReciverName
                                       ,
                                       下单时间 = q.CreateTime
                                       ,
                                       联系电话 = q.Address.ReciverTel
                                       ,
                                       总价 = q.OrderPrice
                                       ,
                                       总数量 = q.Items.Sum(s => s.Quantity),
                                       商品 = GetGoodsToExcel(q.Items)
                                       ,
                                       学校 = q.Address.SchoolName
                                       ,
                                       支付途径 = q.PayWay
                                       ,
                                       支付时间 = q.PayTime.HasValue ? q.PayTime.Value.ToString() : "--"
                                       ,
                                       订单状态 = q.StatusDesc
                                       ,
                                       用户选择送货时间 = q.SendTimeBuckets
                                       ,
                                       地址 = q.Address.DetailAddress
                                       ,
                                       配送方式 = q.ExpressWay
                                   };
            try
            {
                ExcelHelper.DataTable1Excel(dynamicQuery.ConvertToDataTable());
                return new OperationResult(OperationResultType.Success, "导出成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("OutToExcel", ex);
                return new OperationResult(OperationResultType.Error, "导出失败");
            }


        }

        private string GetGoodsToExcel(ICollection<OrderItemsEntity> items)
        {
            return items.Aggregate("", (current, goods) => current + (goods.Goods.ShortTitle + "  X " + goods.Quantity + "<br>"));
        }


    }
}
