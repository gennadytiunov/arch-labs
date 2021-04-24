using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
	public interface IBookingFailureNotified
	{
		Guid BookingId { get; }

		DateTime Date { get; }
	}
}
