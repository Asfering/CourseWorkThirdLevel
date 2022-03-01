using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class AlreadyGetEvalModel
    {
        public int idDoc { get; set; }
        public int idUs { get; set; }
        public bool? LikeUnlike { get; set; }
    }
}