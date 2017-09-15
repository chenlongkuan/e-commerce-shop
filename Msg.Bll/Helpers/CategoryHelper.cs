using System.Collections.Generic;
using System.Linq;
using Msg.Bll.Adapter;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Redis;
using Msg.Tools;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 类目业务逻辑帮助类
    /// </summary>
    public class CategoryHelper
    {
        private readonly EfRepository<CategoriesEntity, int> _cateRepository =
            EfRepository<CategoriesEntity, int>.Instance;

        private static CategoryHelper _instance;

        /// <summary>
        /// The instance
        /// </summary>
        public static CategoryHelper Instance
        {
            get { return _instance ?? (_instance = new CategoryHelper()); }
        }



        /// <summary>
        /// Gets the useable categories in cache.
        /// </summary>
        /// <returns></returns>
        public List<CategoryModel> GetUseableCategoriesInCache()
        {
            var cacheObj = CacheHelper.Get<List<CategoryModel>>(CacheKeys.ALL_CATEGORIES);
            if (cacheObj != null) return cacheObj;

            cacheObj = _cateRepository.LoadEntities(f => f.IsUseable).Project().To<CategoryModel>().ToList();

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.ALL_CATEGORIES, cacheObj, CacheTimeOut.Normal);
            }
            return cacheObj;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CategoriesEntity GetEntity(int id)
        {
            return _cateRepository.FindById(id);
        }

        /// <summary>
        /// Gets the entity for market by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public CategoriesEntity GetEntityForMarketById(int id)
        {
            return _cateRepository.LoadEntities(f => f.Id == id && f.IsUseable && f.IsForMarket, false).FirstOrDefault();
        }


        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public OperationResult AddCateGory(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length > 10)
            {
                return new OperationResult(OperationResultType.ParamError, "name");
            }

            var cate = _cateRepository.AddEntity(new CategoriesEntity() { Name = name });

            if (cate.Id > 0)
            {
                //更新缓存
                var cacheObj = cate.ProjectedAs<CategoryModel, int>();
                CacheUtils.AppendToCollectionCache<List<CategoryModel>, CategoryModel>(CacheKeys.ALL_CATEGORIES, CacheTimeOut.Normal, cacheObj);
            }

            return new OperationResult(cate.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged);

        }

        /// <summary>
        /// Modify the category.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public OperationResult ModifyCategory(int id, string name)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "id");
            }
            if (string.IsNullOrEmpty(name) || name.Length > 10)
            {
                return new OperationResult(OperationResultType.ParamError, "name");
            }

            var cate = _cateRepository.FindById(id);
            if (cate == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "cate");
            }

            cate.Name = name;
            var status = _cateRepository.UpdateEntity(cate);
            if (status)
            {
                //更新缓存
                var cacheObj = cate.ProjectedAs<CategoryModel, int>();
                CacheUtils.UpdateObjInCollectionCache<List<CategoryModel>, CategoryModel>(CacheKeys.ALL_CATEGORIES, CacheTimeOut.Normal, cacheObj);
            }
            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// Gets the categories entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<CategoriesEntity> GetCategoriesEntities(int page, int size, out int total)
        {
            return _cateRepository.LoadEntitiesByPaging(page, size, null, s => s.CreateTime, OrderingOrders.DESC,
                out total);
        }


        /// <summary>
        /// Toggle the category useable.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult ToggleCateGoryUseable(int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError);
            }

            var cate = _cateRepository.FindById(id);
            if (cate == null)
            {
                return new OperationResult(OperationResultType.QueryNull, "cate");
            }

            cate.IsUseable = !cate.IsUseable;
            var status = _cateRepository.UpdateEntity(cate);
            if (status)
            {
                //更新缓存
                var cacheObj = cate.ProjectedAs<CategoryModel, int>();
                CacheUtils.UpdateObjInCollectionCache<List<CategoryModel>, CategoryModel>(CacheKeys.ALL_CATEGORIES, CacheTimeOut.Normal, cacheObj);
            }
            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);

        }

    }
}
