using i2e1_core.Utilities;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public enum OperationType
    {
        NO_OPERATION = 0,
        REBOOT,
        RENAME_SSID,
        CHANGE_SSID_PASSWORD,
        CHANGE_MAC_ALLOWED = 4,
        CHANGE_SPLASH_IMAGE,
        ACTIVATE_MONITOR_MODE = 6,
        ACTIVATE_SHINE_PLUS,
        ACTIVATE_BANDWIDTH_MONITORING = 8,
        CUSTOM_OPERATION = 9,
        ACTIVATE_REVERSE_TUNNEL,
        ACTIVATE_CUSTOM_DNS,
        ACTIVATE_NO_INTERNET = 12,
        DOWNLOAD_FILE,
        UPGRADE_ROUTER,
        ACTIVATE_PIN_HUWAEI_AIRTEL = 16,
        ACTIVATE_ONE_NETWORK,
        RESTORE_ROUTER_CONFIG = 18,
        CHANGE_DEVICE_PASSWORD = 19,
        BLOCK_IP_ADDRESSES = 21,
        TOGGLE_WIFI,
        ACTIVATE_DNS_LOGGING,
        ACTIVATE_ROAMING_SSID = 24,
        FLEET_CUSTOM_OPERATION = 25,
        PURGE_MONITOR_MODE = 28,
        WHITELIST_DOMAIN = 29,
        CREATE_IPSET,
        UPDATE_SSID_PASSWORD,
        DEACTIVATE_BANDWIDTH_MONITORING,
        VIKALP_UPGRADE,
        VPN_FIX,
		SPEED_TEST,
		OTP_CONF,
        DISABLE_SS 
    }
    public enum OperationStatus
    {
        PENDING=0,
        SUCCESS=1,
        EXPIRED=2,
        FAILED=3
    }

    [Serializable]
    public class RemoteManagement : NasId
    {
        public RemoteManagement()
        {
            this.hostName = Constants.DOMAIN_FOR_REMOTE;
            this.operationExpiryTime = DateTime.UtcNow.AddDays(1);
        }

        public RemoteManagement(string hostName)
        {
            this.hostName = hostName;
            this.operationExpiryTime = DateTime.UtcNow.AddDays(1);
        }

        public OperationStatus status { get; set; }

        public string version { get; set; }

        public OperationType operationType { get; set; }

        public string operationid { get; set; }

        public string operationTypeText { get; set; }

        public string operationParameter { get; set; }

        public string operationNasids { get; set; }

        public string operationFinishTime { get; set; }
        public DateTime operationPublishTime { get; set; }
        public DateTime operationExpiryTime { get; set; }
        public string hostName { get; set; }
    }
}
