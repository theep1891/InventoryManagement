using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BaTecInventory
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                 defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }

            );
        }
    }



    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public class NoDirectAccessAttribute : ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        if (filterContext.HttpContext.Request.UrlReferrer == null ||
    //            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
    //        {
    //            filterContext.Result = new RedirectToRouteResult(new
    //                RouteValueDictionary(new { controller = "Account", action = "Login", id = UrlParameter.Optional }));
    //        }
    //    }
    //}

}

