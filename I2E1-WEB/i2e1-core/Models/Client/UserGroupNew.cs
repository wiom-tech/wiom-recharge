using i2e1_basics.Utilities;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models.Client
{
    [Serializable]
    public class UserGroupNew
    {
        public string groupName { get; set; }
        public int groupId { get; set; }
        public LongIdInfo adminId { get; set; }
        public string values { get; set; }
        public Dictionary<string, BasicConfig> basicConfigs { get; set; }

        public UserGroupNew(int groupId, string groupName, string values, LongIdInfo adminId)
        {
            this.groupId = groupId;
            this.adminId = adminId;
            this.groupName = groupName;
            this.values = values;
        }

        public UserGroupNew()
        {

        }
    }
}
