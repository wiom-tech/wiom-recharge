using System;
using System.Collections.Generic;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;

namespace i2e1_core.Models
{
    [Serializable]
    public class UserDataUsage : UserBaseModel
    {
        public long dataUpload { get; set; }

        public long dataDownload { get; set; }

        public long fdmId { get; set; }

        public string otp { get; set; }

        public int days { get; set; }

        public bool isOnline { get; set; }

        public DateTime sessionStart { get; set; }

        public DateTime sessionEnd { get; set; }

        public string macId { get; set; }

        public List<LongIdInfo> nasids { get; set; }
    }
}