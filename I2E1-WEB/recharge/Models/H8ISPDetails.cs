using Amazon.DynamoDBv2.DataModel;
using i2e1_basics.Attributes;

namespace recharge.Models
{
    [DynamoDBTable("h8Integration")]
	public class H8ISPDetails
	{
		[DynamoDBProperty]
		[DynamoDBHashKey]
		public string isp_name { get; set; }
		[DynamoDBProperty]
		[Immutable]
		[DynamoDBRangeKey]
		public string api_type { get; set; }
		[DynamoDBProperty]
		public string url { get; set; }
		[DynamoDBProperty]
		public string systemId { get; set; }
		[DynamoDBProperty]
		public string password { get; set; }
	}
}
