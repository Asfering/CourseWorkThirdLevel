using CommonServiceLocator;
using CourseWorkThirdLevel.Models;
using NUnit.Framework;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseWorkThirdLevel.Controllers
{
    public class DocumentController : Controller
    {

        Kurs3Entities Entity = new Kurs3Entities();
        // GET: Document
        [Route("AllDocuments")]
        [Authorize]
        public ActionResult AllDocuments()
        {
            // заглушка, пока выводятся все документы
            var document = Entity.Documents.SqlQuery("Select * from Documents").ToList();
            ViewBag.Document = document;
            StartSolr.StartEngine();        // подрубаем Solr
            return View();
        }


        public void Add()           // Добавление элемента в Solr, тестирование
        {
            var p = new Document
            {
                Id=15,
                Title="testing",
                Texts="testingText",
                Likes=1,
                Dislikes=2,
                DatePublish=DateTime.Now,
                DateStart=DateTime.Now,
                DateEnd=null,
                Favorites=0
            };

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
            solr.Add(p);
            solr.Commit();
        }

        public void Query()             // тестовый запрос, Solr
        {
            ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
            var results = solr.Query(new SolrQueryByField("Title", "testing"));
            Console.WriteLine(results[0].Title);
        }



        // Гет запрос
        [Route("Document/{id}")]
        [HttpGet]
        [Authorize]
        public ActionResult GetDocument(int? id)
        {
            try
            {
                // Для наглядности использую 3 разных метода работы. SQL запрос, LINQ и обычные Joinы в Entity.
                User user = Entity.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();        // получаем текущего пользователя
                var document = Entity.Documents.SqlQuery($"Select * from Documents Where Id = {id}").ToList();      // вся инфа о документе
                var docComments = from doc in Entity.Documents
                                  join comments in Entity.Comments on doc.Id equals comments.IdDocument
                                  join users in Entity.Users on comments.IdUser equals users.Id
                                  where doc.Id == id
                                  select new CommentModel               // Вспомогательная модель     
                                  {
                                      Id = doc.Id,  // ID Документа
                                      Title = doc.Title,    //Заголовок документа
                                      UsId = comments.IdUser,    // ID пользователя
                                      UsName = users.FirstName + " " + users.SecondName,    // Имя пользователя, который оставил коммент
                                      Likes = comments.Likes,    // Понравилось
                                      Dislikes = comments.Dislikes,  // Не понравилось
                                      IdComment = comments.Id,   // ID комментария
                                      Comment = comments.Msg,    // Сам комментарий
                                      DatePublish = comments.DatePublish // Дата публикации комментария
                                  };

                var alreadyInFav = Entity.Favorites.Join(Entity.Users,
                    f => f.IdUser,
                    u => u.Id,
                    (f, u) => new
                    {
                        idUs = f.IdUser,
                        idDoc = f.IdDocument
                    }).Where(w => w.idDoc == id && w.idUs == user.Id).ToList();

                var alreadyGetEval = Entity.Evaluations.Join(Entity.Users,              // получаем какую оценку поставил пользователь данному документу       
                    e => e.IdUser,
                    u => u.Id,
                    (e, u) => new
                    {
                        docId = e.IdDocument,
                        usId = e.IdUser,
                        likeUnlike = e.LikeUnlike
                    }).Join(Entity.Documents,
                    j => j.docId,
                    d => d.Id,
                    (j, d) => new AlreadyGetEvalModel
                    {
                        idUs = j.usId,
                        idDoc = j.docId,
                        LikeUnlike = j.likeUnlike
                    }).Where(e => (e.idDoc == id && e.idUs == user.Id)).ToList();

                ViewBag.BoolEval = false;       // дефолт значение
                if (alreadyGetEval.Count != 0)  // если оценка уже поставлена
                {
                    ViewBag.BoolEval = true;    // меняем на тру
                }
                ViewBag.AlreadyGetEval = alreadyGetEval;        // берем значения

                ViewBag.BoolFavorites = false;
                if (alreadyInFav.Count != 0) // если на данном аккаунте данный документ уже в избранном. СДелать такое же с лайками!
                {
                    ViewBag.BoolFavorites = true;
                }
                ViewBag.Comments = docComments;     // берем комментарии
                ViewBag.Document = document;        // берем инфу о документе
                ViewBag.YourId = user.Id;           // Твой ID
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }


        // Пост запрос
        [Route("Document/{id}")]
        [HttpPost]
        [Authorize]
        public ActionResult GetDocument (string action, int id)
        {
            User user = Entity.Users.Where(u => u.UserLogin == User.Identity.Name).FirstOrDefault();    // Получение текущего пользователя
            var document = Entity.Documents.Where(c => c.Id == id).FirstOrDefault();
            
            if (action == "Like")   // поставить лайк
            {
                document.Likes++;
                Entity.SaveChanges();

                Evaluation eval = new Evaluation
                {
                    IdUser = user.Id,
                    IdDocument = id,
                    LikeUnlike = true
                };

                Entity.Evaluations.Add(eval);
                Entity.SaveChanges();

            }
            else if (action == "UnLike")   // убрать лайк
            {
                document.Likes--;
                Entity.SaveChanges();

                var ev = Entity.Evaluations.Where(c => c.IdUser == user.Id && c.IdDocument == id).FirstOrDefault();
                Entity.Evaluations.Remove(ev);
                Entity.SaveChanges();

            }
            else if (action == "Dislike")  // поставить дизлайк
            {
                document.Dislikes++;
                Entity.SaveChanges();

                Evaluation eval = new Evaluation
                {
                    IdUser = user.Id,
                    IdDocument = id,
                    LikeUnlike = false
                };

                Entity.Evaluations.Add(eval);
                Entity.SaveChanges();

            } else if (action == "UnDislike")   // убрать дизлайк
            {
                document.Dislikes--;
                Entity.SaveChanges();

                var ev = Entity.Evaluations.Where(c => c.IdUser == user.Id && c.IdDocument == id).FirstOrDefault();
                Entity.Evaluations.Remove(ev);
                Entity.SaveChanges();

            }
            else if(action == "Comment")  // написать комментарий
            {
                Comment comm = new Comment
                {
                    IdUser = user.Id,
                    IdDocument = id,
                    Msg = Request.Form["txtArea"].ToString(),
                    DatePublish = DateTime.Now
                };
                Entity.Comments.Add(comm);
                Entity.SaveChanges();

            } else if(action == "Favorite") // Добавить в избранное
            {
                document.Favorites++;       // Для статистики 
                Entity.SaveChanges();

                Favorite fav = new Favorite
                {
                    IdUser = user.Id,
                    IdDocument = id
                };

                Entity.Favorites.Add(fav);  // Добавляем запись в таблицу
                Entity.SaveChanges();
            }
            else if (action == "DelFavorite")   // убрать из избранного
            {
                document.Favorites--;       // для статистики
                Entity.SaveChanges();

                var fav = Entity.Favorites.Where(c => c.IdDocument == id && c.IdUser == user.Id).FirstOrDefault();      // находим запись в бд
                Entity.Favorites.Remove(fav);       // удаляем
                Entity.SaveChanges();       // сохраняем изменения
            }
            //else if (action == "DeleteComment") // удаление комментария
            //{
            //    //var comm = Entity.Comments.Where(c => c.Id == IdComment).FirstOrDefault();
            //}
            return RedirectToAction("GetDocument", "Document", new { id = id });        // переходим к странице
        }
    }
}