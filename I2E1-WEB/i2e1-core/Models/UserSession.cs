using i2e1_core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace i2e1_core.Models
{
    public enum RadiusUserResponse
    {
        FAILED,
        SUCCESS,
        SESSION_EXHAUSTED,
        DATA_EXHAUSTED,
        BLOCKED,
        DEVICE_OFF,
        NOT_LOGGED_IN,
        VALIDATING,
        DEVICE_LIMIT_REACHED,
        NONE = 100
    }

    [Serializable]
    public class Session
    {
        public Session(string sessionId)
        {
            startTime = endTime = DateTime.UtcNow;
            this.sessionId = sessionId;
            partitionKey = PartitionKey.NONE;
        }

        public decimal radAcctId { get; set; }

        public string sessionId { get; set; }

        public long dataConsumed { get; set; }

        public PartitionKey partitionKey { get; set; }

        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }
    }

    [Serializable]
    public class UserPolicy
    {
        public long dataDaily { get; set; }

        public long dataPeriod { get; set; }

        public bool isVip { get; set; }

        public long secondsDaily { get; set; }

        public DateTime policyStopTime { get; set; }
    }

    [Serializable]
    public class UserSession : UserPolicy
    {
        public UserSession()
        {
            sessions = new List<Session>(5);
        }

        private RadiusUserResponse pRadiusUserResponse;

        public RadiusUserResponse radiusUserResponse
        {
            get
            {
                if (pRadiusUserResponse == RadiusUserResponse.BLOCKED || pRadiusUserResponse == RadiusUserResponse.NOT_LOGGED_IN)
                    return pRadiusUserResponse;
                if (dataDaily < 0)
                    return RadiusUserResponse.DATA_EXHAUSTED;

                if (secondsDaily < 0)
                    return RadiusUserResponse.SESSION_EXHAUSTED;

                if (dataDaily > 0 || dataPeriod > 0)
                {
                    if (CalculateDataLeft() <= 0)
                        return RadiusUserResponse.DATA_EXHAUSTED;
                }

                if (CalculateSecondsLeft() <= 0)
                    return RadiusUserResponse.SESSION_EXHAUSTED;

                return RadiusUserResponse.SUCCESS;
            }
            set
            {
                switch (value)
                {
                    case RadiusUserResponse.DATA_EXHAUSTED:
                        dataDaily = -1;
                        break;
                    case RadiusUserResponse.SESSION_EXHAUSTED:
                        secondsDaily = -1;
                        break;
                }
                pRadiusUserResponse = value;
            }
        }

        public bool allowedToRelogin { get; set; }

        public string accessCode { get; set; }

        public List<Session> sessions { get; set; }

        public long planId { get; set; }

        public void AddOldSessions(List<Session> sessions)
        {
            this.sessions = sessions;
        }

        public void AddSessions(List<Session> sessions)
        {
            this.sessions = sessions;
            if (dataDaily > 0)
            {
                var today = CoreUtil.GetTimeInIST().Date;
                long dataConsumed = sessions.Where(m => CoreUtil.ConvertUtcToIST(m.startTime).Date == today).Sum(m => m.dataConsumed);
                dataDaily += dataConsumed;
            }
            if (dataPeriod > 0)
            {
                long dataConsumed = sessions.Sum(m => m.dataConsumed);
                dataPeriod += dataConsumed;
            }
        }

        public long CalculateDataLeft()
        {
            long dailyLeft = 0;
            if (dataDaily > 0)
            {
                var today = CoreUtil.GetTimeInIST().Date;
                dailyLeft = dataDaily - sessions.Where(m => CoreUtil.ConvertUtcToIST(m.startTime).Date == today).Sum(m => m.dataConsumed);
            }
            if (dataPeriod > 0)
            {
                long periodLeft = dataPeriod - sessions.Sum(m => m.dataConsumed);
                if(dataDaily == 0 || periodLeft < dailyLeft)
                    return periodLeft;
            }
            return dailyLeft;
        }

        public long CalculateSecondsLeft()
        {

            if (dataDaily > 0 && dataPeriod > 0)
            {
                var today = CoreUtil.GetTimeInIST().Date;
                long dailyLeft = dataDaily - sessions.Where(m => CoreUtil.ConvertUtcToIST(m.startTime).Date == today).Sum(m => m.dataConsumed);

                long periodLeft = dataPeriod - sessions.Sum(m => m.dataConsumed);
                if (periodLeft < dailyLeft)
                    return (int)(policyStopTime - DateTime.UtcNow).TotalSeconds;
            }

            if (secondsDaily != 0)
            {
                if (secondsDaily == Constants.SECONDS_IN_DAY)
                {
                    DateTime now = CoreUtil.GetTimeInIST();
                    DateTime startTime = now.Date;
                    DateTime dayEnd = startTime.AddHours(24);
                    return (long)(dayEnd - now).TotalSeconds;
                }
                else
                {
                    int secondsConsumed = 0;
                    DateTime? start = null;
                    var today = CoreUtil.GetTimeInIST().Date;
                    foreach (var session in sessions.Where(m => CoreUtil.ConvertUtcToIST(m.startTime).Date == today))
                    {
                        if (start == null)
                        {
                            secondsConsumed += (int)(session.endTime - session.startTime).TotalSeconds;
                            start = session.endTime;
                        }
                        else if (start.Value < session.endTime)
                        {
                            secondsConsumed += (int)(session.endTime -
                                (session.startTime > start ? session.startTime : start.Value)).TotalSeconds;
                            start = session.endTime;
                        }
                    }

                    return secondsDaily - secondsConsumed;
                }
            }
            else
                return (int)(policyStopTime - DateTime.UtcNow).TotalSeconds;
        }

        public static string GetConfigKey(WifiUser user, long planId)
        {
            if (user.storegroupid != 0)
                return CoreCacheHelper.PLAN_SES + 0 + '_' + user.storegroupid + '_' + user.mobile.ToUpper() + planId;
            else
                return CoreCacheHelper.PLAN_SES + user.nasid + '_' + 0 + '_' + user.mobile.ToUpper() + planId;
        }

        public static string GetMemCacheKey(WifiUser user)
        {
            if (user.storegroupid != 0)
                return "SES" + 0 + '_' + user.storegroupid + '_' + user.mobile.ToUpper();
            else
                return "SES" + user.nasid + '_' + 0 + '_' + user.mobile.ToUpper();
        }
    }
}