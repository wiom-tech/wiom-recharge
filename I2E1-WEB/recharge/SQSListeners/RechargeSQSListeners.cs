using i2e1_basics.Database;
using i2e1_basics.ServiceBus;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Models.RouterPlan;
using i2e1_core.Models.WIOM;
using i2e1_core.QueueHandler;
using i2e1_core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using recharge.Models;
using recharge.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace recharge.SQSListeners
{
    public class RechargeSQSListeners : IQueueHandler
	{
        public static IPACCTIntegration iPACCTIntegration = new IPACCTIntegration();
        
		public Task Register()
		{
			return ServiceBusHelper.StartStandardQueueListener(ServiceBusQueue.isp_integration.ToString(), (msg) =>
			{
				switch (msg.key)
				{
                    case "VERIFY_INTERNET":

                        break;
					case "COMMIT_RECHARGE":
						string mobile = msg.payload["mobile"].ToString();
						Logger.GetInstance().Info($"Processing mobile:{mobile}");
						LongIdInfo nasid = null;
						if (msg.payload["nasid"] != null && !string.IsNullOrEmpty(msg.payload["nasid"].ToString()))
							nasid = LongIdInfo.IdParser(long.Parse(msg.payload["nasid"].ToString()));

                        BasicSQSHelper.SendFIFOMessage(BasicSQSHelper.GetQueueARN("remote.fifo"), nasid.ToString(), new MicroServiceMessage()
                        {
                            key = "SUBMIT_OPERATION",
                            payload = new JObject()
                            {
                                { "nasid", nasid.GetLongId() },
                                { "operationType", OperationType.SPEED_TEST.ToString() },
                                { "operationParameter", string.Empty },
                                { "operationPublishTime", DateTime.UtcNow.AddDays(7) },
                                { "operationExpiryTime", DateTime.UtcNow.AddDays(15) },
                            }
                        });

                        BasicHttpClient basicHttpClient = new BasicHttpClient(CoreUtil.GetCustomerUrl());

						var response = basicHttpClient.PostAsync("/customer/GetWgPolicy_V2", null, new Dictionary<string, object>()
						{
							{ "nasid", nasid == null ? null : nasid.ToSafeDbObject(1) },
							{ "mobile", mobile }
						}).Result;

						var jsonResponse = JsonConvert.DeserializeObject<JObject>(response);
						//var wgStatus = Enum.Parse<WgStatus>(jsonResponse["data"]["wgStatus"].ToString());
						var dict = jsonResponse["data"]["wgPolicy"].ToObject<Dictionary<string, object>>();
						var passportUser = jsonResponse["data"]["fdmUser"].ToObject<HomeRouterPlan>();
						var currentPlan = jsonResponse["data"]["plan"].ToObject<PDOPlan>();
						nasid = LongIdInfo.IdParser(long.Parse(dict["nasid"].ToString()));

						LongIdInfo customerAccountId = LongIdInfo.IdParser(long.Parse(dict["accountId"].ToString()));
						LongIdInfo lcoAccountId = LongIdInfo.IdParser(long.Parse(jsonResponse["data"]["fdmUser"]["createdBy"].ToString()));
						var fdmId = long.Parse(jsonResponse["data"]["fdmUser"]["id"].ToString());

                        Constants.CUSTOMER_LOG.Publish("eligible_for_recharge", mobile,
                        new Dictionary<string, object>()
                        {
                            { "mobile", mobile },
                            { "commission",  currentPlan.GetPriceWithoutDiscount() == 500 ? 250 : 300 },
                            { "nasid", nasid.GetLongId() },
                            { "customerAccountId", customerAccountId.GetLongId()  },
                            { "lcoAccountId", lcoAccountId.GetLongId() }
                        });

                        if ((I2e1ConfigurationManager.DEPLOYED_ON == "prod" || I2e1ConfigurationManager.DEPLOYED_ON == "stage") 
                            && passportUser.charges < 1000 
                            && !passportUser.transactionId.ToUpper().StartsWith("WIFI_SRVC"))
                        {
                            var ispMapping = H8Integration.GetLcoToISPMapping(lcoAccountId);
                            
                            if (ispMapping == null || ispMapping.mappings == null || ispMapping.mappings.Count == 0)
                            {
                                Logger.GetInstance().Info($"No ISP mapping found for LCO Account ID: {lcoAccountId}");
                            }
                            else
                            {
                                Logger.GetInstance().Info($"Matched LCO Account ID. Proceeding with Msg: {JsonConvert.SerializeObject(msg)}. Wgpolicy: {response}");

                                var customerList = GetCustomerDetailsFromMobile(lcoAccountId, ispMapping.mappings, mobile).Result;
                                CustomerISPDetails customerDetails = null;

                                if (customerList.Count != 1)
                                {
                                    var gatewayInfo = CoreDbCalls.GetInstance().GetGatewayInfo(nasid);
                                    try
                                    {
                                        var jObject = JsonConvert.DeserializeObject<JObject>(gatewayInfo);
                                        if (jObject["proto"].ToString() == "pppoe")
                                        {
                                            string pppoeUsername = jObject["username"].ToString();

                                            if (!string.IsNullOrEmpty(pppoeUsername))
                                            {
                                                if (customerList.Count > 0)
                                                    customerDetails = customerList.FirstOrDefault(m => m.username == pppoeUsername);

                                                if (customerDetails == null && CoreDbCalls.GetInstance().CheckIfProceedWithPppoe(nasid, pppoeUsername))
                                                {
                                                    customerDetails = GetCustomerDetailsFromPPPOEUsername(lcoAccountId, ispMapping.mappings, pppoeUsername).Result;
                                                    if (customerDetails != null)
                                                    {
                                                        Constants.CUSTOMER_LOG.Publish("pppoe_with_other_mobile", mobile,
                                                            new Dictionary<string, object>()
                                                            {
                                                                { "mobile", mobile },
                                                                { "nasid", nasid.GetLongId() },
                                                                { "pppoe", pppoeUsername }
                                                            });
                                                    }
                                                }
                                                else
                                                {
                                                    Logger.GetInstance().Info($"Multiple NAS found for PPPOE: {pppoeUsername}");
                                                }
                                            }
                                        }
                                        else if (jObject["proto"].ToString() == "static")
                                        {
                                            string staticIP = jObject["ipaddr"].ToString();
                                            if (customerList.Count > 0 && !string.IsNullOrEmpty(staticIP))
                                                customerDetails = customerList.FirstOrDefault(m => m.staticIP == staticIP);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.GetInstance().Error($"Exception while processing gateway info for NAS ID {nasid}: {ex}");
                                    }
                                }
                                else
                                {
                                    customerDetails = customerList[0];
                                }

                                if (customerDetails != null)
                                {
                                    Logger.GetInstance().Info($"Customer details found for mobile: {mobile}, LCO: {lcoAccountId}");
                                    if (customerDetails.expiryTime < (DateTime)jsonResponse["data"]["fdmUser"]["otpExpiryTime"])
                                    {
                                        bool success = CommitRecharge(nasid, mobile, customerDetails, lcoAccountId).Result;
                                        if (success)
                                        {
                                            ReleaseCommission(nasid, fdmId, lcoAccountId);
                                        }
                                    }
                                    else
                                    {
                                        Logger.GetInstance().Info($"Customer recharge already done for mobile: {mobile}, LCO: {lcoAccountId}");
                                        Constants.CUSTOMER_LOG.Publish("recharge_already_done", mobile,
                                            new Dictionary<string, object>()
                                            {
                                                { "mobile", mobile },
                                                { "nasid", nasid.GetLongId() },
                                                { "ispName", customerDetails.ISP_ISPType_Mapping.ispName },
                                                { "ispType", customerDetails.ISP_ISPType_Mapping.ispType.ToString() },
                                                { "lcoAccountId", lcoAccountId.GetLongId() }
                                            });
                                    }
                                }
                                else
                                {
                                    Logger.GetInstance().Info($"No customer details found for mobile: {mobile}, LCO: {lcoAccountId}");
                                    Constants.CUSTOMER_LOG.Publish("isp_details_not_found", mobile,
                                        new Dictionary<string, object>()
                                        {
                                            { "mobile", mobile },
                                            { "nasid", nasid.GetLongId() },
                                            { "lcoAccountId", lcoAccountId.GetLongId() }
                                        });
                                }
                            }
                        }
						break;
					default:
						throw new Exception("Unknown Message");
				}
				return true;
			});
		}

		public async Task<List<CustomerISPDetails>> GetCustomerDetailsFromMobile(LongIdInfo lcoAccountId, List<ISP_ISPType_Mapping> ispList, string mobile)
		{
            List<CustomerISPDetails> customerList = new List<CustomerISPDetails>();
            foreach (var isp in ispList)
			{
				if(isp.ispType == ISPTYPE.IPACCT)
				{
                    var list = iPACCTIntegration.GetCustomerDetailsFromMobile(isp, mobile);
                    customerList.AddRange(list);
                }
				else
				{
                    ModelDynamoDb<H8ISPDetails> modelDynamoDb = new ModelDynamoDb<H8ISPDetails>();
                    H8ISPDetails h8ISPDetails = modelDynamoDb.GetById(isp.ispName, "CUSTOMER_DETAILS");
                    if (h8ISPDetails == null)
                    {
                        Logger.GetInstance().Info($"No CUSTOMER_DETAILS api found for lco: {lcoAccountId}");
                    }

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

                    customerList.AddRange(parseCustomerDetailsResponse(isp, responseContent, mobile));
                }
			}

			return customerList;
		}

		public async Task<CustomerISPDetails> GetCustomerDetailsFromPPPOEUsername(LongIdInfo lcoAccountId, List<ISP_ISPType_Mapping> ispList, string username)
		{
			CustomerISPDetails customerISPDetailsFinal = null;
			foreach (var isp in ispList)
			{
				if(isp.ispType == ISPTYPE.H8)
				{
                    ModelDynamoDb<H8ISPDetails> modelDynamoDb = new ModelDynamoDb<H8ISPDetails>();
                    H8ISPDetails h8ISPDetails = modelDynamoDb.GetById(isp.ispName, "CUSTOMER_DETAILS");
                    if (h8ISPDetails == null)
                    {
                        Logger.GetInstance().Info($"No CUSTOMER_DETAILS api found for lco: {lcoAccountId}");
                    }

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
                            UserName = username
                        }
                    }), Encoding.UTF8, "application/json");

                    var result = await httpClient.PostAsync(h8ISPDetails.url, content);
                    var responseContent = await result.Content.ReadAsStringAsync();

                    var list = parseCustomerDetailsResponse(isp, responseContent, username);

                    CustomerISPDetails customerDetails = list.FirstOrDefault();

                    if (customerDetails != null)
                    {
                        if (customerISPDetailsFinal != null)
                        {
                            Logger.GetInstance().Error($"Another isp:{isp.ispName} found for username:{username}");
                            return null;
                        }
                        else
                        {
                            customerISPDetailsFinal = customerDetails;
                            Logger.GetInstance().Info($"isp:{isp.ispName} found for username:{username}");
                        }
                    }
                }
			}

			return customerISPDetailsFinal;
		}

		public async Task<bool> CommitRecharge(LongIdInfo nasid, string mobile, CustomerISPDetails customerISPDetails, LongIdInfo lcoAccountId)
		{
            ISPRechargeResponse iSPRechargeResponse = new ISPRechargeResponse();

            if (customerISPDetails.ISP_ISPType_Mapping.ispType == ISPTYPE.IPACCT)
			{
                iSPRechargeResponse = iPACCTIntegration.CommitRecharge(mobile, customerISPDetails);
            }
			else
			{
                ModelDynamoDb<H8ISPDetails> modelDynamoDb = new ModelDynamoDb<H8ISPDetails>();
                H8ISPDetails h8ISPDetails = modelDynamoDb.GetById(customerISPDetails.ISP_ISPType_Mapping.ispName, "ISP_RECHARGE");
                if (h8ISPDetails == null)
                {
                    Logger.GetInstance().Info($"No ISP_RECHARGE api found for isp: {customerISPDetails.ISP_ISPType_Mapping.ispName}");
                }

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
                        UserName = customerISPDetails.username,
                        Plan = customerISPDetails.planName,
                        FutureRenewal = customerISPDetails.expiryTime > DateTime.UtcNow ? "yes" : "no",
                        Upgrade = "renewal",
                        RenewWithoutRefund = "no"
                    }
                }), Encoding.UTF8, "application/json");

                var result = await httpClient.PostAsync(h8ISPDetails.url, content);
                var responseContent = await result.Content.ReadAsStringAsync();
                Logger.GetInstance().Info(responseContent);
                var jObject = JsonConvert.DeserializeObject<JObject>(responseContent);
                iSPRechargeResponse.price = jObject?["d"]["PlanMRP"].ToString();
                iSPRechargeResponse.responseMessage = jObject?["d"]["returnMessage"].ToString();
                string status = jObject["d"]["returnCode"].ToString();
				if(status == "0")
                {
					iSPRechargeResponse.success = true;
                    iSPRechargeResponse.invoice = jObject?["d"]["InvoiceNo"].ToString();
                    iSPRechargeResponse.newExpiry = (DateTime)jObject["d"]["PlanExpiryDate"];
                }
            }

            if (iSPRechargeResponse.success)
            {
                Logger.GetInstance().Info("rechage is sucessfull");
                Logger.GetInstance().Info(JsonConvert.SerializeObject(iSPRechargeResponse));
                Logger.GetInstance().Info(JsonConvert.SerializeObject(customerISPDetails));
                if ((iSPRechargeResponse.newExpiry.Value - customerISPDetails.expiryTime).TotalDays > 20)
                {
                    Constants.CUSTOMER_LOG.Publish("isp_recharge_successful", mobile,
                        new Dictionary<string, object>()
                        {
                            { "mobile", mobile },
                            { "nasid", nasid.GetLongId() },
                            { "ispName", customerISPDetails.ISP_ISPType_Mapping.ispName },
                            { "ispType", customerISPDetails.ISP_ISPType_Mapping.ispType.ToString() },
                            { "oldExpiryDate", customerISPDetails.expiryTime },
                            { "newExpiryDate", iSPRechargeResponse.newExpiry },
                            { "rechargePrice", iSPRechargeResponse.price },
                            { "invoiceNo", iSPRechargeResponse.invoice },
                            { "plan", customerISPDetails.planName },
                            { "lcoAccountId", lcoAccountId.GetLongId() },
                            { "message", iSPRechargeResponse.responseMessage }
                        });
                    Logger.GetInstance().Info($"Recharge Successful for user:{mobile}, pppoe:{customerISPDetails.username} with plan:{customerISPDetails.planName}. Expiry extended from:{customerISPDetails.expiryTime} to:{iSPRechargeResponse.newExpiry}");
                }
				else
					iSPRechargeResponse.success = false;
            }
            else
            {
                Constants.CUSTOMER_LOG.Publish("isp_recharge_failed", mobile,
                        new Dictionary<string, object>()
                        {
                            { "mobile", mobile },
                            { "nasid", nasid.GetLongId() },
                            { "ispName", customerISPDetails.ISP_ISPType_Mapping.ispName },
                            { "ispType", customerISPDetails.ISP_ISPType_Mapping.ispType.ToString() },
                            { "oldExpiryDate", customerISPDetails.expiryTime },
                            { "rechargePrice", iSPRechargeResponse.price },
                            { "plan", customerISPDetails.planName },
                            { "lcoAccountId", lcoAccountId.GetLongId() },
                            { "message", iSPRechargeResponse.responseMessage }
                        });
                Logger.GetInstance().Info($"Recharge failed for user:{mobile}, pppoe:{customerISPDetails.username} with plan:{customerISPDetails.planName}. Failue Reason:{iSPRechargeResponse.responseMessage}");
            }

			return iSPRechargeResponse.success;
        }

		public async Task<bool> ReleaseCommission(LongIdInfo nasid, long fdmId, LongIdInfo lcoAccountId)
		{
            BasicSQSHelper.SendFIFOMessage(BasicSQSHelper.GetQueueARN("incentiveAndPenalization.fifo"), lcoAccountId.ToString(), new MicroServiceMessage()
            {
                key = "RELEASE_COMMISSION_V2",
                payload = new JObject()
                            {
                                { "commissionObjects", new JArray(){ new JObject() { { "nasId", nasid.GetLongId() }, { "fdmId", fdmId } } } },
                                { "userId", I2e1ConfigurationManager.WIOM_ADMIN_USER_ID.GetLongId() }
                            }
            });
            Logger.GetInstance().Info($"Released commission for nasid:{nasid} fdmId:{fdmId}");

            return true;
		}

        private List<CustomerISPDetails> parseCustomerDetailsResponse(ISP_ISPType_Mapping iSP_ISPType_Mapping, string responseContent, string userName)
        {
            List<CustomerISPDetails> customerList = new List<CustomerISPDetails>();
            Logger.GetInstance().Info(responseContent);
            JObject jObject = JsonConvert.DeserializeObject<JObject>(responseContent);
            
            var response = jObject["d"];

            int returnCode = (int)response["returnCode"];
            string returnMessage = response["returnMessage"]?.ToString();

            if (returnCode != 0 && returnCode != Constants.USER_DOES_NOT_EXISTS_ISP_RESPONSE_CODE)
            {
                Logger.GetInstance().Error($"Failed to fetch customer details: Code={returnCode}, Message={returnMessage}");
                string subject = $"[Customer Fetch Failure] Error while retrieving customer details for ISP: {iSP_ISPType_Mapping.ispName}, ISP Type: {iSP_ISPType_Mapping.ispType}, Username: {userName}";
                string emailBody = returnMessage;
                return customerList; 
            }

            foreach (var userDetails in response["CustomerList"])
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
    }
}
