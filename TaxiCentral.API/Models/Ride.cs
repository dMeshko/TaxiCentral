namespace TaxiCentral.API.Models
{
    public class Ride
    {
        public Guid Id { get; set; }
        public Driver Driver { get; set; } = null!;
        public LatLng StartingPoint { get; set; } = null!;
        public LatLng? DestinationPoint { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? DestinationTime { get; set; }
        public string? Comment { get; set; }
        public double? Mileage { get; set; }
        public double? Cost { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Current;
    }

    public enum RideStatus
    {
        Current,
        Complete
    }
}
