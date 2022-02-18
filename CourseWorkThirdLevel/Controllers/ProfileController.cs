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

        [Route("{id}")]
        public ActionResult GetProfile(int? id)
        {
            /*var documents = from d in ent.Documents
                            join f in ent.Favorites on d.Id equals f.IdDocument
                            where f.IdUser == id
                            select new DocumentClass { d.Id, d.Title } ;
            */

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

            var doc = ent.Database.SqlQuery<Document>($"Select * from Documents inner join Favorites on Favorites.IdDocument = Id where Favorites.IdUser = {id}").ToList();

            ViewBag.Documents = doc;

            User user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = ent.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();
                // var us = ent.Users.SqlQuery("select * from Users").ToList();
                ViewBag.YourId = user.Id;
            }

            ViewBag.Id = id;


            return View();
        }
    }
}