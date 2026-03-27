using i2e1_core.Utilities;
using System;

namespace i2e1_core.Models.WIOM
{
    [Serializable]
    public class PDOPlan
    {
        // public long id { get; set; }
        // public int combined_setting_id { get; set; }
        // public string name { get; set; }
        public long time_limit {get; set;}
        public long data_limit { get; set; }
        public int speed_limit_mbps { get; set; }
        public double price { get; set; }
        public int discount { get; set; }
        // public bool active { get; set; }
        // public int concurrent_devices { get; set; }
        // public double costPrice { get; set; }

        public int GetPriceWithoutDiscount()
        {
            return (int)this.price - this.discount;
        }

        public string DataLimitToString(string currLang = "en")
        {
            if (data_limit == 0)
            {
                if (currLang == "hi")
                    return "अनलिमिटेड डाटा";
                else
                    return "Unlimited Data";
            }

            string dU = "GB";
            long dD = Constants.ONE_MB;
            if (data_limit < 1024 && data_limit >= 1)
            {
                dU = "MB";
                dD = 1;
            }
            else if (data_limit >= 1024)
            {
                dU = "GB";
                dD = Constants.ONE_KB;
            }

            return (data_limit / dD) + dU;
        }

        public string TimeLimitToString()
        {
            string tU = "Day";
            int tD = Constants.SECONDS_IN_DAY;

            if (time_limit >= Constants.SECONDS_IN_DAY)
            {
                tU = "Day";
                tD = Constants.SECONDS_IN_DAY;
            }
            else if (time_limit < Constants.SECONDS_IN_DAY && time_limit >= 3600)
            {
                tU = "Hour";
                tD = 3600;

            }
            else if (time_limit < 3600)
            {
                tU = "Min";
                tD = 60;
            }

            return (time_limit / tD) + tU;
        }

        public string TimeLimitToStringInHour(string lang)
        {
            string tU = lang == "en" ? "Hour" : "घंटे";
            int tD = 3600;

            return (time_limit / tD) + " " +  tU;
        }

        public string GetSpeedRange()
        {
            switch (this.speed_limit_mbps)
            {
                case 100:
                    return "80-100 Mbps";
                case 50:
                    return "40-50 Mbps";
                default:
                    return "25-30 Mbps";

            }
        }

        public bool IsLongTermPlan()
        {
            return time_limit > Constants.SECONDS_IN_DAY * 30;
        }
    }
}