using NinjectDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class ScanCountController : Controller
    {
        [HttpGet]
        public void Index()
        {
            TRP_ScanCount_DI di = new TRP_ScanCount_DI();
            //di.getBll().CountByUrl("http://mg.dzing.com.cn/show/xwt19");
            Response.Redirect("http://mg.dzing.com.cn/show/xwt19");
        }

        [HttpGet]
        public void Count(string activityId)
        {
            TRP_ScanCount_DI di = new TRP_ScanCount_DI();
            di.getBll().CountById(activityId);
        }
    }
}
