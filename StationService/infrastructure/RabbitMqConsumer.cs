
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StationService.application;
using System.Text;
using System.Text.Json;
using static StationService.domain.entities.StationEvents;

namespace StationService.infrastructure
{
    public class RabbitMqConsumer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IStationService _stationService;

        public RabbitMqConsumer(IConnection connection, IModel channel, IStationService stationService)
        {
            _stationService = stationService;

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "rabbitmq";
            factory.Port = 5672;
            factory.UserName = "myuser";
            factory.Password = "mypassword";
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (ea.RoutingKey == "stationRequested")
                    {
                        var stationRequestedEvent = JsonSerializer.Deserialize<StationRequestedEvent>(message);
                        if (stationRequestedEvent != null)
                        {
                            var station = await _stationService.GetStationByIdAsync(stationRequestedEvent.Id);
                            if (station != null)
                            {
                                var stationJson = JsonSerializer.Serialize(station);
                                var responseBytes = Encoding.UTF8.GetBytes(stationJson);
                                _channel.BasicPublish(
                                    exchange: "",
                                    routingKey: ea.BasicProperties.ReplyTo,
                                    basicProperties: null,
                                    body: responseBytes
                                );
                            }
                        }
                    }
                    else if (ea.RoutingKey == "allStationsRequested")
                    {
                        var allStationsRequestedEvent = JsonSerializer.Deserialize<AllStationsRequestedEvent>(message);
                        if (allStationsRequestedEvent != null)
                        {
                            var stations = await _stationService.GetStationsAsync();
                            var stationsJson = JsonSerializer.Serialize(stations);
                            var responseBytes = Encoding.UTF8.GetBytes(stationsJson);
                            _channel.BasicPublish(
                                exchange: "",
                                routingKey: ea.BasicProperties.ReplyTo,
                                basicProperties: null,
                                body: responseBytes
                            );
                        }
                    }
                    else if (ea.RoutingKey == "chargeEbike")
                    {
                        var chargeEbikeEvent = JsonSerializer.Deserialize<ChargeEbike>(message);
                        if (chargeEbikeEvent != null)
                        {
                            var bikeId = chargeEbikeEvent.BikeId;
                            var stationId = chargeEbikeEvent.StationId;

                            var result = await _stationService.chargeBike(stationId, bikeId);
                            var responseBytes = Encoding.UTF8.GetBytes(result);
                            _channel.BasicPublish(
                                exchange: "",
                                routingKey: ea.BasicProperties.ReplyTo,
                                basicProperties: null,
                                body: responseBytes
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            };
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}
