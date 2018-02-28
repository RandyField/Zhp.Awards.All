using BLL;
using Model;
using Newtonsoft.Json;
using NinjectDI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace UI.Controllers
{
    public class GetJsonUserInfoController : Controller
    {
        //
        // GET: /GetJsonUserInfo/
        TRF_WeChatUserInfo_DI weChatDi = new TRF_WeChatUserInfo_DI();

        public ActionResult Index()
        {
            StreamReader sr = new StreamReader(HttpContext.Server.MapPath("user_tag4.json"), System.Text.Encoding.GetEncoding("utf-8"));
            string content = sr.ReadToEnd().ToString();
            sr.Close();
            Hashtable hs = JsonConvert.DeserializeObject<Hashtable>(content);
            var user_list = hs["user_list"].ToString();
            Hashtable list = JsonConvert.DeserializeObject<Hashtable>(hs["user_list"].ToString());
            //wxUserInfoModel userinfo = JsonConvert.DeserializeObject<wxUserInfoModel>(list["user_info_list"].ToString());

            
            //XmlSerializer xs = new XmlSerializer(typeof(List<wxUserInfoModel>));

            //Stream stream = new FileStream("C:\\zzl.XML", FileMode.Open,

            //                FileAccess.Read, FileShare.Read);

            List<wxUserInfoModel> listUser = JsonConvert.DeserializeObject<List<wxUserInfoModel>>(list["user_info_list"].ToString());
            TRF_WeChatUserInfo_DI weChatDi = new TRF_WeChatUserInfo_DI();
            TRF_WeChatUserInfo_BLL bll = weChatDi.getBll();
            foreach (var item in listUser)
            {
                bll.SaveJson(item);
            }
           
            return View();
        }

    }
}
