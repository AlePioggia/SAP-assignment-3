namespace RentalService.domain.entities
{
    public class Ride
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string EBikeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CreditUsed { get; set; } = 0;

        public Ride(string userId, string eBikeId)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            EBikeId = eBikeId;
            StartTime = DateTime.UtcNow;
        }

        public void DeductCredit(int amount)
        {
            CreditUsed += amount;
        }

        public void EndRide()
        {
            EndTime = DateTime.UtcNow;
        }

        public TimeSpan GetRideDuration()
        {
            return EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;
        }
    }
}
