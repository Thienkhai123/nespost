using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WebSite.Models;
using System.Collections.Generic;

namespace WebSite.Excel
{
    public partial class rptPhieuGuiSagawaTuSinhA5_VinID : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPhieuGuiSagawaTuSinhA5_VinID()
        {
            InitializeComponent();
        }
        public void SetData(List<CSS_OP_BOOKING_SGEV_WEB_EX> list)
        {
            foreach(var item in list)
            {
                if (!string.IsNullOrWhiteSpace(item.TT_BO_SUNG))
                {
                    var sokien = item.TT_BO_SUNG.Split('}')[0];
                    sokien = sokien.Replace("{", "").Replace("Số kiện: ","");
                    item.TT_BO_SUNG = sokien;
                }
                item.SO_COD_STRING =item.SO_COD==0 ? "0 vnđ" : String.Format("{0:0,0 vnđ}", item.SO_COD);
                item.DIA_CHI = string.IsNullOrWhiteSpace(item.HUYEN_NHAN) ? item.DIA_CHI : item.DIA_CHI + ", " + item.HUYEN_NHAN;
                item.NGAY_KY_GUI = DateTime.Now.Hour + " h " + DateTime.Now.ToString("mm") + " ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;

                item.TRONG_LUONG_STRING = item.TRONG_LUONG == 0 ? "" : item.TRONG_LUONG.ToString("N2");

                if (item.DIA_CHI.Length > 180)
                {
                    item.DIA_CHI = item.DIA_CHI.Replace("\n", ".");
                }
                if (item.DIA_CHI_NGUOI_GUI.Length > 180)
                {
                    item.DIA_CHI_NGUOI_GUI = item.DIA_CHI.Replace("\n", ".");
                }
                if (item.NOI_DUNG.Length > 180)
                {
                    item.NOI_DUNG = item.NOI_DUNG.Replace("\n", ".");
                }

            }
            
            objectDataSource1.DataSource = list;
        }
    }
}
