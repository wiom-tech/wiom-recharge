using Amazon.Lambda;
using Amazon.Lambda.Model;
using Force.Crc32;
using i2e1_basics.Models;
using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Models.RouterPlan;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InvocationType = Amazon.Lambda.InvocationType;

namespace i2e1_core.Utilities
{
    public class CoreUtil: i2e1_basics.Utilities.BasicUtil
    {
		public static string GetCustomerUrl()
		{
			string baseUrl = "https://customer.dev.i2e1.in";
			if (I2e1ConfigurationManager.DEPLOYED_ON == "prod")
			{
				baseUrl = "https://customer.i2e1.in";
			}
			else if (!String.IsNullOrEmpty(I2e1ConfigurationManager.DEPLOYED_ON))
			{
				baseUrl = $"https://customer.{I2e1ConfigurationManager.DEPLOYED_ON}.i2e1.in";
			}
			return baseUrl;
		}

		public static NasSplitTemplate NasResolver2(string macstring)
        {
            NasSplitTemplate template = new NasSplitTemplate();
            long validnas = 0;
            if (string.IsNullOrEmpty(macstring) || long.TryParse(macstring, out validnas))
            {
                template.nasid = LongIdInfo.IdParser(validnas);
                return template;
            }

            macstring = macstring.ToUpper().Replace('-', ':');
            string[] nases = macstring.Split('_');
            for (int i = 0; i < nases.Length && i < 2; ++i)
            {
                nases[i] = nases[i].Replace("-00-00-00-00-00-00-00-00", "");
                template = CoreCacheHelper.GetInstance().GetRouterNasidFromMacv2(CoreUtil.GetNormalisedMac(nases[i]));
                if (template != null && template.nasid != null)
                    break;
            }

            if(template == null)
            {
                template = new NasSplitTemplate()
                {
                    nasid = new LongIdInfo(ShardHelper.SHARD0, DBObjectType.INACTIVE_NAS, 1)
                };
            }
            if (template.nasid == null)
                Logger.GetInstance().Error("RESOLVED NASID IS 0 : " +  macstring);

            return template;
        }

        public static string GetNormalisedMac(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            if (!value.Contains(':'))
            {
                if (value.Contains('-'))
                    value = value.Replace('-', ':');
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < value.Length; ++i)
                    {
                        if (i != 0 && i % 2 == 0)
                            sb.Append(':');
                        sb.Append(value[i]);
                    }
                    value = sb.ToString();
                }
            }

            return value.ToUpper();
        }
    }
    
}
