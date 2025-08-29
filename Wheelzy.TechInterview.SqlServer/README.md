# Wheelzy – Multi-proyecto (.NET 8, SQL Server) con Swagger en la API

Estructura:
- `Wheelzy.Domain` → Entidades y DTOs
- `Wheelzy.Infrastructure` → EF Core (SqlServer), DbContext, seed, consultas
- `Wheelzy.Api` → Minimal API, **Swagger** habilitado

## Requisitos
- .NET SDK 8.x
- Docker (opcional) para SQL Server local

## SQL Server local (opcional)
```bash
docker compose up -d
```

## Build & Run
```bash
cd src
dotnet restore
dotnet build -c Release
dotnet run --project Wheelzy.Api/Wheelzy.Api.csproj
```
- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`

## Migraciones (opcional, reemplazando EnsureCreated)
```bash
dotnet tool install --global dotnet-ef

dotnet ef migrations add Initial   --project Wheelzy.Infrastructure/Wheelzy.Infrastructure.csproj   --startup-project Wheelzy.Api/Wheelzy.Api.csproj

dotnet ef database update   --project Wheelzy.Infrastructure/Wheelzy.Infrastructure.csproj   --startup-project Wheelzy.Api/Wheelzy.Api.csproj
```
Luego podés quitar `EnsureCreated` del `Program.cs` de la API.
