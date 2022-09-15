using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class MessageDetail
    {
        public int ID { get; set; }
        public string PHIEN_LAM_VIEC { get; set; }
        public string Fr_UserID { get; set; }
        public string Fr_UserCode { get; set; }
        public string Fr_UserName { get; set; }
        public string Fr_UserRole { get; set; }
        public string Message { get; set; }
        public string email { get; set; }
        public bool isRead { get; set; }
        public DateTime Time { get; set; }
        public string Time_String { get; set; }
        public DateTime Insert_Time { get; set; }
        public string To_UserName { get; set; }
        public string To_UserCode { get; set; }
        public string To_CompanyCode { get; set; }
        public string To_UserID { get; set; }
        public int UnRead { get; set; }
        public string STATION_CODE { get; set; }
        public string STATION_NAME { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GHI_CHU { get; set; }
        public string Loai_DV { get; set; }
        public string laguage { get; set; }
    }
}