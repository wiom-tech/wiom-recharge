using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class MWifiBox : BaseWiomBilling
    {
        [DynamoDBProperty]
        public double discount { get; set; }
        [DynamoDBProperty]
        public long planId { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBGlobalSecondaryIndexHashKey(TableSecondaryIndexMapping.WIOMBILLINGWIFI_NASID)]
        public LongIdInfo nasid { get; set; }
        [DynamoDBProperty]
        public int quantity { get; set; }
        [DynamoDBProperty]
        public TransactionAddress billingAddress { get; set; }
        [DynamoDBProperty]
        public TransactionAddress deliveryAddress { get; set; }
        [DynamoDBProperty]
        public string webLink { get; set; }
        [DynamoDBProperty]
        public string invoice { get; set; }
        [DynamoDBProperty]
        public string gst { get; set; }
        [DynamoDBProperty]
        public string couponCode { get; set; }
        public string getAddressString()
        {
            string addresStr = deliveryAddress.address + ", " + deliveryAddress.landmark +
                               ", " + deliveryAddress.city + ", " + deliveryAddress.state + ", " + deliveryAddress.pincode;
            return addresStr;
        }
    }
}
