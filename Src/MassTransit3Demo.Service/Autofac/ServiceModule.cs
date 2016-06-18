using Autofac;
using ConfigInjector.Configuration;
using MassTransit3Demo.Core;

namespace MassTransit3Demo.Service.Autofac
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterSettings(builder);
        }

        private void RegisterSettings(ContainerBuilder builder)
        {
            ConfigurationConfigurator.RegisterConfigurationSettings()
                   .FromAssemblies(CoreAssembly.Assembly)
                   .RegisterWithContainer(configSetting => builder.RegisterInstance(configSetting).AsSelf().SingleInstance())
                   .ExcludeSettingKeys(new[] { "serilog:minimum-level" })
                   .DoYourThing();
        }

    }
}
