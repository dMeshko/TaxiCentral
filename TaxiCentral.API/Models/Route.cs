namespace TaxiCentral.API.Models
{
    public class Route
    {
        public Guid Id { get; set; }
        public LatLng TargetStartingPoint { get; set; } = null!;
        public LatLng TargetDestinationPoint { get; set; }
        public string? ClientComment { get; set; }
        public string? DriverComment { get; set; }
        public int? EstimatedTimeOfArrival { get; set; }
        //todo: add ReportedBy
        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ArrivalTime { get; set; }
        public Ride? Ride { get; set; }
        public Driver? Driver { get; set; }
        public RouteStatus Status { get; set; } = RouteStatus.Pending;
        public string? VoidReason { get; set; }
    }

    public enum RouteStatus
    {
        Pending,
        Current,
        WaitingForClient,
        Complete,
        Voided
    }
}
