using i2e1_basics.Models;
using i2e1_basics.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace i2e1_core.Attributes
{
    public class CoreModelBinderAttribute : ActionFilterAttribute
    {
        public async override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.HttpContext;
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                var body = context.Request.Body;

                if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    MethodInfo methodInfo = controllerActionDescriptor.MethodInfo;
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    string bodyStr = null;
                    try
                    {
                        using (StreamReader reader = new StreamReader(body, Encoding.UTF8, true, 1024, true))
                        {
                            bodyStr = await reader.ReadToEndAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.GetInstance().Error(ex.ToString());
                        filterContext.Result = new JsonResult(new ErrorResponse(ResponseStatus.FAILURE, CoreErrorCodes.BAD_REQUEST));
                    }

                    if (!string.IsNullOrEmpty(bodyStr))
                    {
                        JObject jObject = null;
                        try
                        {
                            jObject = JsonConvert.DeserializeObject<JObject>(bodyStr);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("not able to deserialise into object: " + ex.ToString());
                        }

                        if (jObject != null)
                        {
                            foreach (var parameterInfo in parameterInfos)
                            {
                                if (jObject.TryGetValue(parameterInfo.Name, out var value))
                                {
                                    if (value != null)
                                    {
                                        try
                                        {
                                            filterContext.ActionArguments[parameterInfo.Name] = value.ToObject(parameterInfo.ParameterType);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.GetInstance().Error("Could not map key " + parameterInfo.Name + " " + ex.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        var obj = jObject.ToObject(parameterInfo.ParameterType);
                                        if (obj != null)
                                            filterContext.ActionArguments[parameterInfo.Name] = obj;
                                    }
                                    catch (Exception ex)
                                    {
                                        //Logger.GetInstance().Error("Could not map object " + parameterInfo.Name + " " + ex.ToString());
                                    }
                                }
                            }
                        }  
                    }
                }
            }
        }
    }
}