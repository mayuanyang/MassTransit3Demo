using System;

namespace MassTransit3Demo.Messages.Events
{
    public class TransactionReversedEvent
    {
        public Guid TransactionId { get; set; }
    }
}