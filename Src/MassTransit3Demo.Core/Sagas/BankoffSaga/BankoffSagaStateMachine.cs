using System;
using Automatonymous;
using MassTransit3Demo.Messages.Events;

namespace MassTransit3Demo.Core.Sagas.BankoffSaga
{
    public class ReversalSagaStateMachine :
         MassTransitStateMachine<ReversalSaga>
    {
        public ReversalSagaStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => TransactionAcknowledged, x => x.CorrelateById(context => context.Message.TransactionId));
            Event(() => TransactionReversed, x => x.CorrelateById(context => context.Message.TransactionId));


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
