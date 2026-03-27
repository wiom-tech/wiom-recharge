using i2e1_basics.Models;
using i2e1_basics.Database;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Models.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace i2e1_core.Utilities
{
    public class RadAcctDbCalls
    {
        //kindly dont change this method or inform app team before doing so , 
        //so as to not break the existing admin app
        public static List<UserDataUsage> GetDataUsage(bool showNumber, LongIdInfo nasid, DateTime start, DateTime end)
        {
            List<UserDataUsage> dataUsage = new List<UserDataUsage>();
            var reader = CoreBigQuery.ExecuteQuery(@"SELECT
    UserName,
    SUM(AcctInputOctets) AS AcctInputOctets,
    SUM(AcctOutputOctets) AS AcctOutputOctets,
    COUNT(DISTINCT CAST(AcctStartTime AS DATE)) AS days,
    CAST(MAX(AcctStartTime) AS date) as date,
    MAX(AcctStopTime) AS stop
FROM
wiom.p_radacct WHERE 
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + start.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + end.ToString("yyyy-MM-dd") + "') and " +
"routernasid = '" + nasid + @"' GROUP BY
    UserName,
    CAST(AcctStartTime AS DATE)
ORDER BY
    stop DESC,
    UserName ASC; ", false);

            while (reader.Read())
            {
                if (reader["AcctInputOctets"] != DBNull.Value && reader["AcctOutputOctets"] != DBNull.Value)
                {
                    var mobile = reader["UserName"].ToString();

                    UserDataUsage userData = new UserDataUsage()
                    {
                        dataUpload = (long)reader["AcctOutputOctets"],
                        dataDownload = (long)reader["AcctInputOctets"],
                        days = Convert.ToInt32(reader["days"]),
                        sessionEnd = (DateTime)reader["date"],
                        nasid = nasid
                    };
                    userData.SetMobile(showNumber, mobile);
                    dataUsage.Add(userData);
                }
            }

            return dataUsage;
        }

        //TODO : Change for sharding
        public static List<UserDataUsage> GetDetailedDataReport(bool showNumber, List<LongIdInfo> nasids, string mobile, DateTime start, DateTime end)
        {
            List<UserDataUsage> dataUsage = new List<UserDataUsage>();
            string query = null;
            if (string.IsNullOrEmpty(mobile))
            {
                query = @"SELECT
    UserName,
    RouterNasId,
    AcctInputOctets,
    AcctOutputOctets,
    AcctStartTime,
    AcctStopTime,
    CallingStationId,
    routerNasId,
    plan_id,
    otp
FROM
wiom.p_radacct WHERE 
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + start.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + end.ToString("yyyy-MM-dd") + "') and " +
"routernasid in (" + string.Join(',', nasids.Select(m => "'" + m + "'")) + @")";
            }
            else
            {

                query = @"SELECT
    UserName,
    RouterNasId,
    AcctInputOctets,
    AcctOutputOctets,
    AcctStartTime,
    AcctStopTime,
    CallingStationId,
    routerNasId,
    plan_id,
    otp
FROM
wiom.p_radacct WHERE 
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + start.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + end.ToString("yyyy-MM-dd") + "') and " +
"routernasid in (" + string.Join(',', nasids.Select(m => "'" + m + "'")) + @") and UserName = '" + mobile + "'"; 
            }

            var reader = CoreBigQuery.ExecuteQuery(query, false);
            while (reader.Read())
            {
                if (reader["AcctInputOctets"] != DBNull.Value && reader["AcctOutputOctets"] != DBNull.Value)
                {
                    UserDataUsage userData = new UserDataUsage();
                    userData.SetMobile(showNumber, reader["UserName"].ToString());
                    userData.dataUpload = (long)reader["AcctOutputOctets"];
                    userData.dataDownload = (long)reader["AcctInputOctets"];
                    //userData.nasid =  reader["RouterNasId"].ToString();
                    userData.sessionStart = ((DateTime)reader["AcctStartTime"]).Add(new TimeSpan(5, 30, 0));
                    userData.sessionEnd = ((DateTime)reader["AcctStopTime"]).Add(new TimeSpan(5, 30, 0));
                    userData.macId = reader["CallingStationId"].ToString();
                    userData.nasid = LongIdInfo.IdParser(Convert.ToInt64(reader["RouterNasId"]));
                    userData.fdmId = (long)reader["plan_id"];
                    userData.otp = reader["otp"] == null ? string.Empty : reader["otp"].ToString();
                    dataUsage.Add(userData);
                }
            }

            return dataUsage;
        }

        public static JsonResponse GetUsersInThisDuration(LongIdInfo nasid, DateTime startDate, DateTime endDate)
        {
            RadacctDb radacctDb = new RadacctDb(nasid);
            HashSet<string> users = new HashSet<string>();
            radacctDb.ExecuteAutoPartition("SELECT DISTINCT UserName AS username FROM {{table_name}} WHERE RouterNasId = @nasid AND AcctStartTime >= @start_date AND AcctStartTime < @end_date;",
                new MySqlExecuteAllResponseHandler((reader, shardId) =>
                {
                    while (reader.Read())
                    {
                        users.Add(reader["username"].ToString());
                    }
                }), string.Empty,
                new { nasid, start_date = startDate, end_date = endDate }, startDate, endDate);

            Hashtable data = new Hashtable();
            data.Add(nasid.ToString(), users.Count.ToString());
            return new JsonResponse(ResponseStatus.SUCCESS, "", users);
        }

        public static List<UserDataUsage> GetOnlineUsers(bool showNumber, LongIdInfo nasid)
        {
            List<UserDataUsage> list = new List<UserDataUsage>();
            var reader = CoreBigQuery.ExecuteQuery(@"SELECT UserName, AcctSessionId FROM
wiom.p_radacct WHERE
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + DateTime.UtcNow.AddMinutes(-5).ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "') and " +
"routernasid = '" + nasid + "' and status != 2", false);

            while (reader.Read())
            {
                var dataUsage = new UserDataUsage();
                dataUsage.SetMobile(showNumber, reader["UserName"].ToString());
                list.Add(dataUsage);
            }

            return list;
        }


        public static JsonResponse GetStats(bool showNumber, LongIdInfo nasid, DateTime start, DateTime end)
        {
            RadacctDb radacctDb = new RadacctDb(nasid);
            List<RouterStat> stats = new List<RouterStat>();
            radacctDb.ExecuteAutoPartition(@"SELECT
    username AS mobile,
    callingstationid AS mac_id,
    HOUR(AcctStartTime) AS hour,
    DAYOFYEAR(AcctStartTime) AS day,
    COUNT(*) AS count
FROM
    {{table_name}}
WHERE
    routernasid = @nasid
    AND AcctStartTime >= @start_time
    AND AcctStopTime <= @end_time
GROUP BY
    username,
    callingstationid,
    HOUR(AcctStartTime),
    DAYOFYEAR(AcctStartTime);
",
                new MySqlExecuteAllResponseHandler((reader, shardId) =>
                {
                    while (reader.Read())
                    {
                        RouterStat stat = new RouterStat();
                        stat.SetMobile(showNumber, reader["mobile"].ToString());
                        stat.macId = reader["mac_id"].ToString();
                        stat.hour = (int)reader["hour"];
                        stat.day = (int)reader["day"];
                        stat.count = (int)reader["count"];
                        stats.Add(stat);
                    }
                }), string.Empty, new { nasid, start_time = start, end_time = end }, start, end);
            return new JsonResponse(ResponseStatus.SUCCESS, "", stats);
        }

        public static Dictionary<long, string> GetLastLoginTime(List<LongIdInfo> nasids)
        {
            var list = new Dictionary<long, string>();
            foreach (LongIdInfo nas in nasids)
            {
                list[nas.GetLongId()] = "-";
            }
            var currentDate = DateTime.UtcNow;
            var reader = CoreBigQuery.ExecuteQuery(@"SELECT   routernasid,   
MAX(AcctStopTime) AS last_used FROM wiom.p_radacct WHERE
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + currentDate.AddDays(-10).ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + currentDate.ToString("yyyy-MM-dd") + "') and " +
"routernasid IN ("+ string.Join(',', nasids.Select(m => "'"+m+"'")) +") GROUP BY    routernasid; ", false);

            while (reader.Read())
            {
                long longNas = Convert.ToInt64(reader["routernasid"]);
                if (list[longNas] == "-")
                    list[longNas] = (DateTime.UtcNow - (DateTime)reader["last_used"]).TotalMinutes.ToString();
            }

            return list;
        }

        public static Dictionary<long, string> GetDataUsedInInterval(int interval, List<LongIdInfo> nasids, bool intervalIsMinutes = false)
        {
            Dictionary<long, string> dict = new Dictionary<long, string>();
            Dictionary<long, double> temp = new Dictionary<long, double>();
            var time = DateTime.UtcNow;
            if (intervalIsMinutes)
                time = time.AddMinutes(-interval);
            else
                time = time.AddDays(-interval);
            foreach (LongIdInfo nas in nasids)
            {
                temp[nas.GetLongId()] = 0;
            }


            var reader = CoreBigQuery.ExecuteQuery(@"SELECT
    RouterNasId AS routernasid,
    SUM(CAST(AcctInputOctets AS bigint)) + SUM(CAST(AcctOutputOctets AS bigint)) AS data_used,
    SUM(CAST(AcctInputOctets AS bigint)) AS download,
    SUM(CAST(AcctOutputOctets AS bigint)) AS upload
FROM wiom.p_radacct WHERE
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + time.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "') and " +
"routernasid IN (" + string.Join(',', nasids) + ") GROUP BY    routernasid; ", false);

            while (reader.Read())
            {
                long longNas = Convert.ToInt64(reader["routernasid"]);
                long dataUsed = (reader["data_used"] == DBNull.Value ? 0 : (long)reader["data_used"]);
                temp[longNas] = temp[longNas] + (((double)dataUsed) / (1024 * 1024));
            }

            foreach (LongIdInfo nas in nasids)
            {
                dict[nas.GetLongId()] = Convert.ToString(temp[nas.GetLongId()]);
            }
            return dict;
        }

        public static Dictionary<long, int> GetUsersInDuration(List<LongIdInfo> nasids, DateTime startTime)
        {
            var dict = new Dictionary<long, int>();
            foreach (var nas in nasids)
            {
                dict[nas.GetLongId()]= 0;
            }

            var reader = CoreBigQuery.ExecuteQuery(@"SELECT   routernasid,    COUNT(DISTINCT username) AS count FROM
wiom.p_radacct WHERE
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + startTime.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "') and " +
"routernasid IN (" + string.Join(',', nasids.Select(m => "'" + m + "'")) + ") GROUP BY    routernasid; ", false);

            while (reader.Read())
            {
                long longNas = Convert.ToInt64(reader["routernasid"]);
                dict[longNas] = (int)reader["count"];
            }

            return dict;
        }

        public static Dictionary<long, double> GetDataUsedInDuration(List<LongIdInfo> nasids, DateTime startTime)
        {
            var dict = new Dictionary<long, double>();
            foreach (var nas in nasids)
            {
                dict.Add(nas.GetLongId(), 0);
            }

            var currentDate = DateTime.UtcNow;
            var reader = CoreBigQuery.ExecuteQuery(@"SELECT routernasid, SUM(AcctInputOctets) + SUM(AcctOutputOctets) AS data_used FROM
wiom.p_radacct WHERE
TIMESTAMP_TRUNC(AcctStartTime, DAY) between TIMESTAMP('" + startTime.ToString("yyyy-MM-dd") + "') and TIMESTAMP('" + currentDate.ToString("yyyy-MM-dd") + "') and " +
"routernasid IN (" + string.Join(',', nasids.Select(m => "'" + m + "'")) + ") GROUP BY    routernasid; ", false);

            while (reader.Read())
            {
                long longNas = Convert.ToInt64(reader["routernasid"]);
                long dataUsed = (reader["data_used"] == DBNull.Value ? 0 : Convert.ToInt64(reader["data_used"]));
                dict[longNas] = dict[longNas] + (((double)dataUsed) / (1024 * 1024));
            }

            return dict;
        }
    }
}
