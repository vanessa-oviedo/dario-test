using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Wheelzy.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<WheetzyDbContext>
{
    public WheetzyDbContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<WheetzyDbContext>();
        optionsBuilder.UseSqlServer(cfg.GetConnectionString("Default"));
        return new WheetzyDbContext(optionsBuilder.Options);
    }
}
