using Common.Helper;
using DAL;
using EFModel;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BLL
{
    public class TRP_ScanCount_BLL
    {
        I_TRP_ScanCount_DAL idal;

        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object asyncLock = new object();


        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_ScanCount_BLL(I_TRP_ScanCount_DAL i_param)
        {
            idal = i_param;

        }

        //// 定义一个静态变量来保存类的实例
        //private static TRP_ScanCount_BLL mySingBll;

        //// 定义一个标识确保线程同步
        //private static readonly object locker = new object();


        //// 定义私有构造函数，使外界不能创建该类实例
        //private TRP_ScanCount_BLL()
        //{

        //}

        ////定义公有方法提供一个全局访问点。
        //public static TRP_ScanCount_BLL GetInstance()
        //{
        //    //这里的lock其实使用的原理可以用一个词语来概括“互斥”这个概念也是操作系统的精髓
        //    //其实就是当一个进程进来访问的时候，其他进程便先挂起状态
        //    if (mySingBll == null)//区别就在这里
        //    {
        //        lock (locker)
        //        {
        //            // 如果类的实例不存在则创建，否则直接返回
        //            if (mySingBll == null)
        //            {
        //                mySingBll = new TRP_ScanCount_BLL();
        //            }
        //        }
        //    }
        //    return mySingBll;
        //}

        /// <summary>
        /// 红包扫码计数
        /// </summary>
        /// <param name="activityId">活动id</param>
        public string CountById(string activityId)
        {
            string sum = "";
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityId))
                    {
                        Expression<Func<TRP_ScanCount, bool>> exp = a => a.ActivityId == activityId;
                        Expression<Func<TRP_ScanCount, bool>> exp1 = a => a.ActivityName == null;
                        exp = CompileLinqSearch.AndAlso(exp, exp1);
                        var iquerable = idal.FindBy(exp);
                        var list = iquerable.ToList();
                        if (list.Count == 0)
                        {
                            TRP_ScanCount model = new TRP_ScanCount();
                            model.ActivityId = activityId;
                            model.Count = 1;
                            idal.Add(model);
                            idal.Save();
                            sum = model.Count.ToString();
                        }
                        else
                        {
                            TRP_ScanCount model = list.Single();
                            model.Count = model.Count + 1;
                            idal.Edit(model);
                            idal.Save();
                            sum = model.Count.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包扫码计数异常，异常信息：{0}", ex.ToString()));
                }
            }
            return sum;
        }

        /// <summary>
        /// 红包扫码计数
        /// </summary>
        /// <param name="activityName">活动名称</param>
        public void CountByName(string activityName)
        {
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityName))
                    {
                        Expression<Func<TRP_ScanCount, bool>> exp = a => a.ActivityName == activityName;
                        var iquerable = idal.FindBy(exp);
                        var list = iquerable.ToList();
                        if (list.Count == 0)
                        {
                            TRP_ScanCount model = new TRP_ScanCount();
                            model.ActivityName = activityName;
                            model.Count = 1;
                            idal.Add(model);
                            idal.Save();
                        }
                        else
                        {
                            TRP_ScanCount model = list.Single();
                            model.Count = model.Count + 1;
                            idal.Edit(model);
                            idal.Save();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包扫码计数异常，异常信息：{0}", ex.ToString()));
                }
            }
        }

        /// <summary>
        /// 根据活动id 和 活动名称 记录扫码次数
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityName"></param>
        public void CountByNameAndId(string activityId, string activityName)
        {
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityName))
                    {
                        Expression<Func<TRP_ScanCount, bool>> exp = a => 1 == 1;
                        Expression<Func<TRP_ScanCount, bool>> tempexp = a => 1 == 1;
                        if (!string.IsNullOrWhiteSpace(activityId))
                        {

                            exp = a => a.ActivityId == activityId;
                            tempexp = a => a.ActivityName == activityName;
                            //多条件拼装
                            exp = CompileLinqSearch.AndAlso<TRP_ScanCount>(tempexp, exp);
                            var iquerable = idal.FindBy(exp);
                            var list = iquerable.ToList();
                            if (list.Count == 0)
                            {
                                TRP_ScanCount model = new TRP_ScanCount();
                                model.ActivityName = activityName;
                                model.ActivityId = activityId;
                                model.Count = 1;
                                model.UpdateTime = DateTime.Now;
                                idal.Add(model);
                                idal.Save();
                            }
                            else
                            {
                                TRP_ScanCount model = list.Single();
                                model.Count = model.Count + 1;
                                model.UpdateTime = DateTime.Now;
                                idal.Edit(model);
                                idal.Save();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包扫码计数异常，活动ID{0}，活动名称{1}异常信息：{2}", activityId, activityName, ex.ToString()));
                }

            }
        }

        /// <summary>
        /// 红包扫码计数
        /// </summary>
        /// <param name="url">url</param>
        public void CountByUrl(string url,string activityname)
        {
            lock (asyncLock)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        Expression<Func<TRP_ScanCount, bool>> exp = a => a.Url.Contains(url);
                        var iquerable = idal.FindBy(exp);
                        var list = iquerable.ToList();
                        if (list.Count == 0)
                        {
                            TRP_ScanCount model = new TRP_ScanCount();
                            model.Url = url;
                            model.Count = 1;
                            model.ActivityName = activityname;
                            idal.Add(model);
                            idal.Save();
                        }
                        else
                        {
                            TRP_ScanCount model = list.FirstOrDefault();
                            model.Count = model.Count + 1;
                            model.ActivityName = activityname;
                            idal.Edit(model);
                            idal.Save();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包扫码计数异常，异常信息：{0}", ex.ToString()));
                }
            }
        }

    }
}
