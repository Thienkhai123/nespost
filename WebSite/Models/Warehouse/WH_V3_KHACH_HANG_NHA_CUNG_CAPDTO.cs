using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_DOI_TUONG { get; set; }
        public String TEN_DOI_TUONG { get; set; }
        public String MA_LOAI_DOI_TUONG { get; set; }
        public String MA_CHA { get; set; }
        public String MA_THAM_CHIEU_DOI_TAC { get; set; }
        public String MA_THAM_CHIEU_NOI_BO { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public String NGUOI_LIEN_HE_TEN { get; set; }
        public String NGUOI_LIEN_HE_EMAIL { get; set; }
        public String NGUOI_LIEN_HE_SO_DIEN_THOAI { get; set; }
        public String NGUOI_LIEN_HE_DIA_CHI { get; set; }
        public String NGUOI_LIEN_HE_FAX { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_NHOM_KHO { get; set; }
        #endregion Thuộc tính
    }
}