using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
	public interface IBookingSuccessNotified
	{
		Guid BookingId { get; }

		DateTime Date { get; }
	}
}
