using System.ComponentModel.DataAnnotations;

namespace TaxiCentral.API.Models
{
    public class RideDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ScheduleRideViewModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
