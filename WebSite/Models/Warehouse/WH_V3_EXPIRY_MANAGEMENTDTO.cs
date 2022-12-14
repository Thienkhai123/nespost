using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_EXPIRY_MANAGEMENTDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_WARE_HOUSE { get; set; }
        public String TEN_WARE_HOUSE { get; set; }
        public String MA_BUT_TOAN { get; set; }
        public DateTime NGAY_BUT_TOAN { get; set; }
        public DateTime GIO_GIO_BUT_TOAN { get; set; }
        public String LOAI_BUT_TOAN { get; set; }
        public String MA_SAN_PHAM { get; set; }
        public String TEN_SAN_PHAM { get; set; }
        public String MA_DON_VI_TINH { get; set; }
        public String TEN_DON_VI_TINH { get; set; }
        public String MA_THAM_CHIEU { get; set; }
        public String MA_THAM_CHIEU_CHI_TIET { get; set; }
        public String MANAGEMENT_DATA_CODE { get; set; }
        public Decimal SO_LUONG { get; set; }
        public String MA_DON_VI_TINH_QUY_DOI { get; set; }
        public String TEN_DON_VI_TINH_QUY_DOI { get; set; }
        public Decimal SO_LUONG_QUY_DOI { get; set; }
        public Decimal TY_LE_QUY_DOI { get; set; }
        public String MA_OWNER { get; set; }
        public DateTime NGAY_SAN_XUAT { get; set; }
        public String DON_VI_HAN_SU_DUNG { get; set; }
        public Decimal HAN_SU_DUNG { get; set; }
        public DateTime NGAY_HET_HAN { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String INSERT_USER { get; set; }
        public String NGAY_HH { get; set; }
        #endregion Thuộc tính
    }
}