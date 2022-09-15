using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }

        public Int32 STT { get; set; }

        public String MA_YEU_CAU_NHAP_KHO { get; set; }

        public String MA_YEU_CAU_NHAP_KHO_CHI_TIET { get; set; }

        public String MA_SAN_PHAM { get; set; }

        public String TEN_SAN_PHAM { get; set; }

        public String MA_DINH_DANH { get; set; }

        public String MA_SP_NHA_CUNG_CAP { get; set; }

        public String MA_HANG_VINMART { get; set; }

        public String MA_DON_VI_TINH { get; set; }

        public String TEN_DON_VI_TINH { get; set; }

        public Decimal SO_LUONG { get; set; }

        public Decimal DON_GIA_NHAP { get; set; }

        public Decimal THANH_TIEN { get; set; }

        public Decimal VAT { get; set; }

        public Decimal TONG_TIEN { get; set; }

        public Decimal DON_GIA_XUAT { get; set; }

        public String THOI_HAN_SU_DUNG { get; set; }

        public String MA_NHA_CUNG_CAP { get; set; }

        public String TEN_NHA_CUNG_CAP { get; set; }

        public String PHAN_LOAI_SP { get; set; }

        public String MA_DON_VI_TINH_QUY_DOI { get; set; }

        public String TEN_DON_VI_TINH_QUY_DOI { get; set; }

        public Decimal SO_LUONG_QUY_DOI { get; set; }

        public Decimal TY_LE_QUY_DOI { get; set; }

        public String GHI_CHU { get; set; }

        public String INSERT_USER { get; set; }

        public DateTime INSERT_DATE { get; set; }

        public DateTime INSERT_TIME { get; set; }

        public String UPDATE_USER { get; set; }

        public String PHAN_LOAI_KHAY { get; set; }

        public DateTime UPDATE_DATE { get; set; }

        public DateTime UPDATE_TIME { get; set; }

        public DateTime NGAY_SAN_XUAT { get; set; }

        public String DON_VI_HAN_SU_DUNG { get; set; }

        public Decimal HAN_SU_DUNG { get; set; }

        public DateTime NGAY_HET_HAN { get; set; }

        #endregion Thuộc tính
    }
}