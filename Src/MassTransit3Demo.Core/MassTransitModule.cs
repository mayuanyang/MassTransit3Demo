﻿using System;
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
using MassTransit3Demo.Core.Sagas.ReversalSaga;
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
            // Register Serilog and get it log to Seq and file
            builder.Register<ILogger>((c, p) => new LoggerConfiguration()
            .ReadFrom.AppSettings()
                .WriteTo.RollingFile(
                    AppDomain.CurrentDomain.GetData("APPBASE").ToString() + "/Logs/Log-{Date}.txt")
                .WriteTo.Seq(c.Resolve<SeqSinkUrlSetting>(), LogEventLevel.Debug)
                .CreateLogger())
                .SingleInstance();

            // Register all the consumers
            builder.RegisterConsumers(typeof(MassTransitModule).Assembly).AsSelf().AsImplementedInterfaces();

            // Register the ConsumerFactory for each consumer, this will ensure each message will have its own nested lifetimescope
            builder.RegisterGeneric(typeof(AutofacConsumerFactory<>)).WithParameter(new NamedParameter("name", "message")).As(typeof(IConsumerFactory<>));

            // Register the bus
            builder.Register(context =>
            {
                var username = context.Resolve<QueueUserNameSetting>();
                var password = context.Resolve<QueuePasswordSetting>();

                var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
                      {
                          // Add middleware to the pipeline between the transport and endpoint
                          cfg.UseSayHello();

                          var uri = context.Resolve<RabbitMqBaseUriSetting>();
                          cfg.Host(new Uri(uri), x =>
                          {
                              x.Username(username);
                              x.Password(password);
                          });

                          // A bus without a receive end point is a publish only bus
                          if (_isReceiveEndPoint)
                          {
                              ConfigureEndPoints(cfg, context);
                          }
                          
                      });
                
                
                busControl.Start();
                return busControl;
            })
                .SingleInstance()
                .As<IBusControl>() // IBusControl let you start and stop the bus
                .As<IBus>()
                .AutoActivate();

            RegisterMessageRequestClient(builder);
        }
        private void RegisterMessageRequestClient(ContainerBuilder builder)
        {
            // Request/Response
            builder.RegisterGeneric(typeof(RequestClient<,>)).AsSelf().AsImplementedInterfaces();

        }
        private void ConfigureEndPoints(IBusFactoryConfigurator cfg, IComponentContext context)
        {
            var baseQueueName = context.Resolve<BaseQueueNameSetting>();
            var requestQueuePostfix = context.Resolve<RequestResponseQueueNamePostfixSetting>();
            var generalQueuePostfix = context.Resolve<GeneralQueueNamePostfixSetting>();

            var reversalSagaStateMachine = new ReversalSagaStateMachine();

            var sagaRepository = new Lazy<ISagaRepository<ReversalSaga>>(() => new InMemorySagaRepository<ReversalSaga>());

            // The general queue 1
            cfg.ReceiveEndpoint($"{baseQueueName}_{generalQueuePostfix}_1", ep =>
            {
                // // This will defer send/ publish until after the consumer completes successfully in this end point 
                ep.UseInMemoryOutbox();
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());

                ep.StateMachineSaga(reversalSagaStateMachine, sagaRepository.Value);

                ep.Consumer(context.Resolve<IConsumerFactory<PrintToConsoleCommandConsumer>>());
                ep.Consumer(context.Resolve<IConsumerFactory<MessageIsPrintedEventConsumer>>());
                
                // The publish pipeline
                ep.ConfigurePublish(x =>
                {
                   x.UseEventStore<SendContext>();
                });

            });

            // The general queue 2
            cfg.ReceiveEndpoint($"{baseQueueName}_{generalQueuePostfix}_2", ep =>
            {
                // // This will defer send/ publish until after the consumer completes successfully in this end point 
                ep.UseInMemoryOutbox();

                ep.Consumer(context.Resolve<IConsumerFactory<OrderPlacedEventConsumer>>());
               
                ep.ConfigurePublish(x =>
                {
                    x.UseEventStore<SendContext>();
                });

            });

            // The conversation queue, request/response
            cfg.ReceiveEndpoint($"{baseQueueName}_{requestQueuePostfix}", ep =>
            {
                // This will defer send/ publish until after the consumer completes successfully
                ep.UseInMemoryOutbox();
                ep.Consumer(context.Resolve<IConsumerFactory<SimpleRequestConsumer>>());
                ep.UsePerformanceLogger(context.Resolve<ILogger>());
                ep.UseExceptionLogger(context.Resolve<ILogger>());
                
            });


        }
    }
}
