using System;

namespace i2e1_core.Models.Client
{
    [Serializable]
    public class AdvanceConfig
    {
        public AdvanceConfigType configType { get; set; }

        public string[] parameters { get; set; }
        public string value { get; set; }

        public AdvanceConfig()
        {
        }

        public string GetJoinedParameters()
        {
            if (configType == AdvanceConfigType.OPERATING_HOURS || configType == AdvanceConfigType.GLOBAL_OTP)
            {
                if (!String.IsNullOrEmpty(value)) return value;
            }
            else if (parameters != null)
                return string.Join(",", parameters);
            return string.Empty;
        }

        public void SetJoinedParameters(string value)
        {
            if (configType == AdvanceConfigType.OPERATING_HOURS || configType == AdvanceConfigType.GLOBAL_OTP)
            {
                if (!String.IsNullOrEmpty(value)) this.value = value;
            } 
            else if (value != null) parameters = value.Split(',');
            else parameters = new string[0];
        }

        public AdvanceConfig(AdvanceConfigType configType)
        {
            this.configType = configType;
            if (configType == AdvanceConfigType.AUTH_TYPE) this.parameters = new string[] { "0" };
            else this.parameters = new string[] { };
            this.value = "";
        }

        public AdvanceConfig(AdvanceConfigType configType, string value)
        {
            this.configType = configType;
            this.SetJoinedParameters(value);
        }

    }

    public enum AdvanceConfigType
    {
        AUTH_TYPE = 1,
        ROUTER_SPLASH_IMAGE = 2,
        LOGIN_PAGE_IMAGE = 3,
        LANDING_PAGE_CONFIG = 4,
        TEMPLATE_CONFIG = 5,
        SHINE_PLUS_ACTIVATION = 6,
        DEVICE_ACTIVATION = 7,
        OPERATING_HOURS = 8,
        FACEBOOK_PAGE = 9,
        FACEBOOK_CHECKIN = 11,
        HIDE_QUESTION = 12,
        NO_OF_DEVICES_PER_USER = 13,
        GLOBAL_OTP = 14,
        AUTO_LOGIN_SPAN = 15,
        VALIDATE_USER_LOGIN = 16, // this tells weather user should be allowed access or not
        DATA_VOUCHER_DETAILS = 17,
        OVERRIDE_OTP = 18,
        LANGUAGE_PREFERENCE = 19,
        NAS_DAILY_DATA_LIMIT = 20,
        PAYMENT_MODE = 21
    }
}
