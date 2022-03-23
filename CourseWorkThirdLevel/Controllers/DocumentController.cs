using CommonServiceLocator;
using CourseWorkThirdLevel.Models;
using NUnit.Framework;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CourseWorkThirdLevel.Controllers
{
    public class DocumentController : Controller
    {

        Kurs3Entities Entity = new Kurs3Entities();
        /// <summary>
        /// Get запрос Search
        /// </summary>
        /// <param name="FindDoc"></param>
        /// <param name="YearStart"></param>
        /// <param name="YearEnd"></param>
        /// <param name="checkBoxActual"></param>
        /// <returns></returns>
        [Route("Search")]
        [HttpGet]
        public ActionResult AllDocuments(string FindDoc, System.DateTime? DateStart, System.DateTime? DateEnd, bool? checkBoxActual)
        {
            try
            {
                StartSolr.StartEngine();        // подрубаем Solr
                ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();

                if(FindDoc == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                // тут надо сделать форич, который будет брать входящую строку, делить на слова и добавлять в запрос.
                string[] words = FindDoc.Split(new char[] { ' ', ',', '!', '.', '-', ';', ':', '—', '-' }, StringSplitOptions.RemoveEmptyEntries);
                string tempText = "";
                string tempTitle = "";
                foreach (var item in words)
                {
                    if (item != words[words.Length - 1])
                    {
                        tempText += $"Texts: {item} and ";
                        tempTitle += $"Title: {item} and ";
                    }
                    else if (words.Length == 1 || item == words[words.Length - 1])
                    {
                        tempText += $"Texts: {item}";
                        tempTitle += $"Title: {item}";
                    }
                }

                SolrQueryResults<Document> document = new SolrQueryResults<Document>();
                SolrQueryResults<Document> documentCrutch = new SolrQueryResults<Document>();

                
                //ДОДЕЛАТЬ!
                if (DateStart == null)
                {
                    if (DateEnd == null)
                    {
                        if (checkBoxActual == false)         // даты начала нет, даты конца нет, все актуальности
                        {
                            // верно
                            document = solr.Query($"{tempText} and {tempTitle}");          // беру всю выборку
                        }
                        else                                // даты начала нет, даты конца нет, актуальность
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                // верно
                                new SolrQuery("!DateEnd: [* TO NOW]"),          // Беру правую часть от Now
                                new SolrQuery("DateStart: [* TO NOW]")          // беру левую часть от Now
                                }
                            });
                        }
                    }
                    else
                    {
                        if (checkBoxActual == false)                 // даты начала нет, дата конца есть, все актуальности
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                // верно
                                new SolrQuery($"DateEnd: [* TO {DateEnd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateEnd.Value.TimeOfDay}Z]")           // Беру левую часть от Даты конца
                                }
                            });
                            documentCrutch = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                               {
                                new SolrQuery($"!DateEnd: *")        // Беру всё, где нет DateEnd. Т.е. должно быть актуально.
                               }
                            });

                        }
                        else                                        // даты начала нет, дата конца есть, актуальность
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                new SolrQuery($"DateEnd: [NOW TO {DateEnd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateEnd.Value.TimeOfDay}Z]"),        // Беру правую часть от Now
                                new SolrQuery("DateStart: [* TO NOW]")      // Беру левую часть от Now
                                }
                            });
                            documentCrutch = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                new SolrQuery($"!DateEnd: *"),        // Беру всё, где нет DateEnd. Т.е. должно быть актуально.
                                new SolrQuery("DateStart: [* TO NOW]")      // Беру левую часть от Now
                                }
                            });
                        }
                    }
                }
                else    // DateStart != null 
                {
                    if (DateEnd == null)
                    {
                        if (checkBoxActual == false)         // дата начала есть, даты конца нет, все актуальности
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                // верно
                                new SolrQuery($"DateStart: [{DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateStart.Value.TimeOfDay}Z TO *]")     // Беру правую часть от Начала даты
                                }
                            });
                        }
                        else                                // дата начала есть, даты конца нет, актуальность
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                               {
                               // верно
                                new SolrQuery($"!DateEnd: [* TO NOW]"),     // Беру правую часть от Now
                                new SolrQuery($"DateStart: [{DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateStart.Value.TimeOfDay}Z TO NOW]")       // Беру левую часть от Now
                               }
                            });
                        }
                    }
                    else
                    {
                        if (checkBoxActual == false)                 // дата начала есть, дата конца есть, все актуальности
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                               {
                               // верно
                                new SolrQuery($"DateStart: [{DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateStart.Value.TimeOfDay}Z TO *]"),        // Беру правую часть от начала
                                new SolrQuery($"DateEnd: [* TO {DateEnd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateEnd.Value.TimeOfDay}Z]")               // Беру левую часть от конца
                               }
                            });
                            documentCrutch = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                               {
                                new SolrQuery($"!DateEnd: *"),        // Беру всё, где нет DateEnd. Т.е. должно быть актуально.
                                new SolrQuery($"DateStart: [{DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateStart.Value.TimeOfDay}Z TO *]")
                               }
                            });
                        }
                        else                                            // дата начала есть, дата конца есть, актуальность
                        {
                            document = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                               {
                               // тут ещё надо dateend != *
                                new SolrQuery($"DateEnd: [NOW TO {DateEnd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateEnd.Value.TimeOfDay}Z]"),        // Беру правую часть от Now
                                new SolrQuery($"DateStart: [{DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}T{DateStart.Value.TimeOfDay}Z TO NOW]")   // Беру левую часть от Now
                               }
                            });
                            documentCrutch = solr.Query($"{tempText} and {tempTitle}", new QueryOptions
                            {
                                FilterQueries = new ISolrQuery[]
                                {
                                new SolrQuery($"!DateEnd: *"),        // Беру всё, где нет DateEnd. Т.е. должно быть актуально.
                                new SolrQuery("DateStart: [* TO NOW]")      // Беру левую часть от Now
                                }
                            });
                        }
                    }
                }


                //!DateEnd: * - получить все документы, где нет даты окончания
                //DateStart:[2000-11-01T00:00:00Z TO 2022-02-01T23:59:59Z] - Дата, фильтрация
                //Texts:Номера and Texts: США - Так правильнее, нежели Texts: Номера США

                ViewBag.Document = document;
                ViewBag.DocumentCrutch = documentCrutch;

                // Считываем данные из поисковика для отображения на странице.
                ViewBag.FindDoc = FindDoc;
                if (DateStart != null)
                    ViewBag.DateStart = DateStart.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);   // Дата не сохраняется(
                if (DateEnd != null)
                    ViewBag.DateEnd = DateEnd.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                ViewBag.CheckBox = checkBoxActual.Value;


                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Пост запрос Search
        /// </summary>
        /// <param name="action"></param>
        /// <param name="FindDoc"></param>
        /// <param name="YearStart"></param>
        /// <param name="YearEnd"></param>
        /// <param name="checkBoxActual"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Search")]
        public ActionResult AllDocuments(string action, string FindDoc, System.DateTime? DateStart, System.DateTime? DateEnd, bool? checkBoxActual)
        {
            try
            {
                if (action == "Search")
                {
                    if (DateEnd != null && DateStart != null && (DateEnd < DateStart || DateStart > DateEnd))
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


        /// <summary>
        /// Метод для перегрузки данных
        /// </summary>
        /// <returns></returns>
        [Route("RefreshDoc")]
        [Authorize]
        public ActionResult RefreshDoc()
        {
            // Процесс обновления данных из БД. Суть проста: Всё чистим и грузим с нуля.
            // Следует подумать над обновлением данных. Например, при лайке или дизлайке, комменте и т.д.
            StartSolr.StartEngine();        // подрубаем Solr
            ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();   // подключение
            solr.Delete(new SolrQuery("*:*"));      // очищаем Solr
            var document = Entity.Documents.SqlQuery("Select * from Documents").ToList();   // Достаем документы из БД
            
            foreach (var item in document)      // достаем каждый документ из бд
            {
                var doc = new Document          // создаем элемент
                {
                    Id = item.Id,
                    Title = item.Title,
                    Texts = item.Texts,
                    Likes = item.Likes,
                    Dislikes = item.Dislikes,
                    DatePublish = item.DatePublish,
                    DateStart = item.DateStart,
                    DateEnd = item.DateEnd,
                    Favorites = item.Favorites
                };

                solr.Add(doc);          // добавляем в солр
            }

            solr.Commit();              // сохраняем solr

            return RedirectToAction("Index", "Home");
        }


        //public void Add()           // Добавление элемента в Solr, тестирование
        //{
        //    var p = new Document
        //    {
        //        Id=14,
        //        Title="testingUp",
        //        Texts="testingTextUp",
        //        Likes=1,
        //        Dislikes=2,
        //        DatePublish=DateTime.Now,
        //        DateStart=DateTime.Now,
        //        DateEnd=null,
        //        Favorites=0
        //    };

        //    var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
        //    solr.Add(p);
        //    solr.Commit();
        //}

        //public void Query()             // тестовый запрос, Solr
        //{
        //    ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
            
        //    //var results = solr.Query(new SolrQuery("*:*")); - Селектим всё!
            
        //    var results = solr.Query(new SolrQuery("*:*"));
        //    Console.WriteLine(results[0].Title) ;
        //}

        //public void Delete()
        //{
        //    ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
        //    //var results = solr.Query(new SolrQuery("Title:testing"));

        //    // Оно работает!!
        //    //solr.Delete(new SolrQuery("IdDoc:14"));

        //    solr.Commit();
        //}


        /// <summary>
        /// Получаем документ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                if(document.Count == 0)
                {
                    return RedirectToAction("Error404", "Error");
                }
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


        /// <summary>
        /// Пост запрос получения документа
        /// </summary>
        /// <param name="action"></param>
        /// <param name="id"></param>
        /// <returns></returns>
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
            return RedirectToAction("GetDocument", "Document", new { id = id });        // переходим к странице
        }
    }
}