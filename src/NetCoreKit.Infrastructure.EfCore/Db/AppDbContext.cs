using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Db
{
  public abstract class AppDbContext : DbContext
  {
    protected AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      var config = this.GetService<IConfiguration>();

      var typeToRegisters = new List<Type>();

      var ourModules = config.LoadFullAssemblies();

      typeToRegisters.AddRange(ourModules.SelectMany(m => m.DefinedTypes));

      RegisterEntities(builder, typeToRegisters);

      RegisterConvention(builder);

      base.OnModelCreating(builder);

      RegisterCustomMappings(builder, typeToRegisters);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
      var eventBus = this.GetService<IDomainEventBus>();

      var result = base.SaveChangesAsync(cancellationToken);

      if (eventBus != null)
        SaveChangesWithEvents(eventBus);

      return result;
    }

    public override int SaveChanges()
    {
      var eventBus = this.GetService<IDomainEventBus>();

      var result = base.SaveChanges();

      if (eventBus != null)
        SaveChangesWithEvents(eventBus);

      return result;
    }

    /// <summary>
    ///   Source:
    ///   https://github.com/ardalis/CleanArchitecture/blob/master/src/CleanArchitecture.Infrastructure/Data/AppDbContext.cs
    /// </summary>
    private void SaveChangesWithEvents(IDomainEventBus domainEventBus)
    {
      var entitiesWithEvents = ChangeTracker.Entries<IAggregateRoot>()
        .Select(e => e.Entity)
        .Where(e => e.GetUncommittedEvents().Any())
        .ToArray();

      foreach (var entity in entitiesWithEvents)
      {
        var events = entity.GetUncommittedEvents().ToArray();
        entity.GetUncommittedEvents().Clear();
        foreach (var domainEvent in events)
          domainEventBus.Publish(new EventEnvelope(domainEvent));
      }
    }

    private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
    {
      // TODO: will optimize this more
      var types = typeToRegisters.Where(x =>
        typeof(IEntity).IsAssignableFrom(x) &&
        !x.GetTypeInfo().IsAbstract);

      foreach (var type in types) modelBuilder.Entity(type);
    }

    private static void RegisterConvention(ModelBuilder modelBuilder)
    {
      var types = modelBuilder.Model.GetEntityTypes()
        .Where(entity => entity.ClrType.Namespace != null);

      foreach (var entityType in types)
        modelBuilder.Entity(entityType.Name).ToTable(entityType.ClrType.Name.Pluralize());
    }

    private static void RegisterCustomMappings(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
    {
      var customModelBuilderTypes =
        typeToRegisters.Where(x => typeof(ICustomModelBuilder).IsAssignableFrom(x));

      foreach (var builderType in customModelBuilderTypes)
        if (builderType != null && builderType != typeof(ICustomModelBuilder))
        {
          var builder = (ICustomModelBuilder)Activator.CreateInstance(builderType);
          builder.Build(modelBuilder);
        }
    }
  }
}
