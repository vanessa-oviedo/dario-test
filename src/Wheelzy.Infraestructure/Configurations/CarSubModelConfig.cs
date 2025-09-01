using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations;

public class CarSubmodelConfiguration : IEntityTypeConfiguration<CarSubmodel>
{
    public void Configure(EntityTypeBuilder<CarSubmodel> b)
    {
        b.ToTable("CarSubmodel");
        b.HasKey(x => x.SubmodelId);
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();

        b.HasOne(x => x.Model)
            .WithMany(m => m.Submodels)
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.ModelId, x.Name }).IsUnique().HasDatabaseName("UQ_CarSubmodel");
        b.HasIndex(x => x.ModelId).HasDatabaseName("IX_CarSubmodel_ModelId");
    }
}