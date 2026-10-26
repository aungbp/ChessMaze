# BCIT615 A2 EF Core and SQLite support package v0.9

Aligned with the recovery-baseline model contract. All supported pathways use `<solution root>\data\saved-games.db` unless `CHESSMAZE_DB_PATH` is set.

## Setup and validation
```powershell
dotnet tool restore
dotnet restore .\BCIT615.A2.EFCore.Support.sln
dotnet build .\BCIT615.A2.EFCore.Support.sln --no-restore
dotnet ef migrations has-pending-model-changes --project .\src\ChessMaze.Persistence\ChessMaze.Persistence.csproj --startup-project .\tools\ChessMaze.DatabaseTool\ChessMaze.DatabaseTool.csproj --no-build
dotnet ef migrations list --project .\src\ChessMaze.Persistence\ChessMaze.Persistence.csproj --startup-project .\tools\ChessMaze.DatabaseTool\ChessMaze.DatabaseTool.csproj --no-build
dotnet ef database update --project .\src\ChessMaze.Persistence\ChessMaze.Persistence.csproj --startup-project .\tools\ChessMaze.DatabaseTool\ChessMaze.DatabaseTool.csproj --no-build
dotnet test .\tests\ChessMaze.Tests\ChessMaze.Tests.csproj --no-build --logger "console;verbosity=detailed"
```

## Safe local reset
Close applications using the database, remove `.\data\saved-games.db`, re-run `dotnet ef database update`, then confirm the database was recreated. Use `Get-ChildItem -Recurse -Filter saved-games.db` to identify unexpected copies.
