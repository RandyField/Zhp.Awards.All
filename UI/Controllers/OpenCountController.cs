using NinjectDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class OpenCountController : Controller
    {
        //
        // GET: /OpenCount/

        [HttpGet]
        public void Index(string activityId)
        {
            TRP_OpenCount_DI di = new TRP_OpenCount_DI();
            di.getBll().Count(activityId);
        }

        public void LogicDelete(string activityId)
        {
            TRP_AwardReceive_DI di = new TRP_AwardReceive_DI();
            di.getBll().LogicDelete(activityId);
        }

        public void ClearOpenCount(string activityId)
        {
            TRP_OpenCount_DI di = new TRP_OpenCount_DI();
            di.getBll().ClearCount(activityId);
        }
    }
}
