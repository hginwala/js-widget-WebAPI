using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml.Serialization;
using Data.SolrReIndexing.Models;
using JAR.Models;
using JARi.Controllers;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace jar.Controllers
{
    public class PZDataImportController : Controller
    {
        public const string PlatFormAPIBaseUri = "http://platform.dev.talentbrew.com";

        public ActionResult Index(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"] != null && Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {

                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }

                using (SqlCeConnection conn = new SqlCeConnection(@"Data Source=|DataDirectory|\PZSampleData.sdf"))
                {
                    using (SqlCeCommand cmdInsert = conn.CreateCommand())
                    {
                        conn.Open();
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            string query = "Insert into JobAlerts(IPCountry,IPRegion,IPCity) Values('" +
                            ds.Tables[0].Rows[i]["IPCountry"].ToString() + "','" + ds.Tables[0].Rows[i]["IPRegion"].ToString() +
                            "','" + ds.Tables[0].Rows[i]["IPCity"].ToString() + "')";                            
                                 
                            cmdInsert.CommandText = query;
                            cmdInsert.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
            return View();
        }

        public ActionResult ValidateLocations(int size = 100)
        {
            DataTable dat = new DataTable();
            LocationModel model = new LocationModel();
            using (SqlCeConnection conn = new SqlCeConnection(@"Data Source=|DataDirectory|\PZSampleData.sdf"))
            {
                using (SqlCeDataAdapter adap = new SqlCeDataAdapter("SELECT IPCountry,IPRegion,IPCity FROM JobAlerts", conn))
                {
                    adap.Fill(dat);
                }
            }
            for (int i = 0; i < size; i++)
            {
                var crRow = dat.Rows[i];
                string country = crRow["IPCountry"].ToString();
                string region = crRow["IPRegion"].ToString();
                string city = crRow["IPCity"].ToString();
                if (string.IsNullOrEmpty(country) && string.IsNullOrEmpty(region) && string.IsNullOrEmpty(city))
                    continue;
                var matchloc = LocationFinder(country,region,city);
                if (matchloc.Exact == null && matchloc.Suggestions == null)
                {
                    var loctext = "<p class=\"text-danger\">NO MATCH FOUND</p>";
                    model.LocationModelList.Add(new LocationModel { IPCountry = country, ExactMatch = false, IPRegion = region, IPCity = city, MatchedLocation = loctext });

                }
                else if (matchloc.Exact != null)
                {
                    var loctext = matchloc.Exact.City + ", " + matchloc.Exact.Division1Code + ", " + matchloc.Exact.CountryCode;
                    model.LocationModelList.Add(new LocationModel { IPCountry = country, ExactMatch = true, IPRegion = region, IPCity = city, MatchedLocation = loctext });
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var a in matchloc.Suggestions)
                    {
                        sb.AppendFormat("{0},{1},{2} </br>", a.City, a.Division1Code, a.CountryCode);
                    }
                    var loctext = sb.ToString();
                    model.LocationModelList.Add(new LocationModel { IPCountry = country, ExactMatch = false, IPRegion = region, IPCity = city, MatchedLocation = loctext });
                }
            }
            return View(model);

        }

        public LocationFinder LocationFinder(string country, string region, string city)
        {
            LocationFinder finderobj = null;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(PlatFormAPIBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var url = string.Format("api/v1/locationfinder?country={0}&region={1}&city={2}", country, region, city);

                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    finderobj = response.Content.ReadAsAsync<LocationFinder>().Result;

                }
            }
            return finderobj;
        }




    }


    public class LocationModel
    {
        public List<LocationModel> LocationModelList = new List<LocationModel>();
        public string IPCountry { get; set; }
        public string IPRegion { get; set; }
        public string IPCity { get; set; }
        public string MatchedLocation { get; set; }
        public bool ExactMatch { get; set; }
    }

    public class LocationFinder
    {
        public LocationResponse Exact { get; set; }
        public IEnumerable<LocationResponse> Suggestions { get; set; }
    }



}
