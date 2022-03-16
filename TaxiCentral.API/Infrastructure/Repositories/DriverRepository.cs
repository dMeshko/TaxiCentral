using Microsoft.EntityFrameworkCore;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public interface IDriverRepository : IBaseRepository<Driver>
    {
        Task<bool> AlreadyExists(Driver driver);
        Task<Driver?> GetDriverByPin(string? pin);
        Task<List<Driver>> GetDriversInArea(LatLng location, int radius);
    }

    public class DriverRepository : BaseRepository<Driver>, IDriverRepository
    {
        public DriverRepository(TaxiCentralContext context) : base(context) { }

        public Task<bool> AlreadyExists(Driver driver)
        {
            return driver.Id != Guid.Empty
                ? Query.AnyAsync(x => x.Id != driver.Id && x.Pin == driver.Pin)
                : Query.AnyAsync(x => x.Pin == driver.Pin);
        }

        public Task<Driver?> GetDriverByPin(string? pin)
        {
            return Query.FirstOrDefaultAsync(x => x.Pin == pin);
        }

        public Task<List<Driver>> GetDriversInArea(LatLng location, int radius)
        {
            //todo: rework this pls!!
            return GetAll().ToListAsync();
        }
    }
}
