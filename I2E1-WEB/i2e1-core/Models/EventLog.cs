using System;

namespace i2e1_core.Models
{
    [Serializable]
    public enum LogType
    {
        INFO = 0,
        WARNING,
        ERROR,
        DEBUG
    }
    [Serializable]
    public class EventLog
    {
        private static int sequencer = 0;
        public EventLog(int nasid,string macId, string mobile, string eventSource, string sessionId, string eventType, string response)
        {
            this.nasid = nasid;
            this.eventSource = eventSource;
            this.mobile = mobile;
            this.macId = macId;
            this.sessionId = sessionId;
            this.eventType = eventType;
            this.response = response;
        }

        public string mobile { get; set; }

        public int nasid { get; set; }

        public string macId { get; set; }

        public string sessionId { get; set; }

        public string eventSource { get; set; }

        public string eventType { get; set; }

        public string response { get; set; }
    }
}
