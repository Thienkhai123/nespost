using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebSite.Filter;
using WebSite.Language;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class Lien_HeController : Controller
    {
        // GET: Lien_He
        [HttpGet]
        public ActionResult Index(int cs = 0)
        {
            ViewBag.cs = cs;
            ViewData["lstDichVu"] = lstDichVu();
            return View();
        }

        public ActionResult Error(int statusCode, string lag, bool isLienHe)
        {

            if (statusCode == 404)
            {
                var httpCookie = Request.Cookies["language"];
                if (httpCookie != null)
                {
                    var cookie = Response.Cookies["language"];
                    if (cookie != null)
                    {
                        cookie.Value = lag;
                        cookie.Expires = DateTime.Now.AddDays(15);
                    }
                }

                if (isLienHe)
                {
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        public ActionResult Contact(string name, string phone, string email, string message, string subject, string company, string dichvu)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(company))
                {
                    return Json(Service.ErrorV2(Resource.errchonCNdehotro));
                }

                var dv = lstDichVu().FirstOrDefault(x => x.Value == dichvu);
                var tendichvu = dv != null ? dv.Text : "";

                var phienlamviec = Guid.NewGuid().ToString();

                // lưu vào db
                MessageDetail dto = new MessageDetail();
                dto.To_CompanyCode = company;

                dto.GHI_CHU = subject;

                string emailtext = "<a href=\"mailto: " + email + "\" target=\"_blank\">" + email + "</a>";
                dto.Message = "Từ: " + name + " <" + emailtext + "> <br/>";
                dto.Message += "Tiêu đề : " + subject + "<br/>" + "<br/>";
                dto.Message += "Nội dung: " + tendichvu + "<br/>"+ message + "<br/> Phone: " + phone;
                dto.email = email;
                dto.Insert_Time = DateTime.Now;
                dto.Time = DateTime.Now;
                dto.Fr_UserID = dto.Fr_UserCode = phienlamviec;
                dto.Fr_UserName = name;
                dto.PHIEN_LAM_VIEC = phienlamviec;
                dto.Fr_UserRole = "KHACH_LH";
                dto.Loai_DV = dichvu;
                dto.laguage = ClientSession.getLanguage();

                string inputCa = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("INSERT_MESSAGE", inputCa);
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
                return Json(Service.ErrorV2(Resource.errchonCNdehotro));
            }
        }

        public List<SelectListItem> lstDichVu()
        {
            List<SelectListItem> lstDichVu = new List<SelectListItem>
            {
                new SelectListItem { Text = "Dịch vụ chuyển phát nhanh", Value ="CPN"},
                new SelectListItem { Text = "Dịch vụ phát hỏa tốc, yêu cầu", Value ="PHT"},
                new SelectListItem { Text = "Dịch vụ hàng giá trị cao, Express", Value ="GTC"},
                new SelectListItem { Text = "Dịch vụ chuyển phát thường", Value ="CPT"},
                new SelectListItem { Text = "Dịch vuh phát tiết kiệm/ 48h", Value ="PTK"}
            };

            return lstDichVu;
        }
    }
}