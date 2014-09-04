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
            return View();
        }

        public ViewResult BasicSearch()
        {
            return View();
        }

    }
}
