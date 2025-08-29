namespace Wheelzy.Domain;

public record CreateCaseRequest(
    int CustomerId, short Year, int MakeId, int ModelId, int? SubmodelId, string ZipCodeId
);

public record AddQuoteRequest(decimal Amount, int BuyerId);

public record ChangeStatusRequest(string Code, DateTime? StatusDate);

public record CaseSummaryDto(
    int CaseId, short Year, string Make, string Model, string? Submodel, string Zip,
    string? CurrentBuyer, decimal? CurrentQuote, string? CurrentStatus, DateTime? StatusDate
);
