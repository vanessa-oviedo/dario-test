using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class CarModelConfig : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> b)
        {
            b.ToTable("CarModel");
            b.HasKey(x => x.ModelId);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();

            b.HasOne(x => x.Make)
                .WithMany(m => m.Models)
                .HasForeignKey(x => x.MakeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.MakeId, x.Name }).IsUnique().HasDatabaseName("UQ_CarModel");
            b.HasIndex(x => x.MakeId).HasDatabaseName("IX_CarModel_MakeId");
        }
    }
}
