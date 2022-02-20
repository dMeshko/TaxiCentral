using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public class RidesDataStore
    {
        public List<RideDto> Rides { get; set; }
        public static RidesDataStore Current { get; } = new RidesDataStore();

        public RidesDataStore()
        {
            Rides = new List<RideDto>()
            {
                new RideDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "Bitola"
                },
                new RideDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "Mogila"
                },
                new RideDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "Skopje"
                }
            };
        }
    }
}
