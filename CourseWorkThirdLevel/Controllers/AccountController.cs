using CourseWorkThirdLevel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CourseWorkThirdLevel.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (Kurs3Entities db = new Kurs3Entities())
                {
                    user = db.Users.FirstOrDefault(u => u.UserLogin == model.UserLogin);
                }
                if (user == null)
                {
                    using (Kurs3Entities db = new Kurs3Entities())
                    {
                        db.Users.Add(new User { UserLogin = model.UserLogin, UserPassword = model.UserPassword, FirstName = model.FirstName, SecondName = model.SecondName });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.UserLogin == model.UserLogin && u.UserPassword == model.UserPassword).FirstOrDefault();
                    }

                    if(user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserLogin, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                using (Kurs3Entities db = new Kurs3Entities())
                {
                    user = db.Users.FirstOrDefault(u => u.UserLogin == model.UserLogin && u.UserPassword == model.UserPassword);

                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.UserLogin, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }
    }
}