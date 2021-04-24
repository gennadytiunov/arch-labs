using System;

namespace OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Commands
{
    public class MakePayment
    {
        public Guid BookingId { get; }

        public DateTime Date { get; }

        public MakePayment(
            Guid bookingId,
            DateTime date)
        {
            BookingId = bookingId;
            Date = date;
        }
    }
}
