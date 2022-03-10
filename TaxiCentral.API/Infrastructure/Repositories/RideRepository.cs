using Microsoft.EntityFrameworkCore;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public interface IRideRepository : IBaseRepository<Ride>
    {
        Task<List<Ride>> GetAllForDriver(Guid driverId);
    }

    public class RideRepository : BaseRepository<Ride>, IRideRepository
    {
        public RideRepository(TaxiCentralContext context) : base(context) { }

        public Task<List<Ride>> GetAllForDriver(Guid driverId)
        {
            return Query.Include(x => x.Driver)
                .Where(x => x.Driver.Id == driverId)
                .ToListAsync();
        }
    }
}
