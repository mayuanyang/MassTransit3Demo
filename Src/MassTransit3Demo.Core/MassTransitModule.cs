using System;
using Autofac;
using Automatonymous;
using MassTransit;
using MassTransit.AutofacIntegration;
using MassTransit.Saga;
using MassTransit3Demo.Core.Consumers;
using MassTransit3Demo.Core.MiddlewareExtensions.ExceptionLogger;
using MassTransit3Demo.Core.MiddlewareExtensions.PerformanceLogger;
using MassTransit3Demo.Core.MiddlewareExtensions.PublishMessageSink;
using MassTransit3Demo.Core.MiddlewareExtensions.SayHello;
using MassTransit3Demo.Core.Sagas.BankoffSaga;
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
                          cfg.UseSayHello();

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

                       
                          //cfg.ConfigurePublish(x =>
                          //{
                          //    x.UseSendExecute(y =>
                          //    {
                          //        Console.WriteLine(y.ConversationId);
                          //    });

                          //    x.UsePublishMessageSink<SendContext>();

                          //});

                          

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

            var reversalSagaStateMachine = new ReversalSagaStateMachine();

            var sagaRepository = new Lazy<ISagaRepository<ReversalSaga>>(() => new InMemorySagaRepository<ReversalSaga>());

            // The general queue
            cfg.ReceiveEndpoint(baseQueueName + "_" + generalQueuePostfix, ep =>
            {
                // // This will defer send/ publish until after the consumer completes successfully in this end point 
                ep.UseInMemoryOutbox();

                ep.Consumer(context.Resolve<IConsumerFactory<PrintToConsoleCommandConsumer>>());
                ep.Consumer(context.Resolve<IConsumerFactory<OrderPlacedEventConsumer>>());
                ep.Consumer(context.Resolve<IConsumerFactory<MessageIsPrintedEventConsumer>>());
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());
                ep.StateMachineSaga(reversalSagaStateMachine, sagaRepository.Value);

                ep.ConfigurePublish(x =>
                {
                   x.UsePublishMessageSink<SendContext>();
                });

            });

            // The conversation queue
            cfg.ReceiveEndpoint(baseQueueName + "_" + requestQueuePostfix, ep =>
            {
                ep.Consumer(context.Resolve<IConsumerFactory<SimpleRequestConsumer>>());
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());
                // This will defer send/ publish until after the consumer completes successfully
                ep.UseInMemoryOutbox();
            });


        }
    }
}
