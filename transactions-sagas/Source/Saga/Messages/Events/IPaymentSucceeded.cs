using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
    public interface IPaymentSucceeded 
    {
	    Guid BookingId { get; }

	    DateTime Date { get; }
    }
}
