using BLL;
using Common;
using EFModel;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NinjectDI;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using UI.Models;

namespace UI.Controllers
{
    public class BaseController : Controller
    {
        public readonly string _appId = ConfigurationManager.AppSettings["AppId"];
        public readonly string _appsecret = ConfigurationManager.AppSettings["Appsecret"];
        public readonly string _key = ConfigurationManager.AppSettings["encryption"];

        public readonly string _activityName = ConfigurationManager.AppSettings["activityname"];

        public View_AwardReceive_DI viewDi = new View_AwardReceive_DI();
        public TRF_WeChatUserInfo_DI weChatDi = new TRF_WeChatUserInfo_DI();
        public TRP_AwardReceive_DI awardDi = new TRP_AwardReceive_DI();
        public TRP_AwardDetail_DI detailDi = new TRP_AwardDetail_DI();
        public TRP_ClientLog_DI logDi = new TRP_ClientLog_DI();
        public TRP_ScanCount_DI scanCountDi = new TRP_ScanCount_DI();
        public TRP_QRCodeScanLimited_DI QRCodeScanLimitedDI = new TRP_QRCodeScanLimited_DI();

        /// <summary>
        /// 红包-跳转页面
        /// </summary>
        /// <param name="tburl"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RedirectRedPocket(Dictionary<string, string> tburl, string key)
        {
            bool flag = false;
            try
            {
                Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
                Response.BufferOutput = true;//设置输出缓冲 
                if (!Response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
                {
                    Response.Redirect(tburl[key].Trim());
                    flag = true;
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("BaseContrller,跳转异常信息：{0},key：{1},url：{2}", ex.ToString(), key, tburl[key]));
            }
            return flag;
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
        /// 扫码次数是否在有效期内
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="QRRCode"></param>
        /// <param name="limitedTime"></param>
        /// <returns></returns>
        public bool IsInDate(string activityId, string QRRCode, int limitedTime)
        {
            bool IsInDate = false;
            try
            {
                IsInDate = QRCodeScanLimitedDI.getBll().IsOverTime(activityId, QRRCode, limitedTime);
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("BaseContrller,判断二维码扫码次数有效异常，异常信息：{0}", ex.ToString()));
            }
            return IsInDate;
        }

        /// <summary>
        /// 红包-随机跳转页面
        /// </summary>
        /// <param name="tburl">url字典 1-string</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public bool RedirectRandom(Dictionary<int, string> tburl, int length, out string url)
        {
            bool flag = false;
            url = "";
            try
            {
                Random rd = new Random();
                int webIndex = rd.Next(1, length + 1);

                Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
                Response.BufferOutput = true;//设置输出缓冲 
                if (!Response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
                {
                    Response.Redirect(tburl[webIndex]);
                    url = tburl[webIndex];
                    flag = true;
                    Response.Close();
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("BaseContrller,随机跳转页面异常信息：{0}", ex.ToString()));
            }
            return flag;
        }


        /// <summary>
        /// 重定向至微信授权方式重定向
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="type"></param>
        public void ResponseWXRedirect(string url)
        {
            Response.Clear();//这里是关键，清除在返回前已经设置好的标头信息，这样后面的跳转才不会报错
            Response.BufferOutput = true;//设置输出缓冲 
            if (!Response.IsRequestBeingRedirected) //在跳转之前做判断,防止重复
            {
                Response.Redirect(string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx6953deeefe22a83b&redirect_uri={0}&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect",
                                        url), true);
                Response.Close();
            }
        }

        /// <summary>
        /// 红包-获取微信用户信息
        /// </summary>
        /// <param name="code">微信端code</param>
        /// <returns></returns>
        public wxUserInfoModel GetWxUserInfo(string code)
        {
            wxUserInfoModel model = null;

            try
            {
                HttpHelper httpHelper = new HttpHelper();

                //获取tokenjson
                string tokenJson = httpHelper.HttpGet(String.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", _appId, _appsecret, code), "");
                JObject tokenJsonObj = null;
                if (!string.IsNullOrWhiteSpace(tokenJson))
                {
                    tokenJsonObj = JObject.Parse(tokenJson);
                    if (tokenJsonObj != null)
                    {
                        string userInfoJson = "";
                        //获取用户信息json
                        userInfoJson = httpHelper.HttpGet(string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", tokenJsonObj["access_token"], tokenJsonObj["openid"]), "");
                        if (!string.IsNullOrWhiteSpace(userInfoJson))
                        {
                            //json反序列化为实体
                            model = JsonConvert.DeserializeObject<wxUserInfoModel>(userInfoJson);

                            if (model.Openid == null)
                            {
                                model = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("BaseContrller,获取用户微信信息异常，异常信息：{0}", ex.ToString()));
            }
            return model;
        }


        /// <summary>
        /// 红包-根据活动id获取奖品信息
        /// </summary>
        /// <param name="activityId">活动Id</param>
        /// <returns></returns>
        public AwardsInfoModel GetAwardsInfo(string activityId)
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
                Common.Helper.Logger.Info(string.Format("根据活动id获取奖品信息异常,异常信息：{0},活动ID：{1}", ex.ToString(), activityId));
            }
            return awardsInfo;
        }

        /// <summary>
        /// 红包-核销领奖
        /// </summary>
        /// <param name="Openid"></param>
        /// <param name="AwardDetailId"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult ReceivedAward(AwardReceivedModel model)
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
        /// 红包-是否已领奖品
        /// </summary>
        /// <param name="openId">微信</param>
        /// <param name="activityId">活动Id</param>
        /// <returns></returns>   
        public bool isTakeAward(string openId, string activityId)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            flag = bll.isTakeAward(openId, activityId);
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="giftid"></param>
        /// <returns></returns>
        public bool isGetAward(string activityId, string giftid)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            var realId = DESEncrypt.Decrypt(giftid, _key);
            flag = bll.isGetAward(activityId, realId);
            return flag;
        }


        /// <summary>
        /// 红包-保存用户领奖信息
        /// </summary>
        /// <param name="openid">微信openId</param>
        /// <param name="id">奖品Id 加密</param>
        public bool saveUserAwardReceiveInfo(string openid, string id)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            var realId = DESEncrypt.Decrypt(id, _key);
            flag = bll.update(realId, openid);
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
        ///// IResponseMessageBase
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="totalnum"></param>
        /// <returns></returns>
        public bool SendCash(string openid, string totalnum, RequestModel request)
        {
            bool success = false;
            try
            {
                var wxmin = Convert.ToInt32(ConfigurationManager.AppSettings["TenPayV3_WXMin"]);
                var wxmax = Convert.ToInt32(ConfigurationManager.AppSettings["TenPayV3_WXMax"]);
                var min = Convert.ToInt32(ConfigurationManager.AppSettings["TenPayV3_RainMin"]);
                var max = Convert.ToInt32(ConfigurationManager.AppSettings["TenPayV3_RainMax"]);
                //string TenPayV3_Sender = ConfigurationManager.AppSettings["TenPayV3_Sender"]; //红包发送者名称
                //string TenPayV3_Wish = ConfigurationManager.AppSettings["TenPayV3_Wish"]; //红包祝福语
                //string TenPayV3_Game = ConfigurationManager.AppSettings["TenPayV3_Game"]; //活动名称
                //string TenPayV3_Remark = ConfigurationManager.AppSettings["TenPayV3_Remark"]; //备注信息

                string TenPayV3_Sender = request.TenPaySender; //红包发送者名称
                string TenPayV3_Wish = request.TenPayWish; //红包祝福语
                string TenPayV3_Game = request.TenPayGame; //活动名称
                string TenPayV3_Remark = request.TenPayRemark; //备注信息

                var ret = SendCash2(openid, totalnum, TenPayV3_Sender, TenPayV3_Wish, TenPayV3_Game, TenPayV3_Remark);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);
                //string jsonResult = JsonConvert.SerializeXmlNode(doc);

                //XmlSerializer serializer = new XmlSerializer(typeof(Senparc.Weixin.MP.AdvancedAPIs.Result));
                //Common.Helper.Logger.Info("CashRedPocketController-SendCash ：" + jsonResult + "cash");

                //var rs = JsonConvert.DeserializeObject<Senparc.Weixin.MP.AdvancedAPIs.Result>(jsonResult);

                //Common.Helper.Logger.Info("OnEvent_ScanCommonCashRequest ：" + "return_code:" + rs.return_code + ";return_msg:" + rs.return_msg
                //            + ";result_code:" + rs.result_code + ";err_code:" + rs.err_code + ";err_code_des:" + rs.err_code_des
                //           + ";total_amount:" + totalnum + "cash");

                //{"#cdata-section":"FAIL"}
                //{"#cdata-section":"SUCCESSS"}

                //if (ret.Contains("<result_code><![CDATA[FAIL]]></result_code>"))
                //{
                //    //Common.Helper.Logger.Info("\"result_code\":{\"#cdata-section\":\"FAIL\"}\"");
                //    success = true;
                //}
                if (ret.Contains("<result_code><![CDATA[SUCCESS]]></result_code>"))
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("发送现金红包异常,异常信息{0}", ex.ToString()));

            }
            return success;
        }

        /// <summary>
        /// 发送现金红包
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="total_amount"></param>
        /// <param name="TenPayV3_Sender"></param>
        /// <param name="TenPayV3_Wish"></param>
        /// <param name="TenPayV3_Game"></param>
        /// <param name="TenPayV3_Remark"></param>
        /// <returns></returns>
        public string SendCash2(string openid, string total_amount, string TenPayV3_Sender, string TenPayV3_Wish, string TenPayV3_Game, string TenPayV3_Remark)
        {
            try
            {
                var mchbillno = DateTime.Now.ToString("HHmmss") + TenPayV3Util.BuildRandomStr(28);
                var nonceStr = TenPayV3Util.GetNoncestr();
                var packageReqHandler = new RequestHandler(null);

                //设置package订单参数
                string TenPayV3_AppId = ConfigurationManager.AppSettings["TenPayV3_AppId"];
                string TenPayV3_MchId = ConfigurationManager.AppSettings["TenPayV3_MchId"];
                string TenPayV3_Key = ConfigurationManager.AppSettings["TenPayV3_Key"];
                packageReqHandler.SetParameter("nonce_str", nonceStr); //随机字符串
                //packageReqHandler.SetParameter("wxappid", "wx6953deeefe22a83b"); //公众账号ID
                packageReqHandler.SetParameter("wxappid", TenPayV3_AppId); //公众账号ID
                packageReqHandler.SetParameter("mch_id", TenPayV3_MchId); //商户号
                packageReqHandler.SetParameter("mch_billno", mchbillno); //填入商家订单号
                packageReqHandler.SetParameter("send_name", TenPayV3_Sender); //红包发送者名称
                packageReqHandler.SetParameter("re_openid", openid); //接受收红包的用户的openId
                packageReqHandler.SetParameter("total_amount", total_amount); //付款金额，单位分
                packageReqHandler.SetParameter("total_num", "1"); //红包发放总人数
                packageReqHandler.SetParameter("wishing", TenPayV3_Wish); //红包祝福语
                packageReqHandler.SetParameter("client_ip", System.Web.HttpContext.Current.Request.UserHostAddress); //调用接口的机器Ip地址
                packageReqHandler.SetParameter("act_name", TenPayV3_Game); //活动名称
                packageReqHandler.SetParameter("remark", TenPayV3_Remark); //备注信息
                var sign = packageReqHandler.CreateMd5Sign("key", TenPayV3_Key);
                packageReqHandler.SetParameter("sign", sign); //签名

                //最新的官方文档中将以下三个字段去除了
                //packageReqHandler.SetParameter("nick_name", "提供方名称");                 //提供方名称
                //packageReqHandler.SetParameter("max_value", "100");                //最大红包金额，单位分
                //packageReqHandler.SetParameter("min_value", "100");                //最小红包金额，单位分


                //发红包需要post的数据
                var data = packageReqHandler.ParseXML();

                //发红包接口地址
                var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";

                //本地或者服务器的证书位置（证书在微信支付申请成功发来的通知邮件中）
                var cert = ConfigurationManager.AppSettings["TenPayV3_Cert"];

                //私钥（在安装证书时设置）
                var password = ConfigurationManager.AppSettings["TenPayV3_CertPass"];

                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

                //调用证书
                var cer = new X509Certificate2(cert, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                #region 发起post请求

                var webrequest = (HttpWebRequest)WebRequest.Create(url);
                webrequest.ClientCertificates.Add(cer);
                webrequest.Method = "post";

                var postdatabyte = Encoding.UTF8.GetBytes(data);
                webrequest.ContentLength = postdatabyte.Length;
                Stream stream;
                stream = webrequest.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();

                var httpWebResponse = (HttpWebResponse)webrequest.GetResponse();
                var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                var responseContent = streamReader.ReadToEnd();
                return responseContent;

                #endregion
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Info(string.Format("发送现金红包SendCash2异常,异常信息{0}", ex.ToString()));
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        /// <summary>
        /// 此微信号是否已参加本次活动
        /// activityId=value,openId is exist
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
        /// 此微信号是否已参加本次活动3次以上
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="openId"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public bool isAttendToday(string activityId, string openId, int times)
        {
            bool flag = false;
            TRP_AwardReceive_BLL bll = awardDi.getBll();
            flag = bll.isAttendWxByActivity(activityId, openId,times);
            return flag;
        }
    }
}
