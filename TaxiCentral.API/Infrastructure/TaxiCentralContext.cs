using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure
{
    public class TaxiCentralContext : DbContext
    {
        public TaxiCentralContext(DbContextOptions<TaxiCentralContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //delete, useless
            modelBuilder.Entity<Driver>()
                .HasData(new Driver("Darko", "Meshkovski", "1234"));

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
