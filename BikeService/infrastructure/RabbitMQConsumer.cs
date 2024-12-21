using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using BikeService.application;
using RabbitMQ.Client;
using BikeService.domain.entities;

namespace BikeService.infrastructure
{
    public class RabbitMqConsumer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBikeService _bikeService;
        private readonly IPositionNotifier _positionNotifier;

        public RabbitMqConsumer(IBikeService bikeService, IPositionNotifier positionNotifier)
        {
            _bikeService = bikeService;
            _positionNotifier = positionNotifier;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "rabbitmq";
            factory.Port = 5672;
            factory.UserName = "myuser";
            factory.Password = "mypassword";
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "bike-position-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.QueueBind(
                queue: "bike-position-queue",
                exchange: "bike.events",
                routingKey: "bike.position.updated"
            );
        }

        public Task StartAsync(CancellationToken cancellationToken)
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
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}
