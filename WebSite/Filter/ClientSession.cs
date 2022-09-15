using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using WebSite.Models;
using WebSite.Models.Warehouse;

namespace WebSite.Filter
{
    public class ClientSession
    {
        public static void LuuSessionWH(CSS_CRM_NGUOI_DUNGDTO userRes)
        {
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
        }

        public static CSS_CRM_NGUOI_DUNGDTO getCustomer()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user;
        }
        public static CSS_CRM_NGUOI_DUNGDTO getUser()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user;
        }
        public static string getCusCode()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user == null ? "" : user.CUSTOMER_CODE;
        }
        public static string getCusName()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user == null ? "" : user.NAME;
        }
        public static string getCompanyCode()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user==null?"": user.COMPANY_CODE;
        }
        public static string getUserName()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user == null ? "" : user.USER_NAME;
        }

        public static string getMaKHCon()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user == null ? "" : user.MA_KHACH_HANG_CON;
        }
        public static string getTenDayDuKHCon()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user == null ? "" : user.FULL_NAME;
        }

        public static int getSLMaKHCon()
        {
            List<CUSTOMER> lstMa = HttpContext.Current.Session["DANH_SACH_KH"] as List<CUSTOMER>;
            return lstMa.Count();
        }
        public static List<string> getLstMaKH()
        {
            List<CUSTOMER> lstMa = HttpContext.Current.Session["DANH_SACH_KH"] as List<CUSTOMER>;
            return lstMa.Select(x=>x.CUSTOMER_CODE).Distinct().ToList();
        }
        public static List<CUSTOMER> getDanhSachKH()
        {
            List<CUSTOMER> lstMa = HttpContext.Current.Session["DANH_SACH_KH"] as List<CUSTOMER>;
            return lstMa;
        }
        public static List<CUSTOMER> getFullLstMaKH()
        {
            List<CUSTOMER> lstMa = HttpContext.Current.Session["DANH_SACH_KH"] as List<CUSTOMER>;
            foreach (var item in lstMa)
            {
                item.FULL_NAME = "[" + item.LOCATION + "] " + item.CUSTOMER_CODE + " - " + item.NAME;
            }
            return lstMa;
        }
        public static List<CUSTOMER> getFullInForCustomer()
        {
            List<CUSTOMER> lstMa = HttpContext.Current.Session["DANH_SACH_KH"] as List<CUSTOMER>;
            foreach (var item in lstMa)
            {
                item.FULL_NAME = item.CUSTOMER_CODE + " - " + "[" + item.LOCATION + "] " + item.NAME + " - " + item.STATION_NAME;
            }
            return lstMa;
        }

        public static string getWH_OWNER()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.MA_WARE_HOUSE_OWNER;
        }
        public static string getWH_NCC()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.MA_WARE_HOUSE_NCC;
        }
        public static string getWH_OWNER_NAME()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.TEN_WARE_HOUSE_OWNER;
        }
        public static string getWH_NCC_NAME()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.TEN_WARE_HOUSE_NCC;
        }
        public static string getKHNCC_OWNER()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.MA_KHNCC_OWNER;
        }
        public static string getKHNCC_NCC()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.MA_KHNCC_NCC;
        }
        public static string getKHNCC_OWNER_NAME()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.TEN_KHNCC_OWNER;
        }
        public static string getKHNCC_NCC_NAME()
        {
            CSS_CRM_NGUOI_DUNGDTO user = HttpContext.Current.Session["User"] as CSS_CRM_NGUOI_DUNGDTO;
            return user.TEN_KHNCC_NCC;
        }
        public static string getLanguage()
        {
            var cookie = HttpContext.Current.Request.Cookies["Language"];
            return cookie != null ? cookie.Value : "";
        }
        public static bool getLanguage(string lag)
        {
            var cookie = HttpContext.Current.Request.Cookies["Language"];
            return cookie != null && cookie.Value.Equals(lag) ? true : false;
        }
    }
}
