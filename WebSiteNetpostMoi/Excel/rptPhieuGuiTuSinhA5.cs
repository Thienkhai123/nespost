using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WebSite.Models;
using System.Collections.Generic;
using WebSite.Filter;

namespace WebSite.Excel
{
    public partial class rptPhieuGuiTuSinhA5 : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPhieuGuiTuSinhA5()
        {
            InitializeComponent();
        }
        public void SetData(List<CSS_OP_BOOKING_SGEV_WEB_EX> list)
        {
            if (ClientSession.getUser().isInNgayLayHang == "true")
            {
                xrNguoiGuiKy.Text = string.Format("{0} h {1} ngày {2} tháng {3} năm {4}", DateTime.Now.ToString("HH"), DateTime.Now.ToString("mm"), DateTime.Now.ToString("dd"), DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy"));
            }
            objectDataSource1.DataSource = list;
        }

        private void xrLabel11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToDouble(this.GetCurrentColumnValue("SO_COD")) <= 0)
            {
                XRControl control = (XRControl)sender;
                control.Visible = false;
            }
        }
    }
}
