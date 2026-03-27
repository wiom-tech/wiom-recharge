using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;
using i2e1_core.Models.WIOM;
using i2e1_core.Utilities;
using System;

namespace i2e1_core.Models
{
    [DynamoDBTable(TableModelMapping.BOOKING)]
    public class MBooking
    {
        [DynamoDBHashKey]
        public string mobile { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime addedTime { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime modifiedTime { get; set; }

        [DynamoDBProperty]
        public long task_id { get; set; }

        [DynamoDBProperty]
        public string name { get; set; }

        [DynamoDBProperty]
        public int status { get; set; }

        [DynamoDBProperty]
        public string address { get; set; }

        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo? googleAddressId { get; set; }

        [DynamoDBProperty]
        public int? bookingFee { get; set; }

        [DynamoDBProperty]
        public int bookingPayment { get; set; }

        [DynamoDBProperty]
        public int installationPayment { get; set; }

        [DynamoDBProperty]
        public int planId { get; set; }

        [DynamoDBProperty]
        public int settingId { get; set; }

        [DynamoDBProperty]
        public string ssid { get; set; }

        [DynamoDBProperty]
        public string wifiPass { get; set; }

        [DynamoDBProperty]
        public bool subscription { get; set; }

        [DynamoDBProperty]
        public string installationStep { get; set; }

        [DynamoDBProperty]
        public int paymentStatus { get; set; } = 0;

        [DynamoDBProperty]
        public string leadType { get; set; }
        [DynamoDBProperty]
        public string otp { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime pref_inst_date { get; set; }

        [DynamoDBProperty]
        public bool hasPaidOnline { get; set; }

        [DynamoDBProperty]
        public bool wiomMember { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime bookingFeeRefundedDate { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime cancellationBookingDate { get; set; }
        [DynamoDBProperty]
        public string cancelReason { get; set; }
    }
}
