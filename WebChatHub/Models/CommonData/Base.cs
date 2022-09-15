using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebChatHub.Models
{
    public class BaseRequest
    {
        public string RequestType { get; set; }
        public string RequestData { get; set; }
        public string LoginTime { get; set; }
    }
    public class BaseResponse
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public string ResponseData { get; set; }
    }
    public static class BaseResponseEX
    {
        public static bool IsError(this BaseResponse obj)
        {
            return obj.Status == "NOT OK";
        }
    }
    public class BaseRequestAndroid
    {
        public string RequestType { get; set; }
        public string RequestData { get; set; }
        public string LoginTime { get; set; }
    }
    public class BaseResponseAndroid
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public string ResponseData { get; set; }
        public string ResponseData_EX { get; set; }
        public bool IsError
        {
            get
            {
                if (Status.Equals("NOT OK"))
                {
                    return true;
                }
                else
                    return false;
            }
        }
    }
    public class getjson
    {
        public BaseRequestAndroid request;
    }
    public class rq
    {
        public String requestData { get; set; }
    }
    public class response
    {
        public string CallRequestResult;
    }
    public class CSS_OP_TAT_TRACKING_INFODTO
    {
        public string HAWB_NO { get; set; }
        public string DESC { get; set; }
        public string INSERT_TIME { get; set; }
        public string URLImage { get; set; }
    }
    public class CSS_OP_TAT_TRACKING_INFO
    {
        public string HAWB_NO { get; set; }
    }
    public class BaseResponseWebSGEV
    {
        public string Status { get; set; }
        public string Description { get; set; }
        public string ResponseData { get; set; }
        public ResponseTinhHuyenXa ResponseTinhHuyenXa { get; set; }
    }

    public class ResponseTinhHuyenXa
    {
        public string DataTinh { get; set; }
        public string DataHuyen { get; set; }
        public string DataXa { get; set; }
        public string DataCust { get; set; }
    }

    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }        
    }

}