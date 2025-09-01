using AutoMapper;
using Wheelzy.Application.DTOs.Catalog;
using Wheelzy.Application.Models;

namespace Wheelzy.API.Mapping.Profiles;

public sealed class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<CarMake, MakeDto>();
        CreateMap<CarModel, ModelDto>();
        CreateMap<CarSubmodel, SubModelDto>();
    }
}
