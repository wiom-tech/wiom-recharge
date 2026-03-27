using i2e1_basics.Utilities;
using i2e1_basics.Database;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace i2e1_core.Utilities
{
    public class DeliveryUtility
    {
        public static int StoreMessages(DeliveryMsg msg)
        {
            Logger.GetInstance().Info(String.Format("DeliveryUtility : StoreMessages called with messages : {0}", JsonConvert.SerializeObject(msg)));
            return new ShardQueryExecutor<int>(new GetSqlCommand((out ResponseType res) =>
            {
                DataTable dt = new DataTable();
                //Add columns  
                dt.Columns.Add(new DataColumn("account_id", typeof(long)));
                dt.Columns.Add(new DataColumn("user_id", typeof(long)));
                dt.Columns.Add(new DataColumn("msg", typeof(string)));
                dt.Columns.Add(new DataColumn("type", typeof(int)));
                dt.Columns.Add(new DataColumn("mobile", typeof(string)));

                if (msg.accountIds.Value != null && msg.accountIds.Value.Count > 0)
                    foreach(LongIdInfo account in msg.accountIds.Value) 
                    {
                        dt.Rows.Add(account.ToSafeDbObject(1), null, JsonConvert.SerializeObject(msg.msg), msg.msgType, null);
                    }

                if(msg.userIds != null && msg.userIds.Count > 0)
                    foreach (LongIdInfo user in msg.userIds)
                    {
                        dt.Rows.Add(null, user.ToSafeDbObject(1), JsonConvert.SerializeObject(msg.msg), msg.msgType, null);
                    }

                if (msg.numbers != null && msg.numbers.Count > 0)
                    foreach (string mobile in msg.numbers)
                    {
                        dt.Rows.Add(null, null, JsonConvert.SerializeObject(msg.msg), msg.msgType, mobile);
                    }

                SqlCommand cmd = new SqlCommand(@"INSERT INTO t_account_messages(account_id, user_id, msg, type, mobile)
                    SELECT * FROM @newRecords");

                cmd.Parameters.Add(new SqlParameter("@newRecords", dt) { TypeName = "dbo.type_account_msg" });
                res = ResponseType.READER;
                return cmd;
            }), ShardHelper.SHARD0,
            new ResponseHandler<int>((reader) => {
                if (reader.RecordsAffected > 0)
                {
                    int insertedRows = reader.RecordsAffected;
                    Logger.GetInstance().Info(String.Format("DeliveryUtility : StoredMessages {0}", insertedRows));
                    return insertedRows;
                }
                return 0;
            })).Execute();
        }
    }
}
