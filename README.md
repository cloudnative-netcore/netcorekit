# Cloud Native .NET Core Kit

A set of cloud native tools and utilities for .NET Core.

<p align="left">
  <a href="https://github.com/cloudnative-netcore/netcore-kit/blob/master/LICENSE"><img src="https://img.shields.io/badge/price-FREE-0098f7.svg" alt="Price"></a>
  <a href="https://ci.appveyor.com/api/projects/status/cxfcynyaufo2tp3m?svg=true"><img src="https://ci.appveyor.com/api/projects/status/cxfcynyaufo2tp3m?svg=true" alt="Build Status" data-canonical-src="https://ci.appveyor.com/api/projects/status/cxfcynyaufo2tp3m?svg=true" style="max-width:100%;"></a>
</p>

### Features
- Simple libraries. No frameworks. Little abstraction.
- Modular (Easy to swap out Utils, Domain, AspNetCore, Clean Architecture, Open API, Entity Framework Core...)
- Tries to adhere to the 12 factor application paradigm by Heroku.
- Documentation template with OpenAPI documentation.

### Installation

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

### Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :p

### Licence

Code released under [the MIT license](https://github.com/cloudnative-netcore/netcore-kit/blob/master/LICENSE).