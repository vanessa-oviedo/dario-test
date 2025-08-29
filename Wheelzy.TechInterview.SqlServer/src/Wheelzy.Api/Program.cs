using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Wheelzy.Api.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"] ?? "Wheelzy API",
        Version = builder.Configuration["Swagger:Version"] ?? "v1"
    });
    var xml = Path.Combine(AppContext.BaseDirectory, "Wheelzy.Api.xml");
    if (File.Exists(xml))
        c.IncludeXmlComments(xml);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.EnsureCreatedAndSeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/cases", async (AppDbContext db, CancellationToken ct) =>
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

    return Results.Ok(await q.ToListAsync(ct));
})
.WithName("GetCases")
.Produces<List<CaseSummaryDto>>(StatusCodes.Status200OK);

app.MapPost("/cases", async (AppDbContext db, CreateCaseRequest req, CancellationToken ct) =>
{
    var cc = new CarCase
    {
        CustomerId = req.CustomerId,
        Year = req.Year,
        MakeId = req.MakeId,
        ModelId = req.ModelId,
        SubmodelId = req.SubmodelId,
        ZipCodeId = req.ZipCodeId
    };
    db.CarCases.Add(cc);
    await db.SaveChangesAsync(ct);

    await DbInitializer.GenerateQuotesForCaseAsync(db, cc.CaseId, ct);
    await DbInitializer.SetInitialStatusAsync(db, cc.CaseId, "PENDING", null, "api", ct);

    return Results.Created($"/cases/{cc.CaseId}", new { cc.CaseId });
})
.WithName("CreateCase")
.Produces(StatusCodes.Status201Created);

app.MapPost("/cases/{id:int}/quotes", async (int id, AppDbContext db, AddQuoteRequest req, CancellationToken ct) =>
{
    var exists = await db.CarCases.AnyAsync(x => x.CaseId == id, ct);
    if (!exists) return Results.NotFound();

    db.CaseQuotes.Add(new CaseQuote
    {
        CaseId = id,
        BuyerId = req.BuyerId,
        Amount = req.Amount,
        IsCurrent = false
    });
    await db.SaveChangesAsync(ct);

    var best = await db.CaseQuotes.Where(q => q.CaseId == id).OrderByDescending(q => q.Amount).FirstAsync(ct);
    await DbInitializer.SetCurrentQuoteAsync(db, id, best.CaseQuoteId, ct);

    return Results.NoContent();
})
.WithName("AddQuote")
.Produces(StatusCodes.Status204NoContent);

app.MapPost("/cases/{id:int}/status", async (int id, AppDbContext db, ChangeStatusRequest req, CancellationToken ct) =>
{
    var exists = await db.CarCases.AnyAsync(x => x.CaseId == id, ct);
    if (!exists) return Results.NotFound();

    try
    {
        await DbInitializer.ChangeStatusAsync(db, id, req.Code, req.StatusDate, "api", ct);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    return Results.NoContent();
})
.WithName("ChangeStatus")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound);

app.Run();
public record CaseSummaryDto(
    int CaseId, short Year, string Make, string Model, string? Submodel, string Zip,
    string? CurrentBuyer, decimal? CurrentQuote, string? CurrentStatus, DateTime? StatusDate
);

