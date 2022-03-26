using Microsoft.EntityFrameworkCore;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public interface IRideRepository : IBaseRepository<Ride>
    {
        Task<List<Ride>> GetAllForDriver(Guid driverId);
        Task<List<Ride>> GetAllCurrentForDriver(Guid driverId);
    }

    public class RideRepository : BaseRepository<Ride>, IRideRepository
    {
        public RideRepository(TaxiCentralContext context) : base(context) { }

        //todo: reword
        public Task<List<Ride>> GetAllForDriver(Guid driverId)
        {
            return Query.Where(x => true).ToListAsync();
            //return Query.Include(x => x.Driver)
            //    .Where(x => x.Driver.Id == driverId)
            //    .ToListAsync();
        }

        //todo: reword
        public Task<List<Ride>> GetAllCurrentForDriver(Guid driverId)
        {
            return Query
                .Where(x => x.Status != RideStatus.Complete)
                .OrderBy(x => x.Status)
                //.ThenBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
