using BLL;
using Common;
using EFModel;
using Newtonsoft.Json;
using NinjectDI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace UI.Controllers
{
    public class ValentinesDayController : ApiController
    {
        public readonly string _key = ConfigurationManager.AppSettings["encryption"];

        public Dictionary<string, string> award_imgurl = new Dictionary<string, string>(){ 
                        {"一见钟情大红包","http://www.chinazhihuiping.com/wxredpackets/Content/image/ValentinesDay/award1.png"},
                        {"两情相悦大红包","http://www.chinazhihuiping.com/wxredpackets/Content/image/ValentinesDay/award2.png"},
                        {"情定三生大红包","http://www.chinazhihuiping.com/wxredpackets/Content/image/ValentinesDay/award3.png"}
                        
        };

        public string Get([FromUri]string awardid)
        {
            string awardname = "";
            string imgurl = "http://www.chinazhihuiping.com/wxredpackets/Content/image/ValentinesDay/error.png";
            try
            {
                //解密奖品id
                var realId = DESEncrypt.Decrypt(awardid, _key);

                //奖品是否已被请求
                if (new TRP_AwardReceive_DI().getBll().isGetAward(realId))
                {

                    //获取奖品名称 
                    TRP_AwardDetail model = new TRP_AwardDetail();
                    TRP_AwardDetail_BLL bll = new TRP_AwardDetail_DI().getBll();
                    awardname = bll.GetEntityById(realId).AwardName;

                    //通过奖品名称获取奖品图片url
                    if (award_imgurl.ContainsKey(awardname))
                    {
                        imgurl = award_imgurl[awardname];
                    }                  
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("爱情谷api获取奖品{0}图片url异常,异常信息：{1}", awardid, ex.ToString()));
            }
                   
            var jsonmodel = new
            {
                awardname = awardname,
                imgurl = imgurl
            };

            return JsonConvert.SerializeObject(jsonmodel);
        }

        // POST api/valentinesday
        public void Post([FromBody]string value)
        {
        }

        // PUT api/valentinesday/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/valentinesday/5
        public void Delete(int id)
        {
        }
    }
}
