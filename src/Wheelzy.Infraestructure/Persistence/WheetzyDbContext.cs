using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Persistence
{
    public class WheetzyDbContext : DbContext
    {
        public WheetzyDbContext(DbContextOptions<WheetzyDbContext> options) : base(options) { }

        public DbSet<Make> Makes => Set<Make>();
        public DbSet<Model> Models => Set<Model>();
        public DbSet<SubModel> SubModels => Set<SubModel>();
        public DbSet<Buyer> Buyers => Set<Buyer>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<SellCase> SellCases => Set<SellCase>();
        public DbSet<CaseQuote> CaseQuotes => Set<CaseQuote>();
        public DbSet<CaseStatusHistory> CaseStatusHistories => Set<CaseStatusHistory>();

        // Infra-only entity for base rates by ZIP
        public DbSet<BuyerZipRate> BuyerZipRates => Set<BuyerZipRate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WheetzyDbContext).Assembly);
        }
    }
}
