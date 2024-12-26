namespace SmartCityService.domain.entities
{
    public class Bike
    {
        public string Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Bike(string id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

    }

}
