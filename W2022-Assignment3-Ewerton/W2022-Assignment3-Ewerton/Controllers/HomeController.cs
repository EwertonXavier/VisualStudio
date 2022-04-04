using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace W2022_Assignment3_Ewerton.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return RedirectToAction("","Teacher"); //just a small trick to get to teacher without changing configurations :D
        }
    }
}
