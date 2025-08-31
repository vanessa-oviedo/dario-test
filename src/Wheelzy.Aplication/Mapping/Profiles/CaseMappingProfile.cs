using AutoMapper;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Mapping.Profiles;

public sealed class CaseMappingProfile : Profile
{
    public CaseMappingProfile()
    {
        // Case -> summary/detail are usually custom projections; keep simple maps for inner items
        CreateMap<CaseQuote, CaseQuoteDto>()
            .ForMember(d => d.BuyerName, opt => opt.Ignore()); // se completa en infra si hace falta

        CreateMap<CaseStatusHistory, CaseStatusHistoryDto>()
            .ForMember(d => d.StatusName, opt => opt.Ignore());
    }
}
