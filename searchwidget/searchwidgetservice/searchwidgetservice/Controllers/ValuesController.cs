using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Impl;
using System.Web.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.ComponentModel;
using searchwidgetservice.Models;
using Data.SolrReIndexing.Models;
using System.Web.Http.Cors;
using System.Security.Cryptography;
using searchwidgetservice.CustomAttributes;



namespace searchwidgetservice.Controllers
{
    
    public class ValuesController : ApiController
    {
        

        public Dictionary<string, Domain> domainkeys = new Dictionary<string, Domain>();       

        public ValuesController()
        {
            domainkeys.Add(GetMd5Hash("726jobs.dell.com"), new Domain { domainID = 726, DomainName = "http://localhost:61669" });
            domainkeys.Add(GetMd5Hash("5jobsattmp.com"), new Domain { domainID = 5, DomainName = "jobsattmp.com" });
        }       

        [EnableCors(origins: "*", headers: "*", methods: "get,post")]  
        [CORSAuthorize]
        public HttpResponseMessage Search(string key, string keyword,int maxJobs = 10)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Authorization failed");
            }

            ISolrQuery initialQuery = SolrQuery.All;
            List<JobDocument> jobList = new List<JobDocument>();

            var domain = ParseAuthKey(key);

            var domainFilterQuery = new SolrQueryByField("DomainID", domain.domainID.ToString());           

            var filterQuery = new ISolrQuery[] { domainFilterQuery }.ToList();

            #region SolrCall
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrJobDocument>>();
            string term = string.Empty;

            if (!string.IsNullOrEmpty(keyword))
            {
                string[] tokens = keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                term = getqstring(tokens);
            }

            var res = solr.Query(new SolrQuery(term),
            new QueryOptions
            {
                FilterQueries = filterQuery,
                Start = 0,
                Rows = maxJobs,
                ExtraParams = new Dictionary<string, string> { { "defType", "dismax" }, { "qf", "JobTitle^5.0 JobFullDesc ClientReferenceCode CityName DMAName StateName CountryDesc JobCategoryName Custom1 Custom2 Custom3 Custom4 Custom5" }, { "mm", "1" }, { "q.alt", "*:*" } }

            });


            if (res != null)
            {
                foreach (var job in res)
                {
                    var doc = new JobDocument
                    {
                        JobTitle = job.JobTitle.First(),
                        ApplyURL = job.ApplyURL,
                        JobID = job.JobID,
                        Country = job.CountryDesc.First().ToString(),
                        State = job.StateName.First(),//StateName_Exact.First(),
                        City = job.CityName.First(),//CityName_Exact.First(),
                        //ClientRefCode = job.ClientReferenceCode,
                    };
                    jobList.Add(doc);
                }
            }
            #endregion

            if (jobList.Any())
            {

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, jobList);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "No matching jobs found");
            }

        }

        [CORSAuthorize]
        [EnableCors(origins: "*", headers: "*", methods: "get,post")]       
        public HttpResponseMessage RecentJobs(string key, int maxJobs = 3)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Authorization failed");
            }

            ISolrQuery initialQuery = SolrQuery.All;
            List<JobDocument> jobList = new List<JobDocument>();

            var domain = ParseAuthKey(key);

            var domainFilterQuery = new SolrQueryByField("DomainID", domain.domainID.ToString());            

            var filterQuery = new ISolrQuery[] { domainFilterQuery }.ToList();
            if (maxJobs > 10)
                maxJobs = 5;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<SolrJobDocument>>();

            var res = solr.Query(SolrQuery.All,
            new QueryOptions
            {
                FilterQueries = filterQuery,
                Start = 0,
                Rows = maxJobs,
                OrderBy = new[] { new SortOrder("JobListingDate", Order.DESC) }
            });


            if (res != null)
            {
                foreach (var job in res)
                {
                    var doc = new JobDocument
                    {
                        JobTitle = job.JobTitle.First(),
                        ApplyURL = job.ApplyURL,
                        JobID = job.JobID,
                        Country = job.CountryDesc.First().ToString(),
                        State = job.StateName.First(),//StateName_Exact.First(),
                        City = job.CityName.First(),//CityName_Exact.First(),
                        //ClientRefCode = job.ClientReferenceCode,
                    };
                    jobList.Add(doc);
                }
            }

            if (jobList.Any())
            {

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, jobList);
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No matching jobs found");
            }
        }

        #region private methods
        private string getqstring(string[] tokens)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                sb.Append('"');
                sb.Append(tokens[i]);
                sb.Append('"');
                if (i < (tokens.Length - 1))
                    sb.Append('+');
            }
            return sb.ToString();
        }

        public Domain ParseAuthKey(string key)
        {
            if (domainkeys.ContainsKey(key))
            {
                return domainkeys[key];
            }
            else
                return null;
        }


        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion
    }

    public class Domain
    {
        public int domainID { get; set; }
        public string DomainName { get; set; }
    }

} 