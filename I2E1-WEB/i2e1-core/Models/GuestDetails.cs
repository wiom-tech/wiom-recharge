using System;

namespace i2e1_core.Models
{
    [Serializable]
    public enum OpCode
    {
        Checkin,
        Checkout,
        ChangeField,
        Transfer,
        Sync,
        ManualCheckin
    }

    [Serializable]
    public enum dataPlan
    {
        
        Default = 0 ,
        Plan1,
        Plan2,
        Plan3,
        Plan4
    }

    [Serializable]
    public class GuestDetails
    {
        public string mobile { get; set; }

        public int nasid { get; set; }

        public string guestName { get; set; }

        public string roomNo { get; set; }

        public string oldRoomNo { get; set; }

        public int speed { get; set; }

        public int duration { get; set; }

        public int charges { get; set; }

        public bool status { get; set; }

        public string registrationNo { get; set; }

        public string planid { get; set; }

        public bool guestShare { get; set; }

        public OpCode action { get; set; }

        public dataPlan dataPlan { get; set; }

        public string GenerateMobile()
        {
            return guestName + "@" + roomNo;
        }
    }
}