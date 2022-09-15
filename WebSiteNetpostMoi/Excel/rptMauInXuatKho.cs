using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WebSite.Models.Warehouse;
using System.Collections.Generic;

namespace WebSite.Excel
{
    public partial class rptMauInXuatKho : DevExpress.XtraReports.UI.XtraReport
    {
        public rptMauInXuatKho()
        {
            InitializeComponent();
        }

        public void SetData(WH_V3_XUAT_KHODTO dtoXK, List<WH_V3_XUAT_KHO_CHI_TIETDTO> lstXKCT)
        {
            barcodeMaXK.Text = dtoXK.MA_XUAT_KHO;
            lblThoiGianLapPhieu.Text = dtoXK.INSERT_TIME.ToString("dd/MM/yyyy HH:mm");
            lblNgayThang.Text = "Ngày " + dtoXK.INSERT_TIME.ToString("dd") + " tháng " + dtoXK.INSERT_TIME.ToString("MM") + " năm " + dtoXK.INSERT_TIME.ToString("yyyy");
            lblDonViLapPhieu.Text = dtoXK.TU_TEN_WARE_HOUSE;
            lblNguoiLapPhieu.Text = dtoXK.MA_NHAN_VIEN + " - " + dtoXK.TEN_NHAN_VIEN;
            lblNguoiNhan.Text = dtoXK.TEN_KHACH_HANG;
            lblSDT.Text = dtoXK.KH_SO_DIEN_THOAI_NGUOI_LIEN_HE;
            lblGhiChu.Text = dtoXK.GHI_CHU;
            objectDataSource1.DataSource = lstXKCT;
        }
    }
}
