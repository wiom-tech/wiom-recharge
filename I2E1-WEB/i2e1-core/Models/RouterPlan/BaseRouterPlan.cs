using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.Attributes;
using i2e1_basics.DynamoUtilities;
using System;

namespace i2e1_core.Models.RouterPlan
{
    public class BaseRouterPlan
    {
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime planStartTime { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime planEndTime { get; set; }
        [DynamoDBProperty]
        public long dataLimit { get; set; }
        [DynamoDBProperty]
        public int charges { get; set; }
        [DynamoDBProperty]
        public byte deviceLimit { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime createdTime { get; set; } = DateTime.UtcNow;
        [DynamoDBProperty]
        public long planId { get; set; }
        [DynamoDBProperty]
        public int totalPaid { get; set; }
        [DynamoDBProperty]
        public string transactionId { get; set; }
        [DynamoDBProperty]
        public string otp { get; set; }
        [DynamoDBProperty]
        public int planAmount { get; set; }
        [DynamoDBProperty]
        public long timePlan { get; set; }
        [DynamoDBProperty]
        public string source { get; set; }
        [DynamoDBProperty]
        public string paymentMode { get; set; }
        [DynamoDBProperty]
        public int speedMbps { get; set; }
        [DynamoDBProperty]
        public int cashFee { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime firstRechargeTime { get; set; }
        [DynamoDBProperty]
        [Immutable]
        [DynamoDBRangeKey]
        public long entryUnixEpochTime { get; set; } = System.DateTime.UtcNow.Ticks;
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        [Immutable]
        public DateTime entryDate { get; set; } = DateTime.UtcNow.Date;
        [DynamoDBProperty]
        public long planIncreasedInSec { get; set; }
    }

    
}
