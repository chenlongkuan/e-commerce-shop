using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Msg.Tools;
using Msg.Utils;

namespace Msg.Providers.Repository
{
    public class EfRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {

        #region 单例

        private readonly UnitOfWork.UnitOfWork _uWork = IoC.Current.Resolve<UnitOfWork.UnitOfWork>();

        private static EfRepository<TEntity, TKey> _instance;

        /// <summary>
        /// The instance
        /// </summary>
        public static EfRepository<TEntity, TKey> Instance
        {
            get { return _instance ?? (_instance = new EfRepository<TEntity, TKey>()); }
        }


        #endregion

        public EfRepository()
        {
            //_uWork.Context.Configuration.LazyLoadingEnabled = true;
            //_uWork.Context.Configuration.ValidateOnSaveEnabled = false;
        }


        /// <summary>
        /// Adds the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="immediatelyCommit">是否立即提交</param>
        /// <returns></returns>
        public TEntity AddEntity(TEntity entity, bool immediatelyCommit = true)
        {

            EntityState state = _uWork.Context.Entry(entity).State;
            if (state == EntityState.Detached)
            {
                _uWork.Context.Entry(entity).State = EntityState.Added;
            }
            if (immediatelyCommit)
            {
                _uWork.SaveChanges();
            }

            return entity;
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="immediatelyCommit">是否立即提交</param>
        /// <returns></returns>
        public bool UpdateEntity(TEntity entity, bool immediatelyCommit = true)
        {
            _uWork.Context.Set<TEntity>().Attach(entity);
            _uWork.Context.Entry<TEntity>(entity).State = EntityState.Modified;
            if (immediatelyCommit)
            {
                return _uWork.SaveChanges() > 0;
            }
            return true;
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public bool UpdateEntity(IEnumerable<TEntity> entities)
        {
            try
            {
                _uWork.Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    UpdateEntity(entity,false);
                }
                return _uWork.SaveChanges() > 0;
            }
            finally
            {
                _uWork.Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool DeleteEntity(TKey id)
        {
            TEntity entity = FindById(id);
            return DeleteEntity(entity);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public bool DeleteEntity(TEntity entity)
        {
            _uWork.Context.Set<TEntity>().Attach(entity);
            _uWork.Context.Entry<TEntity>(entity).State = EntityState.Deleted;
            return _uWork.SaveChanges() > 0;
        }



        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public bool DeleteEntity(IEnumerable<TEntity> entities)
        {
            try
            {
                _uWork.Context.Configuration.AutoDetectChangesEnabled = false;
                foreach (TEntity entity in entities)
                {
                    _uWork.Context.Set<TEntity>().Attach(entity);
                    _uWork.Context.Entry<TEntity>(entity).State = EntityState.Deleted;
                }
                return _uWork.SaveChanges() > 0;
            }
            finally
            {
                _uWork.Context.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <param name="whereLambda">The where lambda.</param>
        /// <param name="bNoTracking">是否关闭实体在上下文中的状态追踪</param>
        /// <returns></returns>
        public IQueryable<TEntity> LoadEntities(Expression<Func<TEntity, bool>> whereLambda = null, bool bNoTracking = true)
        {
            if (whereLambda != null)
            {
                var entities = _uWork.Context.Set<TEntity>().AsQueryable();
                if (bNoTracking)
                {
                    entities = entities.AsNoTracking().Where<TEntity>(whereLambda).AsQueryable();
                }
                else
                {
                    entities = entities.Where<TEntity>(whereLambda).AsQueryable();
                }
                return entities;
            }
            else
            {
                var entities = _uWork.Context.Set<TEntity>().AsQueryable();
                if (bNoTracking)
                {
                    entities = entities.AsNoTracking().AsQueryable();
                }
                return entities;
             
            }

        }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="orderBy"></param>
        /// <param name="whereLambda">The where lambda.</param>
        /// <param name="orderingSelector"></param>
        /// <returns></returns>
        public IQueryable<TEntity> LoadEntitiesByPaging<TOrderKey>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> whereLambda, Expression<Func<TEntity, TOrderKey>> orderingSelector, OrderingOrders orderBy, out int total)
        {
            int skinCount = (pageIndex - 1) * pageSize;
            var query = LoadEntities(whereLambda);
            total = query.Count();
            query = orderBy.Equals(OrderingOrders.ASC) ? query.OrderBy(orderingSelector) : query.OrderByDescending(orderingSelector);

            return query.Skip(skinCount)
                     .Take(pageSize).AsQueryable(); ;
        }


        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public TEntity FindById(TKey id)
        {
            return _uWork.Context.Set<TEntity>().Find(id);
        }




        /// <summary>
        /// sql 语句查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, params object[] parameters)
        {
            return _uWork.Context.Database.SqlQuery<T>(sqlQuery, parameters);
        }

        /// <summary>
        /// sql 语句查询分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="total"></param>
        /// <param name="parameters">排序字段</param>
        /// <param name="orderType">升降排序</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="orderColumn"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteQuery<T>(string sqlQuery, string orderColumn, string orderType, int index, int size,
                                              out int total, params object[] parameters)
        {
            var temp = sqlQuery.Substring(7, sqlQuery.LastIndexOf("FROM") - 7);
            var cSql = sqlQuery.Replace(temp, " COUNT(1) ");
            var qSql = PagerHelper.GetPagingSql(sqlQuery, orderColumn, orderType, index, size);

            total = _uWork.Context.Database.SqlQuery<int>(cSql).FirstOrDefault();
            var query = _uWork.Context.Database.SqlQuery<T>(qSql, parameters);

            return query;
        }

        /// <summary>
        /// 执行sql命令，返回受影响的行数
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteCommand(string sqlCommand, params object[] parameters)
        {
            return _uWork.Context.Database.ExecuteSqlCommand(sqlCommand, parameters);
        }

    }

    public enum OrderingOrders
    {
        ASC,
        DESC
    }
}
