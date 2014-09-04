using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace JAR.Models
{
    public class CompanyDomains
    {
        public int SeoSiteId { get; set; }
        public string Domain { get; set; }
        public int CompanyDomainId { get; set; }
    }

    public class CompanyDomainMapping : EntityTypeConfiguration<CompanyDomains>
    {
        public CompanyDomainMapping()
        {
            ToTable("CompanyDomains", "dbo");
            HasKey(t => t.CompanyDomainId);
            Property(t => t.Domain).HasColumnName("Domain");
            Property(t => t.SeoSiteId).HasColumnName("SeoSiteId");
        }
    }
}