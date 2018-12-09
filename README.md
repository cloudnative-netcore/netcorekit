# Cloud-native .NET Core Kit

[![Build status](https://img.shields.io/appveyor/ci/thangchung/netcore-kit.svg)](https://ci.appveyor.com/api/project/thangchung/netcore-kit)
[![Price](https://img.shields.io/badge/price-FREE-0098f7.svg)](https://github.com/cloudnative-netcore/netcorekit/blob/master/LICENSE)
[![version](https://img.shields.io/nuget/v/NetCoreKit.Domain.svg?label=version)](https://www.nuget.org/packages?q=NetCoreKit)

A set of cloud-native tools and utilities for .NET Core.

The goal of this project is implement the most common used cloud-native technologies (cloud-agnostic approach, containerization mechanism, container orchestration and so on) and share with the technical community the best way to develop great applications with .NET Core.

## Give a Star! :star:

If you liked the project or if `netcorekit` helped you, please give a star so that .NET community will know and help them just like you. Thank you very much :+1:

### Features

- [x] Simple libraries. No frameworks. Little abstraction.
- [x] Opt-in and out of the box [features](https://github.com/cloudnative-netcore/netcorekit/wiki/Miniservice-template-guidance) with [Feature Toggles](https://martinfowler.com/articles/feature-toggles.html) technique.
- [x] Generic repository for data persistence.
- [x] Adhere to [twelve-factor app paradigm](https://12factor.net) and more.
- [x] Resilience and health check out of the box.
- [x] Easy for configuration management.
- [x] [Domain-driven Design](https://en.wikipedia.org/wiki/Domain-driven_design) in mind.
- [x] Simply [Clean Architecture](http://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) supports.
- [x] Authentication/Authorization with OAuth 2.0 and OpenID Connect.
- [x] Clean and demystify error, debug logs.
- [x] API versioning from Docker container to WebAPI. 
- [x] Documentation template with OpenAPI documentation.
- [x] Work natively with Kubernetes or even with Service Mesh(Istio).

## Less code to get starting

Small, lightweight, cloud-native out of the box, and much more simple to get starting with miniservices approach. [Why miniservices?](https://thenewstack.io/miniservices-a-realistic-alternative-to-microservices)

### Look how simple we can start as below:

- Without storage, merely calculation and job tasks:

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

- With Entity Framework Core (SQL Server, MySQL, and SQLite providers) comes along with the generic repository in place:

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

- With NoSQL (MongoDb provider) comes along with the generic repository in place:

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

- Read [Get starting](https://github.com/cloudnative-netcore/netcorekit/wiki/Get-Started) section and [Play with Kubernetes](https://github.com/cloudnative-netcore/netcorekit/wiki/Deploy-on-k8s-on-local) section to know more about this cloud-native toolkit.
- Basic usage can be found at [TodoApi Sample](https://github.com/cloudnative-netcore/netcorekit/tree/master/samples/TodoApi)
- More advance usage is at [Coolstore Microservices](https://github.com/vietnam-devs/coolstore-microservices) project.

### Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :p
