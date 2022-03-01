using CourseWorkThirdLevel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseWorkThirdLevel.Controllers
{
    public class ProfileController : Controller
    {
        Kurs3Entities ent = new Kurs3Entities();

        [Route("Profile/{id}")]
        [Authorize]
        public ActionResult GetProfile(int? id)
        {
            try
            {
                User user = null;
                if (User.Identity.IsAuthenticated)
                {
                    // id - ID пользователя, владельца профиля
                    //user = ent.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();    // получение твоего профиля
                    // получаем текущего пользователя
                    // Сделать проверку, является ли ID числом. И так везде!

                    var currentUser = ent.Users.SqlQuery($"Select * from Users where Users.Id = {id}").ToList();
                    // Достаем понравившиеся пользователю документы
                    var docFavorites = ent.Database.SqlQuery<Document>($"Select * from Documents inner join Favorites on Favorites.IdDocument = Id where Favorites.IdUser = {id}").ToList();
                    // Достает оценки пользователя
                    var docEvaluations = ent.Documents.Join(ent.Evaluations,
                        d => d.Id,
                        e => e.IdDocument,
                        (d, e) => new DocumentModel
                        {
                            Id = d.Id,
                            Title = d.Title,
                            UsId = e.IdUser,
                            LikeUnlike = e.LikeUnlike
                        }
                        ).Where(e => e.UsId == id).ToList();
                    // Достаем комментарии пользователя
                    var docComments = ent.Documents.Join(ent.Comments,
                        d => d.Id,
                        e => e.IdDocument,
                        (d, e) => new CommentModel
                        {
                            Id = d.Id,
                            Title = d.Title,
                            UsId = e.IdUser,
                            Likes = e.Likes,
                            Dislikes = e.Dislikes,
                            IdComment = e.Id,
                            Comment = e.Msg,
                            DatePublish = e.DatePublish
                        }
                        ).Where(e => e.UsId == id).ToList();

                    ViewBag.DocEvaluations = docEvaluations;
                    ViewBag.DocFavorites = docFavorites;
                    ViewBag.DocComments = docComments;
                    ViewBag.YourName = currentUser[0].FirstName + " " + currentUser[0].SecondName;
                    ViewBag.Id = currentUser[0].Id;

                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }

                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}