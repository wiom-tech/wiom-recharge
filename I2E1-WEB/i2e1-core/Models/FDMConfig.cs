using i2e1_core.Utilities;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public class FDMConfig
    {
        public long id { get; set; }

        public long selectedPlanId { get; set; }

        public FDMConfig()
        {
            otpIssuedTime = otpExpiryTime = DateTime.UtcNow.AddSeconds(-1);
            deviceAllowed = 1;
        }

        public byte deviceAllowed { get; set; }

        public string mobile { get; set; }

        public long dataPlan { get; set; } // In MB

        public long timePlan { get; set; } // time in secs

        public DateTime otpIssuedTime { get; set; }

        public DateTime otpExpiryTime { get; set; }

        public string otp { get; set; }

        public bool IsHomeRouterCoupon()
        {
            return deviceAllowed >= Constants.HOME_ROUTER_DEVICE_LIMIT;
        }

        public static string GetFDMConfigKey(WifiUser user)
        {
            if (user.storegroupid != 0)
                return CoreCacheHelper.FDM_CONFIGS + user.mobile.ToUpper() + '_' + 0 + '_' + user.storegroupid;
            else
                return CoreCacheHelper.FDM_CONFIGS + user.mobile.ToUpper() + '_' + user.nasid + '_' + 0;
        }

        public static bool IsConfigExpired(FDMConfig conf)
        {
            if (conf == null)
                return true;

            if (conf.otpExpiryTime <= DateTime.UtcNow)
                return true;

            return false;
        }
    }
}