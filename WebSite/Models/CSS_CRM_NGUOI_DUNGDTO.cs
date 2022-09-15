using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models.Warehouse;

namespace WebSite.Models
{
    public class CSS_CRM_NGUOI_DUNGDTO
    {
        public int ID { get; set; }
        public String CUSTOMER_CODE { get; set; }
        public String USER_NAME { get; set; }
        public String PASSWORD { get; set; }
        public String GHI_CHU { get; set; }
        public String INSERT_USER { get; set; }
        public DateTime INSERT_DATE { get; set; }
        public DateTime INSERT_TIME { get; set; }
        public String UPDATE_USER { get; set; }
        public DateTime UPDATE_DATE { get; set; }
        public DateTime UPDATE_TIME {  get;set; }
        public string LOAI_TK { get; set; }
        public string DANH_SACH_KH { get; set; }
        public string NAME { get; set; }
        public string login { get; set; }
        public string MA_KHACH_HANG_CON { get; set; }
        public string FULL_NAME { get; set; }
        public string STATION_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string EMAIL { get; set; }
        public string MA_WARE_HOUSE_OWNER { get; set; }
        public string MA_WARE_HOUSE_NCC { get; set; }
        public string MA_WARE_HOUSE_KH { get; set; }
        public string TEN_WARE_HOUSE_OWNER { get; set; }
        public string TEN_WARE_HOUSE_NCC { get; set; }
        public string TEN_WARE_HOUSE_KH { get; set; }
        public string MA_KHNCC_OWNER { get; set; }
        public string MA_KHNCC_NCC { get; set; }
        public string MA_KHNCC_KH { get; set; }
        public string TEN_KHNCC_OWNER { get; set; }
        public string TEN_KHNCC_NCC { get; set; }
        public string TEN_KHNCC_KH { get; set; }
        public List<CSS_DANH_SACH_MENUDTO> lstMenu { get; set; }
        public string TEMP { get; set; }
        public string PARENT_USER_NAME { get; set; }
        public int IS_ACTIVE { get; set; }
        public string LOAI_USER { get; set; }
        public Boolean DUOC_CHAT { get; set; }
        public string RePass { get; set; }
        public string isSudungMDS { get { return isSudungMDSThayMaBill(); } }
        public string isInNgayLayHang { get { return IsInNgayLayHang(); } }
        
        private string isSudungMDSThayMaBill()
        {
            var kieuin = CommonData.splitCommon(LOAI_TK);
            foreach (var item in kieuin)
            {
                if (item.Value == "SU_DUNG_MDS")
                {
                    return item.Text.Replace("T", "t").Replace("F", "f");
                }
            }
            return "false";
        }
        private string IsInNgayLayHang()
        {
            var kieuin = CommonData.splitCommon(LOAI_TK);
            foreach (var item in kieuin)
            {
                if (item.Value == "IN_NGAY_LAY_HANG")
                {
                    return item.Text.Replace("T", "t").Replace("F", "f");
                }
            }
            return "false";
        }
        public string getKieuIn()
        {
            var kieuin = CommonData.splitCommon(LOAI_TK);
            foreach(var item in kieuin)
            {
                if (item.Value == "P")
                {
                    return item.Text;
                }
            }
            return "A4";
        }
        public string updateLoaiTK(string value , string text)
        {
            var kieuin = CommonData.splitCommon(LOAI_TK);
            var isTimThay = false;
            foreach (var item in kieuin)
            {
                if(item.Value == value)
                {
                    item.Text = text;
                    isTimThay = true;
                }
            }
            if (!isTimThay)
            {
                kieuin.Add(new System.Web.Mvc.SelectListItem() { Value = value, Text= text });
            }
            string str = "";
            foreach(var item in kieuin)
            {
                if (string.IsNullOrEmpty(str))
                {
                    str += "{" + item.Value + ":" + item.Text + "}";
                }
                else
                {
                    str += ",{" + item.Value + ":" + item.Text + "}";
                }
                
            }

            return str;
        }
    }
}