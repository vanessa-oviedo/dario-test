using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Services;
using Wheelzy.Infrastructure.Persistence;
using Wheelzy.Infrastructure.Repositories;

namespace Wheelzy.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "Default")
        {
            var cs = configuration.GetConnectionString(connectionStringName)
                     ?? "Server=(localdb)\\MSSQLLocalDB;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True";

            services.AddDbContext<WheetzyDbContext>(opt =>
                opt.UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(WheetzyDbContext).Assembly.FullName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IClock, SystemClock>();

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IQuoteService, QuoteService>();
            services.AddScoped<IStatusService, StatusService>();
                        
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
            services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBuyerZipCoverageRepository, BuyerZipCoverageRepository>();
            services.AddScoped<IOrderBuyerQuoteRepository, OrderBuyerQuoteRepository>();
            
            return services;
        }
    }
}
