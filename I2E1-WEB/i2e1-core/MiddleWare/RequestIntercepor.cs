using i2e1_core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace i2e1_core.MiddleWare
{
    public class RequestInterceptor
    {
        private readonly RequestDelegate _next;

        public RequestInterceptor(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            return BeginInvoke(context);
        }

        private Task BeginInvoke(HttpContext context)
        {
            string sessionId = "NA";
            string appId = "NA";
            try
            {
                sessionId = context.Request.Headers["sessionId"];
            }
            catch
            {
                sessionId = "";
            }
            try
            {
                appId = context.Request.Headers["appId"];
            }
            catch
            {
                appId = "";
            }
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                CookieUtils.SetCookie(context, "sessionId", sessionId, false, DateTime.UtcNow.AddMinutes(360), false);
            }
            
            if (string.IsNullOrEmpty(appId))
            {
                appId = Guid.NewGuid().ToString();
                CookieUtils.SetCookie(context, "appId", appId, false, DateTime.UtcNow.AddYears(1), false);
            }

            context.Items["appId"] = appId;
            context.Items["sessionId"] = sessionId;
            return _next.Invoke(context);
        }
    }
    public static class RequestInterceptorExtensions
    {
        public static IApplicationBuilder UseInterceptor(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestInterceptor>();
        }
    }
}
