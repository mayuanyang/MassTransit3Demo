using MassTransit3Demo.Messages.MessageContracts;

namespace MassTransit3Demo.Messages
{
    public class SimpleRequest : IRequestItem<SimpleResonse>
    {
        public string Name { get; }

        public SimpleRequest(string name)
        {
            Name = name;
        }
    }
}