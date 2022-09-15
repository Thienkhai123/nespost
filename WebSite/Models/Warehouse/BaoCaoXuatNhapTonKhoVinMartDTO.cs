using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class BaoCaoXuatNhapTonKhoVinMartDTO
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
        public String MA_OWNER
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
        public Decimal VALUE
        {
            get;
            set;
        }
        public DateTime NGAY_BUT_TOAN
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
        public String FARM
        {
            get;
            set;
        }
        public String MA_SAN_PHAM_CHA
        {
            get;
            set;
        }
        #endregion Thuộc tính
    }
}