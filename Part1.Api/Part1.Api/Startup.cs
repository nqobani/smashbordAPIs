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
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Part1.Api.Providers;

[assembly: OwinStartup(typeof(Part1.Api.Startup))]
namespace Part1.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var authIssuer = "http://alchemy-auth-dev.azurewebsites.net";
            var authAudienceId = "256d2a8b-74e8-4b8d-a49c-d99f79a129b2";
            var authAudienceSecret = TextEncodings.Base64Url.Decode("1hG3cPkgw8us1VpHxuWQdhDm2JiHQRVGSbicOCts_7y9WD1p6dQu3BKZ69-0BCw_rXU4G06M9wHtkF1dl3VCbA");

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    AllowedAudiences = new[] { authAudienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(authIssuer, authAudienceSecret)
                    }
                });
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

