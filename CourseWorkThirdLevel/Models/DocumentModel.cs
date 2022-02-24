using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class DocumentModel
    {
        public int Id { get; set; }     // ID документа
        public string Title { get; set; }   // Заголовок
        public int UsId { get; set; }   // ID пользователя
        public bool? LikeUnlike { get; set; }   // Лайки / Дизлайки для понравившихся


    }
}