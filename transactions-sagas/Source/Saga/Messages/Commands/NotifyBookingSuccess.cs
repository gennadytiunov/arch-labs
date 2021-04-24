using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Commands
{
    public class NotifyBookingSuccess
    {
        public Guid BookingId { get; }

        public DateTime Date { get; }

        public NotifyBookingSuccess(
            Guid bookingId,
            DateTime date)
        {
            BookingId = bookingId;
            Date = date;
        }
    }
}
