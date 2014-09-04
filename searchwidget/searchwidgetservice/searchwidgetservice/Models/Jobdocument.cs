using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace searchwidgetservice.Models
{
    public class JobDocument
    {
        
        public string JobTitle { get; set; }        
        public string ApplyURL { get; set; }
        public string JobID { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
      
    }
}