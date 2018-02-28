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
    public class WdtyjController : BaseController
    {
        private const string enter_url = "http://www.chinazhihuiping.com/wxredpackets/Wdtyj/Activity";
        private const string wx_url = "http://www.chinazhihuiping.com/wxredpackets/Wdtyj/WXActivity";
        private const string money_url = "http://www.chinazhihuiping.com/wxredpackets/Wdtyj/GiveMoney";
        private const string str_a_activityname = "【万达太原街红包雨】";
        private const string str_b_activityname = "【万达太原街红包雨】发生重定向-现金红包跳转";
        private const string str_c_activityname = "【万达太原街红包雨】发生重定向-实物红包跳转";
        private const string str_d_activityname = "【万达太原街红包雨】-实物已领取";
        private const string str_e_activityname = "【万达太原街红包雨】-取奖品awardsModel.Class == \"\"";
        private const string str_f_activityname = "【万达太原街红包雨】-取奖品awardsModel.Class == null";

        private const string activityName = "【万达太原街红包雨】";
        private const string TenPaySender = "万达广场";
        private const string TenPayWish = "恭喜发财";
        private const string TenPayGame = "万达广场，红包雨";
        private const string TenPayRemark = "万达广场";

//        罗港圆
//PLAYKING
//鸡公煲
//米斯特
//兰言烫捞
//杰克琼斯
//飘蕾
//KIKC
//北纬30
//帕拉莫
//托尼魅力
//斯凯奇
//素拼礼品


        public Dictionary<string, string> awards_code = new Dictionary<string, string>(){ 
                        {"罗港圆","A"},
                        {"PLAYKING","B"},
                        {"鸡公煲","C"},
                        {"米斯特","D"},
                        {"兰言烫捞","E"},
                        {"杰克琼斯","F"},
                        {"飘蕾","G"},
                        {"KIKC","H"},
                        {"北纬30","I"},
                        {"帕拉莫","J"},
                        {"托尼魅力","K"},
                        {"斯凯奇","L"},
                        {"素拼礼品","M"}
        };



        Dictionary<string, string> r_activity = new Dictionary<string, string>(){
                        {"218","215"},
                        {"217","214"},
                        {"216","213"}
        };

        Dictionary<string, string> mondy_activity = new Dictionary<string, string>(){
                        {"2.8元现金","280"},
                        {"1.8元现金","180"},
                        {"1.0元现金","100"}
        };


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
        public ActionResult Activity(string activityId, string flag, string guid)
        {
            #region 测试


            //ViewData["Openid"] = "";
            //ViewData["wxName"] = "RandyField";
            //ViewData["AwardDetailId"] = "";
            //ViewData["AwardName"] = "";
            //ViewData["Activity"] = "";
            //ViewData["Type"] = "A";
            //return View();

            //string type = "0.88元现金红包";
            //string opienid = "omJcruMSo_al_vHgI7dQHdixsPEM";
            //RequestModel request=new RequestModel {
            //   activityName = "【中兴30周年庆现金红包】",
            //     TenPaySender = "中兴大厦",
            //     TenPayWish = "恭喜发财",
            //     TenPayGame = "30周年庆，红包雨",
            //     TenPayRemark = "中兴大厦"
            //};

            //GiveCash(type, opienid, request);

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

            #endregion

            ActionResult empty = new EmptyResult();
            try
            {
                #region 扫码计数

                if (string.IsNullOrWhiteSpace(flag))
                {
                    //扫码计数-所有
                    scanCountDi.getBll().CountByNameAndId(activityId, str_a_activityname);
                }
                else
                {
                    if (flag.Trim() == "1")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, str_b_activityname);
                    }
                    else
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, str_c_activityname);
                    }

                }

                #endregion

                #region 记录日志

                //日志记录公共部分          
                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? enter_url;
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(activityId);

                #endregion

                #region 重定向至微信授权URL


                //重定向至微信领奖页url，把奖品信息参数包裹
                string url = string.Format("{0}?activityId={1}", wx_url, activityId);

                //url encode编码
                string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);

                //重定向 微信授权url
                ResponseWXRedirect(urlencode);

                #endregion

                return empty;
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("{0}进入领奖页面异常,异常信息：{1}", str_a_activityname, ex.ToString()));
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
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? wx_url;
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion

                #region 获取微信用户信息 昵称，头像等

                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {
                    //微信用户信息未获取到 重定向至活动入口Action
                    string url = string.Format("{0}?activityId={1}", enter_url, request.activityId);
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
                        scanCountDi.getBll().CountByNameAndId(request.activityId, str_d_activityname);
                        return View("HadAttend");
                    }
                    #endregion

                    #region 奖品还未核销 返回Activity视图

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
                            if (awards_code.ContainsKey(awardsName.Trim()))
                            {
                                typeCode = awards_code[awardsName.Trim()];
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
                else
                {
                    //请求奖品
                    AwardsInfoModel awardsModel = GetAwardsInfo(request.activityId);

                    //奖品的Class不为空
                    if (awardsModel.Class != null)
                    {
                        if (awardsModel.Class == "")
                        {
                            scanCountDi.getBll().CountByNameAndId(request.activityId, str_e_activityname);
                            return View("Nogift");
                        }
                        else
                        {
                            string awardsType = awardsModel.Class;
                            string typeCode = "";

                            //Common.Helper.Logger.Info("--test--");
                            //Common.Helper.Logger.Info(awardsModel.Class);
                            //Common.Helper.Logger.Info(awards_code.ContainsKey(awardsModel.Class.Trim()));
                            if (awards_code.ContainsKey(awardsModel.Class.Trim()))
                            {
                                //Common.Helper.Logger.Info(awards_code[awardsModel.Class.Trim()]);
                                typeCode = awards_code[awardsModel.Class.Trim()];
                            }
                            else
                            {
                                return View("Nogift");
                            }
                            ViewData["Type"] = typeCode;

                            //不存在微信用户
                            if (!isExistOpenId(wxUser.Openid))
                            {
                                //保存用户微信信息
                                saveUserInfo(wxUser);
                            }

                            //保存扫码信息
                            saveScanInfo(wxUser.Openid, awardsModel.id);

                            string awardName = awardsType;
                            entity.Description = string.Format("{0},用户在{1}点击红包，二维码扫码进入实物领奖页面，奖品为{1}", str_a_activityname, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), awardsType);
                            entity.PageDesc = string.Format("实物扫码,得到{0}", awardName);
                            entity.ActivityId = int.Parse(request.activityId);
                            logDi.getBll().SaveLog(entity);

                            ViewData["Openid"] = wxUser.Openid ?? "";
                            ViewData["wxName"] = wxUser.Nickname ?? "";
                            ViewData["AwardDetailId"] = awardsModel.id ?? "";
                            ViewData["AwardName"] = awardsModel.Class ?? "";
                            ViewData["Activity"] = request.activityId ?? "";
                            return View("Activity");
                        }
                    }
                    else
                    {
                        scanCountDi.getBll().CountByNameAndId(request.activityId, str_f_activityname);
                        return View("Nogift");
                    }
                }
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
                Common.Helper.Logger.Info(string.Format("{0}领取实物异常,异常信息：{1}", str_a_activityname, ex.ToString()));
                Response.Redirect(string.Format("{0}?activityId={1}&flag={2}", enter_url, request.activityId, 2));
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
    }
}
