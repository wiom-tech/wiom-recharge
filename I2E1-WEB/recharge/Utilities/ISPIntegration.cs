using recharge.Models;
using System.Collections.Generic;

namespace recharge.Utilities
{
    public abstract class ISPIntegration
    {
        public abstract List<CustomerISPDetails> GetCustomerDetailsFromMobile(ISP_ISPType_Mapping ispMapping, string mobile);
        public abstract ISPRechargeResponse CommitRecharge(string mobile, CustomerISPDetails customerISPDetails);
    }
}
