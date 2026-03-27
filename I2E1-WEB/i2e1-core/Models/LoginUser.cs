using System;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;

namespace i2e1_core.Models
{
    [Serializable]
    public class LoginUser
    {
        public LongIdInfo id { get; set; }

        private string pToken;
        public string mobile { get; set; }
        public string name { get; set; }
        public string otp { get; set; }
        public string token
        {
            get
            {
                if (pToken == null || pToken.Contains("$") || id == null)
                    return pToken;
                return id.shard_id + "$" + pToken;
            }
            set
            {
                pToken = value;
            }
        }
        public string fcmToken { get; set; }
    }
}
