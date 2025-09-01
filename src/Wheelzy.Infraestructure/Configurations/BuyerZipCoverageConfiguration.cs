using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    public class BuyerZipCoverageConfiguration : IEntityTypeConfiguration<BuyerZipCoverage>
    {
        public void Configure(EntityTypeBuilder<BuyerZipCoverage> b)
        {
            b.ToTable("BuyerZipCoverage");
            b.HasKey(x => x.BuyerZipCoverageId);

            b.Property(x => x.ZipCode).HasColumnType("char(5)").IsRequired();
            b.Property(x => x.DefaultQuoteAmount).HasColumnType("money");

            b.HasOne(x => x.Buyer)
                .WithMany(x => x.BuyerZipCoverages)
                .HasForeignKey(x => x.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Zip)
                .WithMany(z => z.BuyerCoverages)
                .HasForeignKey(x => x.ZipCode)
                .HasPrincipalKey(z => z.ZipCodeId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.BuyerId, x.ZipCode })
                .IsUnique()
                .HasDatabaseName("UQ_Buyer_Zip");

            b.HasIndex(x => x.ZipCode).HasDatabaseName("IX_BuyerZipCoverage_Zip");
        }
    }
}
