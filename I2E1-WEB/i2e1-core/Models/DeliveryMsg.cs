using i2e1_basics.Utilities;
using System.Collections.Generic;

namespace i2e1_core.Models
{
    public class DeliveryMsg
    {
        public List<string> numbers;
        public List<LongIdInfo> userIds;
        public List<string> emails;
        public KeyValuePair<string, List<LongIdInfo>> accountIds;
        public Dictionary<string, string> msg;
        public MsgType msgType;

        public enum MsgType
        {
            NOTIFICATION = 0,
            SMS,
            EMAIL,
            WHATSAPP_CUSTOMER,
            WHATSAPP_PARTNER
        }
    }
}
