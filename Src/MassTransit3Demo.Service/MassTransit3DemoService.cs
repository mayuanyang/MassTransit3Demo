using System;
using Autofac;
using MassTransit3Demo.Core;

namespace MassTransit3Demo.Service
{
    class MassTransit3DemoService
    {
        public MassTransit3DemoService(Module module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            builder.RegisterModule(new MassTransitModule(true));
            builder.Build();
            
        }

        public void Start()
        {
            Console.WriteLine("Application Started");
        }

        public void Stop() { }
    }
}
