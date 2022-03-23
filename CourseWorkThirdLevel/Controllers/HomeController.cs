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

        [HttpGet]
        public ActionResult Index()
        {
            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = ent.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();
                ViewBag.Id = user.Id;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string action, string FindDoc, System.DateTime? DateStart, System.DateTime? DateEnd, bool? checkBoxActual)
        {
            try
            {
                if (action == "Search")
                {
                    if(DateEnd != null && DateStart != null && (DateEnd < DateStart || DateStart > DateEnd))
                    {
                        return RedirectToAction("AllDocuments", "Document", new { FindDoc = FindDoc, checkBoxActual = checkBoxActual });
                    }
                    return RedirectToAction("AllDocuments", "Document", new { FindDoc = FindDoc, DateStart = DateStart, DateEnd = DateEnd, checkBoxActual = checkBoxActual });
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Курсовая работа Решетняка Р.М. ASP.NET + Solr. По всем вопросам - roma_r9@mail.ru";

            return View();
        }


        protected override void Dispose(bool disposing)
        {
            ent.Dispose();
            base.Dispose(disposing);
        }
    }
}