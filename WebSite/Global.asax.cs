using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebSite.Controllers;

namespace WebSite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string culture = "vi";//ngon ngu mac dinh
            var a = Request.RawUrl;
            if (a.EndsWith(".html"))
            {
                HttpContext.Current.RewritePath("/Home/Index");
            }
            if (Request.Cookies.AllKeys.Contains("language"))
            {
                culture = Request.Cookies["language"].Value;
                if (culture != "vi")
                {
                    Response.Cookies["language"].Value = "vi";
                }
            }
            else
            {
                HttpCookie language = new HttpCookie("language");
                language.Value = "vi";
                language.Expires = DateTime.Now.AddMinutes(1);
                Response.Cookies.Add(language);
            }

            if (Request.Cookies.AllKeys.Contains("UserOnline"))
            {
                Request.Cookies["UserOnline"].Expires = DateTime.Now.AddDays(15);
            }
            else
            {
                HttpCookie user = new HttpCookie("UserOnline");
                Models.CSS_CRM_NGUOI_DUNGDTO dto = new Models.CSS_CRM_NGUOI_DUNGDTO { USER_NAME = Guid.NewGuid().ToString(), COMPANY_CODE = "" };
                user.Value = JsonConvert.SerializeObject(dto);
                user.Expires = DateTime.Now.AddDays(15);
                Response.Cookies.Add(user);
            }
            culture = "vi";
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        protected void Session_Start()
        {
            //string strHostName = "";
            //strHostName = System.Net.Dns.GetHostName();
            //IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            //IPAddress[] addr = ipEntry.AddressList;
            //if (addr.Count() > 0)
            //{
            //    Session["KHACH"] = addr[1].ToString().Replace(".", "-");
            //}
            HttpContext.Current.Session.Timeout = 720;
            //Session["KHACH"] = Guid.NewGuid().ToString();
            //var d = Session["KHACH"].ToString();
        }

        public void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();

            string CurrentURL = Request.Url.AbsolutePath;

            string FilePathExten = Request.CurrentExecutionFilePathExtension;

            if (!string.IsNullOrWhiteSpace(FilePathExten))
            {
                return;
            }

            string lag = "en";
            bool isLienHe = false;

            if (CurrentURL.ToUpper().Contains("ja".ToUpper()) || CurrentURL.ToUpper().Contains("jp".ToUpper()))
                lag = "ja";
            else if(CurrentURL.ToUpper().Contains("vi".ToUpper()) || CurrentURL.ToUpper().Contains("vn".ToUpper()))
            {
                lag = "vi";
            }
            lag = "vi";
            if (CurrentURL.ToUpper().Contains("LIENHE") || CurrentURL.ToUpper().Contains("ja-jp".ToUpper()))
            {
                isLienHe = true;
            }

            if (exception.GetType() == typeof(HttpException))
            {
                var statuscode = (int)((HttpException)exception).GetHttpCode();
                if (statuscode == 404)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Lien_He");
                    routeData.Values.Add("action", "Error");
                    routeData.Values.Add("statusCode", statuscode);
                    routeData.Values.Add("lag", lag);
                    routeData.Values.Add("isLienHe", isLienHe);

                    Response.TrySkipIisCustomErrors = true;
                    IController controller = new Lien_HeController();
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();
                }
            }
        }
    }
}