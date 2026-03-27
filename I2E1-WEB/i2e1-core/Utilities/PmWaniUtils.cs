using i2e1_basics.Utilities;
using Newtonsoft.Json;
using System;
using System.Net;

namespace i2e1_core.Utilities
{
    public class PmWaniUtils
    {
        public const string REG_NO = "PMWANI-DLI-110030-PDOA000010";
        public static void SendDailyData(DateTime date, int newCount, int uniqueCount, long dataConsumedInMB)
        {
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers.Add("WANI-API-KEY", "A1PMdw9nCnTqYupeUj3OaMBX1xxJq5NOZW13fDhYHGFwFqlfYyfU8YM1E1K5lFVQ0Sy8Gpv7bhIKK86OFcIhjocUe7uOFyy9nBMs");
                var data = new
                {
                    regNumber = REG_NO,
                    day = date.ToString("yyyy-MM-dd"),
                    pdoaDataCountry = new {
                    subscriberCount = new {
                        @new = newCount, unique = uniqueCount, },
                        dataConsumed =  dataConsumedInMB}
                };

                var response = client.UploadString("https://pmwani.gov.in/api/stats/pdoa/country/day", JsonConvert.SerializeObject(data));
                Logger.GetInstance().Info(response);
            };
        }
    }
}
