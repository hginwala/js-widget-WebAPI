using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SolrNet.Attributes;

namespace Data.SolrReIndexing.Models
{
    [Serializable]
    public class SolrJobDocument : ICloneable
    {
        #region Private Properties
        private List<String> jobTitleID = new List<String>();
        private List<String> jobTitle = new List<String>();
        private List<String> custom1 = new List<String>();
        private List<String> custom2 = new List<String>();
        private List<String> custom3 = new List<String>();
        private List<String> custom4 = new List<String>();
        private List<String> custom5 = new List<String>();
        private List<String> location_geohash = new List<String>();
        private List<String> jobCategoryName = new List<String>();               
        private List<String> jobCategoryID = new List<String>();
        private List<String> countryDesc = new List<String>();
        private List<String> countryDescFull = new List<String>();
        private List<String> dmaID = new List<String>();
        private List<String> dmaName = new List<String>();
        private List<String> locationCityID = new List<String>();
        private List<String> cityName = new List<String>();
        private List<String> stateName = new List<String>();
        private List<String> domainID = new List<String>();
        private List<String> domain = new List<String>();
        private List<String> domainEnvironmentHandle = new List<String>();

        private string jobID = string.Empty;
        private string applyURL = string.Empty;
        private string companyID = string.Empty;
        private string companyName = string.Empty;        
        private string clientReferenceCode = string.Empty;
        private string jobTitle_search = string.Empty;
        private string jobShortDesc = string.Empty;
        private string jobShortDesc_search = string.Empty;
        private string jobFullDesc = string.Empty;
        #endregion

        #region Ctor
        public SolrJobDocument()
        {
        }
        #endregion

        [SolrUniqueKey("JobID")]
        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }
        [SolrField("ApplyURL")]
        public string ApplyURL
        {

            get { return applyURL; }
            set { applyURL = value; }
        }
        [SolrField("CompanyID")]
        public string CompanyID
        {
            get { return companyID; }
            set { companyID = value; }
        }
        [SolrField("CompanyName")]
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }
        
        [SolrField("ClientReferenceCode")]
        public string ClientReferenceCode
        {
            get { return clientReferenceCode; }
            set { clientReferenceCode = value; }
        }
        [SolrField]
        public List<string> JobTitleID
        {
            get { return jobTitleID; }
            set { jobTitleID = value; }
        }
        [SolrField]
        public List<string> JobTitle
        {
            get { return jobTitle; }
            set { jobTitle = value; }
        }
        [SolrField]
        public string JobTitle_search
        {
            get { return jobTitle_search; }
            set { jobTitle_search = value; }
        }
        [SolrField]
        public DateTime JobListingDate { get; set; }
        [SolrField]
        public string JobShortDesc
        {
            get { return jobShortDesc; }
            set { jobShortDesc = value; }
        }
        [SolrField]
        public string JobShortDesc_search
        {
            get { return jobShortDesc_search; }
            set { jobShortDesc_search = value; }
        }
        [SolrField]
        public string JobFullDesc
        {
            get { return jobFullDesc; }
            set { jobFullDesc = value; }
        }
        [SolrField]
        public List<string> Custom1
        {
            get { return custom1; }
            set { custom1 = value; }
        }
        [SolrField]
        public List<string> Custom2
        {
            get { return custom2; }
            set { custom2 = value; }
        }
        [SolrField]
        public List<string> Custom3
        {
            get { return custom3; }
            set { custom3 = value; }
        }
        [SolrField]
        public List<string> Custom4
        {
            get { return custom4; }
            set { custom4 = value; }
        }
        [SolrField]
        public List<string> Custom5
        {
            get { return custom5; }
            set { custom5 = value; }
        }

        [SolrField("Location_geohash")]
        public List<string> Location_geohash
        {
            get { return location_geohash; }
            set { location_geohash = value; }
        }
        [SolrField("JobCategoryName")]
        public List<string> JobCategoryName
        {
            get { return jobCategoryName; }
            set { jobCategoryName = value; }
        }

        [SolrField("JobCategoryID")]
        public List<string> JobCategoryID
        {
            get { return jobCategoryID; }
            set { jobCategoryID = value; }
        }
        [SolrField("CountryDesc")]
        public List<string> CountryDesc
        {
            get { return countryDesc; }
            set { countryDesc = value; }
        }
        [SolrField("CountryDescFull")]
        public List<string> CountryDescFull
        {

            get { return countryDescFull; }
            set { countryDescFull = value; }
        }
        
        [SolrField("DMAID")]
        public List<string> DMAID
        {

            get { return dmaID; }
            set { dmaID = value; }
        }
        [SolrField("DMAName")]
        public List<string> DMAName
        {
            get { return dmaName; }
            set { dmaName = value; }
        }
        
        [SolrField("LocationCityID")]
        public List<string> LocationCityID
        {
            get { return locationCityID; }
            set { locationCityID = value; }
        }
        [SolrField("CityName")]
        public List<string> CityName
        {
            get { return cityName; }
            set { cityName = value; }
        }

        [SolrField("StateName")]
        public List<string> StateName
        {
            get { return stateName; }
            set { stateName = value; }
        }
        
        [SolrField("DomainID")]
        public List<string> DomainID
        {

            get { return domainID; }
            set { domainID = value; }
        }
        [SolrField("Domain")]
        public List<string> Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        [SolrField("DomainEnvironmentHandle")]
        public List<string> DomainEnvironmentHandle
        {
            get { return domainEnvironmentHandle; }
            set { domainEnvironmentHandle = value; }
        }

        [SolrField("NewJob")]
        public bool NewJob{get;set;}
       
                       
        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        public override string ToString()
        {
            int hash = 99;

            hash = (hash * 7) + this.JobID.GetHashCode();
            hash = (hash * 7) + this.ApplyURL.GetHashCode();
            hash = (hash * 7) + this.CompanyID.GetHashCode();
            hash = (hash * 7) + this.CompanyName.GetHashCode();
            hash = (hash * 7) + this.ClientReferenceCode.GetHashCode();
            hash = (hash * 7) + string.Join("|", this.JobTitleID).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.JobTitle).GetHashCode();
            hash = (hash * 7) + this.jobShortDesc.GetHashCode();
            hash = (hash * 7) + this.jobFullDesc.GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Custom1 ).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Custom2).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Custom3).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Custom4).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Custom5).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.JobCategoryID).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.JobCategoryName).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.LocationCityID).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.CityName).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.DMAID).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.DMAName).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.CountryDesc).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.countryDescFull).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.StateName).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Location_geohash).GetHashCode();  
            hash = (hash * 7) + string.Join("|", this.DomainID).GetHashCode();
            hash = (hash * 7) + string.Join("|", this.Domain).GetHashCode();    
            hash = (hash * 7) + string.Join("|", this.DomainEnvironmentHandle).GetHashCode();

            return hash.ToString();
        }

    }
}
