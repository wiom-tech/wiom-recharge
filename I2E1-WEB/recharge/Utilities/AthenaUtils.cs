using Amazon;
using Amazon.Athena;
using Amazon.Athena.Model;
using i2e1_basics.Utilities;
using System;
using System.Threading.Tasks;

namespace recharge.Utilities;


public class AthenaUtils
{
    private readonly AmazonAthenaClient athenaClient;

    public AthenaUtils(string accessKey, string secretKey, RegionEndpoint region)
    {
        athenaClient = new AmazonAthenaClient(accessKey, secretKey, region);
    }

    public async Task<StartQueryExecutionResponse> PostAthenaJob(string sqlQuery)
    {
        try
        {
            StartQueryExecutionResponse queryExecutionId = await athenaClient.StartQueryExecutionAsync(new StartQueryExecutionRequest
            {
                QueryString = sqlQuery,
                ResultConfiguration = new ResultConfiguration
                {
                    OutputLocation = $"s3://i2e1-data-lake{(I2e1ConfigurationManager.IS_PROD ? "-prod" : string.Empty)}/",
                }
            }).ConfigureAwait(false);

            return queryExecutionId;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<GetQueryExecutionResponse> GetQueryExecutionStatus(string queryExecutionId)
    {
        try
        {
            var getQueryExecutionRequest = new GetQueryExecutionRequest
            {
                QueryExecutionId = queryExecutionId
            };

            GetQueryExecutionResponse response = await athenaClient.GetQueryExecutionAsync(getQueryExecutionRequest).ConfigureAwait(false);

            return response;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<ResultSet> GetJobResult(StartQueryExecutionResponse queryExecutionId)
    {
        try
        {
            var getQueryResultsRequest = new GetQueryResultsRequest
            {
                QueryExecutionId = queryExecutionId.QueryExecutionId
            };

            GetQueryResultsResponse result = await athenaClient.GetQueryResultsAsync(getQueryResultsRequest).ConfigureAwait(false);
            var results = result.ResultSet;
            Logger.GetInstance().Info($"Fetched results {results.Rows.Count}");
            return results;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}