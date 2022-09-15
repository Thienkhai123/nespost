using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using WebSite.Models;
using WebSite.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web.Mvc;
using WebSite.Filter;
using System.Web;

namespace WebSite.Models
{
    public class CommonData
    {
        public static List<OP_ZONE_MASTERDTO> _listZone;
        public static List<CSS_OP_DISTRICTDTO> _listDistrict;
        public static List<CSS_OP_COMMUNEDTO> _listCommune;

        public static string URL_TRANG_CHU = "https://netpost.vn/Image/";

        // mongo ảnh
        private static ImageFormat getImageFormat(string in_ext)
        {
            ImageFormat if_ret = new ImageFormat(Guid.NewGuid());
            switch (in_ext.ToLower())
            {
                case "bmp":
                    if_ret = ImageFormat.Bmp;
                    break;
                case "gif":
                    if_ret = ImageFormat.Gif;
                    break;
                case "jpg":
                    if_ret = ImageFormat.Jpeg;
                    break;
                case "ico":
                    if_ret = ImageFormat.Icon;
                    break;
                case "png":
                    if_ret = ImageFormat.Png;
                    break;
                case "jpeg":
                    if_ret = ImageFormat.Jpeg;
                    break;
                case "tif":
                    if_ret = ImageFormat.Tiff;
                    break;
            }
            return if_ret;
        }

        public static string SaveImagetoMongoDB(Image image, string fileName)
        {
            var database = GetMongoDataBase("SGV_IMAGE");
            using (var ms = new MemoryStream())
            {
                image.Save(ms, getImageFormat(Path.GetExtension(fileName).Replace(".", "")));
                ms.Position = 0;
                var gridFsInfo = database.GridFS.Upload(ms, fileName);
                var fileId = gridFsInfo.Id;
                return fileId.ToString();
            }
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            Bitmap orig = new Bitmap(image, width, height);

            Bitmap clone = new Bitmap(orig.Width, orig.Height,
            System.Drawing.Imaging.PixelFormat.Format16bppRgb555);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }

        public static MongoDatabase GetMongoDataBase(string dbName)
        {
            string connectMongoString = string.Format(@"mongodb://hawb:saveHawb@118.70.180.205:6872/{0}", dbName);
            MongoClient client = new MongoClient(connectMongoString);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(dbName);
            return db;
        }

        public static string GetBase64FromImage(Stream mStream)
        {
            string base64string = "";
            using (Image img = Image.FromStream(mStream))
            {
                using (Bitmap b = new Bitmap(img, new Size(920, 600)))
                {
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        b.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
                        var imageBytes = ms2.ToArray();
                        base64string = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                    }
                }
            }
            return base64string;
        }

