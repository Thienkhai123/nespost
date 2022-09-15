using WebSite.Models;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class TraCuuDonHangController : Controller
    {
        // GET: TraCuuDonHang

        public static List<CSS_SJC_DON_HANG_COD> lst_End;
        public ActionResult TraCuuDonHang()
        {
            ViewBag.FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
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

                List<CSS_SJC_DON_HANG_COD> lst = new List<CSS_SJC_DON_HANG_COD>();
                if (Session["User"] == null)
                {
                    return RedirectToAction("Login", "Booking");
                }
                if (lst_End == null || lst_End.Count == 0)
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
            lst_End = new List<CSS_SJC_DON_HANG_COD>();
            JMessage msg = new JMessage();

            msg.Error = false;
            return Json(msg);

        }

        public List<CSS_SJC_DON_HANG_COD> GetData(string fromDate, string toDate, string Ma_BPBK = "")
        {
            lst_End = new List<CSS_SJC_DON_HANG_COD>();
            List<CUSTOMER> lstCust = Session["lstCust"] as List<CUSTOMER>;
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            string lstMaKH = string.Join("a", lstCust.Select(x => x.CUSTOMER_CODE).ToList());
            string MA_KHACH_HANG = userRes.CUSTOMER_CODE;
           
            DateTime FrDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
            DateTime ToDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
            ToDate = ToDate.AddDays(1);
            DateTime ngayTemp = new DateTime(1900, 01, 01);
            do
            {
                if (ngayTemp.Year == 1900)
                {
                    ngayTemp = FrDate.AddDays(5);
                }
                else
                {
                    FrDate = ngayTemp;
                    ngayTemp = FrDate.AddDays(5);
                }
                if (ngayTemp > ToDate)
                {
                    ngayTemp = ToDate;
                }
                List<CSS_SJC_DON_HANG_COD> lst = new List<CSS_SJC_DON_HANG_COD>();
                string inputCa = lstMaKH + "," + fromDate + "," + ngayTemp + "," + Ma_BPBK.Trim();
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TRA_CUU_DON_HANG", inputCa);
                if (dtoRes.Status == "OK")
                {
                    
                    lst = JsonConvert.DeserializeObject<List<CSS_SJC_DON_HANG_COD>>(dtoRes.ResponseData);
                    lst_End.AddRange(lst);
                }
                else
                {
                    lst = new List<CSS_SJC_DON_HANG_COD>();
                    lst_End.AddRange(lst);
                }              
            } while (ngayTemp < ToDate);
            lst_End = lst_End.OrderBy(x => x.INSERT_DATE).ToList();
            return lst_End;
        }

        public FileStreamResult ExportToExcel()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("STT", "NO.");
            dic.Add("HAWB_NO", "BILL");
            dic.Add("DELIVERY_TIME_STRING", "Ngày giờ phát");
            dic.Add("NGUOI_NHAN", "CONTACT NAME");
            dic.Add("TITLE", "TITLE");
            dic.Add("COMPANY_NAME", "COMPANY NAME");
            dic.Add("SO_DIEN_THOAI", "PHONE NUMBER");
            dic.Add("ADD1", "ADDRESS");
            dic.Add("NGUOI_NHAN_THUC_TE", "Người nhận thực tế");
            dic.Add("TRONG_LUONG", "Trọng lượng");
            dic.Add("TOS", "Dịch vụ");
            dic.Add("DEST_ZONE", "Nơi đến");
            dic.Add("NGAY_NHAN_STRING", "Ngày gửi");
            dic.Add("GHI_CHU", "Ghi chú");
            dic.Add("TRANG_THAI_PHAT", "Trạng thái phát");
            dic.Add("DON_VI_PHAT", "Đơn vị phát");
            Dictionary<string, List<CSS_SJC_DON_HANG_COD>> dicTenFile = new Dictionary<string, List<CSS_SJC_DON_HANG_COD>>();
            foreach (var v in lst_End)
            {
                if (!dicTenFile.ContainsKey(v.TEN_FILE.Trim()))
                {
                    dicTenFile.Add(v.TEN_FILE.Trim(), new List<CSS_SJC_DON_HANG_COD> { v });
                }
                else
                {
                    dicTenFile[v.TEN_FILE.Trim()].Add(v);
                }
            }
            if (dicTenFile != null)
            {
                var outputStream = new MemoryStream();

                List<byte[]> lstFile = new List<byte[]>();
                using (var zip = new ZipFile())
                {
                    foreach (var key in dicTenFile.Keys)
                    {
                        List<CSS_SJC_DON_HANG_COD> lstTenFile = dicTenFile[key];
                        lstTenFile.OrderBy(x => x.STT_SHEET);
                        Dictionary<string, List<CSS_SJC_DON_HANG_COD>> dicTenSheet = lstTenFile.OrderBy(x => x.STT_SHEET).OrderBy(x => x.STT).GroupBy(u => u.TEN_SHEET).ToDictionary(u => u.Key, x => x.ToList());
                        byte[] file = ExcelExportHelper_TraCuu.ExportExcelWhithMultiSheet(dicTenSheet, key, dic);
                        if (dicTenFile.Count == 1)
                        {
                            Stream stream = new MemoryStream(file);
                            return File(stream, "application/xslx", key);
                        }
                        zip.AddEntry(key, file);
                    }
                    zip.Save(outputStream);
                }
                outputStream.Position = 0;
                return File(outputStream, "application/zip", "Bao_Cao.zip");
                // return File(lstFile[3], ExcelExportHelper_TraCuu.ExcelContentType, "");
            }
            else
            {
                return null;
            }
        }
        public FileStreamResult ExportExcelTong()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("HAWB_NO", "BILL");
            dic.Add("DELIVERY_TIME_STRING", "Ngày giờ phát");
            dic.Add("NGUOI_NHAN", "CONTACT NAME");
            dic.Add("TITLE", "TITLE");
            dic.Add("COMPANY_NAME", "COMPANY NAME");
            dic.Add("SO_DIEN_THOAI", "PHONE NUMBER");
            dic.Add("ADD1", "ADDRESS");
            dic.Add("NGUOI_NHAN_THUC_TE", "Người nhận thực tế");
            dic.Add("TRONG_LUONG", "Trọng lượng");
            dic.Add("TOS", "Dịch vụ");
            dic.Add("DEST_ZONE", "Nơi đến");
            dic.Add("NGAY_NHAN_STRING", "Ngày gửi");
            dic.Add("GHI_CHU", "Ghi chú");
            dic.Add("TRANG_THAI_PHAT", "Trạng thái phát");
            dic.Add("DON_VI_PHAT", "Đơn vị phát");
            foreach (var item in lst_End)
            {
                item.NGAY_NHAN_STRING = item.NGAY_NHAN.ToString("dd/MM/yyyy HH:mm:ss");
            }

            byte[] filecontent = ExcelExportHelper_TraCuu.ExportExcel(lst_End, "Báo cáo ", true, dic);
            Stream stream = new MemoryStream(filecontent);
            return File(stream, ExcelExportHelper_TraCuu.ExcelContentType, "Báo cáo tổng.xlsx");
        }
    }


    public class CSS_SJC_DON_HANG_COD
    {
        public Int32 STT { get; set; }
        public Int32 ID { get; set; }
        public String MA_KHACH_HANG { get; set; }
        public String HAWB_NO { get; set; }
        public Decimal TRONG_LUONG { get; set; }
        public string TOS { get; set; }
        public string DEST_ZONE { get; set; }
        public string DON_VI_PHAT { get; set; }
        public String MA_PHIEU_GIAO_HANG { get; set; }
        public String NGUOI_NHAN { get; set; }
        public String TITLE { get; set; }
        public String COMPANY_NAME { get; set; }
        public String ADD1 { get; set; }
        public String SO_DIEN_THOAI { get; set; }
        public String ADD2 { get; set; }
        public String ADD3 { get; set; }
        public String ADD4 { get; set; }
        public Decimal SO_TIEN_COD { get; set; }
        public String NOI_DUNG { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public String TEN_NGUOI_GUI { get; set; }
        public String DIA_CHI_NGUOI_GUI { get; set; }
        public Decimal SO_KIEN { get; set; }
        public String SO_DT_NGUOI_GUI { get; set; }
        public String DON_VI_UPLOAD { get; set; }
        public String MA_TINH { get; set; }
        public String MA_HUYEN { get; set; }
        public String MA_XA { get; set; }
        public String SO_BD_KIEN_CON { get; set; }
        public String TEN_FILE { get; set; }
        public String TEN_SHEET { get; set; }
        public Int32 STT_SHEET { get; set; }
        public string INSERT_DATE_STRING { get; set; }
        public string FILE_PATH { get; set; }
        public DateTime NGAY_NHAN { get; set; }
        public string NGAY_NHAN_STRING { get; set; }
        public string DELIVERY_TIME_STRING { get; set; }
        public string NGUOI_NHAN_THUC_TE { get; set; }
        public String GHI_CHU { get; set; }
        public string TRANG_THAI_PHAT { get; set; }
    }

}