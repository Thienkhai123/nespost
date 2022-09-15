using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WebSite.Models.Warehouse;
using System.Collections.Generic;

namespace WebSite.Excel
{
    public partial class rptMauInNhapKho : DevExpress.XtraReports.UI.XtraReport
    {
        public rptMauInNhapKho()
        {
            InitializeComponent();
        }

        public void SetData(WH_V3_NHAP_KHODTO dtoNK, List<WH_V3_NHAP_KHO_CHI_TIETDTO> lstNKCT)
        {
            barcodeMaNK.Text = dtoNK.MA_NHAP_KHO;
            lblNgayThang.Text = "Ngày " + dtoNK.INSERT_TIME.ToString("dd") + " tháng " + dtoNK.INSERT_TIME.ToString("MM") + " năm " + dtoNK.INSERT_TIME.ToString("yyyy");
            lblMaNCC.Text = dtoNK.MA_NHA_CUNG_CAP;
            lblTenNCC.Text = dtoNK.TEN_NHA_CUNG_CAP;
            lblDiaChiGiaoHang.Text = dtoNK.DEN_TEN_WARE_HOUSE;
            lblLyDoNK.Text = dtoNK.GHI_CHU;
            lblNgayKy.Text = "Ngày " + DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy");
            objectDataSource1.DataSource = lstNKCT;
        }
    }
}
