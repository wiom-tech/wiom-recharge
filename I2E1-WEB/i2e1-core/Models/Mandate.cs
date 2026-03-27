using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models
{
    public class Mandate
    {
        public int id { get; set; }
        public LongIdInfo account_id { get; set; }
        public string mandate_id { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public int plan_id { get; set; }
        public DateTime added_time { get; set; }
        public string customer_id { get; set; }
        public DateTime mandate_start_date { get; set; }
        public DateTime mandate_end_date { get; set; }
        public int mandate_amount { get; set; }
        public DateTime last_updated_time { get; set; }
        public string extra_data { get; set; }
    }
    public class MandateStatus
    {
        public static string ACTIVE = "ACTIVE";
        public static string REVOKED = "REVOKED";
        public static string PAUSED = "PAUSED";
    }
}
