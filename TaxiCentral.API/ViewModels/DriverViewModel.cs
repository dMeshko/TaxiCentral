using AutoMapper;
using FluentValidation;
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

    public class CreateDriverViewModelValidator : AbstractValidator<CreateDriverViewModel>
    {
        public CreateDriverViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(30);

            RuleFor(x => x.Surname)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50);

            RuleFor(x => x.Pin)
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(10);
        }
    }

    public class UpdateDriverViewModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Pin { get; set; }
    }

    public class UpdateDriverViewModelValidator : AbstractValidator<UpdateDriverViewModel>
    {
        public UpdateDriverViewModelValidator()
        {
            RuleFor(x => x.Name)
                .MinimumLength(2)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .MaximumLength(30)
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Surname)
                .MinimumLength(3)
                .When(x => !string.IsNullOrWhiteSpace(x.Surname))
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.Surname));

            RuleFor(x => x.Pin)
                .MinimumLength(4)
                .When(x => !string.IsNullOrWhiteSpace(x.Pin))
                .MaximumLength(10)
                .When(x => !string.IsNullOrWhiteSpace(x.Pin));
        }
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
