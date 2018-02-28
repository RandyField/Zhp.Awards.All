using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace UI
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Application["OnLineUserCount"] = 0;
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //使用Bundle来引用css有个好处 就是可以把多个css文件在一起请求，浏览器只发一次请求
            BundleTable.EnableOptimizations = true;//启用优化

        }

        //protected void Session_Start(object sender, EventArgs e)
        //{
        //    Application.Lock();
        //    Application["OnLineUserCount"] = Convert.ToInt32(Application["OnLineUserCount"]) + 1;
        //    Application.UnLock();
        //}

        //protected void Session_End(object sender, EventArgs e)
        //{
        //    Application.Lock();
        //    Application["OnLineUserCount"] = Convert.ToInt32(Application["OnLineUserCount"]) - 1;
        //    Application.UnLock();
        //}
    }
}