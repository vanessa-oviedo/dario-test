using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Configurations
{
    public sealed class BuyerZipRateConfig : IEntityTypeConfiguration<BuyerZipRate>
    {
        public void Configure(EntityTypeBuilder<BuyerZipRate> b)
        {
            b.ToTable("BuyerZipRate");
            b.HasKey(x => x.Id);
            b.Property(x => x.ZipCode).HasMaxLength(10).IsRequired();
            b.Property(x => x.Amount).HasColumnType("decimal(12,2)").IsRequired();
            b.HasIndex(x => new { x.BuyerId, x.ZipCode }).IsUnique();
            b.HasOne<Buyer>().WithMany().HasForeignKey(x => x.BuyerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
