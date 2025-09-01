using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class CarConfig : IEntityTypeConfiguration<Car>
    {
        public void Configure(EntityTypeBuilder<Car> b)
        {
            b.ToTable("Car");
            b.HasKey(x => x.CarId);

            b.Property(x => x.Year);

            b.HasOne(x => x.Submodel)
                .WithMany(s => s.Cars)
                .HasForeignKey(x => x.SubmodelId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.SubmodelId).HasDatabaseName("IX_Car_SubmodelId");

            // CHECK (Year BETWEEN 1900 AND 2100)
            b.ToTable(t => t.HasCheckConstraint("CK_Car_YearRange", "[Year] >= 1900 AND [Year] <= 2100"));
        }
    }
}
