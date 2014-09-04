using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace searchwidgetservice.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CORSAuthorize : Attribute, ICorsPolicyProvider
    {
        public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corsRequestContext = request.GetCorsRequestContext();
            var originRequested = corsRequestContext.Origin;
            if (await IsOriginFromAPaidCustomer(originRequested))
            {
                // Grant CORS request
                var policy = new CorsPolicy
                {
                    AllowAnyHeader = true,
                    AllowAnyMethod = true,
                };
                policy.Origins.Add(originRequested);
                return policy;
            }
            else
            {
                // Reject CORS request
                return null;
            }
        }
        private async Task<bool> IsOriginFromAPaidCustomer(string originRequested)
        {
            List<string> verifiedDomains =  getdomains(); // this where you get list of verified domains from db or someother source
            
            return  verifiedDomains.Contains(originRequested);           
        }

        private List<string> getdomains()
        {
            return new List<string>() { "http://localhost:61669", "jobsattmp.com" };
        }
    }

    
}