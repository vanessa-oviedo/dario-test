namespace Wheelzy.Api.Requests
{
    //TODO: Add Data Annotations
    public record CreateCaseRequest(
        int CustomerId, short Year, int MakeId, int ModelId, int? SubmodelId, string ZipCodeId
);
}
