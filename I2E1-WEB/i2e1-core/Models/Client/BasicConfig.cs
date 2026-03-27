using i2e1_core.Utilities;
using System;

namespace i2e1_core.Models.Client
{
    public enum BasicConfigType
    {
        LANDING_PAGE = 2,
        DATA_USAGE_CONTROL_MONTH = 10,
        UPLOAD_BANDWIDTH_AFTER_EXHAUSTED, // in kbps
        DOWNLOAD_BANDWIDTH_AFTER_EXHAUSTED, // in kbps
        NO_OF_SESSIONS = 14,
        DATA_USAGE_PER_SESSION = 16,

        //Radius configs
        SESSION_TIMEOUT = 101,
        CHILLISPOT_BANDWIDTH_MAX_UP = 102,
        CHILLISPOT_BANDWIDTH_MAX_DOWN = 103,
        CHILLISPOT_MAX_TOTAL_OCTETS = 104
        //Radius configs - end
    }

    public enum BasicConfigDataType
    {
        STRING = 0,
        SPEED,
        DATA,
        TIME,
        LIST
    }

    [Serializable]
    public class BasicConfig
    {
        public string text { get; set; }
        public string attribute { get; set; }
        public BasicConfigDataType dataType { get; set; }
        public string value { get; set; }
        public static bool IsValidValue(BasicConfig conf)
        {
            return conf != null && !string.IsNullOrEmpty(conf.value);
        }

        public bool isRadiusConfig()
        {
            switch (this.configType)
            {
                case BasicConfigType.SESSION_TIMEOUT:
                case BasicConfigType.CHILLISPOT_BANDWIDTH_MAX_UP:
                case BasicConfigType.CHILLISPOT_BANDWIDTH_MAX_DOWN:
                case BasicConfigType.CHILLISPOT_MAX_TOTAL_OCTETS:
                    return true;
                default: return false;
            }
        }

        public BasicConfig()
        {

        }

        public BasicConfig(BasicConfigType configType)
        {
            this.value = null;
            this.configType = configType;
        }

        public BasicConfig(BasicConfigType configType, String value)
        {
            this.value = value;
            this.configType = configType;
        }

        private BasicConfigType pConfigType;

        public BasicConfigType configType
        {
            get
            {
                return pConfigType;
            }
            set
            {
                pConfigType = value;
                switch (pConfigType)
                {
                    case BasicConfigType.LANDING_PAGE: text = "Custom Landing Page"; dataType = BasicConfigDataType.STRING; break;
                    case BasicConfigType.DATA_USAGE_CONTROL_MONTH: text = "Maximum data usage / month"; dataType = BasicConfigDataType.DATA; break;
                    case BasicConfigType.UPLOAD_BANDWIDTH_AFTER_EXHAUSTED: text = "Upload Bandwidth after data exhausted"; dataType = BasicConfigDataType.SPEED; break;
                    case BasicConfigType.DOWNLOAD_BANDWIDTH_AFTER_EXHAUSTED: text = "Download Bandwidth after data exhausted"; dataType = BasicConfigDataType.SPEED; break;
                    case BasicConfigType.NO_OF_SESSIONS: text = "Number of Sessions / day. Applicable in case of Session less than 24 hours"; dataType = BasicConfigDataType.STRING; break;
                    case BasicConfigType.DATA_USAGE_PER_SESSION: text = "Maximum data usage / session"; dataType = BasicConfigDataType.DATA; break;



                    // Radius Configs
                    case BasicConfigType.SESSION_TIMEOUT: text = "Session timeout"; dataType = BasicConfigDataType.TIME; this.value = String.IsNullOrEmpty(this.value)? "3600" : this.value; this.attribute = RadiusConstants.SESSION_TIMEOUT; break;
                    case BasicConfigType.CHILLISPOT_BANDWIDTH_MAX_UP: text = "Maximum Upload Bandwidth"; dataType = BasicConfigDataType.SPEED; this.value = String.IsNullOrEmpty(this.value) ? "" : this.value; this.attribute = RadiusConstants.MAX_UPLOAD_BANDWIDTH; break;
                    case BasicConfigType.CHILLISPOT_BANDWIDTH_MAX_DOWN: text = "Maximum Download Bandwidth"; dataType = BasicConfigDataType.SPEED; this.value = String.IsNullOrEmpty(this.value) ? "" : this.value; this.attribute = RadiusConstants.MAX_DOWNLOAD_BANDWIDTH; break;
                    case BasicConfigType.CHILLISPOT_MAX_TOTAL_OCTETS: text = "Maximum data usage / day"; dataType = BasicConfigDataType.DATA; this.value = String.IsNullOrEmpty(this.value) ? "" : this.value; this.attribute = RadiusConstants.MAX_DATA_USAGE; break;
                }
            }
        }
    }
}