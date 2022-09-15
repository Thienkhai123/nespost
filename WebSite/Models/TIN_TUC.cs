using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Models
{
    public class TINTUCDTO
    {
        public int ID { get; set; }
        public string TIEU_DE { get; set; }
        public string TAC_GIA { get; set; }
        public DateTime THOI_GIAN { get; set; }
        public string THOI_GIAN_STRING { get; set; }
        public string NOI_DUNG { get; set; }
        public string TEN_NHOM_TIN { get; set; }
        public string NHOM_TIN { get; set; }
        public string FILE_IMG { get; set; }
        public string MO_TA { get; set; }
        public string TIEU_DE_EN { get; set; }
        public string NOI_DUNG_EN { get; set; }
        public string MO_TA_EN { get; set; }
        public string TIEU_DE_JA { get; set; }
        public string NOI_DUNG_JA { get; set; }
        public string MO_TA_JA { get; set; }
        public string FILE_DINH_KEM { get; set; }
        public string LAG { get; set; }
        public string DIA_DIEM { get; set; }
        public string MA_DIA_DIEM { get; set; }
        public string VI_TRI { get; set; }
        public string VI_TRI_JA { get; set; }
        public string VI_TRI_EN { get; set; }
        public DateTime HAN_NOP { get; set; }
        public string KINH_NGHIEM { get; set; }
        public string HAN_NOP_STRING { get; set; }
        public int TIN_CHUA_DUYET { get; set; }
        public int TAKE { get; set; }
    }
    public class NHOM_TIN_TUC
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
    }
    public class tbl_Apply
    {
        public int ID { get; set; }
        public int PARENT_ID { get; set; }
        public string HO_TEN { get; set; }
        public string SDT { get; set; }
        public string EMAIL { get; set; }
        public string CV { get; set; }
        public DateTime NGAY_APPLY { get; set; }
        public string TRANG_THAI { get; set; }       
    }
    public class Status_Apply {
        public int ID { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public static class DataProvider
    {
        public static IEnumerable<Status_Apply> GetStatus()
        {
            List<Status_Apply> items = new List<Status_Apply>();
            items.Add(new Status_Apply { ID = 2, Text = "Đã duyệt", Value = "DA_DUYET" });
            items.Add(new Status_Apply { ID = 3, Text = "Từ chối", Value = "TU_CHOI" });
            return items;
        }
    }
}