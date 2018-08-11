using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Utils.Extensions;

namespace NetCoreKit.Domain
{
  public abstract class ValueObjectBase
  {
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(this, obj)) return true;
      if (ReferenceEquals(null, obj)) return false;
      if (GetType() != obj.GetType()) return false;
      var vo = obj as ValueObjectBase;
      return GetEqualityComponents().SequenceEqual(vo.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
      return GetEqualityComponents().CombineHashCodes();
    }
  }
}
