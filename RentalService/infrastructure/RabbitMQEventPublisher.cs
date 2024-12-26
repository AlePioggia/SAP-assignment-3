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

        public async Task PerformChargingSequenceAsync(string bikeId, int x, int y)
        {
            // 1. Get nearest station
            var nearestStationEvent = new RideEvents.RequestNearestStationEvent(x, y);
            var nearestStationPosition = await PublishRequestAsync<RideEvents.RequestNearestStationEvent, (int, int)>(
                nearestStationEvent,
                routingKey: "stationRequested",
                replyQueue: "rental_service_reply_queue"
            );

            // 2. Go to digital station
            // var goToStationEvent = new RideEvents.GoToStationEvent(bikeId, nearestStationPosition.Item1, nearestStationPosition.Item2);
            // var stationArrivalResponse = await PublishRequestAsync<RideEvents.GoToStationEvent, StationArrivalResponse>(
            //     goToStationEvent,
            //     routingKey: "goToStation",
            //     replyQueue: "rental_service_reply_queue"
            // );

            // if (stationArrivalResponse == null || !stationArrivalResponse.Success)
            // {
            //     Console.WriteLine("Failed to reach the station.");
            //     return;
            // }

            // Console.WriteLine($"Bike {bikeId} arrived at station.");

            // 3. Charge eBike
            var chargeEbikeEvent = new RideEvents.ChargeEBikeEvent(bikeId, nearestStationPosition.Item1, nearestStationPosition.Item2);
            var chargeResponse = await PublishRequestAsync<RideEvents.ChargeEBikeEvent, string>(
                chargeEbikeEvent,
                routingKey: "chargeEbike",
                replyQueue: "rental_service_reply_queue"
            );
        }


        private async Task<TResponse?> PublishRequestAsync<TRequest, TResponse>(TRequest request, string routingKey, string replyQueue, int timeoutMs = 5000)
        {
            // Serializza il messaggio di richiesta
            var message = JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(message);

            // Creazione di un ID univoco per correlare la richiesta e la risposta
            var correlationId = Guid.NewGuid().ToString();

            // Proprietà del messaggio, incluso il campo ReplyTo
            var props = _channel.CreateBasicProperties();
            props.ReplyTo = replyQueue;
            props.CorrelationId = correlationId;

            // Dichiarazione della coda di risposta
            _channel.QueueDeclare(queue: replyQueue, durable: false, exclusive: false, autoDelete: true, arguments: null);

            // Pubblicazione del messaggio
            _channel.BasicPublish(exchange: "stationExchange", routingKey: routingKey, basicProperties: props, body: body);

            // Attesa della risposta
            var tcs = new TaskCompletionSource<TResponse?>();

            // Consumatore per leggere la risposta dalla coda
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId) // Controlla l'ID di correlazione
                {
                    var responseJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var response = JsonSerializer.Deserialize<TResponse>(responseJson);
                    tcs.SetResult(response); // Imposta il risultato
                }
            };

            _channel.BasicConsume(queue: replyQueue, autoAck: true, consumer: consumer);

            // Timeout per evitare che il task resti bloccato
            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                try
                {
                    cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
                    return await tcs.Task;
                }
                catch (TaskCanceledException)
                {
                    return default; // Timeout: restituisce un valore di default
                }
            }
        }

    }



}