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
    public class YHEyeController : BaseController
    {
        /// <summary>
        /// 活动进入页面
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="flag"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Index(string activityId, string flag, string guid)
        {         
            /*
            if (!string.IsNullOrWhiteSpace(guid))
            {
                if (!IsInDate(activityId, guid, 5))
                {
                    return View("OutOfDate");
                }
            }
            //未传guid
            else
            {
                return View("OutOfDate");
            }
             * */

            ActionResult empty = new EmptyResult();
            try
            {
                if (string.IsNullOrWhiteSpace(flag))
                {
                    //扫码计数-所有
                    scanCountDi.getBll().CountByNameAndId(activityId, "【银海眼科红包雨】");
                }
                else
                {
                    if (flag=="2")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, "【银海眼科红包雨】WXActivity跳转");
                    }
                    else if (flag == "1")
                    {
                        scanCountDi.getBll().CountByNameAndId(activityId, "【银海眼科红包雨】Index跳转");
                    }              
                }

                //日志记录公共部分
                AwardsInfoModel awardsModel = new AwardsInfoModel();
                TRP_ClientLog entity = new TRP_ClientLog();
                entity.CreateTime = DateTime.Now;
                entity.DeleteMark = false;
                entity.Enable = true;
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/wxredpackets/YHEye/Index";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(activityId);

                //请求奖品
                awardsModel = GetAwardsInfo(activityId);

                //奖品实体为空
                if (awardsModel == null)
                {
                    Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", activityId, 1));
                    return empty;
                }

                //奖品的Class不为空
                if (awardsModel.Class != null)
                {
                    if (awardsModel.Class == "")
                    {
                        Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", activityId, 1));
                        return empty;
                    }

                    //非卡重定向 至微信领奖页
                    string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/WXActivity?activityId={0}&giftType={1}&giftId={2}", activityId, awardsModel.Class, awardsModel.id);

                    string urlencode = System.Web.HttpUtility.UrlEncode(url, System.Text.Encoding.UTF8);

                    ResponseWXRedirect(urlencode);

                    return empty;

                }
                else
                {
                    Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", activityId, 1));                           
                    return empty;
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("【银海眼科红包雨】进入领奖页面异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", activityId, 1));
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
                entity.PageUrl = HttpContext.Request.Url.AbsoluteUri ?? "http://www.chinazhihuiping.com/YHEye/WXActivity";
                entity.IPAddress = HttpContext.Request.UserHostAddress ?? "127.0.0.1";
                entity.ActivityId = Convert.ToInt32(request.activityId);

                #endregion

                #region 获取微信用户信息 昵称，头像等

                wxUserInfoModel wxUser = GetWxUserInfo(request.code);

                if (wxUser == null)
                {
                    //重定向 至微信领奖页
                    string url = string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/WXActivity?activityId={0}&giftType={1}&giftId={2}", request.activityId, request.giftType, request.giftId);
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
                        scanCountDi.getBll().CountByNameAndId(request.activityId, "【银海眼科红包雨】-实物已领取");
                        return View("HasAttend");
                        //return empty;
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
                            if (awardsName.Contains("指甲刀"))
                            {
                                typeCode = "A";
                            }
                            else if (awardsName.Contains("小风扇"))
                            {
                                typeCode = "B";
                            }
                            else if (awardsName.Contains("100元"))
                            {
                                typeCode = "C";
                            }
                            else if (awardsName.Contains("眼保仪"))
                            {
                                typeCode = "D";
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
                        return View("WXActivity");
                    }
                    #endregion
                }

                #endregion

                #region 首次参加活动
                if (request.giftType != null)
                {
                    string awardsType = request.giftType;
                    string typeCode = "";
                    if (awardsType.Contains("指甲刀"))
                    {
                        typeCode = "A";
                    }
                    else if (awardsType.Contains("小风扇"))
                    {
                        typeCode = "B";
                    }
                    else if (awardsType.Contains("100元"))
                    {
                        typeCode = "C";
                    }
                    else if (awardsType.Contains("眼保仪"))
                    {
                        typeCode = "D";
                    }                  
                    else
                    {
                        Common.Helper.Logger.Info(string.Format("【银海眼科红包雨】,用户获取奖品:微信用户-OpenId:{0}-领取奖品，未接收到奖品类型,发生了重定向"));
                        Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", request.activityId, 2));
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

                entity.Description = string.Format("【银海眼科红包雨】,用户在{0}点击红包，二维码扫码进入实物领奖页面，奖品为{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), request.giftType);
                entity.PageDesc = string.Format("实物扫码,得到{0}", awardName);
                entity.ActivityId = Convert.ToInt32(DESEncrypt.Decrypt(request.giftId, _key));
                logDi.getBll().SaveLog(entity);

                ViewData["Openid"] = wxUser.Openid ?? "";
                ViewData["wxName"] = wxUser.Nickname ?? "";
                ViewData["AwardDetailId"] = request.giftId ?? "";
                ViewData["AwardName"] = request.giftType ?? "";
                ViewData["Activity"] = request.activityId ?? "";
                return View();

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
                Common.Helper.Logger.Info(string.Format("【银海眼科红包雨】领取实物异常,异常信息：{0}", ex.ToString()));
                Response.Redirect(string.Format("http://www.chinazhihuiping.com/wxredpackets/YHEye/Index?activityId={0}&flag={1}", request.activityId, 2));
                return empty;
            }
        }

    }
}
