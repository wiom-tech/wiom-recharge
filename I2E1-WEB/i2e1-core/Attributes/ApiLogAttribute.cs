using i2e1_basics.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;
using System;
using System.Diagnostics;

namespace i2e1_core.Attributes
{
    public class ApiLogAttribute : ActionFilterAttribute
    {
        private Stopwatch stopwatch;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var apiName = context.HttpContext.Request.Path;
            var queryParams = context.HttpContext.Request.QueryString.ToString();
            // Generate or retrieve the trace ID
            var traceId = GetOrGenerateTraceId(context);
            var message = $"API Call: {apiName} | Query Params: {queryParams}";
            context.HttpContext.Items.Add("traceId", traceId);
            LogContext.PushProperty("traceId", traceId);

            stopwatch = Stopwatch.StartNew();
            Logger.GetInstance().Info(message);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var apiName = context.HttpContext.Request.Path;
            var message = $"API Call: {apiName} | Execution Done";

            stopwatch.Stop();
            var executionTime = stopwatch.ElapsedMilliseconds;
            message += $" | Execution Time: {executionTime} ms";

            LogContext.PushProperty("executionTime", executionTime);

            if(executionTime >= 200)
            {
                Logger.GetInstance().Warning($"API Call: {apiName} is slow. Execution time : {executionTime} ms");
            }

            Logger.GetInstance().Info(message);

            base.OnActionExecuted(context);
        }

        private string GetOrGenerateTraceId(ActionContext context)
        {
            // Check if the query parameters contain a trace ID
            if (context.HttpContext.Request.Query.ContainsKey("traceId"))
            {
                return context.HttpContext.Request.Query["traceId"];
            }
            else
            {
                // Generate a random trace ID (you can use any desired logic)
                string generatedTraceId = Guid.NewGuid().ToString();
                return generatedTraceId;
            }
        }
    }
}
