using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;

namespace APIProject.App_Start
{
    public class UserAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        readonly int Role1 = 0;
        readonly int Role2 = 0;
        public UserAuthenticationFilter()
        {

        }
        public UserAuthenticationFilter(int role1, int role2)
        {
            this.Role1 = role1;
            this.Role2 = role2;
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            UserDetailOutputModel ss = (UserDetailOutputModel)HttpContext.Current.Session[SystemParam.SESSION_LOGIN];
            if (ss != null)
            {
                if ((Role1 > 0 && ss.Role == Role1) || (Role2 > 0 && ss.Role == Role2) )
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues["controller"] = "IntroDuce";
                    routeValues["action"] = "Index";
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
            }

            else
            {
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