using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models
{
    public class RazorpayAccountMapping
    {
        public LongIdInfo accountId { get; set; }
        public string razorPayAccountId { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }

        public string contactId { get; set; }
        public string ifscCode { get; set; }
        public DateTime added_time { get; set; }
        public DateTime modified_time { get; set; }
        public string extraData { get; set; }
    }
}
