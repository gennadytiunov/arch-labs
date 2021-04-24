using System;
using System.Text.Json;
using Automatonymous;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Commands;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events;
using Serilog;

namespace OtusApp.Circus.Booking.Orchestrator.Saga
{
    public sealed class BookingStateMachine : MassTransitStateMachine<BookingState>
    {
        private readonly ILogger _logger = Log.ForContext<BookingStateMachine>();
        
        public BookingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            SubState(() => SuccessNotified, Succeeded);
            SubState(() => FailureNotified, Failed);

            SetCompletedWhenFinalized();

            Initially(
                When(BookingCreatedEvent)
                    .Then(context =>
                    {
                        _logger.Information($"{GetType().Name}: {BookingCreatedEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
                    })
                    .Send(context =>
                    {
	                    var command = new MakePayment(context.Data.BookingId, DateTime.UtcNow);
	                    _logger.Information($"{GetType().Name}: {BookingCreatedEvent.Name}: Sending {command.GetType().Name}: {JsonSerializer.Serialize(command)}");
	                    return command;
                    })
                    .TransitionTo(Created));

            During(Created,
	            When(PaymentInitiatedEvent)
		            .Then(context =>
		            {
			            _logger.Information($"{GetType().Name}: {PaymentInitiatedEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
                    })
		            .TransitionTo(PaymentInitiated));

            During(PaymentInitiated,
                When(PaymentSucceededEvent)
                    .Then(context =>
                    {
                        _logger.Information($"{GetType().Name}: {PaymentSucceededEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
                    })
                    .Send(context =>
                    {
	                    var command = new NotifyBookingSuccess(context.Data.BookingId, DateTime.UtcNow);
	                    _logger.Information($"{GetType().Name}: {PaymentSucceededEvent.Name}: Sending {command.GetType().Name}: {JsonSerializer.Serialize(command)}");
	                    return command;
                    })
                    .TransitionTo(Succeeded));

            During(Succeeded,
	            When(BookingSuccessNotifiedEvent)
		            .Then(context =>
		            {
			            _logger.Information($"{GetType().Name}: {BookingSuccessNotifiedEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
		            })
		            .TransitionTo(SuccessNotified)
		            .Finalize());

            During(PaymentInitiated,
	            When(PaymentFailedEvent)
		            .Then(context =>
		            {
			            _logger.Information($"{GetType().Name}: {PaymentFailedEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
		            })
		            .Send(context =>
		            {
			            var command = new NotifyBookingFailure(context.Data.BookingId, context.Data.Reason, DateTime.UtcNow);
			            _logger.Information($"{GetType().Name}: {PaymentFailedEvent.Name}: Sending {command.GetType().Name}: {JsonSerializer.Serialize(command)}");
			            return command;
		            })
		            .TransitionTo(Failed));

            During(Failed,
	            When(BookingFailureNotifiedEvent)
		            .Then(context =>
		            {
			            _logger.Information($"{GetType().Name}: {BookingFailureNotifiedEvent.Name} received: {JsonSerializer.Serialize(context.Data)}");
		            })
		            .TransitionTo(FailureNotified)
		            .Finalize());

            Event(() => BookingCreatedEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));

			Event(() => PaymentInitiatedEvent,
				x => x.CorrelateById(context => context.Message.BookingId));

            Event(() => PaymentSucceededEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));

            Event(() => BookingSuccessNotifiedEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));

            Event(() => PaymentFailedEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));

            Event(() => BookingFailureNotifiedEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));

            Event(() => BookingCancelledEvent,
	            x => x.CorrelateById(context => context.Message.BookingId));
        }

        #region Events

        public Event<IBookingCreated> BookingCreatedEvent { get; private set; }

        public Event<IPaymentInitiated> PaymentInitiatedEvent { get; private set; }

        public Event<IPaymentSucceeded> PaymentSucceededEvent { get; private set; }

        public Event<IBookingSuccessNotified> BookingSuccessNotifiedEvent { get; private set; }
        
        public Event<IPaymentFailed> PaymentFailedEvent { get; private set; }

        public Event<IBookingFailureNotified> BookingFailureNotifiedEvent { get; private set; }
        
        public Event<IBookingCancelled> BookingCancelledEvent { get; private set; }

        #endregion

        #region States

        public State Created { get; private set; }

        public State PaymentInitiated { get; private set; }

        public State Succeeded { get; private set; }

        public State SuccessNotified { get; private set; }

        public State Failed { get; private set; }

        public State FailureNotified { get; private set; }

        public State Cancelled { get; private set; }

        #endregion
    }
}