using System;

namespace MassTransit3Demo.Messages.Events
{
    public class TransactionAcknowledgedEvent
    {
        public Guid TransactionId { get; set; }

    }
}