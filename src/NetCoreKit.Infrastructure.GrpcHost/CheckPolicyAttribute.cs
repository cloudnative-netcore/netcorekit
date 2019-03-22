using System;

namespace NetCoreKit.Infrastructure.GrpcHost
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
