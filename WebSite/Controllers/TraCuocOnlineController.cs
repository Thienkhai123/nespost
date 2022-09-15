using WebSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class TraCuocOnlineController : Controller
    {
        // GET: TraCuocOnline
        public static List<CSS_OP_TRA_CUU_CUOC> lst_End;

        public ActionResult TraCuocOnline()
        {
            ViewBag.FromDate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (Session["User"] != null)
            {
                GetData(ViewBag.FromDate, ViewBag.ToDate, "");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Booking");
            }
        }

        public ActionResult LoadData(string fromDate, string toDate, string Ma_BPBK = "")
        {
            try
            {
                
                List<CSS_OP_TRA_CUU_CUOC> lst = new List<CSS_OP_TRA_CUU_CUOC>();
                if (Session["User"] == null)
                {
                    return RedirectToAction("Login", "Booking");
                }
                if (lst_End == null|| lst_End.Count==0)
                {
                    lst = GetData(fromDate, toDate, Ma_BPBK);
                }
                else
                {
                    lst = lst_End;
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

        public ActionResult CheckVadidate(string fromDate, string toDate)
        {
            lst_End = new List<CSS_OP_TRA_CUU_CUOC>();
            JMessage msg = new JMessage();
            string[] formats = { "dd/MM/yyyy", "dd/M/yyyy" };
            fromDate = fromDate.Trim();
            DateTime fromdate;
            DateTime.TryParseExact(fromDate, formats, new CultureInfo("en-US"), DateTimeStyles.None, out fromdate);
            toDate = toDate.Trim();
            DateTime todate;
            DateTime.TryParseExact(toDate, formats, new CultureInfo("en-US"), DateTimeStyles.None, out todate);
            if (fromdate.Year < 2019 || todate.Year <2019)
            {
                msg.Error = true;
                msg.Title = "Bạn chỉ được xem từ tháng 01/2019!";
                return Json(msg);
            }
           
            if (fromDate == null || fromDate == "")
            {
                msg.Error = true;
                msg.Title = "Bạn không được bỏ trống Ngày nhận từ!";
                return Json(msg);
            }
            else if (toDate == null || toDate == "")
            {
                msg.Error = true;
                msg.Title = "Bạn không được bỏ trống Đến Ngày!";
                return Json(msg);
            }
            else if(todate.Subtract(fromdate).TotalDays > 31)
            {
                msg.Error = true;
                msg.Title = "Bạn chỉ được tìm kiếm trong khoảng 31 ngày!";
                return Json(msg);
            }
            else
            {
                msg.Error = false;
                
                return Json(msg);
            }
        }
        public List<CSS_OP_TRA_CUU_CUOC> GetData(string fromDate, string toDate, string Ma_BPBK = "")
        {
            lst_End = new List<CSS_OP_TRA_CUU_CUOC>();
            List<CUSTOMER> lstCust = Session["lstCust"] as List<CUSTOMER>;
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            string lstMaKH = string.Join("a", lstCust.Select(x => x.CUSTOMER_CODE).ToList());
            string MA_KHACH_HANG = userRes.CUSTOMER_CODE;
            List<CSS_OP_TRA_CUU_CUOC> lst = new List<CSS_OP_TRA_CUU_CUOC>();
            string inputCa = lstMaKH + "," + fromDate + "," + toDate + "," + Ma_BPBK.Trim();
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TRA_CUU_CUOC_ONLINE", inputCa);
            if (dtoRes.Status == "OK")
            {
                lst = JsonConvert.DeserializeObject<List<CSS_OP_TRA_CUU_CUOC>>(dtoRes.ResponseData);
                lst = lst.OrderBy(x=> DateTime.ParseExact(x.NGAY_NHAN, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture)).ToList();
                lst_End.AddRange(lst);
                var i = 1;
                foreach (var item in lst)
                {
                    item.STT = i;
                    item.NGAY_NHAN = DateTime.ParseExact(item.NGAY_NHAN, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    item.NGAY_CHECK_IN_STRING = item.NGAY_CHECK_IN.ToString("dd/MM/yyyy HH:mm");
                    
                    i++;
                }
                return lst;
            }
            else
                return null;

        }
        public FileContentResult ExportToExcel(string tuNgay, string denNgay)
        {
           

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("STT", "STT");
           
            dic.Add("MA_BILL", "Số bill");
            
            dic.Add("NGAY_CHECK_IN_STRING", "Ngày phát");
            dic.Add("TINH_NHAN", "Từ tỉnh");
            dic.Add("DICH_DEN", "Đến tỉnh");
            dic.Add("TRONG_LUONG", "Trọng lượng");
            dic.Add("CUOC_CHINH", "Cước chính");
            dic.Add("PP_XANG_DAU", "Xăng dầu");
            dic.Add("PP_PHAT_TAN_TAY", "Phát tận tay");
            dic.Add("PP_PHAT_TRONG_NGAY", "Phát trong ngày");
            dic.Add("PP_PHAT_VUNG_SAU_XA", "Phát vùng sâu, vùng xa");
            dic.Add("PP_DONG_THUNG", "Đóng thùng");
            dic.Add("PP_BAO_PHAT", "Báo phát");
            dic.Add("PP_DONG_KIEM", "Đồng kiểm");
            dic.Add("PP_KHAC", "Khác");
            dic.Add("PP_CHUYEN_HOAN", "Chuyển hoàn");            
            dic.Add("TONG_TIEN", "Tổng tiền");
            dic.Add("NGUOI_NHAN", "Người nhận");
            dic.Add("NGAY_NHAN", "Ngày nhận");
            byte[] filecontent = ExcelExportHelper_TraCuu.ExportExcel_TraCuoc(lst_End, "Bảng tra cứu cước ", true, dic);
            tuNgay = DateTime.Now.ToString("dd/MM/yyyyy");
            denNgay = DateTime.Now.ToString("dd/MM/yyyyy");
            tuNgay = tuNgay.Replace("/", "");
            denNgay = denNgay.Replace("/", "");
            string fileName = "Bảng tra cứu cước " + tuNgay + "-" + denNgay + " .xlsx";
            return File(filecontent, ExcelExportHelper_TraCuu.ExcelContentType, fileName);
        }
    }

    public class CSS_OP_TRA_CUU_CUOC
    {
        public int STT { get; set; }
        public string MA_BILL { get; set; }
        public string NGAY_NHAN { get; set; }
       
        public string TINH_NHAN { get; set; }        
        public string DICH_DEN { get; set; }
       
        public float TRONG_LUONG { get; set; }
        public float CUOC_CHINH { get; set; }
        
        public float PP_XANG_DAU { get; set; }
        public float PP_PHAT_TAN_TAY { get; set; }
        public float PP_PHAT_TRONG_NGAY { get; set; }
        public float PP_PHAT_VUNG_SAU_XA { get; set; }
        public float PP_DONG_THUNG { get; set; }
        public float PP_BAO_PHAT { get; set; }
        public float PP_DONG_KIEM { get; set; }
        public float PP_KHAC { get; set; }
        public float PP_CHUYEN_HOAN { get; set; }
        public double VAT { get; set; }       
       
        public float TONG_TIEN { get; set; }
        public string NGAY_CHECK_IN_STRING { get; set; }
        public string NGUOI_NHAN { get; set; }
        public DateTime NGAY_CHECK_IN { get; set; }
    }

}