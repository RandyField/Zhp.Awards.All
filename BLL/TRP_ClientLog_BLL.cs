using Common.Helper;
using DAL;
using EFModel;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class TRP_ClientLog_BLL
    {
        I_TRP_ClientLog_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public TRP_ClientLog_BLL(I_TRP_ClientLog_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 保存日志信息
        /// </summary>
        /// <param name="model"></param>
        public void SaveLog(TRP_ClientLog model)
        {
            try
            {
                idal.Add(model);
                idal.Save();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("写数据库日志异常，异常信息：{0}",ex.ToString()));
            }          
        }
    }
}
