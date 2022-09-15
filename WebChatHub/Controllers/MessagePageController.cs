using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatHub.Models;

namespace WebChatHub.Controllers
{
    public class MessagePageController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            CommonData.getListCSByCompanyCode(true);

            return View();
        }

        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            try
            {
                List<string> lstRole = new List<string> { "Nhân viên dịch vụ khách hàng", "Trưởng phòng Dịch Vụ Khách Hàng", "Nhân viên giám sát" };

                string inputCa = JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = Username.Trim(), PASSWORD = Password.Trim(), LOAI_USER = JsonConvert.SerializeObject(lstRole), LOAI_TK = "H2" });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("LOGIN_ADMIN_SAGAWA", inputCa);
                if (dtoRes.Status == "OK")
                {
                    CSS_CRM_NGUOI_DUNGDTO userRes = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(dtoRes.ResponseData);
                    if (userRes.STATION_CODE.StartsWith("59"))
                    {
                        userRes.COMPANY_CODE = "02";
                    }

                    Session["UserAdmin"] = userRes;
                                      
                    return Json(Service.OK_V1());
                }
                else
                {
                    return Json(Service.ErrorV2(dtoRes.Description));
                }
            }
            catch (Exception e)
            {
                return Json(Service.Error(e));
            }
        }
        
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "MessagePage");
        }

        //chat page
        #region
        public ActionResult Index()
        {
            if (Session["UserAdmin"] == null)
            {
                return RedirectToAction("Login", "MessagePage");
            }

            return View();
        }

        public ActionResult DanhSachTinNhan()
        {
            if (Session["UserAdmin"] == null)
            {
                return RedirectToAction("Login", "MessagePage");
            }

            return View();
        }

        public ActionResult DanhSachTinLienHe()
        {
            if (Session["UserAdmin"] == null)
            {
                return RedirectToAction("Login", "MessagePage");
            }

            return View();
        }

        public ActionResult SEARCH_ALL_MESS()
        {
            JMessage msg = new JMessage();
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    msg.Error = true;
                    msg.Title = "Phiên làm việc đã hết hạn. Mời đăng nhập lại";
                    return Json(msg);
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

                MessageDetail dto = new MessageDetail { To_CompanyCode = user.COMPANY_CODE, To_UserCode = user.USER_NAME, Fr_UserRole = "KHACH" };

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);

                    foreach (var item in list)
                    {
                        item.Time_String = item.Time.ToString("dd/MM HH:mm");
                        item.To_UserName = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == item.To_UserCode).NAME;
                    }
                    msg.Object = list;
                }
                else
                {
                    msg.Error = true;
                    msg.ID = 0;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }
            return Json(msg);
        }
        public ActionResult SEARCH_MESS()
        {
            JMessage msg = new JMessage();
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    msg.Error = true;
                    msg.Title = "Phiên làm việc đã hết hạn. Mời đăng nhập lại";
                    return Json(msg);
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
               
                MessageDetail dto = new MessageDetail { To_CompanyCode = user.COMPANY_CODE, To_UserCode = user.USER_NAME, Fr_UserRole= "CS" };

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);
                   
                    foreach(var item in list)
                    {
                        item.Time_String = item.Time.ToString("dd/MM HH:mm");

                    }
                    msg.Object = list;
                }
                else
                {
                    msg.Error = true;
                    msg.ID = 0;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }
            return Json(msg);
        }
        
        public ActionResult SEARCH_CONTACT_MESS(string fromDate, string toDate)
        {
            JMessage msg = new JMessage();
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    msg.Error = true;
                    msg.Title = "Phiên làm việc đã hết hạn. Mời đăng nhập lại";
                    return Json(msg);
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

                DateTime FromDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", null);
                DateTime ToDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", null);

                MessageDetail dto = new MessageDetail { To_CompanyCode = user.COMPANY_CODE, FromDate = FromDate, ToDate = ToDate, Fr_UserRole = "KHACH_LH" };

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_TIN_NHAN_VANG_LAI", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);

                    foreach (var item in list)
                    {
                        item.Time_String = item.Time.ToString("dd/MM HH:mm");
                    }
                    msg.Object = list;
                }
                else
                {
                    msg.Error = true;
                    msg.ID = 0;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }
            return Json(msg);
        }

        public ActionResult OpenContactMessage(string phienlamviec)
        {
            JMessage msg = new JMessage();
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    msg.Error = true;
                    msg.Title = "Phiên làm việc đã hết hạn. Mời đăng nhập lại";
                    return Json(msg);
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

                string input = JsonConvert.SerializeObject(new MessageDetail { PHIEN_LAM_VIEC = phienlamviec });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS_WITH_ID", input);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);
                    list = list.Where(x => !string.IsNullOrWhiteSpace(x.PHIEN_LAM_VIEC)).OrderBy(x => x.Time).ToList();

                    foreach (var item in list)
                    {
                        item.Time_String = item.Time.ToString("dd/MM HH:mm");
                    }

                    msg.Object = list;
                }
                else
                {
                    msg.Error = true;
                    msg.ID = 0;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }
            return Json(msg);
        }

        public ActionResult QuanLyTaiKhoan()
        {
            if (Session["UserAdmin"] == null)
            {
                return RedirectToAction("Login", "MessagePage");
            }

            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            CSS_CRM_NGUOI_DUNGDTO dto = new CSS_CRM_NGUOI_DUNGDTO { CUSTOMER_CODE = user.USER_NAME };
            string inputCa = JsonConvert.SerializeObject(dto);
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_MA_PHU_KH_BOOKING_ONLINE", inputCa);
            if (dtoRes.IsError)
            {
                return Json(Service.ErrorV2(dtoRes.Description));
            }
            List<CSS_CRM_NGUOI_DUNGDTO> lst = JsonConvert.DeserializeObject<List<CSS_CRM_NGUOI_DUNGDTO>>(dtoRes.ResponseData);

            lst = lst.Where(x => x.USER_NAME != user.USER_NAME).ToList();
            ViewData["lstMaCon"] = lst;
            return View();
        }
        
        public ActionResult CreateAccount(string tenNV, string password)
        {
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    return Json(Service.ErrorV2("Phiên làm việc đã hết hạn. Mời đăng nhập lại"));
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

                CSS_CRM_NGUOI_DUNGDTO dto = new CSS_CRM_NGUOI_DUNGDTO { CUSTOMER_CODE = user.CUSTOMER_CODE, GHI_CHU = tenNV, PASSWORD = password, PARENT_USER_NAME = user.COMPANY_CODE, INSERT_USER = user.USER_NAME, DANH_SACH_KH = "CS_TRANG_CHU" };

                string inputCa = JsonConvert.SerializeObject(dto);
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("CREATE_ACCOUNT_MESSAGEPAGE", inputCa);
                if (dtoRes.Status == "OK")
                {
                    return Json(Service.OK_V1());
                }
                else
                {
                    return Json(Service.ErrorV2(dtoRes.Description));
                }
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult UpdateInfo(CSS_CRM_NGUOI_DUNGDTO dto)
        {
            try
            {
                BaseResponseAndroid rep = Connection.KetNoiLayDuLieu("CAP_NHAT_THONG_TIN_MA_PHU", JsonConvert.SerializeObject(dto));
                if (rep.IsError)
                {
                    return Json(Service.ErrorV2(rep.Description));
                }

                return Json(Service.OK_V1());
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult CheckCSOnOff()
        {
            if (Session["UserAdmin"] == null)
            {
                return Json(Service.ErrorV2("Phiên làm việc đã kết thúc"));
            }
            return Json(Service.OK_V1());
        }
        #endregion
    }
}