using Google.Cloud.BigQuery.V2;
using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace i2e1_core.Models
{
    public class RouterSession
    {
        public string sessionId;
        public DateTime startTime;
        public DateTime lastPingTime;
        public DateTime timeToSaveInDb;

        public static ConcurrentQueue<BigQueryInsertRow> routerSessionStartQueue = new ConcurrentQueue<BigQueryInsertRow>();
        public static ConcurrentDictionary<long, RouterSession> Routerlastonline = new ConcurrentDictionary<long, RouterSession>();
        private static Task SaveToDBTask;

        static RouterSession()
        {
            if (SaveToDBTask == null)
            {
                SaveToDBTask = Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(30 * 1000);
                        try
                        {
                            Logger.GetInstance().Info("Running Timer with batch of " + Routerlastonline.Count);
                            InsertBatchIntoBigQuery(routerSessionStartQueue.Count);
                            InsertUpdateSessionsInBigquery("ping_ended");
                        }
                        catch (Exception ex)
                        {
                            Logger.GetInstance().Error("Error While Saving Sessions. " + ex.ToString());
                        }
                    }
                });
            }
            AppDomain.CurrentDomain.ProcessExit += SaveAllSessions;
        }

        public static void AddRouterTimestamps(LongIdInfo nasid, string updateType)
        {
            try
            {
                var dict = new Dictionary<string, string>() { { "updateType", updateType } };

                string sessionId = Guid.NewGuid().ToString();
                DateTime currentTime = DateTime.UtcNow;
                var rowToInsert = new BigQueryInsertRow
                    {
                        {"sessionId" , sessionId},
                        {"nasid" , nasid.ToSafeDbObject(1)},
                        {"start_time" , currentTime.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"end_time" , currentTime.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"extra_data" , JsonConvert.SerializeObject(dict).ToSafeDbObject()}
                    };
                routerSessionStartQueue.Enqueue(rowToInsert);
                UpdateRouterSession(nasid, sessionId, currentTime, currentTime);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Exception in AddRouterTimestamps: {ex}");
            }
        }

        public static void UpdateRouterSession(LongIdInfo nasid, string sessionId, DateTime startTime, DateTime lastPingTime)
        {
            Routerlastonline.AddOrUpdate(nasid.GetLongId(), new RouterSession()
            {
                sessionId = sessionId,
                startTime = startTime,
                lastPingTime = lastPingTime,
                timeToSaveInDb = DateTime.UtcNow.AddMinutes(10)
            }, (key, current) => {
                current.lastPingTime = lastPingTime;
                current.timeToSaveInDb = DateTime.UtcNow.AddMinutes(10);
                return current;
            });
        }

        public static void SaveAllSessions(object sender, EventArgs e)
        {
            InsertBatchIntoBigQuery(routerSessionStartQueue.Count);
            InsertUpdateSessionsInBigquery("server_restarted", true);
            Logger.GetInstance().Info("Flushed All");
        }

        public static void InsertBatchIntoBigQuery(int count)
        {
            List<BigQueryInsertRow> rowsToInsert = new List<BigQueryInsertRow>(count);
            while (count > 0 && routerSessionStartQueue.TryDequeue(out var record))
            {
                rowsToInsert.Add(record);
                count--;
            }

            if (rowsToInsert.Count > 0)
            {
                CoreBigQuery.InsertRecords(CoreBigQuery.T_ROUTER_SESSIONS, rowsToInsert);
                Logger.GetInstance().Info($"Inserted/Updated {rowsToInsert.Count} row(s) in BigQuery");
            }
        }

        public static void InsertUpdateSessionsInBigquery(string updateType, bool saveImmediately = false)
        {
            var currentDateTime = DateTime.UtcNow;
            var dict = new Dictionary<string, string>() { { "updateType", updateType } };

            List<BigQueryInsertRow> rowsToInsertEnding = new List<BigQueryInsertRow>();
            foreach (var item in Routerlastonline)
            {
                if (saveImmediately || item.Value.timeToSaveInDb <= currentDateTime)
                {
                    var rowToInsertEnding = new BigQueryInsertRow
                    {
                        {"sessionId" , item.Value.sessionId},
                        {"nasid" , item.Key},
                        {"start_time" , item.Value.startTime.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"end_time" , item.Value.lastPingTime.ToString("yyyy-MM-dd HH:mm:ss")},
                        {"extra_data" , JsonConvert.SerializeObject(dict).ToSafeDbObject()}
                    };
                    rowsToInsertEnding.Add(rowToInsertEnding);
                    Routerlastonline.TryRemove(item);
                }
            }

            if (rowsToInsertEnding.Count > 0)
            {
                // Insert rows directly into BigQuery
                CoreBigQuery.InsertRecords(CoreBigQuery.T_ROUTER_SESSIONS, rowsToInsertEnding);
                Logger.GetInstance().Info("Flushed Router Sessions " + rowsToInsertEnding.Count + " rows to BigQuery");
            }
        }
    }
}
