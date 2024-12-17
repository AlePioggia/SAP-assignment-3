using BikeService.application;
using BikeService.domain.entities;
using EventStore.Client;
using System.Text;
using System.Text.Json;

namespace BikeService.infrastructure
{
    public class BikeRepository : IBikeRepository
    {
        private readonly EventStoreClient _client;

        public BikeRepository(EventStoreClient client)
        {
            _client = client;
        }

        public async Task SaveAsync(string streamName, IEnumerable<object> events)
        {
            var eventData = events.Select(e =>
                new EventData(
                    Uuid.NewUuid(),
                    e.GetType().Name,
                    JsonSerializer.SerializeToUtf8Bytes(e)
             ));

            await _client.AppendToStreamAsync(streamName, StreamState.Any, eventData);
        }

        public async Task<IEnumerable<object>> LoadAsync(string streamName)
        {
            var result = _client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start);

            var events = new List<object>();
            await foreach (var resolvedEvent in result)
            {
                var eventType = resolvedEvent.Event.EventType;

                var dataBytes = resolvedEvent.Event.Data.ToArray();

                var dataJson = Encoding.UTF8.GetString(dataBytes);

                events.Add(eventType switch
                {
                    nameof(BikeCreatedEvent) => JsonSerializer.Deserialize<BikeCreatedEvent>(dataJson),
                    nameof(BikePositionUpdatedEvent) => JsonSerializer.Deserialize<BikePositionUpdatedEvent>(dataJson),
                    nameof(BikeStatusChangedEvent) => JsonSerializer.Deserialize<BikeStatusChangedEvent>(dataJson),
                    _ => null
                });
            }
            return events;
        }
    }
}
