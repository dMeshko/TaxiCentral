using AutoMapper;
using FluentValidation;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.ViewModels
{
    public class RideViewModel
    {
        public Guid Id { get; set; }
        public DriverViewModel Driver { get; set; } = null!;
        public LatLngViewModel StartingPoint { get; set; } = null!;
        public LatLngViewModel? DestinationPoint { get; set; }
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

    public class StartRideViewModelValidator : AbstractValidator<StartRideViewModel>
    {
        public StartRideViewModelValidator()
        {
            RuleFor(x => x.StartingPoint.Latitude)
                .NotEmpty();

            RuleFor(x => x.StartingPoint.Longitude)
                .NotEmpty();
        }
    }

    public class FinishRideViewModel
    {
        public LatLngViewModel DestinationPoint { get; set; } = null!;
        public double Mileage { get; set; }
        public double? Cost { get; set; }
    }

    public class FinishRideViewModelValidator : AbstractValidator<FinishRideViewModel>
    {
        public FinishRideViewModelValidator()
        {
            RuleFor(x => x.DestinationPoint.Latitude)
                .NotEmpty();

            RuleFor(x => x.DestinationPoint.Longitude)
                .NotEmpty();

            RuleFor(x => x.Mileage)
                .NotEmpty();
        }
    }

    public class BroadcastRideViewModel
    {
        public LatLngViewModel TargetStartingPoint { get; set; } = null!;
        public LatLngViewModel TargetDestinationPoint { get; set; } = null!;
        public string? Comment { get; set; }
    }

    public class BroadcastRideViewModelValidator : AbstractValidator<BroadcastRideViewModel>
    {
        public BroadcastRideViewModelValidator()
        {
            RuleFor(x => x.TargetStartingPoint.Latitude)
                .NotEmpty();

            RuleFor(x => x.TargetStartingPoint.Longitude)
                .NotEmpty();

            RuleFor(x => x.TargetDestinationPoint.Latitude)
                .NotEmpty();

            RuleFor(x => x.TargetDestinationPoint.Longitude)
                .NotEmpty();
        }
    }

    public class RideProfile : Profile
    {
        public RideProfile()
        {
            CreateMap<Ride, RideViewModel>();
            CreateMap<StartRideViewModel, Ride>();
            CreateMap<FinishRideViewModel, Ride>();
        }
    }
}
