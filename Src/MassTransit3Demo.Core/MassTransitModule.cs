using System;
using Autofac;
using MassTransit;
using MassTransit.AutofacIntegration;
using MassTransit3Demo.Core.Consumers;
using MassTransit3Demo.Core.MiddlewareExtensions.ExceptionLogger;
using MassTransit3Demo.Core.MiddlewareExtensions.PerformanceLogger;
using MassTransit3Demo.Core.Settings;
using Serilog;
using Serilog.Events;

namespace MassTransit3Demo.Core
{
    public class MassTransitModule : Module
    {
        private readonly bool _isReceiveEndPoint;

        public MassTransitModule(bool isReceiveEndPoint)
        {
            _isReceiveEndPoint = isReceiveEndPoint;
        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.Register<ILogger>((c, p) => new LoggerConfiguration()
            .ReadFrom.AppSettings()
                .WriteTo.RollingFile(
                    AppDomain.CurrentDomain.GetData("APPBASE").ToString() + "/Logs/Log-{Date}.txt")
                .WriteTo.Seq(c.Resolve<SeqSinkUrlSetting>(), LogEventLevel.Debug)
                .CreateLogger())
                .SingleInstance();


            builder.RegisterConsumers(typeof(MassTransitModule).Assembly).AsSelf().AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(AutofacConsumerFactory<>)).WithParameter(new NamedParameter("name", "message")).As(typeof(IConsumerFactory<>));
            
            builder.Register<IBusControl>(context =>
            {
                var username = context.Resolve<QueueUserNameSetting>();
                var password = context.Resolve<QueuePasswordSetting>();
              var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var uri = context.Resolve<RabbitMqBaseUriSetting>();
                        cfg.Host(new Uri(uri), x =>
                        {
                            x.Username(username);
                            x.Password(password);
                        });
                        if (_isReceiveEndPoint)
                        {
                            ConfigureEndPoints(cfg, context);
                        }
                        else
                        {
                            cfg.UseExceptionLogger(context.Resolve<ILogger>());
                        }


                    });

                    busControl.Start();
                    return busControl;
            })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>()
                .AutoActivate();

            RegisterMessageRequestClient(builder);
        }
        private void RegisterMessageRequestClient(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RequestClient<,>)).AsSelf().AsImplementedInterfaces();

        }
        private void ConfigureEndPoints(IBusFactoryConfigurator cfg, IComponentContext context)
        {
            var baseQueueName = context.Resolve<BaseQueueNameSetting>();
            var requestQueuePostfix = context.Resolve<RequestResponseQueueNamePostfixSetting>();
            var generalQueuePostfix = context.Resolve<GeneralQueueNamePostfixSetting>();

            // The general queue
            cfg.ReceiveEndpoint(baseQueueName + "_" + generalQueuePostfix, ep =>
            {
                ep.Consumer(context.Resolve<IConsumerFactory<PrintToConsoleCommandConsumer>>());
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());
                

            });

            // The conversation queue
            cfg.ReceiveEndpoint(baseQueueName + "_" + requestQueuePostfix, ep =>
            {
                ep.Consumer(context.Resolve<IConsumerFactory<SimpleRequestConsumer>>());
                
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());
                
            });

            
        }
    }
}
