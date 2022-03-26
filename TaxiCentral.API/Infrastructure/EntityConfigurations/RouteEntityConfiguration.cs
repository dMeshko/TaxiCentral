using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxiCentral.API.Models;
using Route = TaxiCentral.API.Models.Route;

namespace TaxiCentral.API.Infrastructure.EntityConfigurations
{
    public class RouteEntityConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.ToTable("Routes");

            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.TargetStartingPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("TargetStartingLatitude").IsRequired();
                y.Property(w => w.Longitude).HasColumnName("TargetStartingLongitude").IsRequired();
            });

            builder.OwnsOne(x => x.TargetDestinationPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("TargetDestinationLatitude");
                y.Property(w => w.Longitude).HasColumnName("TargetDestinationLongitude");
            });

            builder.Property(x => x.ClientComment)
                .HasColumnType("nvarchar(MAX)");

            builder.Property(x => x.DriverComment)
                .HasColumnType("nvarchar(MAX)");

            builder.Property(x => x.EstimatedTimeOfArrival)
                .IsRequired(false);

            builder.Property(x => x.ReportedAt)
                .IsRequired();

            builder.Property(x => x.ArrivalTime)
                .IsRequired(false);

            builder.HasOne(x => x.Ride)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(x => x.Driver)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.VoidReason)
                .HasColumnType("nvarchar(MAX)")
                .IsRequired(false);
        }
    }
}
