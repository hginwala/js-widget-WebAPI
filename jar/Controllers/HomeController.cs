using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Data.SolrReIndexing.Models;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Impl;
using System.Web.Mvc;
using System.Text;
using System.Net.Http.Headers;
using JAR.Models;
using System.ComponentModel;

namespace JARi.Controllers
{
    public class HomeController : Controller
    {
        JarContext dbcontext = new JarContext();
        bool newJobsFiler = false;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //
            base.OnActionExecuting(filterContext);
            ViewBag.Title = "Solr Job Alerts Poc";
            newJobsFiler = bool.Parse(ConfigurationManager.AppSettings["NewJobsFilter"]);
        }

        public ActionResult Index()
        {
            var domains = dbcontext.CompabyDomains.Select(d => d).OrderBy(x => x.Domain).ToList();

            var list = domains.Select(d => new SelectListItem { Value = d.SeoSiteId.ToString(), Text = d.Domain }).ToList();
            SearchModel model = new SearchModel() { domains = list, newjobsfilter = newJobsFiler };
            return View(model);
        }


        public ActionResult Search(SearchModel model)
        {
            ViewData["kw"] = model.keyword;
            ViewData["loc"] = model.locationText;
            //ISolrQuery initialQuery = SolrQuery.All;
            List<JobDocument> jobList = new List<JobDocument>();
          //  var domainFilterQuery = new SolrQueryByField("DomainID", string.IsNullOrEmpty(model.DomainId) ? "715" : model.DomainId);
            var newjobFq = new SolrQueryByField("NewJob", "true");

            ISolrQuery locfq = null;

            if (model.locationType == TypeId.City)
            {
                int radius;
                var dist = int.TryParse(model.DistanceSelected, out radius);
                locfq = new SolrQueryByDistance("Location_geohash", new Location(model.Latitude, model.Longitude), dist ? (radius*1.6) : 100, CalculationAccuracy.BoundingBox);

            }
            else if (model.locationType == TypeId.Division1)
            {
                locfq = new SolrMultipleCriteriaQuery(new[] {new SolrQueryByField("StateName", model.locationText),new SolrQueryByField("DMAName",model.locationText)},"OR");
            }
            else if (model.locationType == TypeId.Country && !string.IsNullOrEmpty(model.locationText))
            {
                var ctr = model.locationText.ToLower() == "united states" ? "USA" : model.locationText;
                locfq = new SolrMultipleCriteriaQuery(new[] { new SolrQueryByField("CountryDesc", ctr), new SolrQueryByField("CountryDescFull",ctr) }, "OR"); 
            }

            

           var filterQuery = new ISolrQuery[] {  locfq }.ToList();

           //var filterQuery = new ISolrQuery[] { domainFilterQuery }.ToList();

           if (newJobsFiler)
           {
               filterQuery.Add(newjobFq);
           }



            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrJobDocument>>();
            string term = string.Empty;
            if (!string.IsNullOrEmpty(model.keyword))
            {
                string[] tokens = model.keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                term = getqstring(tokens);
            }




            var res = solr.Query(new SolrQuery(term),
            new QueryOptions
            {
               // FilterQueries = filterQuery,
                Start = 0,
                Rows = 300,
                //OrderBy = new[] { new SortOrder("JobTitle_search", Order.ASC) },
                ExtraParams = new Dictionary<string, string> { { "defType", "dismax" }, { "qf", "JobTitle^5.0 JobFullDesc ClientReferenceCode CityName DMAName StateName CountryDesc JobCategoryName2.5^ Custom1 Custom2 Custom3 Custom4 Custom5 JobID" }, { "mm", "1" }, { "q.alt", "*:*" } }

            });

       
            if (res != null)
            {
                foreach (var job in res)
                {
                    var doc = new JobDocument
                    {
                        CompanyName = job.Domain.First(),
                        JobTitle = job.JobTitle.First(), //.CompanyName_Exact,
                        JobDesc = job.JobFullDesc,
                        Country = job.CountryDesc.First().ToString(),
                        State = job.StateName.First(),//StateName_Exact.First(),
                        City = job.CityName.First(),//CityName_Exact.First(),
                        JobCategory = job.JobCategoryID.First(),//JobCategoryName_Exact.First(),
                        ApplyURL = job.ApplyURL,
                        JobID = job.JobID,
                        JobCategoryID = job.JobCategoryID.First(),
                        JobCategoryName = job.JobCategoryName.First(),//JobCategoryName_Exact.First(),
                        JobListingDate = job.JobListingDate,
                        ClientRefCode = job.ClientReferenceCode,
                        MatchingKW = "na",
                        NewJob = job.NewJob,
                        DMAName = job.DMAName.ToList()
                    };
                    jobList.Add(doc);
                }
            }
            ViewData["count"] = jobList.Any() ? jobList.Count : 0;
            if (jobList.Any())
                model.Jobs = jobList;
            else
                TempData["noresults"] = "no results found";
            var domains = dbcontext.CompabyDomains.Select(d => d).OrderBy(x => x.Domain).ToList();
            var list = domains.Select(d => new SelectListItem { Value = d.SeoSiteId.ToString(), Text = d.Domain }).ToList();
            model.domains = list;
            return View("Index", model);

        }


