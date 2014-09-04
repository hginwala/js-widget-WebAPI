using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SolrReIndexing.Models
{
    
     public class Category
    {
        public Category()
        {

        }
        public int? JobCategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? JobID { get; set; }
    }

     public class Domain
     {
         public int DomainID { get; set; }
         public int SeoSiteID { get; set; }
         public int? CompanyID { get; set; }
         public string DomainName { get; set; }
         public string EnvironmentType { get; set; }
     }


    public class Job
    {
        private List<Category> _jobCategories;
        public int JobID { get; set; }
        public string ApplyURL { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string ClientReferenceCode { get; set; }
        public int JobTitleID { get; set; }
        public string JobTitle { get; set; }
        public string JobDescriptionShort { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string str_Custom1 { get; set; }
        public string str_Custom2 { get; set; }
        public string str_Custom3 { get; set; }
        public string str_Custom4 { get; set; }
        public string str_Custom5 { get; set; }
        public string JobDescriptionLong { get; set; }
        public bool NewJob { get; set; }
       
    }

    public class JobMapping : EntityTypeConfiguration<Job>
    {
        public JobMapping()
        {
            HasKey(t => t.JobID);
        }
    }
}
