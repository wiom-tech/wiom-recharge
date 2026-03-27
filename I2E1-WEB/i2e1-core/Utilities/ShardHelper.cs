using i2e1_basics.Database;
using i2e1_basics.Models;
using i2e1_basics.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace i2e1_core.Utilities
{
    public class ShardHelper : i2e1_basics.Utilities.BasicShardHelper
    {
        public string connectionString;
        public string linqconnectionString;
        public const long SHARD0 = 0;

        public string analyticsConnectionString { get; set; }

        private static ShardHelper dbCalls = null;

        private ShardHelper(string connectionString, string analyticsConnectionString, string linqconnectionString)
        {
            this.connectionString = connectionString;
            this.linqconnectionString = linqconnectionString;
            this.analyticsConnectionString = analyticsConnectionString;
        }

        public static ShardHelper CreateInstance(string connectionString, string analyticsConnectionString, string linqconnectionString)
        {
            if (dbCalls == null)
            {
                dbCalls = new ShardHelper(connectionString, analyticsConnectionString, linqconnectionString);
            }
            return dbCalls;
        }

        public static ShardHelper GetInstance()
        {
            return dbCalls;
        }



        private static string SQLPATH = "C:\\Users\\ayush\\Documents\\Source\\spartans_den\\I2E1-WEB" + "\\Shard-Components\\db_script.sql";


        public static void DeleteShard(string dbname, int shardid = 0)
        {
            Microsoft.Data.SqlClient.SqlConnection myConn = new Microsoft.Data.SqlClient.SqlConnection("Server=localhost;Integrated security=SSPI;database=master; TrustServerCertificate=True");
            Microsoft.Data.SqlClient.SqlConnection myConn1 = new Microsoft.Data.SqlClient.SqlConnection("Server=localhost;Integrated security=SSPI;database=Master_db; TrustServerCertificate=True");
            string script = "Drop Database @name_of_db";
            string script1 = "Delete from Shard_Table where Shard_name='@name_of_db'";
            script = script.Replace("@name_of_db", dbname);
            script1 = script1.Replace("@name_of_db", dbname);



            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new Microsoft.SqlServer.Management.Common.ServerConnection(myConn));
            Microsoft.SqlServer.Management.Smo.Server server1 = new Microsoft.SqlServer.Management.Smo.Server(new Microsoft.SqlServer.Management.Common.ServerConnection(myConn1));

            try
            {
                server.ConnectionContext.ExecuteNonQuery(script);
                server1.ConnectionContext.ExecuteNonQuery(script1);
                Console.WriteLine("Shard is Deleted Successfully");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
                if (myConn1.State == ConnectionState.Open)
                {
                    myConn1.Close();
                }
            }


        }

        public static void CreateShard(string dbname, string foldername, string shardserver)
        {

            // Make entry into Shard_table
            long shardid = MakeShardEntry(dbname, shardserver);
            if (shardid > 0)
            {
                Microsoft.Data.SqlClient.SqlConnection myConn = new Microsoft.Data.SqlClient.SqlConnection("Server=localhost;Integrated security=SSPI;database=master; TrustServerCertificate=True");//where the shard script will run(change parameters if needed)
                string script = File.ReadAllText(SQLPATH);//update sqlpath before running this function
                script = script.Replace("@name_of_db", dbname);
                script = script.Replace("@name_of_folder", foldername);
                script = script.Replace("@id_of_shard", shardid.ToString());

                Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(new Microsoft.SqlServer.Management.Common.ServerConnection(myConn));

                try
                {
                    server.ConnectionContext.ExecuteNonQuery(script);
                    Console.WriteLine(String.Format("DataBase is Created Successfully Shard ID: {0}", shardid));
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                }

            }
            else
            {
                Console.WriteLine("Something Went Wrong while Shard registeration");
            }

        }

        public static NasSplitTemplate GetLongNasFromMac(string mac)
        {
            return new MasterQueryExecutor<NasSplitTemplate>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"select * from t_device where mac = @mac");

                cmd.Parameters.Add(new SqlParameter("@mac", mac));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<NasSplitTemplate>((reader) =>
            {
                if (reader.Read())
                {
                    if (reader["nasid"] == DBNull.Value || reader["shard_id"] == DBNull.Value)
                        return new NasSplitTemplate(reader.GetString("device_id"), new LongIdInfo(0, DBObjectType.INACTIVE_NAS, 1), mac);

                    return new NasSplitTemplate(reader.GetString("device_id"), new LongIdInfo(Convert.ToInt64(reader["shard_id"]), DBObjectType.ACTIVE_NAS, Convert.ToInt64(reader["nasid"])), mac);
                }
                return null;
            })).Execute();
        }

        public static long InsertIntoBaseMapping(string mobile, LongIdInfo longId)
        {
            if (ShardHelper.getLongUserIdFromMobile(mobile) == null)
            {
                return new MasterQueryExecutor<long>(new GetSqlCommand((out ResponseType res) => {
                    SqlCommand cmd = new SqlCommand(@"Insert into username_longid_mapping(LongUserID, Phone_number) values(@ID,@mobile)");
                    cmd.Parameters.Add(new SqlParameter("@ID", longId.GetLongId()));
                    cmd.Parameters.Add(new SqlParameter("@mobile", mobile));
                    res = ResponseType.READER;
                    return cmd;
                }),
                new ResponseHandler<long>((reader) => {
                    if (reader.Read())
                    {
                        return 1;
                    }
                    return 0;
                })).Execute();
            }

            return 0;
        }



        public static LongIdInfo getLongUserIdFromMobile(string mobile)
        {
            return new MasterQueryExecutor<LongIdInfo>(new GetSqlCommand((out ResponseType res) =>
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"
                Select LongUserID from username_longid_mapping where Phone_Number=@mobile");


                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@mobile", mobile));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<LongIdInfo>((reader) =>
            {
                if (reader.Read())
                {
                    long longUserId = Convert.ToInt64(reader["LongUserID"]);
                    return LongIdInfo.IdParser(longUserId);
                }
                else
                { return null; }


            })).Execute();
        }

        public static bool SetLongUserIdFromMobile(string mobile, LongIdInfo id)
        {
            return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"
                UPDATE username_longid_mapping SET LongUserID = @adminId where Phone_Number=@mobile");


                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@mobile", mobile));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@adminId", id.ToSafeDbObject(1)));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<bool>((reader) =>
            {
                if (reader.Read())
                {
                    return true;
                }
                else
                { return false; }


            })).Execute();

        }
        /**** Remove longid_mapping by Mobile Number *****/
        public static bool RemoveLongUserFromMobile(string mobile)
        {
            return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"
                delete from Master_db.dbo.username_longid_mapping where Phone_Number = @mobile");
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@mobile", mobile));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<bool>((reader) =>
            {
                if (reader.Read())
                {
                    return true;
                }
                else
                { return false; }


            })).Execute();

        }

        static long MakeShardEntry(string shardname, string shardserver)
        {
            return new MasterQueryExecutor<long>(new GetSqlCommand((out ResponseType res) =>
            {
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(@"
                Declare @shardid bigint;
                Insert into Shard_Table (Shard_name, shard_server) values (@shardname, @shardserver)
                Set @shardid = SCOPE_IDENTITY();
                Select @shardid as shardid");


                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@shardname", shardname));
                cmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@shardserver", shardserver));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<long>((reader) =>
            {
                if (reader.Read())
                {
                    return (long)reader["shardid"];
                }
                else
                { return -1; }


            })).Execute();
        }

        public static ShardInfo GetDetailsFromShardId(long id)
        {
            return new MasterQueryExecutor<ShardInfo>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"select * from Shard_Table where Shard_id = @shardid");
                cmd.Parameters.Add(new SqlParameter("@shardid", id));
                res = ResponseType.READER;
                return cmd;
            }),
            new ResponseHandler<ShardInfo>((reader) =>
            {
                if (reader.Read())
                {
                    return new ShardInfo()
                    {
                        shardName = reader["Shard_name"].ToString(),
                        shardServer = reader["shard_server"].ToString(),
                        slaveName = reader["slave_name"].ToString(),
                        slaveServer = reader["slave_server"].ToString(),
                        secondServer = reader["second_server"].ToString()
                    };
                }
                return null;
            })).Execute();
        }

        public static NasSplitTemplate GetNasDetailsFromLongNas(LongIdInfo nas, bool disableCache = false)
        {
            return CoreCacheHelper.GetInstance().getValueFromCache<NasSplitTemplate>(CoreCacheHelper.NAS_DETAILS_FROM_NAS, nas, () => {
                return new MasterQueryExecutor<NasSplitTemplate>(new GetSqlCommand((out ResponseType res) =>
                {
                    var shardId = nas.shard_id;
                    SqlCommand cmd = new SqlCommand(@"Select * from t_device where nasid = @nas and shard_id = @shardId;");
                    cmd.Parameters.Add(new SqlParameter("@nas", nas.ToSafeDbObject()));
                    cmd.Parameters.Add(new SqlParameter("@shardId", shardId.ToSafeDbObject()));
                    res = ResponseType.READER;
                    return cmd;
                }),
                new ResponseHandler<NasSplitTemplate>((reader) =>
                {
                    if (reader.Read())
                    {
                        return new NasSplitTemplate(reader.GetString("device_id"), nas, reader.GetString("mac"));
                    }
                    return null;
                })).Execute();
            }, disableCache);
        }

        public static NasSplitTemplate GetNasDetailsFromDeviceId(string deviceId, bool disableCache = false)
        {
            return CoreCacheHelper.GetInstance().getValueFromCache<NasSplitTemplate>(CoreCacheHelper.NAS_DETAILS_FROM_DEVICEID, deviceId.ToLower(), () =>
            {
                return new MasterQueryExecutor<NasSplitTemplate>(new GetSqlCommand((out ResponseType res) =>
                {
                    SqlCommand cmd = new SqlCommand(@"Select mac, nasid, shard_id from t_device where device_id = @device_id;");
                    cmd.Parameters.Add(new SqlParameter("@device_id", deviceId.ToSafeDbObject()));
                    res = ResponseType.READER;
                    return cmd;
                }),
                new ResponseHandler<NasSplitTemplate>((reader) =>
                {
                    if (reader.Read())
                    {
                        return new NasSplitTemplate(deviceId, new LongIdInfo(reader.GetValueOrDefault<int>("shard_id"), DBObjectType.ACTIVE_NAS, reader.GetValueOrDefault<long>("nasid")), reader.GetValueOrDefault<string>("mac"));
                    }
                    return null;
                })).Execute();
            }, disableCache);
        }

        public static void ResetDeviceCache(string mac, string deviceId, LongIdInfo nasid)
        {
            CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_FROM_MACV2, mac);
            CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_DETAILS_FROM_NAS, nasid);
            CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_DETAILS_FROM_DEVICEID, deviceId.ToLower());
        }

        public static bool RemoveMacNasMapping(LongIdInfo nasid)
        {
            return new MasterQueryExecutor<bool>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"delete from mac_nasid_mapping output deleted.macid, deleted.device_id  where nasid = @nas");
                cmd.Parameters.Add(new SqlParameter("@nas", nasid.ToSafeDbObject()));
                res = ResponseType.READER;
                return cmd;

            }),
            new ResponseHandler<bool>((reader) =>
            {
                while (reader.Read())
                {
                    string dmac = reader["macid"].ToString();
                    string dvc = reader["device_id"].ToString();
                    CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_FROM_MACV2, dmac);
                    CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_DETAILS_FROM_NAS, nasid);
                    CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_DETAILS_FROM_DEVICEID, dvc.ToLower());
                }
                return true;
            })).Execute();

        }

     

        public static Object InsertLongID()
        {
            var count = new ShardQueryExecutor<string>(new GetSqlCommand((out ResponseType res) =>
            {
                SqlCommand cmd = new SqlCommand(@"Alter table t_router_sessions Add longid bigint");


                res = ResponseType.READER;
                return cmd;
            }),
            ((reader, shardId) =>
            {
            })).ExecuteAll();
            return count;
        }

        

        public static MySqlCommand FillParametersFromObject(MySqlCommand cmd, object param)
        {
            if (param != null)
            {
                foreach (PropertyInfo prop in param.GetType().GetProperties())
                {
                    var value = prop.GetValue(param);
                    if (value != null && value.GetType() == typeof(LongIdInfo))
                        cmd.Parameters.Add(new MySqlParameter("@" + prop.Name, ((LongIdInfo)value).ToSafeDbObject()));
                    else
                        cmd.Parameters.Add(new MySqlParameter("@" + prop.Name, value.ToSafeDbObject()));
                }
            }
            return cmd;
        }
    }

}

