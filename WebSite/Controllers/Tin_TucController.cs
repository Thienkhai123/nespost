using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;
using PagedList;
using WebSite.Filter;

namespace WebSite.Controllers
{
    public class Tin_TucController : Controller
    {
        // GET: Tin_Tuc

        public ActionResult Index(int? page)
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TIN_TUC" , LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                lstTin = lstTin.OrderByDescending(x=>x.THOI_GIAN).ToList();
            }
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(lstTin.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Tim_Kiem(string SearchNew, int? page)
        {
            var str = SearchNew;
            ViewBag.Search = str;
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
                lstTin = lstTin.Where(x => x.MO_TA.ToLower().Contains(str.ToLower()) || x.TIEU_DE.ToLower().Contains(str.ToLower()) || x.NOI_DUNG.ToLower().Contains(str.ToLower())).ToList();
                lstTin = lstTin.OrderByDescending(x => x.THOI_GIAN).ToList();
            }
            int pageSize = 9;
            int pageNumber = (page ?? 1);
            return View(lstTin.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detail(string id)
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { ID = Int32.Parse(id), LAG = ClientSession.getLanguage() };
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TIN_TUC", JsonConvert.SerializeObject(dto));
            if (dtoRes.Status == "OK")
            {
                lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
            }
            TINTUCDTO dtotin = lstTin.FirstOrDefault(x => x.ID == Convert.ToInt32(id));
            if (dtotin != null)
            {
                dtotin.THOI_GIAN_STRING = dtotin.THOI_GIAN.ToString("dd/MM/yyyy HH:mm tt");
            }
            return View(dtotin);
        }
    }
}