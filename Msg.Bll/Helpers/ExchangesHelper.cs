using System;
using System.Linq;
using System.Linq.Expressions;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 积分兑换逻辑帮助类
    /// </summary>
    public class ExchangesHelper
    {
        private readonly EfRepository<CreditGoodsEntity, int> _cgoodsRepository =
            EfRepository<CreditGoodsEntity, int>.Instance;

        private readonly EfRepository<CreditsExchangeLogsEntity, int> _exchangesRepository =
            EfRepository<CreditsExchangeLogsEntity, int>.Instance;

        /// <summary>
        /// Gets the exchange logs entities.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<CreditsExchangeLogsEntity> GetExchangeLogsEntities(int userId, int page, int size, out int total)
        {
            return _exchangesRepository.LoadEntitiesByPaging(page, size, f => f.User.Id == userId, s => s.Id,
                OrderingOrders.DESC, out total);
        }


        /// <summary>
        /// 兑换商品
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="goodsId">积分兑换商品Id</param>
        /// <param name="quantiy">The quantiy.</param>
        /// <param name="addressId">收货地址ID</param>
        /// <returns></returns>
        public OperationResult ExchangeGoods(int userId, int goodsId, int quantiy, int addressId)
        {
            if (quantiy <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "兑换数量错误");
            }
            if (addressId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "收货地址Id错误");
            }

            var user = UsersHelper.Instance.GetUser(userId);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "用户不存在");
            }
            if (!user.IsActive || !user.IsUseable)
            {
                return new OperationResult(OperationResultType.PurviewLack, "用户未激活或不可用");
            }
            var goods = _cgoodsRepository.FindById(goodsId);
            if (goods == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "不存在该商品");
            }
            if (!goods.IsUseable)
            {
                return new OperationResult(OperationResultType.QueryNull, "该商品暂不可兑换");
            }
            if (quantiy > goods.ExchangeTimes)
            {
                return new OperationResult(OperationResultType.NoChanged, "您已超过最大兑换次数，如需继续兑换，请修改兑换件数");
            }
            if (quantiy > goods.Quantity)
            {
                return new OperationResult(OperationResultType.NoChanged, "库存数量不足，如需继续兑换，请修改兑换件数");
            }

            var exchangeLogCount = goods.ExchangeLogs.Where(f => f.User.Id == user.Id).Sum(s => s.Quantity);
            if (exchangeLogCount >= goods.ExchangeTimes)
            {
                return new OperationResult(OperationResultType.Warning, "您的兑换次数已经达到最多件数，不能再次兑换");
            }

            if (quantiy + exchangeLogCount >= goods.ExchangeTimes)
            {
                return new OperationResult(OperationResultType.Warning, string.Format("您至多还能兑换{0}件该商品; 如需继续兑换，请修改兑换件数", goods.ExchangeTimes - exchangeLogCount));
            }

            var sumExchangeCount = goods.ExchangeLogs.Sum(f => f.Quantity);


            if (goods.Quantity <= sumExchangeCount)
            {
                return new OperationResult(OperationResultType.NoChanged, "库存数量不足，如需继续兑换，请修改兑换件数");
            }
            if (quantiy + sumExchangeCount >= goods.Quantity)
            {
                var canExchangeCount = 0;
                if (goods.Quantity > (goods.ExchangeTimes - exchangeLogCount))
                {
                    canExchangeCount = goods.Quantity - (goods.ExchangeTimes - exchangeLogCount);
                }
          
                return new OperationResult(OperationResultType.NoChanged, string.Format("库存数量不足,您至多能兑换{0}件，不能兑换", canExchangeCount));
            }
            if (user.Credits < goods.NeedCredits)
            {
                return new OperationResult(OperationResultType.Warning, string.Format("您的积分不足，还差{0}个积分", goods.NeedCredits - user.Credits));
            }

            var address = UsersHelper.Instance.GetUserAddressEntity(addressId);
            if (address == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "收货地址不存在");
            }
            if (address.User.Id != userId)
            {
                return new OperationResult(OperationResultType.IllegalOperation, "收货地址对应用户不正确");
            }
            var log = new CreditsExchangeLogsEntity()
            {
                Quantity = quantiy,
                CreditsCost = quantiy * goods.NeedCredits,
                CreateTime = DateTime.Now,
                IsSended = false,
                User = user,
                CreditGoods = goods,
                Address = address
            };

            log = _exchangesRepository.AddEntity(log);
            return new OperationResult(log.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged, "", log);
        }



        public OperationResult SendCreditsExchange(int id)
        {
            var result = new OperationResult(OperationResultType.NoChanged);
            var model =  _exchangesRepository.FindById(id);
            model.IsSended = true;
            result.ResultType = _exchangesRepository.UpdateEntity(model) ? OperationResultType.Success : result.ResultType;
            return result;

        }

        public IQueryable<CreditsExchangeLogsEntity> GetAllExchangeLogsEntities( out int total, int? quantity, bool? isSended, string nickName ="", string mobile="", string goodName ="",int page = 1, int size =10)
        {
            Expression<Func<CreditsExchangeLogsEntity, bool>> filter = f => f.Id > 0;
            Expression<Func<CreditsExchangeLogsEntity, DateTime>> orderBy = f => f.CreateTime;
            if (isSended.HasValue)
            {
                filter = filter.And(f => f.IsSended == isSended);
            }
            if (quantity.HasValue)
            {
                filter = filter.And(f => f.Quantity == quantity);
            }
            if (goodName != "" && !string.IsNullOrEmpty(goodName))
            {
                filter = filter.And(f => f.CreditGoods.Name.Contains(goodName));
            }
            if (nickName != "" && !string.IsNullOrEmpty(nickName))
            {
                filter = filter.And(f => f.User.UserName.Contains(nickName));
            }
            if (mobile !="" && !string.IsNullOrEmpty(mobile))
            {
                filter = filter.And(f => f.User.Mobile.Contains(mobile));
            }

            return  _exchangesRepository.LoadEntitiesByPaging(page, 10, filter, orderBy, OrderingOrders.DESC, out total);

    }
}
}
