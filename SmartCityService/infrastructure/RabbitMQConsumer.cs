
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using SmartCityService.domain.entities;
using SmartCityService.application;

namespace SmartCityService.infrastructure
{
    public class RabbitMQConsumer : IHostedService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly application.SmartCityService _smartCityService;

        public RabbitMQConsumer(IConfiguration configuration, application.SmartCityService bootstrapService)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "rabbitmq";
            factory.Port = 5672;
            factory.UserName = "myuser";
            factory.Password = "mypassword";

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _smartCityService = bootstrapService;

            _channel.QueueDeclare("digital_twin_init_bikes_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("digital_twin_init_stations_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("digital_twin_charge_request_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare("digital_twin_reach_user_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bikeConsumer = new EventingBasicConsumer(_channel);
            bikeConsumer.Received += async (model, ea) => await HandleBikeInitializationAsync(ea);
            _channel.BasicConsume(queue: "digital_twin_init_bikes_queue", autoAck: true, consumer: bikeConsumer);

            var stationConsumer = new EventingBasicConsumer(_channel);
            stationConsumer.Received += async (model, ea) => await HandleStationInitializationAsync(ea);
            _channel.BasicConsume(queue: "digital_twin_init_stations_queue", autoAck: true, consumer: stationConsumer);

            var chargeBikeConsumer = new EventingBasicConsumer(_channel);
            chargeBikeConsumer.Received += async (model, ea) => await HandleChargeBikeRequestAsync(ea);
            _channel.BasicConsume(queue: "digital_twin_charge_request_queue", autoAck: true, consumer: chargeBikeConsumer);

            var reachUserConsumer = new EventingBasicConsumer(_channel);
            reachUserConsumer.Received += async (model, ea) => await HandleReachUserRequestAsync(ea);
            _channel.BasicConsume(queue: "digital_twin_reach_user_queue", autoAck: true, consumer: reachUserConsumer);

            Console.WriteLine("RabbitMQ Consumers Started.");
            return Task.CompletedTask;
        }

        private async Task HandleBikeInitializationAsync(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var bikes = JsonSerializer.Deserialize<List<Bike>>(message);
                if (bikes != null)
                {
                    _smartCityService.InitializeBikes(bikes);
                    Console.WriteLine($"[Digital Twin] Initialized with {bikes.Count} bikes.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to initialize bikes: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task HandleStationInitializationAsync(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var stations = JsonSerializer.Deserialize<List<Station>>(message);
                if (stations != null)
                {
                    _smartCityService.InitializeStations(stations);
                    Console.WriteLine($"[Digital Twin] Initialized with {stations.Count} stations.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to initialize stations: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private async Task HandleReachUserRequestAsync(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try 
            {
                var request = JsonSerializer.Deserialize<ReachUserEvent>(message);
                if (request != null)
                {
                    var bike = _smartCityService.GetBikeById(request.bikeId);
                    if (bike == null)
                    {
                        Console.WriteLine($"[Error] Bike with ID {request.bikeId} not found.");
                        return;
                    }

                    int currentX = bike.X;
                    int currentY = bike.Y;
                    int targetX = request.x;
                    int targetY = request.y;

                    while (currentX != targetX || currentY != targetY) 
                    {
                        if (currentX < targetX) currentX++;
                        else if (currentX > targetX) currentX--;

                        if (currentY < targetY) currentY++;
                        else if (currentY > targetY) currentY--;

                        bike.X = currentX;
                        bike.Y = currentY;
                        _smartCityService.AddOrUpdateBike(bike);

                        var progressEvent = new BikeUpdatedEvent(bike.Id, currentX, currentY);
                        var progressMessage = JsonSerializer.Serialize(progressEvent);
                        var progressBytes = Encoding.UTF8.GetBytes(progressMessage);

                        _channel.BasicPublish(
                            exchange: "",
                            routingKey: "bike-position-queue",
                            basicProperties: null,
                            body: progressBytes
                        );

                        await Task.Delay(500);
                    }

                    var bikeReachedEvent = new ReachUserEvent(bike.Id, request.x, request.y);

                    _smartCityService.AddOrUpdateBike(bike);

                    var bikeUpdatedEvent = new BikeUpdatedEvent(bike.Id, request.x, request.y);
                    var bikeReachedMessage = JsonSerializer.Serialize(bikeReachedEvent);
                    var bikeReachedBytes = Encoding.UTF8.GetBytes(bikeReachedMessage);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: "bike-reached-user-queue",
                        basicProperties: null,
                        body: bikeReachedBytes
                    );
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to process reach user request: {ex.Message}");
            }
        }

        private async Task HandleChargeBikeRequestAsync(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var request = JsonSerializer.Deserialize<ChargeBikeRequest>(message);
                if (request != null)
                {
                    Console.WriteLine($"[Digital Twin] Received charge request for bike {request.BikeId}.");

                    var bike = _smartCityService.GetBikeById(request.BikeId);
                    if (bike == null)
                    {
                        Console.WriteLine($"[Error] Bike with ID {request.BikeId} not found.");
                        return;
                    }

                    Station station = null;
                    station = _smartCityService.FindNearestStation(bike.X, bike.Y);
                    if (station == null)
                    {
                        Console.WriteLine($"[Error] No suitable station found for bike {request.BikeId}.");
                        return;
                    }

                    var updatedBike = _smartCityService.ChargeBike(request.BikeId, station.Id);

                    var chargeNotification = new ChargeBikeRequest(request.BikeId, station.Id); 

                    var notificationMessage = JsonSerializer.Serialize(chargeNotification);
                    var notificationBytes = Encoding.UTF8.GetBytes(notificationMessage);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: "station_charge_notification_queue",
                        basicProperties: null,
                        body: notificationBytes
                    );

                    var bikeUpdatedEvent = new BikeUpdatedEvent(updatedBike.Id, updatedBike.X, updatedBike.Y);
                    var bikeUpdatedMessage = JsonSerializer.Serialize(bikeUpdatedEvent);    
                    var bikeUpdatedBytes = Encoding.UTF8.GetBytes(bikeUpdatedMessage);

                    _channel.BasicPublish(
                        exchange: "",
                        routingKey: "bike-position-queue",
                        basicProperties: null,
                        body: bikeUpdatedBytes
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to process charge bike request: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ Consumers Stopping...");
            Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
