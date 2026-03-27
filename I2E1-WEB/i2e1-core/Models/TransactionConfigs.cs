using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.DynamoUtilities;
using i2e1_basics.Utilities;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models
{

    [DynamoDBTable(TableModelMapping.TRANSACTIONS)]
    public sealed class Transactions
    {
        internal List<Transactions> list;
        internal List<Transactions> filteredTransactions;

        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        [DynamoDBHashKey]
        public LongIdInfo account_id { get; set; }

        [DynamoDBProperty]
        public long id { get; set; }

        [DynamoDBProperty]
        public string? transaction_type { get; set; }

        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo parent_account_id { get; set; }

        [DynamoDBProperty]
        public string? account_role_type { get; set; }

        [DynamoDBProperty]
        public int? task_id { get; set; }

        [DynamoDBProperty(typeof(LongIdInfoConverter))]
        public LongIdInfo user_id { get; set; }

        [DynamoDBProperty]
        public decimal? amount { get; set; }

      
        [DynamoDBProperty]
        public bool? claimed_status { get; set; }

        [DynamoDBProperty]
        public bool? locked_status { get; set; }

        [DynamoDBProperty]
        public bool? approval_status { get; set; } //will use for rohit wallet approval

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? claimed_date { get; set; }

        [DynamoDBProperty]
        public string? claimed_transaction_id { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? expire_date { get; set; }

        [DynamoDBProperty]
        public string? extra_data { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? created { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? modified { get; set; }

        public decimal TotalUnclaimedAmount { get; set; }
        public Dictionary<DateTime, GroupedTransactionData> groupedTransactions { get; set; }

        public Dictionary<DateTime, decimal> claimedTransactionsGrouped { get; internal set; }

        public Dictionary<DateTime, GroupedTransactionData> TotalUnclaimedAmountList { get; set; }

       
        public class GroupedTransactionData
        {
            public decimal? TotalAmount { get; set; }
            public List<Transactions>? Records { get; set; }

            internal bool Any()
            {
                throw new NotImplementedException();
            }
        }

        public Transactions()
        {
            // Generate a unique ID for the id property
            id = GenerateUniqueId();
        }

        private int GenerateUniqueId()
        {
            // Your logic for generating a unique ID (e.g., timestamp + random number)
            // In this example, I'm using a combination of ticks from DateTime and a random number
            Random random = new Random();
            return Math.Abs((int)(DateTime.UtcNow.Ticks + random.Next(1, 1000)));
        }
    }
}
