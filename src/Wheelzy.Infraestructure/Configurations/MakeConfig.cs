using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class MakeConfig : IEntityTypeConfiguration<Make>
    {
        public void Configure(EntityTypeBuilder<Make> b)
        {
            b.ToTable("Make");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        }
    }
}
