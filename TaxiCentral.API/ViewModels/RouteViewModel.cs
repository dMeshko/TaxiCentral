using AutoMapper;
using FluentValidation;
using Route = TaxiCentral.API.Models.Route;

namespace TaxiCentral.API.ViewModels
{
    public class RouteViewModel
    {
        public Guid Id { get; set; }
        public LatLngViewModel TargetStartingPoint { get; set; } = null!;
        public LatLngViewModel? TargetDestinationPoint { get; set; }
        public string? Comment { get; set; }
        public int? EstimatedTimeOfArrival { get; set; }
        public DateTime ReportedAt { get; set; }
        public IdentityLookupViewModel Status { get; set; } = null!;
        public string? VoidReason { get; set; }
    }

    public class AddRouteViewModel
    {
        public LatLngViewModel TargetStartingPoint { get; set; } = null!;
        public LatLngViewModel? TargetDestinationPoint { get; set; }
        public string? Comment { get; set; }
    }

    public class AddRouteViewModelValidator : AbstractValidator<AddRouteViewModel>
    {
        public AddRouteViewModelValidator()
        {
            RuleFor(x => x.TargetStartingPoint.Latitude)
                .NotEmpty();

            RuleFor(x => x.TargetStartingPoint.Longitude)
                .NotEmpty();
        }
    }

    public class AcceptRideViewModel
    {
        public Guid RouteId { get; set; }
        public int EstimatedTimeOfArrival { get; set; }
    }

    public class TerminateRouteViewModel
    {
        public Guid RouteId { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class TerminateRouteViewModelValidator : AbstractValidator<TerminateRouteViewModel>
    {
        public TerminateRouteViewModelValidator()
        {
            RuleFor(x => x.RouteId)
                .NotEmpty();

            RuleFor(x => x.Reason)
                .NotEmpty();
        }
    }

    public class RouteProfile : Profile
    {
        public RouteProfile()
        {
            CreateMap<Route, RouteViewModel>();
            CreateMap<AddRouteViewModel, Route>();
        }
    }
}
