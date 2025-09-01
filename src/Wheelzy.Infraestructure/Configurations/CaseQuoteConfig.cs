using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    //public sealed class CaseQuoteConfig : IEntityTypeConfiguration<CaseQuote>
    //{
    //    public void Configure(EntityTypeBuilder<CaseQuote> b)
    //    {
    //        b.ToTable("CaseQuote");
    //        b.HasKey(x => x.Id);
    //        b.Property(x => x.Amount).HasColumnType("decimal(12,2)");
    //        b.Property(x => x.CreatedAtUtc).HasPrecision(0);
    //        b.HasIndex(x => new { x.CaseId, x.IsCurrent })
    //            .IsUnique()
    //            .HasFilter("[IsCurrent] = 1"); // unique current quote per case
    //        b.HasOne<SellCase>().WithMany(sc => sc.Quotes).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
    //        b.HasOne<Buyer>().WithMany().HasForeignKey(x => x.BuyerId).OnDelete(DeleteBehavior.Restrict);
    //    }
    //}
}
