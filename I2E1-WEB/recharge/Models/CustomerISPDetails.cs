using System;
using Amazon.DynamoDBv2.DataModel;

namespace recharge.Models
{
    public enum ISPTYPE 
	{ 
		H8,
		IPACCT
	}

	public class ISP_ISPType_Mapping
	{
        public ISPTYPE ispType { get; set; }
		public string ispName {  get; set; }
    }

	public class CustomerISPDetails
	{
		public ISP_ISPType_Mapping ISP_ISPType_Mapping {  get; set; }
		public string networkType { get; set; }
		public string username { get; set; }
		public string staticIP { get; set; }
		public string planName {  get; set; }
		public DateTime expiryTime { get; set; }
	}

	public class ISPRechargeResponse
	{
		public bool success { get; set; }
		public string responseMessage { get; set; }
		public string price { get; set; }
		public string invoice { get; set; }
		public DateTime? newExpiry {  get; set; }
	}
}
