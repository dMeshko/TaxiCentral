using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaxiCentral.API.Models;

namespace TaxiCentral.API.Infrastructure.EntityConfigurations
{
    public class DriverEntityConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Pin)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasData(new Driver("Darko", "Meshkovski", "1234") { Id = Guid.NewGuid() });
        }
    }
}
