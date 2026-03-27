using i2e1_core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace i2e1_core.Models
{
    public class PartitionKey
    {
        public string Key { get; set; }

        public static DateTime PartitionStart = new DateTime(2020, 02, 01);

        public static PartitionKey CURRENT_MONTH
        {
            get
            {
                var time = CoreUtil.ConvertUtcToIST(DateTime.UtcNow);
                return new PartitionKey() { Key = "_" + time.Year + "_" + time.Month };
            }
        }
        public static PartitionKey PREVIOUS_MONTH
        {
            get
            {
                var time = CoreUtil.ConvertUtcToIST(DateTime.UtcNow.AddMonths(-1));
                return new PartitionKey() { Key = "_" + time.Year + "_" + time.Month };
            }
        }

        public static PartitionKey NONE
        {
            get
            {

                return new PartitionKey() { Key = ""};
            }
        }

        public static bool operator ==(PartitionKey key1, PartitionKey key2)
        {
            return key1.Key == key2.Key;
        }

        public static bool operator !=(PartitionKey key1, PartitionKey key2)
        {
            return key1.Key != key2.Key;
        }
    }

}
