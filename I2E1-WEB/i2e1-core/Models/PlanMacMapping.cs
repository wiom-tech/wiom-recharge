using i2e1_core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace i2e1_core.Models
{
    public class PlanMacMapping
    {
        public long planId { get; set; }

        public string mac { get; set; }

        public string device { get; set; }

        public static string GetConfigKey(WifiUser user)
        {
            if (user.storegroupid != 0)
                return CoreCacheHelper.PLAN_MAC_MAPPING + user.mobile.ToUpper() + '_' + 0 + '_' + user.storegroupid;
            else
                return CoreCacheHelper.PLAN_MAC_MAPPING + user.mobile.ToUpper() + '_' + user.nasid + '_' + 0;
        }
    }
}
