using Microsoft.EntityFrameworkCore;
using TaxiCentral.API.Models;
using Route = TaxiCentral.API.Models.Route;

namespace TaxiCentral.API.Infrastructure.Repositories
{
    public interface IRouteRepository : IBaseRepository<Route>
    {
        Task<List<Route>> GetAllPendingRoutes();
        IQueryable<Route> GetAllVoidedRoutes();
    }

    public class RouteRepository : BaseRepository<Route>, IRouteRepository
    {
        public RouteRepository(TaxiCentralContext context) : base(context)
        {

        }

        public Task<List<Route>> GetAllPendingRoutes()
        {
            return Query.Where(x => x.Status == RouteStatus.Pending 
                                    && x.Ride == null)
                .OrderBy(x => x.ReportedAt)
                .ToListAsync();
        }

        public IQueryable<Route> GetAllVoidedRoutes()
        {
            return Query.Where(x => x.Status == RouteStatus.Voided);
        }
    }
}
