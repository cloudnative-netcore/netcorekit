# netcore-kit

A set of Cloud Native tools and utilities for .NET Core.

## Installation

**NetCoreKit** can be [found here on NuGet](https://www.nuget.org/packages?q=NetCoreKit) and can be installed by copying and pasting the following command into your Package Manager Console within Visual Studio (Tools > NuGet Package Manager > Package Manager Console).

```bash
Install-Package NetCoreKit.Utils
Install-Package NetCoreKit.Domain
Install-Package NetCoreKit.Infrastructure.AspNetCore
Install-Package NetCoreKit.Infrastructure.AspNetCore.CleanArch
Install-Package NetCoreKit.Infrastructure.AspNetCore.Miniservice
Install-Package NetCoreKit.Infrastructure.AspNetCore.OpenApi
Install-Package NetCoreKit.Infrastructure.EfCore
Install-Package NetCoreKit.Infrastructure.EfCore.SqlServer
```

Alternatively if you're using .NET Core then you can install **NetCoreKit** via the command line interface with the following command:

```bash
dotnet add package NetCoreKit.Utils
dotnet add package NetCoreKit.Domain
dotnet add package NetCoreKit.Infrastructure.AspNetCore
dotnet add package NetCoreKit.Infrastructure.AspNetCore.CleanArch
dotnet add package NetCoreKit.Infrastructure.AspNetCore.Miniservice
dotnet add package NetCoreKit.Infrastructure.AspNetCore.OpenApi
dotnet add package NetCoreKit.Infrastructure.EfCore
dotnet add package NetCoreKit.Infrastructure.EfCore.SqlServer
```