namespace BikeService.domain.entities
{
    public class Bike
    {
        public Guid Id { get; private set; }
        public string Model { get; private set; }
        public string Status { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private List<object> _changes = new();
        public IEnumerable<object> Changes => _changes;

        public Bike(Guid id, string model, int X, int Y)
        {
            Apply(new BikeCreatedEvent(id, model, "Available", X, Y));
        }

        public void UpdatePosition(int X, int Y)
        {
            Apply(new BikePositionUpdatedEvent(Id, X, Y));
        }

        public void ChangeStatus(string newStatus)
        {
            Apply(new BikeStatusChangedEvent(Id, newStatus));
        }

        private void Apply(object @event)
        {
            When(@event);
            _changes.Add(@event);
        }

        private void When(object @event)
        {
            switch (@event)
            {
                case BikeCreatedEvent e:
                    Id = e.BikeId;
                    Model = e.Model;
                    Status = e.Status;
                    X = e.X;
                    Y = e.Y;
                    break;
                case BikePositionUpdatedEvent e:
                    X = e.X;
                    Y = e.Y;
                    break;
                case BikeStatusChangedEvent e:
                    Status = e.Status;
                    break;
            }
        }

        public void Load(IEnumerable<object> history)
        {
            foreach (var e in history) When(e);
        }
    }

}
