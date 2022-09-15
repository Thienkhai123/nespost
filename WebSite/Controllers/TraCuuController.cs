using WebSite.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class TraCuuController : Controller
    {
        public static List<CSS_OP_TRA_CUU> lst_End;
        // GET: Report_LG
        public ActionResult TraCuu()
        {
            ViewBag.FromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (Session["User"] != null)
            {
                GetData("Tất cả", ViewBag.FromDate, ViewBag.ToDate, "");
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Booking");
            }
        }
        public ActionResult Test()
        {

            return View();

        }

        public ActionResult LoadData(string TrangThai, string fromDate, string toDate, string Ma_BPBK = "")
        {
            try
            {
                List<CSS_OP_TRA_CUU> lst = new List<CSS_OP_TRA_CUU>();
                if (Session["User"] == null)
                {
                    return RedirectToAction("Login", "Booking");
                }
                if (lst_End == null || lst_End.Count == 0)
                {
                    lst = GetData(TrangThai, fromDate, toDate, Ma_BPBK);
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
        public ActionResult CheckVadidate()
        {
            lst_End = new List<CSS_OP_TRA_CUU>();
            JMessage msg = new JMessage();
            return Json(msg);
        }
        public List<CSS_OP_TRA_CUU> GetData(string TrangThai, string fromDate, string toDate, string Ma_BPBK = "")
        {
            lst_End = new List<CSS_OP_TRA_CUU>();
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            string MA_KHACH_HANG = userRes.CUSTOMER_CODE;
            List<CSS_OP_TRA_CUU> lst = new List<CSS_OP_TRA_CUU>();
            string inputCa = TrangThai + "," + MA_KHACH_HANG + "," + fromDate + "," + toDate + "," + userRes.CUSTOMER_CODE + "," + Ma_BPBK.Trim();
            BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("LIST_TRA_CUU_BPBK", inputCa);
            if (dtoRes.Status == "OK")
            {
                lst = JsonConvert.DeserializeObject<List<CSS_OP_TRA_CUU>>(dtoRes.ResponseData);
                lst = lst.OrderBy(x => x.NGAY_NHAN).ToList();
                lst_End.AddRange(lst);
                var i = 1;
                foreach (var item in lst)
                {
                    item.STT = i++;
                    if (item.NGAY_NHAN == DateTime.MinValue)
                        item.NGAY_NHAN_STRING = "";
                    else
                        item.NGAY_NHAN_STRING = item.NGAY_NHAN.ToString("dd/MM/yyyy");
                    if (item.NGAY_PHAT == DateTime.MinValue)
                    {
                        item.NGAY_PHAT_STRING = "";
                        item.GIO_PHAT_STRING = "";
                    }
                    else
                    {
                        item.NGAY_PHAT_STRING = item.NGAY_PHAT.ToString("dd/MM/yyyy");
                        item.GIO_PHAT_STRING = item.NGAY_PHAT.ToString("HH:mm");
                    }
                    if (item.GIO_HEN_PHAT.Year == 1900 || item.GIO_HEN_PHAT.Year == 0001)
                    {
                        item.GIO_HEN_PHAT = item.NGAY_NHAN;
                        var x = 0;
                        for (var j = 1; j <= 4; j++)
                        {
                            item.GIO_HEN_PHAT = item.GIO_HEN_PHAT.AddDays(1);
                            if (item.GIO_HEN_PHAT.DayOfWeek.ToString().Equals("Sunday"))
                            {
                                x = x + 1;
                            }
                        }
                        item.GIO_HEN_PHAT = item.GIO_HEN_PHAT.AddDays(x);
                        item.GIO_HEN_PHAT_STRING = "15:00 " + item.GIO_HEN_PHAT.ToString("dd/MM/yyyy");
                    }
                    else
                        item.GIO_HEN_PHAT_STRING = item.GIO_HEN_PHAT.ToString("HH:mm dd/MM/yyyy");
                    item.CUS_CODE = userRes.CUSTOMER_CODE;
                    item.LOAI_USER = userRes.LOAI_TK;
                }
                return lst;
            }
            else
            {
                return null;
            }
        }
        public FileContentResult ExportToExcel(string tuNgay, string denNgay)
        {

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("NGAY_NHAN_STRING", "Ngày nhận");
            dic.Add("MA_BILL", "Số bill");
            dic.Add("XR1", "XR1");
            dic.Add("TINH_PHAT", "Tỉnh phát");
            dic.Add("NGAY_PHAT_STRING", "Ngày phát");
            dic.Add("GIO_PHAT_STRING", "Giờ phát");
            dic.Add("NGUOI_NHAN", "Người nhận");
            dic.Add("TINH_TRANG", "Trạng thái");
            dic.Add("GIO_HEN_PHAT_STRING", "Giờ hẹn phát");
            dic.Add("GHI_CHU", "Ghi chú");
            byte[] filecontent = ExcelExportHelper_TraCuu.ExportExcel(lst_End, "Báo cáo trạng thái bưu phẩm bưu kiện", true, dic);
            tuNgay = DateTime.Now.ToString("dd/MM/yyyyy");
            denNgay = DateTime.Now.ToString("dd/MM/yyyyy");
            tuNgay = tuNgay.Replace("/", "");
            denNgay = denNgay.Replace("/", "");
            string fileName = "Báo cáo trạng thái bưu phẩm bưu kiện " + tuNgay + "-" + denNgay + " .xlsx";
            return File(filecontent, ExcelExportHelper_TraCuu.ExcelContentType, fileName);
        }

        public class CSS_OP_TRA_CUU
        {
            public int STT { get; set; }
            public string MA_BILL { get; set; }
            public string CUS_CODE { get; set; }
            public DateTime NGAY_NHAN { get; set; }
            public DateTime NGAY_PHAT { get; set; }
            public string NGUOI_NHAN { get; set; }
            public string TINH_TRANG { get; set; }
            public string NGAY_NHAN_STRING { get; set; }
            public string NGAY_PHAT_STRING { get; set; }
            public string GIO_PHAT_STRING { get; set; }
            public string TINH_PHAT { get; set; }
            public string XR1 { get; set; }
            public string ANH_LIEN_VANG { get; set; }
            public string ANH_LIEN_VANG_PARSE { get; set; }
            public DateTime GIO_HEN_PHAT { get; set; }
            public string GIO_HEN_PHAT_STRING { get; set; }
            public string GHI_CHU { get; set; }
            public string INSERT_USER { get; set; }
            public string LOAI_USER { get; set; }
        }


        //base64string = ImageToBase64(multiPageImage, ImageFormat.Tiff);
        public ActionResult ImageToBase64(string MA_BILL)
        {
            string fileName = "";
            JMessage msg = new JMessage();
            var obj = lst_End.FirstOrDefault(x => x.MA_BILL == MA_BILL);
            if (obj != null)
            {
                fileName = "http://hawb.sagawa.vn:8899/hawb" + obj.ANH_LIEN_VANG;
            }
            msg.Title = ConvertTiffToJpeg(fileName);
            msg.Error = false;
            return Json(msg);
        }

        public class JMessage
        {
            public string Title { get; set; }
            public Object Object { get; set; }
            public Boolean Error { get; set; }
        }
        public string ConvertTiffToJpeg(string fileName)
        {
            try
            {
                //fileName = fileName.Replace("@", @"/").Replace("#", @"\").Replace("%",".");
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(fileName);
                Bitmap bitmap; bitmap = new Bitmap(stream);

                var path = Server.MapPath("~/PhoTo/");

                if (bitmap != null)
                    bitmap.Save(path + "a.jpg", ImageFormat.Jpeg);
                byte[] imageArray = System.IO.File.ReadAllBytes(path + "a.jpg");

                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                stream.Flush();
                stream.Close();
                client.Dispose();
                return "data:image/jpg;base64," + base64ImageRepresentation;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult GHI_LOG(string MA_BILL, string GIO_HEN_PHAT, string GHI_CHU)
        {
            JMessage msg = new JMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(GHI_CHU))
                {
                    msg.Error = true;
                    msg.Title = "Bạn phải nhập ghi chú!";
                    return Json(msg);
                }
                CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;

                CSS_OP_TRA_CUU dto = new CSS_OP_TRA_CUU { MA_BILL = MA_BILL, GHI_CHU = GHI_CHU, GIO_HEN_PHAT_STRING = GIO_HEN_PHAT, INSERT_USER = userRes.USER_NAME };
                string inputCa = JsonConvert.SerializeObject(dto);
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("GHI_LOG", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.Title = "Lưu thành công";
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }
                lst_End = new List<CSS_OP_TRA_CUU>();
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
    }
}