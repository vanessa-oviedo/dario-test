using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class SellCaseConfig : IEntityTypeConfiguration<SellCase>
    {
        public void Configure(EntityTypeBuilder<SellCase> b)
        {
            b.ToTable("SellCase");
            b.HasKey(x => x.Id);
            b.Property(x => x.ZipCode).HasMaxLength(10).IsRequired();
            b.Property(x => x.CreatedAtUtc).HasPrecision(0);
            b.HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
            b.Navigation(x => x.Quotes).AutoInclude(false);
            b.Navigation(x => x.StatusHistory).AutoInclude(false);
        }
    }
}
