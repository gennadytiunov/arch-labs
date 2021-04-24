using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
    public interface IPaymentInitiated
	{
		Guid BookingId { get; }

		Guid PaymentId { get; }

		DateTime Date { get; }
	}
}
