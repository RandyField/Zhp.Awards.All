using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class WxGetTokenController : Controller
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Index(string code,string Id)
        {
            HttpHelper httpHelper = new HttpHelper();

            string _appId = "wx6953deeefe22a83b";
            string _appsecret = "5a68901a81ddef53cc7c59a2b76c0f2f";

            //var url = 'https://api.weixin.qq.com/sns/oauth2/access_token?appid=wx6953deeefe22a83b&secret=5a68901a81ddef53cc7c59a2b76c0f2f&code=' + getCode + '&grant_type=authorization_code'

            #region  微信网页授权
            string tokenJson = httpHelper.HttpGet(String.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", _appId, _appsecret, code), "");
            JObject tokenJsonObj = JObject.Parse(tokenJson);
            var userInfoJson = httpHelper.HttpGet(string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", tokenJsonObj["access_token"], tokenJsonObj["openid"]), "");
            var model = JsonConvert.DeserializeObject<wxUserInfoModel>(userInfoJson);
            //ViewBag.Nam = userInfoJsonObj.Nickname;
            //ViewBag.img = userInfoJsonObj.Headimgurl;

            //{
            //        "openid":"omJcruMrU2DqG5oIx6fTXbs07mGM",
            //        "nickname":"你眼睛很漂亮",
            //        "sex":1,
            //        "language":"zh_CN",
            //        "city":"",
            //        "province":"",
            //        "country":"中国",
            //        "headimgurl":"http:\/\/wx.qlogo.cn\/mmopen\/HmVQlX9WkBuaN8ElhSCu63uX3AnSiadxGiaficymqdzwfJ7icbLlJr8wJfp1WaejjAjnUtJzfEDeZIcg9jZUNh392SauLib8hKiaiab\/0",
            //        "privilege":[]
            //}

            //传入后台，判断是否具有领奖资格

            //无资格
            //返回false 

            //有资格
            //返回true
            #endregion
            return View(model);
        }
    }
}
