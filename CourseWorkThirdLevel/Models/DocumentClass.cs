using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourseWorkThirdLevel.Models
{
    public class DocumentClass : IEnumerable<Document>
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }

        public IEnumerator<Document> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}