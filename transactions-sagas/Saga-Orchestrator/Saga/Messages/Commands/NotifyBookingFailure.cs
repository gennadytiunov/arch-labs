using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Commands
{
    public class NotifyBookingFailure
    {
        public Guid BookingId { get; }

        public string Reason { get; }

        public DateTime Date { get; }

        public NotifyBookingFailure(
            Guid bookingId,
            string reason,
            DateTime date)
        {
            BookingId = bookingId;
            Reason = reason;
            Date = date;
        }
    }
}
