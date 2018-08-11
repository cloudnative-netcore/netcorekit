using System;

namespace NetCoreKit.Domain
{
  public interface IDomainEvent
  {
    int EventVersion { get; set; }
    DateTime OccurredOn { get; set; }
  }
}
