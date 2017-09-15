using System;
using System.Collections;
using System.Configuration;
using System.Data.Entity;
using System.Threading;
using System.Web;

namespace Msg.Providers.Context
{
    /// <summary>
    /// Manages the lifecycle of the object context.Uses a context per http request approach or one per thread in non web applications
    /// </summary>
    public static class ContextManager
    {


       private static readonly Hashtable _contextQueue = new Hashtable();      
        private const string _key = "Enterprise.Data.Manager";
        private static string _connectionString = string.Empty;




        static ContextManager()
        {
            if (ConfigurationManager.ConnectionStrings["default"] == null || String.IsNullOrEmpty(ConfigurationManager.ConnectionStrings["default"].ToString()))
                throw new ArgumentNullException("Please Create and Define Connection key in ConnectionStrings section of the Confuguration File");

            _connectionString = ConfigurationManager.ConnectionStrings["default"].ToString();
        }


  
        /// <summary>
        /// Gets the repository context
        /// </summary>
        /// <returns>An object representing the repository context</returns>
        public static DbContext GetRepositoryContext()
        {
            DbContext objectContext = GetCurrentObjectContext();
            if (objectContext == null)
            {
                objectContext = new EfDbContext();
                StoreCurrentObjectContext(objectContext);
            }
            return objectContext;
        }

        /// <summary>
        /// Sets the repository context
        /// </summary>
        /// <param name="repositoryContext">An object representing the repository context</param>
        public static void SetRepositoryContext(object repositoryContext)
        {
            if (repositoryContext == null)
            {
                RemoveCurrentObjectContext();
            }
            else if (repositoryContext is DbContext)
            {
                StoreCurrentObjectContext((DbContext)repositoryContext);
            }
        }




        /// <summary>
        /// gets the current object context 		
        /// </summary>
        private static DbContext GetCurrentObjectContext()
        {
            DbContext objectContext = null;
            if (HttpContext.Current == null)
                objectContext = GetCurrentThreadObjectContext();
            else
                objectContext = GetCurrentHttpContextObjectContext();
            return objectContext;
        }

        /// <summary>
        /// sets the current session 		
        /// </summary>
        private static void StoreCurrentObjectContext(DbContext objectContext)
        {
            if (HttpContext.Current == null)
                StoreCurrentThreadObjectContext(objectContext);
            else
                StoreCurrentHttpContextObjectContext(objectContext);
        }

        /// <summary>
        /// remove current object context 		
        /// </summary>
        private static void RemoveCurrentObjectContext()
        {
            if (HttpContext.Current == null)
                RemoveCurrentThreadObjectContext();
            else
                RemoveCurrentHttpContextObjectContext();
        }

  

        /// <summary>
        /// gets the object context for the current thread
        /// </summary>
        private static DbContext GetCurrentHttpContextObjectContext()
        {
            DbContext objectContext = null;
            if (HttpContext.Current.Items.Contains(_key))
                objectContext = (DbContext)HttpContext.Current.Items[_key];
            return objectContext;
        }

        private static void StoreCurrentHttpContextObjectContext(DbContext objectContext)
        {
            if (HttpContext.Current.Items.Contains(_key))
                HttpContext.Current.Items[_key] = objectContext;
            else
                HttpContext.Current.Items.Add(_key, objectContext);
        }

        /// <summary>
        /// remove the session for the currennt HttpContext
        /// </summary>
        private static void RemoveCurrentHttpContextObjectContext()
        {
            DbContext objectContext = GetCurrentHttpContextObjectContext();
            if (objectContext != null)
            {
                HttpContext.Current.Items.Remove(_key);
                objectContext.Dispose();
            }
        }




        /// <summary>
        /// gets the session for the current thread
        /// </summary>
        private static DbContext GetCurrentThreadObjectContext()
        {
            DbContext objectContext = null;
            Thread threadCurrent = Thread.CurrentThread;
            if (threadCurrent.Name == null)
                threadCurrent.Name = Guid.NewGuid().ToString();
            else
            {
                object threadObjectContext = null;
                lock (_contextQueue.SyncRoot)
                {
                    threadObjectContext = _contextQueue[BuildContextThreadName()];
                }
                if (threadObjectContext != null)
                    objectContext = (DbContext)threadObjectContext;
            }
            return objectContext;
        }

        private static void StoreCurrentThreadObjectContext(DbContext objectContext)
        {
            lock (_contextQueue.SyncRoot)
            {
                if (_contextQueue.Contains(BuildContextThreadName()))
                    _contextQueue[BuildContextThreadName()] = objectContext;
                else
                    _contextQueue.Add(BuildContextThreadName(), objectContext);
            }
        }

        private static void RemoveCurrentThreadObjectContext()
        {
            lock (_contextQueue.SyncRoot)
            {
                if (_contextQueue.Contains(BuildContextThreadName()))
                {
                    DbContext objectContext = (DbContext)_contextQueue[BuildContextThreadName()];
                    if (objectContext != null)
                    {
                        objectContext.Dispose();
                    }
                    _contextQueue.Remove(BuildContextThreadName());
                }
            }
        }

        private static string BuildContextThreadName()
        {
            return Thread.CurrentThread.Name;
        }




    }
}