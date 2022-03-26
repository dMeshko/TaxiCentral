namespace TaxiCentral.API.Models
{
    public class Ride
    {
        public Guid Id { get; set; }
        public LatLng ActualStartingPoint { get; set; } = null!;
        public LatLng? ActualDestinationPoint { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? DestinationTime { get; set; }
        public double? Mileage { get; set; }
        public double? Cost { get; set; }
        /// <summary>
        /// Actual time of arrival since the call was made
        /// </summary>
        public int? TimeOfArrival { get; set; }
        /// <summary>
        /// Waiting time in minutes
        /// </summary>
        public int? WaitingTime { get; set; }
        public RideStatus Status { get; set; } = RideStatus.Current;
    }

    public enum RideStatus
    {
        Current,
        Complete
    }
}
