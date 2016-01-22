using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorrWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string returnUrl)
        {
            ViewBag.listIndex = -1;
            ViewBag.eventIndex = "None";
            ViewBag.gameIndex = -1;
            ViewBag.selectedPanel = 0;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}