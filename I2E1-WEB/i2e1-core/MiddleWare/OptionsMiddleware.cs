using Amazon.Util;
using i2e1_core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace i2e1_core.MiddleWare
{
    public class OptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public OptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            return BeginInvoke(context);
        }

        private Task BeginInvoke(HttpContext context)
        {
            try
            {
                var uri = new Uri(context.Request.Headers["referer"].ToString());
                var origin = uri.Scheme + "://" + uri.Host;
                if (origin.IndexOf("localhost") > -1 && (uri.Port != 80 && uri.Port != 443))
                    origin += ":" + uri.Port;
                context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            }
            catch
            {
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                }
            }

            //context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "http://localhost:4200" });

            var headersToAdd = new Dictionary<string, string[]>
                {
                    { "Access-Control-Allow-Headers", new[] { "Origin,X-Requested-With, Authorization, Content-Type, Accept, version-code, version-name, i2e1-admin-token, i2e1-admin-view-token, otp-user, appId, sessionId, login-user-session, wl-token, jwtToken, " + CustomHeader.APP_NAME + ", " + CustomHeader.APP_VERSION } },
                    { "Access-Control-Allow-Methods", new[] { "GET, POST, PUT, DELETE, OPTIONS" } },
                    { "Access-Control-Allow-Credentials", new[] { "true" } },
                    { "Access-Control-Expose-Headers", new[] { "set-cookie", "location" } }
                };

            // Check if each header key already exists before adding it
            foreach (var header in headersToAdd)
            {
                if (!context.Response.Headers.ContainsKey(header.Key))
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }
            }
            if (context.Request.Method == "OPTIONS")
            {
                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            }
            return _next.Invoke(context);
        }
    }
}
