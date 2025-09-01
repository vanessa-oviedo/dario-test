using Wheelzy.API.Mapping.Profiles;

namespace Wheelzy.Application.Mapping.Registration
{
    public static class ApplicationMappingRegistration
    {
        public static IServiceCollection AddApplicationMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<OrderMappingProfile>();
            });
            return services;
        }
    }
}
