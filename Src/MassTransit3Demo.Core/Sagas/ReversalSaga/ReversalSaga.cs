using System;
using Automatonymous;

namespace MassTransit3Demo.Core.Sagas.ReversalSaga
{
    /// <summary>
    /// When a transaction is reversed, reversal should not be sent to C4 until it received acknowledgement for the reversed transaction
    /// </summary>
    public class ReversalSaga :
        SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
