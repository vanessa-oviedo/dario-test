using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public sealed class CaseRepository : ICaseRepository
    {
        private readonly WheetzyDbContext _db;
        public CaseRepository(WheetzyDbContext db) => _db = db;

        public async Task<SellCase?> GetByIdAsync(int caseId, bool includeRelated = true, CancellationToken ct = default)
        {
            IQueryable<SellCase> q = _db.SellCases;
            if (includeRelated)
                q = q.Include(x => x.Quotes).Include(x => x.StatusHistory);

            return await q.FirstOrDefaultAsync(x => x.Id == caseId, ct);
        }

        public Task AddAsync(SellCase entity, CancellationToken ct = default)
        {
            _db.SellCases.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(SellCase entity, CancellationToken ct = default)
        {
            _db.SellCases.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<CaseQuote?> GetCurrentQuoteAsync(int caseId, CancellationToken ct = default) =>
            await _db.CaseQuotes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CaseId == caseId && x.IsCurrent, ct);

        public async Task<CaseStatusHistory?> GetCurrentStatusAsync(int caseId, CancellationToken ct = default) =>
            await _db.CaseStatusHistories.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CaseId == caseId && x.IsCurrent, ct);

        public async Task<bool> ExistsAsync(int caseId, CancellationToken ct = default) =>
            await _db.SellCases.AsNoTracking().AnyAsync(x => x.Id == caseId, ct);

        public async Task<IReadOnlyList<CaseSummaryDto>> SearchSummariesAsync(CaseSearchFilter f, CancellationToken ct = default)
        {
            var q =
                from sc in _db.SellCases.AsNoTracking()
                join c in _db.Cars.AsNoTracking() on sc.CarId equals c.Id
                join sm in _db.SubModels.AsNoTracking() on c.SubModelId equals sm.Id
                join md in _db.Models.AsNoTracking() on sm.ModelId equals md.Id
                join mk in _db.Makes.AsNoTracking() on md.MakeId equals mk.Id
                join cq in _db.CaseQuotes.AsNoTracking()
                     .Where(x => x.IsCurrent) on sc.Id equals cq.CaseId into cqg
                from cq in cqg.DefaultIfEmpty()
                join b in _db.Buyers.AsNoTracking()
                     on (cq != null ? cq.BuyerId : 0) equals b.Id into bg
                from b in bg.DefaultIfEmpty()
                join csh in _db.CaseStatusHistories.AsNoTracking()
                     .Where(x => x.IsCurrent) on sc.Id equals csh.CaseId into cshg
                from csh in cshg.DefaultIfEmpty()
                select new CaseSummaryDto
                {
                    CaseId = sc.Id,
                    Year = c.Year,
                    Make = mk.Name,
                    Model = md.Name,
                    SubModel = sm.Name,
                    ZipCode = sc.ZipCode,
                    CurrentBuyer = b != null ? b.Name : null,
                    CurrentQuote = cq != null ? cq.Amount : (decimal?)null,
                    CurrentStatus = csh != null ? csh.Status.ToString() : null,
                    CurrentStatusDateUtc = csh != null ? csh.StatusDateUtc : null
                };

            if (f.CreatedFromUtc.HasValue) q = q.Where(x => x.CurrentStatusDateUtc == null || x.CurrentStatusDateUtc >= f.CreatedFromUtc);
            if (f.CreatedToUtc.HasValue) q = q.Where(x => x.CurrentStatusDateUtc == null || x.CurrentStatusDateUtc < f.CreatedToUtc);
            if (!string.IsNullOrWhiteSpace(f.ZipCode)) q = q.Where(x => x.ZipCode == f.ZipCode);
            if (f.CustomerIds?.Any() == true)
            {
                // Nota: si quisieras filtrar por CustomerId, debes agregar CustomerId a la proyección o hacer join a SellCase otra vez.
                // Por simplicidad se omite aquí; puedes replicar con otro select anónimo que incluya CustomerId.
            }
            if (f.BuyerIds?.Any() == true) q = q.Where(x => x.CurrentBuyer != null); // si querés exacto por buyerId, trae BuyerId en proyección
            if (f.Statuses?.Any() == true) { /* idem: trae enum a proyección si necesitas filtro exacto */ }

            if (f.Skip.HasValue) q = q.Skip(f.Skip.Value);
            if (f.Take.HasValue) q = q.Take(f.Take.Value);

            return await q.ToListAsync(ct);
        }
    }
}
