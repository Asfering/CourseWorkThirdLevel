using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class CommentModel
    {
        public int Id { get; set; }     // ID документа
        public int IdComment { get; set; }  // ID комментария
        public string Title { get; set; }   // Заголовок
        public int UsId { get; set; }   // ID пользователя
        public string UsName { get; set; }  // Имя пользователя
        public string Comment { get; set; } // Коммент
        public int? Likes { get; set; } // Лайки для комментов
        public int? Dislikes { get; set; } // Дизлайки для комментов
        public System.DateTime? DatePublish { get; set; } // Дата публикации
    }
}