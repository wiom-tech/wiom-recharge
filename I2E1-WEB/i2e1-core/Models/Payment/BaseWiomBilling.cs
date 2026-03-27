using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.Attributes;
using i2e1_basics.DynamoUtilities;
using i2e1_core.Attributes;
using i2e1_core.DynamoUtilities;
using System;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class BaseWiomBilling
    {
        [DynamoDBHashKey]
        public string mobile { get; set; }
        [DynamoDBRangeKey]
        [DynamoDBProperty]
        public string transactionId { get; set; }
        [DynamoDBProperty]
        public double payableAmount { get; set; }
        [DynamoDBProperty]
        public string source { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime createDate { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? updateDate { get; set; } = DateTime.UtcNow;
        [DynamoDBProperty]
        public int? paymentStatus { get; set; }
        [DynamoDBProperty]
        public string txnStatus { get; set; }
        [DynamoDBProperty]
        public string responseMsg { get; set; }
        [DynamoDBProperty(typeof(PaymentTypeConverter))]
        public PaymentType paymentType { get; set; }
        [DynamoDBProperty]
        public string orderId { get; set; }
        [DynamoDBProperty]
        public string extraParam { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        [ImmutableAttribute]
        public DateTime entryDate { get; set; } = DateTime.UtcNow.Date;
        [DynamoDBProperty]
        public string webLink { get; set; }
        [DynamoDBProperty]
        public string returnUrl { get; set; }
        [DynamoDBProperty]
        public int? refundStatus { get; set; }
        [DynamoDBProperty]
        public bool? webPaymentFlow { get; set; }

    }
    public class TransactionId
    {
        public string source { get; set; }
        public string mobile { get; set; }
        public int timeSecond { get; set; }
        public int shardId { get; set; }
        public string transactionId { get; set; }
        public string getTransactionId(string source, string mobile, int shardId)
        {
            this.timeSecond = (int)(DateTime.UtcNow - new DateTime(2020, 1, 1)).TotalSeconds;
            this.source = source;
            this.mobile = mobile;
            this.shardId = shardId;
            this.transactionId = this.source + "_" + this.mobile + "_" + this.timeSecond + "_" + this.shardId.ToString();
            return this.transactionId;
        }
        public static TransactionId GetInstance(string transactionid)
        {
            if (transactionid == null)
                return null;
            string[] parts = transactionid.Split('_');

            TransactionId transactionId = new TransactionId
            {
                source = parts[0],
                mobile = parts[1],
                timeSecond = int.Parse(parts[2]),
                shardId = int.Parse(parts[3])
            };
            transactionId.transactionId = transactionid;
            return transactionId;
        }
    }
}
