# Cloud-native .NET Core Kit

[![Price](https://img.shields.io/badge/price-FREE-0098f7.svg)](https://github.com/cloudnative-netcore/netcorekit/blob/master/LICENSE)
[![version](https://img.shields.io/nuget/v/NetCoreKit.Domain.svg?label=version)](https://www.nuget.org/packages?q=NetCoreKit)
[![Build status](https://img.shields.io/appveyor/ci/thangchung/netcore-kit.svg)](https://ci.appveyor.com/api/project/thangchung/netcore-kit)

A set of cloud-native tools and utilities for .NET Core.

### Features

- Simple libraries. No frameworks. Little abstraction.
- Opt-in and out of the box [features](https://github.com/cloudnative-netcore/netcorekit/wiki/Miniservice-template-guidance) with [Feature Toggles](https://martinfowler.com/articles/feature-toggles.html) technique.
- Generic repository for data persistence.
- Adhere to [twelve-factor app paradigm](https://12factor.net) and more.
- Resilience and health check out of the box.
- Easy for configuration management.
- [Domain-driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) in mind.
- Simply [Clean Architecture](http://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) supports.
- Authentication/Authorization with OAuth 2.0 and OpenID Connect.
- Clean and demystify error, debug logs.
- API versioning from Docker container to WebAPI. 
- Documentation template with OpenAPI documentation.
- Work natively with Kubernetes or even with Service Mesh(Istio).

## Less code to get starting

Small, lightweight, cloud-native out of the box, and much more simple to get starting with miniservices approach. [Why miniservices?](https://thenewstack.io/miniservices-a-realistic-alternative-to-microservices)

- No data storage

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMiniService();
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseMiniService();
  }
}
```

- With Entity Framework

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddEfCoreMiniService<TodoListDbContext>(svc => svc.AddEfCoreMySqlDb());
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseMiniService();
  }
}
```

- With MongoDb

```csharp
public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddMongoMiniService();
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseMiniService();
  }
}
```

<details>
  <summary>More guidances to get starting</summary>

- Read [Get starting](https://github.com/cloudnative-netcore/netcorekit/wiki/Get-Started) section and [Play with Kubernetes](https://github.com/cloudnative-netcore/netcorekit/wiki/Deploy-on-k8s-on-local) section to know more about this cloud-native toolkit.
- Basic usage can be found at [TodoApi Sample](https://github.com/cloudnative-netcore/netcorekit/tree/master/samples/TodoApi)
- More advance usage is at [Coolstore Microservices](https://github.com/vietnam-devs/coolstore-microservices) project.

</details>

### Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :p
