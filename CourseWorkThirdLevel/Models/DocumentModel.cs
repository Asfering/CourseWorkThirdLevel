using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class DocumentModel
    {
        public int Id { get; set; }     // ID документа
        public int IdComment { get; set; }  // ID комментария
        public string Title { get; set; }   // Заголовок
        public int UsId { get; set; }   // ID пользователя
        public string Comment { get; set; } // Коммент
        public bool? LikeUnlike { get; set; }   // Лайки / Дизлайки для понравившихся
        public int? Likes { get; set; } // Лайки для комментов
        public int? Dislikes { get; set; } // Дизлайки для комментов


    }
}