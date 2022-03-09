using System;
using SolrNet;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonServiceLocator;

namespace CourseWorkThirdLevel.Models
{
    public class StartSolr
    {
        public static void StartEngine()
        {
            Console.WriteLine(Environment.NewLine + "- Starting Solr");
            Startup.Init<Document>("http://localhost:8983/solr/Course3");
            ISolrOperations<Document> solr = ServiceLocator.Current.GetInstance<ISolrOperations<Document>>();
        }
    }
}