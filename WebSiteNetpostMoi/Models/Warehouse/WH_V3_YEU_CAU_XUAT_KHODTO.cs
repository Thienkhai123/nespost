using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_YEU_CAU_XUAT_KHODTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_YEU_CAU_XUAT_KHO_TONG { get; set; }
        public String MA_YEU_CAU_XUAT_KHO { get; set; }
        public String MA_OWNER { get; set; }
        public DateTime NGAY_LAP { get; set; }
        public DateTime GIO_LAP { get; set; }
        public DateTime NGAY_DINH_XUAT_KHO { get; set; }
        public DateTime GIO_DINH_XUAT_KHO { get; set; }
        public String GHI_CHU { get; set; }
        public String TU_MA_WARE_HOUSE { get; set; }
        public String TU_TEN_WARE_HOUSE { get; set; }
        public String DEN_MA_WARE_HOUSE { get; set; }
        public String DEN_TEN_WARE_HOUSE { get; set; }
        public String MA_KHACH_HANG { get; set; }
        public String TEN_KHACH_HANG { get; set; }
        public String MA_NV_GIAO_HANG { get; set; }
        public String TEN_NV_GIAO_HANG { get; set; }
        public String KH_DIA_CHI_DAY_DU { get; set; }
        public String KH_NGUOI_LIEN_HE { get; set; }
        public String KH_EMAIL_NGUOI_LIEN_HE { get; set; }
        public String KH_SO_DIEN_THOAI_NGUOI_LIEN_HE { get; set; }
        public String KH_DIA_CHI_NGUOI_LIEN_HE { get; set; }
        public String MA_TO { get; set; }
        public String MA_MAU_XUAT_KHO { get; set; }
        public String MA_NHA_CUNG_CAP { get; set; }
        public String TEN_NHA_CUNG_CAP { get; set; }
        public String TRANG_THAI { get; set; }
        public String MA_PO_CUA_HANG { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_LOAI_XUAT_KHO { get; set; }
        public String BIEN_SO_XE { get; set; }
        public String LOAI_FARM_XUAT { get; set; }
        #endregion Thuộc tính
    }
}