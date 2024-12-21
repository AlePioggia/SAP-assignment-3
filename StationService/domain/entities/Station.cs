namespace StationService.domain.entities
{
    public class Station
    {
        public string Id { get; set; }
        public (int, int) Position { get; set; }
        public int Capacity { get; set; }
        public List<string> Bikes { get; set; }
        public DateTime CreatedAt { get; set; }

        public Station((int, int) position, int capacity, DateTime createdAt)
        {
            Id = Guid.NewGuid().ToString();
            Position = position;
            Capacity = capacity;
            Bikes = new List<string>();
            CreatedAt = createdAt;
        }
    }
}
