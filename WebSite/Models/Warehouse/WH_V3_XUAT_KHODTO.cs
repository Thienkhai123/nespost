using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_XUAT_KHODTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_XUAT_KHO { get; set; }
        public String MA_YEU_CAU_XUAT_KHO { get; set; }
        public String MA_OWNER { get; set; }
        public String MA_NHAN_VIEN { get; set; }
        public String TEN_NHAN_VIEN { get; set; }
        public DateTime NGAY_XUAT_KHO { get; set; }
        public DateTime GIO_XUAT_KHO { get; set; }
        public DateTime NGAY_XUAT_DU_KIEN { get; set; }
        public DateTime GIO_XUAT_DU_KIEN { get; set; }
        public String TU_MA_WARE_HOUSE { get; set; }
        public String TU_TEN_WARE_HOUSE { get; set; }
        public String DEN_MA_WARE_HOUSE { get; set; }
        public String DEN_TEN_WARE_HOUSE { get; set; }
        public String MA_KHACH_HANG { get; set; }
        public String TEN_KHACH_HANG { get; set; }
        public String MA_NHA_CUNG_CAP { get; set; }
        public String MA_LOAI_XUAT_KHO { get; set; }
        public String TEN_NHA_CUNG_CAP { get; set; }
        public String MA_NV_GIAO_HANG { get; set; }
        public String TEN_NV_GIAO_HANG { get; set; }
        public String KH_DIA_CHI_DAY_DU { get; set; }
        public String KH_NGUOI_LIEN_HE { get; set; }
        public String KH_EMAIL_NGUOI_LIEN_HE { get; set; }
        public String KH_SO_DIEN_THOAI_NGUOI_LIEN_HE { get; set; }
        public String KH_DIA_CHI_NGUOI_LIEN_HE { get; set; }
        public String MA_TO { get; set; }
        public String MA_PO_CUA_HANG { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public String TRANG_THAI_SHOW { get; set; }
        public Boolean DA_BOOKING { get; set; }
        public Boolean XAC_NHAN_DO_BAN_GIAO_TTGD { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_TUYEN_KET_NOI { get; set; }
        public String TEN_TUYEN_KET_NOI { get; set; }
        public String BIEN_SO_XE { get; set; }
        public String TEN_DIEM_PHAT_HANG { get; set; }
        public DateTime GIO_PHAT_HANG_DU_KIEN { get; set; }
        public Boolean IS_PRINTED { get; set; }
        public Boolean DA_DIEU_CHINH_CCDC { get; set; }
        public Boolean IS_LAY_SO_LUONG_PHAI_THU { get; set; }
        public String LOAI_FARM_XUAT { get; set; }
        public String DICH_VU { get; set; }
        #endregion Thuộc tính
        #region Thuộc tính thêm
        public DateTime TU_NGAY { get; set; }
        public DateTime DEN_NGAY { get; set; }
        public string SEARCH_BY { get; set; }
        public List<WH_V3_XUAT_KHO_CHI_TIETDTO> ListXuatKhoChiTiet { get; set; }
        public string MA_NHOM_KHO { get; set; }
        public string MA_TINH { get; set; }
        public string MA_HUYEN { get; set; }
        public string DIA_CHI_SO_NHA { get; set; }
        public string MA_KHO_KH { get; set; }
        public string TEN_TINH { get; set; }
        public string TEN_HUYEN { get; set; }
        public string SO_DIEN_THOAI { get; set; }
        #endregion
    }
}