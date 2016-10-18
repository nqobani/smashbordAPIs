using Microsoft.Practices.Unity;
using Nest;
using Part1.ApplicationLogic.Interfaces;
using Part1.ApplicationLogic.Services;
using Part1.Data;

namespace Part1.Common.Helpers
{
    public class DiContainerHelper
    {
        public static void RegisterComponents<T>(UnityContainer container) where T : LifetimeManager, new()
        {
            container.RegisterType<IValueService, ValueService>(new T());

            container.RegisterType<IElasticClient, ApplicationElasticClient>(new T());

            container.RegisterType<ICountMessages, MessageCountService>(new T());
        }
    }
}