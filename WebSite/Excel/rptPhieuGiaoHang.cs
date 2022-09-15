using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WebSite.Models;

namespace WebSite.Excel
{
    public partial class rptPhieuGiaoHang : DevExpress.XtraReports.UI.XtraReport
    {
        public rptPhieuGiaoHang()
        {
            InitializeComponent();
        }
        public void SetData(List<CSS_OP_CHI_TIET_DON_HANG_VIN_IDDTO> lst, CSS_OP_BOOKING_SGEV_WEB dto)
        {
            lbtennguoinhan.Text = dto.TEN_NGUOI_NHAN;
            if (!string.IsNullOrWhiteSpace(dto.SDT_NHAN))
            {
                lbtennguoinhan.Text = lbtennguoinhan.Text + Environment.NewLine + dto.SDT_NHAN;
            }
            lbdiachinguoinhan.Text = dto.DIA_CHI;
            lbMaBPBK.Text = "Mã phiếu giao: " + dto.MA_BPBK;
            lbMaPO.Text = dto.MA_DON_HANG_VINID;
            lbNgaydathang.Text = string.IsNullOrWhiteSpace(dto.NGAY_DAT_HANG) ? "" : Convert.ToDateTime(dto.NGAY_DAT_HANG).ToString("dd/MM/yyyy");
            lbNgayIn.Text = "Ngày in phiếu: " + DateTime.Now.ToString("dd/MM/yyyy");
            lbghichu.Text = dto.NOI_DUNG;
            lbphivc.Text = dto.PHI_VAN_CHUYEN_VINID.ToString("N0");
            lbtratruoc.Text = "- " + dto.TRA_TRUOC.ToString("N0");
            lbphaithu.Text = (lst.Sum(x => x.THANH_TIEN) + dto.PHI_VAN_CHUYEN_VINID - dto.TRA_TRUOC).ToString("N0");
            objectDataSource1.DataSource = lst;
        }

        private void xrLabel15_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (e.PageIndex != e.PageCount-1)
                e.Cancel = true;
        }

        private void xrLabel8_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            XRControl pageInfo = sender as XRControl;
            pageInfo.Text = e.PageIndex + 1 + "/" + e.PageCount;
        }
    }
}
