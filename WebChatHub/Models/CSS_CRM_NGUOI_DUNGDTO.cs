using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChatHub.Models
{
    public class CSS_CRM_NGUOI_DUNGDTO
    {
        public int ID { get; set; }
        public String CUSTOMER_CODE { get; set; }
        public String USER_NAME { get; set; }
        public String PASSWORD { get; set; }
        public String GHI_CHU { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME {  get;set; }
        public string LOAI_TK { get; set; }
        public string DANH_SACH_KH { get; set; }
        public string NAME { get; set; }
        public string login { get; set; }
        public string MA_KHACH_HANG_CON { get; set; }
        public string TEN_KHACH_HANG_CON { get; set; }
        public string FULL_NAME { get; set; }
        public string STATION_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string EMAIL { get; set; }
        public string TEMP { get; set; }
        public string PARENT_USER_NAME { get; set; }
        public int IS_ACTIVE { get; set; }
        public string LOAI_USER { get; set; }
        public string STATION_NAME { get; set; }

    }
}