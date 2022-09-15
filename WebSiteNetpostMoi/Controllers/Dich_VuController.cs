using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Filter;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class Dich_VuController : Controller
    {
        // GET: Dich_Vu
        public ActionResult Index(string id)
        {
            return View("chuyen_phat_nhanh");
        }
        public ActionResult chuyen_phat_nhanh(bool partialview = false)
        {
            if (partialview)
            {
                return PartialView();
            }

            return View();
        }

        public ActionResult phat_hoa_toc(bool partialview=false)
        {
            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        
        public ActionResult GetQuyTrinhDuongBien()
        {
            try
            {
                List<TINTUCDTO> lst = new List<TINTUCDTO>();
                TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "DUONG_BIEN", LAG = ClientSession.getLanguage() };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                    return Json(Service.OK_V1(lst));
                }

                return Json(Service.ErrorV2(dtoRes.Description));
            }
            catch(Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        
        public ActionResult sgx(bool partialview = false)
        {
            List<TINTUCDTO> lst = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "CHUYEN_PHAT_QUOC_TE,CHUYEN_PHAT_ND,QD_CHUNG_CHUYEN_PHAT", LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("GET_LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lst = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
            }

            if (partialview)
            {
                return PartialView(lst);
            }

            return View(lst);
        }

        public ActionResult GetNoiDungDichVu(string id)
        {
            try
            {
                List<TINTUCDTO> lst = new List<TINTUCDTO>();
                TINTUCDTO dto = new TINTUCDTO { LAG = ClientSession.getLanguage(), ID = int.Parse(id) };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                    return Json(Service.OK_V1(lst));
                }

                return Json(Service.ErrorV2(dtoRes.Description));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }

        public ActionResult DownLoadFile(string id)
        {
            string filename = "";
            string name = "";
            if (id == "baogiachuyenphatnoidia")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "SERVICE FEE FOR DOMESTIC COURIER SERVICE.pdf";                   
                }
                else
                {
                    name =  "BẢNG GIÁ DỊCH VỤ CHUYỂN PHÁT NỘI ĐỊA.pdf";
                }

            }
            if (id == "baogiachuyenphatquocte")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name =  "SERVICE FEE FOR INTERNATIONAL COURIER SERVICE.pdf";
                }
                else
                {
                    name = "BẢNG GIÁ DỊCH VỤ CHUYỂN PHÁT QUỐC TẾ.pdf";
                }
            }
            if (id == "quytrinhxulybuuphambuukien")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "THE PROCESS ON DOMESTIC & INTERNATIONAL DELIVERY PARCEL HANDLING.pdf";

                }
                else
                {
                    name = "Quy trình xử lý bưu phẩm, bưu kiện (VN).pdf";
                }
            }
            if (id == "quytrinhchuyenphatnoidia")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "DOMESTIC COURIER PROCESS.pdf";

                }
                else
                {
                    name = "QUY TRÌNH CHUYỂN PHÁT NỘI ĐỊA-TV-1.pdf";
                }
            }
            if (id == "quytrinhchuyenphatquocte")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "INTERNATONAL COURIER PROCESS.pdf";

                }
                else
                {
                    name = "QUY TRÌNH CHUYỂN PHÁT QUỐC TẾ-TV.pdf";
                }
            }
            if (id == "quydinhhanghoachuyenphatnoidia")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "RULES FOR CARGO OF DOMESTIC COURIER SERVICE.pdf";

                }
                else
                {
                    name = "QUY ĐỊNH ĐỐI VỚI HÀNG HÓA CHUYỂN PHÁT NỘI ĐỊA- TV.pdf";

                }
            }
            if (id == "quydinhhanghoachuyenphatquocte")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "Rules for cargo international courier service.pdf";

                }
                else
                {
                    name = "QUY ĐỊNH ĐỐI VỚI HÀNG HÓA CHUYỂN PHÁT QUỐC TẾ-TV.pdf";

                }
            }
            if (id == "tieuchuanchatluongdichvu")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "Quality postal service standard.pdf";

                }
                else
                {
                    name =  "QUY ĐỊNH TIÊU CHUẨN CHẤT LƯỢNG DỊCH VỤ.pdf";

                }
            }
            if (id == "hoadonchungtuguikemhanghoa")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "REQUIRED DOCUMENT TO BE ENCLOSED WITH GOODS FOR TRANSPORTATION.pdf";

                }
                else
                {
                    name = "HÓA ĐƠN CHỨNG TỪ GỬI KÈM HÀNG HÓA VẬN CHUYỂN- TV.pdf";
                }
            }
            if (id == "dichvugiatrigiatangcuahanghoa")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "VALUE ADDED SERVICES OF DOMESTIC POSTAL.pdf";

                }
                else
                {
                    name = "DỊCH VỤ GIÁ TRỊ GIA TĂNG CỦA DỊCH VỤ BƯU CHÍNH NỘI ĐỊA.pdf";
                }
            }
            if (id == "quydinhdonggoihanghoa")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "PACKING REGULATION OF DOCUMENTS AND GOODS.pdf";

                }
                else
                {
                    name = "QUY ĐỊNH ĐỐI VỚI VIỆC ĐÓNG GÓI HÀNG HÓA-TV.pdf";
                }
            }
            if (id == "xulybuuphamvothuanhan")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "HANDLING PARCEL WITHOUT RECEIVER.pdf";

                }
                else
                {
                    name = "XỬ LÝ BƯU PHẨM VÔ THỪA NHẬN- TV.pdf";
                }
            }
            if (id == "trachnhiemboithuonggiaiquyetkhieunai")
            {
                if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                {
                    name = "RESPONSIBILITY FOR COMPENSATION OF DAMAGES AND TIME TO RESOLVE COMPLAINTS.pdf";

                }
                else
                {
                    name = "QUY ĐỊNH TRÁCH NHIỆM BỒI THƯỜNG THIỆT HẠI VÀ THỜI GIAN GIẢI QUYẾT KHIẾU NẠI- TV.pdf";
                }
            }
            filename = Server.MapPath("~/ckfinder/News/files/") + name;
            return File(filename, "application/pdf", name);
        }
        public ActionResult gia_tri_cao(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        public ActionResult trucking(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        public ActionResult tiet_kiem(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        public ActionResult gia_tang(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        public ActionResult removal(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
        public ActionResult project_cargo(bool partialview = false)
        {

            if (partialview)
            {
                return PartialView();
            }

            return View();
        }
    }
}