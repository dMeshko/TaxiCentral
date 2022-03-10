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

            builder.OwnsOne(x => x.StartingPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("StartingLatitude").IsRequired();
                y.Property(w => w.Longitude).HasColumnName("StartingLongitude").IsRequired();
            });

            builder.OwnsOne(x => x.DestinationPoint, y =>
            {
                y.Property(w => w.Latitude).HasColumnName("DestinationLatitude");
                y.Property(w => w.Longitude).HasColumnName("DestinationLongitude");
            });

            builder.Property(x => x.StartTime)
                .IsRequired();

            builder.Property(x => x.DestinationTime);

            builder.Property(x => x.Comment)
                .HasColumnType("nvarchar(MAX)");

            builder.Property(x => x.Mileage).IsRequired(false);

            builder.Property(x => x.Cost).IsRequired(false);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
