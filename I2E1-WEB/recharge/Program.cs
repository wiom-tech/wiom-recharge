using i2e1_basics.Utilities;
using i2e1_core.MiddleWare;
using i2e1_core.QueueHandler;
using i2e1_core.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using recharge.MiddleWare;
using recharge.Utilities;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;

//Environment.SetEnvironmentVariable("DEPLOYED_ON", "stage");
Logger.CreateInstance(true);
Logger.GetInstance().Info("App Started");
I2e1ConfigurationManager.GetInstance().SetAllServerConfig();

CoreDbCalls.CreateInstance();
CoreCacheHelper.CreateInstance(
	CoreCacheHelper.GetCacheConnectionString(
		I2e1ConfigurationManager.GetInstance().GetSetting("RedisCacheServer"),
		I2e1ConfigurationManager.GetInstance().GetInt("RedisCachePort"),
		I2e1ConfigurationManager.GetInstance().GetSetting("RedisCachePassword")),
	0);

S3BasicUtils.CreateInstance("AKIAROGPLCNH47NMD276", "16d6K5Y6oCrYLnaN3mmkguBttUT+xnSiftjd+0L2");

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options =>
{
	options.AllowSynchronousIO = true;
});

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

builder.Services.AddMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHostedService<Worker>();
builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()  // Assuming the handlers are in the same assembly as Program
    .AddClasses(classes => classes.AssignableTo<IQueueHandler>())  // Only classes implementing ISQSHandler
    .AsImplementedInterfaces()  // Register as their implemented interfaces
    .WithTransientLifetime()  // You may adjust the lifetime scope as needed
);

var app = builder.Build();
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddMemoryCache())
    .Build();

IMemoryCache cache =
    host.Services.GetRequiredService<IMemoryCache>();

if (Environment.GetEnvironmentVariable("SUPERVISOR") == "remote-i2e1-1")
    SppedTestUtils.CreateInstance(cache).Start();

app.UseMiddleware<OptionsMiddleware>();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
         context.Features.Get<IExceptionHandlerPathFeature>();
        if (context.Items.TryGetValue("traceId", out var traceId))
        {
            LogContext.PushProperty("traceId", traceId);
            Logger.GetInstance().Error($"TraceId: {traceId} | Path: {exceptionHandlerPathFeature.Path} Exception: {exceptionHandlerPathFeature.Error}");
        }
        else
        {
            Logger.GetInstance().Error($"Path: {exceptionHandlerPathFeature.Path} Exception: {exceptionHandlerPathFeature.Error}");
        }
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{ \"traceId\": \"{traceId}\" }}");
    });
});

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.Use(async (context, next) => {
    await next();
    var callbacks = (List<Callback>)context.Items["OnEndRequest"];
    if (callbacks != null)
    {
        for (int i = 0; i < callbacks.Count; i++)
        {
            callbacks[i]();
        }
    }
});

app.UseMiddleware<RemoveChunkMiddleWare>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapControllerRoute(
        "default", "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute("all", "{*url}", new { controller = "Home", action = "Index" });
});

app.Run();