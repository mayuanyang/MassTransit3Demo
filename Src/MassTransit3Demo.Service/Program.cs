using MassTransit3Demo.Service.Autofac;
using Topshelf;

namespace MassTransit3Demo.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                 
            {
                x.Service<MassTransit3DemoService>(s =>                        
                {
                    s.ConstructUsing(name => new MassTransit3DemoService(new ServiceModule()));   
                    s.WhenStarted(tc => tc.Start());             
                    s.WhenStopped(tc => tc.Stop());              
                });
                x.RunAsLocalSystem();                            

                x.SetDescription("MassTransit3DemoService");        
                x.SetDisplayName("MassTransit3DemoService");                       
                x.SetServiceName("MassTransit3DemoService");                          
            });
            
        }
    }
}
