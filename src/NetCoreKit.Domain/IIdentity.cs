using System;

namespace NetCoreKit.Domain
{
    /// <summary>
    ///     Supertype for all Identity types
    /// </summary>
    public interface IIdentity
    {
        Guid Id { get; }
    }
}
