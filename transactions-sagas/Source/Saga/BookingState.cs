using System;
using Automatonymous;

namespace OtusApp.Circus.Booking.Orchestrator.Saga
{
    public class BookingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }
    }
}
