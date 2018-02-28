using Common;
using Common.Helper;
using EFModel;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NinjectDI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class GetWxUserInfoController : Controller
    {
        private readonly string _appId = ConfigurationManager.AppSettings["AppId"];
        private readonly string _appsecret = ConfigurationManager.AppSettings["Appsecret"];
        private readonly string _key = ConfigurationManager.AppSettings["encryption"];

        public ActionResult Index()
        {

            HttpHelper httpHelper = new HttpHelper();

            #region  微信网页授权
            TRF_WeChatUserInfo_DI weChatDi = new TRF_WeChatUserInfo_DI();
            List<TRF_WeChatUserInfo> list = weChatDi.getBll().getAll();
            int i = 0;
            foreach (TRF_WeChatUserInfo item in list)
            {
                i++;
                //https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET
                //System.Net.ServicePointManager.SecurityProtocol =
                //SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                string tokenJson = httpHelper.HttpGet(String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", _appId, _appsecret), "");
                JObject tokenJsonObj = null;
                if (!string.IsNullOrWhiteSpace(tokenJson))
                {
                    tokenJsonObj = JObject.Parse(tokenJson);
                }
                else
                {
                    return View("Error");
                }

                string userInfoJson = "";
                wxUserInfoModel model = null;
                if (tokenJsonObj != null)
                {
                    var access_token = tokenJsonObj["access_token"];
                    var expires_in = tokenJsonObj["expires_in"];
                    var open_id = "";
                  
                    

                    open_id = item.openid;
                    userInfoJson = httpHelper.HttpGet(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", access_token, open_id), "");
                    if (!string.IsNullOrWhiteSpace(userInfoJson))
                    {
                        model = JsonConvert.DeserializeObject<wxUserInfoModel>(userInfoJson);
                    }
                    if (model != null)
                    {
                        weChatDi.getBll().updateUserInfo(model);
                        Logger.Error("更新第1条信息"+i);
                    }

                    //userInfoJson = httpHelper.HttpGet(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", tokenJsonObj["access_token"], tokenJsonObj["expires_in"]), "");
                    //if (!string.IsNullOrWhiteSpace(userInfoJson))
                    //{
                    //    model = JsonConvert.DeserializeObject<wxUserInfoModel>(userInfoJson);
                    //}
                    //else
                    //{
                    //    return View("Error");
                    //}
                }
                else
                {
                    return View();
                }
            }

            #endregion
            return View();
        }

    }
}
