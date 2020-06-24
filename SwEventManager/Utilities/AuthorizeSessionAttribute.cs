using Microsoft.AspNet.Identity;
using SwEventManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SwEventManager.Utilities
{
    public class SessionCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["User"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                                { "Controller", "Home" },
                                { "Action", "Index" }
                                });
            }
        }
    }

    public class AdminCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            if (session != null && session["User"] != null)
            {
                    if(!session["IsAdmin"].Equals("True"))
                    {
                            filterContext.Result = new RedirectToRouteResult(
                   new RouteValueDictionary {
                                { "Controller", "UserEvents" },
                                { "Action", "Index" }
                               }); 
                    }              
            }
        }
    }


}