﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class TINH
    {
        public String ZONE_CODE
        {
            get;
            set;
        }
        public String ZONE_NAME
        {
            get;
            set;
        }
        public String TEN_CO_DAU
        {
            get;
            set;
        }
    }

    public class HUYEN
    {
        public String DISTRICT_CODE
        {
            get; set;
        }


        public String DISTRICT_NAME
        {
            get; set;
        }

        public String ZONE_NAME
        {
            get; set;
        }
        public String ZONE_CODE
        {
            get; set;
        }
        public String PUBLIC_CODE
        {
            get; set;
        }
        public string DESCRIPTION { get; set; }
        public string NAME { get; set; }
    }
}