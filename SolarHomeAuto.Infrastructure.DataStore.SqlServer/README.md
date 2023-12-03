# Entity Framework Migrations

To install dotnet ef tools

```
dotnet tool install --global dotnet-ef
```

To update database with latest migrations
```
cd SolarHomeAuto.Infrastructure.DataStore.SqlServer
dotnet ef database update
```

To create migrations SQL script
```
dotnet ef migrations script --idempotent > migrate.sql
```

To rollback to specific migration
```
dotnet ef database update InitialCreate
```

To add new migration:
```
cd SolarHomeAuto.Infrastructure.DataStore
dotnet ef migrations add InitialCreate
```

To remove last migration
```
dotnet ef migrations remove
```