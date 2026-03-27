using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;

namespace i2e1_core.Models
{
    public class Coupon
    {
        public int id { get; set; }
        public int amount { get; set; }
        public string name { get; set; }
        public string coupon_type { get; set; }
        public int vaildity_days { get; set; }
        public int discount { get; set; }
        public int frequency { get; set; }
        public string coupon_code { get; set; }
        public Dictionary<string, object> extra_data { get; set; }
    }

    public class CouponInstance : Coupon
    {
        public int blueprint_id { get; set; }

        public LongIdInfo account_id { get; set; }
        public int used_count { get; set; }
        public DateTime issued_time { get; set; }
        public DateTime expiry_time { get; set; }
    }
}
