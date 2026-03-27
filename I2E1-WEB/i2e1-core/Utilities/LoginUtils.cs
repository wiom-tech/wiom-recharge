using i2e1_core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace i2e1_core.Utilities
{
    public class LoginUtils
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string createCookieToken(string mobile)
        {
            string token = "";
            string encodedData = Base64Encode(mobile);
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                token = String.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(encodedData + encodedData.Substring(5, encodedData.Length - 5) + encodedData.Substring(0, 5))).Select(item => item.ToString("x2")));
            }
            return token;
        }

        public static string generateCHAP(string challenge, string otp)
        {
            if (string.IsNullOrEmpty(challenge))
                return string.Empty;

            string hexChal = pack(challenge);
            string uamSecret = "spartans";
            string s = CreateMD5(hexChal + uamSecret);
            string newChal = pack(s);
            string papp = unpack(calcXor(otp, newChal));
            return papp;
        }

        private static string CreateMD5(string input)
        {
            string testString = input;
            byte[] asciiBytes = new byte[testString.Length];
            for (int i = 0; i < testString.Length; ++i)
            {
                asciiBytes[i] = (byte)testString[i];
            }
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private static string calcXor(string a, string b)
        {
            char[] charAArray = a.ToCharArray();
            char[] charBArray = b.ToCharArray();
            int len = 0;

            char[] larger = charAArray;

            if (a.Length > b.Length)
                len = b.Length - 1;
            else
            {
                len = a.Length - 1;
                larger = charBArray;
            }

            for (int i = 0; i <= len; i++)
            {
                larger[i] = (char)(charAArray[i] ^ charBArray[i]); //error here
            }

            return new string(larger);
        }

        private static string pack(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length;)
            {
                int code = getIntFromHex(str[i]) * 16 + getIntFromHex(str[i + 1]);
                sb.Append((char)code);
                i += 2;
            }
            return sb.ToString();
        }

        private static int getIntFromHex(char code)
        {
            if (code >= '0' && code <= '9')
                return code - '0';
            else
                return code - 'a' + 10;
        }

        private static string unpack(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length;)
            {
                sb.Append(getHexFromInt(str[i] / 16));
                sb.Append(getHexFromInt(str[i] % 16));
                i++;
            }
            return sb.ToString();
        }

        private static char getHexFromInt(int code)
        {
            if (code < 10)
                return (char)('0' + code);
            else
                return (char)('a' + code - 10);
        }
    }
}
