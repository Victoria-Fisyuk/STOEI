using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Caching;

namespace TAiMStore.Model.Classes
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Возвращает объект из кэша, если объекта нет - добавляет его в кэш.
        /// </summary>
        /// <typeparam name="T">Тип объекта в кэше</typeparam>
        /// <param name="cache"></param>
        /// <param name="key">ключ объекта в кэше</param>
        /// <param name="expiresSeconds">Время жизни объекта в кэше от момента кэширования</param>
        /// <param name="setter">Функция, инициализирующая объект для помещения в кэш</param>
        /// <returns></returns>
        public static T CheckCache<T>(this Cache cache, string key, int expiresSeconds, Func<T> setter)
        {
            var result = cache[key];
            if (result == null)
            {
                result = setter();
                if (result != null)
                    cache.Add(key, result, null, DateTime.Now.AddSeconds(expiresSeconds), Cache.NoSlidingExpiration,
                              CacheItemPriority.Normal, null);
            }
            return (T)result;
        }

        /// <summary>
        /// Возвращает ключ для кэша
        /// Для генерации ключа используются параметры, передаваемые вызывающему методу
        /// и имя самого вызывающего метода
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="parameters">параметры, передаваемые вызывающему методу</param>
        /// <returns></returns>
        public static string GenerateCacheKey(this Cache cache, params object[] parameters)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            string key = methodBase.Name;
            foreach (object o in parameters)
            {
                key = key.Insert(0, o.ToString());
            }
            return key;
        }
    }
}
