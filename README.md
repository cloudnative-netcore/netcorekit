# Cloud Native .NET Core Kit

A set of cloud native tools and utilities for .NET Core.

[![Price](https://img.shields.io/badge/price-FREE-0098f7.svg)](https://github.com/cloudnative-netcore/netcore-kit/blob/master/LICENSE)
[![Build status](https://ci.appveyor.com/api/projects/status/cxfcynyaufo2tp3m?svg=true)](https://ci.appveyor.com/api/project/thangchung/netcore-kit)
[![version](https://img.shields.io/nuget/v/NetCoreKit.Domain.svg?label=version)](https://www.nuget.org/packages?q=NetCoreKit)

### Features
- Simple libraries. No frameworks. Little abstraction.
- Modular (Easy to swap out Utils, Domain, AspNetCore, Clean Architecture, Open API, Entity Framework Core...)
- Adhere to [twelve-factor app paradigm](https://12factor.net) and more.
- Resilience and health check out of the box.
- Easy for configuration management.
- Simply clean architecture supports.
- Authentication/Authorization with OAuth 2.0 and OpenID Connect.
- Clean and demystify error, debug logs.
- API versioning from Docker container to WebAPI. 
- Documentation template with OpenAPI documentation.
- Work natively with Kubernetes or even with Service Mesh.

### Installation

**NetCoreKit** can be [found here on NuGet](https://www.nuget.org/packages?q=NetCoreKit) and can be installed by copying and pasting the following command into your Package Manager Console within Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

```bash
Install-Package NetCoreKit.Utils
Install-Package NetCoreKit.Domain
Install-Package NetCoreKit.Infrastructure
Install-Package NetCoreKit.Infrastructure.AspNetCore
Install-Package NetCoreKit.Infrastructure.AspNetCore.CleanArch
Install-Package NetCoreKit.Infrastructure.AspNetCore.Miniservice
Install-Package NetCoreKit.Infrastructure.AspNetCore.OpenApi
Install-Package NetCoreKit.Infrastructure.EfCore
Install-Package NetCoreKit.Infrastructure.EfCore.SqlServer
Install-Package NetCoreKit.Infrastructure.EfCore.MySql
```

Alternatively if you're using .NET Core then you can install **NetCoreKit** via the command line interface with the following command:

```bash
dotnet add package NetCoreKit.Utils
dotnet add package NetCoreKit.Domain
dotnet add package NetCoreKit.Infrastructure
dotnet add package NetCoreKit.Infrastructure.AspNetCore
dotnet add package NetCoreKit.Infrastructure.AspNetCore.CleanArch
dotnet add package NetCoreKit.Infrastructure.AspNetCore.Miniservice
dotnet add package NetCoreKit.Infrastructure.AspNetCore.OpenApi
dotnet add package NetCoreKit.Infrastructure.EfCore
dotnet add package NetCoreKit.Infrastructure.EfCore.SqlServer
dotnet add package NetCoreKit.Infrastructure.EfCore.MySql
```

### Up and Running

- Open up the `netcore-kit.sln`, then press `F5`
- We should see OpenAPI UI of `samples\TodoApi` sample
- Just play around with it.

### Database Providers

- SQL Server

```
> docker run --name sqlserverdb -p 1433:1433 -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=Passw0rd  microsoft/mssql-server-linux:2017-latest
```

- MySQL

```
> docker run --name mysqldb -p 3306:3306 -e MYSQL_ROOT_PASSWORD=P@ssw0rd -e MYSQL_PASSWORD=P@ssw0rd mysql:8.0.12
```

- Add middleware in `samples\TodoApi\Startup.cs` as following

```csharp
services.AddMiniService<TodoDbContext>(
  new[] {typeof(Startup)},
    svc =>
    {
      // svc.AddEfCoreSqlServerDb();
      svc.AddEfCoreMySqlDb();
      svc.AddExternalSystemHealthChecks();
    });
```

- Put connection string for each type of database into `ConnectionStrings` section in the `appsettings.json` file as below

```json
"mssqldb": "Server=tcp:127.0.0.1,1433;Database=maindb;User Id=cs;Password=P@ssw0rd;"
```

```json
"mysqldb": "server=127.0.0.1;port=3306;uid=root;pwd=P@ssw0rd;database=maindb"
```

> Basic used can be found at [TodoApi Sample](https://github.com/cloudnative-netcore/netcore-kit/tree/master/samples/TodoApi), more advance at [Coolstore Microservices](https://github.com/vietnam-devs/coolstore-microservices) project. We use this libs for building up the whole technical stack for them.

### Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :p

### Licence

Code released under [the MIT license](https://github.com/cloudnative-netcore/netcore-kit/blob/master/LICENSE).
