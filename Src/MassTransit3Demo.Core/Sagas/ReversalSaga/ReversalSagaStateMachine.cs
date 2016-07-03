using System;
using Automatonymous;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.Sagas.ReversalSaga
{
    public class ReversalSagaStateMachine :
         MassTransitStateMachine<ReversalSaga>
    {
        public ReversalSagaStateMachine()
        {
            // To tell the state machine the field that to hold the state
            InstanceState(x => x.CurrentState);

            // Tell the state machine the event it listens to and the correlation Id being used
            Event(() => TransactionAcknowledged, x => x.CorrelateById(context => context.Message.TransactionId));
            Event(() => TransactionReversed, x => x.CorrelateById(context => context.Message.TransactionId));

            // Define how state get changed base on event and what behavior should be triggered
            Initially(
                When(TransactionReversed)
                    .Then(context =>
                    {
                        context.Instance.Created= DateTime.Now;
                        SaveReversalTransaction(context.Instance.CorrelationId);
                    })
                    .TransitionTo(WaitingForAck)
                );

            During(WaitingForAck,
                When(TransactionAcknowledged)
                    .Then(context => SendReversalToTargetSystem(context.Instance.CorrelationId))
                    .Finalize()
                );

            // Saga will be removed from the repo once it is finalized
            SetCompletedWhenFinalized();
        }

        private void SendReversalToTargetSystem(Guid txId)
        {
            Console.WriteLine($"Reversal ack is received for correlation Id: {txId}, saga can now be completed");
        }



        private void SaveReversalTransaction(Guid txId)
        {
            Console.WriteLine($"Reversal is received for Correlation Id: {txId}, waiting for ack");
        }

        public State ReversalReceived { get; private set; }
        public State WaitingForAck { get; private set; }

        public Event<TransactionReversedEvent> TransactionReversed { get; private set; }

        public Event<TransactionAcknowledgedEvent> TransactionAcknowledged { get; private set; }




       
    }
}
