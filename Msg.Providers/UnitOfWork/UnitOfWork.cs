using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Msg.Providers.Context;
using Msg.Tools.Logging;

namespace Msg.Providers.UnitOfWork
{
    public class UnitOfWork
    {

        /// <summary>
        /// 获取 当前使用的数据访问上下文对象
        /// </summary>
        public DbContext Context
        {
            get { return ContextManager.GetRepositoryContext(); }
        }


        public int SaveChanges()
        {
            try
            {
                var rowEffect= Context.SaveChanges();
                return rowEffect;
            }
            catch (DbEntityValidationException ex)
            {
                LogHelper.WriteException("SaveChanges->",ex);
                return 0;
            }

        }


        #region IDisposable 成员

        private bool _disposed;
        public void Dispose()
        {
            if (Context != null && Context.Database.Connection.State == ConnectionState.Open)
                Context.Database.Connection.Close();

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                Context.Dispose();
            _disposed = true;
        }

        #endregion



    }
}
