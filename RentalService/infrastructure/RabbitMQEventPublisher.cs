using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RentalService.application;
using RentalService.domain.events;
using System.Text;
using System.Text.Json;

namespace RentalService.infrastructure
{
    public class RabbitMQEventPublisher : IEventPublisher, IDisposable
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQEventPublisher(IConfiguration configuration)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
            factory.UserName = "myuser";
            factory.Password = "mypassword";

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "bike.events", type: "topic");

            _channel.QueueDeclare(queue: "bike-position-queue",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            _channel.QueueBind(queue: "bike-position-queue",
                            exchange: "bike.events",
                            routingKey: "bike.position.updated");

            _channel.ExchangeDeclare("stationExchange", ExchangeType.Direct);

            _channel.QueueDeclare("stationRequestedQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("allStationsRequestedQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("chargeEbikeQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind("stationRequestedQueue", "stationExchange", "stationRequested");
            _channel.QueueBind("allStationsRequestedQueue", "stationExchange", "allStationsRequested");
            _channel.QueueBind("chargeEbikeQueue", "stationExchange", "chargeEbike");
        }

        public async Task PublishAsync(RideEvents.BikePositionUpdatedEvent positionEvent)
        {
            var message = JsonSerializer.Serialize(positionEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "bike.events",
                                  routingKey: "bike.position.updated",
                                  basicProperties: null,
                                  body: body);

            await Task.CompletedTask;
        }

        public async Task PublishAsync(RideEvents.RideEndedEvent rideEndedEvent)
        {
            var message = JsonSerializer.Serialize(rideEndedEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "bike.events",
                                  routingKey: "ride.ended",
                                  basicProperties: null,
                                  body: body);

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }

        public async Task PublishAsync(RideEvents.ChargeEBikeEvent chargeEBikeEvent)
        {
            var message = JsonSerializer.Serialize(chargeEBikeEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "stationExchange",
                                  routingKey: "chargeEbike",
                                  basicProperties: null,
                                  body: body);

            await Task.CompletedTask;
        }

        public async Task PublishAsync(RideEvents.RequestStationInfoEvent requestStationInfoEvent)
        {
            var message = JsonSerializer.Serialize(requestStationInfoEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "stationExchange",
                                  routingKey: "stationRequested",
                                  basicProperties: null,
                                  body: body);

            await Task.CompletedTask;
        }

        public async Task PublishAsync(RideEvents.RequestAllStationsEvent requestAllStationsEvent)
        {
            var message = JsonSerializer.Serialize(requestAllStationsEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "stationExchange",
                                  routingKey: "allStationsRequested",
                                  basicProperties: null,
                                  body: body);

            await Task.CompletedTask;
        }
    }
}