using i2e1_basics.Utilities;
using i2e1_core.Models;
using i2e1_core.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System.Timers;

namespace recharge.Utilities;

public class SppedTestUtils
{
    private static SppedTestUtils instance = null;
    private Timer speedTestTimer;
    private const int minutesIn2Days = Constants.MINUTES_IN_DAY * 2;

    private SppedTestUtils(IMemoryCache memoryCache)
    {
        if (speedTestTimer == null)
        {
            //speedTestTimer = new Timer(60 * 1000);
            //speedTestTimer.Elapsed += new ElapsedEventHandler((m, n) =>
            //{
            //    int minutesSince2022 = (int)(DateTime.UtcNow - new DateTime(2022, 1, 1)).TotalMinutes;
            //    int minuteInterval = minutesSince2022 % minutesIn2Days;
            //    Logger.GetInstance().Info("SpeedTest Timer for modulo:" + minuteInterval);
            //    var data = CoreAccountService.GetAllNasAndLcoMapping(memoryCache);
            //    foreach(var list in data)
            //    {
            //        int index = (minuteInterval * list.Value.Count) / minutesIn2Days;
            //        if (index < list.Value.Count)
            //        {
            //            LongIdInfo nas = list.Value[index];
            //            if (nas != null)
            //            {
            //                SendSpeedTestCommand(nas, "timer");
            //                list.Value[index] = null;
            //            }
            //        }
            //    }
            //});                
        }
    }

    public static SppedTestUtils CreateInstance(IMemoryCache memoryCache)
    {
        if (instance == null)
            instance = new SppedTestUtils(memoryCache);

        return instance;
    }

    public static SppedTestUtils GetInstance()
    {
        return instance;
    }

    public void Start()
    {
        speedTestTimer.Start();
    }

    public static async Task<string> SendSpeedTestCommand(Controller controller, LongIdInfo longNasid, string source, int operationId = 0)
    {
        string response = await WebUtils.RenderTemplateToMqttCommand(controller, string.Empty, "runSpeedtest.cshtml", 
            new RemoteManagement() {
                nasid = longNasid.ToString(),
                operationParameter = $"{1},{source},{operationId}"
            });
        var deviceConfig = ShardHelper.GetNasDetailsFromLongNas(longNasid);
        if(deviceConfig != null && !string.IsNullOrEmpty(deviceConfig.mac))
        {
            MQTTManager.Publish(deviceConfig.mac.ToLower(), response, Constants.SECONDS_IN_HOUR);
            Logger.GetInstance().Info("SpeedTest triggerd for nas:" + longNasid.ToSafeDbObject(1));
        }
        return response;
    }
}