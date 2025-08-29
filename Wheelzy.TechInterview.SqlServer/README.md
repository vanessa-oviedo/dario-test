# Wheelzy Tech Interview – SQL Server variant (.NET 8 + EF Core SqlServer)

Esta versión usa **SQL Server** (no SQLite). Incluye `docker-compose` para levantar un SQL local.

## Requisitos
- .NET SDK 8.x
- Docker (para SQL Server local) o un SQL Server disponible

## Levantar SQL Server en Docker
```bash
docker compose up -d
# Esperá unos segundos hasta que SQL esté "healthy"
```

Credenciales por defecto (solo desarrollo):
- **Server**: localhost,1433
- **User**: sa
- **Password**: Passw0rd!ChangeMe
- **DB**: WheelzyDb

> Cambiá la password en `docker-compose.yml` si querés.

## Restaurar y compilar
```bash
cd src
dotnet restore
dotnet build -c Release
```

## Correr la API
```bash
dotnet run --project Wheelzy.Api/Wheelzy.Api.csproj
```
- La API crea la base y datos mínimos al iniciar (usa `EnsureCreated` para simplificar el ejercicio).
- Endpoint: `GET http://localhost:5000/cases`

## Semilla (Seeder)
```bash
dotnet run --project Wheelzy.Tools.Seed/Wheelzy.Tools.Seed.csproj
```

## Migraciones (opcional, si preferís `dotnet ef`)
```bash
# Instalar herramientas si no las tenés
dotnet tool install --global dotnet-ef

# Crear migración inicial
dotnet ef migrations add Initial --project Wheelzy.Infrastructure/Wheelzy.Infrastructure.csproj --startup-project Wheelzy.Api/Wheelzy.Api.csproj

# Aplicar migraciones
dotnet ef database update --project Wheelzy.Infrastructure/Wheelzy.Infrastructure.csproj --startup-project Wheelzy.Api/Wheelzy.Api.csproj
```
> Si usás migraciones, podés quitar `EnsureCreated` del `Program.cs` de la API.