        private string getqstring(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                sb.Append('"');
                //sb.Append('*');
                sb.Append(tokens[i]);
                //sb.Append('*');
                sb.Append('"');
                if (i < (tokens.Length - 1))
                    sb.Append('+');
            }
            return sb.ToString();
        }

        public JsonResult GetLocationData(string term)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://platform.dev.talentbrew.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var url = string.Format("/api/v1/location?text={0}", term);
                // HTTP GET
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var locations = response.Content.ReadAsAsync<IEnumerable<LocationResponse>>().Result;
                    return new JsonResult { Data = locations, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }

    public enum KeywordSearchType
    {
        AllJobFields = 1,
        JobTitleOnly = 2
    }

    public class JobDocument
    {
        public List<string> DMAName { get; set; }
        public string CompanyName { get; set; }
        //public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public string MatchingKW { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string JobCategory { get; set; }
        public string ApplyURL { get; set; }
        public string JobID { get; set; }
        public string JobCategoryID { get; set; }
        public string JobCategoryName { get; set; }
        public DateTime JobListingDate { get; set; }
        public string ClientRefCode { get; set; }
        public bool NewJob { get; set; }
        public string JobDesc { get; set; }
    }

    public class LocationResponse
    {
        public long Id { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Division1 { get; set; }
        public string Division1Code { get; set; }
        public string City { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public string Type { get; set; }
        public TypeId TypeId { get; set; }
    }

    public enum TypeId
    {
        // Represents a country
        Country = 0,
        // Represents a primary administrative division of a country, such as a state in the United States
        Division1 = 1,
        // Represents a subdivision of a first-order administrative division, such as a county in the United States
        Division2 = 2,
        // Represents a subdivision of a second-order administrative division
        Division3 = 3,
        // Represents a subdivision of a third-order administrative division
        Division4 = 4,
        // Represents a city
        City = 5,
        Unknown = 999
    }

    public class SearchModel
    {
        public bool newjobsfilter { get; set; }
        public string keyword { get; set; }
        public string locationText { get; set; }
        public long locationValue { get; set; }
        public TypeId locationType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string DomainId { get; set; }
        public List<SelectListItem> domains = new List<SelectListItem>();
        public List<JobDocument> Jobs = new List<JobDocument>();
        [DisplayName("Radius")]
        public string DistanceSelected { get; set; }
        public List<SelectListItem> Radius = new List<SelectListItem>(){
            new SelectListItem {Text = "50 miles", Value="50",Selected = true},
            new SelectListItem {Text ="100 miles", Value="100"},
            new SelectListItem {Text ="150 miles", Value ="150"}
        };

    }
}




