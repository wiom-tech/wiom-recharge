using i2e1_basics.Database;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OperationType = i2e1_core.Models.OperationType;

namespace i2e1_core.Utilities
{
    public class CoreDbCalls
    {
        private static CoreDbCalls dbCalls = null;

        private CoreDbCalls()
        {
        }

        public static CoreDbCalls CreateInstance()
        {
            if (dbCalls == null)
            {
                dbCalls = new CoreDbCalls();
            }
            return dbCalls;
        }

        public static CoreDbCalls GetInstance()
        {
            return dbCalls;
        }

        public KeyValuePair<int, bool> GetStoreGroupAndIsAccessCodeApplied(LongIdInfo longNasid)
        {
            return new ShardQueryExecutor<KeyValuePair<int, bool>>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("select single_operation_id, parameters from t_single_nas_operations where single_operation_id in (16, 17) and nas_id=@nasid");
                cmd.Parameters.Add(new SqlParameter("@nasid", longNasid.local_value));
                res = ResponseType.READER;
                return cmd;
            }), longNasid.shard_id,
            new ResponseHandler<KeyValuePair<int, bool>>((reader) =>
            {
                int storeGroup = 0;
                bool askAccessCode = false;
                while (reader.Read())
                {
                    int operationId = (int)reader["single_operation_id"];
                    string result = reader["parameters"].ToString();
                    if (!String.IsNullOrEmpty(result))
                    {
                        if (operationId == 16)
                        {
                            String[] parameters = result.Split(new String[] { "," }, StringSplitOptions.None);
                            if (parameters.Length >= 9 && parameters[8] == "True") askAccessCode = true;
                        }
                        else if (operationId == 17)
                        {
                            storeGroup = int.Parse(result);
                        }
                    }
                }
                return new KeyValuePair<int, bool>(storeGroup, askAccessCode);
            })).Execute();
        }

        public string GetSingleNasOperation(LongIdInfo longNasId, int singleNasOperationId)
        {
            return new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("select parameters from t_single_nas_operations where single_operation_id = @operation_id and nas_id = @nas");

                cmd.Parameters.Add(new SqlParameter("@operation_id", singleNasOperationId));
                cmd.Parameters.Add(new SqlParameter("@nas", longNasId.local_value));
                res = ResponseType.READER;
                return cmd;
            }), longNasId.shard_id,
            new ResponseHandler<string>((reader) =>
            {
                string result = "";
                if (reader.Read())
                {
                    result = reader["parameters"].ToString();
                }
                return result;

            })).Execute();
        }

        public int GetCombinedSettingIdOnNas(LongIdInfo longNasid)
        {
            return new ShardQueryExecutor<int>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("select combined_setting_id from t_combined_setting_nas_mapping where nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@nasid", longNasid.local_value));
                res = ResponseType.READER;
                return cmd;
            }), longNasid.shard_id,
            new ResponseHandler<int>((reader) =>
            {
                if (reader.Read())
                {
                    return (int)reader["combined_setting_id"];
                }
                return 0;
            })).Execute();
        }

        public string UpdateRouterPing(RemoteManagement remote, int wifiCount, string process)
        {
            long shardId = LongIdInfo.IdParser(Convert.ToInt64(remote.nasid)).shard_id;
            //string[] processlist = process.Split(':');
            return new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("update_router_ping");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@nasid", remote.backEndNasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@secondarynasid", remote.secondaryNas.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@controller_id", 1));
                cmd.Parameters.Add(new SqlParameter("@version", int.Parse(remote.version)));
                cmd.Parameters.Add(new SqlParameter("@wifi_count", wifiCount));
                cmd.Parameters.Add(new SqlParameter("@process", process));
                res = ResponseType.READER;
                return cmd;
            }), shardId,
            new ResponseHandler<string>((reader) =>
            {
                if (reader.Read())
                {
                    remote.operationType = (OperationType)(byte)reader["operation_type"];
                    remote.operationParameter = reader["operation_parameter"].ToString();
                    remote.operationid = reader["operation_id"].ToString();
                    return reader["operation_text"].ToString();
                }
                return string.Empty;
            })).Execute();
        }

        public bool updateControllerDetails(DeviceConfig config)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"update t_controller set ssid = COALESCE(@ssid, ssid), device_mac = COALESCE(@lan_mac, device_mac),
                    router_wifi_mac = COALESCE(@wifi_mac, router_wifi_mac), router_lan_mac= COALESCE(@lan_mac, router_lan_mac), modified_time = getutcdate() 
                    where router_nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@lan_mac", CoreUtil.ToSafeDbObject(config.macid)));
                cmd.Parameters.Add(new SqlParameter("@wifi_mac", CoreUtil.ToSafeDbObject(config.macid2)));
                cmd.Parameters.Add(new SqlParameter("@nasid", config.nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@ssid", CoreUtil.ToSafeDbObject(config.ssid)));
                res = ResponseType.NONQUERY;
                return cmd;
            }), config.nasid.shard_id,
            new ResponseHandler<bool>((reader) =>
            {
                return true;
            })).Execute();
        }

        public string updateControllerDetailsv2(DeviceConfig config)
        {
            string configs = string.Empty;
            if (!string.IsNullOrEmpty(config.firmwareVersion) && config.firmwareVersion.Length > 100)
                config.firmwareVersion = config.firmwareVersion.Substring(0, 100);
            if (!string.IsNullOrEmpty(config.deviceType) && config.deviceType.Length > 100)
                config.deviceType = config.deviceType.Substring(0, 100);

            return new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"update t_controller set ssiddata = COALESCE(@ssiddata, ssiddata), device_mac = COALESCE(@lan_mac, device_mac),
                    router_wifi_mac = COALESCE(@wifi_mac, router_wifi_mac), router_lan_mac= COALESCE(@lan_mac, router_lan_mac), modified_time = getutcdate() 
                    , firmwareVersion= COALESCE(@firmwareVersion,firmwareVersion), device_type= COALESCE(@deviceType,device_type),gatewayInfo= COALESCE(@gatewayInfo,gatewayInfo)
                    output inserted.configs
                    where router_nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@lan_mac", CoreUtil.ToSafeDbObject(config.macid)));
                cmd.Parameters.Add(new SqlParameter("@wifi_mac", CoreUtil.ToSafeDbObject(config.macid2)));
                cmd.Parameters.Add(new SqlParameter("@nasid", config.nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@ssiddata", CoreUtil.ToSafeDbObject(config.ssid)));
                cmd.Parameters.Add(new SqlParameter("@deviceType", CoreUtil.ToSafeDbObject(config.deviceType)));
                cmd.Parameters.Add(new SqlParameter("@firmwareVersion", CoreUtil.ToSafeDbObject(config.firmwareVersion)));
                cmd.Parameters.Add(new SqlParameter("@gatewayInfo", CoreUtil.ToSafeDbObject(config.gatewayInfo)));
                res = ResponseType.READER;
                return cmd;
            }), config.nasid.shard_id,
            new ResponseHandler<string>((reader) =>
            {
                if (reader.Read())
                {
                    configs = reader["configs"].ToString();
                }
                return configs;
            })).Execute();
        }

        public bool updateControllerRebootTime(LongIdInfo nasid)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"update t_controller set lastRebootTime = getutcdate() 
                    where router_nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                res = ResponseType.NONQUERY;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<bool>((reader) =>
            {
                return true;
            })).Execute();
        }

        public void CommitOperationSuccess(LongIdInfo nasid, int Operationid, int status)
        {
            new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("update t_router_operation set operation_finish_time=GETUTCDATE(),status=@status where operation_id=@op_id and router_nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@op_id", Operationid));
                cmd.Parameters.Add(new SqlParameter("@status", status));
                res = ResponseType.NONQUERY;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<bool>((reader) =>
            {
                return true;
            })).Execute();
        }

        public bool MaintainConfig(string config, LongIdInfo longNasid)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                string query = "update t_controller set configs=configs+@config where router_nas_id=@nasid and configs not like @configlike";
                SqlCommand cmd = new SqlCommand(query);
                cmd.Parameters.Add(new SqlParameter("@config", config));
                cmd.Parameters.Add(new SqlParameter("@configlike", string.Format("%{0}%", config)));
                cmd.Parameters.Add(new SqlParameter("@nasid", longNasid.ToSafeDbObject()));
                res = ResponseType.NONQUERY;
                return cmd;
            }), longNasid.shard_id,
             new ResponseHandler<bool>((reader) =>
             {
                 return true;
             })).Execute();
        }

        public bool SubmitConfig(DeviceConfig config, LongIdInfo userId)
        {
            return new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("save_device_build_config");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@nasid", config.nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@username", userId.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@secondarynasid", config.secondaryNasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@controller_id", 1));
                cmd.Parameters.Add(new SqlParameter("@mac", config.macid));
                cmd.Parameters.Add(new SqlParameter("@device_type", config.deviceType));
                cmd.Parameters.Add(new SqlParameter("@device_password", config.devicePassword));
                cmd.Parameters.Add(new SqlParameter("@ap_password", config.accessPointPassword));
                cmd.Parameters.Add(new SqlParameter("@mode", string.IsNullOrEmpty(config.monitormode) ? "1" : "0"));
                cmd.Parameters.Add(new SqlParameter("@product_id", config.productId));
                cmd.Parameters.Add(new SqlParameter("@channel_type", config.channelType));
                cmd.Parameters.Add(new SqlParameter("@channel_name", string.IsNullOrEmpty(config.channelName) ? string.Empty : config.channelName));
                res = ResponseType.NONQUERY;
                return cmd;
            }), config.nasid.shard_id,
             new ResponseHandler<bool>((reader) =>
             {
                 //ShardHelper.updateMacNasMapping(config.macid, config.nasid);
                 return true;
             })).Execute();
        }

        public void CommitUpgradeSuccess(LongIdInfo nasid, int status)
        {
            new ShardQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("update t_router_operation set operation_finish_time=GETUTCDATE(),status= @status where router_nas_id = @nasid and operation_type= 14 and status = 0");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@status", status));

                res = ResponseType.NONQUERY;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<bool>((reader) =>
            {
                return true;
            })).Execute();
        }

        public int AddNasBandwidth(LongIdInfo nasid, int type, float down_bw, int operationId, string source)
        {
            return new ShardQueryExecutor<int>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("insert into t_router_bandwidth(nasid, type, download_bw, operation_id, source) values(@nasid, @type, @down_bw, @operation_id, @source);");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@type", type));
                cmd.Parameters.Add(new SqlParameter("@down_bw", down_bw));
                cmd.Parameters.Add(new SqlParameter("@operation_id", operationId));
                cmd.Parameters.Add(new SqlParameter("@source", CoreUtil.ToSafeDbObject(source)));
                res = ResponseType.READER;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<int>((reader) =>
            {
                if (reader.Read())
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            })).Execute();
        }

        public string GetGatewayInfo(LongIdInfo nasid)
        {
            return new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("select gatewayInfo from t_controller where router_nas_id = @nasid");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                res = ResponseType.READER;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<string>((reader) =>
            {
                if (reader.Read())
                {
                    return reader["gatewayInfo"].ToString();
                }
                return null;
            })).Execute();
        }

		public bool CheckIfProceedWithPppoe(LongIdInfo nasid, string pppoe)
		{
			List<KeyValuePair<LongIdInfo, DateTime>> list = new List<KeyValuePair<LongIdInfo, DateTime>>();
			new ShardQueryExecutor<List<KeyValuePair<LongIdInfo, DateTime>>>(new GetSqlCommand((out ResponseType res) =>
			{
				SqlCommand cmd = new SqlCommand("select router_nas_id, last_ping_time from t_controller where JSON_VALUE(gatewayInfo, '$.username')= @pppoe order by last_ping_time desc");
				cmd.Parameters.Add(new SqlParameter("@pppoe", pppoe));
				res = ResponseType.READER;
				return cmd;
			}), nasid.shard_id,
			new ResponseHandler<List<KeyValuePair<LongIdInfo, DateTime>>>((reader) =>
			{
				while (reader.Read())
				{
                    list.Add(new KeyValuePair<LongIdInfo, DateTime>(new LongIdInfo(nasid.shard_id, DBObjectType.ACTIVE_NAS, reader["router_nas_id"]), (DateTime)reader["last_ping_time"]));
				}
				return list;
			})).Execute();

            if (list != null)
            {
                if(list.Count == 1)
                    return true;

                if (list[0].Key.GetLongId() == nasid.GetLongId() && (list[0].Value - list[1].Value).TotalDays > 10)
                    return true;
            }

            return false;
		}

		public string SubmitBasicOp(RemoteManagement remote, LongIdInfo nasid, string operationText)
        {
            return new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("submit_basic_operation");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                cmd.Parameters.Add(new SqlParameter("@controller_id", 1));
                cmd.Parameters.Add(new SqlParameter("@operation_text", operationText == null ? string.Empty : operationText));
                cmd.Parameters.Add(new SqlParameter("@operation_type", remote.operationType));
                cmd.Parameters.Add(new SqlParameter("@operation_parameter", remote.operationParameter));
                cmd.Parameters.Add(new SqlParameter("@expiry", remote.operationExpiryTime));
                res = ResponseType.READER;
                return cmd;
            }), remote.backEndNasid.shard_id,
            new ResponseHandler<string>((reader) =>
            {
                string opid = "";
                if (reader.Read())
                {
                    opid=reader["operation_id"].ToString();
                }
                return opid;

            })).Execute();
        }

        public KeyValuePair<string, string> GetFirmwareUrlFromId(int id)
        {
            return new ShardQueryExecutor<KeyValuePair<string, string>>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand("select * from t_firmware where id = @id");
                cmd.Parameters.Add(new SqlParameter("@id", id));
                res = ResponseType.READER;
                return cmd;
            }), ShardHelper.SHARD0,
            new ResponseHandler<KeyValuePair<string, string>>((reader) =>
            {
                if (reader.Read())
                {
                    return new KeyValuePair<string, string>(reader["url"].ToString(), reader["router_model"].ToString());
                }
                return new KeyValuePair<string, string>("","");
            })).Execute();
        }

        public List<RemoteManagement> GetPendingOp(LongIdInfo nasid, string host)
        {
            return new ShardQueryExecutor<List<RemoteManagement>>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"select top 20 operation_id, operation_type, operation_finish_time, operation_parameter,
                    status, operation_publish_time, operation_expiry_time
                    from t_router_operation WHERE router_nas_id = @nasid and operation_origin=1
                    order by operation_id desc");
                cmd.Parameters.Add(new SqlParameter("@nasid", nasid.ToSafeDbObject()));
                res = ResponseType.READER;
                return cmd;
            }), nasid.shard_id,
            new ResponseHandler<List<RemoteManagement>>((reader) =>
            {
                var current = DateTime.UtcNow;
                var list = new List<RemoteManagement>();
                while (reader.Read())
                {
                    RemoteManagement remote = new RemoteManagement(host);
                    remote.operationType = (OperationType)(byte)reader["operation_type"];
                    remote.operationParameter = reader["operation_parameter"].ToString();
                    remote.operationTypeText = remote.operationType.ToString();
                    remote.status = (OperationStatus)reader["status"];
                    remote.operationPublishTime = (DateTime)reader["operation_publish_time"];
                    if (remote.operationType != OperationType.NO_OPERATION)
                    {
                        var finishTime = reader["operation_finish_time"];
                        if (finishTime != DBNull.Value)
                            remote.operationFinishTime = CoreUtil.ConvertUtcToIST((DateTime)finishTime).ToString();

                        list.Add(remote);
                    }
                    remote.operationExpiryTime = (DateTime)reader["operation_expiry_time"];
                    if (remote.operationExpiryTime < current && remote.status == 0)
                        remote.status = OperationStatus.EXPIRED;
                }

                return list;

            })).Execute();
        }

        public bool CheckSpecialCaseVikalp(long lco_account_id)
        {
            return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) AS count from t_device WHERE lco_account_id = @lco_account_id AND type = 'VIKALP'");

                cmd.Parameters.Add(new SqlParameter("@lco_account_id", lco_account_id));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<bool>((reader) =>
            {
                if (reader.Read())
                {
                  return Convert.ToInt64(reader["count"]) >= 2;
                }
                return true;
            })).Execute();
        }

        public bool CheckPartnerVikalpEligibility(long lco_account_id)
        {
          //SELECT COUNT(*) from t_device WHERE lco_account_id = 'newVikalp.lcoid' AND type = 'VIKALP'
          return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) AS count from t_device WHERE lco_account_id = @lco_account_id AND type = 'VIKALP'");

                cmd.Parameters.Add(new SqlParameter("@lco_account_id", lco_account_id));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<bool>((reader) =>
            {
                if (reader.Read())
                {
                  return Convert.ToInt64(reader["count"]) == 1;
                }
                return true;
            })).Execute();
        }


        public bool RevokePartnerVikalp(long lco_account_id)
        {
          //UPDATE t_device SET lco_account_id = NULL WHERE lco_account_id = newVikalp.lcoid
          try {
            return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
              {
                  SqlCommand cmd = new SqlCommand(@"UPDATE t_device SET lco_account_id = NULL, modified_time = GETUTCDATE() WHERE lco_account_id = @lco_account_id AND type = 'VIKALP'");
                  cmd.Parameters.Add(new SqlParameter("@lco_account_id", lco_account_id));
                  res = ResponseType.READER;
                  return cmd;
              }),
              new ResponseHandler<bool>((reader) =>
              {
                return true;
              })).Execute();
        } catch (Exception)
          {
            return false;
          }
        }

        public bool CheckVikalpEligibility(string mac)
        {
          //SELECT COUNT(*) from t_device WHERE mac = 'newVikalp.mac' AND lco_account_id IS NOT NULL AND type = 'VIKALP'
          return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) AS count from t_device WHERE mac = @mac AND lco_account_id IS NOT NULL AND type = 'VIKALP'");

                cmd.Parameters.Add(new SqlParameter("@mac", mac));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<bool>((reader) =>
            {
                if (reader.Read())
                {
                  return Convert.ToInt64(reader["count"]) == 1;
                }
                return true;
            })).Execute();
        }

        public List<string> Get10MostRecentVikalps()
        {
          //SELECT TOP 10 * from Master_db.dbo.t_device WHERE type = 'VIKALP' ORDER BY added_time DESC
          return new MasterQueryExecutor<List<string>>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"SELECT TOP 10 device_id from Master_db.dbo.t_device WHERE type = 'VIKALP' ORDER BY added_time DESC");
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<List<string>>((reader) =>
            {
                var list = new List<string>();
                while (reader.Read())
                {
                  list.Add(reader["device_id"].ToString());
                }
                return list;
            })).Execute();
          
        }

        public string ProvisionVikalp(string new_device_id, JObject newVikalp, int controller)
        {
          try
          {
            return new MasterQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
              {
              var dict = new Dictionary<string, string>();
              dict["wg"] = (string)newVikalp["wg"];
              dict["wgpub"] = (string)newVikalp["wgpub"];
              dict["wgpsk"] = (string)newVikalp["wgpsk"];
              dict["controller"] = controller.ToString();
              SqlCommand cmd = new SqlCommand(@"
                  IF EXISTS (SELECT * FROM t_device WHERE mac = @mac)
                  BEGIN
                    UPDATE t_device SET lco_account_id = @lco_account_id, modified_time = GETUTCDATE(), extra_data = @extra_data WHERE mac = @mac
                    SELECT device_id FROM t_device WHERE mac = @mac
                    END
                  ELSE 
                  BEGIN
                    INSERT INTO t_device (device_id, mac, lco_account_id, version, model, type, product, added_time, modified_time, extra_data) VALUES (@new_device_id, @mac, @lco_account_id, 'v1', 'RaspberryPi5', 'VIKALP', 'VIKALP', GETUTCDATE(), GETUTCDATE(), @extra_data);
                    SELECT device_id FROM t_device WHERE mac = @mac
                    END");

              cmd.Parameters.Add(new SqlParameter("@new_device_id", new_device_id));
              cmd.Parameters.Add(new SqlParameter("@mac", (string)newVikalp["mac"]));
              cmd.Parameters.Add(new SqlParameter("@lco_account_id", (string)newVikalp["lcoid"]));
              cmd.Parameters.Add(new SqlParameter("@extra_data", JsonConvert.SerializeObject(dict).ToSafeDbObject()));
              res = ResponseType.READER;
              return cmd;
              }),
            new ResponseHandler<string>((reader) =>
              {
                if (reader.Read()){
                  return reader["device_id"].ToString();
                }
              return "0";
              })).Execute();
          }
          catch (Exception ex)
          {
            Logger.GetInstance().Error($"Error in ProvisionVikalp error msg: {ex.Message}");
            return "0";
          }
        }

        public List<List<string>> GetPeers(int controller)
        {
          try
          {
            return new MasterQueryExecutor<List<List<string>>>(new GetSqlCommand((out ResponseType res) =>
              {
                SqlCommand cmd = new SqlCommand(@"SELECT device_id, JSON_VALUE(extra_data, '$.wgpub') AS wgpub, JSON_VALUE(extra_data, '$.wgpsk') AS wgpsk from Master_db.dbo.t_device WHERE type = 'VIKALP' AND JSON_VALUE(extra_data, '$.controller') = @controller");
                cmd.Parameters.Add(new SqlParameter("@controller", controller.ToString()));
                res = ResponseType.READER;
                return cmd;
              }),
              new ResponseHandler<List<List<string>>>((reader) =>
                {
                  var peers = new List<List<string>>();

                  while (reader.Read())
                  {
                    var peer = new List<string>();
                    peer.Add(reader["device_id"].ToString());
                    peer.Add(reader["wgpub"].ToString());
                    peer.Add(reader["wgpsk"].ToString());
                    peers.Add(peer);
                  }
                  return peers;
            })).Execute();
          }
          catch (System.Exception ex)
          {
            Console.WriteLine(ex);
            return new List<List<string>>();
          }
        }
    }
}
