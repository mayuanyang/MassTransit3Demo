namespace MassTransit3Demo.Messages.Events
{
    public interface IOrderPlacedEvent
    {
        decimal OrderAmount { get; set; }
    }
}