public class Make { public int MakeId { get; set; } public string Name { get; set; } = null!; public ICollection<Model> Models { get; set; } = new List<Model>(); }
public class Model { public int ModelId { get; set; } public int MakeId { get; set; } public string Name { get; set; } = null!; public Make? Make { get; set; } public ICollection<Submodel> Submodels { get; set; } = new List<Submodel>(); }
public class Submodel { public int SubmodelId { get; set; } public int ModelId { get; set; } public string Name { get; set; } = null!; public Model? Model { get; set; } }
public class ZipCode { public string ZipCodeId { get; set; } = null!; }
public class Customer { public int CustomerId { get; set; } public string FullName { get; set; } = null!; public string? Email { get; set; } }
public class Buyer { public int BuyerId { get; set; } public string Name { get; set; } = null!; public ICollection<BuyerZipQuote> BuyerZipQuotes { get; set; } = new List<BuyerZipQuote>(); }
public class BuyerZipQuote { public int BuyerId { get; set; } public string ZipCodeId { get; set; } = null!; public decimal Amount { get; set; } public Buyer? Buyer { get; set; } public ZipCode? ZipCode { get; set; } }
public class CarCase { public int CaseId { get; set; } public int CustomerId { get; set; } public short Year { get; set; } public int MakeId { get; set; } public int ModelId { get; set; } public int? SubmodelId { get; set; } public string ZipCodeId { get; set; } = null!; public Customer? Customer { get; set; } public Make? Make { get; set; } public Model? Model { get; set; } public Submodel? Submodel { get; set; } public ZipCode? ZipCode { get; set; } public ICollection<CaseQuote> Quotes { get; set; } = new List<CaseQuote>(); public ICollection<CaseStatus> Statuses { get; set; } = new List<CaseStatus>(); }
public class CaseQuote { public long CaseQuoteId { get; set; } public int CaseId { get; set; } public int BuyerId { get; set; } public decimal Amount { get; set; } public bool IsCurrent { get; set; } public CarCase? Case { get; set; } public Buyer? Buyer { get; set; } }
public class Status { public int StatusId { get; set; } public string Name { get; set; } = null!; public string Code { get; set; } = null!; }
public class CaseStatus { public long CaseStatusId { get; set; } public int CaseId { get; set; } public int StatusId { get; set; } public DateTime? StatusDate { get; set; } public string? ChangedBy { get; set; } public bool IsCurrent { get; set; } public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; public CarCase? Case { get; set; } public Status? Status { get; set; } }
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Make> Makes => Set<Make>();
    public DbSet<Model> Models => Set<Model>();
    public DbSet<Submodel> Submodels => Set<Submodel>();
    public DbSet<ZipCode> ZipCodes => Set<ZipCode>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Buyer> Buyers => Set<Buyer>();
    public DbSet<BuyerZipQuote> BuyerZipQuotes => Set<BuyerZipQuote>();
    public DbSet<CarCase> CarCases => Set<CarCase>();
    public DbSet<CaseQuote> CaseQuotes => Set<CaseQuote>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<CaseStatus> CaseStatuses => Set<CaseStatus>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Make>(e => { e.HasKey(x => x.MakeId); e.HasIndex(x => x.Name).IsUnique(); });
        b.Entity<Model>(e => { e.HasKey(x => x.ModelId); e.HasOne(x => x.Make).WithMany(x => x.Models).HasForeignKey(x => x.MakeId); e.HasIndex(x => new { x.MakeId, x.Name }).IsUnique(); });
        b.Entity<Submodel>(e => { e.HasKey(x => x.SubmodelId); e.HasOne(x => x.Model).WithMany(x => x.Submodels).HasForeignKey(x => x.ModelId); e.HasIndex(x => new { x.ModelId, x.Name }).IsUnique(); });
        b.Entity<ZipCode>(e => e.HasKey(x => x.ZipCodeId));
        b.Entity<Customer>(e => { e.HasKey(x => x.CustomerId); e.Property(x => x.FullName).IsRequired(); e.HasIndex(x => x.FullName); });
        b.Entity<Buyer>(e => { e.HasKey(x => x.BuyerId); e.HasIndex(x => x.Name).IsUnique(); });
        b.Entity<BuyerZipQuote>(e => { e.HasKey(x => new { x.BuyerId, x.ZipCodeId }); e.Property(x => x.Amount).HasColumnType("decimal(12,2)"); e.HasOne(x => x.Buyer).WithMany(x => x.BuyerZipQuotes).HasForeignKey(x => x.BuyerId); });
        b.Entity<CarCase>(e => { e.HasKey(x => x.CaseId); e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId); e.HasOne(x => x.Make).WithMany().HasForeignKey(x => x.MakeId); e.HasOne(x => x.Model).WithMany().HasForeignKey(x => x.ModelId); e.HasOne(x => x.Submodel).WithMany().HasForeignKey(x => x.SubmodelId); e.HasOne(x => x.ZipCode).WithMany().HasForeignKey(x => x.ZipCodeId); e.HasIndex(x => x.ZipCodeId); });
        b.Entity<CaseQuote>(e => { e.HasKey(x => x.CaseQuoteId); e.Property(x => x.Amount).HasColumnType("decimal(12,2)"); e.HasOne(x => x.Case).WithMany(x => x.Quotes).HasForeignKey(x => x.CaseId); e.HasOne(x => x.Buyer).WithMany().HasForeignKey(x => x.BuyerId); e.HasIndex(x => new { x.CaseId, x.IsCurrent }).HasFilter("IsCurrent = 1").IsUnique(); });
        b.Entity<Status>(e => { e.HasKey(x => x.StatusId); e.HasIndex(x => x.Code).IsUnique(); });
        b.Entity<CaseStatus>(e => { e.HasKey(x => x.CaseStatusId); e.HasOne(x => x.Case).WithMany(x => x.Statuses).HasForeignKey(x => x.CaseId); e.HasOne(x => x.Status).WithMany().HasForeignKey(x => x.StatusId); e.HasIndex(x => new { x.CaseId, x.IsCurrent }).HasFilter("IsCurrent = 1").IsUnique(); });
    }
}
public static class DbInitializer
{
    public static async Task EnsureCreatedAndSeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if (!await db.Statuses.AnyAsync(ct)) db.Statuses.AddRange(new Status { Name="Pending Acceptance", Code="PENDING"}, new Status {Name="Accepted", Code="ACCEPTED"}, new Status {Name="Picked Up", Code="PICKED_UP"});
        if (!await db.Makes.AnyAsync(ct)) { var ford=new Make {Name="Ford"}; var toyota=new Make {Name="Toyota"}; db.Makes.AddRange(ford,toyota); db.Models.AddRange(new Model {Name="Fiesta", Make=ford}, new Model {Name="Focus", Make=ford}, new Model {Name="Corolla", Make=toyota}); db.Submodels.AddRange(new Submodel {Name="ST", Model=db.Models.Local.First(m=>m.Name=="Focus")}, new Submodel {Name="SE", Model=db.Models.Local.First(m=>m.Name=="Corolla")}); }
        if (!await db.ZipCodes.AnyAsync(ct)) db.ZipCodes.AddRange(new ZipCode {ZipCodeId="33101"}, new ZipCode {ZipCodeId="90001"});
        if (!await db.Customers.AnyAsync(ct)) db.Customers.AddRange(new Customer {FullName="Alice Johnson", Email="alice@example.com"}, new Customer {FullName="Bob Smith", Email="bob@example.com"});
        if (!await db.Buyers.AnyAsync(ct)) db.Buyers.AddRange(new Buyer {Name="Buyer A"}, new Buyer {Name="Buyer B"});
        await db.SaveChangesAsync(ct);
        if (!await db.BuyerZipQuotes.AnyAsync(ct)) { var buyers = await db.Buyers.AsNoTracking().ToListAsync(ct); var zips = await db.ZipCodes.AsNoTracking().ToListAsync(ct); var rnd=new Random(42); foreach(var b in buyers) foreach(var z in zips) db.BuyerZipQuotes.Add(new BuyerZipQuote { BuyerId=b.BuyerId, ZipCodeId=z.ZipCodeId, Amount=rnd.Next(500,1500)}); }
        await db.SaveChangesAsync(ct);
        if (!await db.CarCases.AnyAsync(ct)) { var alice=await db.Customers.FirstAsync(ct); var ford=await db.Makes.FirstAsync(ct); var focus=await db.Models.FirstAsync(x=>x.Name=="Focus", ct); var st=await db.Submodels.FirstAsync(x=>x.Name=="ST", ct); var zip=await db.ZipCodes.FirstAsync(ct);
            var case1=new CarCase { CustomerId=alice.CustomerId, Year=2018, MakeId=ford.MakeId, ModelId=focus.ModelId, SubmodelId=st.SubmodelId, ZipCodeId=zip.ZipCodeId };
            db.CarCases.Add(case1); await db.SaveChangesAsync(ct);
            await GenerateQuotesForCaseAsync(db, case1.CaseId, ct);
            await SetInitialStatusAsync(db, case1.CaseId, "PENDING", null, "system", ct);
        }
        await db.SaveChangesAsync(ct);
    }
    public static async Task GenerateQuotesForCaseAsync(AppDbContext db, int caseId, CancellationToken ct) { var cc=await db.CarCases.Include(x=>x.Quotes).FirstAsync(x=>x.CaseId==caseId, ct); var buyers=await db.BuyerZipQuotes.AsNoTracking().Where(x=>x.ZipCodeId==cc.ZipCodeId).ToListAsync(ct); foreach(var b in buyers) db.CaseQuotes.Add(new CaseQuote { CaseId=cc.CaseId, BuyerId=b.BuyerId, Amount=b.Amount, IsCurrent=false }); await db.SaveChangesAsync(ct); var best=await db.CaseQuotes.Where(q=>q.CaseId==caseId).OrderByDescending(q=>q.Amount).FirstAsync(ct); await SetCurrentQuoteAsync(db, caseId, best.CaseQuoteId, ct); }
    public static async Task SetCurrentQuoteAsync(AppDbContext db, int caseId, long caseQuoteId, CancellationToken ct) { var quotes=await db.CaseQuotes.Where(q=>q.CaseId==caseId).ToListAsync(ct); foreach(var q in quotes) q.IsCurrent = q.CaseQuoteId==caseQuoteId; await db.SaveChangesAsync(ct); }
    public static async Task SetInitialStatusAsync(AppDbContext db, int caseId, string code, DateTime? statusDate, string changedBy, CancellationToken ct) => await ChangeStatusAsync(db, caseId, code, statusDate, changedBy, ct);
    public static async Task ChangeStatusAsync(AppDbContext db, int caseId, string code, DateTime? statusDate, string changedBy, CancellationToken ct) { var status=await db.Statuses.FirstAsync(s=>s.Code==code, ct); if (status.Code=="PICKED_UP" && statusDate is null) throw new InvalidOperationException("StatusDate es obligatorio para PICKED_UP"); var current=await db.CaseStatuses.Where(x=>x.CaseId==caseId && x.IsCurrent).ToListAsync(ct); foreach(var cs in current) cs.IsCurrent=false; db.CaseStatuses.Add(new CaseStatus { CaseId=caseId, StatusId=status.StatusId, StatusDate=statusDate, ChangedBy=changedBy, IsCurrent=true, CreatedAtUtc=DateTime.UtcNow }); await db.SaveChangesAsync(ct); }
}
