using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_PRODUCT_TYPEDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_SAN_PHAM { get; set; }
        public String TEN_SAN_PHAM { get; set; }
        public String MA_DINH_DANH { get; set; }
        public String MA_DINH_DANH_TEMPLATE { get; set; }
        public String MA_SP_NHA_CUNG_CAP { get; set; }
        public String MA_SP_NHA_SAN_XUAT { get; set; }
        public String MA_THAM_CHIEU { get; set; }
        public String MA_THAM_CHIEU_2 { get; set; }
        public String MA_DON_VI_TINH { get; set; }
        public String TEN_DON_VI_TINH { get; set; }
        public String MA_DON_VI_TINH_QUY_DOI { get; set; }
        public String TEN_DON_VI_TINH_QUY_DOI { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public Boolean CO_SERI { get; set; }
        public String MA_NHOM_KHO { get; set; }
        public String TEN_DON_VI_NHAP_KHAU { get; set; }
        public String DIA_CHI_DON_VI_NHAP_KHAU { get; set; }
        public String MA_NHA_CUNG_CAP { get; set; }
        public String TEN_NHA_CUNG_CAP { get; set; }
        public String TEN_DON_VI_PHAN_PHOI { get; set; }
        public String DIA_CHI_DON_VI_PHAN_PHOI { get; set; }
        public String NHA_SAN_XUAT { get; set; }
        public String DIA_CHI_NHA_SAN_XUAT { get; set; }
        public String NUOC_SAN_XUAT { get; set; }
        public String MA_NHOM_SAN_PHAM { get; set; }
        public String TEN_NHOM_SAN_PHAM { get; set; }
        public Boolean SAN_PHAM_COMBO { get; set; }
        public String DON_VI_HAN_SU_DUNG { get; set; }
        public Decimal HAN_SU_DUNG { get; set; }
        public String MA_NGANH_HANG { get; set; }
        public String TEN_NGANH_HANG { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_OWNER { get; set; }
        public String MA_SP_OWNER { get; set; }
        public String LOAI_SAN_PHAM { get; set; }
        public Decimal TRONG_LUONG { get; set; }
        public Boolean BAT_BUOC_XUAT_KHO_THEO_HAN_SD { get; set; }
        public decimal SL_TON { get; set; }
        public string NGAY_SX { get; set; }
        public string IMG { get; set; }
        public decimal DON_GIA { get; set; }
        public string MO_TA { get; set; }
        public string URL_IMG1 { get; set; }
        public string URL_IMG2 { get; set; }
        public string URL_IMG3 { get; set; }
        public string URL_IMG4 { get; set; }
        public string URL_IMG5 { get; set; }
        public string TYPE { get; set; }
        #endregion Thuộc tính
    }
}