using i2e1_basics.Models;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using System;
using System.Net;

namespace i2e1_core.Utilities
{
    public class RemoteUtils
    {
        public static void sendoperationtonewdevice(LongIdInfo longNasid, OperationType routeroperation, string parameter = "", string hostname = "")
        {
            RemoteManagement remote = new RemoteManagement(hostname);
            remote.nasid = longNasid.GetLongId().ToString();
            remote.operationType = routeroperation;
            remote.operationParameter = parameter;
            HandleRemoteOperation(remote);
        }
        public static string SubmitOperation(RemoteManagement remote, NasSplitTemplate deviceConfig, bool enableMQTT = false)
        {
            string operationId = CoreDbCalls.GetInstance().SubmitBasicOp(remote, deviceConfig.nasid, null);
            if (enableMQTT && !string.IsNullOrEmpty(deviceConfig.mac))
            {
                if(deviceConfig.deviceId.ToUpper().StartsWith("SY"))
                    MQTTManager.Publish(deviceConfig.mac.ToLower(), "/usr/sbin/wiom/remote", 24 * 60 * 60);
                else if (deviceConfig.deviceId.ToUpper().StartsWith("GX"))
                    MQTTManager.PublishSSL(deviceConfig.mac.ToLower(), "/usr/sbin/wiom/remote", 24 * 60 * 60);
                else
                    MQTTManager.Publish(deviceConfig.mac.ToLower(), "/etc/config/remote.sh", 24 * 60 * 60);
            }
            return operationId;
        }

        public static void HandleRemoteOperation(RemoteManagement remote)
        {
            bool operationSubmitted = false;
            bool enableMQTT = true;
            var deviceConfig = ShardHelper.GetNasDetailsFromLongNas(remote.backEndNasid);
            switch (remote.operationType)
            {
                case OperationType.ACTIVATE_BANDWIDTH_MONITORING:
                    if (!string.IsNullOrEmpty(remote.operationNasids))
                    {
                        remote.operationParameter = string.Empty;
                        DateTime current = DateTime.UtcNow;
                        remote.operationExpiryTime = current;

                        var nasIds = remote.operationNasids.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < nasIds.Length; ++i)
                        {
                            int numericNas = int.Parse(nasIds[i].Trim());
                            using (var client = new WebClient())
                            {
                                client.DownloadString($"http://remote.i2e1.in/Remote/SendSpeedTestCommand.php?nasid={numericNas}&controllerId=1&version=95");
                            }
                        }
                    }
                    operationSubmitted = true;
                    break;
                case OperationType.UPGRADE_ROUTER:
                    if (!string.IsNullOrEmpty(remote.operationNasids))
                    {
                        var nasIds = remote.operationNasids.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nasIds.Length <= 200)
                        {
                            for (int i = 0; i < nasIds.Length; ++i)
                            {
                                deviceConfig.nasid = LongIdInfo.IdParser(long.Parse(nasIds[i].Trim()));
                                SubmitOperation(remote, deviceConfig, enableMQTT);
                            }
                        }
                        operationSubmitted = true;
                    }
                    break;
                case OperationType.CREATE_IPSET:
                    remote.operationType = OperationType.CREATE_IPSET;
                    break;
                case OperationType.UPDATE_SSID_PASSWORD:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.RENAME_SSID:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    if (!string.IsNullOrEmpty(remote.operationNasids))
                    {
                        var nasids = remote.operationNasids.Trim();
                        foreach (var nas in nasids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            deviceConfig.nasid = LongIdInfo.IdParser(long.Parse(nas));
                            SubmitOperation(remote, deviceConfig, false);
                        }
                    }
                    break;
                case OperationType.CHANGE_DEVICE_PASSWORD:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.CHANGE_SSID_PASSWORD:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.ACTIVATE_REVERSE_TUNNEL:
                    int port = 12345;
                    if (port == 0)
                    {
                        remote.operationParameter = "No Free Port Available";
                    }
                    else
                    {
                        remote.operationParameter = port.ToString();
                    }
                    break;
                case OperationType.ACTIVATE_ROAMING_SSID:
                    if (!string.IsNullOrEmpty(remote.operationParameter))
                    {
                        var tempparameter = remote.operationParameter;
                        remote.operationParameter = string.Empty;
                        foreach (var entry in tempparameter.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            CoreDbCalls.GetInstance().SubmitBasicOp(remote, LongIdInfo.IdParser(long.Parse(entry.Trim())), null);
                        }
                    }
                    return;
                case OperationType.FLEET_CUSTOM_OPERATION:
                    enableMQTT = false;
                    if (!string.IsNullOrEmpty(remote.operationParameter) && !string.IsNullOrEmpty(remote.operationNasids))
                    {
                        var nasids = remote.operationNasids;
                        remote.operationParameter = string.Empty;
                        foreach (var nas in nasids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            deviceConfig.nasid = LongIdInfo.IdParser(long.Parse(nas.Trim()));
                            SubmitOperation(remote, deviceConfig, enableMQTT);
                        }
                    }
                    return;
                case OperationType.RESTORE_ROUTER_CONFIG:
                    remote.operationParameter = remote.backEndNasid + "_" + 1 + "_" + remote.operationParameter;
                    break;
                case OperationType.VIKALP_UPGRADE:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.VPN_FIX:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.OTP_CONF:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
                case OperationType.DISABLE_SS:
                    remote.operationExpiryTime = DateTime.UtcNow.AddDays(15);
                    break;
            }
            if (!operationSubmitted)
                SubmitOperation(remote, deviceConfig, enableMQTT);
        }
    }
}
