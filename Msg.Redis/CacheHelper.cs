using System;
using System.Collections.Generic;
using Msg.Config;
using ServiceStack.Redis;

namespace Msg.Redis
{


    /// <summary>
    /// 公共缓存策略接口
    /// </summary>
    public sealed class CacheHelper
    {
        #region - Variable -


        private static PooledRedisClientManager _prcm;//redis服务IP和端口

        #endregion

        #region - Constructor -

        CacheHelper()
        {
            CreateManager();
        }

        #endregion

        #region - Method -


        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public static IRedisClient GetClient(out bool IsConnected)
        {
            IRedisClient client;
            IsConnected = false;
            try
            {
                if (_prcm == null)
                {
                    CreateManager();
                    if (_prcm != null)
                    {
                        client = _prcm.GetClient();
                        IsConnected = client != null;
                        return client;
                    }

                }
                else
                {
                    client = _prcm.GetClient();
                    IsConnected = client != null;
                    return client;
                }
            }
            catch (Exception)
            {
                IsConnected = false;
            }
          
            return null;
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="datetime">过期时间</param>
        public static void List_SetExpire(string key, DateTime datetime)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="datetime">过期时间</param>
        public static void Hash_SetExpire(string key, DateTime datetime)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return;
            using (var redis = client)
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        #endregion


        #region 链接


        /// <summary>
        /// 连接池管理
        /// </summary>
        /// <returns></returns>
        private static void CreateManager()
        {

            var redisConfig = CacheConfigManager.GetConfig("c:\\Config\\Redis.config");
            //根据配置是否开启缓存
            if (redisConfig.model == 1)
            {
                var readWriteHosts = new string[] { redisConfig.HostName + ":" + redisConfig.HostPort };
                var readOnlyHosts = new string[] { redisConfig.HostName + ":" + redisConfig.HostPort };
                //支持读写分离，均衡负载
                _prcm = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
                {
                    MaxWritePoolSize = 5000,//“写”链接池链接数
                    MaxReadPoolSize = 5000,//“读”链接池链接数
                    AutoStart = true,
                });
            }

        }
        #endregion


