using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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
            _channel.QueueDeclare("reachUserQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);  

            _channel.QueueBind("stationRequestedQueue", "stationExchange", "stationRequested");
            _channel.QueueBind("allStationsRequestedQueue", "stationExchange", "allStationsRequested");
            _channel.QueueBind("chargeEbikeQueue", "stationExchange", "chargeEbike");
            _channel.QueueBind("reachUserQueue", "reachUserExchange", "reachUser");
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

        public async Task PerformChargingSequenceAsync(string bikeId, int x, int y)
        { 
            throw new NotImplementedException();
        }


        private async Task<TResponse?> PublishRequestAsync<TRequest, TResponse>(TRequest request, string routingKey, string replyQueue, int timeoutMs = 5000)
        {
            var message = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(message);

            var correlationId = Guid.NewGuid().ToString();

            var props = _channel.CreateBasicProperties();
            props.ReplyTo = replyQueue;
            props.CorrelationId = correlationId;

            _channel.QueueDeclare(queue: replyQueue, durable: false, exclusive: false, autoDelete: true, arguments: null);

            _channel.BasicPublish(exchange: "stationExchange", routingKey: routingKey, basicProperties: props, body: body);

            var tcs = new TaskCompletionSource<TResponse?>();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var responseJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var response = JsonSerializer.Deserialize<TResponse>(responseJson);
                    tcs.SetResult(response); 
                }
            };

            _channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);

            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                try
                {
                    cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
                    return await tcs.Task;
                }
                catch (TaskCanceledException)
                {
                    return default;
                }
            }
        }

        public async Task PublishAsync(RideEvents.ReachUserEvent reachUserEvent)
        {
            var message = JsonSerializer.Serialize(reachUserEvent);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "reachUserExchange",
                                  routingKey: "reachUser",
                                  basicProperties: null,
                                  body: body);

            await WaitForBikeToReachUser(reachUserEvent.BikeId);

            await Task.CompletedTask;
        }

        private async Task WaitForBikeToReachUser(string bikeId)
        {
            var tcs = new TaskCompletionSource();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventMessage = JsonSerializer.Deserialize<RideEvents.BikeReachedUserEvent>(message);

                if (eventMessage != null && eventMessage.BikeId == bikeId)
                {
                    tcs.SetResult();
                }
            };

            _channel.BasicConsume(queue: "bike-reached-queue", autoAck: true, consumer: consumer);

            await tcs.Task;
        }

        public async Task WaitForPositionUpdateConfirmation(string bikeId)
        {
            var tcs = new TaskCompletionSource();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventMessage = JsonSerializer.Deserialize<RideEvents.BikePositionUpdatedConfirmationEvent>(message);

                if (eventMessage != null && eventMessage.BikeId == bikeId)
                {
                    tcs.SetResult();
                }
            };

            _channel.BasicConsume(queue: "bike-position-updated-confirmation-queue", autoAck: true, consumer: consumer);

            await tcs.Task;
        }
        
    }



}