using AutoMapper;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.ViewModels
{
    public class RideViewModel
    {
        public Guid Id { get; set; }
        public DriverViewModel Driver { get; set; } = null!;
        public LatLngViewModel StartingPoint { get; set; } = null!;
        public LatLngViewModel DestinationPoint { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? DestinationTime { get; set; }
        public string? Comment { get; set; }
        public double? Mileage { get; set; }
        public double? Cost { get; set; }
        public IdentityLookupViewModel Status { get; set; } = null!;
    }

    public class StartRideViewModel
    {
        public LatLngViewModel StartingPoint { get; set; } = null!;
        public string? Comment { get; set; }
    }

    public class FinishRideViewModel
    {
        public LatLngViewModel DestinationPoint { get; set; } = null!;
        public double Mileage { get; set; }
        public double? Cost { get; set; }
    }

    public class RideProfile : Profile
    {
        public RideProfile()
        {
            CreateMap<Ride, RideViewModel>();
            CreateMap<StartRideViewModel, Ride>();
            CreateMap<Ride, FinishRideViewModel>();
        }
    }
}
