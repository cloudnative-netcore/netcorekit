using System.Collections.Generic;

namespace NetCoreKit.Domain
{
  public class PaginatedItem<TResponse> : ValueObjectBase
  {
    public PaginatedItem(int totalItems, int totalPages, IReadOnlyList<TResponse> items)
    {
      TotalItems = totalItems;
      TotalPages = totalPages;
      Items = items;
    }

    public int TotalItems { get; }

    public long TotalPages { get; }

    public IReadOnlyList<TResponse> Items { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return TotalItems;
      yield return TotalPages;
      yield return Items;
    }
  }
}
