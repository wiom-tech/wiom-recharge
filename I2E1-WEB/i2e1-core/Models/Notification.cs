using i2e1_basics.Utilities;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models
{
    public enum NotificationType
    {
        INFO = 0,
        WARNING = 1,
        FAILURE = 2,
        BANDWIDTH = 3,
        ROUTER = 4,
        USERS = 5,
        ROUTER_IDLE = 6
    }

    public enum MessageDeliveryType
    {
        EMAIL,
        SMS,
        APP_ALERT,
        REPORT,
        ALL,
        API,
        APP_ALERT_AND_EMAIL,
        BROWSER_NOTIFICATION
    }

    public class Notification
    {
        public string id { get; set; }

        public string deviceId { get; set; }

        public LongIdInfo nasid { get; set; }

        public string username { get; set; }

        public int controllerId { get; set; }

        public string storeName { get; set; }

        public List<string> emailId { get; set; }

        public string[] phoneNumber { get; set; }

        public string title { get; set; }

        public string url { get; set; }

        public Dictionary<String, String> data { get; set; }

        public string message { get; set; }

        public bool isHTML { get; set; }

        public float state { get; set; }

        public string partnerTag { get; set; }

        public NotificationType notificationType { get; set; }

        public MessageDeliveryType messageType { get; set; }

        public DateTime addedTime { get; set; }

        public string source { get; set; }
    }
}
