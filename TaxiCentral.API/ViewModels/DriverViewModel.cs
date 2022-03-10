using AutoMapper;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.ViewModels
{
    public class DriverViewModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
    }

    public class CreateDriverViewModel
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Pin { get; set; } = null!;
    }

    public class UpdateDriverViewModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Pin { get; set; }
    }

    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverViewModel>()
                .ForMember(x => x.FullName, 
                    y => y.MapFrom(q => q.ToString()));
            CreateMap<CreateDriverViewModel, Driver>();
            CreateMap<UpdateDriverViewModel, Driver>();
        }
    }
}
