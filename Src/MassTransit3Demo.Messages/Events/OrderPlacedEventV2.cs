namespace MassTransit3Demo.Messages.Events
{
    public class OrderPlacedEventV2 : IOrderPlacedEvent, IOrderPlacedEventV2
    {
        public decimal OrderAmount { get; set; }
        public decimal Postage { get; set; }
    }
}