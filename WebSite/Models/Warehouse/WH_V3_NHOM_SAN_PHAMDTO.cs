using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_NHOM_SAN_PHAMDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }

        public String MA_NHOM_KHO { get; set; }

        public String MA_DINH_DANH_TEMPLATE { get; set; }

        public String MA_NHOM_SAN_PHAM { get; set; }

        public String TEN_NHOM_SAN_PHAM { get; set; }

        public String MA_TREE { get; set; }

        public String MA_NHOM_SAN_PHAM_CHA { get; set; }

        public String TEN_NHOM_SAN_PHAM_CHA { get; set; }

        public String GHI_CHU { get; set; }

        public String MA_NHOM_THAM_CHIEU { get; set; }

        public String TRANG_THAI { get; set; }

        public String INSERT_USER { get; set; }

        public DateTime INSERT_DATE { get; set; }

        public DateTime INSERT_TIME { get; set; }

        public String UPDATE_USER { get; set; }

        public DateTime UPDATE_DATE { get; set; }

        public DateTime UPDATE_TIME { get; set; }

        #endregion Thuộc tính
    }
}