using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class AdminController : Controller
    {
        //login, logout
        #region
        [NoCache]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Username, string Password)
        {
            string lag = Request.Cookies["language"].Value;
            lag = "vi-VN";
            JMessage msg = new JMessage();
            try
            {
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new Account() { USER_NAME = Username.Trim(), Password = CommonData.MD5Hash(Password.Trim()) });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("LOGIN_WEBSITE_SAGAWA", inputCa);
                if (dtoRes.Status == "OK")
                {
                    CSS_CRM_NGUOI_DUNGDTO userRes = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(dtoRes.ResponseData);
                    Session["UserAdmin"] = userRes;
                    if (userRes.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.SALE_PART))
                    {
                        msg.ID = 1;
                    }                    
                    else if(userRes.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.HR_PART))
                    {
                        msg.ID = 2;
                    }
                    else
                    {
                        msg.ID = 10;
                        msg.Title = "Bạn không có quyền đăng nhập.";
                    }
                }
                else
                {
                    msg.ID = 10;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception e)
            {
                msg.ID = 10;
                msg.Title = e.Message;
                ViewBag.Message = e.Message;
            }
            return Json(msg);
        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult ResetPass(string pass, string newpsw)
        {
            JMessage msg = new JMessage();
            try
            {
                if (Session["UserAdmin"] == null)
                {
                    return Json(Service.ErrorV2("Phiên làm việc đã hết hạn. Mời đăng nhập lại"));
                }

                CSS_CRM_NGUOI_DUNGDTO user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
                pass = CommonData.MD5Hash(pass);
                newpsw = CommonData.MD5Hash(newpsw);

                if (user.PASSWORD != pass)
                {
                    return Json(Service.ErrorV2("Mật khẩu hiện tại không đúng. Vui lòng kiểm tra lại."));
                }

                BaseResponseAndroid dtoRes = Connection.CallServiceObject("RESET_PASS_H2", new CSS_CRM_NGUOI_DUNGDTO() { ID = user.ID, PASSWORD = user.PASSWORD, RePass = newpsw });
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    user.PASSWORD = newpsw;
                    Session["UserAdmin"] = user;
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

        #endregion

        //tin tức
        #region 
        // GET: Admin
        [HttpGet]
        public ActionResult TinTuc()
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null && user.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.SALE_PART))
            {
                return View();
                
            }
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult ThemTinTuc(string id)
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;

            if (user != null && user.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.SALE_PART))
            {
                TINTUCDTO dto = new TINTUCDTO();
                if (string.IsNullOrWhiteSpace(id))
                {
                    return View(dto);
                }
                dto.ID = Int32.Parse(id);
                dto.NHOM_TIN = "TIN_TUC";
                List<TINTUCDTO> listTinTuc = new List<TINTUCDTO>();
                
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    listTinTuc = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                }
                return View(listTinTuc[0]);
            }

            return RedirectToAction("Login", "Admin");
        }

        [ValidateInput(false)]
        public ActionResult GridViewPartial()
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TIN_TUC" };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                
                foreach (var item in lstTin)
                {
                    item.MO_TA = string.IsNullOrWhiteSpace(item.MO_TA) ? "" : item.MO_TA.Length < 40 ? item.MO_TA : item.MO_TA.Substring(0, 40) + "...";
                    item.MO_TA_EN = string.IsNullOrWhiteSpace(item.MO_TA_EN) ? "" : item.MO_TA_EN.Length < 40 ? item.MO_TA_EN : item.MO_TA_EN.Substring(0, 40) + "...";
                    item.MO_TA_JA = string.IsNullOrWhiteSpace(item.MO_TA_JA) ? "" : item.MO_TA_JA.Length < 40 ? item.MO_TA_JA : item.MO_TA_JA.Substring(0, 40) + "...";
                    item.THOI_GIAN_STRING = item.THOI_GIAN.ToString("dd/MM/yyyy HH:mm");
                }
                lstTin = lstTin.OrderByDescending(x => x.THOI_GIAN).ToList();
            }

            return PartialView("_GridViewPartial", lstTin);
        }

        //tuyển dụng
        public ActionResult Tuyen_Dung()
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null && user.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.HR_PART))
            {
                return View();
            }
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult ThemTinTuyenDung(string id)
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            
            if (user != null && user.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.HR_PART))
            {
                TINTUCDTO dto = new TINTUCDTO();
                if (string.IsNullOrWhiteSpace(id))
                {
                    return View(dto);
                }

                List<TINTUCDTO> listTinTuc = new List<TINTUCDTO>();
                dto.ID = Int32.Parse(id);
                dto.NHOM_TIN = "TUYEN_DUNG";
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    listTinTuc = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                }
                return View(listTinTuc[0]);
            }


            return RedirectToAction("Login", "Admin");
        }

        public ActionResult GridTuyenDung()
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TUYEN_DUNG" };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);               
                foreach (var item in lstTin)
                {
                    item.MO_TA = string.IsNullOrWhiteSpace(item.MO_TA) ? "" : item.MO_TA.Length < 40 ? item.MO_TA : item.MO_TA.Substring(0, 40) + "...";                    
                    item.THOI_GIAN_STRING = item.THOI_GIAN.ToString("dd/MM/yyyy HH:mm");
                }
                ViewData["SO_HO_SO"] = lstTin.Sum(x=>x.TIN_CHUA_DUYET).ToString();
                lstTin = lstTin.OrderByDescending(x => x.THOI_GIAN).OrderByDescending(x => x.TIN_CHUA_DUYET).ToList();
            }
            return PartialView("_GridTuyenDung", lstTin);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ApplyJob(tbl_Apply model)
        {
            JMessage msg = new JMessage();
            try
            {
                HttpFileCollectionBase tepdinhkem = Request.Files;
                if (tepdinhkem != null)
                {
                    var listTep = tepdinhkem.GetMultiple("FileDinhKem");
                    if (!Directory.Exists(Server.MapPath("~/ckfinder/News/CV")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ckfinder/News/CV"));
                    }
                    foreach (var item in listTep)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ckfinder/News/CV"), fileName);
                        item.SaveAs(path);
                        model.CV = "/ckfinder/News/CV/" + fileName;
                    }
                }
                model.TRANG_THAI = "CHUA_DUYET";
                model.NGAY_APPLY = DateTime.Now;
                string inputCa = JsonConvert.SerializeObject(model);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("APPLY_JOB", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
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

            }
            return Json(msg);
        }

        public ActionResult ShowHoSoTuyenDung(string ID)
        {
            JMessage msg = new JMessage();
            try
            {
                List<tbl_Apply> lstApply = new List<tbl_Apply>();
                tbl_Apply dto = new tbl_Apply { PARENT_ID = Int32.Parse(ID) };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_APPLY_JOB", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    lstApply = JsonConvert.DeserializeObject<List<tbl_Apply>>(dtoRes.ResponseData);
                    foreach (var item in lstApply)
                    {
                        if (string.IsNullOrWhiteSpace(item.TRANG_THAI) || item.TRANG_THAI.Equals("CHUA_DUYET"))
                        {
                            item.TRANG_THAI = "Chưa duyệt";
                        }
                        if (item.TRANG_THAI.Equals("DA_DUYET"))
                        {
                            item.TRANG_THAI = "Đã duyệt";
                        }
                        if (item.TRANG_THAI.Equals("TU_CHOI"))
                        {
                            item.TRANG_THAI = "Từ chối";
                        }
                        item.CV = item.CV.Replace("/ckfinder/News/CV/", "");
                    }
                    lstApply = lstApply.OrderByDescending(x => x.NGAY_APPLY).ToList();
                    Session["gridDetail"] = lstApply;
                    msg.Error = false;
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description ;
                }
            }
            catch(Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult ApplicationPartial()
        {
            List<tbl_Apply> lstApply = new List<tbl_Apply>();
            if (Session["gridDetail"] != null)
            {
                lstApply = Session["gridDetail"] as List<tbl_Apply>;
            }
            return PartialView("_ApplicationPartial", lstApply);
        }
        public ActionResult ApplicationForm()
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null && user.LOAI_TK.Equals(CommonData.ROLE_WEBSITE_ADMIN.HR_PART))
            {
                return View();
                
            }
            return RedirectToAction("Login", "Admin");
        }
        #endregion
      
        //loại dịch vụ
        #region
        public ActionResult Dich_Vu()
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null && user.USER_NAME == "017692")
            {
                
                return View();               
            }
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult ThemDichVu(string id)
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user != null && user.USER_NAME == "017692")
            {
                ViewData["LOAI_DICH_VU"] = LOAI_DICH_VU();

                TINTUCDTO dto = new TINTUCDTO();
                if (string.IsNullOrWhiteSpace(id))
                {
                    return View(dto);
                }

                List<TINTUCDTO> listTinTuc = new List<TINTUCDTO>();

                dto.ID = Int32.Parse(id);
                dto.NHOM_TIN = string.Join(",", LOAI_DICH_VU().Select(x => x.Value).ToList());

                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    listTinTuc = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                    foreach (var item in listTinTuc)
                    {
                        item.TEN_NHOM_TIN = LOAI_DICH_VU().FirstOrDefault(x => x.Value == item.NHOM_TIN) != null ? LOAI_DICH_VU().FirstOrDefault(x => x.Value == item.NHOM_TIN).Text : "";
                    }
                }
                return View(listTinTuc[0]);
            }
            return RedirectToAction("Login", "Admin");         
        }
        public ActionResult QuyDinhDV()
        {
            List<TINTUCDTO> lst = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = string.Join(",", LOAI_DICH_VU().Select(x=>x.Value).ToList()) };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lst = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                foreach(var item in lst)
                {
                    item.THOI_GIAN_STRING = item.THOI_GIAN.ToString("dd/MM/yyyy");
                    item.TEN_NHOM_TIN = LOAI_DICH_VU().FirstOrDefault(x => x.Value == item.NHOM_TIN) != null ? LOAI_DICH_VU().FirstOrDefault(x => x.Value == item.NHOM_TIN).Text : "";
                }
                lst = lst.OrderByDescending(x => x.THOI_GIAN).ToList();
            }
            return PartialView("_QuyDinhDV", lst);
        }       
        public ActionResult GetListDV()
        {
            JMessage msg = new JMessage();
            try
            {
                List<SelectListItem> lstTinh = LOAI_DICH_VU();
              
                
                msg.Object = lstTinh;
                msg.Error = false;
            }
            catch (Exception e)
            {
                msg.Title = e.Message;

            }
            return Json(msg);
        }

        public List<SelectListItem> LOAI_DICH_VU()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Chuyển phát nội địa", Value = "CHUYEN_PHAT_ND" });
            items.Add(new SelectListItem { Text = "Chuyển phát quốc tế", Value = "CHUYEN_PHAT_QUOC_TE" });
            items.Add(new SelectListItem { Text = "Quy định chung chuyển phát", Value = "QD_CHUNG_CHUYEN_PHAT" });
            items.Add(new SelectListItem { Text = "Vận tải hàng không", Value = "HANG_KHONG" });
            items.Add(new SelectListItem { Text = "Vận tải biển", Value = "DUONG_BIEN" });
            items.Add(new SelectListItem { Text = "Vận tải bộ", Value = "DUONG_BO" });
            items.Add(new SelectListItem { Text = "Thủ tục hải quan", Value = "HAI_QUAN" });
            items.Add(new SelectListItem { Text = "Kho bãi", Value = "KHO_BAI" });
            items.Add(new SelectListItem { Text = "Kiểm kim, kiểm phẩm", Value = "KIEM_KIM_KIEM_PHAM" });
            items.Add(new SelectListItem { Text = "Dọn chuyển", Value = "DON_CHUYEN" });
            items.Add(new SelectListItem { Text = "Hàng dự án", Value = "HANG_DU_AN" });
            return items;
        }
        
        //thông tin công ty
        public ActionResult ThongTinCongTy()
        {
            var user = Session["UserAdmin"] as CSS_CRM_NGUOI_DUNGDTO;
            if (user == null && user.LOAI_TK.Contains(CommonData.ROLE_WEBSITE_ADMIN.SALE_PART))
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        #endregion

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult InsertNews(TINTUCDTO model)
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
                HttpFileCollectionBase tepdinhkem = Request.Files;
                if (tepdinhkem != null)
                {
                    if (!Directory.Exists(Server.MapPath("~/ckfinder/News/files")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ckfinder/News/files"));
                    }
                    if (!Directory.Exists(Server.MapPath("~/ckfinder/News/images")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ckfinder/News/images"));
                    }
                    var listTep = tepdinhkem.GetMultiple("FileDinhKem");
                    foreach (var item in listTep)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ckfinder/News/files"), fileName);
                        item.SaveAs(path);
                        model.FILE_DINH_KEM = "/ckfinder/News/files/" + fileName + "," + model.FILE_DINH_KEM;
                    }
                    var img = tepdinhkem.GetMultiple("FileAnh");
                    foreach (var item in img)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ckfinder/News/images"), fileName);
                        item.SaveAs(path);
                        model.FILE_IMG = "/ckfinder/News/images/" + fileName;
                    }
                }
                if (model.NHOM_TIN!="TIN_TUC" && model.NHOM_TIN!="TUYEN_DUNG")
                {
                    model.TAC_GIA = CommonData.RemoveUnicode(model.TIEU_DE,true).ToLower();
                }
                model.THOI_GIAN = DateTime.Now;

                string inputCa = JsonConvert.SerializeObject(model);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIN_TUC_MOI", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;

                }
                else
                {
                    msg.Title = dtoRes.Description;
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
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNew(TINTUCDTO model)
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
                HttpFileCollectionBase tepdinhkem = Request.Files;
                if (tepdinhkem != null)
                {
                    if (!Directory.Exists(Server.MapPath("~/ckfinder/News/images")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ckfinder/News/images"));
                    }
                    if (!Directory.Exists(Server.MapPath("~/ckfinder/News/files")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/ckfinder/News/files"));
                    }
                    var listTep = tepdinhkem.GetMultiple("FileDinhKem");
                    foreach (var item in listTep)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ckfinder/News/files"), fileName);
                        item.SaveAs(path);
                        model.FILE_DINH_KEM = "/ckfinder/News/files/" + fileName + ";" + model.FILE_DINH_KEM;
                    }
                    var img = tepdinhkem.GetMultiple("FileAnh");
                    foreach (var item in img)
                    {
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ckfinder/News/images"), fileName);
                        item.SaveAs(path);
                        model.FILE_IMG = "/ckfinder/News/images/" + fileName;
                    }
                }
                model.THOI_GIAN = DateTime.Now;
                if (model.HAN_NOP.Year==0001)
                {
                    model.HAN_NOP = new DateTime(1900, model.HAN_NOP.Month, model.HAN_NOP.Day);
                }
                string inputCa = JsonConvert.SerializeObject(model);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SUA_TIN_TUC", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                }
                else
                {
                    msg.Title = dtoRes.Description;
                    msg.Error = true;
                    msg.ID = 0;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        [HttpPost]
        public ActionResult Delete(string id)
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
                TINTUCDTO dto = new TINTUCDTO { ID = Int32.Parse(id) };
                string input = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("XOA_TIN_TUC", input);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                }
                else
                {
                    msg.Title = dtoRes.Description;
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

        public ActionResult UpdatePartial(string id, string status)
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
                tbl_Apply dto = new tbl_Apply { ID = Int32.Parse(id), TRANG_THAI = status };              
                string input = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("UPDATE_STATUST_HO_SO", input);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.ID = 1;
                    if (string.IsNullOrWhiteSpace(status) || status.Equals("CHUA_DUYET"))
                    {
                        msg.Title = "Chưa duyệt";
                    }
                    if (status.Equals("DA_DUYET"))
                    {
                        msg.Title = "Đã duyệt";
                    }
                    if (status.Equals("TU_CHOI"))
                    {
                        msg.Title = "Từ chối";
                    }
                }
                else
                {
                    msg.Title = dtoRes.Description;
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
    }
}