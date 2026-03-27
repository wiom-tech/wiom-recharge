using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;

namespace i2e1_core.Models.Payment
{
    [DynamoDBTable(TableModelMapping.WIOMBILLINGWIFI)]
    public class MCustomerBooking : BaseWiomBilling
    {
        [DynamoDBProperty]
        public long leadId { get; set; }

        [DynamoDBProperty]
        public string paymentKey { get; set; }
    }
}
