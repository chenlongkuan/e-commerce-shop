using System.Collections.Generic;
using System.Linq;
using ServiceStack.Common.Utils;

namespace Msg.Redis
{
    public static class CacheUtils
    {
        /// <summary>
        /// 追加对象到已存在的缓存集合
        /// </summary>
        /// <returns></returns>
        public static void AppendToCollectionCache<TCollection, TObj>(string key, int timeOut, TObj obj) where TCollection : IList<TObj>, new()
        {
            var cacheObj = CacheHelper.Get<TCollection>(key);
            if (cacheObj != null)
            {
                cacheObj.Add(obj);
            }
            else
            {
                cacheObj = new TCollection() { obj };
            }
            CacheHelper.Remove(key);
            CacheHelper.Put(key, cacheObj, timeOut);
        }

        /// <summary>
        /// 更新对象到已存在的缓存集合
        /// </summary>
        /// <returns></returns>
        public static void UpdateObjInCollectionCache<TCollection, TObj>(string key, int timeOut, TObj obj)
            where TCollection : IList<TObj>, new()
        {
            var cacheObj = CacheHelper.Get<TCollection>(key);
            if (cacheObj != null)
            {
                if (cacheObj.Any(f => f.GetId() == obj.GetId()))//验证是否已经存在于缓存集合中
                {
                    for (var i = 0; i < cacheObj.Count; i++)
                    {
                        if (cacheObj[i].GetId() == obj.GetId())
                        {
                            cacheObj[i] = obj;//存在则修改对应对象
                        }
                    }
                }
                else
                {
                    cacheObj.Add(obj);//不存在则新增至该缓冲集合
                }

            }
            else
            {
                cacheObj = new TCollection() { obj };
            }
            CacheHelper.Remove(key);
            CacheHelper.Put(key, cacheObj, timeOut);
        }

        /// <summary>
        /// 更新对象到已存在的缓存对象
        /// </summary>
        /// <returns></returns>
        public static void UpdateCache<T>(string key, int timeOut, T obj) where T : class
        {
            CacheHelper.Remove(key);
            CacheHelper.Put(key, obj, timeOut);

        }


        /// <summary>
        /// 删除已存在的缓存集合中的指定对象
        /// </summary>
        /// <returns></returns>
        public static void DropObjInCollectionCache<T, TKey>(string key, int timeOut, TKey id)
            where T :class
        {
            var cacheObj = CacheHelper.Get<List<T>>(key);
            if (cacheObj != null)
            {
                for (var i = 0; i < cacheObj.Count; i++)
                {
                    if (cacheObj[i].GetId().ToString() == id.ToString())
                    {
                        cacheObj.Remove(cacheObj[i]);//存在则修改对应对象
                    }
                }
            }

            CacheHelper.Remove(key);
            CacheHelper.Put(key, cacheObj, timeOut);
        }


    }
}
