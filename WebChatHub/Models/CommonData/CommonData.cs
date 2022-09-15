using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WebChatHub.Models
{
    public class CommonData
    {
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public static string encrypt(string ToEncrypt)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(ToEncrypt));
        }
        public static string decrypt(string cypherString)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(cypherString));
        }

        public static List<CSS_CRM_NGUOI_DUNGDTO> _lstCS_BOOKING = new List<CSS_CRM_NGUOI_DUNGDTO>();
        public static List<CSS_CRM_NGUOI_DUNGDTO> _lstCS_VTM = new List<CSS_CRM_NGUOI_DUNGDTO>();
        public static void getListCSByCompanyCode(bool isGet = false)
        {
            if (_lstCS_VTM.Count == 0 || _lstCS_BOOKING.Count ==0 || isGet)
            {
                List<string> lstRole = new List<string> { "Nhân viên dịch vụ khách hàng", "Trưởng phòng Dịch Vụ Khách Hàng", "Nhân viên giám sát" };
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("GET_LIST_CS", JsonConvert.SerializeObject(lstRole));
                if (dtoRes.Status == "OK")
                {
                    List<CSS_CRM_NGUOI_DUNGDTO> _lstCS = JsonConvert.DeserializeObject<List<CSS_CRM_NGUOI_DUNGDTO>>(dtoRes.ResponseData);
                    foreach (var item in _lstCS)
                    {
                        if (item.STATION_CODE.StartsWith("59"))
                            item.COMPANY_CODE = "02";

                        item.FULL_NAME = item.USER_NAME + " - " + item.NAME;

                        if(item.LOAI_TK == "H2")
                        {
                            _lstCS_BOOKING.Add(item);
                        }
                        else
                        {
                            _lstCS_VTM.Add(item);
                        }
                    }
                }
            }
        }
    }
}