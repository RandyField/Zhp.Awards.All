using Common.Helper;
using EFModel;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class TRP_AwardDetail_BLL
    {
        I_TRP_AwardDetail_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_AwardDetail_BLL(I_TRP_AwardDetail_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 获取奖品详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TRP_AwardDetail GetEntityById(string id)
        {
            TRP_AwardDetail model = null;
            try
            {
                var efid = 0;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    efid = Convert.ToInt32(id.Trim());
                }

                model = idal.Find(efid);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("获取奖品详情异常,异常信息:{0},id:{1}",ex.ToString(),id));
            }

            return model;
        }
    }
}
