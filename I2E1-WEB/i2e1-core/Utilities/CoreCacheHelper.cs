using i2e1_basics.Models;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace i2e1_core.Utilities
{
	public delegate T GetValue<T>();

    public delegate T GetValueWithExpiry<T>(out TimeSpan expiryTimeSpan);

    public class CoreCacheHelper
    {
        public const string PLAN_CONF = "PLAN_CONF";
        public const string MAC_CONFIG = "MAC_CONFIG";
        public const string TEMPLATE_CONTENT = "TEMPLATE_CONTENT";
        public const string NAS_VOUCHER = "NAS_VOUCHER";
        public const string FDM_CONFIGS = "FDM_CONFIGS";
        public const string ROOM_NO_LASTNAME = "ROOM_NO_LASTNAME";
        public const string VIP_LIST = "VIP_LIST";
        public const string WHITE_BLOCK_LIST = "WHITE_BLOCK_LIST";
        public const string PLAN_SES = "PLAN_SES";
        public const string PLAN_MAC_MAPPING = "PLAN_MAC_MAPPING";
        public const string ROUTER_BASIC = "ROUTER_BASIC";
        public const string ROUTER_TAG_DETAILS = "ROUTER_TAG_DETAILS";
        public const string NAS_FROM_MACV2 = "NAS_FROM_MACV2";
        public const string GRID_BY_SIZE = "GRID_BY_SIZE";
        public const string NAS_USER_GROUPS = "NAS_USER_GROUPS";
        public const string CAMPAIGN_TARGETING = "CAMPAIGN_TARGETING";
        public const string ACTIVE_CAMPAIGN = "ACTIVE_CAMPAIGN";
        public const string GOOGLE_LOCATION_DETAILS = "GOOGLE_LOCATION_DETAILS";
        public const string LOCATION_WEATHER_DETAILS = "LOCATION_WEATHER_DETAILS";
        public const string LOCATION_ID_DETAILS = "LOCATION_ID_DETAILS";
        public const string B2CC_LINQ_PROMOTION = "B2CC_LINQ_PROMOTION";
        public const string COUNTRY_GROUP = "COUNTRY_GROUP";
        public const string STATE_GROUP = "STATE_GROUP";
        public const string CITY_GROUP = "CITY_GROUP";
        public const string WIKI_RESPONSE = "WIKI_RESPONSE";
        public const string GET_ALL_SUBCATEGORIES = "GET_ALL_SUBCATEGORIES";
        public const string GET_FEATURED_CONTENT = "GET_FEATURED_CONTENT";
        public const string UNREGISTERED_NASES = "UNREGISTERED_NASES";
        public const string DIY_REG_OTP = "DIY_REG_OTP";

        public const string PARTNER_DETAIL = "PARTNER_DETAIL";
        public const string CAMPAIGN_REPORTS = "CAMPAIGN_REPORTS";
        public const string FOOTFALL_SUMMARY = "FOOTFALL_SUMMARY";
        public const string ADVERTISISING_TEMPLATE = "ADVERTISISING_TEMPLATE";

        public const string SWAPP_PROMOTION = "SWAPP_PROMOTION";

        public const string DAILY_NAS_DATA_LIMIT = "DAILY_NAS_DATA_LIMIT";

        public const string ACCOUNT_ID_FROM_USER_ID = "ACCOUNT_ID_FROM_USER_ID";
        public const string USER_ACCOUNT = "USER_ACCOUNT";
        public const string SHORT_URL = "SHORTY";

        public const string LATEST_APP_VERSIONS = "LATEST_APP_VERSIONS";
        public const string PARTNER_EDUCATION_VIDEO = "PARTNER_EDUCATION_VIDEO";
        public const string APP_USER_ACCESS = "APP_USER_ACCESS";
        public const string RATING_VARIABLES = "RATING_VARIABLES_";
        public const string NAS_DETAILS_FROM_DEVICEID = "NAS_DETAILS_FROM_DEVICEID_";
        public const string NAS_DETAILS_FROM_NAS = "NAS_DETAILS_FROM_NAS_";
        public const string WANI_CREDENTIALS = "WANI_CREDENTIALS";
        public const string SERVER_10_APP_VERSIONS = "SERVER_10_APP_VERSIONS";
        public const string LATEST_APP_VERSIONS0 = "LATEST_APP_VERSIONS0";
        public const string SERVER_10_ACCOUNTS = "SERVER_10_ACCOUNTS";
        public const string FORCE_APP_VERSION_UPDATE = "FORCE_APP_VERSION_UPDATE";
        public const string ADVERTISISING_ID_LOGIN_USER = "ADVERTISISING_ID_LOGIN_USER_";
        public const string LAST_POWER = "LAST_POWER";

        public IDatabase cache;
        public IMemoryCache memoryCache;

        private static CoreCacheHelper cacheHelper = null;

        protected CoreCacheHelper(string cacheConnectionString, int cachedbId)
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(cacheConnectionString);
            this.cache = connection.GetDatabase(cachedbId);
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public static CoreCacheHelper CreateInstance(string cacheConnectionString, int cachedbId)
        {
            if (cacheHelper == null)
            {
                cacheHelper = new CoreCacheHelper(cacheConnectionString, cachedbId);
            }
            return cacheHelper;
        }

        public static CoreCacheHelper GetInstance()
        {
            return cacheHelper;
        }

        public void setCache(string key, object data, TimeSpan? interval = null)
        {
            var stringData = JsonConvert.SerializeObject(data);
            cache.StringSetAsync(key, stringData, interval, When.Always, CommandFlags.FireAndForget);
        }

        protected RedisValue getCache(string key)
        {
            return cache.StringGet(key);
        }

        public void Reset(string key, object id)
        {
            Logger.GetInstance().Info("clearing the cache for : " + key + id);
            cache.KeyDelete(key + id, CommandFlags.FireAndForget);
        }

        public void Reset(string key, LongIdInfo id)
        {
            cache.KeyDelete(key + id.ToSafeDbObject(1), CommandFlags.FireAndForget);
        }

        public void ResetAll(LongIdInfo nasid)
        {
            Reset(MAC_CONFIG, nasid);
            Reset(VIP_LIST, nasid);
            Reset(WHITE_BLOCK_LIST, nasid);
            Reset(ROUTER_BASIC, nasid);
        }


        public T getValueFromCache<T>(string key, object id, GetValue<T> getValue, bool disableCache = false)
        {
            try
            {
                RedisValue data = RedisValue.Null;
                T value;
                if (!disableCache)
                {
                    data = getCache(key + id);
                }

                if (disableCache || data.IsNullOrEmpty)
                {
                    value = getValue();
                    if(value != null)
                    {
                        var time = CoreUtil.GetTimeInIST();
                        TimeSpan interval = time.AddDays(1).Date - time;
                        setCache(key + id, value, interval);
                    }
                }
                else
                {
                    value = JsonConvert.DeserializeObject<T>(data);
                }

                return value;
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error(ex.ToString());
                throw;
            }
        }


        private PlanMacMapping parsePlanMacMapping(string entry, string mac, out bool isExpired)
        {
            var values = entry.ToString().Split('_');
            long planId = long.Parse(values[0]);

            DateTime lastActive;
            if (long.TryParse(values[1], out long timeStamp))
                lastActive = DateTime.FromBinary(timeStamp);
            else
                lastActive = DateTime.Parse(values[1]);

            isExpired = false;
            if (planId != 0 && (DateTime.UtcNow - lastActive).TotalSeconds > 300)
            {
                isExpired = true;
            }
            return new PlanMacMapping()
            {
                mac = mac,
                planId = planId,
                device = values[2]
            };
        }



        public NasSplitTemplate GetRouterNasidFromMacv2(string macId)
        {
            return getValueFromCache(NAS_FROM_MACV2, macId,
            () =>
            {
                return ShardHelper.GetLongNasFromMac(macId);
            });
        }

        public string getUserBasicConfigKey(int combinedSettingId, int userGroupId)
        {
            return combinedSettingId + ".USERGROUP_BASIC_CONFIG_" + userGroupId;
        }



        public void SetShortUrl(string id, string longUrl, int ttl)
        {
            setCache(SHORT_URL + id, longUrl, new TimeSpan(0, ttl, 0));
        }

        public static string GetCacheConnectionString(string redisHost, int redisPort, string redisPassword)
        {         
            var connection = string.Format("{0}:{1},ssl=False,abortConnect=False,syncTimeout=5000,responseTimeout=5000,writeBuffer=78643200",
                I2e1ConfigurationManager.GetInstance().GetPrivateIP(redisHost), redisPort);
            if (!string.IsNullOrEmpty(redisPassword))
            {
                connection += ",password=" + redisPassword;
            }
            Logger.GetInstance().Info("cache connection url: " + connection);
            return connection;
        }
    }
}
