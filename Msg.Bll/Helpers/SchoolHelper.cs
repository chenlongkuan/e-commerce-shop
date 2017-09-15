using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Msg.Bll.Adapter;
using Msg.Bll.Models;
using Msg.Entities;
using Msg.Providers.Repository;
using Msg.Redis;
using Msg.Tools;
using Msg.Tools.ExpressionExtend;

namespace Msg.Bll.Helpers
{
    /// <summary>
    /// 学校业务逻辑帮助类
    /// </summary>
    public class SchoolHelper
    {
        private readonly EfRepository<SchoolEntity, int> _schoolRepository = EfRepository<SchoolEntity, int>.Instance;
        private readonly EfRepository<RegionEntity, int> _regionRepository = EfRepository<RegionEntity, int>.Instance;


        /// <summary>
        /// Gets the useable entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<RegionEntity> GetRegionEntities(int page, int size, out int total)
        {
            return _regionRepository.LoadEntitiesByPaging(page, size, null, s => s.Id, OrderingOrders.DESC,
                out total);
        }

        /// <summary>
        /// Gets the useable schools in cache.
        /// </summary>
        /// <returns></returns>
        public List<RegionModel> GetUseableRegionInCache()
        {
            var cacheObj = CacheHelper.Get<List<RegionModel>>(CacheKeys.ALL_REGION);
            if (cacheObj != null) return cacheObj;

            cacheObj = _regionRepository.LoadEntities(f => f.IsUsable).Project().To<RegionModel>().ToList();

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.ALL_REGION, cacheObj, CacheTimeOut.SchoolModel);
            }
            return cacheObj;
        }


        /// <summary>
        /// Gets the entity by identifier.
        /// </summary>
        /// <param name="schoolId">The school identifier.</param>
        /// <returns></returns>
        public SchoolEntity GetEntityById(int schoolId)
        {
            if (schoolId <= 0)
            {
                return null;
            }
            return _schoolRepository.FindById(schoolId);
        }


        /// <summary>
        /// Gets the useable schools in cache.
        /// </summary>
        /// <returns></returns>
        public List<SchoolModel> GetUseableSchoolsInCache()
        {
            var cacheObj = CacheHelper.Get<List<SchoolModel>>(CacheKeys.ALL_SCHOOLS);
            if (cacheObj != null) return cacheObj;

            cacheObj = GetSchoolModelsByFilter(f => f.IsUsable && f.Region.IsUsable);

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.ALL_SCHOOLS, cacheObj, CacheTimeOut.SchoolModel);
            }
            return cacheObj;
        }

        /// <summary>
        /// Gets the useable schools in cache.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        /// <returns></returns>
        public List<SchoolModel> GetUseableSchoolsInCache(int regionId)
        {
            var cacheObj = CacheHelper.Get<List<SchoolModel>>(CacheKeys.SCHOOLS_REGION);
            if (cacheObj != null) return cacheObj;

            Expression<Func<SchoolEntity, bool>> filter = f => f.Region.Id == regionId && f.IsUsable && f.Region.IsUsable;

            cacheObj = GetSchoolModelsByFilter(filter);

            if (cacheObj != null)
            {
                CacheHelper.Put(CacheKeys.SCHOOLS_REGION + regionId.ToString(), cacheObj, CacheTimeOut.SchoolModel);
            }
            return cacheObj;
        }

        private List<SchoolModel> GetSchoolModelsByFilter(Expression<Func<SchoolEntity, bool>> filter)
        {
            List<SchoolModel> cacheObj;
            var query = _schoolRepository.LoadEntities(filter).OrderBy(s => s.OrderNum).ThenByDescending(s => s.IsHot).AsQueryable();

            var dynamicQuery = from q in query
                               select new
                               {
                                   q.Id,
                                   RegionId = q.Region.Id,
                                   q.Name,
                                   q.SchoolFirstCode,
                                   q.IsHot,
                                   q.OrderNum,
                                   q.IsUsable,
                                   q.CreateTime
                               };
            var obj = dynamicQuery.Project().To<SchoolModel>().ToList();
            return obj;
        }

        /// <summary>
        /// Gets the school entities.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="size">The size.</param>
        /// <param name="regionId">The region identifier.</param>
        /// <param name="total">The total.</param>
        /// <returns></returns>
        public IQueryable<SchoolEntity> GetSchoolEntities(int page, int size, out int total, int? regionId = null)
        {
            Expression<Func<SchoolEntity, bool>> filter = null;
            Expression<Func<SchoolEntity, int>> sortCondition = s => s.Id;
            if (regionId.HasValue)
            {
                filter = f => f.Region.Id == regionId.Value;
            }
            var query= _schoolRepository.LoadEntitiesByPaging(page, size, filter, sortCondition, OrderingOrders.DESC,
                out total);
            return query;
        }

        /// <summary>
        /// 新增或编辑学校
        /// </summary>
        /// <param name="school">The school.</param>
        /// <param name="regionId">The region identifier.</param>
        /// <returns></returns>
        public OperationResult SaveSchool(SchoolEntity school, int regionId)
        {
            if (string.IsNullOrEmpty(school.Name))
            {
                return new OperationResult(OperationResultType.ParamError, "学校名称");
            }
            if (string.IsNullOrEmpty(school.SchoolFirstCode))
            {
                return new OperationResult(OperationResultType.ParamError, "拼音首字母");
            }
            if (regionId < 1)
            {
                return new OperationResult(OperationResultType.ParamError, "区域");
            }
            if (school.Id > 0)//编辑
            {
                var entity = _schoolRepository.FindById(school.Id);
                entity.Name = school.Name;
                entity.SchoolFirstCode = school.SchoolFirstCode;
                entity.IsHot = school.IsHot;
                entity.IsUsable = school.IsUsable;
                entity.OrderNum = school.OrderNum;
                entity.Region = _regionRepository.FindById(regionId);

                var status = _schoolRepository.UpdateEntity(entity);

                if (status)
                {
                    var updateObj = entity.ProjectedAs<SchoolModel, int>();
                    //更新缓存
                    CacheUtils.UpdateObjInCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.ALL_SCHOOLS, CacheTimeOut.SchoolModel, updateObj);
                    CacheUtils.UpdateObjInCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.SCHOOLS_REGION + entity.Region.Id, CacheTimeOut.SchoolModel, updateObj);
                }
                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
            }
            else
            {
                school.Region = _regionRepository.FindById(regionId);
                var result = _schoolRepository.AddEntity(school);
                if (result.Id > 0)
                {
                    var updateObj = result.ProjectedAs<SchoolModel, int>();
                    //更新缓存
                    CacheUtils.AppendToCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.ALL_SCHOOLS, CacheTimeOut.SchoolModel, updateObj);
                    CacheUtils.AppendToCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.SCHOOLS_REGION + result.Region.Id, CacheTimeOut.SchoolModel, updateObj);
                }
                return new OperationResult(result.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
            }
        }


        /// <summary>
        /// 切换学校可用状态
        /// </summary>
        /// <param name="schoolId">The school identifier.</param>
        /// <returns></returns>
        public OperationResult ToggleSchoolUseable(int schoolId)
        {
            var school = _schoolRepository.FindById(schoolId);
            school.IsUsable = !school.IsUsable;
            var status = _schoolRepository.UpdateEntity(school);

            if (status)
            {
                var updateObj = school.ProjectedAs<SchoolModel, int>();
                //更新缓存
                CacheUtils.UpdateObjInCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.ALL_SCHOOLS, CacheTimeOut.SchoolModel, updateObj);
                CacheUtils.UpdateObjInCollectionCache<List<SchoolModel>, SchoolModel>(CacheKeys.SCHOOLS_REGION + school.Region.Id, CacheTimeOut.SchoolModel, updateObj);
            }


            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }

        /// <summary>
        /// 新增或编辑区域
        /// </summary>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public OperationResult SaveRegion(RegionEntity region)
        {
            if (string.IsNullOrEmpty(region.Name))
            {
                return new OperationResult(OperationResultType.ParamError, "区域名称");
            }
            if (string.IsNullOrEmpty(region.PinyinCode))
            {
                return new OperationResult(OperationResultType.ParamError, "区域名称拼音简码");
            }
            if (region.Id > 0)
            {
                var entity = _regionRepository.FindById(region.Id);
                entity.Name = region.Name;
                entity.PinyinCode = region.PinyinCode;
                entity.IsUsable = region.IsUsable;
                var status = _regionRepository.UpdateEntity(entity);
                if (status)
                {
                    var updateObj = entity.ProjectedAs<RegionModel, int>();
                    //更新缓存
                    CacheUtils.UpdateObjInCollectionCache<List<RegionModel>, RegionModel>(CacheKeys.ALL_REGION, CacheTimeOut.SchoolModel, updateObj);
                }

                return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
            }
            else
            {
                var result = _regionRepository.AddEntity(region);
                if (result.Id > 0)
                {
                    var updateObj = result.ProjectedAs<RegionModel, int>();
                    //更新缓存
                    CacheUtils.AppendToCollectionCache<List<RegionModel>, RegionModel>(CacheKeys.ALL_REGION, CacheTimeOut.SchoolModel, updateObj);
                }
                return new OperationResult(result.Id > 0 ? OperationResultType.Success : OperationResultType.NoChanged);
            }
        }

        /// <summary>
        /// 切换区域可用状态
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        /// <returns></returns>
        public OperationResult ToggleRegionUseable(int regionId)
        {
            var region = _regionRepository.FindById(regionId);
            region.IsUsable = !region.IsUsable;
            var status = _regionRepository.UpdateEntity(region);
            if (status)
            {
                var updateObj = region.ProjectedAs<RegionModel, int>();
                //更新缓存
                CacheUtils.UpdateObjInCollectionCache<List<RegionModel>, RegionModel>(CacheKeys.ALL_REGION, CacheTimeOut.SchoolModel, updateObj);
            }

            return new OperationResult(status ? OperationResultType.Success : OperationResultType.NoChanged);
        }
    }
}
