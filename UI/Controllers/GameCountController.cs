using NinjectDI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class GameCountController : Controller
    {
        //
        // GET: /GameCount/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string Count(string activityId)
        {
            TRP_ScanCount_DI di = new TRP_ScanCount_DI();
            string sum = di.getBll().CountById(activityId);
            return sum;
        }

    }
}
