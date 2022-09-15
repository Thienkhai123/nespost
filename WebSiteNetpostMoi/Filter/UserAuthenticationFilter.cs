using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using WebSite.Language;
using WebSite.Models;

namespace WebSite.Filter
{
    public class UserAuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        public string _action = "";
        public UserAuthenticationFilter(string action = "")
        {
            _action = action;
        }
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            
        }
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }
        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //}
        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //}
        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //}
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!_action.Contains("login"))
            {
                if (filterContext.HttpContext.Session["User"] == null)
                {
                    filterContext.Controller.TempData["Error"] = Booking.msg_ket_thuc_phien_lam_viec;
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Booking" }));
                }
                if (ClientSession.getCusCode().Equals("0131858"))
                {
                    HttpContext.Current.Session.Timeout = 1440;
                    if (HttpContext.Current.Request.Cookies["CheckTime"] == null)
                    {
                        HttpCookie CheckTime = new HttpCookie("CheckTime");
                        CheckTime.Value = DateTime.Now.ToString();
                        CheckTime.Expires = DateTime.Now.AddDays(1);
                        HttpContext.Current.Response.Cookies.Add(CheckTime);
                    }
                    else
                    {
                        HttpContext.Current.Response.Cookies["CheckTime"].Value = DateTime.Now.ToString();
                    }
                }
            }
            if(_action.Contains("role_Action"))
            {
                if (!string.IsNullOrWhiteSpace(ClientSession.getCustomer().PARENT_USER_NAME) || ClientSession.getUserName().StartsWith("CS") || ClientSession.getUserName().EndsWith("ORDERS"))
                {
                    filterContext.Controller.TempData["AddError"] = Booking.msg_role_using_action;
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "DanhSachDonHang", controller = "Booking" }));
                }
            }

            
        }
    }
}