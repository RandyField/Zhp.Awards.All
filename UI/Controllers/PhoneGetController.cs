using Common;
using EFModel;
using Model;
using Newtonsoft.Json;
using NinjectDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{

    /// <summary>
    /// 红包-手机号码
    /// </summary>
    public class PhoneGetController : Controller
    {
        TRP_ScanCount_DI scanCountDi = new TRP_ScanCount_DI();
        TRP_ClientLog_DI logDi = new TRP_ClientLog_DI();

        /// <summary>
        /// 输手机号码 领取红包奖品
        /// </summary>
        /// <param name="url"></param>
        /// <param name="activityId"></param>
        /// <param name="flag"></param>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public ActionResult Index(string url, string activityId, string flag, string activityName, string computername)
        {
            ActionResult empty = new EmptyResult();
            try
            {
                if (string.IsNullOrWhiteSpace(flag))
                {
                    //扫码计数-所有
                    scanCountDi.getBll().CountByNameAndId(activityId, activityName);
                }
                else
                {
                    //http://www.chinazhihuiping.com/wxredpackets/SY_WDActivity/Activity?activityId={0}&flag={1}
                    scanCountDi.getBll().CountByNameAndId(activityId, ""+activityName+"重定向");
                }
                AwardsInfoModel awardsModel = new AwardsInfoModel();
                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/PhoneGet/Index";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(activityId);

                //请求奖品
                awardsModel = GetAwardsInfo(activityId,activityName);

                //奖品实体为null
                if (awardsModel == null)
                {
                    Common.Helper.Logger.Info(string.Format("awardsModel == null"));

                    return View("Error");
                }

                //奖品实体的类型为null
                if (awardsModel.Class != null)
                {
                    //奖品实体的类型为""
                    if (awardsModel.Class == "")
                    {
                        Common.Helper.Logger.Info(string.Format("awardsModel.Class==''"));

                        return View("Error");
                    }
                }

                //获取奖品的类型实现奖品页面的跳转
                if (string.IsNullOrWhiteSpace(computername))
                {
                    ResponseRedirect(string.Format("" + url + "?&type=&id={0}", awardsModel.id));
                }
                else
                {
                    ResponseRedirect(string.Format("" + url + "?&type=&id={0}&computername={1}", awardsModel.id,computername));
                }
                
                return empty;
            }
            catch (Exception ex)
            {

                Common.Helper.Logger.Info(string.Format(""+activityName+"进入领奖页面异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/PhoneGet/Index?url={0}&activityId={1}&flag={2}&activityName={3}", url, activityId,flag,activityName));
                return empty;
            }
        }

        /// <summary>
        /// 重定向
        /// </summary>
        /// <param name="activityId"></param>
        public void ResponseRedirect(string url)
        {
            Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            Response.BufferOutput = true;//设置输出缓冲 
            if (!Response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                Response.Redirect(url, true);
                Response.Close();
            }
        }


        /// <summary>
        /// 根据活动id获取奖品信息
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public AwardsInfoModel GetAwardsInfo(string activityId,string activityName)
        {
            AwardsInfoModel awardsInfo = null;
            try
            {
                HttpHelper httpHelper = new HttpHelper();
                string awards = httpHelper.HttpGet(String.Format("http://www.chinazhihuiping.com:89/RedPacketService/GetRedPacketServie?activityId={0}", activityId), "");
                //string awards = httpHelper.HttpGet(String.Format("http://www.chinazhihuiping.com:89/RedPacketService/GetRedPacketServie?activityId={0}", activityId), "");
                if (string.IsNullOrWhiteSpace(awards))
                {
                    return awardsInfo;
                }
                //反序列化获取奖品信息
                awardsInfo = JsonConvert.DeserializeObject<AwardsInfoModel>(awards);
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("" + activityName + "根据活动id获取奖品信息异常,异常信息：{0}", ex.ToString()));
            }
            return awardsInfo;
        }

    }
}
