using i2e1_basics.Utilities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;

namespace i2e1_core.Utilities
{
    public class MemoryCacheHelper
    {
        public static string Hash(string[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                try
                {
                    string input = "";
                    foreach (var val in data)
                    {
                        if (string.IsNullOrEmpty(val))
                            throw new ArgumentNullException();
                        input = input + val;
                    }
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
                catch (Exception ex)
                {
                    Logger.GetInstance().Error(ex.ToString());
                    return string.Empty;
                }
            }
        }

        public static void SetWithSlidingExpiration(string key, object data, TimeSpan timeSpan)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(timeSpan)
                .SetSize(1);
            CoreCacheHelper.GetInstance().memoryCache.Set(key, data, cacheEntryOptions);
        }

        public static void Set(string key, object data, TimeSpan timeSpan)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(timeSpan)
                .SetSize(1);
            CoreCacheHelper.GetInstance().memoryCache.Set(key, data, cacheEntryOptions);
        }

        public static T Get<T>(string key)
        {
            CoreCacheHelper.GetInstance().memoryCache.TryGetValue(key, out T value);
            return value;
        }

        public static T GetValueFromCache<T>(string key, GetValue<T> getValue, TimeSpan? timeSpan = null)
        {
            try
            {
                if (CoreCacheHelper.GetInstance().memoryCache.TryGetValue(key, out T value))
                {
                    return value;
                }
                else
                {
                    value = getValue();
                    if (timeSpan == null)
                    {
                        timeSpan = TimeSpan.FromSeconds(600);
                    }
                    Set(key, value, timeSpan.Value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error(ex.ToString());
                throw;
            }
        }
    }
}
