using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Web;

namespace SimpleRuntimeCache.App.CacheHelper
{
    public class CacheWrapper
    {
        private static readonly object LockObj = new object();
        private static readonly ObjectCache Cache = MemoryCache.Default;

        public static void Add<T>(T o, string key, DateTimeOffset? absoluteExpiration)
        {
            lock (LockObj)
            {
                DateTimeOffset absoluteExpirationValue = absoluteExpiration.HasValue ? absoluteExpiration.Value : new DateTimeOffset(DateTime.UtcNow.AddMinutes(5));
                Cache.Add(key, o, absoluteExpirationValue);
            }
        }

        private static void Clear(string key)
        {
            lock (LockObj)
            {
                Cache.Remove(key);
            }
        }

        public static bool Exists(string key)
        {
            lock (LockObj)
            {
                return Cache[key] != null;
            }
        }

        public static bool Get<T>(string key, out T value)
        {
            try
            {
                if (!Exists(key))
                {
                    value = default(T);
                    return false;
                }

                lock (LockObj)
                {
                    value = (T)Cache[key];
                }
            }
            catch (Exception ex)
            {
                //log ex
                value = default(T);
                return false;
            }

            return true;
        }

        public static bool Update<T>(string key, T additionalFields, out T value)
        {
            lock (LockObj)
            {
                try
                {
                    if (!Exists(key))
                    {
                        value = default(T);
                        return false;
                    }

                    value = (T)Cache[key];

                    // Call AddRange method using reflection
                    if (value.GetType().IsGenericType)
                    {
                        MethodInfo addRangeMethod = value.GetType().GetMethod("AddRange");
                        addRangeMethod.Invoke(value, new object[] { additionalFields });
                    }
                }
                catch (Exception ex)
                {
                    //log ex
                    value = default(T);
                    return false;
                }

                return true;
            }
        }

        public static void Invalidate(Action a, string key)
        {
            Clear(key);
            a.Invoke();
        }
    }
}