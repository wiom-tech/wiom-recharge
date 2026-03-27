using i2e1_basics.Utilities;
using i2e1_core.Utilities;
using Microsoft.AspNetCore.Http;
using System;

namespace i2e1_core.Models
{
    [Serializable]
    public class UserBaseModel
    {
        public static string privateKey = I2e1ConfigurationManager.GetInstance().GetSetting("PrivateKey");
        public static string publicKey = I2e1ConfigurationManager.GetInstance().GetSetting("PublicKey");
        private string pMobile;
        public string mobile
        {
            get
            {
                return pMobile;
            }
        }

        public void SetMobile(bool showNumber, string value)
        {
            mkey = Encryption.RSAEncrypt(value, publicKey);
            pMobile = GetMaskedNumber(showNumber, value);
        }

        public LongIdInfo nasid { get; set; }
        public string inviteCode { get; set; }
        public string inviteUrl { get; set; }
        public string appId { get; set; }

        public string mkey { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public bool isSalesPerson { get; set; }

        public string GetDecryptedValue()
        {
            return mkey == null ? null : Encryption.RSADecrypt(mkey, privateKey);
        }

        public static string GetMaskedNumber(bool showNumber, string mobile)
        {
            if (!showNumber)
            {
                if (mobile.Length >= 4)
                    mobile = "xxxxxx" + mobile.Substring(mobile.Length - 4);
                else
                    mobile = "xxxxxx" + mobile;
            }
            return mobile;
        }
    }
}