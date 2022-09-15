using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Models
{
    public class CSS_OP_BOOKING_SGEV_WEB
    {
        public Int32 id
        {
            get; set;
        }

        public String PO
        {
            get; set;
        }
        public String MA_BPBK
        {
            get; set;
        }
        public String UPDATE_TIME_STRING
        {
            get; set;
        }
        public String TRANG_THAI_PHAT
        {
            get; set;
        }
        public string TEN_KHACH_HANG { get; set; }
        public String TEN_NGUOI_GUI
        {
            get; set;
        }

        public String MA_KHACH_HANG
        {
            get; set;
        }
        public String TEN_NGUOI_NHAN
        {
            get; set;
        }
        public String SDT_NHAN
        {
            get; set;
        }
        public String TINH_NHAN
        {
            get; set;
        }
        public String HUYEN_NHAN
        {
            get; set;
        }


        public String XA_NHAN
        {
            get; set;
        }


        public String MA_TINH_NHAN
        {
            get; set;
        }


        public String MA_HUYEN_NHAN
        {
            get; set;
        }
        public String MA_XA_NHAN
        {
            get; set;
        }
        public String LOAI_BPBK
        {
            get; set;
        }
        public String DICH_VU
        {
            get; set;
        }
        public String TEN_DICH_VU
        {
            get; set;
        }
        public String DIA_CHI_NGUOI_GUI

        {
            get; set;
        }
        public String DIA_CHI
        {
            get; set;
        }

        public String THOI_GIAN_HEN_NHAN
        {
            get; set;
        }

        public Decimal TRONG_LUONG
        {
            get; set;
        }

        public String NOI_DUNG
        {
            get; set;
        }

        public String INSERT_USER
        {
            get; set;
        }
        public DateTime INSERT_DATE
        {
            get; set;
        }
        public DateTime INSERT_TIME
        {
            get; set;
        }
        public String UPDATE_USER
        {
            get; set;
        }
        public DateTime UPDATE_DATE
        {
            get; set;
        }
        public DateTime UPDATE_TIME
        {
            get; set;
        }
        public string SDT_GUI { get; set; }
        public int Status { get; set; }
        public int STT { get; set; }
        public bool Check { get; set; }
        public string NGAY_KY_GUI { get; set; }
        public string TT_BO_SUNG { get; set; }
        public string SO_THAM_CHIEU { get; set; }
        public decimal SO_COD { get; set; }
        public string SO_COD_STRING { get; set; }
        public decimal TL_QUY_DOI { get; set; }
        public string LOAI_DV { get; set; }
        public string GHI_CHU { get; set; }
        public DateTime THOI_GIAN_GUI { get; set; }
        public Decimal PHI_VAN_CHUYEN_VINID { get; set; }
        public Decimal TRA_TRUOC { get; set; }
        public String MA_DON_HANG_VINID { get; set; }
        public String NGAY_DAT_HANG { get; set; }
        public DateTime NGAY_KH_DU_KIEN_LAY_HANG { get; set; }
        public string MA_TINH_LH { get; set; }
        public string MA_HUYEN_LH { get; set; }
        public string KT_HANG { get; set; }
        public Decimal DOANH_THU { get; set; }
        public string LT_SO_KHUNG { get; set; }
        public List<string> lstMA_BPBK { get; set; }
        public List<string> lstMA_KHACH_HANG { get; set; }
        public List<Int32> lstid { get; set; }
        public string HT_THANH_TOAN { get; set; }
    }

    public class CSS_OP_BOOKING_SGEV_WEB_EX : CSS_OP_BOOKING_SGEV_WEB
    {
        public bool CHON { get; set; }
        public string TINH_LH { get; set; }
        public string HUYEN_LH { get; set; }
        public string SO_KIEN { get; set; }
        public string SO_HOA_DON { get; set; }
        public string GT_DON_HANG { get; set; }
        public string HOAN_BBBG { get; set; }
        public string KHAC { get; set; }
        public string DONG_KIEM { get; set; }
        public string PHAT_TAN_TAY { get; set; }
        public string BAO_PHAT { get; set; }
        public string NGAY_HEN_PHAT_SIEU_THI { get; set; }
        public string TRONG_LUONG_STRING { get; set; }
        public string CPN { get; set; }
        public string CPT { get; set; }
        public string PHT { get; set; }
        public string PYC { get; set; }
        public string EXP { get; set; }
        public string SPX { get; set; }
        public string DOX { get; set; }
        public string PCT { get; set; }
        public string PCN { get; set; }
        public string PTK { get; set; }
        public string TT_CT { get; set; }
        public string TT_NGAY { get; set; }
        public string TT_DN { get; set; }
        public string MA_MIEN { get; set; }
        public string isSudungMDS { get; set; }
        public bool isDONG_KIEM
        {
            get
            {
                return string.IsNullOrWhiteSpace(DONG_KIEM) ? false : true;
            }
        }
        public bool isPhatTanTay
        {
            get
            {
                return string.IsNullOrWhiteSpace(PHAT_TAN_TAY) ? false : true;
            }
        }
        public bool isBaoPhat
        {
            get
            {
                return string.IsNullOrWhiteSpace(BAO_PHAT) ? false : true;
            }
        }
        public bool isKhac
        {
            get
            {
                return string.IsNullOrWhiteSpace(KHAC) ? false : true;
            }
        }
        public int SL_BILL { get; set; }

    }

}