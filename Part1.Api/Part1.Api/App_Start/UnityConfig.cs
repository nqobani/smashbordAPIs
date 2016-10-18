using Microsoft.Owin.Security.DataProtection;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Part1.Common.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using MvcUnityDependencyResolver = Unity.Mvc5.UnityDependencyResolver;
using WebApiUnityDependencyResolver = Unity.WebApi.UnityDependencyResolver;

namespace Part1.Api.App_Start
{
    public class UnityConfig
    {
        public static UnityContainer RegisterComponents(IDataProtectionProvider dataProtectionProvider)
        {
            var container = new UnityContainer();

            DiContainerHelper.RegisterComponents<HierarchicalLifetimeManager>(container);

            var settings = new JsonSerializerSettings();
#if DEBUG
            settings.Formatting = Formatting.Indented;
#else
            settings.Formatting = Formatting.None;
#endif
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var jsonSerializer = JsonSerializer.Create(settings);
            container.RegisterInstance(jsonSerializer);

            DependencyResolver.SetResolver(new MvcUnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new WebApiUnityDependencyResolver(container);

            return container;
        }
    }
}