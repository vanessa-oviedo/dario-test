using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Configurations
{
    //public sealed class CaseStatusHistoryConfig : IEntityTypeConfiguration<CaseStatusHistory>
    //{
    //    public void Configure(EntityTypeBuilder<CaseStatusHistory> b)
    //    {
    //        b.ToTable("CaseStatusHistory");
    //        b.HasKey(x => x.Id);
    //        b.Property(x => x.ChangedBy).HasMaxLength(200).IsRequired();
    //        b.Property(x => x.CreatedAtUtc).HasPrecision(0);

    //        // Unique current status per case
    //        b.HasIndex(x => new { x.CaseId, x.IsCurrent })
    //            .IsUnique()
    //            .HasFilter("[IsCurrent] = 1");

    //        // Check (en SQL) PickedUp -> StatusDateUtc NOT NULL (se implementa vía trigger/check en migración si querés)
    //        // Aquí solo dejamos comentario, EF no crea CHECK por condiciones sobre valores de columna enum fácilmente.

    //        b.HasOne<SellCase>().WithMany(sc => sc.StatusHistory).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
    //    }
    //}
}
