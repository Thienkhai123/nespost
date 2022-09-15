using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChatHub.Models;

namespace WebChatHub.Controllers
{
    public class MessageOnlinePageController : Controller
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
                List<string> lstRole = new List<string> { "cs trang chủ" };
                string inputCa = JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = Username.Trim(), PASSWORD = Password.Trim(), LOAI_USER = JsonConvert.SerializeObject(lstRole), DANH_SACH_KH = "CS_TRANG_CHU" });
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

        public ActionResult CreateAccount(string maNV, string tenNV, string password)
        {
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    return Json(Service.ErrorV2("Phiên làm việc đã hết hạn. Mời đăng nhập lại"));
                }
                var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

                CSS_CRM_NGUOI_DUNGDTO dto = new CSS_CRM_NGUOI_DUNGDTO { CUSTOMER_CODE = maNV, USER_NAME = maNV, GHI_CHU = tenNV, PASSWORD = password, PARENT_USER_NAME = user.COMPANY_CODE, INSERT_USER = user.USER_NAME, DANH_SACH_KH = "CS_TRANG_CHU" };

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

        //public ActionResult ResetPass(string pass, string newpsw)
        //{
        //    JMessage msg = new JMessage();
        //    try
        //    {
        //        if (Session["UserAdmin"] == null)
        //        {
        //            return Json(Service.ErrorV2("Phiên làm việc đã hết hạn. Mời đăng nhập lại"));
        //        }

        //        CSS_CRM_NGUOI_DUNGDTO user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
        //        if (user.PASSWORD != pass)
        //        {
        //            return Json(Service.ErrorV2("Mật khẩu hiện tại không đúng. Vui lòng kiểm tra lại."));
        //        }

        //        string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new Account() { USER_NAME = user.USER_NAME, Password = newpsw.Trim() });
        //        BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("RESET_PASS", inputCa);
        //        if (dtoRes.Status == "OK")
        //        {
        //            msg.Error = false;
        //            user.PASSWORD = newpsw;
        //            Session["UserAdmin"] = user;
        //            return Json(msg);
        //        }
        //        else
        //        {
        //            msg.Error = true;
        //            msg.Object = null;
        //            msg.Title = dtoRes.Description;
        //            return Json(msg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(Service.Error(ex));
        //    }
        //}

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

                MessageDetail dto = new MessageDetail { To_CompanyCode = user.COMPANY_CODE, To_UserCode = user.USER_NAME, Fr_UserRole = "KHACH_ONL" };

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);

                    foreach (var item in list)
                    {
                        item.Time_String = item.Time.ToString("dd/MM HH:mm");

                        var cs = CommonData._lstCS_BOOKING.FirstOrDefault(x => x.USER_NAME == item.To_UserCode);
                        item.To_UserName = cs != null ? cs.NAME : "";
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

                MessageDetail dto = new MessageDetail { To_CompanyCode = user.COMPANY_CODE, To_UserCode = user.USER_NAME, Fr_UserRole = "CS" };

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);

                    foreach (var item in list)
                    {
                        item.Time_String = item.Time.ToString("HH:mm dd/MM/yyyy");

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
        public ActionResult OpenMessage(string id, bool Miss = false)
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

                string input = JsonConvert.SerializeObject(new MessageDetail { PHIEN_LAM_VIEC = id });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_MESS_WITH_ID", input);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    var list = JsonConvert.DeserializeObject<List<MessageDetail>>(dtoRes.ResponseData);
                    list = list.Where(x => !string.IsNullOrWhiteSpace(x.PHIEN_LAM_VIEC)).ToList();

                    Dictionary<string, List<MessageDetail>> dic = new Dictionary<string, List<MessageDetail>>();
                    foreach (var item in list)
                    {
                        if (dic.ContainsKey(item.PHIEN_LAM_VIEC))
                        {
                            dic[item.PHIEN_LAM_VIEC].Add(item);
                        }
                        else
                        {
                            dic.Add(item.PHIEN_LAM_VIEC, new List<MessageDetail>());
                            dic[item.PHIEN_LAM_VIEC].Add(item);
                        }
                    }
                    foreach (var key in dic.Keys)
                    {
                        var lst = dic[key] as List<MessageDetail>;
                        if (lst != null)
                        {
                            lst = lst.OrderBy(x => x.Time).ToList();
                        }
                    }
                    msg.Object = dic;
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
                        item.Time_String = item.Time.ToString("HH:mm dd/MM/yyyy");
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
                        item.Time_String = item.Time.ToString("HH:mm dd/MM/yyyy");
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

        public ActionResult _lstCS()
        {
            return PartialView();
        }

        #endregion
    }
}