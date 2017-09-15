using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
    /// 商品逻辑业务帮助类
    /// </summary>
    public class GoodsHelper
    {
        private readonly EfRepository<GoodsEntity, int> _goodsRepository = EfRepository<GoodsEntity, int>.Instance;

        private readonly EfRepository<ProductsEntity, int> _productRepository =
            EfRepository<ProductsEntity, int>.Instance;


        private readonly EfRepository<GoodsOpreationLogsEntity, int> _opLogsRepository =
            EfRepository<GoodsOpreationLogsEntity, int>.Instance;

        private readonly EfRepository<CreditGoodsEntity, int> _creditGoodsRepository =
            EfRepository<CreditGoodsEntity, int>.Instance;


        #region 单例

        private static GoodsHelper _instance;

        public static GoodsHelper Instance
        {
            get { return _instance ?? (_instance = new GoodsHelper()); }
        }

        #endregion



        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public GoodsEntity GetEntity(int? id)
        {
            return id.HasValue ? _goodsRepository.FindById(id.Value) : new GoodsEntity();
        }


        /// <summary>
        /// Gets the on selling goods entities.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="cateId">The cate identifier.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetSellingGoodsEntities(string name, int? brandId, int? cateId, string sortColumn, int page, int size, out int total)
        {
            if (brandId == 0)
            {
                total = 0;
                return null;
            }

            Expression<Func<GoodsEntity, bool>> filter = f => f.IsOnSelling;
            Expression<Func<GoodsEntity, int>> sortCondition;

            if (!string.IsNullOrEmpty(name))
            {
                filter = f => f.ShortTitle.Contains(name) || f.LongTitle.Contains(name);
            }
            if (brandId.HasValue)
            {
                filter = filter.And(f => f.Product.Brand.Id == brandId);
            }

            if (cateId.HasValue)
            {
                filter = filter.And(f => f.Product.Category.Id == cateId);
            }

            switch (sortColumn)
            {
                case "soldCount":
                    sortCondition = s => s.SoldCount + s.FakeSoldCount;
                    break;
                case "quantity":
                    sortCondition = s => s.Product.Quantity;
                    break;
                default:
                    sortCondition = s => s.Id;
                    break;
            }
            var query = _goodsRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                        out total);

            return query;
        }

        /// <summary>
        /// 手机端首页
        /// </summary>
        /// <param name="brandIds"></param>
        /// <param name="cateIds"></param>
        /// <param name="isMarket">是否是创业</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetMobileIndex(int?[] brandIds, int?[] cateIds, bool? isMarket, int index, int size, out int total)
        {
            Expression<Func<GoodsEntity, bool>> filter = f => f.IsOnSelling && f.IsUseable && f.Product.IsUseable;
            if (brandIds == null && cateIds == null)
            {
                filter = filter.And(f => f.IsForIndex);
            }
            if (brandIds != null)
            {
                filter = filter.And(f => brandIds.Contains(f.Product.Brand.Id));
            }
            if (cateIds != null)
            {
                filter = filter.And(f => cateIds.Contains(f.Product.Category.Id));
            }
            if (isMarket.HasValue && cateIds != null)
            {
                filter = filter.And(f => cateIds.Contains(f.Product.Category.Id) && f.Product.ApplyEntity != null && f.Product.Suppliers != null);
            }
            var query = _goodsRepository.LoadEntities(filter).OrderByDescending(s => s.SoldCount + s.FakeSoldCount);
            total = query.Count();
            return query.Skip((index - 1) * size).Take(size);
        }

        /// <summary>
        /// 获取推荐的商品
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetRecommendGoodsEntities(int size)
        {
            return _goodsRepository.LoadEntities(f => f.IsOnSelling && !f.Product.IsVirtual).OrderByRandom().Take(size);
        }

        /// <summary>
        /// 获取热销的商品
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetHotSellingGoodsEntities(int size, int page)
        {
            var query = _goodsRepository.LoadEntities(f => f.IsOnSelling && f.IsUseable).OrderByDescending(f => f.SoldCount + f.FakeSoldCount).Skip((page - 1) * size).Take(size);
            if (!query.Any())
            {
                query = _goodsRepository.LoadEntities(f => f.IsOnSelling && f.IsUseable).OrderByDescending(f => f.SoldCount + f.FakeSoldCount).Take(size);
            }
            return query;
        }

        /// <summary>
        /// Modify the goods.
        /// </summary>
        /// <param name="goods">The goods.</param>
        /// <returns></returns>
        public OperationResult ModifyGoods(GoodsEntity goods)
        {
            if (goods.Id < 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id");
            }
            if (string.IsNullOrEmpty(goods.ShortTitle) || goods.ShortTitle.Length > 200)
            {
                return new OperationResult(OperationResultType.ParamError, "ShortTitle");
            }
            if (string.IsNullOrEmpty(goods.LongTitle) || goods.LongTitle.Length > 500)
            {
                return new OperationResult(OperationResultType.ParamError, "LongTitle");
            }
            if (goods.SellPrice < 0)
            {
                return new OperationResult(OperationResultType.ParamError, "售卖价格必须大于零");
            }
            if (goods.IsSale && (!goods.SaleStartTime.HasValue || !goods.SaleEndTime.HasValue))
            {
                return new OperationResult(OperationResultType.ParamError, "请选择特卖时间");
            }
            goods.ExpressCost = goods.ExpressCost == 0 ? ConfigModel.DefaultExpressCost : goods.ExpressCost;

            if (goods.Id > 0)//编辑
            {
                var entity = _goodsRepository.FindById(goods.Id);
                entity.ShortTitle = goods.ShortTitle;
                entity.LongTitle = goods.LongTitle;
                entity.SellPrice = goods.SellPrice;
                entity.Desc = goods.Desc;
                entity.IsSale = goods.IsSale;
                entity.ExpressCost = goods.ExpressCost;
                entity.SaleStartTime = goods.SaleStartTime;
                entity.SaleEndTime = goods.SaleEndTime;
                entity.IsOnSelling = goods.IsOnSelling;
                entity.IsCashDelivery = goods.IsCashDelivery;
                entity.IsCashOnline = goods.IsCashOnline;
                entity.SoldCount = goods.SoldCount;
                entity.FakeSoldCount = goods.FakeSoldCount;
                entity.MarketPrice = goods.MarketPrice;
                entity.IsForIndex = goods.IsForIndex;
                entity.LimitBuyCount = goods.LimitBuyCount;
                var status = _goodsRepository.UpdateEntity(entity);

                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);

            }

            return new OperationResult(OperationResultType.ParamError, "goodsId");
        }

        /// <summary>
        /// 切换商品首页特推状态
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <returns></returns>
        public OperationResult ToggleGoodsToIndex(int goodsId)
        {
            var entity = _goodsRepository.FindById(goodsId);
            entity.IsForIndex = !entity.IsForIndex;
            var status = _goodsRepository.UpdateEntity(entity);

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// 修改商品售卖数量
        /// </summary>
        /// <param name="goodsId">The goods identifier.</param>
        /// <param name="soldCount">The sold count.</param>
        /// <param name="immediatelyCommit">是否立即提交</param>
        /// <returns></returns>
        private void ModifyGoodsSoldCount(int goodsId, int soldCount, bool immediatelyCommit)
        {
            var goods = GetEntity(goodsId);
            goods.SoldCount += soldCount;
            var status = _goodsRepository.UpdateEntity(goods, immediatelyCommit);

        }


        /// <summary>
        /// 修改商品售卖数量
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public OperationResult ModifyGoodsSoldCount(ICollection<OrderItemsEntity> items)
        {
            foreach (var item in items)
            {
                ModifyGoodsSoldCount(item.Goods.Id, item.Quantity, false);
            }

            var effectRows = IoC.Current.Resolve<UnitOfWork>().SaveChanges();
            return new OperationResult(effectRows > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
        }


        /// <summary>
        /// 商品上架 (新增)
        /// </summary>
        /// <param name="goods"></param>
        /// <param name="productId">库存Id</param>
        /// <param name="operatorUserId">操作人Id</param>
        /// <param name="operationRemark"></param>
        /// <returns></returns>
        public OperationResult UpGoods(GoodsEntity goods, int productId, int operatorUserId, string operationRemark)
        {
            #region validParams

            if (productId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "productId");
            }
            if (string.IsNullOrEmpty(goods.ShortTitle) || goods.ShortTitle.Length > 200)
            {
                return new OperationResult(OperationResultType.ParamError, "ShortTitle");
            }
            if (string.IsNullOrEmpty(goods.LongTitle) || goods.LongTitle.Length > 500)
            {
                return new OperationResult(OperationResultType.ParamError, "LongTitle");
            }
            if (goods.SellPrice <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "售卖价格不能小于等于0");
            }

            if (goods.IsSale && (!goods.SaleStartTime.HasValue || !goods.SaleEndTime.HasValue))
            {
                return new OperationResult(OperationResultType.ParamError, "SaleStartTime or SaleEndTime");
            }

            #endregion

            try
            {
                var product = _productRepository.FindById(productId);//库存产品

                if (product == null)
                {
                    return new OperationResult(OperationResultType.QueryNull, "product");
                }

                if (!product.IsVirtual)//实物商品则默认邮费,否则实体中默认为0
                {
                    goods.ExpressCost = goods.ExpressCost == 0 ? ConfigModel.DefaultExpressCost : goods.ExpressCost;
                }


                if (string.IsNullOrEmpty(goods.Desc))
                {
                    goods.Desc = product.Desc;
                }

                goods.Product = product;

                goods = _goodsRepository.AddEntity(goods);

                if (goods.Id > 0)
                {
                    //商品操作动作涉及逻辑
                    GoodsOperationAction(goods, operatorUserId, operationRemark);

                    return new OperationResult(OperationResultType.Success, "", goods);
                }
                return new OperationResult(OperationResultType.NoChanged);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException("上架商品", ex);
                return new OperationResult(OperationResultType.Error);
            }
        }

        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="goodsId">已上架商品Id</param>
        /// <param name="operatorUserId">操作人Id</param>
        /// <param name="operationRemark">操作备注</param>
        /// <returns></returns>
        public OperationResult DownGoods(int goodsId, int operatorUserId, string operationRemark)
        {
            if (goodsId <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "goodsId");
            }
            var goods = _goodsRepository.FindById(goodsId);
            if (goods == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "Goods");
            }

            goods.IsOnSelling = false;
            goods.IsUseable = false;
            var status = _goodsRepository.UpdateEntity(goods);

            if (status)
            {
                //商品操作动作涉及逻辑
                GoodsOperationAction(goods, operatorUserId, operationRemark);
            }

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);

        }

        /// <summary>
        /// 获取商品单价
        /// </summary>
        /// <param name="goods">The goods.</param>
        /// <returns></returns>
        public static decimal GetSinglePrice(GoodsEntity goods)
        {
            return goods.IsSale
                && goods.SaleStartTime >= DateTime.Now
                && goods.SaleEndTime <= DateTime.Now
                && goods.SalePrice.HasValue
                ? goods.SalePrice.Value
                : goods.SellPrice;
        }

        /// <summary>
        /// 获取订单商品总价
        /// </summary>
        /// <param name="goods"></param>
        /// <returns></returns>
        public static decimal GetOrderGoodsPrice(IEnumerable<OrderItemsEntity> goods)
        {
            return goods.Sum(item => GetSinglePrice(item.Goods));
        }

        #region 首页数据提供方法

        /// <summary>
        /// Gets the index of the milks of.
        /// </summary>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="takeSize">提取数据数量</param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetMilksOfIndex(string brand, int takeSize)
        {
            var brandId = GetBrandIdFromBrandDescInCache(brand);
            if (brandId == 0)
            {
                return null;
            }
            var query = _goodsRepository.LoadEntities(
                f => f.IsOnSelling
                    && f.IsForIndex
                     && f.IsUseable
                     && f.Product.IsUseable
                     && f.Product.Brand.IsUseable
                     && f.Product.Category.IsUseable
                     && f.Product.ApplyEntity == null
                     && f.Product.Brand.Id == brandId
                ).OrderByDescending(s => s.SoldCount + s.FakeSoldCount).Take(takeSize); ;

            return query;
        }

        public int GetBrandIdFromBrandDescInCache(string brand)
        {
            var brandId = 0;
            var brandCache = BrandsHelper.Instance.GetUesebaleBrandsModelsInCache();
            switch (brand)
            {
                case "yili":
                    var yili = brandCache.FirstOrDefault(f => f.Name.Contains("伊利"));
                    brandId = yili == null ? 0 : yili.Id;
                    break;
                case "mengniu":
                    var mengniu = brandCache.FirstOrDefault(f => f.Name.Contains("蒙牛"));
                    brandId = mengniu == null ? 0 : mengniu.Id;
                    break;
                case "tianyou":
                    var tianyou = brandCache.FirstOrDefault(f => f.Name.Contains("天友"));
                    brandId = tianyou == null ? 0 : tianyou.Id;
                    break;
                case "dream":
                    var dream = brandCache.FirstOrDefault(f => f.Name.Contains("奶牛梦工厂"));
                    brandId = dream == null ? 0 : dream.Id;
                    break;
                default:
                    brandId = 0;
                    break;
            }
            return brandId;
        }


        /// <summary>
        /// 按类目获取首页数据
        /// </summary>
        /// <param name="cateId">The cate identifier.</param>
        /// <param name="takeSize">提取数据数量</param>
        /// <returns></returns>
        public IQueryable<GoodsEntity> GetIndexDataByCateId(int cateId, int takeSize)
        {
            var query = _goodsRepository.LoadEntities(
                f => f.IsOnSelling
                     && f.IsForIndex
                     && f.IsUseable
                     && f.Product.IsUseable
                     && f.Product.Brand.IsUseable
                     && f.Product.Category.IsUseable
                     && f.Product.ApplyEntity == null
                     && f.Product.Category.Id == cateId
                ).OrderByDescending(s => s.SoldCount + s.FakeSoldCount).Take(takeSize);

            return query;
        }

        /// <summary>
        /// 获取首页创业格子数据
        /// </summary>
        /// <param name="takeSize">提取数据数量</param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetEsGridGoodsEntities(int takeSize)
        {
            var query = _goodsRepository.LoadEntities(
              f => f.IsOnSelling
                   && f.IsUseable
                   && f.Product.IsUseable
                   && f.Product.Brand.IsUseable
                   && f.Product.Category.IsUseable
                   && f.Product.ApplyEntity != null
                   && f.Product.Suppliers != null
              ).OrderByDescending(s => s.SoldCount + s.FakeSoldCount).Take(takeSize);

            var dynamicQuery = from q in query.ToList()
                               select new
                                   {
                                       q.Id,
                                       Logo = Utils.ImagesHelper.GetImgWidthHeight(q.Product.MainLogo, 230, 230, false, 80),
                                       SellPrice = q.SellPrice.ToString("F"),
                                       SoldCount = q.SoldCount + q.FakeSoldCount,
                                       q.ShortTitle,
                                       SchoolName = q.Product.Suppliers.User.School.Name,
                                       UserName = q.Product.Suppliers.User.NickName
                                   };

            return dynamicQuery;
        }

        /// <summary>
        /// 获取首页积分兑换商品
        /// </summary>
        /// <param name="creditBegin">积分筛选起始值</param>
        /// <param name="creditEnd">积分筛选截止值</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetUseAbleCreditGoodsEntities(int creditBegin, int creditEnd, int size)
        {
            var query = _creditGoodsRepository.LoadEntities(
                f => f.NeedCredits >= creditBegin
                    && f.IsUseable
                     && f.NeedCredits <= creditEnd
                     && f.StartTime <= DateTime.Now
                     && f.EndTime > DateTime.Now).OrderByRandom().Take(size);

            var dynamicQuery = from q in query.ToList()
                               select new
                                   {
                                       q.Id,
                                       Logo = ImagesHelper.GetImgWidthHeight(q.Logo, 190, 135, false, 80),
                                       q.Name,
                                       q.NeedCredits
                                   };

            return dynamicQuery;
        }

        #endregion

        #region 积分商品

        /// <summary>
        /// Gets the credit goods.
        /// </summary>
        /// <param name="creditBegin">积分筛选起始值</param>
        /// <param name="creditEnd">积分筛选截止值</param>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public List<CreditGoodsLiteModel> GetCreditGoods(int creditBegin, int creditEnd, int page, int size, out int total)
        {
            Expression<Func<CreditGoodsEntity, bool>> filter = f => f.NeedCredits >= creditBegin
                                                                    && f.IsUseable
                                                                    && f.NeedCredits <= creditEnd
                                                                    && f.StartTime <= DateTime.Now
                                                                    && f.EndTime > DateTime.Now;


            var query = _creditGoodsRepository.LoadEntitiesByPaging(page, size, filter, s => s.Id, OrderingOrders.DESC,
                out total);


            return query.Project().To<CreditGoodsLiteModel>().ToList();
        }

        /// <summary>
        /// Gets the credit goods.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CreditGoodsEntity GetCreditGoods(int id)
        {
            return _creditGoodsRepository.LoadEntities(f => f.Id == id && f.IsUseable).SingleOrDefault();
        }

        #endregion

        #region 创业集市（创业格子）

        /// <summary>
        /// 获取首页创业格子数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<MarketGoodsModel> GetEsGridGoodsEntities(CategoriesEnum cate, int page, int size, out int total)
        {
            var query = _goodsRepository.LoadEntities(
              f => f.IsOnSelling
                   && f.IsUseable
                   && f.Product.IsUseable
                   && f.Product.Brand.IsUseable
                   && f.Product.Category.IsUseable
                   && f.Product.ApplyEntity != null
                   && f.Product.Suppliers != null
                   && f.Product.Category.Id == (int)cate
              ).OrderByDescending(s => s.SoldCount + s.FakeSoldCount).Skip((page - 1) * size).Take(size);

            total = query.Count();

            var dynamicQuery = from q in query
                               select new
                               {
                                   q.Id,
                                   Logo = q.Product.Logo,
                                   SellPrice = q.SellPrice,
                                   SoldCount = q.SoldCount + q.FakeSoldCount,
                                   q.ShortTitle,
                                   SchoolName = q.Product.Suppliers.User.School.Name,
                                   UserName = q.Product.Suppliers.User.NickName,
                                   q.IsOnSelling
                               };

            return dynamicQuery.Project().To<MarketGoodsModel>().ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 商品操作动作
        /// </summary>
        /// <param name="goods">The goods.</param>
        /// <param name="operatorId">The operator identifier.</param>
        /// <param name="operationRemark">The operation remark.</param>
        private void GoodsOperationAction(GoodsEntity goods, int operatorId, string operationRemark)
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var supplier = EfRepository<SuppliersEntity, int>.Instance.LoadEntities(f => f.User.Id == operatorId).SingleOrDefault();
                    if (supplier != null)
                    {
                        //是创业者需更改库存申请表状态

                        var apply = goods.Product.ApplyEntity;
                        if (apply != null)
                        {
                            var applys = EfRepository<ProductsApplyEntity, int>.Instance.FindById(apply.Id);

                            if (applys != null)
                            {
                                applys.ApplyStatus = goods.IsOnSelling ? (byte)ApplyStatusEnum.GoodsUp : (byte)ApplyStatusEnum.GoodsDown;
                                applys.OperateRemark = operationRemark;
                            }

                            EfRepository<ProductsApplyEntity, int>.Instance.UpdateEntity(applys);
                        }


                    }
                    if (supplier == null)//非创业者只写入操作日志
                    {
                        var logs = new GoodsOpreationLogsEntity()
                        {
                            GoodsId = goods.Id,
                            OperationContent = goods.IsOnSelling ? OperationDesc.Up : OperationDesc.Down,
                            OperatorUserId = operatorId
                        };

                        _opLogsRepository.AddEntity(logs);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteException("GoodsOperationAction", ex);

                }

            });


        }

        #endregion

        #region 浏览记录

        public void SaveGoodsViewHistory(int goodsId)
        {
            var cookie = Utils.Utils.GetCookie("GoodsViewHistory");
            if (string.IsNullOrEmpty(cookie))
            {
                cookie += goodsId.ToString();
            }
            else if (cookie.Contains("|"))
            {

                var idsList = cookie.Split('|').ToList();

                if (idsList.All(f => f != goodsId.ToString()))
                {
                    idsList.Add(goodsId.ToString());
                }

                cookie = idsList.Aggregate((a, n) => a + "|" + n);

                //cookie = cookie.Substring(0, cookie.Length - 1);

            }
            else if (cookie != goodsId.ToString())
            {
                cookie += "|" + goodsId.ToString();
            }
            Utils.Utils.WriteCookie("GoodsViewHistory", cookie, 43200);

        }

        public IQueryable<GoodsEntity> GetGoodsViewHistory(int size)
        {
            var cookie = Utils.Utils.GetCookie("GoodsViewHistory");
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            else
            {
                if (cookie.Contains("|"))
                {
                    try
                    {
                        var goodsList = cookie.Split('|').ToList().Select(int.Parse).ToList();
                        if (goodsList.Count > size)
                        {
                            goodsList.Reverse();
                            goodsList = goodsList.Take(goodsList.Count - size).ToList();
                        }
                        return _goodsRepository.LoadEntities(f => goodsList.Any(id => id == f.Id) && f.IsOnSelling && f.IsUseable);

                    }
                    catch
                    {
                        return null;
                    }

                }
                else
                {
                    var goodsId = 0;
                    if (int.TryParse(cookie, out goodsId))
                    {
                        return _goodsRepository.LoadEntities(f => f.Id == goodsId && f.IsOnSelling && f.IsUseable);
                    }
                    return null;
                }
            }
        }


        #endregion
    }
}
