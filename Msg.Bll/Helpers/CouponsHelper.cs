using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Msg.Bll.Adapter;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers;
using Msg.Providers.Repository;
using Msg.Providers.UnitOfWork;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;
using Msg.Tools.Extensions;
using Msg.Tools.Logging;
using Msg.Utils;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 优惠券相关业务帮助类
    /// </summary>
    public class CouponsHelper
    {

        private readonly EfRepository<CouponsEntity, Guid> _couponRepository =
            EfRepository<CouponsEntity, Guid>.Instance;

        private readonly EfRepository<CouponCostLogsEntity, int> _costRepository =
            EfRepository<CouponCostLogsEntity, int>.Instance;

        private readonly EfRepository<CouponSendLogsEntity, int> _sendRepository =
            EfRepository<CouponSendLogsEntity, int>.Instance;


        private readonly EfRepository<UsersEntity, int> _userRepository = EfRepository<UsersEntity, int>.Instance;


        private readonly UnitOfWork _unitOfWork = IoC.Current.Resolve<UnitOfWork>();

        #region 单例

        private static CouponsHelper _instance;

        public static CouponsHelper Instance
        {
            get { return _instance ?? (_instance = new CouponsHelper()); }
        }

        #endregion

        #region 注册赠送优惠券

        /// <summary>
        /// 获取注册赠送优惠券活动优惠券
        /// </summary>
        /// <returns></returns>
        private CouponsEntity GetRegistSendCoupon()
        {
            return _couponRepository.LoadEntities(f => f.CouponName.Contains("注册赠送") && f.IsUseable, bNoTracking: false).OrderBy(f => f.CreateTime).FirstOrDefault();
        }

        /// <summary>
        /// 注册赠送优惠券
        /// </summary>
        /// <param name="user">The user.</param>
        public void SendCouponForRegist(UsersEntity user)
        {
            var coupon = GetRegistSendCoupon();
            var sendLog = new CouponSendLogsEntity()
            {
                User = user,
                Coupon = coupon,
                Num = ConfigHelper.GetConfigInt("RegistSendCouponQuantityPerUser")
            };

            sendLog = _sendRepository.AddEntity(sendLog);
        }

        #endregion

        /// <summary>
        /// 批量赠送优惠券
        /// </summary>
        /// <param name="toUserIds">To user ids.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns></returns>
        public OperationResult SendCouponsBatch(string toUserIds, string couponId)
        {
            if (string.IsNullOrEmpty(toUserIds))
            {
                return new OperationResult(OperationResultType.ParamError, "请选择受赠用户");
            }
            if (!couponId.IsGuid())
            {
                return new OperationResult(OperationResultType.ParamError, "优惠券参数有误");
            }

            if (toUserIds.Contains(","))
            {
                var toUIdList = toUserIds.Split(',').ToList();
                foreach (var userId in toUIdList)
                {
                    SendCoupon(int.Parse(userId), couponId, false);
                }
            }
            else
            {
                SendCoupon(int.Parse(toUserIds), couponId, false);
            }
            var effectRows = _unitOfWork.SaveChanges();
            return new OperationResult(effectRows > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// 发送优惠券
        /// </summary>
        /// <param name="toUserId">To user identifier.</param>
        /// <param name="couponId">The coupon identifier.</param>
        /// <returns></returns>
        public OperationResult SendCoupon(int toUserId, string couponId, bool immediatelyCommit = true)
        {
            if (toUserId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "toUserId");
            }
            if (!couponId.IsGuid())
            {
                return new OperationResult(OperationResultType.ParamError, "couponId");
            }
            var user = _userRepository.FindById(toUserId);
            if (user == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "user");
            }
            var coupon = _couponRepository.FindById(Guid.Parse(couponId));
            if (coupon == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "coupon");
            }

            var sendLog = _sendRepository.LoadEntities(f => f.Coupon.Id == coupon.Id && f.User.Id == toUserId, false).SingleOrDefault();

            if (sendLog != null)
            {
                sendLog.Num++;
                _sendRepository.UpdateEntity(sendLog, immediatelyCommit);
            }
            else
            {
                sendLog = new CouponSendLogsEntity()
               {
                   User = user,
                   Coupon = coupon
               };

                sendLog = _sendRepository.AddEntity(sendLog, immediatelyCommit);
            }

            return new OperationResult(sendLog.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// Gets the type of the useable coupons entities by.
        /// </summary>
        /// <param name="couponType">Type of the coupon.</param>
        /// <returns></returns>
        public dynamic GetUseableCouponsEntitiesByType(CouponTypeEnum couponType)
        {
            Expression<Func<CouponsEntity, bool>> filter = f => f.IsUseable && f.Type == couponType;
            var query = _couponRepository.LoadEntities(filter);
            var dynamicQuery = from q in query select new { q.Id, q.CouponName };
            return dynamicQuery.ToList();
        }


        /// <summary>
        /// Gets the coupons entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <param name="couponType">Type of the coupon.</param>
        /// <param name="isUseable">The is useable.</param>
        /// <param name="couponsName"></param>
        /// <returns></returns>
        public IQueryable<CouponsEntity> GetCouponsEntities(int page, int size, out int total, CouponTypeEnum? couponType = null,
            bool? isUseable = null, string couponsName = "")
        {
            Expression<Func<CouponsEntity, bool>> filter = f => f.IsUseable == true;
            Expression<Func<CouponsEntity, DateTime>> sortCondition = s => s.CreateTime;
            if (couponType.HasValue)
            {
                filter = filter.And(f => f.Type == couponType);
            }
            if (isUseable.HasValue)
            {
                filter = filter.And(f => f.IsUseable == isUseable.Value);
            }
            if (couponsName != "" && !string.IsNullOrEmpty(couponsName))
            {
                filter = filter.And(f => f.CouponName.Contains(couponsName));
            }
            return _couponRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                out total);
        }

        /// <summary>
        /// Gets my coupons entities.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <param name="couponType">Type of the coupon.</param>
        /// <returns></returns>
        public IQueryable<CouponSendLogsEntity> GetMyCouponsEntities(int userId, int page, int size, out int total,
            CouponTypeEnum? couponType = null)
        {

            Expression<Func<CouponSendLogsEntity, bool>> filter = f => f.User.Id == userId && f.Coupon.EndTime > DateTime.Now;
            Expression<Func<CouponSendLogsEntity, int>> sortCondition = s => s.Id;
            if (couponType.HasValue)
            {
                filter = f => f.Coupon.Type == couponType;
            }

            var query = _sendRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                out total).ToList();
            return query.Where(f => f.CostLogs.Sum(s => s.CostNum) < f.Num).AsQueryable();

        }

        /// <summary>
        /// 获取我的可用的优惠券
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="sumPrice">The sum price.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public List<CouponSendLogModel> GetMyUnUseCoupons(int userId, decimal sumPrice, int size)
        {
            Expression<Func<CouponSendLogsEntity, bool>> filter = f => f.User.Id == userId
                && (f.CostLogs.Sum(s => (int?)s.CostNum) ?? 0) < f.Num
                && (f.Coupon.Type == CouponTypeEnum.CashDeduction || (f.Coupon.Type == CouponTypeEnum.FullReduceDeduction && f.Coupon.FullPrice <= sumPrice))
                && f.Coupon.StartTime <= DateTime.Now
                && f.Coupon.EndTime > DateTime.Now;

            var query = _sendRepository.LoadEntities(filter);

            var dynamicQuery = from q in query
                               select new
                                   {
                                       q.Id,
                                       q.Coupon.CouponName,
                                       q.Coupon.Logo,
                                       q.Coupon.Type,
                                       q.Coupon.CouponValue,
                                       q.Num,
                                       LeftNum = q.Num - q.CostLogs.Count
                                   };
            var coupons = dynamicQuery.Project().To<CouponSendLogModel>().ToList();
            return coupons;
        }

        /// <summary>
        /// Gets the send logs entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CouponSendLogsEntity GetSendLogsEntity(int id)
        {
            return _sendRepository.FindById(id);
        }


        /// <summary>
        /// Adds the or modify coupons.
        /// </summary>
        /// <param name="couponsEntity">The coupons entity.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public OperationResult AddOrModifyCoupons(CouponsEntity couponsEntity, string type)
        {
            if (couponsEntity.Type.Equals(CouponTypeEnum.FullReduceDeduction))
            {
                couponsEntity.ReducePrice = couponsEntity.CouponValue;
            }
          
            couponsEntity.IsUseable = true;
            try
            {
                switch (type)
                {
                    case "Add":
                        if (VaildPara(couponsEntity).ResultType.Equals(OperationResultType.Success))
                            _couponRepository.AddEntity(couponsEntity);
                        break;
                    case "Modify":
                        if (VaildPara(couponsEntity).ResultType.Equals(OperationResultType.Success))
                            _couponRepository.UpdateEntity(couponsEntity);
                        break;
                    case "Del":
                        couponsEntity = _couponRepository.FindById(couponsEntity.Id);
                        couponsEntity.IsUseable = false;
                        _couponRepository.UpdateEntity(couponsEntity);
                        break;
                }

                return new OperationResult(OperationResultType.Success);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("AddOrModifyCoupons =>", ex);
                return new OperationResult(OperationResultType.NoChanged);
            }


        }

        /// <summary>
        /// Vailds the para.
        /// </summary>
        /// <param name="couponsEntity">The coupons entity.</param>
        /// <returns></returns>
        protected OperationResult VaildPara(CouponsEntity couponsEntity)
        {
            if (!couponsEntity.Type.Equals(CouponTypeEnum.CashDeduction) && !couponsEntity.Type.Equals(CouponTypeEnum.FullReduceDeduction))
            {
                return new OperationResult(OperationResultType.ParamError, "Type");
            }
            if (string.IsNullOrEmpty(couponsEntity.CouponName) || couponsEntity.CouponName.Length > 20)
            {
                return new OperationResult(OperationResultType.ParamError, "CouponName");
            }
            if (couponsEntity.CouponValue < 0)
            {
                return new OperationResult(OperationResultType.ParamError, "面值必须大于0");
            }
            if (couponsEntity.StartTime > couponsEntity.EndTime)
            {
                return new OperationResult(OperationResultType.ParamError, "开始时间必须在结束时间前后");
            }
            return new OperationResult(OperationResultType.Success);
        }



        /// <summary>
        /// 按Id获取优惠券实体
        /// </summary>
        /// <returns></returns>
        public CouponsEntity GetEntity(string id)
        {
            if (!id.IsGuid())
            {
                return null;
            }
            return _couponRepository.FindById(Guid.Parse(id));
        }


    }
}
