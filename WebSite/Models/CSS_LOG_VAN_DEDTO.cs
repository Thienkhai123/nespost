using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class CSS_LOG_VAN_DEDTO
    {
        public Int32 ID { get; set; }
        public Int32 STT { get; set; }
        public string MA_CHUC_NANG { get; set; }
        public string TEN_CHUC_NANG { get; set; }
        public string MA_NHAN_VIEN_DANG_XU_LY { get; set; }
        public string TEN_NHAN_VIEN_DANG_XU_LY { get; set; }
        public string MA_NHOM_NGUYEN_NHAN_CAP_1 { get; set; }
        public string MA_NHOM_NGUYEN_NHAN_CAP_2 { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public string INSERT_USER { get; set; }
        public string INSERT_USER_NAME { get; set; }
        public string MA_THAM_CHIEU { get; set; }
        public string MA_LOAI_NGUYEN_NHAN { get; set; }
        public string TEN_LOAI_NGUYEN_NHAN { get; set; }
        public Decimal VALUE_1 { get; set; }
        public DateTime TU_NGAY { get; set; } = DateTime.Now;
        public DateTime DEN_NGAY { get; set; } = DateTime.Now;
        public string GHI_CHU { get; set; }
        public string PHI_DONG_GOI { get; set; }
        public string PHI_KIEM_DICH { get; set; }
        public string PHI_PHAT_LAI { get; set; }
        public string PHI_XE_NC { get; set; }
        public string PHI_XU_LY_KHAC { get; set; }
        public decimal VALUE_2
        {
            get;
            set;
        }
        public decimal VALUE_3
        {
            get;
            set;
        }
        public string TEN_NHOM_NGUYEN_NHAN_CAP_1 { get; set; }
        public decimal TRONG_LUONG { get; set; }
        public int SO_KIEN_DONG_GOI { get; set; }
        public decimal TRONG_LUONG_CHENH_LECH { get; set; }
        public int SO_KIEN_CHENH_LECH { get; set; }
    }
}