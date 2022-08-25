using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using Data.Utils;

namespace APIProject.App_Start
{
    public class RolerAuthenticationFilter : ActionFilterAttribute
    {
        readonly int Role;
        public RolerAuthenticationFilter(int _role)
        {
            this.Role = _role;
        }
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            
            UserDetailOutputModel ss = (UserDetailOutputModel)HttpContext.Current.Session[SystemParam.SESSION_LOGIN];
            if (filterContext.HttpContext.Session[SystemParam.SESSION_LOGIN] != null||ss.Role!=Role)
            {
                //Lay ra contronller hien tai
                //Chuyen ve trang dang nhap
                var routeValues = new RouteValueDictionary();
                routeValues["controller"] = "Home";
                routeValues["action"] = "Login";
                filterContext.Result = new RedirectToRouteResult(routeValues);
            }
            
        }

        //Runs after the OnAuthentication method  
        //------------//
        //OnAuthenticationChallenge:- if Method gets called when Authentication or Authorization is 
        //failed and this method is called after
        //Execution of Action Method but before rendering of View
        //------------//
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //We are checking Result is null or Result is HttpUnauthorizedResult 
            // if yes then we are Redirect to Error View
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error"
                };
            }
        }
    }
}