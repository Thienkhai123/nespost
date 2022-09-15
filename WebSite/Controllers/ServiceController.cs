using WebSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSite.Filter;
using System.Net.Mail;
using DevExpress.Web.Mvc;

namespace WebSite.Controllers
{
    public class ServiceController : Controller
    {
        public ActionResult GetListTinhHuyen()
        {
            JMessage msg = new JMessage();
            try
            {
                CommonData.GetAllTinhHuyen();
                msg.Object = CommonData._listZone;
                msg.Error = false;
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;

            }
            return Json(msg);
        }
        public ActionResult GetListHuyen(string zone_code)
        {
            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ValueField = "DISTRICT_CODE";
                p.TextField = "DISTRICT_NAME";
                p.ValueType = typeof(string);
                if (String.IsNullOrEmpty(zone_code))
                    p.BindList(new List<CSS_OP_DISTRICTDTO>());
                else
                {
                    p.BindList(CommonData.GetDistrict(zone_code));
                }
            });
        }
        public List<SelectListItem> lstDV()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Chuyển phát thường - SGEV", Value = "CPT" });
            items.Add(new SelectListItem { Text = "Chuyển phát nhanh - SGEV", Value = "CPN" });
            items.Add(new SelectListItem { Text = "Phát hỏa tốc", Value = "PHT" });
            return items;
        }
        public List<SelectListItem> lstLoaiBPBK()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Hàng", Value = "SPX" });
            items.Add(new SelectListItem { Text = "Thư", Value = "DOX" });
            return items;
        }
        public JsonResult GetCustomer()
        {
            List<CUSTOMER> lstCustomer = new List<CUSTOMER>();
            lstCustomer = Session["DANH_SACH_KH"] as List<CUSTOMER>;
            return Json(lstCustomer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAutoNum(string SubCode)
        {
            JMessage msg = new JMessage();
            try
            {
                msg.Object = CommonData.GetAutoNum(SubCode, 1);
                msg.Error = false;
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;

            }
            return Json(msg);
        }

        public ActionResult TaoMaKHCon(CSS_CRM_NGUOI_DUNGDTO dto)
        {
            try
            {
                dto.CUSTOMER_CODE = ClientSession.getCusCode();
                dto.PARENT_USER_NAME = ClientSession.getUserName();
                dto.INSERT_USER = dto.CUSTOMER_CODE;
                BaseResponseAndroid rep = Connection.KetNoiLayDuLieu("TAO_MA_KHACH_HANG_CON", JsonConvert.SerializeObject(dto));
                if (rep.IsError)
                {
                    return Json(Service.ErrorV2(rep.Description));
                }
                CSS_CRM_NGUOI_DUNGDTO user = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(rep.ResponseData);
                return Json(Service.OK_V1(user));
            }
            catch (Exception e)
            {
                return Json(Service.Error(e));
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

        //check username
        public ActionResult TaoTKOnline(string makh)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO userRes = new CSS_CRM_NGUOI_DUNGDTO { CUSTOMER_CODE = makh, INSERT_USER = ClientSession.getCusCode() };
                BaseResponseAndroid rep = Connection.KetNoiLayDuLieu("TAO_TAI_KHOAN_BOOKING_ONLINE", JsonConvert.SerializeObject(userRes));
                if (rep.Status == "OK")
                {
                    msg.Error = false;
                    CSS_CRM_NGUOI_DUNGDTO user = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(rep.ResponseData);
                    msg.Object = user;
                }
                else
                {
                    msg.Error = true;
                    msg.Title = rep.Description;
                }
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;
            }
            return Json(msg);
        }

        //tạo tài khoản 
        public ActionResult SendEmail(string username, string email)
        {
            try
            {
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = username, EMAIL = email });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("CHECK_CUSTOMER", inputCa);
                if (dtoRes.Status == "OK")
                {
                    return Json(Service.Ok_V2(null, "Hệ thống đã gửi mật khẩu vào địa chỉ email của bạn. Vui lòng check email."));
                }
                else if (dtoRes.ResponseData!=null && dtoRes.ResponseData.Equals("1"))
                {
                    return Json(Service.ErrorV2(ClientSession.getLanguage().Equals("vi") ? "Không tìm thấy người dùng này. Vui lòng liên hệ dịch vụ khách hàng" : "Not found username. Please contact customer service for assistance in changing the password"));
                }
                else if (dtoRes.ResponseData != null && dtoRes.ResponseData.Equals("2"))
                {
                    return Json(Service.ErrorV2(ClientSession.getLanguage().Equals("vi") ? "Không tìm thấy địa chỉ email này. Vui lòng liên hệ dịch vụ khách hàng" : "Not found email address. Please contact customer service for assistance in changing the password"));
                }
                return Json(Service.ErrorV2(dtoRes.Description));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult Register(CUSTOMER dto)
        {
            try
            {
                if (CommonData._listDistrict.FirstOrDefault(x => x.DISTRICT_CODE == dto.FIN_DISTRICT_CODE) == null)
                {
                    return Json(Service.ErrorV2("Không tìm thấy thông tin quận huyện lấy hàng"));
                }
                string input = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("REGISTER", input);
                if (dtoRes.Status == "OK")
                {
                    CSS_LOG_VAN_DEDTO dtoLog = JsonConvert.DeserializeObject<CSS_LOG_VAN_DEDTO>(dtoRes.ResponseData);

                    return Json(Service.OK_V1(dtoLog, "Đăng ký tài khoản thành công"));
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

        public ActionResult VerifyAccount()
        {
            try
            {
                var isCaptchaValid = CaptchaExtension.GetCode("captcha") == Request.Form["tbCode"] ? true : false;
                if (!isCaptchaValid)
                    return Json(Service.ErrorV2(ClientSession.getLanguage("vi") ? "Mã captcha không đúng. Mời thử lại" : "Captcha code error, please try again"));

                string customer_code = Request.Form["customer_code"];
                string ma_xn = Request.Form["ma_xn"];
                string ma_xn_moi = Request.Form["maxacnhan"];
                if (ma_xn != ma_xn_moi)
                {
                    return Json(Service.ErrorV2(ClientSession.getLanguage("vi") ? "Mã xác nhận không chính xác. Vui lòng thử lại." : "Verification code incorrect. Please try again"));
                }

                CSS_LOG_VAN_DEDTO dto = new CSS_LOG_VAN_DEDTO { MA_THAM_CHIEU = customer_code };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("VERIFY_ACCOUNT", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    CSS_CRM_NGUOI_DUNGDTO userRes = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(dtoRes.ResponseData);
                    List<CUSTOMER> lstCust = !string.IsNullOrWhiteSpace(dtoRes.ResponseData_EX) ? JsonConvert.DeserializeObject<List<CUSTOMER>>(dtoRes.ResponseData_EX) : new List<CUSTOMER>();
                    Session["DANH_SACH_KH"] = lstCust;

                    userRes.MA_KHACH_HANG_CON = userRes.CUSTOMER_CODE.ToString();
                    var cust = lstCust.FirstOrDefault(x => x.CUSTOMER_CODE == userRes.CUSTOMER_CODE.ToString());
                    userRes.NAME = cust != null ? cust.NAME : "";
                    userRes.FULL_NAME = "[" + cust.LOCATION + "] " + cust.CUSTOMER_CODE + " - " + cust.NAME;

                    Session["User"] = userRes;

                    return Json(Service.OK_V1(userRes, "Xác nhận tài khoản thành công"));
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

        public ActionResult CaptchaPartial()
        {
            return PartialView("CaptchaPartial");
        }

        //chọn đơn vị và khách hàng do cs booking
        public ActionResult ChonKhachHangBooking()
        {
            return PartialView("_cbChonKhachHang");
        }

        public ActionResult ChooseBranch(string CompanyCode)
        {
            try
            {
                List<string> lstMien = new List<string> { "02", "03" };
                var httpCookie = Request.Cookies["UserOnline"];

                CSS_CRM_NGUOI_DUNGDTO user = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(httpCookie.Value);
                if (!string.IsNullOrWhiteSpace(user.COMPANY_CODE) && user.COMPANY_CODE != CompanyCode)
                {
                    if(!lstMien.Contains(user.COMPANY_CODE) || !lstMien.Contains(CompanyCode))
                    {
                        user.USER_NAME = Guid.NewGuid().ToString();
                    }
                }
                user.COMPANY_CODE = CompanyCode;

                var cookie = Response.Cookies["UserOnline"];
                cookie.Value = JsonConvert.SerializeObject(user);
                cookie.Expires = DateTime.Now.AddDays(15);
                return Json(Service.OK_V1(user));
            }
            catch (Exception ex)
            {
                return Json(Service.ErrorV2(ex.Message));
            }
        }
    }
}