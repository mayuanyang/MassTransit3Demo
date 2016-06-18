using System;
using MassTransit;
using MassTransit3Demo.Core.Settings;

namespace MassTransit3Demo.Core
{
    public class RequestClient<TRequest, TResponse> : MessageRequestClient<TRequest, TResponse> where TRequest : class where TResponse : class
    {
        public RequestClient(IBus bus, BaseQueueNameSetting baseQueueName, RabbitMqBaseUriSetting uriSetting, RequestResponseQueueNamePostfixSetting postfixSetting) : 
            base(bus, new Uri(uriSetting + "/" + baseQueueName + "_" + postfixSetting), TimeSpan.FromSeconds(30), null, null)
        {
        }
    }

}
