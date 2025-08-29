using Microsoft.EntityFrameworkCore;
using Wheelzy.Domain;

namespace Wheelzy.Infrastructure;

public static class DbInitializer
{
    public static async Task EnsureCreatedAndSeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);

        if (!await db.Statuses.AnyAsync(ct))
        {
            db.Statuses.AddRange(
                new Status { Name = "Pending Acceptance", Code = "PENDING" },
                new Status { Name = "Accepted", Code = "ACCEPTED" },
                new Status { Name = "Picked Up", Code = "PICKED_UP" }
            );
        }

        if (!await db.Makes.AnyAsync(ct))
        {
            var ford = new Make { Name = "Ford" };
            var toyota = new Make { Name = "Toyota" };

            var fiesta = new Model { Name = "Fiesta", Make = ford };
            var focus = new Model { Name = "Focus", Make = ford };
            var corolla = new Model { Name = "Corolla", Make = toyota };

            var st = new Submodel { Name = "ST", Model = focus };
            var se = new Submodel { Name = "SE", Model = corolla };

            db.Makes.AddRange(ford, toyota);
            db.Models.AddRange(fiesta, focus, corolla);
            db.Submodels.AddRange(st, se);
        }

        if (!await db.ZipCodes.AnyAsync(ct))
        {
            db.ZipCodes.AddRange(
                new ZipCode { ZipCodeId = "33101" },
                new ZipCode { ZipCodeId = "90001" }
            );
        }

        if (!await db.Customers.AnyAsync(ct))
        {
            db.Customers.AddRange(
                new Customer { FullName = "Alice Johnson", Email = "alice@example.com" },
                new Customer { FullName = "Bob Smith", Email = "bob@example.com" }
            );
        }

        if (!await db.Buyers.AnyAsync(ct))
        {
            var b1 = new Buyer { Name = "Buyer A" };
            var b2 = new Buyer { Name = "Buyer B" };
            db.Buyers.AddRange(b1, b2);
        }

        await db.SaveChangesAsync(ct);

        if (!await db.BuyerZipQuotes.AnyAsync(ct))
        {
            var buyers = await db.Buyers.AsNoTracking().ToListAsync(ct);
            var zips = await db.ZipCodes.AsNoTracking().ToListAsync(ct);
            var rnd = new Random(42);
            foreach (var b in buyers)
            {
                foreach (var z in zips)
                {
                    db.BuyerZipQuotes.Add(new BuyerZipQuote
                    {
                        BuyerId = b.BuyerId,
                        ZipCodeId = z.ZipCodeId,
                        Amount = rnd.Next(500, 1500)
                    });
                }
            }
        }

        await db.SaveChangesAsync(ct);

        if (!await db.CarCases.AnyAsync(ct))
        {
            var alice = await db.Customers.FirstAsync(ct);
            var ford = await db.Makes.FirstAsync(ct);
            var focus = await db.Models.FirstAsync(x => x.Name == "Focus", ct);
            var st = await db.Submodels.FirstAsync(x => x.Name == "ST", ct);
            var zip = await db.ZipCodes.FirstAsync(ct);

            var case1 = new CarCase
            {
                CustomerId = alice.CustomerId,
                Year = 2018,
                MakeId = ford.MakeId,
                ModelId = focus.ModelId,
                SubmodelId = st.SubmodelId,
                ZipCodeId = zip.ZipCodeId
            };
            db.CarCases.Add(case1);
            await db.SaveChangesAsync(ct);

            await GenerateQuotesForCaseAsync(db, case1.CaseId, ct);
            await SetInitialStatusAsync(db, case1.CaseId, "PENDING", null, "system", ct);
        }

        await db.SaveChangesAsync(ct);
    }

    public static async Task GenerateQuotesForCaseAsync(AppDbContext db, int caseId, CancellationToken ct)
    {
        var cc = await db.CarCases.Include(x => x.Quotes).FirstAsync(x => x.CaseId == caseId, ct);
        var buyersForZip =
            await db.BuyerZipQuotes.AsNoTracking().Where(x => x.ZipCodeId == cc.ZipCodeId).ToListAsync(ct);

        foreach (var b in buyersForZip)
        {
            db.CaseQuotes.Add(new CaseQuote
            {
                CaseId = cc.CaseId,
                BuyerId = b.BuyerId,
                Amount = b.Amount,
                IsCurrent = false
            });
        }
        await db.SaveChangesAsync(ct);

        var best = await db.CaseQuotes.Where(q => q.CaseId == caseId).OrderByDescending(q => q.Amount).FirstAsync(ct);
        await SetCurrentQuoteAsync(db, caseId, best.CaseQuoteId, ct);
    }

    public static async Task SetCurrentQuoteAsync(AppDbContext db, int caseId, long caseQuoteId, CancellationToken ct)
    {
        var quotes = await db.CaseQuotes.Where(q => q.CaseId == caseId).ToListAsync(ct);
        foreach (var q in quotes) q.IsCurrent = q.CaseQuoteId == caseQuoteId;
        await db.SaveChangesAsync(ct);
    }

    public static async Task SetInitialStatusAsync(AppDbContext db, int caseId, string code, DateTime? statusDate, string changedBy, CancellationToken ct)
    {
        await ChangeStatusAsync(db, caseId, code, statusDate, changedBy, ct);
    }

    public static async Task ChangeStatusAsync(AppDbContext db, int caseId, string code, DateTime? statusDate, string changedBy, CancellationToken ct)
    {
        var status = await db.Statuses.FirstAsync(s => s.Code == code, ct);
        if (status.Code == "PICKED_UP" && statusDate is null)
            throw new InvalidOperationException("StatusDate es obligatorio para PICKED_UP");

        var current = await db.CaseStatuses.Where(x => x.CaseId == caseId && x.IsCurrent).ToListAsync(ct);
        foreach (var cs in current) cs.IsCurrent = false;

        db.CaseStatuses.Add(new CaseStatus
        {
            CaseId = caseId,
            StatusId = status.StatusId,
            StatusDate = statusDate,
            ChangedBy = changedBy,
            IsCurrent = true,
            CreatedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
    }
}
