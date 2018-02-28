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
    public class TRP_QRCodeScanLimited_BLL
    {
        I_TRP_QRCodeScanLimited_DAL idal;

        /// <summary>
        /// 线程锁
        /// </summary>
        private static object asyncLock = new object();

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_QRCodeScanLimited_BLL(I_TRP_QRCodeScanLimited_DAL i_param)
        {
            idal = i_param;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityId"></param>
        public bool IsOverTime(string activityId, string QRRCode, int limitedTime)
        {
            lock (asyncLock)
            {
                bool isExpiryDate = false;
                try
                {
                    if (!string.IsNullOrWhiteSpace(activityId) &&
                        !string.IsNullOrWhiteSpace(QRRCode))
                    {

                        Expression<Func<TRP_QRCodeScanLimited, bool>> exp = a => a.ActivityId == activityId;
                        Expression<Func<TRP_QRCodeScanLimited, bool>> exp1 = a => a.QRRCode == QRRCode;
                        //多条件拼装
                        exp = CompileLinqSearch.AndAlso<TRP_QRCodeScanLimited>(exp, exp1);
                        var iquerable = idal.FindBy(exp);
                        var list = iquerable.ToList();
                        if (list.Count == 0)
                        {
                            TRP_QRCodeScanLimited model = new TRP_QRCodeScanLimited();
                            model.ActivityId = activityId;
                            model.QRRCode = QRRCode;
                            model.LimitedCount = 1;
                            model.UpdateTime = DateTime.Now;
                            idal.Add(model);
                            idal.Save();
                            isExpiryDate = true;
                        }
                        else
                        {
                            TRP_QRCodeScanLimited model = list.FirstOrDefault();
                            model.LimitedCount = model.LimitedCount + 1;
                            model.UpdateTime = DateTime.Now;
                            idal.Edit(model);
                            idal.Save();
                            if (Convert.ToInt32(model.LimitedCount) <= limitedTime)
                            {
                                isExpiryDate = true;
                            }
                            else
                            {
                                isExpiryDate = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(string.Format("红包打开计数异常，异常信息：{0}", ex.ToString()));
                }
                return isExpiryDate;
            }
        }
    }
}
