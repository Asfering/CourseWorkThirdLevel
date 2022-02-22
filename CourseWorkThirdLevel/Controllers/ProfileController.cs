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
            /*var documents = from d in ent.Documents
                            join f in ent.Favorites on d.Id equals f.IdDocument
                            where f.IdUser == id
                            select new DocumentClass { d.Id, d.Title } ;
            */



            /*foreach(var did in documents)
            {
                Console.WriteLine(did.Id + " " + did.Title);
            }*/


            /*var FavTitle = ent.Documents.Join(ent.Favorites,
                d => d.Id,
                f => f.IdDocument,
                (d, f) => new
                {
                    Id = d.Id,
                    Title = d.Title
                });
            
            ViewBag.Favorites = FavTitle;*/
            //ViewBag.FavoritesTitle = ent.Database.ExecuteSqlCommand($"Select Title from Documents inner join Favorites on Favorites.IdDocument = Id where Favorites.IdUser = {id}");
            //ViewBag.FavoritesTitle = ent.Documents.SqlQuery("Select Title from Documents inner join Favorites on Favorites.IdDocument = Id where Favorites.IdUser = 2").ToList();

            //ViewBag.FavoritesId = ent.Documents.SqlQuery("Select Id from Documents " +
            //   $"inner join Favorites on Favorites.IdDocument = Id where Favorites.IdUser = {id}").ToList();

            /*var documents = ent.Documents.Join(ent.Favorites,
               d => d.Id,
               f => f.IdDocument,
               (d, f) => new DocumentClass
               {
                   UserId = f.IdUser,
                   Id = d.Id,
                   Title = d.Title
               }).Where(e => e.UserId == id);

           ViewBag.Documents = documents;*/

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                // id - ID пользователя, владельца профиля
                user = ent.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();    // получение твоего профиля
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
                    ).Where(e=>e.UsId == id).ToList();
                // Достаем комментарии пользователя
                var docComments = ent.Documents.Join(ent.Comments,
                    d => d.Id,
                    e => e.IdDocument,
                    (d, e) => new DocumentModel
                    {
                        Id = d.Id,
                        Title = d.Title,
                        UsId = e.IdUser,
                        Likes = e.Likes,
                        Dislikes = e.Dislikes,
                        IdComment = e.Id,
                        Comment = e.Msg,
                    }
                    ).Where(e => e.UsId == id).ToList();

                ViewBag.DocEvaluations = docEvaluations;
                ViewBag.DocFavorites = docFavorites;
                ViewBag.DocComments = docComments;
                ViewBag.YourId = user.Id;
                ViewBag.Id = id;
                
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}