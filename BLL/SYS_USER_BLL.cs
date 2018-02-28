using Common;
using Common.Enum;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BLL
{
    public class SYS_USER_BLL : Base_BLL
    {
        I_SYS_USER_DAL idal;

        //构造函数或者类的Setter访问器 ， 参数为实现接口的类的实例
        public SYS_USER_BLL(I_SYS_USER_DAL i_param)
        {
            idal = i_param;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="isSSO"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public LoginStatus Login(string loginName, string password, bool isSSO, ref string message)
        {
            return idal.Login(loginName, password, isSSO, ref message);
        }

        /// <summary>
        /// 注销登陆
        /// </summary>
        public void Logout()
        {
            idal.Logout();
        }
    }
}
