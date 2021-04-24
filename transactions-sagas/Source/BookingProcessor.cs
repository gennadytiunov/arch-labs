using System;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Options;
using OtusApp.Circus.Booking.Domain.Entities.Enums;
using OtusApp.Circus.Booking.Domain.Repositories;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Commands;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events;
using OtusApp.Circus.Kafka;
using Serilog;

namespace OtusApp.Circus.Booking.Orchestrator
{
    public class BookingProcessorDefinition : ConsumerDefinition<BookingProcessor> {}

    public class BookingProcessor :
	    IConsumer<MakePayment>,
	    IConsumer<IPaymentInitiated>,
		IConsumer<IPaymentSucceeded>,
	    IConsumer<IPaymentFailed>,
        IConsumer<NotifyBookingSuccess>,
	    IConsumer<NotifyBookingFailure>,
	    IConsumer<NotifyBookingCancellation>
    {
	    private readonly IBusControl _busControl;

        private readonly IBookingRepository _bookingRepository;
        private readonly IKafkaProxy _kafkaProxy;
        private readonly KafkaSettings _kafkaSettings;

        private readonly ILogger _logger = Log.ForContext<BookingProcessor>();

        public BookingProcessor(
	        IBusControl busControl,
			IBookingRepository bookingRepository,
            IKafkaProxy kafkaProxy,
            IOptions<KafkaSettings> kafkaSettings)
        {
	        _busControl = busControl;
            _bookingRepository = bookingRepository;
            _kafkaProxy = kafkaProxy;
            _kafkaSettings = kafkaSettings.Value;
        }

        public async Task Consume(ConsumeContext<MakePayment> context)
        {
            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

            var booking = _bookingRepository.Get(context.Message.BookingId);
            
			var paymentId = Guid.NewGuid();

            var makePayment = new Payment.Contract.Messages.MakePayment
            {
                PaymentId = paymentId,
                UserId = booking.UserId,
                Amount = booking.Amount,
                Currency = booking.Currency,
                Date = DateTime.UtcNow
            };

			_logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending IPaymentInitiated.");
            await _busControl.Publish<IPaymentInitiated>(new { BookingId = booking.Id, PaymentId = paymentId, DateTime.UtcNow });

            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending {makePayment.GetType().Name} : {JsonSerializer.Serialize(makePayment)}");
            await _kafkaProxy.SendMessage(_kafkaSettings.PaymentsTopic, makePayment);
        }

        public async Task Consume(ConsumeContext<IPaymentInitiated> context)
        {
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

	        await Task.Run(() =>
	        {
		        var booking = _bookingRepository.Get(context.Message.BookingId);
		        booking.PaymentId = context.Message.PaymentId;
		        booking.Status = BookingStatus.Created;
		        booking.SubStatus = BookingSubStatus.PaymentInitiated;
				booking.UpdateDate = DateTime.UtcNow;
		        _bookingRepository.Update(booking);
	        });

	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Booking status updated.");
        }

        public async Task Consume(ConsumeContext<IPaymentSucceeded> context)
        {
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

	        await Task.Run(() =>
	        {
				var booking = _bookingRepository.Get(context.Message.BookingId);
				booking.Status = BookingStatus.Succeeded;
				booking.SubStatus = BookingSubStatus.PaymentSucceeded;
				booking.UpdateDate = DateTime.UtcNow;
				_bookingRepository.Update(booking);
	        });

	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Booking status updated.");
        }

        public async Task Consume(ConsumeContext<IPaymentFailed> context)
        {
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

	        await Task.Run(() =>
	        {
				var booking = _bookingRepository.Get(context.Message.BookingId); 
				booking.Status = BookingStatus.Failed;
				booking.SubStatus = BookingSubStatus.PaymentFailed;
				booking.UpdateDate = DateTime.UtcNow;
				_bookingRepository.Update(booking);
	        });
            
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Booking status updated.");
        }

        public async Task Consume(ConsumeContext<NotifyBookingSuccess> context)
        {
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

	        var booking = _bookingRepository.Get(context.Message.BookingId);

            var notifyBookingSuccess = new Notification.Contract.Messages.NotifyBookingSuccess
	        {
		        BookingId = booking.Id,
		        Email = booking.Email,
		        Amount = booking.Amount,
		        Currency = booking.Currency,
		        Date = DateTime.UtcNow
	        };

            await Task.Delay(5000);

            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending IBookingSuccessNotified.");
            await _busControl.Send<IBookingSuccessNotified>(new { BookingId = booking.Id, DateTime.UtcNow });

            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending {notifyBookingSuccess.GetType().Name} : {JsonSerializer.Serialize(notifyBookingSuccess)}");
	        await _kafkaProxy.SendMessage(_kafkaSettings.NotificationsTopic, notifyBookingSuccess);

	        booking.Status = BookingStatus.Succeeded;
            booking.SubStatus = BookingSubStatus.BookingSuccessNotified;

			_bookingRepository.Update(booking);
        }

        public async Task Consume(ConsumeContext<NotifyBookingFailure> context)
        {
	        _logger.Information($"{GetType().Name}: {context.Message.GetType().Name} received: {JsonSerializer.Serialize(context.Message)}");

	        var booking = _bookingRepository.Get(context.Message.BookingId);

	        var notifyBookingFailure = new Notification.Contract.Messages.NotifyBookingFailure
	        {
		        BookingId = booking.Id,
		        Email = booking.Email,
		        Amount = booking.Amount,
		        Currency = booking.Currency,
		        Reason = context.Message.Reason,
		        Date = DateTime.UtcNow
	        };

	        await Task.Delay(5000);
            
            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending IBookingFailureNotified.");
	        await _busControl.Send<IBookingFailureNotified>(new { BookingId = booking.Id, DateTime.UtcNow });

            _logger.Information($"{GetType().Name}: {context.Message.GetType().Name}: Sending {notifyBookingFailure.GetType().Name} : {JsonSerializer.Serialize(notifyBookingFailure)}");
            await _kafkaProxy.SendMessage(_kafkaSettings.NotificationsTopic, notifyBookingFailure);

	        booking.Status = BookingStatus.Failed;
	        booking.SubStatus = BookingSubStatus.BookingFailureNotified;
	        _bookingRepository.Update(booking);
        }

        public async Task Consume(ConsumeContext<NotifyBookingCancellation> context)
        {
	        throw new NotImplementedException();
        }
    }
}