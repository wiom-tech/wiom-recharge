using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public enum IntegrationType
    {
        iframe_normal = 0
    }

    [Serializable]
    public enum languageList
    {
        en = 0,
        hi,
        gu,
        mr,
        bn
    }

    [Serializable]
    public enum PaymentCurrencyType
    {
        INR = 0,
        USD,
        EUR,
        GBP,
        SGD
    }

    [Serializable]
    public enum PaymentMode
    {
        ONLINE = 0,
        CHEQUE = 1,
        DEMAND_DRAFT = 2,
        LCO
    }

    [Serializable]
    public class PaymentObject
    {
        public PaymentObject(){
            billing_country = "India";
            billing_tel = "7829599988";
            billing_name = "omnia";
            billing_adress = "dummy";
            billing_city = "New Delhi";
            billing_state = "Delhi";
            billing_zip = "110016";
            billing_email = "dummy@dummy.in";
        }
        public string name { get; set; }

        public LongIdInfo nasid { get; set; }

        public string merchant_id { get; set; }

        public string tid { get; set; }

        public LongIdInfo admin_user_id { get; set; }

        public PaymentMode paymentMode { get; set; }

        public long invoice_id { get; set; }

        public long order_id { get; set; }

        public string amount { get; set; }

        public string remark { get; set; }

        public PaymentCurrencyType currency {get;set;}

        public string redirect_url { get; set; }

        public string cancel_url { get; set; }

        public IntegrationType integration_type { get; set; }

        public languageList language { get; set; }

        public string billing_country { get;set; }

        public string billing_tel {get;set;}

        public string billing_name { get; set; }

        public string billing_adress {get;set;}

        public string billing_city {get;set;}

        public string billing_state {get;set;}

        public string billing_zip {get;set;}

        public string billing_email {get;set;}
    }
    public class PilotCashLoan
    {
        public LongIdInfo accountId { get; set; }
        public int cashAmount { get; set; }
        public int id { get; set; }
        public int period { get; set; }

        public double recurringDeduct { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }


}