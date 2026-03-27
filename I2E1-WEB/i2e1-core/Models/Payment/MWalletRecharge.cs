using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class MWalletRecharge : BaseWiomBilling
    {
        [DynamoDBProperty]
        public long planId { get; set; }
        [DynamoDBProperty]
        public long leadId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo accountId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo userId { get; set; }
        [DynamoDBProperty]
        public long settingId { get; set; }
        [DynamoDBProperty]
        public bool wiomMember { get; set; }
        [DynamoDBProperty]
        public bool subscription { get; set; }
    }
}
