using i2e1_basics.Models;
using i2e1_core.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace i2e1_core.Utilities;

[ApiLog]
public class BaseController : Controller
{
    public ActionResult ViewTo(string view)
    {
        return ViewTo(view, null);
    }

    public ActionResult ViewTo(string view, object model)
    {
        return base.View(view, model);
    }

    public JsonResult JsonResult(object obj)
    {
        return new JsonResult(obj);
    }

    public JsonResult JsonResult(ResponseStatus status)
    {
        return new JsonResult(new JsonResponse(status, null));
    }


    public JsonResult JsonResult(ResponseStatus status, object obj)
    {
        return new JsonResult(new JsonResponse(status, obj));
    }

    public JsonResult JsonResult(ResponseStatus status, string msg, object obj)
    {
        return new JsonResult(new JsonResponse(status, msg, obj));
    }
    public JsonResult JsonResult(ResponseStatus status, string msg, object obj, object settings)
    {
        return new JsonResult(new JsonResponse(status, msg, obj, settings));
    }

    public JsonResult JsonResult(ErrorCode error, string msg = null)
    {
        return new JsonResult(new ErrorResponse(ResponseStatus.FAILURE, error, msg));
    }
}
