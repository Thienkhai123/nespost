using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class BaoCaoXuatNhapTonKhoVinMartParaDTO
    {
        #region Thuộc tính
        public String MA_SAN_PHAM
        {
            get;
            set;
        }
        public String MA_WARE_HOUSE
        {
            get;
            set;
        }
        public String MA_CELL
        {
            get;
            set;
        }
        public String TEN_LOAI_LOCATION
        {
            get;
            set;
        }
        public String TEN_LOCATION
        {
            get;
            set;
        }
        public String TEN_CELL
        {
            get;
            set;
        }
        public String MA_DON_VI_TINH
        {
            get;
            set;
        }
        public String TEN_DON_VI_TINH
        {
            get;
            set;
        }
        public String MA_SP_NCC
        {
            get;
            set;
        }
        public String TEN_SAN_PHAM
        {
            get;
            set;
        }
        public Decimal TONG_TL_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_NHAP_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_CHO_XU_LY_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_HANG_HONG_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_KIEM_DEM_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_NHAP_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_CHO_XU_LY_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_HANG_HONG_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_KIEM_DEM_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_HANG_KIEM_DEM_TRONG_KY
        {
            get;
            set;
        }
        public Decimal TRONG_LUONG_NHAP
        {
            get;
            set;
        }
        public Decimal TRONG_LUONG_XUAT
        {
            get;
            set;
        }
        public Decimal TONG_TL_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_TAM_NHAP_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_CHO_XUAT_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TL_TAM_NHAP_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal TL_CHO_XUAT_TON_CUOI_KY
        {
            get;
            set;
        }
        public String MA_OWNER
        {
            get;
            set;
        }
        public String MA_HANG_VINMART
        {
            get;
            set;
        }
        public String FARM
        {
            get;
            set;
        }
        public String FARM_CODE
        {
            get;
            set;
        }
        public DateTime NGAY_BUT_TOAN
        {
            get;
            set;
        }
        public Decimal DONG_GOI_XONG_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal DA_TACH_HUY_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal CHO_TACH_HUY_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal CHO_DONG_GOI_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal DONG_GOI_XONG_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal DA_TACH_HUY_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal CHO_TACH_HUY_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal CHO_DONG_GOI_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal NHAP_KHO_DO_DONG_HUY_COMBO
        {
            get;
            set;
        }
        public Decimal XUAT_KHO_DO_DONG_HUY_COMBO
        {
            get;
            set;
        }
        public bool IS_COMBO
        {
            get;
            set;
        }
        public String MA_BUT_TOAN_THAM_CHIEU
        {
            get;
            set;
        }
        public String MA_THAM_CHIEU_2
        {
            get;
            set;
        }
        public Decimal TONG_THUC_TE_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TONG_THUC_TE_TON_CUOI_KY
        {
            get;
            set;
        }
        public DateTime NGAY_HET_HAN
        {
            get;
            set;
        }
        public DateTime NGAY_BUT_TOAN_THAM_CHIEU
        {
            get;
            set;
        }
        public Decimal HANG_BOOKING_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal HANG_BOOKING_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal HIEU_SO_BOOK_VA_TON_THUC_TE_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal HIEU_SO_BOOK_VA_TON_THUC_TE_TON_CUOI_KY
        {
            get;
            set;
        }
        public String MA_SAN_PHAM_CHA
        {
            get;
            set;
        }
        public Decimal NHAP_CHO_CHIA_TON_CUOI_KY
        {
            get;
            set;
        }
        public Decimal NHAP_CHO_CHIA_TON_DAU_KY
        {
            get;
            set;
        }
        public Decimal TRONG_LUONG { get; set; }

        public Decimal TRONG_LUONG_TON_DAU_KY { get; set; }

        public Decimal TRONG_LUONG_NHAP_TRONG_KY { get; set; }

        public Decimal TRONG_LUONG_XUAT_TRONG_KY { get; set; }

        public Decimal TRONG_LUONG_TON_CUOI_KY { get; set; }
        public Decimal DOANH_THU_NHAP { get; set; }
        public DateTime TU_NGAY { get; set; }
        public DateTime DEN_NGAY { get; set; }
        public string TYPE { get; set; }
        #endregion Thuộc tính
    }
   
}