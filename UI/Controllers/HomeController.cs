using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.Title = "首页";

            //if (Request.Cookies["lastVisit"] != null)
            //{
            //    HttpCookie aCookie = Request.Cookies["lastVisit"];
            //    Server.HtmlEncode(aCookie.Value);
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Login");

            //}
            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Title = "首页";
            return View();
        }

    }
}
