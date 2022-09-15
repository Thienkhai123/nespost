using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class OP_ZONE_MASTERDTO
    {
        public String COMPANY_CODE { get; set; }
        public String ZONE_CODE { get; set; }
        public String ZONE_NAME { get; set; }
        public String DEF_STATION { get; set; }
        public String REGISTER_DATE { get; set; }
        public String MA_VUNG_POD { get; set; }
        public String TEN_VUNG_POD { get; set; }
        public String TEN_CO_DAU { get; set; }
        public String PUBLIC_CODE { get; set; }
        public String MA_MIEN { get; set; }
        public String TEN_VIET_TAT
        {
            get;
            set;
        }
    }
}