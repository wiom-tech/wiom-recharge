using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i2e1_login.Models
{
    public enum pushSource
    {
        WIOM_DASHBOARD = 1
    }

    public enum pushType
    {
        NEW_CONNECTION = 100
    }

    public enum pushSender
    {
        WIOM_BACKEND = 1
    }

    public class PushNotification
    {
        public int transaction_id { get; set; }
        public pushSource source { get; set; }
        public pushType type { get; set; }
        public pushSender sender { get; set; }
        public string recipientId { get; set; }
        public int recipientSubid { get; set; }
        public string recipientToken { get; set; }
        public string payload { get; set; }
        public bool clicked { get; set; }
        public bool completed { get; set; }
        public bool viewed { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public DateTime sentAt { get; set; }
        public DateTime expireAt { get; set; }
        public int irrelevantIn { get; set; }
        public DateTime insertedAt { get; set; }
        public string delivery_response { get; set; }
    }
}
