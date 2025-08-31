using AutoMapper;
using Wheelzy.Application.DTOs.Buyer;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Mapping.Profiles;

public sealed class BuyerMappingProfile : Profile
{
    public BuyerMappingProfile()
    {
        CreateMap<Buyer, BuyerDto>();
    }
}
