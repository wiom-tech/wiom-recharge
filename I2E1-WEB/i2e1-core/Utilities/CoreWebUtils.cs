using i2e1_basics.Utilities;
using i2e1_core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace i2e1_core.Utilities
{
    public class CoreWebUtils
    {
        public static void submitDeviceConfig(DeviceConfig config, LongIdInfo userId)
        {
            config.macid = i2e1_core.Utilities.CoreUtil.GetNormalisedMac(config.macid);
            if (string.IsNullOrEmpty(config.devicePassword))
                config.devicePassword = "spartans@000" + config.nasid;
            CoreDbCalls.GetInstance().SubmitConfig(config, userId);
            CoreCacheHelper.GetInstance().Reset(CoreCacheHelper.NAS_FROM_MACV2, config.macid);
        }

        public static async Task<string> RenderRazorViewToString(Controller controller, string viewPath, object model)
        {
            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
                ViewEngineResult viewResult = viewEngine.GetView("", viewPath, true);

                var viewData = controller.ViewData; // Ideally copy this or make a fake of it
                viewData.Model = model;
                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    viewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString().Replace("\r", ""); ;
            }
        }
    }
}
