# DataAccess Implementation Readme

### Create Migrations:

```
dotnet ef -s ./../Presentation migrations add <YourMigrationName> -c MGCapDbContext | MGCapIdentityDbContext> -o "<Migrations/[MGCapDb | MGCapIdentityDb]>"


dotnet ef -s ../../Presentation migrations add <YourMigrationName> -c MGCapDbContext -o "Migrations/MGCapDb"

```

### Run Migrations:

```
dotnet ef -s ../../Presentation database update -c MGCapDbContext"

```
