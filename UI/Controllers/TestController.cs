using Common;
using EFModel;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class TestController : BaseController
    {

        public ActionResult Index()
        {
            for (int i = 0; i < 500; i++)
            {
                AwardsInfoModel model = GetAwardsInfo("171");
                Common.Helper.Logger.Info(JsonConvert.SerializeObject(model));
            }
            return View();
        }
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
        public ActionResult Activity(string activityId, string flag, string guid)
        {
            /*
            if (!string.IsNullOrWhiteSpace(guid))
            {
                if (!IsInDate(activityId, guid, 10))
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
                    scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】");
                }
                else
                {
                    if (flag.Trim() == "1")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】发生重定向-现金红包跳转)");
                    }
                    else
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】发生重定向-非现金红包跳转)");
                    }

                }

                //日志记录公共部分
                AwardsInfoModel awardsModel = new AwardsInfoModel();
                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/Test/Activity";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(activityId);

                //请求奖品
                awardsModel = GetAwardsInfo(activityId);

                //奖品实体为空
                if (awardsModel == null)
                {
                    Dictionary<int, string> dic = new Dictionary<int, string>();
                    dic.Add(1, "https://shop13299622.wxrrd.com/coupon/get_code/68929");
                    dic.Add(2, "https://shop13299622.wxrrd.com/coupon/get_code/68928");
                    dic.Add(3, "https://shop13299622.wxrrd.com/coupon/get_code/68927");
                    dic.Add(4, "https://shop13299622.wxrrd.com/coupon/get_code/68926");
                    dic.Add(5, "https://shop13299622.wxrrd.com/coupon/get_code/68925");
                    dic.Add(6, "https://shop13299622.wxrrd.com/coupon/get_code/68924");

                    dic.Add(7, "https://shop13299622.wxrrd.com/coupon/get_code/68923");
                    dic.Add(8, "https://shop13299622.wxrrd.com/coupon/get_code/68922");
                    dic.Add(9, "https://shop13299622.wxrrd.com/coupon/get_code/68921");

                    //RedirectRandom(dic, 9);
                    scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】-取奖品awardsModel == null, 随机跳转");
                    return empty;
                }

                //奖品的Class不为空
                if (awardsModel.Class != null)
                {
                    if (awardsModel.Class == "")
                    {
                        Dictionary<int, string> dic = new Dictionary<int, string>();
                        dic.Add(1, "https://shop13299622.wxrrd.com/coupon/get_code/68929");
                        dic.Add(2, "https://shop13299622.wxrrd.com/coupon/get_code/68928");
                        dic.Add(3, "https://shop13299622.wxrrd.com/coupon/get_code/68927");
                        dic.Add(4, "https://shop13299622.wxrrd.com/coupon/get_code/68926");
                        dic.Add(5, "https://shop13299622.wxrrd.com/coupon/get_code/68925");
                        dic.Add(6, "https://shop13299622.wxrrd.com/coupon/get_code/68924");

                        dic.Add(7, "https://shop13299622.wxrrd.com/coupon/get_code/68923");
                        dic.Add(8, "https://shop13299622.wxrrd.com/coupon/get_code/68922");
                        dic.Add(9, "https://shop13299622.wxrrd.com/coupon/get_code/68921");

                        //RedirectRandom(dic, 9);

                        scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】-取奖品awardsModel.Class=='', 随机跳转");
                        return empty;
                    }

                    //如果是券 则直接写日志，跳转
                    string awardsCard = "卡";

                    #region 卡
                    if (awardsModel.Class.Contains(awardsCard))
                    {
                        Dictionary<string, string> dicStr = new Dictionary<string, string>();

                        //50元 
                        dicStr.Add("50元青色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68929");
                        dicStr.Add("50元橘色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68928");
                        dicStr.Add("50元橙色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68927");
                        dicStr.Add("50元黄色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68926");
                        dicStr.Add("50元土黄色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68925");
                        dicStr.Add("50元紫色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68924");

                        //100元
                        dicStr.Add("100元蓝色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68923");
                        dicStr.Add("100元绿色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68922");
                        dicStr.Add("100元青色卡", "https://shop13299622.wxrrd.com/coupon/get_code/68921");

                        //通票
                        //dicStr.Add("通票卡", "https://shop13299622.wxrrd.com/coupon/get_code/68930");
                        if (awardsModel.Class.Trim() == "通票卡")
                        {
                            scanCountDi.getBll().CountByNameAndId(activityId, string.Format("【热高乐园红包雨】-奖品:{0}", "100元蓝色卡"));
                            RedirectRedPocket(dicStr, "100元蓝色卡");
                        }
                        else
                        {
                            RedirectRedPocket(dicStr, awardsModel.Class);
                            scanCountDi.getBll().CountByNameAndId(activityId, string.Format("【热高乐园红包雨】-奖品:{0}", awardsModel.Class));
                        }

                        entity.Description = string.Format("用户在{0}点击红包，【热高乐园红包雨】二维码扫码，{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), awardsModel.Class);

                        //数据库记录日志
                        logDi.getBll().SaveLog(entity);

                        return empty;
                    }
                    #endregion

                    //非卡
                    else
                    {
                        //非卡重定向 至微信领奖页
                        string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/Test/WXActivity?activityId={0}&giftType={1}&giftId={2}", activityId, awardsModel.Class, awardsModel.id);

                        string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);

                        ResponseWXRedirect(urlencode);

                        return empty;
                    }
                }
                else
                {
                    Dictionary<int, string> dic = new Dictionary<int, string>();
                    dic.Add(1, "https://shop13299622.wxrrd.com/coupon/get_code/68929");
                    dic.Add(2, "https://shop13299622.wxrrd.com/coupon/get_code/68928");
                    dic.Add(3, "https://shop13299622.wxrrd.com/coupon/get_code/68927");
                    dic.Add(4, "https://shop13299622.wxrrd.com/coupon/get_code/68926");
                    dic.Add(5, "https://shop13299622.wxrrd.com/coupon/get_code/68925");
                    dic.Add(6, "https://shop13299622.wxrrd.com/coupon/get_code/68924");

                    dic.Add(7, "https://shop13299622.wxrrd.com/coupon/get_code/68923");
                    dic.Add(8, "https://shop13299622.wxrrd.com/coupon/get_code/68922");
                    dic.Add(9, "https://shop13299622.wxrrd.com/coupon/get_code/68921");

                    //RedirectRandom(dic, 9);
                    scanCountDi.getBll().CountByNameAndId(activityId, "【热高乐园红包雨】-取奖品awardsModel.Class == null, 随机跳转");
                    return empty;
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("【热高乐园红包雨】进入领奖页面异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/Activity?activityId={0}&flag={1}", activityId, 2));
                return empty;
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
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/ReGaoPark/WXActivity";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion

                #region 获取微信用户信息 昵称，头像等

                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {
                    //非卡重定向 至微信领奖页
                    string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/WXActivity?activityId={0}&giftType={1}&giftId={2}", request.activityId, request.giftType, request.giftId);
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
                        Dictionary<int, string> dic = new Dictionary<int, string>();
                        dic.Add(1, "https://shop13299622.wxrrd.com/coupon/get_code/68929");
                        dic.Add(2, "https://shop13299622.wxrrd.com/coupon/get_code/68928");
                        dic.Add(3, "https://shop13299622.wxrrd.com/coupon/get_code/68927");
                        dic.Add(4, "https://shop13299622.wxrrd.com/coupon/get_code/68926");
                        dic.Add(5, "https://shop13299622.wxrrd.com/coupon/get_code/68925");
                        dic.Add(6, "https://shop13299622.wxrrd.com/coupon/get_code/68924");
                        //RedirectRandom(dic, 6);
                        scanCountDi.getBll().CountByNameAndId(request.activityId, "【热高乐园红包雨】-实物已领取, 随机跳转");
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
                            awardsName = detailModel.AwardName;
                        }

                        //奖品类型
                        if (!string.IsNullOrWhiteSpace(awardsName))
                        {
                            string typeCode = "";
                            typeCode = "A";
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
                    string awardsType = request.giftType;
                    string typeCode = "";
                    if (awardsType.Contains("笔记本"))
                    {
                        typeCode = "A";
                    }
                    else
                    {
                        Common.Helper.Logger.Info(string.Format("【热高乐园红包雨】,用户获取奖品:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型,发生了重定向"));
                        Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/Activity?activityId={0}&flag={1}", request.activityId, 2));
                        return empty;
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


                entity.Description = string.Format("【热高乐园红包雨】,用户在{0}点击红包，二维码扫码进入实物领奖页面，奖品为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.giftType);
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
                Common.Helper.Logger.Info(string.Format("【热高乐园红包雨】领取实物异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/Activity?activityId={0}&flag={1}", request.activityId, 2));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public ActionResult Cash(string activityId, string guid)
        {
            /*
            if (!string.IsNullOrWhiteSpace(guid))
            {
                if (!IsInDate(activityId, guid, 10))
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
                string activityName = "热高乐园现金红包";
                string TenPaySender = "热高乐园";
                string TenPayWish = "欢乐无界限，乐高常相伴";
                string TenPayGame = "热高乐园,开园献礼";
                string TenPayRemark = "热高乐园";
                string responseurl = "http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/Activity?activityId=125";
                string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/GiveMoney?activityId={0}&" +
                "activityName={1}&TenPaySender={2}&TenPayWish={3}&TenPayGame={4}&TenPayRemark={5}&" +
                "url={6}", activityId, activityName, TenPaySender, TenPayWish, TenPayGame, TenPayRemark, responseurl);

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
        public ActionResult GiveMoney(RequestModel request)
        {
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
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/ReGaoPark/GiveMoney";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion


                //获取微信用户信息
                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {
                    Response.Redirect(request.url + "&flag=1");
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
                        Response.Redirect(request.url + "&flag=1");
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
                            Response.Redirect(request.url + "&flag=1");
                            return empty;
                        }

                        //奖品类型
                        if (!string.IsNullOrWhiteSpace(awardsName))
                        {
                            GiveCash(awardsName, wxUser.Openid, request);
                        }
                        else
                        {
                            Response.Redirect(request.url + "&flag=1");
                            return empty;
                        }

                        entity.Description = string.Format("用户在{0}点击红包，现金红包（为重复扫码,上次未核销奖品）", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        entity.PageDesc = "实物扫码,上次未核销奖品";
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

                //奖品实体为null
                if (awardsModel == null)
                {
                    Response.Redirect(request.url + "&flag=1");
                    return empty;
                }

                //奖品实体的类型为null
                if (awardsModel.Class != null)
                {
                    //奖品实体的类型为""
                    if (awardsModel.Class == "")
                    {
                        Response.Redirect(request.url + "&flag=1");
                        return empty;
                    }
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
                    Response.Redirect(request.url + "&flag=1");
                    return empty;
                }

            }
            catch (Exception ex)
            {
                Response.Redirect(request.url);
                Common.Helper.Logger.Info(string.Format("获取用户微信Openid,发现金异常,异常信息：{0}", ex.ToString()));
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
                    Response.Redirect(request.url);
                    return false;
                }
                if (awardsType.Contains("1.8元"))
                {
                    iMoney = "180";
                }
                else if (awardsType.Contains("8.8元"))
                {
                    iMoney = "880";
                }
                else if (awardsType.Trim() == "88元现金红包")
                {
                    iMoney = "8800";
                }
                else if (awardsType.Trim() == "188元现金红包")
                {
                    iMoney = "18800";
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
    }
}
