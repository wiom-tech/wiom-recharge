using i2e1_basics.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models
{
    [Serializable]
    public class Schedule
    {
        public bool isScheduled { get; set; }

        public int repeatIntervalInMin { get; set; }

        public DateTime startTime {get;set;}

        public DateTime? endTime { get; set; }
    }
    [Serializable]
    public abstract class Trigger
    {
        public TriggerInfo triggerInfo { get; set; }

        public Trigger()
        {
        }

        public abstract List<TriggerReply> ProcessScheduledTask(params string[] parameters);

        public virtual void PostProcessing(List<TriggerReply> parameters)
        {
        }
    }

    [Serializable]
    public class TriggerInfo
    {
        public int triggerId { get; set; }

        public string triggerName { get; set; }

        public string[] sendEmailTo { get; set; }

        public Schedule scheduleCycle { get; set; }

        public string codePath { get; set; }

        public string[] parameters { get; set; }

        public DateTime? lastRanTime { get; set; }

        public DateTime? lastRanTimeSuccess { get; set; }
    }

    [Serializable]
    public class TriggerDetails
    {
        public int campaign_id { get; set;}

        public int offer_id { get; set; }

        public LongIdInfo adminId { get; set; }

        public LongIdInfo nasid { get; set; }

        public string id { get; set; }

        public int segment { get; set; }

        public int discount { get; set; }

        public string title { get; set; }

        public int validity { get; set; }

        public string content { get; set; }

        public string storeCategory { get; set; }

        public string storeName { get; set; }

        public List<KeyValuePair<string, object>> attributes { get; set; }

        public string senderId { get; set; }
    }

    [Serializable]
    public class TriggerReply
    {
        public bool canceled { get; set; }

        public string mobile { get; set; }

        public LongIdInfo nasid { get; set; }

        public string emailId { get; set; }

        public string trackingId { get; set; }

        public MessageDeliveryType deliveryType { get; set; }

        public TriggerDetails triggerDetails { get; set; }

        public List<KeyValuePair<string, object>> attributes { get; set; }
    }
    [Serializable]
    public class Primal_SMS_Report
    {
        public string reportId { get; set; }

        public string mobile { get; set; }

        public string commId { get; set; }

        public LongIdInfo nasid { get; set; }

        public int smsCategory { get; set; }

        public int segment { get; set; }

        public int discount { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SmsStatus status { get; set; }
    }
}
