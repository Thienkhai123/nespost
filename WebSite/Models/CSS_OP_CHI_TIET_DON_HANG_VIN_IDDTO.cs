using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO
    {
        public Int32 ID { get; set; }
        public String MA_PO { get; set; }
        public String MA_HANG_VIN { get; set; }
        public String MA_NCC { get; set; }
        public String TEN_HANG_HOA { get; set; }
        public String DVT { get; set; }
        public Int32 SO_LUONG { get; set; }
        public Decimal DON_GIA { get; set; }
        public Decimal THANH_TIEN { get; set; }
        public Decimal DAI { get; set; }
        public Decimal RONG { get; set; }
        public Decimal CAO { get; set; }
        public Decimal TRONG_LUONG_THUC { get; set; }
        public String MA_BPBK { get; set; }

        public Int32 SO_TT { get; set; }
        public Decimal DON_GIA_UU_DAI { get; set; }
    }
}