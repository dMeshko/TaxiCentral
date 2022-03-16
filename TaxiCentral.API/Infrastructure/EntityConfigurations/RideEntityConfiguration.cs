using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.EntityConfigurations
{
    public class RideEntityConfiguration : IEntityTypeConfiguration<Ride>
    {
        public void Configure(EntityTypeBuilder<Ride> builder)
        {
            builder.ToTable("Rides");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Driver)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

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

            builder.OwnsOne(x => x.ActualStartingPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("ActualStartingLatitude").IsRequired();
                y.Property(w => w.Longitude).HasColumnName("ActualStartingLongitude").IsRequired();
            });

            builder.OwnsOne(x => x.ActualDestinationPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("ActualDestinationLatitude");
                y.Property(w => w.Longitude).HasColumnName("ActualDestinationLongitude");
            });

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.DestinationTime);

            builder.Property(x => x.Comment)
                .HasColumnType("nvarchar(MAX)");

            builder.Property(x => x.Mileage).IsRequired(false);

            builder.Property(x => x.Cost).IsRequired(false);

            builder.Property(x => x.EstimatedTimeOfArrival).IsRequired(false);

            builder.Property(x => x.TimeOfArrival).IsRequired(false);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
