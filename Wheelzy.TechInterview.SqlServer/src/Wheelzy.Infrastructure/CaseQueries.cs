using Microsoft.EntityFrameworkCore;
using Wheelzy.Domain;

namespace Wheelzy.Infrastructure;

public class CaseQueries(AppDbContext db)
{
    public Task<List<CaseSummaryDto>> GetCaseSummariesAsync(CancellationToken ct = default)
    {
        var q =
            from cc in db.CarCases.AsNoTracking()
            join mk in db.Makes on cc.MakeId equals mk.MakeId
            join md in db.Models on cc.ModelId equals md.ModelId
            from sm in db.Submodels.Where(x => x.SubmodelId == cc.SubmodelId).DefaultIfEmpty()
            from cq in db.CaseQuotes.Where(q => q.CaseId == cc.CaseId && q.IsCurrent).DefaultIfEmpty()
            from b in db.Buyers.Where(x => cq != null && x.BuyerId == cq.BuyerId).DefaultIfEmpty()
            from cs in db.CaseStatuses.Where(s => s.CaseId == cc.CaseId && s.IsCurrent).DefaultIfEmpty()
            from st in db.Statuses.Where(s => cs != null && s.StatusId == cs.StatusId).DefaultIfEmpty()
            select new CaseSummaryDto(
                cc.CaseId, cc.Year,
                mk.Name, md.Name, sm.Name, cc.ZipCodeId,
                b.Name, cq.Amount, st.Name, cs.StatusDate
            );

        return q.ToListAsync(ct);
    }
}
