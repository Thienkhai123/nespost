using DevExpress.Web.Mvc;
using Newtonsoft.Json;
using WebSite.Models;
using WebSite.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Language;
using WebSite.Filter;
using WebSite.Excel;
using System.IO;
using DevExpress.XtraPrinting;

namespace WebSite.Controllers
{
    public class WarehouseController : Controller
    {
        // GET: QuanLySanPham
        public ActionResult Index()
        {
            return View();
        }

        public static readonly string MA_NHOM_KHO_VPP = "VPP";
        public static readonly string MA_NHOM_KHO_VINDS = "VINDS";
        public static readonly string MA_NHOM_KHO_VINECO = "VINECO";
        public static readonly string MA_NHOM_KHO_VINMART = "VINMART";
        public static readonly string MA_NHOM_KHO_VINAPHONE = "VINAPHONE";
        public static readonly string MA_NHOM_KHO_VINATEXMART = "VINATEXMART";
        public static readonly string MA_NHOM_KHO_VCMV2 = "VCM_V2";
        public static readonly string MA_NHOM_KHO_FUJI = "FUJI";

        public static readonly String WARE_HOUSE_NCC = "NCC";
        public static readonly String WARE_HOUSE_KH = "KH";
        public static readonly String WARE_HOUSE_OWNER = "OWNER";
        public static readonly String WARE_HOUSE_XUAT_HUY = "XUAT_HUY";
        public List<CSS_DIC_GENERAL_DICTIONARY> lstTrangThai = new List<CSS_DIC_GENERAL_DICTIONARY>()
            {
                 new CSS_DIC_GENERAL_DICTIONARY() { DICTIONARY_CODE = "ACTIVE", DICTIONARY_NAME = Booking.WH_hoatDong },
                 new CSS_DIC_GENERAL_DICTIONARY() { DICTIONARY_CODE = "CLOSED", DICTIONARY_NAME = Booking.WH_dongCua },
                 new CSS_DIC_GENERAL_DICTIONARY() { DICTIONARY_CODE = "SUSPEND", DICTIONARY_NAME = Booking.WH_tamNgung }
            };
        public ActionResult QuanLyKho()
        {
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            if (string.IsNullOrWhiteSpace(userRes.MA_WARE_HOUSE_OWNER))
            {
                if (_listZone == null)
                {
                    _listZone = CommonData.GetZone();
                }
                if (_listDistrict == null)
                {
                    _listDistrict = CommonData.GetDistrict();
                }
                if (_listCommune == null)
                {
                    _listCommune = CommonData.GetCommune();
                }
                ViewData["TrangThai"] = lstTrangThai;
                return View();
            }
            else
            {
                return RedirectToAction("QuanLyDanhSachKhohang");
            }
        }
        public ActionResult GetAutonumKhoHangMoi()
        {
            JMessage mess = new JMessage();
            try
            {
                isTaoMaKhoKhachHang = false;
                WH_V3_WARE_HOUSEDTO dto = new WH_V3_WARE_HOUSEDTO();
                dto.MA_WARE_HOUSE = CommonData.GetAutoNum("WHVEC", 1)[0];
                List<WH_V3_WARE_HOUSEDTO> lst = new List<WH_V3_WARE_HOUSEDTO>() { dto };
                mess.Error = false;
                mess.Object = lst;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult QuanLyDanhMucNhomSP()
        {
            ViewData["NhomKhoHang"] = new List<CSS_DIC_GENERAL_DICTIONARY>() { new CSS_DIC_GENERAL_DICTIONARY() { DICTIONARY_CODE = ClientSession.getCusCode(), DICTIONARY_NAME = ClientSession.getCusName() } }; ;
            return View();
        }

        public ActionResult grvDanhMucNhomSP()
        {
            var model = Session["NhomSP"] as List<WH_V3_NHOM_SAN_PHAMDTO>;
            if (model == null)
                model = new List<WH_V3_NHOM_SAN_PHAMDTO>();
            return PartialView("grvDanhMucNhomSP", model);
        }

        public ActionResult TimKiemNhomSP(string maNhomKho)
        {
            Message mess = new Message();
            WH_V3_NHOM_SAN_PHAMDTO dto = new WH_V3_NHOM_SAN_PHAMDTO();
            try
            {
                if (string.IsNullOrWhiteSpace(maNhomKho))
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_moiChonNhomKhoHang;
                }
                else
                {
                    mess.isError = false;
                    dto.MA_NHOM_KHO = maNhomKho.Trim();
                    var rp = Connection.CallService("WH_WEB_TIM_KIEM_NHOM_SP", new List<WH_V3_NHOM_SAN_PHAMDTO> { dto });
                    if (rp.IsError)
                    {
                        throw new Exception(rp.Description);
                    }
                    List<WH_V3_NHOM_SAN_PHAMDTO> lstNhomSP = JsonConvert.DeserializeObject<List<WH_V3_NHOM_SAN_PHAMDTO>>(rp.ResponseData);
                    foreach (var item in lstNhomSP)
                    {
                        if (item.TRANG_THAI.Equals("ACTIVE"))
                        {
                            item.TRANG_THAI = "Hoạt động";
                        }
                    }
                    Session["NhomSP"] = lstNhomSP;
                }
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }

        public ActionResult GetAutonum()
        {
            JMessage mess = new JMessage();
            try
            {
                WH_V3_NHOM_SAN_PHAMDTO dto = new WH_V3_NHOM_SAN_PHAMDTO();
                dto.MA_NHOM_SAN_PHAM = CommonData.GetAutoNum("WHV3NSP", 1)[0];
                List<WH_V3_NHOM_SAN_PHAMDTO> lst = new List<WH_V3_NHOM_SAN_PHAMDTO>() { dto };
                mess.Error = false;
                mess.Object = lst;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult AddNhomSP(string maNhomKho, string maNhomSP, string tenNhomSP)
        {
            Message mess = new Message();
            try
            {
                WH_V3_NHOM_SAN_PHAMDTO dto = new WH_V3_NHOM_SAN_PHAMDTO();
                dto.MA_NHOM_KHO = maNhomKho;
                dto.MA_NHOM_SAN_PHAM = maNhomSP;
                dto.TEN_NHOM_SAN_PHAM = tenNhomSP;
                dto.INSERT_USER = ClientSession.getCusCode();
                dto.UPDATE_USER = ClientSession.getCusCode();
                var rp = Connection.CallService("WH_WEB_LUU_MA_NHOM_SAN_PHAM", new List<WH_V3_NHOM_SAN_PHAMDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                mess.isSuccess = true;
                mess.Mess = Booking.WH_themThanhCong;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult QuanLyDanhMucSanPham()
        {
            try
            {
                var rp = Connection.CallService("WH_WEB_TIM_KIEM_DVT", new List<WH_V3_UNIT_TYPEDTO>());
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                List<WH_V3_UNIT_TYPEDTO> lstDVT = JsonConvert.DeserializeObject<List<WH_V3_UNIT_TYPEDTO>>(rp.ResponseData);
                var rpNhomSP = Connection.CallService("WH_WEB_TIM_KIEM_NHOM_SP", new List<WH_V3_NHOM_SAN_PHAMDTO> { new WH_V3_NHOM_SAN_PHAMDTO() { MA_NHOM_KHO = ClientSession.getCusCode() } });
                if (rpNhomSP.IsError)
                {
                    throw new Exception(rpNhomSP.Description);
                }
                List<WH_V3_NHOM_SAN_PHAMDTO> lstNhomSP = JsonConvert.DeserializeObject<List<WH_V3_NHOM_SAN_PHAMDTO>>(rpNhomSP.ResponseData);
                ViewData["DVT"] = lstDVT;
                ViewData["NhomSP"] = lstNhomSP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        public ActionResult grvDanhMucSanPham()
        {
            var model = Session["SanPham"] as List<WH_V3_PRODUCT_TYPEDTO>;
            if (model == null)
                model = new List<WH_V3_PRODUCT_TYPEDTO>();
            return PartialView("grvDanhMucSanPham", model);
        }
        public ActionResult TimKiemSP(string tenSP)
        {
            Message mess = new Message();
            WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
            try
            {
                mess.isError = false;
                dto.TEN_SAN_PHAM = tenSP.Trim();
                dto.MA_NHOM_KHO = ClientSession.getCusCode();
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                var rp = Connection.CallService("WH_WEB_TIM_KIEM_SP", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                List<WH_V3_PRODUCT_TYPEDTO> lstSP = JsonConvert.DeserializeObject<List<WH_V3_PRODUCT_TYPEDTO>>(rp.ResponseData);
                foreach (var item in lstSP)
                {
                    if (item.TRANG_THAI.Equals("ACTIVE"))
                    {
                        item.TRANG_THAI = "Hoạt động";
                    }
                }
                Session["SanPham"] = lstSP;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult GetAutonumSP()
        {
            JMessage mess = new JMessage();
            try
            {
                WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
                dto.MA_SAN_PHAM = CommonData.GetAutoNum("WHV3PRO", 1)[0];
                List<WH_V3_PRODUCT_TYPEDTO> lst = new List<WH_V3_PRODUCT_TYPEDTO>() { dto };
                mess.Error = false;
                mess.Object = lst;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult AddSP(WH_V3_PRODUCT_TYPEDTO model)
        {
            Message mess = new Message();
            try
            {
                WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
                dto.MA_SAN_PHAM = model.MA_SAN_PHAM;
                dto.TEN_SAN_PHAM = model.TEN_SAN_PHAM;
                dto.MA_DON_VI_TINH = model.MA_DON_VI_TINH;
                dto.TEN_DON_VI_TINH = model.TEN_DON_VI_TINH;
                dto.MA_DON_VI_TINH_QUY_DOI = model.MA_DON_VI_TINH_QUY_DOI;
                dto.TEN_DON_VI_TINH_QUY_DOI = model.TEN_DON_VI_TINH_QUY_DOI;
                dto.MA_DINH_DANH = ClientSession.getWH_OWNER_NAME();
                dto.MA_DINH_DANH_TEMPLATE = ClientSession.getWH_OWNER();
                dto.MA_SP_NHA_CUNG_CAP = dto.MA_SP_NHA_SAN_XUAT = dto.MA_THAM_CHIEU = dto.MA_THAM_CHIEU_2 = dto.MA_SP_OWNER = model.MA_SP_NHA_CUNG_CAP;
                dto.GHI_CHU = "WEB";
                dto.TRANG_THAI = "ACTIVE";
                dto.CO_SERI = model.CO_SERI;
                dto.MA_NHOM_KHO = ClientSession.getCusCode();
                dto.MA_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC();
                dto.TEN_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC_NAME();
                dto.NUOC_SAN_XUAT = "VN";
                dto.MA_NHOM_SAN_PHAM = model.MA_NHOM_SAN_PHAM;
                dto.SAN_PHAM_COMBO = false;
                dto.INSERT_DATE = dto.INSERT_TIME = dto.UPDATE_DATE = dto.UPDATE_TIME = DateTime.Now;
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                dto.MA_NGANH_HANG = "WHV3NH000000006";
                dto.TEN_NGANH_HANG = "Bình thường";
                dto.LOAI_SAN_PHAM = "BINH_THUONG";
                dto.TRONG_LUONG = model.TRONG_LUONG;
                dto.BAT_BUOC_XUAT_KHO_THEO_HAN_SD = model.BAT_BUOC_XUAT_KHO_THEO_HAN_SD;
                dto.DON_GIA = model.DON_GIA;
                dto.MO_TA = model.MO_TA;
                dto.INSERT_USER = ClientSession.getCusCode();
                dto.UPDATE_USER = ClientSession.getCusCode();
                dto.TYPE = model.TYPE;

                //Lưu ảnh
                string image1 = "";
                string image2 = "";
                string image3 = "";
                string image4 = "";
                string image5 = "";

                if (Request != null)
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (var i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null)
                        {
                            string FileName = file.FileName;
                            Image image = Image.FromStream(file.InputStream, true);
                            Image fileTemp = CommonData.ResizeImage(image, 800, 600);
                            string imageID = CommonData.SaveImagetoMongoDB(fileTemp, FileName);
                            if (string.IsNullOrWhiteSpace(image1)) image1 = imageID;
                            else if (string.IsNullOrWhiteSpace(image2)) image2 = imageID;
                            else if (string.IsNullOrWhiteSpace(image3)) image3 = imageID;
                            else if (string.IsNullOrWhiteSpace(image4)) image4 = imageID;
                            else image5 = imageID;
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(image1))
                {
                    dto.IMG = image1;
                }
                if (!string.IsNullOrWhiteSpace(image2))
                {
                    dto.IMG += ";" + image2;
                }
                if (!string.IsNullOrWhiteSpace(image3))
                {
                    dto.IMG += ";" + image3;
                }
                if (!string.IsNullOrWhiteSpace(image4))
                {
                    dto.IMG += ";" + image4;
                }
                if (!string.IsNullOrWhiteSpace(image5))
                {
                    dto.IMG += ";" + image5;
                }
                var rp = Connection.CallService("WH_WEB_LUU_SAN_PHAM", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                mess.isSuccess = true;
                if (dto.TYPE == "ADD")
                {
                    mess.Mess = Booking.WH_themThanhCong;
                }
                else
                {
                    mess.Mess = Booking.WH_suaThanhCong;
                }
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult CapNhatGhiChu(WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO dto)
        {
            JMessage msg = new JMessage();
            try
            {
                List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> lst = Session["YeuCauLuu"] as List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>;
                foreach (var item in lst)
                {
                    if (item.ID == dto.ID)
                    {
                        item.GHI_CHU = dto.GHI_CHU;
                    }
                }
                Session["YeuCauLuu"] = lst;
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult CapNhatGhiChuXK(WH_V3_XUAT_KHO_CHI_TIETDTO dto)
        {
            JMessage msg = new JMessage();
            try
            {
                List<WH_V3_XUAT_KHO_CHI_TIETDTO> lst = Session["YeuCauLuu"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
                foreach (var item in lst)
                {
                    if (item.ID == dto.ID)
                    {
                        item.GHI_CHU = dto.GHI_CHU;
                    }
                }
                Session["YeuCauLuu"] = lst;
            }
            catch (Exception ex)
            {
                msg.Error = true;
                msg.Title = ex.Message;
            }
            return Json(msg);
        }
        public ActionResult LuuNhapKho(string maYCNK, string ngayDinhNK, string gioDinhNK, string ngayDatHang, string maPO, string ghiChu)
        {
            Message mess = new Message();
            try
            {
                WH_V3_YEU_CAU_NHAP_KHODTO dto = new WH_V3_YEU_CAU_NHAP_KHODTO();
                dto.MA_YEU_CAU_NHAP_KHO = maYCNK;
                dto.NGAY_DINH_NHAP_KHO = DateTime.ParseExact(ngayDinhNK, "dd/MM/yyyy", null);
                dto.GIO_DINH_NHAP_KHO = DateTime.Now;
                dto.NGAY_LAP = DateTime.ParseExact(ngayDatHang, "dd/MM/yyyy", null);
                dto.GIO_LAP = dto.NGAY_LAP;
                dto.MA_PO_NHA_CUNG_CAP = maPO;
                dto.GHI_CHU = ghiChu;
                dto.TRANG_THAI = "DA_NHAP_KHO";
                dto.TRANG_THAI_SHOW = "Đã nhập kho";
                dto.TU_MA_WARE_HOUSE = ClientSession.getWH_NCC();
                dto.TU_TEN_WARE_HOUSE = ClientSession.getWH_NCC_NAME();
                dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.DEN_TEN_WARE_HOUSE = ClientSession.getWH_OWNER_NAME();
                dto.INSERT_USER = ClientSession.getCusCode();
                dto.INSERT_DATE = dto.INSERT_TIME = DateTime.Now;
                dto.UPDATE_DATE = dto.UPDATE_TIME = DateTime.Now;
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                dto.MA_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC();
                dto.TEN_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC_NAME();
                dto.LOAI_NHAP_KHO = "BINH_THUONG";
                List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> lstTrenLuoi = Session["YeuCau"] as List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>;
                if (lstTrenLuoi == null)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_khongCoChiTiet;
                    return Json(mess);
                }
                foreach (var item in lstTrenLuoi)
                {
                    item.MA_YEU_CAU_NHAP_KHO = dto.MA_YEU_CAU_NHAP_KHO;
                    item.MA_YEU_CAU_NHAP_KHO_CHI_TIET = CommonData.GetAutoNum("WHV3MNKCT", 1)[0];
                    item.INSERT_DATE = item.INSERT_TIME = item.UPDATE_DATE = item.UPDATE_TIME = DateTime.Now;
                }
                dto.LstYeuCauChiTiet = lstTrenLuoi;
                List<WH_V3_YEU_CAU_NHAP_KHODTO> lstNhapKho = new List<WH_V3_YEU_CAU_NHAP_KHODTO> { dto };
                var rp = Connection.CallService("WH_WEB_LUU_NHAP_KHO", lstNhapKho);
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }

                mess.isError = false;
                mess.Mess = Booking.WH_themThanhCong;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult LuuXuatKho(string maXK, string ngayDinhXK, string ghiChu, string maKhoKH, string tenKH, string soDienThoai, string tinh, string huyen, string diaChiSoNha)
        {
            Message mess = new Message();
            try
            {
                WH_V3_XUAT_KHODTO dto = new WH_V3_XUAT_KHODTO();
                dto.MA_XUAT_KHO = maXK;
                dto.NGAY_XUAT_KHO = dto.NGAY_XUAT_DU_KIEN = DateTime.ParseExact(ngayDinhXK, "dd/MM/yyyy", null);
                dto.GIO_XUAT_KHO = dto.GIO_XUAT_DU_KIEN = DateTime.Now;
                dto.GIO_PHAT_HANG_DU_KIEN = new DateTime(1900, 1, 1);
                if (string.IsNullOrWhiteSpace(dto.MA_XUAT_KHO))
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_maXKKhongDuocBoTrong;
                    return Json(mess);
                }
                dto.GHI_CHU = ghiChu;
                dto.TRANG_THAI = "MOI_LAP";
                dto.TRANG_THAI_SHOW = "Mới lập";
                dto.TU_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.TU_TEN_WARE_HOUSE = ClientSession.getWH_OWNER_NAME();
                //dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_KH();
                //dto.DEN_TEN_WARE_HOUSE = ClientSession.getWH_KH_NAME();
                dto.INSERT_USER = ClientSession.getCusCode();
                dto.INSERT_DATE = dto.INSERT_TIME = DateTime.Now;
                dto.UPDATE_DATE = dto.UPDATE_TIME = DateTime.Now;
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                dto.MA_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC();
                dto.TEN_NHA_CUNG_CAP = ClientSession.getKHNCC_NCC_NAME();
                dto.MA_LOAI_XUAT_KHO = "XUAT_KHO";
                dto.MA_NHOM_KHO = ClientSession.getCusCode();
                dto.MA_NHAN_VIEN = ClientSession.getCusCode();
                dto.TEN_NHAN_VIEN = ClientSession.getCusName();
                List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstTrenLuoi = Session["YeuCauXK"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
                if (lstTrenLuoi == null)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_khongCoChiTiet;
                    return Json(mess);
                }
                foreach (var item in lstTrenLuoi)
                {
                    item.MA_XUAT_KHO = dto.MA_XUAT_KHO;
                    item.MA_XUAT_KHO_CHI_TIET = CommonData.GetAutoNum("WHV3XKCT", 1)[0];
                    item.INSERT_DATE = item.INSERT_TIME = item.UPDATE_DATE = item.UPDATE_TIME = DateTime.Now;
                }
                dto.ListXuatKhoChiTiet = lstTrenLuoi;
                var dicTinh = _listZone.Where(u => !string.IsNullOrEmpty(u.TEN_CO_DAU)).GroupBy(u => u.TEN_CO_DAU).ToDictionary(u => u.Key, x => x.ToList());
                if (dicTinh.ContainsKey(tinh))
                {
                    dto.MA_TINH = dicTinh[tinh][0].ZONE_CODE;
                }
                else
                {
                    dto.MA_TINH = tinh;
                }
                var dicHuyen = _listDistrict.GroupBy(u => u.DISTRICT_NAME).ToDictionary(u => u.Key, x => x.ToList());
                if (dicHuyen.ContainsKey(huyen))
                {
                    dto.MA_HUYEN = dicHuyen[huyen][0].DISTRICT_CODE;
                }
                else
                {
                    dto.MA_HUYEN = huyen;
                }
                dto.DIA_CHI_SO_NHA = diaChiSoNha;
                dto.MA_KHO_KH = maKhoKH;
                dto.TEN_KHACH_HANG = tenKH;
                dto.SO_DIEN_THOAI = soDienThoai;
                var rp = Connection.CallService("WH_WEB_LUU_XUAT_KHO", new List<WH_V3_XUAT_KHODTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                Session["YeuCauXK"] = null;
                mess.isError = false;
                mess.Mess = Booking.WH_themThanhCong;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult GetThongTinSanPham(string maSanPham, string maHang, string maSPNSX, bool isTheoMaSP, bool xuatKho)
        {
            JMessage mess = new JMessage();
            try
            {
                List<WH_V3_PRODUCT_TYPEDTO> lstSP = new List<WH_V3_PRODUCT_TYPEDTO>();
                if (isTheoMaSP)
                {
                    lstSP = Session["SanPham"] as List<WH_V3_PRODUCT_TYPEDTO>;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(maHang) && !string.IsNullOrWhiteSpace(maSPNSX))
                    {
                        mess.Error = true;
                        mess.Title = Booking.WH_khongNhapMaHangNSX;
                        return Json(mess);
                    }
                    WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
                    dto.MA_NHOM_KHO = ClientSession.getCusCode();
                    dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                    if (!string.IsNullOrWhiteSpace(maHang))
                    {
                        dto.MA_SP_OWNER = maHang;
                        var rp = Connection.CallService("WH_WEB_TIM_KIEM_SP_THEO_MA_HANG_MA_SP_NSX", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                        if (rp.IsError)
                        {
                            throw new Exception(rp.Description);
                        }
                        lstSP = JsonConvert.DeserializeObject<List<WH_V3_PRODUCT_TYPEDTO>>(rp.ResponseData);
                    }
                    else if (!string.IsNullOrWhiteSpace(maSPNSX))
                    {
                        dto.MA_SP_NHA_SAN_XUAT = maSPNSX;
                        var rp = Connection.CallService("WH_WEB_TIM_KIEM_SP_THEO_MA_HANG_MA_SP_NSX", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                        if (rp.IsError)
                        {
                            throw new Exception(rp.Description);
                        }
                        lstSP = JsonConvert.DeserializeObject<List<WH_V3_PRODUCT_TYPEDTO>>(rp.ResponseData);
                    }
                    Session["SP"] = lstSP;
                    if (lstSP.Count == 0)
                    {
                        mess.Error = true;
                        mess.Title = Booking.WH_khongTimDuocSP;
                        return Json(mess);
                    }
                    maSanPham = lstSP[0].MA_SAN_PHAM;
                }
                var dicSP = lstSP.GroupBy(u => u.MA_SAN_PHAM).ToDictionary(u => u.Key, x => x.ToList());
                List<WH_V3_PRODUCT_TYPEDTO> lstResult = new List<WH_V3_PRODUCT_TYPEDTO>();
                if (xuatKho)
                {
                    if (dicSP.ContainsKey(maSanPham))
                    {
                        WH_V3_EXPIRY_MANAGEMENTDTO dto = new WH_V3_EXPIRY_MANAGEMENTDTO() { MA_SAN_PHAM = maSanPham, MA_WARE_HOUSE = ClientSession.getWH_OWNER() };
                        var rp = Connection.CallService("WH_WEB_SEARCH_TON_KHO", new List<WH_V3_EXPIRY_MANAGEMENTDTO> { dto });
                        if (rp.IsError)
                        {
                            throw new Exception(rp.Description);
                        }
                        List<WH_V3_EXPIRY_MANAGEMENTDTO> lstCell = JsonConvert.DeserializeObject<List<WH_V3_EXPIRY_MANAGEMENTDTO>>(rp.ResponseData);
                        if (lstCell.Count == 0)
                        {
                            mess.Error = true;
                            mess.Title = Booking.WH_SPKhongConTrongKho;
                            return Json(mess);
                        }
                        int STT = 1;
                        foreach (var item in lstCell)
                        {
                            item.ID = STT;
                            STT++;
                        }
                        dicSP[maSanPham][0].SL_TON = lstCell[0].SO_LUONG;
                        dicSP[maSanPham][0].NGAY_SX = lstCell[0].NGAY_SAN_XUAT.ToString("dd/MM/yyyy");
                        //dicSP[maSanPham][0].SL_TON = lstCell.Sum(u => u.SO_LUONG);
                        lstResult.Add(dicSP[maSanPham][0]);
                        Session["NgaySX"] = lstCell;
                    }
                    else
                    {
                        mess.Error = true;
                        mess.Title = Booking.WH_khongTonTaiMaHang;
                        return Json(mess);
                    }
                }
                else
                {
                    lstResult = lstSP.Where(u => u.MA_SAN_PHAM == maSanPham).ToList();
                    if (!string.IsNullOrWhiteSpace(lstResult[0].IMG))
                    {
                        List<string> lstIDImg = lstResult[0].IMG.Split(';').Where(u => u != "").ToList();

                        for (int item = 0; item < lstIDImg.Count(); item++)
                        {
                            switch (item)
                            {
                                case 0:
                                    lstResult[0].URL_IMG1 = CommonData.GetImageFromMongoDB(lstIDImg[item]);
                                    break;
                                case 1:
                                    lstResult[0].URL_IMG2 = CommonData.GetImageFromMongoDB(lstIDImg[item]);
                                    break;
                                case 2:
                                    lstResult[0].URL_IMG3 = CommonData.GetImageFromMongoDB(lstIDImg[item]);
                                    break;
                                case 3:
                                    lstResult[0].URL_IMG4 = CommonData.GetImageFromMongoDB(lstIDImg[item]);
                                    break;
                                case 4:
                                    lstResult[0].URL_IMG5 = CommonData.GetImageFromMongoDB(lstIDImg[item]);
                                    break;
                            }
                        }
                    }
                }
                mess.Error = false;
                mess.Object = lstResult;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult ThemSanPhamLenLuoi(string maSanPham, string soLuong, string soLuongQD, string ngaySX, string ngayHH, string ghiChu)
        {
            Message mess = new Message();
            try
            {
                if (string.IsNullOrWhiteSpace(maSanPham))
                {
                    var SP = Session["SP"] as List<WH_V3_PRODUCT_TYPEDTO>;
                    maSanPham = SP[0].MA_SAN_PHAM;
                }
                List<WH_V3_PRODUCT_TYPEDTO> lstSP = Session["SanPham"] as List<WH_V3_PRODUCT_TYPEDTO>;
                var dicSP = lstSP.GroupBy(u => u.MA_SAN_PHAM).ToDictionary(u => u.Key, x => x.ToList());
                List<WH_V3_PRODUCT_TYPEDTO> lstSPThem = new List<WH_V3_PRODUCT_TYPEDTO>();
                if (dicSP.ContainsKey(maSanPham))
                {
                    lstSPThem.Add(dicSP[maSanPham][0]);
                }
                if (lstSP == null)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_SPKhongDuocTrong;
                    return Json(mess);
                }
                decimal sl = Convert.ToDecimal(soLuong);
                decimal slqd = Convert.ToDecimal(soLuongQD);
                DateTime ngaySanXuat = DateTime.ParseExact(ngaySX, "dd/MM/yyyy", null);
                DateTime ngayHetHan = DateTime.ParseExact(ngayHH, "dd/MM/yyyy", null);
                if (sl <= 0)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_soLuongKhongDuocTrong;
                    return Json(mess);
                }
                if ((ngayHetHan - ngaySanXuat).TotalDays < 0)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_ngayHHLonHonNgaySX;
                    return Json(mess);
                }
                List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> lstThemLenLuoi = new List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>();
                foreach (var item in lstSPThem)
                {
                    WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO dto = new WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO();
                    dto.MA_SAN_PHAM = item.MA_SAN_PHAM;
                    dto.MA_HANG_VINMART = dto.MA_SP_NHA_CUNG_CAP = item.MA_SP_NHA_CUNG_CAP;
                    dto.TEN_SAN_PHAM = item.TEN_SAN_PHAM;
                    dto.TEN_DON_VI_TINH = item.TEN_DON_VI_TINH;
                    dto.TEN_DON_VI_TINH_QUY_DOI = item.TEN_DON_VI_TINH_QUY_DOI;
                    dto.SO_LUONG = sl;
                    dto.TY_LE_QUY_DOI = 1;
                    dto.SO_LUONG_QUY_DOI = slqd;
                    dto.NGAY_SAN_XUAT = ngaySanXuat;
                    dto.NGAY_HET_HAN = ngayHetHan;
                    dto.GHI_CHU = ghiChu;
                    lstThemLenLuoi.Add(dto);
                }
                List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> lstTrenLuoi = Session["YeuCau"] as List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>;
                if (lstTrenLuoi == null)
                {
                    lstTrenLuoi = new List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>();
                }
                lstTrenLuoi.AddRange(lstThemLenLuoi);
                int id = 1;
                foreach (var item in lstTrenLuoi)
                {
                    item.ID = id;
                    id++;
                }
                Session["NgaySX"] = null;
                Session["YeuCau"] = lstTrenLuoi;
                mess.isError = false;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult XoaChiTiet(int id, string loai)
        {
            Message mess = new Message();
            try
            {
                switch (loai)
                {
                    case "NK":
                        List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO> lstNKTrenLuoi = Session["YeuCau"] as List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>;
                        lstNKTrenLuoi.RemoveAll(u => u.ID == id);
                        Session["YeuCau"] = lstNKTrenLuoi;
                        break;
                    case "XK":
                        List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstXKTrenLuoi = Session["YeuCauXK"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
                        lstXKTrenLuoi.RemoveAll(u => u.ID == id);
                        Session["YeuCauXK"] = lstXKTrenLuoi;
                        break;
                }
                mess.isError = false;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
                return Json(mess);
            }
            return Json(mess);
        }
        public ActionResult ThemSanPhamLenLuoiXK(string maSanPham, string soLuong, string soLuongQD, string soLuongTon, string ghiChu)
        {
            Message mess = new Message();
            try
            {
                if (string.IsNullOrWhiteSpace(maSanPham))
                {
                    var SP = Session["SP"] as List<WH_V3_PRODUCT_TYPEDTO>;
                    maSanPham = SP[0].MA_SAN_PHAM;
                }
                List<WH_V3_PRODUCT_TYPEDTO> lstSP = Session["SanPham"] as List<WH_V3_PRODUCT_TYPEDTO>;
                var dicSP = lstSP.GroupBy(u => u.MA_SAN_PHAM).ToDictionary(u => u.Key, x => x.ToList());
                List<WH_V3_PRODUCT_TYPEDTO> lstSPThem = new List<WH_V3_PRODUCT_TYPEDTO>();
                if (dicSP.ContainsKey(maSanPham))
                {
                    lstSPThem.Add(dicSP[maSanPham][0]);
                }
                if (lstSP == null)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_SPKhongDuocTrong;
                    return Json(mess);
                }
                decimal sl = Convert.ToDecimal(soLuong);
                decimal slqd = Convert.ToDecimal(soLuongQD);
                decimal slTon = Convert.ToDecimal(soLuongTon);
                List<WH_V3_EXPIRY_MANAGEMENTDTO> lstNgaySXChon = Session["NgaySXChon"] as List<WH_V3_EXPIRY_MANAGEMENTDTO>;
                if (lstNgaySXChon == null)
                {
                    lstNgaySXChon = new List<WH_V3_EXPIRY_MANAGEMENTDTO>();
                }
                DateTime ngaySanXuat = new DateTime(1900, 1, 1);
                DateTime ngayHetHan = new DateTime(1900, 1, 1);
                if (lstNgaySXChon.Count > 0)
                {
                    ngaySanXuat = lstNgaySXChon[0].NGAY_SAN_XUAT;
                    ngayHetHan = lstNgaySXChon[0].NGAY_HET_HAN;
                }
                if (sl <= 0)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_soLuongKhongDuocTrong;
                    return Json(mess);
                }
                if (sl > slTon)
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_soLuongTonKhongDu;
                    return Json(mess);
                }

                //Lấy location
                LOCATION_AND_CELL dtoLocationCell = new LOCATION_AND_CELL() { MA_NHOM_KHO = ClientSession.getCusCode() };
                var rp = Connection.CallService("WH_WEB_TIM_KIEM_LOCATION_CELL", new List<LOCATION_AND_CELL> { dtoLocationCell });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstLocationCell = JsonConvert.DeserializeObject<List<LOCATION_AND_CELL>>(rp.ResponseData);

                List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstThemLenLuoi = new List<WH_V3_XUAT_KHO_CHI_TIETDTO>();
                foreach (var item in lstSPThem)
                {
                    WH_V3_XUAT_KHO_CHI_TIETDTO dto = new WH_V3_XUAT_KHO_CHI_TIETDTO();
                    dto.MA_SAN_PHAM = item.MA_SAN_PHAM;
                    dto.MA_HANG_VINMART = dto.MA_SP_NHA_CUNG_CAP = item.MA_SP_NHA_CUNG_CAP;
                    dto.TEN_SAN_PHAM = item.TEN_SAN_PHAM;
                    dto.TEN_DON_VI_TINH = item.TEN_DON_VI_TINH;
                    dto.TEN_DON_VI_TINH_QUY_DOI = item.TEN_DON_VI_TINH_QUY_DOI;
                    dto.SO_LUONG = sl;
                    dto.TY_LE_QUY_DOI = 1;
                    dto.SO_LUONG_QUY_DOI = slqd;
                    dto.NGAY_SAN_XUAT = ngaySanXuat;
                    dto.NGAY_HET_HAN = ngayHetHan;
                    dto.GHI_CHU = ghiChu;


                    //Hỏi lại
                    var lstTuCell = lstLocationCell.Where(u => u.TYPE == "CELL" && u.MA_LOAI == "INPUT").Select(u => u.MA_CELL).ToList();
                    if (lstTuCell.Count > 0)
                    {
                        dto.TU_MA_CELL = /*"WHV3CELL000014572"*/lstTuCell[0];
                    }
                    var lstTuLocation = lstLocationCell.Where(u => u.TYPE == "LOCATION" && u.MA_LOAI == "INPUT").Select(u => u.MA_LOCATION).ToList();
                    if (lstTuLocation.Count > 0)
                    {
                        dto.TU_MA_LOCATION = /*"WHV3LOC000001143"*/lstTuLocation[0];
                    }
                    var lstDenCell = lstLocationCell.Where(u => u.TYPE == "CELL" && u.MA_LOAI == "DEFAULT").Select(u => u.MA_CELL).ToList();
                    if (lstDenCell.Count > 0)
                    {
                        dto.DEN_MA_CELL = /*"WHVEC000000297CELL"*/lstDenCell[0];
                    }
                    var lstDenLocation = lstLocationCell.Where(u => u.TYPE == "LOCATION" && u.MA_LOAI == "DEFAULT").Select(u => u.MA_LOCATION).ToList();
                    if (lstDenLocation.Count > 0)
                    {
                        dto.DEN_MA_LOCATION = /*"WHVEC000000297LOCATI"*/lstDenLocation[0];
                    }
                    dto.PHAN_LOAI_SP = "NORMAL";
                    lstThemLenLuoi.Add(dto);
                }
                List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstTrenLuoi = Session["YeuCauXK"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
                if (lstTrenLuoi == null)
                {
                    lstTrenLuoi = new List<WH_V3_XUAT_KHO_CHI_TIETDTO>();
                }
                lstTrenLuoi.AddRange(lstThemLenLuoi);
                int id = 1;
                foreach (var item in lstTrenLuoi)
                {
                    item.ID = id;
                    id++;
                }
                Session["YeuCauXK"] = lstTrenLuoi;
                mess.isError = false;
                Session["NgaySXChon"] = null;
                Session["NgaySX"] = null;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult _grvThongTinYCNhapKhoPartial()
        {
            var model = Session["YeuCau"] as List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>;
            if (model == null)
                model = new List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>();
            model = model.OrderBy(u => u.INSERT_TIME).ToList();
            Session["YeuCauLuu"] = model;
            return PartialView("_grvThongTinYCNhapKhoPartial", model);
        }
        public ActionResult GLUNgaySXPartial()
        {
            List<WH_V3_EXPIRY_MANAGEMENTDTO> lstNgaySX = Session["NgaySX"] as List<WH_V3_EXPIRY_MANAGEMENTDTO>;
            if (lstNgaySX == null)
            {
                lstNgaySX = new List<WH_V3_EXPIRY_MANAGEMENTDTO>();
            }
            List<WH_V3_EXPIRY_MANAGEMENTDTO> model = lstNgaySX;
            return PartialView("GLUNgaySXPartial", model);
        }
        public ActionResult changeNgaySX(string ngaySX)
        {
            JMessage mess = new JMessage();
            try
            {
                List<WH_V3_EXPIRY_MANAGEMENTDTO> lstNgaySX = Session["NgaySX"] as List<WH_V3_EXPIRY_MANAGEMENTDTO>;
                if (lstNgaySX == null)
                {
                    lstNgaySX = new List<WH_V3_EXPIRY_MANAGEMENTDTO>();
                }
                int index = 0;
                if (!string.IsNullOrWhiteSpace(ngaySX))
                {
                    index = Convert.ToInt32(ngaySX) - 1;
                }
                lstNgaySX[index].NGAY_HH = lstNgaySX[index].NGAY_HET_HAN.ToString("dd/MM/yyyy");
                List<WH_V3_EXPIRY_MANAGEMENTDTO> lstReturn = new List<WH_V3_EXPIRY_MANAGEMENTDTO>() { lstNgaySX[index] };
                Session["NgaySXChon"] = lstReturn;
                mess.Error = false;
                mess.Object = lstReturn;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult YeuCauNhapKhoChiTiet()
        {
            JMessage mess = new JMessage();
            Session["YeuCau"] = new List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>();
            try
            {
                WH_V3_YEU_CAU_NHAP_KHODTO model = new WH_V3_YEU_CAU_NHAP_KHODTO()
                {
                    MA_YEU_CAU_NHAP_KHO = CommonData.GetAutoNum("WHV3YCNK", 1)[0],
                    NGAY_LAP = DateTime.Now,
                    NGAY_DINH_NHAP_KHO = DateTime.Now,
                    GIO_DINH_NHAP_KHO = DateTime.Now
                };
                WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
                dto.MA_NHOM_KHO = ClientSession.getCusCode();
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                var rp = Connection.CallService("WH_WEB_TIM_KIEM_SP", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstSP = JsonConvert.DeserializeObject<List<WH_V3_PRODUCT_TYPEDTO>>(rp.ResponseData);
                ViewData["SanPham"] = lstSP;
                Session["SanPham"] = lstSP;
                mess.Error = false;
                mess.Object = lstSP;
                return View(model);
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
                return Json(mess);
            }
        }
        public ActionResult YeuCauXuatKhoChiTiet()
        {
            JMessage mess = new JMessage();
            Session["YeuCauXK"] = new List<WH_V3_YEU_CAU_NHAP_KHO_CHI_TIETDTO>();
            try
            {
                WH_V3_XUAT_KHODTO model = new WH_V3_XUAT_KHODTO()
                {
                    MA_XUAT_KHO = CommonData.GetAutoNum("WHV3XK", 1)[0],
                    NGAY_XUAT_KHO = DateTime.Now,
                    GIO_XUAT_KHO = DateTime.Now
                };
                WH_V3_PRODUCT_TYPEDTO dto = new WH_V3_PRODUCT_TYPEDTO();
                dto.MA_NHOM_KHO = ClientSession.getCusCode();
                dto.MA_OWNER = ClientSession.getKHNCC_OWNER();
                var rp = Connection.CallService("WH_WEB_TIM_KIEM_SP", new List<WH_V3_PRODUCT_TYPEDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstSP = JsonConvert.DeserializeObject<List<WH_V3_PRODUCT_TYPEDTO>>(rp.ResponseData);

                WH_V3_WARE_HOUSEDTO dtoKH = new WH_V3_WARE_HOUSEDTO();
                dtoKH.MA_NHOM_KHO = ClientSession.getCusCode();
                var rpKH = Connection.CallService("WH_WEB_TIM_KIEM_KHO_HANG", new List<WH_V3_WARE_HOUSEDTO> { dtoKH });
                if (rpKH.IsError)
                {
                    throw new Exception(rpKH.Description);
                }
                List<WH_V3_WARE_HOUSEDTO> lstNhomKhoKH = new List<WH_V3_WARE_HOUSEDTO>() { new WH_V3_WARE_HOUSEDTO() { ID = 1, MA_WARE_HOUSE = "KHLE", TEN_WARE_HOUSE = "Khách hàng lẻ" } };
                List<WH_V3_WARE_HOUSEDTO> lstNhomKhoKHSearch = JsonConvert.DeserializeObject<List<WH_V3_WARE_HOUSEDTO>>(rpKH.ResponseData)
                    .Where(u => u.LOAI_WARE_HOUSE == WARE_HOUSE_KH).ToList();
                int i = 2;
                foreach (var item in lstNhomKhoKHSearch)
                {
                    item.ID = i;
                    i++;
                }
                lstNhomKhoKH.AddRange(lstNhomKhoKHSearch);
                ViewData["SanPham"] = lstSP;
                Session["SanPham"] = lstSP;
                ViewData["KhoKhachHang"] = lstNhomKhoKH;
                Session["KhoKhachHang"] = lstNhomKhoKH;
                mess.Error = false;
                mess.Object = lstSP;
                return View(model);
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
                return Json(mess);
            }
        }
        public ActionResult _grvThongTinYCXuatKhoPartial()
        {
            var model = Session["YeuCauXK"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
            if (model == null)
                model = new List<WH_V3_XUAT_KHO_CHI_TIETDTO>();
            model = model.OrderBy(u => u.INSERT_TIME).ToList();
            Session["YeuCauLuu"] = model;
            return PartialView("_grvThongTinYCXuatKhoPartial", model);
        }
        public ActionResult BaoCaoXuatNhapTon()
        {
            BaoCaoXuatNhapTonKhoVinMartParaDTO model = new BaoCaoXuatNhapTonKhoVinMartParaDTO()
            {
                TU_NGAY = DateTime.Now,
                DEN_NGAY = DateTime.Now
            };

            return View(model);
        }
        private Dictionary<string, string> dicXuatNhapTonSearch = new Dictionary<string, string>()
        {
            { "TL_CHO_XUAT_TON_DAU_KY","CHO_XUAT_TON_DAU_KY" },
            { "TL_HANG_HONG_TON_DAU_KY","HANG_HONG_TON_DAU_KY" },
            { "TRONG_LUONG_NHAP","NHAP_KHO" },
            { "TRONG_LUONG_XUAT","XUAT_KHO" },
            { "TL_CHO_XUAT_TON_CUOI_KY","CHO_XUAT_TON_CUOI_KY" },
            { "TL_HANG_HONG_TON_CUOI_KY","HANG_HONG_TON_CUOI_KY" }
        };
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BaoCaoXuatNhapTon(BaoCaoXuatNhapTonKhoVinMartParaDTO dto)
        {
            dto.MA_WARE_HOUSE = ClientSession.getWH_OWNER();
            dto.TU_NGAY = ComboBoxExtension.GetValue<DateTime>("detTuNgay");
            dto.DEN_NGAY = ComboBoxExtension.GetValue<DateTime>("detDenNgay");
            Message mess = new Message();
            BaoCaoXuatNhapTonKhoVinMartParaDTO model = new BaoCaoXuatNhapTonKhoVinMartParaDTO()
            {
                TU_NGAY = dto.TU_NGAY,
                DEN_NGAY = dto.DEN_NGAY
            };
            try
            {
                List<BaoCaoXuatNhapTonKhoVinMartParaDTO> lstShow = new List<BaoCaoXuatNhapTonKhoVinMartParaDTO>();
                BaoCaoXuatNhapTonKhoVinMartParaDTO dtoShow = new BaoCaoXuatNhapTonKhoVinMartParaDTO();

                var rp = Connection.CallServiceObject("WH_WEB_BAO_CAO_XUAT_NHAP_TON", dto);
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<BaoCaoXuatNhapTonKhoVinMartParaDTO>>(rp.ResponseData);

                Session["BCXuatNhapTon"] = lstReturn;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                Session["BCXuatNhapTon"] = null;
                return View(model);
            }
        }
        public ActionResult grvBaoCaoXuatNhapTon()
        {
            var model = Session["BCXuatNhapTon"] as List<BaoCaoXuatNhapTonKhoVinMartParaDTO>;
            if (model == null)
                model = new List<BaoCaoXuatNhapTonKhoVinMartParaDTO>();
            return PartialView("grvBaoCaoXuatNhapTon", model);
        }
        public void searchNhapKho(WH_V3_NHAP_KHODTO dto)
        {
            List<WH_V3_NHAP_KHODTO> lstReturn = new List<WH_V3_NHAP_KHODTO>();
            if (dto.DEN_MA_WARE_HOUSE != null)
            {
                var rp = Connection.CallService("WH_WEB_SEARCH_NHAP_KHO", new List<WH_V3_NHAP_KHODTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                lstReturn = JsonConvert.DeserializeObject<List<WH_V3_NHAP_KHODTO>>(rp.ResponseData);
            }
            foreach (var item in lstReturn)
            {
                switch (item.TRANG_THAI)
                {
                    case "DA_XAC_NHAN_NHAP_KHO":
                        item.TRANG_THAI_SHOW = "Đã xác nhận";
                        break;
                    case "DA_HUY":
                        item.TRANG_THAI_SHOW = "Đã hủy";
                        break;
                    case "MOI_LAP":
                        item.TRANG_THAI_SHOW = "Mới lập";
                        break;
                }
            }
            Session["QuanLyNhapKho"] = lstReturn;
        }
        public ActionResult QuanLyNhapKho()
        {
            Message mess = new Message();
            try
            {
                WH_V3_NHAP_KHODTO dto = new WH_V3_NHAP_KHODTO();
                dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.TU_NGAY = DateTime.Now.Date;
                dto.DEN_NGAY = DateTime.Now.Date;
                dto.SEARCH_BY = "NGAY_NHAP_KHO";
                searchNhapKho(dto);

                WH_V3_NHAP_KHODTO model = new WH_V3_NHAP_KHODTO()
                {
                    TU_NGAY = DateTime.Now.AddDays(-1).Date,
                    DEN_NGAY = DateTime.Now.Date
                };
                return View(model);
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
                return Json(mess);
            }
        }
        [HttpPost]
        public ActionResult QuanLyNhapKho(WH_V3_NHAP_KHODTO dto)
        {
            Message mess = new Message();
            try
            {
                dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.TU_NGAY = ComboBoxExtension.GetValue<DateTime>("detTuNgay");
                dto.DEN_NGAY = ComboBoxExtension.GetValue<DateTime>("detDenNgay");
                dto.SEARCH_BY = RadioButtonExtension.GetValue<string>("SEARCH_BY");
                searchNhapKho(dto);

                WH_V3_NHAP_KHODTO model = new WH_V3_NHAP_KHODTO()
                {
                    TU_NGAY = dto.TU_NGAY.Date,
                    DEN_NGAY = dto.DEN_NGAY.Date
                };
                return View(model);
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
                return Json(mess);
            }
        }
        public ActionResult grvQuanLyNhapKho()
        {
            var model = Session["QuanLyNhapKho"] as List<WH_V3_NHAP_KHODTO>;
            if (model == null)
                model = new List<WH_V3_NHAP_KHODTO>();
            return PartialView("grvQuanLyNhapKho", model);
        }
        public void searchXuatKho(WH_V3_XUAT_KHODTO dto)
        {
            List<WH_V3_XUAT_KHODTO> lstReturn = new List<WH_V3_XUAT_KHODTO>();
            if (dto.DEN_MA_WARE_HOUSE != null)
            {
                var rp = Connection.CallService("WH_WEB_SEARCH_XUAT_KHO", new List<WH_V3_XUAT_KHODTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                lstReturn = JsonConvert.DeserializeObject<List<WH_V3_XUAT_KHODTO>>(rp.ResponseData);
            }

            foreach (var item in lstReturn)
            {
                switch (item.TRANG_THAI)
                {
                    case "DA_XAC_NHAN_XUAT_KHO":
                        item.TRANG_THAI_SHOW = "Đã xác nhận";
                        break;
                    case "DA_HUY":
                        item.TRANG_THAI_SHOW = "Đã hủy";
                        break;
                    case "MOI_LAP":
                        item.TRANG_THAI_SHOW = "Đã xác nhận";
                        item.TRANG_THAI = "DA_XAC_NHAN_XUAT_KHO";
                        if (item.MA_LOAI_XUAT_KHO == "DAT_HANG")
                        {
                            item.TRANG_THAI_SHOW = "Mới lập";
                            item.TRANG_THAI = "MOI_LAP";
                        }
                        break;
                }
            }
            Session["QuanLyXuatKho"] = lstReturn;
        }
        public ActionResult QuanLyXuatKho()
        {
            Message mess = new Message();
            try
            {
                WH_V3_XUAT_KHODTO dto = new WH_V3_XUAT_KHODTO();
                dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.TU_NGAY = DateTime.Now.AddDays(-1).Date;
                dto.DEN_NGAY = DateTime.Now.Date;
                dto.SEARCH_BY = "NGAY_XUAT_KHO";
                searchXuatKho(dto);

                WH_V3_XUAT_KHODTO model = new WH_V3_XUAT_KHODTO()
                {
                    TU_NGAY = DateTime.Now.AddDays(-1).Date,
                    DEN_NGAY = DateTime.Now.Date
                };
                return View(model);
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
                return Json(mess);
            }
        }
        [HttpPost]
        public ActionResult QuanLyXuatKho(WH_V3_XUAT_KHODTO dto)
        {
            try
            {
                dto.DEN_MA_WARE_HOUSE = ClientSession.getWH_OWNER();
                dto.TU_NGAY = ComboBoxExtension.GetValue<DateTime>("detTuNgay");
                dto.DEN_NGAY = ComboBoxExtension.GetValue<DateTime>("detDenNgay");
                var rp = Connection.CallService("WH_WEB_SEARCH_XUAT_KHO", new List<WH_V3_XUAT_KHODTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<WH_V3_XUAT_KHODTO>>(rp.ResponseData);
                foreach (var item in lstReturn)
                {
                    switch (item.TRANG_THAI)
                    {
                        case "DA_XAC_NHAN_XUAT_KHO":
                            item.TRANG_THAI_SHOW = "Đã xác nhận";
                            break;
                        case "DA_HUY":
                            item.TRANG_THAI_SHOW = "Đã hủy";
                            break;
                        case "MOI_LAP":
                            item.TRANG_THAI_SHOW = "Đã xác nhận";
                            item.TRANG_THAI = "DA_XAC_NHAN_XUAT_KHO";
                            if (item.MA_LOAI_XUAT_KHO == "DAT_HANG")
                            {
                                item.TRANG_THAI_SHOW = "Mới lập";
                                item.TRANG_THAI = "MOI_LAP";
                            }
                            break;
                    }
                }
                Session["QuanLyXuatKho"] = lstReturn;
                WH_V3_XUAT_KHODTO model = new WH_V3_XUAT_KHODTO()
                {
                    TU_NGAY = dto.TU_NGAY.Date,
                    DEN_NGAY = dto.DEN_NGAY.Date
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public ActionResult grvQuanLyXuatKho()
        {
            var model = Session["QuanLyXuatKho"] as List<WH_V3_XUAT_KHODTO>;
            if (model == null)
                model = new List<WH_V3_XUAT_KHODTO>();
            return PartialView("grvQuanLyXuatKho", model);
        }
        public ActionResult nhapKhoDetail()
        {
            var model = Session["ChiTietNhapKho"] as List<WH_V3_NHAP_KHO_CHI_TIETDTO>;
            if (model == null)
                model = new List<WH_V3_NHAP_KHO_CHI_TIETDTO>();
            return PartialView("nhapKhoDetail", model);
        }
        public ActionResult ShowDetailNhapKho(string maNK)
        {
            Message mes = new Message();
            try
            {
                var model = Session["ChiTietNhapKho"] as List<WH_V3_NHAP_KHO_CHI_TIETDTO>;
                if (model == null)
                    model = new List<WH_V3_NHAP_KHO_CHI_TIETDTO>();
                if (string.IsNullOrWhiteSpace(maNK))
                {
                    return Json(mes);
                }
                WH_V3_NHAP_KHO_CHI_TIETDTO dto = new WH_V3_NHAP_KHO_CHI_TIETDTO() { MA_NHAP_KHO = maNK };
                var rp = Connection.CallService("WH_WEB_SEARCH_NHAP_KHO_CHI_TIET", new List<WH_V3_NHAP_KHO_CHI_TIETDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<WH_V3_NHAP_KHO_CHI_TIETDTO>>(rp.ResponseData);
                Session["ChiTietNhapKho"] = lstReturn;
                mes.isSuccess = true;
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json(mes);
        }
        public ActionResult HuyNhapKho(string maNK)
        {
            Message mes = new Message();
            try
            {
                if (string.IsNullOrWhiteSpace(maNK))
                {
                    mes.isError = true;
                    mes.Mess = Booking.WH_vuiLongChonDongThaoTac;
                    return Json(mes);
                }
                if (string.IsNullOrWhiteSpace(maNK))
                {
                    return Json(mes);
                }
                List<WH_V3_NHAP_KHODTO> lstNhapKho = Session["QuanLyNhapKho"] as List<WH_V3_NHAP_KHODTO>;
                WH_V3_NHAP_KHODTO dtoNhapKhoChon = xacNhanVaHuyNK(lstNhapKho, maNK, true);
                var rp = Connection.CallService("WH_WEB_HUY_NHAP_KHO", new List<WH_V3_NHAP_KHODTO> { dtoNhapKhoChon });
                if (rp.IsError)
                {
                    mes.isError = true;
                    mes.Mess = rp.Description;
                    return Json(mes);
                }

                mes.isError = false;
                mes.Mess = Booking.WH_huyThanhCong;
            }
            catch (Exception ex)
            {
                mes.isError = true;
                return Json(ex.Message);
            }
            return Json(mes);
        }
        public static WH_V3_NHAP_KHODTO xacNhanVaHuyNK(List<WH_V3_NHAP_KHODTO> lstNhapKho, string maNK, bool isHuyNK = false)
        {
            if (lstNhapKho == null)
                lstNhapKho = new List<WH_V3_NHAP_KHODTO>();
            WH_V3_NHAP_KHODTO dtoNhapKhoChon = lstNhapKho.Where(u => u.MA_NHAP_KHO == maNK).ToList()[0];
            dtoNhapKhoChon.isHuyNK = isHuyNK;
            dtoNhapKhoChon.UPDATE_USER = ClientSession.getCusCode();
            dtoNhapKhoChon.UPDATE_USER_NAME = ClientSession.getCusName();
            dtoNhapKhoChon.UPDATE_DATE = dtoNhapKhoChon.UPDATE_TIME = DateTime.Now;
            return dtoNhapKhoChon;
        }
        public ActionResult HuyXuatKho(string maXK)
        {
            Message mes = new Message();
            try
            {
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    mes.isError = true;
                    mes.Mess = Booking.WH_vuiLongChonDongThaoTac;
                    return Json(mes);
                }
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    return Json(mes);
                }
                List<WH_V3_XUAT_KHODTO> lstXuatKho = Session["QuanLyXuatKho"] as List<WH_V3_XUAT_KHODTO>;
                WH_V3_XUAT_KHODTO dtoXK = lstXuatKho.Where(u => u.MA_XUAT_KHO == maXK).ToList()[0];
                dtoXK.UPDATE_USER = ClientSession.getCusCode();
                dtoXK.UPDATE_DATE = dtoXK.UPDATE_TIME = DateTime.Now;
                if (dtoXK.TRANG_THAI == "DA_HUY")
                {
                    mes.isError = true;
                    mes.Mess = Booking.WH_ycDaDuocHuy;
                    return Json(mes);
                }
                var rp = Connection.CallService("WH_WEB_HUY_XUAT_KHO", new List<WH_V3_XUAT_KHODTO> { dtoXK });
                if (rp.IsError)
                {
                    mes.isError = true;
                    mes.Mess = rp.Description;
                    return Json(mes);
                }

                mes.isError = false;
                mes.Mess = Booking.WH_huyThanhCong;
            }
            catch (Exception ex)
            {
                mes.isError = true;
                return Json(ex.Message);
            }
            return Json(mes);
        }
        public ActionResult XacNhanXuatKho(string maXK)
        {
            Message mes = new Message();
            try
            {
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    mes.isError = true;
                    mes.Mess = Booking.WH_vuiLongChonDongThaoTac;
                    return Json(mes);
                }
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    return Json(mes);
                }
                List<WH_V3_XUAT_KHODTO> lstXuatKho = Session["QuanLyXuatKho"] as List<WH_V3_XUAT_KHODTO>;
                WH_V3_XUAT_KHODTO dtoXK = lstXuatKho.Where(u => u.MA_XUAT_KHO == maXK).ToList()[0];
                dtoXK.UPDATE_USER = ClientSession.getCusCode();
                dtoXK.UPDATE_DATE = dtoXK.UPDATE_TIME = DateTime.Now;
                if (dtoXK.TRANG_THAI != "MOI_LAP")
                {
                    mes.isError = true;
                    mes.Mess = Booking.WH_chiDuocThaoTacYCMoiLap;
                    return Json(mes);
                }
                var rp = Connection.CallService("WH_WEB_XAC_NHAN_XUAT_KHO", new List<WH_V3_XUAT_KHODTO> { dtoXK });
                if (rp.IsError)
                {
                    mes.isError = true;
                    mes.Mess = rp.Description;
                    return Json(mes);
                }

                mes.isError = false;
                mes.Mess = Booking.WH_huyThanhCong;
            }
            catch (Exception ex)
            {
                mes.isError = true;
                return Json(ex.Message);
            }
            return Json(mes);
        }
        public ActionResult xuatKhoDetail()
        {
            var model = Session["ChiTietXuatKho"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
            if (model == null)
                model = new List<WH_V3_XUAT_KHO_CHI_TIETDTO>();
            return PartialView("xuatKhoDetail", model);
        }
        public ActionResult ShowDetailXuatKho(string maXK)
        {
            Message mes = new Message();
            try
            {
                var model = Session["ChiTietXuatKho"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;
                if (model == null)
                    model = new List<WH_V3_XUAT_KHO_CHI_TIETDTO>();
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    return Json(mes);
                }
                WH_V3_XUAT_KHO_CHI_TIETDTO dto = new WH_V3_XUAT_KHO_CHI_TIETDTO() { MA_XUAT_KHO = maXK };
                var rp = Connection.CallService("WH_WEB_SEARCH_XUAT_KHO_CHI_TIET", new List<WH_V3_XUAT_KHO_CHI_TIETDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<WH_V3_XUAT_KHO_CHI_TIETDTO>>(rp.ResponseData);
                Session["ChiTietXuatKho"] = lstReturn;
                mes.isSuccess = true;
                return Json(mes);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        private static List<OP_ZONE_MASTERDTO> _listZone;
        private static List<CSS_OP_DISTRICTDTO> _listDistrict;
        private static List<CSS_OP_COMMUNEDTO> _listCommune;
        public ActionResult QuanLyDanhSachKhohang()
        {
            Message mes = new Message();
            try
            {
                if (_listZone == null)
                {
                    _listZone = CommonData.GetZone();
                }
                if (_listDistrict == null)
                {
                    _listDistrict = CommonData.GetDistrict();
                }
                if (_listCommune == null)
                {
                    _listCommune = CommonData.GetCommune();
                }
                ViewData["NhomKhoHang"] = new List<CSS_DIC_GENERAL_DICTIONARY>() { new CSS_DIC_GENERAL_DICTIONARY() { DICTIONARY_CODE = ClientSession.getCusCode(), DICTIONARY_NAME = ClientSession.getCusName() } };
                ViewData["TrangThai"] = lstTrangThai;
                return View();
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
        public ActionResult grvDanhSachKhoHang()
        {
            var model = Session["NhomKhoKH"] as List<WH_V3_WARE_HOUSEDTO>;
            if (model == null)
                model = new List<WH_V3_WARE_HOUSEDTO>();
            return PartialView("grvDanhSachKhoHang", model);
        }
        public ActionResult TimKiemKhoHang(string maNhomKho)
        {
            Message mess = new Message();
            WH_V3_WARE_HOUSEDTO dto = new WH_V3_WARE_HOUSEDTO();
            try
            {
                if (string.IsNullOrWhiteSpace(maNhomKho))
                {
                    mess.isError = true;
                    mess.Mess = Booking.WH_moiChonNhomKhoHang;
                }
                else
                {
                    mess.isError = false;
                    dto.MA_NHOM_KHO = maNhomKho.Trim();
                    var rp = Connection.CallService("WH_WEB_TIM_KIEM_KHO_HANG", new List<WH_V3_WARE_HOUSEDTO> { dto });
                    if (rp.IsError)
                    {
                        throw new Exception(rp.Description);
                    }
                    List<WH_V3_WARE_HOUSEDTO> lstNhomKho = JsonConvert.DeserializeObject<List<WH_V3_WARE_HOUSEDTO>>(rp.ResponseData);
                    var lstNhomKhoKH = lstNhomKho.Where(u => u.LOAI_WARE_HOUSE == WARE_HOUSE_KH).ToList();
                    foreach (var item in lstNhomKhoKH)
                    {
                        switch (item.TRANG_THAI)
                        {
                            case "ACTIVE":
                                item.TRANG_THAI = "Hoạt động";
                                break;
                            case "CLOSED":
                                item.TRANG_THAI = "Đóng cửa";
                                break;
                        }
                        switch (item.LOAI_WARE_HOUSE)
                        {
                            case "OWNER":
                                item.LOAI_WARE_HOUSE = "Owner";
                                break;
                            case "NCC":
                                item.LOAI_WARE_HOUSE = "Nhà cung cấp";
                                break;
                            case "KH":
                                item.LOAI_WARE_HOUSE = "Khách hàng";
                                break;
                        }
                    }
                    Session["NhomKho"] = lstNhomKho;
                    Session["NhomKhoKH"] = lstNhomKhoKH;
                }
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public static bool isTaoMaKhoKhachHang;
        public ActionResult GetAutonumKhoHang(string maNhomKho)
        {
            JMessage mess = new JMessage();
            try
            {
                List<WH_V3_WARE_HOUSEDTO> lstKhoHang = Session["NhomKho"] as List<WH_V3_WARE_HOUSEDTO>;
                var lstOwner = lstKhoHang.Where(u => u.LOAI_WARE_HOUSE == WARE_HOUSE_OWNER).ToList();
                if (lstOwner.Count > 0)
                {
                    isTaoMaKhoKhachHang = true;
                }
                else
                {
                    isTaoMaKhoKhachHang = false;
                }
                WH_V3_WARE_HOUSEDTO dto = new WH_V3_WARE_HOUSEDTO();
                dto.MA_WARE_HOUSE = CommonData.GetAutoNum("WHVEC", 1)[0];
                List<WH_V3_WARE_HOUSEDTO> lst = new List<WH_V3_WARE_HOUSEDTO>() { dto };
                Session["MaNhomKho"] = maNhomKho;
                mess.Error = false;
                mess.Object = lst;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }
        public void layLaiThongTinWH()
        {
            CSS_CRM_NGUOI_DUNGDTO userRes = Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            List<WH_V3_WARE_HOUSEDTO> lstWH = CommonData.GetKhoHang(userRes.CUSTOMER_CODE);
            List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> lstKHNCC = CommonData.GetKHNCC(userRes.CUSTOMER_CODE);
            if (lstWH.Count > 0)
            {
                userRes.MA_WARE_HOUSE_OWNER = lstWH.Where(u => u.LOAI_WARE_HOUSE == "OWNER").Select(u => u.MA_WARE_HOUSE).ToList()[0];
                userRes.MA_WARE_HOUSE_NCC = lstWH.Where(u => u.LOAI_WARE_HOUSE == "NCC").Select(u => u.MA_WARE_HOUSE).ToList()[0];
                userRes.TEN_WARE_HOUSE_OWNER = lstWH.Where(u => u.LOAI_WARE_HOUSE == "OWNER").Select(u => u.TEN_WARE_HOUSE).ToList()[0];
                userRes.TEN_WARE_HOUSE_NCC = lstWH.Where(u => u.LOAI_WARE_HOUSE == "NCC").Select(u => u.TEN_WARE_HOUSE).ToList()[0];
            }
            if (lstKHNCC.Count > 0)
            {
                userRes.MA_KHNCC_OWNER = lstKHNCC.Where(u => u.MA_LOAI_DOI_TUONG == "OWNER").Select(u => u.MA_DOI_TUONG).ToList()[0];
                userRes.MA_KHNCC_NCC = lstKHNCC.Where(u => u.MA_LOAI_DOI_TUONG == "NCC").Select(u => u.MA_DOI_TUONG).ToList()[0];
                userRes.TEN_KHNCC_OWNER = lstKHNCC.Where(u => u.MA_LOAI_DOI_TUONG == "OWNER").Select(u => u.TEN_DOI_TUONG).ToList()[0];
                userRes.TEN_KHNCC_NCC = lstKHNCC.Where(u => u.MA_LOAI_DOI_TUONG == "NCC").Select(u => u.TEN_DOI_TUONG).ToList()[0];
            }
            Session["User"] = userRes;
        }
        public ActionResult AddKhoHang(string maKhoHang, string tenKhoHang, string maTinh, string maHuyen, string maXa, string diaChiSoNha, string soDienThoai, string trangThai, string maThamChieu, string maNuoc)
        {
            Message mess = new Message();
            try
            {
                Dictionary<string, string> dicWH = new Dictionary<string, string>();
                if (isTaoMaKhoKhachHang)
                {
                    dicWH = new Dictionary<string, string>()
                    {
                        { "KH","Khách hàng" }
                    };
                }
                else
                {
                    dicWH = new Dictionary<string, string>()
                    {
                        { "OWNER","Owner" },
                        { "NCC","Nhà cung cấp" }
                    };
                }
                List<WH_V3_WARE_HOUSEDTO> lstWH = new List<WH_V3_WARE_HOUSEDTO>();
                foreach (var wh in dicWH.Keys)
                {
                    WH_V3_WARE_HOUSEDTO dto = new WH_V3_WARE_HOUSEDTO();
                    dto.MA_NHOM_KHO = ClientSession.getCusCode();
                    dto.TEN_NHOM_KHO = ClientSession.getCusName();

                    dto.LOAI_WARE_HOUSE = wh;
                    switch (wh)
                    {
                        case "OWNER":
                            dto.MA_WARE_HOUSE = maKhoHang;
                            dto.TEN_WARE_HOUSE = tenKhoHang;
                            dto.TEN_LOAI_WARE_HOUSE = "Owner";
                            break;
                        case "NCC":
                            dto.MA_WARE_HOUSE = CommonData.GetAutoNum("WHVEC", 1)[0];
                            dto.TEN_WARE_HOUSE = "Kho nhà cung cấp " + dto.MA_NHOM_KHO;
                            dto.TEN_LOAI_WARE_HOUSE = "Nhà cung cấp";
                            break;
                        case "KH":
                            dto.MA_WARE_HOUSE = maKhoHang;
                            dto.TEN_WARE_HOUSE = tenKhoHang;
                            dto.TEN_LOAI_WARE_HOUSE = "Khách hàng";
                            break;
                    }
                    dto.TRANG_THAI = trangThai;
                    dto.MA_THAM_CHIEU = maThamChieu;
                    dto.MA_TINH = maTinh;
                    dto.MA_HUYEN = maHuyen;
                    dto.MA_XA = maXa;
                    dto.MA_NUOC = maNuoc;
                    dto.DIA_CHI_SO_NHA = diaChiSoNha;
                    dto.SO_DIEN_THOAI = soDienThoai;
                    dto.GHI_CHU = "WEB";
                    dto.INSERT_USER = ClientSession.getCusCode();
                    dto.UPDATE_USER = ClientSession.getCusCode();
                    lstWH.Add(dto);
                }

                var rp = Connection.CallService("WH_WEB_LUU_KHO_HANG", lstWH);
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                layLaiThongTinWH();
                mess.isSuccess = true;
                mess.Mess = Booking.WH_themThanhCong;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult GLUDanhSachNCCPartial()
        {
            string maNhomKho = Session["MaNhomKho"] as string;
            if (string.IsNullOrWhiteSpace(maNhomKho))
            {
                maNhomKho = ClientSession.getCusCode();
            }
            WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO dto = new WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO() { MA_NHOM_KHO = maNhomKho, TRANG_THAI = "ACTIVE", MA_LOAI_DOI_TUONG = "OWNER" };
            var rp = Connection.CallService("WH_WEB_SEARCH_NHA_CUNG_CAP", new List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> { dto });
            if (rp.IsError)
            {
                throw new Exception(rp.Description);
            }
            List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> lstNCC = JsonConvert.DeserializeObject<List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO>>(rp.ResponseData);
            Session["DanhSachNCC"] = lstNCC;
            return PartialView("GLUDanhSachNCCPartial", lstNCC);
        }
        public ActionResult GLUTinh()
        {
            List<OP_ZONE_MASTERDTO> lstTinh = new List<OP_ZONE_MASTERDTO>();
            lstTinh = _listZone.Where(u => u.TEN_CO_DAU != "").ToList();
            Session["TINH"] = lstTinh;
            return PartialView("GLUTinh", lstTinh);
        }
        public ActionResult GLUHuyen()
        {
            List<CSS_OP_DISTRICTDTO> lstHuyen = Session["HUYEN"] as List<CSS_OP_DISTRICTDTO>;
            return PartialView("GLUHuyen", lstHuyen);
        }
        public ActionResult GLUXa()
        {
            List<CSS_OP_COMMUNEDTO> lstXa = Session["XA"] as List<CSS_OP_COMMUNEDTO>;
            if (lstXa == null)
            {
                lstXa = new List<CSS_OP_COMMUNEDTO>();
            }
            return PartialView("GLUXa", lstXa);
        }
        public ActionResult changeTinh(string maTinh)
        {
            Message mess = new Message();
            Session["HUYEN"] = null;
            try
            {
                List<CSS_OP_DISTRICTDTO> lstHuyen = _listDistrict.Where(u => u.ZONE_CODE == maTinh).ToList();
                Session["HUYEN"] = lstHuyen;
                mess.isError = false;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult changeHuyen(string maHuyen)
        {
            Message mess = new Message();
            Session["XA"] = null;
            try
            {
                List<CSS_OP_COMMUNEDTO> lstXa = _listCommune.Where(u => u.DISTRICT_CODE == maHuyen).ToList();
                Session["XA"] = lstXa;
                mess.isError = false;
            }
            catch (Exception ex)
            {
                mess.isError = true;
                mess.Mess = ex.Message;
            }
            return Json(mess);
        }
        public ActionResult TaoDon(string maXK)
        {
            JMessage mes = new JMessage();
            try
            {
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    mes.Error = true;
                    mes.Title = Booking.WH_vuiLongChonDongThaoTac;
                    return Json(mes);
                }
                if (string.IsNullOrWhiteSpace(maXK))
                {
                    return Json(mes);
                }
                List<WH_V3_XUAT_KHODTO> lstXuatKho = Session["QuanLyXuatKho"] as List<WH_V3_XUAT_KHODTO>;
                WH_V3_XUAT_KHODTO dtoXK = lstXuatKho.Where(u => u.MA_XUAT_KHO == maXK).ToList()[0];
                dtoXK.UPDATE_USER = ClientSession.getCusCode();
                dtoXK.UPDATE_DATE = dtoXK.UPDATE_TIME = DateTime.Now;
                if (dtoXK.TRANG_THAI != "DA_XAC_NHAN_XUAT_KHO")
                {
                    mes.Error = true;
                    mes.Title = Booking.WH_chiDuocThaoTacYCMoiLap;
                    return Json(mes);
                }
                List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstXuatKhoChiTiet = Session["ChiTietXuatKho"] as List<WH_V3_XUAT_KHO_CHI_TIETDTO>;

                List<CSS_OP_BOOKING_SGEV_WEB> lstDonTuSinh = new List<CSS_OP_BOOKING_SGEV_WEB>();
                CSS_OP_BOOKING_SGEV_WEB dto = new CSS_OP_BOOKING_SGEV_WEB();
                dto.MA_BPBK = maXK;
                dto.MA_KHACH_HANG = ClientSession.getCusCode();
                dto.TEN_KHACH_HANG = dtoXK.TEN_KHACH_HANG;
                dto.TEN_NGUOI_NHAN = dtoXK.TEN_KHACH_HANG;
                dto.MA_TINH_NHAN = dtoXK.MA_TINH;
                dto.MA_HUYEN_NHAN = dtoXK.MA_HUYEN;
                dto.TINH_NHAN = dtoXK.TEN_TINH;
                dto.HUYEN_NHAN = dtoXK.TEN_HUYEN;
                dto.TEN_NGUOI_GUI = dtoXK.TEN_NHAN_VIEN;
                dto.DIA_CHI = dtoXK.DIA_CHI_SO_NHA;
                if (!string.IsNullOrWhiteSpace(dtoXK.TEN_HUYEN))
                {
                    dto.DIA_CHI += ", " + dtoXK.TEN_HUYEN;
                }
                if (!string.IsNullOrWhiteSpace(dtoXK.TEN_TINH))
                {
                    dto.DIA_CHI += ", " + dtoXK.TEN_TINH;
                }
                dto.SDT_NHAN = dtoXK.SO_DIEN_THOAI;

                dto.INSERT_USER = ClientSession.getCusCode();
                dto.UPDATE_USER = ClientSession.getCusCode();
                foreach (var item in lstXuatKhoChiTiet)
                {
                    dto.NOI_DUNG += Environment.NewLine + item.TEN_SAN_PHAM;
                }
                dto.TT_BO_SUNG = "{" + string.Format("{0:n0}", lstXuatKhoChiTiet.Sum(u => u.SO_LUONG)) + "}";
                lstDonTuSinh.Add(dto);
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(lstDonTuSinh);
                var rp = Connection.CallService("WH_WEB_CHUYEN_DON_SANG_NHA_VAN_CHUYEN", lstDonTuSinh);
                if (rp.IsError)
                {
                    mes.Error = true;
                    mes.Title = rp.Description;
                }
                else
                {
                    mes.Error = false;
                    mes.Title = Booking.WH_themThanhCong;
                }
            }
            catch (Exception ex)
            {
                mes.Error = true;
                mes.Title = ex.Message;
            }
            return Json(mes);
        }
        public ActionResult GetThongKhoKH(string maKhoKH)
        {
            JMessage mess = new JMessage();
            try
            {
                List<TINH> lstTinh1 = Session["TINH"] as List<TINH>;
                List<WH_V3_WARE_HOUSEDTO> lstKhoKH = Session["KhoKhachHang"] as List<WH_V3_WARE_HOUSEDTO>;
                List<WH_V3_WARE_HOUSEDTO> objectKhoKH = lstKhoKH.Where(u => u.MA_WARE_HOUSE == maKhoKH).ToList();
                List<OP_ZONE_MASTERDTO> lstTinh = new List<OP_ZONE_MASTERDTO>()
                {
                    new OP_ZONE_MASTERDTO()
                    {
                        ZONE_CODE = objectKhoKH[0].MA_TINH,
                        ZONE_NAME = objectKhoKH[0].TEN_TINH
                    }
                };
                List<CSS_OP_DISTRICTDTO> lstHuyen = new List<CSS_OP_DISTRICTDTO>()
                {
                    new CSS_OP_DISTRICTDTO()
                    {
                        DISTRICT_CODE = objectKhoKH[0].MA_HUYEN,
                        DISTRICT_NAME = objectKhoKH[0].TEN_HUYEN
                    }
                };
                mess.Error = false;
                mess.Object = objectKhoKH;
            }
            catch (Exception ex)
            {
                mess.Error = true;
                mess.Title = ex.Message;
            }
            return Json(mess);
        }

        public ActionResult InPhieuNK(string maNK)
        {
            try
            {
                var model = Session["QuanLyNhapKho"] as List<WH_V3_NHAP_KHODTO>;
                WH_V3_NHAP_KHODTO dtoNK = model.Where(u => u.MA_NHAP_KHO == maNK).ToList()[0];
                WH_V3_NHAP_KHO_CHI_TIETDTO dto = new WH_V3_NHAP_KHO_CHI_TIETDTO() { MA_NHAP_KHO = maNK };
                var rp = Connection.CallService("WH_WEB_SEARCH_NHAP_KHO_CHI_TIET", new List<WH_V3_NHAP_KHO_CHI_TIETDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<WH_V3_NHAP_KHO_CHI_TIETDTO>>(rp.ResponseData);
                var dicNKCT = lstReturn.GroupBy(u => u.MA_SAN_PHAM).ToDictionary(u => u.Key, x => x.ToList());
                foreach (var item in lstReturn)
                {
                    if (dicNKCT.ContainsKey(item.MA_SAN_PHAM))
                    {
                        item.SO_LUONG_DON_VI_TINH_YEU_CAU = dicNKCT[item.MA_SAN_PHAM].Sum(u => u.SO_LUONG);
                        item.SO_LUONG_CHENH_LECH = item.SO_LUONG_DON_VI_TINH_YEU_CAU - item.SO_LUONG;
                    }
                }

                rptMauInNhapKho rpt = new rptMauInNhapKho();
                rpt.SetData(dtoNK, lstReturn);
                using (MemoryStream ms = new MemoryStream())
                {
                    rpt.ExportToPdf(ms, new PdfExportOptions());
                    return File(ms.ToArray(), "application/pdf");
                }
            }
            catch (Exception ex)
            {
                ViewBag.UploadError = ex.Message;
                return View("QuanLyNhapKho");
            }
        }

        public ActionResult InPhieuXK(string maXK)
        {
            try
            {
                var model = Session["QuanLyXuatKho"] as List<WH_V3_XUAT_KHODTO>;
                WH_V3_XUAT_KHODTO dtoXK = model.Where(u => u.MA_XUAT_KHO == maXK).ToList()[0];
                WH_V3_XUAT_KHO_CHI_TIETDTO dto = new WH_V3_XUAT_KHO_CHI_TIETDTO() { MA_XUAT_KHO = maXK };
                var rp = Connection.CallService("WH_WEB_SEARCH_XUAT_KHO_CHI_TIET", new List<WH_V3_XUAT_KHO_CHI_TIETDTO> { dto });
                if (rp.IsError)
                {
                    throw new Exception(rp.Description);
                }
                var lstReturn = JsonConvert.DeserializeObject<List<WH_V3_XUAT_KHO_CHI_TIETDTO>>(rp.ResponseData);

                rptMauInXuatKho rpt = new rptMauInXuatKho();
                rpt.SetData(dtoXK, lstReturn);
                using (MemoryStream ms = new MemoryStream())
                {
                    rpt.ExportToPdf(ms, new PdfExportOptions());
                    return File(ms.ToArray(), "application/pdf");
                }
            }
            catch (Exception ex)
            {
                ViewBag.UploadError = ex.Message;
                return View("QuanLyXuatKho");
            }
        }
    }
}