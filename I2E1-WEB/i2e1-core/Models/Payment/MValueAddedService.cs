using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class MValueAddedService : BaseWiomBilling
    {
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo accountId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBGlobalSecondaryIndexHashKey(TableSecondaryIndexMapping.WIOMBILLINGWIFI_NASID)]
        public LongIdInfo nasid { get; set; }
        [DynamoDBProperty]
        public long couponId { get; set; }
        [DynamoDBProperty]
        public string mandateId { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? membershipStartTime { get; set; }
        [DynamoDBProperty]
        public bool? wiomMember { get; set; }
    }
}
