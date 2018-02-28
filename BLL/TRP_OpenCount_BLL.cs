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
    public class TRP_OpenCount_BLL
    {
        I_TRP_OpenCount_DAL idal;

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object asyncLock = new object();

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_OpenCount_BLL(I_TRP_OpenCount_DAL i_param)
        {
            idal = i_param;
        }

        public void Count(string activityId)
        {
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityId))
                    {
                        Expression<Func<TRP_OpenCount, bool>> exp = a => a.ActivityId == activityId;
                        var iquerable = idal.FindBy(exp);
                        var list = iquerable.ToList();
                        if (list.Count == 0)
                        {
                            TRP_OpenCount model = new TRP_OpenCount();
                            model.ActivityId = activityId;
                            model.Count = 1;
                            idal.Add(model);
                            idal.Save();
                        }
                        else
                        {
                            TRP_OpenCount model = list.FirstOrDefault();
                            model.Count = model.Count + 1;
                            idal.Edit(model);
                            idal.Save();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包打开计数异常，异常信息：{0}", ex.ToString()));
                }
            }
        }

        /// <summary>
        /// 红包打开计数清零
        /// </summary>
        /// <param name="activityId"></param>
        public void ClearCount(string activityId)
        {
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityId))
                    {
                        Expression<Func<TRP_OpenCount, bool>> exp = a => a.ActivityId == activityId;
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("Count", 0);
                        idal.update(exp, dic);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包打开计数清零异常，异常信息：{0}", ex.ToString()));
                }
            }
        }
    }
}
