using i2e1_basics.Models;
using i2e1_basics.Utilities;
using i2e1_core.Models.WIOM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models.Client
{
    [Serializable]
    public enum RouterState
    {
        NOT_INSTALLED = 0,
        INSTALLED = 1,
        CLOSED = 2,
        IN_TRANSIT = 3,
        NOT_REGISTERED = 5
    }

    [Serializable]
    public class StorePublicDetails : Store
    {
        public string location { get; set; }

        public string partner { get; set; }

        public string emailId { get; set; }

        public int internetBillingStartDay { get; set; }

        public long internetPlan { get; set; }

        public int uploadKpbs { get; set; }

        public int downloadKbps { get; set; }

    }

    [Serializable]
    public class StoreDetails : StorePublicDetails
    {
        public String deviceId { get; set; }

        public LongIdInfo mmNasId { get; set; }

        public double lastPingDelay { get; set; }

        public bool active { get; set; }

        public RouterState routerState { get; set; }
    }
}