using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreKit.Domain;

namespace NetCoreKit.Infrastructure.Mongo
{
  public static class MongoQueryRepositoryExtensions
  {
    public static async Task<TEntity> FindOneAsync<TEntity, TId>(
      this IMongoQueryRepository<TEntity> repo,
      Expression<Func<TEntity, TId>> filter,
      TId id,
      CancellationToken cancellationToken = default(CancellationToken))
      where TEntity : class, IAggregateRoot
    {
      var collection = repo.DbContext.Collection<TEntity>();
      var filterDef = Builders<TEntity>.Filter.Eq(filter, id);
      return await collection.Find(filterDef).FirstOrDefaultAsync(cancellationToken);
    }

    public static async Task<IReadOnlyList<TEntity>> FindListByFieldAsync<TEntity>(
      this IMongoQueryRepository<TEntity> repo,
      Expression<Func<TEntity, bool>> filter = null,
      CancellationToken cancellationToken = default(CancellationToken))
      where TEntity : class, IAggregateRoot
    {
      // only use it if you don't have other solutions :p
      return await repo
        .DbContext
        .Collection<TEntity>()
        .Find(filter)
        .ToListAsync(cancellationToken);
    }

    public static async Task<PaginatedItem<TResponse>> QueryAsync<TEntity, TResponse>(
      this IMongoQueryRepository<TEntity> repo,
      Criterion criterion,
      Expression<Func<TEntity, TResponse>> selector,
      CancellationToken cancellationToken = default(CancellationToken))
      where TEntity : class, IAggregateRoot
    {
      return await GetDataAsync(repo, criterion, selector, null, cancellationToken);
    }

    public static async Task<PaginatedItem<TResponse>> FindAllAsync<TEntity, TResponse>(
      this IMongoQueryRepository<TEntity> repo,
      Criterion criterion,
      Expression<Func<TEntity, TResponse>> selector,
      Expression<Func<TEntity, bool>> filter,
      CancellationToken cancellationToken = default(CancellationToken))
      where TEntity : class, IAggregateRoot
    {
      return await GetDataAsync(repo, criterion, selector, filter, cancellationToken);
    }

    private static async Task<PaginatedItem<TResponse>> GetDataAsync<TEntity, TResponse>(
      IMongoQueryRepository<TEntity> repo,
      Criterion criterion,
      Expression<Func<TEntity, TResponse>> selector,
      Expression<Func<TEntity, bool>> filter = null,
      CancellationToken cancellationToken = default(CancellationToken))
      where TEntity : class, IAggregateRoot
    {
      var collection = repo.DbContext.Collection<TEntity>();
      FilterDefinition<TEntity> filterDef = null;
      SortDefinition<TEntity> sortDef = null;

      if (filter != null)
        filterDef = Builders<TEntity>.Filter.Eq(filter, true);
      
      if (!string.IsNullOrWhiteSpace(criterion.SortBy))
      {
        var isDesc = string.Equals(criterion.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        FieldDefinition<TEntity> field = criterion.SortBy;
        sortDef = !isDesc
          ? Builders<TEntity>.Sort.Ascending(field)
          : Builders<TEntity>.Sort.Descending(field);
      }

      // default we will set it at 0-index
      if (criterion.CurrentPage > 0) criterion.CurrentPage--;

      var results = await collection
        .FindEntityWithProjection(filterDef, selector)
        .SortEntity(sortDef)
        .Skip(criterion.CurrentPage * criterion.PageSize)
        .Limit(criterion.PageSize)
        .ToListAsync(cancellationToken);

      var totalRecord = await collection.CountDocumentsAsync(FilterDefinition<TEntity>.Empty, null, cancellationToken);
      var totalPages = (int)Math.Ceiling((double)totalRecord / criterion.PageSize);

      if (criterion.CurrentPage > totalPages)
      {
        // criterion.SetCurrentPage(totalPages);
      }

      return new PaginatedItem<TResponse>(totalRecord, totalPages, results);
    }

    private static IFindFluent<TEntity, TResponse> SortEntity<TEntity, TResponse>(this IFindFluent<TEntity, TResponse> collectionFluent,
      SortDefinition<TEntity> sortDef
    ) where TEntity : class, IAggregateRoot
    {
      return collectionFluent.Sort(sortDef);
    }

    private static IFindFluent<TEntity, TResponse> FindEntityWithProjection<TEntity, TResponse>(this IMongoCollection<TEntity> collection,
      FilterDefinition<TEntity> filterDef,
      Expression<Func<TEntity, TResponse>> selector
    ) where TEntity : class, IAggregateRoot
    {
      return filterDef != null
        ? collection.Find(filterDef).Project(selector)
        : collection.Find(FilterDefinition<TEntity>.Empty).Project(selector);
    }
  }
}
