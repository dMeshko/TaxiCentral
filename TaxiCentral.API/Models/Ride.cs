namespace TaxiCentral.API.Models
{
    public class Ride
    {
        public Guid Id { get; set; }
        public Driver Driver { get; set; } = null!;
        public LatLng? TargetStartingPoint { get; set; }
        public LatLng? TargetDestinationPoint { get; set; }
        public LatLng ActualStartingPoint { get; set; } = null!;
        public LatLng? ActualDestinationPoint { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? DestinationTime { get; set; }
        public string? Comment { get; set; }
        public double? Mileage { get; set; }
        public double? Cost { get; set; }
        public int? EstimatedTimeOfArrival { get; set; }
        public int? TimeOfArrival { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Waiting;
    }

    public enum RideStatus
    {
        Waiting,
        Current,
        Complete
    }
}
