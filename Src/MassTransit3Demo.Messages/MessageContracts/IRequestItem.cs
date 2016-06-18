namespace MassTransit3Demo.Messages.MessageContracts
{
    public interface IRequestItem<TResponse> where TResponse : IResponseItem
    {
    }
}
