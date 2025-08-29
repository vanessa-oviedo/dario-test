using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Wheelzy.Domain;
using Wheelzy.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Swagger/OpenAPI solo en la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = builder.Configuration["Swagger:Title"] ?? "Wheelzy API",
        Version = builder.Configuration["Swagger:Version"] ?? "v1",
        Description = "API para el assessment, con Domain/Infrastructure separados."
    });
    var xml = Path.Combine(AppContext.BaseDirectory, "Wheelzy.Api.xml");
    if (File.Exists(xml)) c.IncludeXmlComments(xml);
});

builder.Services.AddScoped<CaseQueries>();

var app = builder.Build();

// DB + seed (para el ejercicio). Si usas migraciones, quita esta línea.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.EnsureCreatedAndSeedAsync(db);
}

// Swagger habilitado siempre
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

/// <summary>Lista de casos con datos agregados (Make/Model/Submodel/Zip + Current Buyer/Quote + Status/Date).</summary>
app.MapGet("/cases", async (CaseQueries queries, CancellationToken ct) =>
{
    var list = await queries.GetCaseSummariesAsync(ct);
    return Results.Ok(list);
})
.WithName("GetCases")
.Produces<List<CaseSummaryDto>>(StatusCodes.Status200OK);

/// <summary>Crea un caso y asigna estado inicial PENDING + genera quotes por ZIP.</summary>
app.MapPost("/cases", async (AppDbContext db, CreateCaseRequest req, CancellationToken ct) =>
{
    var cc = new CarCase
    {
        CustomerId = req.CustomerId, Year = req.Year, MakeId = req.MakeId,
        ModelId = req.ModelId, SubmodelId = req.SubmodelId, ZipCodeId = req.ZipCodeId
    };
    db.CarCases.Add(cc);
    await db.SaveChangesAsync(ct);

    await DbInitializer.GenerateQuotesForCaseAsync(db, cc.CaseId, ct);
    await DbInitializer.SetInitialStatusAsync(db, cc.CaseId, "PENDING", null, "api", ct);

    return Results.Created($"/cases/{cc.CaseId}", new { cc.CaseId });
})
.WithName("CreateCase")
.Produces(StatusCodes.Status201Created);

/// <summary>Agrega una quote y recalcula la 'current' (ej: mayor monto).</summary>
app.MapPost("/cases/{id:int}/quotes", async (int id, AppDbContext db, AddQuoteRequest req, CancellationToken ct) =>
{
    var exists = await db.CarCases.AnyAsync(x => x.CaseId == id, ct);
    if (!exists) return Results.NotFound();

    db.CaseQuotes.Add(new CaseQuote { CaseId = id, BuyerId = req.BuyerId, Amount = req.Amount, IsCurrent = false });
    await db.SaveChangesAsync(ct);

    var best = await db.CaseQuotes.Where(q => q.CaseId == id).OrderByDescending(q => q.Amount).FirstAsync(ct);
    await DbInitializer.SetCurrentQuoteAsync(db, id, best.CaseQuoteId, ct);

    return Results.NoContent();
})
.WithName("AddQuote")
.Produces(StatusCodes.Status204NoContent);

/// <summary>Cambia el estado del caso. Si es PICKED_UP, StatusDate es obligatorio.</summary>
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
