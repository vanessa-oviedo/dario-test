using Microsoft.EntityFrameworkCore;
using Wheelzy.Domain;

namespace Wheelzy.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Make> Makes => Set<Make>();
    public DbSet<Model> Models => Set<Model>();
    public DbSet<Submodel> Submodels => Set<Submodel>();
    public DbSet<ZipCode> ZipCodes => Set<ZipCode>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<BuyerZipQuote> BuyerZipQuotes => Set<BuyerZipQuote>();
    public DbSet<CarCase> CarCases => Set<CarCase>();
    public DbSet<CaseQuote> CaseQuotes => Set<CaseQuote>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<CaseStatus> CaseStatuses => Set<CaseStatus>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Make>(e =>
        {
            e.HasKey(x => x.MakeId);
            e.HasIndex(x => x.Name).IsUnique();
        });
        b.Entity<Model>(e =>
        {
            e.HasKey(x => x.ModelId);
            e.HasOne(x => x.Make).WithMany(x => x.Models).HasForeignKey(x => x.MakeId);
            e.HasIndex(x => new { x.MakeId, x.Name }).IsUnique();
        });
        b.Entity<Submodel>(e =>
        {
            e.HasKey(x => x.SubmodelId);
            e.HasOne(x => x.Model).WithMany(x => x.Submodels).HasForeignKey(x => x.ModelId);
            e.HasIndex(x => new { x.ModelId, x.Name }).IsUnique();
        });

        b.Entity<ZipCode>(e => e.HasKey(x => x.ZipCodeId));

        b.Entity<Customer>(e =>
        {
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.FullName).IsRequired();
            e.HasIndex(x => x.FullName);
        });

        b.Entity<Buyer>(e =>
        {
            e.HasKey(x => x.BuyerId);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<BuyerZipQuote>(e =>
        {
            e.HasKey(x => new { x.BuyerId, x.ZipCodeId });
            e.Property(x => x.Amount).HasColumnType("decimal(12,2)");
            e.HasOne(x => x.Buyer).WithMany(x => x.BuyerZipQuotes).HasForeignKey(x => x.BuyerId);
        });

        b.Entity<CarCase>(e =>
        {
            e.HasKey(x => x.CaseId);
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            e.HasOne(x => x.Make).WithMany().HasForeignKey(x => x.MakeId);
            e.HasOne(x => x.Model).WithMany().HasForeignKey(x => x.ModelId);
            e.HasOne(x => x.Submodel).WithMany().HasForeignKey(x => x.SubmodelId);
            e.HasOne(x => x.ZipCode).WithMany().HasForeignKey(x => x.ZipCodeId);
            e.HasIndex(x => x.ZipCodeId);
        });

        b.Entity<CaseQuote>(e =>
        {
            e.HasKey(x => x.CaseQuoteId);
            e.Property(x => x.Amount).HasColumnType("decimal(12,2)");
            e.HasOne(x => x.Case).WithMany(x => x.Quotes).HasForeignKey(x => x.CaseId);
            e.HasOne(x => x.Buyer).WithMany().HasForeignKey(x => x.BuyerId);
            e.HasIndex(x => new { x.CaseId, x.IsCurrent }).HasFilter("IsCurrent = 1").IsUnique();
        });

        b.Entity<Status>(e =>
        {
            e.HasKey(x => x.StatusId);
            e.HasIndex(x => x.Code).IsUnique();
        });

        b.Entity<CaseStatus>(e =>
        {
            e.HasKey(x => x.CaseStatusId);
            e.HasOne(x => x.Case).WithMany(x => x.Statuses).HasForeignKey(x => x.CaseId);
            e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId);
            e.HasIndex(x => new { x.CaseId, x.IsCurrent }).HasFilter("IsCurrent = 1").IsUnique();
        });
    }
}
