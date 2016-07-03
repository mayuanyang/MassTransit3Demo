namespace MassTransit3Demo.Messages.Events
{
    public interface IOrderPlacedEventV2 : IEvent
    {
        decimal OrderAmount { get; set; }
        decimal Postage { get; set; }
    }
}
