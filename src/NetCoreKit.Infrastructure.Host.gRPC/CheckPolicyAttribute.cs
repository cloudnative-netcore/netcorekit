using System;

namespace NetCoreKit.Infrastructure.Host.gRPC
{
    public class CheckPolicyAttribute : Attribute
    {
        public CheckPolicyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
