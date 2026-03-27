using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace i2e1_core.Models
{
    [Serializable]
    public enum UserState
    {
        notyet = 0,
        failed,
        success,
        already,
        logoff
    }
    [Serializable]
    public enum OTPState
    {
        GO_TO_RECEPTION = 0,
        SUCCESS = 1,
        DEVICE_COUNT_REACHED = 2,
        MOBILE_COUNT_REACHED = 3,
        OTP_REQUIRED = 4,
        OTP_SENT 
    }
    [Serializable]
    public enum DeviceType
    {
        CHROME = 0,
        FIREFOX,
        IE,
        OPERA,
        SAFARI,
        EDGE,
        BLINK
    }
    [Serializable]
    public enum ControllerType
    {
        I2E1 = 0,
        ARUBA = 1,
        MICROTIK = 2,
        AIRTEL = 3,
        WIFIDOG = 4,
        RUCKUS = 5,
        HFCL = 6,
        CAMBIUM = 7
    }

    public enum Origin
    {
        WOFR = 0,
        I2E1 = 1,
        LINQ = 2,
        BOT = 7
    }

    public enum StoreGroupId
    {
        NONE = 0,
        TRAI = 9,
        PAYTM = 11
    }

    [Serializable]
    public class UserProfile
    {
        public LongIdInfo id { get; set; }
        public string mobile { get; set; }

        public string email { get; set; }

        public string profilePhoto { get; set; }

        public string name { get; set; }

        public string gender { get; set; }

        public string username { get; set; }
    }

    public class NasId
    {
        private LongIdInfo pNasid;

        private LongIdInfo pSecNas;

        public LongIdInfo backEndNasid
        {
            get
            {
                return pNasid;
            }
        }

        public LongIdInfo secondaryNas { 
            get {
                return pSecNas;
            } 
        }

        public string nasid
        {
            set
            {
                var nastemplatedata = CoreUtil.NasResolver2(value);
                pNasid = nastemplatedata.nasid;
                this.pSecNas = nastemplatedata.secondaryNas;
            }
            get
            {
                return pNasid.ToSafeDbObject(1).ToString();
            }
        }
    }

    public class WifiUser : NasId
    {
        private string pMac;
        public string random { get; set; }
        public string mobile { get; set; }
        public bool isHomeRouter { get; set; }
        public int storegroupid { get; set; }
        public string localId { get; set; }
        public string mac
        {
            get { return pMac; }
            set { pMac = value == null ? null : value.ToUpper().Replace('-', ':'); }
        }

        public static WifiUser CreateUserFromToken(string token, string mac)
        {
            string[] values = token.Split('.');
            var user = new WifiUser()
            {
                isHomeRouter = values[0] == "1" ? true : false,
                storegroupid = int.Parse(values[1]),
                nasid = values[2],
                mobile = values[3],
                random = values[4],
                mac = mac
            };
            if(values.Length > 5)
            {
                user.localId = values[5];
            }

            return user;
        }

        public string GetToken(bool forDb = false)
        {
            if (string.IsNullOrEmpty(random))
                random = "0";

            string uniqueId;

            if (forDb == true || string.IsNullOrEmpty(this.mobile))
                uniqueId = this.mac;
            else
                uniqueId = this.mobile;

            if (uniqueId != null && uniqueId.Contains('.'))
                uniqueId = uniqueId.Replace(".", string.Empty);

            localId = string.IsNullOrEmpty(localId) ? ((int)(DateTime.UtcNow - new DateTime(2020, 1, 1)).TotalSeconds).ToString() : localId;
            var sessionid = $"{(isHomeRouter ? "1": "0")}.{storegroupid}.{nasid}.{uniqueId}.{random}.{localId}";
            return sessionid;
        }
    }

    [Serializable]
    public class User : WifiUser
    {
        public User()
        {
            templateid = new List<int>();
            attributes = new Dictionary<string, string>();
        }

        private string pRouterMac;
        public int combinedSettingId { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string device { get; set; }

        public bool isVip { get; set; }

        public string called
        {
            get { return pRouterMac; }
            set { pRouterMac = value == null ? null : value.ToUpper().Split('_')[0].Replace('-', ':'); }
        }

        public List<int> templateid { get; set; }

        public int userGroupId { get; set; }

        public string uamip { get; set; }

        public string clientip { get; set; }

        public string uamport { get; set; }

        public ControllerType controllertype { get; set; }

        public int smscount { get; set; }

        public string otp { get; set; }

        public Dictionary<string, string> attributes { get; set; }

    }
}
