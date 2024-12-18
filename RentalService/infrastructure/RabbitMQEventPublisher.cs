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

        public RabbitMQEventPublisher()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "bike.events", type: "topic");
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

    }
}