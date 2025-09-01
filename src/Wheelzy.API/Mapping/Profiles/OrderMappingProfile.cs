using AutoMapper;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Requests;

namespace Wheelzy.API.Mapping.Profiles;

public sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Requests.CreateQuoteRequest, CreateQuoteRequest>()
           .ConstructUsing(src => new CreateQuoteRequest(
               src.OrderId ?? 0,       
               src.BuyerId ?? 0,
               src.ZipCode ?? string.Empty, 
               src.Amount,     
               null,                  
               DateTime.UtcNow     
           ));

        CreateMap<Requests.SearchOrdersRequest, SearchOrdersRequest>()
            .ConstructUsing(src => new SearchOrdersRequest(
                src.CreatedFromUtc,
                src.CreatedToUtc,
                src.CustomerIds,
                src.BuyerIds,
                src.Statuses,
                src.ZipCode
            ))
            .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page ?? new PageRequest()));
    }
}
