using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class SubModelConfig : IEntityTypeConfiguration<SubModel>
    {
        public void Configure(EntityTypeBuilder<SubModel> b)
        {
            b.ToTable("SubModel");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => new { x.ModelId, x.Name }).IsUnique();
        }
    }
}
