using i2e1_basics.Attributes;
using i2e1_basics.Models;
using i2e1_basics.Utilities;
using Microsoft.AspNetCore.Mvc;
using recharge.Models;
using recharge.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using i2e1_core.Models.RouterPlan;
using i2e1_core.Models.WIOM;
using i2e1_core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace recharge.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Home/Index.cshtml");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [CoreModelBinder]
    public async Task<JsonResult> TestCustomerDetailsApi(string url, string systemId, string password, string username, string mobile)
    {
        var customerDetails = await H8Integration.GetCustomerDetailsFromPPPOEUsername(new H8ISPDetails()
        {
            url = url,
            systemId = systemId,
            password = password,
        }, username, mobile);

        return new JsonResult(new JsonResponse(ResponseStatus.SUCCESS, customerDetails));
    }

    [CoreModelBinder]
    public async Task<JsonResult> TestRechargeApi(string url, string systemId, string password, string username, string planName)
    {
        var jsonResponse = await H8Integration.CommitRecharge(new H8ISPDetails()
        {
            url = url,
            systemId = systemId,
            password = password,
        }, username, planName);

        return new JsonResult(jsonResponse);
    }

    [CoreModelBinder]
    public async Task<JsonResult> SaveISPDetails(string url, string systemId, string password, string ispName)
    {
        var jsonResponse = await H8Integration.SaveISPDetails(new H8ISPDetails()
        {
            isp_name = ispName,
            url = url,
            systemId = systemId,
            password = password
        });

        return new JsonResult(jsonResponse);
    }

    [CoreModelBinder]
    public JsonResult SaveLcoToIspMapping(LongIdInfo lcoAccountId, ISPTYPE ispType, string ispName)
    {
        if (lcoAccountId == null || string.IsNullOrWhiteSpace(ispName))
        {
            Logger.GetInstance().Error("Invalid input: lcoAccountId or ispName is null/empty.");
            return new JsonResult(new { success = false, message = "Invalid input data" });
        }

        bool isSuccess = H8Integration.SaveLcoToIspMapping(new LcoISPMapping()
        {
            lcoAccountId = lcoAccountId,
            mappings = new List<ISP_ISPType_Mapping>()
            {
                new ISP_ISPType_Mapping { ispType = ispType, ispName = ispName }
            }
        });

        if (isSuccess)
        {
            Logger.GetInstance().Info($"Mapping successfully saved for AccountId: {lcoAccountId}");
            return new JsonResult(new { success = true, message = "Mapping saved successfully" });
        }
        else
        {
            Logger.GetInstance().Error($"Failed to save mapping for AccountId: {lcoAccountId}");
            return new JsonResult(new { success = false, message = "Failed to save mapping" });
        }
    }

    [CoreModelBinder]
    public JsonResult GetLcoToISPMapping(LongIdInfo lcoAccountId)
    {
        if(lcoAccountId == null)
        {
            Logger.GetInstance().Info("GetLcoToISPMapping: Lco account id null");
            return new JsonResult(new { success = true, message = "Lco account id is null"} );
        }
        var result = H8Integration.GetLcoToISPMapping(lcoAccountId);
        return new JsonResult(new { success = true, message = "Mapping saved successfully" , data = result} );
    }

    [HttpGet]
    public JsonResult IsServerUp()
    {
        return new JsonResult(new { success = true, message = "Server is up"});
    }

    [HttpGet]
    public JsonResult TestApiFetchRequest()
    {
        string rawMsg = @"{
        ""timeToExecute"":""2025-06-12T00:04:07.6708885Z"",
        ""ver"":""1.0"",
        ""key"":""COMMIT_RECHARGE"",
        ""server"":""customer-prod-V17"",
        ""environment"":""prod"",
        ""payload"":{
            ""nasid"":281474977089180,
            ""mobile"":""9871812808""
        }
    }";

        // Parse the JSON string into a JObject
        JObject msg = JsonConvert.DeserializeObject<JObject>(rawMsg);
    
        string mobile = msg["payload"]["mobile"].ToString();
        Logger.GetInstance().Info($"Processing mobile: {mobile}");

        LongIdInfo nasid = null;
        if (msg["payload"]["nasid"] != null)
        {
            nasid = LongIdInfo.IdParser(long.Parse(msg["payload"]["nasid"].ToString()));
        }

        BasicHttpClient basicHttpClient = new BasicHttpClient(CoreUtil.GetCustomerUrl());

        var response = basicHttpClient.PostAsync("/customer/GetWgPolicy_V2", null, new Dictionary<string, object>()
        {
            { "nasid", nasid?.ToSafeDbObject(1) },
            { "mobile", mobile }
        }).Result;

        var jsonResponse = JsonConvert.DeserializeObject<JObject>(response);

        var dict = jsonResponse["data"]["wgPolicy"].ToObject<Dictionary<string, object>>();
        var passportUser = jsonResponse["data"]["fdmUser"].ToObject<HomeRouterPlan>();
        var currentPlan = jsonResponse["data"]["plan"].ToObject<PDOPlan>();
        nasid = LongIdInfo.IdParser(long.Parse(dict["nasid"].ToString()));

        LongIdInfo customerAccountId = LongIdInfo.IdParser(long.Parse(dict["accountId"].ToString()));
        LongIdInfo lcoAccountId = LongIdInfo.IdParser(long.Parse(jsonResponse["data"]["fdmUser"]["createdBy"].ToString()));
        var fdmId = long.Parse(jsonResponse["data"]["fdmUser"]["id"].ToString());

        return new JsonResult(ResponseStatus.SUCCESS);
    }


}
