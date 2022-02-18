using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseWorkThirdLevel.Controllers
{
    public class HomeController : Controller
    {
        Kurs3Entities ent = new Kurs3Entities();

        public ActionResult Index()
        {
            return View(ent.Users);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            ent.Dispose();
            base.Dispose(disposing);
        }
    }
}