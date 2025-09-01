using Microsoft.Extensions.DependencyInjection;
using Wheelzy.Application.Mapping.Profiles;

namespace Wheelzy.Application.Mapping.Registration;

public static class ApplicationMappingRegistration
{
    /// <summary>
    /// Registra los perfiles de AutoMapper de la capa Application.
    /// </summary>
    public static IServiceCollection AddApplicationMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<CatalogMappingProfile>();
            cfg.AddProfile<CaseMappingProfile>();
        });
        return services;
    }
}
