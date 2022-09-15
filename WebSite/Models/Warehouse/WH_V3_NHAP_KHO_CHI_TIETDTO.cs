using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_NHAP_KHO_CHI_TIETDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_NHAP_KHO { get; set; }
        public String MA_NHAP_KHO_CHI_TIET { get; set; }
        public String MA_YEU_CAU_NHAP_KHO_CHI_TIET { get; set; }
        public String MA_HANG_VINMART { get; set; }
        public Int32 STT { get; set; }
        public String MA_SAN_PHAM { get; set; }
        public String TEN_SAN_PHAM { get; set; }
        public String MA_DINH_DANH { get; set; }
        public String MA_SP_NHA_CUNG_CAP { get; set; }
        public String MA_DON_VI_TINH { get; set; }
        public String TEN_DON_VI_TINH { get; set; }
        public Decimal SO_LUONG { get; set; }
        public String MA_DON_VI_TINH_QUY_DOI { get; set; }
        public String TEN_DON_VI_TINH_QUY_DOI { get; set; }
        public Decimal SO_LUONG_QUY_DOI { get; set; }
        public Decimal TY_LE_QUY_DOI { get; set; }
        public String GHI_CHU { get; set; }
        public Decimal DON_GIA_NHAP { get; set; }
        public Decimal DON_GIA_XUAT { get; set; }
        public Decimal THANH_TIEN { get; set; }
        public Decimal VAT { get; set; }
        public Decimal TONG_TIEN { get; set; }
        public Decimal SO_LUONG_YEU_CAU { get; set; }
        public String PHAN_LOAI_SP { get; set; }
        public String MA_OWNER { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public DateTime NGAY_SAN_XUAT { get; set; }
        public String DON_VI_HAN_SU_DUNG { get; set; }
        public Decimal HAN_SU_DUNG { get; set; }
        public DateTime NGAY_HET_HAN { get; set; }
        public String LOAI_NHAP { get; set; }
        public String PHAN_LOAI_KHAY { get; set; }


        public Decimal SO_LUONG_LUY_KE
        {
            get;
            set;
        }
        public Decimal SO_LUONG_CON_LAI { get; set; }
        public String MA_NHA_CUNG_CAP
        {
            get;
            set;
        }
        public String TEN_NHA_CUNG_CAP
        {
            get;
            set;
        }
        public String MA_KHACH_HANG
        {
            get;
            set;
        }
        public String TEN_KHACH_HANG
        {
            get;
            set;
        }
        public String MA_PO
        {
            get;
            set;
        }
        public String MA_YEU_CAU_NHAP_KHO
        {
            get;
            set;
        }
        public Decimal SO_LUONG_DA_DONG_THUNG { get; set; }
        public String SO_LO { get; set; }
        public Decimal TONG_TRONG_LUONG { get; set; }
        public Decimal TRONG_LUONG_KHAY { get; set; }
        public Decimal SO_LUONG_KHAY { get; set; }
        public String FARM { get; set; }
        public String MANAGEMENT_DATA_CODE { get; set; }
        public Decimal SO_LUONG_CHENH_LECH { get; set; }
        public String MA_SP_NHA_SAN_XUAT { get; set; }
        public Decimal SO_LUONG_DA_THUC_HIEN { get; set; }
        public DateTime GIO_NHAP_KHO { get; set; }
        public DateTime NGAY_NHAP_KHO { get; set; }
        public String DON_VI_TINH_YEU_CAU { get; set; }
        public Decimal SO_LUONG_DON_VI_TINH_YEU_CAU { get; set; }
        public Decimal SO_LUONG_DON_VI_TINH_QUY_DOI_YEU_CAU { get; set; }
        public Decimal CHENH_LECH_DON_VI_TINH { get; set; }
        public String MA_THAM_CHIEU_2 { get; set; }
        public String MA_THAM_CHIEU { get; set; }
        public String TRANG_THAI { get; set; }
        public DateTime NGAY_DAT_HANG { get; set; }
        public decimal SO_LUONG_DAT_HANG { get; set; }

        public decimal SL_KHAY_NHUA_NHO { get; set; }

        public decimal SL_KHAY_NHUA_LON { get; set; }

        public decimal SL_THUNG_BAO { get; set; }


        #endregion Thuộc tính
    }
}