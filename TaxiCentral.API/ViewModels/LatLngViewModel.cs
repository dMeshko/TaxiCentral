using AutoMapper;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.ViewModels
{
    public class LatLngViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class LatLngProfile : Profile
    {
        public LatLngProfile()
        {
            CreateMap<LatLng, LatLngViewModel>();
            CreateMap<LatLngViewModel, LatLng>();
        }
    }
}
