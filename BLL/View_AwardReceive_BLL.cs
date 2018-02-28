using EFModel;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common.Helper;
using Common;


namespace BLL
{
    public class View_AwardReceive_BLL
    {
        I_View_AwardReceive_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public View_AwardReceive_BLL(I_View_AwardReceive_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 是否已领
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool isExist(string id)
        {
            bool flag = false;
            try
            {           
                Expression<Func<View_AwardReceive, bool>> exp1 = a => 1 == 1;
                Expression<Func<View_AwardReceive, bool>> exp2 = a => 1 == 1;
                Expression<Func<View_AwardReceive, bool>> exp3 =  a=> 1 == 1;
                Expression<Func<View_AwardReceive, bool>> exp4 = a => 1 == 1;
                if (!string.IsNullOrWhiteSpace(id))
                {
                   
                    var ef_id = Convert.ToInt32(id);
                    exp1 = a => a.AwardDetailId == ef_id;
                    exp2 = a => a.DeleteMark == false;
                    exp3 = a => a.Enable == true;
                    exp4 = a => a.SubmitTime != null;
                    var list = idal.FindBy(exp1,exp2,exp3,exp4).ToList();
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
    }
}
