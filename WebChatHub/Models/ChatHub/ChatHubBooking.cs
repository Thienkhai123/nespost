using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using System.Web.Mvc;

namespace WebChatHub.Models
{
    public class ChatHubBooking : Hub
    {
        #region Data Members
        const string KHACH_BOOKING = "KHACH";
        const string CS = "CS";
        const string CS_ADMIN = "CS_ADMIN";

        public static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        public static Dictionary<string, List<MessageDetail>> CurrentMessage = new Dictionary<string, List<MessageDetail>>();
        public static Dictionary<string, List<MessageDetail>> ToDayMessage = new Dictionary<string, List<MessageDetail>>();
        #endregion

        #region Methods
        public void Connect(string UserID, string FullName, string Role, string Companycode, string Stationcode, string StationName)
        {
            if (string.IsNullOrWhiteSpace(UserID))
                return;

            var id = Context.ConnectionId;

            // login and check reload page
            var user = ConnectedUsers.FirstOrDefault(x => x.UserID == UserID);
            if (user == null)
            {
                if (Stationcode.StartsWith("59"))
                    Companycode = "02";

                user = new UserDetail
                {
                    Role = Role,
                    UserID = UserID,
                    FullName = FullName,
                    UserCode = UserID,
                    CompanyCode = Companycode,
                    StationCode = Stationcode,
                    StationName = StationName
                };
                ConnectedUsers.Add(user);
            }
           
            user.ConnectionId = id;
            user.TimeLog = DateTime.Now;
            user.Online = true;
            
            if (user.Role.StartsWith("CS"))
            {
                //lấy mess 
                List<UserDetail> lstKH = new List<UserDetail>();

                foreach (var item in ConnectedUsers)
                {
                    if (item.Role.Equals(KHACH_BOOKING) && item.Online && CurrentMessage[item.UserID].FirstOrDefault(x=>x.To_UserCode == UserID) !=null)
                    {
                        lstKH.Add(item);
                    }
                }

                // lấy thêm list cs
                var lstCS = ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.Online).ToList();

                Clients.Caller.onConnected(UserID, lstKH, lstCS);
                Clients.Clients(ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.CompanyCode == user.CompanyCode).Select(x => x.ConnectionId).ToList()).onNewCSConnected(UserID, user.FullName);
            }
            else
            {
                getMessages();
                //lstTin
                var lstMess = new List<MessageDetail>();
                if (CurrentMessage.ContainsKey(UserID))
                {
                    lstMess = CurrentMessage[UserID].ToList();
                }

                Clients.Caller.onConnected(UserID, lstMess);
                Clients.Clients(ConnectedUsers.Where(x => x.UserID == user.CsIDCare).Select(x => x.ConnectionId).ToList()).onUserConnected(UserID);
            }
        }

        //check cs online và phân tuyến
        public void checkCS(UserDetail fromUser)
        {
            // nếu trong ngày đã chat rồi thì gán lại
            if (ToDayMessage[fromUser.UserID].Count > 0)
            {
                string idCS = ToDayMessage[fromUser.UserID][0].To_UserCode;

                fromUser.CsIDCare = idCS;
                var cs = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == fromUser.CsIDCare);
                fromUser.CsNameCare = cs != null ? cs.NAME : "";
                
                return;
            }
           
            var lstCs = ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.Online && x.CompanyCode == fromUser.CompanyCode).ToList();

            // nếu đã có tin từ trước và cs đã online thì gán lại
            if (string.IsNullOrWhiteSpace(fromUser.CsIDCare) && CurrentMessage[fromUser.UserID].Count > 0)
            {
                string idCS = CurrentMessage[fromUser.UserID][0].To_UserCode;
                if (lstCs.FirstOrDefault(x => x.UserID == idCS) != null)
                {
                    fromUser.CsIDCare = idCS;
                    var cs = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == fromUser.CsIDCare);
                    fromUser.CsNameCare = cs != null ? cs.NAME : "";
                }
            }

            // nếu cs đơn vị phụ trách khách hàng này onl thì gán lại
            if (string.IsNullOrWhiteSpace(fromUser.CsIDCare))
            {
                var cs = lstCs.FirstOrDefault(x => x.StationCode == fromUser.StationCode);
                if (cs != null)
                {
                    fromUser.CsIDCare = cs.UserID;
                    fromUser.CsNameCare = cs != null ? cs.FullName : "";
                }
            }

            // tìm kiếm cs văn phòng và random
            var lstCSvp = lstCs.Where(x => x.StationCode == "0108" || x.StationCode == "0279").ToList();
            if (string.IsNullOrWhiteSpace(fromUser.CsIDCare) && lstCSvp.Count >0)
            {
                Random random = new Random();
                var rd = random.Next(0, lstCSvp.Count());
                var cs = lstCSvp[rd];
                fromUser.CsIDCare = cs.UserID;
                fromUser.CsNameCare = cs.FullName;
            }

            // nếu không có cs nào thì gán cho trưởng
            if (string.IsNullOrWhiteSpace(fromUser.CsIDCare))
            {
                CommonData.getListCSByCompanyCode();
                var csAdmin = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.COMPANY_CODE == fromUser.CompanyCode && x.LOAI_USER == "CS_ADMIN");
                if (csAdmin != null)
                {
                    fromUser.CsIDCare = csAdmin.USER_NAME;
                    fromUser.CsNameCare = csAdmin.NAME;
                }
            }

        }

        //phân lại tuyến cho CS
        public void PhanTuyenCS(string toUserId, string toUserName, string csIDCare, string csFullName)
        {

            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            var csIDold = "";

            var toUser = new UserDetail();
            toUser.lstMess = new List<MessageDetail>();

            string input = JsonConvert.SerializeObject(new MessageDetail() { PHIEN_LAM_VIEC = toUserId, To_UserCode = csIDCare, GHI_CHU = "PHAN_TUYEN_CS", To_UserID = fromUser.UserID });
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("PHAN_TUYEN_CS", input);
            if (dtoRes.Status == "OK")
            {
                var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);

                toUser.lstMess = new List<MessageDetail> { list[0] };
                toUser.UnRead = 1;
                toUser.lstMess[0].Time_String = toUser.lstMess[0].Time.ToString("dd/MM HH:mm");

                toUser.UserID = toUser.UserCode = toUserId;

                toUser.FullName = toUserName;
                toUser.CsIDCare = list[0].To_UserCode;
                toUser.CsNameCare = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == csIDCare).NAME;
                toUser.StationCode = list[0].STATION_CODE;
                toUser.StationName = list[0].STATION_NAME;

                csIDold = list[0].Fr_UserID;
            }

            var cs_old = ConnectedUsers.FirstOrDefault(x => x.UserID == csIDold);
            if (cs_old != null)
                Clients.Client(cs_old.ConnectionId).onUserDisconnected(toUser);

            Clients.Caller.phanTuyenCS(false, toUser.UserID, "Phân tuyến thành công.", csIDCare == fromUser.UserID ? true : false);

            var cs = ConnectedUsers.FirstOrDefault(x => x.UserID == csIDCare);
            if (cs != null)
                Clients.Client(cs.ConnectionId).onNewUserConnected(toUser);
        }

        //gửi tin nhắn và phân quyền 
        public void SendPrivateMessage(string toUserId, string toUserName, string message)
        {
            var lag = "vi";
            if (!string.IsNullOrWhiteSpace(message))
            {
                string fromUserId = Context.ConnectionId;
                var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);
                if (fromUser == null)
                {
                    Clients.Caller.sendErorr("Có lỗi xảy ra, vui lòng reload lại trang để nhắn tin với chúng tôi.");
                }
                var dto = new MessageDetail();
                if (fromUser.Role.Equals(KHACH_BOOKING))
                {
                    // lần chát đầu tiên trong ngày thì check cs online và phân tuyến
                    if (string.IsNullOrWhiteSpace(fromUser.CsIDCare))
                    {
                        checkCS(fromUser);
                    }

                    UserDetail Cs = ConnectedUsers.FirstOrDefault(x => x.UserID == fromUser.CsIDCare);
                    if (Cs == null)
                    {
                        dto = AddMessageinCache(fromUser.UserID, fromUser.FullName, fromUser.Role, fromUser.UserCode, fromUser.CsIDCare, fromUser.CsNameCare, fromUser.CompanyCode, message, fromUser.StationCode, fromUser.StationName, false);
                        Clients.Caller.sendPrivateMessage(dto, fromUser.CsIDCare, fromUser.CsNameCare);
                        // save db
                        SaveDB(dto);
                    }
                    else
                    {
                        dto = AddMessageinCache(fromUser.UserID, fromUser.FullName, fromUser.Role, fromUser.UserCode, Cs.UserID, Cs.FullName, Cs.CompanyCode, message, fromUser.StationCode, fromUser.StationName, false);
                        // gửi trả lại người gọi
                        Clients.Caller.sendPrivateMessage(dto, Cs.UserID, Cs.FullName);
                        Clients.Client(Cs.ConnectionId).sendPrivateMessage(dto, fromUser.UserID, fromUser.FullName);
                        // save db
                        SaveDB(dto);
                    }

                    // lần chat đầu tiên 
                    if (ToDayMessage[fromUser.UserID].ToList().Count == 1)
                    {
                        Task.Run(() => CapNhatPhanTuyenCS(fromUser.UserID, fromUser.CsIDCare, ""));

                        Thread.Sleep(2000);
                        MessageDetail obj = new MessageDetail { Fr_UserID = "HT", Fr_UserName = lag.Equals("vi") ? "Hệ thống" : "System", Fr_UserRole = "HT", Message = lag.Equals("vi") ? "Hệ thống đang kết nối tới tổng đài ... " : "System is connecting to switchboard ...", Time = DateTime.Now, isRead = true };
                        Clients.Caller.sendPrivateMessage(obj, fromUser.UserID, fromUser.FullName);

                        //thông báo tổng đài đang bận
                        Thread newThread = new Thread(checkCSOnline);
                        newThread.Start();
                    }
                }
                else
                {
                    // cs trả lời 
                    UserDetail kh = ConnectedUsers.FirstOrDefault(x => x.UserID == toUserId);
                    if (kh == null)
                    {
                        dto = AddMessageinCache(fromUser.UserID, fromUser.FullName, fromUser.Role, fromUser.UserCode, toUserId, toUserName, fromUser.CompanyCode, message,"","", false, false);
                        // gửi trả cs
                        Clients.Caller.sendPrivateMessage(dto, toUserId, toUserName);
                    }
                    else
                    {
                        // nếu là câu đầu của khách hàng
                        if (ToDayMessage[kh.UserID].Count == 1)
                        {
                            string mes = "";
                            if (lag.Equals("vi"))
                            {
                                mes = "Tư vấn viên " + fromUser.FullName + " đã tham gia cuộc hội thoại.";
                            }
                            else mes = "Counselors " + fromUser.FullName + "  has joined the conversation.";
                            MessageDetail obj = new MessageDetail { Fr_UserID = "HT", Fr_UserName = lag.Equals("vi") ? "Hệ thống" : "System", Fr_UserRole = "HT", Message = mes, Time = DateTime.Now, isRead = false };
                            Clients.Client(kh.ConnectionId).sendPrivateMessage(obj, fromUser.UserID, fromUser.FullName);
                        }

                        dto = AddMessageinCache(fromUser.UserID, fromUser.FullName, fromUser.Role, fromUser.UserCode, kh.UserID, kh.FullName, kh.CompanyCode, message, kh.StationCode, kh.StationName, false);
                        // gửi trả cs
                        Clients.Caller.sendPrivateMessage(dto, kh.UserID, kh.FullName);
                        //gửi trả khách
                        Clients.Client(kh.ConnectionId).sendPrivateMessage(dto, fromUser.UserID, fromUser.FullName);
                    }
                    // save db
                    SaveDB(dto);
                }
            }
        }

        //sử dụng cho sự kiện click vào từng kh
        public void UpdateCsCare(string idKh, bool isJustView = false)
        {
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            UserDetail kh = new UserDetail();

            string input = JsonConvert.SerializeObject(new MessageDetail { To_CompanyCode = fromUser.CompanyCode, PHIEN_LAM_VIEC = idKh });
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS_WITH_ID", input);
            if (dtoRes.Status == "OK")
            {
                var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);
                list = list.OrderBy(x => x.Time).ToList();
                foreach (var item in list)
                {
                    item.Time_String = item.Time.ToString("HH:mm dd/MM/yyyy");
                }

                kh.UserID = kh.UserCode = list[0].Fr_UserID;
                kh.FullName = list[0].Fr_UserName;
                kh.Role = list[0].Fr_UserRole;
                kh.CsIDCare = list[0].To_UserCode;

                var cs = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == kh.CsIDCare);

                kh.CsNameCare = cs != null ? cs.NAME : "";
                kh.lstMess = list;
            }

            if (isJustView)
            {
                Clients.Caller.CsCare(kh, false);
                return;
            }

            Clients.Caller.CsCare(kh, fromUser.UserID.Equals(kh.CsIDCare) ? true : false);

            if (fromUser.UserID == kh.CsIDCare)
            {
                var lstmess = kh.lstMess.Where(x => x.Fr_UserID == kh.UserID && !x.isRead).ToList();

                if (lstmess.Count > 0)
                {
                    foreach (var item in lstmess)
                    {
                        item.isRead = true;
                    }
                    UpdateReadMessIntoDB(kh.UserID, CS);
                }
            }
        }

        // sự kiện update trạng thái đã đọc
        public void UpdateReadMess(string UserID, string Role)
        {
            Task.Run(() => UpdateReadMessIntoDB(UserID, Role));
        }

        public void checkCSOnline()
        {
            System.Threading.Thread.Sleep(60000);
            var user = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null && ToDayMessage[user.UserID].FirstOrDefault(x => x.Fr_UserID != user.UserID) == null)
            {
                MessageDetail dto =
                    new MessageDetail
                    {
                        Fr_UserID = "HT",
                        Fr_UserName = "Hệ thống",
                        Fr_UserRole = "HT",
                        Message = "Hiện tại tổng đài đang bận, chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.",
                        Time = DateTime.Now,
                        isRead = false
                    };
                Clients.Caller.sendPrivateMessage(dto, user.UserID, user.FullName);
            }
        }

        public void CapNhatPhanTuyenCS(string UserID, string CSIDCare, string UpdateMess)
        {
            string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new MessageDetail() { PHIEN_LAM_VIEC = UserID, To_UserCode = CSIDCare, GHI_CHU = UpdateMess });
            BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("PHAN_TUYEN_CS", inputCa);
            if (dtoRes.Status == "OK")
            {

            }
        }

        public override Task OnDisconnected(bool ts)
        {
            if (ts == true)
            {
                var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                if (item != null)
                {
                    item.Online = false;
                    item.TimeLog = DateTime.Now;

                    if (item.Role.Equals(KHACH_BOOKING))
                    {
                        Clients.Clients(ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.CompanyCode == item.CompanyCode).Select(x => x.ConnectionId).ToList()).onUserOffline(item);

                        if (ToDayMessage.ContainsKey(item.UserID))
                            ToDayMessage.Remove(item.UserID);

                        if (CurrentMessage.ContainsKey(item.UserID))
                            CurrentMessage.Remove(item.UserID);
                    }
                    else
                    {
                        var lstCS = ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.Online).ToList();
                        Clients.Clients(lstCS.Select(x => x.ConnectionId).ToList()).onCSOffline(item);
                        bool isMB = lstCS.Where(x => x.CompanyCode == "01").Count() > 0 ? true : false;
                        bool isMN = lstCS.Where(x => x.CompanyCode != "01").Count() > 0 ? true : false;

                        Clients.Clients(ConnectedUsers.Where(x => x.Role.Equals(KHACH_BOOKING)).Select(x => x.ConnectionId).ToList()).checkBranchOnline(isMB, isMN);
                        Clients.Clients(ConnectedUsers.Where(x => x.Role.StartsWith(CS)).Select(x => x.ConnectionId).ToList()).onCSOffline(item);
                    }

                    ConnectedUsers.Remove(item);
                }
            }
            return base.OnDisconnected(ts);
        }

        public void checkCSOnOff()
        {
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            fromUser.TimeLog = DateTime.Now;

            var lstCS = ConnectedUsers.Where(x => x.Role.StartsWith(CS) && x.Online).ToList();

            if (lstCS != null && lstCS.Count > 0)
            {
                foreach (var item in lstCS)
                {
                    if (DateTime.Now.Subtract(item.TimeLog).TotalMinutes >= 5)
                    {
                        item.Online = false;
                    }
                }
            }
        }

        #endregion

        #region private Messages

        private MessageDetail AddMessageinCache(string fromUserId, string fromUserName, string fromUserRole, string fromUserCode, string toUserId, string toUserName, string toUserCompany, string message , string frStationCode, string frStationName, bool read, bool saveCache = true)
        {
            var Time_String = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            var dto = new MessageDetail
            {
                Fr_UserID = fromUserId,
                Fr_UserCode = fromUserCode,
                Fr_UserName = fromUserName,
                Fr_UserRole = fromUserRole,
                To_UserName = toUserName,
                To_CompanyCode = toUserCompany,
                To_UserID = toUserId,
                Message = message,
                Insert_Time = DateTime.Now,
                Time = DateTime.Now,
                isRead = read,
                Time_String = Time_String,
                UnRead = !read ? 1 : 0,
                STATION_CODE = frStationCode,
                STATION_NAME = frStationName
            };

            if (fromUserRole.Equals(KHACH_BOOKING))
            {
                dto.PHIEN_LAM_VIEC = fromUserId;
            }
            else
            {
                dto.PHIEN_LAM_VIEC = toUserId;
            }

            if (saveCache)
            {
                if (!CurrentMessage.Keys.Contains(dto.PHIEN_LAM_VIEC))
                {
                    CurrentMessage.Add(dto.PHIEN_LAM_VIEC, new List<MessageDetail>());

                }
                dto.ID = CurrentMessage[dto.PHIEN_LAM_VIEC].Count + 1;
                CurrentMessage[dto.PHIEN_LAM_VIEC].Add(dto);

                if (!ToDayMessage.Keys.Contains(dto.PHIEN_LAM_VIEC))
                {
                    ToDayMessage.Add(dto.PHIEN_LAM_VIEC, new List<MessageDetail>());
                }
                ToDayMessage[dto.PHIEN_LAM_VIEC].Add(dto);
            }

            return dto;
        }

        private void SaveDB(MessageDetail dto)
        {
            // lưu vào db
            string inputCa = JsonConvert.SerializeObject(dto);
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("INSERT_MESSAGE", inputCa);
            if (dtoRes.Status == "OK")
            {
            }
        }

        public void getMessages()
        {
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
           
            if (ToDayMessage.ContainsKey(fromUser.UserID))
            {
                return;
            }

            string input = JsonConvert.SerializeObject(new MessageDetail { PHIEN_LAM_VIEC = fromUser.UserID });
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_MESS_OF_CUS", input);
            if (dtoRes.Status == "OK")
            {
                var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);
                list = list.OrderBy(x => x.Time).ToList();
                foreach (var item in list)
                {
                    item.Time_String = item.Time.ToString("HH:mm dd/MM/yyyy");
                }
                if (!CurrentMessage.ContainsKey(fromUser.UserID))
                {
                    CurrentMessage.Add(fromUser.UserID, new List<MessageDetail>());
                }
                CurrentMessage[fromUser.UserID] = list;

                if (!ToDayMessage.ContainsKey(fromUser.UserID))
                {
                    ToDayMessage.Add(fromUser.UserID, new List<MessageDetail>());
                }
                ToDayMessage[fromUser.UserID] = list.Where(x => x.Insert_Time == DateTime.Now.Date).ToList();
            }

            Task.Run(() => UpdateReadMessIntoDB(fromUser.UserID, fromUser.Role));
        }

        public string getUnReadMessCount(string UserName)
        {

            if (string.IsNullOrWhiteSpace(UserName))
            {
                return "";
            }

            string input = JsonConvert.SerializeObject(new MessageDetail { PHIEN_LAM_VIEC = UserName, Message = "GetUnReadCount"});
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_MESS_OF_CUS", input);
            if (dtoRes.Status == "OK")
            {
                int unReadCount = JsonConvert.DeserializeObject<int>(dtoRes.ResponseData);

                return unReadCount > 0 ? unReadCount.ToString() : "";
            }

            return "";
        }

        private void UpdateReadMessIntoDB(string UserID, string Role)
        {
            // lưu vào db
            string inputCa = JsonConvert.SerializeObject(new MessageDetail { Fr_UserID = UserID, Fr_UserRole = Role });
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("UPDATE_READ_LIST_MESSAGE", inputCa);
            if (dtoRes.Status == "OK")
            {

            }
        }
        #endregion
    }

}