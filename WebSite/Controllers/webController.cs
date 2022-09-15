using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class webController : Controller
    {
        public ActionResult guest(string HAWB_NO)
        {
            ViewBag.mavandon = HAWB_NO;
            JMessage msg = new JMessage();
            Dictionary<string, List<CSS_TRACK_AND_TRACE>> dic = new Dictionary<string, List<CSS_TRACK_AND_TRACE>>();
            try
            {
                var lag = "";
                //if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                //{
                //    lag = "";
                //}
                //else
                //    lag = "vi-VN";
                //Mặc định tiếng việt
                lag = "vi-VN";
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_TRACK_AND_TRACE() { HAWB_NO = HAWB_NO.Replace("\t", "").Replace("\n", ","), DESC = lag });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_TRACK_AND_TRACE", inputCa);
                List<CSS_TRACK_AND_TRACE> lst;
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_TRACK_AND_TRACE>>(dtoRes.ResponseData);
                    foreach (var item in lst)
                    {
                        item.DESC = item.DESC.Replace("[SGV]DASH LOGISTICS", "Hub_Das");
                        item.DESC = item.DESC.Replace("[SGV]NASCO", "Hub_Nas");
                        item.DESC = item.DESC.Replace("[SGV]VIETTEL", "Hub_Viet");
                        if (lag == "")
                        {
                            item.DESC = item.DESC.Replace("vào lúc", "at").Replace("DA PHAT", "Deliveried successfully to");
                        }
                        item.INSERT_TIME_STRING = item.INSERT_TIME.ToString("dd/MM/yyyy HH:mm");
                        item.HAWB_NO = item.HAWB_NO.ToUpper().Trim();
                        if (item.NOI_DEN == null)
                        {
                            item.NOI_DEN = "";
                        }
                        if (item.TRONG_LUONG == null)
                        {
                            item.TRONG_LUONG = 0;
                        }
                        if (item.DICH_VU == null)
                        {
                            item.DICH_VU = "";
                        }

                    }
                    lst = lst.OrderBy(x => x.HAWB_NO).OrderByDescending(x => x.INSERT_TIME).ToList();
                    dic = lst.GroupBy(x => x.HAWB_NO).ToDictionary(x => x.Key, x => x.ToList());
                    msg.Object = dic;
                    msg.Title = "Get list success!";
                }
            }
            catch (Exception ex)
            {
                msg.Object = null;
                msg.Title = ex.Message;
            }
            return View(dic);
        }
        public ActionResult search()
        {
            string HAWB_NO = Request["vandon"];
            ViewBag.mavandon = HAWB_NO;
            JMessage msg = new JMessage();
            Dictionary<string, List<CSS_TRACK_AND_TRACE>> dic = new Dictionary<string, List<CSS_TRACK_AND_TRACE>>();
            try
            {
                var lag = "";
                //if (Request.Cookies["language"].Value == "ja" || Request.Cookies["language"].Value == "en")
                //{
                //    lag = "";
                //}
                //else
                //    lag = "vi-VN";
                //Mặc định tiếng việt
                lag = "vi-VN";
                string inputCa = Newtonsoft.Json.JsonConvert.SerializeObject(new CSS_TRACK_AND_TRACE() { HAWB_NO = HAWB_NO.Replace("\t", "").Replace("\n", ","), DESC = lag });
                BaseResponseAndroid dtoRes = Connection.KetNoiLayDuLieu("SEARCH_LIST_TRACK_AND_TRACE", inputCa);
                List<CSS_TRACK_AND_TRACE> lst;
                if (dtoRes.Status == "OK")
                {
                    lst = JsonConvert.DeserializeObject<List<CSS_TRACK_AND_TRACE>>(dtoRes.ResponseData);

                    foreach (var item in lst)
                    {
                        item.DESC = item.DESC.Replace("[SGV]DASH LOGISTICS", "Hub_Das");
                        item.DESC = item.DESC.Replace("[SGV]NASCO", "Hub_Nas");
                        item.DESC = item.DESC.Replace("[SGV]VIETTEL", "Hub_Viet");
                        item.DESC = item.DESC.Replace("vào lúc", "at").Replace("DA PHAT", "Finished");
                        item.INSERT_TIME_STRING = item.INSERT_TIME.ToString("dd/MM/yyyy HH:mm");
                    }
                    lst = lst.OrderBy(x => x.HAWB_NO).OrderByDescending(x => x.INSERT_TIME).ToList();
                    dic = lst.GroupBy(x => x.HAWB_NO).ToDictionary(x => x.Key, x => x.ToList());
                    msg.Object = dic;
                    msg.Title = "Get list success!";
                }
            }
            catch (Exception ex)
            {
                msg.Object = null;
                msg.Title = ex.Message;
            }
            return View("guest",dic);
        }
    }
}