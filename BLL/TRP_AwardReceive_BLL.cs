using Common;
using Common.Helper;
using EFModel;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BLL
{
    public class TRP_AwardReceive_BLL
    {
        I_TRP_AwardReceive_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_AwardReceive_BLL(I_TRP_AwardReceive_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TRP_AwardReceive GetByAwardDetailId(string id)
        {
            TRP_AwardReceive model = null;
            try
            {
                var efid = 0;
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    efid = Convert.ToInt32(id);
                    exp = a => a.AwardDetailId == efid;
                }

                model = idal.FindBy(exp).ToList()[0];
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("异常信息:{0},id:{1}", ex.ToString(), id));
            }

            return model;
        }

        /// <summary>
        /// 领奖
        /// </summary>
        /// <param name="model"></param>
        public bool update(string id, string openid)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var efid = Convert.ToInt32(id);
                    exp = a => a.AwardDetailId == efid;
                    tempexp = a => a.OpenId == openid;
                    
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                    Dictionary<string, object> dic = new Dictionary<string, object>();

                    tempexp = a => a.ReceiveTime.Equals(null);

                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                    dic.Add("ReceiveTime", DateTime.Now);
                    idal.update(exp, dic);
                    flag = true;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("核销领奖异常，异常信息:{0}", ex.ToString()));
            }
            return flag;
        }

        /// <summary>
        /// 扫码
        /// </summary>
        /// <param name="model"></param>
        public bool saveScan(string openId, string id)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var efid = Convert.ToInt32(id);
                    exp = a => a.AwardDetailId == efid;
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("OpenId", openId);
                    dic.Add("SubmitTime", DateTime.Now);
                    dic.Add("Phone", "11111111111");
                    idal.update(exp, dic);
                    flag = true;
                }

            }
            catch (Exception ex)
            {
                ;
                Logger.Error(string.Format("保存扫码信息异常，异常信息:{0}", ex.ToString()));
            }
            return flag;
        }

        /// <summary>
        /// 此微信号是否已参加本次活动
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public bool isExistWxByActivity(string activityId, string openId)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(activityId))
                {
                    int dbActivityId = Convert.ToInt32(activityId);
                    tempexp = a => a.ActivityId == dbActivityId;
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                }
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    tempexp = a => a.OpenId == openId;
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                }

                var list = idal.FindBy(exp).ToList();
                if (list.Count > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("此微信号是否已参加本次活动异常，异常信息:{0},活动Id:{1}", ex.ToString(), activityId));
            }
            return flag;
        }


        /// <summary>
        /// 判断此微信号参加是否在规定次数以内
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="openId"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public bool isAttendWxByActivity(string activityId, string openId,int times)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(activityId))
                {
                    int dbActivityId = Convert.ToInt32(activityId);
                    tempexp = a => a.ActivityId == dbActivityId;
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                }
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    tempexp = a => a.OpenId == openId;
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                }

                tempexp = a => a.ReceiveTime!=null;
                //多条件拼装
                exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                int attendtimes = idal.FindBy(exp).ToList().Count;
                if (attendtimes >= times)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断此微信号参加是否在规定次数以内异常，异常信息:{0},活动Id:{1}", ex.ToString(), activityId));
            }
            return flag;
        }

        /// <summary>
        /// 获取此微信是否已参加了红包活动
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isExist(string openId)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp1 = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> exp2 = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> exp3 = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    DateTime satrt = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  00:00:00");
                    DateTime end = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd ") + "  23:59:59");
                    exp1 = a => a.OpenId == openId;
                    exp2 = a => a.SubmitTime > satrt;
                    exp3 = a => a.SubmitTime < end;
                    var list = idal.FindBy(exp1, exp2, exp3).ToList();
                    if (list.Count > 0)
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return flag;
        }

        /// <summary>
        /// 奖品是否已领取
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isGetAward(string activityId,string giftid)
        {
            bool flag = false;
            try
            {

                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;

                exp = a => a.SubmitTime != null;
                int efgiftId = Convert.ToInt32(giftid);
                tempexp = a => a.AwardDetailId == efgiftId;
                exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                if (!string.IsNullOrWhiteSpace(activityId))
                {
                    int efId = Convert.ToInt32(activityId);
                    tempexp = a => a.ActivityId == efId;
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                }

                var list = idal.FindBy(exp).ToList();
                if (list.Count > 0)
                {
                    flag = true;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断奖品是否已领取异常，异常信息{0}", ex.ToString()));
            }

            return flag;
        }

        /// <summary>
        /// 奖品是否已领取
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isGetAward(string giftid)
        {
            bool flag = false;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;

               
                int efgiftId = Convert.ToInt32(giftid);

                exp = a => a.AwardDetailId == efgiftId;

                var list = idal.FindBy(exp).ToList();
                if (list.Count > 0)
                {
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断奖品是否已领取异常，异常信息{0}", ex.ToString()));
            }

            return flag;
        }

        /// <summary>
        /// 奖品是否已领取
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public bool isTakeAward(string openId, string activityId)
        {
            bool flag = false;
            try
            {

                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    tempexp = a => a.OpenId == openId.Trim();
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                }


                tempexp = a => a.ReceiveTime != null;
                exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                if (!string.IsNullOrWhiteSpace(activityId))
                {
                    int efId = Convert.ToInt32(activityId);
                    tempexp = a => a.ActivityId == efId;
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                }

                var list = idal.FindBy(exp).ToList();
                if (list.Count > 0)
                {
                    flag = true;
                }

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断奖品是否已领取异常，异常信息{0}", ex.ToString()));
            }

            return flag;
        }


        /// <summary>
        /// 奖品还未领取
        /// ReceiveTime=null,openId=value,activityId=value
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="activityId"></param>
        /// <returns>奖品未领 返回model 否则返回null</returns>
        public TRP_AwardReceive hadTakeAward(string openId, string activityId)
        {
            TRP_AwardReceive model = null;
            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                Expression<Func<TRP_AwardReceive, bool>> tempexp = a => 1 == 1;
                tempexp = a => a.ReceiveTime.Equals(null);
                exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                if (!string.IsNullOrWhiteSpace(openId))
                {
                    tempexp = a => a.OpenId == openId.Trim();
                    //多条件拼装
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);

                }

                if (!string.IsNullOrWhiteSpace(activityId))
                {
                    int efId = Convert.ToInt32(activityId);
                    tempexp = a => a.ActivityId == efId;
                    exp = CompileLinqSearch.AndAlso<TRP_AwardReceive>(tempexp, exp);
                }

                
            
                var list = idal.FindBy(exp).ToList();
                if (list.Count > 0)
                {
                    model = list.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("判断奖品是否已领取异常，异常信息{0}", ex.ToString()));
            }

            return model;
        }


        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="activityId"></param>
        public void LogicDelete(string activityId)
        {

            try
            {
                Expression<Func<TRP_AwardReceive, bool>> exp = a => 1 == 1;
                int efActivityId = Convert.ToInt32(activityId);
                exp = a => a.ActivityId == efActivityId;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("OpenId", null);
                dic.Add("SubmitTime", null);
                dic.Add("Phone", null);
                dic.Add("ReceiveTime", null);
                idal.update(exp, dic);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("逻辑清空数据异常，活动ID：{0}，异常信息:{1}", activityId, ex.ToString()));
            }

        }
    }
}
