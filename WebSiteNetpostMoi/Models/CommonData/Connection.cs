using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace WebSite.Models
{
    public class Connection
    {
        public static string addressV1;
        public static string addressV2;
        public static BaseResponseWebSGEV KetNoiDangNhap(string RequestType, string RequestData)
        {
            if (string.IsNullOrWhiteSpace(addressV2) || string.IsNullOrWhiteSpace(addressV1))
            {
                getAddress();
            }
            DateTime date = DateTime.Now;
            string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(new getjson() { request = new BaseRequestAndroid() { RequestType = RequestType, RequestData = RequestData, LoginTime = "" } });
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(new rq() { requestData = json2 });
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(addressV1);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (Stream webStream = httpWebRequest.GetRequestStream())
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json1);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var rs = streamReader.ReadToEnd();
            response c = JsonConvert.DeserializeObject<response>(rs);
            return JsonConvert.DeserializeObject<BaseResponseWebSGEV>(c.CallRequestResult);
        }

        public static BaseResponseAndroid KetNoiLayDuLieu(string RequestType, string RequestData)
        {
            if (string.IsNullOrWhiteSpace(addressV2) || string.IsNullOrWhiteSpace(addressV1))
            {
                getAddress();
            }
            string json2 = Newtonsoft.Json.JsonConvert.SerializeObject(new getjson() { request = new BaseRequestAndroid() { RequestType = RequestType, RequestData = RequestData, LoginTime = "" } });
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(new rq() { requestData = json2 });
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(addressV1);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = 500000;
            using (Stream webStream = httpWebRequest.GetRequestStream())
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json1);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var rs = streamReader.ReadToEnd();
            response c = JsonConvert.DeserializeObject<response>(rs);
            return JsonConvert.DeserializeObject<BaseResponseAndroid>(c.CallRequestResult);
        }


        public static void getAddress()
        {
            string path = System.Web.HttpContext.Current.Server.MapPath(@"\Config\servicesConfig.xml");
            XDocument xdc = XDocument.Load(path);
            List<XElement> childList =
                            (from el in xdc.Root.Elements()
                             select el).ToList();
            addressV1 = childList[0].Value;
            addressV2 = childList[1].Value;

            //XElement add = xdc.Element("service").Elements("service").FirstOrDefault(a => (string)a.Attribute("key") == "v1");

        }
        public static BaseResponseAndroid CallService(BaseRequest request, int timeoutMinutes = 5)
        {
            if (string.IsNullOrWhiteSpace(addressV2) || string.IsNullOrWhiteSpace(addressV1))
            {
                getAddress();
            }
            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            timeoutMinutes = timeoutMinutes * 60 * 1000;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(addressV2);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = timeoutMinutes;
            using (Stream webStream = httpWebRequest.GetRequestStream())
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json1);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            var streamReader = new StreamReader(httpResponse.GetResponseStream());

            var rs = streamReader.ReadToEnd();
            BaseResponseAndroid c = JsonConvert.DeserializeObject<BaseResponseAndroid>(rs);
            return c;
        }
        public static BaseResponseAndroid CallService(string RequestType, IList request = null, int timeoutMinutes = 5)
        {
            BaseRequest req = new BaseRequest();
            req.RequestType = RequestType;
            req.RequestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            return CallService(req);
        }
        public static BaseResponseAndroid CallServiceObject(string RequestType, object request = null, int timeoutMinutes = 5)
        {
            BaseRequest req = new BaseRequest();
            req.RequestType = RequestType;
            req.RequestData = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            return CallService(req);
        }
    }
}