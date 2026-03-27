using i2e1_basics.Database;
using i2e1_basics.Models;
using i2e1_basics.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Tsp;
using recharge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace recharge.Utilities
{
    public enum ISPApiType
	{
		CUSTOMER_DETAILS,
		ISP_RECHARGE
	}

	public class H8Integration
	{
		public static H8ISPDetails GetApiFromDatabase(string ispName, ISPApiType iSPApiType)
		{
			ModelDynamoDb<H8ISPDetails> modelDynamoDb = new ModelDynamoDb<H8ISPDetails>();
			return modelDynamoDb.GetById(ispName, iSPApiType.ToString());
		}

        public static bool SaveLcoToIspMapping(LcoISPMapping lcoISPMapping)
        {
            if (lcoISPMapping == null || lcoISPMapping.lcoAccountId == null || lcoISPMapping.mappings == null || !lcoISPMapping.mappings.Any())
            {
                Logger.GetInstance().Error("Invalid input: lcoISPMapping or required fields are null.");
                return false;
            }

            try
            {
                ModelDynamoDb<LcoISPMapping> modelDynamoDb = new ModelDynamoDb<LcoISPMapping>();
                
                // Fetch existing entry
                Logger.GetInstance().Info($"Fetching existing mapping for AccountId: {lcoISPMapping.lcoAccountId}");
                LcoISPMapping existingMapping = modelDynamoDb.GetById<LongIdInfo, string>(lcoISPMapping.lcoAccountId, null);

                if (existingMapping == null)
                {
                    Logger.GetInstance().Info($"Entry does not exist for AccountId: {lcoISPMapping.lcoAccountId}. Creating new entry.");
                    var insertResponse = modelDynamoDb.Insert(lcoISPMapping);
                    return insertResponse;
                }
                else
                {
                    Logger.GetInstance().Info($"Entry found for AccountId: {lcoISPMapping.lcoAccountId}. Updating existing mappings.");

                    // Ensure we don't add duplicate mappings
                    foreach (var newMapping in lcoISPMapping.mappings)
                    {
                        bool exists = existingMapping.mappings.Any(m => m.ispName == newMapping.ispName && m.ispType == newMapping.ispType);
                        if (!exists)
                        {
                            existingMapping.mappings.Add(newMapping);
                            Logger.GetInstance().Info($"Added new ISP mapping: {newMapping.ispName} - {newMapping.ispType}");
                        }
                        else
                        {
                            Logger.GetInstance().Error($"Duplicate mapping found for ISP {newMapping.ispName} - {newMapping.ispType}, skipping.");
                        }
                    }

                    // Save updated entry
                    var updateResponse = modelDynamoDb.Insert(existingMapping);
                    return updateResponse;
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Error while saving mapping: {ex.Message}");
                return false;
            }
        }


        public static async Task<JsonResult> SaveISPDetails(H8ISPDetails ispDetails)
        {
            if (ispDetails == null)
            {
                Logger.GetInstance().Info("SaveISPDetails: Received null ISP details.");
                return new JsonResult(new { success = false, message = "Invalid details" });
            }

            try
            {
                ModelDynamoDb<H8ISPDetails> modelDynamoDb = new ModelDynamoDb<H8ISPDetails>();

                // First entry with CUSTOMER_DETAILS
                ispDetails.api_type = "CUSTOMER_DETAILS";
                Logger.GetInstance().Info($"Saving ISP details: {ispDetails.url} with api_type: {ispDetails.api_type}");
                modelDynamoDb.Insert(ispDetails);

                // Modify for second entry with PLAN_RECHARGE
                string updatedUrl = ispDetails.url.Replace("CustomerDetailList", "PlanRenewal");
                ispDetails.api_type = "PLAN_RECHARGE";
                ispDetails.url = updatedUrl;

                Logger.GetInstance().Info($"Saving ISP details: {updatedUrl} with api_type: {ispDetails.api_type}");
                modelDynamoDb.Insert(ispDetails);

                Logger.GetInstance().Info("SaveISPDetails: Successfully inserted both records.");
                return new JsonResult(new { success = true, message = "ISP Details saved successfully" });
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Info($"SaveISPDetails: Error occurred - {ex.Message}");
                return new JsonResult(new { success = false, message = "Failed to save ISP details" });
            }
        }
        public static async Task<List<CustomerISPDetails>> GetCustomerDetailsFromMobile(H8ISPDetails h8ISPDetails, string mobile)
		{
			HttpClient httpClient = new HttpClient();
			var content = new StringContent(JsonConvert.SerializeObject(new
			{
				Request = new
				{

					requestDate = DateTime.UtcNow.ToString("o"),
					extTransactionId = "-99999",
					systemId = h8ISPDetails.systemId,
					password = h8ISPDetails.password
				},
				parameter = new
				{
					Mobile = mobile
				}
			}), Encoding.UTF8, "application/json");

			var result = await httpClient.PostAsync(h8ISPDetails.url, content);
			var responseContent = await result.Content.ReadAsStringAsync();

			List<CustomerISPDetails> customerDetails = parseCustomerDetailsResponse(new ISP_ISPType_Mapping() { ispType = ISPTYPE.H8, ispName = h8ISPDetails.isp_name }, responseContent);

			if (customerDetails.Count > 0)
			{
				Logger.GetInstance().Info($"isp:{h8ISPDetails.isp_name} found for mobile: {mobile}");
			}

			return customerDetails;
		}

		public static async Task<List<CustomerISPDetails>> GetCustomerDetailsFromPPPOEUsername(H8ISPDetails h8ISPDetails, string username, string mobile)
		{
			HttpClient httpClient = new HttpClient();
			var content = new StringContent(JsonConvert.SerializeObject(new
			{
				Request = new
				{

					requestDate = DateTime.UtcNow.ToString("o"),
					extTransactionId = "-99999",
					systemId = h8ISPDetails.systemId,
					password = h8ISPDetails.password
				},
				parameter = new
				{
					UserName = username,
					Mobile = mobile
				}
			}), Encoding.UTF8, "application/json");

			var result = await httpClient.PostAsync(h8ISPDetails.url, content);
			var responseContent = await result.Content.ReadAsStringAsync();

            List<CustomerISPDetails> customerDetails = parseCustomerDetailsResponse(new ISP_ISPType_Mapping() { ispType = ISPTYPE.H8, ispName = h8ISPDetails.isp_name }, responseContent);

            if (customerDetails.Count > 0)
			{
				Logger.GetInstance().Info($"isp:{h8ISPDetails.isp_name} found for username:{username}");
			}

			return customerDetails;
		}

        public static async Task<JsonResponse> CommitRecharge(H8ISPDetails h8ISPDetails, string username, string planName, string futureRenewal = "yes")
        {
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                Request = new
                {

                    requestDate = DateTime.UtcNow.ToString("o"),
                    extTransactionId = "-99999",
                    systemId = h8ISPDetails.systemId,
                    password = h8ISPDetails.password
                },
                parameter = new
                {
                    UserName = username,
                    Plan = planName,
                    FutureRenewal = futureRenewal,
                    Upgrade = "renewal",
                    RenewWithoutRefund = "no"
                }
            }), Encoding.UTF8, "application/json");

            var result = await httpClient.PostAsync(h8ISPDetails.url, content);
            var responseContent = await result.Content.ReadAsStringAsync();
            Logger.GetInstance().Info(responseContent);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(responseContent);
            string status = jObject["d"]["returnCode"].ToString();
            if (status == "0")
            {
                DateTime newPlanExpiryDate = (DateTime)jObject["d"]["PlanExpiryDate"];
				return new JsonResponse(ResponseStatus.SUCCESS, newPlanExpiryDate);
            }
            else
            {
				return new JsonResponse(ResponseStatus.FAILURE, jObject["d"]["returnMessage"].ToString());
            }
        }

        private static List<CustomerISPDetails> parseCustomerDetailsResponse(ISP_ISPType_Mapping iSP_ISPType_Mapping, string responseContent)
        {
            List<CustomerISPDetails> customerList = new List<CustomerISPDetails>();
            Logger.GetInstance().Info(responseContent);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(responseContent);

            foreach (var userDetails in jObject["d"]["CustomerList"])
            {
                string networkType = userDetails["NetworkTypeName"].ToString();
                string username = userDetails["USERNAME"].ToString();
                string staticIp = userDetails["PreAuthIP"].ToString();
                string planName = userDetails["PLANNAME"].ToString();
                DateTime expiryDate = (DateTime)userDetails["PLANEXPIRY"];

                var customerISPDetails = new CustomerISPDetails()
                {
                    networkType = networkType,
                    username = username,
                    staticIP = staticIp,
                    planName = planName,
                    expiryTime = expiryDate,
                    ISP_ISPType_Mapping = iSP_ISPType_Mapping
                };
                customerList.Add(customerISPDetails);
            }

            return customerList;
        }

        public static LcoISPMapping GetLcoToISPMapping(LongIdInfo lcoAccountId)
        {
            Logger.GetInstance().Info($"Fetching ISP mapping for LCO Account ID: {lcoAccountId}");

            if (lcoAccountId == null)
            {
                Logger.GetInstance().Error("LCO Account ID is null. Returning null.");
                return null;
            }

            try
            {
                ModelDynamoDb<LcoISPMapping> modelDynamoDb = new ModelDynamoDb<LcoISPMapping>();
                return modelDynamoDb.GetById<LongIdInfo, string>(lcoAccountId, null);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Error($"Error fetching ISP mapping for LCO Account ID: {lcoAccountId}. Exception: {ex}");
                return null;
            }
        }

    }
}
