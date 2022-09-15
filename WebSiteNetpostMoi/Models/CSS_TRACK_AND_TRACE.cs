using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class CSS_TRACK_AND_TRACE
    {
        public DateTime INSERT_TIME { get; set; }
        public String ID_MONGO { get; set; }
        public string INSERT_TIME_STRING { get; set; }
        public string DESC { get; set; }
        public string HAWB_NO { get; set; }
        public string URLImage { get; set; }
        public string URLImage2 { get; set; }
        public string URLImage3 { get; set; }
        public string URLImage4 { get; set; }
        public string URLImage5 { get; set; }

        public string CONTENT_TYPE { get; set; }
        public decimal TRONG_LUONG { get; set; }
        public string NOI_DEN { get; set; }
        public string DICH_VU { get; set; }
        public decimal SO_KIEN { get; set; }
    }
    public class DebitNoteOnlineResponse
    {
        public String Type { get; set; }
        public string LOAI_CUOC { get; set; }
        public String FromDate
        {
            get;
            set;
        }
        public String ToDate
        {
            get;
            set;
        }
        public String CustomerCode
        {
            get;
            set;
        }
        public String CustomerName
        {
            get;
            set;
        }
        public decimal SumAmountNoVAT
        {
            get;
            set;
        }
        public decimal SumVATAmount
        {
            get;
            set;
        }
        public decimal SumTotalAmount
        {
            get;
            set;
        }
        public string TenBang { get; set; }
        public string TuNgay { get; set; }
        public List<DebitNoteOnlineDTO> Data
        {
            get;
            set;
        }

    }
    public class DebitNoteOnlineDTO
    {
        public string LOAI_CUOC { get; set; }
        public int SortOrder
        {
            get;
            set;
        }
        public string CustomerCode
        {
            get;
            set;
        }

        public string CustomerName
        {
            get;
            set;
        }
        public string CheckInDate
        {
            get;
            set;
        }
        public string HawbNo
        {
            get;
            set;
        }
        public string DestZone
        {
            get;
            set;
        }
        public string OriginZone
        {
            get;
            set;
        }
        public decimal Weight
        {
            get;
            set;
        }
        public decimal Amount
        {
            get;
            set;
        }
        public decimal FeeAmount
        {
            get;
            set;
        }
        public decimal TotalAmount
        {
            get;
            set;
        }
        public string Weight_string
        {
            get;
            set;
        }
        public string Amount_string
        {
            get;
            set;
        }
        public string FeeAmount_string
        {
            get;
            set;
        }
        public string TotalAmount_string
        {
            get;
            set;
        }
        public int Type
        {
            get;
            set;
        }
        public decimal SumByTypeAmount
        {
            get;
            set;
        }
        public decimal SumByTypeFee
        {
            get;
            set;
        }
        public decimal SumByTypeTotal
        {
            get;
            set;
        }
        public decimal SumByTypeWeight
        {
            get;
            set;
        }
        public decimal SumByCustomerTypeTotal
        {
            get;
            set;
        }
        public string TOS { get; set; }
        public int SoKien
        {
            get;
            set;
        }
        public string GT_DON_HANG { get; set; }
        public string SO_THAM_CHIEU { get; set; }
        public string TEN_NGUOI_GUI { get; set; }
        public string TEN_NGUOI_NHAN { get; set; }

    }

    public class CSS_RECEIPT_DAILY_SALEDTO
    {
        public string CUSTOMER_CODE
        {
            get;
            set;
        }

        public string DEST_ZONE
        {
            get;
            set;
        }
        public string DEST_DISTRICT
        {
            get;
            set;
        }
        public string ORIGIN_ZONE
        {
            get;
            set;
        }
        public Double WEIGHT
        {
            get;
            set;
        }
        public string TOS { get; set; }
        public double NO_PIECES
        {
            get;
            set;
        }
        public Decimal VW_LENGTH
        {
            get;
            set;
        }
        public Decimal VW_WIDTH
        {
            get;
            set;
        }
        public Decimal VW_HEIGHT
        {
            get;
            set;
        }
        public string PKG_TYPE { get; set; }
        public double AMOUNT { get; set; }
        public double VAT { get; set; }
        public double FUEL_CHARGE { get; set; }
        public double VIETNAMESE_NET_AMT { get; set; }
        public string AMOUNT_STR { get; set; }
        public string VAT_STR { get; set; }
        public string FUEL_CHARGE_STR { get; set; }
        public string SERVICE_CHARGE_STR { get; set; }
        public string VIETNAMESE_NET_AMT_STR { get; set; }
        public Double SERVICE_CHARGE
        {
            get;
            set;
        }
        public string TRONG_LUONG_QUY_DOI { get; set; }
        public double CHIEU_DAI { get; set; }
        public double CHIEU_RONG { get; set; }
        public double CHIEU_CAO { get; set; }
        public Boolean isTheoTrongLuongQuyDoi { get; set; }
        public string lstDichVu { get; set; }
        public string lst_kich_thuoc { get; set; }
        public bool isTINH_GIA_TRANG_CHU { get; set; }
        public string THOI_GIAN_DU_KIEN { get; set; }
        public bool isDiHuyenxa { get; set; }
        public List<KICH_THUOC> lstKichThuoc { get; set; }
    }

    public class KICH_THUOC
    {
        public double SO_KIEN { get; set; }
        public double CHIEU_DAI { get; set; }
        public double CHIEU_RONG { get; set; }
        public double CHIEU_CAO { get; set; }
    }
}