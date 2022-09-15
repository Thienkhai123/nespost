using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace WebSite.Models.Warehouse
{
    public class CSS_DANH_SACH_MENUDTO
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public String Code { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public String Role { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime TU_NGAY { get; set; }
        public DateTime DEN_NGAY { get; set; }
    }

    public class CSS_DANH_SACH_MENU_THEO_NHAN_VIENDTO
    {
        public int ID { get; set; }
        public String MA_NHAN_VIEN { get; set; }
        public int MENU_ID { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime TU_NGAY { get; set; }
        public DateTime DEN_NGAY { get; set; }
    }
}
