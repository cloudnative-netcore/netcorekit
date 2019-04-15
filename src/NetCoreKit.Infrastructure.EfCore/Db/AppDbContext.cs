using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.EfCore.Db
{
    public abstract class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly IDomainEventDispatcher _eventBus = null;

        protected AppDbContext(DbContextOptions options, IConfiguration config, IDomainEventDispatcher eventBus = null)
            : base(options)
        {
            _config = config;
            _eventBus = eventBus ?? new MemoryDomainEventDispatcher();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var typeToRegisters = new List<Type>();
            var ourModules = _config.LoadFullAssemblies();

            typeToRegisters.AddRange(ourModules.SelectMany(m => m.DefinedTypes));

            RegisterEntities(builder, typeToRegisters);

            RegisterConvention(builder);

            base.OnModelCreating(builder);

            RegisterCustomMappings(builder, typeToRegisters);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = base.SaveChangesAsync(cancellationToken);
            SaveChangesWithEvents(_eventBus);
            return result;
        }

        public override int SaveChanges()
        {
            var result = base.SaveChanges();
            SaveChangesWithEvents(_eventBus);
            return result;
        }

        /// <summary>
        /// Source: https://github.com/ardalis/CleanArchitecture/blob/master/src/CleanArchitecture.Infrastructure/Data/AppDbContext.cs
        /// </summary>
        private void SaveChangesWithEvents(IDomainEventDispatcher domainEventDispatcher)
        {
            var entities = ChangeTracker.Entries().Select(e => e.Entity);

            entities
                .Where(e =>
                    !e.GetType().BaseType.IsGenericType &&
                    typeof(AggregateRootBase).IsAssignableFrom(e.GetType()))
                .Select(aggregateRoot =>
                {
                    var events = ((IAggregateRoot)aggregateRoot).GetUncommittedEvents();

                    foreach (var domainEvent in events)
                        domainEventDispatcher.Dispatch(domainEvent);

                    ((IAggregateRoot)aggregateRoot).GetUncommittedEvents().Clear();
                    return aggregateRoot;
                })
                .ToArray();

            entities
                .Where(e =>
                    e.GetType().BaseType.IsGenericType &&
                    typeof(AggregateRootWithIdBase<>).IsAssignableFrom(e.GetType().BaseType.GetGenericTypeDefinition()))
                .Select(aggregateRoot =>
                {
                    //todo: need a better code to avoid dynamic
                    var events = ((dynamic)aggregateRoot).GetUncommittedEvents();

                    foreach (var domainEvent in events)
                        domainEventDispatcher.Dispatch(domainEvent);

                    ((dynamic)aggregateRoot).GetUncommittedEvents().Clear();
                    return aggregateRoot;
                })
                .ToArray();
        }

        private static void RegisterEntities(ModelBuilder modelBuilder, IEnumerable<Type> typeToRegisters)
        {
            var concreteTypes = typeToRegisters.Where(x => !x.GetTypeInfo().IsAbstract && !x.GetTypeInfo().IsInterface);
            var types = new List<Type>();

            foreach (var concreteType in concreteTypes)
            {
                if (concreteType.BaseType != null &&
                    (typeof(AggregateRootBase).IsAssignableFrom(concreteType) ||
                     (concreteType.GetTypeInfo().BaseType.IsGenericType &&
                      typeof(AggregateRootWithIdBase<>).IsAssignableFrom(concreteType.GetTypeInfo().BaseType.GetGenericTypeDefinition())
                    )))
                {
                    modelBuilder.Entity(concreteType);
                }
            }
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
