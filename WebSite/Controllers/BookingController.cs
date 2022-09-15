using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Web.UI.WebControls;
using WebSite.Models;
using WebSite.Excel;
using DevExpress.XtraPrinting;
using WebSite.Filter;
using System.Globalization;
using WebSite.Language;
using Framework.Common;

namespace WebSite.Controllers
{
    [UserAuthenticationFilter]
    public class BookingController : Controller
    {
        CultureInfo enUS = new CultureInfo("vi-VN");
        public static Dictionary<string, List<CSS_OP_BOOKING_SGEV_WEB_EX>> dicDonHangTuSinh = new Dictionary<string, List<CSS_OP_BOOKING_SGEV_WEB_EX>>();
        //login, logout, resetpass, chọn khách hàng con       
        #region
        [UserAuthenticationFilter("login")]
        public ActionResult Login()
        {
            CommonData.GetAllTinhHuyen();
            return View();
        }
        [HttpPost]
        [UserAuthenticationFilter("login")]
        public ActionResult Login(string Username, string Password)
        {
            string lag = Request.Cookies["language"].Value;
            lag = "vi-VN";
            JMessage msg = new JMessage();
            try
            {
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new Account() { USER_NAME = Username.Trim(), Password = Password.Trim(), USERID = "" });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("LOGIN_BOOKING_SAGAWA", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    CSS_CRM_NGUOI_DUNGDTO userRes = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(dtoRes.ResponseData);
                    List<CUSTOMER> lstCust = !string.IsNullOrWhiteSpace(dtoRes.ResponseTinhHuyenXa.DataCust) ? JsonConvert.DeserializeObject<List<CUSTOMER>>(dtoRes.ResponseTinhHuyenXa.DataCust.ToString()) : new List<CUSTOMER>();
                    Session["DANH_SACH_KH"] = lstCust;
                    if (userRes.USER_NAME.ToLower().Trim().Equals(userRes.PASSWORD.ToLower().Trim()))
                    {
                        msg.ID = 6;
                        msg.Title = userRes.USER_NAME;
                        userRes.MA_KHACH_HANG_CON = userRes.CUSTOMER_CODE;
                        var cust = lstCust.FirstOrDefault(x => x.CUSTOMER_CODE.ToLower() == userRes.CUSTOMER_CODE.ToLower());
                        userRes.NAME = cust.NAME;
                        userRes.FULL_NAME = "[" + cust.LOCATION + "] " + cust.CUSTOMER_CODE + " - " + cust.NAME;
                        Session["User"] = userRes;
                        return Json(msg);
                    }
                    else if (userRes.USER_NAME.Equals("DLCC"))
                    {
                        msg.ID = 5;
                    }
                    else if (userRes.LOAI_USER.Equals("CS"))
                    {
                        msg.ID = 7;
                    }
                    else if (userRes.LOAI_USER.Equals("TTGD"))
                    {
                        msg.ID = 1;
                        var cust = lstCust.FirstOrDefault(x => x.CUSTOMER_CODE.ToLower() == userRes.CUSTOMER_CODE.ToLower());
                        userRes.NAME = cust != null ? cust.NAME : "";
                        userRes.FULL_NAME = "[" + cust.LOCATION + "] " + cust.CUSTOMER_CODE + " - " + cust.NAME;
                    }
                    else
                    {
                        userRes.MA_KHACH_HANG_CON = userRes.CUSTOMER_CODE;
                        var cust = lstCust.FirstOrDefault(x => x.CUSTOMER_CODE.ToLower() == userRes.CUSTOMER_CODE.ToLower());
                        userRes.NAME = cust != null ? cust.NAME : "";
                        userRes.FULL_NAME = "[" + cust.LOCATION + "] " + cust.CUSTOMER_CODE + " - " + cust.NAME;
                        if (!string.IsNullOrWhiteSpace(userRes.PARENT_USER_NAME))
                        {
                            userRes.NAME = userRes.GHI_CHU;
                        }
                        msg.ID = 1;
                    }
                    userRes.login = "1";
                    //ClientSession.LuuSessionWH(userRes);
                    Session["User"] = userRes;
                }
                else
                {
                    msg.ID = 10;
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.ID = 10;
                msg.Title = e.Message;
            }
            return Json(msg);

        }

        [UserAuthenticationFilter("login")]
        public ActionResult DangXuat()
        {
            var user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            Session.RemoveAll();
            return RedirectToAction("Login");
        }
        [UserAuthenticationFilter("login")]
        public ActionResult ResetPass(string Username, string Password)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new Account() { USER_NAME = Username, Password = Password.Trim() });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("RESET_PASS", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    user.PASSWORD = Password;
                    Session["User"] = user;
                    return Json(msg);
                }
                else
                {
                    msg.Error = true;
                    msg.Object = null;
                    msg.Title = dtoRes.Description;
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult ChonDonVi(string makhachhang, bool getAdress = false)
        {
            try
            {
                List<CUSTOMER> lst = ClientSession.getDanhSachKH();
                if (lst == null || lst.Count == 0)
                {
                    return Json(Service.ErrorV2(Booking.WH_coLoiXayRa));
                }
                var cus = lst.FirstOrDefault(x => x.CUSTOMER_CODE == makhachhang);
                if (cus == null)
                {
                    return Json(Service.ErrorV2(Booking.errKhongTimThayKH));
                }
                var user = ClientSession.getCustomer();
                user.MA_KHACH_HANG_CON = cus.CUSTOMER_CODE;
                user.FULL_NAME = cus.FULL_NAME;
                Session["User"] = user;

                CSS_OP_BOOKING_SGEV_WEB lstBooking = new CSS_OP_BOOKING_SGEV_WEB();
                if (getAdress)
                {
                    CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { MA_KHACH_HANG = user.MA_KHACH_HANG_CON };
                    string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                    BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_DIA_CHI", inputCa);
                    if (dtoRes.Status == "OK")
                    {
                        lstBooking = JsonConvert.DeserializeObject<CSS_OP_BOOKING_SGEV_WEB>(dtoRes.ResponseData);
                    }
                }

                return Json(Service.OK_V1(lstBooking, Booking.msg_luu_thanh_cong));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        public ActionResult CapNhatEmail(List<string> lstEmail)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
                string lag = ClientSession.getLanguage();

                foreach (var item in lstEmail)
                {
                    bool isEmail = CommonData.CheckEmail(item);
                    if (!isEmail)
                    {
                        return Json(Service.ErrorV2("Địa chỉ email \"" + item + "\" không đúng. Vui lòng nhập lại"));
                    }
                    if (item.Contains("@logo"))
                    {
                        return Json(Service.ErrorV2("Địa chỉ email \"" + item + "\" không được chứa '@logo.com'"));
                    }
                    if (lstEmail.Where(x => x == item).Count() > 1)
                    {
                        return Json(Service.ErrorV2("Địa chỉ email \"" + item + "\" bị trùng. Vui lòng kiểm tra lại"));
                    }
                }

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = user.USER_NAME, EMAIL = string.Join(";", lstEmail) });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("CAP_NHAT_EMAIl", inputCa);
                if (dtoRes.Status == "OK")
                {
                    user.EMAIL = string.Join(";", lstEmail);
                    msg.Error = false;
                    msg.Title = Booking.msgCapNhatEmail;
                    return Json(msg);
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }
        }
        public ActionResult ListKhachHang()
        {
            JMessage msg = new JMessage();
            if (Session["User"] != null)
            {
                List<CUSTOMER> lstCust = Session["DANH_SACH_KH"] as List<CUSTOMER>;
                msg.Object = lstCust;
                msg.Error = false;
            }
            else
            {
                msg.Error = true;
            }
            return Json(msg);
        }