        public static string GetImageFromMongoDB(string ImageID)
        {
            try
            {
                var database = GetMongoDataBase("SGV_IMAGE");
                var file = database.GridFS.FindOne(Query.EQ("_id", new ObjectId(ImageID)));
                using (var stream = file.OpenRead())
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    base64String = "data:image/" + getImageFormat(Path.GetExtension(file.Name).Replace(".", "")) + ";base64," + base64String;
                    return base64String;
                }
            }
            catch
            {
                return "";
            }
        }

        //lấy tỉnh, huyện
        public static List<string> lstTinh = new List<string> { "72", "70", "10" };

        public static void GetAllTinhHuyen()
        {
            if (_listZone == null || _listDistrict == null)
            {
                var response = Connection.CallService("LIST_ALL_TINH_HUYEN");
                if (!response.IsError)
                {
                    _listZone = JsonConvert.DeserializeObject<List<OP_ZONE_MASTERDTO>>(response.ResponseData);
                    _listZone = _listZone.OrderBy(x => x.TEN_CO_DAU).ToList();
                    foreach (var item in lstTinh)
                    {
                        var tinh = _listZone.FirstOrDefault(x => x.ZONE_CODE == item);
                        _listZone.Remove(tinh);
                        _listZone.Insert(0, tinh);
                    }

                    _listDistrict = JsonConvert.DeserializeObject<List<CSS_OP_DISTRICTDTO>>(response.ResponseData_EX);
                }
            }
        }

        // không được truyền dữ liệu vào những hàm này trách thay đổi dữ liệu
        public static List<OP_ZONE_MASTERDTO> GetZone()
        {
            if (_listZone == null)
            {
                var response = Connection.CallService("TIM_KIEM_ZONE");
                _listZone = JsonConvert.DeserializeObject<List<OP_ZONE_MASTERDTO>>(response.ResponseData);
                _listZone = _listZone.Where(x => !string.IsNullOrWhiteSpace(x.TEN_CO_DAU)).ToList();
                foreach (var item in lstTinh)
                {
                    var tinh = _listZone.FirstOrDefault(x => x.ZONE_CODE == item);
                    _listZone.Remove(tinh);
                    _listZone.Insert(0, tinh);
                }
            }
            return _listZone;
        }

        public static List<CSS_OP_DISTRICTDTO> GetDistrict()
        {
            if (_listDistrict == null)
            {
                var response = Connection.CallService("TIM_KIEM_DISTRICT", new List<CSS_OP_DISTRICTDTO>());
                _listDistrict = JsonConvert.DeserializeObject<List<CSS_OP_DISTRICTDTO>>(response.ResponseData);
            }
            return _listDistrict;
        }

        public static List<CSS_OP_COMMUNEDTO> GetCommune()
        {
            if (_listCommune == null)
            {
                var response = Connection.CallService("TIM_KIEM_COMMUNE", new List<CSS_OP_COMMUNEDTO>());
                _listCommune = JsonConvert.DeserializeObject<List<CSS_OP_COMMUNEDTO>>(response.ResponseData);
            }
            return _listCommune;
        }

        public static List<CSS_OP_DISTRICTDTO> GetDistrict(string zone_code)
        {
            List<CSS_OP_DISTRICTDTO> listDistrict = new List<CSS_OP_DISTRICTDTO>();
            if (string.IsNullOrWhiteSpace(zone_code))
                return listDistrict;

            if (_listDistrict == null)
            {
                var response = Connection.CallService("TIM_KIEM_DISTRICT_EX", new List<CSS_OP_DISTRICTDTO> { new CSS_OP_DISTRICTDTO { ZONE_CODE = zone_code } });
                listDistrict = JsonConvert.DeserializeObject<List<CSS_OP_DISTRICTDTO>>(response.ResponseData);
            }
            else
            {
                listDistrict = _listDistrict.Where(x => x.ZONE_CODE == zone_code).ToList();
            }
            return listDistrict;
        }

        public static string GetZone(string zone_name)
        {
            string maTinh = "";
            List<OP_ZONE_MASTERDTO> listZone = new List<OP_ZONE_MASTERDTO>();
            if (string.IsNullOrWhiteSpace(zone_name))
                return "";

            if (_listZone != null)
            {
                var a = zone_name;

                string zoneNameTemp = "";
                foreach (var b in zone_name)
                {
                    string ch = RemoveUnicode(b.ToString());

                    if (char.IsLetter(b))
                    {
                        zoneNameTemp += ch;
                    }//Laf chu cai thi cong vao
                }

                maTinh = LayMaTinhTuten(zoneNameTemp);
            }

            return maTinh;
        }

        public static string GetDistrict(string zone_code, string district_name)
        {
            List<CSS_OP_DISTRICTDTO> listDistrict = new List<CSS_OP_DISTRICTDTO>();
            if (string.IsNullOrWhiteSpace(district_name))
                return "";

            if (_listDistrict != null)
            {
                listDistrict = _listDistrict.Where(x => x.ZONE_CODE == zone_code && x.DISTRICT_NAME.ToUpper() == district_name.ToUpper()).ToList();
            }

            return listDistrict.Count > 0 ? listDistrict[0].DISTRICT_CODE : "";
        }

        public static string LayMaMienTheoTinhNhan(string ZONE_CODE)
        {
            List<OP_ZONE_MASTERDTO> listZone = new List<OP_ZONE_MASTERDTO>();
            if (string.IsNullOrWhiteSpace(ZONE_CODE))
                return "";

            if (_listZone != null)
            {
                listZone = _listZone.Where(x => x.ZONE_CODE == ZONE_CODE).ToList();
            }

            if (listZone.Count > 0)
            {
                var mien = CommonData.lstMien().FirstOrDefault(x => x.Value == listZone[0].MA_MIEN);
                return mien != null ? mien.Text : "";
            }

            return "";
        }

        public static List<SelectListItem> lstDichVu(bool isGetAll = false)
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "CPN", Text = "Chuyển phát nhanh" });
            lst.Add(new SelectListItem { Value = "EXP", Text = "Hàng giá trị cao, Express" });
            lst.Add(new SelectListItem { Value = "PHT", Text = "Phát theo YC, Hỏa tốc" });
            lst.Add(new SelectListItem { Value = "PTK", Text = "Tiết kiệm, 48h" });
            lst.Add(new SelectListItem { Value = "CPT", Text = "Chuyển thường/ đường bộ" });

            if (isGetAll)
            {
                lst.Add(new SelectListItem { Value = "ALL", Text = "Tất cả" });
            }

            return lst;
        }
        public static List<SelectListItem> lstLoaiHang()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "", Text = "" });
            lst.Add(new SelectListItem { Value = "DOX", Text = "Tài liệu" });
            lst.Add(new SelectListItem { Value = "SPX", Text = "Hàng hóa" });
            return lst;
        }
        public static List<string> lstHTTT()
        {
            return new List<string> { "Thanh toán cuối tháng", "Thanh toán đầu nhận" };
        }
        public static string getDichVuTuMa(string ma)
        {
            var dichvu = lstDichVu();
            var dv = dichvu.FirstOrDefault(u=>u.Value == ma.Trim().ToUpper());
            if (dv != null)
            {
                return dv.Text;
            }
            return "";
        }
        public static List<SelectListItem> lstKichThuoc()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "KT_LON", Text = "Kích thước lớn(Xe tải v/c)" });
            lst.Add(new SelectListItem { Value = "KT_NHO", Text = "Kích thước nhỏ(Xe máy v/c)" });
            return lst;
        }

        public static List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> GetKHNCC(string maNhomKho)
        {
            var response = Connection.CallService("WH_WEB_SEARCH_NHA_CUNG_CAP", new List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> { new WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO() { MA_NHOM_KHO = maNhomKho } });
            List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO> listKHNCC = JsonConvert.DeserializeObject<List<WH_V3_KHACH_HANG_NHA_CUNG_CAPDTO>>(response.ResponseData);
            return listKHNCC;
        }

        public static List<WH_V3_WARE_HOUSEDTO> GetKhoHang(string maNhomKho)
        {
            var response = Connection.CallService("WH_WEB_TIM_KIEM_KHO_HANG", new List<WH_V3_WARE_HOUSEDTO> { new WH_V3_WARE_HOUSEDTO() { MA_NHOM_KHO = maNhomKho } });
            List<WH_V3_WARE_HOUSEDTO> listKhoHang = JsonConvert.DeserializeObject<List<WH_V3_WARE_HOUSEDTO>>(response.ResponseData);
            return listKhoHang;
        }

        public static List<STATION> GetSTATIONs(string company_code = "")
        {
            var response = Connection.CallService("TIM_KIEM_DON_VI", new List<STATION> { new STATION { COMPANY_STATION = company_code } });
            List<STATION> _listStation = JsonConvert.DeserializeObject<List<STATION>>(response.ResponseData);
            return _listStation;
        }

        public static List<string> GetAutoNum(string MaDonHang, int count)
        {
            List<string> autonum = null;
            string AutoNum = MaDonHang + "," + count;
            BaseRequest req = new BaseRequest();
            req.RequestType = "AUTONUM";
            req.RequestData = AutoNum;
            BaseResponseAndroid rp = Connection.CallService(req);
            if (rp.Status == "OK")
            {
                autonum = JsonConvert.DeserializeObject<List<string>>(rp.ResponseData);
            }
            return autonum;
        }

        public static string RemoveUnicode(string text, bool spec = false)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
            "đ",
            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
            "í","ì","ỉ","ĩ","ị",
            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
            "ý","ỳ","ỷ","ỹ","ỵ",};
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};

            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            if (spec)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!char.IsLetter(text[i]) && !char.IsDigit(text[i]))
                    {
                        text = text.Remove(i, 1);
                    }
                    if (text[i] == '&')
                    {
                        text = text.Remove(i, 1);
                    }
                    if (text[i] == ' ')
                    {
                        text = text.Remove(i, 1);
                    }
                }
            }
            return text.ToUpper().Trim();
        }

        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string encrypt(string ToEncrypt)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(ToEncrypt));
        }

        public static string decrypt(string cypherString)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(cypherString));
        }

        public static bool CheckEmail(string email)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }

        public static bool IsValidString(string value)
        {
            var dem = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (Char.IsLetter(c))
                {
                    dem++;
                }
                if (dem >= 5)
                {
                    return true;
                }
            }
            return false;
        }

        public static DateTime GetFirsDayOfMonth(DateTime date)
        {
            DateTime firsDate = new DateTime(date.Year, date.Month, 1);
            return firsDate;
        }

        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        public static List<SelectListItem> splitService(string service, string value = "")
        {
            List<SelectListItem> lstRt = new List<SelectListItem>();
            if (string.IsNullOrWhiteSpace(service))
            {
                return lstRt;
            }

            var listSplit = service.Split(new string[] { ",{" }, StringSplitOptions.None);
            for (int i = 0; i < listSplit.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(listSplit[i])) continue;
                SelectListItem dto = new SelectListItem();
                listSplit[i] = listSplit[i].Replace("{", "");
                listSplit[i] = listSplit[i].Replace("}", "");
                var listSplitChiTiet = listSplit[i].Split(':');
                if (string.IsNullOrWhiteSpace(listSplitChiTiet[0]))
                    continue;
                if (listSplitChiTiet.Count() == 1)
                {
                    dto.Value = "Số kiện";
                    dto.Text = listSplitChiTiet[0].Trim();
                }
                else
                {
                    dto.Value = listSplitChiTiet[0].Trim();
                    dto.Text = listSplitChiTiet[1].Trim();
                }

                lstRt.Add(dto);
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                lstRt = lstRt.Where(x => x.Value == value).ToList();
            }
            return lstRt;
        }
        public static List<SelectListItem> splitCommon(string service, string value = "")
        {
            List<SelectListItem> lstRt = new List<SelectListItem>();
            if (string.IsNullOrWhiteSpace(service))
            {
                return lstRt;
            }

            var listSplit = service.Split(new string[] { ",{" }, StringSplitOptions.None);
            for (int i = 0; i < listSplit.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(listSplit[i])) continue;
                SelectListItem dto = new SelectListItem();
                listSplit[i] = listSplit[i].Replace("{", "");
                listSplit[i] = listSplit[i].Replace("}", "");
                var listSplitChiTiet = listSplit[i].Split(':');
                if (string.IsNullOrWhiteSpace(listSplitChiTiet[0]))
                    continue;
              
                dto.Value = listSplitChiTiet[0].Trim();
                dto.Text = listSplitChiTiet[1].Trim();

                lstRt.Add(dto);
            }
            if (!string.IsNullOrWhiteSpace(value))
            {
                lstRt = lstRt.Where(x => x.Value == value).ToList();
            }
            return lstRt;
        }
        public static void splitService(CSS_OP_BOOKING_SGEV_WEB_EX dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.TT_BO_SUNG))
            {
                string service = dto.TT_BO_SUNG;
                var listSplit = service.Split(new string[] { ",{" }, StringSplitOptions.None);
                for (int i = 0; i < listSplit.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(listSplit[i])) continue;

                    listSplit[i] = listSplit[i].Replace("{", "");
                    listSplit[i] = listSplit[i].Replace("}", "");
                    var listSplitChiTiet = listSplit[i].Split(':');

                    if (string.IsNullOrWhiteSpace(listSplitChiTiet[0]))
                        continue;

                    if (listSplitChiTiet[0].Trim().Equals("Số kiện"))
                    {
                        dto.SO_KIEN = listSplitChiTiet[1].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("Số hóa đơn"))
                    {
                        dto.SO_HOA_DON = listSplitChiTiet[1].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("GTDH"))
                    {
                        dto.GT_DON_HANG = listSplitChiTiet[1].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("Hoàn BBBG"))
                    {
                        dto.HOAN_BBBG = listSplitChiTiet[1].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("TG_PST"))
                    {
                        dto.NGAY_HEN_PHAT_SIEU_THI = listSplitChiTiet[1].Trim();
                        if (listSplitChiTiet.Count() == 3)
                            dto.NGAY_HEN_PHAT_SIEU_THI += ":" + listSplitChiTiet[2].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("Đồng kiểm"))
                    {
                        dto.DONG_KIEM = listSplitChiTiet[1].Trim();
                    }
                    if (listSplitChiTiet[0].Trim().Equals("HTTT"))
                    {
                        dto.HT_THANH_TOAN = listSplitChiTiet[1].Trim();
                    }
                }

            }
        }

        public class ROLE_WEBSITE_ADMIN
        {
            public static string SALE_PART = "SALE";
            public static string HR_PART = "NHAN_SU";
            public static string ADMIN_PART = "ADMIN";
        }

        public static List<SelectListItem> lstMien()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "MIEN_BAC", Text = "01" });
            lst.Add(new SelectListItem { Value = "MIEN_TRUNG", Text = "02" });
            lst.Add(new SelectListItem { Value = "MIEN_NAM", Text = "03" });
            lst.Add(new SelectListItem { Value = "MIEN_TAY", Text = "04" });

            return lst;
        }

        public static List<CSS_CRM_NGUOI_DUNGDTO> _lstCS = new List<CSS_CRM_NGUOI_DUNGDTO>();
        public static void getListCSByCompanyCode(bool isGet = false)
        {
            if (_lstCS.Count == 0 || isGet)
            {
                List<string> lstRole = new List<string> { "cs trang chủ" };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("GET_LIST_CS", JsonConvert.SerializeObject(lstRole));
                if (dtoRes.Status == "OK")
                {
                    _lstCS = JsonConvert.DeserializeObject<List<CSS_CRM_NGUOI_DUNGDTO>>(dtoRes.ResponseData);
                    foreach (var item in _lstCS)
                    {
                        if (item.STATION_CODE.StartsWith("59"))
                            item.COMPANY_CODE = "02";

                        item.FULL_NAME = item.USER_NAME + " - " + item.NAME;
                    }
                }
            }
        }


        private static Dictionary<string, string> dicTinh = null;
        private static Dictionary<string, Dictionary<string, string>> dicHuyen = null;
        private static Dictionary<string, string> GetDicTinh()
        {
            if (dicTinh != null)
            {
                return dicTinh;
            }
            dicTinh = new Dictionary<string, string>();
            var lstTinh = _listZone;
            foreach (var item in lstTinh)
            {
                var key = XoaBoAllKyTuDacBiet(item.ZONE_NAME);
                if (!dicTinh.ContainsKey(key))
                {
                    dicTinh.Add(key, item.ZONE_CODE);
                }
                if (item.TEN_VIET_TAT.Trim() != "")
                {
                    var lstTenVietTat = item.TEN_VIET_TAT.Split(',');
                    foreach (var a in lstTenVietTat)
                    {
                        if (!dicTinh.ContainsKey(a.Trim()))
                        {
                            dicTinh.Add(a.Trim(), item.ZONE_CODE);
                        }
                    }
                }

                string keyTenCoDau = XoaBoAllKyTuDacBiet(item.TEN_CO_DAU);

                if (!dicTinh.ContainsKey(keyTenCoDau.Trim()))
                {
                    dicTinh.Add(keyTenCoDau.Trim(), item.ZONE_CODE);
                }
            }
            return dicTinh;
        }

        private static string BoKiTuVietTat(string chuoikitu)
        {
            return chuoikitu = chuoikitu.Trim().ToUpper().Replace("TP", "").Replace("THUDO", "")
                .Replace("THANHPHO", "").Replace("TPHO", "")
                .Replace("QUAN", "").Replace("HUYEN", "").Replace("Q.", "").Replace("H.", "").Replace("XA", "").Replace("PHUONG", "");
        }
        private static string BoKiTuDacBiet(string chuoikitu)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in chuoikitu)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }

        public static string RemoveUnicodeAndSpace(string text)
        {
            text = RemoveUnicode(text).Replace(" ", "");
            return text;
        }
        private static string XoaBoAllKyTuDacBiet(string chuoiKiTu)
        {
            chuoiKiTu = chuoiKiTu.Trim().ToUpper();
            chuoiKiTu = RemoveUnicodeAndSpace(chuoiKiTu);
            chuoiKiTu = BoKiTuVietTat(chuoiKiTu);
            chuoiKiTu = BoKiTuDacBiet(chuoiKiTu);
            return chuoiKiTu;
        }
        public static string LayMaTinhTuten(string tenTinh)
        {
            try
            {

                if (tenTinh.Trim() == "") return "";
                tenTinh = XoaBoAllKyTuDacBiet(tenTinh);

                Dictionary<string, string> dicTinh = GetDicTinh();
                if (dicTinh.ContainsKey(tenTinh))
                {
                    return GetDicTinh()[tenTinh];
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }

    public class Account
    {
        public string USER_NAME { get; set; }
        public string Password { get; set; }
        public string USERID { get; set; }

    }

    public class IMAGES
    {
        public ObjectId _id { get; set; }
        public string LOAI { get; set; }
        public string MA_BPBK { get; set; }
        public string DATA_CODE_H2 { get; set; }
        public ObjectId _id_IMAGE { get; set; }
    }
}