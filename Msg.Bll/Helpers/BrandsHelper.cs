using System;
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
    /// 品牌帮助类
    /// </summary>
    public class BrandsHelper
    {
        private readonly EfRepository<BrandsEntity, int> _brandRepository = EfRepository<BrandsEntity, int>.Instance;


        private static BrandsHelper _instance;

        /// <summary>
        /// The instance
        /// </summary>
        public static BrandsHelper Instance
        {
            get { return _instance ?? (_instance = new BrandsHelper()); }
        }


        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public BrandsEntity GetEntity(int id)
        {
            return _brandRepository.FindById(id);
        }

        /// <summary>
        /// Gets the brand model.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public BrandsModel GetBrandModel(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return _brandRepository.FindById(id).ProjectedAs<BrandsModel, int>();

        }

        /// <summary>
        /// Gets the brands models in cache.
        /// </summary>
        /// <returns></returns>
        public List<BrandsModel> GetUesebaleBrandsModelsInCache()
        {
            var cacheObj = CacheHelper.Get<List<BrandsModel>>(CacheKeys.ALL_BRANDS);
            if (cacheObj != null) return cacheObj;

            cacheObj = _brandRepository.LoadEntities(s => s.IsUseable).Project().To<BrandsModel>().ToList();

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.ALL_BRANDS, cacheObj, CacheTimeOut.Normal);
            }
            return cacheObj;
        }

        /// <summary>
        /// Gets the brands entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="total">The total</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public List<BrandsModel> GetBrandsModels(int page, out int total, int size = 30)
        {
            var query = _brandRepository.LoadEntitiesByPaging<DateTime>(page, size, null, s => s.CreateTime,
                OrderingOrders.DESC, out total).OrderByDescending(s => s.IsUseable);

            return query.Project().To<BrandsModel>().ToList();

        }

        /// <summary>
        /// Adds the brand.
        /// </summary>
        /// <param name="brand">The brand.</param>
        /// <returns></returns>
        public OperationResult AddBrand(BrandsModel brand)
        {
            OperationResult addBrand;
            if (!ValidParameters(brand, out addBrand)) return addBrand;
            var entity = new BrandsEntity()
            {
                Name = brand.Name,
                Logo = brand.Logo
            };

            entity = _brandRepository.AddEntity(entity);

            if (entity.Id > 0)
            {
                //更新缓存
                var cacheObj = entity.ProjectedAs<BrandsModel, int>();
                CacheUtils.AppendToCollectionCache<List<BrandsModel>, BrandsModel>(CacheKeys.ALL_BRANDS, CacheTimeOut.Normal, cacheObj);

                return new OperationResult(OperationResultType.Success, "添加成功", entity);
            }

            return new OperationResult(OperationResultType.NoChanged, "添加失败");
        }

        /// <summary>
        /// Valids the parameters.
        /// </summary>
        /// <param name="brand">The brand.</param>
        /// <param name="addBrand">The add brand.</param>
        /// <returns></returns>
        private bool ValidParameters(BrandsModel brand, out OperationResult addBrand)
        {
            PublicHelper.CheckArgument(brand, "brandEntity");

            if (string.IsNullOrEmpty(brand.Name))
            {
                {
                    addBrand = new OperationResult(OperationResultType.ParamError, "品牌名称不能为空");
                    return false;
                }
            }
            if (string.IsNullOrEmpty(brand.Logo) && !Utils.Utils.IsURL(brand.Logo))
            {
                {
                    addBrand = new OperationResult(OperationResultType.ParamError, "品牌Logo不能为空");
                    return false;
                }
            }
            addBrand = null;
            return true;
        }

        /// <summary>
        /// Modifies the brand.
        /// </summary>
        /// <param name="brand">The brand.</param>
        /// <returns></returns>
        public OperationResult ModifyBrand(BrandsModel brand)
        {
            OperationResult addBrand;
            if (!ValidParameters(brand, out addBrand)) return addBrand;

            if (brand.Id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id错误");
            }

            var entity = _brandRepository.FindById(brand.Id);
            entity.Name = brand.Name;
            entity.Logo = brand.Logo;
            entity.IsUseable = brand.IsUseable;
            var status = _brandRepository.UpdateEntity(entity);
            if (status)
            {
                //更新缓存
                var cacheObj = entity.ProjectedAs<BrandsModel, int>();
                CacheUtils.UpdateObjInCollectionCache<List<BrandsModel>, BrandsModel>(CacheKeys.ALL_BRANDS, CacheTimeOut.Normal, cacheObj);

                return new OperationResult(OperationResultType.Success, "修改成功");
            }
            return new OperationResult(OperationResultType.NoChanged);


        }

        /// <summary>
        /// Disables the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult DisableBrand(int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id错误");
            }
            var brand = _brandRepository.FindById(id);
            if (brand != null)
            {
                brand.IsUseable = !brand.IsUseable;
                var status = _brandRepository.UpdateEntity(brand);
                if (status)
                {
                    //更新缓存
                    var cacheObj = brand.ProjectedAs<BrandsModel, int>();
                    CacheUtils.UpdateObjInCollectionCache<List<BrandsModel>, BrandsModel>(CacheKeys.ALL_BRANDS, CacheTimeOut.Normal, cacheObj);

                    return new OperationResult(OperationResultType.Success);
                }
                else { return new OperationResult(OperationResultType.NoChanged); }
            }
            return new OperationResult(OperationResultType.QueryNull, "brand不存在");

        }

        /// <summary>
        /// Deletes the brand.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public OperationResult DeleteBrand(int id)
        {
            if (id <= 0)
            {
                return new OperationResult(OperationResultType.ParamError, "Id错误");
            }

            var status = _brandRepository.DeleteEntity(id);
            if (status)
            {
                //更新缓存
                CacheUtils.DropObjInCollectionCache<BrandsModel, int>(CacheKeys.ALL_BRANDS, CacheTimeOut.Normal, id);
                return new OperationResult(OperationResultType.Success);
            }
            return new OperationResult(OperationResultType.NoChanged);

        }



    }
}
