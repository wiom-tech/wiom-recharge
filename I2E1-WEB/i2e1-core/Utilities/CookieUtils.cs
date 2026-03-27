using Microsoft.AspNetCore.Http;
using System;

namespace i2e1_core.Utilities
{
    public class CookieUtils
    {
        public static void SetCookie(HttpContext httpContext, string name, string value, bool isHttpOnly, DateTime? expires = null, bool secure = true, SameSiteMode ssm = SameSiteMode.Unspecified, string domain =null)
        {
            CookieOptions options = new CookieOptions();
            options.HttpOnly = isHttpOnly;
            options.Secure = secure;
            options.SameSite = ssm;
            if (!string.IsNullOrEmpty(domain))
                options.Domain = domain;
            if (expires != null)
                options.Expires = expires.Value;
            httpContext.Response.Cookies.Append(name, value, options);
        }

        public static string GetCookie(HttpContext httpContext, string name)
        {
            string cookie = httpContext.Request.Cookies[name];
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            else
            {
                return cookie;
            }
        }

        public static void DeleteCookie(HttpContext context, string key, string host = null)
        {
            string val = context.Request.Cookies[key];
            if (!string.IsNullOrEmpty(val))
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.UtcNow.AddDays(-1);
                if(!string.IsNullOrEmpty(host))
                    options.Domain = host;
                context.Response.Cookies.Append(key, val, options);
            }
        }
    }
}
