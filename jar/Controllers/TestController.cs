using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Data.SolrReIndexing.Models;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace jar.Controllers
{
    public class TestController : Controller
    {
        public ViewResult Index()
        {
            //ISolrQuery newJobQuery = new SolrQueryByField("NewJob", "true");
            //Dictionary<string, int> NewCompFacets = new Dictionary<string, int>();
            //var solr = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISolrOperations<SolrJobDocument>>();

            //var results = solr.Query("NewJob:true", new QueryOptions
            //{
            //    Rows = 0,
            //    Facet = new FacetParameters
            //    {
            //        Queries = new[] { new SolrFacetFieldQuery("CompanyID")},
            //        MinCount = 1
            //    }
            //});

            //foreach (var i in results.FacetFields["CompanyID"])
            //{
            //    NewCompFacets.Add(i.Key, i.Value);
            //}
            
            return View();
        }

        public ViewResult BasicSearch()
        {
            return View();
        }

    }
}
