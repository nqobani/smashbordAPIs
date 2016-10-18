using System;
using System.Diagnostics;
using System.Text;
using System.Web.Configuration;
using Nest;

namespace Part1.Data
{
    public class ApplicationElasticClient : ElasticClient
    {
        public ApplicationElasticClient()
#if DEBUG
            : base(
        new ConnectionSettings(new Uri(WebConfigurationManager.AppSettings["ElasticSearch.Host"]), WebConfigurationManager.AppSettings["ElasticSearch.Index.Name"]).SetConnectionStatusHandler(
            h =>
            {
                var req = h.Request == null ? "" : Encoding.UTF8.GetString(h.Request);
                Trace.WriteLine(
                    $"Url: {h.RequestUrl} | Method: {h.RequestMethod} | Request: \n{req}");
            }).SetBasicAuthentication(WebConfigurationManager.AppSettings["ElasticSearch.Username"],
                                           WebConfigurationManager.AppSettings["ElasticSearch.Password"])
          )
#else
             : base(new ConnectionSettings(new Uri(WebConfigurationManager.AppSettings["ElasticSearch.Host"]), WebConfigurationManager.AppSettings["ElasticSearch.Index.Alias.Messages"])
                    .SetBasicAuthentication(WebConfigurationManager.AppSettings["ElasticSearch.Username"],
                                            WebConfigurationManager.AppSettings["ElasticSearch.Password"]))
#endif
        {
        }
    }
}