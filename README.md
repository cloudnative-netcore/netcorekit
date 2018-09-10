# Cloud-native .NET Core Kit
[![Price](https://img.shields.io/badge/price-FREE-0098f7.svg)](https://github.com/cloudnative-netcore/netcorekit/blob/master/LICENSE)
[![version](https://img.shields.io/nuget/v/NetCoreKit.Domain.svg?label=version)](https://www.nuget.org/packages?q=NetCoreKit)
[![Build status](https://img.shields.io/appveyor/ci/thangchung/netcore-kit.svg)](https://ci.appveyor.com/api/project/thangchung/netcore-kit)

A set of cloud-native tools and utilities for .NET Core.

<details>
  <summary>Features</summary>

- Simple libraries. No frameworks. Little abstraction.
- Modular (Easy to swap out Utils, Domain, AspNetCore, Clean Architecture, Open API, Entity Framework Core, Event Bus...)
- Adhere to [twelve-factor app paradigm](https://12factor.net) and more.
- Resilience and health check out of the box.
- Easy for configuration management.
- Simply clean architecture supports.
- Authentication/Authorization with OAuth 2.0 and OpenID Connect.
- Clean and demystify error, debug logs.
- API versioning from Docker container to WebAPI. 
- Documentation template with OpenAPI documentation.
- Work natively with Kubernetes or even with Service Mesh.

</details>

## Less Code to Get Starting

Small, lightweight, cloud-native out of the box, and much more simple to get starting with miniservices approach. [Why miniservices?](https://thenewstack.io/miniservices-a-realistic-alternative-to-microservices)

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMiniService<TodoListDbContext>(
      svc =>
      {
        svc.AddEfSqlLiteDb();
        svc.AddExternalSystemHealthChecks();
      }
    );
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseMiniService();
  }
}
```

<details>
  <summary>More types of equipment to get starting</summary>

- Read [Get starting](https://github.com/cloudnative-netcore/netcorekit/wiki/Get-Started) section and [Play with Kubernetes](https://github.com/cloudnative-netcore/netcorekit/wiki/Deploy-on-k8s-on-local) section to know more about this cloud-native toolkit.
- Basic usage can be found at [TodoApi Sample](https://github.com/cloudnative-netcore/netcorekit/tree/master/samples/TodoApi)
- More advance usage is at [Coolstore Microservices](https://github.com/vietnam-devs/coolstore-microservices) project.

</details>

<details>
  <summary>Contributing</summary>

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :p

</details>
