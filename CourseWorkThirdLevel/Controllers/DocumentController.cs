using CourseWorkThirdLevel.Models;
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
        [Route("Document/AllDocuments")]
        public ActionResult AllDocuments()
        {
            // заглушка, пока выводятся все документы
            var document = Entity.Documents.SqlQuery("Select * from Documents").ToList();
            ViewBag.Document = document;
            return View();
        }

        [Route("Document/{id}")]
        public ActionResult GetDocument(int? id)
        {
            var document = Entity.Documents.SqlQuery($"Select * from Documents Where Id = {id}").ToList();
            var docComments = from doc in Entity.Documents
                              join comments in Entity.Comments on doc.Id equals comments.IdDocument
                              join users in Entity.Users on comments.IdUser equals users.Id
                              where doc.Id == id
                              select new CommentModel
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
            ViewBag.Comments = docComments;
            ViewBag.Document = document;
            return View();
        }
    }
}