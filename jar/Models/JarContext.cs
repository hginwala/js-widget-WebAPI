using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Data.SolrReIndexing.Models;

namespace JAR.Models
{
    public class JarContext : DbContext
    {
        public JarContext()
            : base("seosite")
        {
            Database.SetInitializer<JarContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new CompanyDomainMapping());
            modelBuilder.Configurations.Add(new JobMapping());
            //modelBuilder.Configurations.Add(new WatchCategorysMapping());
            //modelBuilder.Configurations.Add(new WatchSearchsMapping());
            //modelBuilder.Configurations.Add(new lkpJobAlertKeywordSearchTypeMapping());
            //modelBuilder.Configurations.Add(new lkpJobAlertMailTypeMapping());
            //modelBuilder.Configurations.Add(new lkpJobAlertSourceMapping());
            //modelBuilder.Configurations.Add(new SeoSiteMapping());
            //modelBuilder.Configurations.Add(new tblJobAlertSeoSiteUserMapping());
            //modelBuilder.Configurations.Add(new tblJobAlertSeoSiteUserSubscriptionMapping());
            //modelBuilder.Configurations.Add(new FailedJarMigrationLogMapping());
        }


        #region DbSets
        public virtual IDbSet<CompanyDomains> CompabyDomains { get; set; }
        public virtual IDbSet<Job> ActiveJobs { get; set; }

        #endregion




    }


}