using MassTransit3Demo.Messages.MessageContracts;

namespace MassTransit3Demo.Messages
{
    public class SimpleResonse : IResponseItem
    {
        public string Result { get; set; }

        public SimpleResonse(string result)
        {
            Result = result;
        }
    }
}
