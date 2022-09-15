using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_WARE_HOUSEDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }

        public String MA_WARE_HOUSE { get; set; }

        public String TEN_WARE_HOUSE { get; set; }

        public String MA_NHOM_KHO { get; set; }

        public String TEN_NHOM_KHO { get; set; }

        public String LOAI_WARE_HOUSE { get; set; }

        public String TEN_LOAI_WARE_HOUSE { get; set; }

        public String MA_CHA { get; set; }

        public String MA_TINH { get; set; }

        public String MA_HUYEN { get; set; }

        public String MA_XA { get; set; }

        public String MA_NUOC { get; set; }

        public String DIA_CHI_SO_NHA { get; set; }

        public String GHI_CHU { get; set; }

        public String TRANG_THAI { get; set; }

        public String MA_THAM_CHIEU { get; set; }

        public String SO_DIEN_THOAI { get; set; }

        public String FAX { get; set; }

        public String INSERT_USER { get; set; }

        public DateTime INSERT_DATE { get; set; }

        public DateTime INSERT_TIME { get; set; }

        public String UPDATE_USER { get; set; }

        public DateTime UPDATE_DATE { get; set; }

        public DateTime UPDATE_TIME { get; set; }

        public String MA_DON_VI { get; set; }

        public String TEN_DON_VI { get; set; }
        public List<string> LST_NCC { get; set; }
        public String TEN_TINH { get; set; }
        public String TEN_HUYEN { get; set; }
        public String TEN_XA { get; set; }

        #endregion Thuộc tính
    }
}