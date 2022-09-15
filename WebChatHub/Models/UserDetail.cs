using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChatHub.Models
{
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public string UserID { get; set; }
        public string CompanyCode { get; set; }
        public string Role { get; set; }
        public int UnRead { get; set; }
        public string CsIDCare { get; set; }
        public string CsNameCare { get; set; }
        public DateTime TimeLog { get; set; }
        public string TimeLog_Str { get; set; }
        public DateTime FirsLog { get; set; }
        public bool Online { get; set; }
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public List<MessageDetail> lstMess { get; set; }
    }
}