        #region -- 二进制安全的 字符串 string --
        /// <summary>
        /// 添加缓存  长期有效
        /// </summary>
        /// <param name="objId">键</param>
        /// <param name="o">值</param>
        public static void Put(string objId, object o)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                redis.Set(objId, o);
            }
        }

        /// <summary>
        /// 添加缓存   指定过期分钟数
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Obj">值</param>
        /// <param name="minutes">过期时间（分钟）</param>
        public static void Put(string Key, object Obj, int minutes)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return;
            using (var redis = client)
            {
                redis.Set(Key, Obj, new TimeSpan(0, minutes, 0));
            }
        }

        /// <summary>
        /// 移除一个缓存项
        /// </summary>
        /// <param name="objId">键</param>
        public static void Remove(string objId)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return;
            using (var redis = client)
            {
                redis.Remove(objId);
            }

        }

        /// <summary>
        /// 缓存项是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyExists(string key)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return false;
            using (var redis = client)
            {
                return redis.ContainsKey(key);
            }
        }

        /// <summary>
        /// 获取一个指定类型的缓存
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="objId">键</param>
        /// <returns></returns>
        public static T Get<T>(string objId)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return default(T);
            using (var redis = client)
            {
                var obj = redis.Get<T>(objId);
                return obj;
            }
        }

        #endregion

        #region -- 二进制安全的 字符串列表 list of string --

        /// <summary>
        /// 添加集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="t">存入对象</param>
        public static void List_Add<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
                redisTypedClient.Dispose();
            }
        }


        /// <summary>
        /// 移除指定键，指定类型集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="t">存入对象</param>
        /// <returns></returns>
        public static bool List_Remove<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return false;
            using (var redis = Client)
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                var status = redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
                redisTypedClient.Dispose();
                return status;
            }
        }

        /// <summary>
        /// 移除指定键的所有集合
        /// </summary>
        /// <param name="key">键</param>
        public static void List_RemoveAll<T>(string key)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.Lists[key].RemoveAll();
                redisTypedClient.Dispose();
            }
        }

        /// <summary>
        /// 获取指定键的集合元素数量
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static long List_Count(string key)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return 0;
            using (var redis = Client)
            {
                return redis.GetListCount(key);
            }
        }

        /// <summary>
        /// 获取指定键从N开始的X个元素
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="start">起始值</param>
        /// <param name="count">提取数量</param>
        /// <returns></returns>
        public static List<T> List_GetRange<T>(string key, int start, int count)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return default(List<T>);
            using (var redis = Client)
            {
             
                var c = redis.GetTypedClient<T>();
                var list = c.Lists[key].GetRange(start, start + count - 1);
                c.Dispose();
                return list;
            }
        }

        /// <summary>
        /// 获取某键集合的所有元素
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static List<T> List_GetList<T>(string key)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return default(List<T>);
            using (var redis = Client)
            {
                var c = redis.GetTypedClient<T>();
                var list = c.Lists[key].GetAll();
                c.Dispose();
                return list;
                //return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
        }

        /// <summary>
        /// 分页获取某键集合元素
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页项数</param>
        /// <returns></returns>
        public static List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }
        #endregion

        #region -- Hash --

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <param name="dataKey">数据键</param>
        /// <returns></returns>
        public static bool Hash_Exist<T>(string key, string dataKey)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return false;
            using (var redis = client)
            {
                return redis.HashContainsEntry(key, dataKey);
            }
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <param name="dataKey">数据键</param>
        /// <returns></returns>
        public static bool Hash_Set<T>(string key, string dataKey, T t)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return false;
            using (var redis = client)
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.SetEntryInHash(key, dataKey, value);
            }
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <param name="dataKey">数据键</param>
        /// <returns></returns>
        public static bool Hash_Remove(string key, string dataKey)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return false;
            using (var redis = client)
            {
                return redis.RemoveEntryFromHash(key, dataKey);
            }
        }
        /// <summary>
        /// 移除整个hash
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <param name="dataKey">数据键</param>
        /// <returns></returns>
        public static bool Hash_Remove(string key)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return false;
            using (var redis = client)
            {
                return redis.Remove(key);
            }
        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <param name="dataKey">数据键</param>
        /// <returns></returns>
        public static T Hash_Get<T>(string key, string dataKey)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return default(T);
            using (var redis = client)
            {
                string value = redis.GetValueFromHash(key, dataKey);
                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            }
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">hashId</param>
        /// <returns></returns>
        public static List<T> Hash_GetAll<T>(string key)
        {
            var isConnected = false;
            var client = GetClient(out isConnected);
            if (!isConnected) return default(List<T>);
            using (var redis = client)
            {
                var list = redis.GetHashValues(key);
                if (list != null && list.Count > 0)
                {
                    var result = new List<T>();
                    foreach (var item in list)
                    {
                        var value = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(value);
                    }
                    return result;
                }
                return null;
            }
        }
        #endregion

        #region -- 二进制安全的 字符串集合 set of string --

        public static void Set_Add<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.Sets[key].Add(t);
                redisTypedClient.Dispose();
            }
        }
        public static bool Set_Contains<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return false;
            using (var redis = Client)
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                var isContains = redisTypedClient.Sets[key].Contains(t);
                redisTypedClient.Dispose();
                return isContains;
            }
        }
        public static bool Set_Remove<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return false;
            using (var redis = Client)
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                var isRemove = redisTypedClient.Sets[key].Remove(t);
                redisTypedClient.Dispose();
                return isRemove;

            }
        }

        #endregion

        #region -- 有序集合sorted set of string --
        /// <summary>
        ///  添加数据到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public static bool SortedSet_Add<T>(string key, T t, double score)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return false;
            using (var redis = Client)
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.AddItemToSortedSet(key, value, score);
            }
        }
        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool SortedSet_Remove<T>(string key, T t)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return false;
            using (var redis = Client)
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.RemoveItemFromSortedSet(key, value);
            }
        }
        /// <summary>
        /// 修剪SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="size">保留的条数</param>
        /// <returns></returns>
        public static long SortedSet_Trim(string key, int size)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return 0;
            using (var redis = Client)
            {
                return redis.RemoveRangeFromSortedSet(key, size, 9999999);
            }
        }
        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long SortedSet_Count(string key)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return 0;
            using (var redis = Client)
            {
                return redis.GetSortedSetCount(key);
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return default(List<T>);
            using (var redis = Client)
            {
                var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListALL<T>(string key)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return default(List<T>);
            using (var redis = Client)
            {
                var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SortedSet_SetExpire(string key, DateTime datetime)
        {
            var IsConnected = false;
            var Client = GetClient(out IsConnected);
            if (!IsConnected) return;
            using (var redis = Client)
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        //public static double SortedSet_GetItemScore<T>(string key,T t)
        //{
        //    using (IRedisClient redis = GetClient())
        //    {
        //        var data = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
        //        return redis.GetItemScoreInSortedSet(key, data);
        //    }
        //    return 0;
        //}

        #endregion


    }
}
