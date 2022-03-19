using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace TaxiCentral.API.Infrastructure
{
    public class TaxiCentralContext : DbContext
    {
        public TaxiCentralContext(DbContextOptions<TaxiCentralContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
