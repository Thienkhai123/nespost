using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class CSS_DIC_GENERAL_DICTIONARY
    {
        public string idItem { get; set; }
        public string DICTIONARY_TYPE { get; set; }
        public string DICTIONARY_CODE { get; set; }
        public string DICTIONARY_NAME { get; set; }
        public int SORT_ORDER { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public string UPDATE_USER { get; set; }
        public string INSERT_USER { get; set; }
        public string DESCRIPTION { get; set; }
        public string PARENT_CODE { get; set; }
        public int STATUS { get; set; }
    }
}