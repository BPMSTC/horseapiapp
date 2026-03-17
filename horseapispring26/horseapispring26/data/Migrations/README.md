This folder will contain EF Core migrations. To create the initial migration, run:

- Use Visual Studio Package Manager Console:
  Add-Migration AddHorseSchema -Project horseapispring26 -StartupProject horseapispring26

- Or use dotnet CLI (from solution directory):
  dotnet ef migrations add AddHorseSchema --project horseapispring26/horseapispring26.csproj --startup-project horseapispring26/horseapispring26.csproj

Applying migrations (ensure SQL Server is reachable via connection string):
  dotnet ef database update --project horseapispring26/horseapispring26.csproj --startup-project horseapispring26/horseapispring26.csproj
