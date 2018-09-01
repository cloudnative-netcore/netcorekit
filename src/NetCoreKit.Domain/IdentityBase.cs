using System;
using static NetCoreKit.Utils.Helpers.IdHelper;

namespace NetCoreKit.Domain
{
  /// <summary>
  ///   Source: https://github.com/VaughnVernon/IDDD_Samples_NET
  /// </summary>
  public abstract class IdentityBase : IEquatable<IdentityBase>, IIdentity
  {
    protected IdentityBase()
    {
      Id = GenerateId();
    }

    protected IdentityBase(Guid id)
    {
      Id = id;
    }

    public bool Equals(IdentityBase id)
    {
      if (ReferenceEquals(this, id)) return true;
      return !ReferenceEquals(null, id) && Id.Equals(id.Id);
    }

    // currently for Entity Framework, set must be protected, not private.
    // will be fixed in EF 6.
    public Guid Id { get; }

    public override bool Equals(object anotherObject)
    {
      return Equals(anotherObject as IdentityBase);
    }

    public override int GetHashCode()
    {
      return GetType().GetHashCode() * 907 + Id.GetHashCode();
    }

    public override string ToString()
    {
      return $"{GetType().Name} [Id={Id}]";
    }
  }
}
