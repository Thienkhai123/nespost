using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebChatHub.Models
{
    public class Service
    {
        public static string Error(string error)
        {
            JMessage msg = new JMessage();
            msg.ID = 0;
            msg.Error = true;
            msg.Title = error;
            return JsonConvert.SerializeObject(msg);
        }
        public static JMessage Error(Exception e)
        {
            JMessage msg = new JMessage();
            msg.ID = 0;
            msg.Error = true;
            msg.Title = "Lỗi: " + Environment.NewLine + e.Message;
            return msg;
        }
        public static JMessage ErrorV2(string error)
        {
            JMessage msg = new JMessage();
            msg.ID = 0;
            msg.Error = true;
            msg.Title = error;
            return msg;
        }

        public static string OK(object lst = null, string description = null)
        {
            JMessage msg = new JMessage();
            msg.ID = 1;
            msg.Error = false;
            msg.Title = description;
            msg.Object = lst;
            return JsonConvert.SerializeObject(msg);
        }
        public static JMessage OK_V1(object lst = null, string description = null)
        {
            JMessage msg = new JMessage();
            msg.ID = 1;
            msg.Error = false;
            msg.Title = description;
            msg.Object = lst;
            return msg;
        }
        public static JMessage Ok_V2(object lst1 = null, string description = null, object lst2 = null)
        {
            JMessage msg = new JMessage();
            msg.Error = false;
            msg.Title = description;
            msg.Object = lst1;
            msg.Object2 = lst2;
            msg.ID = 1;
            return msg;
        }
    }
    public class JMessage
    {
        public Int32 ID { get; set; }
        public bool Error { get; set; }
        public string Title { get; set; }
        public Object Object { get; set; }
        public Object Object2 { get; set; }
        public String TenNguoiGui { get; set; }
        public string DiaChiNguoiGui { get; set; }
    }
    public class Message
    {
        public bool isError { get; set; }
        public bool isSuccess { get; set; }
        public string Mess { get; set; }
    }
}