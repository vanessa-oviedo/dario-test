using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Models;

namespace Wheelzy.Infrastructure.Persistence
{
    public class WheetzyDbContext : DbContext
    {
        public WheetzyDbContext(DbContextOptions<WheetzyDbContext> options) : base(options) { }

        public DbSet<Buyer> Buyers => Set<Buyer>();
        public DbSet<BuyerZipCoverage> BuyerZipCoverages => Set<BuyerZipCoverage>();
        public DbSet<CarMake> CarMakes => Set<CarMake>();
        public DbSet<CarModel> CarModels => Set<CarModel>();
        public DbSet<CarSubmodel> CarSubmodels => Set<CarSubmodel>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<CustomerCar> CustomerCars => Set<CustomerCar>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderBuyerQuote> OrderBuyerQuotes => Set<OrderBuyerQuote>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<ZipCode> ZipCodes => Set<ZipCode>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WheetzyDbContext).Assembly);
        }
    }
}
