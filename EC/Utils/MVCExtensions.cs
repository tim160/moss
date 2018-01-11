using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EC.Utils
{
    public static class MVCExtensions
    {
        public static string RenderPartialView(this Controller controller, string viewName)
        {
            return RenderPartialView(controller, viewName, null, null);
        }

        public static string RenderPartialView(this Controller controller, string viewName, object model, ViewDataDictionary viewBag)
        {
            if (controller.ControllerContext == null)
            {
                controller.ControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current), new RouteData(), controller);
                controller.RouteData.Values.Add("controller", "ChatController");
            }

            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData = viewBag ?? controller.ViewData;
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}