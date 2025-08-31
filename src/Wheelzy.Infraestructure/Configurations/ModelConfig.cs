using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class ModelConfig : IEntityTypeConfiguration<Model>
    {
        public void Configure(EntityTypeBuilder<Model> b)
        {
            b.ToTable("Model");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => new { x.MakeId, x.Name }).IsUnique();
        }
    }
}
