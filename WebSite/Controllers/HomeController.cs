using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSite.Filter;
using WebSite.Models;
using static WebSite.Models.CommonData;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        //[NoCache]
       
        public ActionResult Index()
        {
            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TIN_TUC", LAG = ClientSession.getLanguage(), TAKE = 2 };
            //BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_TIN_MOI_NHAT", JsonConvert.SerializeObject(dto));
            //if (dtoRes.Status == "OK")
            //{
            //    lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
            //}
            ViewData["_listZone"] = CommonData.GetZone();
            return View(lstTin);
        }
        public ActionResult sitemaps()
        {
            return View();
        }
        public ActionResult about_using_website()
        {
            return View();
        }
        public ActionResult agreement()
        {
            return View();
        }
        public ActionResult privacy_policy()
        {
            return View();
        }
        public ActionResult sitemap()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult GetNews()
        {

            List<TINTUCDTO> lstTin = new List<TINTUCDTO>();
            TINTUCDTO dto = new TINTUCDTO { NHOM_TIN = "TIN_TUC", LAG = ClientSession.getLanguage(), TAKE=2 };
            //BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("TIM_KIEM_TIN_MOI_NHAT", JsonConvert.SerializeObject(dto));
            //if (dtoRes.Status == "OK")
            //{
            //    lstTin = JsonConvert.DeserializeObject<List<TINTUCDTO>>(dtoRes.ResponseData);
            //}

            return PartialView("_ThongBaoPartial", lstTin);
        }

        //Tra cứu BPBK
        public ActionResult ListTrackAndTrace(string MaBPBK)
        {
            JMessage msg = new JMessage();
            try
            {
                var lag = "";
                //if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                //{
                //    //lag = "";
                //    lag = "vi-VN";
                //}
                //else
                lag = "vi-VN";

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_TRACK_AND_TRACE() { HAWB_NO = MaBPBK.Replace("\t", "").Replace("\n", ","), DESC = lag });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_TRACK_AND_TRACE", inputCa);
                List<CSS_TRACK_AND_TRACE> lst;
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_TRACK_AND_TRACE>>(dtoRes.ResponseData);
                    if (lst.Count == 0)
                    {
                        msg.Error = true;
                        msg.Object = null;
                        msg.Title = dtoRes.Description;
                        return Json(msg);
                    }
                    foreach (var item in lst)
                    {
                        item.DESC = item.DESC.Replace("[SGV]DASH LOGISTICS", "Hub_Das");
                        item.DESC = item.DESC.Replace("[SGV]NASCO", "Hub_Nas");
                        item.DESC = item.DESC.Replace("[SGV]VIETTEL", "Hub_Viet");
                        if (lag == "")
                        {
                            item.DESC = item.DESC.Replace("vào lúc", "at").Replace("DA PHAT", "Deliveried successfully to");
                        }
                        item.INSERT_TIME_STRING = item.INSERT_TIME.ToString("dd/MM/yyyy HH:mm");
                        //item.HAWB_NO = item.HAWB_NO.ToUpper().Trim();
                        item.DICH_VU = CommonData.getDichVuTuMa(item.DICH_VU);
                    }
                    lst = lst.OrderBy(x => x.HAWB_NO).OrderByDescending(x => x.INSERT_TIME).ToList();
                    var dic = lst.GroupBy(x => x.HAWB_NO).ToDictionary(x => x.Key, x => x.ToList());
                    msg.Object = dic;
                    msg.Title = "Get list success!";
                }
                else
                {
                    msg.Error = true;
                    msg.Object = null;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception ex)
            {
                msg.Object = null;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult GetImageFromServer(string idmongo, string UrlImage)
        {
            JMessage msg = new JMessage();
            try
            {
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_TRACK_AND_TRACE() { ID_MONGO = idmongo, URLImage = UrlImage });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("CONVERT_IMAGE_FROM_MONGO", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.Title = dtoRes.ResponseData;
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult GetImageFromMongoDB(string idmongo, string URLImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idmongo))
                {
                    using (var client = new WebClient())
                    {
                        byte[] imagebytes;
                        if (URLImage.StartsWith("http"))
                        {
                            imagebytes = client.DownloadData(URLImage);
                        }
                        else
                        {
                            imagebytes = client.DownloadData(CommonData.URL_TRANG_CHU + URLImage);
                        }
                        using (var stream = new MemoryStream(imagebytes))
                        {
                            string base64string = CommonData.GetBase64FromImage(stream);
                            return Json(Service.OK_V1(null, base64string));
                        }
                    }
                }
                else
                {
                    MongoDatabase db = CommonData.GetMongoDataBase("SGV_IMAGE");
                    MongoCollection<IMAGES> collection = db.GetCollection<IMAGES>("IMAGES");
                    IMAGES file1 = collection.FindOne(Query.EQ("DATA_CODE_H2", idmongo));
                    var file = db.GridFS.FindOne(Query.EQ("_id", file1._id_IMAGE));
                    using (var stream = file.OpenRead())
                    {
                        string base64string = CommonData.GetBase64FromImage(stream);
                        return Json(Service.OK_V1(null, base64string));
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
        
        //Báo cáo sai phạm
        public ActionResult BaoCaoSaiPham(string sdt, string note, string idAnhPOD)
        {
            JMessage msg = new JMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(idAnhPOD))
                {
                    msg.Error = true;
                    msg.Title = "Có lỗi xảy ra. Vui lòng tải lại trang";
                    return Json(msg);
                }
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_LOG_VAN_DEDTO() { GHI_CHU = note, MA_THAM_CHIEU = idAnhPOD, MA_LOAI_NGUYEN_NHAN = sdt, MA_CHUC_NANG = "KHACH_HANG_BAO_CAO_SAI_PHAM" });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("KHACH_HANG_BAO_CAO_SAI_PHAM", inputCa);
                if (dtoRes.Status == "OK")
                {
                    msg.Error = false;
                    msg.Title = dtoRes.ResponseData;
                }
                else
                {
                    msg.Error = true;
                    msg.Title = dtoRes.Description;
                }
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        
        //Multi Language
        public ActionResult ChangeLanguage(string culture, string returnUrl)
        {
            //if (!string.IsNullOrEmpty(culture))
            //{
            //    var httpCookie = Request.Cookies["language"];
            //    if (httpCookie != null)
            //    {
            //        var cookie = Response.Cookies["language"];
            //        if (cookie != null)
            //        {
            //            cookie.Value = culture;
            //            cookie.Expires = DateTime.Now.AddDays(15);
            //        }
            //    }
            //}
            returnUrl= "vi-VN";
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Json("OK");
        }
        
        //Chọn kiểu in
        public ActionResult ChonKieuIn(string Print)
        {
            try
            {
                CSS_CRM_NGUOI_DUNGDTO user = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
                user.LOAI_TK = user.updateLoaiTK("P",Print) ;

                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_CRM_NGUOI_DUNGDTO() { USER_NAME = user.USER_NAME, LOAI_TK = user.LOAI_TK });
                BaseResponseWebSGEV dtoRes = Connection.KetNoiDangNhap("CAP_NHAT_KIEU_IN", inputCa);
                if (dtoRes.Status == "OK")
                {
                    Session["User"] = user;
                }
            }
            catch (Exception ex)
            {

            }
            return Json("OK");
        }

        public ActionResult Bang_Gia(string name)
        {
            ViewData["name"] = name;
            return View();
        }

        [HttpPost]
        public ActionResult TinhGia(CSS_RECEIPT_DAILY_SALEDTO dto)
        {
            try
            {
                List<CSS_RECEIPT_DAILY_SALEDTO> lstEnd = new List<CSS_RECEIPT_DAILY_SALEDTO>();

                double trongluong = Convert.ToDouble(Request.Form["WEIGHT"]);
                if (trongluong == 0)
                {
                    return Json(Service.OK_V1());
                }

                var lst_kich_thuoc = JsonConvert.DeserializeObject<List<KICH_THUOC>>(dto.lst_kich_thuoc);
                if (dto.isDiHuyenxa)
                {
                    dto.DEST_ZONE = dto.DEST_ZONE + "1";
                }
                foreach (var item in dto.lstDichVu.Split(','))
                {
                    dto.TOS = item;

                    double trongluongquydoi = 0;
                    bool isTheoTrongLuongQuyDoi = false;

                    foreach(var kien in lst_kich_thuoc)
                    {
                        if (dto.TOS == "CPN" || dto.TOS == "EXP" || dto.TOS == "PHT")
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 6000 * kien.SO_KIEN;
                        }
                        else if (dto.TOS == "PTK")
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 5000 * kien.SO_KIEN;
                        }
                        else
                        {
                            trongluongquydoi += (kien.CHIEU_CAO * kien.CHIEU_DAI * kien.CHIEU_RONG) / 3000 * kien.SO_KIEN;
                        }
                    }

                    if (trongluongquydoi > trongluong)
                    {
                        dto.WEIGHT = trongluongquydoi;
                        isTheoTrongLuongQuyDoi = true;
                    }

                    dto.CUSTOMER_CODE = ClientSession.getMaKHCon();
                    dto.isTINH_GIA_TRANG_CHU = true;
                    

                    BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("DU_KIEN_TINH_GIA_WEB", JsonConvert.SerializeObject(dto));
                    if (dtoRes.IsError)
                    {
                        continue;
                        return Json(Service.ErrorV2(dtoRes.Description));
                    }

                    CSS_RECEIPT_DAILY_SALEDTO dtoReturn = JsonConvert.DeserializeObject<CSS_RECEIPT_DAILY_SALEDTO>(dtoRes.ResponseData);

                    dtoReturn.AMOUNT_STR = (dtoReturn.AMOUNT + dtoReturn.SERVICE_CHARGE + dtoReturn.FUEL_CHARGE).ToString("N0") + " VNĐ";
                    dtoReturn.VAT_STR = dtoReturn.VAT.ToString("N0") + " VNĐ";
                    dtoReturn.VIETNAMESE_NET_AMT_STR = dtoReturn.VIETNAMESE_NET_AMT.ToString("N0") + " VNĐ";
                    dtoReturn.isTheoTrongLuongQuyDoi = isTheoTrongLuongQuyDoi;
                    dtoReturn.TRONG_LUONG_QUY_DOI = dtoReturn.WEIGHT.ToString("N2") + " kg";
                    var dv = CommonData.lstDichVu().FirstOrDefault(u => u.Value == dtoReturn.TOS.Trim().ToUpper());
                    if (dv != null)
                    {
                        dtoReturn.TOS = dv.Text;
                    }
                    dtoReturn.TRONG_LUONG_QUY_DOI = dtoReturn.WEIGHT.ToString() + " kg";
                    lstEnd.Add(dtoReturn);
                }

                return Json(Service.OK_V1(lstEnd));
            }
            catch (Exception ex)
            {
                return Json(Service.Error(ex));
            }
        }
    }
}