using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Practices.Unity;
using Nest;
using Owin;
using Part1.Api.App_Start;
using Part1.Data.EsModels;

[assembly: OwinStartup(typeof(Part1.Api.Startup))]

namespace Part1.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = UnityConfig.RegisterComponents(app.GetDataProtectionProvider());
            ConfigureAuth(app);

            var client = container.Resolve<IElasticClient>();
            EnsureIndices(client, "smashboard-messages");
        }

        private static void EnsureIndices(IElasticClient client, string indexName)
        {
            var index = client.GetIndex(i => i.Index(indexName));
            //if (index.Indices.Count == 0)
            //{
            //    var res = client.CreateIndex(ci => ci
            //     .Index(indexName)
            //     .AddMapping<EsValue>(m => m.MapFromAttributes()));
            //    Debug.WriteLine(res.RequestInformation.Success);

            //Test
            //var firstDoc = new EsValue
            //{
            //    Id = Guid.NewGuid(),
            //    Value = "value0"
            //};

            //var r = client.Index(firstDoc, v => v
            //        .Index(indexName)
            //        .Id(firstDoc.Id.ToString())
            //        .Refresh());

            //Debug.WriteLine(r.RequestInformation.Success);
        }
    }
}

