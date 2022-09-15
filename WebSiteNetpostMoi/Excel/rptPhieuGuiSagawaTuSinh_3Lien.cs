using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WebSite.Models;
using System.Collections.Generic;

namespace WebSite.Excel
{
    public partial class rptPhieuGuiSagawaTuSinh_3Lien : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPhieuGuiSagawaTuSinh_3Lien()
        {
            InitializeComponent();
        }
        public void SetData(List<CSS_OP_BOOKING_SGEV_WEB_EX> list)
        {
            objectDataSource1.DataSource = list;
        }
    }
}
