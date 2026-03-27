using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using System;
using System.Collections.Generic;

namespace i2e1_core.Models
{
    [Serializable]
    public enum DeviceMode
    {
        INITIAL = 0,
        STANDARD,
        MULTIWAN,
        MULTIWAN_CHILLI,
        DONGLE_HUWAEI,
        DONGLE_AIRTEL = 4,
        DONGLE_HUWAEI_MULTIWAN,
        DONGLE_AIRTEL_MULTIWAN
    }
    [Serializable]
    public enum DeviceRole
    {
        CONTROLLER = 0,
        AP,
        BOTH
    }

    public class DeviceSsid
    {
        public string ssid { get; set; }
        public string password { get; set; }
        public string mode { get; set; }
        public string radio { get; set; }
    }

    [Serializable]
    public class DeviceConfig
    {

        public LongIdInfo nasid { get; set; }

        public LongIdInfo secondaryNasid { get; set; }

        private string pMacId;

        private string pMacId2;

        public int page { get; set; }

        public bool isOnPortal { get; set; }

        public string macid { get { return pMacId; } set { pMacId = CoreUtil.GetNormalisedMac(value); } }

        public string macid2 { get { return pMacId2; } set { pMacId2 = CoreUtil.GetNormalisedMac(value); } }

        public string ssid { get; set; }

        public string monitormode { get; set; }

        public string parameters { get; set; } //the idea is certain parameters can be passed by a device . generic variable as of now so dont remove please

        public string deviceName { get; set; }

        public string devicePassword { get; set; }

        public string devicePasswordNew { get; set; }

        public string deviceMode { get; set; }

        public string deviceType { get; set; }

        public DateTime addedtime { get; set; }

        public DateTime modtime { get; set; }

        public DeviceRole deviceRole { get; set; }

        public string accessPoint { get; set; }

        public string accessPointPassword { get; set; }

        public string accessPointPasswordNew { get; set; }

        public string version { get; set; }

        public string defaultPass { get; set; }

        public string splashImage { get; set; }

        public string process { get; set; }

        public string splashText { get; set; }

        public bool status { get; set; }

        public int productId { get; set; }

        public int channelType { get; set; }

        public string channelName { get; set; }
        public List<RouterConfigState> deviceState {get;set;}

        public string firmwareVersion { get; set; }

        public string gatewayInfo { get; set; }
    }

    public class RouterConfigState
    {
        public OperationType operationType { get; set; }

        public string operationid { get; set; }

        public string configName { get; set; }

        public Dictionary<string, object> configs { get; set; }
    }


    public class Ipset
    {
        public int id { get; set; }
        public string ipsetname { get; set; }
        public string domains { get; set; }
    }

    public class Speedtest
    {
        public string result { get; set; }

        public LongIdInfo nasid { get; set; }

        public string download_bw { get; set; }

        public string operation_id { get; set; }

        public string timestamp { get; set; }
    }
}