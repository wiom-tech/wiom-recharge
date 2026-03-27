using i2e1_basics.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace i2e1_core.Utilities
{
    public class MyHttpClient
    {
        private HttpClient httpClient;

        public MyHttpClient()
        {
            httpClient = new HttpClient();
        }

        public void AddHeaders(Dictionary<String, String> headers = null)
        {
            foreach (var kvp in headers)
            {
                httpClient.DefaultRequestHeaders.Add(kvp.Key, kvp.Value);
            }

        }

        public async Task<string> GetRequestAsync(String url, Dictionary<String, String> param = null)
        {
            if (param != null)
            {
                url = url + "?";
                foreach (var kvp in param)
                {
                    url = kvp.Key + "=" + kvp.Value + "&";
                }
                url = url.Remove(url.Length - 1);
            }

            HttpResponseMessage response = await httpClient.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                Logger.GetInstance().Info(String.Format($"MyHttpClient: GetRequestAsync response for : {url} is : {responseBody}"));
            else
                Logger.GetInstance().Info(String.Format($"MyHttpClient: GetRequestAsync request faild for : {url} with status code : {response.StatusCode}"));

            httpClient.Dispose();
            return responseBody;
        }

        public async Task<string> PostRequestAsync(String url, String param)
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(param, Encoding.UTF32, "application/json"));
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                Logger.GetInstance().Info(String.Format($"MyHttpClient: PostRequestAsync response for : {url} is : {responseBody}"));
            else
                Logger.GetInstance().Info(String.Format($"MyHttpClient: PostRequestAsync request faild for : {url} with status code : {response.StatusCode}"));

            httpClient.Dispose();
            return responseBody;
        }

        public async Task<string> PostRequestAsync(String url, MultipartFormDataContent param = null)
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, param);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                Logger.GetInstance().Info(String.Format($"MyHttpClient: PostRequestAsync response for : {url} is : {responseBody}"));
            else
                Logger.GetInstance().Info(String.Format($"MyHttpClient: PostRequestAsync request faild for : {url} with status code : {response.StatusCode}"));

            httpClient.Dispose();
            return responseBody;
        }
    }
}
