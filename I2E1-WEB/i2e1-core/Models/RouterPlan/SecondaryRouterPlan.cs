using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;

namespace i2e1_core.Models.RouterPlan
{
    [DynamoDBTable(TableModelMapping.SECONDARYSIGNALROUTERPLANINFO)]
    public sealed class SecondaryRouterPlan : BaseRouterPlan
    {
        [DynamoDBProperty]
        [DynamoDBHashKey]
        public string mobile { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo nasId { get; set; }
        [DynamoDBProperty]
        public bool authState { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo lcoAccountId { get; set; }
        [DynamoDBProperty]
        public string status { get; set; }
        [DynamoDBProperty]
        public string connection { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo longSecondaryNas { get; set; }
    }
}
