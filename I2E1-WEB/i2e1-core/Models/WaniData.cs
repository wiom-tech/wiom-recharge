using Newtonsoft.Json;

namespace i2e1_core.Models
{
    public class WaniData
    {
        public string ver { get; set; }

        public string username { get; set; }
        public string password { get; set; }
        public string ssid { get; set; }
        public string apMacId { get; set; }
        public string deviceIp { get; set; }
        public string deviceMacId { get; set; }

        [JsonProperty(PropertyName = "app-provider-id")]
        public string app_provider_id { get; set; }

        [JsonProperty(PropertyName = "app-provider-name")]
        public string app_provider_name { get; set; }

        public string timestamp { get; set; }

        public string signature { get; set; }

        [JsonProperty(PropertyName = "payment-address")]
        public string payment_address { get; set; }

        [JsonProperty(PropertyName = "key-exp")]
        public string key_exp { get; set; }

    }
}
