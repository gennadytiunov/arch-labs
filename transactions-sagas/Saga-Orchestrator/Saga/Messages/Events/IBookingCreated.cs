using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events
{
	public interface IBookingCreated
	{
		Guid BookingId { get; }

		DateTime Date { get; }
	}
}
