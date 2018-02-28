using Common;
using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
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
    public class CashRedPocketController : Controller
    {    
        /// <summary>
        ///// IResponseMessageBase
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="totalnum"></param>
        /// <returns></returns>
        public bool SendCash(string openid, string totalnum,RequestModel request)
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
                string jsonResult = JsonConvert.SerializeXmlNode(doc);
                
                XmlSerializer serializer = new XmlSerializer(typeof(Senparc.Weixin.MP.AdvancedAPIs.Result));
                Common.Helper.Logger.Info("CashRedPocketController-SendCash ：" + jsonResult + "cash");
              
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
    }
}
