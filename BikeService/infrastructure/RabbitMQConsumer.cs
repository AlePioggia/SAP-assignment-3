using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using BikeService.application;
using RabbitMQ.Client;
using BikeService.domain.entities;

namespace BikeService.infrastructure
{
    public class RabbitMqConsumer
    {
        private readonly IConnection _connection;
        private IModel _channel;
        private readonly IBikeService _bikeService;
        private readonly IPositionNotifier _positionNotifier;

        public RabbitMqConsumer(IBikeService bikeService, IPositionNotifier positionNotifier)
        {
            _bikeService = bikeService;
            _positionNotifier = positionNotifier;

            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = _connection?.CreateModel();

            _channel?.QueueDeclare("bike-position-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var bikePositionEvent = JsonSerializer.Deserialize<BikePositionUpdatedEvent>(message);
                    if (bikePositionEvent != null)
                    {
                        await _bikeService.UpdateBikePosition(
                            bikePositionEvent.BikeId,
                            bikePositionEvent.X,
                            bikePositionEvent.Y
                        );

                        await _positionNotifier.NotifyPositionAsync(
                            bikePositionEvent.BikeId,
                            bikePositionEvent.X,
                            bikePositionEvent.Y
                        );

                        Console.WriteLine("Event consumed and processed successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: "bike-position-queue", autoAck: true, consumer: consumer);
            Console.WriteLine("RabbitMQ consumer started...");
        }
    }

}
