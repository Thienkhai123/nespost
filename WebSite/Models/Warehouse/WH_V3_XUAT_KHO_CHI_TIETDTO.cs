using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_XUAT_KHO_CHI_TIETDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_XUAT_KHO { get; set; }
        public String MA_XUAT_KHO_CHI_TIET { get; set; }
        public String MA_YEU_CAU_XUAT_KHO_CHI_TIET { get; set; }
        public String MA_HANG_VINMART { get; set; }
        public String MA_OWNER { get; set; }
        public String MA_THUNG { get; set; }
        public Int32 STT { get; set; }
        public String MA_SAN_PHAM { get; set; }
        public String TEN_SAN_PHAM { get; set; }
        public String MA_DINH_DANH { get; set; }
        public String MA_SP_NHA_CUNG_CAP { get; set; }
        public String MA_THAM_CHIEU_2 { get; set; }
        public String PHAN_LOAI_SP { get; set; }
        public String MA_DON_VI_TINH { get; set; }
        public String TEN_DON_VI_TINH { get; set; }
        public String MANAGEMENT_DATA_CODE { get; set; }
        public Decimal SO_LUONG { get; set; }
        public String MA_DON_VI_TINH_QUY_DOI { get; set; }
        public String TEN_DON_VI_TINH_QUY_DOI { get; set; }
        public Decimal SO_LUONG_QUY_DOI { get; set; }
        public Decimal TY_LE_QUY_DOI { get; set; }
        public Decimal TY_LE_QUY_DOI_MAC_DINH { get; set; }
        public Decimal SO_LUONG_TON_THUC_TE { get; set; }
        public Decimal SO_LUONG_DIEU_CHINH { get; set; }
        public Decimal SO_LUONG_QUY_DOI_TON_THUC_TE { get; set; }
        public Decimal SO_LUONG_QUY_DOI_DIEU_CHINH { get; set; }
        public String TU_MA_LOCATION { get; set; }
        public String TU_TEN_LOCATION { get; set; }
        public String DEN_MA_LOCATION { get; set; }
        public String DEN_TEN_LOCATION { get; set; }
        public String TU_MA_CELL { get; set; }
        public String TU_TEN_CELL { get; set; }
        public String DEN_MA_CELL { get; set; }
        public String DEN_TEN_CELL { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public String MA_PO_FILE_CHIA { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public Decimal DON_GIA_XUAT { get; set; }
        public Decimal THANH_TIEN { get; set; }
        public Decimal VAT { get; set; }
        public Decimal TONG_TIEN { get; set; }
        public DateTime NGAY_NHAP_KHO { get; set; }
        public String MA_NHAP_KHO { get; set; }
        public DateTime NGAY_SAN_XUAT { get; set; }
        public String DON_VI_HAN_SU_DUNG { get; set; }
        public Decimal HAN_SU_DUNG { get; set; }
        public DateTime NGAY_HET_HAN { get; set; }
        public string MA_SO { get; set; }
        public string MA_PO_CUA_HANG { get; set; }
        #endregion Thuộc tính
    }
}