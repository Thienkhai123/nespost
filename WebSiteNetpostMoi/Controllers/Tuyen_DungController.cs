using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Filter;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class Tuyen_DungController : Controller
    {
        
        // GET: Tuyen_Dung
        [HttpGet]
        public ActionResult Index(int?page)
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
           
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TUYEN_DUNG", LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                
                lstTin = lstTin.Where(x => !string.IsNullOrWhiteSpace(x.TIEU_DE)).ToList();
                foreach (var item in lstTin)
                {

                    item.KINH_NGHIEM = item.KINH_NGHIEM.Equals("0") ? "Không yêu cầu kinh nghiệm" : item.KINH_NGHIEM + " năm";
                    item.HAN_NOP_STRING = item.HAN_NOP.ToString("dd/MM/yyyy");

                }

                lstTin = lstTin.OrderByDescending(x => x.THOI_GIAN).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(lstTin.ToPagedList(pageNumber, pageSize));

        }
        [HttpGet]
        public ActionResult Detail(string id)
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TUYEN_DUNG", ID = Int32.Parse(id), LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                foreach (var item in lstTin)
                {
                    item.KINH_NGHIEM = item.KINH_NGHIEM.Equals("0") ? "Không yêu cầu kinh nghiệm" : item.KINH_NGHIEM + " năm";
                    item.HAN_NOP_STRING = item.HAN_NOP.ToString("dd/MM/yyyy");
                }
            }

            return View(lstTin[0]);
        }

        public ActionResult Search(int ? page, string key="", string vitri="", string madiadiem="", string diadiem = "")
        {
            var Key = Request.Form["KEY"]==null?key: Request.Form["KEY"];
            var VI_TRI = Request.Form["VI_TRI"] == null ? vitri : Request.Form["VI_TRI"];
            var DIA_DIEM = Request.Form["cbTinhNhan"] == null ? diadiem : Request.Form["cbTinhNhan"];
            var MA_DIA_DIEM = ComboBoxExtension.GetValue<string>("cbTinhNhan");
            MA_DIA_DIEM = string.IsNullOrWhiteSpace(MA_DIA_DIEM) ? madiadiem : MA_DIA_DIEM;

            ViewBag.KEY = Key==null?"": Key.ToString();
            ViewBag.VI_TRI = VI_TRI == null?"": VI_TRI.ToString();
            ViewBag.DIA_DIEM = DIA_DIEM == null ? "" : DIA_DIEM.ToString();
            ViewBag.MA_DIA_DIEM = MA_DIA_DIEM == null ? "" : MA_DIA_DIEM.ToString();

            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TUYEN_DUNG", LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                if (!string.IsNullOrWhiteSpace(MA_DIA_DIEM))
                {
                    lstTin = lstTin.Where(x => x.MA_DIA_DIEM.Contains(MA_DIA_DIEM)).ToList();
                }

                lstTin = lstTin.Where(x => !string.IsNullOrWhiteSpace(x.TIEU_DE)).ToList();
                if (!string.IsNullOrWhiteSpace(Key))
                {
                    lstTin = lstTin.Where(x => x.TIEU_DE.Contains(Key) || x.NOI_DUNG.Contains(Key)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(VI_TRI))
                {
                    lstTin = lstTin.Where(x => x.VI_TRI.Contains(VI_TRI)).ToList();
                }

                foreach (var item in lstTin)
                {
                    item.KINH_NGHIEM = item.KINH_NGHIEM.Equals("0") ? "Không yêu cầu kinh nghiệm" : item.KINH_NGHIEM + " năm";
                    item.HAN_NOP_STRING = item.HAN_NOP.ToString("dd/MM/yyyy");
                }

                lstTin = lstTin.OrderByDescending(x => x.THOI_GIAN).ToList();
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("Index", lstTin.ToPagedList(pageNumber, pageSize));
        }
    }
}