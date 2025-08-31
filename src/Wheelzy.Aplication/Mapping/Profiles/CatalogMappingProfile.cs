using AutoMapper;
using Wheelzy.Application.DTOs.Catalog;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Mapping.Profiles;

public sealed class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Make, MakeDto>();
        CreateMap<Model, ModelDto>();
        CreateMap<SubModel, SubModelDto>();
    }
}
