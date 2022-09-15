using WebSite.Models;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{

    public class DuLieuChamCongController : Controller
    {
        // GET: BieuMau
        static string _FromDate;
        static string _ToDate;
        public ActionResult DuLieuChamCong(int page = 1)
        {
            ViewBag.FromDate = _FromDate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            ViewBag.ToDate = _ToDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (Session["UserID"] != null)
            {
                GetData(ViewBag.FromDate, ViewBag.ToDate, "", "All");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public static List<CSS_DU_LIEU_CHAM_CONGDTO> lst_End;
        public static List<CSS_DU_LIEU_CHAM_CONGDTO> lst_HaveData;
        public static List<CSS_DU_LIEU_CHAM_CONGDTO> lst_NoData;
        public ActionResult LoadData(string fromDate, string toDate, string MaNhanVien, string MaCongTy)
        {
            try
            {
                List<CSS_DU_LIEU_CHAM_CONGDTO> lst = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
                //if (Session["MA_KHACH_HANG"].ToString() == "")
                //{
                //    return RedirectToAction("Login", "Home");
                //}
                if (lst_End == null || lst_End.Count == 0)
                {
                    lst = GetData(fromDate, toDate, MaNhanVien, MaCongTy);
                }
                else
                {
                    lst = lst_HaveData;
                }

                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                recordsTotal = lst.Count();
                //Paging     
                var data = lst.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult CheckVadidate(string fromDate, string toDate, string Ma_NV)
        {
            lst_End = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            lst_HaveData = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            lst_NoData = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            JMessage msg = new JMessage();

            msg.Error = false;
            return Json(msg);

        }

        public List<CSS_DU_LIEU_CHAM_CONGDTO> GetData(string fromDate, string toDate, string manhanvien, string macongty)
        {
            lst_End = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            lst_HaveData = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            lst_NoData = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            _FromDate = fromDate;
            _ToDate = toDate;
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            List<CSS_DU_LIEU_CHAM_CONGDTO> lst = new List<CSS_DU_LIEU_CHAM_CONGDTO>();
            string inputCa = fromDate + "," + toDate + "," + manhanvien + "," + macongty;
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("DU_LIEU_CHAM_CONG", inputCa);
            if (dtoRes.Status == "OK")
            {
                lst = JsonConvert.DeserializeObject<List<CSS_DU_LIEU_CHAM_CONGDTO>>(dtoRes.ResponseData);
                lst_End.AddRange(lst);
                var i = 1;

                List<CSS_DU_LIEU_CHAM_CONGDTO> lstHaveData = new List<CSS_DU_LIEU_CHAM_CONGDTO>();

                foreach (var item in lst)
                {
                    item.STT = i++;
                    if (item.TIME == DateTime.MinValue)
                    {
                        item.TIME_STRING = "";
                        item.TIME_CONVERT = "";
                    }
                    else if (item.TIME.ToString("dd/MM/yyyy").Equals("01/01/1900"))
                    {
                        item.TIME_STRING = "";
                        item.TIME_CONVERT = "";
                    }
                    else
                    {
                        item.TIME_STRING = item.TIME.ToString("dd/MM/yyyy hh:mm:ss tt");
                        item.TIME_CONVERT = item.TIME.ToString("M/d/yyyy h:mm:ss tt");
                    }
                    if (item.COMPANY.Equals("MNA"))
                    {
                        item.COMPANY = "Miền Nam";
                    }
                    else if (item.COMPANY.Equals("MBA"))
                    {
                        item.COMPANY = "Miền Bắc";
                    }
                    else
                    {
                        item.COMPANY = "Miền Trung";
                    }

                    if (!string.IsNullOrWhiteSpace(item.NAME))
                    {
                        lstHaveData.Add(item);
                    }
                    else
                    {
                        lst_NoData.Add(item);
                    }
                }
                lst_HaveData.AddRange(lstHaveData);
                return lstHaveData;
            }
            else
            {
                return null;
            }
        }

        public FileContentResult ExportToExcel(string tuNgay, string denNgay)
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("DEPARTMENT", "Department");
            dic.Add("NAME", "Name");
            dic.Add("NO", "No");
            dic.Add("TIME_CONVERT", "Date/Time");
            dic.Add("IP", "IP machine");
            dic.Add("MACHINE_PLACE", "Machine place");
            dic.Add("COMPANY", "Company place");

            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            dic1.Add("DEPARTMENT", "Department");
            dic1.Add("IP", "IP machine");
            dic1.Add("MACHINE_PLACE", "Machine place");
            dic1.Add("COMPANY", "Company place");

            byte[] filecontent = ExcelExportHelper_TraCuu.ExportExcel_DuLieuChamCong(lst_HaveData, lst_NoData, "", false, dic, dic1);

            tuNgay = _FromDate;
            denNgay = _ToDate;
            tuNgay = tuNgay.Replace("/", "");
            denNgay = denNgay.Replace("/", "");
            string fileName = "Dữ liệu chấm công " + tuNgay + "-" + denNgay + " .xlsx";
            return File(filecontent, ExcelExportHelper_TraCuu.ExcelContentType, fileName);
        }
    }
}