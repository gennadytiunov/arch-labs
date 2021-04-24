using System;
using System.Threading;
using System.Threading.Tasks;
using Circus.Messaging;
using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.Hosting;
using OtusApp.Circus.Booking.Contract.Messages;
using OtusApp.Circus.Booking.Orchestrator.Saga.Messages.Events;
using Serilog;

namespace OtusApp.Circus.Booking.Orchestrator
{
    public class BookingMessageListener : BackgroundService
    {
        private readonly IBusControl _busControl;

        private static readonly string KafkaServers = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Servers"); // "localhost:9092"
        private static readonly string KafkaTopic = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Topic_Bookings"); // "bookings"
        private static readonly string ConsumerGroup = Environment.GetEnvironmentVariable("_OtusApp_Kafka_Group_Booking_BookingMessageListener"); // "Group.Booking.BookingMessageListener"

        private readonly MessageDeserializer _messageDeserializer;
        private readonly ILogger _logger = Log.ForContext<BookingMessageListener>();

        public BookingMessageListener(
            IBusControl busControl,
            MessageDeserializer messageDeserializer)
        {
            _busControl = busControl;
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

                        if (_messageDeserializer.TryDeserializeEvent(result.Message.Value, out BookingCreated bookingCreated))
                        {
                            _logger.Information($"{GetType().Name}: Message {result.Message.Value} is recognized by {GetType().Name} as {nameof(BookingCreated)} event and will be processed.");

	                        await _busControl.Send<IBookingCreated>(
	                            new
	                            {
		                            bookingCreated.BookingId,
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
