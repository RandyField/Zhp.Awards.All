using BLL;
using Common;
using Common.Enum;
using EFModel;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NinjectDI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UI.Models;

namespace UI.Controllers
{
    /// <summary>
    /// Author:ZhangDeng
    /// Date:2017-04-23
    /// </summary>
    public class RedPocketController : Controller
    {
        private readonly string _appId = ConfigurationManager.AppSettings["AppId"];
        private readonly string _appsecret = ConfigurationManager.AppSettings["Appsecret"];
        private readonly string _key = ConfigurationManager.AppSettings["encryption"];

        View_AwardReceive_DI viewDi = new View_AwardReceive_DI();
        TRF_WeChatUserInfo_DI weChatDi = new TRF_WeChatUserInfo_DI();
        TRP_AwardReceive_DI awardDi = new TRP_AwardReceive_DI();
        TRP_AwardDetail_DI detailDi = new TRP_AwardDetail_DI();
        TRP_ClientLog_DI logDi = new TRP_ClientLog_DI();

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult Index(RequestModel request)
        {
            //return View();
            //awardDi.getBll().hadTakeAward("omJcruMSo_al_vHgI7dQHdixsPEM", "44");
            try
            {
                HttpHelper httpHelper = new HttpHelper();

                #region  微信网页授权

                string tokenJson = httpHelper.HttpGet(String.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", _appId, _appsecret, request.code), "");
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
                    userInfoJson = httpHelper.HttpGet(string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", tokenJsonObj["access_token"], tokenJsonObj["openid"]), "");
                    if (!string.IsNullOrWhiteSpace(userInfoJson))
                    {
                        model = JsonConvert.DeserializeObject<wxUserInfoModel>(userInfoJson);
                        if (model.Nickname == null || model.Sex == null)
                        {
                            return View("Error");
                        }
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }

                #endregion

                ActivityStatus status = new ActivityStatus();
                status = ActivityStatus.Unknown;

                //活动id未取到
                if (string.IsNullOrWhiteSpace(request.activityId))
                {
                    return View("Error");
                }



                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";

                //今天已参加活动 无领奖资格  直接返回视图 不请求奖品
                if (isAttendToday(request.activityId, model.Openid))
                {
                    status = ActivityStatus.HasAttend;
                   

                    //判断奖品是否已领 如果没有领取 该奖品继续返回页面
                    var receivedModel = awardDi.getBll().hadTakeAward(model.Openid, request.activityId);
                    if (receivedModel == null)
                    {
                        entity.Description = string.Format("用户在{0}点击红包，二维码实物扫码（已参加活动，为重复扫码）", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        entity.PageDesc = "实物扫码,已参加本次活动";
                        entity.ActivityId = Convert.ToInt32(request.activityId);
                        logDi.getBll().SaveLog(entity);
                        return View("HasAttend");                  
                    }
                    else
                    {
                        //获取奖品ID  int
                        string awardId = receivedModel.AwardDetailId.ToString();
                    
                        TRP_AwardDetail detailModel = detailDi.getBll().GetEntityById(awardId);

                        //奖品名称
                        string awardsName = "";

                        //加密奖品id
                        string ecodeAwardId  = DESEncrypt.Encrypt(awardId, _key);
                        Common.Helper.Logger.Info("发生了重定向奖品Id加密:" + ecodeAwardId);
                       
                        if (detailModel != null)
                        {
                            //奖品名称
                            awardsName = detailModel.AwardName;
                            Common.Helper.Logger.Info("发生了重定向奖品名称:" + awardsName);
                        }

                        if (!string.IsNullOrWhiteSpace(awardsName))
                        {
                            string awardsType = awardsName;
                            string typeCode = "";
                            if (awardsType.Contains("美食券"))
                            {
                                typeCode = "A";
                            }
                            else if (awardsType.Contains("电影券"))
                            {
                                typeCode = "B";
                            }
                            else if (awardsType.Contains("水趣多"))
                            {
                                typeCode = "C";
                            }
                            else
                            {
                                typeCode = "D";
                                Common.Helper.Logger.Info(string.Format("重定向:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型，默认发瓶水",
                                                                                                                             model.Openid));
                            }
                            ViewData["Type"] = typeCode;
                            Common.Helper.Logger.Info("发生了重定向奖品type:" + typeCode);
                        }
                        else
                        {
                            ViewData["Type"] = "";
                        }
                        
                        ViewData["Openid"] = model.Openid ?? "";
                        ViewData["wxName"] = model.Nickname ?? "";
                        ViewData["AwardDetailId"] = ecodeAwardId;
                        ViewData["AwardName"] = awardsName;
                        ViewData["Activity"] = request.activityId ?? "";

                        entity.Description = string.Format("用户在{0}点击红包，二维码实物扫码（为重复扫码,上次未核销奖品）", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        entity.PageDesc = "实物扫码,上次未核销奖品";
                        entity.ActivityId = Convert.ToInt32(request.activityId);
                        logDi.getBll().SaveLog(entity);
                        return View();
                    }
                    //return View("HasAttend");
                }

                string activityId = request.activityId;
                //http://localhost:89/RedPacketService/GetRedPacketServie?activityId=44
                //获取奖品信息
                string awards = httpHelper.HttpGet(String.Format("http://localhost:89/RedPacketService/GetRedPacketServie?activityId={0}", activityId), "");

                if (string.IsNullOrWhiteSpace(awards))
                {
                    return View("Error");
                }

                //反序列化获取奖品信息
                AwardsInfoModel awardsInfo = JsonConvert.DeserializeObject<AwardsInfoModel>(awards);
                if (awardsInfo != null)
                {
                    if (awardsInfo.Class != null)
                    {
                        string awardsType = awardsInfo.Class;
                        string typeCode = "";
                        if (awardsType.Contains("美食券"))
                        {
                            typeCode = "A";
                        }
                        else if (awardsType.Contains("电影券"))
                        {
                            typeCode = "B";
                        }
                        else if (awardsType.Contains("水趣多"))
                        {
                            typeCode = "C";
                        }
                        else
                        {
                            typeCode = "D";
                            Common.Helper.Logger.Info(string.Format("获取奖品:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型，默认发瓶水",
                                                                                                                         model.Openid));
                        }
                        ViewData["Type"] = typeCode;
                    }
                }



                //首次参加本次活动
                status = ActivityStatus.FirstTime;

                //不存在微信用户
                if (!isExistOpenId(model.Openid))
                {
                    //保存用户微信信息
                    saveUserInfo(model);
                }

                //保存扫码信息
                saveScanInfo(model.Openid, awardsInfo.id);

                string awardName = awardsInfo.Class;

                if (status == ActivityStatus.FirstTime)
                {
                    entity.Description = string.Format("用户在{0}点击红包，二维码扫码进入实物领奖页面，奖品为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), awardName);
                    entity.PageDesc = string.Format("实物扫码,得到{0}", awardName);
                    entity.ActivityId = Convert.ToInt32(DESEncrypt.Decrypt(awardsInfo.id, _key));
                    logDi.getBll().SaveLog(entity);
                    ViewData["Openid"] = model.Openid ?? "";
                    ViewData["wxName"] = model.Nickname ?? "";
                    ViewData["AwardDetailId"] = awardsInfo.id ?? "";
                    ViewData["AwardName"] = awardsInfo.Class ?? "";
                    ViewData["Activity"] = request.activityId ?? "";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("进入领奖页面异常,异常信息：{0}", ex.ToString()));
            }

            return View("Error");
        }

        /// <summary>
        /// 核销领奖
        /// </summary>
        /// <param name="Openid"></param>
        /// <param name="AwardDetailId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ReceivedAward(AwardReceivedModel model)
        {
            object data = null;
            try
            {
                var res = new JsonResult();

                //已领取奖品 再次领取失败
                if (isTakeAward(model.Openid, model.ActivityId))
                {
                    //返回json
                    data = new { success = false, status = "2" };
                }
                else
                {
                    if (saveUserAwardReceiveInfo(model.Openid, model.AwardDetailId))
                    {
                        //返回json
                        data = new { success = true, status = "1" };

                        //记录日志
                        TRP_ClientLog entity = new TRP_ClientLog();
                        entity.CreateTime = DateTime.Now;
                        entity.DeleteMark = false;
                        entity.Enable = true;
                        entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets";
                        entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                        var efid = DESEncrypt.Decrypt(model.AwardDetailId, _key);
                        var awardName = model.AwardName;
                        entity.Description = string.Format("{0}工作人员核销领奖，奖品为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), awardName);
                        entity.PageDesc = string.Format("工作人员核销领奖,奖品{0}", awardName);
                        entity.ActivityId = Convert.ToInt32(model.ActivityId);

                        //保存日志
                        logDi.getBll().SaveLog(entity);
                    }
                    else
                    {
                        //返回json
                        data = new { success = false, status = "1" };
                    }
                }

            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("微信用户-OpenId:{0}-领取奖品，保存到数据库中出现错误,，奖品ID为{1}，错误详情:{2}",
                    model.Openid,
                    model.AwardDetailId,
                    ex.ToString()));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  是否已扫
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool isScan(AwardsInfoModel awardsInfo)
        {
            bool flag = false;
            View_AwardReceive_BLL bll = viewDi.getBll();
            var id = DESEncrypt.Decrypt(awardsInfo.id, _key);
            flag = bll.isExist(id);
            return flag;
        }

        /// <summary>
        /// 是否存在微信用户
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isExistOpenId(string openId)
        {
            bool flag = false;
            TRF_WeChatUserInfo_BLL bll = weChatDi.getBll();
            flag = bll.isExist(openId);
            return flag;
        }

        /// <summary>
        /// 保存微信用户信息
        /// </summary>
        /// <param name="model"></param>
        public void saveUserInfo(wxUserInfoModel model)
        {
            TRF_WeChatUserInfo_BLL bll = weChatDi.getBll();
            bll.Save(model);
        }



        /// <summary>
        /// 保存用户领奖信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="id"></param>
        public bool saveUserAwardReceiveInfo(string openid, string id)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            var realId = DESEncrypt.Decrypt(id, _key);
            flag = bll.update(realId, openid);
            return flag;
        }


        /// <summary>
        /// 微信号是否已参加本次活动
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bool isAttendToday(string openid)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            flag = bll.isExist(openid);
            return flag;
        }

        /// <summary>
        /// 此微信号是否已参加本次活动
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <param name="openId">微信OpenID</param>
        /// <returns></returns>
        public bool isAttendToday(string activityId, string openId)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            flag = bll.isExistWxByActivity(activityId, openId);
            return flag;
        }



        /// <summary>
        /// 保存扫码信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool saveScanInfo(string openId, string id)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            var realId = DESEncrypt.Decrypt(id, _key);
            flag = bll.saveScan(openId, realId);
            return flag;
        }

        /// <summary>
        /// 是否已领奖品
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isTakeAward(string openId, string activityId)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            flag = bll.isTakeAward(openId, activityId);
            return flag;
        }
    }
}
