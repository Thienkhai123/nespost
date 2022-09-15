using Newtonsoft.Json;
using System;
using System.Web.Mvc;
using WebChatHub.Models;

namespace WebChatHub.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() 
        {
            if (Session["User"] == null)
            {
                TempData["Error"] = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.";
            }

            return View();
        }

        public ActionResult GetMessage(string UserName)
        {
            try
            {
                ChatHubBooking chat = new ChatHubBooking();
                string unReadCount = chat.getUnReadMessCount(UserName);

                return Json(Service.OK_V1(unReadCount), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult ChatPage(string Username = "", string Password = "", string FullName = "", string CompanyCode = "")
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(CompanyCode))
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng. Vui lòng liên hệ tới hỗ trợ kỹ thuật.";
                return RedirectToAction("Index");
            }

            Username = CommonData.decrypt(Username);
            Password = CommonData.decrypt(Password);

            string input = JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO { USER_NAME = Username, PASSWORD = Password });
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("CHECK_EXISTS_CUSTOMERS", input);
            if (dtoRes.IsError)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng. Vui lòng liên hệ tới hỗ trợ kỹ thuật.";
                return RedirectToAction("Index");
            }

            CSS_CRM_NGUOI_DUNGDTO user = JsonConvert.DeserializeObject<CSS_CRM_NGUOI_DUNGDTO>(dtoRes.ResponseData);
            user.LOAI_USER = "KHACH";

            Session["User"] = user;

            return RedirectToAction("Index");
        }
    }
}