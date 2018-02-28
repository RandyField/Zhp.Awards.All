using Common;
using EFModel;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class NewCityController : BaseController
    {
        #region 发实物奖品

        /// <summary>
        ///  ViewData["Openid"] = "";
        ////ViewData["wxName"] = "RandyField";
        ////ViewData["AwardDetailId"] =  "";
        ////ViewData["AwardName"] =  "";
        ////ViewData["Activity"] =  "";
        ////ViewData["Type"] = "A";
        ////return View();
        ////ActionResult empty = new EmptyResult();
        /// 热高乐园红包雨活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        ///             string type = "1元";
        //string opienid = "omJcruMSo_al_vHgI7dQHdixsPEM";
        //RequestModel request = new RequestModel
        //{
        //    activityName = "【中兴30周年庆现金红包】",
        //    TenPaySender = "中兴大厦",
        //    TenPayWish = "恭喜发财",
        //    TenPayGame = "30周年庆，红包雨",
        //    TenPayRemark = "中兴大厦"
        //};

        //GiveCash(type, opienid, request);
        public ActionResult Activity(string activityId, string flag, string guid)
        {
            /*
            if (!string.IsNullOrWhiteSpace(guid))
            {
                if (!IsInDate(activityId, guid, 4))
                {
                    return View("OutOfDate");
                }
            }
            //未传guid
            else
            {
                return View("OutOfDate");
            }
             */

            ActionResult empty = new EmptyResult();
            try
            {
                if (string.IsNullOrWhiteSpace(flag))
                {
                    //扫码计数-所有
                    scanCountDi.getBll().CountByNameAndId(activityId, _activityName);
                }
                else
                {
                    if (flag.Trim() == "1")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, string.Format("{0}发生重定向-现金红包跳转", _activityName));
                    }
                    else
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, string.Format("{0}发生重定向-非现金红包跳转", _activityName));
                    }

                }

                //日志记录公共部分
                AwardsInfoModel awardsModel = new AwardsInfoModel();
                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/NewCity/Activity";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(activityId);

                //请求奖品
                awardsModel = GetAwardsInfo(activityId);

                //奖品的Class不为空
                if (awardsModel.Class != null)
                {
                    if (awardsModel.Class == "")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, string.Format("{0}-取奖品awardsModel.Class == \"\"", _activityName));
                        return View("Nogift");
                    }

                    //重定向 至微信领奖页
                    string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/NewCity/WXActivity?activityId={0}&giftType={1}&giftId={2}", activityId, awardsModel.Class, awardsModel.id);

                    string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);

                    ResponseWXRedirect(urlencode);

                    return empty;

                }
                else
                {
                    scanCountDi.getBll().CountByNameAndId(activityId, string.Format("{0}-取奖品awardsModel.Class == null", _activityName));
                    return View("Nogift");
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("{0}进入领奖页面异常,异常信息：{1}", _activityName, ex.ToString()));
                return View("Nogift");
            }
        }

        /// <summary>
        /// 微信控制器
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult WXActivity(RequestModel request)
        {
            ActionResult empty = new EmptyResult();
            try
            {
                #region 数据库日志记录  公共部分

                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/NewCity/WXActivity";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion

                #region 获取微信用户信息 昵称，头像等

                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {
                    //非卡重定向 至微信领奖页
                    string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/NewCity/WXActivity?activityId={0}&giftType={1}&giftId={2}", request.activityId, request.giftType, request.giftId);
                    string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);
                    ResponseWXRedirect(urlencode);
                    return empty;
                }

                #endregion

                #region 今天已参加活动

                if (isAttendToday(request.activityId, wxUser.Openid))
                {
                    //判断奖品是否已领
                    var receivedModel = awardDi.getBll().hadTakeAward(wxUser.Openid, request.activityId);

                    #region 奖品已核销

                    //奖品已领
                    if (receivedModel == null)
                    {
                        scanCountDi.getBll().CountByNameAndId(request.activityId, string.Format("{0}-实物已领取", _activityName));
                        return View("HadAttend");
                    }
                    #endregion

                    #region 奖品还未核销

                    //奖品还未领取
                    else
                    {
                        //获取奖品ID  int
                        string awardId = receivedModel.AwardDetailId.ToString();

                        //获取奖品详情
                        TRP_AwardDetail detailModel = detailDi.getBll().GetEntityById(awardId);

                        //奖品名称
                        string awardsName = "";

                        //加密奖品id
                        string ecodeAwardId = DESEncrypt.Encrypt(awardId, _key);


                        if (detailModel != null)
                        {
                            awardsName = detailModel.AwardName;
                        }

                        //奖品类型
                        if (!string.IsNullOrWhiteSpace(awardsName))
                        {
                            string typeCode = "";
                            if (awardsName.Contains("周末礼金"))
                            {
                                typeCode = "A";
                            }
                            else if (awardsName.Contains("健身月卡"))
                            {
                                typeCode = "B";
                            }
                            else if (awardsName.Contains("圆春饼"))
                            {
                                typeCode = "C";
                            }
                            else if (awardsName.Contains("咖啡券"))
                            {
                                typeCode = "D";
                            }
                            else if (awardsName.Contains("公社五折"))
                            {
                                typeCode = "E";
                            }
                            else if (awardsName.Contains("公社三折"))
                            {
                                typeCode = "F";
                            }
                            else if (awardsName.Contains("美甲护理"))
                            {
                                typeCode = "G";
                            }
                            else if (awardsName.Contains("美甲30元"))
                            {
                                typeCode = "H";
                            }
                            else if (awardsName.Contains("一叶一生"))
                            {
                                typeCode = "I";
                            }
                            else if (awardsName.Contains("含花"))
                            {
                                typeCode = "J";
                            }
                            else if (awardsName.Contains("萌宠"))
                            {
                                typeCode = "K";
                            }
                            else if (awardsName.Contains("南山"))
                            {
                                typeCode = "L";
                            }
                            else if (awardsName.Contains("魅可发妆"))
                            {
                                typeCode = "M";
                            }
                            else if (awardsName.Contains("在宥"))
                            {
                                typeCode = "N";
                            }
                            else if (awardsName.Contains("月饼"))
                            {
                                typeCode = "O";
                            }
                            else if (awardsName.Contains("大麦"))
                            {
                                typeCode = "P";
                            }
                            else
                            {
                                return View("Nogift");
                            }
                            ViewData["Type"] = typeCode;
                        }

                        ViewData["Openid"] = wxUser.Openid ?? "";
                        ViewData["wxName"] = wxUser.Nickname ?? "";
                        ViewData["AwardDetailId"] = ecodeAwardId;
                        ViewData["AwardName"] = awardsName;
                        ViewData["Activity"] = request.activityId ?? "";

                        entity.Description = string.Format("用户在{0}点击红包，二维码实物扫码（为重复扫码,上次未核销奖品）", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        entity.PageDesc = "实物扫码,上次未核销奖品";
                        entity.ActivityId = Convert.ToInt32(request.activityId);
                        logDi.getBll().SaveLog(entity);
                        return View("Activity");
                    }
                    #endregion
                }

                #endregion

                #region 首次参加活动
                if (request.giftType != null)
                {
                    string awardsName = request.giftType;
                    string typeCode = "";

                    if (awardsName.Contains("周末礼金"))
                    {
                        typeCode = "A";
                    }
                    else if (awardsName.Contains("健身月卡"))
                    {
                        typeCode = "B";
                    }
                    else if (awardsName.Contains("圆春饼"))
                    {
                        typeCode = "C";
                    }
                    else if (awardsName.Contains("咖啡券"))
                    {
                        typeCode = "D";
                    }
                    else if (awardsName.Contains("公社五折"))
                    {
                        typeCode = "E";
                    }
                    else if (awardsName.Contains("公社三折"))
                    {
                        typeCode = "F";
                    }
                    else if (awardsName.Contains("美甲护理"))
                    {
                        typeCode = "G";
                    }
                    else if (awardsName.Contains("美甲30元"))
                    {
                        typeCode = "H";
                    }
                    else if (awardsName.Contains("一叶一生"))
                    {
                        typeCode = "I";
                    }
                    else if (awardsName.Contains("含花"))
                    {
                        typeCode = "J";
                    }
                    else if (awardsName.Contains("萌宠"))
                    {
                        typeCode = "K";
                    }
                    else if (awardsName.Contains("南山"))
                    {
                        typeCode = "L";
                    }
                    else if (awardsName.Contains("魅可发妆"))
                    {
                        typeCode = "M";
                    }
                    else if (awardsName.Contains("在宥"))
                    {
                        typeCode = "N";
                    }
                    else if (awardsName.Contains("月饼"))
                    {
                        typeCode = "O";
                    }
                    else if (awardsName.Contains("大麦"))
                    {
                        typeCode = "P";
                    }
                    else
                    {
                        return View("Nogift");
                        //Common.Helper.Logger.Info(string.Format("【中兴30周年红包雨】,用户获取奖品:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型,发生了重定向"));
                        //Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/ZTE/Activity?activityId={0}&flag={1}", request.activityId, 2));
                        //return empty;
                    }
                    ViewData["Type"] = typeCode;
                }

                //不存在微信用户
                if (!isExistOpenId(wxUser.Openid))
                {
                    //保存用户微信信息
                    saveUserInfo(wxUser);
                }

                //保存扫码信息
                saveScanInfo(wxUser.Openid, request.giftId);

                string awardName = request.giftType;


                entity.Description = string.Format("{0},用户在{1}点击红包，二维码扫码进入实物领奖页面，奖品为{2}", _activityName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.giftType);
                entity.PageDesc = string.Format("实物扫码,得到{0}", awardName);
                entity.ActivityId = Convert.ToInt32(DESEncrypt.Decrypt(request.giftId, _key));
                logDi.getBll().SaveLog(entity);

                ViewData["Openid"] = wxUser.Openid ?? "";
                ViewData["wxName"] = wxUser.Nickname ?? "";
                ViewData["AwardDetailId"] = request.giftId ?? "";
                ViewData["AwardName"] = request.giftType ?? "";
                ViewData["Activity"] = request.activityId ?? "";
                return View("Activity");

                #endregion

                #region 注释部分 无限制领取次数


                //if (request.giftType != null)
                //{
                //    string awardsType = request.giftType;
                //    string typeCode = "";
                //    if (awardsType.Contains("笔记本"))
                //    {
                //        typeCode = "A";
                //    }
                //    else
                //    {
                //        Common.Helper.Logger.Info(string.Format("【热高乐园红包雨】,用户获取奖品:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型,发生了重定向"));
                //        Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/Activity?activityId={0}&flag={1}", request.activityId, 2));
                //        return empty;
                //    }
                //    ViewData["Type"] = typeCode;
                //}

                ////不存在微信用户
                //if (!isExistOpenId(wxUser.Openid))
                //{
                //    //保存用户微信信息
                //    saveUserInfo(wxUser);
                //}

                ////保存扫码信息
                //saveScanInfo(wxUser.Openid, request.giftId);

                //#region 保存日志记录至数据库

                //entity.Description = string.Format("【热高乐园红包雨】,用户在{0}点击红包，二维码扫码进入实物领奖页面，奖品为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.giftType);
                //entity.PageDesc = string.Format("实物扫码,得到{0}", request.giftType);
                //entity.ActivityId = Convert.ToInt32(DESEncrypt.Decrypt(request.giftId, _key));
                //logDi.getBll().SaveLog(entity);

                //#endregion

                //ViewData["Openid"] = wxUser.Openid ?? "";
                //ViewData["wxName"] = wxUser.Nickname ?? "";
                //ViewData["AwardDetailId"] = request.giftId ?? "";
                //ViewData["AwardName"] = request.giftType ?? "";
                //ViewData["Activity"] = request.activityId ?? "";

                //return View("Activity");

                #endregion
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("{0}领取实物异常,异常信息：{1}", _activityName, ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/NewCity/Activity?activityId={0}&flag={1}", request.activityId, 2));
                return empty;
            }
        }


        /// <summary>
        /// 红包-核销领奖(重写)
        /// </summary>
        /// <param name="Openid"></param>
        /// <param name="AwardDetailId"></param>
        /// <returns></returns>
        [HttpPost]
        public override JsonResult ReceivedAward(AwardReceivedModel model)
        {
            object data = null;
            try
            {
                var res = new JsonResult();
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
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("微信用户-OpenId:{0}-领取奖品，保存到数据库中出现错误,，奖品ID为{1}，错误详情:{2}",
                    model.Openid,
                    model.AwardDetailId,
                    ex.ToString()));
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  发钱

        /// <summary>
        /// 现金红包微信
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult Cash(string activityId, string guid)
        {
            /*
            if (!string.IsNullOrWhiteSpace(guid))
            {
                if (!IsInDate(activityId, guid, 2))
                {
                    return View("OutOfDate");
                }
            }
            //未传guid
            else
            {
                return View("OutOfDate");
            }
             */ 

            Dictionary<string, string> r_activity = new Dictionary<string, string>();
            r_activity.Add("164", "163");
            r_activity.Add("166", "165");
            r_activity.Add("168", "167");

            ActionResult empty = new EmptyResult();
            try
            {
                string activityName = "【中兴新一城现金红包】";
                string TenPaySender = "中兴新一城";
                string TenPayWish = "恭喜发财";
                string TenPayGame = "中兴新一城，红包雨";
                string TenPayRemark = "中兴新一城";

                //跳转实物奖品url
                string responseurl = string.Format("http://www.chinazhihuiping.com/wxredpackets/NewCity/Activity?activityId={0}", r_activity[activityId]); ;

                string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/NewCity/GiveMoney?activityId={0}&" +
                "activityName={1}&TenPaySender={2}&TenPayWish={3}&TenPayGame={4}&TenPayRemark={5}&guid={6}&" +
                "url={7}", activityId, activityName, TenPaySender, TenPayWish, TenPayGame, TenPayRemark, guid, responseurl);
                //Common.Helper.Logger.Info(url);
                string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);

                ResponseWXRedirect(urlencode);
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("现金红包微信跳转异常，异常信息:{0}",
                                 ex.ToString()));
            }

            return empty;
        }

        /// <summary>
        /// 发现金
        /// </summary>
        /// <returns></returns>
        public ActionResult GiveMoney(RequestModel request, string guid)
        {
            //Common.Helper.Logger.Info(guid);
            ActionResult empty = new EmptyResult();
            try
            {
                //扫码计数-所有
                scanCountDi.getBll().CountByNameAndId(request.activityId, request.activityName);


                #region 数据库日志记录  公共部分

                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/NewCity/GiveMoney";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion


                //获取微信用户信息
                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {

                    Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                    return empty;
                }

                //微信用户openid
                string openid = wxUser.Openid;

                //保存微信信息
                if (!isExistOpenId(wxUser.Openid))
                {
                    //保存用户微信信息
                    saveUserInfo(wxUser);
                }

                TRP_AwardReceive receivedModel = null;

                #region 今天已参加活动

                if (isAttendToday(request.activityId, wxUser.Openid))
                {
                    //判断奖品是否已领
                    receivedModel = awardDi.getBll().hadTakeAward(wxUser.Openid, request.activityId);

                    #region 奖品已核销

                    //奖品已领
                    if (receivedModel == null)
                    {

                        Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                        return empty;
                    }

                    #endregion

                    #region 奖品还未核销

                    //奖品还未领取
                    else
                    {
                        //获取奖品ID  int
                        string awardId = receivedModel.AwardDetailId.ToString();

                        //获取奖品详情
                        TRP_AwardDetail detailModel = detailDi.getBll().GetEntityById(awardId);

                        //奖品名称
                        string awardsName = "";

                        //加密奖品id
                        string ecodeAwardId = DESEncrypt.Encrypt(awardId, _key);

                        if (detailModel != null)
                        {
                            //奖品名称
                            awardsName = detailModel.AwardName;
                        }
                        else
                        {
                            Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                            return empty;
                        }

                        //奖品类型
                        if (!string.IsNullOrWhiteSpace(awardsName))
                        {
                            GiveCash(awardsName, wxUser.Openid, request);
                        }
                        else
                        {
                            Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                            return empty;
                        }

                        entity.Description = string.Format("用户在{0}点击红包，现金红包（为重复扫码,上次未核销奖品）", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        entity.PageDesc = "现金扫码,上次未核销奖品";
                        entity.ActivityId = Convert.ToInt32(request.activityId);
                        logDi.getBll().SaveLog(entity);
                        return View("GiveMoney");
                    }
                    #endregion
                }

                #endregion

                #region 今天还未参加活动  则请求奖品

                AwardsInfoModel awardsModel = new AwardsInfoModel();

                //请求奖品
                awardsModel = GetAwardsInfo(request.activityId);

                //奖品实体的类型为null
                if (awardsModel.Class != null)
                {
                    //奖品实体的类型为""
                    if (awardsModel.Class == "")
                    {
                        //return View("Nogift");
                        Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                        return empty;
                    }
                }
                else
                {
                    //return View("Nogift");
                    Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                    return empty;
                }

                #endregion

                //发钱
                if (GiveCash(awardsModel.Class, openid, request))
                {
                    //保存领奖信息-测试
                    saveScanInfo(wxUser.Openid, awardsModel.id);
                    saveUserAwardReceiveInfo(wxUser.Openid, awardsModel.id);
                    return View("GiveMoney");
                }
                else
                {
                    //return View("Nogift");
                    Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                    return empty;
                }

            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("获取用户微信Openid,发现金异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(request.url + "&flag=1&guid=" + guid + "");
                //return View("Nogift");
                return empty;
            }
        }

        /// <summary>
        /// 发现金
        /// </summary>
        /// <param name="awardsType"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bool GiveCash(string awardsType, string openid, RequestModel request)
        {
            bool flag = false;
            try
            {
                string iMoney = "";
                if (string.IsNullOrWhiteSpace(awardsType))
                {
                    //Response.Redirect(request.url);
                    return false;
                }
                if (awardsType.Contains("1元"))
                {
                    iMoney = "100";
                }
                else if (awardsType.Contains("1.8元"))
                {
                    iMoney = "180";
                }
                else if (awardsType.Trim() == "2.8元现金红包")
                {
                    iMoney = "280";
                }
                else if (awardsType.Trim() == "6.8元现金红包")
                {
                    iMoney = "680";
                }
                else if (awardsType.Trim() == "8.8元现金红包")
                {
                    iMoney = "880";
                }

                //发钱
                if (SendCash(openid, iMoney, request))
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("发放现金异常：异常信息-{0},活动id-{1}", ex.ToString(), request.activityId));
            }
            return flag;
        }

        #endregion
    }
}
