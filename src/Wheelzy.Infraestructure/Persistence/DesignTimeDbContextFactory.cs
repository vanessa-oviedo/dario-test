using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Wheelzy.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WheetzyDbContext>
    {
        public WheetzyDbContext CreateDbContext(string[] args)
        {
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs = cfg.GetConnectionString("Default")
                     ?? "Server=(localdb)\\MSSQLLocalDB;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<WheetzyDbContext>()
                .UseSqlServer(cs)
                .Options;

            return new WheetzyDbContext(options);
        }
    }
}
