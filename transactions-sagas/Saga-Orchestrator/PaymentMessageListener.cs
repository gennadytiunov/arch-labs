using System;
using System.Threading;
using System.Threading.Tasks;
using Circus.Messaging;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Hosting;
using OtusApp.Circus.Booking.Domain.Repositories;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events;
using OtusApp.Circus.Payment.Contract.Messages;
using Serilog;

namespace OtusApp.Circus.Booking.Orchestrator
{
    public class PaymentMessageListener : BackgroundService
    {
        private readonly IBusControl _busControl;
        private readonly IBookingRepository _bookingRepository;

        private static readonly string KafkaServers = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Servers"); // "localhost:9092"
        private static readonly string KafkaTopic = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Topic_Payments_Replies"); // "payments-replies"
        private static readonly string ConsumerGroup = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Group_Booking_PaymentMessageListener"); // "Group.Booking.PaymentMessageListener"

        private readonly MessageDeserializer _messageDeserializer;
        private readonly ILogger _logger = Log.ForContext<PaymentMessageListener>();

        public PaymentMessageListener(
            IBusControl busControl,
            IBookingRepository bookingRepository,
            MessageDeserializer messageDeserializer)
        {
            _busControl = busControl;
            _bookingRepository = bookingRepository;
            _messageDeserializer = messageDeserializer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
             await Task.Run(async () =>
             {
                 await DoWork(stoppingToken);

             }, stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = ConsumerGroup,
                BootstrapServers = KafkaServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _logger.Information($"{GetType().Name}: Creating consumer for '{ConsumerGroup}' pointed at '{KafkaServers}'.");
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            _logger.Information($"{GetType().Name}: Subscribing the consumer to topic '{KafkaTopic}'.");
            consumer.Subscribe(KafkaTopic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.Information($"{GetType().Name}: Consuming next message.");
                        var result = consumer.Consume(stoppingToken);
                        _logger.Information($"{GetType().Name}: Consumed message '{result.Message.Value}' at offset '{result.TopicPartitionOffset}'.");

                        if (_messageDeserializer.TryDeserializeEvent(result.Message.Value, out PaymentSucceeded paymentSucceeded))
                        {
                            
	                        _logger.Information($"{GetType().Name}: Message {result.Message.Value} is recognized by {GetType().Name} as {nameof(PaymentSucceeded)} event and will be processed.");

	                        var booking = _bookingRepository.GetByPayment(paymentSucceeded.PaymentId);

	                        await _busControl.Publish<IPaymentSucceeded>(
		                        new
		                        {
			                        BookingId = booking.Id,
			                        DateTime.UtcNow
		                        }, stoppingToken);

                            _logger.Information($"{GetType().Name}: Message {result.Message.Value} was processed.");

                            continue;
                        }

                        if (_messageDeserializer.TryDeserializeEvent(result.Message.Value, out PaymentFailed paymentFailed))
                        {
                            _logger.Information($"{GetType().Name}: Message {result.Message.Value} is recognized by {GetType().Name} as {nameof(PaymentFailed)} event and will be processed.");

                            var booking = _bookingRepository.GetByPayment(paymentSucceeded.PaymentId);

                            await _busControl.Publish<IPaymentFailed>(
                                new
                                {
									BookingId = booking.Id,
									paymentFailed.Reason,
                                    DateTime.UtcNow
                                }, stoppingToken);

                            _logger.Information($"{GetType().Name}: Message {result.Message.Value} was processed.");

                            continue;
                        }

                        _logger.Information($"{GetType().Name}: Message {result.Message.Value} is not recognized by {GetType().Name} and will be skipped.");
                    }
                    catch (ConsumeException e)
                    {
                        _logger.Error($"{GetType().Name}: Error occurred: {e.Error.Reason}.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
        }
    }
}
