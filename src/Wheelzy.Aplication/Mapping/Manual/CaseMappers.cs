using Wheelzy.Application.DTOs.Case;

namespace Wheelzy.Application.Mapping.Manual;

//TODO: Comments in English
public static class CaseMappers
{
    /// <summary>
    /// Simple helper para mapear un resumen a DTO de Application desde el DTO de Interfaces/Queries.
    /// </summary>
    public static CaseSummaryDto ToSummaryDto(Interfaces.Queries.OrderCurrentSummaryDto s) => new()
    {
     
    };
}
