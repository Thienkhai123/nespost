using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_YEU_CAU_NHAP_KHODTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_YEU_CAU_NHAP_KHO { get; set; }
        public DateTime NGAY_LAP { get; set; }
        public DateTime GIO_LAP { get; set; }
        public DateTime NGAY_DINH_NHAP_KHO { get; set; }
        public DateTime GIO_DINH_NHAP_KHO { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public String TRANG_THAI_SHOW { get; set; }
        public String TU_MA_WARE_HOUSE { get; set; }
        public String TU_TEN_WARE_HOUSE { get; set; }
        public String DEN_MA_WARE_HOUSE { get; set; }
        public String DEN_TEN_WARE_HOUSE { get; set; }
        public String TU_DIA_CHI_DAY_DU { get; set; }
        public String TU_NGUOI_LIEN_HE { get; set; }
        public String TU_EMAIL_NGUOI_LIEN_HE { get; set; }
        public String TU_SO_DIEN_THOAI_NGUOI_LIEN_HE { get; set; }
        public String TU_DIA_CHI_NGUOI_LIEN_HE { get; set; }
        public String DEN_DIA_CHI_DAY_DU { get; set; }
        public String DEN_NGUOI_LIEN_HE { get; set; }
        public String DEN_EMAIL_NGUOI_LIEN_HE { get; set; }
        public String DEN_SO_DIEN_THOAI_NGUOI_LIEN_HE { get; set; }
        public String DEN_DIA_CHI_NGUOI_LIEN_HE { get; set; }
        public String MA_PO_NHA_CUNG_CAP { get; set; }
        public String MA_KHACH_HANG { get; set; }
        public String TEN_KHACH_HANG { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_OWNER { get; set; }
        public String MA_NHA_CUNG_CAP { get; set; }
        public String TEN_NHA_CUNG_CAP { get; set; }
        public String LOAI_NHAP_KHO { get; set; }
        public List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> LstYeuCauChiTiet { get; set; }

        #endregion Thuộc tính
    }
}