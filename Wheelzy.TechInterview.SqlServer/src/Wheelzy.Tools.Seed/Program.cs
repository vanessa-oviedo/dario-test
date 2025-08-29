using Microsoft.EntityFrameworkCore;
using Wheelzy.Infrastructure;

Console.WriteLine("== Wheelzy Seeder (SQL Server) ==");

var cs = Environment.GetEnvironmentVariable("WHEELZY_SQL")
    ?? "Server=localhost,1433;Database=WheelzyDb;User Id=sa;Password=Passw0rd!ChangeMe;TrustServerCertificate=True;Encrypt=True";

var optBuilder = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs);
await using var db = new AppDbContext(optBuilder.Options);

await DbInitializer.EnsureCreatedAndSeedAsync(db);

var summaries = await new CaseQueries(db).GetCaseSummariesAsync();
foreach (var s in summaries)
{
    Console.WriteLine($"Case {s.CaseId}: {s.Year} {s.Make} {s.Model} {s.Submodel} | Zip {s.Zip} | Buyer={s.CurrentBuyer} Quote={s.CurrentQuote} | Status={s.CurrentStatus} Date={s.StatusDate}");
}

Console.WriteLine("Seed completo.");
