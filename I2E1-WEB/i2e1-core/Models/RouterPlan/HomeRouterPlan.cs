using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;

namespace i2e1_core.Models.RouterPlan
{
    [DynamoDBTable(TableModelMapping.HOMEROUTERPLANINFO)]
    public sealed class HomeRouterPlan : BaseRouterPlan
    {
        [DynamoDBProperty]
        public string mobile { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBHashKey]
        public LongIdInfo nasId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBGlobalSecondaryIndexHashKey(TableSecondaryIndexMapping.HOMEROUTERPLANINFO_LCOACCOUNTID_PLANSTARTTIME_INDEX)]
        public LongIdInfo lcoAccountId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo accountId { get; set; }
        [DynamoDBProperty]
        public string connection { get; set; }
    }
    public class HOMEOTP
    {
        public const string APP_DOWNLOAD = "APP_DOWNLOAD";
        public const string DONE = "DONE";
        public const string BUFFER = "BUFFER";
        //Associated Primary Plan and secondary plan
        public const string ROAM = "ROAM";
        //Secondary Signal
        public const string PAY_ONLINE = "PAY_ONLINE";
        public const string CASH = "CASH";
        public const string FREE = "FREE";
    }
}
