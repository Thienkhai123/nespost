using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models.Warehouse
{
    public class WH_V3_LOCATIONDTO
    {
        #region Thuộc tính
        public Int32 ID { get; set; }
        public String MA_WARE_HOUSE { get; set; }
        public String TEN_WARE_HOUSE { get; set; }
        public String MA_LOCATION { get; set; }
        public String TEN_LOCATION { get; set; }
        public String LOAI_LOCATION { get; set; }
        public String TEN_LOAI_LOCATION { get; set; }
        public String GHI_CHU { get; set; }
        public String TRANG_THAI { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME { get; set; }
        public String MA_NHOM_KHO { get; set; }
        #endregion Thuộc tính
    }

    public class LOCATION_AND_CELL
    {
        public String MA_WARE_HOUSE { get; set; }
        public String TEN_WARE_HOUSE { get; set; }
        public String MA_LOCATION { get; set; }
        public String TEN_LOCATION { get; set; }
        public String LOAI_LOCATION { get; set; }
        public String TEN_LOAI_LOCATION { get; set; }
        public String MA_CELL { get; set; }
        public String TEN_CELL { get; set; }
        public String MA_LOAI { get; set; }
        public String TRANG_THAI { get; set; }
        public String MA_NHOM_KHO { get; set; }
        public String TYPE { get; set; }
    }
}