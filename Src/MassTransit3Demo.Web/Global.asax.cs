using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using ConfigInjector.Configuration;
using MassTransit3Demo.Core;

namespace MassTransit3Demo.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            ConfigurationConfigurator.RegisterConfigurationSettings()
                .FromAssemblies(CoreAssembly.Assembly)
                .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting).AsSelf().SingleInstance())
                .ExcludeSettingKeys(new[] {
                    "serilog:minimum-level",
                    "webpages:Version",
                    "webpages:Enabled",
                    "ClientValidationEnabled",
                    "UnobtrusiveJavaScriptEnabled",
                "autoFormsAuthentication",
                "enableSimpleMembership"})
                .DoYourThing();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register MVC controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterModule(new MassTransitModule(false));


            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            container.BeginLifetimeScope();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
