# Commands

## Run below commands to run backend project
````
dotnet run src/InteractHub.WebApi/InteractHub.WebApi.csproj

dotnet build src/InteractHub.WebApi/InteractHub.WebApi.csproj

dotnet publish src/InteractHub.WebApi/InteractHub.WebApi.csproj

dotnet clean

dotnet restore

dotnet test
````

## Run below commands in sequence to create architecture for backend
````
mkdir InteractHub
cd InteractHub
dotnet new sln -n=InteractHub
mkdir src

// Domain
dotnet new classlib --output src/InteractHub.Domain
dotnet sln add src/InteractHub.Domain/InteractHub.Domain.csproj

// Application
dotnet new classlib --output src/InteractHub.Application
dotnet sln add src/InteractHub.Application/InteractHub.Application.csproj
dotnet add src/InteractHub.Application/InteractHub.Application.csproj reference src/InteractHub.Domain/InteractHub.Domain.csproj

// Infrastructure
dotnet new classlib --output src/InteractHub.Infrastructure
dotnet sln add src/InteractHub.Infrastructure/InteractHub.Infrastructure.csproj
dotnet add src/InteractHub.Infrastructure/InteractHub.Infrastructure.csproj reference src/InteractHub.Domain/InteractHub.Domain.csproj
dotnet add src/InteractHub.Infrastructure/InteractHub.Infrastructure.csproj reference src/InteractHub.Application/Application.Domain.csproj

// WebApi
dotnet new webapi --output src/InteractHub.WebApi
dotnet sln add src/InteractHub.WebApi/InteractHub.WebApi.csproj
dotnet add src/InteractHub.WebApi/InteractHub.WebApi.csproj reference src/InteractHub.Domain/InteractHub.Domain.csproj
dotnet add src/InteractHub.WebApi/InteractHub.WebApi.csproj reference src/InteractHub.Application/InteractHub.Application.csproj
dotnet add src/InteractHub.WebApi/InteractHub.WebApi.csproj reference src/InteractHub.Infrastructure/InteractHub.Infrastructure.csproj

// Test-Domain
dotnet new xunit --output src/Tests/InteractHub.Domain.Test
dotnet sln add src/Tests/InteractHub.Domain.Test
dotnet add src/Tests/InteractHub.Domain.Test reference src/InteractHub.Domain
dotnet add src/Tests/InteractHub.Domain.Test reference src/InteractHub.Infrastructure

// Test-Application
dotnet new xunit --output src/Tests/InteractHub.Application.Test
dotnet sln add src/Tests/InteractHub.Application.Test
dotnet add src/Tests/InteractHub.Application.Test reference src/InteractHub.Application

// Test-Infrastructure
dotnet new xunit --output src/Tests/InteractHub.Infrastructure.Test
dotnet sln add src/Tests/InteractHub.Infrastructure.Test
dotnet add src/Tests/InteractHub.Infrastructure.Test reference src/InteractHub.Infrastructure
````

## Commands for Db Migration
Install EF tool
```
dotnet tool install --global dotnet-ef
```

In InteractHub.Infrastructure Project, run below command to create new migration
````
dotnet ef migrations add InitialMigration --startup-project ../InteractHub.WebApi/InteractHub.WebApi.csproj --output-dir Data/Migrations
````
