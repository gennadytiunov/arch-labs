using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
    public interface IPaymentFailed
    {
	    Guid BookingId { get; }

        string Reason { get; }

        DateTime Date { get; }
    }
}
