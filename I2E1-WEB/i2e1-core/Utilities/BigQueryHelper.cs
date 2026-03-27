using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.BigQuery.V2;
using Google.Apis.Auth.OAuth2;


namespace i2e1_core.Utilities
{
    public class BigQueryHelper
    {
        private string projectId;
        private string datasetId;
        private string tableId;
        private BigQueryClient client;

        public BigQueryHelper(string projectId, string datasetId, string tableId)
        {
            this.projectId = projectId;
            this.datasetId = datasetId;
            this.tableId = tableId;

            // Authenticate and initialize BigQuery Client
            // GoogleCredential credential = GoogleCredential.FromFile("path_to_your_service_account_json_file");
            // client = BigQueryClient.Create(projectId, credential);
            client = BigQueryClient.Create(projectId); // assuming application default credentials
        }

        public BigQueryHelper(string tableId)
        {
            this.projectId = "i2e1-analytics-153307";
            this.datasetId = "wiom_india";
            this.tableId = tableId;

            // Authenticate and initialize BigQuery Client
            // GoogleCredential credential = GoogleCredential.FromFile("path_to_your_service_account_json_file");
            // client = BigQueryClient.Create(projectId, credential);
            client = BigQueryClient.Create(projectId); // assuming application default credentials
        }

        public void UpdateParameters(string projectId, string datasetId, string tableId)
        {
            this.projectId = projectId;
            this.datasetId = datasetId;
            this.tableId = tableId;
            client = BigQueryClient.Create(projectId); // Update client as well
        }



        public BigQueryResults Execute(string sql)
        {
            // Prepend the table reference to the SQL string
            string sqlWithTable = sql.Replace("{table}", $"`{projectId}.{datasetId}.{tableId}`");

            // Execute the query
            var result = client.ExecuteQuery(sqlWithTable, parameters: null);
            return result;
        }

        public BigQueryResults Execute(string sql, Dictionary<string, BigQueryParameter> parameters = null)
        {
            // Prepend the table reference to the SQL string
            string sqlWithTable = sql.Replace("{table}", $"`{projectId}.{datasetId}.{tableId}`");

            // Execute the query
            var result = client.ExecuteQuery(sqlWithTable, (IEnumerable<BigQueryParameter>)parameters);
            return result;
        }

    }
}

