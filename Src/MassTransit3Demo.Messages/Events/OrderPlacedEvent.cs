namespace MassTransit3Demo.Messages.Events
{
    public class OrderPlacedEvent : IOrderPlacedEvent
    {
        public decimal OrderAmount { get; set; }
        
    }
}