using i2e1_basics.Utilities;
using Newtonsoft.Json;
using recharge.Models;
using ServiceReference;
using System.Collections.Generic;

namespace recharge.Utilities
{
    public class IPACCTIntegration : ISPIntegration
    {
        public const string USERNAME = "avinash";
        public const string PASSWORD = "yadav0903";
        public const string ENDPOINT = "https://ipb1.kcdn.in/0/bgpost?wsdl";//"https://devb.ipacct.com/0/fastpay";

        public PACCTPortTypeClient pACCTPortType;

        public IPACCTIntegration()
        {
            pACCTPortType = new PACCTPortTypeClient(new PACCTPortTypeClient.EndpointConfiguration(), ENDPOINT);
        }

        public override List<CustomerISPDetails> GetCustomerDetailsFromMobile(ISP_ISPType_Mapping ispMapping, string mobile)
        {
            List<CustomerISPDetails> customerList = new List<CustomerISPDetails>();
            var users = pACCTPortType.ipbillSearchAsync(USERNAME, PASSWORD, mobile).Result;
            foreach (var user in users)
            {
                customerList.Add(getCustomerExpiryDetails(new CustomerISPDetails() { 
                    username = user.cid,
                    ISP_ISPType_Mapping = ispMapping
                }));
            }

            return customerList;
        }

        public override ISPRechargeResponse CommitRecharge(string mobile, CustomerISPDetails customerISPDetails)
        {
            var ispRechargeResponse = new ISPRechargeResponse();
            var res = pACCTPortType.ipbillGetDueAsync(USERNAME, PASSWORD, customerISPDetails.username, "", "Wiom", null, null, null, "cash").Result;
            Logger.GetInstance().Info($"IPACCT Response for mobile:{mobile} {JsonConvert.SerializeObject(res)}");
            ispRechargeResponse.responseMessage = pACCTPortType.ipbillPayAsync(USERNAME, PASSWORD, res.pid).Result;
            Logger.GetInstance().Info("isp recharge response for ipacct for mobile : " + mobile);
            Logger.GetInstance().Info(ispRechargeResponse.responseMessage);
            Logger.GetInstance().Info(JsonConvert.SerializeObject(ispRechargeResponse));
            if (ispRechargeResponse.responseMessage.ToLower() == "ok")
            {
                Logger.GetInstance().Info("recharge sucesfull for ipacct mobile : " + mobile);
                ispRechargeResponse.success = true;
                ispRechargeResponse.newExpiry = getCustomerExpiryDetails(customerISPDetails).expiryTime;
                ispRechargeResponse.price = res.total;
            }

            return ispRechargeResponse;
        }

        private CustomerISPDetails getCustomerExpiryDetails(CustomerISPDetails customerISPDetails)
        {
            var xx = pACCTPortType.ipbillGetUserAsync(USERNAME, PASSWORD, customerISPDetails.username).Result;

            var updatedDetails = new CustomerISPDetails
            {
                username = customerISPDetails.username,
                staticIP = xx.ips[0].ip1,
                planName = xx.packagename,
                expiryTime = xx.enddate,
                ISP_ISPType_Mapping = new ISP_ISPType_Mapping
                {
                    ispName = customerISPDetails.ISP_ISPType_Mapping.ispName,
                    ispType = customerISPDetails.ISP_ISPType_Mapping.ispType
                }
            }; 

            return updatedDetails;
        }
    }
}
