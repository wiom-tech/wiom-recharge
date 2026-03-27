using i2e1_basics.Utilities;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public class StoreOperatingHours
    {
        public Slots[] slots { get; set; }
    }
    [Serializable]
    public class Slots
    {
        public string openTime { get; set; } //stored as minute of the day

        public string closeTime { get; set; } //stored as minute of the day
    }
    [Serializable]
    public class StoreOperatingDetails
    {
        public LongIdInfo nasid { get; set; }

        public bool open24Hours { get; set; }

        public StoreOperatingHours[] operatingHours { get; set; }

        public StoreOperatingDetails()
        {
            this.open24Hours = true;
        }
    }
}
