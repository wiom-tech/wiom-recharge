using Google.Apis.Auth.OAuth2;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using System;
using System.Collections.Generic;

namespace i2e1_core.Utilities
{
    public class CoreBigQuery
    {
        private static BigQueryClient bigQueryClient;
        public static TableReference T_ROUTER_SESSIONS;
        public const string dateFormat = "yyyy-MM-dd";


        private static string projectId = "pioneering-axe-398904";
        private static string datasetId = "wiom";
        private static string jsonString = @"{
  ""type"": ""service_account"",
  ""project_id"": ""pioneering-axe-398904"",
  ""private_key_id"": ""1493ff8689ddb6bf68cb91736e058f3fc564a69c"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQClHofU3qLDKAdK\nvO2bXCTUnP6pnjQpyYEK4lzkG71pBp6f6ZPjc0WPy0BMjDbZyCTEdtbhdGCQujgp\nHY9Q3Hx34KSTMpfH+EyB0e9MRxE9jAzhISFLfFgb2v/aepahvz9FkkEnTe7+AFEF\niCsEeWgMZX4MN2Q+zQzjILlz/OfGYG+MtdlGs6kM4UiU5facoC36R/FvNRMcy6W8\n8ZZ7LAnXH36IrnBu/upw/Ft8VxO1NV5+2EvmPv9S7qC82/9wfEHr9+AKatPB+c0r\nU2NuIRGtY7K94T3u2bgIGbHxIRN6qzQSS5EiUgGI2VSBmV6kOh5pzgQ755RgGhFM\nuRHtPtLZAgMBAAECggEAC7WUDVjycuN6TOcD9JH+vKMbfhzNdy8mewadbsG212VU\nR2PRjH4mrzFLMIJ6+0uxq8r/pwJRPMIv7F5/xMy1+OxRz2x1jgVCK6yfCyo5A0PU\nv/xQ96DsQKFxpmUuHG9LBdx+yVJCAfo0xK8o2crgQYzK+QPOlJOG9+5gqGSFbTBZ\nP2d3jU66r65pdx2Sm+jTGsCAqHjuXR2ZP2REbGfUTK3h6X9TQUfiRlMujstz1WQv\n5LpUEEUh1bhQ7MHuohwj8FPiMZb8HJxn4Z1Qmaaw5WMVZgcqCe0J2ASBM3EtWov1\npsnDJuCwTjB+zhFnb7AqLS4gBbRaXs71+/+NXB5erwKBgQDhqOWOhs3sIWgdNnEs\nnPTqdRWy3RcDKhznVtN69h6renYSo7uaIOZTOPQkJ96tdbOsyDahI/Dti+qau804\nmLkgKUKzGr59JlesXikgVv8RplkR5mjDlfkFE1ekUU1So1LYPpp17VSkbg/dJ8F5\nGLX5UjJxVfI+SMISDQUg/PB+cwKBgQC7UdvAcpmV5e6ZXnRA3p9ke8VnlOv5Ffcv\nmISgureQoATMvM6pNqaCjRVODZrf4XpoUcoG74SDeka4sMgVTojSzOFg9sTk9zVd\nzh5FBSja+sldxYKN869wjMvmru09l7vwQ1KyWRsLJbRXkHl7x6JWicBbvd1Q2XB3\nKugrA8PqgwKBgQDYJkbUcc2XbBxlNvvLBwEV/1bsgBgF4PXUpfdmJZAVIvUsP4d4\nSCE3ACvi4gnKzx5u10x0p4+kikwLMO6PUsKoyrzoACsMh4idQ4hTQOGLz3Ir0i7x\ngZsIwJFHhNTy3hyBo92iLdXQttgdN3J8Ay5zhcdphjDGdjzmu2/5PC9EfwKBgQCz\nwznddNz5YmBYpLFx83MJblIiNmNCdhbygS5+RNGWpEoW5PZ6oyymSphwgFPpmCvt\nYtg7Ua5csoKeCWDqOaTKj72WXRrVFHwNWpnn6KytgVCvgbTpXzs1Cpk+9w5LNosw\nZps89pAiYXuxML+0zv92htmn8Qwr44+vfOizJvjj2wKBgGt/mTyx9KdlVXNphQkj\nCwvgshCCkdHpTmcDsxeIPuP9q8oGXt9h8qA2BEcnsUsc5tf1/4v0tq+xjYwwI6Ik\nVZ/v+5bLDt1WcXsRhWHBue9rVajzYBI8lg/WXfZ+QJslVEorw402nzmfsJmU6sYz\nm5cqSptP8XwreOykjXdKdl8K\n-----END PRIVATE KEY-----\n"",
  ""client_email"": ""bigqueryapi@pioneering-axe-398904.iam.gserviceaccount.com"",
  ""client_id"": ""117365634444291350793"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://oauth2.googleapis.com/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/bigqueryapi%40pioneering-axe-398904.iam.gserviceaccount.com"",
  ""universe_domain"": ""googleapis.com""
}";

        static CoreBigQuery()
        {
            bigQueryClient = BigQueryClient.Create(projectId, GoogleCredential.FromJson(jsonString));
            T_ROUTER_SESSIONS = bigQueryClient.GetTableReference(datasetId, "t_router_sessions");
        }

        public static bool uploadRunning = false;

        public static void InsertRecords(TableReference tableReference, List<BigQueryInsertRow> rowsToInsert)
        {
            try
            {
                bigQueryClient.InsertRows(tableReference, rowsToInsert);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error("Error in logging event to google " + ex.ToString());
            }
            uploadRunning = false;
        }

        public static BigQueryResults Execute(string query, QueryOptions queryOptions = null)
        {
            if (queryOptions == null)
            {
                queryOptions = new QueryOptions();
                queryOptions.UseLegacySql = true;
                queryOptions.UseQueryCache = true;
            }

            return bigQueryClient.ExecuteQuery(query, null, queryOptions);
        }

        public static BigQueryDataReader ExecuteQuery(string query, bool isLegacySql)
        {
            var queryOptions = new QueryOptions();
            queryOptions.UseLegacySql = isLegacySql;
            queryOptions.UseQueryCache = true;

            return new BigQueryDataReader(bigQueryClient.ExecuteQuery(query, null, queryOptions));
        }
    }
}
