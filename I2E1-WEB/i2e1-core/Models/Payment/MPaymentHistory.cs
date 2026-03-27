using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class MPaymentHistory: BaseWiomBilling
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
        public bool? wiomMember { get; set; }
        [DynamoDBProperty]
        public bool? subscription { get; set; }
        [DynamoDBProperty]
        public double discount { get; set; }
        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBGlobalSecondaryIndexHashKey(TableSecondaryIndexMapping.WIOMBILLINGWIFI_NASID)]
        public LongIdInfo nasid { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? firstRechargeTime { get; set; }
        [DynamoDBProperty]
        public long time_limit { get; set; }
        [DynamoDBProperty]
        public bool? doReset { get; set; }
        [DynamoDBProperty]
        public string mandateId { get; set; }
        [DynamoDBProperty]
        public long[] couponIds { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? membershipStartTime { get; set; }
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
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? planStartTime { get; set; }
        [DynamoDBProperty]
        public long revenueId { get; set; }
        [DynamoDBProperty]
        public bool? advancePay { get; set; }
        [DynamoDBProperty]
        public string connection { get; set; }
        [DynamoDBProperty]
        public string planName { get; set; }
        [DynamoDBProperty]
        public int billId { get; set; }
        [DynamoDBProperty]
        public string notificationStatus { get; set; }
        [DynamoDBProperty]
        public string paymentKey { get; set; }
        public string getAddressString()
        {
            string addresStr = deliveryAddress.address + ", " + deliveryAddress.landmark +
                               ", " + deliveryAddress.city + ", " + deliveryAddress.state + ", " + deliveryAddress.pincode;
            return addresStr;
        }
    }
}
