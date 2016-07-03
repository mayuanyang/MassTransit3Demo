using System;
using Automatonymous;

namespace MassTransit3Demo.Core.Sagas.ReversalSaga
{
    public class ReversalSaga :
        SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
