using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models
{
    public enum PaymentType
    {
        WIOM_BOX = 0,
        WIFI_RECHARGE = 1,
        RESELLER_RECHARGE = 2,
        VALUE_ADDED_SERVICE = 3,
        PDO_PLAN_RECHARGE = 4,
        ONBOARDING_SUBSCRIPTION = 5,
        HOME_ROUTER_SUBSCRIPTION = 6,
        HOME_ROUTER_WIOM_MEMBER = 7,
        CUSTOMER_WALLET_RECHARGE = 8,
        BOOKING_HOME_ROUTER = 9,
        DONTKNOW = 100,
    }


    [Serializable]
    public class PlanDetail
    {
        public int planId { get; set; }
        public string planName { get; set; }
        public string description { get; set; }
        public bool device { get; set; }
        public int validity { get; set; }
        public double amount { get; set; }
    }

    [Serializable]
    public class TransactionDetail
    {
        public int plan_id { get; set; }
        public int quantity { get; set; }
        public string cuponCode { get; set; }
        public string extra_parm { get; set; }
        public string gst_no { get; set; }
        public string invoice_no { get; set; }
        public LongIdInfo nasid { get; set; }
        public TransactionAddress deliveryAddress { get; set; }
        public TransactionAddress billingAddress { get; set; }
    }

    [Serializable]
    public class TransactionAddress
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string landmark { get; set; }
        public string pincode { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string email { get; set; }
    }

    public class PaymentStatus
    {
        public const int FAILED = 0;
        public const int SUCCESS = 1;
    }
    public class RefundStatus
    {
        public const int INIT_REFUND = 0;
        public const int SUCCESS_REFUND = 1;
    }
}
