using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static NetCoreKit.Utils.Helpers.IdHelper;
using static NetCoreKit.Utils.Helpers.DateTimeHelper;

namespace NetCoreKit.Domain
{
  public interface IEntity : IIdentity
  { }

  public interface IAggregateRoot: IEntity
  {
    IAggregateRoot ApplyEvent(IEvent payload);
    List<IEvent> GetUncommittedEvents();
    void ClearUncommittedEvents();
    IAggregateRoot RemoveEvent(IEvent @event);
    IAggregateRoot AddEvent(IEvent uncommittedEvent);
    IAggregateRoot RegisterHandler<T>(Action<T> handler);
  }

  public abstract class AggregateRootBase : EntityBase, IAggregateRoot
  {
    private readonly List<IEvent> _uncommittedEvents = new List<IEvent>();
    private readonly IDictionary<Type, Action<object>> _handlers = new ConcurrentDictionary<Type, Action<object>>();

    protected AggregateRootBase() : this(GenerateId())
    {
    }

    protected AggregateRootBase(Guid id)
    {
      Id = id;
      Created = GenerateDateTime();
    }

    public int Version { get; protected set; }

    public IAggregateRoot AddEvent(IEvent uncommittedEvent)
    {
      _uncommittedEvents.Add(uncommittedEvent);
      ApplyEvent(uncommittedEvent);
      return this;
    }

    public IAggregateRoot ApplyEvent(IEvent payload)
    {
      if (!_handlers.ContainsKey(payload.GetType())) return this;
      _handlers[payload.GetType()]?.Invoke(payload);
      Version++;
      return this;
    }

    public void ClearUncommittedEvents()
    {
      _uncommittedEvents.Clear();
    }

    public List<IEvent> GetUncommittedEvents()
    {
      return _uncommittedEvents;
    }

    public IAggregateRoot RegisterHandler<T>(Action<T> handler)
    {
      _handlers.Add(typeof(T), e => handler((T)e));
      return this;
    }

    public IAggregateRoot RemoveEvent(IEvent @event)
    {
      if (_uncommittedEvents.Find(e => e == @event) != null) _uncommittedEvents.Remove(@event);
      return this;
    }
  }

  /// <summary>
  /// Source: https://github.com/VaughnVernon/IDDD_Samples_NET
  /// </summary>
  public abstract class EntityBase : IEntity
  {
    protected EntityBase() : this(GenerateId())
    {
    }

    protected EntityBase(Guid id)
    {
      Id = id;
      Created = GenerateDateTime();
    }

    [Key]
    public Guid Id { get; protected set; }

    public DateTime Created { get; protected set; }

    public DateTime Updated { get; protected set; }
  }
}
