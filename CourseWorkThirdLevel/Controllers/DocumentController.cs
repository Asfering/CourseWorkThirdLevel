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
            ViewBag.TitleDoc = document[0].Title;
            ViewBag.Document = document;
            return View();
        }
    }
}