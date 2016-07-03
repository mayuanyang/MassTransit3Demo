using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.Configurators;
using MassTransit.PipeBuilders;
using MassTransit.PipeConfigurators;


namespace MassTransit3Demo.Core.MiddlewareExtensions.SayHello
{
    public class SayHelloPipeSpecification<T> :
        IPipeSpecification<T>
        where T : class, PipeContext
    {
        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new SayHelloFilter<T>());
        }
    }
    
}
