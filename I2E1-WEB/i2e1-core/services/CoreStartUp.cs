using i2e1_core.Utilities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace i2e1_core.services
{
    public class CoreStartUp
    {
        public static void AddOnEndRequest(HttpContext httpContext, Callback callback)
        {
            var callbacks = (List<Callback>)httpContext.Items["OnEndRequest"];
            if (callbacks == null)
            {
                callbacks = new List<Callback>();
                httpContext.Items["OnEndRequest"] = callbacks;
            }
            callbacks.Add(callback);
        }
    }
}