        [HttpPost]
        public ActionResult ChonKhachHang(string CUST_CODE)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                user.MA_KHACH_HANG_CON = CUST_CODE;
                List<CUSTOMER> lstCust = Session["DANH_SACH_KH"] as List<CUSTOMER>;
                var cust = lstCust.Where(x => x.CUSTOMER_CODE == CUST_CODE).ToList();
                user.FULL_NAME = cust[0].FULL_NAME;

                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { MA_KHACH_HANG = user.MA_KHACH_HANG_CON };
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_DIA_CHI", inputCa);
                if (dtoRes.Status == "OK")
                {
                    CSS_OP_BOOKING_SGEV_WEB_EX lst = JsonConvert.DeserializeObject<CSS_OP_BOOKING_SGEV_WEB_EX>(dtoRes.ResponseData);
                    msg.Object = lst;
                }
                msg.Error = false;
                msg.Title = cust[0].FULL_NAME;
                return Json(msg);
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
                return Json(msg);
            }

        }

        public ActionResult CbHuyenNhan(string ZoneCode, string district_code, string name)
        {
            var lst = CommonData.GetDistrict(ZoneCode);
            ViewData["cbName"] = name;
            ViewData["district_code"] = district_code;
            return PartialView("_cbHuyenNhan", lst);
        }

        #endregion

        // Danh sách đơn hàng
        #region
        public ActionResult DanhSachDonHang()
        {
            Session["DSDonHang"] = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            return View();
        }
        public ActionResult GridViewPartial()
        {
            List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            if (Session["DSDonHang"] != null)
            {
                lst = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
            }
            return PartialView("_GridViewPartial", lst);
        }
        public ActionResult Search(string FromDate, string ToDate, string MA_DAT_HANG, string NGUOI_NHAN)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CUSTOMER> lstCust = Session["DANH_SACH_KH"] as List<CUSTOMER>;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                DateTime fDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                DateTime tDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                if (tDate.Subtract(fDate).Days > 90)
                {
                    return Json(Service.ErrorV2(Booking.errChiTimKiemTrong3Thang));
                }

                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { PO = MA_DAT_HANG, INSERT_DATE = fDate, UPDATE_DATE = tDate };
                if (!user.LOAI_USER.StartsWith("KH") || !string.IsNullOrWhiteSpace(user.PARENT_USER_NAME))
                {
                    dto.INSERT_USER = user.USER_NAME;
                }
                else
                    dto.lstMA_KHACH_HANG = lstCust.Select(x => x.CUSTOMER_CODE).Distinct().ToList();

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_DON_HANG_SGEV", inputCa);
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    lst = lst.OrderBy(x => x.id).OrderByDescending(x => x.PO).OrderByDescending(x => x.UPDATE_DATE).ToList();

                    if (!string.IsNullOrWhiteSpace(NGUOI_NHAN))
                    {
                        List<CSS_OP_BOOKING_SGEV_WEB_EX> lstTemp = lst.Where(x => x.TEN_NGUOI_NHAN.ToLower().Contains(NGUOI_NHAN.ToLower())).ToList();
                        if (lstTemp == null || lstTemp.Count == 0)
                        {
                            lstTemp = lst.Where(x => x.SDT_NHAN.ToLower().Contains(NGUOI_NHAN.ToLower())).ToList();
                        }
                        lst = lstTemp;
                    }

                    List<string> lstPO = new List<string>();

                    foreach (var item in lst)
                    {
                        if (!lstPO.Contains(item.PO))
                        {
                            lstPO.Add(item.PO);
                            item.Check = true;
                        }

                        CommonData.splitService(item);

                        item.UPDATE_TIME_STRING = item.UPDATE_TIME.ToString("dd/MM/yyyy");
                        var loahang = CommonData.lstLoaiHang().FirstOrDefault(u => u.Value == item.LOAI_BPBK);
                        if (loahang != null)
                        {
                            item.LOAI_BPBK = loahang.Text;
                        }
                        CommonData.splitService(item);

                        item.TEN_DICH_VU = CommonData.getDichVuTuMa(item.DICH_VU);
                        if (ClientSession.getCustomer().USER_NAME != item.INSERT_USER)
                            item.TEN_NGUOI_GUI = "[" + item.INSERT_USER + "] " + item.TEN_NGUOI_GUI;
                    }

                    if (string.IsNullOrWhiteSpace(MA_DAT_HANG) && string.IsNullOrWhiteSpace(NGUOI_NHAN))
                    {
                        foreach (var item in lst)
                        {
                            if (item.Check)
                                item.SL_BILL = lst.Count(x => x.PO == item.PO);
                        }
                    }

                    msg.Error = false;
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }

                Session["DSDonHang"] = lst;
                return Json(msg);

            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        #endregion

        //Danh sách đơn hàng chi tiết
        #region
        public ActionResult DanhSachDonHangChiTiet(string id)
        {
            ViewBag.PO = id;
            return View();
        }
        public ActionResult DachSachChiTiet(string id)
        {
            List<CSS_OP_BOOKING_SGEV_WEB_EX> lstEnd = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { PO = id };
            string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_DON_HANG_SGEV", inputCa);
            if (dtoRes.Status == "OK")
            {
                lstEnd = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                lstEnd = lstEnd.OrderBy(x => x.id).OrderByDescending(x => x.PO).OrderByDescending(x => x.UPDATE_DATE).ToList();
                var i = 1;
                foreach (var item in lstEnd)
                {
                    item.STT = i;
                    i++;

                    item.UPDATE_TIME_STRING = item.UPDATE_TIME.ToString("dd/MM/yyyy");
                    CommonData.splitService(item);
                }
            }
            return PartialView("_DanhSachChiTiet", lstEnd);
        }
        #endregion

        //DonHangMoiTuSinh
        [HttpGet]
        public ActionResult DonHangMoiTuSinh(string id)
        {
            CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

            List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
            else
                dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);
            lstDonHangTuSinh.Clear();

            CSS_OP_BOOKING_SGEV_WEB_EX obj = new CSS_OP_BOOKING_SGEV_WEB_EX();
            ViewData["item_Copy"] = obj;

            if (!string.IsNullOrWhiteSpace(id))
            {
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new List<string> { id });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_THONG_TIN_BILL", inputCa);
                if (dtoRes.Status == "OK")
                {
                    List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    foreach (var item in lst)
                    {
                        CommonData.splitService(item);
                    }
                    ViewData["item_Copy"] = lst[0];
                    ViewBag.KT_HANG = lst[0].KT_HANG;
                    ViewBag.TINH_LH = lst[0].MA_TINH_LH;
                    ViewBag.HUYEN_LH = lst[0].MA_HUYEN_LH;
                }
            }
            else
            {
                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { MA_KHACH_HANG = user.MA_KHACH_HANG_CON };
                if (!string.IsNullOrWhiteSpace(user.PARENT_USER_NAME) && user.CUSTOMER_CODE == user.MA_KHACH_HANG_CON)
                {
                    dto.INSERT_USER = user.USER_NAME;
                }

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_DIA_CHI", inputCa);
                if (dtoRes.Status == "OK")
                {
                    obj = JsonConvert.DeserializeObject<CSS_OP_BOOKING_SGEV_WEB_EX>(dtoRes.ResponseData);
                    ViewData["item_Copy"] = obj;

                    ViewBag.KT_HANG = obj.KT_HANG;
                    ViewBag.TINH_LH = obj.MA_TINH_LH;
                    ViewBag.HUYEN_LH = obj.MA_HUYEN_LH;
                }
            }

            ViewData["_listZone"] = CommonData._listZone;
            return View();
        }

        //upload, tạo đơn hàng, xóa đơn hàng
        #region
        [HttpPost]
        public ActionResult DonHangMoiTuSinh(CSS_OP_BOOKING_SGEV_WEB_EX model)
        {
            JMessage msg = new JMessage();
            try
            {
                string lag = Request.Cookies["language"].Value;
                lag = "vi-VN";

                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                    lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
                else
                    dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);

                if (!CommonData.IsValidString(CommonData.RemoveUnicode(model.DIA_CHI)))
                {
                    msg.Error = true;
                    msg.Title = Booking.msgDIA_CHI1;
                    return Json(msg);
                }
                if (model.DIA_CHI.Length > 300)
                {
                    return Json(Service.ErrorV2(Booking.errDiaChiNguoiNhan));
                }
                if (string.IsNullOrWhiteSpace(model.SDT_GUI))
                {
                    return Json(Service.ErrorV2(Booking.errSDTgui));
                }

                if (!string.IsNullOrWhiteSpace(model.NGAY_HEN_PHAT_SIEU_THI))
                {
                    DateTime date = DateTime.ParseExact(model.NGAY_HEN_PHAT_SIEU_THI, "dd/MM/yyyy HH:mm", null);
                    if (date.Date.Subtract(DateTime.Now.Date).TotalDays <= 0)
                        return Json(Service.ErrorV2(Booking.erroNgayPhatSTLonHonNgayHT));
                }

                if (string.IsNullOrWhiteSpace(user.MA_KHACH_HANG_CON))
                {
                    return Json(Service.ErrorV2(Booking.msgChonChiNhanh));
                }
                if (user.isSudungMDS == "true")
                {
                    if (model.SO_THAM_CHIEU==null||model.SO_THAM_CHIEU.Length < 8 || model.SO_THAM_CHIEU.Length > 20)
                    {
                        return Json(Service.ErrorV2("Mã đối soát " + model.SO_THAM_CHIEU + " không hợp lệ. Số tham chiếu chỉ được từ 8 đến 20 kí tự."));

                    }
                    foreach (var item in lstDonHangTuSinh)
                    {
                        if (item.SO_THAM_CHIEU == model.SO_THAM_CHIEU)
                        {
                            return Json(Service.ErrorV2("Mã đối soát " + model.SO_THAM_CHIEU + " đã được thêm lên lưới vui lòng kiểm tra lại."));
                        }
                    }
                }


                model.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
                model.TEN_KHACH_HANG = user.FULL_NAME;

                model.INSERT_USER = model.UPDATE_USER = user.USER_NAME;

                model.id = lstDonHangTuSinh.Count;
                lstDonHangTuSinh.Add(model);
                msg.Error = false;
                msg.Object = model;
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;
            }

            return Json(msg);
        }
        [HttpPost]
        public ActionResult ListDatHangTuSinh(CSS_OP_BOOKING_SGEV_WEB_EX model, string ngayyc, bool taoTuMT)
        {
            string lag = Request.Cookies["language"].Value;
            lag = "vi-VN";
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();

                //var date = DateTime.ParseExact(ngayyc, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                //if (date.Subtract(DateTime.Now.Date).TotalDays < 0)
                //{
                //    return Json(Service.ErrorV2(Booking.erroNgayYCLonHonNgayHT));
                //}

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                    lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
                else
                {
                    dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);
                }

                if (string.IsNullOrWhiteSpace(model.SDT_NHAN) && lstDonHangTuSinh.Count == 0)
                {
                    return Json(Service.ErrorV2(Booking.errKhongCoDH));
                }

                if (!string.IsNullOrWhiteSpace(model.SDT_NHAN) && lstDonHangTuSinh.Count == 0)
                {
                    if (model.DIA_CHI.Length > 300)
                    {
                        return Json(Service.ErrorV2(Booking.errDiaChiNguoiNhan));
                    }
                    if (string.IsNullOrWhiteSpace(model.SDT_GUI))
                    {
                        return Json(Service.ErrorV2(Booking.errSDTgui));
                    }

                    if (string.IsNullOrWhiteSpace(user.MA_KHACH_HANG_CON))
                    {
                        return Json(Service.ErrorV2(Booking.msgChonChiNhanh));
                    }

                    model.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
                    model.TEN_KHACH_HANG = user.FULL_NAME;

                    model.INSERT_USER = model.UPDATE_USER = user.USER_NAME;

                    lstDonHangTuSinh.Add(model);
                }

                foreach (var item in lstDonHangTuSinh)
                {
                    if (string.IsNullOrWhiteSpace(item.TEN_NGUOI_GUI))
                    {
                        item.TEN_NGUOI_GUI = model.TEN_NGUOI_GUI;
                    }
                    if (string.IsNullOrWhiteSpace(item.SDT_GUI))
                    {
                        item.SDT_GUI = model.SDT_GUI;
                    }
                    if (string.IsNullOrWhiteSpace(item.DIA_CHI_NGUOI_GUI))
                    {
                        item.DIA_CHI_NGUOI_GUI = model.DIA_CHI_NGUOI_GUI;
                    }

                    if (!string.IsNullOrWhiteSpace(item.SO_KIEN) && item.SO_KIEN != "0")
                    {
                        item.TT_BO_SUNG = "{Số kiện:" + item.SO_KIEN + "}";
                    }

                    if (!string.IsNullOrWhiteSpace(item.SO_HOA_DON))
                    {
                        if (item.SO_HOA_DON.Contains("}") || item.SO_HOA_DON.Contains("{"))
                            return Json(Service.ErrorV2(Booking.errSoHDkokitudacbiet));
                        item.TT_BO_SUNG += ",{Số hóa đơn:" + item.SO_HOA_DON + "}";
                    }

                    if (!string.IsNullOrWhiteSpace(item.GT_DON_HANG))
                    {
                        item.TT_BO_SUNG += ",{GTDH:" + item.GT_DON_HANG + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.HOAN_BBBG))
                    {
                        if (item.HOAN_BBBG.Contains("}") || item.HOAN_BBBG.Contains("{"))
                            return Json(Service.ErrorV2(Booking.errBBBGkokitudacbiet));
                        item.TT_BO_SUNG += ",{Hoàn BBBG:" + item.HOAN_BBBG + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.NGAY_HEN_PHAT_SIEU_THI))
                    {
                        item.TT_BO_SUNG += ",{TG_PST:" + item.NGAY_HEN_PHAT_SIEU_THI + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.DONG_KIEM))
                    {
                        item.TT_BO_SUNG += ",{Đồng kiểm:" + item.DONG_KIEM + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.PHAT_TAN_TAY))
                    {
                        item.TT_BO_SUNG += ",{PTT:" + item.PHAT_TAN_TAY + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.BAO_PHAT))
                    {
                        item.TT_BO_SUNG += ",{BP:" + item.BAO_PHAT + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.HT_THANH_TOAN))
                    {
                        item.TT_BO_SUNG += ",{HTTT:" + item.HT_THANH_TOAN + "}";
                    }
                    item.isSudungMDS = user.isSudungMDS;
                    item.NGAY_KH_DU_KIEN_LAY_HANG = DateTime.Now;
                    item.MA_HUYEN_LH = model.MA_HUYEN_LH;
                    item.MA_TINH_LH = model.MA_TINH_LH;
                    item.KT_HANG = model.KT_HANG;
                    item.TEN_NGUOI_NHAN = item.TEN_NGUOI_NHAN.Trim();
                    item.DIA_CHI = item.DIA_CHI.Trim();
                    var loahang = CommonData.lstLoaiHang().FirstOrDefault(u => u.Text == item.LOAI_BPBK);
                    if (loahang != null)
                    {
                        item.LOAI_BPBK = loahang.Value;
                    }
                }

                string inputCa = JsonConvert.SerializeObject(lstDonHangTuSinh);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("DAT_HANG_TU_SINH_SAGAWA", inputCa);
                if (dtoRes.Status == "OK")
                {
                    List<CSS_OP_BOOKING_SGEV_WEB_EX> lstIn = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    Session["LstIn"] = lstIn;

                    msg.Error = false;
                    msg.Object = lstIn[0];

                    lstDonHangTuSinh.Clear();
                }
                else
                {
                    if (taoTuMT)
                    {
                        lstDonHangTuSinh.Clear();
                    }
                    msg.Title = dtoRes.Description;
                    msg.Error = true;
                }
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;
            }

            return Json(msg);
        }
        [HttpPost]
        public ActionResult Upload(CSS_OP_BOOKING_SGEV_WEB_EX model)
        {
            JMessage msg = new JMessage();
            string lag = Request.Cookies["language"].Value;
            lag = "vi-VN";
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                    lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
                else
                    dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);

                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet[model.TT_BO_SUNG];
                        if (workSheet == null)
                        {
                            return Json(Service.ErrorV2(Booking.errKhongTimThayTenSheet + " " + model.TT_BO_SUNG));
                        }
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        if (noOfRow - 1 > 1000)
                        {
                            return Json(Service.ErrorV2(Booking.errChiUp1000dong));
                        }
                        int id = 0;
                        var lstSoThamChieu = new List<string>();
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            id++;
                            CSS_OP_BOOKING_SGEV_WEB_EX pod = new CSS_OP_BOOKING_SGEV_WEB_EX();
                            if (string.IsNullOrWhiteSpace(user.MA_KHACH_HANG_CON))
                            {
                                Service.ErrorV2(Booking.msgChonChiNhanh);
                            }
                            if ((workSheet.Cells[rowIterator, 1].Value == null || workSheet.Cells[rowIterator, 1].Value.ToString() == "")
                                && (workSheet.Cells[rowIterator, 2].Value == null || workSheet.Cells[rowIterator, 2].Value.ToString() == "")
                                && (workSheet.Cells[rowIterator, 6].Value == null || workSheet.Cells[rowIterator, 6].Value.ToString() == "")
                                && (workSheet.Cells[rowIterator, 5].Value == null || workSheet.Cells[rowIterator, 5].Value.ToString() == "")
                                )
                            {
                                break;
                            }
                            pod.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
                            pod.TEN_KHACH_HANG = user.FULL_NAME;

                            pod.DIA_CHI_NGUOI_GUI = workSheet.Cells[rowIterator, 1].Value == null ? string.Empty : workSheet.Cells[rowIterator, 1].Value.ToString().Trim();
                            pod.TEN_NGUOI_GUI = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                            pod.TEN_NGUOI_NHAN = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString().Trim();
                            pod.SDT_NHAN = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString().Trim();
                            pod.DIA_CHI = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString().Trim();
                            pod.TINH_NHAN = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString().Trim();
                            pod.HUYEN_NHAN = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString().Trim();
                            pod.NOI_DUNG = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString().Trim();
                            //pod.TRONG_LUONG = workSheet.Cells[rowIterator, 10].Value == null ? 0 : decimal.Parse(workSheet.Cells[rowIterator, 10].Value.ToString().Trim());
                            pod.SO_THAM_CHIEU = workSheet.Cells[rowIterator, 10].Value == null ? string.Empty : workSheet.Cells[rowIterator, 10].Value.ToString().Trim();
                            pod.SDT_GUI = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString().Trim();
                            //pod.SO_KIEN = workSheet.Cells[rowIterator, 12].Value == null ? string.Empty : workSheet.Cells[rowIterator, 12].Value.ToString().Trim();
                            //pod.SO_HOA_DON = workSheet.Cells[rowIterator, 14].Value == null ? string.Empty : workSheet.Cells[rowIterator, 14].Value.ToString().Trim();
                            pod.DICH_VU = workSheet.Cells[rowIterator, 11].Value == null ? string.Empty : workSheet.Cells[rowIterator, 11].Value.ToString().Trim();
                            pod.LOAI_BPBK = workSheet.Cells[rowIterator, 12].Value == null ? string.Empty : workSheet.Cells[rowIterator, 12].Value.ToString().Trim();
                            pod.HT_THANH_TOAN = workSheet.Cells[rowIterator, 13].Value == null ? string.Empty : workSheet.Cells[rowIterator, 13].Value.ToString().Trim();
                            pod.SO_COD_STRING = workSheet.Cells[rowIterator, 14].Value == null ? string.Empty : workSheet.Cells[rowIterator, 14].Value.ToString().Trim();
                            pod.id = lstDonHangTuSinh.Count;
                            if (!string.IsNullOrWhiteSpace(pod.LOAI_BPBK))
                            {
                                var dv = CommonData.lstLoaiHang().FirstOrDefault(u => u.Value == pod.LOAI_BPBK.Trim().ToUpper());
                                if (dv != null)
                                {
                                    pod.LOAI_BPBK = dv.Text;
                                }
                                else
                                {
                                    return Json(Service.ErrorV2("Loại BPBK không đúng vui lòng nhập đúng mã dịch vụ" + ". Dòng số: " + rowIterator));
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(pod.SO_COD_STRING))
                            {
                                try
                                {
                                    pod.SO_COD = decimal.Parse(pod.SO_COD_STRING);
                                }
                                catch
                                {
                                    return Json(Service.ErrorV2("Số tiền COD phải là số" + ". Dòng số: " + rowIterator));
                                }
                            }
                            if (string.IsNullOrWhiteSpace(pod.HT_THANH_TOAN))
                            {
                                pod.HT_THANH_TOAN = "Thanh toán cuối tháng";
                            }
                            if (!CommonData.lstHTTT().Contains(pod.HT_THANH_TOAN.Trim()))
                            {
                                return Json(Service.ErrorV2("Hình thức thanh toán không đúng" + ". Dòng số: " + rowIterator));
                            }

                            if (string.IsNullOrWhiteSpace(pod.TEN_NGUOI_NHAN))
                            {
                                return Json(Service.ErrorV2(Booking.NGUOI_NHAN + ". Dòng số: " + rowIterator));
                            }
                            if (user.isSudungMDS == "true")
                            {
                                if (string.IsNullOrWhiteSpace(pod.SO_THAM_CHIEU))
                                {
                                    return Json(Service.ErrorV2("Mã đối soát không được để trống . Dòng số: " + rowIterator));
                                }
                                if (lstSoThamChieu.Contains(pod.SO_THAM_CHIEU))
                                {
                                    return Json(Service.ErrorV2("Mã đối soát " + pod.SO_THAM_CHIEU + " bị trùng . Dòng số: " + rowIterator));
                                }
                                lstSoThamChieu.Add(pod.SO_THAM_CHIEU);
                            }

                            if (string.IsNullOrWhiteSpace(pod.SDT_NHAN))
                            {
                                return Json(Service.ErrorV2(Booking.SDT_NHAN + ". Dòng số: " + rowIterator));
                            }

                            if (string.IsNullOrWhiteSpace(pod.DIA_CHI))
                            {
                                return Json(Service.ErrorV2(Booking.DIA_CHI + ". Dòng số: " + rowIterator));
                            }

                            if (!CommonData.IsValidString(CommonData.RemoveUnicode(pod.DIA_CHI)))
                            {
                                return Json(Service.ErrorV2(Booking.msgDIA_CHI1 + ". Dòng số: " + rowIterator));
                            }
                            if (pod.DIA_CHI.Length > 300)
                            {
                                return Json(Service.ErrorV2(Booking.errDiaChiNguoiNhan + ". Dòng số: " + rowIterator));
                            }

                            if (string.IsNullOrWhiteSpace(pod.TINH_NHAN))
                            {
                                return Json(Service.ErrorV2(Booking.vadtinhnhan));
                            }
                            //if (string.IsNullOrWhiteSpace(pod.HUYEN_NHAN))
                            //{
                            //    return Json(Service.ErrorV2(Booking.vadhuyennhan));
                            //}
                            if (string.IsNullOrWhiteSpace(pod.SDT_GUI))
                            {
                                return Json(Service.ErrorV2(Booking.errSDTgui));
                            }
                            if (string.IsNullOrWhiteSpace(pod.DIA_CHI_NGUOI_GUI))
                            {
                                return Json(Service.ErrorV2(Booking.msg_dia_chi_nguoi_gui + ". Dòng số: " + rowIterator));
                            }
                            if (!string.IsNullOrWhiteSpace(pod.DICH_VU))
                            {
                                var dv = CommonData.lstDichVu().FirstOrDefault(u => u.Value == pod.DICH_VU.Trim().ToUpper());
                                if (dv != null)
                                {
                                    pod.DICH_VU = pod.DICH_VU.Trim().ToUpper();
                                }
                                else
                                {
                                    return Json(Service.ErrorV2("Dịch vụ không đúng vui lòng nhập đúng mã dịch vụ" + ". Dòng số: " + rowIterator));
                                }
                            }

                            //if (workSheet.Cells[rowIterator, 15].Value != null)
                            //{
                            //    try
                            //    {
                            //        var a = Convert.ToDecimal(workSheet.Cells[rowIterator, 15].Value.ToString().Trim());
                            //        pod.GT_DON_HANG = a.ToString("N0");
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        return Json(Service.ErrorV2(Booking.errGTDHSaiDinhDang + ". Dòng số: " + rowIterator));
                            //    }
                            //}
                            //pod.HOAN_BBBG = workSheet.Cells[rowIterator, 16].Value == null ? string.Empty : workSheet.Cells[rowIterator, 16].Value.ToString().Trim();

                            //if (workSheet.Cells[rowIterator, 17].Value != null)
                            //{
                            //    try
                            //    {
                            //        pod.NGAY_HEN_PHAT_SIEU_THI = workSheet.Cells[rowIterator, 17].Value.ToString().Trim();
                            //        var a = DateTime.ParseExact(pod.NGAY_HEN_PHAT_SIEU_THI, "dd/MM/yyyy HH:mm", null);
                            //        if (a.Date.Subtract(DateTime.Now.Date).TotalDays <= 0)
                            //            return Json(Service.ErrorV2(Booking.erroNgayPhatSTLonHonNgayHT + ". Dòng số: " + rowIterator));
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        return Json(Service.ErrorV2(Booking.errTGHenSaiDinhDang + ". Dòng số: " + rowIterator));
                            //    }
                            //}


                            if (!string.IsNullOrWhiteSpace(pod.TINH_NHAN))
                            {
                                pod.MA_TINH_NHAN = CommonData.GetZone(pod.TINH_NHAN);
                            }

                            if (string.IsNullOrWhiteSpace(pod.MA_TINH_NHAN))
                            {
                                return Json(Service.Error(rowIterator, "Tỉnh nhận không đúng. Vui lòng nhập tỉnh nhận theo danh sách tỉnh/TP dưới đây."));
                            }

                            if (!string.IsNullOrWhiteSpace(pod.MA_TINH_NHAN) && !string.IsNullOrWhiteSpace(pod.HUYEN_NHAN))
                            {
                                pod.MA_HUYEN_NHAN = CommonData.GetDistrict(pod.MA_TINH_NHAN, pod.HUYEN_NHAN);
                            }

                            pod.INSERT_USER = pod.UPDATE_USER = user.USER_NAME;
                            lstDonHangTuSinh.Add(pod);
                        }

                        //string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(lstDonHangTuSinh);
                        //BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("UPLOAD_DON_HANG_TU_SINH_SAGAWA", inputCa);
                        //if (dtoRes.Status == "OK")
                        //{
                        //    return Json(Service.OK_V1());
                        //}
                        //else
                        //{
                        //    return Json(Service.ErrorV2(dtoRes.Description));
                        //}
                    }
                    return Json(Service.OK_V1());
                }
                else
                {
                    return Json(Service.ErrorV2(Booking.errKhongTimThayFile));
                }
            }
            catch (Exception e)
            {
                msg.Error = true;
                msg.Title = e.Message;
                return Json(msg);
            }
        }
        [HttpPost]
        public ActionResult XoaDonHangTuSinh(string id)
        {
            JMessage msg = new JMessage();
            CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

            List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
            else
                dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);
            int i = int.Parse(id);
            lstDonHangTuSinh.Remove(lstDonHangTuSinh.FirstOrDefault(x => x.id == i));
            msg.Error = false;

            return Json(msg);
        }
        public ActionResult _grvListBooking()
        {
            CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
            else
                dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);
            return PartialView("_grvListBooking", lstDonHangTuSinh);
        }
        #endregion

        //Action liên quan
        #region
        public ActionResult ListTrackAndTrace(string MaBPBK)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                var lag = "";
                //if (Request.Cookies["language"].Value != "vi")
                //{
                //    lag = "";
                //}
                //else
                //    lag = "vi-VN";
                lag = "vi-VN";
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_TRACK_AND_TRACE() { HAWB_NO = MaBPBK.Replace("\t", "").Replace("\n", ",").ToUpper(), DESC = lag });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_TRACK_AND_TRACE", inputCa);
                List<CSS_TRACK_AND_TRACE> lst;
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_TRACK_AND_TRACE>>(dtoRes.ResponseData);

                    foreach (var item in lst)
                    {
                        item.DESC = item.DESC.Replace("[SGV]DASH LOGISTICS", "Hub_Das");
                        item.DESC = item.DESC.Replace("[SGV]NASCO", "Hub_Nas");
                        item.DESC = item.DESC.Replace("[SGV]VIETTEL", "Hub_Viet");
                        if (lag == "")
                        {
                            item.DESC = item.DESC.Replace("vào lúc", "at").Replace("DA PHAT", "Deliveried successfully to");
                        }
                        item.INSERT_TIME_STRING = item.INSERT_TIME.ToString("dd/MM/yyyy HH:mm");
                        item.HAWB_NO = item.HAWB_NO.ToUpper().Trim();
                    }
                    lst = lst.OrderBy(x => x.HAWB_NO).OrderByDescending(x => x.INSERT_TIME).ToList();
                    var dic = lst.GroupBy(x => x.HAWB_NO).ToDictionary(x => x.Key, x => x.ToList());
                    msg.Object = dic;
                    msg.Title = "Get list success!";
                }
                else
                {
                    msg.Title = dtoRes.Description;
                    msg.Error = true;
                }

            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult XOABPBK(string MA_BPBK, bool isPO)
        {
            JMessage msg = new JMessage();
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;

                CSS_OP_BOOKING_SGEV_WEB dtoXoa = new CSS_OP_BOOKING_SGEV_WEB();

                if (!isPO)
                {
                    dtoXoa = lst.FirstOrDefault(x => x.MA_BPBK == MA_BPBK);

                    if (dtoXoa == null)
                        return Json(Service.ErrorV2(Booking.errKhongTimThayBPBK));

                    dtoXoa = new CSS_OP_BOOKING_SGEV_WEB { MA_BPBK = dtoXoa.MA_BPBK, INSERT_USER = dtoXoa.INSERT_USER, UPDATE_USER = user.USER_NAME };
                }
                else
                {
                    dtoXoa = lst.FirstOrDefault(x => x.PO == MA_BPBK);

                    if (dtoXoa == null)
                        return Json(Service.ErrorV2(Booking.errkhongtimthaymadh));

                    dtoXoa = new CSS_OP_BOOKING_SGEV_WEB { PO = dtoXoa.PO, INSERT_USER = dtoXoa.INSERT_USER, UPDATE_USER = user.USER_NAME };
                }

                string input = JsonConvert.SerializeObject(dtoXoa);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("XOA_BPBK", input);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.Title = "Xóa mã bưu phẩm bưu kiện thành công!";
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult PhieuGui(string PO)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstEnd = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                if (string.IsNullOrWhiteSpace(PO))
                {
                    lstEnd = Session["LstIn"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
                    Session.Remove("LstIn");
                }
                else
                {
                    if (Session["DSDonHang"] != null)
                    {
                        lstEnd = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
                        lstEnd = lstEnd.Where(x => x.PO == PO).ToList();
                    }
                }

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstIn = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                foreach (var item in lstEnd)
                {
                    CSS_OP_BOOKING_SGEV_WEB_EX dto = new CSS_OP_BOOKING_SGEV_WEB_EX();
                    ObjectHelper.Inject(item, dto);

                    dto.TEN_NGUOI_NHAN = dto.TEN_NGUOI_NHAN.Trim();
                    dto.DIA_CHI = dto.DIA_CHI.Trim();


                    if (dto.DICH_VU == "CPT")
                    {
                        dto.CPT = "V";
                    }
                    if (dto.DICH_VU == "CPN")
                    {
                        dto.CPN = "V";
                    }
                    if (dto.DICH_VU == "PHT" || dto.DICH_VU == "PYC")
                    {
                        dto.PHT = "V";
                    }
                    if (dto.DICH_VU == "PTK")
                    {
                        dto.PTK = "V";
                    }
                    if (dto.DICH_VU == "EXP" || dto.DICH_VU == "GTC")
                    {
                        dto.EXP = "V";
                    }
                    if (dto.DICH_VU == "PCT")
                    {
                        dto.PCT = "V";
                    }

                    if (dto.LOAI_BPBK == "DOX" || dto.LOAI_BPBK == "Tài liệu")
                    {
                        dto.DOX = "V";
                    }
                    if (dto.LOAI_BPBK == "SPX" || dto.LOAI_BPBK == "Hàng hóa")
                    {
                        dto.SPX = "V";

                    }

                    CommonData.splitService(dto);

                    if (!string.IsNullOrWhiteSpace(dto.DONG_KIEM))
                    {
                        dto.DONG_KIEM = "V";
                    }
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.SO_HOA_DON) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Số hóa đơn: " + dto.SO_HOA_DON;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.GT_DON_HANG) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Giá trị đơn hàng: " + dto.GT_DON_HANG;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.SO_THAM_CHIEU) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Mã đối soát: " + dto.SO_THAM_CHIEU;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.HOAN_BBBG) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Hoàn BBBG: " + dto.HOAN_BBBG;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.NGAY_HEN_PHAT_SIEU_THI) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Hẹn phát siêu thị: " + dto.NGAY_HEN_PHAT_SIEU_THI;
                    dto.DIA_CHI = string.IsNullOrWhiteSpace(dto.HUYEN_NHAN) ? dto.DIA_CHI : dto.DIA_CHI + ". Quận/Huyện: " + dto.HUYEN_NHAN;
                    if (dto.TT_BO_SUNG.Contains("Phát tận tay"))
                    {
                        dto.NOI_DUNG += Environment.NewLine + "Phát tận tay";
                    }
                    if (dto.TT_BO_SUNG.Contains("Báo phát"))
                    {
                        dto.NOI_DUNG += Environment.NewLine + "Báo phát";
                    }
                    CommonData.splitService(dto);
                    if (dto.HT_THANH_TOAN == "Thanh toán cuối tháng")
                    {
                        dto.TT_CT = "V";
                    }
                    else
                    {
                        dto.TT_DN = "V";
                    }
                    //if (!string.IsNullOrWhiteSpace(dto.DICH_VU))
                    //{
                    //    var dv = CommonData.lstDichVu().FirstOrDefault(u => u.Value == dto.DICH_VU);
                    //    if (dv != null)
                    //    {
                    //        dto.NOI_DUNG += Environment.NewLine + dv.Text;
                    //    }
                    //}

                    dto.TRONG_LUONG_STRING = dto.TRONG_LUONG == 0 ? "" : dto.TRONG_LUONG.ToString("N2");
                    dto.SO_COD_STRING = dto.SO_COD == 0 ? "" : dto.SO_COD.ToString("N0");

                    //dto.NGAY_KY_GUI = "......h, ngày......tháng......năm........";

                    dto.MA_MIEN = CommonData.LayMaMienTheoTinhNhan(dto.MA_TINH_NHAN);
                    dto.MA_MIEN = dto.MA_MIEN + " - " + dto.MA_TINH_NHAN;
                    lstIn.Add(dto);
                }

                lstIn = lstIn.OrderBy(x => x.id).ToList();

                var kieuin = user.getKieuIn();
                if (user.CUSTOMER_CODE.Equals("0131858"))
                {
                    rptPhieuGuiSagawaTuSinhA5_VinID rp = new rptPhieuGuiSagawaTuSinhA5_VinID();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else if (kieuin == "A5")
                {
                    rptPhieuGuiTuSinhA5 rp = new rptPhieuGuiTuSinhA5();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else if (kieuin == "A4_3")
                {
                    rptPhieuGuiSagawaTuSinh_3Lien rp = new rptPhieuGuiSagawaTuSinh_3Lien();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else
                {
                    rptPhieuGuiTuSinh rp = new rptPhieuGuiTuSinh();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["AddError"] = ex.Message;
                return View("DanhSachDonHang");
            }
        }
        public ActionResult SuDungMaDoiSoatThayMaBill(bool ischeck)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
                user.LOAI_TK = user.updateLoaiTK("SU_DUNG_MDS", ischeck.ToString());

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = user.USER_NAME, LOAI_TK = user.LOAI_TK });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("CAP_NHAT_KIEU_IN", inputCa);
                if (dtoRes.Status == "OK")
                {
                    Session["User"] = user;
                }
            }
            catch (Exception ex)
            {

            }
            return Json("OK");
        }
        public ActionResult InNgayLayHang(bool ischeck)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
                user.LOAI_TK = user.updateLoaiTK("IN_NGAY_LAY_HANG", ischeck.ToString());

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = user.USER_NAME, LOAI_TK = user.LOAI_TK });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("CAP_NHAT_KIEU_IN", inputCa);
                if (dtoRes.Status == "OK")
                {
                    Session["User"] = user;
                }
            }
            catch (Exception ex)
            {

            }
            return Json("OK");
        }
        public ActionResult PhieuGui_MaBPBK(string MA_BPBK)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstEnd = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                var lstBPBK = MA_BPBK.Split(':').ToList();

                if (Session["DSDonHang"] != null)
                {
                    lstEnd = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
                    lstEnd = lstEnd.Where(x => lstBPBK.Contains(x.MA_BPBK)).ToList();
                }

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstIn = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                foreach (var item in lstEnd)
                {
                    CSS_OP_BOOKING_SGEV_WEB_EX dto = new CSS_OP_BOOKING_SGEV_WEB_EX();
                    ObjectHelper.Inject(item, dto);

                    dto.TEN_NGUOI_NHAN = dto.TEN_NGUOI_NHAN.Trim();
                    dto.DIA_CHI = dto.DIA_CHI.Trim();


                    if (dto.DICH_VU == "CPT")
                    {
                        dto.CPT = "V";
                    }
                    if (dto.DICH_VU == "CPN")
                    {
                        dto.CPN = "V";
                    }
                    if (dto.DICH_VU == "PHT" || dto.DICH_VU == "PYC")
                    {
                        dto.PHT = "V";
                    }
                    if (dto.DICH_VU == "PTK")
                    {
                        dto.PTK = "V";
                    }
                    if (dto.DICH_VU == "EXP" || dto.DICH_VU == "GTC")
                    {
                        dto.EXP = "V";
                    }
                    if (dto.DICH_VU == "PCT")
                    {
                        dto.PCT = "V";
                    }

                    if (dto.LOAI_BPBK == "DOX" || dto.LOAI_BPBK == "Tài liệu")
                    {
                        dto.DOX = "V";
                    }
                    if (dto.LOAI_BPBK == "SPX" || dto.LOAI_BPBK == "Hàng hóa")
                    {
                        dto.SPX = "V";

                    }

                    CommonData.splitService(dto);

                    if (!string.IsNullOrWhiteSpace(dto.DONG_KIEM))
                    {
                        dto.DONG_KIEM = "V";
                    }
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.SO_HOA_DON) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Số hóa đơn: " + dto.SO_HOA_DON;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.GT_DON_HANG) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Giá trị đơn hàng: " + dto.GT_DON_HANG;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.SO_THAM_CHIEU) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Mã đối soát: " + dto.SO_THAM_CHIEU;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.HOAN_BBBG) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Hoàn BBBG: " + dto.HOAN_BBBG;
                    dto.NOI_DUNG = string.IsNullOrWhiteSpace(dto.NGAY_HEN_PHAT_SIEU_THI) ? dto.NOI_DUNG : dto.NOI_DUNG + Environment.NewLine + "Hẹn phát siêu thị: " + dto.NGAY_HEN_PHAT_SIEU_THI;
                    dto.DIA_CHI = string.IsNullOrWhiteSpace(dto.HUYEN_NHAN) ? dto.DIA_CHI : dto.DIA_CHI + ". Quận/Huyện: " + dto.HUYEN_NHAN;
                    if (dto.TT_BO_SUNG.Contains("Phát tận tay"))
                    {
                        dto.NOI_DUNG += Environment.NewLine + "Phát tận tay";
                    }
                    if (dto.TT_BO_SUNG.Contains("Báo phát"))
                    {
                        dto.NOI_DUNG += Environment.NewLine + "Báo phát";
                    }
                    CommonData.splitService(dto);
                    if (dto.HT_THANH_TOAN == "Thanh toán cuối tháng")
                    {
                        dto.TT_CT = "V";
                    }
                    else
                    {
                        dto.TT_DN = "V";
                    }
                    //if (!string.IsNullOrWhiteSpace(dto.DICH_VU))
                    //{
                    //    var dv = CommonData.lstDichVu().FirstOrDefault(u => u.Value == dto.DICH_VU);
                    //    if (dv != null)
                    //    {
                    //        dto.NOI_DUNG += Environment.NewLine + dv.Text;
                    //    }
                    //}

                    dto.TRONG_LUONG_STRING = dto.TRONG_LUONG == 0 ? "" : dto.TRONG_LUONG.ToString("N2");
                    dto.SO_COD_STRING = dto.SO_COD == 0 ? "" : dto.SO_COD.ToString("N0");

                    //dto.MA_MIEN = CommonData.LayMaMienTheoTinhNhan(dto.MA_TINH_NHAN);
                    //dto.MA_MIEN = dto.MA_MIEN + " - " + dto.MA_TINH_NHAN;
                    lstIn.Add(dto);

                }

                lstIn = lstIn.OrderBy(x => x.id).ToList();

                var kieuin = user.getKieuIn();
                if (user.CUSTOMER_CODE.Equals("0131858"))
                {
                    rptPhieuGuiSagawaTuSinhA5_VinID rp = new rptPhieuGuiSagawaTuSinhA5_VinID();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else if (kieuin == "A5")
                {
                    rptPhieuGuiTuSinhA5 rp = new rptPhieuGuiTuSinhA5();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else if (kieuin == "A4_3")
                {
                    rptPhieuGuiSagawaTuSinh_3Lien rp = new rptPhieuGuiSagawaTuSinh_3Lien();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
                else
                {
                    rptPhieuGuiTuSinh rp = new rptPhieuGuiTuSinh();
                    rp.SetData(lstIn);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //rp.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                        rp.ExportToPdf(ms, new PdfExportOptions());
                        return File(ms.ToArray(), "application/pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["AddError"] = ex.Message;
                return View("DanhSachDonHang");
            }
        }
        public ActionResult ChuyenMaKhachHang(string lstBPBK, string maKH)
        {
            try
            {
                List<string> lstBill = new List<string>();
                lstBPBK = lstBPBK.Replace("\t", "\n");
                foreach (var item in lstBPBK.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(item.Trim()))
                    {
                        lstBill.Add(item.Trim());
                    }
                }

                string input = JsonConvert.SerializeObject(new CSS_OP_BOOKING_SGEV_WEB { lstMA_BPBK = lstBill, MA_KHACH_HANG = maKH, UPDATE_USER = ClientSession.getCusCode(), lstMA_KHACH_HANG = ClientSession.getLstMaKH() });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("CHUYEN_MA_KHACH_HANG_CHO_BILL_CHUA_CHECKIN", input);
                if (dtoRes.Status == "OK")
                {
                    return Json(Service.OK_V1("Chuyển mã khách hàng thành công"));
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
        public ActionResult KhoiPhucBPBK(string lstBPBK)
        {
            try
            {
                List<string> lstBill = new List<string>();
                lstBPBK = lstBPBK.Replace("\t", "\n");
                foreach (var item in lstBPBK.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(item.Trim()))
                    {
                        lstBill.Add(item.Trim());
                    }
                }

                string input = JsonConvert.SerializeObject(new CSS_OP_BOOKING_SGEV_WEB { lstMA_BPBK = lstBill, INSERT_USER = ClientSession.getCusCode() });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("KHOI_PHUC_BPBK", input);
                if (dtoRes.Status == "OK")
                {
                    return Json(Service.OK_V1(null, "Khôi phục bưu phẩm bưu kiện thành công"));
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
        public ActionResult LayDanhSachThongTinNguoiNhan()
        {
            try
            {
                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { MA_KHACH_HANG = ClientSession.getMaKHCon(), INSERT_USER = ClientSession.getUserName() };
                string inputCa = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LAY_DANH_SACH_THONG_TIN_NGUOI_NHAN", inputCa);
                if (dtoRes.IsError)
                {
                    return Json(Service.ErrorV2(dtoRes.Description));
                }
                List<CSS_OP_BOOKING_SGEV_WEB> lst = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB>>(dtoRes.ResponseData);

                return Json(Service.OK_V1(lst));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        #endregion

        //vinID
        #region
        //[HttpPost]
        //public ActionResult Upload_VinID(CSS_OP_BOOKING_SGEV_WEB model)
        //{
        //    JMessage msg = new JMessage();
        //    try
        //    {
        //        CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
        //        string lag = Request.Cookies["language"].Value;
        //        List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
        //        if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
        //            lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
        //        else
        //            dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);

        //        if (Request != null)
        //        {
        //            HttpPostedFileBase file = Request.Files["UploadedFile"];
        //            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //            {
        //                string fileName = file.FileName;
        //                string fileContentType = file.ContentType;
        //                byte[] fileBytes = new byte[file.ContentLength];
        //                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //                using (var package = new ExcelPackage(file.InputStream))
        //                {
        //                    var currentSheet = package.Workbook.Worksheets;
        //                    var workSheet = currentSheet[model.TT_BO_SUNG];
        //                    if (workSheet == null)
        //                    {
        //                        if (lag == "vi")
        //                        {
        //                            ViewBag.UploadError = "Không tìm thấy tên sheet " + model.TT_BO_SUNG + ". Vui lòng kiểm tra lại!";
        //                        }
        //                        else
        //                        {
        //                            ViewBag.UploadError = "Did not find the " + model.TT_BO_SUNG + " sheet name. Please check again!";
        //                        }

        //                        msg.Error = true;
        //                        msg.Title = ViewBag.UploadError;
        //                        msg.Object = lstDonHangTuSinh;
        //                        return Json(msg);
        //                    }
        //                    var noOfCol = workSheet.Dimension.End.Column;
        //                    var noOfRow = workSheet.Dimension.End.Row;
        //                    if (noOfRow - 1 > 1000)
        //                    {
        //                        if (lag == "vi")
        //                        {
        //                            ViewBag.UploadError = "Chỉ được upload tối đa 1000 bưu phẩm bưu kiện!";
        //                        }
        //                        else
        //                        {
        //                            ViewBag.UploadError = "Only upload up to 1000 parcels!";
        //                        }

        //                        msg.Error = true;
        //                        msg.Title = ViewBag.UploadError;
        //                        msg.Object = lstDonHangTuSinh;
        //                        return Json(msg);
        //                    }
        //                    for (int rowIterator = 5; rowIterator <= noOfRow; rowIterator++)
        //                    {
        //                        if (workSheet.Cells[rowIterator, 1].Value == null)
        //                        {
        //                            continue;
        //                        }
        //                        CSS_OP_BOOKING_SGEV_WEB_EX pod = new CSS_OP_BOOKING_SGEV_WEB_EX();
        //                        if (string.IsNullOrWhiteSpace(user.MA_KHACH_HANG_CON))
        //                        {
        //                            Service.ErrorV2(lag == "vi" ? "Vui lòng chọn 1 mã khách hàng để đặt hàng" : "Please choose a customer code for ordering");
        //                        }
        //                        pod.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
        //                        pod.TEN_KHACH_HANG = user.FULL_NAME;
        //                        pod.THOI_GIAN_GUI = workSheet.Cells[rowIterator, 2].Value == null ? DateTime.MinValue : (DateTime)workSheet.Cells[rowIterator, 2].Value;
        //                        pod.SO_THAM_CHIEU = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString().Trim();
        //                        pod.TEN_NGUOI_GUI = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString().Trim();
        //                        pod.DIA_CHI_NGUOI_GUI = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString().Trim();
        //                        pod.TEN_NGUOI_NHAN = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString().Trim();
        //                        pod.DIA_CHI = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString().Trim();
        //                        if (string.IsNullOrWhiteSpace(pod.DIA_CHI))
        //                        {
        //                            msg.Error = true;
        //                            if (lag == "vi")
        //                            {
        //                                msg.Title = "Địa chỉ người nhận không được bỏ trống" + Environment.NewLine + "Lỗi tại dòng: " + rowIterator;
        //                            }
        //                            if (lag == "en" || lag == "ja")
        //                            {
        //                                msg.Title = "The recipient address must not be blank" + Environment.NewLine + "Error at line: " + rowIterator;
        //                            }
        //                            return Json(msg);
        //                        }
        //                        if (!CommonData.IsValidString(CommonData.RemoveUnicode(pod.DIA_CHI)))
        //                        {
        //                            if (lag == "vi")
        //                            {
        //                                ViewBag.UploadError = "Địa chỉ người nhận phải có ít nhất 5 ký tự là chữ cái" + Environment.NewLine + "Lỗi tại dòng: " + rowIterator;
        //                            }
        //                            if (lag == "en" || lag == "ja")
        //                            {
        //                                ViewBag.UploadError = "The recipient address must have at least 5 letters as letters" + Environment.NewLine + "Error at line: " + rowIterator;
        //                            }
        //                            msg.Error = true;
        //                            msg.Object = lstDonHangTuSinh;
        //                            msg.Title = ViewBag.UploadError;
        //                            return Json(msg);
        //                        }
        //                        if (pod.DIA_CHI.Length > 200)
        //                        {
        //                            msg.Error = true;
        //                            if (lag == "vi")
        //                            {
        //                                msg.Title = "Địa chỉ người nhận chỉ có thể nhập tối đa 200 ký tự" + Environment.NewLine + "Lỗi tại dòng: " + rowIterator;
        //                            }
        //                            if (lag == "en" || lag == "ja")
        //                            {
        //                                msg.Title = "The recipient address can only enter up to 200 characters" + Environment.NewLine + "Error at line: " + rowIterator;
        //                            }
        //                            return Json(msg);
        //                        }

        //                        pod.SO_COD = workSheet.Cells[rowIterator, 8].Value == null ? 0 : decimal.Parse(workSheet.Cells[rowIterator, 8].Value.ToString().Trim());
        //                        pod.NOI_DUNG = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString().Trim();
        //                        pod.TRONG_LUONG = workSheet.Cells[rowIterator, 11].Value == null ? 0 : decimal.Parse(workSheet.Cells[rowIterator, 11].Value.ToString().Trim());

        //                        pod.SO_KIEN = workSheet.Cells[rowIterator, 10].Value == null ? string.Empty : workSheet.Cells[rowIterator, 10].Value.ToString().Trim();
        //                        if (!string.IsNullOrWhiteSpace(pod.SO_KIEN) && pod.SO_KIEN != "0")
        //                        {
        //                            pod.TT_BO_SUNG = "{Số kiện: " + pod.SO_KIEN + "}";
        //                        }

        //                        pod.DICH_VU = "";
        //                        pod.TL_QUY_DOI = workSheet.Cells[rowIterator, 12].Value == null ? 0 : decimal.Parse(workSheet.Cells[rowIterator, 12].Value.ToString().Trim());
        //                        pod.LOAI_DV = workSheet.Cells[rowIterator, 13].Value == null ? string.Empty : workSheet.Cells[rowIterator, 13].Value.ToString().Trim();
        //                        pod.GHI_CHU = workSheet.Cells[rowIterator, 14].Value == null ? string.Empty : workSheet.Cells[rowIterator, 14].Value.ToString().Trim();
        //                        pod.id = lstDonHangTuSinh.Count;

        //                        if (user.USER_NAME.EndsWith("ORDERS") || user.USER_NAME.StartsWith("CS000"))
        //                            pod.INSERT_USER = pod.UPDATE_USER = user.USER_NAME;
        //                        else
        //                            pod.INSERT_USER = pod.UPDATE_USER = user.CUSTOMER_CODE;
        //                        lstDonHangTuSinh.Add(pod);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                msg.Error = true;
        //                msg.Object = lstDonHangTuSinh;
        //                msg.Title = "Không tìm thấy file import excel. Mời kiểm tra lại";
        //                return Json(msg);
        //            }
        //        }
        //        msg.Error = false;
        //        msg.Object = lstDonHangTuSinh;
        //        return Json(msg);
        //    }
        //    catch (Exception e)
        //    {
        //        msg.Error = true;
        //        msg.Title = e.Message;
        //        return Json(msg);
        //    }
        //}
        //public ActionResult MultiInPhieuGiaoHang(string lstMA_BPBK)
        //{
        //    try
        //    {
        //        CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
        //        List<CSS_OP_BOOKING_SGEV_WEB_EX> lstMaster = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
        //        List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO> lstChiTiet = new List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO>();

        //        BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("MULTI_IN_PHIEU_GIAO_HANG", JsonConvert.SerializeObject(new CSS_OP_BOOKING_SGEV_WEB { lstMA_BPBK = lstMA_BPBK.Split(',').ToList() }));
        //        if (dtoRes.Status == "OK")
        //        {
        //            lstMaster = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
        //            lstChiTiet = JsonConvert.DeserializeObject<List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO>>(dtoRes.ResponseData_EX);
        //        }

        //        List<XtraReport> reports = new List<XtraReport>();
        //        foreach (var item in lstMaster)
        //        {
        //            rptPhieuGiaoHang rp = new rptPhieuGiaoHang();
        //            List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO> lstCT = lstChiTiet.Where(x => x.MA_BPBK == item.MA_BPBK).ToList();
        //            lstCT = lstCT.OrderBy(x => x.MA_BPBK).ToList();
        //            rp.SetData(lstCT, item);
        //            reports.Add(rp);
        //        }

        //        XtraReport report = new XtraReport();
        //        report.CreateDocument();
        //        foreach (var item in reports)
        //        {
        //            item.CreateDocument();
        //            report.Pages.AddRange(item.Pages);
        //            report.PrintingSystem.ContinuousPageNumbering = true;
        //        }

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            report.ExportToPdf(ms, new PdfExportOptions());

        //            return File(ms.ToArray(), "application/pdf");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["AddError"] = ex.Message;
        //        return View("DanhSachDonHang");
        //    }
        //}
        //public ActionResult InPhieuGiaoHang(string PO)
        //{
        //    try
        //    {
        //        CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
        //        List<CSS_OP_BOOKING_SGEV_WEB_EX> lstMaster = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
        //        List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO> lstChiTiet = new List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO>();

        //        BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("IN_PHIEU_GIAO_HANG", JsonConvert.SerializeObject(new CSS_OP_BOOKING_SGEV_WEB { PO = PO }));
        //        if (dtoRes.Status == "OK")
        //        {
        //            lstMaster = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
        //            lstChiTiet = JsonConvert.DeserializeObject<List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO>>(dtoRes.ResponseData_EX);
        //        }

        //        List<XtraReport> reports = new List<XtraReport>();
        //        foreach (var item in lstMaster)
        //        {
        //            rptPhieuGiaoHang rp = new rptPhieuGiaoHang();
        //            List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO> lstCT = lstChiTiet.Where(x => x.MA_BPBK == item.MA_BPBK).ToList();
        //            lstCT = lstCT.OrderBy(x => x.MA_BPBK).ToList();
        //            rp.SetData(lstCT, item);
        //            reports.Add(rp);
        //        }

        //        XtraReport report = new XtraReport();
        //        report.CreateDocument();
        //        foreach (var item in reports)
        //        {
        //            item.CreateDocument();
        //            report.Pages.AddRange(item.Pages);
        //            report.PrintingSystem.ContinuousPageNumbering = true;
        //        }

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            report.ExportToPdf(ms, new PdfExportOptions());

        //            return File(ms.ToArray(), "application/pdf");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["AddError"] = ex.Message;
        //        return View("DanhSachDonHang");
        //    }
        //}
        //public ActionResult CheckTime()
        //{
        //    JMessage msg = new JMessage();
        //    try
        //    {
        //        var date = Convert.ToDateTime(Request.Cookies["CheckTime"].Value);
        //        if (DateTime.Now.Subtract(date).TotalMinutes > 10)
        //        {
        //            msg.Error = false;
        //        }
        //        else
        //        {
        //            msg.Error = true;
        //        }
        //        Response.Cookies["CheckTime"].Value = DateTime.Now.ToString();
        //        return Json(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        msg.Object = null;
        //        msg.Title = ex.Message;
        //    }
        //    return Json(msg);
        //}
        #endregion

        //Tra cước online
        #region
        [UserAuthenticationFilter("role_Action")]
        public ActionResult TraCuocOnline()
        {
            DateTime date = DateTime.Now;
            ViewBag.FromDate = CommonData.GetFirsDayOfMonth(date).ToString("dd/MM/yyyy");
            ViewBag.ToDate = CommonData.GetLastDayOfMonth(date).ToString("dd/MM/yyyy");
            return View();
        }
        public ActionResult TraCuocTheoNgay(string FromDate, string ToDate)
        {
            try
            {
                DateTime fromDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", null);
                DateTime toDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", null);
                if (toDate.Date.Subtract(fromDate.Date).TotalDays >= 32)
                {
                    return Json(Service.ErrorV2(Booking.errChiTimKiemTrong1Thang));
                }

                DebitNoteOnlineResponse dto = new DebitNoteOnlineResponse { CustomerCode = ClientSession.getCusCode(), FromDate = FromDate, ToDate = ToDate };
                if (ClientSession.getCusCode().Equals("0132078"))
                    dto.Type = "BOOKING";

                string inputCa = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TRA_CUOC_ONLINE", inputCa);
                if (dtoRes.IsError)
                {
                    return Json(Service.ErrorV2(dtoRes.Description));
                }
                DebitNoteOnlineResponse rp = JsonConvert.DeserializeObject<DebitNoteOnlineResponse>(dtoRes.ResponseData);
                Session["lstTraCuu"] = rp;
                if (rp.Data.Count > 0)
                {
                    return Json(Service.OK_V1(null, "true"));
                }
                return Json(Service.OK_V1(null));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        public ActionResult TraCuoc()
        {
            List<DebitNoteOnlineDTO> lst = new List<DebitNoteOnlineDTO>();
            if (Session["lstTraCuu"] != null)
            {
                DebitNoteOnlineResponse dto = Session["lstTraCuu"] as DebitNoteOnlineResponse;
                lst = dto.Data;
            }
            return PartialView("_TraCuoc", lst);
        }
        #endregion

        // Dự kiến tính giá
        #region        
        [HttpPost]
        public ActionResult TinhGia(CSS_RECEIPT_DAILY_SALEDTO dto)
        {
            try
            {
                List<CSS_RECEIPT_DAILY_SALEDTO> lstEnd = new List<CSS_RECEIPT_DAILY_SALEDTO>();

                double trongluong = Convert.ToDouble(Request.Form["WEIGHT"]);
                if (trongluong == 0)
                {
                    return Json(Service.OK_V1());
                }

                var lst_kich_thuoc = JsonConvert.DeserializeObject<List<KICH_THUOC>>(dto.lst_kich_thuoc);
                if (dto.isDiHuyenxa)
                {
                    dto.DEST_ZONE = dto.DEST_ZONE + "1";
                }
                foreach (var item in dto.lstDichVu.Split(','))
                {
                    dto.TOS = item;

                    double trongluongquydoi = 0;
                    bool isTheoTrongLuongQuyDoi = false;

                    foreach (var kien in lst_kich_thuoc)
                    {
                        if (dto.TOS == "CPN" || dto.TOS == "EXP" || dto.TOS == "PHT")
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 6000 * kien.SO_KIEN;
                        }
                        else if (dto.TOS == "PTK")
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 5000 * kien.SO_KIEN;
                        }
                        else
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 3000 * kien.SO_KIEN;
                        }
                    }

                    if (trongluongquydoi > trongluong)
                    {
                        dto.WEIGHT = trongluongquydoi;
                        isTheoTrongLuongQuyDoi = true;
                    }

                    dto.CUSTOMER_CODE = ClientSession.getMaKHCon();

                    BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("DU_KIEN_TINH_GIA_WEB", JsonConvert.SerializeObject(dto));
                    if (dtoRes.IsError)
                    {
                        return Json(Service.ErrorV2(dtoRes.Description));
                    }

                    CSS_RECEIPT_DAILY_SALEDTO dtoReturn = JsonConvert.DeserializeObject<CSS_RECEIPT_DAILY_SALEDTO>(dtoRes.ResponseData);

                    dtoReturn.AMOUNT_STR = (dtoReturn.AMOUNT + dtoReturn.SERVICE_CHARGE + dtoReturn.FUEL_CHARGE).ToString("N0") + " VNĐ";
                    dtoReturn.VAT_STR = dtoReturn.VAT.ToString("N0") + " VNĐ";
                    dtoReturn.VIETNAMESE_NET_AMT_STR = dtoReturn.VIETNAMESE_NET_AMT.ToString("N0") + " VNĐ";
                    dtoReturn.isTheoTrongLuongQuyDoi = isTheoTrongLuongQuyDoi;
                    dtoReturn.TRONG_LUONG_QUY_DOI = dtoReturn.WEIGHT.ToString("N2") + " kg";
                    var dv = CommonData.lstDichVu().FirstOrDefault(u => u.Value == dtoReturn.TOS.Trim().ToUpper());
                    if (dv != null)
                    {
                        dtoReturn.TOS = dv.Text;
                    }
                    dtoReturn.TRONG_LUONG_QUY_DOI = dtoReturn.WEIGHT.ToString() + " kg";
                    lstEnd.Add(dtoReturn);
                }

                return Json(Service.OK_V1(lstEnd));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        #endregion


        // Quản lý mã phụ 
        [UserAuthenticationFilter("role_Action")]
        public ActionResult QuanLyMaPhu()
        {
            CSS_CRM_NGUOI_DUNGDTO dto = new CSS_CRM_NGUOI_DUNGDTO { PARENT_USER_NAME = ClientSession.getUserName() };
            string inputCa = JsonConvert.SerializeObject(dto);
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_MA_PHU_KH_BOOKING_ONLINE", inputCa);
            if (dtoRes.IsError)
            {
                return Json(Service.ErrorV2(dtoRes.Description));
            }
            List<CSS_CRM_NGUOI_DUNGDTO> lst = JsonConvert.DeserializeObject<List<CSS_CRM_NGUOI_DUNGDTO>>(dtoRes.ResponseData);
            ViewData["lstMaPhu"] = lst;
            return View();
        }

        //Quản lý đơn hàng tạm 
        #region
        public ActionResult QuanLyDonHangTam()
        {
            Session["DSDonHang"] = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            return View();
        }
        public ActionResult _GridViewDonHangTam()
        {
            List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
            if (Session["DSDonHang"] != null)
            {
                lst = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
            }
            return PartialView("_GridViewDonHangTam", lst);
        }
        public ActionResult SearchDonHangTam(string FromDate, string ToDate, string MaKhachHang)
        {
            try
            {
                List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();

                if (string.IsNullOrWhiteSpace(MaKhachHang))
                {
                    return Json(Service.ErrorV2(Booking.WH_moiChonKH));
                }

                DateTime fDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                DateTime tDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                if (tDate.Subtract(fDate).Days > 90)
                {
                    return Json(Service.ErrorV2(Booking.errChiTimKiemTrong3Thang));
                }

                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { INSERT_DATE = fDate, UPDATE_DATE = tDate, INSERT_USER = ClientSession.getUserName(), MA_KHACH_HANG = MaKhachHang };

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_DON_HANG_TAM", inputCa);
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    lst = lst.OrderByDescending(x => x.UPDATE_TIME).ToList();

                    foreach (var item in lst)
                    {
                        CommonData.splitService(item);

                        item.UPDATE_TIME_STRING = item.UPDATE_TIME.ToString("dd/MM/yyyy");
                        item.DIA_CHI += string.IsNullOrWhiteSpace(item.HUYEN_NHAN) ? ", " + item.HUYEN_NHAN : "";
                        item.DIA_CHI += string.IsNullOrWhiteSpace(item.TINH_NHAN) ? ", " + item.TINH_NHAN : "";
                    }

                    Session["DSDonHang"] = lst;
                    return Json(Service.OK());
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
        public ActionResult LuuDonHangTam(CSS_OP_BOOKING_SGEV_WEB_EX model)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = ClientSession.getCustomer();

                List<CSS_OP_BOOKING_SGEV_WEB_EX> lstDonHangTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB_EX>();
                if (dicDonHangTuSinh.ContainsKey(user.USER_NAME))
                    lstDonHangTuSinh = dicDonHangTuSinh[user.USER_NAME];
                else
                    dicDonHangTuSinh.Add(user.USER_NAME, lstDonHangTuSinh);

                if (string.IsNullOrWhiteSpace(model.SDT_NHAN) && lstDonHangTuSinh.Count == 0)
                {
                    return Json(Service.ErrorV2(Booking.errKhongCoDH));
                }

                if (!string.IsNullOrWhiteSpace(model.SDT_NHAN) && lstDonHangTuSinh.Count == 0)
                {
                    if (model.DIA_CHI.Length > 300)
                    {
                        return Json(Service.ErrorV2(Booking.errDiaChiNguoiNhan));
                    }
                    if (string.IsNullOrWhiteSpace(model.SDT_GUI))
                    {
                        return Json(Service.ErrorV2(Booking.errSDTgui));
                    }

                    if (string.IsNullOrWhiteSpace(user.MA_KHACH_HANG_CON))
                    {
                        return Json(Service.ErrorV2(Booking.msgChonChiNhanh));
                    }

                    model.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
                    model.TEN_KHACH_HANG = user.FULL_NAME;

                    model.INSERT_USER = model.UPDATE_USER = user.USER_NAME;

                    lstDonHangTuSinh.Add(model);
                }

                foreach (var item in lstDonHangTuSinh)
                {
                    if (!string.IsNullOrWhiteSpace(item.SO_KIEN) && item.SO_KIEN != "0")
                    {
                        item.TT_BO_SUNG = "{Số kiện:" + item.SO_KIEN + "}";
                    }

                    if (!string.IsNullOrWhiteSpace(item.SO_HOA_DON))
                    {
                        if (item.SO_HOA_DON.Contains("}") || item.SO_HOA_DON.Contains("{"))
                            return Json(Service.ErrorV2(Booking.errSoHDkokitudacbiet));
                        item.TT_BO_SUNG += ",{Số hóa đơn:" + item.SO_HOA_DON + "}";
                    }

                    if (!string.IsNullOrWhiteSpace(item.GT_DON_HANG))
                    {
                        item.TT_BO_SUNG += ",{GTDH:" + item.GT_DON_HANG + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.HOAN_BBBG))
                    {
                        if (item.HOAN_BBBG.Contains("}") || item.HOAN_BBBG.Contains("{"))
                            return Json(Service.ErrorV2(Booking.errBBBGkokitudacbiet));
                        item.TT_BO_SUNG += ",{Hoàn BBBG:" + item.HOAN_BBBG + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.NGAY_HEN_PHAT_SIEU_THI))
                    {
                        item.TT_BO_SUNG += ",{TG_PST:" + item.NGAY_HEN_PHAT_SIEU_THI + "}";
                    }
                    if (!string.IsNullOrWhiteSpace(item.DONG_KIEM))
                    {
                        item.TT_BO_SUNG += ",{Đồng kiểm:" + item.DONG_KIEM + "}";
                    }

                    item.TEN_NGUOI_NHAN = item.TEN_NGUOI_NHAN.Trim();
                    item.DIA_CHI = item.DIA_CHI.Trim();
                }

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(lstDonHangTuSinh);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LUU_DON_HANG_TAM_SAGAWA", inputCa);

                lstDonHangTuSinh.Clear();
                if (dtoRes.Status == "OK")
                {
                    return Json(Service.OK());
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
        public ActionResult LuuDonDatHangTuSinh(CSS_OP_BOOKING_SGEV_WEB model, string ngayyc)
        {
            try
            {
                var user = ClientSession.getCustomer();

                var date = DateTime.ParseExact(ngayyc, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                if (date.Subtract(DateTime.Now.Date).TotalDays < 0)
                {
                    return Json(Service.ErrorV2(Booking.erroNgayYCLonHonNgayHT));
                }

                model.NGAY_KH_DU_KIEN_LAY_HANG = date;

                model.MA_KHACH_HANG = user.MA_KHACH_HANG_CON;
                model.TEN_KHACH_HANG = user.FULL_NAME;
                model.INSERT_USER = model.UPDATE_USER = user.USER_NAME;

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LUU_DON_HANG_TU_SINH", inputCa);
                if (dtoRes.Status == "OK")
                {
                    List<CSS_OP_BOOKING_SGEV_WEB_EX> lstIn = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    Session["LstIn"] = lstIn;

                    if (Session["DSDonHang"] != null)
                    {
                        List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
                        foreach (var item in model.lstid)
                        {
                            lst.Remove(lst.FirstOrDefault(x => x.id == item));
                        }
                    }

                    return Json(Service.OK());
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
        public ActionResult GetItem(int id)
        {
            try
            {
                var user = ClientSession.getCustomer();

                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { MA_KHACH_HANG = user.MA_KHACH_HANG_CON };
                if (!string.IsNullOrWhiteSpace(user.PARENT_USER_NAME) && user.CUSTOMER_CODE == user.MA_KHACH_HANG_CON)
                {
                    dto.INSERT_USER = user.USER_NAME;
                }

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_DIA_CHI", inputCa);
                if (dtoRes.Status == "OK")
                {
                    CSS_OP_BOOKING_SGEV_WEB_EX obj = JsonConvert.DeserializeObject<CSS_OP_BOOKING_SGEV_WEB_EX>(dtoRes.ResponseData);

                    if (Session["DSDonHang"] != null)
                    {
                        List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = Session["DSDonHang"] as List<CSS_OP_BOOKING_SGEV_WEB_EX>;
                        var dtoTemp = lst.FirstOrDefault(x => x.id == id);
                        if (dtoTemp != null)
                        {
                            obj.SDT_GUI = dtoTemp.SDT_GUI;
                            obj.DIA_CHI_NGUOI_GUI = dtoTemp.DIA_CHI_NGUOI_GUI;
                            obj.TEN_NGUOI_GUI = dtoTemp.TEN_NGUOI_GUI;

                            return Json(Service.OK_V1(obj));
                        }
                    }
                }

                return Json(Service.ErrorV2(""));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        public ActionResult XoaDonHangTam(List<int> lstid, string FromDate, string ToDate, string MaKhachHang)
        {
            try
            {
                DateTime fDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);
                DateTime tDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", enUS, DateTimeStyles.None);

                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB { lstid = lstid, INSERT_DATE = fDate, UPDATE_DATE = tDate, INSERT_USER = ClientSession.getUserName(), MA_KHACH_HANG = MaKhachHang };

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("XOA_DANH_SACH_DON_TAM", inputCa);
                if (dtoRes.Status == "OK")
                {
                    List<CSS_OP_BOOKING_SGEV_WEB_EX> lst = JsonConvert.DeserializeObject<List<CSS_OP_BOOKING_SGEV_WEB_EX>>(dtoRes.ResponseData);
                    lst = lst.OrderByDescending(x => x.UPDATE_TIME).ToList();

                    foreach (var item in lst)
                    {
                        CommonData.splitService(item);

                        item.UPDATE_TIME_STRING = item.UPDATE_TIME.ToString("dd/MM/yyyy");
                        item.DIA_CHI += string.IsNullOrWhiteSpace(item.HUYEN_NHAN) ? ", " + item.HUYEN_NHAN : "";
                        item.DIA_CHI += string.IsNullOrWhiteSpace(item.TINH_NHAN) ? ", " + item.TINH_NHAN : "";
                    }

                    Session["DSDonHang"] = lst;
                    return Json(Service.OK());
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

        #endregion

    }
}